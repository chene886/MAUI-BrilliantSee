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
    }

    private async void ClearAllComic(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("清空历史记录", "历史记录清空后无法恢复，是否继续?", "确定", "取消");
        if (answer)
        {
            await _vm.ClearHistoryComicsAsync();
        }
    }
}