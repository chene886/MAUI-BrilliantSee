using BrilliantComic.Models.Chapters;
using BrilliantComic.Models.Enums;
using BrilliantComic.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.ViewModels
{
    public partial class BrowseViewModel : ObservableObject, IQueryAttributable
    {
        /// <summary>
        /// 当前章节
        /// </summary>
        [ObservableProperty]
        private Chapter? _chapter;

        /// <summary>
        /// 当前章节在已加载章节集合中的索引
        /// </summary>
        public int _currentChapterIndex = 0;

        /// <summary>
        /// 截止当前章节图片数量
        /// </summary>
        public int utillCrrentChapterImageCount = 0;

        /// <summary>
        /// 已加载章节集合
        /// </summary>
        [ObservableProperty]
        public List<Chapter> _loadedChapter = new List<Chapter>();

        /// <summary>
        /// 已加载章节图片集合
        /// </summary>
        [ObservableProperty]
        public ObservableCollection<ImageSource> _images = new ObservableCollection<ImageSource>();

        /// <summary>
        /// 是否正在加载
        /// </summary>
        public bool IsLoading { get; set; } = false;

        [ObservableProperty]
        public bool _isShowRefresh = false;

        /// <summary>
        /// 当前界面第一个item的索引
        /// </summary>
        private int crrentViewFirstItemIndex = 0;

        /// <summary>
        /// 当前界面最后一个item的索引
        /// </summary>
        private int crrentViewLastItemIndex = 1;

        /// <summary>
        /// 位于当前界面正中的item的索引
        /// </summary>
        private int crrentViewCentricItemIndex = 0;

        /// <summary>
        /// 当前页码
        /// </summary>
        [ObservableProperty]
        public int _currentPageNum = 1;

        /// <summary>
        /// 定时器
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// 当前时间
        /// </summary>
        public string CurrentTime => DateTime.Now.ToString("HH:mm");

        private readonly DBService _db;

        public int CurrentChapterIndex
        {
            get => _currentChapterIndex;
            set
            {
                if (_currentChapterIndex != value)
                {
                    _currentChapterIndex = value;
                    OnPropertyChanged(nameof(CurrentChapterIndex));
                    var newChapter = LoadedChapter[value];
                    if (Chapter != newChapter)
                    {
                        if (Chapter!.Index > newChapter.Index) utillCrrentChapterImageCount -= Chapter.PageCount;
                        else utillCrrentChapterImageCount += newChapter.PageCount;
                        Chapter!.IsSpecial = false;
                        Chapter = LoadedChapter[value];
                        Chapter.IsSpecial = true;
                    }
                    else
                    {
                        utillCrrentChapterImageCount += LoadedChapter[0].PageCount;
                    }
                }
            }
        }

        public BrowseViewModel(DBService db)
        {
            _db = db;
            _timer = new Timer((o) => { OnPropertyChanged(nameof(CurrentTime)); }, null, (60 - DateTime.Now.Second) * 1000, 60000);
        }

        /// <summary>
        /// 获取通过导航传递的参数
        /// </summary>
        /// <param name="query">保存传递数据的字典</param>
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Chapter = (query["Chapter"] as Chapter)!;
            if (Chapter.Url == "")
            {
                return;
            }
            var LastIndex = Chapter.Comic.LastReadedChapterIndex;
            if (Chapter.Index != LastIndex)
            {
                if (LastIndex != -1)
                {
                    var position = LastIndex;
                    if (Chapter.Comic.IsReverseList) position = Chapter.Comic.Chapters.Count() - LastIndex - 1;
                    Chapter.Comic.Chapters.ToList()[position].IsSpecial = false;
                }
                _ = StoreLastReadedChapterIndex();
                Chapter.IsSpecial = true;
            }
            await LoadChapterPicAsync(Chapter, "Init");
            OnPropertyChanged(nameof(Chapter));
        }

        /// <summary>
        /// 加载章节图片
        /// </summary>
        /// <param name="chapter">指定的章节</param>
        /// <param name="flag">加载模式</param>
        /// <returns></returns>
        private async Task LoadChapterPicAsync(Chapter chapter, string flag)
        {
            try
            {
                var tasks = new List<Task<ImageSource>>();
                var picEnumerator = await chapter.GetPicEnumeratorAsync();
                if (flag == "Last")
                {
                    picEnumerator = picEnumerator.Reverse();
                }
                var sourceName = chapter.Comic.SourceName;
                var results = Array.Empty<ImageSource>();
                if (sourceName != "mangahasu")
                {
                    results = picEnumerator.Select(pic => ImageSource.FromUri(new Uri(pic))).ToArray();
                }
                else
                {
                    foreach (var pic in picEnumerator)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            byte[] bytes = await chapter.Comic.Source.HttpClient.GetByteArrayAsync(pic);
                            return ImageSource.FromStream(() => new MemoryStream(bytes));
                        }));
                    }
                    results = await Task.WhenAll(tasks);
                }
                foreach (var image in results)
                {
                    if (flag == "Last") Images.Insert(0, image);
                    else Images.Add(image);
                }
                if (flag == "Last") LoadedChapter.Insert(0, chapter);
                else LoadedChapter.Add(chapter);
                if (flag == "Init") utillCrrentChapterImageCount = Chapter!.PageCount;
            }
            catch (Exception e)
            {
                if (e.Message == "请求失败") _ = Toast.Make(e.Message).Show();
                else _ = Toast.Make("好像出了点小问题，用浏览器打开试试吧").Show();
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 获取新章节并加载图片
        /// </summary>
        /// <param name="flag">指定上一话或下一话</param>
        /// <returns></returns>
        public async Task<bool> UpdateChapterAsync(string flag)
        {
            Chapter? newChapter = Chapter!.Comic.GetNearChapter(Chapter, flag);
            if (newChapter is not null)
            {
                try
                {
                    await LoadChapterPicAsync(newChapter, flag);
                    if (flag == "Last")
                    {
                        CurrentChapterIndex++;
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// 同步收藏和历史的最后阅读章节索引
        /// </summary>
        /// <returns></returns>
        public async Task StoreLastReadedChapterIndex()
        {
            Chapter!.Comic.LastReadedChapterIndex = Chapter.Index;
            var category = Chapter.Comic.Category;
            Chapter.Comic.Category = DBComicCategory.History;
            await _db.UpdateComicAsync(Chapter.Comic);
            if (await _db.IsComicExistAsync(Chapter.Comic, DBComicCategory.Favorite))
            {
                Chapter.Comic.Category = DBComicCategory.Favorite;
                await _db.UpdateComicAsync(Chapter.Comic);
            }
            Chapter.Comic.Category = category;
        }

        /// <summary>
        /// 根据视图变化更新当前章节和页码
        /// </summary>
        /// <param name="fIndex">视图变化后第一个item索引</param>
        /// <param name="cIndex">视图变化后位于正中的item索引</param>
        /// <param name="lIndex">视图变化后最后一个item索引</param>
        /// <returns></returns>
        public async Task ViewChanged(int fIndex, int cIndex, int lIndex)
        {
            if (cIndex != crrentViewCentricItemIndex)
            {
                if (cIndex - crrentViewCentricItemIndex == -1)
                {
                    CurrentPageNum--;
                    if (cIndex != 0 && cIndex == utillCrrentChapterImageCount - Chapter!.PageCount - 1)
                    {
                        CurrentChapterIndex--;
                        CurrentPageNum = Chapter.PageCount;
                        _ = StoreLastReadedChapterIndex();
                    }
                }
                else if (cIndex - crrentViewCentricItemIndex == 1)
                {
                    CurrentPageNum++;
                    if (cIndex == utillCrrentChapterImageCount)
                    {
                        CurrentChapterIndex++;
                        CurrentPageNum = 1;
                        _ = StoreLastReadedChapterIndex();
                    }
                }
                crrentViewCentricItemIndex = cIndex;
            }
            if (fIndex != crrentViewFirstItemIndex)
            {
                crrentViewFirstItemIndex = fIndex;
            }
            if (lIndex != crrentViewLastItemIndex)
            {
                crrentViewLastItemIndex = lIndex;
                if (Images.ToList().Count != 0 && lIndex == Images.LongCount() && !IsLoading)
                {
                    IsLoading = true;
                    _ = Toast.Make("正在加载下一章...").Show();
                    var result = await UpdateChapterAsync("Next");
                    if (result)
                    {
                        _ = Toast.Make("加载成功").Show();
                    }
                    else
                    {
                        _ = Toast.Make("已是最新一话").Show();
                    }
                    IsLoading = false;
                }
            }
        }

        [RelayCommand]
        public async Task LoadLastChapterAsync()
        {
            _ = Toast.Make("正在加载上一章...").Show();
            var result = await UpdateChapterAsync("Last");
            if (result)
            {
                _ = Toast.Make("加载成功").Show();
            }
            else
            {
                _ = Toast.Make("已是第一话").Show();
            }

            IsShowRefresh = false;
        }
    }
}