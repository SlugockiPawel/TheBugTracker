using TheBugTrucker.Services.Interfaces;

namespace TheBugTrucker.Services
{
    public class FileService : IFileService
    {
        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        public Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            throw new NotImplementedException();
        }

        public string GetFileIcon(string file)
        {
            throw new NotImplementedException();
        }

        public string FormatFileSize(long bytes)
        {
            throw new NotImplementedException();
        }
    }
}
