using BookGenerator.Application.Books.Commands.CreateBook;
using BookGenerator.Domain.Core;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography.X509Certificates;

namespace BookGenerator.Application.UnitTests.Books.Commands.CreateBook
{
    [TestClass]
    public class CreateBookCommandValidatorTests
    {
        private CreateBookCommandValidator validator;

        [TestInitialize]
        public void TestInitialize()
        {
            validator = new CreateBookCommandValidator();
        }

        [TestMethod]
        public void TitleIsEmpty_ShouldHaveError()
        {
            var model = new CreateBookCommand(string.Empty);
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BookTitle);
        }

        [TestMethod]
        public void TitleIsTooShort_ShouldHaveError()
        {
            var model = new CreateBookCommand("a");
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BookTitle);
        }

        [TestMethod]
        public void TitleIsTooLong_ShouldHaveError()
        {
            var model = new CreateBookCommand(new string('A', 101));
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BookTitle);
        }

        [TestMethod]
        public void TitleContainsInvalidCharacters_ShouldHaveError()
        {
            var model = new CreateBookCommand("Title with invalid character: @");
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BookTitle);
        }

        [TestMethod]
        public void TitleDoesNotStartWithLetterOrDigit_ShouldHaveError()
        {
            var model = new CreateBookCommand("-Invalid title start");
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BookTitle);
        }

        [TestMethod]
        public void TitleDoesNotEndWithLetterOrDigit_ShouldHaveError()
        {
            var model = new CreateBookCommand("Invalid title end-");
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BookTitle);
        }

        [TestMethod]
        public void TitleHasConsecutiveCommas_ShouldHaveError()
        {
            var model = new CreateBookCommand("Title with consecutive,, commas");
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BookTitle);
        }

        [TestMethod]
        public void TitleHasConsecutiveHyphens_ShouldHaveError()
        {
            var model = new CreateBookCommand("Title with consecutive-- hyphens");
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BookTitle);
        }

        [TestMethod]
        public void TitleIsValid_ShouldNotHaveError()
        {
            var model = new CreateBookCommand("A valid title - Пример допустимого заглавия, 有效的标题");
            var result = validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.BookTitle);
        }
    }
}
