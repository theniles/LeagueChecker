using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Converters;
using RiotSharp;
using RiotSharp.Caching;
using RiotSharp.Endpoints.StaticDataEndpoint.Item;
using RiotSharp.Misc;
using Path = System.IO.Path;

namespace LeagueChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region vars

        private bool wasCancelled;

        private bool collapseMatches;

        public bool CollapseMatches { get { return collapseMatches; } set { collapseMatches = value;OnPropertyChanged(); } }

        public static MainWindow Instance { get; private set; }

        private string searchName;

        public string SearchName
        {
            get
            {
                return searchName;
            }
            set
            {
                searchName = value; OnPropertyChanged();
            }
        }

        private Region selectedRegion;

        public Region SelectedRegion
        {
            get
            {
                return selectedRegion;
            }
            set
            {
                selectedRegion = value; OnPropertyChanged();
            }
        }

        private bool canSearch;

        public bool CanSearch
        {
            get
            {
                return canSearch;
            }
            set
            {
                canSearch = value; OnPropertyChanged();
            }
        }

        private ICollection<DisplayMatch> displayMatches;

        public ICollection<DisplayMatch> DisplayMatches
        {
            get { return displayMatches; }
            set { displayMatches = value;OnPropertyChanged(); }
        }

        private float progressValue;

        public float ProgressValue
        {
            get
            {
                return progressValue;
            }
            set
            {
                progressValue = value; OnPropertyChanged();
            }
        }

        private int desiredMatchCount;

        public int DesiredMatchCount
        {
            get
            {
                return desiredMatchCount;
            }
            set
            {
                desiredMatchCount = value; OnPropertyChanged();
            }
        }

        public string[] Regions { get; private set; }

        private DisplayMatch displayMatch;

        public DisplayMatch DisplayMatch { get { return displayMatch; } set { displayMatch = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region methods and helpers

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Info(object msg)
        {
            MessageBox.Show(msg.ToString(), "NileGG", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region cstrctr
        public MainWindow()
        {
            InitializeComponent();
            App.ddPath = Properties.Settings.Default.ddPath;
            App.apiKey = "RGAPI-f296e88a-c12a-4057-b421-a004cae6a69f";

            App.riotApi = RiotApi.GetDevelopmentInstance(App.apiKey);

            Regions = Enum.GetNames(typeof(Region));
            OnPropertyChanged(nameof(Regions));

            SearchName = "G0odplayer";
            CanSearch = true;

            Instance = this;

            //TODO change this
            DesiredMatchCount = 1;

            ddPathBox.Text = App.ddPath;
        }
        #endregion

        #region ev handlers

        //search btn click
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ProgressValue = 0;
            CanSearch = false;
            wasCancelled = false;
            try
            {
                var player = await App.riotApi.Summoner.GetSummonerByNameAsync(SelectedRegion, SearchName);
                var matchRefs = await App.riotApi.Match.GetMatchListAsync(player.Region, player.AccountId, endIndex: DesiredMatchCount);
                var matches = new ObservableCollection<DisplayMatch>();
                var progressPerMatch = 1f / matchRefs.Matches.Count;
                DisplayMatches = matches;

                foreach (var matchRef in matchRefs.Matches)
                {
                    try
                    {
                        if (wasCancelled)
                        {
                            Info("Cancelled searching the summoner.");
                            break;
                        }

                        var m = await Task.Run(() =>
                        {
                            var match = App.riotApi.Match.GetMatchAsync(player.Region, matchRef.GameId).Result;
                            var dMatch = new DisplayMatch(match, player.Name);

                            return dMatch;
                        });

                        matches.Add(m);
                    }
                    catch(AggregateException exx)
                    {
                        var ex = exx.Flatten();

                        Info("Getting a match from the searched summoner failed. Reasons: " + exx.Message);

                        foreach (var item in ex.InnerExceptions)
                        {
                            Info(item.Message);
                        }
                    }
                    catch(Exception exx)
                    {
                        Info("Getting a match from the searched summoner failed. Reason: " + exx.Message);
                    }

                    ProgressValue += progressPerMatch;
                }

                ProgressValue = 1;
            }
            catch (Exception ex)
            {
                ProgressValue = 0;
                Info("Searching the summoner failed. Reason: " + ex.Message);
            }

            CanSearch = true;
            wasCancelled = false;
        }

        private async void mainWnd_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                bool updated = true;
                var tarlink = await Task.Run(() =>
                {
                    var versions = App.riotApi.StaticData.Versions.GetAllAsync().Result;
                    Version latest = new Version(versions[0]);
                    Version current = new Version(App.leagueVersion);
                    if (latest.Major != current.Major || latest.Minor != current.Minor)
                    {
                        updated = false;
                        return App.riotApi.StaticData.TarballLinks.Get(latest.ToString());
                    }
                    else
                    {
                        return null;
                    }
                });
                if(!updated)
                {
                    Info("Data dragon is not updated to the latest version. Some matches may not be able to be displayed. Update data dragon.");
                }
            }
            catch(Exception ex)
            {
                Info("Checking if the data dragon version is up to date failed. Reason: " + ex.Message);
            }
        }

        //help tbn click
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Info("If there are problems, try the following: first, make sure you're connected to the internet. Second, make sure data dragon is up to date. Third, ask The Nile.");
        }

        //wahts new btn click
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Info("Can see matches as they are being processed.\n" +
                "Proper functionality and controls.\n" +
                "Bigger text and better display.\n" +
                "Proper use of async programming.\n" +
                "More data is available for display.\n" //+
                //"Better query filter."
                //TODO queries
                );
        }

        //match rigth click
        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;

            var dm = grid.DataContext as DisplayMatch;

            dm.IsCollapsed = !dm.IsCollapsed;
        }

        //cancel bt nclick
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            wasCancelled = true;
        }

        //save dd path tb
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ddPath = ddPathBox.Text;
            Properties.Settings.Default.Save();
            Info("Data dragon path was saved. Restart the application to see the change take effect.");
        }

        #endregion

    }
}
