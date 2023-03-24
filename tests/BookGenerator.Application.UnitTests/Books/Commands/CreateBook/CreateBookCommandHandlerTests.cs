using BookGenerator.Application.Books.Commands.CreateBook;
using BookGenerator.Application.Contracts.Books;
using BookGenerator.Domain.Primitives.Result;
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
        private readonly Mock<IBookCreater> bookCreaterMock = new();

        [TestMethod]
        public async Task Handle_ValidCommand_CreatesBook()
        {
            // Arrange
            var command = new CreateBookCommand("Test book");
            bookCreaterMock
                .Setup(x => x.CreateAsync(It.IsAny<string>()))
                .ReturnsAsync(Guid.NewGuid());
            var target = new CreateBookCommandHandler(bookCreaterMock.Object);
            // Act
            Result<CreateBookResponse> actual = await target.Handle(command, default);
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccess);
            Assert.IsNotNull(actual.Value);
            Assert.AreNotEqual(Guid.Empty, actual.Value.BookId);
            bookCreaterMock.Verify(x => x.CreateAsync(command.BookTitle), Times.Once);
        }
        
        [TestMethod]
        public void Constructor_NullParam_ThrowsException()
        {
            // Arrange
            // Act
            Action act = () => new CreateBookCommandHandler(null);
            // Assert
            Assert.ThrowsException<ArgumentNullException>(act);
            bookCreaterMock.Verify(x => x.CreateAsync(It.IsAny<string>()), Times.Never);
        }
    }


}
