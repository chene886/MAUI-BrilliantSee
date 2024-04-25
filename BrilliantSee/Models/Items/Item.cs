using BrilliantSee.Models.Objs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BrilliantSee.Models.Items
{
    public abstract partial class Item : ObservableObject
    {
        /// <summary>
        /// 章节、剧集名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 章节、剧集url
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 章节剧集所属的实体
        /// </summary>
        public required Obj Obj { get; set; }

        /// <summary>
        /// 章节页数
        /// </summary>
        public int PageCount { get; set; } = 0;

        /// <summary>
        /// 章节剧集索引
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// 章节剧集是否为最后阅读
        /// </summary>
        [ObservableProperty]
        public bool _isSpecial = false;

        /// <summary>
        /// 保存章节图片url
        /// </summary>
        public List<string> PicUrls { get; set; } = new List<string>();

        /// <summary>
        /// 剧集所属线路
        /// </summary>
        public string Route { get; set; } = string.Empty;

        /// <summary>
        /// 剧集资源url
        /// </summary>
        public string VideoUrl { get; set; } = string.Empty;

        /// <summary>
        /// 小说内容
        /// </summary>
        [ObservableProperty]
        public string _novelContent = "\t\t\t\t\t\t";

        public Item(string name, string url, int index, bool isSpecial)
        {
            Name = name;
            Url = url;
            Index = index;
            IsSpecial = isSpecial;
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <returns></returns>
        public abstract Task GetResourcesAsync();
    }
}