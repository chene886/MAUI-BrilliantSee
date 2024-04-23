using BrilliantSee.ViewModels;
using CommunityToolkit.Maui.Views;

namespace BrilliantSee.Views;

public partial class VideoPage : ContentPage
{
    private readonly DetailViewModel _vm;

    public VideoPage(DetailViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
        media.Handler?.DisconnectHandler();
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
        await obj!.ScaleTo(1.05, 100);
        await obj!.ScaleTo(1, 100);
        obj!.Shadow = shadow;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        var btn = sender! as Button;
        var CurrentRoute = btn!.Text;
        var btns = new List<Button> { route1, route2, route3, route4 };
        foreach (var item in btns)
        {
            item.TextColor = item.Text == CurrentRoute ? Color.FromArgb("#512BD4") : Color.FromArgb("#000000");
        }
        _vm.SetItemsOnDisplay(CurrentRoute);
    }
}