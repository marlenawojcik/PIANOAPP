using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace PIANOAPP
{
    public partial class IntervalLearnWindow:IntervalWindow
    {
        public IntervalLearnWindow():base()
        {
            random = new Random();
            mainGrid.Children.Add(DesignerClass.CreateButton("Play Interval", "#FFD358ED", OnPlayIntervalClick));
            mainGrid.Children.Add(DesignerClass.CreateImageButton("pack://application:,,,/Images/sound.png", OnPlayIntervalClick));

            mainGrid.Children.Add(DesignerClass.CreateButton("Play Again", "#FFAA41C0", OnPlayAgainIntervalClick));
            mainGrid.Children.Add(DesignerClass.CreateImageButton("pack://application:,,,/Images/repeat.png", OnPlayAgainIntervalClick, 1.2));

            mainGrid.Children.Add(DesignerClass.CreateButton("Play Spread Version", "#FF763D81", OnPlayAgainSpreadIntervalClick));
            mainGrid.Children.Add(DesignerClass.CreateImageButton("pack://application:,,,/Images/spread.png", OnPlayAgainSpreadIntervalClick));

            mainGrid.Children.Add(DesignerClass.CreateButton("Correct Answer", "#FF592165", OnCorrectAnswerClick));
            mainGrid.Children.Add(DesignerClass.CreateImageButton("pack://application:,,,/Images/correct2.png", OnCorrectAnswerClick));

            
            mainGrid.Children.Add(DesignerClass.CreateButton("Check Statistics", "#FF200020", OnCheckClick));
        }


        public static Dictionary<string, int> intervals = new Dictionary<string, int>()
        {
           { "1 ", 0 }, { "2>", 1 }, {"2 ",2 }, {"3>", 3 }, {"3 ", 4 }, { "4 ",5}, {"4</5>", 6 }, {"5 ", 7 },{ "6>", 8 }, { "6 ", 9 }, { "7 ", 10 }, { "7<", 11 }, { "8 ", 12 } };


        private void OnPlayIntervalClick(object sender, RoutedEventArgs e)
        {
            int midi = random.Next(48, 72);
            copyMidi = midi;
            firstNote = NoteLibrary.GetNote(midi);
            int interval = random.Next(1, 13); // Random interval size from 1 to 12
            copyInterval = interval;
            secondNote = NoteLibrary.GetNote(midi + interval);
            SoundStrategy.StartTone(firstNote, midiOut);
            SoundStrategy.StartTone(secondNote, midiOut);
            Thread.Sleep(2000);
            SoundStrategy.StopTone(firstNote, midiOut);
            SoundStrategy.StopTone(secondNote, midiOut);       
        }

        private void OnCorrectAnswerClick(object sender, RoutedEventArgs e)
        {
            if (copyInterval == -1)
            {
                MessageBox.Show($"Play the interval first", "Play Interval",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show($"Correct interval is: {intervals.FirstOrDefault(pair => pair.Value == copyInterval).Key}", "caption", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void OnCheckClick(object sender, RoutedEventArgs e)
        {
            if (goodAns==0 && badAns==0)
            {
                MessageBox.Show($"You don't have any answers yet.","i po co klikasz",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Hand);
            }
            else
            {
                MessageBox.Show($"You answered correctly: {goodAns} times. You answered wrong: {badAns} times. Which gives a result of {goodAns * 100 / (goodAns + badAns)} % ", "caption", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }




    }


}

