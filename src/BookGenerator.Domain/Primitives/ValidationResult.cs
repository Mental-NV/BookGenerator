using BookGenerator.Domain.Primitives.Result;

namespace BookGenerator.Domain.Primitives;

public sealed class ValidationResult : Result.Result, IValidationResult
{
    public ValidationResult(Error[] errors)
        : base(false, IValidationResult.ValidationError)
    {
        Errors = errors;
    }

    public Error[] Errors { get; }

    public static ValidationResult WithErrors(Error[] errors) => new(errors);
}
