using BrilliantComic.Models.Chapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Comics
{
    public class GufengComic : Comic
    {
        private string html = string.Empty;

        public GufengComic(string url, string name, string cover, string author)
        {
            Url = url;
            Cover = cover;
            Name = name;
            Author = author;
        }

        /// <summary>
        /// 获取漫画网页源代码
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> GetHtmlAsync()
        {
            try
            {
                html = await Source.HttpClient.GetStringAsync(Url);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public override string? GetLastestChapterName()
        {
            throw new NotImplementedException();
        }

        public override Chapter? GetNearChapter(Chapter chapter, string flag)
        {
            throw new NotImplementedException();
        }

        public override Task LoadChaptersAsync()
        {
            throw new NotImplementedException();
        }

        public override void LoadMoreData()
        {
            throw new NotImplementedException();
        }
    }
}