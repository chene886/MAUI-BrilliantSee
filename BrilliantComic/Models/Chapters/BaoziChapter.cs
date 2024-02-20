using BrilliantComic.Models.Comics;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Chapters
{
    public class BaoziChapter : Chapter
    {
        /// <summary>
        /// 章节名
        /// </summary>
        public override string Name { get; set; } = string.Empty;

        /// <summary>
        /// 章节url
        /// </summary>
        public override string Url { get; set; } = string.Empty;

        /// <summary>
        /// 章节所属的漫画
        /// </summary>
        public override Comic Comic { get; set; }

        /// <summary>
        /// 章节页数
        /// </summary>
        public override int PageCount { get; set; } = 0;

        /// <summary>
        /// 章节索引
        /// </summary>
        public override int Index { get; set; } = -1;

        public BaoziChapter(Comic comic, string name, string url, int index, bool isSpecial)
        {
            Comic = comic;
            Name = name;
            Url = url;
            Index = index;
            IsSpecial = isSpecial;
        }

        /// <summary>
        /// 获取章节图片枚举器
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task<IEnumerable<string>> GetPicEnumeratorAsync()
        {
            var msg = (await Comic.Source.HttpClient.GetAsync(Url));
            if (msg.RequestMessage is null || msg.RequestMessage.RequestUri is null)
                throw new Exception("包子漫画接口异常");
            var html = (await msg.Content.ReadAsStringAsync()).Replace("\n", string.Empty);
            var match = Regex.Matches(html, "<noscript [\\s\\S]*?src=\\\"([\\s\\S]*?)\\\"[\\s\\S]*?</noscript>");
            var list = new List<string>();
            foreach (Match item in match)
            {
                list.Add(item.Groups[1].Value);
            }
            PageCount = list.Count;
            return list;
        }
    }
}