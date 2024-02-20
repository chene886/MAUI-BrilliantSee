using BrilliantComic.Models.Chapters;
using BrilliantComic.Models.Comics;
using BrilliantComic.Models.Sources;
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
    public partial class SearchViewModel : ObservableObject
    {
        private readonly SourceService _sourceService;

        /// <summary>
        /// 搜索漫画集合
        /// </summary>
        public ObservableCollection<Comic> Comics { get; set; } = new();

        [ObservableProperty]
        private bool _isGettingResult;

        public SearchViewModel(SourceService sourceService)
        {
            _sourceService = sourceService;
        }

        /// <summary>
        /// 搜索漫画
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task SearchAsync(string keyword)
        {
            IsGettingResult = true;

            Comics.Clear();
            await _sourceService.SearchAsync(keyword, Comics);

            IsGettingResult = false;
        }

        /// <summary>
        /// 打开漫画详情页
        /// </summary>
        /// <param name="comic"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task OpenComicAsync(Comic comic)
        {
            comic.Chapters = new List<Chapter>();
            await Shell.Current.GoToAsync("DetailPage", new Dictionary<string, object> { { "Comic", comic } });
        }
    }
}