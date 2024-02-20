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

}