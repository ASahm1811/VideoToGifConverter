using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VideoToGifConverter.Core.Models;
using VideoToGifConverter.Core.Services;

namespace VideoToGifConverter.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ProcessRunner _processRunner = new();
        private readonly FileSystem _fileSystem = new();
        private readonly FFprobeMediaInfoProvider _mediaInfoProvider;
        private readonly VideoConverter _converter;

        private string? _selectedVideoPath;

        public MainWindow()
        {
            InitializeComponent();

            _mediaInfoProvider = new FFprobeMediaInfoProvider(_processRunner);

            _converter = new VideoConverter(
                _processRunner,
                _fileSystem,
                _mediaInfoProvider);
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

            // Validate FPS
            if (FpsComboBox.SelectedItem is not ComboBoxItem selectedItem)
            {
                return;
            }

            if (!int.TryParse(selectedItem.Content?.ToString(), out int fps))
            {
                MessageBox.Show("Invalid FPS value.");
                return;
            }

            // Validate width
            if (!int.TryParse(WidthTextBox.Text, out int width))
            {
                MessageBox.Show("Please enter a valid width.");
                return;
            }

            if (width <= 0)
            {
                MessageBox.Show("Width must be greater than 0.");
                return;
            }

            // Create options
            var options = new GifConversionOptions
            {
                Fps = fps,
                Width = width
            };

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "GIF files (*.gif)|*.gif";

            string fileName = Path.GetFileNameWithoutExtension(_selectedVideoPath);
            saveFileDialog.FileName = $"{fileName}.gif";

            bool? result = saveFileDialog.ShowDialog();

            if (result != true)
            {
                return;
            }

            string outputPath = saveFileDialog.FileName;

            // NOW start the converting UI
            ConvertButton.IsEnabled = false;
            ConvertButton.Content = "Converting...";
            ConversionProgressBar.Visibility = Visibility.Visible;

            try
            {
                bool success = await _converter.ConvertToGifAsync(
                    _selectedVideoPath,
                    outputPath,
                    options);

                if (success)
                {
                    MessageBox.Show("Conversion completed!");
                }
                else
                {
                    MessageBox.Show($"Conversion failed:\n\n{_converter.LastError}");
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
                ConversionProgressBar.Visibility = Visibility.Collapsed;
            }
        }
    }
}