using BookGenerator.Domain.Core;
using BookGenerator.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace BookGenerator.Persistence;

public class BookRepository : IBookRepository
{
    private readonly BookContext context;

    public BookRepository(BookContext context)
    {
        this.context = context;
    }

    public async Task<Book> GetAsync(Guid bookId)
    {
        return await context.Books.Include(c => c.Chapters.OrderBy(x => x.Order)).FirstAsync(c => c.Id == bookId);
    }

    public async Task InsertAsync(Book book)
    {
        await context.Books.AddAsync(book);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        context.Books.Update(book);
        await context.SaveChangesAsync();
    }

    public async Task<BookProgress> GetProgressAsync(Guid bookId)
    {
        return await context.BookProgresses.FindAsync(bookId);
    }

    public async Task UpdateProgressAsync(BookProgress bookProgress)
    {
        context.BookProgresses.Update(bookProgress);
        await context.SaveChangesAsync();
    }

    public async Task InsertProgressAsync(BookProgress bookProgress)
    {
        context.BookProgresses.Add(bookProgress);
        await context.SaveChangesAsync();
    }

    public async Task InsertChapterAsync(Chapter chapter)
    {
        await context.Chapters.AddAsync(chapter);
        await context.SaveChangesAsync();
    }

    public async Task UpdateChapterAsync(Chapter chapter)
    {
        context.Chapters.Update(chapter);
        await context.SaveChangesAsync();
    }

    public async Task<Chapter> GetChapterAsync(Guid chapterId)
    {
        return await context.Chapters.FindAsync(chapterId);
    }
}
