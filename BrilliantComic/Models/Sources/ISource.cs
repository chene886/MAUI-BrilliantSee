using BrilliantComic.Models.Comics;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Sources
{
    public interface ISource
    {
        HttpClient HttpClient { get; }

        string Name { get; set; }

        abstract bool IsSelected { get; set; }

        /// <summary>
        /// 搜索漫画
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns></returns>
        Task<IEnumerable<Comic>> SearchAsync(string keyword);

        /// <summary>
        /// 从存储的漫画数据创建漫画实体
        /// </summary>
        /// <param name="dbComic"></param>
        /// <returns></returns>
        Comic CreateComicFromDBComic(DBComic dbComic);

        /// <summary>
        /// 从漫画实体创建存储的漫画数据
        /// </summary>
        /// <param name="comic"></param>
        /// <returns></returns>
        DBComic CreateDBComicFromComic(Comic comic);
    }
}