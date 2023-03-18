namespace BookGenerator.Domain.Core;

public class BookProgress
{
    public Guid BookId { get; set; }
    public BookStatus Status { get; set; }
    public string Title { get; set; }
    public int Progress { get; set; }
}
