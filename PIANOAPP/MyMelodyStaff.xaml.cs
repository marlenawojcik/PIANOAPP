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
using System.Xml.Linq;

namespace PIANOAPP
{
    /// <summary>
    /// Logika interakcji dla klasy MyMelodyStaff.xaml
    /// </summary>
    public partial class MyMelodyStaff : Window
    {
        public MusicStaff staff { get; set; }
        public MidiOut midiOut;
        public Song selectedSong;
        private bool isPlaying = false;

        public MyMelodyStaff(Song song)
        {
            midiOut = MidiManager.GetMidiOut();
            selectedSong = song;
            staff = new MusicStaff();
            InitializeComponent();
            staff.DrawStaffLines(song, staffCanvas);
            staffCanvas.Width = Length();
            staff.DrawNotesAndRests(song, staffCanvas);
            this.Closing += OnClosed;
            
            
        }
        private int Length()
        {
            int endX = 0;
            foreach (var element in selectedSong.Elements)
            {
                endX += 40 + (element.Duration / 100);
            }
            return endX + 100; }
        
    
        
        private void OnPlayButtonClick(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                isPlaying = false;
            }
            else
            {

                isPlaying = true;
                Task.Run(() =>
                {


                    foreach (var element in selectedSong.Elements)
                    {

                        if (!isPlaying)

                        {
                            break;
                        }
                        selectedSong.Strategy.PlayTone(element, midiOut);
                    }
                    //this.Close();


                });
            }
            }
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            SaveWindow.SaveCanvasToFile(staffCanvas);
        }
        protected virtual void OnClosed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isPlaying = false;

        }
    }
}
