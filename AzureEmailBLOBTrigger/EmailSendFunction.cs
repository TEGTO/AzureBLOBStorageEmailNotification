using Azure.Storage.Blobs;
using AzureEmailBLOBTrigger.Options;
using AzureEmailBLOBTrigger.Sevices.CreateFileUriServices;
using AzureEmailBLOBTrigger.Sevices.EmailNotificatorServices;
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
        private readonly ICreateFileUri createFileUri;

        public EmailSendFunction(ILogger<EmailSendFunction> logger, IEmailNotificator emailNotificator, IOptions<EmailSendOptions> configuration, ICreateFileUri createFileUri)
        {
            this.logger = logger;
            this.emailNotificator = emailNotificator;
            this.configuration = configuration.Value;
            this.createFileUri = createFileUri;
        }
        [Function(nameof(EmailSendFunction))]
        public async Task Run([BlobTrigger("%importcontainer%/{name}")] BlobClient blobClient, string name, IDictionary<string, string> metadata)
        {
            try
            {
                logger.LogInformation($"Blob trigger function processed blob: {name}");
                if (CheckMetaDataConditions(metadata))
                {
                    var blobUriWithSas = await createFileUri.CreateFileUri(blobClient, configuration);
                    string emailBody = $"<h1>Congratulations!</h1>\r\n<h2>Your file '{GetOriginalFileName(name)}' has been successfully uploaded.</h2> \r\n<h2><a href=\"{blobUriWithSas}\">Link to file<a></h2>";
                    await emailNotificator.SendEmail(metadata["email"], "File Upload", emailBody);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error occured with file '{name}': {ex.Message}");
            }
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
