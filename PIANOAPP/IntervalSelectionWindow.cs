using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace PIANOAPP
{
    public partial class IntervalSelectionWindow:SelectionWindow
    {
        public  IntervalSelectionWindow():base(){}
        public override void InitializeComponents(Grid mainGrid)
        {
            base.InitializeComponents(mainGrid);
           

            // Tworzenie przycisków
            Button sheetButton = DesignerClass.CreateStyledButton("Learn", OnLearnButtonClick);
            Button waterfallButton = DesignerClass.CreateStyledButton("Listen", OnListenButtonClick);

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
        private void OnLearnButtonClick(object sender, RoutedEventArgs e)
        {
            var selectionWindow = new IntervalLearnWindow();
            selectionWindow.Show();
            this.Close();
        }

        private void OnListenButtonClick(object sender, RoutedEventArgs e)
        {
            var selectionWindow = new IntervalListenWindow();
            selectionWindow.Show();
            this.Close();
        }
    }
}
