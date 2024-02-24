using Microsoft.AspNetCore.Components.Forms;

namespace ReenbitTestTask.Services
{
    public interface IFileCloudManagerService
    {
        public Task<bool> UploadFileAsync(IBrowserFile file);
    }
}
