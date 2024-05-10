using BrilliantSee.Models.Items;
using BrilliantSee.Models.Enums;
using BrilliantSee.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using BrilliantSee.Models;

namespace BrilliantSee.ViewModels
{
    public partial class BrowseViewModel : ObservableObject, IQueryAttributable
    {
        private readonly DBService _db;
        private readonly MessageService _ms;
        //private readonly AIService _ai;

        /// <summary>
        /// 当前章节
        /// </summary>
        [ObservableProperty]
        private Item? _chapter;

        ///// <summary>
        ///// 当前章节在已加载章节集合中的索引
        ///// </summary>
        //public int _currentChapterIndex = 0;

        ///// <summary>
        ///// 已加载章节集合
        ///// </summary>
        //[ObservableProperty]
        //public List<Item> _loadedChapter = new List<Item>();

        /// <summary>
        /// 当前章节图片集合
        /// </summary>
        public ObservableCollection<string> Images { get; set; } = new();

        /// <summary>
        /// 是否正在加载
        /// </summary>
        [ObservableProperty]
        public bool _isLoading = false;

        /// <summary>
        /// 是否显示刷新按钮
        /// </summary>
        [ObservableProperty]
        public bool _isShowRefresh = false;

        /// <summary>
        /// 是否显示加载下一章节按钮
        /// </summary>
        [ObservableProperty]
        public bool _isShowButton = false;

        /// <summary>
        /// 加载下一章节按钮内容
        /// </summary>
        [ObservableProperty]
        public string _buttonContent = "点击加载下一话";

        /// <summary>
        /// 滚动到顶部事件
        /// </summary>
        public event Action ScrollToTop = delegate { };

        ///// <summary>
        ///// 当前页码
        ///// </summary>
        //[ObservableProperty]
        //public int _currentPageNum = 1;

        /// <summary>
        /// 定时器
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// 当前时间
        /// </summary>
        public string CurrentTime => DateTime.Now.ToString("HH:mm");

        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public double ScreenWidth { get; set; } = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

        //public int CurrentChapterIndex
        //{
        //    get => _currentChapterIndex;
        //    set
        //    {
        //        if (_currentChapterIndex != value)
        //        {
        //            _currentChapterIndex = value;
        //            OnPropertyChanged(nameof(CurrentChapterIndex));
        //        }
        //        Chapter = LoadedChapter[value];
        //    }
        //}

        public BrowseViewModel(DBService db, MessageService ms)
        {
            _db = db;
            _ms = ms;
            //_ai = MauiProgram.servicesProvider!.GetRequiredService<AIService>();
            //if (_ai.hasModel)
            //{
            //    _ai.RemovePlugins();
            //    _ai.ImportPlugins(new Services.Plugins.BrowsePlugins(_db));
            //}
            _timer = new Timer((o) => { OnPropertyChanged(nameof(CurrentTime)); }, null, (60 - DateTime.Now.Second) * 1000, 60000);
        }

        /// <summary>
        /// 获取通过导航传递的参数
        /// </summary>
        /// <param name="query">保存传递数据的字典</param>
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Chapter = (query["Chapter"] as Item)!;
            if (Chapter.Url == "")
            {
                return;
            }
            IsLoading = true;
            _ = Chapter.Obj.ChangeLastReadedItemIndex(Chapter.Index, _db);
            await LoadChapterResourcesAsync(Chapter);
            OnPropertyChanged(nameof(Chapter));
            IsLoading = false;
            if (Images.Any() || Chapter.NovelContent != string.Empty)
            {
                IsShowButton = true;
            }
        }

        /// <summary>
        /// 加载章节图片或小说内容
        /// </summary>
        /// <param name="chapter">指定的章节</param>
        /// <returns></returns>
        private async Task LoadChapterResourcesAsync(Item chapter)
        {
            if (chapter.PicUrls.Count == 0 && chapter.NovelContent == string.Empty)
            {
                try
                {
                    await chapter.GetResourcesAsync();
                }
                catch (Exception e)
                {
                    if (e.Message == "请求失败") _ms.WriteMessage(e.Message);
                    else _ms.WriteMessage("好像出了点小问题，用浏览器打开试试吧");
                }
            }
            if (chapter.Obj.SourceCategory == SourceCategory.Comic)
            {
                Images.Clear();
                foreach (var url in chapter.PicUrls)
                {
                    Images.Add(url);
                }
            }
            ButtonContent = chapter!.Index == chapter.Obj.ItemCount - 1 ? "已是最新一话" : "点击加载下一话";
            _ = Task.Run(() => chapter.Obj.PreLoadAsync(chapter, _db));
        }

        /// <summary>
        /// 更新章节，加载资源
        /// </summary>
        /// <param name="flag">指定上一话或下一话</param>
        /// <returns></returns>
        public async Task<bool> UpdateChapterAsync(string flag)
        {
            var newChapter = Chapter!.Obj.GetNewItem(Chapter, flag);
            if (newChapter is null)
            {
                return false;
            }
            try
            {
                await LoadChapterResourcesAsync(newChapter);
            }
            catch { }
            Chapter = newChapter;
            _ = Chapter.Obj.ChangeLastReadedItemIndex(Chapter.Index, _db);
            return true;
        }

        /// <summary>
        /// 加载新章节
        /// </summary>
        /// <param name="flag">上一章或下一章</param>
        /// <returns></returns>
        [RelayCommand]
        public async Task LoadNewChapterAsync(string flag)
        {
            var result = false;

            var isNext = flag == "Next";
            if (isNext) IsLoading = true;
            _ms.WriteMessage("正在加载...");
            result = await UpdateChapterAsync(flag);
            if (result)
            {
                _ms.WriteMessage("加载成功");
                ScrollToTop.Invoke();
            }
            else
            {
                var unSuccess = flag == "Next" ? "已是最新一话" : "已是第一话";
                _ms.WriteMessage(unSuccess);
            }
            if (isNext) IsLoading = false;
            IsShowRefresh = false;
        }
    }
}