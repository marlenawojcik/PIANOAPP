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

    public partial class ChooseSoundWindow : Window
    {
        private PianoKeyboardWindow parentWindow { get; set; }

        public ChooseSoundWindow(PianoKeyboardWindow parent)
        {
            InitializeComponent();
            parentWindow = parent;
        }

        private void PianoButton_Click(object sender, RoutedEventArgs e)
        {
            parentWindow.soundStrategy = new PianoStrategy();
            parentWindow.soundStrategy.SetInstrument(parentWindow.midiOut);
            MessageBox.Show("Instrument set to piano!");
            this.Close();
        }
        private void OrganButton_Click(object sender, RoutedEventArgs e)
        {
            parentWindow.soundStrategy = new OrganStrategy();
            parentWindow.soundStrategy.SetInstrument(parentWindow.midiOut);
            MessageBox.Show("Instrument set to organ!");
            this.Close();
        }

        private void ViolinButton_Click(object sender, RoutedEventArgs e)
        {
            parentWindow.soundStrategy = new ViolinStrategy();
            parentWindow.soundStrategy.SetInstrument(parentWindow.midiOut);

            MessageBox.Show("Instrument set to violin!");
            this.Close();
        }

    }

}

