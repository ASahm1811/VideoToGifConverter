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

    public bool ConvertToGif(string inputPath, string outputPath)
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
        process.WaitForExit();


        return process.ExitCode == 0;
    }

}
