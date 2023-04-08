using BookGenerator.Domain.DomainEvents;
using BookGenerator.Domain.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BookGenerator.Application.Books.Events;

internal sealed class BookCreationStartedDomainEventHandler
    : INotificationHandler<BookCreationStartedDomainEvent>
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public BookCreationStartedDomainEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    public async Task Handle(BookCreationStartedDomainEvent notification, CancellationToken cancellationToken)
    {
        var task = new Task(async () =>
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                IBookCreater bookCreater = scope.ServiceProvider.GetRequiredService<IBookCreater>();
                await bookCreater.CreateAsync(notification.BookId);
            }
        });
        task.Start();
        await Task.CompletedTask;
    }
}
