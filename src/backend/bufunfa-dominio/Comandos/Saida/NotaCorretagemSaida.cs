using JNogueira.Bufunfa.Dominio.Entidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JNogueira.Bufunfa.Dominio.Comandos
{
    /// <summary>
    /// Comando de saída para as informações de uma nota de corretagem
    /// </summary>
    public class NotaCorretagemSaida
    {
        /// <summary>
        /// Id da nota
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Conta associada a nota
        /// </summary>
        public ContaSaida Conta { get; }

        /// <summary>
        /// Data do pregão
        /// </summary>
        [JsonConverter(typeof(JsonDateFormatConverter), "dd/MM/yyyy")]
        public DateTime DataPregao { get; }

        /// <summary>
        /// Número da nota
        /// </summary>
        public string Numero { get; }

        /// <summary>
        /// Valor da taxa de liquidação
        /// </summary>
        public decimal? ValorTaxaLiquidacao { get; }

        /// <summary>
        /// Valor da taxa de registro
        /// </summary>
        public decimal? ValorTaxaRegistro { get; }

        /// <summary>
        /// Valor da taxa do termo
        /// </summary>
        public decimal? ValorTaxaTermo { get; }

        /// <summary>
        /// Valor da taxa A.N.A
        /// </summary>
        public decimal? ValorTaxaAna { get; }

        /// <summary>
        /// Valor dos emolumentos
        /// </summary>
        public decimal? ValorEmolumentos { get; }

        /// <summary>
        /// Valor da taxa de corretagem
        /// </summary>
        public decimal? ValorTaxaCorretagem { get; }

        /// <summary>
        /// Valor do ISS
        /// </summary>
        public decimal? ValorIss { get; }

        /// <summary>
        /// Valor do IRRF
        /// </summary>
        public decimal? ValorIrrf { get; }

        /// <summary>
        /// Valor outras taxas
        /// </summary>
        public decimal? ValorOutrasTaxas { get; }

        /// <summary>
        /// Observação da nota
        /// </summary>
        public string Observacao { get; }

        /// <summary>
        /// Lançamentos da nota
        /// </summary>
        public IEnumerable<LancamentoSaida> Lancamentos { get; }

        public NotaCorretagemSaida(NotaCorretagem nota)
        {
            if (nota == null)
                return;

            this.Id                  = nota.Id;
            this.DataPregao          = nota.DataPregao;
            this.Numero              = nota.Numero;
            this.ValorTaxaLiquidacao = nota.ValorTaxaLiquidacao;
            this.ValorTaxaRegistro   = nota.ValorTaxaRegistro;
            this.ValorEmolumentos    = nota.ValorEmolumentos;
            this.ValorIrrf           = nota.ValorIrrf;
            this.ValorIss            = nota.ValorIss;
            this.ValorOutrasTaxas    = nota.ValorOutrasTaxas;
            this.ValorTaxaAna        = nota.ValorTaxaAna;
            this.ValorTaxaCorretagem = nota.ValorTaxaCorretagem;
            this.ValorTaxaTermo      = nota.ValorTaxaTermo;
            this.Observacao          = nota.Observacao;
            this.Conta               = new ContaSaida(nota.Conta);
            this.Lancamentos         = nota.Lancamentos.Select(x => new LancamentoSaida(x));
        }

        public NotaCorretagemSaida(
            int id,
            ContaSaida conta,
            DateTime dataPregao,
            string numero,
            decimal? valorTaxaLiquidacao,
            decimal? valorTaxaRegistro,
            decimal? valorTaxaTermo,
            decimal? valorTaxaAna,
            decimal? valorEmolumentos,
            decimal? valorTaxaCorretagem,
            decimal? valorIss,
            decimal? valorIrrf,
            decimal? valorOutrasTaxas,
            string observacao,
            IEnumerable<LancamentoSaida> lancamentos)
        {
            Id                  = id;
            Conta               = conta;
            DataPregao          = dataPregao;
            Numero              = numero;
            ValorTaxaLiquidacao = valorTaxaLiquidacao;
            ValorTaxaRegistro   = valorTaxaRegistro;
            ValorTaxaTermo      = valorTaxaTermo;
            ValorTaxaAna        = valorTaxaAna;
            ValorEmolumentos    = valorEmolumentos;
            ValorTaxaCorretagem = valorTaxaCorretagem;
            ValorIss            = valorIss;
            ValorIrrf           = valorIrrf;
            ValorOutrasTaxas    = valorOutrasTaxas;
            Observacao          = observacao;
            Lancamentos         = lancamentos;
        }

        public override string ToString()
        {
            var descricao = new List<string>
            {
                this.Numero,

                this.DataPregao.ToString("dd/MM/yyyy")
            };

            return string.Join(" » ", descricao);
        }
    }
}
