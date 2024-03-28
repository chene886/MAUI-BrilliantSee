using BrilliantComic.Models.Comics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Chapters
{
    public class GufengChapter : Chapter
    {
        public GufengChapter(string name, string url, int index, bool isSpecial) : base(name, url, index, isSpecial)
        {
        }

        public override async Task GetPicEnumeratorAsync()
        {
            try
            {
                var msg = (await Comic.Source.HttpClient!.GetAsync(Url));
                if (msg.RequestMessage is null || msg.RequestMessage.RequestUri is null)
                    throw new Exception("接口异常,请等待维护");
                var html = await msg.Content.ReadAsStringAsync();
                var start = html.IndexOf("chapterImages");
                var end = html.IndexOf("chapterPrice");
                if (start < 0 || end < 0)
                {
                    throw new Exception("接口异常,请等待维护");
                }
                html = html.Substring(start, end - start);
                var match = Regex.Matches(html, "[\\[,]\"(.*?)\"");
                var pathHead = "https://res.xiaoqinre.com/" + Regex.Match(html, "chapterPath = \"(.*?)\"").Groups[1].Value;
                foreach (Match item in match)
                {
                    PicUrls.Add(pathHead + item.Groups[1].Value);
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