using Zadachi.Migrations;

namespace Zadachi.Lib.FileService
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;

        public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string subDirectory = "")
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is empty");

                var uploadPath = Path.Combine(_environment.WebRootPath, subDirectory);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                fileName = $"{file.FileName.Replace(".txt", "")}_{DateTime.Now.ToString().Replace("/", "")}.txt";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation($"File uploaded: {filePath}");
                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                throw;
            }
        }

        public async Task<byte[]> DownloadFileAsync(string fileName, string subDirectory = "")
        {
            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, subDirectory, fileName);

                if (!File.Exists(filePath))
                    throw new FileNotFoundException("File not found");

                return await File.ReadAllBytesAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file");
                throw;
            }
        }

        public bool DeleteFile(string fileName, string subDirectory = "")
        {
            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, subDirectory, fileName);

                if (!File.Exists(filePath))
                    return false;

                File.Delete(filePath);
                _logger.LogInformation($"File deleted: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file");
                return false;
            }
        }
    }
}
