using BrilliantComic.ViewModels;
using CommunityToolkit.Maui.Core.Platform;

namespace BrilliantComic.Views;

public partial class SearchPage : ContentPage
{
    private readonly SearchViewModel _vm;

    public SearchPage(SearchViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
        this.Loaded += SearchPage_Loaded;
    }

    /// <summary>
    /// 页面出现时输入框获取焦点
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SearchPage_Loaded(object? sender, EventArgs e)
    {
        await Task.Delay(100);
        this.input.Focus();
    }

    private void HideKeyboard(object sender, TappedEventArgs e)
    {
        if (input.IsSoftKeyboardShowing())
        {
            _ = input.HideKeyboardAsync(CancellationToken.None);
        }
    }

    private void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        if (e.FirstVisibleItemIndex == 0)
        {
            this.floatButton.IsVisible = false;
        }
        else
        {
            this.floatButton.IsVisible = true;
        }
    }

    private void BacktoTop(object sender, TappedEventArgs e)
    {
        this.comicList.ScrollTo(0, position:ScrollToPosition.Start);
    }
}