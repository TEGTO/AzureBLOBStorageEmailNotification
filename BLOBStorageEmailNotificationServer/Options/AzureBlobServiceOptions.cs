namespace BLOBStorageEmailNotificationServer.Options
{
    public class AzureBlobServiceOptions
    {
        public const string OptionPosition = "AzureBlobServiceOptions";

        public string StorageAccount { get; set; }
        public string StorageAccessKey { get; set; }
        public string ContainerName { get; set; }
        public int MaxFileSize { get; set; }
        public string FileNameRegex { get; set; }
        public string[] AllowedFileFormats { get; set; }
    }
}
