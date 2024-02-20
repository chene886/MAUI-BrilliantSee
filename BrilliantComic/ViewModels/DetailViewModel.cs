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
        [ObservableProperty]
        private Comic? _comic;

        [ObservableProperty]
        private ImageSource? _favoriteImage;

        [ObservableProperty]
        private bool _isReverseListEnabled;

        [ObservableProperty]
        private bool _isGettingResult;

        private readonly DBService _db;

        public DetailViewModel(DBService db)
        {
            _db = db;
        }

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
            if(Comic!.Description == string.Empty)
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

        private async Task AddHistoryComicAsync()
        {
            await _db.SaveComicAsync(Comic!, DBComicCategory.History);
        }

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

        [RelayCommand]
        private async Task JumpToBrowserAsync()
        {
            await Launcher.OpenAsync(new Uri(Comic!.Url));
        }

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

        [RelayCommand]
        private async Task OpenChapterAsync(Chapter chapter)
        {
            await Shell.Current.GoToAsync("BrowsePage", new Dictionary<string, object> { { "Chapter", chapter } });
        }

        [RelayCommand]
        private async Task TestAsync()
        {
            await Shell.Current.GoToAsync("FavoritePage");
        }
    }
}