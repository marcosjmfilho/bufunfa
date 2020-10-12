using JNogueira.Bufunfa.Dominio.Entidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JNogueira.Bufunfa.Dominio.Comandos
{
    /// <summary>
    /// Comando de saída para as informações de um lançamento
    /// </summary>
    public class LancamentoSaida
    {
        /// <summary>
        /// ID do lançamento
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// ID da parcela que originou o lançamento
        /// </summary>
        public int? IdParcela { get; }

        /// <summary>
        /// Data do lançamento
        /// </summary>
        [JsonConverter(typeof(JsonDateFormatConverter), "dd/MM/yyyy")]
        public DateTime Data { get; }

        /// <summary>
        /// Valor do lançamento
        /// </summary>
        public decimal Valor { get; }

        /// <summary>
        /// Quantidade de ações (quando renda variável)
        /// </summary>
        public int? QuantidadeAcoes { get; }

        /// <summary>
        /// ID da transferência
        /// </summary>
        public string IdTransferencia { get; }

        /// <summary>
        /// Observação do lançamento
        /// </summary>
        public string Observacao { get; }

        /// <summary>
        /// Conta associada ao lançamento
        /// </summary>
        public ContaSaida Conta { get; }

        /// <summary>
        /// Pessoa associada ao lançamento
        /// </summary>
        public PessoaSaida Pessoa { get; }

        /// <summary>
        /// Categoria do lançamento
        /// </summary>
        public CategoriaSaida Categoria { get; }

        /// <summary>
        /// Parcela associada ao lançamento
        /// </summary>
        public ParcelaSaida Parcela { get; }

        /// <summary>
        /// Anexos do lançamento
        /// </summary>
        public IEnumerable<LancamentoAnexoSaida> Anexos { get; }

        /// <summary>
        /// Detalhes do lançamento
        /// </summary>
        public IEnumerable<LancamentoDetalheSaida> Detalhes { get; }


        public LancamentoSaida(Lancamento lancamento)
        {
            if (lancamento == null)
                return;

            this.Id              = lancamento.Id;
            this.IdParcela       = lancamento.IdParcela;
            this.Data            = lancamento.Data;
            this.Valor           = lancamento.Valor;
            this.QuantidadeAcoes = lancamento.QtdRendaVariavel;
            this.IdTransferencia = lancamento.IdTransferencia;
            this.Observacao      = lancamento.Observacao;
            this.Conta           = new ContaSaida(lancamento.Conta);
            this.Pessoa          = lancamento.IdPessoa.HasValue ? new PessoaSaida(lancamento.Pessoa) : null;
            this.Categoria       = new CategoriaSaida(lancamento.Categoria);
            this.Parcela         = lancamento.IdParcela.HasValue ? new ParcelaSaida(lancamento.Parcela) : null;
            this.Anexos          = lancamento.Anexos?.Select(x => new LancamentoAnexoSaida(x));
            this.Detalhes        = lancamento.Detalhes?.Select(x => new LancamentoDetalheSaida(x));
        }

        public LancamentoSaida(
            int id,
            DateTime data,
            decimal valor,
            ContaSaida conta,
            CategoriaSaida categoria,
            PessoaSaida pessoa = null,
            ParcelaSaida parcela = null,
            LancamentoAnexoSaida anexo = null,
            LancamentoDetalheSaida detalhe = null,
            string idTransferencia = null,
            string observacao = null,
            int? quantidadeAtivo = null)
        {
            this.Id              = id;
            this.Data            = data;
            this.Valor           = valor;
            this.QuantidadeAcoes = quantidadeAtivo;
            this.IdTransferencia = IdTransferencia;
            this.Conta           = conta;
            this.Categoria       = categoria;
            this.Pessoa          = pessoa;
            this.Parcela         = parcela;
            this.Anexos          = anexo != null ? new[] { anexo } : null;
            this.Detalhes        = detalhe != null ? new[] { detalhe } : null;
            this.Observacao      = observacao;
        }

        public override string ToString()
        {
            var descricao = new List<string>
            {
                this.Conta.Nome,

                this.Categoria.Caminho,

                this.Data.ToString("dd/MM/yyyy"),

                this.Valor.ToString("C2")
            };

            return string.Join(" » ", descricao);
        }
    }

    /// <summary>
    /// Comando de sáida para as informações de um anexo
    /// </summary>
    public class LancamentoAnexoSaida
    {
        /// <summary>
        /// ID do anexo
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Id do lançamento
        /// </summary>
        public int IdLancamento { get; }

        /// <summary>
        /// Id do arquivo no Google Drive
        /// </summary>
        public string IdGoogleDrive { get; }

        /// <summary>
        /// Descrição do anexo
        /// </summary>
        public string Descricao { get; }

        /// <summary>
        /// Nome do arquivo do anexo
        /// </summary>
        public string NomeArquivo { get; }

        public LancamentoAnexoSaida(LancamentoAnexo anexo)
        {
            if (anexo == null)
                return;

            this.Id = anexo.Id;
            this.IdLancamento = anexo.IdLancamento;
            this.IdGoogleDrive = anexo.IdGoogleDrive;
            this.Descricao = anexo.Descricao;
            this.NomeArquivo = anexo.NomeArquivo;
        }

        public LancamentoAnexoSaida(
            int id,
            int idLancamento,
            string idGoogleDrive,
            string descricao,
            string nomeArquivo)
        {
            Id = id;
            IdLancamento = idLancamento;
            IdGoogleDrive = idGoogleDrive;
            Descricao = descricao;
            NomeArquivo = nomeArquivo;
        }

        public override string ToString()
        {
            return $"{this.Descricao} - {this.NomeArquivo}";
        }
    }

    /// <summary>
    /// Comando de sáida para as informações de um detalhe
    /// </summary>
    public class LancamentoDetalheSaida
    {
        /// <summary>
        /// ID do detalhe
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Id do lançamento
        /// </summary>
        public int IdLancamento { get; }

        /// <summary>
        /// Valor
        /// </summary>
        public decimal Valor { get; }

        /// <summary>
        /// Observação
        /// </summary>
        public string Observacao { get; }

        /// <summary>
        /// Categoria do detalhe
        /// </summary>
        public CategoriaSaida Categoria { get; }

        /// <summary>
        /// Lancamento do detalhe
        /// </summary>
        public LancamentoDetalheLancamentoSaida Lancamento { get; set; }

        public LancamentoDetalheSaida(LancamentoDetalhe detalhe)
        {
            if (detalhe == null)
                return;

            this.Id = detalhe.Id;
            this.IdLancamento = detalhe.IdLancamento;
            this.Valor = detalhe.Valor;
            this.Observacao = detalhe.Observacao;
            this.Categoria = detalhe.Categoria != null
                ? new CategoriaSaida(detalhe.Categoria)
                : null;
            this.Lancamento = detalhe.Lancamento != null
                ? new LancamentoDetalheLancamentoSaida(detalhe.Lancamento)
                : null;
        }

        public LancamentoDetalheSaida(
            int id,
            int idLancamento,
            decimal valor,
            CategoriaSaida categoria,
            string observacao = null)
        {
            Id = id;
            IdLancamento = idLancamento;
            Valor = valor;
            Observacao = observacao;
            Categoria = categoria;
            Lancamento = null;
        }
    }

    public class LancamentoDetalheLancamentoSaida
    {
        [JsonConverter(typeof(JsonDateFormatConverter), "dd/MM/yyyy")]
        public DateTime Data { get; }

        public ContaSaida Conta { get; }

        public PessoaSaida Pessoa { get; }

        public LancamentoDetalheLancamentoSaida(Lancamento lancamento)
        {
            this.Data = lancamento.Data;
            this.Conta = new ContaSaida(lancamento.Conta);
            this.Pessoa = new PessoaSaida(lancamento.Pessoa);
        }
    }
}
