using ContratacaoService.Application.DTOs;
using MediatR;

namespace ContratacaoService.Application.UseCases.ListarContratos;

public class ListarContratosQuery : IRequest<IEnumerable<ContratoDto>>
{
}
