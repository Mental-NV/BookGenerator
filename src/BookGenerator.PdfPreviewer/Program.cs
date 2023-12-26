using BookGenerator.Domain.Core;
using BookGenerator.Domain.Services;
using BookGenerator.Infrastructure.Books;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

QuestPDF.Settings.License = LicenseType.Community;

Book book = new Book("A sample book");

for (int i = 0; i < 10; i++)
{
    Chapter chapter = new Chapter() 
    {
        BookId = book.Id,
        Order = i,
        Title = Placeholders.Name(),
    };

    chapter.Content = $"#{chapter.Title}\r\n{Placeholders.Paragraph()}\r\n##{Placeholders.Name()}\r\n{Placeholders.Paragraph()}\r\n###{Placeholders.Name()}\r\n{Placeholders.Paragraphs()}";

    book.Chapters.Add(chapter);
}

BookDocument bookDocument = new(book);
byte[] bytes = bookDocument.GeneratePdf();
bookDocument.ShowInPreviewer();



