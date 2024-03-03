using BrilliantComic.Models.Comics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Chapters
{
    public class GodaChapter : Chapter
    {
        public GodaChapter(string name, string url, int index, bool isSpecial) : base(name, url, index, isSpecial)
        {
        }

        /// <summary>
        /// 获取章节图片
        /// </summary>
        /// <returns>章节图片枚举器</returns>
        /// <exception cref="Exception"></exception>
        public override async Task<IEnumerable<string>> GetPicEnumeratorAsync()
        {
            try
            {
                var msg = (await Comic.Source.HttpClient.GetAsync(Url));
                if (msg.RequestMessage is null || msg.RequestMessage.RequestUri is null)
                    throw new Exception("接口异常,请等待维护");
                var html = await msg.Content.ReadAsStringAsync();
                html = html.Substring(html.IndexOf("w-full h-full"));
                var match = Regex.Matches(html, "w-full h-full[\\s\\S]*?src=\"(.*?)\"");
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