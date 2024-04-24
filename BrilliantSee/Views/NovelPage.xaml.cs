using BrilliantSee.ViewModels;

namespace BrilliantSee.Views;

public partial class NovelPage : ContentPage
{
    private readonly BrowseViewModel _vm;

    public NovelPage(BrowseViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button!.Text == "点击加载下一话")
        {
            this.list.Command.Execute("Next");
            await this.content.ScrollToAsync(0, 0, false);
        }
    }
}