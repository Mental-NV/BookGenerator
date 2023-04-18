using BookGenerator.Domain.Core;
using BookGenerator.Domain.Services;
using System.Text;

namespace BookGenerator.Infrastructure.Books;

public class BookConverter : IBookConverter
{
    public BookFile ToTextFile(Book book)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(book.Title);
        foreach (var chapter in book.Chapters)
        {
            sb.AppendLine(chapter.Title);
            sb.AppendLine(chapter.Content);
        }
        return new BookFile()
        {
            Name = $"{book.Title}.md",
            Content = Encoding.UTF8.GetBytes(sb.ToString()),
            ContentType = "text/plain"
        };
    }
}
