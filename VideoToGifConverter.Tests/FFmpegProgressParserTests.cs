using VideoToGifConverter.Core.Services;

namespace VideoToGifConverter.Tests;

public class FFmpegProgressParserTests
{
    [Fact]
    public void ParseOutTimeUs_ShouldReturnSeconds_WhenLineContainsValidValue()
    {
        // Arrange
        string line = "out_time_us=5000000";

        // Act
        double? result = FFmpegProgressParser.ParseOutTimeUs(line);

        // Assert
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void ParseOutTimeUs_ShouldReturnNull_WhenLineDoesNotContainOutTimeUs()
    {
        // Arrange
        string line = "frame=100";

        // Act
        double? result = FFmpegProgressParser.ParseOutTimeUs(line);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ParseOutTimeUs_ShouldReturnNull_WhenValueIsInvalid()
    {
        // Arrange
        string line = "out_time_us=not-a-number";

        // Act
        double? result = FFmpegProgressParser.ParseOutTimeUs(line);

        // Assert
        Assert.Null(result);
    }
}