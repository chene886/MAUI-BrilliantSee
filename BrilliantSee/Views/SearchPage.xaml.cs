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

    private Button[] Buttons;

    private int CurrentButtonIndex = 0;
    private SwipeDirection _direction { get; set; }
    private double _offset { get; set; } = 0;

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
        Buttons = new Button[] { all, novels, comics, videos };
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
        _vm.ChangeCurrentCategory(selectedCategory);
        _ = ButtonTapped(sender, typeof(Button));
        CurrentButtonIndex = Array.IndexOf(Buttons, button);

        this.comicList.ItemsSource = _vm.GetObjsOnDisplay();
    }

    private void SwipeView_SwipeChanging(object sender, SwipeChangingEventArgs e)
    {
        _direction = e.SwipeDirection;
        _offset = e.Offset;
    }

    private void SwipeView_SwipeEnded(object sender, SwipeEndedEventArgs e)
    {
        var value = _direction == SwipeDirection.Left ? 1 : -1;
        if (Math.Abs(_offset) > 24)
        {
            swipeView.Close();
            var index = CurrentButtonIndex + value;
            if (index < 0 || index > 3)
            {
                return;
            }
            CurrentButtonIndex = index;
            Button_Clicked(Buttons[CurrentButtonIndex], e);
        }
    }
}