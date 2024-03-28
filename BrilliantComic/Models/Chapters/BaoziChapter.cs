using BrilliantComic.Models.Comics;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
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
        /// 获取章节图片
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task GetPicEnumeratorAsync()
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
                        url = Url.Replace(".html", $"_{count}.html");
                    }
                    var msg = (await Comic.Source.HttpClient!.GetAsync(url));
                    if (msg.RequestMessage is null || msg.RequestMessage.RequestUri is null)
                    {
                        if (count == 1)
                        {
                            throw new Exception("请求失败");
                        }
                        break;
                    }
                    html = (await msg.Content.ReadAsStringAsync()).Replace("\n", string.Empty);
                    match = Regex.Matches(html, "<noscript [\\s\\S]*?src=\\\"([\\s\\S]*?)\\\"[\\s\\S]*?</noscript>");
                    foreach (Match item in match)
                    {
                        PicUrls.Add(item.Groups[1].Value);
                    }
                    count++;
                } while (Regex.Matches(html, "点击进入下一页").FirstOrDefault() is not null);
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