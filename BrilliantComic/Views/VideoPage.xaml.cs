namespace BrilliantSee.Views;

public partial class VideoPage : ContentPage
{
    public List<string> Urls = new List<string>()
    {
        "第一集",
        "第二集",
        "第三集",
        "第四集",
        "第五集",
    };

    public VideoPage()
    {
        this.BindingContext = this;
        InitializeComponent();
    }
}