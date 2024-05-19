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
            var content = string.Empty;
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
                        content += "\t\t\t\t\t\t" + item.Groups[1].Value + "\r\n\r\n";
                    }
                    count++;
                } while (Regex.Matches(html, "下一页").FirstOrDefault() is not null);
                NovelContent = content.Replace("&lt;/p&gt;", "").Replace("\t\t\t\t\t\tdengbi.netdmxsw.comqqxsw.comyifan.net\r\r\n\r\n\t\t\t\t\t\tshuyue.netepzw.netqqwxw.comxsguan.com\r\r\n\r\n\t\t\t\t\t\txs007.comzhuike.netreadw.com23zw.cc\r\n\r\n", "").Replace("\t\t\t\t\t\tepzww3366xs80wxxsxs\r\r\n\r\n\t\t\t\t\t\tyjxs3jwx8pzwxiaohongshu\r\r\n\r\n\t\t\t\t\t\tkanshubahxsw7tbiquhe\r\n\r\n", "").Replace("\t\t\t\t\t\tepzww.com3366xs.com80wx.comxsxs.cc\r\r\n\r\n\t\t\t\t\t\tyjxs.cc3jwx.com8pzw.comxiaohongshu.cc\r\r\n\r\n\t\t\t\t\t\tkanshuba.cchmxsw.com7cct.combiquhe.com\r\n\r\n", "").Replace("\t\t\t\t\t\tdengbidxswqqxswyifan\r\r\n\r\n\t\t\t\t\t\tshuyueepzwqqwxwxsguan\r\r\n\r\n\t\t\t\t\t\txs007zhuikereadw23zw\r\n\r\n", "");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}