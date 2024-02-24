using Microsoft.AspNetCore.Components.Forms;

namespace ReenbitTestTask.Services
{
    public interface IFileCloudManagerService
    {
        public Task UploadFileAsync(IBrowserFile file, string email);
    }
}
