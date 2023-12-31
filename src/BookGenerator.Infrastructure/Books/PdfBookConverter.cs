using BookGenerator.Domain.Core;
using BookGenerator.Domain.Services;
using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using QuestPDF.Elements;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using QuestPDF.Drawing;

namespace BookGenerator.Infrastructure.Books;

public class PdfBookConverter : IBookConverter
{
    public BookFile Convert(Book book)
    {
        BookDocument bookDocument = new(book);

        return new BookFile
        {
            Content = bookDocument.GeneratePdf(),
            Name = $"{book.Title}.pdf",
            ContentType = "application/pdf"
        };
    }
}

public class BookDocument : IDocument
{
    private readonly Book book;

    public BookDocument(Book book)
    {
        this.book = book;
    }

    public DocumentMetadata GetMetadata()
    {
        return DocumentMetadata.Default;
    }

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(50);
                page.DefaultTextStyle(style => style.FontFamily("Noto"));
                page.Content().Element(ComposeContent);
                page.Footer().Dynamic(new PageNumberFooter());
            });
    }

    public void ComposeContent(IContainer container)
    {
        container.Column(stack =>
        {
            stack.Item()
                 .PaddingTop(300)
                 .AlignCenter()
                 .Text(book.Title)
                 .FontSize(26)
                 .SemiBold();
            stack.Item()
                 .PageBreak();

            foreach (var chapter in book.Chapters)
            {
                string[] paragraphs = chapter.Content.Split("\n");
                foreach (var paragraph in paragraphs)
                {
                    if (paragraph.StartsWith("###", StringComparison.InvariantCultureIgnoreCase))
                    {
                        stack.Item().PaddingVertical(5).Text(paragraph.TrimStart('#')).FontSize(18).SemiBold();
                    }
                    else if (paragraph.StartsWith("##", StringComparison.InvariantCultureIgnoreCase))
                    {
                        stack.Item().PaddingVertical(15).Text(paragraph.TrimStart('#')).FontSize(22).SemiBold();
                    }
                    else if (paragraph.StartsWith("#", StringComparison.InvariantCultureIgnoreCase))
                    {
                        stack.Item().PaddingVertical(25).Text(paragraph.TrimStart('#')).FontSize(26).SemiBold();
                    }
                    else
                    {
                        stack.Item().Text(text =>
                        {
                            text.DefaultTextStyle(style => style.FontSize(12).NormalWeight());

                            var normal = TextStyle.Default.NormalWeight();
                            var bold = TextStyle.Default.SemiBold();

                            int pos = 0;
                            while (pos < paragraph.Length)
                            {
                                int p1 = paragraph.IndexOf("**", pos);
                                int p2 = paragraph.IndexOf("**", p1 + 2);
                                if (p1 != -1 && p2 != -1)
                                {
                                    if (pos < p1)
                                    {
                                        text.Span(paragraph[pos..p1]).Style(normal);
                                    }
                                    text.Span(paragraph[(p1 + 2)..p2]).Style(bold);
                                    pos = p2 + 2;
                                }
                                else
                                {
                                    text.Span(paragraph[pos..]).Style(normal);
                                    pos = paragraph.Length;
                                }
                            }
                        });


                    }   
                }
                if (chapter != book.Chapters.Last())
                {
                    stack.Item().PageBreak();
                }
            }
        });
    }
}

class PageNumberFooter : IDynamicComponent
{
    public DynamicComponentComposeResult Compose(DynamicContext context)
    {
        var content = context.CreateElement(element =>
        {
            element.ShowIf(context.PageNumber > 1).AlignCenter().Text(x =>
            {
                x.CurrentPageNumber();
            });
        });

        return new DynamicComponentComposeResult()
        {
            Content = content,
            HasMoreContent = false
        };
    }
}
