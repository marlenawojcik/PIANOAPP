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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Formats.Asn1.AsnWriter;

namespace PIANOAPP
{

    public abstract partial class PianoKeyboardWindow : Window
    {
        public bool isPlaying = false;
        public double change = 1;
        public ISoundStrategy soundStrategy { get; set; }
        public MidiOut midiOut { get; set; }
        public Song currentSong { get; set; }
        public MusicStaff staff { get; set; }

        private List<MusicElement> recordedElements;
        private bool isRecording = false;
        private List<DateTime> keyDownTimes; // Czas naciśnięcia
        private List<DateTime> keyUpTimes;
        public List<Note> clickedButton { get; set; }
        public ScoringManager scoringManager { get; set; }
        public int startingOctave { get; set; }
        public int numberOfOctaves { get; set; }
        public bool isAnimationPaused = false;



        public PianoKeyboardWindow()
        {
            InitializeComponent();
            change = 1;
            this.Closing += OnClosed;
            midiOut = MidiManager.GetMidiOut();
            soundStrategy = new PianoStrategy();
            clickedButton = new List<Note>();
            recordedElements = new List<MusicElement>();
            keyDownTimes = new List<DateTime>();
            keyUpTimes = new List<DateTime>();

        }

        

        public void GenerateKeyboard()
        {
            var notes = GetNotesPerOctaves();
            double totalWidth = 0;
            //int startingNoteIndex = notes.FindIndex(note => note.Name == "G3");
            foreach (var note in notes)
            {
                var button = CreateKeyboardButton(note);
                button.PreviewMouseDown += OnKeyMouseDown;
                button.PreviewMouseUp += OnKeyMouseUp;

                var border = button.Template.FindName("Border", button) as Border;
                if (border != null)
                {
                    border.CornerRadius = new CornerRadius(10);  // Ustawienie zaokrąglonych rogów
                }

                KeyboardPanel.Children.Add(button);
                totalWidth += button.Width + button.Margin.Left + button.Margin.Right;
                keyDownTimes = new List<DateTime>();
                keyUpTimes = new List<DateTime>();
            }
            KeyboardPanel.Width = totalWidth;
            staffCanvas.Width = totalWidth;
        }

        public List<Note> GetNotesPerOctaves(int startingOctave = 3, int numberOfOctaves = 7)
        {
            this.startingOctave = startingOctave;
            this.numberOfOctaves = numberOfOctaves;
            var notes = new List<Note>();
            var baseNoteNames = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            for (int octave = startingOctave; octave < startingOctave + numberOfOctaves; octave++)
            {
                foreach (var baseNote in baseNoteNames)
                {
                    string noteName = $"{baseNote}{octave}";
                    try
                    {
                        // Pobranie nuty z biblioteki
                        notes.Add(NoteLibrary.GetNote(noteName));
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Błąd: {ex.Message}");
                    }
                }
            }

            return notes;
        }

        public Button CreateKeyboardButton(Note note)
        {
            return new Button
            {
                Content = note.Name,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Width = note.Name.Contains("#") ? 40 : 60,
                VerticalAlignment = VerticalAlignment.Top,
                Height = note.Name.Contains("#") ? 220 : 300,
                Background = note.Name.Contains("#") ? Brushes.Black : Brushes.White,
                Foreground = note.Name.Contains("#") ? Brushes.White : Brushes.Black,
                BorderBrush = Brushes.Gray, // Kolor obramowania
                BorderThickness = new Thickness(1), // Grubość obramowania
                Padding = new Thickness(5), // Margines wewnętrzny
                Margin = new Thickness(2, 80, 2, 2), // Margines zewnętrzny
                Tag = note,
                Effect = new DropShadowEffect // Dodanie cienia
                {
                    Color = Colors.Gray,
                    ShadowDepth = 5,
                    BlurRadius = 10,
                },
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
        }

        public virtual void OnKeyMouseDown(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Note note)
            {
                Console.WriteLine($"Stat playing: {note}");
                soundStrategy.StartTone(note, midiOut);
                clickedButton.Add(note);
                if (isRecording)
                {
                    DateTime currentTime = DateTime.Now;
                    keyDownTimes.Add(currentTime);
                    if (keyUpTimes.Count() > 0)
                    {
                        DateTime lastKeyUpTime = keyUpTimes[keyUpTimes.Count - 1];
                        int pauseDuration = (int)(currentTime - lastKeyUpTime).TotalMilliseconds;
                        if (pauseDuration > 0)
                        {
                            recordedElements.Add(new Pause(pauseDuration));
                        }
                        keyUpTimes.RemoveAt(keyUpTimes.Count - 1);
                    }

                }

            }

        }

        public void OnKeyMouseUp(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Note note)
            {
                Console.WriteLine($"Stop playing: {note}");
                soundStrategy.StopTone(note, midiOut);
                if (isRecording)
                {
                    // Oblicz czas trwania naciskania klawisza
                    DateTime currentTime = DateTime.Now;
                    keyUpTimes.Add(currentTime);
                    if (keyDownTimes.Count > 0)
                    {
                        DateTime lastKeyDownTime = keyDownTimes[keyDownTimes.Count - 1];
                        int noteDuration = (int)(currentTime - lastKeyDownTime).TotalMilliseconds;
                        //Note newNote = new Note(note.MidiNum, note.Position, note.Name, noteDuration)
                        recordedElements.Add(new Note(note.MidiNum, note.Position, note.Name, noteDuration));
                        keyDownTimes.RemoveAt(keyDownTimes.Count - 1);
                    }
                }
            }

        }
        private void OnSoundButtonClick(object sender, RoutedEventArgs e)
        {
            var chooseSoundWindow = new ChooseSoundWindow(this);
            chooseSoundWindow.Show();
        }

        public virtual void OnRepeatButtonClick(object sender, RoutedEventArgs e)
        {
            scoringManager.Reset();
        }
 
        private void OnFinishButtonClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("FinishButton clicked.");
            if (scoringManager == null || currentSong == null)
            {
                Console.WriteLine("scoringManager or currentSong is null.");
                return;
            }
            scoringManager.ComparePressedNotes(currentSong);
            Console.WriteLine("ComparePressedNotes executed.");
            string percent = scoringManager.CalculateMatch(currentSong);
            MessageBox.Show($"Animation completed! Your score: {percent}", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void GenerateButtons()
        {
            Canvas buttonCanvas = this.FindName("ButtonCanvas") as Canvas;

            if (buttonCanvas == null)
            {
                MessageBox.Show("Nie znaleziono buttonCanvas");
                return;
            }

            Button repeatButton = new Button
            {
                Content = "Repeat",
                Style = DesignerClass.CreateButtonStyle()
            };
            repeatButton.Click += OnRepeatButtonClick;
            Canvas.SetLeft(repeatButton, 250); // Pozycja X
            Canvas.SetTop(repeatButton, 10); // Pozycja Y
            buttonCanvas.Children.Add(repeatButton);


            Button finishButton = new Button
            {
                Content = "Finish",

                Style = DesignerClass.CreateButtonStyle(),

            };
            finishButton.Click += OnFinishButtonClick;
            Canvas.SetLeft(finishButton, 375); // Pozycja X
            Canvas.SetTop(finishButton, 10);  // Pozycja Y
            buttonCanvas.Children.Add(finishButton);



            Button playButton = new Button
            {
                Content = "Play",
                Style = DesignerClass.CreateButtonStyle()
            };
            playButton.Click += OnPlayButtonClick;
            Canvas.SetLeft(playButton, 500); // Pozycja X
            Canvas.SetTop(playButton, 10); // Pozycja Y
            buttonCanvas.Children.Add(playButton);


            Button tempoButton = new Button
            {
                Content = "Tempo",
                Style = DesignerClass.CreateButtonStyle()
            };
            tempoButton.Click += OnTempoButtonClick;
            Canvas.SetLeft(tempoButton, 625); // Pozycja X
            Canvas.SetTop(tempoButton, 10); // Pozycja Y
            buttonCanvas.Children.Add(tempoButton);


            Button startButton = new Button
            {
                Content = "Start",
                Style = DesignerClass.CreateButtonStyle()
            };
            startButton.Click += OnStartButtonClick;
            Canvas.SetLeft(startButton, 750); // Pozycja X
            Canvas.SetTop(startButton, 10);  // Pozycja Y
            buttonCanvas.Children.Add(startButton);
        }


        private void OnRecordButtonClick(object sender, RoutedEventArgs e)
        {
            {
                if (!isRecording)
                {
                    isRecording = true;
                    recordedElements.Clear();
                    MessageBox.Show("Recording started", "Record", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    isRecording = false;
                    keyUpTimes.Clear();
                    var recordedCopy = new List<MusicElement>(recordedElements);
                    string melodyName = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter the name of your melody:",
                    "Save Melody",
                    $"Melody {DateTime.Now:yyyy-MM-dd HH:mm:ss}"); // Domyślna nazwa

                    if (string.IsNullOrWhiteSpace(melodyName))
                    {
                        MessageBox.Show("Melody name cannot be empty. Melody was not saved.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    Song melody = new Song(melodyName, recordedCopy);
                    melody.Instrument = soundStrategy.GetType().Name;
                    melody.Strategy = soundStrategy;
                    SavedMelodiesCollection.SavedMelodies.Add(melody);
                    MessageBox.Show("Recording saved", "Record", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }


        public virtual void OnPlayButtonClick(object sender, RoutedEventArgs e)
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
                
                foreach (var element in currentSong.Elements)
                {
                    if (!isPlaying)

                    {
                        break;
                    }
                    else
                    {

                        MusicElement ciopyElement = element.Clone();
                        ciopyElement.Duration = (int)(Math.Round(element.Duration / change));

                        soundStrategy.PlayTone(ciopyElement, midiOut);
                    }
                }
            });
            }          

        }
        public virtual void OnTempoButtonClick(object sender, RoutedEventArgs e)
        {
            Window tempoWindow = new Window
            {
                Title = "tempo",
                Width = 300,
                Height = 290,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
                ResizeMode = ResizeMode.NoResize
            };
            StackPanel stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };
            TextBlock header = new TextBlock
            {
                Text = "Choose the tempo:",
                FontSize = 20,
                FontFamily= new FontFamily("Bodoni MT Black"),
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10),
                Foreground = new SolidColorBrush(Color.FromRgb(50, 50, 50)),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(header);
            void AddTempoButton(double newTempo)
            {
                Button button = new Button
                {
                    Content = newTempo.ToString("0.0"),
                    Style = DesignerClass.CreateButtonStyle()
                };

                // Dodanie efektu cienia
                button.Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 10,
                    ShadowDepth = 2
                };

                // Obsługa zdarzenia kliknięcia
                button.Click += (s, args) =>
                {
                    change = newTempo; 
                    MessageBox.Show($"Tempo is set to: {change}", "Tempo", MessageBoxButton.OK, MessageBoxImage.Information); // Potwierdzenie wyboru
                    tempoWindow.Close(); 
                };
                stackPanel.Children.Add(button);
            }
            AddTempoButton(0.5);
            AddTempoButton(0.75);
            AddTempoButton(1.0);
            AddTempoButton(1.5);
            AddTempoButton(2.0);
            AddTempoButton(3.0);
            tempoWindow.Content = stackPanel;

            tempoWindow.ShowDialog();
        }
    
        public virtual void OnStartButtonClick(object sender, RoutedEventArgs e)
        {

        }

        protected virtual void OnClosed(object sender, System.ComponentModel.CancelEventArgs e)
{
            isPlaying = false; 

        }
       

    }

}

    

