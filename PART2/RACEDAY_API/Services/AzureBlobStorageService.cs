using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RACEDAY_API.Services
{
    public class AzureBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString =
                configuration["AzureBlobStorage:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Azure Blob Storage connection string is missing.");
            }

            _blobServiceClient =
                new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadImageAsync(
            IFormFile file,
            string containerName)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Please select an image.");
            }

            var allowedTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            if (!allowedTypes.Contains(file.ContentType))
            {
                throw new ArgumentException(
                    "Only JPEG, PNG and WebP images are allowed.");
            }

            var containerClient =
                _blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync(
                PublicAccessType.None);

            var extension = Path.GetExtension(file.FileName);
            var blobName = $"{Guid.NewGuid()}{extension}";

            var blobClient =
                containerClient.GetBlobClient(blobName);

            using var stream = file.OpenReadStream();

            await blobClient.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = file.ContentType
                    }
                });

            return blobClient.Uri.ToString();
        }

        public string GetImageReadUrl(
    string imageUrl,
    string containerName)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                throw new ArgumentException("Image URL is required.");
            }

            var uri = new Uri(imageUrl);

            var blobName = Uri.UnescapeDataString(
                Path.GetFileName(uri.AbsolutePath));

            var containerClient =
                _blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(blobName);

            if (!blobClient.CanGenerateSasUri)
            {
                throw new InvalidOperationException(
                    "Unable to generate a secure image access link.");
            }

            var sasUri = blobClient.GenerateSasUri(
                Azure.Storage.Sas.BlobSasPermissions.Read,
                DateTimeOffset.UtcNow.AddMinutes(30));

            return sasUri.ToString();
        }


    }
}