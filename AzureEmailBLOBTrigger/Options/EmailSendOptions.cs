namespace AzureEmailBLOBTrigger.Options
{
    public class EmailSendOptions
    {
        public const string OptionPosition = "EmailSendOptions";

        public string FileNameRegex { get; set; }
        public string EmailConnectionString { get; set; }
        public string EmailFromAddress { get; set; }
        public double EmailMessageExpirationHours { get; set; }
    }
}
