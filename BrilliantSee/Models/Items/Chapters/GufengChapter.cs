﻿using BrilliantSee.Models.Objs;
using System.Text.RegularExpressions;

namespace BrilliantSee.Models.Items.Chapters
{
    public class GufengChapter : Item
    {
        public GufengChapter(string name, string url, int index, bool isSpecial) : base(name, url, index, isSpecial)
        {
        }

        public override async Task GetResourcesAsync()
        {
            try
            {
                var picUrls = new List<string>();
                var html = await Obj.Source.GetHtmlAsync(Url);
                if (html == string.Empty)
                    throw new Exception("请求失败");
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
                    picUrls.Add(pathHead + item.Groups[1].Value);
                }
                PicUrls = picUrls.Distinct().ToList();
                PageCount = PicUrls.Count;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}