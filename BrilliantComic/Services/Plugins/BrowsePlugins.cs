using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Services.Plugins
{
    public sealed class BrowsePlugins
    {
        public DBService _db;

        public BrowsePlugins(DBService db)
        {
            _db = db;
        }

        //返回功能
        [KernelFunction, Description("返回或返回某个页面，无需关注指定哪个页面，只要涉及返回都可调用")]
        public async Task GoBackAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() => { Shell.Current.SendBackButtonPressed(); });
        }
    }
}