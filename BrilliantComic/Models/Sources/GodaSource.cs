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
    public partial class GodaSource : Source
    {
        private readonly SourceService _sourceService;

        public GodaSource(SourceService sourceService)
        {
            SetHttpClient("https://godamanga.com/");
            Name = "Goda漫画";
            _sourceService = sourceService;
        }

        /// <summary>
        /// 搜索匹配关键词的漫画
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns></returns>
        public override async Task<IEnumerable<Comic>> SearchAsync(string keyword)
        {
            var url = $"https://godamanga.com/s/{keyword}?pagw=1";
            try
            {
                var response = await HttpClient!.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return Array.Empty<Comic>();
                }
                var html = await response.Content.ReadAsStringAsync();
                string pattern = "pb-2\"[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?url=(.*?)&[\\s\\S]*?h3[\\s\\S]*?>(.*?)<";
                var matches = Regex.Matches(html, pattern);

                var comics = new List<Comic>();

                foreach (Match match in matches)
                {
                    var comic = new GodaComic("https://godamanga.com" + match.Groups[1].Value, match.Groups[3].Value, match.Groups[2].Value.Replace("%3A", ":").Replace("%2F", "/"), "暂无作者信息")
                    {
                        Source = this,
                        SourceName = Name,
                        LastestUpdateTime = "(暂无最后更新信息)"
                    };
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