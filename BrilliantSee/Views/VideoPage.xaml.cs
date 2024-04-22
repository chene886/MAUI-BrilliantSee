using BrilliantSee.ViewModels;
using CommunityToolkit.Maui.Views;

namespace BrilliantSee.Views;

public partial class VideoPage : ContentPage
{
    private readonly DetailViewModel _vm;

    public VideoPage(DetailViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
        media.Handler?.DisconnectHandler();
    }
}