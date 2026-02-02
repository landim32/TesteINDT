using FluentValidation;

namespace PropostaService.Application.UseCases.CriarProposta;

public class CriarPropostaValidator : AbstractValidator<CriarPropostaCommand>
{
    public CriarPropostaValidator()
    {
        RuleFor(x => x.NomeCliente)
            .NotEmpty().WithMessage("Nome do cliente é obrigatório")
            .MinimumLength(3).WithMessage("Nome do cliente deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("Nome do cliente deve ter no máximo 200 caracteres");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório")
            .Must(ValidarCpf).WithMessage("CPF inválido");

        RuleFor(x => x.TipoSeguro)
            .NotEmpty().WithMessage("Tipo de seguro é obrigatório")
            .MinimumLength(3).WithMessage("Tipo de seguro deve ter no mínimo 3 caracteres")
            .MaximumLength(50).WithMessage("Tipo de seguro deve ter no máximo 50 caracteres");

        RuleFor(x => x.ValorCobertura)
            .GreaterThan(0).WithMessage("Valor da cobertura deve ser maior que zero");

        RuleFor(x => x.ValorPremio)
            .GreaterThan(0).WithMessage("Valor do prêmio deve ser maior que zero");

        RuleFor(x => x)
            .Must(x => x.ValorPremio < x.ValorCobertura)
            .WithMessage("Valor do prêmio deve ser menor que o valor da cobertura");
    }

    private bool ValidarCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var cpfLimpo = cpf.Replace(".", "").Replace("-", "").Trim();

        if (cpfLimpo.Length != 11)
            return false;

        if (!cpfLimpo.All(char.IsDigit))
            return false;

        if (cpfLimpo.Distinct().Count() == 1)
            return false;

        return true;
    }
}
