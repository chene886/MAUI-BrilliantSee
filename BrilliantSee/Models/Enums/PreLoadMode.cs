using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.Models.Enums
{
    public enum PreLoadMode
    {
        /// <summary>
        /// 不提前加载
        /// </summary>
        None = 0,

        /// <summary>
        /// 提前加载下一章节
        /// </summary>
        Next = 1,

        /// <summary>
        /// 提前加载上一章节和下一章节
        /// </summary>
        Both = 2,
    }
}