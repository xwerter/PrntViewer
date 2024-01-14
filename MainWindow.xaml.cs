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
        ImagLoader imageLoader = new ImagLoader(666);

        public MainWindow()
        {
            InitializeComponent();
            UpdateContent();
        }

        private async void buttonForward_Click(object sender, RoutedEventArgs e)
        {
            await imageLoader.IncreaseImageUrl();
            UpdateContent();
        }

        private async void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            await imageLoader.DecreaseImageUrl();
            UpdateContent();
        }

        private async void buttonDownload_Click(object sender, RoutedEventArgs e)
        {
            await imageLoader.DownloadAndSaveImageAsync();
        }

        private async void buttonSerch_Click(object sender, RoutedEventArgs e)
        {
            int number;
            int.TryParse(textBoxSearch.Text, out number);
            if (number != 0)
            {
                await imageLoader.SetImageUrl(number);
                UpdateContent();
            }
        }

        private void UpdateContent()
        {
            labelImgNumber.Content = $"{imageLoader.CurrentImageNumber}/{imageLoader.CurrentImageAlphanumberValue}";

            var bitsrc = new BitmapImage(new Uri(imageLoader.CurrentImageUrl));
            ImageViewer.Source = bitsrc;
        }
    }
}
