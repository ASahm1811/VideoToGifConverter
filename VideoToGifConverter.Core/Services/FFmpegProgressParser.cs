using System.Globalization;
using System.Text.RegularExpressions;

namespace VideoToGifConverter.Core.Services;

public static class FFmpegProgressParser
{
    public static double? ParseOutTimeUs(string line)
    {
        const string prefix = "out_time_us=";

        if (!line.StartsWith(prefix, StringComparison.Ordinal))
        {
            return null;
        }

        string value = line[prefix.Length..];

        if (!long.TryParse(
                value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out long microseconds))
        {
            return null;
        }

        return microseconds / 1_000_000.0;
    }
}