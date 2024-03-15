using BrilliantComic.ViewModels;
using CommunityToolkit.Maui.Alerts;

namespace BrilliantComic.Views;

public partial class FavoritePage : ContentPage
{
    private readonly FavoriteViewModel _vm;

    public FavoritePage(FavoriteViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
    }

    private void JumpToSearchPage(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("SearchPage");
    }

    /// <summary>
    /// 页面出现时加载收藏的漫画
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.OnLoadFavoriteComicAsync();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var obj = sender! as Frame;
        var shadow = obj!.Shadow;
        obj!.Shadow = new Shadow()
        {
            Offset = new Point(0, 8),
            Opacity = (float)0.3,
            Radius = 14,
        };
        await obj!.ScaleTo(1.15, 200);
        await obj!.ScaleTo(1, 200);
        obj!.Shadow = shadow;
    }

    private void AITapped(object sender, TappedEventArgs e)
    {
        TapGestureRecognizer_Tapped(sender, e);
        if (_vm._aiService.hasModel)
        {
            this.model.IsVisible = false;
            this.key.IsVisible = false;
            this.url.IsVisible = false;
            this.pormpt.IsVisible = true;
        }
        else
        {
            this.pormpt.IsVisible = false;
            this.model.IsVisible = true;
            this.key.IsVisible = true;
            this.url.IsVisible = true;
        }
        this.AIWindow.IsVisible = !this.AIWindow.IsVisible;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        _vm.IsGettingResult = true;
        var message = string.Empty;
        if (_vm._aiService.hasModel)
        {
            await _vm._aiService.SolvePromptAsync(this.pormpt.Text);
            message = "圆满完成任务";
        }
        else
        {
            if (this.model.Text is null || this.key.Text is null || this.url.Text is null)
            {
                message = "请填写完整信息";
            }
            else
            {
                _vm._aiService.UpdateModel(this.model.Text, this.key.Text, this.url.Text);
                message = "模型信息已导入";
            }
        }
        _vm.IsGettingResult = false;
        this.AIWindow.IsVisible = false;
        _ = Toast.Make(message).Show();
    }
}