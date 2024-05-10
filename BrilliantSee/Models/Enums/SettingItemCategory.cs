using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.Models.Enums
{
    public enum SettingItemCategory
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,

        /// <summary>
        /// 源
        /// </summary>
        Source = 1,

        /// <summary>
        /// 定制
        /// </summary>
        Custom = 2,

        /// <summary>
        /// 通用
        /// </summary>
        General = 3,

        /// <summary>
        /// 关于
        /// </summary>
        About = 4,

        /// <summary>
        /// AI模型
        /// </summary>
        AIModel = 5,

        /// <summary>
        /// 语音
        /// </summary>
        Audio = 6,

        /// <summary>
        /// 提示
        /// </summary>
        Tip = 7,
    }
}