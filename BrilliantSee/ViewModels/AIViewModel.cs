using BrilliantSee.Models;
using BrilliantSee.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BrilliantSee.ViewModels
{
    public partial class AIViewModel : ObservableObject
    {
        private readonly AIService _aiService;
        private readonly DBService _db;
        private readonly MessageService _ms;

        /// <summary>
        /// 是否有模型
        /// </summary>
        public bool hasModel { get; set; } = false;

        /// <summary>
        /// 是否正在等待
        /// </summary>
        [ObservableProperty]
        public bool _isWaiting = false;

        //[ObservableProperty]
        //public ImageSource _audioIcon = ImageSource.FromFile("disable_audio.png");
        //public string AudioStatus { get; set; } = "false";

        /// <summary>
        /// 数据库模型配置
        /// </summary>
        public List<SettingItem> modelConfigs { get; set; } = new List<SettingItem>();

        public AIViewModel(DBService db, MessageService ms)
        {
            _aiService = MauiProgram.servicesProvider!.GetRequiredService<AIService>();
            _db = db;
            _ms = ms;
            hasModel = _aiService.hasModel;
            //_aiService.RemovePlugins();
        }

        //private async Task GetAudioStatus()
        //{
        //    var audio = await _db.GetSettingItemsAsync("Audio");
        //    AudioStatus = audio[0].Value;
        //}

        /// <summary>
        /// 更新模型并保存到数据库
        /// </summary>
        /// <param name="name">模型名</param>
        /// <param name="key">模型key</param>
        /// <param name="url">模型代理地址</param>
        /// <returns></returns>
        public async Task UpdateModel(string name, string key, string url)
        {
            IsWaiting = true;
            _aiService.InitKernel(name, key, url);
            IsWaiting = false;
            hasModel = true;
            _ms.WriteMessage("模型更新成功");

            //保存到数据库
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
        }

        /// <summary>
        /// 处理用户输入的字符串
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns></returns>
        public async Task<string> Chat(string prompt)
        {
            IsWaiting = true;
            var result = await Task.Run(async Task<string>? () => { return await _aiService.SolvePromptAsync(prompt); });
            IsWaiting = false;
            return result;
        }

        /// <summary>
        /// 判断字符串是否合理
        /// </summary>
        /// <param name="strs">待检测的字符串数组</param>
        /// <returns></returns>
        public bool IsAcceptableString(string[] strs)
        {
            var acceptable = true;
            foreach (var str in strs)
            {
                if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
                {
                    acceptable = false;
                    break;
                }
            }
            if (!acceptable)
                _ms.WriteMessage("请填写合理的内容");
            return acceptable;
        }
    }
}