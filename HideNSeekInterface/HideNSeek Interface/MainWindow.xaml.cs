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

        private void BtnClickHost(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Host Lobby");
            Window window = new LobbyWindow();
            window.Show();
            this.Close();
        }

        private void BtnClickQuit(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Quit");
            this.Close();
        }

        private void BtnClickJoin(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Join Lobby");
            /// Checken joincode toevoegen
            Window window = new LobbyWindow();
            window.Show();
            this.Close();
        }
    }
}
