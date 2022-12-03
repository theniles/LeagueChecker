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
    public class SummonerSpell
    {

        public string DisplayName { get; private set; }

        public string Name { get; private set; }

        public int ID { get; private set; }

        public string IconPath
        {
            get
            {
                return Path.Combine(App.ddPath, App.leagueVersion, @"img\spell", Name + ".png");
            }
        }

        private SummonerSpell()
        {

        }

        private static ImmutableDictionary<int, SummonerSpell> summonerSpells;

        public static ImmutableDictionary<int, SummonerSpell> Spells
        {
            get
            {
                if (summonerSpells == null)
                {
                    var builder = ImmutableDictionary.CreateBuilder<int, SummonerSpell>();

                    var text = File.ReadAllText(Path.Combine(App.ddPath, App.leagueVersion, @"data\en_GB\summoner.json"));
                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);

                    JObject data = (JObject)json["data"];

                    foreach (var v in data)
                    {
                        SummonerSpell spell = new SummonerSpell();

                        spell.Name = (string)v.Value["id"];
                        spell.DisplayName = (string)v.Value["name"];
                        spell.ID = Convert.ToInt32((string)v.Value["key"]);

                        builder.Add(spell.ID, spell);
                    }

                    summonerSpells = builder.ToImmutable();
                }
                return summonerSpells;
            }
        }
    }
}
