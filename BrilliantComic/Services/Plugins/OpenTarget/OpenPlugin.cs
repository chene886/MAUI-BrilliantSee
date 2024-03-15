using BrilliantComic.Models.Chapters;
using BrilliantComic.Models.Comics;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Services.Plugins.OpenTarget
{
    public sealed class OpenPlugin
    {
        [KernelFunction, Description("打开指定的漫画")]
        private async Task OpenComicAsync(
                       [Description("要打开的漫画")] Comic comic)
        {
            await Shell.Current.GoToAsync("DetailPage", new Dictionary<string, object> { { "Comic", comic } });
        }

        [KernelFunction, Description("打开指定的章节")]
        private async Task OpenChapterAsync(
                       [Description("要打开的章节")] Chapter chapter)
        {
            await Shell.Current.GoToAsync("ChapterPage", new Dictionary<string, object> { { "Chapter", chapter } });
        }
    }
}