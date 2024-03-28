using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BrilliantComic.Models.Chapters
{
    public class HasuChapter : Chapter
    {
        public HasuChapter(string name, string url, int index, bool isSpecial) : base(name, url, index, isSpecial)
        {
        }

        public override async Task<IEnumerable<string>> GetPicEnumeratorAsync()
        {
            try
            {
                var msg = (await Comic.Source.HttpClient!.GetAsync(Url));
                if (msg.RequestMessage is null || msg.RequestMessage.RequestUri is null)
                    throw new Exception("接口异常,请等待维护");
                var html = await msg.Content.ReadAsStringAsync();
                var start = html.IndexOf("loadchapter");
                var end = html.IndexOf("Comments");
                if (start < 0 || end < 0)
                {
                    throw new Exception("接口异常,请等待维护");
                }
                html = html.Substring(start, end - start);
                var match = Regex.Matches(html, "pagenum[\\s\\S]*?data-src=\"(.*?)\"");
                var list = new List<string>();
                foreach (Match item in match)
                {
                    list.Add(item.Groups[1].Value);
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