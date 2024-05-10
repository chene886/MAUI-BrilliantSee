using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Objs.Comics;
using BrilliantSee.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantSee.Models.Sources.ComicSources
{
    public partial class GodaEnSource : Source
    {
        public GodaEnSource()
        {
            SetHttpClient("https://manhuascans.org/");
            Name = "Goda(英)";
            HasMore = 1;
        }

        /// <summary>
        /// 搜索匹配关键词的漫画
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns></returns>
        public override async Task<IEnumerable<Obj>> SearchAsync(string keyword)
        {
            var url = $"https://manhuascans.org/s/{keyword}?page={ResultNum}";
            var html = await GetHtmlAsync(url);
            if (html == string.Empty) { return Array.Empty<Obj>(); }

            string pattern = "pb-2[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?alt=\"(.*?)\"[\\s\\S]*?src=\"(.*?)\"";
            var matches = Regex.Matches(html, pattern);
            if (matches.Count < 30) { HasMore = 0; }

            var comics = new List<Obj>();
            foreach (Match match in matches)
            {
                var comic = new GodaEnComic()
                {
                    Url = match.Groups[1].Value.Replace(" ", ""),
                    Name = match.Groups[2].Value,
                    Cover = match.Groups[3].Value.Replace("%3A", ":").Replace("%2F", "/"),
                    Source = this,
                    SourceName = Name,
                    SourceCategory = Category,
                };
                comics.Add(comic);
            }

            return comics;
        }
    }
}