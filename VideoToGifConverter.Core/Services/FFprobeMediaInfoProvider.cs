using System.Diagnostics;

namespace VideoToGifConverter.Core.Services;

public class FFprobeMediaInfoProvider : IMediaInfoProvider
{
    private readonly IProcessRunner _processRunner;

    public FFprobeMediaInfoProvider(IProcessRunner processRunner)
    {
        _processRunner = processRunner;
    }

    public async Task<double> GetDurationAsync(string inputPath)
    {

        string pathExe = Path.Combine(AppContext.BaseDirectory, "ffmpeg", "ffprobe.exe");

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pathExe,
            Arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{inputPath}\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        _processRunner.Start(startInfo);

        string output = await _processRunner.ReadStandardOutputAsync();

        await _processRunner.WaitForExitAsync();

        if (!double.TryParse(
                output.Trim(),
                System.Globalization.CultureInfo.InvariantCulture,
                out double duration))
        {
            throw new InvalidOperationException(
                "Could not determine video duration.");
        }

        return duration;
    }
}