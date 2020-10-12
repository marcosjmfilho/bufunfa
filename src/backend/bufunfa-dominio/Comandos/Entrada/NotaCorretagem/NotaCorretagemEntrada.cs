using JNogueira.Bufunfa.Dominio.Resources;
using JNogueira.NotifiqueMe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JNogueira.Bufunfa.Dominio.Comandos
{
    public class NotaCorretagemEntrada : Notificavel
    {
        /// <summary>
        /// Id do usuário
        /// </summary>
        public int IdUsuario { get; }

        /// <summary>
        /// Id da conta
        /// </summary>
        public int IdConta { get; }

        /// <summary>
        /// Data do pregão
        /// </summary>
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
        /// Lançamentos
        /// </summary>
        public IEnumerable<LancamentoNotaEntrada> Lancamentos { get; }

        public NotaCorretagemEntrada(
            int idUsuario,
            int idConta,
            DateTime dataPregao,
            string numero,
            IEnumerable<LancamentoNotaEntrada> lancamentos,
            decimal? valorTaxaLiquidacao = null,
            decimal? valorTaxaRegistro = null,
            decimal? valorTaxaTermo = null,
            decimal? valorTaxaAna = null,
            decimal? valorEmolumentos = null,
            decimal? valorTaxaCorretagem = null,
            decimal? valorIss = null,
            decimal? valorIrrf = null,
            decimal? valorOutrasTaxas = null,
            string observacao = null)
        {
            IdUsuario           = idUsuario;
            IdConta             = idConta;
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

            this.NotificarSeMenorOuIgualA(this.IdUsuario, 0, Mensagem.Id_Usuario_Invalido)
                .NotificarSeMenorOuIgualA(this.IdConta, 0, ContaMensagem.Id_Conta_Invalido)
                .NotificarSeNuloOuVazio(this.Numero, NotaCorretagemMensagem.Numero_Obrigatorio_Nao_Informado)
                .NotificarSeMaiorQue(this.DataPregao, DateTime.Today, NotaCorretagemMensagem.Data_Pregao_Obrigatorio_Nao_Informado)
                .NotificarSeVerdadeiro(this.Lancamentos?.Any() == false, NotaCorretagemMensagem.Lancamentos_Obrigatorios_Nao_Informado)
                .NotificarSeVerdadeiro(!string.IsNullOrEmpty(this.Observacao) && this.Observacao.Length > 500, NotaCorretagemMensagem.Observacao_Tamanho_Maximo_Excedido);

            foreach (var lancamento in Lancamentos)
            {
                this.AdicionarNotificacoes(lancamento.Notificacoes);
            }
        }
    }
}