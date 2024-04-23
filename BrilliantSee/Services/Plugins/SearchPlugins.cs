using BrilliantSee.Models.Objs;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.Services.Plugins
{
    public sealed class SearchPlugins
    {
        public DBService _db;
        public SourceService _sc;

        public SearchPlugins(DBService db, SourceService sc)
        {
            _db = db;
            _sc = sc;
        }

        //返回功能

        [KernelFunction, Description("返回或返回某个页面,无需关注指定哪个页面，只要涉及返回都可调用")]
        public async Task GoBackAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() => { Shell.Current.SendBackButtonPressed(); });
        }

        [KernelFunction, Description("搜索指定名字的漫画")]
        public async Task FindComicByNameAsync(
                                             [Description("要查找的漫画名称")] string name,
                                                [Description("存放搜索结果的漫画集合")] ObservableCollection<Obj> Comics)
        {
            //await _sc.SearchAsync(name, Comics, "Init");
        }

        [KernelFunction, Description("获取漫画集合中指定下标的漫画")]
        [return: Description("获取到的漫画（可能没有）")]
        public Obj? GetComicAsync(
                                                [Description("用于获取结果的漫画集合")] ObservableCollection<Obj> comics,
            [Description("指定的下标")] int index)
        {
            return comics[index];
        }
    }
}