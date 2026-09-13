using VideoToGifConverter.Core.Services;

namespace VideoToGifConverter.Tests;

public class FakeFileSystem : IFileSystem
{
    public bool FileExistsResult { get; set; }
    
    public bool DirectoryExistsResult { get; set; } = true;

    public string? MissingFilePath { get; set; }

    public bool DeleteFileCalled { get; private set; }

    public string? DeletedFilePath { get; private set; }

    public bool FileExists(string path)
    {
        if (path == MissingFilePath)
        {
            return false;
        }

        return FileExistsResult;
    }

    public bool DirectoryExists(string path)
    {
        return DirectoryExistsResult;
    }

    public void DeleteFile(string path)
    {
        DeleteFileCalled = true;
        DeletedFilePath = path;
    }
}