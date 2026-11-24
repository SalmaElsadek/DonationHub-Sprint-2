using DonationHub.Application.Features.Categories.Commands.UpdateCategory;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace DonationHub.UnitTests.Application.Validators;

public class UpdateCategoryCommandValidatorTests
{
    private readonly UpdateCategoryCommandValidator _validator = new();

    private static UpdateCategoryCommand CreateValidCommand(
        Guid? id = null,
        string? nameAr = null,
        string? nameEn = null,
        string? descriptionAr = null,
        string? descriptionEn = null) =>
        new(
            id ?? Guid.NewGuid(),
            nameAr ?? "الصحة",
            nameEn ?? "Health",
            descriptionAr ?? "وصف محدث للفئة",
            descriptionEn ?? "Updated category description");

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
    public void Validate_EmptyId_FailsForId()
    {
        // Arrange
        var command = CreateValidCommand(id: Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id);
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
        var command = CreateValidCommand(nameAr: new string('ي', 151));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.NameAr);
    }
}