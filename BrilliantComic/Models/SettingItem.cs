using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Models
{
    public class SettingItem
    {
        /// <summary>
        /// 储存数据库的主键
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;
    }
}