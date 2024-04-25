using BrilliantSee.Models.Objs;
using System.Text.RegularExpressions;

namespace BrilliantSee.Models.Items.Chapters
{
    public class GodaChapter : Item
    {
        public GodaChapter(string name, string url, int index, bool isSpecial) : base(name, url, index, isSpecial)
        {
        }

        /// <summary>
        /// 获取章节图片
        /// </summary>
        /// <returns>章节图片枚举器</returns>
        /// <exception cref="Exception"></exception>
        public override async Task GetResourcesAsync()
        {
            try
            {
                var msg = await Obj.Source.HttpClient!.GetAsync(Url);
                if (msg.RequestMessage is null || msg.RequestMessage.RequestUri is null)
                    throw new Exception("接口异常,请等待维护");
                var html = await msg.Content.ReadAsStringAsync();
                html = html.Substring(html.IndexOf("w-full h-full"));
                var match = Regex.Matches(html, "w-full h-full[\\s\\S]*?src=\"(.*?)\"");
                foreach (Match item in match)
                {
                    PicUrls.Add(item.Groups[1].Value);
                }
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