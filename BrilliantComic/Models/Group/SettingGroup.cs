using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Group
{
    public class SettingGroup : List<SettingItem>
    {
        public string Name { get; set; }

        public SettingGroup(string name, List<SettingItem> items) : base(items)
        {
            Name = name;
        }
    }
}