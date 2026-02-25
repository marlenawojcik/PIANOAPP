using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PIANOAPP
{
    public partial class KeyboardWithSongWindow : PianoKeyboardWindow
    {
        public  KeyboardWithSongWindow(Song song):base()
        {
            staff = new MusicStaff();
            currentSong = song;
            scoringManager = new ScoringManager(song);
            GenerateKeyboard();
            GenerateButtons();
            staff.DrawStaffLines(song, staffCanvas);
            staff.DrawNotesAndRests(song, staffCanvas);
        }

        public void StartAnimatingStaff()
        {
            if (currentSong != null && staff != null)
            {
                staff.AnimateStaff(currentSong, staffCanvas, change);
            }
        }
        public override void OnKeyMouseDown(object sender, RoutedEventArgs e)
        {
           base.OnKeyMouseDown(sender, e);
            if (sender is Button button && button.Tag is Note note)
            {
                scoringManager.AddPressedNote(note); // Dodanie nuty do listy
            }
        }
        public override void OnRepeatButtonClick(object sender, RoutedEventArgs e)
        {
            base.OnRepeatButtonClick(sender,e);
            StartAnimatingStaff();
        }
        public override void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            StartAnimatingStaff();
        }
        public override void OnTempoButtonClick(object sender, RoutedEventArgs e)
        {
            base.OnTempoButtonClick(sender, e);
            StartAnimatingStaff();
        }
       


    }
    
}
