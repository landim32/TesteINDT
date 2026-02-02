using FluentValidation;

namespace PropostaService.Application.UseCases.AlterarStatusProposta;

public class AlterarStatusPropostaValidator : AbstractValidator<AlterarStatusPropostaCommand>
{
    public AlterarStatusPropostaValidator()
    {
        RuleFor(x => x.PropostaId)
            .NotEmpty().WithMessage("ID da proposta é obrigatório");

        RuleFor(x => x.NovoStatus)
            .IsInEnum().WithMessage("Status inválido");
    }
}
