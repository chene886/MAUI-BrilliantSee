using BrilliantComic.ViewModels;

namespace BrilliantComic.Views;

public partial class SettingPage : ContentPage
{
    private readonly SettingViewModel _vm;

    public SettingPage(SettingViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var obj = sender! as Button;
        var shadow = obj!.Shadow;
        obj!.Shadow = new Shadow()
        {
            Offset = new Point(0, 8),
            Opacity = (float)0.3,
            Radius = 14,
        };
        await obj!.ScaleTo(1.05, 50);
        await obj!.ScaleTo(1, 50);
        obj!.Shadow = shadow;
        if (!obj!.Text.Contains("ȥ"))
        {
            TapGestureRecognizer_Tapped(sender, new TappedEventArgs(e));
            if (obj!.Text.Contains("�鿴"))
            {
                await this.message.ScrollToAsync(this.message.Children.First(), ScrollToPosition.Start, false);
                await _vm.SetMessageAsync(obj!.Text);
            }
        }
        else await _vm.GoToAsync(obj.Text);
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        this.cover.IsVisible = !this.cover.IsVisible;
        this.window.IsVisible = !this.window.IsVisible;
    }
}