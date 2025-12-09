using Ardalis.Result;

namespace OrderService.Infrastructure.HttpClients.Abstractions;

public interface IUserService
{
    Task<Result> UserExists(Guid userId, CancellationToken cancellationToken = default);
}
