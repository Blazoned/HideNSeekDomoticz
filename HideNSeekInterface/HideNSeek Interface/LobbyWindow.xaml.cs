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
        private ILobby _lobby;

        /// <summary>
        /// True if window was initialised as a host.
        /// </summary>
        private bool IsHost { get { return _host != null; } }

        public LobbyWindow(string username, string address = null)
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(address))
                InitialiseHost(username);
            else
                InitialiseGuest(username, address).RunSynchronously();
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
        {/// toevoegen naam van spelwindow aan functie
            Console.WriteLine("Game start");
            ///Window window =
            // window.Show();
            this.Close();
        }

        #region Methods
        #region Init
        /// <summary>
        /// Initialise the lobby for a host.
        /// </summary>
        /// <param name="username">The user's identifier.</param>
        private void InitialiseHost(string username)
        {
            _host = new Host(username);
            _lobby = _host.Lobby;

            // Configure player listbox
            lBoxPlayers.Items.Add($"YOU: {username}");
            _host.PlayerConnected += () =>
            {
                lBoxPlayers.Items.Clear();
                lBoxPlayers.Items.Add($"YOU: {username}");
                foreach (Player player in _host.Lobby._hiders)
                {
                    if (player.PlayerName != username)
                        lBoxPlayers.Items.Add(player.PlayerName);
                }
                foreach (Player player in _host.Lobby._seekers)
                {
                    if (player.PlayerName != username)
                        lBoxPlayers.Items.Add(player.PlayerName);
                }
            };

            // Configure buttons
            btnDisconnect.Content = "Disband";

            // Configure address label
            _ = Task.Run(() =>
            {
                // Wait for the host to properly setup
                Thread.Sleep(100);
                lbAddress.Content = _host.Address;
            });
        }

        /// <summary>
        /// Initialise the lobby for a guest.
        /// </summary>
        /// <param name="username">The user's identifier.</param>
        /// <param name="address">The ip address.</param>
        private async Task InitialiseGuest(string username, string address)
        {
            // Connect to host
            _lobby = new Lobby();

            try
            {
                await _lobby.ConnectAsync(address, username);
            }
            catch (Exception e)
            {
                Window window = new MainWindow();
                window.Show();
                this.Close();
                MessageBox.Show("Connection refused!",
                                "HideNSeek",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

#if DEBUG
                Console.WriteLine(e.Message);
#endif
            }

            // Configure player listbox
            lBoxPlayers.Items.Add($"YOU: {username}");
            // _host.Lobby.PlayerConnected += (Player player) => { lBoxPlayers.Items.Add(player.PlayerName); };

            // Configure buttons
            btnDisconnect.Content = "Disconnect";
            btnStart.IsEnabled = false;

            // Configure address label
            lbAddress.Content = address;
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
            // throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
