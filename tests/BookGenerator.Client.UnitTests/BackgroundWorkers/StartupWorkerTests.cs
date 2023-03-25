using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookGenerator.Client.BackgroundWorkers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookGenerator.Client.ApiServices;
using Moq;
using BookGenerator.Application.Contracts.Books;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.Primitives.Result;

namespace BookGenerator.Client.BackgroundWorkers.Tests
{
    [TestClass()]
    public class StartupWorkerTests
    {
        [TestMethod()]
        public void Constructor_ThrowsArgumentNullException_WhenBookApiIsNull()
        {
            // Arrange & Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => new StartupWorker(null));
        }

        [TestMethod()]
        public async Task StartAsync_CallsGetStatusAsync_WithEmptyGuid()
        {
            // Arrange
            var getStatusAsyncCalled = new TaskCompletionSource<bool>();
            var bookApiMock = new Mock<IBookApiService>();
            bookApiMock.Setup(api => api.GetStatusAsync(Guid.Empty))
                .Callback(() => getStatusAsyncCalled.SetResult(true))
                .ReturnsAsync(new GetStatusResponse("dummy title", BookStatus.Pending, 5));

            var startupWorker = new StartupWorker(bookApiMock.Object);

            // Act
            await startupWorker.StartAsync(CancellationToken.None);

            // Wait for the GetStatusAsync method to be called in the background
            await getStatusAsyncCalled.Task;

            // Assert
            bookApiMock.Verify(api => api.GetStatusAsync(Guid.Empty), Times.Once);
        }

        [TestMethod()]
        public async Task StopAsync_DoesNotThrowException()
        {
            // Arrange
            var bookApiMock = new Mock<IBookApiService>();
            var startupWorker = new StartupWorker(bookApiMock.Object);

            // Act & Assert
            await startupWorker.StopAsync(CancellationToken.None);
        }
    }
}