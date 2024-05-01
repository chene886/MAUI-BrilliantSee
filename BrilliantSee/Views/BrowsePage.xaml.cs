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
    /// 滚动到顶部
    /// </summary>
    private void OnScrollToTop()
    {
        this.listView.ScrollTo(_vm.Images.First(), position: ScrollToPosition.Start,  false);
    }

    /// <summary>
    /// 退出页面时，取消加载当前章节图片
    /// </summary>
    protected override void OnDisappearing()
    {
        _vm.CancelLoadCurrentChapterImage();

        base.OnDisappearing();
    }
}