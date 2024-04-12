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

    private void Button_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button!.Text == "点击加载下一话")
        {
            this.list.Command.Execute("Next");
            if (_vm.Images.Count > 0) this.listView.ScrollTo(_vm.Images.First(), ScrollToPosition.Start, false);
        }
    }
}