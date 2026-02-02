using FluentValidation;

namespace PropostaService.Application.UseCases.AtualizarProposta;

public class AtualizarPropostaValidator : AbstractValidator<AtualizarPropostaCommand>
{
    public AtualizarPropostaValidator()
    {
        RuleFor(x => x.PropostaId)
            .NotEmpty().WithMessage("ID da proposta é obrigatório");

        When(x => !string.IsNullOrWhiteSpace(x.NomeCliente), () =>
        {
            RuleFor(x => x.NomeCliente!)
                .MinimumLength(3).WithMessage("Nome do cliente deve ter no mínimo 3 caracteres")
                .MaximumLength(200).WithMessage("Nome do cliente deve ter no máximo 200 caracteres");
        });

        When(x => x.ValorCobertura.HasValue, () =>
        {
            RuleFor(x => x.ValorCobertura!.Value)
                .GreaterThan(0).WithMessage("Valor da cobertura deve ser maior que zero");
        });

        When(x => x.ValorPremio.HasValue, () =>
        {
            RuleFor(x => x.ValorPremio!.Value)
                .GreaterThan(0).WithMessage("Valor do prêmio deve ser maior que zero");
        });

        When(x => x.ValorCobertura.HasValue && x.ValorPremio.HasValue, () =>
        {
            RuleFor(x => x)
                .Must(x => x.ValorPremio!.Value < x.ValorCobertura!.Value)
                .WithMessage("Valor do prêmio deve ser menor que o valor da cobertura");
        });
    }
}
