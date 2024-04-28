using System.Text.RegularExpressions;

namespace BrilliantSee.Models.Items.Chapters
{
    public class HasuChapter : Item
    {
        public HasuChapter(string name, string url, int index, bool isSpecial) : base(name, url, index, isSpecial)
        {
        }

        public override async Task<IEnumerable<string>> GetResourcesAsync()
        {
            try
            {
                //var msg = await Obj.Source.HttpClient!.GetAsync(Url);
                //if (msg.RequestMessage is null || msg.RequestMessage.RequestUri is null)
                //    throw new Exception("接口异常,请等待维护");
                //var html = await msg.Content.ReadAsStringAsync();
                var html = await Obj.Source.GetHtmlAsync(Url);
                if (html == string.Empty)
                    throw new Exception("请求失败");
                var start = html.IndexOf("loadchapter");
                var end = html.IndexOf("Comments");
                if (start < 0 || end < 0)
                {
                    throw new Exception("接口异常,请等待维护");
                }
                html = html.Substring(start, end - start);
                var match = Regex.Matches(html, "pagenum[\\s\\S]*?data-src=\"(.*?)\"");
                var list = new List<string>();
                foreach (Match item in match)
                {
                    list.Add(item.Groups[1].Value);
                }
                if (list.Count == 1) list.Add(list[0]);
                PageCount = list.Count;
                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}