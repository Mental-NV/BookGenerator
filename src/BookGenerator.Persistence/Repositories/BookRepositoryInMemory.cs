using BookGenerator.Domain.Core;
using BookGenerator.Domain.Services;
using System.Collections.Concurrent;

namespace BookGenerator.Persistence.Repositories;

public class BookRepositoryInMemory : IBookRepository
{
    private readonly static ConcurrentDictionary<Guid, Book> bookStore = new ConcurrentDictionary<Guid, Book>();
    private readonly static ConcurrentDictionary<Guid, BookProgress> progressStore = new ConcurrentDictionary<Guid, BookProgress>();

    public async Task<Book> GetAsync(Guid bookId)
    {
        bookStore.TryGetValue(bookId, out Book book);
        return await Task.FromResult(book);
    }
    public async Task InsertAsync(Book book)
    {
        bookStore[book.Id] = book;
        await Task.FromResult(book);
    }

    public async Task UpdateAsync(Book book)
    {
        bookStore[book.Id] = book;
        await Task.FromResult(book);
    }

    public async Task<BookProgress> GetProgressAsync(Guid bookId)
    {
        progressStore.TryGetValue(bookId, out BookProgress progress);
        return await Task.FromResult(progress);
    }

    public async Task UpdateProgressAsync(BookProgress bookProgress)
    {
        progressStore[bookProgress.BookId] = bookProgress;
        await Task.FromResult(bookProgress);
    }

    public async Task InsertProgressAsync(BookProgress bookProgress)
    {
        progressStore[bookProgress.BookId] = bookProgress;
        await Task.FromResult(bookProgress);
    }

    public async Task InsertChapterAsync(Chapter chapter)
    {
        bookStore[chapter.BookId].Chapters.Add(chapter);
        await Task.FromResult(chapter);
    }

    public async Task UpdateChapterAsync(Chapter chapter)
    {
        var c = bookStore[chapter.BookId].Chapters.First(x => x.Id == chapter.Id);
        c.Title = chapter.Title;
        c.Order = chapter.Order;
        c.Content = chapter.Content;
        await Task.FromResult(chapter);
    }

    public async Task<Chapter> GetChapterAsync(Guid chapterId)
    {
        return await Task.FromResult(bookStore.Values.SelectMany(x => x.Chapters).First(x => x.Id == chapterId));
    }
}
