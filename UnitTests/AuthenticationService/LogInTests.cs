using Ardalis.Result;
using AuthenticationService.Abstractions;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Features.LogIn;
using AuthenticationService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.AuthenticationService;

public class LogInTests
{
    private readonly ServiceContext _context;
    private readonly LogInCommandHandler _handler;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenProvider> _tokenProviderMock;
    private readonly Mock<ILogger<LogInCommandHandler>> _loggerMock;

    public LogInTests()
    {
        var options = new DbContextOptionsBuilder<ServiceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ServiceContext(options);

        _passwordHasherMock = new Mock<IPasswordHasher>();
        _loggerMock = new Mock<ILogger<LogInCommandHandler>>();
        _tokenProviderMock = new Mock<ITokenProvider>();

        _handler = new LogInCommandHandler(_context, _passwordHasherMock.Object,
            _tokenProviderMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task LogInCommandHandler_WhenOk_ShouldReturnSuccess()
    {
        // Arrange
        string email = "email@email.com";
        string name = "name";
        List<string> roles = ["admin", "manager"];

        _context.Users.Add(new User
        {
            Email = email,
            Id = Guid.NewGuid(),
            Name = name,
            PasswordHash = "passwordHash",
            Roles = roles
        });
        await _context.SaveChangesAsync();

        var command = new LogInCommand(email, "password");

        _passwordHasherMock
            .Setup(e => e.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        _tokenProviderMock
            .Setup(e => e.GetAccessToken(It.IsAny<Guid>(), email, roles))
            .Returns("access_token");

        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task LogInCommandHandler_WhenUserDoesNotExist_ShouldReturnResultUnauthorized()
    {
        // Arrange
        var command = new LogInCommand("email@email.com", "Password123!");
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsUnauthorized());
    }

    [Fact]
    public async Task LogInCommandHandler_WhenPasswordIsIncorrect_ShouldReturnResultUnauthorized()
    {
        // Arrange
        string sameEmail = "same@email.com";

        _context.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = sameEmail,
            Name = "Name",
            PasswordHash = "passwordHash",
            Roles = ["admin", "manager"]
        });
        await _context.SaveChangesAsync();

        var command = new LogInCommand(sameEmail, "wrongPassword123!");

        _passwordHasherMock
            .Setup(e => e.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsUnauthorized());
    }
}
