namespace AzureEmailBLOBTrigger.Options
{
    public class TriggerOptions
    {
        public const string OptionPosition = "TriggerOptions";

        public string FileNameRegex { get; set; }
        public double MessageExpirationHours { get; set; }
    }
}
