using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantSee.Models.Items.Chapters
{
    public class DingDianChapter : Item
    {
        public DingDianChapter(string name, string url, int index, bool isSpecial) : base(name, url, index, isSpecial)
        {
        }

        public override async Task GetResourcesAsync()
        {
            var url = string.Empty;
            var count = 1;
            var html = string.Empty;
            do
            {
                url = Url.Replace(".html", $"_{count}.html");
                html = await Obj.Source.GetHtmlAsync(url);
                html = html.Substring(html.IndexOf("reader-main"), html.IndexOf("本站最新网址") - html.IndexOf("reader-main"));
                var match = Regex.Matches(html, "<p>([\\s\\S]*?)</p>");
                foreach (Match item in match)
                {
                    NovelContent += item.Groups[1].Value + "\r\n\r\n\t\t\t\t\t\t";
                }
                count++;
            } while (html.Contains("下一页"));
        }
    }
}