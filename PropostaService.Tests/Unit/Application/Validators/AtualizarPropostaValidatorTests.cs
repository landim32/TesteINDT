using FluentAssertions;
using FluentValidation.TestHelper;
using PropostaService.Application.UseCases.AtualizarProposta;
using Xunit;

namespace PropostaService.Tests.Unit.Application.Validators;

public class AtualizarPropostaValidatorTests
{
    private readonly AtualizarPropostaValidator _validator;

    public AtualizarPropostaValidatorTests()
    {
        _validator = new AtualizarPropostaValidator();
    }

    [Fact]
    public void Validar_ComDadosValidos_NaoDeveTerErros()
    {
        var command = new AtualizarPropostaCommand(
            Guid.NewGuid(),
            "João da Silva",
            50000,
            1000
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_ComPropostaIdVazio_DeveTerErro()
    {
        var command = new AtualizarPropostaCommand(
            Guid.Empty,
            "João da Silva",
            null,
            null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PropostaId)
            .WithErrorMessage("ID da proposta é obrigatório");
    }

    [Fact]
    public void Validar_ComNomeClienteMuitoCurto_DeveTerErro()
    {
        var command = new AtualizarPropostaCommand(
            Guid.NewGuid(),
            "Jo",
            null,
            null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NomeCliente)
            .WithErrorMessage("Nome do cliente deve ter no mínimo 3 caracteres");
    }

    [Fact]
    public void Validar_ComNomeClienteMuitoLongo_DeveTerErro()
    {
        var command = new AtualizarPropostaCommand(
            Guid.NewGuid(),
            new string('A', 201),
            null,
            null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NomeCliente)
            .WithErrorMessage("Nome do cliente deve ter no máximo 200 caracteres");
    }

    [Fact]
    public void Validar_SemNomeCliente_NaoDeveTerErro()
    {
        var command = new AtualizarPropostaCommand(
            Guid.NewGuid(),
            null,
            null,
            null
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.NomeCliente);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1000)]
    public void Validar_ComValorCoberturaInvalido_DeveTerErro(decimal valorInvalido)
    {
        var command = new AtualizarPropostaCommand(
            Guid.NewGuid(),
            null,
            valorInvalido,
            null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveAnyValidationError();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Validar_ComValorPremioInvalido_DeveTerErro(decimal valorInvalido)
    {
        var command = new AtualizarPropostaCommand(
            Guid.NewGuid(),
            null,
            null,
            valorInvalido
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Validar_ComValorPremioMaiorQueCobertura_DeveTerErro()
    {
        var command = new AtualizarPropostaCommand(
            Guid.NewGuid(),
            null,
            1000,
            2000
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Valor do prêmio deve ser menor que o valor da cobertura");
    }

    [Fact]
    public void Validar_SemValoresSeguro_NaoDeveTerErro()
    {
        var command = new AtualizarPropostaCommand(
            Guid.NewGuid(),
            "João da Silva",
            null,
            null
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ValorCobertura);
        result.ShouldNotHaveValidationErrorFor(x => x.ValorPremio);
    }

    [Fact]
    public void Validar_ApenasComValorCobertura_NaoDeveTerErroDeComparacao()
    {
        var command = new AtualizarPropostaCommand(
            Guid.NewGuid(),
            null,
            50000,
            null
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Validar_ApenasComValorPremio_NaoDeveTerErroDeComparacao()
    {
        var command = new AtualizarPropostaCommand(
            Guid.NewGuid(),
            null,
            null,
            1000
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x);
    }
}
