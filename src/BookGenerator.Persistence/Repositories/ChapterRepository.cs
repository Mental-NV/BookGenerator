using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.Repositories;

namespace BookGenerator.Persistence.Repositories;

internal sealed class ChapterRepository : GenericRepository<Chapter>, IChapterRepository
{
    public ChapterRepository(IDbContext dbContext) : base(dbContext)
    {
    }
}
