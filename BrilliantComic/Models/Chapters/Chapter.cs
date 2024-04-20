using BrilliantSee.Models.Objs;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.Models.Chapters
{
    public abstract partial class Chapter : ObservableObject
    {
        /// <summary>
        /// 章节名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 章节url
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 章节所属的漫画
        /// </summary>
        public required Obj Comic { get; set; }

        /// <summary>
        /// 章节页数
        /// </summary>
        public int PageCount { get; set; } = 0;

        /// <summary>
        /// 章节索引
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// 章节是否为最后阅读章节
        /// </summary>
        [ObservableProperty]
        public bool _isSpecial = false;

        public List<string> PicUrls { get; set; } = new List<string>();

        public Chapter(string name, string url, int index, bool isSpecial)
        {
            Name = name;
            Url = url;
            Index = index;
            IsSpecial = isSpecial;
        }

        /// <summary>
        /// 获取章节图片枚举器
        /// </summary>
        /// <returns></returns>
        public abstract Task GetPicEnumeratorAsync();
    }
}