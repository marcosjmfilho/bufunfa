using JNogueira.Bufunfa.Dominio.Resources;
using JNogueira.NotifiqueMe;
using System;

namespace JNogueira.Bufunfa.Dominio.Comandos
{
    public class LancamentoEntrada : Notificavel
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
        /// Id da categoria
        /// </summary>
        public int IdCategoria { get; }

        /// <summary>
        /// Id da pessoa
        /// </summary>
        public int? IdPessoa { get; }

        /// <summary>
        /// Id da parcela
        /// </summary>
        public int? IdParcela { get; }

        /// <summary>
        /// Data do lançamento
        /// </summary>
        public DateTime Data { get; }

        /// <summary>
        /// Valor do lançamento
        /// </summary>
        public decimal Valor { get; }

        /// <summary>
        /// Quantidade de ações
        /// </summary>
        public int? QuantidadeAcoes { get; }

        /// <summary>
        /// Observação do lançamento
        /// </summary>
        public string Observacao { get; }

        public LancamentoEntrada(
            int idUsuario,
            int idConta,
            int idCategoria,
            DateTime data,
            decimal valor,
            int? quantidadeAcoes = null,
            int? idPessoa = null,
            int? idParcela = null,
            string observacao = null)
        {
            this.IdUsuario       = idUsuario;
            this.IdConta         = idConta;
            this.IdCategoria     = idCategoria;
            this.IdParcela       = idParcela;
            this.Data            = data.Date;
            this.Valor           = valor;
            this.QuantidadeAcoes = quantidadeAcoes;
            this.IdPessoa        = idPessoa;
            this.Observacao      = observacao;

            this.NotificarSeMenorOuIgualA(this.IdUsuario, 0, Mensagem.Id_Usuario_Invalido)
                .NotificarSeMenorOuIgualA(this.IdConta, 0, ContaMensagem.Id_Conta_Invalido)
                .NotificarSeMenorOuIgualA(this.IdCategoria, 0, CategoriaMensagem.Id_Categoria_Invalido)
                .NotificarSeMaiorQue(this.Data, DateTime.Today, LancamentoMensagem.Data_Lancamento_Maior_Data_Corrente)
                .NotificarSeVerdadeiro(this.IdPessoa.HasValue && this.IdPessoa.Value < 1, PessoaMensagem.Id_Pessoa_Invalido)
                .NotificarSeVerdadeiro(this.IdParcela.HasValue && this.IdParcela.Value < 1, ParcelaMensagem.Id_Parcela_Invalido)
                .NotificarSeVerdadeiro(!string.IsNullOrEmpty(this.Observacao) && this.Observacao.Length > 500, LancamentoMensagem.Observacao_Tamanho_Maximo_Excedido);
        }
    }
}
