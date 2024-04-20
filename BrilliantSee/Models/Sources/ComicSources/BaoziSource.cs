using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Objs.Comics;
using BrilliantSee.Services;
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

namespace BrilliantSee.Models.Sources.ComicSources
{
    public partial class BaoziSource : Source
    {
        public BaoziSource()
        {
            SetHttpClient("https://cn.baozimh.com/");
            Name = "包子漫画";
        }

        /// <summary>
        /// 搜索匹配关键词的漫画
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns></returns>
        public override async Task<IEnumerable<Obj>> SearchAsync(string keyword)
        {
            var url = $"https://cn.baozimh.com/search?q={keyword}";
            var html = await GetHtmlAsync(url);
            if (html == string.Empty) { return Array.Empty<Obj>(); }

            string pattern = "comics-card.*?href=\\\"(.*?)\\\".*?title=\\\"(.*?)\\\"[\\s\\S]*?src=\"(.*?)\"[\\s\\S]*?small.*?>[\\s\\r\\n]*([\\s\\S]*?)</small>";
            var matches = Regex.Matches(html, pattern);

            var comics = new List<Obj>();
            foreach (Match match in matches)
            {
                var comic = new BaoziComic()
                {
                    Url = "https://cn.baozimh.com" + match.Groups[1].Value,
                    Name = match.Groups[2].Value,
                    Cover = match.Groups[3].Value,
                    Author = match.Groups[4].Value,
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