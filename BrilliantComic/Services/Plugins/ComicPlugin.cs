using Microsoft.SemanticKernel;
using BrilliantComic.Models.Comics;
using BrilliantComic.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using BrilliantComic.Models.Chapters;
using System.Collections.ObjectModel;

namespace BrilliantComic.Services.Plugins
{
    public sealed class ComicPlugin
    {
        public DBService _db;
        public SourceService _sc;

        public ComicPlugin(DBService db, SourceService sc)
        {
            _db = db;
            _sc = sc;
        }

        //删除功能

        [KernelFunction, Description("删除指定漫画历史记录或取消收藏指定漫画")]
        [return: Description("是否成功删除或是否成功取消收藏")]
        public async Task<bool> DeleteComicAsync(
            [Description("指定的漫画")] Comic comic)
        {
            // 通过依赖注入获取数据库服务
            var result = await _db.DeleteComicAsync(comic, comic.Category);
            if (result > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [KernelFunction, Description("清空历史漫画")]
        [return: Description("是否成功清空")]
        public async Task<bool> ClearHistoryAsync()
        {
            var Comics = await _db.GetComicsAsync(DBComicCategory.History);
            foreach (var item in Comics)
            {
                await _db.DeleteComicAsync(item, item.Category);
            }
            Comics = await _db.GetComicsAsync(DBComicCategory.History);
            if (Comics.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [KernelFunction, Description("清空收藏漫画")]
        [return: Description("是否成功清空")]
        public async Task<bool> ClearFavoriteAsync()
        {
            var Comics = await _db.GetComicsAsync(DBComicCategory.Favorite);
            foreach (var item in Comics)
            {
                await _db.DeleteComicAsync(item, item.Category);
            }
            Comics = await _db.GetComicsAsync(DBComicCategory.Favorite);
            if (Comics.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //打开功能

        [KernelFunction, Description("打开指定的漫画")]
        public async Task OpenComicAsync(
               [Description("要打开的漫画")] Comic comic)
        {
            await Shell.Current.GoToAsync("DetailPage", new Dictionary<string, object> { { "Comic", comic } });
        }

        [KernelFunction, Description("打开指定的章节")]
        public async Task OpenChapterAsync(
                       [Description("要打开的章节")] Chapter chapter)
        {
            await Shell.Current.GoToAsync("ChapterPage", new Dictionary<string, object> { { "Chapter", chapter } });
        }

        //查找功能

        [KernelFunction, Description("查找指定的历史漫画")]
        [return: Description("查到的漫画（可能没有）")]
        public async Task<Comic>? FindHistoryAsync(
                          [Description("要查找的漫画名称")] string name)
        {
            var Comics = await _db.GetComicsAsync(DBComicCategory.History);
            //模糊查找漫画名取第一个
            var comic = Comics.Where(c => c.Name.Contains(name)).FirstOrDefault();
            return comic;
        }

        [KernelFunction, Description("查找指定的收藏漫画")]
        [return: Description("查到的漫画（可能没有）")]
        public async Task<Comic>? FindFavoriteAsync(
                          [Description("要查找的漫画名称")] string name)
        {
            var Comics = await _db.GetComicsAsync(DBComicCategory.Favorite);
            //模糊查找漫画名取第一个
            var comic = Comics.Where(c => c.Name.Contains(name)).FirstOrDefault();
            return comic;
        }

        [KernelFunction, Description("在没有指明要在收藏漫画或历史漫画列表中查找的情况时，搜索指定名字的漫画")]
        [return: Description("搜索结果组成的漫画集合")]
        public async Task<ObservableCollection<Comic>>? FindComicByNameAsync(
                                             [Description("要查找的漫画名称")] string name)
        {
            var Comics = new ObservableCollection<Comic>();
            await _sc.SearchAsync(name, Comics, "AI");
            return Comics;
        }

        [KernelFunction, Description("获取漫画集合中的第一个漫画")]
        [return: Description("获取到的漫画（可能没有）")]
        public Comic? GetFirstComicAsync(
                                                [Description("用于获取结果的漫画集合")] ObservableCollection<Comic> comics)
        {
            return comics.FirstOrDefault();
        }

        [KernelFunction, Description("在特定漫画中查找指定名字的章节")]
        [return: Description("获取到的章节（可能没有）")]
        public Chapter? FindChapterAsync(
                                  [Description("特定的漫画")] Comic comic,
                                 [Description("要查找的章节名称")] string name)
        {
            var chapter = comic.Chapters.Where(c => c.Name.Contains(name)).FirstOrDefault();
            return chapter;
        }

        [KernelFunction, Description("获得特定漫画的第一个章节")]
        [return: Description("获取到的章节（可能没有）")]
        public Chapter? GetFirstChapterAsync(
                                             [Description("特定的漫画")] Comic comic)
        {
            if (comic.IsReverseList == true) return comic.Chapters.LastOrDefault();
            else return comic.Chapters.FirstOrDefault();
        }

        [KernelFunction, Description("获得特定漫画的最后一个或最新章节")]
        [return: Description("获取到的章节（可能没有）")]
        public Chapter? GetLastChapterAsync(
                                                        [Description("特定的漫画")] Comic comic)
        {
            if (comic.IsReverseList == true) return comic.Chapters.FirstOrDefault();
            else return comic.Chapters.LastOrDefault();
        }

        [KernelFunction, Description("获得特定漫画的最后浏览章节")]
        [return: Description("获取到的章节（可能没有）")]
        public Chapter? GetLastReadChapterAsync(
                                                                   [Description("特定的漫画")] Comic comic)
        {
            if (comic.LastReadedChapterIndex == -1) return null;
            return comic.Chapters.ToList()[comic.LastReadedChapterIndex];
        }

        //辅助功能

        [KernelFunction, Description("查找失败或删除失败（结果为空或为false）提示用户")]
        public async Task<bool> FailTaskAsync()
        {
            await Toast.Make("任务失败，请检查输入是否有误！").Show();
            return false;
        }
    }
}