namespace VideoToGifConverter.Core.Services;

public interface IFileSystem
{
    bool FileExists(string path);

    bool DirectoryExists(string path);

    void DeleteFile(string path);
}