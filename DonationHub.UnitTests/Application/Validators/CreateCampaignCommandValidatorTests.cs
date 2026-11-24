using DonationHub.Application.Features.Campaigns.Commands.CreateCampaign;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace DonationHub.UnitTests.Application.Validators;

public class CreateCampaignCommandValidatorTests
{
    private readonly CreateCampaignCommandValidator _validator = new();

    private static CreateCampaignCommand CreateValidCommand(
     string? titleAr = null,
     string? titleEn = null,
     string? descriptionAr = null,
     string? descriptionEn = null,
     decimal? goalAmount = null,
     string? goalCurrency = null,
     DateTimeOffset? endDate = null,
     Guid? categoryId = null,
     Guid? createdByUserId = null,
     string? bannerFileName = null) =>
     new(
         titleAr ?? "حملة تعليمية",
         titleEn ?? "Education Campaign",
         descriptionAr ?? "وصف الحملة التعليمية",
         descriptionEn ?? "Education campaign description",
         goalAmount ?? 5000m,
         goalCurrency ?? "EGP",
         endDate ?? DateTimeOffset.UtcNow.AddDays(30),
         categoryId ?? Guid.NewGuid(),
         createdByUserId ?? Guid.NewGuid(),
         bannerFileName);

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
    public void Validate_MissingTitlesGoalZeroAndEmptyCategory_FailsWithExpectedErrors()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            TitleAr = "",
            TitleEn = "   ",
            GoalAmount = 0m,
            CategoryId = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.TitleAr);
        result.ShouldHaveValidationErrorFor(x => x.TitleEn);
        result.ShouldHaveValidationErrorFor(x => x.GoalAmount);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Theory]
    [InlineData("EG")]
    [InlineData("EGYP")]
    public void Validate_InvalidGoalCurrencyLength_FailsForGoalCurrency(string invalidCurrency)
    {
        // Arrange
        var command = CreateValidCommand() with { GoalCurrency = invalidCurrency };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.GoalCurrency);
    }

    [Fact]
    public void Validate_TitleExceeding200Characters_FailsForTitle()
    {
        // Arrange
        var command = CreateValidCommand() with { TitleEn = new string('a', 201) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.TitleEn);
    }
}