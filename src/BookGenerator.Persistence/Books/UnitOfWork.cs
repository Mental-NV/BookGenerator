using BookGenerator.Application.Abstractions.Data;

namespace BookGenerator.Persistence.Books;

public class UnitOfWork : IUnitOfWork
{
    private readonly BookDbContext dbContext;

    public UnitOfWork(BookDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
