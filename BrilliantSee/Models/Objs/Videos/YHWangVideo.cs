using BrilliantSee.Models.Chapters;
using BrilliantSee.Models.Episodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantSee.Models.Objs.Videos
{
    public class YHWangVideo : Obj
    {
        public override string? GetLastestItemName()
        {
            return Regex.Matches(Html, "<li\\sid=\"(.*?)\"><a\\stitle=\"(.*?)\"\\shref=\"(.*?)\"").Count().ToString();
        }

        public override async Task LoadItemsAsync()
        {
            var flag = true;
            var episodes = new List<YHWangEpisode>();

            var matches = Regex.Matches(Html, "<li\\sid=\"(.*?)\"><a\\stitle=\"(.*?)\"\\shref=\"(.*?)\"").ToList();
            var start = matches.Count() - 1;
            if (flag)
            {
                matches.Reverse();
            }
            if (matches.FirstOrDefault() is not null)
            {
                LastestItemName = matches.Count().ToString();
            }
            foreach (Match match in matches)
            {
                //获取第一个参数的第一个字符
                var ch = match.Groups[1].Value[0];

                episodes.Add(new YHWangEpisode(
                    match.Groups[2].Value,
                    "https://www.yhdmwang.com" + match.Groups[3].Value,
                    start,
                    start == LastReadedItemIndex)
                { Obj = this, Route = match.Groups[1].Value[0] == '1' ? "线路一" : "线路二" });
                start--;
            }
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Items = episodes;
                ItemCount = Items.Count();
            });
        }

        public override void LoadMoreData()
        {
            if (!string.IsNullOrEmpty(Html))
            {
                var match = Regex.Match(Html, "<p[\\s\\S]*?>(.*?)<[\\s\\S]*?导演：(.*?)<[\\s\\S]*?主演：(.*?)<[\\s\\S]*?muted[\\s\\S]*?</span>(.*?)<[\\s\\S]*?<p>(.*?)</p>");
                Tag = match.Groups[1].Value.Replace(" ", "") + "/评分：" + match.Groups[4].Value;
                Director = "导演："+ match.Groups[2].Value;
                Actors ="主演："+ match.Groups[3].Value;
                Description = "简介：" + Regex.Replace(match.Groups[5].Value, "<[^>]+>", "").Replace("　　", "");
            }
        }
    }
}