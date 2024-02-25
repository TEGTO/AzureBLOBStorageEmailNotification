using Azure.Storage.Blobs;
using BLOBStorageEmailNotificationServer.Options;
using Fare;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace BLOBStorageEmailNotificationServer.Services
{
    public class AzureBlobService : IFileCloudManagerService
    {
        private readonly AzureBlobServiceOptions configuration;
        private readonly BlobServiceClient blobServiceClient;

        public AzureBlobService(BlobServiceClient blobServiceClient, IOptions<AzureBlobServiceOptions> configuration)
        {
            this.blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
            this.configuration = configuration.Value;
        }
        public async Task UploadFileAsync(IBrowserFile file, string email)
        {
            if (FileValidation(file))
            {
                string fileName = GenerateNewFileName(file);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(configuration.ContainerName);
                var blobClient = blobContainerClient.GetBlobClient(fileName);
                using (var stream = file.OpenReadStream(configuration.MaxFileSize))
                {
                    var response = await blobClient.UploadAsync(stream, true);
                    if (response == null || response.GetRawResponse().Status != 201)
                        throw new InvalidOperationException("Blob storage upload error!");
                }
                await blobClient.SetMetadataAsync(new Dictionary<string, string> { ["email"] = email });
            }
            else
                throw new NotSupportedException("Invalid file extension!");
        }
        private bool FileValidation(IBrowserFile file)
        {
            string fileExtension = Path.GetExtension(file.ContentType);
            foreach (var fileFormat in configuration.AllowedFileFormats)
            {
                if (fileExtension == fileFormat)
                    return true;
            }
            return false;
        }
        private string GenerateNewFileName(IBrowserFile file)
        {
            var xeger = new Xeger(configuration.FileNameRegex, new Random());
            return xeger.Generate() + file.Name;
        }
    }
}
