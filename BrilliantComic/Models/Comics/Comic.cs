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
    public abstract partial class Comic:ObservableObject
    {
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
        public string Author { get; set; } = string.Empty;

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
        /// 漫画章节
        /// </summary>
        [ObservableProperty]
        public IEnumerable<Chapter> _chapters  = new List<Chapter>();

        /// <summary>
        /// 漫画章节是否倒序
        /// </summary>
        public bool IsReverseList { get; set; } = false;

        public DBComicCategory Category { get; set; } = DBComicCategory.Default;

        public abstract Task LoadMoreDataAsync();

        public abstract Task LoadChaptersAsync(bool flag);

        public abstract Chapter GetNearChapter(Chapter chapter, string flag);
    }
}