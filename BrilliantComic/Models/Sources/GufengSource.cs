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
        private readonly SourceService _sourceService;

        public GufengSource(SourceService sourceService)
        {
            SetHttpClient("https://m.gufengmh9.com/");
            Name = "古风漫画";
            _sourceService = sourceService;
        }

        /// <summary>
        /// 搜索匹配关键词的漫画
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns></returns>
        public override async Task<IEnumerable<Comic>> SearchAsync(string keyword)
        {
            var url = $"https://m.gufengmh9.com/search/?keywords={keyword}&page=1";
            try
            {
                var response = await HttpClient!.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return Array.Empty<Comic>();
                }
                var html = await response.Content.ReadAsStringAsync();
                string pattern = "itemBox\"[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?alt=\"(.*?)\"[\\s\\S]*?src=\"(.*?)\"[\\s\\S]*?me\">(.*?)<[\\s\\S]*?date\">(.*?)<";
                var matches = Regex.Matches(html, pattern);

                var comics = new List<Comic>();

                foreach (Match match in matches)
                {
                    var comic = new GufengComic(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value) { Source = this, SourceName = Name, LastestUpdateTime = "(更新时间：" + match.Groups[5].Value + ")" };

                    comics.Add(comic);
                }

                return comics;
            }
            catch
            {
                return Array.Empty<Comic>();
            }
        }
    }
}