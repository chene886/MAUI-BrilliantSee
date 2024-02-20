using BrilliantComic.Models.Comics;
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
    public partial class FavoriteViewModel : ObservableObject
    {
        public readonly DBService _db;

        [ObservableProperty]
        private bool _isGettingResult;

        /// <summary>
        /// 历史漫画集合
        /// </summary>
        public ObservableCollection<Comic> Comics { get; set; } = new();

        /// <summary>
        /// 加载历史漫画
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadFavoriteComicAsync()
        {
            Comics.Clear();
            IsGettingResult = true;
            var comics = await _db.GetComicsAsync(Models.Enums.DBComicCategory.Favorite);
            comics.Reverse();
            foreach (var item in comics)
            {
                Comics.Add(item);
            }
            IsGettingResult = false;
        }

        public FavoriteViewModel(DBService db)
        {
            _db = db;
        }

        [RelayCommand]
        private async Task OpenComicAsync(Comic comic)
        {
            await Shell.Current.GoToAsync("DetailPage", new Dictionary<string, object> { { "Comic", comic } });
        }
    }
}