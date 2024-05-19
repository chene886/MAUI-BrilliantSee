using BrilliantSee.ViewModels;

namespace BrilliantSee.Views;

public partial class BrowsePage : ContentPage
{
    private readonly BrowseViewModel _vm;

    public BrowsePage(BrowseViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
        _vm.ScrollToTop += OnScrollToTop;
    }

    /// <summary>
    /// ¹ö¶¯µ½¶¥²¿
    /// </summary>
    private void OnScrollToTop()
    {
        this.collectionView.ScrollTo(_vm.Images.First(), -1, ScrollToPosition.Start, false);
    }

    private void collectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        _vm.CurrentPageNum = e.CenterItemIndex + 1;
    }

    private async void img_Error(object sender, FFImageLoading.Maui.CachedImageEvents.ErrorEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync( () =>
        {
            var img = (View)sender;
            img.FindByName<Button>("btn").IsVisible = true;
        });
    }

    private async void btn_Clicked(object sender, EventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            var btn = (Button)sender;
            btn.IsVisible = false;
            btn.FindByName<FFImageLoading.Maui.CachedImage>("img").ReloadImage();
        });
    }

    private async void img_Loaded(object sender, EventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            var img = (View)sender;
            img.FindByName<Button>("btn").IsVisible = false;
        });
    }
}