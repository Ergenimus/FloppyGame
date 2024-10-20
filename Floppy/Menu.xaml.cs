using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Floppy
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Menu : Window
    {
        MediaPlayer menuPlayer = new MediaPlayer();

        bool music = true;

        public Menu()
        {
            InitializeComponent();
            Menu_Music(music);
        }
        private void Menu_Music(bool music)
        {
            if (music)
            {
                menuPlayer.Open(new Uri($"{Environment.CurrentDirectory}\\muzak\\menuMuzak.wav"));
                menuPlayer.Volume = 0.1;
                menuPlayer.Play();
            } 
            else
            {
                menuPlayer.Stop();
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow gameWindow = new MainWindow(music);
                this.Visibility = Visibility.Collapsed;
                menuPlayer.Stop();
                gameWindow.ShowDialog();
            } 
            finally
            {
                this.Visibility = Visibility.Visible;
                menuPlayer.Play();
            }
        }

        private void MusicButton_Checked(object sender, RoutedEventArgs e)
        {
            music = false;
            Menu_Music(music);
        }

        private void MusicButton_Unchecked(object sender, RoutedEventArgs e)
        {
            music = true;
            Menu_Music(music);
        }

        private void ResultsButton_Click(object sender, RoutedEventArgs e)
        {
            ListBox results = new ListBox();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
