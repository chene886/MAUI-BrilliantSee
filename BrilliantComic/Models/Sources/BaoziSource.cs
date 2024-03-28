using BrilliantComic.Models.Comics;
using BrilliantComic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Sources
{
    public partial class BaoziSource : Source
    {
        private readonly SourceService _sourceService;

        public BaoziSource(SourceService sourceService)
        {
            SetHttpClient("https://cn.baozimh.com/");
            Name = "包子漫画";
            _sourceService = sourceService;
        }

        /// <summary>
        /// 搜索匹配关键词的漫画
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns></returns>
        public override async Task<IEnumerable<Comic>> SearchAsync(string keyword)
        {
            var url = $"https://cn.baozimh.com/search?q={keyword}";
            try
            {
                var response = await HttpClient!.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return Array.Empty<Comic>();
                }
                var html = await response.Content.ReadAsStringAsync();
                string pattern = "comics-card.*?href=\\\"(.*?)\\\".*?title=\\\"(.*?)\\\"[\\s\\S]*?src=\"(.*?)\"[\\s\\S]*?small.*?>[\\s\\r\\n]*([\\s\\S]*?)</small>";
                var matches = Regex.Matches(html, pattern);

                var comics = new List<Comic>();

                foreach (Match match in matches)
                {
                    var comic = new BaoziComic("https://cn.baozimh.com" + match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value + " ") { Source = this, SourceName = Name, LastestUpdateTime = "(暂无最后更新信息)" };
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