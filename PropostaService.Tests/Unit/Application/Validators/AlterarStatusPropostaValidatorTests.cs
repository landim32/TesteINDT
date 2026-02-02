using FluentAssertions;
using FluentValidation.TestHelper;
using PropostaService.Application.UseCases.AlterarStatusProposta;
using PropostaService.Domain.Enums;
using Xunit;

namespace PropostaService.Tests.Unit.Application.Validators;

public class AlterarStatusPropostaValidatorTests
{
    private readonly AlterarStatusPropostaValidator _validator;

    public AlterarStatusPropostaValidatorTests()
    {
        _validator = new AlterarStatusPropostaValidator();
    }

    [Fact]
    public void Validar_ComDadosValidos_NaoDeveTerErros()
    {
        var command = new AlterarStatusPropostaCommand(
            Guid.NewGuid(),
            StatusProposta.Aprovada
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_ComPropostaIdVazio_DeveTerErro()
    {
        var command = new AlterarStatusPropostaCommand(
            Guid.Empty,
            StatusProposta.Aprovada
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PropostaId)
            .WithErrorMessage("ID da proposta é obrigatório");
    }

    [Theory]
    [InlineData(StatusProposta.EmAnalise)]
    [InlineData(StatusProposta.Aprovada)]
    [InlineData(StatusProposta.Rejeitada)]
    public void Validar_ComStatusValido_NaoDeveTerErro(StatusProposta status)
    {
        var command = new AlterarStatusPropostaCommand(
            Guid.NewGuid(),
            status
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.NovoStatus);
    }

    [Fact]
    public void Validar_ComStatusInvalido_DeveTerErro()
    {
        var command = new AlterarStatusPropostaCommand(
            Guid.NewGuid(),
            (StatusProposta)999
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NovoStatus)
            .WithErrorMessage("Status inválido");
    }
}
