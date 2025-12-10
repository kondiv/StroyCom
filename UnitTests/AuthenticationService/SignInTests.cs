using Ardalis.Result;
using AuthenticationService.Abstractions;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Features.SignIn;
using AuthenticationService.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.AuthenticationService;

public class SignInTests
{
    private readonly Mock<ILogger<SignInCommandHandler>> _loggerMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly ServiceContext _context;
    private readonly Mock<IValidator<SignInCommand>> _validatorMock; 
    private readonly SignInCommandHandler _handler;

    public SignInTests()
    {
        _loggerMock = new Mock<ILogger<SignInCommandHandler>>();
        _validatorMock = new Mock<IValidator<SignInCommand>>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        var options = new DbContextOptionsBuilder<ServiceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ServiceContext(options);

        _handler = new SignInCommandHandler(_context, _validatorMock.Object, _passwordHasherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task SignInCommandHandler_WhenOk_ShouldReturnResultSuccess()
    {
        // Arrange
        var command = new SignInCommand("email@email.com", "Name", "Parol123!", ["admin"]);
        var cancellationToken = new CancellationTokenSource().Token;

        _passwordHasherMock.Setup(e => e.Hash(It.IsAny<string>()))
            .Returns("passwordHash");

        _validatorMock.Setup(e => e.Validate(command))
            .Returns(new FluentValidation.Results.ValidationResult
            {
                Errors = []
            });

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SignInCommandHandler_WhenEmailAlreadyTaken_ShouldReturnResultConflict()
    {
        // Arrange
        string sameEmail = "taken@mail.com";

        _context.Users.Add(new User
        {
            Email = sameEmail,
            Id = Guid.NewGuid(),
            Name = "Name",
            PasswordHash = "passwordHash",
            Roles = ["admin", "engineer"]
        });
        await _context.SaveChangesAsync();

        var command = new SignInCommand(sameEmail, "Name", "Password123!", ["admin"]);

        _passwordHasherMock.Setup(e => e.Hash(It.IsAny<string>()))
            .Returns("passwordHash");

        _validatorMock.Setup(e => e.Validate(command))
            .Returns(new FluentValidation.Results.ValidationResult
            {
                Errors = []
            });

        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsConflict());
    }

    [Fact]
    public async Task SignInCommandHandler_WhenValidatorErrorsOccurred_ShouldReturnResultInvalid()
    {
        // Arrange
        var command = new SignInCommand("kjsadfh", "12345", "dsaf", [""]);

        _passwordHasherMock.Setup(e => e.Hash(It.IsAny<string>()))
            .Returns("passwordHash");

        _validatorMock.Setup(e => e.Validate(command))
            .Returns(new FluentValidation.Results.ValidationResult
            {
                Errors = [new("Email", "InvalidEmail")]
            });

        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsInvalid());
    }
}
