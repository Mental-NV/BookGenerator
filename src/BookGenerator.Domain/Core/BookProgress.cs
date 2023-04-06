using BookGenerator.Domain.Abstraction;
using BookGenerator.Domain.Primitives;

namespace BookGenerator.Domain.Core;

public class BookProgress : Entity, IAuditableEntity
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
    public string ErrorMessage { get; set; }

    public DateTime CreateOnUtc { get; protected init; }
    public DateTime? ModifiedOnUtc { get; protected init; }
}
