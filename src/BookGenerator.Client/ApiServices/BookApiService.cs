using BookGenerator.Application.Contracts.Books;
using BookGenerator.Domain.Core;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookGenerator.Client.ApiServices;

public class BookApiService : BaseApiService, IBookApiService
{
    public BookApiService(IHttpClientFactory httpClientFactory)
    {
        if (httpClientFactory is null)
        {
            throw new ArgumentNullException(nameof(httpClientFactory));
        }

        this.httpClient = httpClientFactory.CreateClient("BookApiClient");
    }

    public async Task<CreateBookResponse> CreateAsync(string bookTitle)
    {
        CreateBookResponse result = await this.PostAsync<CreateBookResponse, string>($"/api/book", bookTitle);
        return result;
    }

    public async Task<BookFile> GetResultAsync(Guid bookId)
    {
        var result = await this.GetAsync<BookFile>($"/api/book/download/{bookId}");
        return result;
    }

    public async Task<GetStatusResponse> GetStatusAsync(Guid bookId)
    {
        var result = await this.GetAsync<GetStatusResponse>($"/api/book/{bookId}");
        return result;
    }
}
