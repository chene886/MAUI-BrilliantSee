using BrilliantSee.Models;
using BrilliantSee.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BrilliantSee.ViewModels
{
    public partial class SettingViewModel : ObservableObject
    {
        private readonly DBService _db;
        private readonly MessageService _ms;
        public ObservableCollection<Group<SettingItem>> SettingGroups { get; set; } = new ObservableCollection<Group<SettingItem>>();

        public List<SettingItem> SettingItems_1 { get; set; } = new List<SettingItem>();
        public List<SettingItem> SettingItems_2 { get; set; } = new List<SettingItem>();

        [ObservableProperty]
        public string _message = string.Empty;

        public SettingViewModel(DBService db, MessageService ms)
        {
            _db = db;
            _ms = ms;
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
            SettingGroups.Add(new Group<SettingItem>("通用", SettingItems_1));
            SettingGroups.Add(new Group<SettingItem>("关于", SettingItems_2));
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
                    await Launcher.OpenAsync(new Uri("https://github.com/chene886/MAUI-BrilliantSee"));
                    break;

                case "去分享":
                    var Message = await _db.GetSettingItemMessageAsync("去分享");
                    await Clipboard.SetTextAsync(Message);
                    _ms.WriteMessage("已复制下载链接，快分享给您的小伙伴吧");
                    break;

                default:
                    if (Email.Default.IsComposeSupported) await Email.Default.ComposeAsync("BrilliantSee用户反馈", "", "3256366564@qq.com");
                    else _ms.WriteMessage("未找到邮件应用");
                    break;
            };
        }
    }
}