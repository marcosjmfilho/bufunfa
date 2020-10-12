using JNogueira.Bufunfa.Dominio.Comandos;
using JNogueira.Bufunfa.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNogueira.Bufunfa.Dominio.Interfaces.Dados
{
    /// <summary>
    /// Define o contrato do repositório de notas de corretagem
    /// </summary>
    public interface INotaCorretagemRepositorio
    {
        /// <summary>
        /// Obtém uma nota a partir do seu ID
        /// </summary>
        Task<NotaCorretagem> ObterPorId(int idNota);

        ///// <summary>
        ///// Obtém os lançamentos relacionadas a uma transferência, a partir do ID da transferência
        ///// </summary>
        //Task<IEnumerable<Lancamento>> ObterPorIdTransferencia(string idTransferencia);

        ///// <summary>
        ///// Obtém os lançamentos de um usuário por período para uma determinada conta.
        ///// </summary>
        //Task<IEnumerable<Lancamento>> ObterPorPeriodo(int idConta, DateTime dataInicio, DateTime dataFim);

        ///// <summary>
        ///// Obtém os lançamentos de um usuário por período .
        ///// </summary>
        //Task<IEnumerable<Lancamento>> ObterPorPeriodo(DateTime dataInicio, DateTime dataFim, int idUsuario);

        ///// <summary>
        ///// Obtém os lançamentos baseados nos parâmetros de procura
        ///// </summary>
        //Task<ProcurarSaida> Procurar(ProcurarLancamentoEntrada procurarEntrada);

        /// <summary>
        /// Insere uma nova nota de corretagem
        /// </summary>
        Task Inserir(NotaCorretagem nota);

        /// <summary>
        /// Atualiza as informações da nota de corretagem
        /// </summary>
        void Atualizar(NotaCorretagem nota);

        /// <summary>
        /// Deleta uma nota
        /// </summary>
        void Deletar(NotaCorretagem nota);

        ///// <summary>
        ///// Deleta todos os lançamentos relacionados a uma conta.
        ///// </summary>
        //void DeletarPorConta(int idConta);
    }
}
