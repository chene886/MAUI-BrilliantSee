using BrilliantSee.ViewModels;
using CommunityToolkit.Maui.Views;

namespace BrilliantSee.Views;

public partial class VideoPage : ContentPage
{
    private readonly DetailViewModel _vm;

    /// <summary>
    /// 线路选择按钮
    /// </summary>
    private readonly Dictionary<string, Button> Buttons;

    /// <summary>
    /// 当前线路
    /// </summary>
    private string CurrentRoute { get; set; } = "线路一";

    public VideoPage(DetailViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
        Buttons = new Dictionary<string, Button>()
        {
            { "线路一", route1 },
            { "线路二", route2 },
            { "线路三", route3 },
            { "线路四", route4 }
        };
    }

    /// <summary>
    /// 页面退出是断开mediaelement资源避免内存泄漏
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
        media.Handler?.DisconnectHandler();
    }

    /// <summary>
    /// 按钮点击效果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="type"></param>
    private async Task ButtonTapped(object sender, Type type)
    {
        View obj = type == typeof(Frame) ? (Frame)sender! : (Button)sender!;
        var shadow = obj!.Shadow;
        obj!.Shadow = new Shadow()
        {
            Offset = new Point(0, 0),
            Opacity = (float)0.3,
            Radius = 14,
        };
        await obj!.ScaleTo(1.05, 100);
        await obj!.ScaleTo(1, 100);
        obj!.Shadow = shadow;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        _ = ButtonTapped(sender, typeof(Frame));
    }

    //切换线路，更新UI
    private void Button_Clicked(object sender, EventArgs e)
    {
        var btn = sender! as Button;
        var SelectedRoute = btn!.Text;
        if (CurrentRoute != SelectedRoute)
        {
            Buttons[CurrentRoute].TextColor = Color.FromArgb("#000000");
            CurrentRoute = SelectedRoute;
            btn.TextColor = Color.FromArgb("#512BD4");
            _ = ButtonTapped(sender, typeof(Button));
            _vm.SetItemsOnDisplay(CurrentRoute);
        }
    }
}