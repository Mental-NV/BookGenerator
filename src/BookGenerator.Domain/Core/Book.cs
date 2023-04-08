using BookGenerator.Domain.Abstraction;
using BookGenerator.Domain.DomainEvents;
using BookGenerator.Domain.Primitives;

namespace BookGenerator.Domain.Core;

public class Book : AggregateRoot, IAuditableEntity
{
    public Book(string title)
        : base(Guid.NewGuid())
    {
        this.Title = title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Book"/> class.
    /// </summary>
    /// <remarks>
    /// Required by EF Core.
    /// </remarks>
    private Book()
    { 
    }

    public string Title { get; set; }

    public virtual List<Chapter> Chapters { get; set; } = new List<Chapter>();

    public DateTime CreateOnUtc { get; protected init; }

    public DateTime? ModifiedOnUtc { get; protected init; }

    public static (Book, BookProgress) Create(string title)
    {
        Book book = new(title);
        BookProgress progress = new(book)
        {
            Status = BookStatus.Pending,
            Title = title
        };
        book.RaiseDomainEvent(new BookCreationStartedDomainEvent(book.Id));

        return (book, progress);
    }
}
