using BrilliantSee.Models;
using BrilliantSee.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.Services
{
    public class ComicImageManageService
    {
        public ComicImageManageService()
        {
        }

        /// <summary>
        /// 获取漫画图片项
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ComicImageItem GetComicImageItem(string url)
        {
            var item = new ComicImageItem(url);
            item.LoadImageTask = LoadImageAsync(item);

            return item;
        }

        /// <summary>
        /// 重试加载图片
        /// </summary>
        /// <param name="item"></param>
        public void RetryLoadImage(ComicImageItem item)
        {
            item.LoadImageTask = LoadImageAsync(item);
        }

        /// <summary>
        /// 取消加载图片
        /// </summary>
        /// <param name="item"></param>
        public void CancelLoadImage(ComicImageItem item)
        {
            item.Cts.Cancel();
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Task LoadImageAsync(ComicImageItem item) => Task.Run(async () =>
        {
            if (item.Cts.IsCancellationRequested)
                item.Cts = new CancellationTokenSource();

            item.State = ComicImageItemState.Loading;

            //这里可以做一下缓存机制

            //加载图片
            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(item.Url);
            var source = ImageSource.FromStream(() => new MemoryStream(bytes));

            item.Source = source;
            item.State = ComicImageItemState.Success;
        })
            .ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    item.State = ComicImageItemState.Failed;
                }
            });
    }
}