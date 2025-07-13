namespace Zadachi.Lib.FileService
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string subDirectory = "");
        Task<byte[]> DownloadFileAsync(string fileName, string subDirectory = "");
        bool DeleteFile(string fileName, string subDirectory = "");
    }
}
