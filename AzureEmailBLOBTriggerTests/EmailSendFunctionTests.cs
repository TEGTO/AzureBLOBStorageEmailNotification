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
        private Mock<ILogger<EmailSendFunction>> loggerMock;
        private Mock<IEmailNotificator> emailNotificatorMock;
        private Mock<IOptions<EmailSendOptions>> configurationMock;
        private Mock<ICreateFileUri> createFileUriMock;
        private Dictionary<string, string> metadata;
        private Mock<BlobClient> blobClientMock;
        private string testFileName;
        private EmailSendFunction function;

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILogger<EmailSendFunction>>();
            emailNotificatorMock = new Mock<IEmailNotificator>();
            configurationMock = new Mock<IOptions<EmailSendOptions>>();
            metadata = new Dictionary<string, string> { { "email", "test@example.com" } };
            blobClientMock = new Mock<BlobClient>();
            testFileName = "733736553832507-@testfile.pdf";
            createFileUriMock = new Mock<ICreateFileUri>();

            createFileUriMock.Setup(x => x.CreateFileUri(It.IsAny<BlobClient>(), It.IsAny<EmailSendOptions>())).ReturnsAsync(new Uri("https://example.com/sasUri"));

            configurationMock.Setup(x => x.Value).Returns(new EmailSendOptions
            {
                FileNameRegex = "\\d{15}-@",
                EmailMessageExpirationHours = 24
            });
            function = new EmailSendFunction(loggerMock.Object, emailNotificatorMock.Object, configurationMock.Object, createFileUriMock.Object);
        }
        [Test]
        public async Task Run_ValidBlob_Triggered()
        {
            // Arrange
            string actualEmailBody = null;
            emailNotificatorMock
             .Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
             .Callback<string, string, string>((email, subject, body) => actualEmailBody = body);
            // Act
            await function.Run(blobClientMock.Object, testFileName, metadata);
            // Assert
            string expectedEmailBody = $"<h1>Congratulations!</h1>\r\n<h2>Your file 'testfile.pdf' has been successfully uploaded.</h2>";
            emailNotificatorMock.Verify(x => x.SendEmail("test@example.com", "File Upload", It.IsAny<string>()), Times.Once);
            Assert.True(actualEmailBody.Contains(expectedEmailBody));
        }
        [Test]
        public async Task Run_MetadataIsBull_NotTriggered()
        {
            // Arrange
            metadata = null;
            // Act
            await function.Run(blobClientMock.Object, testFileName, metadata);
            // Assert
            emailNotificatorMock.Verify(x => x.SendEmail("test@example.com", "File Upload", It.IsAny<string>()), Times.Never);
        }
        [Test]
        public async Task Run_NoMetaData_LoggerTriggered()
        {
            // Arrange
            emailNotificatorMock
            .Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InvalidOperationException("Send Email Exception"));
            string expectedErrorMessage = $"Error occured with file '{testFileName}': Send Email Exception";
            // Act
            await function.Run(blobClientMock.Object, testFileName, metadata);
            // Assert
            loggerMock.Verify(
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
            emailNotificatorMock
             .Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
             .Callback<string, string, string>((email, subject, body) => actualEmailBody = body);
            // Act
            await function.Run(blobClientMock.Object, testFileName, metadata);
            // Assert
            string expectedEmailBody = $"<h1>Congratulations!</h1>\r\n<h2>Your file 'testfile.pdf' has been successfully uploaded.</h2>";
            emailNotificatorMock.Verify(x => x.SendEmail("test@example.com", "File Upload", It.IsAny<string>()), Times.Once);
            Assert.True(actualEmailBody.Contains(expectedEmailBody));
        }
        [Test]
        public async Task Run_InValidRegex_SaveCurrentFileName()
        {
            // Arrange
            testFileName = "InavalidRegex123-@testfile.pdf";
            string actualEmailBody = null;
            emailNotificatorMock
             .Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
             .Callback<string, string, string>((email, subject, body) => actualEmailBody = body);
            // Act
            await function.Run(blobClientMock.Object, testFileName, metadata);
            // Assert
            string expectedEmailBody = $"<h1>Congratulations!</h1>\r\n<h2>Your file 'InavalidRegex123-@testfile.pdf' has been successfully uploaded.</h2>";
            emailNotificatorMock.Verify(x => x.SendEmail("test@example.com", "File Upload", It.IsAny<string>()), Times.Once);
            Assert.True(actualEmailBody.Contains(expectedEmailBody));
        }
        [Test]
        public async Task Run_CanGenerateURI_GeneratedURI()
        {
            // Act
            await function.Run(blobClientMock.Object, testFileName, metadata);
            // Assert
            createFileUriMock.Verify(x => x.CreateFileUri(blobClientMock.Object, It.IsAny<EmailSendOptions>()), Times.Once);
        }
        [Test]
        public async Task Run_CanNOTGenerateURI_GeneratedURI()
        {
            //Arrange
            createFileUriMock.Setup(x => x.CreateFileUri(It.IsAny<BlobClient>(), It.IsAny<EmailSendOptions>())).Throws(new InvalidOperationException("Error during uri generation!"));
            string expectedErrorMessage = $"Error occured with file '{testFileName}': Error during uri generation!";
            // Act
            await function.Run(blobClientMock.Object, testFileName, metadata);
            // Assert
            createFileUriMock.Verify(x => x.CreateFileUri(blobClientMock.Object, It.IsAny<EmailSendOptions>()), Times.Once);
            loggerMock.Verify(
              x => x.Log(
                 It.Is<LogLevel>(l => l == LogLevel.Error),
                 It.IsAny<EventId>(),
                 It.Is<It.IsAnyType>((v, t) => v.ToString() == expectedErrorMessage),
                 It.IsAny<Exception>(),
                 It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }
    }
}
