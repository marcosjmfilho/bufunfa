using JNogueira.Bufunfa.Dominio.Resources;
using JNogueira.NotifiqueMe;

namespace JNogueira.Bufunfa.Dominio.Comandos
{
    /// <summary>
    /// Comando utilizado para entrada de conta
    /// </summary>
    public class ContaEntrada : Notificavel
    {
        /// <summary>
        /// Id do usuário proprietário da conta
        /// </summary>
        public int IdUsuario { get; }

        /// <summary>
        /// Nome da conta
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Tipo da conta
        /// </summary>
        public TipoConta Tipo { get; }

        /// <summary>
        /// Valor inicial do saldo da conta
        /// </summary>
        public decimal? ValorSaldoInicial { get; }

        /// <summary>
        /// Nome da instituição financeira a qual a conta pertence
        /// </summary>
        public string NomeInstituicao { get; }

        /// <summary>
        /// Número da agência da conta
        /// </summary>
        public string NumeroAgencia { get; }

        /// <summary>
        /// Número da conta
        /// </summary>
        public string Numero { get; }

        /// <summary>
        /// Ranking da conta
        /// </summary>
        public int? Ranking { get; }

        public ContaEntrada(
            int idUsuario,
            string nome,
            TipoConta tipo,
            decimal? valorSaldoInicial = null,
            string nomeInstituicao = null,
            string numeroAgencia = null,
            string numero = null,
            int? ranking = null)
        {
            this.IdUsuario         = idUsuario;
            this.Nome              = tipo == TipoConta.Acoes ? nome.ToUpper() : nome;
            this.Tipo              = tipo;
            this.ValorSaldoInicial = valorSaldoInicial.HasValue && valorSaldoInicial.Value == 0 ? null : valorSaldoInicial;
            this.NomeInstituicao   = nomeInstituicao;
            this.NumeroAgencia     = numeroAgencia;
            this.Numero            = numero;
            this.Ranking           = ranking;

            this.NotificarSeMenorOuIgualA(this.IdUsuario, 0, Mensagem.Id_Usuario_Invalido)
                .NotificarSeNuloOuVazio(this.Nome, ContaMensagem.Nome_Obrigatorio_Nao_Informado)
                .NotificarSeVerdadeiro(!string.IsNullOrEmpty(this.Nome) && this.Nome.Length > 100, ContaMensagem.Nome_Tamanho_Maximo_Excedido)
                .NotificarSeVerdadeiro(!string.IsNullOrEmpty(this.NomeInstituicao) && this.NomeInstituicao.Length > 500, ContaMensagem.Nome_Instituicao_Tamanho_Maximo_Excedido)
                .NotificarSeVerdadeiro(!string.IsNullOrEmpty(this.NumeroAgencia) && this.NumeroAgencia.Length > 20, ContaMensagem.Numero_Agencia_Tamanho_Maximo_Excedido)
                .NotificarSeVerdadeiro(!string.IsNullOrEmpty(this.Numero) && this.Numero.Length > 20, ContaMensagem.Numero_Tamanho_Maximo_Excedido);
        }
    }
}
