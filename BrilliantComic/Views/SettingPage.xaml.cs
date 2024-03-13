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
        await obj!.ScaleTo(1.15, 125);
        await obj!.ScaleTo(1, 125);
        obj!.Shadow = shadow;
        _vm.IsWindowVisible = !_vm.IsWindowVisible;
    }
}