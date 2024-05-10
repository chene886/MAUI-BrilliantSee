using BrilliantSee.Models;
using BrilliantSee.Models.Enums;
using BrilliantSee.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BrilliantSee.ViewModels
{
    public partial class SettingViewModel : ObservableObject
    {
        private readonly DBService _db;
        private readonly MessageService _ms;

        /// <summary>
        /// 设置项组
        /// </summary>
        public ObservableCollection<Group<SettingItem>> SettingGroups { get; set; } = new ObservableCollection<Group<SettingItem>>();

        /// <summary>
        /// 窗口显示内容
        /// </summary>
        [ObservableProperty]
        public string _content = string.Empty;

        public SettingViewModel(DBService db, MessageService ms)
        {
            _db = db;
            _ms = ms;
            _ = InitSettingsAsync();
        }

        /// <summary>
        /// 初始化设置项
        /// </summary>
        /// <returns></returns>
        public async Task InitSettingsAsync()
        {
            var General = await _db.GetSettingItemsAsync((int)SettingItemCategory.General);
            var About = await _db.GetSettingItemsAsync((int)SettingItemCategory.About);
            SettingGroups.Add(new Group<SettingItem>("通用", General));
            SettingGroups.Add(new Group<SettingItem>("关于", About));
        }

        /// <summary>
        /// 设置窗口内容
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SetContentAsync(string value)
        {
            Content = await _db.GetSettingItemContentAsync(value);
        }

        /// <summary>
        /// 根据value执行不同操作（复制分享内容/打开链接/发送邮件）
        /// </summary>
        /// <param name="value">按钮内容</param>
        /// <returns></returns>
        public async Task GoToAsync(string value)
        {
            switch (value)
            {
                case "去支持":
                    await Launcher.OpenAsync(new Uri("https://github.com/chene886/MAUI-BrilliantSee"));
                    break;

                case "去分享":
                    var content = await _db.GetSettingItemContentAsync("去分享");
                    await Clipboard.SetTextAsync(content);
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