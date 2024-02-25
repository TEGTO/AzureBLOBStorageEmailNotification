using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using AzureEmailBLOBTrigger.Options;
using AzureEmailBLOBTrigger.Sevices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace AzureEmailBLOBTrigger
{
    public class EmailSendFunction
    {
        private readonly ILogger<EmailSendFunction> logger;
        private readonly IEmailNotificator emailNotificator;
        private readonly EmailSendOptions configuration;

        public EmailSendFunction(ILogger<EmailSendFunction> logger, IEmailNotificator emailNotificator, IOptions<EmailSendOptions> configuration)
        {
            this.logger = logger;
            this.emailNotificator = emailNotificator;
            this.configuration = configuration.Value;
        }
        [Function(nameof(EmailSendFunction))]
        public async Task Run([BlobTrigger("%importcontainer%/{name}")] BlobClient blobClient, string name)
        {
            try
            {
                BlobProperties properties = await blobClient.GetPropertiesAsync();
                IDictionary<string, string> metadata = properties.Metadata;
                if (CheckMetaDataConditions(metadata))
                {
                    logger.LogInformation($"Blob trigger function processed blob: {name}");
                    var blobUriWithSas = await CreateServiceSASBlob(blobClient);
                    string emailBody = $"<h1>Congratulations!</h1>\r\n<h2>Your file '{GetOriginalFileName(name)}' has been successfully uploaded.</h2> \r\n<h2><a href=\"{blobUriWithSas}\">Link to file<a></h2>";
                    await emailNotificator.SendEmail(properties.Metadata["email"], "File Upload", emailBody);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error occured with file '{name}': {ex.Message}");
            }
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
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(configuration.EmailMessageExpirationHours);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
                Uri sasURI = blobClient.GenerateSasUri(sasBuilder);
                return sasURI;
            }
            else
                return null;
        }
        private bool CheckMetaDataConditions(IDictionary<string, string> metadata) =>
            metadata != null && metadata.ContainsKey("email");
        private string GetOriginalFileName(string fileName)
        {
            Match match = Regex.Match(fileName, configuration.FileNameRegex);
            return fileName.Substring(match.Length);
        }
    }
}
