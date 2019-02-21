using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Security.Cryptography;


namespace SimpleBC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        private BlockChain theBlockChain;

        public MainWindow()
        {
            theBlockChain = BlockChain.GetInitializedBlockChain();
            // an alternative hashing method can be passed here, eg
            // theBlockChain = BlockChain.GetInitializedBlockChain(new SHA512CryptoServiceProvider());

            InitializeComponent();
            Width = Hash.HASHLEN * 10 + 100; //size to suit hash algorithm
            DataContext = theBlockChain;
            TC.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TC.SelectedContent is Block B)
                    Task.Factory.StartNew(B.Mine);
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.scan.co.uk");
        }
    }



    public class BrushColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? Colors.DarkGreen.ToString() : Colors.Red.ToString();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }

    
}


