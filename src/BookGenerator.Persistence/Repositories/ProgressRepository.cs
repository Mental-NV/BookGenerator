using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.Repositories;

namespace BookGenerator.Persistence.Repositories;

internal sealed class ProgressRepository : GenericRepository<BookProgress>, IProgressRepository
{
    public ProgressRepository(IDbContext dbContext) : base(dbContext)
    {
    }
}
