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

        public override async Task<IEnumerable<string>> GetPicEnumeratorAsync()
        {
            try
            {
                var msg = (await Comic.Source.HttpClient.GetAsync(Url));
                if (msg.RequestMessage is null || msg.RequestMessage.RequestUri is null)
                    throw new Exception("接口异常,请等待维护");
                var html = await msg.Content.ReadAsStringAsync();
                var start = html.IndexOf("chapterImages");
                var end = html.IndexOf("chapterPrice");
                html = html.Substring(start,end-start);
                var match = Regex.Matches(html, "[\\[,]\"(.*?)\"");
                var pathHead = "https://res.xiaoqinre.com/"+Regex.Match(html, "chapterPath = \"(.*?)\"").Groups[1].Value;
                var list = new List<string>();
                foreach (Match item in match)
                {
                    list.Add(pathHead+item.Groups[1].Value);
                }
                if (list.Count == 1) list.Add(list[0]);
                PageCount = list.Count;
                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}