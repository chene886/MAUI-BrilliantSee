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
using BrilliantSee.Models.Sources.NovelSources;
using BrilliantSee.Models.Objs.Novels;

namespace BrilliantSee.Services
{
    public class SourceService
    {
        /// <summary>
        /// 储存图源的字典
        /// </summary>
        private Dictionary<string, Source> _sources = new();

        /// <summary>
        /// 储存实体的字典
        /// </summary>
        private Dictionary<string, Obj> _objs = new();

        /// <summary>
        /// 注册源
        /// </summary>
        public SourceService()
        {
            //小说源
            var dingdian = new DingDianSource();
            var dingdianNovel = new DingDianNovel() { Source = dingdian };
            _sources.Add(dingdian.Name, dingdian);
            _objs.Add(dingdian.Name, dingdianNovel);

            //漫画源
            var baozi = new BaoziSource();
            var gufeng = new GufengSource();
            //var goda = new GodaSource();
            var godaEn = new GodaEnSource();
            var baoziComic = new BaoziComic() { Source = baozi };
            var gufengComic = new GufengComic() { Source = gufeng };
            //var godaComic = new GodaComic() { Source = goda };
            var godaEnComic = new GodaEnComic() { Source = godaEn };
            _sources.Add(baozi.Name, baozi);
            _sources.Add(gufeng.Name, gufeng);
            //_sources.Add(goda.Name, goda);
            _sources.Add(godaEn.Name, godaEn);
            _objs.Add(baozi.Name, baoziComic);
            _objs.Add(gufeng.Name, gufengComic);
            //_objs.Add(goda.Name, godaComic);
            _objs.Add(godaEn.Name, godaEnComic);

            //动漫源
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
        /// 获取所有源
        /// </summary>
        /// <returns></returns>
        public List<Source> GetSources()
        {
            return _sources.Values.ToList();
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="allObjs">保存全部结果的集合</param>
        /// <param name="cateObjs">保存具体类型结果的集合</param>
        /// <param name="flag">搜索模式:初始搜索和追加搜索</param>
        /// <param name="category">搜索的源类别</param>
        /// <returns></returns>
        public async Task SearchAsync(string keyword, ObservableCollection<Obj> allObjs, ObservableCollection<Obj> cateObjs, string flag, SourceCategory category)
        {
            var hasSourceSelected = _sources.Values.Where(s => s.IsSelected == true).Count() > 0;
            if (!hasSourceSelected)
            {
                return;
            }
            IEnumerable<Source> sources;
            sources = _sources.Values.Where(s => s.IsSelected == true && s.Category == category);
            if (flag != "Init") sources = sources.Where(s => s.HasMore == 1);
            if (!sources.Any()) return;
            if (flag == "Init")
            {
                foreach (var source in sources)
                {
                    source.HasMore = source.HasMore == -1 ? -1 : 1;
                    source.ResultNum = 1;
                }
            }
            else
            {
                foreach (var source in sources)
                {
                    source.ResultNum++;
                }
            }
            //并发使用所有源去搜索
            var tasks = new List<Task>();
            foreach (var source in sources)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var result = await source.SearchAsync(keyword)!;
                    if (!result.Any()) return;
                    foreach (var item in result)
                    {
                        //初始搜索时，将第一个源除外的每个源的第一个结果插入到集合的第二个位置
                        if (item == result.First() && flag == "Init")
                        {
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                if (cateObjs.Any()) cateObjs.Insert(1, item);
                                else cateObjs.Add(item);
                                if (allObjs.Any()) allObjs.Insert(1, item);
                                else allObjs.Add(item);
                            });
                            continue;
                        }
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            cateObjs.Add(item);
                            allObjs.Add(item);
                        });
                    }
                }));
            }
            //等待所有图源搜索完成
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 根据源名获取源
        /// </summary>
        /// <param name="name">源名</param>
        /// <returns></returns>
        public Source? GetSource(string name)
        {
            _sources.TryGetValue(name, out var result);
            return result;
        }

        /// <summary>
        /// 根据源名获取实体
        /// </summary>
        /// <param name="name">源名</param>
        /// <returns></returns>
        public Obj? GetComic(string name)
        {
            _objs.TryGetValue(name, out var result);
            return result;
        }
    }
}