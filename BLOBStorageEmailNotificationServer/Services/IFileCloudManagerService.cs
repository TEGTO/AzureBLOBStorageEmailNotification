using Microsoft.AspNetCore.Components.Forms;

namespace BLOBStorageEmailNotificationServer.Services
{
    public interface IFileCloudManagerService
    {
        public Task UploadFileAsync(IBrowserFile file, string email);
    }
}
