using BrilliantComic.Models.Chapters;
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
        private string html = string.Empty;

        public HasuComic(string url, string name, string cover, string author)
        {
            Url = url;
            Cover = cover;
            Name = name;
            Author = author;
        }

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

        public override string? GetLastestChapterName()
        {
            var Istart = html.IndexOf("list-chapter");
            var end = html.IndexOf("div-comment");
            if (Istart < 0 || end < 0)
            {
                return "";
            }
            var chaptershtml = html.Substring(Istart, end - Istart);
            var match = Regex.Match(chaptershtml, "name[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?/span>(.*?)<");
            if (match is not null)
            {
                return match.Groups[2].Value;
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
            var Istart = html.IndexOf("list-chapter");
            var end = html.IndexOf("div-comment");
            var chapters = new List<HasuChapter>();
            if (Istart < 0 || end < 0)
            {
                Chapters = Chapters.Append(new GufengChapter("暂无章节", "", -1, false) { Comic = this });
                return;
            }
            var chaptershtml = html.Substring(Istart, end - Istart);
            var matches = Regex.Matches(chaptershtml, "name[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?/span>(.*?)<").ToList();
            var start = matches.Count() - 1;
            if (matches.FirstOrDefault() is not null)
            {
                LastestChapterName = matches.FirstOrDefault()!.Groups[2].Value;
                LastestUpdateTime = Regex.Match(chaptershtml, "").Groups[1].Value;
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
            var start = html.IndexOf("Author");
            var end = html.IndexOf("list-chapter");
            var moreDataHtml = html.Substring(start, end - start);
            if (!string.IsNullOrEmpty(html))
            {
                var result = Regex.Match(moreDataHtml, "Author[\\s\\S]*?<a[\\s\\S]*?>(.*?)<[\\s\\S]*?Artist[\\s\\S]*?<a[\\s\\S]*?>(.*?)<[\\s\\S]*?Status[\\s\\S]*?<a[\\s\\S]*?>(.*?)<");
                Author = result.Groups[1].Value + "(作者)," + result.Groups[2].Value + "(画手)";
                Status = result.Groups[3].Value;
                var result2 = Regex.Match(moreDataHtml, "Summary[\\s\\S]*?<div>([\\s\\S]*?)</div>");
                Description = result2.Groups[1].Value.Replace("<p>", "").Replace("\\n", "");
            }
        }
    }
}