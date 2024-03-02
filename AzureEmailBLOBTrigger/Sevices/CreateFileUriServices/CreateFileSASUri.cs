using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using AzureEmailBLOBTrigger.Options;

namespace AzureEmailBLOBTrigger.Sevices.CreateFileUriServices
{
    public class CreateFileSASUri : ICreateFileUri
    {
        public async Task<Uri> CreateFileUri(BlobClient blobClient, TriggerOptions configuration)
        {
            if (blobClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(configuration.MessageExpirationHours);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
                Uri sasURI = blobClient.GenerateSasUri(sasBuilder);
                return sasURI;
            }
            else
                throw new InvalidOperationException("Can't generate SAS uri!");
        }
    }
}
