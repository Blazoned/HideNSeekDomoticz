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

namespace HideNSeek.Interface
{
    /// <summary>
    /// Interaction logic for LobbyWindow.xaml
    /// </summary>
    public partial class LobbyWindow : Window
    {
        private Host _host;
        private Player _player;
        private Lobby _lobby;

        /// <summary>
        /// True if window was initialised as a host.
        /// </summary>
        private bool IsHost { get { return _host != null; } }

        public LobbyWindow(Lobby lobby, Host host)
        {
            InitializeComponent();

            this._host = host;
            this._player = host.Lobby.HostPlayer;
            this._lobby = lobby;

            InitialiseHost();
        }

        public LobbyWindow(Lobby lobby, Player player)
        {
            InitializeComponent();

            this._player = player;
            this._lobby = lobby;

            InitialiseGuest();
        }

        private void WindowClosedDisconnect(object sender, EventArgs e)
        {
            if (IsHost)
                Disbandon();
            else
                Disconnect();
        }

        private void BtnClickDisconnect(object sender, RoutedEventArgs e)
        {
            if (IsHost)
                Disbandon();
            else
                Disconnect();

            Window window = new MainWindow();
            window.Show();
            this.Close();
        }

        private void BtnClickStart(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Game start");
            _host.StartGame();
        }

        #region Methods
        #region Init
        /// <summary>
        /// Initialise the lobby for a host.
        /// </summary>
        private void InitialiseHost()
        {
            // Configure player listbox
            lBoxPlayers.Items.Add($"YOU: {_lobby.HostPlayer.PlayerName}");
            _lobby.PlayerConnected += (player) =>
            {
                lBoxPlayers.Items.Add($"{player.PlayerName}");
            };
            _lobby.PlayerDisconnected += (player) =>
            {
                Console.WriteLine(lBoxPlayers.Items.IndexOf(player.PlayerName));
                lBoxPlayers.Items.Remove(player.PlayerName);
            };

            // Configure buttons
            btnDisconnect.Content = "Disband";

            // Configure game start
            _lobby.HostPlayer.BeginGame += OpenGameWindow;
        }

        /// <summary>
        /// Initialise the lobby for a guest.
        /// </summary>
        private void InitialiseGuest()
        {
            // Configure player listbox
            lBoxPlayers.Items.Add($"YOU: {_player.PlayerName}");

            foreach(Hider h in _lobby._hiders)
                if (h.PlayerName != _player.PlayerName)
                    lBoxPlayers.Items.Add($"{h.PlayerName}");

            foreach (Seeker s in _lobby._seekers)
                if (s.PlayerName != _player.PlayerName)
                    lBoxPlayers.Items.Add($"{s.PlayerName}");

            _lobby.PlayerConnected += (p) =>
            {
                lBoxPlayers.Items.Add($"{p.PlayerName}");
            };
            _lobby.PlayerDisconnected += (p) =>
            {
                for (int i=0;i<lBoxPlayers.Items.Count;i++)
                {
                    if (lBoxPlayers.Items[i].ToString() == p.PlayerName)
                        lBoxPlayers.Items.RemoveAt(i);
                }
            };

            // Configure buttons
            btnDisconnect.Content = "Disconnect";
            btnStart.IsEnabled = false;

            // Configure game start
            _player.BeginGame += OpenGameWindow;
        }

        private void OpenGameWindow()
        {
            Window window = new GameWindow(_lobby, _player, _lobby.HostPlayer.PlayerName);
            window.Show();
            this.Close();
        }
        #endregion

        #region Disconnect
        /// <summary>
        /// Disbandon the lobby for all players.
        /// </summary>
        private void Disbandon()
        {
            _host.DisbandLobby();
        }

        /// <summary>
        /// Disconnect from the lobby.
        /// </summary>
        private void Disconnect()
        {
            _lobby.Disconnect(_player);
        }
        #endregion
        #endregion
    }
}
