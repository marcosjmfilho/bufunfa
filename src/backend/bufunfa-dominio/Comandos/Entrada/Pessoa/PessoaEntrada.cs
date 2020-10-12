using JNogueira.Bufunfa.Dominio.Resources;
using JNogueira.NotifiqueMe;

namespace JNogueira.Bufunfa.Dominio.Comandos
{
    public class PessoaEntrada : Notificavel
    {
        /// <summary>
        /// Id do usuário
        /// </summary>
        public int IdUsuario { get; }

        /// <summary>
        /// Nome da pessoa
        /// </summary>
        public string Nome { get; }

        public PessoaEntrada(int idUsuario, string nome)
        {
            this.IdUsuario = idUsuario;
            this.Nome      = nome;

            this.NotificarSeMenorOuIgualA(this.IdUsuario, 0, Mensagem.Id_Usuario_Invalido)
                .NotificarSeNuloOuVazio(this.Nome, PessoaMensagem.Nome_Obrigatorio_Nao_Informado)
                .NotificarSeVerdadeiro(!string.IsNullOrEmpty(this.Nome) && this.Nome.Length > 200, PessoaMensagem.Nome_Tamanho_Maximo_Excedido);
        }
    }
}
