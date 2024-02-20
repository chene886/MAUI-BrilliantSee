using BrilliantComic.ViewModels;

namespace BrilliantComic.Views;

public partial class FavoritePage : ContentPage
{
    private readonly FavoriteViewModel _vm;

    public FavoritePage(FavoriteViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
    }

    private void JumpToSearchPage(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("SearchPage");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _vm.OnLoadFavoriteComicAsync();
    }
}