using VideoToGifConverter.Core.Services;

namespace VideoToGifConverter.Tests;

public class FakeFileSystem : IFileSystem
{
    public bool FileExistsResult { get; set; }

    public bool DeleteFileCalled { get; private set; }

    public string? DeletedFilePath { get; private set; }

    public bool FileExists(string path)
    {
        return FileExistsResult;
    }

    public void DeleteFile(string path)
    {
        DeleteFileCalled = true;
        DeletedFilePath = path;
    }
}