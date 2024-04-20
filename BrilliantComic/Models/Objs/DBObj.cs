using BrilliantSee.Models.Chapters;
using BrilliantSee.Models.Enums;
using SQLite;

namespace BrilliantSee.Models.Objs
{
    public class DBObj
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
        /// 漫画链接
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 最后阅读章节索引
        /// </summary>
        public int LastReadedChapterIndex { get; set; } = -1;

        /// <summary>
        /// 是否有更新
        /// </summary>
        public bool IsUpdate { get; set; } = false;

        /// <summary>
        /// 最新章节名
        /// </summary>
        public string LastestChapterName { get; set; } = string.Empty;

        /// <summary>
        /// 漫画源
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// 漫画源名
        /// </summary>
        public string SourceName { get; set; } = string.Empty;

        /// <summary>
        /// 分类
        /// </summary>
        public DBObjCategory Category { get; set; } = DBObjCategory.Default;

        /// <summary>
        /// 来源分类
        /// </summary>
        public SourceCategory SourceCategory { get; set; }
    }
}