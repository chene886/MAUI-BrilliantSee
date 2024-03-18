using BrilliantComic.ViewModels;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Platform;

namespace BrilliantComic.Views;

public partial class AIPage : ContentPage
{
    private readonly AIViewModel _vm;

    public AIPage(AIViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        this.model.IsVisible = !this.model.IsVisible;
        this.cover.IsVisible = !this.cover.IsVisible;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var obj = sender! as Button;
        var shadow = obj!.Shadow;
        obj!.Shadow = new Shadow()
        {
            Offset = new Point(0, 8),
            Opacity = (float)0.3,
            Radius = 14,
        };
        await obj!.ScaleTo(1.15, 200);
        await obj!.ScaleTo(1, 200);
        obj!.Shadow = shadow;
    }

    private void UpdateModel(object sender, EventArgs e)
    {
        Button_Clicked(sender, e);
        _vm.UpdateModel(this.name.Text, this.key.Text, this.url.Text);
    }

    private async void StartChat(object sender, EventArgs e)
    {
        Button_Clicked(sender, e);
        var input = this.prompt.Text;
        if (!string.IsNullOrEmpty(input))
        {
            this.prompt.Text = string.Empty;
#if ANDROID
        if (prompt.IsSoftKeyboardShowing())
        {
            _ = prompt.HideKeyboardAsync(CancellationToken.None);
        }
#endif
            this.chat.Children.Add(new Frame()
            {
                Content = new Label()
                {
                    Text = input,

                    TextColor = Color.FromArgb("#512BD4"),
                },
                BackgroundColor = Color.FromArgb("#FFFFFF"),
                Padding = new Thickness(8),
                Margin = new Thickness(0, 24, 0, 0),
                CornerRadius = 10,
                MaximumWidthRequest = 320,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
            });
            await this.chatList.ScrollToAsync(chat, ScrollToPosition.End, false);
            var result = await _vm.Chat(input);
            if (result is not null)
            {
                this.chat.Children.Add(new Frame()
                {
                    Content = new Label()
                    {
                        Text = result,
                        TextColor = Color.FromArgb("#FFFFFF"),
                    },
                    CornerRadius = 10,
                    Padding = new Thickness(8),
                    Margin = new Thickness(0, 24, 0, 0),
                    BackgroundColor = Color.FromArgb("#512BD4"),
                    MaximumWidthRequest = 320,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                });
                await this.chatList.ScrollToAsync(chat, ScrollToPosition.End, false);
            }
        }
        else
        {
            _ = Toast.Make("请正确输入内容").Show();
        }
    }
}