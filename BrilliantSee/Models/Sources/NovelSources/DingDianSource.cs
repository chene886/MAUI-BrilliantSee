using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Objs.Novels;
using BrilliantSee.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Toast = CommunityToolkit.Maui.Alerts.Toast;

namespace BrilliantSee.Models.Sources.NovelSources
{
    public class DingDianSource : Source
    {
        public DingDianSource()
        {
            Name = "顶点小说";
            SetHttpClient("https://www.ddxs.vip/");
            Category = Enums.SourceCategory.Novel;
        }

        public override async Task<IEnumerable<Obj>> SearchAsync(string keyword)
        {
            var url = $"https://www.ddxs.vip/search/?searchkey={keyword}";
            var html = await GetHtmlAsync(url);
            if (html == string.Empty) { return Array.Empty<Obj>(); }
            if (html.Contains("alert(\"搜索间隔: 30 秒\")"))
            {
                return Array.Empty<Obj>().Append(new DingDianNovel() { Cover = "https://i.postimg.cc/xTMSDvYK/nocover.jpg", Name = "顶点源搜索需间隔30秒", Url = "https://www.ddxs.vip", Source = this });
            }
            string pattern = "s2[\\s\\S]*?href=\"(.*?)\">(.*?)<[\\s\\S]*?s4\">(.*?)<[\\s\\S]*?s5\">(.*?)<";
            var matches = Regex.Matches(html, pattern);
            if (matches.Count == 0) { return Array.Empty<Obj>(); }
            matches.ToList().RemoveAt(0);
            var novels = new List<Obj>();
            foreach (Match match in matches)
            {
                var novel = new DingDianNovel()
                {
                    Url = "https://www.ddxs.vip" + match.Groups[1].Value,
                    Name = match.Groups[2].Value,
                    Cover = "https://i.postimg.cc/xTMSDvYK/nocover.jpg",
                    Author = match.Groups[3].Value,
                    LastestUpdateTime = match.Groups[4].Value,
                    Source = this,
                    SourceName = Name,
                    SourceCategory = Category,
                };
                novels.Add(novel);
            }
            return novels;
        }
    }
}