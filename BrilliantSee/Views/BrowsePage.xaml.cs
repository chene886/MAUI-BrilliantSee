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

    /// <summary>
    /// 退出页面时，取消加载当前章节图片
    /// </summary>
    protected override void OnDisappearing()
    {
        _vm.CancelLoadCurrentChapterImage();

        base.OnDisappearing();
    }
}