using BrilliantComic.ViewModels;

namespace BrilliantComic.Views;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryViewModel _vm;

    public HistoryPage(HistoryViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
        
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _vm.OnLoadHistoryComicAsync();
    }
}