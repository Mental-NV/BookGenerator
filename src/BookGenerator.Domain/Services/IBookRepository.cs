using BookGenerator.Domain.Core;

namespace BookGenerator.Domain.Services;

public interface IBookRepository
{
    Task<Book> GetAsync(Guid bookId);
    Task InsertAsync(Book book);
    Task UpdateAsync(Book book);
    Task InsertProgressAsync(BookProgress bookProgress);
    Task UpdateProgressAsync(BookProgress bookProgress);
    Task<BookProgress> GetProgressAsync(Guid bookId);
    Task InsertChapterAsync(Chapter chapter);
    Task UpdateChapterAsync(Chapter chapter);
    Task<Chapter> GetChapterAsync(Guid chapterId);
}
