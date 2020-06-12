using HideNSeek.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HideNSeek.Interface
{
    /// <summary>
    /// Interaction logic for SeekingWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        ILobby _lobby;
        Seeker _seeker;
        Hider _hider;
        string _hiderName;

        public GameWindow(ILobby lobby, Player player, string hiderName = "")
        {
            InitializeComponent();
            
            this._lobby = lobby;
            this._hiderName = hiderName;

            if (player is Seeker)
                InitialiseSeeker(player as Seeker);
            else if (player is Hider)
                InitialiseHider(player as Hider);
        }

        #region Methods
        private void InitialiseSeeker(Seeker seeker)
        {
            _seeker = seeker;

            // Labels
            lbRole.Content = "Seeker";

            // Combobox add rooms
            var maps = seeker.ViewMaps();

            foreach (Room room in maps.Values.FirstOrDefault().Rooms)
            {
                cbGuess.Items.Add(room.RoomName);
            }
            cbGuess.SelectedIndex = 0;

            // Pre Seeking Set
            btnGuess.IsEnabled = false;

            // Enable timer
            _ = Task.Run(() => { SeekerWait(); } );
        }

        private void InitialiseHider(Hider hider)
        {
            _hider = hider;

            // Labels
            lbRole.Content = "Hider";

            // Hide seeker buttons
            btnGuess.Visibility = Visibility.Collapsed;
            cbGuess.Visibility = Visibility.Collapsed;

            // Enable tracking
            _ = Task.Run(() => { HiderTrack(); });
        }

        private void SeekerWait()
        {
            // Wait till seeking
            int timeRem = _seeker.Lobby.GetRemainingHidingTime();

            while (timeRem > 0)
            {
                timeRem = _seeker.Lobby.GetRemainingHidingTime();
                int sec = timeRem % 60;
                int min = timeRem / 60;
                string time = $"You can start seeking in:\n{min}:{sec.ToString("00")}";

                string position = $"The hider is currently in room:\n{_seeker.Lobby.GetHiderLocation(_hiderName).RoomName}";

                Dispatcher.Invoke(() =>
                {
                    lbStatus.Content = $"{time}\n\n{position}";
                });

                Thread.Sleep(1000);
            }

            // Enable seeking
            Dispatcher.Invoke(() =>
            {
                btnGuess.IsEnabled = true;
                lbStatus.Content = $"You can start seeking now!\n\nPlayer last seen in room:\n{_seeker.Lobby.GetHiderLocation(_hiderName).RoomName}";
            });
        }

        private void HiderTrack()
        {
            // Wait till seeking
            int timeRem = _hider.Lobby.GetRemainingHidingTime();

            while (timeRem > 0)
            {
                timeRem = _hider.Lobby.GetRemainingHidingTime();
                int sec = timeRem % 60;
                int min = timeRem / 60;
                string time = $"Seekers can still see you for:\n{min}:{sec.ToString("00")}";

                string position = $"You are currently in room:\n{_hider.GetPosition().RoomName}";

                Dispatcher.Invoke(() =>
                {
                    lbStatus.Content = $"{time}\n\n{position}";
                });

                Thread.Sleep(1000);
            }

            // Enable seeking
            while (!_hider.IsFound)
            {
                Dispatcher.Invoke(() =>
                {
                    lbStatus.Content = $"The seekers can no longer see you!\n\nYou are currently in room:\n{_hider.GetPosition().RoomName}";
                });

                Thread.Sleep(1000);
            }

            MessageBox.Show("You have been found!",
                            "HideNSeek",
                            MessageBoxButton.OK,
                            MessageBoxImage.Asterisk);

            EndGame();
        }

        private void btnGuess_Click(object sender, RoutedEventArgs e)
        {
            if (_seeker.GuessRoom(_hiderName, cbGuess.SelectedItem.ToString()))
            {
                MessageBox.Show($"You found hider \"{_hiderName}\"!",
                                "HideNSeek",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);

                EndGame(true);
            }
            else
            {
                _ = Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        lbGuessStatus.Content = $"The hider \"{_hiderName}\" was not in room \n\"{cbGuess.SelectedItem.ToString()}\"!";
                    });

                    Thread.Sleep(2500);

                    Dispatcher.Invoke(() =>
                    {
                        lbGuessStatus.Content = "";
                    });
                });
            }
        }

        private void EndGame(bool openWindow = false)
        {
            if (openWindow)
            {
                Window window = new MainWindow();
                window.Show();
            }
            this.Close();
        }
        #endregion
    }
}
