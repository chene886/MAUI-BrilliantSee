using BrilliantSee.Models.Objs;
using System.Text.RegularExpressions;

namespace BrilliantSee.Models.Items.Chapters
{
    public class BaoziChapter : Item
    {
        public BaoziChapter(string name, string url, int index, bool isSpecial) : base(name, url, index, isSpecial)
        {
        }

        /// <summary>
        /// 获取章节图片
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task GetResourcesAsync()
        {
            try
            {
                var url = Url;
                var count = 1;
                var html = string.Empty;
                MatchCollection match;
                do
                {
                    if (count != 1)
                    {
                        url = Url + $"_{count}";
                    }
                    //var msg = await Obj.Source.HttpClient!.GetAsync(url);
                    //if (msg.RequestMessage is null || msg.RequestMessage.RequestUri is null)
                    //{
                    //    if (count == 1)
                    //    {
                    //        throw new Exception("请求失败");
                    //    }
                    //    break;
                    //}
                    //html = (await msg.Content.ReadAsStringAsync()).Replace("\n", string.Empty);
                    html = (await Obj.Source.GetHtmlAsync(url)).Replace("\n", string.Empty);
                    if (html == string.Empty) throw new Exception("请求失败");
                    match = Regex.Matches(html, "<noscript [\\s\\S]*?src=\\\"([\\s\\S]*?)\\\"[\\s\\S]*?</noscript>");
                    foreach (Match item in match)
                    {
                        PicUrls.Add(item.Groups[1].Value);
                    }
                    count++;
                } while (Regex.Matches(html, "点击进入下一页").FirstOrDefault() is not null || Regex.Matches(html, "點擊進入下一頁").FirstOrDefault() is not null);
                if (PicUrls.Count == 1) PicUrls.Add(PicUrls[0]);
                PageCount = PicUrls.Count;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}