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

        if (options.Fps <= 0)
        {
            LastError = "FPS must be greater than 0.";
            return false;
        }

        if (options.Width <= 0)
        {
            LastError = "Width must be greater than 0.";
            return false;
        }

        if (string.Equals(
        inputPath,
        outputPath,
        StringComparison.OrdinalIgnoreCase))
        {
            LastError = "Input and output files must be different.";
            return false;
        }

        string? outputDirectory = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrEmpty(outputDirectory) &&
            !_fileSystem.DirectoryExists(outputDirectory))
        {
            LastError = "Output directory was not found.";
            return false;
        }

        string pathExe = Path.Combine(AppContext.BaseDirectory, "ffmpeg", "ffmpeg.exe");

        if (!_fileSystem.FileExists(pathExe))
        {
            LastError = "FFmpeg executable was not found.";
            return false;
        }

        if (!_fileSystem.FileExists(inputPath))
        {
            LastError = "Input video file was not found.";
            return false;
        }

        double duration;

        try
        {
            duration = await _mediaInfoProvider.GetDurationAsync(inputPath);
        }
        catch (InvalidOperationException ex)
        {
            LastError = ex.Message;
            return false;
        }

        if (duration <= 0)
        {
            LastError = "Invalid video duration.";
            return false;
        }

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

        try
        {
            string? errorOutput = null;

            string? line;

            while ((line = await _processRunner.ReadStandardErrorLineAsync()) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();

                errorOutput = line;

                double? currentSeconds =
                    FFmpegProgressParser.ParseOutTimeUs(line);

                if (currentSeconds.HasValue && duration > 0)
                {
                    double percentage =
                        currentSeconds.Value / duration * 100;

                    percentage = Math.Clamp(percentage, 0, 100);

                    progress?.Report(percentage);
                }

                if (line == "progress=end")
                {
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
        catch (OperationCanceledException)
        {
            _fileSystem.DeleteFile(outputPath);
            throw;
        }
    }

}
