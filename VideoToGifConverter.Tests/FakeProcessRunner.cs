using System.Diagnostics;
using VideoToGifConverter.Core.Services;

namespace VideoToGifConverter.Tests;

public class FakeProcessRunner : IProcessRunner
{
    public int ExitCode { get; set; }

    public string ErrorOutput { get; set; } = string.Empty;

    public void Start(ProcessStartInfo startInfo)
    {
    }

    public Task<string> ReadStandardErrorAsync()
    {
        return Task.FromResult(ErrorOutput);
    }

    public Task WaitForExitAsync()
    {
        return Task.CompletedTask;
    }
}