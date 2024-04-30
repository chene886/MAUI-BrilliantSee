using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.Models.Enums
{
    /// <summary>
    /// 漫画图片项状态
    /// </summary>
    public enum ComicImageItemState
    {
        /// <summary>
        /// 加载中
        /// </summary>
        Loading,

        /// <summary>
        /// 加载成功
        /// </summary>
        Success,

        /// <summary>
        /// 加载失败
        /// </summary>
        Failed
    }
}