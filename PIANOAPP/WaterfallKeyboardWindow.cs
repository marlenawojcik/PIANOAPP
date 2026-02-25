using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Animation;
using System.Threading;

namespace PIANOAPP
{
    public partial class WaterfallKeyboardWindow : PianoKeyboardWindow
    {
        private List<DispatcherTimer> activeTimers = new List<DispatcherTimer>();

        private DispatcherTimer animationTimer;
        private double elapsedTime = 0; // Czas, który upłynął od rozpoczęcia animacji
        private const int NoteSpeed = 5;
        double time {  get; set; }
        public WaterfallKeyboardWindow(Song song) : base()
        {
            currentSong = song;
            scoringManager = new ScoringManager(song);
            GenerateKeyboard();
            GenerateButtons();

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
            base.OnRepeatButtonClick(sender, e);
            staffCanvas.Children.Clear();
            CreateFallingBlock(staffCanvas, currentSong);
        }
        public override void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            staffCanvas.Children.Clear();
            CreateFallingBlock(staffCanvas, currentSong);
        }

        public override void OnTempoButtonClick(object sender, RoutedEventArgs e)
        {
            base.OnTempoButtonClick(sender, e);
            staffCanvas.Children.Clear();
            isPlaying = false;
        }

        public double GetNoteXPosition(string noteName)
        {
            double currentX = 0;

            foreach (var child in KeyboardPanel.Children)
            {
                if (child is Button button)
                {
                    if (button.Content.ToString() == noteName)
                    {
                        return currentX; // Zwróć aktualną pozycję X
                    }

                    // Dodaj szerokość klawisza (oraz marginesy) do aktualnej pozycji X
                    currentX += button.Width + button.Margin.Left + button.Margin.Right;
                }
            }

            throw new ArgumentException($"Note {noteName} not found in the keyboard range.");
        }

        public override void OnPlayButtonClick(object sender, RoutedEventArgs e)
        {
            //change = 1;
            if (isPlaying)
            {
                isPlaying = false;
                staffCanvas.Children.Clear();
            }
            else
            {
                staffCanvas.Children.Clear();
                CreateFallingBlock(staffCanvas, currentSong);
                

                isPlaying = true;
                Task.Run(() =>
                {
                   // CreateFallingBlock(staffCanvas, currentSong);
                    MusicElement pause = new Pause((int)Math.Round(time));
                    soundStrategy.PlayTone(pause, midiOut);
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


        public void CreateFallingBlock(Canvas canvas, Song song, double delayMs = 0)
        {

            int elements = -1;
            foreach (MusicElement element in song.Elements)
            {
                //double changecopy = change;
                elements += 1;
                if (element is Note note)
                {

                    var block = new Grid
                    {
                        Width = note.Name.Contains("#") ? 35 : 60,
                        Height = note.Duration / 5
                    };

                    // Dodanie prostokąta (tła) jako pierwsze dziecko
                    var blockBackground = new Rectangle
                    {
                        RadiusX = 5, // Zaokrąglone rogi
                        RadiusY = 5,
                        Fill = Brushes.DarkViolet, 
                        Stroke = Brushes.Black, 
                        StrokeThickness = 1, 
                    };
                    block.Children.Add(blockBackground);

                    // Dodanie tekstu na prostokącie
                    var blockText = new TextBlock
                    {
                        Text = note.Name,             // Wyświetlenie nazwy nuty
                        Foreground = Brushes.White,   // Kolor tekstu
                        FontSize = 12,                // Rozmiar czcionki
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontWeight = FontWeights.Bold // Pogrubienie
                    };
                    block.Children.Add(blockText);




                    delayMs = song.StartingTime(elements)/change;
                    // Ustawienie początkowej pozycji
                    Canvas.SetLeft(block, GetNoteXPosition(note.Name)); // Pozycja X
                    Canvas.SetTop(block, -block.Height);  
                    // Dodanie bloczka do Canvas
                    canvas.Children.Add(block);
                    // Timer do animacji\
                    var timer = new DispatcherTimer
                        {
                            Interval = TimeSpan.FromMilliseconds(5) // Około 60 FPS
                        };
                        double speed = 1000.0*change; // Jednostki na sekundę;
                    if (change < 1) { time = (canvas.ActualHeight / speed) * 3200; } else { time = (canvas.ActualHeight / speed) * 2500; }
                        int intervalMs = 5; // Interwał w milisekundach
                        double step = speed * (intervalMs / 1000.0);
                        //przesuwa o 1000 jdnostek na 1000 milisekund
                        double currentY = -block.Height; // Aktualna pozycja Y bloczka

                        if (delayMs > 0)
                        {
                            Task.Delay((int)Math.Floor(delayMs)).ContinueWith(_ =>
                                {
                                    
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            timer.Start();
                                            // Uruchomienie timera po upływie opóźnienia
                                        });                                  
                                });

                        }
                        else
                        {                          
                                timer.Start();
                            // Jeśli brak opóźnienia, startuje od razu
                        }

                        timer.Tick += (sender, e) =>
                        {
                            currentY += step; // Przesunięcie bloczka w dół o 0.5 mm
                            Canvas.SetTop(block, currentY);

                            // Jeśli bloczek wyjdzie poza ekran, zatrzymaj animację
                            if (currentY > canvas.ActualHeight)
                            {
                                timer.Stop(); // Zatrzymaj timer
                                canvas.Children.Remove(block);
                              
                            }
                        };
                    }                              
            }
        }
    }
}