using BrilliantComic.ViewModels;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Platform;

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
        if (_vm._ai.hasModel)
        {
            _vm._ai.RemovePlugins();
            _vm._ai.ImportPlugins(new Services.Plugins.FavoritePlugin(_vm._db));
        }
        var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.StorageWrite>();
        }
        status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Microphone>();
        }
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