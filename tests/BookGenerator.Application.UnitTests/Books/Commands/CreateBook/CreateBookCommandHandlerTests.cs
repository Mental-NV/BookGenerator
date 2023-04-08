using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Application.Books.Commands.CreateBook;
using BookGenerator.Application.Contracts.Books;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.Primitives.Result;
using BookGenerator.Domain.Repositories;
using BookGenerator.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookGenerator.Application.UnitTests.Books.Commands.CreateBook
{
    [TestClass]
    public class CreateBookCommandHandlerTests
    {
        [TestMethod]
        public async Task Handle_ValidRequest_CreatesBookAndReturnsSuccess()
        {
            // Arrange
            var bookRepositoryMock = new Mock<IBookRepository>();
            var progressRepositoryMock = new Mock<IProgressRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var handler = new CreateBookCommandHandler(bookRepositoryMock.Object, progressRepositoryMock.Object, unitOfWorkMock.Object);
            var request = new CreateBookCommand("Test Book");

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreNotEqual(Guid.Empty, result.Value.BookId);
            bookRepositoryMock.Verify(br => br.Insert(It.IsAny<Book>()), Times.Once);
            progressRepositoryMock.Verify(pr => pr.Insert(It.IsAny<BookProgress>()), Times.Once);
            unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [TestMethod]
        public async Task Handle_SaveChangesFailed_ReturnsFailure()
        {
            // Arrange
            var bookRepositoryMock = new Mock<IBookRepository>();
            var progressRepositoryMock = new Mock<IProgressRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

            var handler = new CreateBookCommandHandler(bookRepositoryMock.Object, progressRepositoryMock.Object, unitOfWorkMock.Object);
            var request = new CreateBookCommand("Test Book");

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Book.CreateFailed", result.Error.Code);
            bookRepositoryMock.Verify(br => br.Insert(It.IsAny<Book>()), Times.Once);
            progressRepositoryMock.Verify(pr => pr.Insert(It.IsAny<BookProgress>()), Times.Once);
            unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
        }
    }


}
