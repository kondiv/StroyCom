namespace AuthenticationService.Contracts;

public sealed record SignInRequest(string Email, string Name, string Password, string[] Roles);
