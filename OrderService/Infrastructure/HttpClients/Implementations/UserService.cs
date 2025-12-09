using Ardalis.Result;
using OrderService.Infrastructure.HttpClients.Abstractions;

namespace OrderService.Infrastructure.HttpClients.Implementations;

public sealed class UserService(
    HttpClient httpClient,
    ILogger<UserService> logger) : IUserService
{
    public async Task<Result> UserExists(Guid userId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Checking user existence in UserService");

        try
        {
            var response = await httpClient.GetAsync(
                $"http://authentication.service:8080/api/v1/users/{userId}:exists",
                cancellationToken);

            return response.IsSuccessStatusCode ? Result.Success() : Result.NotFound();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to check user existence");
            return Result.Error("Failed to check user existence, try again later");
        }
    }
}
