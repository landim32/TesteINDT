using FluentAssertions;
using FluentValidation.TestHelper;
using PropostaService.Application.UseCases.CriarProposta;
using Xunit;

namespace PropostaService.Tests.Unit.Application.Validators;

public class CriarPropostaValidatorTests
{
    private readonly CriarPropostaValidator _validator;

    public CriarPropostaValidatorTests()
    {
        _validator = new CriarPropostaValidator();
    }

    [Fact]
    public void Validar_ComDadosValidos_NaoDeveTerErros()
    {
        var command = new CriarPropostaCommand(
            "João da Silva",
            "12345678909",
            "Auto",
            50000,
            1000
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validar_ComNomeClienteVazio_DeveTerErro(string nomeInvalido)
    {
        var command = new CriarPropostaCommand(
            nomeInvalido,
            "12345678909",
            "Auto",
            50000,
            1000
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NomeCliente)
            .WithErrorMessage("Nome do cliente é obrigatório");
    }

    [Fact]
    public void Validar_ComNomeClienteMuitoCurto_DeveTerErro()
    {
        var command = new CriarPropostaCommand(
            "Jo",
            "12345678909",
            "Auto",
            50000,
            1000
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NomeCliente)
            .WithErrorMessage("Nome do cliente deve ter no mínimo 3 caracteres");
    }

    [Fact]
    public void Validar_ComNomeClienteMuitoLongo_DeveTerErro()
    {
        var command = new CriarPropostaCommand(
            new string('A', 201),
            "12345678909",
            "Auto",
            50000,
            1000
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NomeCliente)
            .WithErrorMessage("Nome do cliente deve ter no máximo 200 caracteres");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validar_ComCpfVazio_DeveTerErro(string cpfInvalido)
    {
        var command = new CriarPropostaCommand(
            "João da Silva",
            cpfInvalido,
            "Auto",
            50000,
            1000
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Cpf);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345678901234")]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    public void Validar_ComCpfInvalido_DeveTerErro(string cpfInvalido)
    {
        var command = new CriarPropostaCommand(
            "João da Silva",
            cpfInvalido,
            "Auto",
            50000,
            1000
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("CPF inválido");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validar_ComTipoSeguroVazio_DeveTerErro(string tipoInvalido)
    {
        var command = new CriarPropostaCommand(
            "João da Silva",
            "12345678909",
            tipoInvalido,
            50000,
            1000
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TipoSeguro)
            .WithErrorMessage("Tipo de seguro é obrigatório");
    }

    [Fact]
    public void Validar_ComTipoSeguroMuitoCurto_DeveTerErro()
    {
        var command = new CriarPropostaCommand(
            "João da Silva",
            "12345678909",
            "Ab",
            50000,
            1000
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TipoSeguro)
            .WithErrorMessage("Tipo de seguro deve ter no mínimo 3 caracteres");
    }

    [Fact]
    public void Validar_ComTipoSeguroMuitoLongo_DeveTerErro()
    {
        var command = new CriarPropostaCommand(
            "João da Silva",
            "12345678909",
            new string('A', 51),
            50000,
            1000
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TipoSeguro)
            .WithErrorMessage("Tipo de seguro deve ter no máximo 50 caracteres");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1000)]
    public void Validar_ComValorCoberturaInvalido_DeveTerErro(decimal valorInvalido)
    {
        var command = new CriarPropostaCommand(
            "João da Silva",
            "12345678909",
            "Auto",
            valorInvalido,
            1000
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ValorCobertura)
            .WithErrorMessage("Valor da cobertura deve ser maior que zero");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Validar_ComValorPremioInvalido_DeveTerErro(decimal valorInvalido)
    {
        var command = new CriarPropostaCommand(
            "João da Silva",
            "12345678909",
            "Auto",
            50000,
            valorInvalido
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ValorPremio)
            .WithErrorMessage("Valor do prêmio deve ser maior que zero");
    }

    [Fact]
    public void Validar_ComValorPremioMaiorQueCobertura_DeveTerErro()
    {
        var command = new CriarPropostaCommand(
            "João da Silva",
            "12345678909",
            "Auto",
            1000,
            2000
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Valor do prêmio deve ser menor que o valor da cobertura");
    }

    [Fact]
    public void Validar_ComCpfFormatado_NaoDeveTerErro()
    {
        var command = new CriarPropostaCommand(
            "João da Silva",
            "123.456.789-09",
            "Auto",
            50000,
            1000
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Cpf);
    }
}
