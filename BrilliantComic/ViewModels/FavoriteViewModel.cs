using BrilliantComic.Models.Comics;
using BrilliantComic.Models.Enums;
using BrilliantComic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.ViewModels
{
    public partial class FavoriteViewModel : ObservableObject
    {
        public readonly DBService _db;

        /// <summary>
        /// 是否正在获取结果
        /// </summary>
        [ObservableProperty]
        private bool _isGettingResult;

        private bool IsRefresh { get; set; } = false;

        /// <summary>
        /// 储存收藏漫画集合
        /// </summary>
        public ObservableCollection<Comic> Comics { get; set; } = new();

        /// <summary>
        /// 加载收藏漫画
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadFavoriteComicAsync()
        {
            Comics.Clear();
            IsGettingResult = true;
            var comics = await _db.GetComicsAsync(DBComicCategory.Favorite);
            comics.Reverse();
            foreach (var item in comics)
            {
                Comics.Add(item);
            }
            if (!IsRefresh)
            {
                IsGettingResult = false;
            }
        }

        public FavoriteViewModel(DBService db)
        {
            _db = db;
        }

        /// <summary>
        /// 导航到漫画详情页并传递漫画对象
        /// </summary>
        /// <param name="comic">指定打开的漫画</param>
        /// <returns></returns>
        [RelayCommand]
        private async Task OpenComicAsync(Comic comic)
        {
            await Shell.Current.GoToAsync("DetailPage", new Dictionary<string, object> { { "Comic", comic } });
        }

        /// <summary>
        /// 检查收藏漫画是否有更新
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task CheckForUpdatedAsync() =>
            await Task.Run(async () =>
                {
                    _ = MainThread.InvokeOnMainThreadAsync(() => { IsGettingResult = true; IsRefresh = true; });
                    var comics = await _db.GetComicsAsync(DBComicCategory.Favorite);
                    foreach (var item in comics)
                    {
                        if (!item.IsUpdate)
                        {
                            await item.GetHtmlAsync();
                            var lastestChapterName = item.GetLastestChapterName();
                            if (lastestChapterName is not null && lastestChapterName != item.LastestChapterName)
                            {
                                item.IsUpdate = true;
                                await _db.SaveComicAsync(item, DBComicCategory.Favorite);
                                MainThread.BeginInvokeOnMainThread(() => _ = OnLoadFavoriteComicAsync());
                            }
                        }
                    }
                    _ = MainThread.InvokeOnMainThreadAsync(() => { IsRefresh = false; IsGettingResult = false; });
                });
    }
}