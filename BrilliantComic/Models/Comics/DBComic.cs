using BrilliantComic.Models.Chapters;
using BrilliantComic.Models.Enums;
using SQLite;

namespace BrilliantComic.Models.Comics
{
    public class DBComic
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
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// 漫画章节
        /// </summary>
        public string Chapters { get; set; } = string.Empty;

        /// <summary>
        /// 漫画章节是否倒序
        /// </summary>
        public bool IsReverseList { get; set; }

        /// <summary>
        /// 章节分类
        /// </summary>
        public DBComicCategory Category { get; set; } = DBComicCategory.Default;
    }
}