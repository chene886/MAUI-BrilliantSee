using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Sources.MusicSources;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Platform;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BrilliantSee.Controls;

public partial class BottomSheet : ContentView, INotifyPropertyChanged
{
    public string MusicSource { get; set; } = string.Empty;
    public ObservableCollection<Obj> Musics { get; set; } = new();

    private MMPlayerSource _source = new();

    //public event PropertyChangedEventHandler PropertyChanged = delegate { };

    //protected override void OnPropertyChanged(string propertyName)
    //{
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //}

    public BottomSheet()
    {
        this.BindingContext = this;
        InitializeComponent();
    }

    private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        //if(Math.Abs(e.TotalX) > 50)
        //{
        //    return;
        //}
        frame.HeightRequest -= e.TotalY;
    }

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
    }

    //[RelayCommand]
    //private async Task SearchAsync(string keyword)
    //{
    //    //if (Entry.IsSoftKeyboardShowing()) await Entry.HideKeyboardAsync();
    //    //if (string.IsNullOrWhiteSpace(keyword) || keyword == "")
    //    //{
    //    //    _ = Toast.Make("请输入正确的关键词").Show();
    //    //    return;
    //    //}
    //    //else
    //    //{
    //    //    var result = await _source.SearchAsync(keyword);
    //    //    if (!result.Any())
    //    //    {
    //    //        _ = Toast.Make("未找到相关音乐").Show();
    //    //        return;
    //    //    }
    //    //    Musics.Clear();
    //    //    foreach (var music in result)
    //    //    {
    //    //        Musics.Add(music);
    //    //    }
    //    //    OnPropertyChanged(nameof(Musics));
    //    //}
    //}

    //[RelayCommand]
    //private async Task Play(Obj music)
    //{
    //}
}