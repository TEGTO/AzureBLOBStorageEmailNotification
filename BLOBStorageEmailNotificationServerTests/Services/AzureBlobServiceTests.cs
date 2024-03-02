using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BLOBStorageEmailNotificationServer.Options;
using BLOBStorageEmailNotificationServer.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Moq;

namespace BLOBStorageEmailNotificationServer.Tests.Services
{
    [TestFixture]
    public class AzureBlobServiceTests
    {
        private Mock<BlobServiceClient> mockBlobServiceClient;
        private Mock<Response> mockRawResponse;
        private Mock<IOptions<ServerOptions>> mockOptions;
        private Mock<IBrowserFile> mockFile;

        [SetUp]
        public void Setup()
        {
            mockBlobServiceClient = new Mock<BlobServiceClient>();
            var mockBlobContainerClient = new Mock<BlobContainerClient>();
            var mockBlobClient = new Mock<BlobClient>();
            var response = new Mock<Response<BlobContentInfo>>();
            mockRawResponse = new Mock<Response>();

            mockBlobServiceClient.Setup(x => x.GetBlobContainerClient(It.IsAny<string>())).Returns(
               mockBlobContainerClient.Object);

            mockBlobContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(() => mockBlobClient.Object);

            mockBlobClient.Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);

            response.Setup(x => x.GetRawResponse()).Returns(mockRawResponse.Object);

            mockRawResponse.Setup(x => x.Status).Returns(201);

            mockOptions = new Mock<IOptions<ServerOptions>>();
            mockOptions.Setup(x => x.Value)
                      .Returns(new ServerOptions
                      {
                          ContainerName = "files",
                          MaxFileSize = 1024 * 1024 * 10,
                          AllowedFileFormats = new string[2] { ".txt", ".pdf" },
                          FileNameRegex = "\\d{15}-@"
                      });

            mockFile = new Mock<IBrowserFile>();
            mockFile.Setup(x => x.ContentType).Returns("application.pdf");
            mockFile.Setup(x => x.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>())).Returns(new MemoryStream(new byte[1024]));

        }
        [Test]
        public async Task UploadFileAsync_ValidFile_UploadsFile()
        {
            //Arrange
            var azureBlobService = new AzureBlobService(mockBlobServiceClient.Object, mockOptions.Object);
            // Act
            AsyncTestDelegate act = async () => await azureBlobService.UploadFileAsync(mockFile.Object, "test@example.com");
            // Assert
            Assert.DoesNotThrowAsync(act);
        }
        [Test]
        public void UploadFileAsync_InvalidFileFormat_ThrowsNotSupportedException()
        {
            //Arrange
            mockFile.Setup(x => x.ContentType).Returns("application.docx");
            var azureBlobService = new AzureBlobService(mockBlobServiceClient.Object, mockOptions.Object);
            // Act
            AsyncTestDelegate act = async () => await azureBlobService.UploadFileAsync(mockFile.Object, "test@example.com");
            // Assert
            Assert.ThrowsAsync<NotSupportedException>(act);
        }
        [Test]
        public void UploadFileAsync_ResponseError_ThrowsInvalidOperationException()
        {
            //Arrange
            mockRawResponse.Setup(x => x.Status).Returns(400);
            var azureBlobService = new AzureBlobService(mockBlobServiceClient.Object, mockOptions.Object);
            // Act
            AsyncTestDelegate act = async () => await azureBlobService.UploadFileAsync(mockFile.Object, "test@example.com");
            // Assert
            Exception ex = Assert.ThrowsAsync<InvalidOperationException>(act);
            Assert.That(ex.Message, Is.EqualTo("Blob storage upload error!"));
        }
        [Test]
        public void UploadFileAsync_ExcessMaxSizeFileLimit_ThrowsInvalidOperationException()
        {
            //Arrange
            mockOptions.Setup(x => x.Value).Returns(new ServerOptions
            {
                ContainerName = "files",
                MaxFileSize = 1,
                AllowedFileFormats = new string[2] { ".txt", ".pdf" },
                FileNameRegex = "\\d{15}-@"
            });
            mockFile.Setup(x => x.OpenReadStream(1, It.IsAny<CancellationToken>())).Throws(new InvalidOperationException("ExcessedMaxSizeFileLimit"));
            var azureBlobService = new AzureBlobService(mockBlobServiceClient.Object, mockOptions.Object);
            // Act
            AsyncTestDelegate act = async () => await azureBlobService.UploadFileAsync(mockFile.Object, "test@example.com");
            // Assert
            Exception ex = Assert.ThrowsAsync<InvalidOperationException>(act);
            Assert.That(ex.Message, Is.EqualTo("ExcessedMaxSizeFileLimit"));
        }
        [Test]
        public void UploadFileAsync_InvalidBlobServiceClient_ThrowsNullReferenceException()
        {
            //Arrange
            mockBlobServiceClient = new Mock<BlobServiceClient>();
            var azureBlobService = new AzureBlobService(mockBlobServiceClient.Object, mockOptions.Object);
            // Act
            AsyncTestDelegate act = async () => await azureBlobService.UploadFileAsync(mockFile.Object, "test@example.com");
            // Assert
            Exception ex = Assert.ThrowsAsync<NullReferenceException>(act);
            Assert.That(ex.Message, Is.EqualTo("Object reference not set to an instance of an object."));
        }
        [Test]
        public void UploadFileAsync_InvalidAzureBlobServiceOptions_ThrowsNullReferenceException()
        {
            //Arrange
            mockOptions = new Mock<IOptions<ServerOptions>>();
            var azureBlobService = new AzureBlobService(mockBlobServiceClient.Object, mockOptions.Object);
            // Act
            AsyncTestDelegate act = async () => await azureBlobService.UploadFileAsync(mockFile.Object, "test@example.com");
            // Assert
            Exception ex = Assert.ThrowsAsync<NullReferenceException>(act);
            Assert.That(ex.Message, Is.EqualTo("Object reference not set to an instance of an object."));
        }
    }
}