using System.Diagnostics;
using VideoToGifConverter.Core.Services;

namespace VideoToGifConverter.Tests;

public class FakeProcessRunner : IProcessRunner
{
    public string StandardOutput { get; set; } = string.Empty;
    public int ExitCode { get; set; }

    public List<string> ErrorOutputLines { get; } = new List<string>();

    public ProcessStartInfo? StartInfo { get; private set; }

    private int _currentLineIndex;

    public void Start(ProcessStartInfo startInfo)
    {
        StartInfo = startInfo;
        _currentLineIndex = 0;
    }

    public Task<string?> ReadStandardErrorLineAsync()
    {
        if (_currentLineIndex >= ErrorOutputLines.Count)
        {
            return Task.FromResult<string?>(null);
        }

        string line = ErrorOutputLines[_currentLineIndex];
        _currentLineIndex++;

        return Task.FromResult<string?>(line);
    }

    public Task<string> ReadStandardOutputAsync()
    {
        return Task.FromResult(StandardOutput);
    }

    public Task WaitForExitAsync()
    {
        return Task.CompletedTask;
    }
}