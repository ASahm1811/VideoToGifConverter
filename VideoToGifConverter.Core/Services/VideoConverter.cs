using System.Diagnostics;
using VideoToGifConverter.Core.Models;

namespace VideoToGifConverter.Core.Services;

public class VideoConverter
{
    private readonly IProcessRunner _processRunner;
    private readonly IFileSystem _fileSystem;

    public string? LastError { get; private set; }

    public VideoConverter(IProcessRunner processRunner, IFileSystem fileSystem)
    {
        _processRunner = processRunner;
        _fileSystem = fileSystem;
    }

    public string GetFileName(string filePath)
    {
        return Path.GetFileName(filePath);
    }

    public async Task<bool> ConvertToGifAsync(string inputPath, string outputPath, GifConversionOptions options)
    {
        LastError = null;

        string pathExe = Path.Combine(AppContext.BaseDirectory, "ffmpeg", "ffmpeg.exe");

        if (!_fileSystem.FileExists(pathExe))
        {
            LastError = "FFmpeg executable was not found.";
            return false;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = pathExe;
        startInfo.Arguments = $"-y -i \"{inputPath}\" -r {options.Fps} -vf \"scale={options.Width}:-1\" \"{outputPath}\"";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardError = true;

        _processRunner.Start(startInfo);

        string? errorOutput = null;

        string? line;

        while ((line = await _processRunner.ReadStandardErrorLineAsync()) != null)
        {
            errorOutput = line;
        }

        await _processRunner.WaitForExitAsync();

        if (_processRunner.ExitCode != 0)
        {
            LastError = errorOutput;
            return false;
        }

        return true;
    }

}
