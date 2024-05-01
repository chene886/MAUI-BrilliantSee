using BrilliantSee.ViewModels;

namespace BrilliantSee.Views;

public partial class SettingPage : ContentPage
{
    private readonly SettingViewModel _vm;

    public SettingPage(SettingViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
    }

    //根据按钮内容执行不同操作(复制分享内容/打开链接/发送邮件/设置窗口内容，更新UI)
    private async void Button_Clicked(object sender, EventArgs e)
    {
        var obj = sender! as Button;
        var shadow = obj!.Shadow;
        obj.Shadow = new Shadow()
        {
            Offset = new Point(0, 0),
            Opacity = (float)0.3,
            Radius = 14,
        };
        await obj.ScaleTo(1.05, 100);
        await obj.ScaleTo(1, 100);
        obj.Shadow = shadow;
        if (obj.Text.Contains("去")) await _vm.GoToAsync(obj.Text);
        else
        {
            TapGestureRecognizer_Tapped(sender, new TappedEventArgs(e));
            if (obj.Text == "确定") return;
            await this.message.ScrollToAsync(0, 0, false);
            await _vm.SetContentAsync(obj.Text);
        }
    }

    //关闭或开启窗口和遮罩
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        this.cover.IsVisible = !this.cover.IsVisible;
        this.window.IsVisible = !this.window.IsVisible;
    }
}