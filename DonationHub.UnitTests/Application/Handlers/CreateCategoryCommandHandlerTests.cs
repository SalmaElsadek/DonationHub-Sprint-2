using AutoMapper;
using DonationHub.Application.Common.Results;
using DonationHub.Application.DTOs;
using DonationHub.Application.Features.Categories.Commands.CreateCategory;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DonationHub.UnitTests.Application.Handlers;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Category>> _categoryRepo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<TimeProvider> _timeProvider = new();

    private readonly DateTimeOffset _fixedNow = new(2026, 11, 24, 12, 0, 0, TimeSpan.Zero);

    public CreateCategoryCommandHandlerTests()
    {
        _timeProvider.Setup(t => t.GetUtcNow()).Returns(_fixedNow);
    }

    private static CreateCategoryCommand CreateValidCommand(
        string nameAr = "الرعاية الصحية",
        string nameEn = "Healthcare",
        string descriptionAr = "وصف الفئة",
        string descriptionEn = "Category description") =>
        new(
            nameAr,
            nameEn,
            descriptionAr,
            descriptionEn);

    [Fact]
    public async Task Handle_ValidCommand_AddsCategoryCompletesUnitOfWorkAndReturnsMappedDto()
    {
        // Arrange
        var command = CreateValidCommand();
        var mappedDto = new CategoryDTO
        {
            NameAr = command.NameAr,
            NameEn = command.NameEn
        };

        _categoryRepo
            .Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category c, CancellationToken _) => c);

        _unitOfWork
            .Setup(u => u.CompeleteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapper
            .Setup(m => m.Map<CategoryDTO>(It.IsAny<Category>()))
            .Returns(mappedDto);

        var handler = new CreateCategoryCommandHandler(
            _categoryRepo.Object,
            _mapper.Object,
            _unitOfWork.Object,
            _timeProvider.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.NameAr.Should().Be(command.NameAr);
        result.Value.NameEn.Should().Be(command.NameEn);

        _categoryRepo.Verify(
            r => r.AddAsync(
                It.Is<Category>(c =>
                    c.NameAr == command.NameAr &&
                    c.NameEn == command.NameEn &&
                    c.UpdatedAt == _fixedNow),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            u => u.CompeleteAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _mapper.Verify(
            m => m.Map<CategoryDTO>(It.IsAny<Category>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidName_ThrowsArgumentNullExceptionAndDoesNotSave()
    {
        // Arrange
        var command = CreateValidCommand(nameAr: "");

        var handler = new CreateCategoryCommandHandler(
            _categoryRepo.Object,
            _mapper.Object,
            _unitOfWork.Object,
            _timeProvider.Object);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
        _categoryRepo.Verify(
            r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWork.Verify(
            u => u.CompeleteAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}