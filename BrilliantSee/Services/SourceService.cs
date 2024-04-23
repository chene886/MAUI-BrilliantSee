using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Objs.Videos;
using BrilliantSee.Models.Objs.Comics;
using BrilliantSee.Models.Sources;
using BrilliantSee.Models.Sources.ComicSources;
using BrilliantSee.Models.Sources.VideoSources;
using CommunityToolkit.Maui.Alerts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrilliantSee.Models.Enums;

namespace BrilliantSee.Services
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
        private Dictionary<string, Obj> _objs = new();

        /// <summary>
        /// 注册图源
        /// </summary>
        public SourceService()
        {
            //漫画图源
            var baozi = new BaoziSource();
            var gufeng = new GufengSource();
            var goda = new GodaSource();
            var godaEn = new GodaEnSource();
            var baoziComic = new BaoziComic() { Source = baozi };
            var gufengComic = new GufengComic() { Source = gufeng };
            var godaComic = new GodaComic() { Source = goda };
            var godaEnComic = new GodaEnComic() { Source = godaEn };
            _sources.Add(baozi.Name, baozi);
            _sources.Add(gufeng.Name, gufeng);
            _sources.Add(goda.Name, goda);
            _sources.Add(godaEn.Name, godaEn);
            _objs.Add(baozi.Name, baoziComic);
            _objs.Add(gufeng.Name, gufengComic);
            _objs.Add(goda.Name, godaComic);
            _objs.Add(godaEn.Name, godaEnComic);

            //动漫图源
            var yhwang = new YHWangSource();
            //var omofun = new OmoFunSource();
            var yhwangVideo = new YHWangVideo() { Source = yhwang };
            //var omofunVideo = new OmoFunVideo() { Source = omofun };
            _sources.Add(yhwang.Name, yhwang);
            _objs.Add(yhwang.Name, yhwangVideo);
            //_sources.Add(omofun.Name, omofun);
            //_objs.Add(omofun.Name, omofunVideo);
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
        /// <param name="comics">保存漫画结果的集合</param>
        /// <param name="videos">保存动漫结果的集合</param>
        /// <param name="category">搜索的图源类别</param>
        /// <returns></returns>
        public async Task SearchAsync(string keyword, ObservableCollection<Obj> comics, ObservableCollection<Obj> videos, string flag, SourceCategory category)
        {
            IEnumerable<Source> sources;
            if (flag == "Init")
            {
                sources = _sources.Values.Where(s => s.IsSelected == true);
                foreach (var source in sources)
                {
                    source.HasMore = source.HasMore == -1 ? -1 : 1;
                    source.ResultNum = 1;
                }
            }
            else
            {
                sources = _sources.Values.Where(s => s.IsSelected == true && s.HasMore == 1 && s.Category == category);
                if (!sources.Any()) return;
                foreach (var source in sources)
                {
                    source.ResultNum++;
                }
            }
            //并发使用所有图源去搜索
            var tasks = new List<Task>();
            foreach (var source in sources)
            {
                var objs = source.Category == SourceCategory.Comic ? comics : videos;
                tasks.Add(Task.Run(async () =>
                {
                    var result = await source.SearchAsync(keyword)!;
                    if (result.Any())
                    {
                        foreach (var item in result)
                        {
                            if (item == result.First() && flag == "Init" && objs.Any())
                            {
                                await MainThread.InvokeOnMainThreadAsync(() =>
                                {
                                    objs.Insert(1, item);
                                });
                                continue;
                            }
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                objs.Add(item);
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
        public Obj? GetComic(string name)
        {
            _objs.TryGetValue(name, out var result);
            return result;
        }
    }
}