using BrilliantComic.Models;
using BrilliantComic.Services;
using BrilliantComic.Views;

namespace BrilliantComic
{
    public partial class App : Application
    {
        private readonly AIService _ai;
        private readonly DBService _db;

        public List<SettingItem> modelConfigs { get; set; } = new List<SettingItem>();

        public App(AIService ai, DBService db)
        {
            _ai = ai;
            _db = db;
            InitializeComponent();

            MainPage = new AppShell();

            Routing.RegisterRoute("SettingPage", typeof(SettingPage));
            Routing.RegisterRoute("SearchPage", typeof(SearchPage));
            Routing.RegisterRoute("DetailPage", typeof(DetailPage));
            Routing.RegisterRoute("BrowsePage", typeof(BrowsePage));
            Routing.RegisterRoute("AIPage", typeof(AIPage));
            _ = InitKernelAsync();
        }

        private async Task InitKernelAsync()
        {
            modelConfigs = await _db.GetSettingItemsAsync("AIModel");
            var modelId = modelConfigs.Where(s => s.Name == "ModelId").First().Value;
            var apiKey = modelConfigs.Where(s => s.Name == "ApiKey").First().Value;
            var apiUrl = modelConfigs.Where(s => s.Name == "ApiUrl").First().Value;
            if (modelId != "" && apiKey != "" && apiUrl != "")
            {
                _ai.InitKernel(modelId, apiKey, apiUrl);
                _ai.hasModel = true;
            }
        }
    }
}