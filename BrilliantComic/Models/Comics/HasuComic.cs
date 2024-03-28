﻿using BrilliantComic.Models.Chapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Comics
{
    public class HasuComic : Comic
    {

        public HasuComic(string url, string name, string cover, string author)
        {
            Url = url;
            Cover = cover;
            Name = name;
            Author = author;
        }

        public HasuComic()
        {
        }

        public override string? GetLastestChapterName()
        {
            var Istart = Html.IndexOf("list-chapter");
            var end = Html.IndexOf("div-comment");
            if (Istart < 0 || end < 0)
            {
                return "";
            }
            var chaptershtml = Html.Substring(Istart, end - Istart);
            var match = Regex.Match(chaptershtml, "name[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?/span>(.*?)<");
            if (match is not null)
            {
                return match.Groups[2].Value;
            }
            return "";
        }

        public override async Task LoadChaptersAsync()
        {
            var Istart = Html.IndexOf("list-chapter");
            var end = Html.IndexOf("div-comment");
            var chapters = new List<HasuChapter>();
            if (Istart < 0 || end < 0)
            {
                Chapters = Chapters.Append(new HasuChapter("暂无章节", "", -1, false) { Comic = this });
                return;
            }
            var chaptershtml = Html.Substring(Istart, end - Istart);
            var matches = Regex.Matches(chaptershtml, "name[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?/span>(.*?)<").ToList();
            var start = matches.Count() - 1;
            if (matches.FirstOrDefault() is not null)
            {
                LastestChapterName = matches.FirstOrDefault()!.Groups[2].Value;
            }
            foreach (Match match in matches)
            {
                var isSpecial = false;
                var url = match.Groups[1].Value;
                var name = match.Groups[2].Value;
                if (start == LastReadedChapterIndex) isSpecial = true;
                chapters.Add(new HasuChapter(name, url, start, isSpecial) { Comic = this });
                start--;
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Chapters = chapters;
            });
        }

        public override void LoadMoreData()
        {
            var start = Html.IndexOf("Author");
            var end = Html.IndexOf("list-chapter");
            if (start < 0 || end < 0)
            {
                Chapters = Chapters.Append(new HasuChapter("暂无章节", "", -1, false) { Comic = this });
                return;
            }
            var moreDataHtml = Html.Substring(start, end - start);
            if (!string.IsNullOrEmpty(Html))
            {
                var result = Regex.Match(moreDataHtml, "Author[\\s\\S]*?<a[\\s\\S]*?>(.*?)<[\\s\\S]*?Artist[\\s\\S]*?<a[\\s\\S]*?>(.*?)<[\\s\\S]*?Status[\\s\\S]*?<a[\\s\\S]*?>(.*?)<");
                Author = result.Groups[1].Value + "(作者)," + result.Groups[2].Value + "(画手)";
                Status = result.Groups[3].Value;
                Description = Regex.Match(moreDataHtml, "Summary[\\s\\S]*?<div>([\\s\\S]*?)</div>").Groups[1].Value.Replace("<p>", "").Replace("\\n", "");
                moreDataHtml = Html.Substring(Html.IndexOf("list-chapter"));
                LastestUpdateTime = "(" + Regex.Match(moreDataHtml, "td.*?date-updated\">(.*?)<").Groups[1].Value + ")";
            }
        }
    }
}