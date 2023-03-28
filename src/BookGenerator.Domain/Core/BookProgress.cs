using BookGenerator.Domain.Primitives;

namespace BookGenerator.Domain.Core;

public class BookProgress : Entity
{
    public BookProgress(Book book) : base(book.Id)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Book"/> class.
    /// </summary>
    /// <remarks>
    /// Required by EF Core.
    /// </remarks>
    private BookProgress()
    {
    }

    public BookStatus Status { get; set; }
    public string Title { get; set; }
    public int Progress { get; set; }
}
