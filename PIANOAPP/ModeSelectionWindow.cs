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
    public partial class ModeSelectionWindow : SelectionWindow
    {
        private Song selectedSong;

        public ModeSelectionWindow(Song song):base()
        {
            selectedSong = song;
        }
        public override void InitializeComponents(Grid mainGrid)
        {
            base.InitializeComponents(mainGrid);
            // Tworzenie przycisków
            Button sheetButton = DesignerClass.CreateStyledButton("Sheet", OnSheetButtonClick);
            Button waterfallButton = DesignerClass.CreateStyledButton("Waterfall", OnWaterfallButtonClick);

            // Ustawienie przycisków w siatce
            Grid.SetColumn(sheetButton, 0);
            Grid.SetColumn(waterfallButton, 1);

            // Marginesy i pozycjonowanie
            sheetButton.Margin = new Thickness(10, 179, 50, 195);
            waterfallButton.Margin = new Thickness(10, 179, 50, 195);

            // Dodanie przycisków do siatki
            mainGrid.Children.Add(sheetButton);
            mainGrid.Children.Add(waterfallButton);

            // Ustawienie głównej zawartości okna
            this.Content = mainGrid;
        }
        private void OnSheetButtonClick(object sender, RoutedEventArgs e)
        {
            var selectionWindow = new KeyboardWithSongWindow(selectedSong);
            selectionWindow.Show();
            this.Close();
        }
        private void OnWaterfallButtonClick(object sender, RoutedEventArgs e)
        {
            var selectionWindow = new WaterfallKeyboardWindow(selectedSong);
            selectionWindow.Show();
            this.Close();
        }
    }  
}
