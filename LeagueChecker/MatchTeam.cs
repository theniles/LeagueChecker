using RiotSharp.Endpoints.MatchEndpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeagueChecker
{
    public class MatchTeam
    {
        public int Kills { get; private set; }

        public int Assists { get; private set; }

        public int Deaths { get; private set; }

        public List<MatchPlayer> Players
        {
            get; private set;
        }

        public TeamStats Stats { get; private set; }

        public MatchTeam(Match match, bool isRedTeam)
        {
            int index = isRedTeam ? 1 : 0;
            Stats = match.Teams[index];

            var players = from p in match.Participants
                          where p.TeamId == Stats.TeamId
                          select p;
            Players = new List<MatchPlayer>(players.Count());

            foreach (var item in players)
            {
                var partId = match.ParticipantIdentities.First((p) => p.ParticipantId == item.ParticipantId);

                Players.Add(new MatchPlayer(partId.Player, item));
            }

            Kills = (int)(from p in Players select p.Participant.Stats.Kills).Sum();
            Assists = (int)(from p in Players select p.Participant.Stats.Assists).Sum();
            Deaths = (int)(from p in Players select p.Participant.Stats.Deaths).Sum();
        }
    }
}
