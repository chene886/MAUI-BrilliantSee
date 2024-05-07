using BrilliantSee.Models.Items;
using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Enums;
using BrilliantSee.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BrilliantSee.ViewModels
{
    public partial class DetailViewModel : ObservableObject, IQueryAttributable
    {
        public readonly DBService _db;
        public readonly MessageService _ms;
        //private readonly AIService _ai;

        /// <summary>
        /// 当前实体
        /// </summary>
        [ObservableProperty]
        private Obj? _obj;

        /// <summary>
        /// 视频url
        /// </summary>
        [ObservableProperty]
        public string _videoUrl = string.Empty;

        /// <summary>
        /// 是否正在设置视频
        /// </summary>
        [ObservableProperty]
        public bool _isSettingVideo = false;

        /// <summary>
        /// 当前线路
        /// </summary>
        [ObservableProperty]
        public string _currentRoute = "线路一";

        /// <summary>
        /// 收藏图标
        /// </summary>
        [ObservableProperty]
        private ImageSource? _favoriteImage;

        /// <summary>
        /// 排序图标
        /// </summary>
        [ObservableProperty]
        private ImageSource? _orderImage = ImageSource.FromFile("reverse.png");

        /// <summary>
        /// 是否允许反转章节列表
        /// </summary>
        [ObservableProperty]
        private bool _isReverseListEnabled;

        /// <summary>
        /// 是否正在获取结果
        /// </summary>
        [ObservableProperty]
        private bool _isGettingResult;

        /// <summary>
        /// 当前显示的剧集(视频页)
        /// </summary>
        [ObservableProperty]
        public IEnumerable<Item> _itemsOnDisPlay = new List<Item>();

        public DetailViewModel(DBService db, MessageService ms)
        {
            _db = db;
            _ms = ms;
            //_ai = MauiProgram.servicesProvider!.GetRequiredService<AIService>();
            //if (_ai.hasModel)
            //{
            //    _ai.RemovePlugins();
            //    _ai.ImportPlugins(new Services.Plugins.DetailPlugins(_db));
            //}
        }

        /// <summary>
        /// 设置当前实体，加载更多实体数据，储存为历史记录
        /// </summary>
        /// <param name="query">储存导航传递数据的字典</param>
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (Obj is not null)
            {
                return;
            }
            Obj = query["Obj"] as Obj;

            IsGettingResult = true;
            IsReverseListEnabled = false;
            //判断是否已收藏，设置收藏图标
            var isExist = await _db.IsComicExistAsync(Obj!, DBObjCategory.Favorite);
            if (isExist)
            {
                FavoriteImage = ImageSource.FromFile("is_favorite.png");
                Obj!.Category = DBObjCategory.Favorite;
            }
            else FavoriteImage = ImageSource.FromFile("not_favorite.png");
            //如果数据未加载，加载更多数据和章节或剧集
            var isSuccess = true;
            if (!Obj!.Items.Any())
            {
                isSuccess = await Obj!.GetHtmlAsync();
                if (isSuccess)
                {
                    Obj!.LoadMoreData();
                    OnPropertyChanged(nameof(Obj));
                    await Task.Run(() => Obj!.LoadItemsAsync());
                    if (isExist && Obj!.IsUpdate)
                    {
                        Obj!.IsUpdate = false;
                        _ = _db.UpdateComicAsync(Obj!);
                    }
                }
                else _ms.WriteMessage("好像出了点小问题，用浏览器打开试试吧");
            }
            ItemsOnDisPlay = new ObservableCollection<Item>(Obj!.Items.Where(c => c.Route == "线路一"));
            IsReverseListEnabled = true;
            IsGettingResult = false;
            if (isSuccess) _ = AddHistoryAsync();
        }

        /// <summary>
        /// 添加当前实体历史记录
        /// </summary>
        private async Task AddHistoryAsync()
        {
            await _db.SaveObjAsync(Obj!, DBObjCategory.History);
        }

        /// <summary>
        /// 切换收藏状态并储存实体或删除实体
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SwitchIsFavorite()
        {
            if (Obj is null)
            {
                return;
            }
            if (Obj.Category == DBObjCategory.Favorite)
            {
                FavoriteImage = ImageSource.FromFile("not_favorite.png");
                await _db.DeleteObjAsync(Obj, Obj.Category);
                Obj.Category = DBObjCategory.History;
            }
            else
            {
                FavoriteImage = ImageSource.FromFile("is_favorite.png");
                Obj.Category = DBObjCategory.Favorite;
                await _db.SaveObjAsync(Obj, Obj.Category);
            }
            OnPropertyChanged(nameof(Obj));
        }

        /// <summary>
        /// 跳转到浏览器浏览
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task JumpToBrowserAsync()
        {
            await Launcher.OpenAsync(new Uri(Obj!.Url));
        }

        /// <summary>
        /// 章节或剧集列表倒序
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task ReverseListAsync()
        {
            IsGettingResult = true;
            IsReverseListEnabled = false;
            Obj!.IsReverseList = !Obj.IsReverseList;
            //切换排序图标
            if (!Obj!.IsReverseList)
            {
                OrderImage = ImageSource.FromFile("positive.png");
            }
            else
            {
                OrderImage = ImageSource.FromFile("reverse.png");
            }
            await Task.Run(() =>
            {
                Obj.Items = Obj.Items.Reverse();
                if (ItemsOnDisPlay.Any())
                {
                    ItemsOnDisPlay = ItemsOnDisPlay.Reverse();
                }
            });
            IsReverseListEnabled = true;
            IsGettingResult = false;
        }

        /// <summary>
        /// 跳转到漫画浏览页或小说页
        /// </summary>
        /// <param name="chapter">导航传递章节</param>
        /// <returns></returns>
        [RelayCommand]
        private async Task OpenChapterAsync(Item chapter)
        {
            if (chapter.Name != "暂无章节")
            {
                var page = chapter.Obj.SourceCategory == SourceCategory.Comic ? "BrowsePage" : "NovelPage";
                await Shell.Current.GoToAsync(page, new Dictionary<string, object> { { "Chapter", chapter } });
            }
            else
            {
                _ms.WriteMessage("章节无法显示");
            }
        }

        /// <summary>
        /// 设置媒体播放器播放指定视频url，若未获取视频url则获取
        /// </summary>
        /// <param name="video">指定的视频</param>
        /// <returns></returns>
        [RelayCommand]
        private async Task SetVideoAsync(Item video)
        {
            IsSettingVideo = true;
            if (video.Index != Obj!.LastReadedItemIndex)
            {
                _ = video.Obj.ChangeLastReadedItemIndex(video.Index, _db);
            }
            if (video.VideoUrl == string.Empty)
            {
                try
                {
                    await video.GetResourcesAsync();
                }
                catch (Exception e)
                {
                    if (e.Message == "请求失败") _ms.WriteMessage(e.Message);
                    else _ms.WriteMessage("好像出了点小问题，用浏览器打开试试吧");
                }
            }
            VideoUrl = video.VideoUrl;
            IsSettingVideo = false;
        }

        /// <summary>
        /// 打开最后浏览记录（若有）
        /// </summary>
        [RelayCommand]
        private async Task OpenHistoryAsync()
        {
            if (Obj!.LastReadedItemIndex != -1 && Obj.Items.Any())
            {
                var historyItem = Obj.Items.ToList().Find(c => c.Index == Obj.LastReadedItemIndex);
                if (Obj.SourceCategory == SourceCategory.Video) await SetVideoAsync(historyItem!);
                else await OpenChapterAsync(historyItem!);
            }
            else
            {
                _ms.WriteMessage("暂无章节浏览记录");
            }
        }

        /// <summary>
        /// 设置当前线路的剧集集合
        /// </summary>
        public void SetItemsOnDisplay()
        {
            ItemsOnDisPlay = Obj!.Items.Where(c => c.Route == CurrentRoute);
        }
    }
}