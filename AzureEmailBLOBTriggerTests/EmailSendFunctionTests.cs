using Azure.Storage.Blobs;
using AzureEmailBLOBTrigger.Options;
using AzureEmailBLOBTrigger.Sevices.CreateFileUriServices;
using AzureEmailBLOBTrigger.Sevices.EmailNotificatorServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace AzureEmailBLOBTrigger.Tests
{
    [TestFixture]
    public class EmailSendFunctionTests
    {
        private Mock<ILogger<EmailSendFunction>> mockLogger;
        private Mock<IEmailNotificator> mockEmailNotificator;
        private Mock<IOptions<EmailSendOptions>> mockConfiguration;
        private Mock<ICreateFileUri> mockCreateFileUri;
        private Mock<BlobClient> mockBlobClient;
        private Dictionary<string, string> metadata;
        private string testFileName;
        private EmailSendFunction emailSendFunction;

        [SetUp]
        public void Setup()
        {
            mockLogger = new Mock<ILogger<EmailSendFunction>>();
            mockEmailNotificator = new Mock<IEmailNotificator>();
            mockConfiguration = new Mock<IOptions<EmailSendOptions>>();
            metadata = new Dictionary<string, string> { { "email", "test@example.com" } };
            mockBlobClient = new Mock<BlobClient>();
            testFileName = "733736553832507-@testfile.pdf";
            mockCreateFileUri = new Mock<ICreateFileUri>();

            mockCreateFileUri.Setup(x => x.CreateFileUri(It.IsAny<BlobClient>(), It.IsAny<EmailSendOptions>())).ReturnsAsync(new Uri("https://example.com/sasUri"));

            mockConfiguration.Setup(x => x.Value).Returns(new EmailSendOptions
            {
                FileNameRegex = "\\d{15}-@",
                EmailMessageExpirationHours = 24
            });
            emailSendFunction = new EmailSendFunction(mockLogger.Object, mockEmailNotificator.Object, mockConfiguration.Object, mockCreateFileUri.Object);
        }
        [Test]
        public async Task Run_ValidBlob_Triggered()
        {
            // Arrange
            string actualEmailBody = null;
            mockEmailNotificator
             .Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
             .Callback<string, string, string>((email, subject, body) => actualEmailBody = body);
            // Act
            await emailSendFunction.Run(mockBlobClient.Object, testFileName, metadata);
            // Assert
            string expectedEmailBody = $"<h1>Congratulations!</h1>\r\n<h2>Your file 'testfile.pdf' has been successfully uploaded.</h2>";
            mockEmailNotificator.Verify(x => x.SendEmail("test@example.com", "File Upload", It.IsAny<string>()), Times.Once);
            Assert.True(actualEmailBody.Contains(expectedEmailBody));
        }
        [Test]
        public async Task Run_MetadataIsBull_NotTriggered()
        {
            // Arrange
            metadata = null;
            // Act
            await emailSendFunction.Run(mockBlobClient.Object, testFileName, metadata);
            // Assert
            mockEmailNotificator.Verify(x => x.SendEmail("test@example.com", "File Upload", It.IsAny<string>()), Times.Never);
        }
        [Test]
        public async Task Run_NoMetaData_LoggerTriggered()
        {
            // Arrange
            mockEmailNotificator
            .Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InvalidOperationException("Send Email Exception"));
            string expectedErrorMessage = $"Error occured with file '{testFileName}': Send Email Exception";
            // Act
            await emailSendFunction.Run(mockBlobClient.Object, testFileName, metadata);
            // Assert
            mockLogger.Verify(
              x => x.Log(
                 It.Is<LogLevel>(l => l == LogLevel.Error),
                 It.IsAny<EventId>(),
                 It.Is<It.IsAnyType>((v, t) => v.ToString() == expectedErrorMessage),
                 It.IsAny<Exception>(),
                 It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }
        [Test]
        public async Task Run_ValidRegex_ValidFileName()
        {
            // Arrange
            testFileName = "733736553832507-@testfile.pdf";
            string actualEmailBody = null;
            mockEmailNotificator
             .Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
             .Callback<string, string, string>((email, subject, body) => actualEmailBody = body);
            // Act
            await emailSendFunction.Run(mockBlobClient.Object, testFileName, metadata);
            // Assert
            string expectedEmailBody = $"<h1>Congratulations!</h1>\r\n<h2>Your file 'testfile.pdf' has been successfully uploaded.</h2>";
            mockEmailNotificator.Verify(x => x.SendEmail("test@example.com", "File Upload", It.IsAny<string>()), Times.Once);
            Assert.True(actualEmailBody.Contains(expectedEmailBody));
        }
        [Test]
        public async Task Run_InValidRegex_NotChangedCurrentFileName()
        {
            // Arrange
            testFileName = "InavalidRegex123-@testfile.pdf";
            string actualEmailBody = null;
            mockEmailNotificator
             .Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
             .Callback<string, string, string>((email, subject, body) => actualEmailBody = body);
            // Act
            await emailSendFunction.Run(mockBlobClient.Object, testFileName, metadata);
            // Assert
            string expectedEmailBody = $"<h1>Congratulations!</h1>\r\n<h2>Your file 'InavalidRegex123-@testfile.pdf' has been successfully uploaded.</h2>";
            mockEmailNotificator.Verify(x => x.SendEmail("test@example.com", "File Upload", It.IsAny<string>()), Times.Once);
            Assert.True(actualEmailBody.Contains(expectedEmailBody));
        }
        [Test]
        public async Task Run_CanGenerateURI_GeneratedURI()
        {
            // Act
            await emailSendFunction.Run(mockBlobClient.Object, testFileName, metadata);
            // Assert
            mockCreateFileUri.Verify(x => x.CreateFileUri(mockBlobClient.Object, It.IsAny<EmailSendOptions>()), Times.Once);
        }
        [Test]
        public async Task Run_CanNOTGenerateURI_URIGenerateError()
        {
            //Arrange
            mockCreateFileUri.Setup(x => x.CreateFileUri(It.IsAny<BlobClient>(), It.IsAny<EmailSendOptions>())).Throws(new InvalidOperationException("Error during uri generation!"));
            string expectedErrorMessage = $"Error occured with file '{testFileName}': Error during uri generation!";
            // Act
            await emailSendFunction.Run(mockBlobClient.Object, testFileName, metadata);
            // Assert
            mockCreateFileUri.Verify(x => x.CreateFileUri(mockBlobClient.Object, It.IsAny<EmailSendOptions>()), Times.Once);
            mockLogger.Verify(
              x => x.Log(
                 It.Is<LogLevel>(l => l == LogLevel.Error),
                 It.IsAny<EventId>(),
                 It.Is<It.IsAnyType>((v, t) => v.ToString() == expectedErrorMessage),
                 It.IsAny<Exception>(),
                 It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }
    }
}
