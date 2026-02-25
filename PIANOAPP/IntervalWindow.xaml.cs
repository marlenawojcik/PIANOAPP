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
    /// <summary>
    /// Logika interakcji dla klasy InWindow.xaml
    /// </summary>
    public abstract partial class IntervalWindow : Window
    {
        public ISoundStrategy SoundStrategy {  get; set; }
        public MidiOut midiOut { get; set; }
        public Note firstNote { get; set; }
        public Note secondNote { get; set; }
        public  Random random { get; set; }
        public int copyMidi { get; set; }
        public int badAns { get; set; }
        public int goodAns { get; set; }

        public int copyInterval { get; set; }
        public static Dictionary<string, int> intervals = new Dictionary<string, int>()
        {
           { "1 ", 0 }, { "2>", 1 }, {"2 ",2 }, {"3>", 3 }, {"3 ", 4 }, { "4 ",5}, {"4<", 6 }, {"5 ", 7 },{ "6>", 8 }, { "6 ", 9 }, { "7 ", 10 }, { "7<", 11 }, { "8 ", 12 } };

        public IntervalWindow()
        {
            
            InitializeComponent();
            midiOut = MidiManager.GetMidiOut();
            this.SoundStrategy = new PianoStrategy();
            badAns = 0;
            goodAns = 0;
            random = new Random();
            copyInterval = -1;
        }

       

        public void OnPlayAgainIntervalClick(object sender, RoutedEventArgs e)
        {
            if (copyInterval == -1)
            {
                MessageBox.Show($"Play interval first!", "Play Interval",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
            }
            else
            {
                
                firstNote = NoteLibrary.GetNote(copyMidi);
                secondNote = NoteLibrary.GetNote(copyMidi + copyInterval);
                SoundStrategy.StartTone(firstNote, midiOut);
                SoundStrategy.StartTone(secondNote, midiOut);
                Thread.Sleep(2000);
                SoundStrategy.StopTone(firstNote, midiOut);
                SoundStrategy.StopTone(secondNote, midiOut);
            }

        }
        public void OnPlayAgainSpreadIntervalClick(object sender, RoutedEventArgs e)
        {
            if (copyInterval == -1)
            {
                MessageBox.Show($"Play interval first", "Play Interval",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Hand);
            }
            else
            {
                firstNote = NoteLibrary.GetNote(copyMidi);
                secondNote = NoteLibrary.GetNote(copyMidi + copyInterval);
                SoundStrategy.PlayTone(firstNote, midiOut);
                SoundStrategy.PlayTone(secondNote, midiOut);
            }

        }

        public virtual void OnIntervalButtonClick(object sender, RoutedEventArgs e)
        {
            if (copyInterval == -1)
            {
                MessageBox.Show($"Play interval first", "Play Interval",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Hand);
            }
            else
            {
                if (sender is Button button && button.Content is string buttonContent)
                {
                    string interval = intervals.FirstOrDefault(pair => pair.Value == copyInterval).Key;

                    if (buttonContent.Substring(0, 2) == interval)

                    {
                        goodAns++;
                        MessageBox.Show("Correct Answer", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        badAns++;
                        MessageBox.Show($"Invalid answer. Try again",
                                        "Invalin",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                    }
                }
            }
        }
       
        // Metoda tworząca przycisk z obrazem
        
    }

}
