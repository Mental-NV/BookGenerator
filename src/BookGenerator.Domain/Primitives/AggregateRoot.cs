namespace BookGenerator.Domain.Primitives;

public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> domainEvents = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
    /// </summary>
    /// <param name="id">The aggregate root id.</param>
    public AggregateRoot(Guid id)
        : base(id)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
    /// </summary>
    /// <remarks>
    /// Required by EF Core.
    /// </remarks>
    protected AggregateRoot()
    { 
    }

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => domainEvents.ToList();

    public void ClearDomainEvents() => domainEvents.Clear();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }
}
