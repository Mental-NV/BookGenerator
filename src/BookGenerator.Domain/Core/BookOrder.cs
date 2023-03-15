namespace BookGenerator.Domain.Core;

public class BookOrder
{
    public Guid BookId { get; set; }
    public BookStatus Status { get; set; }
    public string Title { get; set; }
}
