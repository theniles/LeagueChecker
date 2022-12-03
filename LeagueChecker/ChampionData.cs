using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChecker
{
    public class ChampionData
    {
        private ChampionData(string name, string displayName, int id)
        {
            Name = name;
            DisplayName = displayName;
            ID = id;
        }

        private ChampionData()
        {

        }

        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public string IconPath
        {
            get
            {
                return Path.Combine(App.ddPath, App.leagueVersion, @"img\champion", Name + ".png");
            }
        }

        public int ID { get; private set; }

        private static ImmutableDictionary<int, ChampionData> championDatas;

        public static ImmutableDictionary<int, ChampionData> ChampionDatas
        {
            get
            {
                if (championDatas == null)
                {
                    string path = Path.Combine(App.ddPath, App.leagueVersion, @"data\en_GB\champion.json");
                    var builder = ImmutableDictionary.CreateBuilder<int, ChampionData>();
                    var reader = new StreamReader(path);
                    JsonTextReader json = new JsonTextReader(reader);

                    bool readingChamps = false;

                    string currProp = null;

                    ChampionData tempChampData = null;

                    while (json.Read())
                    {
                        if (json.Value != null)
                        {
                            switch (json.TokenType)
                            {
                                case JsonToken.PropertyName:
                                    currProp = (string)json.Value;
                                    switch ((string)json.Value)
                                    {
                                        case "data":
                                            readingChamps = true;
                                            break;
                                    }
                                    break;
                                case JsonToken.String:
                                    if (readingChamps && currProp != null)
                                    {
                                        switch (currProp)
                                        {
                                            case "key":
                                                tempChampData.ID = Convert.ToInt32((string)json.Value);
                                                break;
                                            case "name":
                                                tempChampData.DisplayName = (string)json.Value;
                                                builder.Add(tempChampData.ID, tempChampData);
                                                break;
                                            case "id":
                                                tempChampData = new ChampionData();
                                                tempChampData.Name = (string)json.Value;
                                                break;
                                        }
                                    }
                                    break;
                            }

                        }
                    }
                    json.Close();
                    reader.Dispose();

                    championDatas = builder.ToImmutable();
                }
                return championDatas;
            }
        }
    }
}
