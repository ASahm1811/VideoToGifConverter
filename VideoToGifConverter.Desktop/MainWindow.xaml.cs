using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoToGifConverter.Core.Services;

namespace VideoToGifConverter.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectVideoButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle video selection logic here
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Select a video to convert";
            dialog.Filter = "Video Files|*.mp4;*.avi;*.mov;*.mkv;*.wmv|All Files|*.*";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filePath = dialog.FileName;

                MessageBox.Show(filePath);
            }
        }

    }
}