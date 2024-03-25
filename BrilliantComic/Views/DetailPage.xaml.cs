using BrilliantComic.Models.Comics;
using BrilliantComic.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics.Metrics;

namespace BrilliantComic.Views;

public partial class DetailPage : ContentPage
{
    private readonly DetailViewModel _vm;

    public DetailPage(DetailViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
        this.Loaded += DetailPage_Loaded;
    }

    private async void DetailPage_Loaded(object? sender, EventArgs e)
    {
        this.audio.IsVisible = await _vm._db.GetAudioStatus();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var obj = sender! as Frame;
        var shadow = obj!.Shadow;
        obj!.Shadow = new Shadow()
        {
            Offset = new Point(0, 8),
            Opacity = (float)0.3,
            Radius = 14,
        };
        await obj!.ScaleTo(1.15, 250);
        await obj!.ScaleTo(1, 250);
        obj!.Shadow = shadow;
    }
}