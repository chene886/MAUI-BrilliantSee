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
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var obj = sender! as Frame;
        var shadow = obj!.Shadow;
        obj!.Shadow = new Shadow()
        {
            Offset = new Point(0, 10),
            Opacity = (float)0.5,
            Radius = 16,
        };
        await obj!.ScaleTo(1.15, 125);
        await obj!.ScaleTo(1, 125);
        obj!.Shadow = shadow;
    }
}