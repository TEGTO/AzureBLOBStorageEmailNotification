namespace AzureEmailBLOBTrigger.Options
{
    public class EmailServiceConnectionOptions
    {
        public const string OptionPosition = "EmailServiceConnectionOptions";

        public string EmailConnectionString { get; set; }
        public string EmailFromAddress { get; set; }
    }
}
