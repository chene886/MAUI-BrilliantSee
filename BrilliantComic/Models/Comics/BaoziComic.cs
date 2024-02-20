using BrilliantComic.Models.Chapters;
using BrilliantComic.Models.Sources;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Comics
{
    public class BaoziComic : Comic
    {
        private string html = string.Empty;

        public BaoziComic(string url, string name, string cover, string author)
        {
            Url = url;
            Cover = cover;
            Name = name;
            Author = author;
        }

        /// <summary>
        /// 获取更多漫画信息
        /// </summary>
        /// <returns></returns>
        ///
        public override async Task LoadMoreDataAsync()
        {
            await LoadDescAsync();
        }

        /// <summary>
        /// 获取漫画简介
        /// </summary>
        /// <returns></returns>
        public async Task LoadDescAsync()
        {
            html = await Source.HttpClient.GetStringAsync(Url);
            if (!string.IsNullOrEmpty(html))
            {
                Description = "        " + Regex.Match(html, "comics-detail__desc overflow-hidden[\\s\\S]*?>[\\s\\r\\n]*([\\s\\S]*?)</p>").Groups[1].Value;
            }
        }

        /// <summary>
        /// 获取漫画章节信息
        /// </summary>
        /// <returns></returns>
        public override async Task LoadChaptersAsync(bool flag)
        {
            var index = "章节目录";
            var chapters = new List<BaoziChapter>();
            if (html.IndexOf(index) < 0)
            {
                index = "class=\"section-title\"";
                flag = !flag;
                if (html.IndexOf(index) < 0)
                {
                    chapters.Add(new BaoziChapter(this, "暂无章节", "", -1, false));
                    return;
                }
            }

            var chaptershtml = html.Substring(html.IndexOf(index));
            var matches = Regex.Matches(chaptershtml, "comics-chapters[\\s\\S]*?<span.*?>([\\s\\S]*?)</span>").ToList();
            var start = 0;
            if (flag)
            {
                matches.Reverse();
            }
            foreach (Match match in matches)
            {
                var isSpecial = false;
                var url = "https://cn.czmanga.com/comic/chapter/" + Url.Split("/").Last() + "/0_" + start.ToString() + ".html";
                var name = match.Groups[1].Value;
                if (start == LastReadedChapterIndex) isSpecial = true;
                chapters.Add(new BaoziChapter(this, name, url, start, isSpecial));
                start++;
            }
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Chapters = chapters;
            });
        }

        /// <summary>
        /// 获取相邻章节
        /// </summary>
        /// <param name="chapter">当前章节</param>
        /// <param name="flag">需要上一章或下一章</param>
        /// <returns></returns>
        public override Chapter GetNearChapter(Chapter chapter, string flag)
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
            if (index < 0 || index >= Chapters.Count()) return new BaoziChapter(this, "暂无章节", "", -1, false);
            return Chapters.ElementAtOrDefault(index)!;
        }
    }
}