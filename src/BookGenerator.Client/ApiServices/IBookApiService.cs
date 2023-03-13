using BookGenerator.Application.Contracts.Books;
using BookGenerator.Domain.Core;
using System;
using System.Threading.Tasks;

namespace BookGenerator.Client.ApiServices;

internal interface IBookApiService
{
    Task<CreateBookResponse> CreateAsync(string bookTitle);
    Task<GetStatusResponse> GetStatusAsync(Guid bookId);
    Task<BookFile> GetResult(Guid bookId);
}
