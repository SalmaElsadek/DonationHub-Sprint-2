using DonationHub.Application.Features.Categories.Commands.CreateCategory;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace DonationHub.UnitTests.Application.Validators;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    private static CreateCategoryCommand CreateValidCommand(
        string? nameAr = null,
        string? nameEn = null,
        string? descriptionAr = null,
        string? descriptionEn = null) =>
        new(
            nameAr ?? "التعليم",
            nameEn ?? "Education",
            descriptionAr ?? "وصف الفئة",
            descriptionEn ?? "Category description");

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
    [InlineData("", "Valid")]
    [InlineData("   ", "Valid")]
    [InlineData("صالح", "")]
    [InlineData("صالح", "   ")]
    public void Validate_MissingNames_FailsForExpectedName(string nameAr, string nameEn)
    {
        // Arrange
        var command = CreateValidCommand(nameAr: nameAr, nameEn: nameEn);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        if (string.IsNullOrWhiteSpace(nameAr))
            result.ShouldHaveValidationErrorFor(x => x.NameAr);
        if (string.IsNullOrWhiteSpace(nameEn))
            result.ShouldHaveValidationErrorFor(x => x.NameEn);
    }

    [Fact]
    public void Validate_NameExceeding150Characters_FailsForName()
    {
        // Arrange
        var command = CreateValidCommand(nameEn: new string('x', 151));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.NameEn);
    }
}