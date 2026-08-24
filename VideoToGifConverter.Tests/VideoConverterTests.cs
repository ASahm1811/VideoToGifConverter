using VideoToGifConverter.Core.Models;
using VideoToGifConverter.Core.Services;

namespace VideoToGifConverter.Tests
{
    public class VideoConverterTests
    {
        [Fact]
        public void GetFileName_ShouldReturnCorrectFileName()
        {
            // Arrange
            var fakeProcessRunner = new FakeProcessRunner();
            var fakeFileSystem = new FakeFileSystem
            {
                FileExistsResult = true
            };
            var converter = new VideoConverter(fakeProcessRunner, fakeFileSystem);
            string filePath = @"C:\Videos\sample.mp4";

            // Act
            string fileName = converter.GetFileName(filePath);

            // Assert
            Assert.Equal("sample.mp4", fileName);
        }

        [Fact]
        public void GetFileName_ShouldHandleSpacesInFileName()
        {
            // Arrange
            var fakeProcessRunner = new FakeProcessRunner();
            var fakeFileSystem = new FakeFileSystem
            {
                FileExistsResult = true
            };
            var converter = new VideoConverter(fakeProcessRunner, fakeFileSystem);
            string filePath = @"C:\Videos\my sample video.mp4";

            // Act
            string fileName = converter.GetFileName(filePath);

            // Assert
            Assert.Equal("my sample video.mp4", fileName);
        }

        [Fact]
        public void GetFileName_ShouldHandleDifferentExtensions()
        {
            // Arrange
            var fakeProcessRunner = new FakeProcessRunner();
            var fakeFileSystem = new FakeFileSystem
            {
                FileExistsResult = true
            };
            var converter = new VideoConverter(fakeProcessRunner, fakeFileSystem);
            string filePath = @"C:\Videos\sample.avi";
            // Act
            string fileName = converter.GetFileName(filePath);
            // Assert
            Assert.Equal("sample.avi", fileName);
        }

        [Fact]
        public void GetFileName_ShouldHandleNoExtension()
        {
            // Arrange
            var fakeProcessRunner = new FakeProcessRunner();
            var fakeFileSystem = new FakeFileSystem
            {
                FileExistsResult = true
            };
            var converter = new VideoConverter(fakeProcessRunner, fakeFileSystem);
            string filePath = @"C:\Videos\sample";
            // Act
            string fileName = converter.GetFileName(filePath);
            // Assert
            Assert.Equal("sample", fileName);
        }

        [Fact]
        public void GetFileName_ShouldHandleEmptyPath()
        {
            // Arrange
            var fakeProcessRunner = new FakeProcessRunner();
            var fakeFileSystem = new FakeFileSystem
            {
                FileExistsResult = true
            };
            var converter = new VideoConverter(fakeProcessRunner, fakeFileSystem);
            string filePath = string.Empty;
            // Act
            string fileName = converter.GetFileName(filePath);
            // Assert
            Assert.Equal(string.Empty, fileName);
        }

        [Fact]
        public async Task ConvertToGifAsync_ShouldReturnFalse_WhenFfmpegIsMissing()
        {
            // Arrange
            var fakeProcessRunner = new FakeProcessRunner();
            var fakeFileSystem = new FakeFileSystem
            {
                FileExistsResult = false
            };
            var converter = new VideoConverter(fakeProcessRunner, fakeFileSystem);

            var options = new GifConversionOptions
            {
                Fps = 10,
                Width = 480
            };

            string inputPath = "test.mp4";
            string outputPath = "test.gif";

            // Act
            bool result = await converter.ConvertToGifAsync(
                inputPath,
                outputPath,
                options);

            // Assert
            Assert.False(result);
            Assert.Equal("FFmpeg executable was not found.", converter.LastError);
        }

        [Fact]
        public async Task ConvertToGifAsync_ShouldReturnFalse_WhenProcessFails()
        {
            // Arrange
            var fakeProcessRunner = new FakeProcessRunner
            {
                ExitCode = 1,
                ErrorOutput = "Fake FFmpeg error"
            };

            var fakeFileSystem = new FakeFileSystem
            {
                FileExistsResult = true
            };

            var converter = new VideoConverter(fakeProcessRunner, fakeFileSystem);

            var options = new GifConversionOptions
            {
                Fps = 10,
                Width = 480
            };

            string inputPath = "test.mp4";
            string outputPath = "test.gif";

            // Act
            bool result = await converter.ConvertToGifAsync(
                inputPath,
                outputPath,
                options);

            // Assert
            Assert.False(result);
            Assert.Equal("Fake FFmpeg error", converter.LastError);
        }

        [Fact]
        public async Task ConvertToGifAsync_ShouldReturnTrue_WhenProcessSucceeds()
        {
            // Arrange
            var fakeProcessRunner = new FakeProcessRunner
            {
                ExitCode = 0,
                ErrorOutput = string.Empty
            };

            var fakeFileSystem = new FakeFileSystem
            {
                FileExistsResult = true
            };

            var converter = new VideoConverter(
                fakeProcessRunner,
                fakeFileSystem);

            var options = new GifConversionOptions
            {
                Fps = 10,
                Width = 480
            };

            string inputPath = "test.mp4";
            string outputPath = "test.gif";

            // Act
            bool result = await converter.ConvertToGifAsync(
                inputPath,
                outputPath,
                options);

            // Assert
            Assert.True(result);
            Assert.Null(converter.LastError);
        }

        [Fact]
        public async Task ConvertToGifAsync_ShouldUseCorrectFfmpegArguments()
        {
            // Arrange
            var fakeProcessRunner = new FakeProcessRunner
            {
                ExitCode = 0
            };

            var fakeFileSystem = new FakeFileSystem
            {
                FileExistsResult = true
            };

            var converter = new VideoConverter(
                fakeProcessRunner,
                fakeFileSystem);

            var options = new GifConversionOptions
            {
                Fps = 15,
                Width = 720
            };

            string inputPath = @"C:\Videos\my video.mp4";
            string outputPath = @"C:\Output\my video.gif";

            // Act
            bool result = await converter.ConvertToGifAsync(
                inputPath,
                outputPath,
                options);

            // Assert
            Assert.True(result);
            Assert.NotNull(fakeProcessRunner.StartInfo);

            Assert.Equal(
                $"-y -i \"{inputPath}\" -r 15 -vf \"scale=720:-1\" \"{outputPath}\"",
                fakeProcessRunner.StartInfo!.Arguments);
        }

        [Fact]
        public async Task ConvertToGifAsync_ShouldConfigureProcessCorrectly()
        {
            // Arrange
            var fakeProcessRunner = new FakeProcessRunner
            {
                ExitCode = 0
            };

            var fakeFileSystem = new FakeFileSystem
            {
                FileExistsResult = true
            };

            var converter = new VideoConverter(
                fakeProcessRunner,
                fakeFileSystem);

            var options = new GifConversionOptions
            {
                Fps = 10,
                Width = 480
            };

            // Act
            bool result = await converter.ConvertToGifAsync(
                "test.mp4",
                "test.gif",
                options);

            // Assert
            Assert.True(result);
            Assert.NotNull(fakeProcessRunner.StartInfo);

            Assert.False(fakeProcessRunner.StartInfo!.UseShellExecute);
            Assert.True(fakeProcessRunner.StartInfo.CreateNoWindow);
            Assert.True(fakeProcessRunner.StartInfo.RedirectStandardError);
        }


    }
}
