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
    public partial class DefaultKeyboardWindow:PianoKeyboardWindow
    {

        public DefaultKeyboardWindow():base()
        {
            staff = new MusicStaff();
            GenerateKeyboard();
      }
        public override void OnKeyMouseDown(object sender, RoutedEventArgs e)
        {

            base.OnKeyMouseDown(sender, e);
            if (sender is Button button && button.Tag is Note note)
            {
                    AddElementsToStaff();             
            }
        }

        private void AddElementsToStaff()
        {
            if (staff != null && staffCanvas != null)
            {;
                staffCanvas.Children.Clear();
                staff.DrawStaffLines(clickedButton, staffCanvas);
                staff.AddElementsToStaff(clickedButton, staffCanvas);
            }
            if (staff == null || staffCanvas == null)
            {
                Console.WriteLine("Staff or Canvas not initialized.");
                return;
            }
        }            
    }
}
