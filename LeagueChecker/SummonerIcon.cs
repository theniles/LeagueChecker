using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChecker
{
    public static class SummonerIcon
    {
        public static string GetPath(int ID)
        {
            return Path.Combine(App.ddPath, App.leagueVersion, @"img\profileicon", ID + ".png");
        }
    }
}
