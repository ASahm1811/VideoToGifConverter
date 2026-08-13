namespace VideoToGifConverter.Core.Services;

public interface IFileSystem
{
    bool FileExists(string path);
}