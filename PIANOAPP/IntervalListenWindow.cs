using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace PIANOAPP
{
    public partial class IntervalListenWindow : IntervalWindow
    {
        
        public IntervalListenWindow():base()
        {
           // this.SoundStrategy = new PianoStrategy();
            mainGrid.VerticalAlignment = VerticalAlignment.Top;
            mainGrid.Margin = new Thickness(0, mainGrid.Height/4, 0, 0);
            mainGrid.Children.Add(DesignerClass.CreateButton("Play Again", "#FFAA41C0", OnPlayAgainIntervalClick));
            mainGrid.Children.Add(DesignerClass.CreateImageButton("pack://application:,,,/Images/repeat.png", OnPlayAgainIntervalClick, 1.2));
            mainGrid.Children.Add(DesignerClass.CreateButton("Play Spread Version", "#FF763D81", OnPlayAgainSpreadIntervalClick));
            mainGrid.Children.Add(DesignerClass.CreateImageButton("pack://application:,,,/Images/spread.png", OnPlayAgainSpreadIntervalClick));
        }

        
        public override void OnIntervalButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is string buttonContent)
            {
                string interval = buttonContent.Substring(0, 2);
                if (intervals.TryGetValue(interval, out int intervalValue))
                {
                    int midi = random.Next(48, 72);
                    copyMidi = midi;
                    firstNote = NoteLibrary.GetNote(midi);
                    copyInterval = intervalValue;
                    secondNote = NoteLibrary.GetNote(midi + intervalValue);
                    SoundStrategy.StartTone(firstNote, midiOut);
                    SoundStrategy.StartTone(secondNote, midiOut);
                    Thread.Sleep(2000);
                    SoundStrategy.StopTone(firstNote, midiOut);
                    SoundStrategy.StopTone(secondNote, midiOut);
                }
            }
        }
    }
}
