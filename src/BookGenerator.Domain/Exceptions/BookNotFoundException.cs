namespace BookGenerator.Domain.Exceptions;

public class BookNotFoundException : BookDomainException
{
    public BookNotFoundException(string message) : base(message)
    {
    }
}
