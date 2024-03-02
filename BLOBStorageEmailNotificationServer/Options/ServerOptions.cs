namespace BLOBStorageEmailNotificationServer.Options
{
    public class ServerOptions
    {
        public const string OptionPosition = "ServerOptions";

        public string ContainerName { get; set; }
        public int MaxFileSize { get; set; }
        public string FileNameRegex { get; set; }
        public string[] AllowedFileFormats { get; set; }
    }
}
