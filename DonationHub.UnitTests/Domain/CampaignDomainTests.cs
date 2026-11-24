using DonationHub.Domain.Entities;
using DonationHub.Domain.Enum;
using DonationHub.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace DonationHub.UnitTests.Domain;

public class CampaignDomainTests
{
    private static DateTimeOffset FakeNow(DateTimeOffset fixedUtc)
    {
        var clock = new Mock<TimeProvider>();
        clock.Setup(x => x.GetUtcNow()).Returns(fixedUtc);
        return clock.Object.GetUtcNow();
    }

    private static Campaign CreateValidCampaign(DateTimeOffset now, decimal goal = 1000m)
    {
        return Campaign.Create(
            titleAr: "حملة خير",
            titleEn: "Charity Campaign",
            descriptionAr: "وصف الحملة",
            descriptionEn: "Campaign description",
            goalAmount:Money.Create(goal, "EGP"),
            endDate: now.AddDays(15),
            categoryId: Guid.NewGuid(),
            createdByUserId: Guid.NewGuid(),
            time: now);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void UpdateGoalAmount_ZeroOrNegative_ThrowsArgumentException(decimal invalidGoal)
    {
        // Arrange
        var now = FakeNow(new DateTimeOffset(2026, 11, 24, 12, 0, 0, TimeSpan.Zero));
        var campaign = CreateValidCampaign(now, goal: 1000m);

        // Act
        var act = () => campaign.UpdateGoalAmount(Money.Create(invalidGoal, "EGP"), now.AddHours(1));

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_EndDateInPast_ThrowsArgumentException()
    {
        // Arrange
        var now = FakeNow(new DateTimeOffset(2026, 11, 24, 12, 0, 0, TimeSpan.Zero));
        var pastEndDate = now.AddMinutes(-1);

        // Act
        var act = () => Campaign.Create(
            titleAr: "حملة خير",
            titleEn: "Charity Campaign",
            descriptionAr: "وصف الحملة",
            descriptionEn: "Campaign description",
            goalAmount: Money.Create(1000m, "EGP"),
            endDate: pastEndDate,
            categoryId: Guid.NewGuid(),
            createdByUserId: Guid.NewGuid(),
            time: now);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    //Goal cannot be updated below the raised amount
    [Fact]
    public void UpdateGoalAmount_BelowRaisedAmount_ThrowsArgumentException()
    {
        // Arrange
        var now = FakeNow(new DateTimeOffset(2026, 11, 24, 12, 0, 0, TimeSpan.Zero));
        var campaign = CreateValidCampaign(now, goal: 1000m);
        campaign.AddDonation(400m, now.AddHours(1));

        // Act
        var act = () => campaign.UpdateGoalAmount(Money.Create(300m, "EGP"), now.AddHours(2));

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateGoalAmount_WhenCompleted_ThrowsInvalidOperationException()
    {
        // Arrange
        var now = FakeNow(new DateTimeOffset(2026, 11, 24, 12, 0, 0, TimeSpan.Zero));
        var campaign = CreateValidCampaign(now, goal: 500m);
        campaign.AddDonation(500m, now.AddHours(1)); //Completed

        // Act
        var act = () => campaign.UpdateGoalAmount(Money.Create(800m, "EGP"), now.AddHours(2));

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void UpdateGoalAmount_WhenExpired_ThrowsInvalidOperationException()
    {
        // Arrange
        var now = FakeNow(new DateTimeOffset(2026, 11, 24, 12, 0, 0, TimeSpan.Zero));
        var campaign = CreateValidCampaign(now, goal: 1000m);
        var afterEnd = campaign.EndDate.AddMinutes(1);
        campaign.MakeItExpired(afterEnd);

        // Act
        var act = () => campaign.UpdateGoalAmount(Money.Create(1500m, "EGP"), afterEnd.AddMinutes(1));

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddDonation_ReachingGoal_TransitionsStatusToCompleted()
    {
        // Arrange
        var now = FakeNow(new DateTimeOffset(2026, 11, 24, 12, 0, 0, TimeSpan.Zero));
        var campaign = CreateValidCampaign(now, goal: 1000m);

        // Act
        campaign.AddDonation(1000m, now.AddHours(1));

        // Assert
        campaign.Status.Should().Be(CampaignStatus.Completed);
        campaign.RaisedAmount.Should().Be(1000m);
    }
}