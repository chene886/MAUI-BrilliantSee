using BrilliantComic.Models.Chapters;
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
    public partial class BrowseViewModel : ObservableObject, IQueryAttributable
    {
        /// <summary>
        /// 定时器
        /// </summary>
        private readonly Timer _timer;

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
        public ObservableCollection<string> _Images = new ObservableCollection<string>();

        /// <summary>
        /// 当前页码
        /// </summary>
        [ObservableProperty]
        public int _currentPageNum = 1;

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
                    if(Chapter != newChapter)
                    {
                        if(Chapter!.Index > newChapter.Index)utillCrrentChapterImageCount -= Chapter.PageCount;
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
            var picEnumerator = await chapter.GetPicEnumeratorAsync();
            if (flag == "Init")
            {
                var images = new ObservableCollection<string>();
                foreach (var pic in picEnumerator)
                {
                    images.Add(pic);
                }
                Images = images;
                LoadedChapter.Add(chapter);
                utillCrrentChapterImageCount = chapter.PageCount;
                await UpdateChapterAsync("Last");
            }
            else if (flag == "Last")
            {
                foreach (var pic in picEnumerator.Reverse())
                {
                    Images.Insert(0, pic);
                }
                LoadedChapter.Insert(0, chapter);
            }
            else
            {
                foreach (var pic in picEnumerator)
                {
                    Images.Add(pic);
                }
                LoadedChapter.Add(chapter);
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
                await LoadChapterPicAsync(newChapter, flag);
                if(flag == "Last")
                {
                    CurrentChapterIndex++;
                }
                return true;
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
    }
}