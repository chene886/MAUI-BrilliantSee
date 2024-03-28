using BrilliantComic.Models.Chapters;
using BrilliantComic.Models.Enums;
using BrilliantComic.Models.Sources;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Comics
{
    public abstract partial class Comic : ObservableObject
    {
        /// <summary>
        /// 储存数据库的主键
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// 漫画html
        /// </summary>
        public string Html { get; set; } = string.Empty;

        /// <summary>
        /// 封面链接
        /// </summary>
        public string Cover { get; set; } = string.Empty;

        /// <summary>
        /// 漫画名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 漫画作者
        /// </summary>
        [ObservableProperty]
        public string _author = string.Empty;

        /// <summary>
        /// 漫画简介
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 漫画链接
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 最后阅读章节索引
        /// </summary>
        public int LastReadedChapterIndex { get; set; } = -1;

        /// <summary>
        /// 漫画源
        /// </summary>
        public required Source Source { get; set; }

        /// <summary>
        /// 漫画源名
        /// </summary>
        public string SourceName { get; set; } = string.Empty;

        /// <summary>
        /// 最新章节名
        /// </summary>
        public string LastestChapterName { get; set; } = string.Empty;

        /// <summary>
        /// 最新更新时间
        /// </summary>
        [ObservableProperty]
        public string _lastestUpdateTime = string.Empty;

        /// <summary>
        /// 是否有更新
        /// </summary>
        public bool IsUpdate { get; set; } = false;

        /// <summary>
        /// 漫画状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 漫画章节
        /// </summary>
        [ObservableProperty]
        public IEnumerable<Chapter> _chapters = new List<Chapter>();

        /// <summary>
        /// 章节数量
        /// </summary>
        public int ChapterCount { get; set; } = 0;

        /// <summary>
        /// 漫画章节是否倒序
        /// </summary>
        public bool IsReverseList { get; set; } = true;

        /// <summary>
        /// 漫画分类
        /// </summary>
        public DBComicCategory Category { get; set; } = DBComicCategory.Default;

        /// <summary>
        /// 从存储的漫画数据创建漫画实体
        /// </summary>
        /// <param name="dbComic"></param>
        /// <returns></returns>
        public Comic CreateComicFromDBComic(DBComic dbComic)
        {
            Url = dbComic.Url;
            Name = dbComic.Name;
            Cover = dbComic.Cover;
            Author = dbComic.Author;
            Id = dbComic.Id;
            Category = dbComic.Category;
            LastReadedChapterIndex = dbComic.LastReadedChapterIndex;
            IsUpdate = dbComic.IsUpdate;
            LastestChapterName = dbComic.LastestChapterName;
            SourceName = dbComic.SourceName;
            return (Comic)this.MemberwiseClone();
        }

        /// <summary>
        /// 从漫画实体创建存储的漫画数据
        /// </summary>
        /// <param name="comic"></param>
        /// <returns></returns>
        public DBComic CreateDBComicFromComic(Comic comic)
        {
            return new DBComic
            {
                Id = comic.Id,
                Name = comic.Name,
                Author = comic.Author,
                Cover = comic.Cover,
                Source = this.SourceName,
                Url = comic.Url,
                Category = comic.Category,
                LastReadedChapterIndex = comic.LastReadedChapterIndex,
                IsUpdate = comic.IsUpdate,
                LastestChapterName = comic.LastestChapterName,
                SourceName = comic.SourceName
            };
        }

        /// <summary>
        /// 获取网站html
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetHtmlAsync()
        {
            try
            {
                Html = await Source.HttpClient!.GetStringAsync(Url);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取更多漫画数据
        /// </summary>
        /// <returns></returns>
        public abstract void LoadMoreData();

        /// <summary>
        /// 获取最新章节名
        /// </summary>
        /// <returns></returns>
        public abstract string? GetLastestChapterName();

        /// <summary>
        /// 加载章节信息
        /// </summary>
        /// <returns></returns>
        public abstract Task LoadChaptersAsync();

        /// <summary>
        /// 从当前章节获取上一章节或下一章节
        /// </summary>
        /// <param name="chapter">当前章节</param>
        /// <param name="flag">获取上一章节或下一章节的标志</param>
        /// <returns></returns>
        public Chapter? GetNearChapter(Chapter chapter, string flag)
        {
            var tempChapters = Chapters.ToList();
            int index = tempChapters.IndexOf(chapter);
            bool turn = flag == "Last";
            index = IsReverseList == turn ? index + 1 : index - 1;
            if (index < 0 || index >= Chapters.Count()) return null;
            return Chapters.ElementAtOrDefault(index)!;
        }
    }
}