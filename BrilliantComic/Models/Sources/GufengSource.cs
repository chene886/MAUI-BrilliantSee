using BrilliantComic.Models.Comics;
using BrilliantComic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Sources
{
    public partial class GufengSource : Source
    {
        public GufengSource()
        {
            SetHttpClient("https://m.gufengmh9.com/");
            Name = "古风漫画";
            HasMore = 1;
        }

        /// <summary>
        /// 搜索匹配关键词的漫画
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns></returns>
        public override async Task<IEnumerable<Comic>> SearchAsync(string keyword)
        {
            var url = $"https://m.gufengmh9.com/search/?keywords={keyword}&page={ResultNum}";
            var html = await GetHtmlAsync(url);
            if (html == string.Empty) { return Array.Empty<Comic>(); }

            string pattern = "itemBox\"[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?alt=\"(.*?)\"[\\s\\S]*?src=\"(.*?)\"[\\s\\S]*?me\">(.*?)<[\\s\\S]*?date\">(.*?)<";
            var matches = Regex.Matches(html, pattern);
            if (matches.Count < 36) { HasMore = 0; }

            var comics = new List<Comic>();
            foreach (Match match in matches)
            {
                var comic = new GufengComic()
                {
                    Url = match.Groups[1].Value,
                    Name = match.Groups[2].Value,
                    Cover = match.Groups[3].Value,
                    Author = match.Groups[4].Value,
                    LastestUpdateTime = "(更新时间：" + match.Groups[5].Value + ")",
                    Source = this,
                    SourceName = Name,
                };
                comics.Add(comic);
            }
            return comics;
        }
    }
}