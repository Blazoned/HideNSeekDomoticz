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
using System.Windows.Shapes;

namespace HideNSeek.Interface
{
    /// <summary>
    /// Interaction logic for LobbyWindow.xaml
    /// </summary>
    public partial class LobbyWindow : Window
    {
        public LobbyWindow()
        {
            InitializeComponent();
        }

        private void BtnClickDisconnect(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Lobby");
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

        
    }
}
