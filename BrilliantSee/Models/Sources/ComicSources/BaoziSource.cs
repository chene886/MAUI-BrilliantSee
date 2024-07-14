using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Objs.Comics;
using BrilliantSee.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui;
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

            var comics = new List<Obj>();

            try
            {
                string pattern = "comics-card.*?href=\\\"(.*?)\\\".*?title=\\\"(.*?)\\\"[\\s\\S]*?src=\"(.*?)\"[\\s\\S]*?small.*?>[\\s\\r\\n]*([\\s\\S]*?)</small>";
                var matches = Regex.Matches(html, pattern);
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
            catch
            {
                return Array.Empty<Obj>();
            }
        }

        public async Task<IEnumerable<string>> GetHotWordAsync()
        {
            var url = "https://cn.baozimh.com/";
            var html = await GetHtmlAsync(url);
            if (html == string.Empty) { return Array.Empty<string>(); }

            var hotWords = new List<string>();
            try
            {
                html = html.Substring(html.IndexOf("最近更新"));
                string pattern = "<a[\\s\\S]*?label=\"(.*?)\"[\\s\\S]*?<sma";
                var matches = Regex.Matches(html, pattern);
                foreach (Match match in matches)
                {
                    hotWords.Add(match.Groups[1].Value.Replace(" ", ""));
                    if (hotWords.Count == 10) { break; }
                }
                return hotWords;
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        public async Task<IEnumerable<Obj>> GetRecommandAsync()
        {
            var url = "https://cn.baozimh.com/";
            var html = await GetHtmlAsync(url);
            if (html == string.Empty) { return Array.Empty<Obj>(); }

            var comics = new List<Obj>();
            html = html.Substring(html.IndexOf("body"), html.IndexOf("最近更新") - html.IndexOf("body"));
            try
            {
                string pattern = "comics-card.*?href=\\\"(.*?)\\\".*?title=\\\"(.*?)\\\"[\\s\\S]*?src=\"(.*?)\"[\\s\\S]*?small.*?>[\\s\\S]*?</small>";
                var matches = Regex.Matches(html, pattern);
                foreach (Match match in matches)
                {
                    var comic = new BaoziComic()
                    {
                        Url = "https://cn.baozimh.com" + match.Groups[1].Value,
                        Name = match.Groups[2].Value,
                        Cover = match.Groups[3].Value,
                        Author = "暂无作者",
                        Source = this,
                        SourceName = Name,
                        SourceCategory = Category,
                    };
                    comics.Add(comic);
                }
                return comics;
            }
            catch
            {
                return Array.Empty<Obj>();
            }
        }
    }
}