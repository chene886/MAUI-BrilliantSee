using BrilliantComic.Models.Chapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Comics
{
    public class GodaComic : Comic
    {
        private string html = string.Empty;

        public GodaComic(string url, string name, string cover, string author)
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

        /// <summary>
        /// 获取更多漫画信息
        /// </summary>
        public override void LoadMoreData()
        {
            //截取两个字符串之间的内容
            var start = html.IndexOf("bannersUite");
            var end = html.IndexOf("ed:block\">");
            if(end < 0) end = html.IndexOf("chapterlist");
            if (start < 0 || end < 0)
            {
                Status = "连载中";
                Description = "暂无简介";
                LastestUpdateTime = "暂无更新时间";
                Author = "暂无作者";
                return;
            }
            var moreDataHtml = html.Substring(start, end - start);
            if (!string.IsNullOrEmpty(html))
            {
                var result = Regex.Match(moreDataHtml, "<span class=\"text-xs[\\s\\S]*?>[\\s](.*?)[\\s]<[\\s\\S]*?<p[\\s\\S]*?>([\\s\\S]*?)<");
                Status = result.Groups[1].Value;
                Description = result.Groups[2].Value.Replace("\\n", "").Replace("amp;", "");
                var result1 = Regex.Match(moreDataHtml, "italic[\\s\\S]*?>(.*?)[\\s]<");
                LastestUpdateTime = "(更新时间：" + result1.Groups[1].Value + ")";
                var result2 = Regex.Matches(moreDataHtml, "author[\\s\\S]*?<span>[\\s](.*?)<").ToList();
                Author = "";
                foreach (Match item in result2)
                {
                    Author += item.Groups[1].Value;
                }
            }
        }

        /// <summary>
        /// 获取漫画章节信息
        /// </summary>
        /// <returns></returns>
        public override async Task LoadChaptersAsync()
        {
            var index = "最近章節";
            var flag = true;
            var chapters = new List<GodaChapter>();
            try
            {
                var newUrl = Url.Replace("/manga", "/chapterlist");
                var chaptershtml = await Source.HttpClient.GetStringAsync(newUrl);

                if (html.IndexOf(index) < 0)
                {
                    Chapters = Chapters.Append(new GodaChapter("暂无章节", "", -1, false) { Comic = this });
                    return;
                }
                chaptershtml = chaptershtml.Substring(chaptershtml.IndexOf(index));
                var matches = Regex.Matches(chaptershtml, "chapteritem[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?title[\\s\\S]*?>[\\s](.*?)[\\s]<").ToList();
                if (flag) matches.Reverse();
                var start = matches.Count() - 1;
                if (matches.FirstOrDefault() is not null)
                {
                    LastestChapterName = matches.FirstOrDefault()!.Groups[2].Value;
                    foreach (Match match in matches)
                    {
                        var isSpecial = false;
                        var url = match.Groups[1].Value;
                        var name = match.Groups[2].Value;
                        if (start == LastReadedChapterIndex) isSpecial = true;
                        chapters.Add(new GodaChapter(name, url, start, isSpecial) { Comic = this });
                        start--;
                    }
                }
                else
                {
                    LastestChapterName = "";
                    Chapters = Chapters.Append(new GodaChapter("暂无章节", "", -1, false) { Comic = this });
                    return;
                }
            }
            catch
            {
                Chapters = Chapters.Append(new GodaChapter("暂无章节", "", -1, false) { Comic = this });
                return;
            }
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Chapters = chapters;
                ChapterCount = Chapters.Count();
            });
        }

        /// <summary>
        /// 获取最新章节名
        /// </summary>
        /// <returns></returns>
        public override string? GetLastestChapterName()
        {
            var index = "最近章節";
            if (html.IndexOf(index) < 0)
            {
                return null;
            }
            var chaptershtml = html.Substring(html.IndexOf(index));
            var match = Regex.Match(chaptershtml, "chaptertitle[\\s\\S]*?>[\\s](.*?)[\\s]<");
            if (match is not null)
            {
                return match.Groups[1].Value;
            }
            else { return null; }
        }

        /// <summary>
        /// 获取相邻章节
        /// </summary>
        /// <param name="chapter">当前章节</param>
        /// <param name="flag">上一章或下一章</param>
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
    }
}