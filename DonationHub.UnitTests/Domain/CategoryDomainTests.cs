using DonationHub.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DonationHub.UnitTests.Domain;

public class CategoryDomainTests
{
    private static DateTimeOffset FakeNow(DateTimeOffset fixedUtc)
    {
        var clock = new Mock<TimeProvider>();
        clock.Setup(x => x.GetUtcNow()).Returns(fixedUtc);
        return clock.Object.GetUtcNow();
    }

    [Theory]
    [InlineData("", "Valid")]
    [InlineData("   ", "Valid")]
    [InlineData("صالح", "")]
    [InlineData("صالح", "   ")]
    public void Create_MissingName_ThrowsArgumentNullException(string nameAr, string nameEn)
    {
        // Arrange
        var now = FakeNow(new DateTimeOffset(2026, 11, 24, 12, 0, 0, TimeSpan.Zero));

        // Act
        var act = () => Category.Create(nameAr, nameEn, "وصف", "Description", now);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_NameExceedsMaxLength_ThrowsArgumentException()
    {
        // Arrange
        var now = FakeNow(new DateTimeOffset(2026, 11, 24, 12, 0, 0, TimeSpan.Zero));
        var longName = new string('a', 101);

        // Act
        var act = () => Category.Create("صالح", longName, "وصف", "Description", now);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CanBeDeleted_WhenNewCategoryHasNoCampaigns_ReturnsTrue()
    {
        // Arrange
        var now = FakeNow(new DateTimeOffset(2026, 11, 24, 12, 0, 0, TimeSpan.Zero));
        var category = Category.Create("الصحة", "Health", "وصف", "Description", now);

        // Act
        var canDelete = category.CanBeDeleted();

        // Assert
        canDelete.Should().BeTrue();
    }

    [Fact]
    public void UpdateCreate_InvalidUpdatedName_ThrowsException()
    {
        // Arrange
        var now = FakeNow(new DateTimeOffset(2026, 11, 24, 12, 0, 0, TimeSpan.Zero));
        var category = Category.Create("الصحة", "Health", "وصف", "Description", now);

        // Act
        var act = () => category.UpdateCreate("", "Health", "وصف", "Description", now.AddHours(1));

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}