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
        public override string Name { get; set; } = string.Empty;
        public override string Url { get; set; } = string.Empty;
        public override Comic Comic { get; set; }
        public override int PageCount { get; set; } = 0;

        public override int Index { get; set; } = -1;

        public BaoziChapter(Comic comic, string name, string url, int index, bool isSpecial)
        {
            Comic = comic;
            Name = name;
            Url = url;
            Index = index;
            IsSpecial = isSpecial;
        }

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