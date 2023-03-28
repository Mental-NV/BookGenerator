using BookGenerator.Domain.Core;
using BookGenerator.Domain.Services;
using BookGenerator.Persistence.Books;
using Microsoft.EntityFrameworkCore;

namespace BookGenerator.Persistence.Repositories;

public class BookRepository : IBookRepository
{
    private readonly BookDbContext context;

    public BookRepository(BookDbContext context)
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
    }

    public async Task UpdateAsync(Book book)
    {
        context.Books.Update(book);
        await Task.CompletedTask;
    }

    public async Task<BookProgress> GetProgressAsync(Guid bookId)
    {
        return await context.BookProgresses.FindAsync(bookId);
    }

    public async Task UpdateProgressAsync(BookProgress bookProgress)
    {
        context.BookProgresses.Update(bookProgress);
        await Task.CompletedTask;
    }

    public async Task InsertProgressAsync(BookProgress bookProgress)
    {
        context.BookProgresses.Add(bookProgress);
        await Task.CompletedTask;
    }

    public async Task InsertChapterAsync(Chapter chapter)
    {
        await context.Chapters.AddAsync(chapter);
    }

    public async Task UpdateChapterAsync(Chapter chapter)
    {
        context.Chapters.Update(chapter);
        await Task.CompletedTask;
    }

    public async Task<Chapter> GetChapterAsync(Guid chapterId)
    {
        return await context.Chapters.FindAsync(chapterId);
    }
}
