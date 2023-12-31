using BookGenerator.Domain.Core;
using BookGenerator.Domain.Services;
using BookGenerator.Infrastructure;
using BookGenerator.Infrastructure.Books;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using System.Drawing;

DependencyInjection.InitializeQueryPdf();

Book book = new Book("A sample book");

for (int i = 0; i < 10; i++)
{
    Chapter chapter = new Chapter() 
    {
        BookId = book.Id,
        Order = i,
        Title = Placeholders.Name(),
    };

    chapter.Content = $"#{chapter.Title}\r\n{Placeholders.Paragraph()}\r\nNormal text sample. **Bold text sample.** Another normal text. **An important text.** Normal text.\r\n##{Placeholders.Name()}\r\n{Placeholders.Paragraph()}\r\n###{Placeholders.Name()}\r\nNormal text sample. **Bold text sample.** Another normal text. **An important text.** Normal text.\r\n{Placeholders.Paragraphs()}";

    book.Chapters.Add(chapter);
}

BookDocument bookDocument = new(book);
bookDocument.GeneratePdf("test.pdf");
bookDocument.ShowInPreviewer();



