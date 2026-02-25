using NAudio.Midi;
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
   
    public partial class MelodyWindow : Window
    {
        MidiOut midiOut;
        public List<Song> availableMelodies;
        public MelodyWindow()
        {
            InitializeComponent();
            midiOut = MidiManager.GetMidiOut();
            availableMelodies = SavedMelodiesCollection.SavedMelodies;
            MelodyList();
        }
       
        public void MelodyList()
        {
            foreach (var song in availableMelodies)
            {              
               var button = new Button()
               {
                   Content = new TextBlock
                   {
                       Text = song.Title,
                       FontSize = 16,
                       FontWeight = FontWeights.Bold,
                       TextWrapping = TextWrapping.Wrap,
                       TextAlignment = TextAlignment.Center,
                       Foreground = new SolidColorBrush(Color.FromRgb(82, 69, 76))
                   },
                   Tag = song,
                   Width = RecordedSongsPanel.Width,
                   Height = 50,
                    BorderBrush = new SolidColorBrush(Color.FromRgb(82, 69, 76)), // Kolor obramowania
                   BorderThickness = new Thickness(2),
                   Margin = new Thickness(5), // Odstęp między przyciskami
                   HorizontalAlignment = HorizontalAlignment.Stretch,
                   VerticalAlignment = VerticalAlignment.Center,
                   Cursor = Cursors.Hand
               };
                button.Click += OnSongSelected;
                RecordedSongsPanel.Height=(button.Width+button.Margin.Top+button.Margin.Bottom)*availableMelodies.Count;
                RecordedSongsPanel.Children.Add(button);
            }
        }
        public void OnSongSelected(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Song selectedSong)
            {
                var selectionWindow = new MyMelodyStaff(selectedSong);
                selectionWindow.Show();
                
            }
        }
    }
}

