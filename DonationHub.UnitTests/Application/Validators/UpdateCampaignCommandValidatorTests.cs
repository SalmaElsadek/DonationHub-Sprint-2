using DonationHub.Application.Features.Campaigns.Commands.UpdateCampaign;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace DonationHub.UnitTests.Application.Validators;

public class UpdateCampaignCommandValidatorTests
{
    private readonly UpdateCampaignCommandValidator _validator = new();

    private static UpdateCampaignCommand CreateValidCommand(
    Guid? id = null,
    string? titleAr = null,
    string? titleEn = null,
    string? descriptionAr = null,
    string? descriptionEn = null,
    decimal? goalAmount = null,
    string? goalCurrency = null,
    DateTimeOffset? endDate = null,
    Guid? categoryId = null,
    byte[]? rowVersion = null) =>
    new(
        id ?? Guid.NewGuid(),
        titleAr ?? "حملة محدثة",
        titleEn ?? "Updated Campaign",
        descriptionAr ?? "وصف محدث",
        descriptionEn ?? "Updated description",
        goalAmount ?? 2500m,
        goalCurrency ?? "EGP",
        endDate ?? DateTimeOffset.UtcNow.AddDays(30),
        categoryId ?? Guid.NewGuid(),
        rowVersion ?? Array.Empty<byte>());

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

    [Fact]
    public void Validate_EmptyIdAndInvalidGoal_FailsWithExpectedErrors()
    {
        // Arrange
        var command = CreateValidCommand(
            id: Guid.Empty,
            goalAmount: 0m,
            categoryId: Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.GoalAmount);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_MissingTitles_FailsForTitles(string invalidTitle)
    {
        // Arrange
        var command = CreateValidCommand(
            titleAr: invalidTitle,
            titleEn: invalidTitle);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.TitleAr);
        result.ShouldHaveValidationErrorFor(x => x.TitleEn);
    }

    [Theory]
    [InlineData("EG")]
    [InlineData("USDX")]
    public void Validate_InvalidGoalCurrency_FailsForGoalCurrency(string invalidCurrency)
    {
        // Arrange
        var command = CreateValidCommand(goalCurrency: invalidCurrency);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.GoalCurrency);
    }
}