using BrilliantComic.Models.Chapters;
using BrilliantComic.Models.Sources;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public override async Task GetHtmlAsync()
        {
            html = await Source.HttpClient.GetStringAsync(Url);
        }

        /// <summary>
        /// 获取更多漫画信息
        /// </summary>
        /// <returns></returns>
        ///
        public override void LoadMoreData()
        {
            //截取两个字符串之间的内容
            var start = html.IndexOf("<body");
            var end = html.IndexOf("猜你喜欢");
            var moreDataHtml = html.Substring(start, end - start);
            if (!string.IsNullOrEmpty(html))
            {
                Status = Regex.Match(moreDataHtml, "tag-list[\\s\\S]*?<span[\\s\\S]*?>(.*?)</span>").Groups[1].Value;
                var lastestUpdateTime = Regex.Match(moreDataHtml, "<em[\\s\\S]*?>[\\s\\r\\n]*([\\s\\S]*?)[\\s\\r\\n]*</em>").Groups[1].Value;
                LastestUpdateTime = lastestUpdateTime == "" ? "(暂无更新时间)" : lastestUpdateTime;
                Description = "        " + Regex.Match(moreDataHtml, "comics-detail__desc overflow-hidden[\\s\\S]*?>[\\s\\r\\n]*([\\s\\S]*?)</p>").Groups[1].Value;
            }
        }

        /// <summary>
        /// 获取漫画章节信息
        /// </summary>
        /// <returns></returns>
        public override async Task LoadChaptersAsync()
        {
            var index = "章节目录";
            var flag = true;
            var chapters = new List<BaoziChapter>();
            if (html.IndexOf(index) < 0)
            {
                index = "class=\"section-title\"";
                flag = !flag;
                if (html.IndexOf(index) < 0)
                {
                    Chapters = Chapters.Append(new BaoziChapter(this, "暂无章节", "", -1, false));
                    return;
                }
            }

            var chaptershtml = html.Substring(html.IndexOf(index));
            var matches = Regex.Matches(chaptershtml, "comics-chapters[\\s\\S]*?<span.*?>([\\s\\S]*?)</span>").ToList();
            var start = matches.Count() - 1;
            if (flag)
            {
                matches.Reverse();
            }
            if (matches.FirstOrDefault() is not null)
            {
                LastestChapterName = matches.FirstOrDefault()!.Groups[1].Value;
            }
            foreach (Match match in matches)
            {
                var isSpecial = false;
                var url = "https://cn.czmanga.com/comic/chapter/" + Url.Split("/").Last() + "/0_" + start.ToString() + ".html";
                var name = match.Groups[1].Value;
                if (start == LastReadedChapterIndex) isSpecial = true;
                chapters.Add(new BaoziChapter(this, name, url, start, isSpecial));
                start--;
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

        public override string? GetLastestChapterName()
        {
            var index = "章节目录";
            var flag = true;
            if (html.IndexOf(index) < 0)
            {
                index = "class=\"section-title\"";
                flag = !flag;
                if (html.IndexOf(index) < 0)
                {
                    return null;
                }
            }
            var chaptershtml = html.Substring(html.IndexOf(index));
            var matches = Regex.Matches(chaptershtml, "comics-chapters[\\s\\S]*?<span.*?>([\\s\\S]*?)</span>").ToList();
            if (flag) matches.Reverse();
            if (matches.FirstOrDefault() is not null)
            {
                return matches.FirstOrDefault()!.Groups[1].Value;
            }
            else { return null; }
        }
    }
}