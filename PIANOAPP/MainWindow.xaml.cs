using NAudio.Midi;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech.Synthesis;

namespace PIANOAPP
{

    public partial class MainWindow : Window
    {
        string info = "Dzień dobry, życzę przyjemnego korzystania z aplikacji. Pozdrawiam";

        public MainWindow()
        {

            
            InitializeComponent();



            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {
                synth.SetOutputToDefaultAudioDevice();  // Ustawienie domyślnego urządzenia audio
                synth.SelectVoice("Microsoft Paulina Desktop");    // Wybór głosu (np. Microsoft Zira lub David)
                synth.Speak(info);                      // Odtworzenie wiersza
            }

        }
        private void OnKeyboardButtonClick(object sender, RoutedEventArgs e)
        {
            //var keyboardWindow = PianoKeyboardWindow.CreateDefaultKeyboard();
            var keyboardWindow = new DefaultKeyboardWindow();
            keyboardWindow.Show();
        }

        private void OnPlayButtonClick(object sender, RoutedEventArgs e)
        {
            var selectionWindow = new SongWindow();
            selectionWindow.Show();
        }

        private void OnIntervalButtonClick(object sender, RoutedEventArgs e)
        {
            var selectionWindow = new IntervalSelectionWindow();
            selectionWindow.Show();
        }
        private void OnMyMelodiesButtonClick(object sender, RoutedEventArgs e)
        {
            if (SavedMelodiesCollection.SavedMelodies.Count != 0)
            {
                var selectionWindow = new MelodyWindow();
                selectionWindow.Show();
            }
            else
            {
                MessageBox.Show("No melodies saved.");
            }
        }

    }
}
