using VideoToGifConverter.Core.Services;

namespace VideoToGifConverter.Tests;

public class FakeFileSystem : IFileSystem
{
    public bool FileExistsResult { get; set; }

    public bool FileExists(string path)
    {
        return FileExistsResult;
    }
}