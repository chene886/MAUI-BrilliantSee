using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Objs.Videos;
using BrilliantSee.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantSee.Models.Sources.VideoSources
{
    public class YHWangSource : Source
    {
        public YHWangSource()
        {
            SetHttpClient("https://www.yhdmwang.com/");
            Name = "樱花动漫网";
            HasMore = 1;
            Category = SourceCategory.Video;
        }

        public override async Task<IEnumerable<Obj>> SearchAsync(string keyword)
        {
            var url = $"https://www.yhdmwang.com/search.php?page={ResultNum}&searchword={keyword}";
            var html = await GetHtmlAsync(url);
            if (html == string.Empty) { return Array.Empty<Obj>(); }

            string pattern = "vodlist__box[\\s\\S]*?href=\"(.*?)\"\\s*title=\"(.*?)\"\\s*data-original=\"(.*?)\"[\\s\\S]*?text\\s[\\s\\S]*?b>(.*?)<[\\s\\S]*?<p[\\s\\S]*?>(.*?)</p>";
            var matches = Regex.Matches(html, pattern);
            if (matches.Count < 24) { HasMore = 0; }

            var objs = new List<Obj>();
            foreach (Match match in matches)
            {
                var obj = new YHWangVideo()
                {
                    Url = "https://www.yhdmwang.com" + match.Groups[1].Value,
                    Name = match.Groups[2].Value,
                    Cover = "https://www.yhdmwang.com" + match.Groups[3].Value,
                    LastestUpdateTime = match.Groups[4].Value,
                    Author = string.Join("，", Regex.Replace(match.Groups[5].Value, "<[^>]+>", "").Split("&nbsp;", StringSplitOptions.RemoveEmptyEntries)),
                    Source = this,
                    SourceName = Name,
                    SourceCategory = Category,
                };
                objs.Add(obj);
            }
            return objs;
        }
    }
}