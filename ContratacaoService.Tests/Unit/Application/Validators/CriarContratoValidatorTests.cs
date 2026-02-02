using FluentAssertions;
using FluentValidation.TestHelper;
using ContratacaoService.Application.UseCases.CriarContrato;
using Xunit;

namespace ContratacaoService.Tests.Unit.Application.Validators;

public class CriarContratoValidatorTests
{
    private readonly CriarContratoValidator _validator;

    public CriarContratoValidatorTests()
    {
        _validator = new CriarContratoValidator();
    }

    [Fact]
    public void Validar_ComPropostaIdValido_NaoDeveTerErros()
    {
        var command = new CriarContratoCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_ComPropostaIdVazio_DeveTerErro()
    {
        var command = new CriarContratoCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PropostaId)
            .WithErrorMessage("O ID da proposta não pode ser vazio");
    }
}
