using BrilliantComic.ViewModels;
using CommunityToolkit.Maui.Alerts;

namespace BrilliantComic.Views;

public partial class BrowsePage : ContentPage
{
    /// <summary>
    /// 当前第一个可见item在CollectionView中的索引
    /// </summary>
    private int crrentFirstVisibleItemIndex = 0;

    /// <summary>
    /// 当前最后一个可见item在CollectionView中的索引
    /// </summary>
    private int crrentLastVisibleItemIndex = 0;

    /// <summary>
    /// 当前位于可见界面中间的item在CollectionView中的索引
    /// </summary>
    private int crrentCenterItemIndex = 0;

    /// <summary>
    /// 第二个已加载章节到当前章节图片数量总和
    /// </summary>
    private int utillCrrentChapterImageCount = 0;

    private readonly BrowseViewModel _vm;

    public BrowsePage(BrowseViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
    }

    /// <summary>
    /// 根据滚动位置加载上一章或下一章及切换章节
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        if (e.FirstVisibleItemIndex != crrentFirstVisibleItemIndex)
        {
            crrentFirstVisibleItemIndex = e.FirstVisibleItemIndex;
            if (e.FirstVisibleItemIndex == 0)
            {
                var toast = Toast.Make("正在加载上一章");
                _ = toast.Show();
                var result = await _vm.UpdateChapterAsync("Last");
                if (result)
                {
                    utillCrrentChapterImageCount += _vm.Chapter!.PageCount;
                    _vm.CurrentChapterIndex++;

                    var toast1 = Toast.Make("加载成功");
                    _ = toast1.Show();
                }
                else
                {
                    var toast1 = Toast.Make("已是第一话");
                    _ = toast1.Show();
                }
            }
        }
        if (e.LastVisibleItemIndex != crrentLastVisibleItemIndex)
        {
            crrentLastVisibleItemIndex = e.LastVisibleItemIndex;
            if (e.LastVisibleItemIndex == _vm.Images.ToList().Count - 1)
            {
                var toast = Toast.Make("正在加载下一章");
                _ = toast.Show();
                var result = await _vm.UpdateChapterAsync("Next");
                if (result)
                {
                    var toast1 = Toast.Make("加载成功");
                    _ = toast1.Show();
                }
                else
                {
                    var toast1 = Toast.Make("已是最新一话");
                    _ = toast1.Show();
                }
            }
        }
        if (e.CenterItemIndex != crrentCenterItemIndex)
        {
            if (e.CenterItemIndex < crrentCenterItemIndex)
            {
                _vm.CurrentPageNum--;
            }
            else
            {
                _vm.CurrentPageNum++;
            }
            if (e.CenterItemIndex == utillCrrentChapterImageCount - _vm.Chapter!.PageCount + _vm.LoadedChapter[0].PageCount - 1 && crrentCenterItemIndex > e.CenterItemIndex)
            {
                utillCrrentChapterImageCount -= _vm.Chapter.PageCount;
                _vm.CurrentChapterIndex--;
                _vm.CurrentPageNum = _vm.Chapter.PageCount;
                _ = _vm.StoreLastReadedChapterIndex();
            }
            else if (e.CenterItemIndex == utillCrrentChapterImageCount + _vm.LoadedChapter[0].PageCount && crrentCenterItemIndex < e.CenterItemIndex)
            {
                _vm.CurrentChapterIndex++;
                _vm.CurrentPageNum = 1;
                utillCrrentChapterImageCount += _vm.Chapter.PageCount;
                _ = _vm.StoreLastReadedChapterIndex();
            }
            crrentCenterItemIndex = e.CenterItemIndex;
        }
    }
}