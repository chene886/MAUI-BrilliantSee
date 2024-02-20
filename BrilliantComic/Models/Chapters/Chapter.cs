using BrilliantComic.Models.Comics;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Chapters
{
    public abstract partial class Chapter : ObservableObject
    {
        /// <summary>
        /// 章节名
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// 章节url
        /// </summary>
        public abstract string Url { get; set; }

        /// <summary>
        /// 章节所属的漫画
        /// </summary>
        public abstract Comic Comic { get; set; }

        /// <summary>
        /// 章节页数
        /// </summary>
        public abstract int PageCount { get; set; }

        /// <summary>
        /// 章节索引
        /// </summary>
        public abstract int Index { get; set; }

        /// <summary>
        /// 章节是否为最后阅读章节
        /// </summary>
        [ObservableProperty]
        public bool _isSpecial = false;

        /// <summary>
        /// 获取章节图片枚举器
        /// </summary>
        /// <returns></returns>
        public abstract Task<IEnumerable<string>> GetPicEnumeratorAsync();
    }
}