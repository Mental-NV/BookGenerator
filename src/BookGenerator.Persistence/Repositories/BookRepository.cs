using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.Repositories;
using BookGenerator.Persistence.Books;
using Microsoft.EntityFrameworkCore;

namespace BookGenerator.Persistence.Repositories;

internal class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(IDbContext dbContext) : base(dbContext)
    {
    }

    public new async Task<Book> GetByIdAsync(Guid id)
    {
        return await DbContext.Set<Book>().Include(x => x.Chapters.OrderBy(c => c.Order)).FirstOrDefaultAsync(x => x.Id == id);
    }
}
