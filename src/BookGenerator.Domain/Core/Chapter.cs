using BookGenerator.Domain.Primitives;

namespace BookGenerator.Domain.Core;

public class Chapter : Entity
{
    public Chapter()
        : base(Guid.NewGuid())
    {
    }
    public int Order { get; set; }
    public Guid BookId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}
