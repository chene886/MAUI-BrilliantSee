using BrilliantSee.Models;
using BrilliantSee.Models.Items;
using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Enums;
using BrilliantSee.Models.Sources;
using BrilliantSee.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BrilliantSee.ViewModels
{
    public partial class SearchViewModel : ObservableObject
    {
        private readonly SourceService _sourceService;
        private readonly MessageService _ms;
        private readonly DBService _db;
        //private readonly AIService _ai;

        /// <summary>
        /// 当前选中的类别
        /// </summary>
        public SourceCategory CurrentCategory { get; set; } = SourceCategory.All;

        /// <summary>
        /// 当前类别的对象数量
        /// </summary>
        public int CurrentObjsCount { get; set; } = 0;

        /// <summary>
        /// 所有类别的对象集合
        /// </summary>
        public ObservableCollection<Obj> AllObjs { get; set; } = new();

        /// <summary>
        /// 小说类别的对象集合
        /// </summary>
        private ObservableCollection<Obj> Novels { get; set; } = new();

        /// <summary>
        /// 漫画类别的对象集合
        /// </summary>
        private ObservableCollection<Obj> Comics { get; set; } = new();

        /// <summary>
        /// 动漫类别的对象集合
        /// </summary>
        private ObservableCollection<Obj> Videos { get; set; } = new();

        /// <summary>
        /// 类别与类别对象集合的映射
        /// </summary>
        private Dictionary<SourceCategory, ObservableCollection<Obj>> ObjContainers { get; set; } = new();

        /// <summary>
        /// 是否正在获取结果
        /// </summary>
        [ObservableProperty]
        private bool _isGettingResult;

        /// <summary>
        /// 是否显示源列表
        /// </summary>
        [ObservableProperty]
        private bool _isSourceListVisible = false;

        /// <summary>
        /// 源列表
        /// </summary>
        [ObservableProperty]
        private List<Source> _sources = new();

        /// <summary>
        /// 源列表组
        /// </summary>
        public ObservableCollection<Group<Source>> SourceGroups { get; set; } = new ObservableCollection<Group<Source>>();

        /// <summary>
        /// 数据库源设置项
        /// </summary>
        private List<SettingItem> SettingItems { get; set; } = new();

        private string Keyword = string.Empty;

        public SearchViewModel(SourceService sourceService, DBService db, MessageService ms)
        {
            _db = db;
            _sourceService = sourceService;
            _ms = ms;
            //_ai = MauiProgram.servicesProvider!.GetRequiredService<AIService>();

            Sources = _sourceService.GetSources();
            _ = InitSettingsAsync();
            //if (_ai.hasModel)
            //{
            //    _ai.RemovePlugins();
            //    _ai.ImportPlugins(new Services.Plugins.SearchPlugins(_db, _sourceService));
            //}

            ObjContainers.Add(SourceCategory.All, AllObjs);
            ObjContainers.Add(SourceCategory.Novel, Novels);
            ObjContainers.Add(SourceCategory.Comic, Comics);
            ObjContainers.Add(SourceCategory.Video, Videos);
            SourceGroups.Add(new Group<Source>("小说", Sources.Where(s => s.Category == SourceCategory.Novel).ToList()));
            SourceGroups.Add(new Group<Source>("漫画", Sources.Where(s => s.Category == SourceCategory.Comic).ToList()));
            SourceGroups.Add(new Group<Source>("动漫", Sources.Where(s => s.Category == SourceCategory.Video).ToList()));
        }

        /// <summary>
        /// 初始化源设置项
        /// </summary>
        /// <returns></returns>
        private async Task InitSettingsAsync()
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
        /// <param name="keyword">搜索关键词</param>
        /// <returns></returns>
        [RelayCommand]
        private async Task SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                _ms.WriteMessage("请输入正确的关键词");
                return;
            }
            var hasSourceSelected = Sources.Where(s => s.IsSelected == true).Count() > 0;
            if (hasSourceSelected)
            {
                Keyword = keyword;
                IsGettingResult = true;
                IsSourceListVisible = false;
                AllObjs.Clear();
                List<Task> tasks = new List<Task>();
                foreach (var key in ObjContainers.Keys)
                {
                    ObjContainers[key].Clear();
                    tasks.Add(Task.Run(async () => await _sourceService.SearchAsync(keyword, AllObjs, ObjContainers[key], "Init", key)));
                }
                await Task.WhenAll(tasks);
                var count = Comics.Count + Videos.Count + Novels.Count;
                if (count == 0) { _ms.WriteMessage("搜索结果为空，换其他源试试吧"); }
                else { _ms.WriteMessage($"搜索到{Novels.Count}部小说，{Comics.Count}部漫画，{Videos.Count}部动漫"); }
                CurrentObjsCount = count;
                IsGettingResult = false;
            }
            else
            {
                _ms.WriteMessage("请选择至少一个图源");
                return;
            }
        }

        /// <summary>
        /// 获取更多结果
        /// </summary>
        /// <returns></returns>
        public async Task GetMoreAsync()
        {
            _ms.WriteMessage("正在加载更多结果");
            var count = Comics.Count + Videos.Count + Novels.Count;
            IsGettingResult = true;
            IsSourceListVisible = false;
            if (CurrentCategory == SourceCategory.All)
            {
                List<Task> tasks = new List<Task>();
                foreach (var key in ObjContainers.Keys)
                {
                    tasks.Add(Task.Run(async () => await _sourceService.SearchAsync(Keyword, AllObjs, ObjContainers[key], "More", key)));
                }
                await Task.WhenAll(tasks);
            }
            else await _sourceService.SearchAsync(Keyword, AllObjs, ObjContainers[CurrentCategory], "More", CurrentCategory);
            var message = Comics.Count + Videos.Count + Novels.Count > count ? $"加载了{Comics.Count + Videos.Count + Novels.Count - count}个结果" : "没有更多结果了";
            _ms.WriteMessage(message);
            IsGettingResult = false;
        }

        /// <summary>
        /// 打开详情页或视频页并传递实体
        /// </summary>
        /// <param name="obj">指定打开的实体</param>
        /// <returns></returns>
        [RelayCommand]
        private async Task OpenObjAsync(Obj obj)
        {
            IsSourceListVisible = false;
            var page = obj.SourceCategory == SourceCategory.Video ? "VideoPage" : "DetailPage";
            await Shell.Current.GoToAsync(page, new Dictionary<string, object> { { "Obj", obj } });
        }

        /// <summary>
        /// 更改源选择状态并更新数据库
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task ChangeIsSelectedAsync(Source source)
        {
            source.IsSelected = !source.IsSelected;
            var item = SettingItems.First(s => s.Name == source.Name);
            item!.Value = source.IsSelected ? "IsSelected" : "NotSelected";
            await _db.UpdateSettingItemAsync(item);
        }

        /// <summary>
        /// 更改源列表可见性
        /// </summary>
        [RelayCommand]
        private void ChangeSourceListVisible()
        {
            IsSourceListVisible = !IsSourceListVisible;
        }

        /// <summary>
        /// 更改当前类别
        /// </summary>
        /// <param name="category"></param>
        public void ChangeCurrentCategory(SourceCategory category)
        {
            CurrentCategory = category;
            CurrentObjsCount = ObjContainers[category].Count;
        }

        /// <summary>
        /// 获取当前类别的对象集合
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Obj> GetObjsOnDisplay()
        {
            return ObjContainers[CurrentCategory];
        }
    }
}