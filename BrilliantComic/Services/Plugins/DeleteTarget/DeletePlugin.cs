using BrilliantComic.Models.Comics;
using BrilliantComic.Models.Enums;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Services.Plugins.DeleteTarget
{
    public sealed class DeletePlugin
    {
        private readonly DBService _db;

        public DeletePlugin(DBService db)
        {
            _db = db;
        }

        [KernelFunction, Description("删除指定漫画历史记录或取消收藏指定漫画")]
        [return: Description("是否成功删除或是否成功取消收藏")]
        private async Task<bool> DeleteComicAsync(
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
        private async Task<bool> ClearHistoryAsync()
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
        private async Task<bool> ClearFavoriteAsync()
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
    }
}