using BrilliantSee.Models.Items.Chapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantSee.Models.Objs.Novels
{
    public class DingDianNovel : Obj
    {
        public override string? GetLastestItemName()
        {
            return Regex.Match(Html, "lastest_chapter_name[\\s\\S]*?=\"(.*?)\"").Groups[1].Value;
        }

        public override async Task LoadItemsAsync()
        {
            var count = 0;
            var html = string.Empty;
            var index = 0;
            do
            {
                count++;
                var url = Url.TrimEnd('/') + $"_{count}/";
                html = await Source.GetHtmlAsync(url);
                html = html.Substring(html.IndexOf("章节目录"));
                var matches = Regex.Matches(html, "<li>[\\s\\S]*?href=\"(.*?)\">(.*?)<");
                foreach (Match match in matches)
                {
                    var item = new DingDianChapter(
                        match.Groups[2].Value,
                        "https://www.ddxs.vip" + match.Groups[1].Value,
                        index,
                        index == LastReadedItemIndex)
                    { Obj = this };
                    Items = Items.Append(item);
                    index++;
                }
            } while (html.Contains("下一页"));
            Items = Items.Reverse();
            ItemCount = Items.Count();
            LastestItemName = Items.FirstOrDefault()!.Name;
        }

        public override void LoadMoreData()
        {
            if (!string.IsNullOrEmpty(Html))
            {
                var match = Regex.Match(Html, "img\\s[\\s\\S]*?src=\"(.*?)\"[\\s\\S]*?态：(.*?)<[\\s\\S]*?数：<span>(.*?)<[\\s\\S]*?/strong>([\\s\\S]*?)<p");
                Cover = match.Groups[1].Value;
                Status = match.Groups[2].Value;
                CharCount = "总字数：" + match.Groups[3].Value;
                Description = match.Groups[4].Value.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            }
        }
    }
}