using BrilliantSee.Models;
using BrilliantSee.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrilliantSee.ViewModels
{
    public partial class AIViewModel : ObservableObject
    {
        public readonly AIService _aiService;
        public readonly DBService _db;
        public bool hasModel { get; set; } = false;

        [ObservableProperty]
        public bool _isGettingResult = false;

        [ObservableProperty]
        public ImageSource _audioIcon = ImageSource.FromFile("disable_audio.png");

        public List<SettingItem> modelConfigs { get; set; } = new List<SettingItem>();

        public string AudioStatus { get; set; } = "false";

        public AIViewModel(DBService db)
        {
            _aiService = MauiProgram.servicesProvider!.GetRequiredService<AIService>();
            _db = db;
            hasModel = _aiService.hasModel;
            _aiService.RemovePlugins();
        }

        private async Task GetAudioStatus()
        {
            var audio = await _db.GetSettingItemsAsync("Audio");
            AudioStatus = audio[0].Value;
        }

        public async void UpdateModel(string name, string key, string url)
        {
            IsGettingResult = true;
            _aiService.InitKernel(name, key, url);
            modelConfigs = await _db.GetSettingItemsAsync("AIModel");
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