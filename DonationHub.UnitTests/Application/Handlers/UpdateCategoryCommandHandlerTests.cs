using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Features.Categories.Commands.UpdateCategory;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DonationHub.UnitTests.Application.Handlers;

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Category>> _categoryRepo = new();
    private readonly Mock<TimeProvider> _timeProvider = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly DateTimeOffset _fixedNow = new(2026, 11, 24, 12, 0, 0, TimeSpan.Zero);

    public UpdateCategoryCommandHandlerTests()
    {
        _timeProvider.Setup(t => t.GetUtcNow()).Returns(_fixedNow);
    }

    private static UpdateCategoryCommand CreateValidCommand(
        Guid? id = null,
        string nameAr = "التعليم المحدث",
        string nameEn = "Updated Education",
        string descriptionAr = "وصف محدث",
        string descriptionEn = "Updated description") =>
        new(
            id ?? Guid.NewGuid(),
            nameAr,
            nameEn,
            descriptionAr,
            descriptionEn);

    [Fact]
    public async Task Handle_UnknownCategoryId_ReturnsNotFoundAndDoesNotSave()
    {
        // Arrange
        var command = CreateValidCommand();
        _categoryRepo
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var handler = new UpdateCategoryCommandHandler(
            _categoryRepo.Object,
            _timeProvider.Object,
            _unitOfWork.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.NotFound);
        _categoryRepo.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWork.Verify(u => u.CompeleteAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesCategoryAndCallsUnitOfWorkOnce()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = Category.Create(
            "التعليم",
            "Education",
            "وصف قديم",
            "Old description",
            _fixedNow.AddDays(-5));

        var command = CreateValidCommand(
            id: categoryId,
            nameAr: "التعليم الأساسي",
            nameEn: "Basic Education",
            descriptionAr: "وصف جديد",
            descriptionEn: "New description");

        _categoryRepo
            .Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _unitOfWork
            .Setup(u => u.CompeleteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateCategoryCommandHandler(
            _categoryRepo.Object,
            _timeProvider.Object,
            _unitOfWork.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingCategory.NameAr.Should().Be("التعليم الأساسي");
        existingCategory.NameEn.Should().Be("Basic Education");
        existingCategory.DescriptionAr.Should().Be("وصف جديد");
        existingCategory.DescriptionEn.Should().Be("New description");
        existingCategory.UpdatedAt.Should().Be(_fixedNow);

        _categoryRepo.Verify(r => r.Update(existingCategory), Times.Once);
        _unitOfWork.Verify(u => u.CompeleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidNameInCommand_ThrowsArgumentNullExceptionBeforeSaving()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = Category.Create(
            "التعليم",
            "Education",
            "وصف",
            "Description",
            _fixedNow.AddDays(-2));

        var command = CreateValidCommand(id: categoryId, nameAr: "");

        _categoryRepo
            .Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        var handler = new UpdateCategoryCommandHandler(
            _categoryRepo.Object,
            _timeProvider.Object,
            _unitOfWork.Object);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
        _categoryRepo.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWork.Verify(u => u.CompeleteAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}