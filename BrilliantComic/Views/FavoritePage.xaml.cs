using BrilliantSee.Models.Enums;
using BrilliantSee.ViewModels;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Platform;
using System.Text.RegularExpressions;

namespace BrilliantSee.Views;

public partial class FavoritePage : ContentPage
{
    private readonly FavoriteViewModel _vm;

    public FavoritePage(FavoriteViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
        _ = CheckUpdate();
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
        await _vm.OnLoadFavoriteObjAsync();
        if (_vm._ai.hasModel)
        {
            _vm._ai.RemovePlugins();
            _vm._ai.ImportPlugins(new Services.Plugins.FavoritePlugin(_vm._db));
        }
        this.audio.IsVisible = await _vm._db.GetAudioStatus();
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
            Offset = new Point(0, 0),
            Opacity = (float)0.3,
            Radius = 14,
        };
        await obj!.ScaleTo(1.05, 100);
        await obj!.ScaleTo(1, 100);
        obj!.Shadow = shadow;
    }

    public async Task CheckUpdate()
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync("https://www.123pan.com/s/6cnjjv-6njBv.html");
        var html = await response.Content.ReadAsStringAsync();
        var match = Regex.Match(html, "\"FileName\"[\\s\\S]*?\"(.*?)\"");
        if (match.Success)
        {
            var version = match.Groups[1].Value;
            if (version != "BrilliantSee_v2.0")
            {
                bool answer = await DisplayAlert("检测到新版本", "是否更新?", "快让朕瞧瞧", "朕不感兴趣");
                if (answer)
                {
                    await Launcher.OpenAsync("https://www.123pan.com/s/6cnjjv-6njBv.html");
                }
            }
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var button = sender! as Button;
        var text = button!.Text;
        var buttons = new List<Button>() { this.comics, this.novels, this.videos };
        foreach (var item in buttons)
        {
            item.FontSize = item.Text == text ? 18 : 14;
        }
        _vm.CurrentCategory = text == "漫画" ? SourceCategory.Comic : text == "小说" ? SourceCategory.Novel : SourceCategory.Video;
        await _vm.OnLoadFavoriteObjAsync();
    }
}