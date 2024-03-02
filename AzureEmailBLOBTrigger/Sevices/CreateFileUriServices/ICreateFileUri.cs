using Azure.Storage.Blobs;
using AzureEmailBLOBTrigger.Options;

namespace AzureEmailBLOBTrigger.Sevices.CreateFileUriServices
{
    public interface ICreateFileUri
    {
        public Task<Uri> CreateFileUri(BlobClient blobClient, TriggerOptions configuration);
    }
}
