using BrilliantSee.ViewModels;

namespace BrilliantSee.Views;

public partial class DetailPage : ContentPage
{
    private readonly DetailViewModel _vm;

    public DetailPage(DetailViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
        //this.Loaded += DetailPage_Loaded;
    }

    //private async void DetailPage_Loaded(object? sender, EventArgs e)
    //{
    //    this.audio.IsVisible = await _vm._db.GetAudioStatus();
    //}

    /// <summary>
    /// 按钮点击效果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var obj = sender! as Frame;
        await obj!.ScaleTo(1.05, 100);
        await obj!.ScaleTo(1, 100);
    }
}