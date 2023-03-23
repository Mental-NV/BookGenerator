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
            .Matches(@"^[a-zA-Z0-9 ,'\-]+$").WithMessage("Title can only contain letters, digits, spaces, commas, hyphens and apostrophes.")
            .Matches(@"^[a-zA-Z0-9].*[a-zA-Z0-9]$").WithMessage("Title must start and end with a letter or digit.")
            .Matches(@"^[^,\-']*(,[^,\-']*[a-zA-Z0-9][^,\-']*)*$").WithMessage("Title can only contain consecutive commas if they are separated by letters or digits.")
            .Matches(@"^[^-',]+(-[^-',]+)*$").WithMessage("Title can only contain consecutive hyphens if they are separated by letters or digits.");
    }
}
