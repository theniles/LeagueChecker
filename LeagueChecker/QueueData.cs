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
    public class QueueData
    {
        public long ID { get; private set; }

        public string Name { get; private set; }
        private QueueData(long id, string desc)
        {
            ID = id;
            Name = desc;
        }

        private static ImmutableDictionary<long, QueueData> seasons;

        public static ImmutableDictionary<long, QueueData> Queues
        {
            get
            {
                if (seasons == null)
                {
                    var builder = ImmutableDictionary.CreateBuilder<long, QueueData>();

                    var text = File.ReadAllText(Path.Combine(App.ddPath, "queues.json"));

                    var json = JsonConvert.DeserializeObject<JArray>(text);

                    foreach (JObject o in json)
                    {
                        var s = new QueueData((long)o["queueId"], (string)o["description"]);

                        builder.Add(s.ID, s);
                    }

                    seasons = builder.ToImmutable();
                }
                return seasons;
            }
        }
    }
}
