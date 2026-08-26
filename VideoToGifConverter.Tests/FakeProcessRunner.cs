using System.Diagnostics;
using VideoToGifConverter.Core.Services;

namespace VideoToGifConverter.Tests;

public class FakeProcessRunner : IProcessRunner
{
    public int ExitCode { get; set; }

    public string ErrorOutput { get; set; } = string.Empty;

    public ProcessStartInfo? StartInfo { get; private set; }

    private bool _hasReadErrorOutput;

    public void Start(ProcessStartInfo startInfo)
    {
        StartInfo = startInfo;
        _hasReadErrorOutput = false;
    }

    public Task<string?> ReadStandardErrorLineAsync()
    {
        if (_hasReadErrorOutput)
        {
            return Task.FromResult<string?>(null);
        }

        _hasReadErrorOutput = true;

        return Task.FromResult<string?>(ErrorOutput);
    }

    public Task WaitForExitAsync()
    {
        return Task.CompletedTask;
    }
}