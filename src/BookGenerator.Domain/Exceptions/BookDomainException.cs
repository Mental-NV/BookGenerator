namespace BookGenerator.Domain.Exceptions
{
    public class BookDomainException : Exception
    {
        public BookDomainException(string message) : base(message)
        {
        }
    }
}
