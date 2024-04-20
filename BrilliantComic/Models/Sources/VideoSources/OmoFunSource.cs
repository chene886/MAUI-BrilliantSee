using BrilliantSee.Models.Enums;
using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Objs.Videos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantSee.Models.Sources.VideoSources
{
    public class OmoFunSource : Source
    {
        public OmoFunSource()
        {
            SetHttpClient("https://www.211dm.com/");
            Name = "OMO动漫";
            HasMore = 1;
            Category = SourceCategory.Video;
        }

        public override async Task<IEnumerable<Obj>> SearchAsync(string keyword)
        {
            //var url = $"https://www.211dm.com/search/{keyword}----------{ResultNum}---.html";
            var url = $"https://www.211dm.com/search/-------------.html?wd={keyword}&submit=";
            var html = await GetHtmlAsync(url);
            if (html == string.Empty) { return Array.Empty<Obj>(); }

            string pattern = "gEI[\\s\\S]*?href=\"(.*?)\"\\stitle=\"(.*?)\"[\\s\\S]*?=\"(.*?)\"[\\s\\S]*?DE\\s[\\s\\S]*?<b>(.*?)<[\\s\\S]*?<p[\\s\\S]*?>(.*?)<";
            var matches = Regex.Matches(html, pattern);
            if (matches.Count < 24) { HasMore = 0; }

            var objs = new List<Obj>();
            foreach (Match match in matches)
            {
                var obj = new OmoFunVideo()
                {
                    Url = "https://www.211dm.com" + match.Groups[1].Value,
                    Name = match.Groups[2].Value,
                    Cover = match.Groups[3].Value,
                    LastestUpdateTime = match.Groups[4].Value,
                    Author = match.Groups[5].Value,
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