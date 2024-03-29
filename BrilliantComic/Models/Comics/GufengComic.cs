﻿using BrilliantComic.Models.Chapters;
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
        public override void LoadMoreData()
        {
            var start = Html.IndexOf("Cover");
            var end = Html.IndexOf("comic-chapters");
            if (start < 0 || end < 0)
            {
                Chapters = Chapters.Append(new GufengChapter("暂无章节", "", -1, false) { Comic = this });
                return;
            }
            var moreDataHtml = Html.Substring(start, end - start);
            if (!string.IsNullOrEmpty(Html))
            {
                var result = Regex.Match(moreDataHtml, "</dd[\\s\\S]*?</dd[\\s\\S]*?</dd[\\s\\S]*?<dd[\\s\\S]*?>(.*?)<[\\s\\S]*?简介：(.*?)<");
                Status = "连载中";
                Description = result.Groups[2].Value.Replace("\\n", "");
                LastestUpdateTime = "(更新时间：" + result.Groups[1].Value + ")";
            }
        }

        public override string? GetLastestChapterName()
        {
            var start = Html.IndexOf("Cover");
            var end = Html.IndexOf("开始阅读");
            if (start < 0 || end < 0)
            {
                return "";
            }
            var moreDataHtml = Html.Substring(start, end - start);
            if (!string.IsNullOrEmpty(Html))
            {
                var result = Regex.Match(moreDataHtml, "dd[\\s\\S]*?>(.*?)<");
                return result.Groups[1].Value;
            }
            return "";
        }

        public override async Task LoadChaptersAsync()
        {
            var index = "comic-chapters";
            var flag = true;
            var chapters = new List<GufengChapter>();

            if (Html.IndexOf(index) < 0)
            {
                Chapters = Chapters.Append(new GufengChapter("暂无章节", "", -1, false) { Comic = this });
                return;
            }
            var chaptershtml = Html.Substring(Html.IndexOf(index));
            var matches = Regex.Matches(chaptershtml, "<li>[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?<span>(.*?)<").ToList();
            if (flag) matches.Reverse();
            var start = matches.Count() - 1;
            if (matches.FirstOrDefault() is not null)
            {
                LastestChapterName = matches.FirstOrDefault()!.Groups[2].Value;
            }
            foreach (Match match in matches)
            {
                chapters.Add(new GufengChapter(
                    match.Groups[2].Value,
                    "https://m.gufengmh9.com/" + match.Groups[1].Value,
                    start,
                    start == LastReadedChapterIndex)
                { Comic = this });
                start--;
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Chapters = chapters;
                ChapterCount = Chapters.Count();
            });
        }
    }
}