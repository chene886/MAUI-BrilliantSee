using BrilliantSee.Models.Items;
using BrilliantSee.Models.Items.Episodes;
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
                Tag = Regex.Match(Html, "<p[\\s\\S]*?>(.*?)<").Groups[1].Value.Replace(" ", "") + "/评分：" + Regex.Match(Html, "评分[\\s\\S]*?</span>(.*?)<").Groups[1].Value;
                Director = "导演：" + Regex.Match(Html, "导演：(.*?)<").Groups[1].Value;
                Actors = "主演：" + Regex.Match(Html, "主演：(.*?)<").Groups[1].Value;
                var match = Regex.Match(Html, "description[\\s\\S]*?剧情：(.*?)\"");
                Description = "简介：" + Regex.Replace(match.Groups[1].Value, "<[^>]+>", "").Replace("　　", "");
            }
        }
    }
}