using BrilliantComic.Models;
using BrilliantComic.Models.Comics;
using BrilliantComic.Models.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Services
{
    public class DBService
    {
        /// <summary>
        /// 数据库文件名
        /// </summary>
        private const string _dbFile = "BrilliantComic.db3";

        /// <summary>
        /// 数据库打开标志
        /// </summary>
        private const SQLite.SQLiteOpenFlags _flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        private readonly SourceService _sourceService;
        private SQLiteAsyncConnection _db;

        public DBService(SourceService sourceService)
        {
            _sourceService = sourceService;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, _dbFile);
            _db = new SQLiteAsyncConnection(dbPath, _flags);

            _ = InitAsync();
        }

        /// <summary>
        /// 初始化数据库，创建表
        /// </summary>
        /// <returns></returns>
        private async Task InitAsync()
        {
            await _db.CreateTableAsync<DBComic>();
            await _db.CreateTableAsync<SettingItem>();
            var count = await _db.Table<SettingItem>().CountAsync();
            if (count > 0) { return; }
            var defaultSettingItems = new List<SettingItem>
            {
                new SettingItem { Name = "包子漫画", Value = "IsSelected", Category = "Source" },
                new SettingItem { Name = "古风漫画", Value = "IsSelected", Category = "Source" },
                new SettingItem { Name = "Goda漫画", Value = "IsSelected", Category = "Source" },
                //new SettingItem { Name = "mangahasu", Value = "IsSelected", Category = "Source" },
                new SettingItem { Name = "分享应用", Value = "去分享", Category = "通用" },
                new SettingItem { Name = "错误反馈", Value = "去反馈", Category = "通用" },
                new SettingItem { Name = "支持开源", Value = "去支持", Category = "通用" },
                new SettingItem { Name = "免责声明", Value = "点击查看", Category = "关于" },
                new SettingItem { Name = "隐私政策", Value = "点击查看", Category = "关于" },
                new SettingItem { Name = "用户协议", Value = "点击查看", Category = "关于" },
                new SettingItem { Name = "ModelId", Value = "", Category = "AIModel" },
                new SettingItem { Name = "ApiKey", Value ="", Category = "AIModel" },
                new SettingItem { Name = "ApiUrl", Value = "", Category = "AIModel" },
                new SettingItem { Name = "AudioStatus", Value = "false", Category = "Audio" },
            };
            await _db.InsertAllAsync(defaultSettingItems);
        }

        /// <summary>
        /// 获取漫画集合
        /// </summary>
        /// <param name="category">需要获取的漫画类型</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<Comic>> GetComicsAsync(DBComicCategory category)
        {
            var dbComics = await _db.Table<DBComic>()
                .Where(i => i.Category == category)
                .ToListAsync();
            var ret = dbComics
                .Select(i => _sourceService.GetComic(i.SourceName)!.CreateComicFromDBComic(i))
                .Where(c => c is not null)
                .Select(c => c!).ToList();

            return ret;
        }

        /// <summary>
        /// 检查漫画是否存在
        /// </summary>
        /// <param name="comic">需要检查的漫画</param>
        /// <param name="category">漫画的类型</param>
        /// <returns></returns>
        public async Task<bool> IsComicExistAsync(Comic comic, DBComicCategory category)
        {
            return await _db.Table<DBComic>().Where(i => i.Url == comic.Url && i.Category == category).CountAsync() > 0;
        }

        /// <summary>
        /// 保存漫画
        /// </summary>
        /// <param name="comic">需要保存的漫画</param>
        /// <param name="category">漫画的类型</param>
        /// <returns></returns>
        public async Task<int> SaveComicAsync(Comic comic, DBComicCategory category)
        {
            var dbComic = comic.CreateDBComicFromComic(comic);
            dbComic.Category = category;
            if (_db.Table<DBComic>().Where(i => i.Url == comic.Url && i.Category == category).CountAsync().Result > 0)
            {
                await this.DeleteComicAsync(comic, category);
            }
            return await _db.InsertAsync(dbComic);
        }

        /// <summary>
        /// 更新漫画信息
        /// </summary>
        /// <param name="comic">需要更新的漫画</param>
        /// <returns></returns>
        public async Task<int> UpdateComicAsync(Comic comic)
        {
            var dbComic = comic.CreateDBComicFromComic(comic);
            var existingComic = await _db.Table<DBComic>()
                                    .Where(c => c.Url == comic.Url && c.Category == comic.Category)
                                    .FirstOrDefaultAsync();
            // 如果存在具有相同Url和LastReadedChapterIndex的记录，更新那个记录
            dbComic.Id = existingComic.Id; // 确保我们更新的是正确的记录
            return await _db.UpdateAsync(dbComic);
        }

        /// <summary>
        /// 删除漫画
        /// </summary>
        /// <param name="comic">需要删除的漫画</param>
        /// <param name="category">漫画的类型</param>
        /// <returns></returns>
        public async Task<int> DeleteComicAsync(Comic comic, DBComicCategory category)
        {
            return await _db.Table<DBComic>().DeleteAsync(i => i.Url == comic.Url && i.Category == category);
        }

        
        /// <summary>
        /// 获取对应类别的设置项
        /// </summary>
        /// <param name="category">类别</param>
        /// <returns></returns>
        public async Task<List<SettingItem>> GetSettingItemsAsync(string category)
        {
            return await _db.Table<SettingItem>().Where(i => i.Category == category).ToListAsync();
        }

        /// <summary>
        /// 更新设置项
        /// </summary>
        /// <param name="setting">设置项</param>
        /// <returns></returns>
        public async Task<int> UpdateSettingItemAsync(SettingItem setting)
        {
            return await _db.UpdateAsync(setting);
        }

        /// <summary>
        /// 获取语音功能状态
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetAudioStatus()
        {
            var Item = await GetSettingItemsAsync("Audio");
            return Item[0].Value == "true";
        }
    }
}