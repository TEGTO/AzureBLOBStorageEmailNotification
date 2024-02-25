using Azure.Storage.Blobs;
using AzureEmailBLOBTrigger.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureEmailBLOBTrigger.Sevices.CreateFileUriServices
{
    public interface ICreateFileUri
    {
        public Task<Uri> CreateFileUri(BlobClient blobClient, EmailSendOptions configuration);
    }
}
