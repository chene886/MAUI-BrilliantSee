using BrilliantSee.Models.Enums;
using BrilliantSee.ViewModels;
using CommunityToolkit.Maui.Core.Platform;

namespace BrilliantSee.Views;

public partial class SearchPage : ContentPage
{
    private readonly SearchViewModel _vm;

    private SourceCategory category = SourceCategory.Comic;

    public SearchPage(SearchViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
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
        this.audio.IsVisible = await _vm._db.GetAudioStatus();
    }

    private void HideKeyboard(object sender, TappedEventArgs e)
    {
#if ANDROID
        if (input.IsSoftKeyboardShowing())
        {
            _ = input.HideKeyboardAsync(CancellationToken.None);
        }
#endif
    }

    private async void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        this.floatButton.IsVisible = e.FirstVisibleItemIndex == 0 ? false : true;
        var count = category == SourceCategory.Comic ? _vm.Comics.Count : category == SourceCategory.Novel ? _vm.Novels.Count : _vm.Videos.Count;
        if (e.LastVisibleItemIndex == count - 1 && _vm.IsGettingResult == false && count != 0)
        {
            await _vm.GetMoreAsync(category);
        }
    }

    private void BacktoTop(object sender, EventArgs e)
    {
        this.comicList.ScrollTo(0, position: ScrollToPosition.Start);
    }

    private async void ButtonTapped(object sender, Type type)
    {
        var obj = sender! as View;
        if (typeof(HorizontalStackLayout) == type)
        {
            obj = sender! as HorizontalStackLayout;
        }
        else if (typeof(Button) == type)
        {
            obj = sender! as Button;
        }
        obj!.Shadow = new Shadow()
        {
            Offset = new Point(0, 8),
            Opacity = (float)0.3,
            Radius = 14,
        };
        await obj!.ScaleTo(1.15, 200);
        obj!.Shadow = new Shadow()
        {
            Radius = 0,
        };
        await obj!.ScaleTo(1, 200);
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        ButtonTapped(sender, sender.GetType());
    }

    private void floatButton_Pressed(object sender, EventArgs e)
    {
        ButtonTapped(sender, sender.GetType());
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        var button = sender! as Button;
        var text = button!.Text;
        var buttons = new List<Button>() { this.comics, this.novels, this.videos };
        foreach (var item in buttons)
        {
            item.FontSize = item.Text == text ? 18 : 14;
        }
        _vm.IsGettingResult = true;
        this.comicList.ItemsSource = text == "漫画" ? _vm.Comics : text == "小说" ? _vm.Novels : _vm.Videos;
        _vm.IsGettingResult = false;
        category = text == "漫画" ? SourceCategory.Comic : text == "小说" ? SourceCategory.Novel : SourceCategory.Video;
    }
}