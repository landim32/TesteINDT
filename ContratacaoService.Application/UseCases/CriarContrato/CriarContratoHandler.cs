using AutoMapper;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Saga;
using MediatR;

namespace ContratacaoService.Application.UseCases.CriarContrato;

public class CriarContratoHandler : IRequestHandler<CriarContratoCommand, ContratoDto>
{
    private readonly ContratacaoSaga _contratacaoSaga;
    private readonly IMapper _mapper;

    public CriarContratoHandler(
        ContratacaoSaga contratacaoSaga,
        IMapper mapper)
    {
        _contratacaoSaga = contratacaoSaga;
        _mapper = mapper;
    }

    public async Task<ContratoDto> Handle(CriarContratoCommand request, CancellationToken cancellationToken)
    {
        var contrato = await _contratacaoSaga.ExecutarAsync(request.PropostaId, cancellationToken);
        return _mapper.Map<ContratoDto>(contrato);
    }
}
