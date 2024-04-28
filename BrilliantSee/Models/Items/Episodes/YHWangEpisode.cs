using System.Text.RegularExpressions;

namespace BrilliantSee.Models.Items.Episodes
{
    public class YHWangEpisode : Item
    {
        public YHWangEpisode(string name, string url, int index, bool isSpecial) : base(name, url, index, isSpecial)
        {
        }

        public override async Task GetResourcesAsync()
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
                var match = Regex.Match(html, "video[\\s\\S]*?now[\\s\\S]*?\"(.*?)\";");
                var head = match.Groups[1].Value.Substring(0, match.Groups[1].Value.IndexOf("/share"));
                //msg = await Obj.Source.HttpClient!.GetAsync(match.Groups[1].Value);
                //if (msg.RequestMessage is null || msg.RequestMessage.RequestUri is null)
                //    throw new Exception("接口异常,请等待维护");
                //html = await msg.Content.ReadAsStringAsync();
                html = await Obj.Source.GetHtmlAsync(match.Groups[1].Value);
                if (html == string.Empty)
                    throw new Exception("请求失败");
                match = Regex.Match(html, "url\"[\\s\\S]*?\"(.*?)\"");
                VideoUrl = head + match.Groups[1].Value;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}