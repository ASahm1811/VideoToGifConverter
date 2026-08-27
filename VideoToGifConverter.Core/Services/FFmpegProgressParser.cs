using System.Globalization;
using System.Text.RegularExpressions;

namespace VideoToGifConverter.Core.Services;

public static class FFmpegProgressParser
{
    public static TimeSpan? ParseTime(string line)
    {
        Match match = Regex.Match(
            line,
            @"time=(\d{2}):(\d{2}):(\d{2})\.(\d{2})");

        if (!match.Success)
        {
            return null;
        }

        bool success = TimeSpan.TryParseExact(
            match.Groups[1].Value + ":" +
            match.Groups[2].Value + ":" +
            match.Groups[3].Value + "." +
            match.Groups[4].Value,
            @"hh\:mm\:ss\.ff",
            CultureInfo.InvariantCulture,
            out TimeSpan time);

        return success ? time : null;
    }
}