using VideoToGifConverter.Core.Services;

namespace VideoToGifConverter.Tests;

public class FakeMediaInfoProvider : IMediaInfoProvider
{
    public double Duration { get; set; }

    public Exception? ExceptionToThrow { get; set; }

    public Task<double> GetDurationAsync(string inputPath)
    {
        if (ExceptionToThrow != null)
        {
            throw ExceptionToThrow;
        }

        return Task.FromResult(Duration);
    }
}