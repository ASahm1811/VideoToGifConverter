using VideoToGifConverter.Core.Services;

namespace VideoToGifConverter.Tests;

public class FFmpegProgressParserTests
{
    [Fact]
    public void ParseTime_ShouldReturnTime_WhenLineContainsValidTime()
    {
        // Arrange
        string line =
            "frame=250 fps=30 time=00:00:08.33 bitrate=1234.5kbits/s";

        // Act
        TimeSpan? result = FFmpegProgressParser.ParseTime(line);

        // Assert
        Assert.Equal(
            TimeSpan.FromSeconds(8.33),
            result);
    }

    [Fact]
    public void ParseTime_ShouldReturnNull_WhenLineDoesNotContainTime()
    {
        // Arrange
        string line =
            "frame=250 fps=30 bitrate=1234.5kbits/s";

        // Act
        TimeSpan? result = FFmpegProgressParser.ParseTime(line);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ParseTime_ShouldReturnNull_WhenTimeIsInvalid()
    {
        // Arrange
        string line =
            "frame=250 fps=30 time=not-a-time bitrate=1234.5kbits/s";

        // Act
        TimeSpan? result = FFmpegProgressParser.ParseTime(line);

        // Assert
        Assert.Null(result);
    }
}