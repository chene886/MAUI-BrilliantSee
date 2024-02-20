using BrilliantComic.ViewModels;

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
}