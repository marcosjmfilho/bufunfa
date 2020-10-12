using System.ComponentModel;

namespace JNogueira.Bufunfa.Dominio.Comandos
{
    /// <summary>
    /// Classe com opções de filtro para procura de pessoas
    /// </summary>
    public class ProcurarPessoaEntrada : ProcurarEntrada<PessoaOrdenarPor>
    {
        public string Nome { get; }

        public ProcurarPessoaEntrada(
            int idUsuario,
            string nome = null,
            PessoaOrdenarPor ordenarPor = PessoaOrdenarPor.Nome,
            string ordenarSentido = "ASC",
            int? paginaIndex = 1,
            int? paginaTamanho = 10)
            : base(idUsuario, ordenarPor, ordenarSentido, paginaIndex, paginaTamanho)
        {
            this.Nome = nome;
        }
    }

    public enum PessoaOrdenarPor
    {
        [Description("ID da pessoa")]
        Id,
        [Description("Nome da pessoa")]
        Nome
    }
}
