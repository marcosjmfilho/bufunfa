using JNogueira.Bufunfa.Dominio.Resources;
using JNogueira.NotifiqueMe;

namespace JNogueira.Bufunfa.Dominio.Comandos
{
    /// <summary>
    /// Comando utilizado para o cadastro de uma nova categoria
    /// </summary>
    public class CategoriaEntrada : Notificavel
    {
        /// <summary>
        /// Id do usuário proprietário
        /// </summary>
        public int IdUsuario { get; }

        /// <summary>
        /// Id da categoria pai
        /// </summary>
        public int? IdCategoriaPai { get; }
        
        /// <summary>
        /// Nome da pessoa
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Tipo da categoria
        /// </summary>
        public string Tipo { get; }

        public CategoriaEntrada(
            int idUsuario,
            string nome,
            string tipo,
            int? idCategoriaPai = null)
        {
            this.IdUsuario      = idUsuario;
            this.Nome           = nome;
            this.Tipo           = tipo?.ToUpper();
            this.IdCategoriaPai = idCategoriaPai;

            this.NotificarSeMenorOuIgualA(this.IdUsuario, 0, Mensagem.Id_Usuario_Invalido)
                .NotificarSeNuloOuVazio(this.Nome, CategoriaMensagem.Nome_Obrigatorio_Nao_Informado)
                .NotificarSeNuloOuVazio(this.Tipo, CategoriaMensagem.Tipo_Obrigatorio_Nao_Informado)
                .NotificarSeVerdadeiro(!string.IsNullOrEmpty(this.Nome) && this.Nome.Length > 100, CategoriaMensagem.Nome_Tamanho_Maximo_Excedido)
                .NotificarSeVerdadeiro(this.Tipo != "D" && this.Tipo != "C", CategoriaMensagem.Tipo_Invalido)
                .NotificarSeMenorQue(this.IdCategoriaPai.Value, 1, CategoriaMensagem.Id_Categoria_Pai_Invalido);
        }
    }
}