using TheBugTrucker.Services.Interfaces;

namespace TheBugTrucker.Services
{
    public class FileService : IFileService
    {
        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                memoryStream.Close();
                await memoryStream.DisposeAsync();

                return byteFile;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData);
                return string.Format($"data:{extension};base64,{imageBase64Data}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
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
