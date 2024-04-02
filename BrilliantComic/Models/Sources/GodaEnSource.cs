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
    public partial class GodaEnSource : Source
    {
        public GodaEnSource()
        {
            SetHttpClient("https://godamanga.art/");
            Name = "Goda(英)";
        }

        /// <summary>
        /// 搜索匹配关键词的漫画
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns></returns>
        public override async Task<IEnumerable<Comic>> SearchAsync(string keyword)
        {
            var url = $"https://godamanga.art/?s={keyword}&post_type=wp-manga";
            var html = await GetHtmlAsync(url);
            if (html == string.Empty) { return Array.Empty<Comic>(); }

            string pattern = "image-container[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?src=\"(.*?)\"[\\s\\S]*?alt=\"(.*?)\"";
            var matches = Regex.Matches(html, pattern);

            var comics = new List<Comic>();
            foreach (Match match in matches)
            {
                var comic = new GodaEnComic()
                {
                    Url = match.Groups[1].Value.Replace(" ", ""),
                    Name = match.Groups[3].Value,
                    Cover = match.Groups[2].Value.Replace("%3A", ":").Replace("%2F", "/"),
                    Source = this,
                    SourceName = Name,
                };
                comics.Add(comic);
            }

            return comics;
        }
    }
}