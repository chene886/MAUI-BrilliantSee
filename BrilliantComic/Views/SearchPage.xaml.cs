using BrilliantComic.ViewModels;
using CommunityToolkit.Maui.Core.Platform;

namespace BrilliantComic.Views;

public partial class SearchPage : ContentPage
{
    private readonly SearchViewModel _vm;

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
        await Task.Delay(100);
        this.input.Focus();
    }

    private void HideKeyboard(object sender, TappedEventArgs e)
    {
        if (input.IsSoftKeyboardShowing())
        {
            _ = input.HideKeyboardAsync(CancellationToken.None);
        }
    }

    private void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        if (e.FirstVisibleItemIndex == 0)
        {
            this.floatButton.IsVisible = false;
        }
        else
        {
            this.floatButton.IsVisible = true;
        }
    }

    private void BacktoTop(object sender, EventArgs e)
    {
        this.comicList.ScrollTo(0, position: ScrollToPosition.Start);
    }

    private async void ButtonTapped(object sender, Type type)
    {
        var obj = sender! as View;
        if (typeof(CheckBox) == type)
        {
            var obj1 = sender! as CheckBox;
            obj = obj1!.Parent as HorizontalStackLayout;
        }
        else if (typeof(HorizontalStackLayout) == type)
        {
            obj = sender! as HorizontalStackLayout;
        }
        else if (typeof(Button) == type)
        {
            obj = sender! as Button;
        }
        obj!.Shadow = new Shadow()
        {
            Offset = new Point(0, 10),
            Opacity = (float)0.5,
            Radius = 16,
        };
        await obj!.ScaleTo(1.15, 125, Easing.Default);
        obj!.Shadow = new Shadow()
        {
            Radius = 0,
        };
        await obj!.ScaleTo(1, 125, Easing.Default);
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        ButtonTapped(sender, sender.GetType());
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        ButtonTapped(sender, sender.GetType());
    }

    private void floatButton_Pressed(object sender, EventArgs e)
    {
        ButtonTapped(sender, sender.GetType());
    }
}