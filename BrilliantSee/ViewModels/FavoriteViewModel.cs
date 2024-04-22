using BrilliantSee.Models;
using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Enums;
using BrilliantSee.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.ViewModels
{
    public partial class FavoriteViewModel : ObservableObject
    {
        public readonly DBService _db;
        public readonly AIService _ai;

        public SourceCategory CurrentCategory { get; set; } = SourceCategory.Comic;

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
            IsGettingResult = true;
            var comics = await _db.GetObjsAsync(DBObjCategory.Favorite, CurrentCategory);
            comics.Reverse();
            foreach (var item in comics)
            {
                Objs.Add(item);
            }
            if (!IsRefresh)
            {
                IsGettingResult = false;
            }
        }

        public FavoriteViewModel(DBService db)
        {
            _db = db;
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
            var page = obj.SourceCategory == SourceCategory.Comic ? "DetailPage" : "VideoPage";
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
                    var objs = await _db.GetObjsAsync(DBObjCategory.Favorite, CurrentCategory);
                    if (objs.Count() != 0)
                    {
                        foreach (var item in objs)
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
                                        _ = Toast.Make(message1).Show();
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
                        _ = Toast.Make(message).Show();
                    });
                });

        [RelayCommand]
        private async Task JumpToAIPage()
        {
            await Shell.Current.GoToAsync("AIPage");
        }
    }
}