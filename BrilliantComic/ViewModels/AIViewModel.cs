using BrilliantComic.Models;
using BrilliantComic.Services;
using CommunityToolkit.Maui.Alerts;
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
            _ = InitModelAsync();
        }

        public async Task InitModelAsync()
        {
            modelConfigs = await _db.GetSettingItemsAsync("AIModel");
            var modelId = modelConfigs.Where(s => s.Name == "ModelId").First().Value;
            var apiKey = modelConfigs.Where(s => s.Name == "ApiKey").First().Value;
            var apiUrl = modelConfigs.Where(s => s.Name == "ApiUrl").First().Value;
            if (modelId != "" && apiKey != "" && apiUrl != "")
            {
                _aiService.InitKernel(modelId, apiKey, apiUrl);
                hasModel = true;
            }
        }

        public void UpdateModel(string name, string key, string url)
        {
            IsGettingResult = true;
            var message = string.Empty;
            if (name is null || key is null || url is null)
            {
                message = "请填写完整信息";
            }
            else
            {
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
                message = "模型信息已导入";
            }
            hasModel = true;
            IsGettingResult = false;
            _ = Toast.Make(message).Show();
        }

        public async Task<string> Chat(string prompt)
        {
            IsGettingResult = true;
            var result = string.Empty;
            if (prompt is null)
            {
                _ = Toast.Make("请正确输入内容").Show();
            }
            else
            {
                result = await _aiService.SolvePromptAsync(prompt);
            }
            IsGettingResult = false;
            return result;
        }
    }
}