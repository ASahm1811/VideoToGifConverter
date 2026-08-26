using VideoToGifConverter.Core.Services;

namespace VideoToGifConverter.Tests;

public class FFprobeMediaInfoProviderTests
{
    [Fact]
    public async Task GetDurationAsync_ShouldReturnDuration()
    {
        // Arrange
        var fakeProcessRunner = new FakeProcessRunner
        {
            ExitCode = 0,
            StandardOutput = "20.533333"
        };

        var provider = new FFprobeMediaInfoProvider(fakeProcessRunner);

        // Act
        double duration = await provider.GetDurationAsync("test.mp4");

        // Assert
        Assert.Equal(20.533333, duration);
    }
}