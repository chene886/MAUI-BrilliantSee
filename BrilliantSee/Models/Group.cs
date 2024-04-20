using BrilliantSee.Models.Sources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.Models
{
    public class Group<T> : List<T>
    {
        public string Name { get; set; }

        public Group(string name, List<T> items) : base(items)
        {
            Name = name;
        }
    }
}