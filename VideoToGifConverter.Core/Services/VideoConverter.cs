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

    public void ConvertToGif(string inputPath, string outputPath)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "notepad.exe";

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

    }

}
