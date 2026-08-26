namespace VideoToGifConverter.Core.Services;

public interface IMediaInfoProvider
{
    Task<double> GetDurationAsync(string inputPath);
}