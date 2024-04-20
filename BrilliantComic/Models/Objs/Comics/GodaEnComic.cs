using BrilliantSee.Models.Chapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantSee.Models.Objs.Comics
{
    public class GodaEnComic : Obj
    {
        /// <summary>
        /// 获取更多漫画信息
        /// </summary>
        public override void LoadMoreData()
        {
            //截取两个字符串之间的内容
            var start = Html.IndexOf("Author");
            if (start < 0)
            {
                Status = "连载中";
                Description = "暂无简介";
                LastestUpdateTime = "暂无更新时间";
                Author = "暂无作者";
                return;
            }
            var moreDataHtml = Html.Substring(start);
            if (!string.IsNullOrEmpty(Html))
            {
                var result = Regex.Match(moreDataHtml, "Author[\\s\\S]*?>(.*?)<[\\s\\S]*?Status:\\s(.*?)<[\\s\\S]*?<p[\\s\\S]*?>(.*?)<");
                Author = result.Groups[1].Value;
                Status = result.Groups[2].Value;
                Description = result.Groups[3].Value.Replace("\\n", "").Replace("amp;", "");
                var result1 = Regex.Match(moreDataHtml, "<i>(.*?)<");
                LastestUpdateTime = result1.Groups[1].Value;
            }
        }

        /// <summary>
        /// 获取漫画章节信息
        /// </summary>
        /// <returns></returns>
        public override async Task LoadChaptersAsync()
        {
            var chapters = new List<GodaEnChapter>();
            try
            {
                var newUrl = Url.Replace("/manga", "/chapterlist");
                newUrl = newUrl.Substring(0, newUrl.Length - 1) + ".html";
                var chaptershtml = await Source.HttpClient!.GetStringAsync(newUrl);

                var matches = Regex.Matches(chaptershtml, "<a id[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?>([\\s\\S]*?)<").ToList();
                var start = matches.Count() - 1;
                if (matches.FirstOrDefault() is not null)
                {
                    LastestChapterName = matches.First().Groups[2].Value.Replace("\\n", "").Replace(" ", "");
                    foreach (Match match in matches)
                    {
                        chapters.Add(new GodaEnChapter(
                            match.Groups[2].Value.Replace("\\n", "").Replace(" ", ""),
                            match.Groups[1].Value,
                            start,
                            start == LastReadedChapterIndex)
                        { Comic = this });
                        start--;
                    }
                }
                else
                {
                    LastestChapterName = "";
                    Chapters = Chapters.Append(new GodaEnChapter("暂无章节", "", -1, false) { Comic = this });
                    return;
                }
            }
            catch
            {
                Chapters = Chapters.Append(new GodaEnChapter("暂无章节", "", -1, false) { Comic = this });
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
            var index = "listing\"";
            if (Html.IndexOf(index) < 0)
            {
                return null;
            }
            var chaptershtml = Html.Substring(Html.IndexOf(index));
            var match = Regex.Match(chaptershtml, "manga-chapter\"[\\s\\S]*?>([\\s\\S]*?)<");
            if (match is not null)
            {
                return match.Groups[1].Value;
            }
            else { return null; }
        }
    }
}