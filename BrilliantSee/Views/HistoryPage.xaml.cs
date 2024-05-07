using BrilliantSee.Models.Enums;
using BrilliantSee.ViewModels;

namespace BrilliantSee.Views;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryViewModel _vm;

    /// <summary>
    /// 按钮文本对应的类别
    /// </summary>
    private Dictionary<string, SourceCategory> Categories;

    public HistoryPage(HistoryViewModel vm)
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
    }

    /// <summary>
    /// 页面出现时加载历史记录的漫画
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.OnLoadHistoryObjAsync();
        //if (_vm._ai.hasModel)
        //{
        //    _vm._ai.RemovePlugins();
        //    _vm._ai.ImportPlugins(new Services.Plugins.HistoryPlugins(_vm._db));
        //}
        //this.audio.IsVisible = await _vm._db.GetAudioStatus();
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

    /// <summary>
    /// 清空历史记录提示
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void CleanTapped(object sender, TappedEventArgs e)
    {
        _ = ButtonTapped(sender, typeof(Frame));
        bool answer = await DisplayAlert("清空历史记录", "历史记录清空后无法恢复，是否继续?", "确定", "取消");
        if (answer)
        {
            await _vm.ClearHistoryObjsAsync();
        }
    }

    /// <summary>
    /// 跳转到设置页面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void JumpToSettingPage(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("SettingPage");
    }

    /// <summary>
    /// 切换类别，加载不同类别的历史记录，更新UI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Button_Clicked(object sender, EventArgs e)
    {
        var button = sender! as Button;
        var selectedCategory = Categories[button!.Text];

        if (selectedCategory == _vm.CurrentCategory) return;
        _vm.ChangeCurrentCategory(selectedCategory);
        _ = ButtonTapped(sender, typeof(Button));

        await _vm.OnLoadHistoryObjAsync();
    }
}