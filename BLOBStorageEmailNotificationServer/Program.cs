using Azure.Storage.Blobs;
using Azure.Storage;
using BLOBStorageEmailNotificationServer.Components;
using BLOBStorageEmailNotificationServer.Options;
using BLOBStorageEmailNotificationServer.Services;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddRazorComponents()
    .AddInteractiveServerComponents();

var azureBlobServiceConfig = builder.Configuration.GetSection(AzureBlobServiceOptions.OptionPosition);
services.Configure<AzureBlobServiceOptions>(azureBlobServiceConfig);

var azureBlobServiceOptions = azureBlobServiceConfig.Get<AzureBlobServiceOptions>();
var credential = new StorageSharedKeyCredential(azureBlobServiceOptions.StorageAccount, azureBlobServiceOptions.StorageAccessKey);
var blobUri = $"https://{azureBlobServiceOptions.StorageAccount}.blob.core.windows.net";
var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);

services.AddSingleton(blobServiceClient);
services.AddSingleton<IFileCloudManagerService, AzureBlobService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();