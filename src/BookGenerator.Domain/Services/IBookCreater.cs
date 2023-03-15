using BookGenerator.Domain.Core;

namespace BookGenerator.Domain.Services;

public interface IBookCreater
{
    Task<Guid> CreateAsync(string bookTitle);
}
