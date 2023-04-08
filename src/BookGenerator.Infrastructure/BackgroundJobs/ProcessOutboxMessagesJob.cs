using BookGenerator.Application.Contracts;
using BookGenerator.Domain.Primitives;
using BookGenerator.Persistence.Books;
using BookGenerator.Persistence.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Text.Json;

namespace BookGenerator.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesJob : IJob
{
    private readonly BookDbContext dbContext;
    private readonly IPublisher publisher;
    private readonly ILogger<ProcessOutboxMessagesJob> logger;

    public ProcessOutboxMessagesJob(BookDbContext dbContext, IPublisher publisher)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await dbContext
            .OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        foreach (OutboxMessage outboxMessage in messages)
        {
            IDomainEvent domainEvent = JsonSerializer
                .Deserialize<IDomainEvent>(outboxMessage.Content,
                new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = null
                });

            if (domainEvent is null)
            {
                continue;
            }

            try
            {
                await publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to publish a domain event from an outbox message {Id}, exception: {ex}", outboxMessage.Id, ex);
            }

            outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
        }

        if (dbContext.ChangeTracker.HasChanges())
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
