using HideNSeek.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace HideNSeek.Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Host _host;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Open a lobby as a host.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClickHost(object sender, RoutedEventArgs e)
        {
            if (!UsernameCheck())
                return;

            btnHost.IsEnabled = false;

            _host = new Host(tbUsername.Text);
            _host.SetHidingTime(10);

            Window window = new LobbyWindow(_host.Lobby, _host);
            window.Show();
        }

        private void BtnClickJoin(object sender, RoutedEventArgs e)
        {
            if (!UsernameCheck() || !IpAddressCheck() || !HostAvailableCheck())
                return;

            Player player = _host.Lobby.Connect(tbAddress.Text, tbUsername.Text);

            if (player == null)
            {
                MessageBox.Show("Username already taken!",
                                "HideNSeek",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);

                return;
            } 

            Window window = new LobbyWindow(_host.Lobby, player);
            window.Show();
            this.Close();
        }

        /// <summary>
        /// Exit the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClickQuit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region Methods
        /// <summary>
        /// Check if username has been put in.
        /// </summary>
        /// <returns>Returns true if there's a valid username.</returns>
        public bool UsernameCheck()
        {
            bool result = !string.IsNullOrEmpty(tbUsername.Text);

            if (!result)
                MessageBox.Show("You have not filled in a username yet!",
                                "HideNSeek",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);

            return result;
        }
        /// <summary>
        /// Check if ip address has been put in.
        /// </summary>
        /// <returns>Returns true if there's a valid ip address.</returns>
        public bool IpAddressCheck()
        {
            bool result = !string.IsNullOrEmpty(tbAddress.Text);

            if (!result)
                MessageBox.Show("You have not filled in an ip address yet!",
                                "HideNSeek",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);

            return result;
        }
        /// <summary>
        /// Check if host is available.
        /// </summary>
        /// <returns>Returns true if there's a valid host.</returns>
        public bool HostAvailableCheck()
        {
            bool result = _host != null;

            if (!result)
                MessageBox.Show("No connection was found!",
                                "HideNSeek",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);

            return result;
        }
        #endregion
    }
}
