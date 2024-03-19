using BrilliantComic.Models;
using BrilliantComic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.ViewModels
{
    public partial class AIViewModel : ObservableObject
    {
        private readonly AIService _aiService;
        private readonly DBService _db;
        public bool hasModel { get; set; } = false;

        [ObservableProperty]
        public bool _isGettingResult = false;

        public List<SettingItem> modelConfigs { get; set; } = new List<SettingItem>();

        public AIViewModel(AIService aiService, DBService db)
        {
            _aiService = aiService;
            _db = db;
            hasModel = _aiService.hasModel;
            _aiService.RemovePlugins();
        }

        public void UpdateModel(string name, string key, string url)
        {
            IsGettingResult = true;
            _aiService.InitKernel(name, key, url);
            foreach (var item in modelConfigs)
            {
                switch (item.Name)
                {
                    case "ModelId":
                        item.Value = name;
                        break;

                    case "ApiKey":
                        item.Value = key;
                        break;

                    case "ApiUrl":
                        item.Value = url;
                        break;
                }
                _ = _db.UpdateSettingItemAsync(item);
            }
            hasModel = true;
            IsGettingResult = false;
        }

        public async Task<string> Chat(string prompt)
        {
            IsGettingResult = true;
            var result = string.Empty;
            result = await Task.Run(async Task<string>? () => { return await _aiService.SolvePromptAsync(prompt); });
            IsGettingResult = false;
            return result;
        }
    }
}