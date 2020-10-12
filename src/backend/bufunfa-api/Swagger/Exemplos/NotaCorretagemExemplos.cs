using JNogueira.Bufunfa.Api.ViewModels;
using JNogueira.Bufunfa.Dominio;
using JNogueira.Bufunfa.Dominio.Comandos;
using JNogueira.Bufunfa.Dominio.Resources;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace JNogueira.Bufunfa.Api.Swagger.Exemplos
{
    public class NotaCorretagemRequestExemplo : IExamplesProvider<NotaCorretagemViewModel>
    {
        public NotaCorretagemViewModel GetExamples()
        {
            return new NotaCorretagemViewModel
            {
                DataPregao = DateTime.Now,
                IdConta = 3,
                Numero = "12367213897",
                ValorEmolumentos = (decimal)0.12,
                ValorIrrf = (decimal)0.12,
                ValorIss = (decimal)0.12,
                ValorOutrasTaxas = (decimal)0.12,
                ValorTaxaAna = (decimal)0.12,
                ValorTaxaCorretagem = (decimal)0.12,
                ValorTaxaLiquidacao = (decimal)0.12,
                ValorTaxaRegistro = (decimal)0.12,
                ValorTaxaTermo = (decimal)0.12,
                Observacao = "Nota de corretagem exemplo",
                Lancamentos = new[]
                {
                    new LancamentoNotaViewModel
                    {
                        IdAtivo = 1,
                        Quantidade = 10,
                        Observacao = "Observação",
                        TipoNegociacao = TipoNegociacaoAtivo.Compra,
                        ValorPrecoUnitario = (decimal)43.54
                    },
                    new LancamentoNotaViewModel
                    {
                        IdAtivo = 2,
                        Quantidade = 3,
                        Observacao = "Observação",
                        TipoNegociacao = TipoNegociacaoAtivo.Compra,
                        ValorPrecoUnitario = (decimal)12.54
                    }
                }
            };
        }
    }

    public class NotaSaidaExemplo : NotaCorretagemSaida
    {
        public NotaSaidaExemplo()
            : base(
                  3,
                  new ContaSaidaExemplo(),
                  DateTime.Now,
                  "121212121",
                  (decimal)0.12,
                  (decimal)0.12,
                  (decimal)0.12,
                  (decimal)0.12,
                  (decimal)0.12,
                  (decimal)0.12,
                  (decimal)0.12,
                  (decimal)0.12,
                  (decimal)0.12,
                  "Nota exemplo",
                  new[] { new LancamentoSaidaExemplo() })
        {

        }
    }

    public class NotaCorretagemSaidaExemplo : NotaCorretagemSaida
    {
        public NotaCorretagemSaidaExemplo()
            : base(
                1,
                new ContaSaida(3, "Conta X", TipoConta.ContaCorrente, (decimal)115.54, "Banco Santander S/A", "3345", "01005539-0"),
                DateTime.Now,
                "1092109",
                (decimal)23.34,
                (decimal)23.34,
                (decimal)23.34,
                (decimal)23.34,
                (decimal)23.34,
                (decimal)23.34,
                (decimal)23.34,
                (decimal)23.34,
                (decimal)23.34,
                "Observação",
                new[]
                {
                    new LancamentoSaida(1, DateTime.Now, (decimal)23.34, new ContaSaida(3, "Conta X", TipoConta.ContaCorrente, (decimal)115.54, "Banco Santander S/A", "3345", "01005539-0"), new CategoriaSaida(4, "Salário", TipoCategoria.Credito, "CRÉDITO » Salário"), new PessoaSaida(1, "Meu patrão"), new ParcelaSaida(2, 2, null, DateTime.Now, (decimal)12.12, 1, false, false, null, null), new LancamentoAnexoSaida(1, 1, "1gF8wE6OVfCnghANI70A-gh9rXc-jNGob", "Comprovante", "comprovante.pdf"), null, "Observação qualquer")
                })
        {

        }
    }

    public class ObterNotaPorIdResponseExemplo : IExamplesProvider<Saida>
    {
        public Saida GetExamples()
        {
            return new Saida
            {
                Sucesso = true,
                Mensagens = new[] { NotaCorretagemMensagem.Nota_Encontrada_Com_Sucesso },
                Retorno = new NotaCorretagemSaidaExemplo()
            };
        }
    }

    public class CadastrarNotaResponseExemplo : IExamplesProvider<Saida>
    {
        public Saida GetExamples()
        {
            return new Saida
            {
                Sucesso = true,
                Mensagens = new[] { NotaCorretagemMensagem.Nota_Cadastrada_Com_Sucesso },
                Retorno = new NotaSaidaExemplo()
            };
        }
    }

    public class AlterarNotaResponseExemplo : IExamplesProvider<Saida>
    {
        public Saida GetExamples()
        {
            return new Saida
            {
                Sucesso = true,
                Mensagens = new[] { NotaCorretagemMensagem.Nota_Alterada_Com_Sucesso },
                Retorno = new NotaSaidaExemplo()
            };
        }
    }
}
