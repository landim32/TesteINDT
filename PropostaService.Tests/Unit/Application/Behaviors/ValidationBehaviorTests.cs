using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using PropostaService.Application.Behaviors;
using Xunit;

namespace PropostaService.Tests.Unit.Application.Behaviors;

public class ValidationBehaviorTests
{
    public class TestRequest : IRequest<string>
    {
        public string Value { get; set; } = string.Empty;
    }

    [Fact]
    public async Task Handle_SemValidadores_DeveChamarProximoHandler()
    {
        var validators = new List<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest { Value = "test" };
        var nextCalled = false;
        
        RequestHandlerDelegate<string> next = () =>
        {
            nextCalled = true;
            return Task.FromResult("success");
        };

        var result = await behavior.Handle(request, next, CancellationToken.None);

        result.Should().Be("success");
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ComValidadorValido_DeveChamarProximoHandler()
    {
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest { Value = "test" };
        var nextCalled = false;
        
        RequestHandlerDelegate<string> next = () =>
        {
            nextCalled = true;
            return Task.FromResult("success");
        };

        var result = await behavior.Handle(request, next, CancellationToken.None);

        result.Should().Be("success");
        nextCalled.Should().BeTrue();
        validatorMock.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ComValidadorInvalido_DeveLancarValidationException()
    {
        var validationFailure = new ValidationFailure("Value", "Value is required");
        var validationResult = new ValidationResult(new[] { validationFailure });
        
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest { Value = "" };
        
        RequestHandlerDelegate<string> next = () => Task.FromResult("success");

        var action = async () => await behavior.Handle(request, next, CancellationToken.None);

        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ComMultiplosValidadores_DeveValidarTodos()
    {
        var validator1Mock = new Mock<IValidator<TestRequest>>();
        validator1Mock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var validator2Mock = new Mock<IValidator<TestRequest>>();
        validator2Mock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var validators = new List<IValidator<TestRequest>> { validator1Mock.Object, validator2Mock.Object };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest { Value = "test" };
        
        RequestHandlerDelegate<string> next = () => Task.FromResult("success");

        await behavior.Handle(request, next, CancellationToken.None);

        validator1Mock.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
        validator2Mock.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
