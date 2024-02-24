using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components.Forms;

namespace ReenbitTestTask.Services
{
    public class AzureBlobService : IFileCloudManagerService
    {
        private const string STORAGE_ACCOUNT = ""; //Not very secure, but okay 
        private const string STORAGE_ACCESS_KEY = ""; //Not very secure, but okay 

        private readonly BlobServiceClient blobServiceClient;

        public AzureBlobService()
        {
            var credential = new StorageSharedKeyCredential(STORAGE_ACCOUNT, STORAGE_ACCESS_KEY);
            var blobUri = $"https://{STORAGE_ACCOUNT}.blob.core.windows.net";
            blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        }
        public async Task<bool> UploadFileAsync(IBrowserFile file)
        {
            string fileName = $"{file.Name}-{Guid.NewGuid()}";
            var blobContainerClient = blobServiceClient.GetBlobContainerClient("test");
            var blobClient = blobContainerClient.GetBlobClient(fileName);
            using (var stream = file.OpenReadStream())
            {
                var response = await blobClient.UploadAsync(stream, true);
                if (response != null && response.GetRawResponse().Status == 201)
                    return true;
                else
                    return false;
            }
        }
    }
}
