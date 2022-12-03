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
    public class RuneData
    {
        public string DisplayName { get; private set; }

        public int ID { get; private set; }

        public string IconPath
        {
            get; private set;
        }

        private RuneData()
        {

        }

        private static ImmutableDictionary<int, RuneData> runeDatas;

        public static ImmutableDictionary<int, RuneData> RuneDatas
        {
            get
            {
                if (runeDatas == null)
                {
                    var builder = ImmutableDictionary.CreateBuilder<int, RuneData>();

                    var text = File.ReadAllText(Path.Combine(App.ddPath, App.leagueVersion, @"data\en_GB\runesReforged.json"));

                    var json = JsonConvert.DeserializeObject<JArray>(text);

                    foreach (JObject o in json)
                    {
                        RuneData runeTree = new RuneData();
                        runeTree.ID = (int)o["id"];
                        runeTree.DisplayName = (string)o["name"];
                        runeTree.IconPath = Path.Combine(App.ddPath, "img", (string)o["icon"]);

                        JArray runes = (JArray)o["slots"];
                        JObject keystones = (JObject)runes[0];

                        var keystonesArr = (JArray)keystones["runes"];

                        foreach (JObject rune in keystonesArr)
                        {
                            RuneData runeData = new RuneData();
                            runeData.DisplayName = (string)rune["name"];
                            runeData.ID = (int)rune["id"];
                            runeData.IconPath = Path.Combine(App.ddPath, "img", (string)rune["icon"]);
                            builder.Add(runeData.ID, runeData);
                        }

                        builder.Add(runeTree.ID, runeTree);
                    }

                    //to repreent teh abscence of a rune or a rune that is not supported
                    //to mantain some legacy compatibility with matches before runes reforged
                    builder.Add(-1, new RuneData() { DisplayName = "", ID = -1, IconPath = null });

                    runeDatas = builder.ToImmutable();
                }
                return runeDatas;
            }
        }
    }
}
