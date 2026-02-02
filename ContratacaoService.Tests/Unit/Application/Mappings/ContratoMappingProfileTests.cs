using AutoMapper;
using FluentAssertions;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Mappings;
using ContratacaoService.Tests.Builders;
using Xunit;

namespace ContratacaoService.Tests.Unit.Application.Mappings;

public class ContratoMappingProfileTests
{
    private readonly IMapper _mapper;

    public ContratoMappingProfileTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ContratoMappingProfile>();
        });

        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void Map_ContratoParaContratoDto_DeveMappearCorretamente()
    {
        var contrato = new ContratoBuilder().Build();

        var contratoDto = _mapper.Map<ContratoDto>(contrato);

        contratoDto.Should().NotBeNull();
        contratoDto.Id.Should().Be(contrato.Id);
        contratoDto.PropostaId.Should().Be(contrato.PropostaId.Value);
        contratoDto.DataContratacao.Should().Be(contrato.DataContratacao);
        contratoDto.Status.Should().Be(contrato.Status);
        contratoDto.DataCriacao.Should().Be(contrato.DataCriacao);
        contratoDto.DataAtualizacao.Should().Be(contrato.DataAtualizacao);
    }

    [Fact]
    public void Map_ContratoCanceladoParaContratoDto_DeveMappearCorretamente()
    {
        var contrato = new ContratoBuilder().BuildCancelado();

        var contratoDto = _mapper.Map<ContratoDto>(contrato);

        contratoDto.Should().NotBeNull();
        contratoDto.Status.Should().Be(contrato.Status);
        contratoDto.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void Map_ListaDeContratosParaListaDeDtos_DeveMappearCorretamente()
    {
        var contratos = new[]
        {
            new ContratoBuilder().Build(),
            new ContratoBuilder().Build()
        };

        var contratosDto = _mapper.Map<ContratoDto[]>(contratos);

        contratosDto.Should().NotBeNull();
        contratosDto.Should().HaveCount(2);
    }

    [Fact]
    public void ConfigurationProfile_DeveSerValido()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ContratoMappingProfile>();
        });

        configuration.AssertConfigurationIsValid();
    }
}
