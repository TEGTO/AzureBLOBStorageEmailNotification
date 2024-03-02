namespace BLOBStorageEmailNotificationServer.Options
{
    internal class StorageConnectionOptions
    {
        public const string OptionPosition = "StorageConnectionOptions";

        public string StorageAccount { get; set; }
        public string StorageAccessKey { get; set; }
    }
}
