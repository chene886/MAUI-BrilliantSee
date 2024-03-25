using BrilliantComic.ViewModels;
using System.Diagnostics;

namespace BrilliantComic.Views;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryViewModel _vm;

    public HistoryPage(HistoryViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
    }

    /// <summary>
    /// 页面出现时加载历史记录的漫画
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.OnLoadHistoryComicAsync();
        if (_vm._ai.hasModel)
        {
            _vm._ai.RemovePlugins();
            _vm._ai.ImportPlugins(new Services.Plugins.HistoryPlugins(_vm._db));
        }
        this.audio.IsVisible = await _vm._db.GetAudioStatus();
    }

    private async void CleanTapped(object sender, TappedEventArgs e)
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
        bool answer = await DisplayAlert("清空历史记录", "历史记录清空后无法恢复，是否继续?", "确定", "取消");
        if (answer)
        {
            await _vm.ClearHistoryComicsAsync();
        }
        await obj!.ScaleTo(1, 200);
        obj!.Shadow = shadow;
    }

    private void JumpToSettingPage(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("SettingPage");
    }
}