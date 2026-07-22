using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace VideoToGifConverter.Core.Services;

public class VideoConverter
{
    public string GetFileName(string filePath)
    {
        return Path.GetFileName(filePath);
    }

    public async Task<bool> ConvertToGifAsync(string inputPath, string outputPath)
    {
        string pathExe = Path.Combine(AppContext.BaseDirectory, "ffmpeg", "ffmpeg.exe");

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = pathExe;
        startInfo.Arguments = $"-y -i \"{inputPath}\" \"{outputPath}\"";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();
        await process.WaitForExitAsync();


        return process.ExitCode == 0;
    }

}
