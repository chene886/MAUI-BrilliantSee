using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Enums;
using BrilliantSee.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BrilliantSee.ViewModels
{
    public partial class HistoryViewModel : ObservableObject
    {
        public readonly DBService _db;
        private readonly MessageService _ms;
        //public readonly AIService _ai;

        /// <summary>
        /// 当前选中的类别
        /// </summary>
        public SourceCategory CurrentCategory { get; set; } = SourceCategory.All;

        ///// <summary>
        ///// 是否正在获取结果
        ///// </summary>
        //[ObservableProperty]
        //private bool _isGettingResult;

        /// <summary>
        /// 储存历史记录的集合
        /// </summary>
        public ObservableCollection<Obj> Objs { get; set; } = new();

        /// <summary>
        /// 加载历史记录
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadHistoryObjAsync()
        {
            Objs.Clear();
            var objs = await _db.GetObjsAsync(Models.Enums.DBObjCategory.History, CurrentCategory);
            foreach (var item in objs)
            {
                Objs.Insert(0, item);
            }
        }

        public HistoryViewModel(DBService db, MessageService ms)
        {
            _db = db;
            _ms = ms;
            //_ai = MauiProgram.servicesProvider!.GetRequiredService<AIService>();
        }

        /// <summary>
        /// 清空历史记录
        /// </summary>
        /// <returns></returns>
        public async Task ClearHistoryObjsAsync()
        {
            if (Objs.Count == 0)
            {
                _ms.WriteMessage("暂无历史记录");
                return;
            }
            foreach (var item in Objs)
            {
                await _db.DeleteObjAsync(item, item.Category);
            }
            Objs.Clear();
            _ms.WriteMessage("历史记录已清空");
        }

        /// <summary>
        /// 导航到详情页或视频页并传递实体
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
        /// 清除指定的历史记录
        /// </summary>
        /// <param name="comic"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task ClearObjAsync(Obj comic)
        {
            await _db.DeleteObjAsync(comic, comic.Category);
            Objs.Remove(comic);
        }

        /// <summary>
        /// 更改当前类别
        /// </summary>
        /// <param name="category"></param>
        public void ChangeCurrentCategory(SourceCategory category)
        {
            CurrentCategory = category;
        }
    }
}