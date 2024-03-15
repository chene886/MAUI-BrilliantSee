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
            var gufeng = new GufengSource(this);
            var goda = new GodaSource(this);
            //var hasu = new HasuSource(this);
            _sources.Add("BaoZi", baozi);
            _sourceNames.Add(baozi, "BaoZi");
            _sources.Add("GuFeng", gufeng);
            _sourceNames.Add(gufeng, "GuFeng");
            _sources.Add("Goda", goda);
            _sourceNames.Add(goda, "Goda");
            //_sources.Add("Hasu", hasu);
            //_sourceNames.Add(hasu, "Hasu");
        }

        public List<ISource> GetSources()
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
            IEnumerable<ISource> sources;
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