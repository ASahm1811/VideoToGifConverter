using System.Diagnostics;

namespace VideoToGifConverter.Core.Services;

public interface IProcessRunner
{
    void Start(ProcessStartInfo startInfo);

    Task<string> ReadStandardErrorAsync();

    Task WaitForExitAsync();

    int ExitCode { get; }
}
