using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Application.Abstractions.Messsaging;
using BookGenerator.Application.Contracts.Books;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.Primitives;
using BookGenerator.Domain.Primitives.Result;
using BookGenerator.Domain.Repositories;
using BookGenerator.Domain.Services;

namespace BookGenerator.Application.Books.Commands.CreateBook;

public sealed class CreateBookCommandHandler : ICommandHandler<CreateBookCommand, CreateBookResponse>
{
    private readonly IBookRepository bookRepository;
    private readonly IProgressRepository progressRepository;
    private readonly IUnitOfWork unitOfWork;

    public CreateBookCommandHandler(IBookRepository bookRepository, IProgressRepository progressRepository, IUnitOfWork unitOfWork)
    {
        this.bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        this.progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<CreateBookResponse>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        (Book book, BookProgress progress) = Book.Create(request.BookTitle);
        bookRepository.Insert(book);
        progressRepository.Insert(progress);

        int rowAffected = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (rowAffected == 0)
        {
            return Result.Failure<CreateBookResponse>(new Error("Book.CreateFailed", "Failed to create book"));
        }
        
        return new CreateBookResponse(book.Id);
    }
}
