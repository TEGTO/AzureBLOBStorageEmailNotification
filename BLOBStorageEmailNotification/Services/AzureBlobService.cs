using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components.Forms;

namespace ReenbitTestTask.Services
{
    public class AzureBlobService : IFileCloudManagerService
    {
        private const string STORAGE_ACCOUNT = ""; //Not very secure, but okay 
        private const string STORAGE_ACCESS_KEY = ""; //Not very secure, but okay 
        private const string CONTAINER_NAME = "files";
        const int MAX_FILE_SIZE = 1024 * 1024 * 10;

        private readonly BlobServiceClient blobServiceClient;

        public AzureBlobService()
        {
            var credential = new StorageSharedKeyCredential(STORAGE_ACCOUNT, STORAGE_ACCESS_KEY);
            var blobUri = $"https://{STORAGE_ACCOUNT}.blob.core.windows.net";
            blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        }
        public async Task UploadFileAsync(IBrowserFile file, string email)
        {
            string fileExtension = Path.GetExtension(file.ContentType);
            if (fileExtension == ".docx" || fileExtension == ".document")
            {
                string fileName = $"{DateTime.Now.DayOfYear}{DateTime.Now.TimeOfDay}-&${file.Name}";
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(CONTAINER_NAME);
                var blobClient = blobContainerClient.GetBlobClient(fileName);
                using (var stream = file.OpenReadStream(MAX_FILE_SIZE))
                {
                    var response = await blobClient.UploadAsync(stream, true);
                    if (response == null || response.GetRawResponse().Status != 201)
                        throw new InvalidOperationException("Blob storage upload error!");
                }
                await blobClient.SetMetadataAsync(new Dictionary<string, string> { ["email"] = email });
            }
            else
                throw new InvalidOperationException("Invalid file extension!");
        }
    }
}
