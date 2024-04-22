using BrilliantSee.Models;
using BrilliantSee.Models.Chapters;
using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Enums;
using BrilliantSee.Models.Sources;
using BrilliantSee.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.ViewModels
{
    public partial class SearchViewModel : ObservableObject
    {
        private readonly SourceService _sourceService;

        public readonly DBService _db;

        private readonly AIService _ai;

        /// <summary>
        /// 储存搜索结果集合
        /// </summary>
        public ObservableCollection<Obj> Objs { get; set; } = new();

        public ObservableCollection<Obj> ObjsOnDisplay { get; set; } = new();

        public SourceCategory CurrentCategory { get; set; } = SourceCategory.Comic;

        /// <summary>
        /// 是否正在获取结果
        /// </summary>
        [ObservableProperty]
        private bool _isGettingResult;

        [ObservableProperty]
        private bool _isSourceListVisible = false;

        private string Keyword = string.Empty;

        [ObservableProperty]
        private List<Source> _sources = new();

        public ObservableCollection<Group<Source>> SourceGroups { get; set; } = new ObservableCollection<Group<Source>>();

        private List<SettingItem> SettingItems { get; set; } = new();

        public SearchViewModel(SourceService sourceService, DBService db)
        {
            _sourceService = sourceService;
            _db = db;
            _ai = MauiProgram.servicesProvider!.GetRequiredService<AIService>();
            Sources = _sourceService.GetSources();
            SourceGroups.Add(new Group<Source>("漫画", Sources.Where(s => s.Category == SourceCategory.Comic).ToList()));
            SourceGroups.Add(new Group<Source>("动漫", Sources.Where(s => s.Category == SourceCategory.Video).ToList()));
            _ = InitSettingsAsync();
            if (_ai.hasModel)
            {
                _ai.RemovePlugins();
                _ai.ImportPlugins(new Services.Plugins.SearchPlugins(_db, _sourceService));
            }
        }

        public async Task InitSettingsAsync()
        {
            SettingItems = await _db.GetSettingItemsAsync("Source");
            foreach (var source in Sources)
            {
                source.IsSelected = SettingItems.First(s => s.Name == source.Name).Value == "IsSelected";
            }
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
                Keyword = keyword;
                IsGettingResult = true;
                IsSourceListVisible = false;
                Objs.Clear();
                await _sourceService.SearchAsync(keyword, Objs, "Init");
                if (Objs.Count == 0) { _ = Toast.Make("搜索结果为空，换一个图源试试吧").Show(); }
                IsGettingResult = false;
            }
            else
            {
                _ = Toast.Make("请选择至少一个图源").Show();
                return;
            }
        }

        public async Task GetMoreAsync()
        {
            _ = Toast.Make("正在加载更多结果").Show();
            var count = Objs.Count;
            IsGettingResult = true;
            IsSourceListVisible = false;
            await _sourceService.SearchAsync(Keyword, Objs, "More");
            var message = Objs.Count == count ? "没有更多结果了" : $"加载了{Objs.Count - count}个结果";
            _ = Toast.Make(message).Show();
            IsGettingResult = false;
        }

        /// <summary>
        /// 打开漫画详情页并传递漫画对象
        /// </summary>
        /// <param name="obj">指定打开的实体</param>
        /// <returns></returns>
        [RelayCommand]
        private async Task OpenObjAsync(Obj obj)
        {
            IsSourceListVisible = false;
            obj.Items = new List<Chapter>();
            var page = obj.SourceCategory == SourceCategory.Comic ? "DetailPage" : "VideoPage";
            await Shell.Current.GoToAsync(page, new Dictionary<string, object> { { "Obj", obj } });
        }

        [RelayCommand]
        private async Task ChangeIsSelectedAsync(Source source)
        {
            source.IsSelected = !source.IsSelected;
            var item = SettingItems.First(s => s.Name == source.Name);
            item!.Value = source.IsSelected ? "IsSelected" : "NotSelected";
            await _db.UpdateSettingItemAsync(item);
        }

        [RelayCommand]
        private void ChangeSourceListVisible()
        {
            IsSourceListVisible = !IsSourceListVisible;
        }
    }
}