using BrilliantSee.Models.Items.Chapters;
using System.Text.RegularExpressions;

namespace BrilliantSee.Models.Objs.Comics
{
    public class GodaComic : Obj
    {
        /// <summary>
        /// 获取更多漫画信息
        /// </summary>
        public override void LoadMoreData()
        {
            //截取两个字符串之间的内容
            var start = Html.IndexOf("bannersUite");
            var end = Html.IndexOf("ChapterHistory");
            if (end < 0) end = Html.IndexOf("chapterlist");
            if (start < 0 || end < 0)
            {
                Status = "连载中";
                Description = "暂无简介";
                LastestUpdateTime = "暂无更新时间";
                Author = "暂无作者";
                return;
            }
            var moreDataHtml = Html.Substring(start, end - start);
            if (!string.IsNullOrEmpty(Html))
            {
                var result = Regex.Match(moreDataHtml, "<span class=\"text-xs[\\s\\S]*?>[\\s](.*?)[\\s]<[\\s\\S]*?<p[\\s\\S]*?>([\\s\\S]*?)<");
                Status = result.Groups[1].Value;
                Description = result.Groups[2].Value.Replace("\\n", "").Replace("amp;", "");
                var result1 = Regex.Match(moreDataHtml, "italic[\\s\\S]*?>(.*?)[\\s]<");
                LastestUpdateTime = "(最新章节：" + result1.Groups[1].Value + ")";
                var result2 = Regex.Matches(moreDataHtml, "\"/manga-author[\\s\\S]*?<span>(.*?)<").ToList();
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
        public override async Task LoadItemsAsync()
        {
            var index = "最近章節";
            var flag = true;
            var chapters = new List<GodaChapter>();
            try
            {
                var newUrl = Url.Replace("/manga", "/chapterlist");
                var chaptershtml = await Source.HttpClient!.GetStringAsync(newUrl);

                if (Html.IndexOf(index) < 0)
                {
                    Items = Items.Append(new GodaChapter("暂无章节", "", -1, false) { Obj = this });
                    return;
                }
                chaptershtml = chaptershtml.Substring(chaptershtml.IndexOf(index));
                var matches = Regex.Matches(chaptershtml, "chapteritem[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?title[\\s\\S]*?>[\\s](.*?)[\\s]<").ToList();
                if (flag) matches.Reverse();
                var start = matches.Count() - 1;
                if (matches.FirstOrDefault() is not null)
                {
                    LastestItemName = matches.FirstOrDefault()!.Groups[2].Value;
                    foreach (Match match in matches)
                    {
                        chapters.Add(new GodaChapter(
                            match.Groups[2].Value,
                            match.Groups[1].Value,
                            start,
                            start == LastReadedItemIndex)
                        { Obj = this });
                        start--;
                    }
                }
                else
                {
                    LastestItemName = "";
                    Items = Items.Append(new GodaChapter("暂无章节", "", -1, false) { Obj = this });
                    return;
                }
            }
            catch
            {
                Items = Items.Append(new GodaChapter("暂无章节", "", -1, false) { Obj = this });
                return;
            }
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Items = chapters;
                ItemCount = Items.Count();
            });
        }

        /// <summary>
        /// 获取最新章节名
        /// </summary>
        /// <returns></returns>
        public override string? GetLastestItemName()
        {
            var index = "最近章節";
            if (Html.IndexOf(index) < 0)
            {
                return null;
            }
            var chaptershtml = Html.Substring(Html.IndexOf(index));
            var match = Regex.Match(chaptershtml, "chaptertitle[\\s\\S]*?>[\\s](.*?)[\\s]<");
            if (match is not null)
            {
                return match.Groups[1].Value;
            }
            else { return null; }
        }
    }
}