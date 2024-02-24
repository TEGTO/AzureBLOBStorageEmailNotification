using Azure;
using Azure.Communication.Email;

namespace AzureEmailBLOBTrigger.Sevices
{
    public class EmailNotificator : IEmailNotificator
    {
        private const string CONNECTION_STRING = "";
        private const string EMAIL_FROM = "";

        private EmailClient emailClient;

        public EmailNotificator()
        {
            emailClient = new EmailClient(CONNECTION_STRING);
        }
        public async Task SendEmail(string email, string title, string emailBody)
        {
            await emailClient.SendAsync(
                WaitUntil.Completed,
                senderAddress: EMAIL_FROM,
                recipientAddress: email,
                subject: title,
                htmlContent: emailBody
                );
        }
    }
}
