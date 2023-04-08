using BookGenerator.Application.Contracts;
using BookGenerator.Domain.Primitives;
using BookGenerator.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace BookGenerator.Persistence.Interceptors;

public sealed class ConvertDomainEventsToOutboxMessagesInterceptor
    : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext dbContext = eventData.Context;

        if (dbContext == null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        List<OutboxMessage> events = dbContext.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.GetDomainEvents();
                aggregateRoot.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage()
            {
                Id = Guid.NewGuid(),
                OccuredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonSerializer.Serialize(
                    domainEvent,
                    new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = null
                    })
            })
            .ToList();

        if (events.Any())
        {
            dbContext.Set<OutboxMessage>().AddRange(events);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
