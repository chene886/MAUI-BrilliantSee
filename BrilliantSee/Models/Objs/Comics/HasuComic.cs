using BrilliantSee.Models.Items.Chapters;
using System.Text.RegularExpressions;

namespace BrilliantSee.Models.Objs.Comics
{
    public class HasuComic : Obj
    {
        public override string? GetLastestItemName()
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

        public override async Task LoadItemsAsync()
        {
            var Istart = Html.IndexOf("list-chapter");
            var end = Html.IndexOf("div-comment");
            var chapters = new List<HasuChapter>();
            if (Istart < 0 || end < 0)
            {
                Items = Items.Append(new HasuChapter("暂无章节", "", -1, false) { Obj = this });
                return;
            }
            var chaptershtml = Html.Substring(Istart, end - Istart);
            var matches = Regex.Matches(chaptershtml, "name[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?/span>(.*?)<").ToList();
            var start = matches.Count() - 1;
            if (matches.FirstOrDefault() is not null)
            {
                LastestItemName = matches.FirstOrDefault()!.Groups[2].Value;
            }
            foreach (Match match in matches)
            {
                chapters.Add(new HasuChapter(
                    match.Groups[2].Value,
                    match.Groups[1].Value,
                    start,
                    start == LastReadedItemIndex)
                { Obj = this });
                start--;
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Items = chapters;
            });
        }

        public override void LoadMoreData()
        {
            var start = Html.IndexOf("Author");
            var end = Html.IndexOf("list-chapter");
            if (start < 0 || end < 0)
            {
                Items = Items.Append(new HasuChapter("暂无章节", "", -1, false) { Obj = this });
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