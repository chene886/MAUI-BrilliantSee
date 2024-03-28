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
    public partial class HasuSource : Source
    {
        private readonly SourceService _sourceService;

        public HasuSource(SourceService sourceService)
        {
            SetHttpClient("https://mangahasu.se/");
            Name = "HasuManga";
            _sourceService = sourceService;
        }

        public override async Task<IEnumerable<Comic>> SearchAsync(string keyword)
        {
            var url = $"https://mangahasu.se/advanced-search.html?keyword={keyword}";
            try
            {
                var response = await HttpClient!.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return Array.Empty<Comic>();
                }
                var html = await response.Content.ReadAsStringAsync();
                var start = html.IndexOf("imgage");
                var end = html.IndexOf("setTimeout");
                if (start < 0 || end < 0)
                {
                    throw new Exception("接口异常,请等待维护");
                }
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
                    var comic = new HasuComic(match.Groups[2].Value, match.Groups[3].Value.Replace("&quot;", ""), match.Groups[1].Value, "(暂无作者信息)")
                    { Source = this, SourceName = Name, LastestUpdateTime = "(暂无最后更新信息)" };
                    comics.Add(comic);
                    if (comics.Count == matches.Count - 10)
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
    }
}