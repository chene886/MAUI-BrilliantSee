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
    public partial class HasuSource : ObservableObject, ISource
    {
        public HttpClient HttpClient { get; set; } = new HttpClient(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip
        });

        public string Name { get; set; } = "Hasu(英)";

        [ObservableProperty]
        public bool _isSelected = true;

        private readonly SourceService _sourceService;

        public HasuSource(SourceService sourceService)
        {
            _sourceService = sourceService;
        }

        public async Task<IEnumerable<Comic>> SearchAsync(string keyword)
        {
            if (!HttpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 16_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.6 Mobile/15E148 Safari/604.1 Edg/122.0.0.0");
            }
            HttpClient.DefaultRequestHeaders.Remove("Referer");
            HttpClient.DefaultRequestHeaders.Add("Referer", "https://mangahasu.se/");

            var url = $"https://mangahasu.se/advanced-search.html?keyword={keyword}";
            try
            {
                var response = await HttpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return Array.Empty<Comic>();
                }
                var html = await response.Content.ReadAsStringAsync();
                var start = html.IndexOf("imgage");
                var end = html.IndexOf("setTimeout");
                html = html.Substring(start, end - start);
                string pattern = "lazy[\\s\\S]*?src=\"(.*?)\"[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?h3>(.*?)<";
                if (html.IndexOf("date_") == -1)
                {
                    return Array.Empty<Comic>();
                }
                var matches = Regex.Matches(html, pattern);
                var comics = new List<Comic>();
                foreach (Match match in matches)
                {
                    var comic = new HasuComic(match.Groups[2].Value, match.Groups[3].Value.Replace("&quot;", ""), match.Groups[1].Value, "(暂无作者信息)") { Source = this, SourceName = "Hasu(英)", LastestUpdateTime = "(暂无最后更新信息)" };
                    comics.Add(comic);
                    if(comics.Count == matches.Count-10)
                    {
                        break;
                    }
                }

                return comics;
            }
            catch
            {
                return Array.Empty<Comic>();
            }
        }

        /// <summary>
        /// 从存储的漫画数据创建漫画实体
        /// </summary>
        /// <param name="dbComic"></param>
        /// <returns></returns>
        public Comic CreateComicFromDBComic(DBComic dbComic)
        {
            Comic comic = new HasuComic(dbComic.Url, dbComic.Name, dbComic.Cover, dbComic.Author)
            {
                Id = dbComic.Id,
                Category = dbComic.Category,
                LastReadedChapterIndex = dbComic.LastReadedChapterIndex,
                IsUpdate = dbComic.IsUpdate,
                LastestChapterName = dbComic.LastestChapterName,
                SourceName = dbComic.SourceName,
                Source = this
            };

            return comic;
        }

        /// <summary>
        /// 从漫画实体创建存储的漫画数据
        /// </summary>
        /// <param name="comic"></param>
        /// <returns></returns>
        public DBComic CreateDBComicFromComic(Comic comic)
        {
            return new DBComic
            {
                Id = comic.Id,
                Name = comic.Name,
                Author = comic.Author,
                Cover = comic.Cover,
                Source = _sourceService.GetSourceName(this)!,
                Url = comic.Url,
                Category = comic.Category,
                LastReadedChapterIndex = comic.LastReadedChapterIndex,
                IsUpdate = comic.IsUpdate,
                LastestChapterName = comic.LastestChapterName,
                SourceName = comic.SourceName
            };
        }
    }
}