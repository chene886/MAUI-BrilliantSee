using BrilliantComic.ViewModels;
using CommunityToolkit.Maui.Alerts;

namespace BrilliantComic.Views;

public partial class BrowsePage : ContentPage
{
    private readonly BrowseViewModel _vm;

    public BrowsePage(BrowseViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
    }

    /// <summary>
    /// 位置发生变化时
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        await _vm.ViewChanged(e.FirstVisibleItemIndex,e.CenterItemIndex,e.LastVisibleItemIndex);
    }
}