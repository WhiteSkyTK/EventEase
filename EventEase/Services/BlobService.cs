using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace EventEase.Services
{
    public interface IBlobService
    {
        Task<string> UploadImageAsync(IFormFile file, string containerName);
    }

    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(IConfiguration configuration)
        {
            // Connects using the string from appsettings.json
            _blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzureStorage"));
        }

        public async Task<string> UploadImageAsync(IFormFile file, string containerName)
        {
            if (file == null || file.Length == 0) return null;

            // 1. Create container if it doesn't exist
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            // 2. Generate a unique name for the image
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = containerClient.GetBlobClient(fileName);

            // 3. Upload the file
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            // 4. Return the URL so we can save it to the database
            return blobClient.Uri.ToString();
        }
    }
}