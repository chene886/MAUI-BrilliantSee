using System.Text.RegularExpressions;

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
            try
            {
                do
                {
                    url = Url.Replace(".html", $"_{count}.html");
                    html = await Obj.Source.GetHtmlAsync(url);
                    if (html == string.Empty) throw new Exception("请求失败");
                    html = html.Substring(html.IndexOf("reader-main"), html.IndexOf("本站最新网址") - html.IndexOf("reader-main"));
                    var match = Regex.Matches(html, "<p>([\\s\\S]*?)</p>");
                    foreach (Match item in match)
                    {
                        NovelContent += "\t\t\t\t\t\t" + item.Groups[1].Value + "\r\n\r\n";
                    }
                    count++;
                } while (Regex.Matches(html, "下一页").FirstOrDefault() is not null);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}