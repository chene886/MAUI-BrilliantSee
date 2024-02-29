using BrilliantComic.ViewModels;
using CommunityToolkit.Maui.Alerts;

namespace BrilliantComic.Views;

public partial class BrowsePage : ContentPage
{
    /// <summary>
    /// 是否正在加载
    /// </summary>
    private bool isLoading = false;

    /// <summary>
    /// 当前第一个可见item在CollectionView中的索引
    /// </summary>
    private int crrentFirstVisibleItemIndex = 0;

    /// <summary>
    /// 当前最后一个可见item在CollectionView中的索引
    /// </summary>
    private int crrentLastVisibleItemIndex = 1;

    /// <summary>
    /// 当前位于可见界面中间的item在CollectionView中的索引
    /// </summary>
    private int crrentCenterItemIndex = 0;

    private readonly BrowseViewModel _vm;

    public BrowsePage(BrowseViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
    }

    /// <summary>
    /// 根据滚动位置修正当前参数
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        if (e.CenterItemIndex != crrentCenterItemIndex)
        {
            if (e.CenterItemIndex - crrentCenterItemIndex == -1)
            {
                _vm.CurrentPageNum--;
                if (e.CenterItemIndex != 0 && e.CenterItemIndex == _vm.utillCrrentChapterImageCount - _vm.Chapter!.PageCount - 1)
                {
                    _vm.CurrentChapterIndex--;
                    _vm.CurrentPageNum = _vm.Chapter.PageCount;
                    _ = _vm.StoreLastReadedChapterIndex();
                }
            }
            else if (e.CenterItemIndex - crrentCenterItemIndex == 1)
            {
                _vm.CurrentPageNum++;
                if (e.CenterItemIndex == _vm.utillCrrentChapterImageCount)
                {
                    _vm.CurrentChapterIndex++;
                    _vm.CurrentPageNum = 1;
                    _ = _vm.StoreLastReadedChapterIndex();
                }
            }
            crrentCenterItemIndex = e.CenterItemIndex;
        }
        if (e.FirstVisibleItemIndex != crrentFirstVisibleItemIndex)
        {
            crrentFirstVisibleItemIndex = e.FirstVisibleItemIndex;
            if (e.FirstVisibleItemIndex == 0 && !isLoading)
            {
                isLoading = true;
                _ = Toast.Make("正在加载上一章...").Show();
                var result = await _vm.UpdateChapterAsync("Last");
                if (result)
                {
                    _ = Toast.Make("加载成功").Show();
                }
                else
                {
                    _ = Toast.Make("已是第一话").Show();
                }
                isLoading = false;
            }
        }
        if (e.LastVisibleItemIndex != crrentLastVisibleItemIndex)
        {
            crrentLastVisibleItemIndex = e.LastVisibleItemIndex;
            if (_vm.Images.ToList().Count != 0 && e.LastVisibleItemIndex == _vm.Images.LongCount() && !isLoading)
            {
                isLoading = true;
                _ = Toast.Make("正在加载下一章...").Show();
                var result = await _vm.UpdateChapterAsync("Next");
                if (result)
                {
                    _ = Toast.Make("加载成功").Show();
                }
                else
                {
                    _ = Toast.Make("已是最新一话").Show();
                }
                isLoading = false;
            }
        }
    }
}