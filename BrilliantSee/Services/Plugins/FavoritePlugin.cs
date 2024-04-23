using Microsoft.SemanticKernel;
using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using System.Collections.ObjectModel;

namespace BrilliantSee.Services.Plugins
{
    public sealed class FavoritePlugin
    {
        public DBService _db;

        public FavoritePlugin(DBService db)
        {
            _db = db;
        }

        //删除功能

        [KernelFunction, Description("取消收藏指定漫画")]
        [return: Description("是否成功取消收藏")]
        public async Task<bool> CancelFavoriteComicAsync(
            [Description("指定的漫画")] Obj comic)
        {
            var result = await _db.DeleteObjAsync(comic, comic.Category);
            if (result > -1)
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
            var Comics = await _db.GetObjsAsync(DBObjCategory.Favorite, SourceCategory.Comic);
            foreach (var item in Comics)
            {
                await _db.DeleteObjAsync(item, item.Category);
            }
            Comics = await _db.GetObjsAsync(DBObjCategory.Favorite, SourceCategory.Comic);
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
        [KernelFunction, Description("打开指定的收藏漫画")]
        public async Task OpenComicAsync(
               [Description("要打开的漫画")] Obj comic)
        {
            if (comic is not null)
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.GoToAsync("DetailPage", new Dictionary<string, object> { { "Comic", comic } });
                });
        }

        [KernelFunction, Description("打开某个页面，例如'打开搜索页',仅能够打开以下页面：历史记录页(HistoryPage)，搜索页(SearchPage)")]
        public async Task OpenPageAsync(
                                  [Description("要打开的页面,输入需要为对应的括号内的英文，不能多不能少")] string page)
        {
            await MainThread.InvokeOnMainThreadAsync(async () => { await Shell.Current.GoToAsync(page); });
        }

        //查找功能
        [KernelFunction, Description("查找指定的收藏漫画")]
        [return: Description("查到的漫画（可能没有）")]
        public async Task<Obj>? FindFavoriteAsync(
                          [Description("要查找的漫画名称")] string name)
        {
            var Comics = await _db.GetObjsAsync(DBObjCategory.Favorite, SourceCategory.Comic);
            //模糊查找漫画名取第一个
            var comic = Comics.Where(c => c.Name.Contains(name)).FirstOrDefault();
            return comic!;
        }
    }
}