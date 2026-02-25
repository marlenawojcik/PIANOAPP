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
    public partial class SelectionWindow : Window
    {
        private Grid mainGrid;
        public SelectionWindow()
        {
            mainGrid = new Grid();
            InitializeComponents(mainGrid);
        }

        public virtual void InitializeComponents(Grid mainGrid)
        {
            this.Title = "ModeSelectionWindow";
            this.Height = 600;
            this.Width = 800;
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            ImageBrush backgroundBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/chorus.jpg")),
                Stretch = Stretch.Fill
            };
            mainGrid.Background = backgroundBrush;
        }



    }
}
