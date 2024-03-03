using BrilliantComic.Models.Comics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Models.Chapters
{
    public class GufengChapter : Chapter
    {
        public GufengChapter(string name, string url, int index, bool isSpecial) : base(name, url, index, isSpecial)
        {
        }

        public override Task<IEnumerable<string>> GetPicEnumeratorAsync()
        {
            throw new NotImplementedException();
        }
    }
}