using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PIANOAPP
{
    public interface IDrawer
    {
        void Draw(MusicElement element, Canvas canvas, double currentX, double startStaff, double lineSpacing);
    }
    public static class DrawerFactory
    {
        public static IDrawer GetDrawer(MusicElement element)
        {
            return element.Type switch
            {
                "note" => new NoteDrawer(),
                "pause" => new PauseDrawer(),
                _ => throw new ArgumentException($"Unknown element type: {element.Type}")
            };
        }
    }
    public class PauseDrawer : IDrawer
    {
        public void Draw(MusicElement element, Canvas canvas, double currentX, double startStaff, double lineSpacing)
        {
            double staffY = startStaff;

            // Rysowanie pauz w zależności od długości
            switch (element.Duration)
            {
                case 4000:
                    DrawPauseRectangle(currentX, staffY + 20, 10, 5, canvas);
                    break;
                case 3000:
                    DrawPauseRectangle(currentX, staffY + 30, 10, 5, canvas);
                    DrawDot(currentX + 15, staffY + 20, canvas);
                    break;
                case 2000:
                    DrawPauseRectangle(currentX, staffY + 35, 10, 5, canvas);
                    break;
                case 1000:
                    DrawEighthPause(currentX, staffY + lineSpacing, canvas);
                    DrawEighthPause(currentX, staffY + lineSpacing, canvas);
                    break;
                case 500:
                    DrawEighthPause(currentX, staffY + lineSpacing, canvas);
                    break;
                case 250:
                    DrawSixteenthPause(currentX, staffY + lineSpacing, canvas);
                    break;
                default:
                    DrawNakedPauseRectangle(currentX, staffY + 25, 15, 10, canvas);
                    break;
            }
        }
        private void DrawDot(double x, double y, Canvas canvas)
        {
            var ellipse = new Ellipse
            {
                Width = 4,
                Height = 4,
                Fill = Brushes.Black
            };
            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);
            canvas.Children.Add(ellipse);
        }

        private void DrawPauseRectangle(double x, double y, double width, double height, Canvas canvas)
        {
            var rect = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = Brushes.Black
            };
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            canvas.Children.Add(rect);
        }

        private void DrawNakedPauseRectangle(double x, double y, double width, double height, Canvas canvas)
        {
            var rect = new Rectangle
            {
                Width = width,
                Height = height,
                Stroke = Brushes.Black, 
                StrokeThickness = 2,
                Fill = Brushes.Transparent 
            };
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            canvas.Children.Add(rect);
        }

        private void DrawEighthPause(double x, double y, Canvas canvas)
        {
            var line = new Line
            {
                X1 = x,
                Y1 = y,
                X2 = x + 10,
                Y2 = y + 20,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            canvas.Children.Add(line);

            var ellipse = new Ellipse
            {
                Width = 4,
                Height = 4,
                Fill = Brushes.Black
            };
            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y + 15);
            canvas.Children.Add(ellipse);
        }

        private void DrawSixteenthPause(double x, double y, Canvas canvas)
        {
            DrawEighthPause(x, y, canvas);

            var line = new Line
            {
                X1 = x,
                Y1 = y + 10,
                X2 = x + 10,
                Y2 = y + 30,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            canvas.Children.Add(line);
        }
    }

        public class NoteDrawer : IDrawer
        {
            int ledgerline = 0;
            double lineSpacing = 20;
            double startStaff = 100;
            public void Draw(MusicElement element, Canvas canvas, double currentX, double startStaff, double lineSpacing)
            {
                double noteY = GetNoteYPosition(element.Position, startStaff, lineSpacing);
                var noteX = currentX;

                // Rysowanie linii pomocniczych
                if (element.Position <= -2)
                {
                    DrawLedgerLines(element.Position, noteX, noteY, canvas);
                }

                // Rysowanie krzyżyka (#) dla nut z podwyższeniem
                if (element.Name.Contains("#"))
                {
                    DrawHash(noteX, noteY, canvas);
                }

                // Rysowanie nuty w zależności od długości
                switch (element.Duration)
                {
                    case 1000:
                        DrawFilledNoteWithStem(noteX, noteY, canvas);
                            break;
                    case 1500:
                        DrawFilledNoteWithStem(noteX, noteY, canvas);
                        DrawDot(noteX + 12, noteY + 5, canvas);
                        break;
                    case 2000:
                        DrawEmptyNoteWithStem(noteX, noteY, canvas);
                        break;
                    case 3000:
                        DrawEmptyNoteWithStem(noteX, noteY, canvas);
                        DrawDot(noteX + 12, noteY + 5, canvas);
                        break;
                    case 4000:
                        DrawWholeNote(noteX, noteY, canvas);
                        break;
                    case 750:
                        DrawEighthNoteWithFlag(noteX, noteY, canvas, 1);
                        DrawDot(noteX + 12, noteY + 5, canvas);
                        break;
                    case 500:
                        DrawEighthNoteWithFlag(noteX, noteY, canvas, 1);
                        break;
                    case 250:
                        DrawEighthNoteWithFlag(noteX, noteY, canvas, 2);
                        break;
                    default:
                        DrawFilledNote(noteX, noteY, canvas);
                        break;
                }
            }

            private void DrawDot(double x, double y, Canvas canvas)
            {
                var ellipse = new Ellipse
                {
                    Width = 4,
                    Height = 4,
                    Fill = Brushes.Black
                };
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);
                canvas.Children.Add(ellipse);
            }

            private void DrawHash(double x, double y, Canvas canvas)
            {

                TextBlock textBlock = new TextBlock
                {
                    Text = "#",
                    FontSize = 25,
                    Foreground = new SolidColorBrush(Colors.Black)
                };
                Canvas.SetLeft(textBlock, x - 17);
                Canvas.SetTop(textBlock, y - 15);
                canvas.Children.Add(textBlock);
            }
            private double GetNoteYPosition(int position, double baseY, double lineSpacing)
            {
                return baseY - position * (lineSpacing / 2) + 75;
            }
            private void DrawFilledNote(double x, double y, Canvas canvas)
            {
                var ellipse = new Ellipse
                {
                    Width = 12,
                    Height = 12,
                    Fill = Brushes.Black
                };
                Canvas.SetLeft(ellipse, x - 1);
                Canvas.SetTop(ellipse, y - 1);
                canvas.Children.Add(ellipse);
            }
            private void DrawFilledNoteWithStem(double x, double y, Canvas canvas)
            {
                var ellipse = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Black
                };
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);

                if (y <= 125)
                {
                    var line = new Line
                    {
                        X1 = x,
                        Y1 = y + 5,
                        X2 = x,
                        Y2 = y + 45,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(ellipse);
                    canvas.Children.Add(line);
                }
                else
                {
                    var line = new Line
                    {
                        X1 = x + 10,
                        Y1 = y + 5,
                        X2 = x + 10,
                        Y2 = y - 40,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(ellipse);
                    canvas.Children.Add(line);
                }
            }
            private void DrawEmptyNoteWithStem(double x, double y, Canvas canvas)
            {
                var ellipse = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);
                canvas.Children.Add(ellipse);

                if (y <= 125)
                {
                    var line = new Line
                    {

                        X1 = x,
                        Y1 = y + 5,
                        X2 = x,
                        Y2 = y + 45,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(line);
                }
                else
                {
                    var line = new Line
                    {
                        X1 = x + 10,
                        Y1 = y + 5,
                        X2 = x + 10,
                        Y2 = y - 40,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(line);
                }
            }

            private void DrawWholeNote(double x, double y, Canvas canvas)
            {
                var ellipse = new Ellipse
                {
                    Width = 13,
                    Height = 10,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);
                canvas.Children.Add(ellipse);
            }
            private void DrawEighthNoteWithFlag(double x, double y, Canvas canvas, int flags)
            {
                DrawFilledNoteWithStem(x, y, canvas);

                // Dodanie daszków
                for (int i = 0; i < flags; i++)
                {
                    if (y <= 125)
                    {
                        var line = new Line
                        {
                            X1 = x,
                            Y1 = y + 45 + (i * 5),
                            X2 = x + 10,
                            Y2 = y + 40 + (i * 5),
                            Stroke = Brushes.Black,
                            StrokeThickness = 1
                        };
                        canvas.Children.Add(line);
                    }
                    else
                    {
                        var line = new Line
                        {
                            X1 = x + 10,
                            Y1 = y - 40 + (i * 5),
                            X2 = x + 20,
                            Y2 = y - 35 + (i * 5),
                            Stroke = Brushes.Black,
                            StrokeThickness = 1
                        }; canvas.Children.Add(line);
                    }

                }
            }

            private void DrawLedgerLines(int position, double x, double baseY, Canvas canvas)
            {
                int ledgerLineCount = (int)Math.Floor((decimal)Math.Abs(position / 2));
                ledgerline = ledgerLineCount;
                for (int i = 0; i <= ledgerLineCount; i++)
                {
                    var line = new Line
                    {
                        X1 = x - 5,
                        X2 = x + 15,
                        Y1 = 180 + 20 * i,
                        Y2 = 180 + 20 * i,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(line);
                }
            }
        }
}
