using FluentValidation;

namespace ContratacaoService.Application.UseCases.CriarContrato;

public class CriarContratoValidator : AbstractValidator<CriarContratoCommand>
{
    public CriarContratoValidator()
    {
        RuleFor(x => x.PropostaId)
            .NotEmpty()
            .WithMessage("O ID da proposta é obrigatório")
            .NotEqual(Guid.Empty)
            .WithMessage("O ID da proposta não pode ser vazio");
    }
}
