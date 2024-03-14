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
        public required ISource Source { get; set; }

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
        /// 获取网站html
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> GetHtmlAsync();

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
        public abstract Chapter? GetNearChapter(Chapter chapter, string flag);
    }
}