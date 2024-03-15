using BrilliantComic.Models.Chapters;
using BrilliantComic.Models.Comics;
using BrilliantComic.Models.Enums;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Services.Plugins.FindTarget
{
    public sealed class FindPlugin
    {
        private readonly DBService _db;
        private readonly SourceService _sourceService;

        public FindPlugin(DBService db, SourceService sourceService)
        {
            _db = db;
            _sourceService = sourceService;
        }

        [KernelFunction, Description("查找指定的历史漫画")]
        [return: Description("查到的漫画（可能没有）")]
        private async Task<Comic>? FindHistoryAsync(
                                  [Description("要查找的漫画名称")] string name)
        {
            var Comics = await _db.GetComicsAsync(DBComicCategory.History);
            //模糊查找漫画名取第一个
            var comic = Comics.Where(c => c.Name.Contains(name)).FirstOrDefault();
            return comic;
        }

        [KernelFunction, Description("查找指定的收藏漫画")]
        [return: Description("查到的漫画（可能没有）")]
        private async Task<Comic>? FindFavoriteAsync(
                          [Description("要查找的漫画名称")] string name)
        {
            var Comics = await _db.GetComicsAsync(DBComicCategory.Favorite);
            //模糊查找漫画名取第一个
            var comic = Comics.Where(c => c.Name.Contains(name)).FirstOrDefault();
            return comic;
        }

        [KernelFunction, Description("在没有指明要在收藏漫画或历史漫画列表中查找的情况时，搜索指定名字的漫画")]
        [return: Description("搜索结果组成的漫画集合")]
        private async Task<ObservableCollection<Comic>>? FindComicByNameAsync(
                                             [Description("要查找的漫画名称")] string name)
        {
            var Comics = new ObservableCollection<Comic>();
            await _sourceService.SearchAsync(name, Comics, "AI");
            return Comics;
        }

        [KernelFunction, Description("获取漫画集合中的第一个漫画")]
        [return: Description("获取到的漫画（可能没有）")]
        private Comic? GetFirstComicAsync(
                                                [Description("用于获取结果的漫画集合")] ObservableCollection<Comic> comics)
        {
            return comics.FirstOrDefault();
        }

        [KernelFunction, Description("在特定漫画中查找指定名字的章节")]
        [return: Description("获取到的章节（可能没有）")]
        private Chapter? FindChapterAsync(
                                  [Description("特定的漫画")] Comic comic,
                                 [Description("要查找的章节名称")] string name)
        {
            var chapter = comic.Chapters.Where(c => c.Name.Contains(name)).FirstOrDefault();
            return chapter;
        }

        [KernelFunction, Description("获得特定漫画的第一个章节")]
        [return: Description("获取到的章节（可能没有）")]
        private Chapter? GetFirstChapterAsync(
                                             [Description("特定的漫画")] Comic comic)
        {
            if (comic.IsReverseList == true) return comic.Chapters.LastOrDefault();
            else return comic.Chapters.FirstOrDefault();
        }

        [KernelFunction, Description("获得特定漫画的最后一个或最新章节")]
        [return: Description("获取到的章节（可能没有）")]
        private Chapter? GetLastChapterAsync(
                                                        [Description("特定的漫画")] Comic comic)
        {
            if (comic.IsReverseList == true) return comic.Chapters.FirstOrDefault();
            else return comic.Chapters.LastOrDefault();
        }

        [KernelFunction, Description("获得特定漫画的最后浏览章节")]
        [return: Description("获取到的章节（可能没有）")]
        private Chapter? GetLastReadChapterAsync(
                                                                   [Description("特定的漫画")] Comic comic)
        {
            if (comic.LastReadedChapterIndex == -1) return null;
            return comic.Chapters.ToList()[comic.LastReadedChapterIndex];
        }
    }
}