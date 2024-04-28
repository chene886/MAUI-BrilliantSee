using BrilliantSee.Models;
using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Enums;
using BrilliantSee.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BrilliantSee.ViewModels
{
    public partial class FavoriteViewModel : ObservableObject
    {
        public readonly DBService _db;
        public readonly AIService _ai;
        private readonly MessageService _ms;

        public SourceCategory CurrentCategory { get; set; } = SourceCategory.All;

        /// <summary>
        /// 是否正在获取结果
        /// </summary>
        [ObservableProperty]
        private bool _isGettingResult;

        private bool IsRefresh { get; set; } = false;

        public List<SettingItem> modelConfigs { get; set; } = new List<SettingItem>();

        /// <summary>
        /// 储存收藏漫画集合
        /// </summary>
        public ObservableCollection<Obj> Objs { get; set; } = new();

        /// <summary>
        /// 加载收藏漫画
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadFavoriteObjAsync()
        {
            Objs.Clear();
            //IsGettingResult = true;
            var objs = await _db.GetObjsAsync(DBObjCategory.Favorite, CurrentCategory);
            //comics.Reverse();
            foreach (var item in objs)
            {
                Objs.Insert(0, item);
            }
            //if (!IsRefresh)
            //{
            //    IsGettingResult = false;
            //}
        }

        public FavoriteViewModel(DBService db, MessageService ms)
        {
            _db = db;
            _ms = ms;
            _ai = MauiProgram.servicesProvider!.GetRequiredService<AIService>();
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

        /// <summary>
        /// 导航到漫画详情页并传递漫画对象
        /// </summary>
        /// <param name="obj">指定打开的实体</param>
        /// <returns></returns>
        [RelayCommand]
        private async Task OpenObjAsync(Obj obj)
        {
            var page = obj.SourceCategory == SourceCategory.Video ? "VideoPage" : "DetailPage";
            await Shell.Current.GoToAsync(page, new Dictionary<string, object> { { "Obj", obj } });
        }

        [RelayCommand]
        private async Task CancelFavoriteAsync(Obj comic)
        {
            await _db.DeleteObjAsync(comic, comic.Category);
            Objs.Remove(comic);
        }

        /// <summary>
        /// 检查收藏漫画是否有更新
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task CheckForUpdatedAsync() =>
            await Task.Run(async () =>
                {
                    var hasUpdate = false;
                    var message = "暂无收藏内容";
                    _ = MainThread.InvokeOnMainThreadAsync(() => { IsGettingResult = true; IsRefresh = true; });
                    //var objs = await _db.GetObjsAsync(DBObjCategory.Favorite, CurrentCategory);
                    if (Objs.Count() != 0)
                    {
                        foreach (var item in Objs)
                        {
                            if (!item.IsUpdate)
                            {
                                var isSuccess = await item.GetHtmlAsync();
                                if (isSuccess)
                                {
                                    var lastestChapterName = item.GetLastestItemName();
                                    if (lastestChapterName is not null && lastestChapterName != item.LastestItemName)
                                    {
                                        item.IsUpdate = true;
                                        hasUpdate = true;
                                        await _db.SaveObjAsync(item, DBObjCategory.Favorite);
                                        _ = MainThread.InvokeOnMainThreadAsync(() => _ = OnLoadFavoriteObjAsync());
                                    }
                                }
                                else
                                {
                                    var message1 = $"{item.Name}检查更新失败,不如点开看看吧";
                                    await Task.Delay(500);
                                    _ = MainThread.InvokeOnMainThreadAsync(() =>
                                    {
                                        _ms.WriteMessage(message1);
                                    });
                                    continue;
                                }
                            }
                        }
                        message = hasUpdate ? "检查更新完成" : "暂无内容更新";
                    }
                    _ = MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        IsRefresh = false;
                        IsGettingResult = false;
                        _ms.WriteMessage(message);
                    });
                });

        [RelayCommand]
        private async Task JumpToAIPage()
        {
            await Shell.Current.GoToAsync("AIPage");
        }
    }
}