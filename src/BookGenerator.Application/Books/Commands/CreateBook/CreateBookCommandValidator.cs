using FluentValidation;
using System.Text.RegularExpressions;

namespace BookGenerator.Application.Books.Commands.CreateBook;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.BookTitle)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Title cannot be empty.")
            .Length(1, 100).WithMessage("Title must be between 1 and 100 characters long.")
            .Matches(new Regex(@"^[\p{L}\p{N}]+[\p{L}\p{N} ,'-]*[\p{L}\p{N}]+$")).WithMessage("Title can only contain letters, digits, spaces, commas, hyphens, and apostrophes, and must start and end with a letter or digit.")
            .Must(NotHaveConsecutiveSpacesOrPunctuation).WithMessage("Title cannot have consecutive spaces or punctuation marks.");
    }

    private bool NotHaveConsecutiveSpacesOrPunctuation(string title)
    {
        var consecutiveSpacesOrPunctuation = new Regex(@"(\s\s)|([,'-]{2})");
        return !consecutiveSpacesOrPunctuation.IsMatch(title);
    }
}
