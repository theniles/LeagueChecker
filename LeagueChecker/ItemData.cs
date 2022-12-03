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
    public class ItemData
    {

        private ItemData()
        {

        }

        public long ID { get; private set; }

        public string DisplayName { get; private set; }

        public string IconPath
        {
            get
            {
                if (String.IsNullOrEmpty(DisplayName))
                {
                    return null;
                }
                else
                {
                    return Path.Combine(App.ddPath, App.leagueVersion, "img\\item", ID + ".png");
                }
            }
        }

        private static ImmutableDictionary<long, ItemData> itemDatas;

        public static ImmutableDictionary<long, ItemData> ItemDatas
        {
            get
            {
                if (itemDatas == null)
                {
                    var builder = ImmutableDictionary.CreateBuilder<long, ItemData>();

                    var text = File.ReadAllText(Path.Combine(App.ddPath, App.leagueVersion, @"data\en_GB\item.json"));

                    var json = JsonConvert.DeserializeObject<JObject>(text);

                    var data = (JObject)json["data"];

                    foreach (var item in data)
                    {
                        ItemData itemData = new ItemData();
                        itemData.ID = Convert.ToInt32(item.Key);
                        itemData.DisplayName = (string)item.Value["name"];
                        builder.Add(itemData.ID, itemData);
                    }
                    var emptyItem = new ItemData();

                    emptyItem.DisplayName = "";
                    emptyItem.ID = 0;

                    builder.Add(0, emptyItem);

                    itemDatas = builder.ToImmutable();
                }

                return itemDatas;
            }
        }
    }
}
