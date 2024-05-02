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
        this.listView.ScrollTo(_vm.Images.First(), position: ScrollToPosition.Start, false);
    }
}