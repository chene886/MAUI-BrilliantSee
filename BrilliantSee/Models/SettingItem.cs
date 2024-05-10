using BrilliantSee.Models.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.Models
{
    public class SettingItem
    {
        /// <summary>
        /// 储存数据库的主键
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string ValueString { get; set; } = string.Empty;

        public int ValueInt { get; set; } = 0;

        public int Category { get; set; } = (int)SettingItemCategory.Default;

        public string Content { get; set; } = string.Empty;
    }
}