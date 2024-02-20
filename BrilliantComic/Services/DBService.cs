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
        private const string _dbFile = "BrilliantComic.db3";

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
        }

        /// <summary>
        /// 获取漫画集合
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<Comic>> GetComicsAsync(DBComicCategory category)
        {
            var dbComics = await _db.Table<DBComic>()
                    .Where(i => i.Category == category)
                    .ToListAsync();
            var ret = dbComics
                .Select(i => _sourceService.GetSource(i.Source)?.CreateComicFromDBComic(i))
                .Where(c => c is not null)
                .Select(c => c!).ToList();

            return ret;
        }

        public async Task<bool> IsComicExistAsync(Comic comic, DBComicCategory category)
        {
            return await _db.Table<DBComic>().Where(i => i.Url == comic.Url && i.Category == category).CountAsync() > 0;
        }

        /// <summary>
        /// 保存漫画
        /// </summary>
        /// <param name="comic"></param>
        /// <returns></returns>
        public async Task<int> SaveComicAsync(Comic comic, DBComicCategory category)
        {
            var dbComic = comic.Source.CreateDBComicFromComic(comic);
            dbComic.Category = category;
            if (_db.Table<DBComic>().Where(i => i.Url == comic.Url && i.Category == category).CountAsync().Result > 0)
            {
                await this.DeleteComicAsync(comic, category);
            }
            return await _db.InsertAsync(dbComic);
        }

        public async Task<int> UpdateComicAsync(Comic comic)
        {
            var dbComic = comic.Source.CreateDBComicFromComic(comic);
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
        /// <param name="comic"></param>
        /// <returns></returns>
        public async Task<int> DeleteComicAsync(Comic comic, DBComicCategory category)
        {
            return await _db.Table<DBComic>().DeleteAsync(i => i.Url == comic.Url && i.Category == category);
        }
    }
}