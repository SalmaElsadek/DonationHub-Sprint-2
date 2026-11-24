using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Features.Campaigns.Commands.CreateCampaign;
using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DonationHub.UnitTests.Application.Handlers;

public class CreateCampaignCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Campaign>> _campaignRepo = new();
    private readonly Mock<IGenericRepository<Category>> _categoryRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<TimeProvider> _timeProvider = new();

    private readonly DateTimeOffset _fixedNow = new(2026, 11, 24, 12, 0, 0, TimeSpan.Zero);

    public CreateCampaignCommandHandlerTests()
    {
        _timeProvider.Setup(t => t.GetUtcNow()).Returns(_fixedNow);
    }

    private CreateCampaignCommand CreateValidCommand(
        Guid? categoryId = null,
        DateTimeOffset? endDate = null) =>
        new(
            "حملة تعليمية",
            "Education Campaign",
            "وصف الحملة",
            "Campaign description",
            1000m,
            "EGP",
         endDate ?? DateTimeOffset.UtcNow.AddDays(10),
            categoryId ?? Guid.NewGuid(),
            Guid.NewGuid(),
            null);

    [Fact]
    public async Task Handle_UnknownCategory_ReturnsNotFoundAndDoesNotSave()
    {
        // Arrange
        var command = CreateValidCommand();
        _categoryRepo
            .Setup(r => r.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var handler = new CreateCampaignCommandHandler(
            _campaignRepo.Object,
            _categoryRepo.Object,
            _unitOfWork.Object,
            _timeProvider.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.NotFound);
        _campaignRepo.Verify(
            r => r.AddAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWork.Verify(
            u => u.CompeleteAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ValidRequest_AddsCampaignAndCallsUnitOfWorkOnce()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = Category.Create("التعليم", "Education", "وصف", "Desc", _fixedNow);
        var command = CreateValidCommand(categoryId: categoryId, endDate: _fixedNow.AddDays(10));

        _categoryRepo
            .Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _campaignRepo
            .Setup(r => r.AddAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Campaign c, CancellationToken _) => c);

        _unitOfWork
            .Setup(u => u.CompeleteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateCampaignCommandHandler(
            _campaignRepo.Object,
            _categoryRepo.Object,
            _unitOfWork.Object,
            _timeProvider.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.TitleEn.Should().Be(command.TitleEn);
        result.Value.GoalAmount.Should().Be(command.GoalAmount);
        result.Value.DaysLeft.Should().Be(10);

        _campaignRepo.Verify(
            r => r.AddAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWork.Verify(
            u => u.CompeleteAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EndDateInPast_ReturnsEndDateInPastErrorAndDoesNotSave()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = Category.Create("التعليم", "Education", "وصف", "Desc", _fixedNow);
        var command = CreateValidCommand(categoryId: categoryId, endDate: _fixedNow.AddDays(-1));

        _categoryRepo
            .Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        var handler = new CreateCampaignCommandHandler(
            _campaignRepo.Object,
            _categoryRepo.Object,
            _unitOfWork.Object,
            _timeProvider.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CampaignErrors.EndDateInPast);
        _unitOfWork.Verify(
            u => u.CompeleteAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}