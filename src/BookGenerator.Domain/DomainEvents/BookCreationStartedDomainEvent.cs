using BookGenerator.Domain.Primitives;

namespace BookGenerator.Domain.DomainEvents;

public sealed record BookCreationStartedDomainEvent(Guid BookId) : IDomainEvent;
