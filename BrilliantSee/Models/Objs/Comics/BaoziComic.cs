using BrilliantSee.Models.Items.Chapters;
using BrilliantSee.Models.Sources;
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

namespace BrilliantSee.Models.Objs.Comics
{
    public class BaoziComic : Obj
    {
        /// <summary>
        /// 获取更多漫画信息
        /// </summary>
        /// <returns></returns>
        ///
        public override void LoadMoreData()
        {
            //截取两个字符串之间的内容
            var start = Html.IndexOf("<body");
            var end = Html.IndexOf("猜你喜欢");
            if (start < 0 || end < 0)
            {
                Items = Items.Append(new BaoziChapter("暂无章节", "", -1, false) { Obj = this });
                return;
            }
            var moreDataHtml = Html.Substring(start, end - start);
            if (!string.IsNullOrEmpty(Html))
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
        public override async Task LoadItemsAsync()
        {
            var index = "章节目录";
            var flag = true;
            var chapters = new List<BaoziChapter>();
            if (Html.IndexOf(index) < 0)
            {
                index = "class=\"section-title\"";
                flag = !flag;
                if (Html.IndexOf(index) < 0)
                {
                    Items = Items.Append(new BaoziChapter("暂无章节", "", -1, false) { Obj = this });
                    return;
                }
            }

            var chaptershtml = Html.Substring(Html.IndexOf(index));
            var matches = Regex.Matches(chaptershtml, "comics-chapters\"[\\s\\S]*?href=\"(.*?)\"[\\s\\S]*?<span.*?>([\\s\\S]*?)</span>").ToList();
            var start = matches.Count() - 1;
            if (flag)
            {
                matches.Reverse();
            }
            if (matches.FirstOrDefault() is not null)
            {
                LastestItemName = matches.FirstOrDefault()!.Groups[2].Value;
            }
            foreach (Match match in matches)
            {
                chapters.Add(new BaoziChapter(
                    match.Groups[2].Value,
                    "https://www.czmanga.com" + match.Groups[1].Value.Replace("amp;", ""),
                    start,
                    start == LastReadedItemIndex)
                { Obj = this });
                start--;
            }
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Items = chapters;
                ItemCount = Items.Count();
            });
        }

        public override string? GetLastestItemName()
        {
            var index = "章节目录";
            var flag = true;
            if (Html.IndexOf(index) < 0)
            {
                index = "class=\"section-title\"";
                flag = !flag;
                if (Html.IndexOf(index) < 0)
                {
                    return null;
                }
            }
            var chaptershtml = Html.Substring(Html.IndexOf(index));
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