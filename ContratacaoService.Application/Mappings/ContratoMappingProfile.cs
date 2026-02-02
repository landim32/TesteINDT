using AutoMapper;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Application.Mappings;

public class ContratoMappingProfile : Profile
{
    public ContratoMappingProfile()
    {
        CreateMap<Contrato, ContratoDto>()
            .ForMember(dest => dest.PropostaId, opt => opt.MapFrom(src => src.PropostaId.Value));
    }
}
