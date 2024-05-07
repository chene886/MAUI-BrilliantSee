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
        [ObservableProperty]
        public SourceCategory _currentCategory = SourceCategory.All;

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
            foreach (var item in objs.Where(i => i.IsHide == false))
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
        /// <param name="obj">实体</param>
        /// <returns></returns>
        [RelayCommand]
        private async Task ClearObjAsync(Obj obj)
        {
            await _db.DeleteObjAsync(obj, obj.Category);
            Objs.Remove(obj);
            _ms.WriteMessage("已删除历史记录");
        }

        /// <summary>
        /// 收藏指定的实体
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task AddFavoriteAsync(Obj obj)
        {
            var isExist = await _db.IsComicExistAsync(obj, DBObjCategory.Favorite);
            if (isExist)
            {
                _ms.WriteMessage("已是收藏");
            }
            else
            {
                obj.Category = DBObjCategory.Favorite;
                await _db.SaveObjAsync(obj, obj.Category);
                obj.Category = DBObjCategory.History;
                _ms.WriteMessage("收藏成功");
            }
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