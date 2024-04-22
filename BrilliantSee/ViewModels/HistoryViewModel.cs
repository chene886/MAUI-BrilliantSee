using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Enums;
using BrilliantSee.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.ViewModels
{
    public partial class HistoryViewModel : ObservableObject
    {
        public readonly DBService _db;
        public readonly AIService _ai;

        public SourceCategory CurrentCategory { get; set; } = SourceCategory.Comic;

        /// <summary>
        /// 是否正在获取结果
        /// </summary>
        [ObservableProperty]
        private bool _isGettingResult;

        /// <summary>
        /// 储存历史漫画的集合
        /// </summary>
        public ObservableCollection<Obj> Objs { get; set; } = new();

        /// <summary>
        /// 加载历史漫画
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadHistoryObjAsync()
        {
            Objs.Clear();
            IsGettingResult = true;
            var objs = await _db.GetObjsAsync(Models.Enums.DBObjCategory.History, CurrentCategory);
            objs.Reverse();
            foreach (var item in objs)
            {
                Objs.Add(item);
            }
            IsGettingResult = false;
        }

        public HistoryViewModel(DBService db)
        {
            _db = db;
            _ai = MauiProgram.servicesProvider!.GetRequiredService<AIService>();
        }

        /// <summary>
        /// 清空历史漫画
        /// </summary>
        /// <returns></returns>
        public async Task ClearHistoryObjsAsync()
        {
            if (Objs.Count == 0)
            {
                _ = Toast.Make("暂无历史记录").Show();
                return;
            }
            foreach (var item in Objs)
            {
                await _db.DeleteObjAsync(item, item.Category);
            }
            Objs.Clear();
            _ = Toast.Make("历史记录已清空").Show();
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
        private async Task ClearObjAsync(Obj comic)
        {
            await _db.DeleteObjAsync(comic, comic.Category);
            Objs.Remove(comic);
        }
    }
}