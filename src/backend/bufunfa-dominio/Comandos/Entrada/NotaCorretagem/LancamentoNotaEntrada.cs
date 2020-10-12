using JNogueira.Bufunfa.Dominio.Resources;
using JNogueira.NotifiqueMe;

namespace JNogueira.Bufunfa.Dominio.Comandos
{
    public class LancamentoNotaEntrada : Notificavel
    {
        /// <summary>
        /// Id do ativo
        /// </summary>
        public int IdAtivo { get; }

        /// <summary>
        /// Quantidade
        /// </summary>
        public int Quantidade { get; }

        /// <summary>
        /// Valor do preço unitário
        /// </summary>
        public decimal ValorPrecoUnitario { get; }

        /// <summary>
        /// Tipo da negociação (crédito ou débito)
        /// </summary>
        public string TipoNegociacao { get; }

        /// <summary>
        /// Observação do lançamento
        /// </summary>
        public string Observacao { get; }

        public LancamentoNotaEntrada(
            int idAtivo,
            int quantidade,
            decimal valorPrecoUnitario,
            string tipoNegociacao,
            string observacao = null)
        {
            IdAtivo            = idAtivo;
            Quantidade         = quantidade;
            ValorPrecoUnitario = valorPrecoUnitario;
            TipoNegociacao     = tipoNegociacao;
            Observacao         = observacao;

            this.NotificarSeMenorOuIgualA(this.IdAtivo, 0, "O ID do ativo informado é inválido.")
                .NotificarSeMenorOuIgualA(this.Quantidade, 0, "A quantidade informada é inválida.")
                .NotificarSeMenorOuIgualA(this.ValorPrecoUnitario, 0, "O valor do preço unitário informado é inválido.")
                .NotificarSeVerdadeiro(this.TipoNegociacao != "C" && this.TipoNegociacao != "V", "O tipo de negociação informado é inválido.")
                .NotificarSeVerdadeiro(!string.IsNullOrEmpty(this.Observacao) && this.Observacao.Length > 500, LancamentoMensagem.Observacao_Tamanho_Maximo_Excedido);
        }
    }
}
