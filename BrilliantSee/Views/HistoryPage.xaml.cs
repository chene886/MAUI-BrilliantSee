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

    private Button[] Buttons;

    private int CurrentButtonIndex = 0;
    private SwipeDirection _direction { get; set; }
    private double _offset { get; set; } = 0;

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
        Buttons = new Button[] { all, novels, comics, videos };
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

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        var imgbtn = sender! as ImageButton;
        var grid = imgbtn!.Parent as Grid;
        grid!.IsVisible = false;
    }

    private void DragGestureRecognizer_DragStarting(object sender, DragStartingEventArgs e)
    {
        e.Cancel = true;
        var drag = (DragGestureRecognizer)sender!;
        drag.FindByName<Grid>("buttons").IsVisible = true;
    }
}