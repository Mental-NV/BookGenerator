using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Domain.Abstraction;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.Primitives;
using BookGenerator.Persistence.Outbox;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        DateTime utcNow = DateTime.UtcNow;

        UpdateAudidableEntities(utcNow);

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .Property(x => x.Title)
            .HasColumnType("nvarchar(250)")
            .IsRequired();

        modelBuilder.Entity<Book>()
            .Property(x => x.CreateOnUtc)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Chapter>()
            .Property(x => x.Title)
            .HasColumnType("nvarchar(250)")
            .IsRequired();

        modelBuilder.Entity<Chapter>()
            .Property(x => x.CreateOnUtc)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Chapter>()
            .Property(x => x.Content)
            .IsRequired();

        modelBuilder.Entity<BookProgress>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<BookProgress>()
            .Property(x => x.Title)
            .HasColumnType("nvarchar(250)")
            .IsRequired();

        modelBuilder.Entity<BookProgress>()
            .Property(x => x.CreateOnUtc)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<BookProgress>()
            .HasOne<Book>()
            .WithOne()
            .HasForeignKey<BookProgress>(x => x.Id);

        modelBuilder.Entity<OutboxMessage>()
            .Property(x => x.Type)
            .HasColumnType("nvarchar(250)");

        modelBuilder.Entity<OutboxMessage>()
            .Property(x => x.Content)
            .HasColumnType("nvarchar(4000)");

        modelBuilder.Entity<OutboxMessage>()
            .Property(x => x.Error)
            .HasColumnType("nvarchar(4000)");
    }

    /// <inheritdoc />
    public new DbSet<TEntity> Set<TEntity>()
        where TEntity : Entity
        => base.Set<TEntity>();

    /// <inheritdoc />
    public async Task<TEntity> GetBydIdAsync<TEntity>(Guid id)
        where TEntity : Entity
    {
        if (id == Guid.Empty)
        {
            return null;
        }
        return await Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id);
    }

    /// <inheritdoc />
    public void Insert<TEntity>(TEntity entity)
        where TEntity : Entity
        => Set<TEntity>().Add(entity);

    /// <inheritdoc />
    public void InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities)
        where TEntity : Entity
        => Set<TEntity>().AddRange(entities);

    /// <inheritdoc />
    public new void Remove<TEntity>(TEntity entity)
        where TEntity : Entity
        => Set<TEntity>().Remove(entity);

    /// <inheritdoc />
    public Task<int> ExecuteSqlAsync(string sql, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken = default)
        => Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);

    private void UpdateAudidableEntities(DateTime utcNow)
    {
        foreach (EntityEntry<IAuditableEntity> auditableEntity in this.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (auditableEntity.State == EntityState.Added)
            {
                auditableEntity.Property(nameof(IAuditableEntity.CreateOnUtc)).CurrentValue = utcNow;
            }

            if (auditableEntity.State == EntityState.Modified)
            {
                auditableEntity.Property(nameof(IAuditableEntity.ModifiedOnUtc)).CurrentValue = utcNow;
            }
        }
    }
}
