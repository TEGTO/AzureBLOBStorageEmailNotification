using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Components.Forms;

namespace ReenbitTestTask.Services
{
    public class AzureBlobService : IFileCloudManagerService
    {
        private const string STORAGE_ACCOUNT = ""; //Not very secure, but okay 
        private const string STORAGE_ACCESS_KEY = ""; //Not very secure, but okay 
        private const string CONNECTION_STRING = "";
        private const string CONTAINER_NAME = "files";

        private readonly BlobServiceClient blobServiceClient;
        private readonly QueueServiceClient queueServiceClient;

        public AzureBlobService()
        {
            var credential = new StorageSharedKeyCredential(STORAGE_ACCOUNT, STORAGE_ACCESS_KEY);
            var blobUri = $"https://{STORAGE_ACCOUNT}.blob.core.windows.net";
            blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            queueServiceClient = new QueueServiceClient(connectionString);
        }
        public async Task UploadFileAsync(IBrowserFile file, string email)
        {
            string fileExtension = Path.GetExtension(file.ContentType);
            if (fileExtension == ".docx" || fileExtension == ".document")
            {
                string fileName = $"{DateTime.Now.DayOfYear}{DateTime.Now.TimeOfDay}-{file.Name}";
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(CONTAINER_NAME);
                var blobClient = blobContainerClient.GetBlobClient(fileName);
                using (var stream = file.OpenReadStream())
                {
                    var response = await blobClient.UploadAsync(stream, true);
                    if (response == null || response.GetRawResponse().Status != 201)
                        throw new InvalidOperationException("Blob storage upload error!");
                }

                // Instantiate a QueueServiceClient which will be used to create and manipulate the queue
                var queueServiceClient = new QueueServiceClient(connectionString);

                // Get a reference to the queue
                string queueName = "emailqueue"; // Specify the name of your queue
                var queueClient = queueServiceClient.GetQueueClient(queueName);
            }
            else
                throw new InvalidOperationException("Invalid file extension!");
        }
    }
}
