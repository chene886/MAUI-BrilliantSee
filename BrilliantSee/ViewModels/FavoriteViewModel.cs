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

        /// <summary>
        /// 当前选中的类别
        /// </summary>
        [ObservableProperty]
        public SourceCategory _currentCategory = SourceCategory.All;

        ///// <summary>
        ///// 是否正在获取结果
        ///// </summary>
        //[ObservableProperty]
        //private bool _isGettingResult;

        /// <summary>
        /// 是否正在检查实体更新
        /// </summary>
        [ObservableProperty]
        private bool _isCheckingUpdate = false;

        /// <summary>
        /// 初始化模型所需的配置
        /// </summary>
        public List<SettingItem> modelConfigs { get; set; } = new List<SettingItem>();

        /// <summary>
        /// 储存收藏实体集合
        /// </summary>
        public ObservableCollection<Obj> Objs { get; set; } = new();

        /// <summary>
        /// 触发隐藏弹窗提示
        /// </summary>
        public event Action ShowHideTip = delegate { };

        [ObservableProperty]
        public bool _isShowHideObj = false;

        /// <summary>
        /// 加载收藏实体
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadFavoriteObjAsync()
        {
            Objs.Clear();
            var objs = await _db.GetObjsAsync(DBObjCategory.Favorite, CurrentCategory);
            foreach (var item in objs.Where(i => i.IsHide == IsShowHideObj))
            {
                Objs.Insert(0, item);
            }
        }

        public FavoriteViewModel(DBService db, MessageService ms)
        {
            _db = db;
            _ms = ms;
            _ai = MauiProgram.servicesProvider!.GetRequiredService<AIService>();
            _ = InitKernelAsync();
        }

        /// <summary>
        /// 初始化模型
        /// </summary>
        /// <returns></returns>
        private async Task InitKernelAsync()
        {
            modelConfigs = await _db.GetSettingItemsAsync((int)SettingItemCategory.AIModel);
            var modelId = modelConfigs.Where(s => s.Name == "ModelId").First().ValueString;
            var apiKey = modelConfigs.Where(s => s.Name == "ApiKey").First().ValueString;
            var apiUrl = modelConfigs.Where(s => s.Name == "ApiUrl").First().ValueString;
            if (modelId != "" && apiKey != "" && apiUrl != "")
            {
                _ai.InitKernel(modelId, apiKey, apiUrl);
                _ai.hasModel = true;
            }
        }

        /// <summary>
        /// 导航到详情页或视频页并传递实体对象
        /// </summary>
        /// <param name="obj">指定打开的实体</param>
        /// <returns></returns>
        [RelayCommand]
        private async Task OpenObjAsync(Obj obj)
        {
            var page = obj.SourceCategory == SourceCategory.Video ? "VideoPage" : "DetailPage";
            await Shell.Current.GoToAsync(page, new Dictionary<string, object> { { "Obj", obj } });
        }

        /// <summary>
        /// 取消收藏指定的实体
        /// </summary>
        /// <param name="comic"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task CancelFavoriteAsync(Obj comic)
        {
            Objs.Remove(comic);
            await _db.DeleteObjAsync(comic, comic.Category);
            if (comic.IsHide) await _db.DeleteObjAsync(comic, DBObjCategory.History);
            _ms.WriteMessage("已取消收藏");
        }

        /// <summary>
        /// 隐藏或取消隐藏指定的实体，判断是否显示隐藏提示
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task HideObjAsync(Obj obj)
        {
            var message = obj.IsHide ? "已取消隐藏" : "已隐藏";
            obj.IsHide = !obj.IsHide;
            await _db.SaveObjAsync(obj, DBObjCategory.Favorite);
            await _db.SaveObjAsync(obj, DBObjCategory.History);
            var HideTip = await _db.GetSettingItemsAsync((int)SettingItemCategory.Tip);
            if (HideTip.Any() && HideTip.First().ValueInt == 1)
            {
                ShowHideTip.Invoke();
            }
            Objs.Remove(obj);
            _ms.WriteMessage(message);
        }

        /// <summary>
        /// 不再显示隐藏提示
        /// </summary>
        /// <returns></returns>
        public async Task DontShowAgain()
        {
            var HideTip = await _db.GetSettingItemsAsync((int)SettingItemCategory.Tip);
            var Tip = HideTip.First();
            Tip.ValueInt = 0;
            await _db.UpdateSettingItemAsync(Tip);
        }

        /// <summary>
        /// 检查收藏实体是否有更新
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task CheckForUpdatedAsync() =>
            await Task.Run(async () =>
                {
                    _ms.WriteMessage("正在更新收藏内容...");
                    var hasUpdate = false;
                    var message = "暂无收藏内容";
                    _ = MainThread.InvokeOnMainThreadAsync(() => { IsCheckingUpdate = true; });
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
                        IsCheckingUpdate = false;
                        _ms.WriteMessage(message);
                    });
                });

        /// <summary>
        /// 跳转页面
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task JumpPageAsync(string page)
        {
            await Shell.Current.GoToAsync(page);
        }

        /// <summary>
        /// 更改当前类别
        /// </summary>
        /// <param name="category"></param>
        public void ChangeCurrentCategory(SourceCategory category)
        {
            CurrentCategory = category;
        }

        [RelayCommand]
        private async Task ShowHideObjAsync()
        {
            IsShowHideObj = !IsShowHideObj;
            var message = IsShowHideObj ? "已进入隐藏视图" : "已退出隐藏视图";
            await OnLoadFavoriteObjAsync();
            _ms.WriteMessage(message);
        }
    }
}