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

namespace PIANOAPP
{

    public partial class SongWindow : Window
    {
        List<Song> availableSongs = new List<Song>();

        public SongWindow()
        {
            InitializeComponent();
            availableSongs = SavedMelodiesCollection.DefaultSongs;
            SongList();

        }
        public void SongList()
        {
            foreach (var song in availableSongs)
            {
                var button = new Button()
                {
                    Content = song.Title,
                    Tag = song,
                    Margin = new Thickness(5),
                    Padding = new Thickness(10),
                    Background = null,
                    Foreground = Brushes.White,
                    BorderBrush = Brushes.White,
                    BorderThickness = new Thickness(1),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                };
                button.Click += OnSongSelected;
                SongListPanel.Children.Add(button);
            }
        }

        public virtual void OnSongSelected(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Song selectedSong)
            {
                if (selectedSong != null)
                {
                    var keyboardWindow = new ModeSelectionWindow(selectedSong);

                    keyboardWindow.Show();
                }
                else
                {
                    MessageBox.Show("No song selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                this.Close();
            }
        }

    }
}
