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

    /// <summary>
    /// 页面出现时加载收藏的漫画
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.OnLoadFavoriteComicAsync();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var obj = sender! as Frame;
        var shadow = obj!.Shadow;
        obj!.Shadow = new Shadow()
        {
            Offset = new Point(0, 8),
            Opacity = (float)0.3,
            Radius = 14,
        };
        await obj!.ScaleTo(1.15, 200);
        await obj!.ScaleTo(1, 200);
        obj!.Shadow = shadow;
    }
}