using BrilliantSee.Models.Enums;
using BrilliantSee.ViewModels;
using CommunityToolkit.Maui.Core.Platform;

namespace BrilliantSee.Views;

public partial class SearchPage : ContentPage
{
    private readonly SearchViewModel _vm;

    /// <summary>
    /// 按钮文本对应的类别
    /// </summary>
    private Dictionary<string, SourceCategory> Categories;

    /// <summary>
    /// 类别对应的按钮
    /// </summary>
    private Dictionary<SourceCategory, Button> Buttons;

    public SearchPage(SearchViewModel vm)
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
        this.Loaded += SearchPage_Loaded;
    }

    /// <summary>
    /// 页面出现时输入框获取焦点
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SearchPage_Loaded(object? sender, EventArgs e)
    {
        this.input.Focus();
        await Task.Delay(250);
        this.input.Focus();
        //this.audio.IsVisible = await _vm._db.GetAudioStatus();
    }

    /// <summary>
    /// 收起键盘
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HideKeyboard(object sender, TappedEventArgs e)
    {
#if ANDROID
        if (input.IsSoftKeyboardShowing())
        {
            _ = input.HideKeyboardAsync(CancellationToken.None);
        }
#endif
    }

    /// <summary>
    /// 监听滚动事件，实现返回顶部按钮的显示与隐藏以及到底触发加载更多
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        this.floatButton.IsVisible = e.FirstVisibleItemIndex == 0 ? false : true;
        if (e.LastVisibleItemIndex == _vm.CurrentObjsCount - 1 && _vm.IsGettingResult == false && _vm.CurrentObjsCount != 0)
        {
            await _vm.GetMoreAsync();
        }
    }

    /// <summary>
    /// 返回顶部
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BacktoTop(object sender, EventArgs e)
    {
        this.comicList.ScrollTo(0, position: ScrollToPosition.Start);
    }

    /// <summary>
    /// 按钮点击效果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="type"></param>
    private async Task ButtonTapped(object sender, Type type)
    {
        View obj = (View)sender;
        await obj!.ScaleTo(1.15, 100);
        await obj!.ScaleTo(1, 100);
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        _ = ButtonTapped(sender, sender.GetType());
    }

    private void floatButton_Pressed(object sender, EventArgs e)
    {
        _ = ButtonTapped(sender, sender.GetType());
    }

    /// <summary>
    /// 切换类别，更新UI,刷新列表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Clicked(object sender, EventArgs e)
    {
        var button = sender! as Button;
        var selectedCategory = Categories[button!.Text];

        if (selectedCategory == _vm.CurrentCategory) return;
        Buttons[_vm.CurrentCategory].TextColor = Color.FromArgb("#212121");
        button.TextColor = Color.FromArgb("#512BD4");
        _ = ButtonTapped(sender, typeof(Button));

        _vm.ChangeCurrentCategory(selectedCategory);
        this.comicList.ItemsSource = _vm.GetObjsOnDisplay();
    }
}