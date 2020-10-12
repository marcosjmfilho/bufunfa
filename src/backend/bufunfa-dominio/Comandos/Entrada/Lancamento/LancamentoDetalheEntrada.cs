﻿using JNogueira.Bufunfa.Dominio.Resources;
using JNogueira.NotifiqueMe;

namespace JNogueira.Bufunfa.Dominio.Comandos
{
    public class LancamentoDetalheEntrada : Notificavel
    {
        /// <summary>
        /// Id da categoria
        /// </summary>
        public int IdCategoria { get; }

        /// <summary>
        /// Valor
        /// </summary>
        public decimal Valor { get; }

        /// <summary>
        /// Observação
        /// </summary>
        public string Observacao { get; }

        public LancamentoDetalheEntrada(int idCategoria, decimal valor, string observacao)
        {
            this.IdCategoria = idCategoria;
            this.Valor       = valor;
            this.Observacao  = observacao;

            this.NotificarSeMenorOuIgualA(this.IdCategoria, 0, CategoriaMensagem.Id_Categoria_Invalido)
                .NotificarSeVerdadeiro(!string.IsNullOrEmpty(this.Observacao) && this.Observacao.Length > 500, LancamentoDetalheMensagem.Observacao_Tamanho_Maximo_Excedido);
        }
    }
}
