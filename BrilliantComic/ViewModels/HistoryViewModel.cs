using BrilliantComic.Models.Comics;
using BrilliantComic.Models.Enums;
using BrilliantComic.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.ViewModels
{
    public partial class HistoryViewModel : ObservableObject
    {
        public readonly DBService _db;
        public readonly AIService _ai;

        /// <summary>
        /// 是否正在获取结果
        /// </summary>
        [ObservableProperty]
        private bool _isGettingResult;

        /// <summary>
        /// 储存历史漫画的集合
        /// </summary>
        public ObservableCollection<Comic> Comics { get; set; } = new();

        /// <summary>
        /// 加载历史漫画
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadHistoryComicAsync()
        {
            Comics.Clear();
            IsGettingResult = true;
            var comics = await _db.GetComicsAsync(Models.Enums.DBComicCategory.History);
            comics.Reverse();
            foreach (var item in comics)
            {
                Comics.Add(item);
            }
            IsGettingResult = false;
        }

        public HistoryViewModel(DBService db)
        {
            _db = db;
            _ai = MauiProgram.servicesProvider!.GetRequiredService<AIService>();
        }

        /// <summary>
        /// 清空历史漫画
        /// </summary>
        /// <returns></returns>
        public async Task ClearHistoryComicsAsync()
        {
            if (Comics.Count == 0)
            {
                _ = Toast.Make("暂无历史记录").Show();
                return;
            }
            foreach (var item in Comics)
            {
                await _db.DeleteComicAsync(item, item.Category);
            }
            Comics.Clear();
            _ = Toast.Make("历史记录已清空").Show();
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

        [RelayCommand]
        private async Task ClearAsync(Comic comic)
        {
            await _db.DeleteComicAsync(comic, comic.Category);
            Comics.Remove(comic);
        }
    }
}