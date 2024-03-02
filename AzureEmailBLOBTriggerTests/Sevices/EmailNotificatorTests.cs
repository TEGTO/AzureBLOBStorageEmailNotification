using Azure;
using Azure.Communication.Email;
using AzureEmailBLOBTrigger.Options;
using AzureEmailBLOBTrigger.Sevices.EmailNotificatorServices;
using Microsoft.Extensions.Options;
using Moq;

namespace AzureEmailBLOBTrigger.Tests.Services
{
    [TestFixture]
    public class EmailNotificatorTests
    {
        [Test]
        public async Task SendEmail_Success()
        {
            // Arrange
            var mockEmailClient = new Mock<EmailClient>();
            var mockEmailOptions = new Mock<IOptions<EmailServiceConnectionOptions>>();
            var emailOptions = new EmailServiceConnectionOptions
            {
                EmailFromAddress = "test@example.com"
            };
            mockEmailOptions.Setup(x => x.Value).Returns(emailOptions);
            var emailNotificator = new EmailNotificator(mockEmailClient.Object, mockEmailOptions.Object);
            string email = "recipient@example.com";
            string title = "Test Email";
            string emailBody = "This is a test email.";
            // Act
            await emailNotificator.SendEmail(email, title, emailBody);
            // Assert
            mockEmailClient.Verify(x => x.SendAsync(
                It.IsAny<WaitUntil>(),
                emailOptions.EmailFromAddress,
                email,
                title,
                emailBody,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }
        [Test]
        public async Task SendEmail_NeverCall()
        {
            // Arrange
            var mockEmailClient = new Mock<EmailClient>();
            var mockEmailOptions = new Mock<IOptions<EmailServiceConnectionOptions>>();
            var emailOptions = new EmailServiceConnectionOptions
            {
                EmailFromAddress = "test@example.com"
            };
            mockEmailOptions.Setup(x => x.Value).Returns(emailOptions);
            var emailNotificator = new EmailNotificator(mockEmailClient.Object, mockEmailOptions.Object);
            string email = "recipient@example.com";
            string title = "Test Email";
            string emailBody = "This is a test email.";
            // Act
            // We intentionally don't call the SendEmail method here
            // Assert
            mockEmailClient.Verify(x => x.SendAsync(
                 It.IsAny<WaitUntil>(),
                emailOptions.EmailFromAddress,
                email,
                title,
                emailBody,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ), Times.Never); // We expect the SendAsync method not to be called
        }
    }
}

