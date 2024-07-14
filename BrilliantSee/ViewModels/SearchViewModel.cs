using BrilliantSee.Models;
using BrilliantSee.Models.Items;
using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Enums;
using BrilliantSee.Models.Sources;
using BrilliantSee.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using BrilliantSee.Models.Sources.ComicSources;
using CommunityToolkit.Maui.Core.Extensions;

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
        [ObservableProperty]
        public SourceCategory _currentCategory = SourceCategory.All;

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

        public ObservableCollection<string> HotWord { get; set; } = new();

        public ObservableCollection<string> SearchRecord { get; set; } = new();

        [ObservableProperty]
        public bool _isShowSearchResult = false;

        [ObservableProperty]
        public bool _isShowSearchMessage = true;

        [ObservableProperty]
        public bool _isShowRecord = false;

        public ObservableCollection<Obj> Recommand { get; set; } = new();

        private string Keyword = string.Empty;

        public SearchViewModel(SourceService sourceService, DBService db, MessageService ms)
        {
            _db = db;
            _sourceService = sourceService;
            _ms = ms;
            //_ai = MauiProgram.servicesProvider!.GetRequiredService<AIService>();

            Sources = _sourceService.GetSources();
            _ = InitSettingsAsync();
            _ = GetHotWordAsync();
            _ = GetRecommandObjsAsync();
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
            SettingItems = await _db.GetSettingItemsAsync((int)SettingItemCategory.Source);
            foreach (var source in Sources)
            {
                source.IsSelected = SettingItems.First(s => s.Name == source.Name).ValueInt == 1;
            }
            var searchItems = await _db.GetSettingItemsAsync((int)SettingItemCategory.SearchRecord);
            var item = searchItems.First();
            SettingItems.Add(item);
            if (string.IsNullOrWhiteSpace(item.ValueString) || string.IsNullOrEmpty(item.ValueString)) return;
            foreach (var record in item.ValueString.Split("$$$"))
            {
                SearchRecord.Add(record);
            }
            IsShowRecord = true;
        }

        private async Task UpdateSearchRecordAsync(String newRecord)
        {
            IsShowRecord = true;
            var newRecordIndex = SearchRecord.IndexOf(newRecord);
            if (newRecordIndex != -1)
            {
                SearchRecord.Move(newRecordIndex, 0);
            }
            else
            {
                SearchRecord.Insert(0, newRecord);
            }

            var item = SettingItems.First(s => s.Category == (int)SettingItemCategory.SearchRecord);
            item.ValueString = string.Join("$$$", SearchRecord);
            await _db.UpdateSettingItemAsync(item);
        }

        public async Task ClearSearchRecordAsync()
        {
            SearchRecord.Clear();
            var item = SettingItems.First(s => s.Category == (int)SettingItemCategory.SearchRecord);
            item.ValueString = string.Empty;
            await _db.UpdateSettingItemAsync(item);
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
            await UpdateSearchRecordAsync(keyword);
            _ms.WriteMessage("正在搜索...");
            IsShowSearchMessage = false;
            IsShowSearchResult = true;
            IsGettingResult = true;
            IsSourceListVisible = false;

            Keyword = keyword;
            AllObjs.Clear();

            List<Task> tasks = new List<Task>();
            foreach (var key in ObjContainers.Keys)
            {
                ObjContainers[key].Clear();
                tasks.Add(Task.Run(async () => await _sourceService.SearchAsync(keyword, AllObjs, ObjContainers[key], "Init", key)));
            }
            await Task.WhenAll(tasks);

            CurrentObjsCount = ObjContainers[CurrentCategory].Count;
            IsGettingResult = false;
            var message = Comics.Count + Videos.Count + Novels.Count == 0 ? "搜索结果为空，换其他源试试吧" : $"搜索到{Novels.Count}部小说，{Comics.Count}部漫画，{Videos.Count}部动漫";
            _ms.WriteMessage(message);
        }

        /// <summary>
        /// 获取更多结果
        /// </summary>
        /// <returns></returns>
        public async Task GetMoreAsync()
        {
            _ms.WriteMessage("正在加载更多结果");
            IsGettingResult = true;
            IsSourceListVisible = false;

            var count = Comics.Count + Videos.Count + Novels.Count;

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

            IsGettingResult = false;
            CurrentObjsCount = ObjContainers[CurrentCategory].Count;
            var message = Comics.Count + Videos.Count + Novels.Count > count ? $"加载了{Comics.Count + Videos.Count + Novels.Count - count}个结果" : "没有更多结果了";
            _ms.WriteMessage(message);
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
            item!.ValueInt = source.IsSelected ? 1 : 0;
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

        [RelayCommand]
        private async Task GetRecommandObjsAsync()
        {
            var objs = await _db.GetObjsAsync(DBObjCategory.Recommend, SourceCategory.Comic);
            if (objs.Count == 0)
            {
                var Objs = await new BaoziSource().GetRecommandAsync();
                foreach (var obj in Objs)
                {
                    obj.Category = DBObjCategory.Recommend;
                    obj.SourceCategory = SourceCategory.Comic;
                    objs.Add(obj);
                    await _db.SaveObjAsync(obj, DBObjCategory.Recommend);
                }
            }
            //随机获取六个不重复推荐漫画
            Recommand.Clear();
            var random = new Random();
            var indexs = new List<int>();
            while (indexs.Count < 6)
            {
                var index = random.Next(0, objs.Count);
                if (!indexs.Contains(index))
                {
                    indexs.Add(index);
                    Recommand.Add(objs[index]);
                }
            }
        }

        /// <summary>
        /// 获取热词
        /// </summary>
        private async Task GetHotWordAsync()
        {
            //设置为每天获取一次
            var items = await _db.GetSettingItemsAsync((int)SettingItemCategory.HotSearch);
            var item = items.First();
            if (string.IsNullOrEmpty(item.ValueString) || string.IsNullOrWhiteSpace(item.ValueString) || item.ValueString.Split("$$$$")[0] != DateTime.Now.ToString("yyyy-MM-dd"))
            {
                //获取热词
                var words = await new BaoziSource().GetHotWordAsync();
                foreach (var word in words)
                {
                    HotWord.Add(word);
                }
                item.ValueString = DateTime.Now.ToString("yyyy-MM-dd") + "$$$$" + string.Join("$$$", HotWord);
                await _db.UpdateSettingItemAsync(item);
            }
            else
            {
                foreach (var word in item.ValueString.Split("$$$$")[1].Split("$$$"))
                {
                    HotWord.Add(word);
                }
            }
        }
    }
}