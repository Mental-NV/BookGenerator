using BookGenerator.Domain.Core;
using BookGenerator.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace BookGenerator.Application.Abstractions.Data;

public interface IDbContext
{
    DbSet<Book> Books { get; set; }

    DbSet<BookProgress> BookProgresses { get; set; }

    DbSet<Chapter> Chapters { get; set; }
}
