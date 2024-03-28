﻿using BrilliantComic.Models.Comics;
using BrilliantComic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Sources
{
    public abstract partial class Source : ObservableObject
    {
        public HttpClient? HttpClient { get; set; }

        public string Name { get; set; } = string.Empty;

        [ObservableProperty]
        public bool _isSelected = false;

        /// <summary>
        /// 配置HttpClient
        /// </summary>
        /// <param name="referer">请求头Referer</param>
        public void SetHttpClient(string referer)
        {
            HttpClient = new HttpClient(new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip
            })
            {
                DefaultRequestHeaders =
            {
                { "User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 16_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.6 Mobile/15E148 Safari/604.1 Edg/122.0.0.0"},
                { "Referer", referer}
            }
            };
        }

        /// <summary>
        /// 搜索漫画
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns></returns>
        public abstract Task<IEnumerable<Comic>> SearchAsync(string keyword);
    }
}