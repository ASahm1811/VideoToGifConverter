using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using VideoToGifConverter.Core.Services;
using System.Threading.Tasks;

namespace VideoToGifConverter.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly VideoConverter _converter = new VideoConverter();
        private string? _selectedVideoPath;

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

                _selectedVideoPath = filePath;

                string fileName = _converter.GetFileName(filePath);

                SelectedVideoText.Text = fileName;


            }
        }

        private async void Convert_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedVideoPath == null)
            {
                MessageBox.Show("Please select a video first.");
                return;
            }

            string outputPath = Path.ChangeExtension(_selectedVideoPath, ".gif");

            ConvertButton.IsEnabled = false;
            ConvertButton.Content = "Converting...";

            try
            {
                bool success = await _converter.ConvertToGifAsync(_selectedVideoPath, outputPath);

                if (success)
                {
                    MessageBox.Show("Conversion completed!");
                }
                else
                {
                    MessageBox.Show("Conversion failed.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred:\n{ex.Message}");
            }
            finally
            {
                ConvertButton.IsEnabled = true;
                ConvertButton.Content = "Convert";
            }
        }
    }
}