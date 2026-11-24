using DonationHub.Application.Common.Behaviors;
using DonationHub.Application.Common.Results;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using Xunit;

namespace DonationHub.UnitTests.Application.Behaviors;

public class ValidationBehaviorTests
{
    public record SampleCommand(string Name) : IRequest<Result<string>>;

    [Fact]
    public async Task Handle_WhenValidationFails_ReturnsValidationFailedAndNeverCallsNext()
    {
        // Arrange
        var validator = new Mock<IValidator<SampleCommand>>();
        validator
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<SampleCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure(nameof(SampleCommand.Name), "Name is required")
            }));

        var next = new Mock<RequestHandlerDelegate<Result<string>>>();
        var behavior = new ValidationBehavior<SampleCommand, Result<string>>(new[] { validator.Object });

        // Act
        var result = await behavior.Handle(
            new SampleCommand(""),
            next.Object,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Failed");
        result.Error.Type.Should().Be(ErrorType.Validation);
        next.Verify(n => n(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenValidationSucceeds_CallsNextAndReturnsResponse()
    {
        // Arrange
        var validator = new Mock<IValidator<SampleCommand>>();
        validator
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<SampleCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var expected = Result<string>.Success("Done");
        var next = new Mock<RequestHandlerDelegate<Result<string>>>();
        next.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var behavior = new ValidationBehavior<SampleCommand, Result<string>>(new[] { validator.Object });

        // Act
        var result = await behavior.Handle(
            new SampleCommand("Valid"),
            next.Object,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Done");
        next.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoValidatorsRegistered_CallsNextDirectly()
    {
        // Arrange
        var expected = Result<string>.Success("NoValidators");
        var next = new Mock<RequestHandlerDelegate<Result<string>>>();
        next.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var behavior = new ValidationBehavior<SampleCommand, Result<string>>(
            Enumerable.Empty<IValidator<SampleCommand>>());

        // Act
        var result = await behavior.Handle(
            new SampleCommand("Any"),
            next.Object,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("NoValidators");
        next.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }
}