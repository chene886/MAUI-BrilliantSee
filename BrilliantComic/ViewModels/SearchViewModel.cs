﻿using BrilliantComic.Models.Chapters;
using BrilliantComic.Models.Comics;
using BrilliantComic.Models.Sources;
using BrilliantComic.Services;
using CommunityToolkit.Maui.Alerts;
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
        /// 储存搜索漫画的结果集合
        /// </summary>
        public ObservableCollection<Comic> Comics { get; set; } = new();

        /// <summary>
        /// 是否正在获取结果
        /// </summary>
        [ObservableProperty]
        private bool _isGettingResult;

        [ObservableProperty]
        private bool _isSourceListVisible = false;

        [ObservableProperty]
        private List<ISource> _sources = new();

        public SearchViewModel(SourceService sourceService)
        {
            _sourceService = sourceService;
            Sources = _sourceService.GetSources();
        }

        /// <summary>
        /// 搜索漫画
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                _ = Toast.Make("请输入正确的关键词").Show();
                return;
            }
            var hasSourceSelected = Sources.Where(s => s.IsSelected == true).Count() > 0;
            if (hasSourceSelected)
            {
                IsGettingResult = true;
                IsSourceListVisible = false;
                Comics.Clear();
                await _sourceService.SearchAsync(keyword, Comics);
                if (Comics.Count == 0) { _ = Toast.Make("搜索结果为空，换一个\n图源试试吧").Show(); }
                IsGettingResult = false;
            }
            else
            {
                _ = Toast.Make("请选择至少一个图源").Show();
                return;
            }
        }

        /// <summary>
        /// 打开漫画详情页并传递漫画对象
        /// </summary>
        /// <param name="comic">指定打开的漫画</param>
        /// <returns></returns>
        [RelayCommand]
        private async Task OpenComicAsync(Comic comic)
        {
            IsSourceListVisible = false;
            comic.Chapters = new List<Chapter>();
            await Shell.Current.GoToAsync("DetailPage", new Dictionary<string, object> { { "Comic", comic } });
        }

        [RelayCommand]
        private void ChangeIsSelected(ISource source)
        {
            source.IsSelected = !source.IsSelected;
        }

        [RelayCommand]
        private void ChangeSourceListVisible()
        {
            IsSourceListVisible = !IsSourceListVisible;
        }
    }
}