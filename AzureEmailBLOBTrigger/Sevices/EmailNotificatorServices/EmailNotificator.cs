using Azure;
using Azure.Communication.Email;
using AzureEmailBLOBTrigger.Options;
using Microsoft.Extensions.Options;

namespace AzureEmailBLOBTrigger.Sevices.EmailNotificatorServices
{
    public class EmailNotificator : IEmailNotificator
    {
        private readonly EmailClient emailClient;
        private readonly EmailSendOptions configuration;

        public EmailNotificator(EmailClient emailClient, IOptions<EmailSendOptions> configuration)
        {
            this.configuration = configuration.Value;
            this.emailClient = emailClient;
        }
        public async Task SendEmail(string email, string title, string emailBody)
        {
            await emailClient.SendAsync(
                WaitUntil.Completed,
                senderAddress: configuration.EmailFromAddress,
                recipientAddress: email,
                subject: title,
                htmlContent: emailBody
                );
        }
    }
}
