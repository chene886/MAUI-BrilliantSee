using BrilliantSee.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.Models
{
    /// <summary>
    /// 漫画图片项
    /// </summary>
    public partial class ComicImageItem : ObservableObject
    {
        /// <summary>
        /// 取消任务token
        /// </summary>
        public CancellationTokenSource Cts { get; set; } = new();

        /// <summary>
        /// 加载图片任务
        /// </summary>
        public Task? LoadImageTask { get; set; }

        /// <summary>
        /// 源链接
        /// </summary>
        [ObservableProperty]
        private string _url = string.Empty;

        /// <summary>
        /// 图片源
        /// </summary>
        [ObservableProperty]
        private ImageSource? _source;

        /// <summary>
        /// 漫画图片项状态
        /// </summary>
        [ObservableProperty]
        private ComicImageItemState _state = ComicImageItemState.Loading;

        public ComicImageItem(string url)
        {
            _url = url;
        }
    }
}