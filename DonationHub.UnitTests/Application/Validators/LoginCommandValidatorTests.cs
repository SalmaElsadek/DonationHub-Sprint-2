using DonationHub.Application.Features.Auth.Commands.Login;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace DonationHub.UnitTests.Application.Validators;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    private static LoginCommand CreateValidCommand(
        string? email = null,
        string? password = null) =>
        new(
            email ?? "donor@example.com",
            password ?? "StrongPass1");

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    public void Validate_InvalidOrMissingEmail_FailsForEmail(string invalidEmail)
    {
        // Arrange
        var command = CreateValidCommand(email: invalidEmail);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_MissingPassword_FailsForPassword(string invalidPassword)
    {
        // Arrange
        var command = CreateValidCommand(password: invalidPassword);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}