using System;
using System.Collections.Generic;

namespace ForeverFactory.Tests.ApresentacaoCompartilhamento.Models;

public class InserirComboRequest
{
    public string UploadId { get; set; }
    public string ArquivoComboId { get; set; }
    public ComboSolicitadoDto ComboSolicitado { get; set; }
}

public class ComboSolicitadoDto
{
    public DateTime DataInicioVigencia { get; set; }
    public DateTime DataFimVigencia { get; set; }
    public int DisponibilidadeDia { get; set; }
    public int DisponibilidadeMes { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public IList<ProdutoComboSolicitadoDto> Produtos { get; set; }
    public IList<ClienteComboSolicitadoDto> ClientesFiliais { get; set; }
}

public class ProdutoComboSolicitadoDto
{
    public int Codigo { get; set; }
    public int Quantidade { get; set; }
    public double Preco { get; set; }
}

public record ClienteComboSolicitadoDto {
    public int CodigoCliente { get; set; }
    public int CodigoFilial { get; set; }
    public string CodigoUnificado => Cliente.GerarCodigoUnificado(CodigoFilial, CodigoCliente);
}

public class Cliente
{
    public int CodigoFilial { get; set; }
    public int Codigo { get; set; }
    public string Status { get; set; }
    public string DataCadastro { get; set; }
    public string CpfCnpj { get; set; }
        
    public string CodigoUnificado { get; private set; }

    public Cliente(int codigoFilial, int codigo)
    {
        CodigoFilial = codigoFilial;
        Codigo = codigo;
        CodigoUnificado = GerarCodigoUnificado(codigoFilial, codigo);
    }

    public static string GerarCodigoUnificado(int codigoFilial, int codigo)
    {
        return $"{codigoFilial}_{codigo}";
    }
}