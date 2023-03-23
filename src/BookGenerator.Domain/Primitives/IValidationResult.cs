namespace BookGenerator.Domain.Primitives;

public interface IValidationResult
{
    public static readonly Error ValidationError = new(
        "Validation Error",
        "A validation problem occured");

    Error[] Errors { get; }
}
