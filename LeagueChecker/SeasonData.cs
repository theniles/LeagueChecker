using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChecker
{
    public class SeasonData
    {
        public int ID { get; private set; }

        public string Name { get; private set; }
        private SeasonData(int id, string desc)
        {
            ID = id;
            Name = desc;
        }

        private static ImmutableDictionary<int, SeasonData> seasons;

        public static ImmutableDictionary<int, SeasonData> Seasons
        {
            get
            {
                if (seasons == null)
                {
                    var builder = ImmutableDictionary.CreateBuilder<int, SeasonData>();

                    var text = File.ReadAllText(Path.Combine(App.ddPath, "seasons.json"));

                    var json = JsonConvert.DeserializeObject<JArray>(text);

                    foreach (JObject o in json)
                    {
                        var s = new SeasonData((int)o["id"], (string)o["season"]);

                        builder.Add(s.ID, s);
                    }

                    seasons = builder.ToImmutable();
                }
                return seasons;
            }
        }
    }
}
