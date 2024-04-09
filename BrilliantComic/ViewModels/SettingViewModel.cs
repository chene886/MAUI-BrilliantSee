using BrilliantComic.Models;
using BrilliantComic.Models.Group;
using BrilliantComic.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.ApplicationModel.Communication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.ViewModels
{
    public partial class SettingViewModel : ObservableObject
    {
        private readonly DBService _db;
        public ObservableCollection<SettingGroup> SettingGroups { get; set; } = new ObservableCollection<SettingGroup>();

        public List<SettingItem> SettingItems_1 { get; set; } = new List<SettingItem>();
        public List<SettingItem> SettingItems_2 { get; set; } = new List<SettingItem>();

        [ObservableProperty]
        public string _message = string.Empty;

        public SettingViewModel(DBService db)
        {
            _db = db;
            _ = InitSettingsAsync();
        }

        public async Task InitSettingsAsync()
        {
            var settingItems = await _db.GetSettingItemsAsync("通用");
            foreach (var item in settingItems)
            {
                SettingItems_1.Add(item);
            }
            settingItems = await _db.GetSettingItemsAsync("关于");
            foreach (var item in settingItems)
            {
                SettingItems_2.Add(item);
            }
            SettingGroups.Add(new SettingGroup("通用", SettingItems_1));
            SettingGroups.Add(new SettingGroup("关于", SettingItems_2));
        }

        public async Task SetMessageAsync(string value)
        {
            Message = await _db.GetSettingItemMessageAsync(value);
        }

        public async Task GoToAsync(string value)
        {
            switch (value)
            {
                case "去支持":
                    await Launcher.OpenAsync(new Uri("https://github.com/chene886/MAUI-BrilliantComic"));
                    break;

                case "去分享":
                    var Message = await _db.GetSettingItemMessageAsync("去分享");
                    await Clipboard.SetTextAsync(Message);
                    _ = Toast.Make("已复制下载链接，快分享给您的小伙伴吧").Show();
                    break;

                default:
                    if (Email.Default.IsComposeSupported)await Email.Default.ComposeAsync("BrilliantComic用户反馈","","3256366564@qq.com");
                    else _ = Toast.Make("未找到邮件应用").Show();
                    break;
            };
        }
    }
}