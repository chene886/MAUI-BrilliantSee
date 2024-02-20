﻿using BrilliantComic.Models.Comics;
using BrilliantComic.Models.Sources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Services
{
    public class SourceService
    {
        /// <summary>
        /// 储存图源的字典
        /// </summary>
        private Dictionary<string, ISource> _sources = new();

        /// <summary>
        /// 储存图源名的字典
        /// </summary>
        private Dictionary<ISource, string> _sourceNames = new();

        /// <summary>
        /// 注册图源
        /// </summary>
        public SourceService()
        {
            var baozi = new BaoziSource(this);
            _sources.Add("BaoZi", baozi);
            _sourceNames.Add(baozi, "BaoZi");
        }

        /// <summary>
        /// 搜索漫画
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="comics">保存结果的集合</param>
        /// <returns></returns>
        public async Task SearchAsync(string keyword, ObservableCollection<Comic> comics)
        {
            //并发使用所有图源去搜索
            var tasks = new List<Task>();
            foreach (var source in _sources.Values)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var result = await source.SearchAsync(keyword);
                    foreach (var item in result)
                    {
                        //委托到UI线程将漫画添加到集合
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            comics.Add(item);
                        });
                    }
                }));
            }

            //等待所有图源搜索完成
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 根据图源名获取图源实体
        /// </summary>
        /// <param name="source">图源名</param>
        /// <returns></returns>
        public ISource? GetSource(string source)
        {
            _sources.TryGetValue(source, out var result);
            return result;
        }

        /// <summary>
        /// 根据图源实体获取图源名
        /// </summary>
        /// <param name="source">图源</param>
        /// <returns></returns>
        public string? GetSourceName(ISource source)
        {
            _sourceNames.TryGetValue(source, out var result);
            return result;
        }
    }
}