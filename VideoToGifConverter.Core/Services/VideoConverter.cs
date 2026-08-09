using System.Diagnostics;
using VideoToGifConverter.Core.Models;

namespace VideoToGifConverter.Core.Services;

public class VideoConverter
{
    public string? LastError { get; private set; }

    public string GetFileName(string filePath)
    {
        return Path.GetFileName(filePath);
    }

    public async Task<bool> ConvertToGifAsync(string inputPath, string outputPath, GifConversionOptions options)
    {
        LastError = null;

        string pathExe = Path.Combine(AppContext.BaseDirectory, "ffmpeg", "ffmpeg.exe");

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = pathExe;
        startInfo.Arguments = $"-y -i \"{inputPath}\" -r {options.Fps} -vf \"scale={options.Width}:-1\" \"{outputPath}\"";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardError = true;

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        string errorOutput = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();


        if (process.ExitCode != 0)
        {
            LastError = errorOutput;
            return false;
        }

        return true;
    }

}
