using BrilliantComic.Views;

namespace BrilliantComic
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            Routing.RegisterRoute("SearchPage", typeof(SearchPage));
            Routing.RegisterRoute("DetailPage", typeof(DetailPage));
            Routing.RegisterRoute("BrowsePage", typeof(BrowsePage));
            Routing.RegisterRoute("HistoryPage", typeof(HistoryPage));
            Routing.RegisterRoute("FavoritePage", typeof(FavoritePage));
        }
    }
}