using BrilliantSee.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                var msg = await Obj.Source.HttpClient!.GetAsync(Url);
                if (msg.RequestMessage is null || msg.RequestMessage.RequestUri is null)
                    throw new Exception("接口异常,请等待维护");
                var html = await msg.Content.ReadAsStringAsync();
                var match = Regex.Match(html, "video[\\s\\S]*?now[\\s\\S]*?\"(.*?)\";");
                var head = match.Groups[1].Value.Substring(0, match.Groups[1].Value.IndexOf("/share"));
                msg = await Obj.Source.HttpClient!.GetAsync(match.Groups[1].Value);
                if (msg.RequestMessage is null || msg.RequestMessage.RequestUri is null)
                    throw new Exception("接口异常,请等待维护");
                html = await msg.Content.ReadAsStringAsync();
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