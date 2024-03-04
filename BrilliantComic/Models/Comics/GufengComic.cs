using BrilliantComic.Models.Chapters;
using CommunityToolkit.Maui.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Comics
{
    public class GufengComic : Comic
    {
        private string html = string.Empty;

        public GufengComic(string url, string name, string cover, string author)
        {
            Url = url;
            Cover = cover;
            Name = name;
            Author = author;
        }

        /// <summary>
        /// 获取漫画网页源代码
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> GetHtmlAsync()
        {
            try
            {
                html = await Source.HttpClient.GetStringAsync(Url);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public override void LoadMoreData()
        {

                var start = html.IndexOf("Cover");
                var end = html.IndexOf("章节列表");
                var moreDataHtml = html.Substring(start, end - start);
                if (!string.IsNullOrEmpty(html))
                {
                    var result = Regex.Match(moreDataHtml, "</dd[\\s\\S]*?</dd[\\s\\S]*?</dd[\\s\\S]*?<dd[\\s\\S]*?>(.*?)<[\\s\\S]*?简介：(.*?)<");
                    Status = "连载中";
                    Description = result.Groups[2].Value.Replace("\\n", "");
                    LastestUpdateTime = "(更新时间：" + result.Groups[1].Value + ")";
                }
            
        }

        public override string? GetLastestChapterName()
        {
            var start = html.IndexOf("Cover");
            var end = html.IndexOf("作者");
            var moreDataHtml = html.Substring(start, end - start);
            if (!string.IsNullOrEmpty(html))
            {
                var result = Regex.Match(moreDataHtml, "dd[\\s\\S]*?>(.*?)<");
                return result.Groups[1].Value;
            }
            return "";
        }

        public override Chapter? GetNearChapter(Chapter chapter, string flag)
        {
            int index = -1;
            int change = 1;
            if (IsReverseList) change = -1;
            var tempChapters = Chapters.ToList();
            if (flag == "Last")
            {
                index = tempChapters.IndexOf(chapter) - change;
            }
            else if (flag == "Next")
            {
                index = tempChapters.IndexOf(chapter) + change;
            }
            if (index < 0 || index >= Chapters.Count()) return null;
            return Chapters.ElementAtOrDefault(index)!;
        }

        public override async Task LoadChaptersAsync()
        {
            var index = "章节列表";
            var flag = true;
            var chapters = new List<GufengChapter>();

            if (html.IndexOf(index) < 0)
            {
                Chapters = Chapters.Append(new GufengChapter("暂无章节", "", -1, false) { Comic = this });
                return;
            }
            var chaptershtml = html.Substring(html.IndexOf(index));
            var matches = Regex.Matches(chaptershtml, "<li>[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?<span>(.*?)<").ToList();
            if (flag) matches.Reverse();
            var start = matches.Count() - 1;
            if (matches.FirstOrDefault() is not null)
            {
                LastestChapterName = matches.FirstOrDefault()!.Groups[2].Value;
            }
            foreach (Match match in matches)
            {
                var isSpecial = false;
                var url = "https://m.gufengmh9.com/" + match.Groups[1].Value;
                var name = match.Groups[2].Value;
                if (start == LastReadedChapterIndex) isSpecial = true;
                chapters.Add(new GufengChapter(name, url, start, isSpecial) { Comic = this });
                start--;
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Chapters = chapters;
            });
        }
    }
}