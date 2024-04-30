using BrilliantSee.Models;
using BrilliantSee.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantSee.Services
{
    public class ComicImageManageService
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(3, 3);

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
            if (item.State != ComicImageItemState.Success)
            {
                item.Cts.Cancel();
            }
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

            await _semaphore.WaitAsync(item.Cts.Token);
            try
            {
                //判断是否存在缓存
                var basePath = Path.Combine(FileSystem.AppDataDirectory, "imagesCache");
                var key = GenerateCacheKey(item.Url);
                var cachePath = Path.Combine(basePath, key + ".jpg");

                if (!File.Exists(cachePath))
                {
                    //加载图片
                    using var client = new HttpClient();
                    var bytes = await client.GetByteArrayAsync(item.Url, item.Cts.Token);

                    //保存缓存
                    if (!Directory.Exists(basePath))
                        Directory.CreateDirectory(basePath);
                    await File.WriteAllBytesAsync(cachePath, bytes);
                }

                var source = ImageSource.FromFile(cachePath);
                item.Source = source;
                item.State = ComicImageItemState.Success;
            }
            finally
            {
                _semaphore.Release();
            }
        })
            .ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    item.State = ComicImageItemState.Failed;
                }
            });

        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GenerateCacheKey(string url)
        {
            var bytes = MD5.HashData(Encoding.UTF8.GetBytes(url));
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}