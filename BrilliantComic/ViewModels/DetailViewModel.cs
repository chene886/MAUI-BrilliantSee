using BrilliantComic.Models.Chapters;
using BrilliantComic.Models.Comics;
using BrilliantComic.Models.Enums;
using BrilliantComic.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.ViewModels
{
    public partial class DetailViewModel : ObservableObject, IQueryAttributable
    {
        /// <summary>
        /// 当前漫画
        /// </summary>
        [ObservableProperty]
        private Comic? _comic;

        /// <summary>
        /// 收藏图标
        /// </summary>
        [ObservableProperty]
        private ImageSource? _favoriteImage;

        /// <summary>
        /// 是否允许反转章节列表
        /// </summary>
        [ObservableProperty]
        private bool _isReverseListEnabled;

        /// <summary>
        /// 是否正在获取结果
        /// </summary>
        [ObservableProperty]
        private bool _isGettingResult;

        private readonly DBService _db;

        public DetailViewModel(DBService db)
        {
            _db = db;
        }

        /// <summary>
        /// 设置当前漫画，加载更多漫画数据，储存漫画到历史记录
        /// </summary>
        /// <param name="query">储存导航传递数据的字典</param>
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (Comic is not null)
            {
                return;
            }
            Comic = query["Comic"] as Comic;

            IsGettingResult = true;

            var isExist = await _db.IsComicExistAsync(Comic!, DBComicCategory.Favorite);
            if (isExist)
            {
                FavoriteImage = ImageSource.FromFile("isfavorite.png");
                Comic!.Category = DBComicCategory.Favorite;
            }
            else FavoriteImage = ImageSource.FromFile("notfavorite.png");
            if (Comic!.Description == string.Empty)
            {
                await Comic!.LoadMoreDataAsync();
            }
            OnPropertyChanged(nameof(Comic));

            Comic!.IsReverseList = false;
            IsReverseListEnabled = false;
            await Task.Run(() => Comic!.LoadChaptersAsync(Comic!.IsReverseList));
            IsReverseListEnabled = true;

            IsGettingResult = false;

            _ = AddHistoryComicAsync();
        }

        /// <summary>
        /// 储存漫画到历史记录
        /// </summary>
        private async Task AddHistoryComicAsync()
        {
            await _db.SaveComicAsync(Comic!, DBComicCategory.History);
        }

        /// <summary>
        /// 切换收藏状态并储存漫画或删除漫画
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SwitchIsFavorite()
        {
            if (Comic is null)
            {
                return;
            }
            if (Comic.Category == DBComicCategory.Favorite)
            {
                FavoriteImage = ImageSource.FromFile("notfavorite.png");
                await _db.DeleteComicAsync(Comic, Comic.Category);
                Comic.Category = DBComicCategory.History;
            }
            else
            {
                FavoriteImage = ImageSource.FromFile("isfavorite.png");
                Comic.Category = DBComicCategory.Favorite;
                await _db.SaveComicAsync(Comic, Comic.Category);
            }
            OnPropertyChanged(nameof(Comic));
        }

        /// <summary>
        /// 跳转到浏览器浏览漫画
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task JumpToBrowserAsync()
        {
            await Launcher.OpenAsync(new Uri(Comic!.Url));
        }

        /// <summary>
        /// 章节列表倒序
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task ReverseListAsync()
        {
            IsGettingResult = true;
            IsReverseListEnabled = false;
            Comic!.IsReverseList = !Comic.IsReverseList;
            await Task.Run(() =>
            {
                var tempChapters = Comic.Chapters.ToList();
                tempChapters.Reverse();
                Comic.Chapters = tempChapters;
            });
            IsReverseListEnabled = true;
            IsGettingResult = false;
        }

        /// <summary>
        /// 跳转到章节浏览页
        /// </summary>
        /// <param name="chapter">导航传递章节</param>
        /// <returns></returns>
        [RelayCommand]
        private async Task OpenChapterAsync(Chapter chapter)
        {
            await Shell.Current.GoToAsync("BrowsePage", new Dictionary<string, object> { { "Chapter", chapter } });
        }
    }
}