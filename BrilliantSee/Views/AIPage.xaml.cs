using BrilliantSee.Models;
using BrilliantSee.ViewModels;
using CommunityToolkit.Maui.Core.Platform;

namespace BrilliantSee.Views;

public partial class AIPage : ContentPage
{
    private readonly AIViewModel _vm;

    //public bool IsVoice { get; set; } = false;
    //public SettingItem AudioSetting { get; set; } = new SettingItem();

    public AIPage(AIViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        //_ = InitSetting();
        InitializeComponent();
    }

    //private async Task InitSetting()
    //{
    //    var audio = await _vm._db.GetSettingItemsAsync("Audio");
    //    AudioSetting = audio[0];
    //    _vm.AudioIcon = AudioSetting.Value == "true" ? ImageSource.FromFile("enable_audio.png") : ImageSource.FromFile("disable_audio.png");
    //    this.audio.IsVisible = AudioSetting.Value == "true";
    //}

    /// <summary>
    /// 页面出现时检测是否有模型
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        //若无，对用户能够进行的操作进行限制
        if (!_vm.hasModel)
        {
            this.updateModel.IsEnabled = false;
            this.model.IsVisible = true;
            this.cover.IsVisible = true;
            this.cover.IsEnabled = false;
            //this.audioStatus.IsEnabled = false;
        }
    }

    /// <summary>
    /// 收起或展开更新模型窗口和遮罩层
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        this.model.IsVisible = !this.model.IsVisible;
        this.cover.IsVisible = !this.cover.IsVisible;
    }

    //同上
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        ToolbarItem_Clicked(sender, e);
    }

    /// <summary>
    /// 按钮点击效果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
    }

    /// <summary>
    /// 读取输入判断是否可用，若可用则更新UI，取消用户操作限制，更新模型
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void UpdateModel(object sender, EventArgs e)
    {
        Button_Clicked(sender, e);
        var isAccept = _vm.IsAcceptableString(new string[] { this.name.Text, this.key.Text, this.url.Text });
        if (isAccept)
        {
            //收起键盘
#if ANDROID
            _ = this.name.HideKeyboardAsync(CancellationToken.None);
            _ = this.key.HideKeyboardAsync(CancellationToken.None);
            _ = this.url.HideKeyboardAsync(CancellationToken.None);
#endif
            ToolbarItem_Clicked(sender, e);
            //取消用户操作限制
            if (this.updateModel.IsEnabled is false)
            {
                this.updateModel.IsEnabled = true;
                this.cover.IsEnabled = true;
                //this.audioStatus.IsEnabled = true;
            }
            await _vm.UpdateModel(this.name.Text, this.key.Text, this.url.Text);
        }
    }

    /// <summary>
    /// 读取输入并对话
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void StartChat(object sender, EventArgs e)
    {
        Button_Clicked(sender, e);
        var IsAccept = _vm.IsAcceptableString(new string[] { this.prompt.Text });
        if (IsAccept)
        {
            var input = this.prompt.Text;
            this.prompt.Text = string.Empty;
            //收起键盘
#if ANDROID
            if (prompt.IsSoftKeyboardShowing())
            {
                _ = prompt.HideKeyboardAsync(CancellationToken.None);
            }
#endif
            _ = AddChatBubble(input, true);
            var result = await _vm.Chat(input);
            if (result is not null)
            {
                _ = AddChatBubble(result, false);
            }
        }
    }

    /// <summary>
    /// 添加聊天气泡
    /// </summary>
    /// <param name="text">内容</param>
    /// <param name="isUser">是否为用户发送</param>
    /// <returns></returns>
    private async Task AddChatBubble(string text, bool isUser)
    {
        string backgroundColor = isUser ? "#FFFFFF" : "#512BD4";
        string textColor = isUser ? "#512BD4" : "#FFFFFF";
        var horizontalOptions = isUser ? LayoutOptions.End : LayoutOptions.Start;
        this.chat.Children.Add(new Frame()
        {
            Content = new Label()
            {
                Text = text,
                TextColor = Color.FromArgb(textColor),
                FontFamily = "",
            },
            CornerRadius = 10,
            Padding = new Thickness(8),
            Margin = new Thickness(0, 24, 0, 0),
            BackgroundColor = Color.FromArgb(backgroundColor),
            MaximumWidthRequest = 320,
            HorizontalOptions = horizontalOptions,
        });
        await ScrollAsync();
    }

    /// <summary>
    /// 滚动到最新的聊天气泡
    /// </summary>
    /// <returns></returns>
    private async Task ScrollAsync()
    {
        var x = this.result.X;
        var y = this.result.Y;
        await this.chatList.ScrollToAsync(x, y, true);
    }

    //private void ChangeAudioStatus(object sender, EventArgs e)
    //{
    //    AudioSetting.Value = AudioSetting.Value == "true" ? "false" : "true";
    //    _vm.AudioIcon = AudioSetting.Value == "true" ? ImageSource.FromFile("enable_audio.png") : ImageSource.FromFile("disable_audio.png");
    //    //this.audio.IsVisible = AudioSetting.Value == "true";
    //    _ = _vm._db.UpdateSettingItemAsync(AudioSetting);
    //}
}