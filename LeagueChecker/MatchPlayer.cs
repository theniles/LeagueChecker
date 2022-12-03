using RiotSharp.Endpoints.LeagueEndpoint;
using RiotSharp.Endpoints.MatchEndpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeagueChecker
{
    public class MatchPlayer
    {
        public Player Player { get; private set; }

        public Participant Participant { get; private set; }

        public string IconPath
        {
            get
            {
                return SummonerIcon.GetPath(Player.ProfileIcon);
            }
        }

        public ChampionData Champion { get; private set; }

        public SummonerSpell Spell1 { get; private set; }

        public SummonerSpell Spell2 { get; private set; }

        public string KDA { get; private set; }

        public List<RankedData> Ranks
        {
            get; set;
        }

        public RankedData DisplayRank
        {
            get; private set;
        }

        public float DamageBarValue
        { get; set; }


        public List<ItemData> ItemIconPaths
        { get; private set; }

        public RuneData PrimaryRune { get; private set; }

        public RuneData SecondaryRune { get; private set; }

        public string DisplayRankToolTip
        {
            get; private set;
        }

        public MatchPlayer(Player player, Participant participant)
        {
            ItemIconPaths = new List<ItemData>();
            var s = participant.Stats;
            ItemIconPaths.Add(ItemData.ItemDatas[s.Item0]);
            ItemIconPaths.Add(ItemData.ItemDatas[s.Item1]);
            ItemIconPaths.Add(ItemData.ItemDatas[s.Item2]);
            ItemIconPaths.Add(ItemData.ItemDatas[s.Item3]);
            ItemIconPaths.Add(ItemData.ItemDatas[s.Item4]);
            ItemIconPaths.Add(ItemData.ItemDatas[s.Item5]);
            ItemIconPaths.Add(ItemData.ItemDatas[s.Item6]);

            Spell1 = SummonerSpell.Spells[participant.Spell1Id];
            Spell2 = SummonerSpell.Spells[participant.Spell2Id];

            Participant = participant;
            Player = player;
            var info = App.riotApi.League.GetLeagueEntriesBySummonerAsync(
                MainWindow.Instance.SelectedRegion,
                player.SummonerId).Result;
            Ranks = new List<RankedData>();
            foreach (var item in info)
            {
                Ranks.Add(new RankedData(item));
            }

            Champion = ChampionData.ChampionDatas[Participant.ChampionId];
            KDA = participant.Stats.Kills + "/" + participant.Stats.Deaths + "/" + participant.Stats.Assists + " lvl" + participant.Stats.ChampLevel;

            PrimaryRune = RuneData.RuneDatas[participant.Stats.Perk0];
            SecondaryRune = RuneData.RuneDatas[participant.Stats.PerkSubStyle];

            if (Ranks.Count > 0)
            {
                DisplayRank = Ranks[0];
                DisplayRankToolTip = DisplayRank.Queue + " " + DisplayRank.Tier + " " + DisplayRank.Rank;
            }
        }
    }
}
