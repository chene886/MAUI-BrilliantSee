using BrilliantComic.Models.Comics;
using BrilliantComic.Models.Enums;
using CommunityToolkit.Maui.Alerts;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Services.Plugins
{
    public sealed class HistoryPlugins
    {
        public DBService _db;

        public HistoryPlugins(DBService db)
        {
            _db = db;
        }

        //删除功能

        [KernelFunction, Description("删除指定漫画的历史记录")]
        [return: Description("是否成功删除")]
        public async Task<bool> DeleteComicAsync(
    [Description("指定的漫画")] Comic comic)
        {
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

        //打开功能
        [KernelFunction, Description("打开指定的历史记录")]
        public async Task OpenComicAsync(
               [Description("要打开的漫画")] Comic comic)
        {
            if (comic is not null)
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.GoToAsync("DetailPage", new Dictionary<string, object> { { "Comic", comic } });
                });
        }

        [KernelFunction, Description("打开某个页面，例如'打开搜索页',仅能够打开以下页面：收藏页(FavoritePage)，系统设置页(SettingPage)")]
        public async Task OpenPageAsync(
                          [Description("要打开的页面,输入需要为对应的括号内的英文，不能多不能少")] string page)
        {
            await MainThread.InvokeOnMainThreadAsync(async () => { await Shell.Current.GoToAsync(page); });
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
    }
}