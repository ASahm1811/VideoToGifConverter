using VideoToGifConverter.Core.Services;

namespace VideoToGifConverter.Tests;

public class FakeMediaInfoProvider : IMediaInfoProvider
{
    public double Duration { get; set; }

    public Task<double> GetDurationAsync(string inputPath)
    {
        return Task.FromResult(Duration);
    }
}