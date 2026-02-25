using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Buffers.Text;
using System.Windows.Media.Animation;
using System.Windows.Forms.Design;
using System.Diagnostics.Eventing.Reader;
using MathNet.Numerics.Integration;
using static System.Windows.Forms.AxHost;
using System.Threading;
using System.Xml.Linq;
using static System.Windows.Forms.LinkLabel;

namespace PIANOAPP
{
    public class MusicStaff
    {
        double lineSpacing = 20;
        double startStaff = 100;
        int ledgerline = 0;

        Image violinImage = new Image
        {
            Source = new BitmapImage(new Uri("C:\\Users\\marle\\OneDrive\\Pulpit\\PianoAPp\\PianoAPp\\Images\\violin.jpg")),                                                                                                                //Height = imageHeight, // Dostosowanie wysokości obrazu
            Stretch = Stretch.UniformToFill // Dostosowanie obrazu
        };

        public double EndX(Song song)
        {
            double endX = 0;
            foreach(var element in song.Elements)
            {
                endX += 35 + (element.Duration / 100);
            }
            return endX+100;
        }

        public void DrawStaffLines(Song song,Canvas canvas)
        {
            
            if (song.Title == "twinkyButBetter")
            {
                InsertPicture(song, canvas);
            }
            else
            {
                double startX = 50;
                double endX = EndX(song);

                Canvas.SetLeft(violinImage, startX); 
                Canvas.SetTop(violinImage, 80); 

                // Dodaj obraz na canvas
                canvas.Children.Add(violinImage);


                for (int i = 0; i < 5; i++) // Pięć linii pięciolinii
                {
                    var line = new Line
                    {
                        X1 = startX,
                        Y1 = startStaff + i * lineSpacing,
                        X2 = endX,
                        Y2 = startStaff + i * lineSpacing,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(line);
                }
                var endLine = new Line
                {
                    X1 = endX,
                    Y1 = startStaff,
                    X2 = endX,
                    Y2 = startStaff + 4 * lineSpacing,
                    Stroke = Brushes.Black,
                    StrokeThickness = 3
                };
                canvas.Children.Add(endLine);
                var endLine2 = new Line
                {
                    X1 = endX-4,
                    Y1 = startStaff,
                    X2 = endX-4,
                    Y2 = startStaff + 4 * lineSpacing,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                canvas.Children.Add(endLine2);
            }
        }
        public void DrawStaffLines(List<Note> clicks, Canvas canvas)
        {           
            double endX;
            
                double startX = 20;
            endX = 45 * clicks.Count + 80;
            double lineSpacing = 20;

            Image violinImage = new Image
            {
                Source = new BitmapImage(new Uri("C:\\Users\\marle\\OneDrive\\Pulpit\\PianoAPp\\PianoAPp\\Images\\violin.jpg")), // Ścieżka do obrazu
                                                                                                                                 //Width = imageHeight/2, // Dostosowanie szerokości obrazu
                                                                                                                    //Height = imageHeight, // Dostosowanie wysokości obrazu
                Stretch = Stretch.UniformToFill // Dostosowanie obrazu
            };

            Canvas.SetLeft(violinImage, startX); 
            Canvas.SetTop(violinImage, 80); 
            canvas.Children.Add(violinImage);
            for (int i = 0; i < 5; i++) // Pięć linii pięciolinii
            {
                if (endX <= canvas.Width)
                {
                    var line = new Line
                    {
                        X1 = startX,
                        Y1 = startStaff + i * lineSpacing,
                        X2 = endX,
                        Y2 = startStaff + i * lineSpacing,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(line);
                    var endLine = new Line
                    {
                        X1 = endX,
                        Y1 = startStaff,
                        X2 = endX,
                        Y2 = startStaff + 4 * lineSpacing,
                        Stroke = Brushes.Black,
                        StrokeThickness = 3
                    };
                    canvas.Children.Add(endLine);
                    var endLine2 = new Line
                    {
                        X1 = endX - 4,
                        Y1 = startStaff,
                        X2 = endX - 4,
                        Y2 = startStaff + 4 * lineSpacing,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(endLine2);
                }
                else
                {

                    var line1 = new Line
                    {
                        X1 = startX,
                        Y1 = startStaff + i * lineSpacing,
                        X2 = canvas.ActualWidth,
                        Y2 = startStaff + i * lineSpacing,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(line1);
                }
            }
        }

        public void AddElementsToStaff(List<Note> clickedButton, Canvas canvas)
        { 
        double currentXPosition = 80;
            double requiredWidth = currentXPosition + clickedButton.Count * 40 + 20;
            if (canvas.Width > requiredWidth)
            {
                foreach (Note note in clickedButton)
                {
                    double noteY = GetNoteYPosition(note.Position, startStaff, lineSpacing);
                    if (note.Position <= -2)
                    {
                        DrawLedgerLines(note.Position, currentXPosition, noteY, canvas);
                    }
                    if (note.Name.Contains("#"))

                    {
                        DrawHash(currentXPosition, noteY, canvas);
                    }
                    DrawFilledNote(currentXPosition, noteY, canvas);
                    DrawInfo(currentXPosition, startStaff + 120, canvas, note);

                    currentXPosition += 40;
                }
            }
            else
            {
                List<Note> clickedButtonCopy = clickedButton;
                clickedButtonCopy.RemoveAt(0);
                foreach (Note note in clickedButtonCopy)
                {
                    
                    double noteY = GetNoteYPosition(note.Position, startStaff, lineSpacing);

                    DrawFilledNote(currentXPosition, noteY, canvas);
                    DrawInfo(currentXPosition, startStaff + 120, canvas, note);

                    currentXPosition += 40;
                }
            }           
        }

        public void InsertPicture(Song song, Canvas canvas)
        {
            Image staffImage = new Image
            {
                Source = new BitmapImage(new Uri("C:\\Users\\marle\\OneDrive\\Pulpit\\PianoAPp\\PianoAPp\\Images\\twinky.png")), // Ścieżka do obrazu
                
                Stretch = Stretch.UniformToFill // Dostosowanie obrazu
            };

            // Ustawienie pozycji obrazu (np. początek 5 linii)
            Canvas.SetLeft(staffImage, canvas.Width/10); // Przesuwamy obraz w lewo, aby pasował do początku pięciolinii
            Canvas.SetTop(staffImage, startStaff); 
            canvas.Children.Add(staffImage);

        }
        public void DrawNotesAndRests(Song song, Canvas canvas)
        {
            double currentX = 100; // Pozycja startowa na pięciolinii
            if (song.Title == "twinkyButBetter")
            {
                InsertPicture(song, canvas);
            }
            else
            {

                foreach (var element in song.Elements)
                {
                    // Pobierz odpowiedni Drawer z fabryki
                    var drawer = DrawerFactory.GetDrawer(element);
                    drawer.Draw(element, canvas, currentX, startStaff, lineSpacing);
                    DrawInfo(currentX, startStaff + 120, canvas, element);
                    currentX += element.Name.Contains("#") ? 40 + (element.Duration / 100) : 30 + (element.Duration / 100);
                }
            }
        }

        public double GetNoteYPosition(int position, double baseY, double lineSpacing)
        {
            return baseY - position * (lineSpacing / 2)+75;
        }

        public void AnimateStaff(Song song, Canvas canvas, double change)
        {
            double endX = EndX(song); 
            double duration =song.Length/change+2000;

            if (duration <= 0) return;

            var transform = new TranslateTransform();
            canvas.RenderTransform = transform;

            var animation = new DoubleAnimation
            {
                From = 0,
                To = -endX,
                Duration = TimeSpan.FromMilliseconds(duration),
                BeginTime = TimeSpan.FromMilliseconds(1500),

            };

            transform.BeginAnimation(TranslateTransform.XProperty, animation);

        }
        
        private void DrawLedgerLines(int position, double x, double baseY, Canvas canvas)
        {         
                int ledgerLineCount0 = Math.Abs(position / 2);
                int ledgerLineCount = (int)Math.Floor((decimal)ledgerLineCount0);
            ledgerline = ledgerLineCount;
            
                for (int i = 0; i <= ledgerLineCount; i++)
                {
                    var line = new Line
                    {
                        X1 = x - 5,
                        X2 = x + 15,
                        Y1 = 180+20*i,
                        Y2 = 180 + 20 * i,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(line);
                }
            
            
        }
        public void DrawInfo(double x, double y, Canvas canvas, MusicElement element)
        {
            double yPosition = element.Position < -2 ? 220 + 20 * ledgerline : y;
            if (element.Type == "pause")
            {
                TextBlock textBlock = new TextBlock
                {
                    Text ="P ",
                    FontSize = 14,
                    Foreground = new SolidColorBrush(Colors.Black)
                };
                TextBlock textBlock2 = new TextBlock
                {
                    Text =  element.Duration+" ms",
                    FontSize = 10,
                    Foreground = new SolidColorBrush(Colors.Black)
                };
                Canvas.SetLeft(textBlock, x);
                Canvas.SetTop(textBlock, yPosition);
                canvas.Children.Add(textBlock);
                Canvas.SetLeft(textBlock2, x-10);
                Canvas.SetTop(textBlock2, yPosition+20);
                canvas.Children.Add(textBlock2);
            }

            else
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = element.Name,
                    FontSize = 14,
                    Foreground = new SolidColorBrush(Colors.Black)
                };
                TextBlock textBlock2 = new TextBlock
                {
                    Text = element.Duration + "ms",
                    FontSize = 10,
                    Foreground = new SolidColorBrush(Colors.Black)
                };

                if (element.Name.Contains("#"))
                {
                    Canvas.SetLeft(textBlock, x - 15);
                    Canvas.SetTop(textBlock, yPosition);
                    Canvas.SetLeft(textBlock2, x-15);
                    Canvas.SetTop(textBlock2, yPosition + 20);
                }
                else
                {
                    // Ustawienie pozycji na kanwie
                    Canvas.SetLeft(textBlock, x);
                    Canvas.SetTop(textBlock, yPosition);
                    Canvas.SetLeft(textBlock2, x-10);
                    Canvas.SetTop(textBlock2, yPosition + 20);
                }
                canvas.Children.Add(textBlock);
                canvas.Children.Add(textBlock2);
            }          
        }
        public void DrawHash(double x, double y, Canvas canvas)
        {

            TextBlock textBlock = new TextBlock
            {
                Text = "#",
                FontSize = 25,
                Foreground = new SolidColorBrush(Colors.Black)
            };
            Canvas.SetLeft(textBlock, x-17);
            Canvas.SetTop(textBlock, y-15);
            canvas.Children.Add(textBlock);
        }
        public void DrawFilledNote(double x, double y, Canvas canvas)
        {
            var ellipse = new Ellipse
            {
                Width = 12,
                Height = 12,
                Fill = Brushes.Black
            };
            Canvas.SetLeft(ellipse, x-1);
            Canvas.SetTop(ellipse, y-1);
            canvas.Children.Add(ellipse);
        }
            
        
    }
}
