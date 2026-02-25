using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Win32;

using System.Windows;
using System.Windows.Controls;


namespace PIANOAPP
{
    internal class SaveWindow
    {
        public static void SaveCanvasToFile(Canvas canvas)
        {
            // Tworzenie i konfigurowanie dialogu zapisu pliku
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "SaveWindowImage",
                Filter = "Image PNG (*.png)|*.png", // Możesz zmienić na inny format, jeśli chcesz
                FileName = "file1.png" // Domyślna nazwa pliku
            };

            // Wyświetlenie dialogu zapisu i sprawdzenie, czy użytkownik wybrał ścieżkę
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                // Ustawienie rozmiarów płótna (ważne dla pełnego renderowania)
                Size size = new Size(canvas.Width, canvas.Height);
                canvas.Measure(size);
                canvas.Arrange(new Rect(new Size(canvas.Width, canvas.Height)));

                // Tworzenie obrazu z płótna
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                     (int)canvas.Width,
                    (int)canvas.Height,
                    96, // DPI w poziomie
                        96, // DPI w pionie
                    PixelFormats.Pbgra32);
                renderBitmap.Render(canvas);
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                MessageBox.Show($"Image saved to: {filePath}");
            }
            else
            {
                MessageBox.Show("Save canceled.");
            }
        }
    }
}
