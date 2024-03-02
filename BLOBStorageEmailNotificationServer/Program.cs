using Azure.Storage;
using Azure.Storage.Blobs;
using BLOBStorageEmailNotificationServer.Components;
using BLOBStorageEmailNotificationServer.Options;
using BLOBStorageEmailNotificationServer.Services;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;
services.AddRazorComponents()
    .AddInteractiveServerComponents();

var azureBlobServiceConfig = configuration.GetSection(ServerOptions.OptionPosition);
services.Configure<ServerOptions>(azureBlobServiceConfig);

var storageConfig = configuration.GetSection(StorageConnectionOptions.OptionPosition);
services.Configure<StorageConnectionOptions>(storageConfig);

var storageOptions = storageConfig.Get<StorageConnectionOptions>();

var credential = new StorageSharedKeyCredential(storageOptions.StorageAccount, storageOptions.StorageAccessKey);
var blobUri = $"https://{storageOptions.StorageAccount}.blob.core.windows.net";
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