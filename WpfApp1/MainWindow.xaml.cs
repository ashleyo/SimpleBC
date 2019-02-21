using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            TheNumber = 2;
        }

        public int TheNumber { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void numeric_texbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[0-9]");
            if (!regex.IsMatch(e.Text) || NumericBox.Text != "")
                {
                NumericBox.Text = String.Empty;
                e.Handled = true;
                }
        }

        public void ClickHandler(object sender, RoutedEventArgs e)
        {
            if (sender is Button B)
            {
                TheNumber = B.Name.Equals("Up") ? TheNumber + 1 : TheNumber - 1;
                NotifyPropertyChanged("TheNumber");
            }
        }
    }

    public class BrushColorConverter : IValueConverter
    {
        private Dictionary<int, string> colors = new Dictionary<int, string>()
        {
            { 0,"White" },
            { 1,"Red" },
            { 2, "Blue" },
            { 3,"Green"},
            { 4,"Yellow"},
            { 5,"Magenta"},
            { 6,"Cyan" } 
        };

        public object Convert(object value, Type targetType,
                                object parameter, CultureInfo culture)
            => colors[Math.Abs(((int)value % colors.Count))];

        public object ConvertBack(object value, Type targetType,
                                    object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
