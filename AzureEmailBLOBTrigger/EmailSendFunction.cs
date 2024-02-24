using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using AzureEmailBLOBTrigger.Sevices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureEmailBLOBTrigger
{
    public class EmailSendFunction
    {
        private const string CONTAINER_NAME = "files";

        private readonly ILogger<EmailSendFunction> logger;
        private readonly IEmailNotificator emailNotificator;

        public EmailSendFunction(ILogger<EmailSendFunction> logger, IEmailNotificator emailNotificator)
        {
            this.logger = logger;
            this.emailNotificator = emailNotificator;
        }
        [Function(nameof(EmailSendFunction))]
        public async Task Run([BlobTrigger(CONTAINER_NAME + "/{name}")] BlobClient blobStream, string name)
        {
            logger.LogInformation($"Blob trigger function processed blob: {name}");
            var blobUriWithSas = await CreateServiceSASBlob(blobStream);
            string emailBody = $"Your file '{name}' has been successfully uploaded. " +
                               $"You can access it using the following link: {blobUriWithSas}";
            //logger.LogInformation(emailBody);
            emailNotificator.SendEmail("pshonovskij@gmail.com", "File Upload", emailBody);
        }
        private async Task<Uri> CreateServiceSASBlob(BlobClient blobClient)
        {
            if (blobClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
                Uri sasURI = blobClient.GenerateSasUri(sasBuilder);
                return sasURI;
            }
            else
                return null;
        }
    }
}
