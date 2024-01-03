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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScreenshotViewer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ImagLoader il = new ImagLoader(666);

        public MainWindow()
        {
            InitializeComponent();
            UpdateContent();
        }

        private async void buttonForward_Click(object sender, RoutedEventArgs e)
        {
            il.IncreaseImageUrl();

            UpdateContent();

        }

        private async void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            il.DecreaseImageUrl();

            UpdateContent();
        }

        private void buttonDownload_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonSerch_Click(object sender, RoutedEventArgs e)
        {
            int number;
            int.TryParse(textBoxSearch.Text, out number);
            if (number != 0)
            {
                il.SetImageUrl(number);
                UpdateContent();
            }
            
        }

        private void UpdateContent()
        {
            labelImgNumber.Content = $"{il.CurentImageNumber}/{il.CurentImageAlphanumberValue}";

            var bitsrc = new BitmapImage(new Uri(il.CurrentImageUrl));
            ImageViewer.Source = bitsrc;
        }
    }
}
