using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Features.Auth.Commands.Login;
using DonationHub.Infrastructure.Identity.DTOs;
using DonationHub.Infrastructure.Identity.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace DonationHub.UnitTests.Application.Handlers;

public class LoginCommandHandlerTests
{
    private readonly Mock<IAuthenticationService> _authenticationService = new();

    [Theory]
    [InlineData("unknown@example.com", "AnyPassword1!")]
    [InlineData("existing@example.com", "WrongPassword!")]
    public async Task Handle_UnknownEmailOrWrongPassword_ReturnsSameInvalidCredentialsError(
        string email,
        string password)
    {
        // Arrange
        var command = new LoginCommand(email, password);
        var expectedFailure = Result.Failure<AuthDTO>(AuthenticationErrors.InvalidCredentials);

        _authenticationService
            .Setup(s => s.LoginAsync(
                It.Is<LoginDTO>(dto => dto.Email == email && dto.Password == password),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFailure);

        var handler = new LoginCommandHandler(_authenticationService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthenticationErrors.InvalidCredentials);
        _authenticationService.Verify(
            s => s.LoginAsync(
                It.Is<LoginDTO>(dto => dto.Email == email && dto.Password == password),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccessWithAuthDto()
    {
        // Arrange
        var command = new LoginCommand("user@example.com", "CorrectPass1!");
        var authDto = new AuthDTO
        {
            UserId = Guid.NewGuid(),
            Email = "user@example.com",
            FullName = "Test User",
            AccessToken = "jwt-access-token",
            AccessTokenExpiration = DateTimeOffset.UtcNow.AddHours(1),
            RefreshToken = "refresh-token",
            RefreshTokenExpiration = DateTimeOffset.UtcNow.AddDays(7),
            Roles = new List<string> { "Donor" }
        };
        var expectedSuccess = Result.Success(authDto);

        _authenticationService
            .Setup(s => s.LoginAsync(
                It.Is<LoginDTO>(dto => dto.Email == command.Email && dto.Password == command.Password),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSuccess);

        var handler = new LoginCommandHandler(_authenticationService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be("user@example.com");
        result.Value.AccessToken.Should().Be("jwt-access-token");
        result.Value.Roles.Should().Contain("Donor");
    }
}