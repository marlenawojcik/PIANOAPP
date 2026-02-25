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
    public class DesignerClass
    {
        public static Style CreateButtonStyle()
        {
            // Tworzenie stylu
            var style = new Style(typeof(Button));
            style.Setters.Add(new Setter(Button.WidthProperty, 100.0));
            style.Setters.Add(new Setter(Button.HeightProperty, 30.0));
            style.Setters.Add(new Setter(Button.HorizontalAlignmentProperty, HorizontalAlignment.Center));
            style.Setters.Add(new Setter(Button.VerticalAlignmentProperty, VerticalAlignment.Center));
            style.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(114, 40, 64)))); // #FF722840
            style.Setters.Add(new Setter(Button.ForegroundProperty, Brushes.White));
            style.Setters.Add(new Setter(Button.FontWeightProperty, FontWeights.Bold));
            style.Setters.Add(new Setter(Button.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(114, 40, 64)))); // #FF722840
            style.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(2)));
            style.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(5)));
            var trigger = new Trigger
            {
                Property = Button.IsMouseOverProperty,
                Value = true
            };
            trigger.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Gray));
            trigger.Setters.Add(new Setter(Button.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(114, 40, 64)))); // #FF722840
            style.Triggers.Add(trigger);

            return style;
        }



        public static Button CreateButton(string content, string backgroundColor, RoutedEventHandler clickHandler)
        {
            var button = new Button
            {
                Content = content,
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString(backgroundColor),
                Foreground = Brushes.White,
                FontFamily = new FontFamily("Bodoni MT"),
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                Margin = new Thickness(10),
                Width = 250,
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            // Definicja stylu z Triggerami
            var style = new Style(typeof(Button));

            // Ustawienia domyślne
            style.Setters.Add(new Setter(Button.BackgroundProperty, button.Background));
            style.Setters.Add(new Setter(Button.ForegroundProperty, button.Foreground));
            style.Setters.Add(new Setter(Button.BorderBrushProperty, button.BorderBrush));
            style.Setters.Add(new Setter(Button.BorderThicknessProperty, button.BorderThickness));

            // Hover trigger (IsMouseOver)
            var mouseOverTrigger = new Trigger
            {
                Property = Button.IsMouseOverProperty,
                Value = true
            };
            mouseOverTrigger.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.DarkGray));
            mouseOverTrigger.Setters.Add(new Setter(Button.ForegroundProperty, Brushes.LightGray));

            // Click trigger (IsPressed)
            var isPressedTrigger = new Trigger
            {
                Property = Button.IsPressedProperty,
                Value = true
            };
            isPressedTrigger.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.DimGray));
            isPressedTrigger.Setters.Add(new Setter(Button.ForegroundProperty, Brushes.White));

            // Dodajemy triggery do stylu
            style.Triggers.Add(mouseOverTrigger);
            style.Triggers.Add(isPressedTrigger);

            // Przypisujemy styl do przycisku
            button.Style = style;

            // Dodanie obsługi zdarzenia Click
            button.Click += clickHandler;

            return button;
        }
        public static Button CreateImageButton(string imagePath, RoutedEventHandler clickHandler, double opacity = 1.0)
        {
            Image image = new Image
            {
                Source = new BitmapImage(new Uri(imagePath)),
                Width = 70,
                Height = 60,
                RenderTransformOrigin = new Point(1.144, 0.896),
                Opacity = opacity
            };

            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(image);
            var button = new Button
            {
                Width = 70,
                Height = 60,
                Background = null,
                BorderBrush = Brushes.Transparent,
                Content = stackPanel
            };
            button.Click += clickHandler;
            return button;
        }

        public static Button CreateStyledButton(string content, RoutedEventHandler clickHandler)
        {
            // Tworzenie szablonu przycisku
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            borderFactory.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Button.BorderBrushProperty));
            borderFactory.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Button.BorderThicknessProperty));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));

            var contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            borderFactory.AppendChild(contentPresenterFactory);

            var template = new ControlTemplate(typeof(Button))
            {
                VisualTree = borderFactory
            };

            // Konfiguracja przycisku
            var button = new Button
            {
                Content = content,
                FontSize = 20,
                Height = 100,
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromRgb(37, 52, 67)),
                Foreground = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(37, 52, 67)),
                BorderThickness = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                FontWeight = FontWeights.Bold,
                Padding = new Thickness(0, 0, 0, 0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Template = template
            };
            button.Click += clickHandler;
            return button;
        }


    }

}
