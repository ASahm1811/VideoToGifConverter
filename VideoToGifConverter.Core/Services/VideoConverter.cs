using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace VideoToGifConverter.Core.Services;

public class VideoConverter
{

    public string GetVersion()
    {
        return "VideoToGifConverter v0.1";
    }

    public string GetFileName(string filePath)
    {
        return Path.GetFileName(filePath);
    }

}
