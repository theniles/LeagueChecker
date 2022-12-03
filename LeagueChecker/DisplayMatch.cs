using RiotSharp.Endpoints.MatchEndpoint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChecker
{
    public class DisplayMatch : INotifyPropertyChanged
    {
        public QueueData QueueType
        {
            get; set;
        }

        public MatchPlayer SearchedSummoner { get; private set; }

        public SeasonData SeasonData
        { get; set; }

        private bool isCollapsed;

        public bool IsCollapsed { get { return isCollapsed; } set { isCollapsed = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCollapsed))); } }

        public DateTime MatchDate { get; set; }

        public TimeSpan Duration { get; set; }

        public MatchTeam BlueTeam { get; private set; }

        public MatchTeam RedTeam { get; private set; }

        public bool SearchedSummonerWon { get; private set; }

        public DisplayMatch(Match match, string summonerName)
        {
            Duration = match.GameDuration;
            MatchDate = match.GameCreation;
            QueueType = QueueData.Queues[match.QueueId];
            SeasonData = SeasonData.Seasons[match.SeasonId];

            BlueTeam = new MatchTeam(match, false);

            RedTeam = new MatchTeam(match, true);

            var damages = 
                BlueTeam.Players.Select((p) => p.Participant.Stats.TotalDamageDealtToChampions)
                .Concat(RedTeam.Players.Select((p)=>p.Participant.Stats.TotalDamageDealtToChampions));
            var maxDamage = (double)damages.Max();

            foreach (var item in BlueTeam.Players)
            {
                item.DamageBarValue = (float)(item.Participant.Stats.TotalDamageDealtToChampions / maxDamage);
            }

            foreach (var item in RedTeam.Players)
            {
                item.DamageBarValue = (float)(item.Participant.Stats.TotalDamageDealtToChampions / maxDamage);
            }

            var blueNames = from player in BlueTeam.Players
                            select player.Player.SummonerName;

            var isBlue = blueNames.Contains(summonerName);

            var blueWon = BlueTeam.Stats.Win != "Fail";

            if(blueWon)
            {
                if(isBlue)
                {
                    SearchedSummonerWon = true;
                }
                else
                {
                    SearchedSummonerWon = false;
                }
            }
            else
            {
                if(isBlue)
                {
                    SearchedSummonerWon = false;
                }
                else
                {
                    SearchedSummonerWon = true;
                }
            }

            SearchedSummoner = RedTeam.Players.Find((s) => s.Player.SummonerName == summonerName);
            if(SearchedSummoner == null)
                SearchedSummoner = BlueTeam.Players.Find((s) => s.Player.SummonerName == summonerName);

            if (MainWindow.Instance.CollapseMatches)
                IsCollapsed = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
