using AutoMapper;
using PropostaService.Application.DTOs;
using PropostaService.Domain.Entities;

namespace PropostaService.Application.Mappings;

public class PropostaMappingProfile : Profile
{
    public PropostaMappingProfile()
    {
        CreateMap<Proposta, PropostaDto>()
            .ForMember(dest => dest.NomeCliente, opt => opt.MapFrom(src => src.Cliente.Nome))
            .ForMember(dest => dest.Cpf, opt => opt.MapFrom(src => src.Cliente.Cpf.Valor))
            .ForMember(dest => dest.TipoSeguro, opt => opt.MapFrom(src => src.Seguro.Tipo.Valor))
            .ForMember(dest => dest.ValorCobertura, opt => opt.MapFrom(src => src.Seguro.ValorCobertura.Valor))
            .ForMember(dest => dest.ValorPremio, opt => opt.MapFrom(src => src.Seguro.ValorPremio.Valor));
    }
}
