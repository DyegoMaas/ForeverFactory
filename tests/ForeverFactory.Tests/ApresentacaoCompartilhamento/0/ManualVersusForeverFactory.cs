using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Extensions;
using ForeverFactory.Tests.ApresentacaoCompartilhamento.Models;
using Xunit;

namespace ForeverFactory.Tests.ApresentacaoCompartilhamento;

public class ManualVersusForeverFactory
{
    [Fact]
    public void os_objetos_gerados_devem_ser_iguais()
    {
        var inserirComboRequestA = new InserirComboRequestBuilder().Construir();
        var inserirComboRequestB = new InserirComboRequestFactory().Build();
        
        inserirComboRequestA.Should().BeEquivalentTo(inserirComboRequestB);
    }
    
    [Fact]
    public void os_objetos_gerados_devem_ser_iguais2()
    {
        var inserirComboRequestA = new InserirComboRequestBuilder()
            .ComProdutos(
                new ProdutoComboSolicitadoDto {Codigo = 11, Quantidade = 1, Preco = 11.11d},
                new ProdutoComboSolicitadoDto {Codigo = 22, Quantidade = 2, Preco = 22.22d},
                new ProdutoComboSolicitadoDto {Codigo = 33, Quantidade = 3, Preco = 33.33d}
            )
            .ComClientes(
                new ClienteComboSolicitadoDto {CodigoCliente = 11111, CodigoFilial = 111},
                new ClienteComboSolicitadoDto {CodigoCliente = 22222, CodigoFilial = 111},
                new ClienteComboSolicitadoDto {CodigoCliente = 33333, CodigoFilial = 111}
            )
            .Construir();

        var inserirComboRequestB = new InserirComboRequestFactory2()
            .With(x => x.ComboSolicitado = new ComboSolicitadoDtoFactory()
                .With(combo => combo.Produtos = new []
                {
                    new ProdutoComboSolicitadoDto {Codigo = 11, Quantidade = 1, Preco = 11.11d},
                    new ProdutoComboSolicitadoDto {Codigo = 22, Quantidade = 2, Preco = 22.22d},
                    new ProdutoComboSolicitadoDto {Codigo = 33, Quantidade = 3, Preco = 33.33d}
                })
                .With(combo => combo.ClientesFiliais = new []
                {
                    new ClienteComboSolicitadoDto {CodigoCliente = 11111, CodigoFilial = 111},
                    new ClienteComboSolicitadoDto {CodigoCliente = 22222, CodigoFilial = 111},
                    new ClienteComboSolicitadoDto {CodigoCliente = 33333, CodigoFilial = 111}
                })
                .Build()
            )
            .Build();
        
        inserirComboRequestA.Should().BeEquivalentTo(inserirComboRequestB);
    }
    
    public class InserirComboRequestBuilder
    {
        private Guid? _idArquivoBlob;
        private Guid? _idArquivoCombo;
        private IList<ProdutoComboSolicitadoDto> _produtos;
        private IList<ClienteComboSolicitadoDto> _clientes;

        public InserirComboRequest Construir()
        {
            return new InserirComboRequest
            {
                
                UploadId = _idArquivoBlob?.ToString() ?? "4be1569c-a1aa-4f3e-9456-d6ed4ad987af",
                ArquivoComboId = _idArquivoCombo?.ToString() ?? "7f226d19-c061-4009-bdde-8c6c0c57bd06",
                ComboSolicitado = new ComboSolicitadoDto
                {
                    DataInicioVigencia = 2.December(2021),
                    DataFimVigencia = 31.December(2021),
                    DisponibilidadeDia = 7,
                    DisponibilidadeMes = 31,
                    Titulo = "Titulo Combo 1",
                    Descricao = "Descricao Combo 1",
                    Produtos = _produtos ?? new[]
                    {
                        new ProdutoComboSolicitadoDto
                        {
                            Codigo = 1,
                            Quantidade = 1,
                            Preco = 12.23d,
                        },
                    },
                    ClientesFiliais = _clientes ?? new[]
                    {
                        new ClienteComboSolicitadoDto
                        {
                            CodigoCliente = 1,
                            CodigoFilial = 1,
                        },
                    },
                }
            };
        }

        public InserirComboRequestBuilder ComIdArquivoBlob(Guid idArquivoBlob)
        {
            _idArquivoBlob = idArquivoBlob;
            return this;
        }

        public InserirComboRequestBuilder ComIdArquivoCombo(Guid idArquivoCombo)
        {
            _idArquivoCombo = idArquivoCombo;
            return this;
        }

        public InserirComboRequestBuilder ComProdutos(params ProdutoComboSolicitadoDto[] produtos)
        {
            _produtos = produtos;
            return this;
        }

        public InserirComboRequestBuilder ComClientes(params ClienteComboSolicitadoDto[] clientes)
        {
            _clientes = clientes;
            return this;
        }
    }
}

public class InserirComboRequestFactory : MagicFactory<InserirComboRequest>
{
    protected override void Customize(ICustomizeFactoryOptions<InserirComboRequest> customization)
    {
        customization
            .Set(x => x.UploadId = "4be1569c-a1aa-4f3e-9456-d6ed4ad987af")
            .Set(x => x.ArquivoComboId = "7f226d19-c061-4009-bdde-8c6c0c57bd06")
            .Set(x => x.ComboSolicitado = new ComboSolicitadoDto
            {
                DataInicioVigencia = 2.December(2021),
                DataFimVigencia = 31.December(2021),
                DisponibilidadeDia = 7,
                DisponibilidadeMes = 31,
                Titulo = "Titulo Combo 1",
                Descricao = "Descricao Combo 1",
                Produtos = new[]
                {
                    new ProdutoComboSolicitadoDto
                    {
                        Codigo = 1,
                        Quantidade = 1,
                        Preco = 12.23d,
                    },
                },
                ClientesFiliais = new[]
                {
                    new ClienteComboSolicitadoDto
                    {
                        CodigoCliente = 1,
                        CodigoFilial = 1,
                    },
                },
            });
    }
}

public class InserirComboRequestFactory2 : MagicFactory<InserirComboRequest>
{
    protected override void Customize(ICustomizeFactoryOptions<InserirComboRequest> customization)
    {
        customization
            .Set(x => x.UploadId = "4be1569c-a1aa-4f3e-9456-d6ed4ad987af")
            .Set(x => x.ArquivoComboId = "7f226d19-c061-4009-bdde-8c6c0c57bd06")
            .Set(x => x.ComboSolicitado = new ComboSolicitadoDtoFactory().Build());
    }
}

public class ComboSolicitadoDtoFactory : MagicFactory<ComboSolicitadoDto>
{
    protected override void Customize(ICustomizeFactoryOptions<ComboSolicitadoDto> customization)
    {
        customization
            .Set(x => x.DataInicioVigencia = 2.December(2021))
            .Set(x => x.DataFimVigencia = 31.December(2021))
            .Set(x => x.DisponibilidadeDia = 7)
            .Set(x => x.DisponibilidadeMes = 31)
            .Set(x => x.Titulo = "Titulo Combo 1")
            .Set(x => x.Descricao = "Descricao Combo 1")
            .Set(x => x.Produtos = new[]
            {
                new ProdutoComboSolicitadoDto
                {
                    Codigo = 1,
                    Quantidade = 1,
                    Preco = 12.23d,
                },
            })
            .Set(x => x.ClientesFiliais = new[]
            {
                new ClienteComboSolicitadoDto
                {
                    CodigoCliente = 1,
                    CodigoFilial = 1,
                },
            });
    }
}



