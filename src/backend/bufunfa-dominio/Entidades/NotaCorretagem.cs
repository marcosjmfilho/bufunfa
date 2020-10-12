using JNogueira.Bufunfa.Dominio.Comandos;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JNogueira.Bufunfa.Dominio.Entidades
{
    /// <summary>
    /// Classe que representa uma nota de corretagem
    /// </summary>
    public class NotaCorretagem
    {
        /// <summary>
        /// Id da nota
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Id do usuário proprietário
        /// </summary>
        public int IdUsuario { get; private set; }

        /// <summary>
        /// Id da conta
        /// </summary>
        public int IdConta { get; private set; }

        /// <summary>
        /// Data do pregão
        /// </summary>
        public DateTime DataPregao { get; private set; }

        /// <summary>
        /// Número da nota
        /// </summary>
        public string Numero { get; private set; }

        /// <summary>
        /// Valor da taxa de liquidação
        /// </summary>
        public decimal? ValorTaxaLiquidacao { get; private set; }

        /// <summary>
        /// Valor da taxa de registro
        /// </summary>
        public decimal? ValorTaxaRegistro { get; private set; }

        /// <summary>
        /// Valor da taxa do termo
        /// </summary>
        public decimal? ValorTaxaTermo { get; private set; }

        /// <summary>
        /// Valor da taxa A.N.A
        /// </summary>
        public decimal? ValorTaxaAna { get; private set; }

        /// <summary>
        /// Valor dos emolumentos
        /// </summary>
        public decimal? ValorEmolumentos { get; private set; }

        /// <summary>
        /// Valor da taxa de corretagem
        /// </summary>
        public decimal? ValorTaxaCorretagem { get; private set; }

        /// <summary>
        /// Valor do ISS
        /// </summary>
        public decimal? ValorIss { get; private set; }

        /// <summary>
        /// Valor do IRRF
        /// </summary>
        public decimal? ValorIrrf { get; private set; }

        /// <summary>
        /// Valor outras taxas
        /// </summary>
        public decimal? ValorOutrasTaxas { get; private set; }

        /// <summary>
        /// Observação da nota
        /// </summary>
        public string Observacao { get; private set; }

        /// <summary>
        /// Conta associada nota
        /// </summary>
        public Conta Conta { get; private set; }

        /// <summary>
        /// Lançamentos
        /// </summary>
        public IEnumerable<Lancamento> Lancamentos { get; private set; }

        private NotaCorretagem()
        {
            this.Lancamentos = new List<Lancamento>();
        }

        public NotaCorretagem(NotaCorretagemEntrada entrada)
            : this()
        {
            if (entrada.Invalido)
                return;

            this.DataPregao          = entrada.DataPregao;
            this.IdConta             = entrada.IdConta;
            this.IdUsuario           = entrada.IdUsuario;
            this.Numero              = entrada.Numero;
            this.Observacao          = entrada.Observacao;
            this.ValorEmolumentos    = entrada.ValorEmolumentos;
            this.ValorIrrf           = entrada.ValorIrrf;
            this.ValorIss            = entrada.ValorIss;
            this.ValorOutrasTaxas    = entrada.ValorOutrasTaxas;
            this.ValorTaxaAna        = entrada.ValorTaxaAna;
            this.ValorTaxaCorretagem = entrada.ValorTaxaCorretagem;
            this.ValorTaxaLiquidacao = entrada.ValorTaxaLiquidacao;
            this.ValorTaxaRegistro   = entrada.ValorTaxaRegistro;
            this.ValorTaxaTermo      = entrada.ValorTaxaTermo;

            this.Lancamentos = CriarLancamentos(entrada);
        }

        public void Alterar(NotaCorretagemEntrada entrada)
        {
            if (entrada.Invalido)
                return;

            this.DataPregao = entrada.DataPregao;
            this.IdConta = entrada.IdConta;
            this.IdUsuario = entrada.IdUsuario;
            this.Numero = entrada.Numero;
            this.Observacao = entrada.Observacao;
            this.ValorEmolumentos = entrada.ValorEmolumentos;
            this.ValorIrrf = entrada.ValorIrrf;
            this.ValorIss = entrada.ValorIss;
            this.ValorOutrasTaxas = entrada.ValorOutrasTaxas;
            this.ValorTaxaAna = entrada.ValorTaxaAna;
            this.ValorTaxaCorretagem = entrada.ValorTaxaCorretagem;
            this.ValorTaxaLiquidacao = entrada.ValorTaxaLiquidacao;
            this.ValorTaxaRegistro = entrada.ValorTaxaRegistro;
            this.ValorTaxaTermo = entrada.ValorTaxaTermo;

            this.Lancamentos = CriarLancamentos(entrada);
        }

        private IEnumerable<Lancamento> CriarLancamentos(NotaCorretagemEntrada entrada)
        {
            var lancamentos = new List<Lancamento>();

            // Cria os lançamentos associados ao ativo referente a compra.
            foreach (var lancamento in entrada.Lancamentos.Where(x => x.TipoNegociacao == TipoNegociacaoAtivo.Compra))
            {
                lancamentos.Add(new Lancamento(
                    entrada.IdUsuario,
                    lancamento.IdAtivo,
                    (int)TipoCategoriaEspecial.CompraAcoes,
                    entrada.DataPregao,
                    lancamento.Quantidade * lancamento.ValorPrecoUnitario,
                    lancamento.Quantidade,
                    lancamento.Observacao));
            }

            // Cria os lançamentos associados ao ativo referente a venda.
            foreach (var lancamento in entrada.Lancamentos.Where(x => x.TipoNegociacao == TipoNegociacaoAtivo.Venda))
            {
                lancamentos.Add(new Lancamento(
                    entrada.IdUsuario,
                    lancamento.IdAtivo,
                    (int)TipoCategoriaEspecial.VendaAcoes,
                    entrada.DataPregao,
                    lancamento.Quantidade * lancamento.ValorPrecoUnitario,
                    lancamento.Quantidade,
                    lancamento.Observacao));
            }

            // Cria o lançamento de débito para a conta, referente as operações de compra de ativos.
            if (entrada.Lancamentos.Any(x => x.TipoNegociacao == TipoNegociacaoAtivo.Compra))
            {
                lancamentos.Add(new Lancamento(
                    entrada.IdUsuario,
                    entrada.IdConta,
                    (int)TipoCategoriaEspecial.NotaCorretagemCompraAtivos,
                    entrada.DataPregao,
                    entrada.Lancamentos.Where(x => x.TipoNegociacao == TipoNegociacaoAtivo.Compra).Sum(x => x.Quantidade * x.ValorPrecoUnitario),
                    null,
                    $"Número da nota: {entrada.Numero}"));
            }

            // Cria o lançamento de crédito para a conta, referente as operações de venda de ativos.
            if (entrada.Lancamentos.Any(x => x.TipoNegociacao == TipoNegociacaoAtivo.Venda))
            {
                lancamentos.Add(new Lancamento(
                    entrada.IdUsuario,
                    entrada.IdConta,
                    (int)TipoCategoriaEspecial.NotaCorretagemVendaAtivos,
                    entrada.DataPregao,
                    entrada.Lancamentos.Where(x => x.TipoNegociacao == TipoNegociacaoAtivo.Venda).Sum(x => x.Quantidade * x.ValorPrecoUnitario),
                    null,
                    $"Número da nota: {entrada.Numero}"));
            }

            return lancamentos;
        }
    }
}
