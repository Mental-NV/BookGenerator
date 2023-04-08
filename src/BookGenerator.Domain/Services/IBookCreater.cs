using BookGenerator.Domain.Core;

namespace BookGenerator.Domain.Services;

public interface IBookCreater
{
    Task CreateAsync(Guid bookId);
}
