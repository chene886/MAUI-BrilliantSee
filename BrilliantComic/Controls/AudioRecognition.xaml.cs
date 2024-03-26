using BrilliantComic.Services;
using Plugin.Maui.Audio;

namespace BrilliantComic.Controls;

public partial class AudioRecognition : ContentView
{
    private readonly AIService _ai;

    public AudioRecognition()
    {
        _ai = MauiProgram.servicesProvider!.GetRequiredService<AIService>();
        InitializeComponent();
    }

    private async void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        this.listening.Color = Color.FromArgb("#eeFF0000");
        await Task.Delay(100);
        this.listening.IsVisible = false;
        this.wave.IsVisible = false;
        this.listening.Color = Color.FromArgb("#ee512BD4");
        await _ai.StopMessageAsync("Canceled");
    }

    private async void ImageButton_Pressed(object sender, EventArgs e)
    {
        this.listening.IsVisible = true;
        this.wave.IsVisible = true;
        await _ai.BeingMessageAsync();
    }

    private async void ImageButton_Finished(object sender, EventArgs e)
    {
        this.listening.IsVisible = false;
        this.wave.IsVisible = false;
        var message = await _ai.StopMessageAsync("Finished");
        await _ai.SolvePromptAsync(message);
    }
}