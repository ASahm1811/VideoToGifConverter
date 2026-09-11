using System.Diagnostics;

namespace VideoToGifConverter.Core.Services;

public interface IProcessRunner
{
    void Start(ProcessStartInfo startInfo);

    void Kill();

    Task<string?> ReadStandardErrorLineAsync();

    Task<string> ReadStandardOutputAsync();

    Task WaitForExitAsync();

    int ExitCode { get; }
}
