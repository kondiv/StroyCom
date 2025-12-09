using AuthenticationService.Domain.Models;
using FluentValidation;

namespace AuthenticationService.Features.SignIn;

public sealed class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Email is required")
            .Matches(@"^((?!\.)[\w\-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$").WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email max length 255");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(128).WithMessage("Name max length 128");

        RuleFor(c => c.Password)
            .Matches(@"[a-z]").WithMessage("At least one lowercase letter required")
            .Matches(@"[A-Z]").WithMessage("At least one uppercase letter required")
            .Matches(@"[0-9]").WithMessage("At least one digit required")
            .MinimumLength(6).WithMessage("Password minimum length 6");

        RuleForEach(c => c.Roles)
            .NotEmpty().WithMessage("Role is required")
            .Must(BeValidRole).WithName("Must be valid role");
    }

    private static bool BeValidRole(string role)
    {
        return Role.AllRoles().Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase));
    }
}
