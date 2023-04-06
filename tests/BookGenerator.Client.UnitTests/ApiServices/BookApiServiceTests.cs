using BookGenerator.Application.Contracts.Books;
using BookGenerator.Domain.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookGenerator.Client.ApiServices.Tests
{
    [TestClass()]
    public class BookApiServiceTests
    {
        [TestMethod()]
        public void Constructor_ThrowsArgumentNullException_WhenHttpClientFactoryIsNull()
        {
            // Arrange & Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => new BookApiService(null, Mock.Of<IOptions<JsonOptions>>()));
            Assert.ThrowsException<ArgumentNullException>(() => new BookApiService(Mock.Of<IHttpClientFactory>(), null));
        }

        [TestMethod()]
        public void Constructor_DoesNotThrowException_WhenHttpClientFactoryIsNotNull()
        {
            // Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var mockJsonOptions = CreateJsonOptions();

            // Act & Assert
            var service = new BookApiService(httpClientFactoryMock.Object, mockJsonOptions);
            Assert.IsNotNull(service);
        }

        [TestMethod()]
        public async Task CreateAsyncTest()
        {
            // Arrange
            CreateBookResponse expected = new CreateBookResponse(Guid.NewGuid());
            var mockJsonOptions = CreateJsonOptions();
            IHttpClientFactory mockHttpClientFactory = CreateHttpClientFactory(
                JsonSerializer.Serialize(expected, mockJsonOptions.Value.JsonSerializerOptions));
            var service = new BookApiService(mockHttpClientFactory, mockJsonOptions);

            // Act
            CreateBookResponse actual = await service.CreateAsync("Test");

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.BookId, actual.BookId);
        }

        [TestMethod()]
        public async Task GetStatusAsyncTest()
        {
            // Arrange
            var expected = new GetStatusResponse("dummy title", BookStatus.Pending, 5, null);
            var mockJsonOptions = CreateJsonOptions();
            var expectedJson = JsonSerializer.Serialize(expected, mockJsonOptions.Value.JsonSerializerOptions);
            IHttpClientFactory mockHttpClientFactory = CreateHttpClientFactory(expectedJson);
            var service = new BookApiService(mockHttpClientFactory, mockJsonOptions);

            // Act
            GetStatusResponse actual = await service.GetStatusAsync(Guid.NewGuid());

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.BookTitle, actual.BookTitle);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.Progress, actual.Progress);
        }

        [TestMethod()]
        public async Task GetResultAsyncTest()
        {
            // Arrange
            var expected = new BookFile() 
            { 
                Content = Encoding.UTF8.GetBytes("dummy content"), 
                Name = "dummy name", 
                ContentType = "dummy content type" 
            };
            var mockJsonOptions = CreateJsonOptions();
            var expectedJson = JsonSerializer.Serialize(expected, mockJsonOptions.Value.JsonSerializerOptions);
            IHttpClientFactory mockHttpClientFactory = CreateHttpClientFactory(expectedJson);
            var service = new BookApiService(mockHttpClientFactory, mockJsonOptions);
            // Act
            BookFile actual = await service.GetResultAsync(Guid.NewGuid());
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Content);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(Encoding.UTF8.GetString(expected.Content), Encoding.UTF8.GetString(actual.Content));
        }

        private static IHttpClientFactory CreateHttpClientFactory(string expectedJsonString)
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedJsonString)
            };
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expectedResponse);
            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("http://dummyurl");
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(factory => factory.CreateClient("BookApiClient"))
                .Returns(httpClient);
            return httpClientFactoryMock.Object;
        }

        private static IOptions<JsonOptions> CreateJsonOptions()
        {
            JsonOptions optionsMock = new();
            optionsMock.JsonSerializerOptions.PropertyNamingPolicy = null;
            optionsMock.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

            var options = new Mock<IOptions<JsonOptions>>();
            options
                .Setup(o => o.Value)
                .Returns(optionsMock);
            return options.Object;
        }
    }
}