using BookGenerator.Domain.Core;

namespace BookGenerator.Domain.Services;

public interface IBookConverter
{
    BookFile Convert(Book book, BookFileFormat format);
}
