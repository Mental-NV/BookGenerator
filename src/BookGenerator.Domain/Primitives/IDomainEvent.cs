using BookGenerator.Domain.DomainEvents;
using MediatR;
using System.Text.Json.Serialization;

namespace BookGenerator.Domain.Primitives;

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(typeof(IDomainEvent), 0)]
[JsonDerivedType(typeof(BookCreationStartedDomainEvent), 1)]
public interface IDomainEvent : INotification
{
}
