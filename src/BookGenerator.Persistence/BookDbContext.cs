using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Domain.Core;
using Microsoft.EntityFrameworkCore;

namespace BookGenerator.Persistence.Books;

public class BookDbContext : DbContext, IDbContext, IUnitOfWork
{
    public BookDbContext(DbContextOptions<BookDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }

    public DbSet<BookProgress> BookProgresses { get; set; }

    public DbSet<Chapter> Chapters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .Property(x => x.Title)
            .HasColumnType("nvarchar(250)")
            .IsRequired();

        modelBuilder.Entity<Chapter>()
            .Property(x => x.Title)
            .HasColumnType("nvarchar(250)")
            .IsRequired();

        modelBuilder.Entity<Chapter>()
            .Property(x => x.Content)
            .IsRequired();

        modelBuilder.Entity<BookProgress>()
            .HasKey(x => x.BookId);

        modelBuilder.Entity<BookProgress>()
            .Property(x => x.Title)
            .HasColumnType("nvarchar(250)")
            .IsRequired();

        modelBuilder.Entity<BookProgress>()
            .HasOne<Book>()
            .WithOne()
            .HasForeignKey<BookProgress>(x => x.BookId);
    }
}
