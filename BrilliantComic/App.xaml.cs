using BrilliantComic.Views;

namespace BrilliantComic
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            Routing.RegisterRoute("SettingPage", typeof(SettingPage));
            Routing.RegisterRoute("SearchPage", typeof(SearchPage));
            Routing.RegisterRoute("DetailPage", typeof(DetailPage));
            Routing.RegisterRoute("BrowsePage", typeof(BrowsePage));
        }
    }
}