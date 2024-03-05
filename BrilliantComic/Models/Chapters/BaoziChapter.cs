using BrilliantComic.Models.Comics;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Chapters
{
    public class BaoziChapter : Chapter
    {
        public BaoziChapter(string name, string url, int index, bool isSpecial) : base(name, url, index, isSpecial)
        {
        }

        /// <summary>
        /// 获取章节图片枚举器
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task<IEnumerable<string>> GetPicEnumeratorAsync()
        {
            try
            {
                var msg = (await Comic.Source.HttpClient.GetAsync(Url));
                if (msg.RequestMessage is null || msg.RequestMessage.RequestUri is null)
                    throw new Exception("接口异常,请等待维护");
                var html = (await msg.Content.ReadAsStringAsync()).Replace("\n", string.Empty);
                var match = Regex.Matches(html, "<noscript [\\s\\S]*?src=\\\"([\\s\\S]*?)\\\"[\\s\\S]*?</noscript>");
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