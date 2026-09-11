using System.Diagnostics;
using VideoToGifConverter.Core.Models;

namespace VideoToGifConverter.Core.Services;

public class VideoConverter
{
    private readonly IProcessRunner _processRunner;
    private readonly IFileSystem _fileSystem;
    private readonly IMediaInfoProvider _mediaInfoProvider;

    public string? LastError { get; private set; }

    public VideoConverter(IProcessRunner processRunner, IFileSystem fileSystem, 
        IMediaInfoProvider mediaInfoProvider)
    {
        _processRunner = processRunner;
        _fileSystem = fileSystem;
        _mediaInfoProvider = mediaInfoProvider;
    }

    public string GetFileName(string filePath)
    {
        return Path.GetFileName(filePath);
    }

    public async Task<bool> ConvertToGifAsync(string inputPath, 
        string outputPath, 
        GifConversionOptions options, 
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        LastError = null;

        cancellationToken.ThrowIfCancellationRequested();

        string pathExe = Path.Combine(AppContext.BaseDirectory, "ffmpeg", "ffmpeg.exe");

        if (!_fileSystem.FileExists(pathExe))
        {
            LastError = "FFmpeg executable was not found.";
            return false;
        }

        double duration = await _mediaInfoProvider.GetDurationAsync(inputPath);

        cancellationToken.ThrowIfCancellationRequested();

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = pathExe;
        startInfo.Arguments = $"-y -i \"{inputPath}\" -r {options.Fps} -vf \"scale={options.Width}:-1\" -progress pipe:2 \"{outputPath}\"";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardError = true;

        _processRunner.Start(startInfo);

        using CancellationTokenRegistration cancellationRegistration =
          cancellationToken.Register(() =>
          {
              _processRunner.Kill();
          });

        string? errorOutput = null;

        string? line;

        while ((line = await _processRunner.ReadStandardErrorLineAsync()) != null)
        {
            //System.Diagnostics.Debug.WriteLine($"FFMPEG LINE: {line}");

            cancellationToken.ThrowIfCancellationRequested();

            errorOutput = line;

            double? currentSeconds = FFmpegProgressParser.ParseOutTimeUs(line);

            //System.Diagnostics.Debug.WriteLine(
            //    $"PARSED TIME: {currentSeconds}, DURATION: {duration}");

            if (currentSeconds.HasValue && duration > 0)
            {
                double percentage =
                    currentSeconds.Value / duration * 100;

                percentage = Math.Clamp(percentage, 0, 100);

                //System.Diagnostics.Debug.WriteLine(
                //    $"CALCULATED PROGRESS: {percentage}%");

                //System.Diagnostics.Debug.WriteLine(
                //    $"Progress object null: {progress is null}");

                progress?.Report(percentage);
  
            }

            if (line == "progress=end")
            {
                //System.Diagnostics.Debug.WriteLine("Calling Report(100)");
                progress?.Report(100);
            }
        }

        await _processRunner.WaitForExitAsync();

        cancellationToken.ThrowIfCancellationRequested();

        if (_processRunner.ExitCode != 0)
        {
            LastError = errorOutput;
            return false;
        }

        return true;
    }

}
