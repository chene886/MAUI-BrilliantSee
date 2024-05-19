using BrilliantSee.Models.Objs;
using BrilliantSee.Models.Objs.Musics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrilliantSee.Models.Sources.MusicSources
{
    public class MMPlayerSource : Source
    {
        public MMPlayerSource()
        {
            SetHttpClient("https://mu-api.yuk0.com/");
            Name = "MMPlayer";
            HasMore = 1;
        }

        public override async Task<IEnumerable<Obj>> SearchAsync(string keyword)
        {
            var url = $"https://mu-api.yuk0.com/search?offset={ResultNum}&limit=30&keywords={keyword}";
            var html = await GetHtmlAsync(url);
            if (html == string.Empty) { return Array.Empty<Obj>(); }

            var musics = new List<Obj>();

            try
            {
                string pattern = "\"id\":\\s*(.*?),[\\s\\S]*?\"name\":\\s*\"(.*?)\"[\\s\\S]*?artists[\\s\\S]*?name\":\\s*\"(.*?)\"[\\s\\S]*?album\"[\\s\\S]*?mark";
                var matches = Regex.Matches(html, pattern);
                if (matches.Count < 30) { HasMore = 0; }
                foreach (Match match in matches)
                {
                    var music = new MMPlayerMusic()
                    {
                        Url = $"https://music.163.com/song/media/outer/url?id={match.Groups[1].Value}.mp3",
                        Name = match.Groups[2].Value,
                        //Singer = match.Groups[3].Value,
                        Source = this,
                        SourceName = Name,
                        SourceCategory = Category,
                    };
                    musics.Add(music);
                }
                return musics;
            }
            catch
            {
                return Array.Empty<Obj>();
            }
        }
    }
}