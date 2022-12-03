using RiotSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LeagueChecker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string ddPath;

        public static string apiKey;

        public static string leagueVersion = "10.8.1";

        public static RiotApi riotApi;
    }
}
