using BrilliantSee.Models.Enums;
using BrilliantSee.ViewModels;
using System.Text.RegularExpressions;

namespace BrilliantSee.Views;

public partial class FavoritePage : ContentPage
{
    private readonly FavoriteViewModel _vm;

    /// <summary>
    /// 按钮文本对应的类别
    /// </summary>
    private Dictionary<string, SourceCategory> Categories;

    /// <summary>
    /// 类别对应的按钮
    /// </summary>
    private Dictionary<SourceCategory, Button> Buttons;

    public FavoritePage(FavoriteViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
        Categories = new Dictionary<string, SourceCategory>()
        {
            { "全部", SourceCategory.All },
            { "小说", SourceCategory.Novel },
            { "漫画", SourceCategory.Comic },
            { "动漫", SourceCategory.Video }
        };
        Buttons = new Dictionary<SourceCategory, Button>()
        {
            { SourceCategory.All, all },
            { SourceCategory.Novel, novels },
            { SourceCategory.Comic, comics },
            { SourceCategory.Video, videos }
        };
        _ = CheckUpdate();
        _vm.ShowHideTip += OnShowHideTip;
    }

    /// <summary>
    /// 页面出现时加载收藏的实体
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.OnLoadFavoriteObjAsync();
        //if (_vm._ai.hasModel)
        //{
        //    _vm._ai.RemovePlugins();
        //    _vm._ai.ImportPlugins(new Services.Plugins.FavoritePlugin(_vm._db));
        //}
        //this.audio.IsVisible = await _vm._db.GetAudioStatus();
        //var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
        //if (status != PermissionStatus.Granted)
        //{
        //    status = await Permissions.RequestAsync<Permissions.StorageWrite>();
        //}
        //status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        //if (status != PermissionStatus.Granted)
        //{
        //    status = await Permissions.RequestAsync<Permissions.Microphone>();
        //}
    }

    /// <summary>
    /// 隐藏提示
    /// </summary>
    private async void OnShowHideTip()
    {
        var answer = await DisplayAlert("操作提示", "隐藏视图和正常视图可通过右下角AI助手按钮左滑来切换，隐藏内容不会出现在历史记录，隐藏状态下取消收藏会一并取消隐藏状态", "不再提示", "下次提醒我");
        if (answer)
        {
            await _vm.DontShowAgain();
        }
    }

    /// <summary>
    /// 按钮点击效果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="type"></param>
    private async Task ButtonTapped(object sender, Type type)
    {
        View obj = type == typeof(Frame) ? (Frame)sender! : (Button)sender!;
        await obj!.ScaleTo(1.15, 100);
        await obj!.ScaleTo(1, 100);
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        _ = ButtonTapped(sender, typeof(Frame));
    }

    /// <summary>
    /// 检测是否有新版本
    /// </summary>
    /// <returns></returns>
    public async Task CheckUpdate()
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://www.123pan.com/s/6cnjjv-6njBv.html");
            var html = await response.Content.ReadAsStringAsync();
            var match = Regex.Match(html, "\"FileName\"[\\s\\S]*?\"(.*?)\"");
            if (match.Success)
            {
                var version = match.Groups[1].Value;
                if (version != "BrilliantSee_v2.2.5")
                {
                    bool answer = await DisplayAlert("检测到新版本", "是否更新?", "快让朕瞧瞧", "朕不感兴趣");
                    if (answer)
                    {
                        await Launcher.OpenAsync("https://www.123pan.com/s/6cnjjv-6njBv.html");
                    }
                }
            }
        }
        catch { }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var button = sender! as Button;
        var selectedCategory = Categories[button!.Text];

        if (selectedCategory == _vm.CurrentCategory) return;
        Buttons[_vm.CurrentCategory].TextColor = Color.FromArgb("#212121");
        button.TextColor = Color.FromArgb("#512BD4");
        _ = ButtonTapped(sender, typeof(Button));

        _vm.ChangeCurrentCategory(selectedCategory);
        await _vm.OnLoadFavoriteObjAsync();
    }
}