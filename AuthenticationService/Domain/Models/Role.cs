namespace AuthenticationService.Domain.Models;

public static class Role
{
    public const string Manager = "Manager";

    public const string Engineer = "Engineer";

    public const string Admin = "Admin";

    public static List<string> AllRoles() => [Manager, Engineer, Admin];
}
