using AutoMapper;
using FluentAssertions;
using PropostaService.Application.DTOs;
using PropostaService.Application.Mappings;
using PropostaService.Tests.Builders;
using Xunit;

namespace PropostaService.Tests.Unit.Application.Mappings;

public class PropostaMappingProfileTests
{
    private readonly IMapper _mapper;

    public PropostaMappingProfileTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PropostaMappingProfile>();
        });

        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void Map_PropostaParaPropostaDto_DeveMappearCorretamente()
    {
        var proposta = new PropostaBuilder()
            .ComNomeCliente("João da Silva")
            .ComCpf("12345678909")
            .ComTipoSeguro("Auto")
            .ComValorCobertura(50000)
            .ComValorPremio(1000)
            .Build();

        var propostaDto = _mapper.Map<PropostaDto>(proposta);

        propostaDto.Should().NotBeNull();
        propostaDto.Id.Should().Be(proposta.Id);
        propostaDto.NomeCliente.Should().Be("João da Silva");
        propostaDto.Cpf.Should().Be("12345678909");
        propostaDto.TipoSeguro.Should().Be("Auto");
        propostaDto.ValorCobertura.Should().Be(50000);
        propostaDto.ValorPremio.Should().Be(1000);
        propostaDto.DataCriacao.Should().Be(proposta.DataCriacao);
        propostaDto.Status.Should().Be(proposta.Status);
    }

    [Fact]
    public void Map_PropostaAprovadaParaPropostaDto_DeveMappearCorretamente()
    {
        var proposta = new PropostaBuilder().BuildAprovada();

        var propostaDto = _mapper.Map<PropostaDto>(proposta);

        propostaDto.Should().NotBeNull();
        propostaDto.Status.Should().Be(proposta.Status);
    }

    [Fact]
    public void Map_ListaDePropostasParaListaDeDtos_DeveMappearCorretamente()
    {
        var propostas = new[]
        {
            new PropostaBuilder().ComNomeCliente("João").Build(),
            new PropostaBuilder().ComNomeCliente("Maria").Build()
        };

        var propostasDto = _mapper.Map<PropostaDto[]>(propostas);

        propostasDto.Should().NotBeNull();
        propostasDto.Should().HaveCount(2);
        propostasDto[0].NomeCliente.Should().Be("João");
        propostasDto[1].NomeCliente.Should().Be("Maria");
    }

    [Fact]
    public void ConfigurationProfile_DeveSerValido()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PropostaMappingProfile>();
        });

        configuration.AssertConfigurationIsValid();
    }
}
