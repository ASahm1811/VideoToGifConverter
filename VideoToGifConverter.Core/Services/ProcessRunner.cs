using System.Diagnostics;

namespace VideoToGifConverter.Core.Services;

public class ProcessRunner : IProcessRunner
{
    private Process? _process;

    public int ExitCode => _process?.ExitCode ?? -1;

    public void Start(ProcessStartInfo startInfo)
    {
        _process = new Process
        {
            StartInfo = startInfo
        };

        _process.Start();
    }

    public async Task<string?> ReadStandardErrorLineAsync()
    {
        if (_process == null)
        {
            throw new InvalidOperationException("Process has not been started.");
        }

        return await _process.StandardError.ReadLineAsync();
    }

    public async Task WaitForExitAsync()
    {
        if (_process == null)
        {
            throw new InvalidOperationException("Process has not been started.");
        }

        await _process.WaitForExitAsync();
    }
}