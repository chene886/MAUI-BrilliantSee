using CommunityToolkit.Maui.Alerts;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Services.Plugins.Other
{
    public sealed class OtherPlugin
    {
        [KernelFunction, Description("查找失败或获取失败或删除失败（结果为空或为False）提示用户")]
        private async Task<bool> FailTaskAsync()
        {
            await Toast.Make("任务失败，请检查输入是否有误！").Show();
            return true;
        }
    }
}