using RiotSharp.Endpoints.LeagueEndpoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChecker
{
    public class RankedData
    {
        public RankedData(LeagueEntry league)
        {
            Tier = league.Tier;
            Rank = league.Rank;
            LeaguePoints = league.LeaguePoints;
            Wins = league.Wins;
            Losses = league.Losses;
            Queue = league.QueueType;
        }

        public string Tier { get; private set; }

        public string Rank { get; private set; }

        public int LeaguePoints { get; private set; }

        public int Wins { get; private set; }

        public int Losses { get; private set; }

        public string Queue { get; private set; }

        public string IconPath
        {
            get 
            {
                return Path.Combine(App.ddPath, "ranked-emblems", "Emblem_" + Tier[0] + Tier.ToLower().Substring(1) + ".png");
            }
        }
    }
}
