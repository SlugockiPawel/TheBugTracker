using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Services
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
            string fileImage = "default";

            if (!string.IsNullOrWhiteSpace(fileImage))
            {
                fileImage = Path.GetExtension(file).Replace(".", "");
                return $"/img/png/{fileImage}.png";
            }

            return fileImage;
        }

        public string FormatFileSize(long bytes)
        {
            int counter = 0;
            decimal fileSize = bytes;

            while (Math.Round(fileSize / 1024) > 1)
            {
                fileSize /= bytes;
                counter++;
            }

            return string.Format("{0:n1}{1}", fileSize, suffixes);
        }
    }
}