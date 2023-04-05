using BookGenerator.Domain.Abstraction;
using BookGenerator.Domain.Primitives;

namespace BookGenerator.Domain.Core;

public class Chapter : Entity, IAuditableEntity
{
    public Chapter()
        : base(Guid.NewGuid())
    {
    }
    public int Order { get; set; }
    public Guid BookId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public DateTime CreateOnUtc { get; protected init; }

    public DateTime? ModifiedOnUtc { get; protected init; }
}
