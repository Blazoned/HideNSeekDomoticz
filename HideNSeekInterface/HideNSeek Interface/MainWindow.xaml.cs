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

            Window window = new LobbyWindow(tbUsername.Text, true);
            window.Show();
            this.Close();
        }

        private void BtnClickJoin(object sender, RoutedEventArgs e)
        {
            if (!UsernameCheck())
                return;

            Window window = new LobbyWindow(tbUsername.Text, false);
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

            MessageBox.Show("You have not filled in a username yet!",
                            "HideNSeek",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);

            return result;
        }
        #endregion
    }
}
