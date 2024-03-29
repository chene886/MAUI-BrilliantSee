using BrilliantComic.Models.Comics;
using BrilliantComic.Models.Sources;
using CommunityToolkit.Maui.Alerts;
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
        private Dictionary<string, Source> _sources = new();

        /// <summary>
        /// 储存漫画的字典
        /// </summary>
        private Dictionary<string, Comic> _comics = new();

        /// <summary>
        /// 注册图源
        /// </summary>
        public SourceService()
        {
            var baozi = new BaoziSource();
            var gufeng = new GufengSource();
            var goda = new GodaSource();
            var baoziComic = new BaoziComic() { Source = baozi };
            var gufengComic = new GufengComic() { Source = gufeng };
            var godaComic = new GodaComic() { Source = goda };
            //var hasu = new HasuSource();
            _sources.Add(baozi.Name, baozi);
            _sources.Add(gufeng.Name, gufeng);
            _sources.Add(goda.Name, goda);
            _comics.Add(baozi.Name, baoziComic);
            _comics.Add(gufeng.Name, gufengComic);
            _comics.Add(goda.Name, godaComic);
            //_sources.Add(hasu.Name, hasu);
        }

        /// <summary>
        /// 获取所有图源
        /// </summary>
        /// <returns></returns>
        public List<Source> GetSources()
        {
            return _sources.Values.ToList();
        }

        /// <summary>
        /// 搜索漫画
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="comics">保存结果的集合</param>
        /// <returns></returns>
        public async Task SearchAsync(string keyword, ObservableCollection<Comic> comics, string flag)
        {
            IEnumerable<Source> sources;
            if (flag == "Default")
            {
                sources = _sources.Values.Where(s => s.IsSelected == true);
            }
            else
            {
                sources = _sources.Values;
            }
            //并发使用所有图源去搜索
            var tasks = new List<Task>();
            foreach (var source in sources)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var result = await source.SearchAsync(keyword)!;
                    if (result.Any())
                    {
                        foreach (var item in result)
                        {
                            if (item == result.First() && comics.Any())
                            {
                                await MainThread.InvokeOnMainThreadAsync(() =>
                                {
                                    comics.Insert(1, item);
                                });
                                continue;
                            }
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                comics.Add(item);
                            });
                        }
                    }
                }));
            }

            //等待所有图源搜索完成
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 根据图源名获取图源实体
        /// </summary>
        /// <param name="name">图源名</param>
        /// <returns></returns>
        public Source? GetSource(string name)
        {
            _sources.TryGetValue(name, out var result);
            return result;
        }

        /// <summary>
        /// 根据图源名获取图源漫画实体
        /// </summary>
        /// <param name="name">图源名</param>
        /// <returns></returns>
        public Comic? GetComic(string name)
        {
            _comics.TryGetValue(name, out var result);
            return result;
        }
    }
}