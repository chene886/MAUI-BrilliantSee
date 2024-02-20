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
        private readonly Timer _timer;

        /// <summary>
        /// 当前章节
        /// </summary>
        [ObservableProperty]
        private Chapter? _chapter;

        /// <summary>
        /// 已加载章节集合
        /// </summary>
        [ObservableProperty]
        private List<Chapter> _loadedChapter = new List<Chapter>();

        /// <summary>
        /// 已加载章节图片集合
        /// </summary>
        [ObservableProperty]
        public ObservableCollection<string> _Images  = new ObservableCollection<string>();

        /// <summary>
        /// 当前时间
        /// </summary>
        public string CurrentTime => DateTime.Now.ToString("HH:mm");

        /// <summary>
        /// 当前章节在已加载章节中的索引
        /// </summary>
        private int _currentChapterIndex = 0;

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
                    Chapter!.IsSpecial = false;
                    Chapter = LoadedChapter[value];
                    Chapter.IsSpecial = true;
                }
            }
        }

        public BrowseViewModel(DBService db)
        {
            _db = db;
            _timer = new Timer((o) => { OnPropertyChanged(nameof(CurrentTime)); }, null, (60 - DateTime.Now.Second) * 1000, 60000);
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Chapter = (query["Chapter"] as Chapter)!;
            if (Chapter.Url == "")
            {
                return;
            }
            if (Chapter.Index != Chapter.Comic.LastReadedChapterIndex)
            {
                if (Chapter.Comic.LastReadedChapterIndex != -1)
                {
                    var LastReadedChapter = Chapter.Comic.Chapters.ToList()[Chapter.Comic.LastReadedChapterIndex];
                    LastReadedChapter.IsSpecial = false;
                }
                _ = StoreLastReadedChapterIndex();
                Chapter.IsSpecial = true;
            }
            await LoadChapterPicAsync(Chapter, "Init");
            OnPropertyChanged(nameof(Chapter));
        }

        private async Task LoadChapterPicAsync(Chapter chapter, string flag)
        {
            var picEnumerator = await chapter.GetPicEnumeratorAsync();
            if(flag == "Init")
            {
                var images = new ObservableCollection<string>();
                foreach (var pic in picEnumerator)
                {
                    images.Add(pic);
                }
                Images = images;
                LoadedChapter.Add(chapter);
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

        public async Task<bool> UpdateChapterAsync(string flag)
        {
            Chapter newChapter = Chapter!.Comic.GetNearChapter(Chapter, flag);
            if (newChapter.Url == "")
            {
                return false;
            }
            await LoadChapterPicAsync(newChapter, flag);
            return true;
        }

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