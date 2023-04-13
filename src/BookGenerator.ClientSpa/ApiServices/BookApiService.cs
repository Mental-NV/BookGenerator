using BookGenerator.Application.Contracts.Books;
using BookGenerator.Domain.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookGenerator.ClientSpa.ApiServices;

public class BookApiService : BaseApiService, IBookApiService
{
    public BookApiService(IHttpClientFactory httpClientFactory, IOptions<JsonOptions> jsonOptions, ILogger<BaseApiService> logger)
        : base(jsonOptions, logger, httpClientFactory, "BookApiClient")
    {
    }

    public async Task<CreateBookResponse?> CreateAsync(string bookTitle)
    {
        CreateBookResponse? result = await PostAsync<CreateBookResponse?, string>($"/api/book", bookTitle);
        return result;
    }

    public async Task<BookFile?> GetResultAsync(Guid bookId)
    {
        var result = await GetAsync<BookFile?>($"/api/book/download/{bookId}");
        return result;
    }

    public async Task<GetStatusResponse?> GetStatusAsync(Guid bookId)
    {
        var result = await GetAsync<GetStatusResponse?>($"/api/book/{bookId}");
        return result;
    }
}
