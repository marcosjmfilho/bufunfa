using JNogueira.Bufunfa.Dominio;
using JNogueira.Bufunfa.Dominio.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JNogueira.Bufunfa.Api.ViewModels
{
    public class NotaCorretagemViewModel
    {
        /// <summary>
        /// Id da conta
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(ContaMensagem), ErrorMessageResourceName = "Id_Conta_Invalido")]
        public int? IdConta { get; set; }

        /// <summary>
        /// Data do lançamento
        /// </summary>
        [Required(ErrorMessage = "A data do pergão é obrigatória e não foi informada.")]
        [JsonConverter(typeof(JsonDateFormatConverter), "dd/MM/yyyy")]
        public DateTime DataPregao { get; set; }

        /// <summary>
        /// Número da nota
        /// </summary>
        public string Numero { get; set; }

        /// <summary>
        /// Valor da taxa de liquidação
        /// </summary>
        public decimal? ValorTaxaLiquidacao { get; set; }

        /// <summary>
        /// Valor da taxa de registro
        /// </summary>
        public decimal? ValorTaxaRegistro { get; set; }

        /// <summary>
        /// Valor da taxa do termo
        /// </summary>
        public decimal? ValorTaxaTermo { get; set; }

        /// <summary>
        /// Valor da taxa A.N.A
        /// </summary>
        public decimal? ValorTaxaAna { get; set; }

        /// <summary>
        /// Valor dos emolumentos
        /// </summary>
        public decimal? ValorEmolumentos { get; set; }

        /// <summary>
        /// Valor da taxa de corretagem
        /// </summary>
        public decimal? ValorTaxaCorretagem { get; set; }

        /// <summary>
        /// Valor do ISS
        /// </summary>
        public decimal? ValorIss { get; set; }

        /// <summary>
        /// Valor do IRRF
        /// </summary>
        public decimal? ValorIrrf { get; set; }

        /// <summary>
        /// Valor outras taxas
        /// </summary>
        public decimal? ValorOutrasTaxas { get; set; }

        /// <summary>
        /// Observação sobre a nota de corretagem
        /// </summary>
        [MaxLength(500, ErrorMessageResourceType = typeof(NotaCorretagemMensagem), ErrorMessageResourceName = "Observacao_Tamanho_Maximo_Excedido")]
        public string Observacao { get; set; }

        /// <summary>
        /// Lançamentos da nota de corretagem
        /// </summary>
        public IEnumerable<LancamentoNotaViewModel> Lancamentos { get; set; }
    }

    public class LancamentoNotaViewModel
    {
        /// <summary>
        /// Id do ativo
        /// </summary>
        public int IdAtivo { get; set; }

        /// <summary>
        /// Quantidade
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Valor do preço unitário
        /// </summary>
        public decimal ValorPrecoUnitario { get; set; }

        /// <summary>
        /// Tipo da negociação (crédito ou débito)
        /// </summary>
        public string TipoNegociacao { get; set; }

        /// <summary>
        /// Observação do lançamento
        /// </summary>
        public string Observacao { get; set; }
    }
}
