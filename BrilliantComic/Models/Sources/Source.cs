using BrilliantComic.Models.Comics;
using BrilliantComic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Sources
{
    public abstract partial class Source : ObservableObject
    {
        public HttpClient? HttpClient { get; set; }

        public string Name { get; set; } = string.Empty;

        public int HasMore { get; set; } = -1;

        public int ResultNum { get; set; } = 1;

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

        public async Task<string> GetHtmlAsync(string url)
        {
            try
            {
                var response = await HttpClient!.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return string.Empty;
                }
                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return string.Empty;
            }
        }

        ///// <summary>
        ///// 解析html
        ///// </summary>
        ///// <returns></returns>
        //public string SubHtml(string html, string startString = "", string endString = "")
        //{
        //    var flag = startString == "" ? (endString == "" ? "None" : "End") : (endString == "" ? "Start" : "Both");
        //    switch (flag)
        //    {
        //        case "Start":
        //            {
        //                var startIndex = html.IndexOf(startString);
        //                html = startIndex == -1 ? html : html.Substring(startIndex);
        //                break;
        //            }
        //        case "End":
        //            {
        //                var endIndex = html.IndexOf(endString);
        //                html = endIndex == -1 ? html : html.Substring(0, endIndex);
        //                break;
        //            }
        //        case "Both":
        //            {
        //                var startIndex = html.IndexOf(startString);
        //                var endIndex = html.IndexOf(endString);
        //                html = startIndex == -1 ? (endIndex == -1 ? html : html.Substring(0, endIndex)) : (endIndex == -1 ? html.Substring(startIndex) : html.Substring(startIndex, endIndex - startIndex));
        //                break;
        //            }
        //        default:
        //            break;
        //    }
        //    return html;
        //}

        /// <summary>
        /// 搜索漫画
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns></returns>
        public abstract Task<IEnumerable<Comic>> SearchAsync(string keyword);
    }
}