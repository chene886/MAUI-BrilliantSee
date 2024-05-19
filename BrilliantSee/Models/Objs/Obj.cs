using BrilliantSee.Models.Items;
using BrilliantSee.Models.Enums;
using BrilliantSee.Models.Sources;
using BrilliantSee.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.Models.Objs
{
    public abstract partial class Obj : ObservableObject
    {
        /// <summary>
        /// 储存数据库的主键
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// 漫画html
        /// </summary>
        public string Html { get; set; } = string.Empty;

        /// <summary>
        /// 封面链接
        /// </summary>
        [ObservableProperty]
        public string _cover = string.Empty;

        /// <summary>
        /// 漫画名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 漫画作者
        /// </summary>
        [ObservableProperty]
        public string _author = "(暂无作者信息)";

        /// <summary>
        /// 漫画简介
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 漫画链接
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 最后阅读章节索引
        /// </summary>
        public int LastReadedItemIndex { get; set; } = -1;

        /// <summary>
        /// 漫画源
        /// </summary>
        public required Source Source { get; set; }

        /// <summary>
        /// 漫画源名
        /// </summary>
        public string SourceName { get; set; } = string.Empty;

        /// <summary>
        /// 最新章节名
        /// </summary>
        public string LastestItemName { get; set; } = string.Empty;

        /// <summary>
        /// 最新更新时间
        /// </summary>
        [ObservableProperty]
        public string _lastestUpdateTime = "(暂无最后更新信息)";

        /// <summary>
        /// 是否有更新
        /// </summary>
        public bool IsUpdate { get; set; } = false;

        /// <summary>
        /// 漫画状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 漫画章节
        /// </summary>
        [ObservableProperty]
        public IEnumerable<Item> _items = new List<Item>();

        /// <summary>
        /// 章节数量
        /// </summary>
        public int ItemCount { get; set; } = 0;

        /// <summary>
        /// 章节是否倒序
        /// </summary>
        public bool IsReverseList { get; set; } = true;

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsHide { get; set; } = false;

        /// <summary>
        /// 实体分类
        /// </summary>
        public DBObjCategory Category { get; set; } = DBObjCategory.Default;

        /// <summary>
        /// 实体来源分类
        /// </summary>
        public SourceCategory SourceCategory { get; set; }

        //影视
        public string Director { get; set; } = string.Empty;

        public string Actors { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;

        //小说
        public string CharCount { get; set; } = string.Empty;

        //音乐
        //public string Album { get; set; } = string.Empty;

        //public string Singer { get; set; } = string.Empty;
        //public Dictionary<string, string> Lyrics { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 从存储的漫画数据创建漫画实体
        /// </summary>
        /// <param name="dbObj"></param>
        /// <returns></returns>
        public Obj CreateObjFromDBObj(DBObj dbObj)
        {
            Url = dbObj.Url;
            Name = dbObj.Name;
            Cover = dbObj.Cover;
            Author = dbObj.Author;
            Id = dbObj.Id;
            Category = dbObj.Category;
            SourceCategory = dbObj.SourceCategory;
            LastReadedItemIndex = dbObj.LastReadedItemIndex;
            IsUpdate = dbObj.IsUpdate;
            IsHide = dbObj.IsHide;
            LastestItemName = dbObj.LastestItemName;
            SourceName = dbObj.SourceName;
            return (Obj)this.MemberwiseClone();
        }

        /// <summary>
        /// 从漫画实体创建存储的漫画数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public DBObj CreateDBObjFromObj(Obj obj)
        {
            return new DBObj
            {
                Id = obj.Id,
                Name = obj.Name,
                Author = obj.Author,
                Cover = obj.Cover,
                Source = this.SourceName,
                Url = obj.Url,
                Category = obj.Category,
                SourceCategory = obj.SourceCategory,
                LastReadedItemIndex = obj.LastReadedItemIndex,
                IsUpdate = obj.IsUpdate,
                IsHide = obj.IsHide,
                LastestItemName = obj.LastestItemName,
                SourceName = obj.SourceName,
            };
        }

        /// <summary>
        /// 获取网站html
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetHtmlAsync()
        {
            Html = await Source.GetHtmlAsync(Url);
            if (Html == string.Empty)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 从当前章节获取上一章节或下一章节
        /// </summary>
        /// <param name="chapter">当前章节</param>
        /// <param name="flag">获取上一章节或下一章节的标志</param>
        /// <returns></returns>
        public Item? GetNewItem(Item chapter, string flag)
        {
            var tempItems = Items.ToList();
            int index = tempItems.IndexOf(chapter);
            bool turn = flag == "Last";
            index = IsReverseList == turn ? index + 1 : index - 1;
            if (index < 0 || index >= Items.Count()) return null;
            return Items.ElementAtOrDefault(index)!;
        }

        public async Task ChangeLastReadedItemIndex(int index, DBService _db)
        {
            if (index != LastReadedItemIndex)
            {
                var position = IsReverseList ? Items.Count() - index - 1 : index;
                Items.ToList()[position].IsSpecial = true;
                if (LastReadedItemIndex != -1)
                {
                    position = IsReverseList ? Items.Count() - LastReadedItemIndex - 1 : LastReadedItemIndex;
                    Items.ToList()[position].IsSpecial = false;
                }
                LastReadedItemIndex = index;
                var category = Category;
                Category = DBObjCategory.History;
                await _db.UpdateComicAsync(this);
                if (await _db.IsComicExistAsync(this, DBObjCategory.Favorite))
                {
                    Category = DBObjCategory.Favorite;
                    await _db.UpdateComicAsync(this);
                }
                Category = category;
            }
        }

        public async Task PreLoadAsync(Item item, DBService _db)
        {
            var mode = await _db.GetSettingItemsAsync((int)SettingItemCategory.Custom);
            if (mode.Any())
            {
                try
                {
                    if (mode.First().ValueInt == (int)PreLoadMode.None) return;
                    var nextItem = GetNewItem(item, "Next");
                    if (nextItem is not null && !nextItem.PicUrls.Any() && nextItem.NovelContent == string.Empty && nextItem.VideoUrl == string.Empty)
                    {
                        _ = nextItem.GetResourcesAsync();
                    }
                    if (mode.First().ValueInt == (int)PreLoadMode.Both)
                    {
                        var lastItem = GetNewItem(item, "Last");
                        if (lastItem is not null && !lastItem.PicUrls.Any() && lastItem.NovelContent == string.Empty && lastItem.VideoUrl == string.Empty)
                        {
                            _ = lastItem.GetResourcesAsync();
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// 获取更多漫画数据
        /// </summary>
        /// <returns></returns>
        public abstract void LoadMoreData();

        /// <summary>
        /// 获取最新章节名
        /// </summary>
        /// <returns></returns>
        public abstract string? GetLastestItemName();

        /// <summary>
        /// 加载章节信息
        /// </summary>
        /// <returns></returns>
        public abstract Task LoadItemsAsync();
    }
}