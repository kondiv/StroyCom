namespace AuthenticationService.Abstractions;

public interface ITokenProvider
{
    string GetAccessToken(Guid userId, string email, List<string> roles);
}
