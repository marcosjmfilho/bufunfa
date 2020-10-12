using JNogueira.Bufunfa.Web.Filters;
using JNogueira.Bufunfa.Web.Helpers;
using JNogueira.Bufunfa.Web.Models;
using JNogueira.Bufunfa.Web.Proxy;
using JNogueira.Bufunfa.Web.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JNogueira.Bufunfa.Web.Controllers
{
    [Authorize]
    [Route("renda-variavel")]
    public class RendaVariavelController : BaseController
    {
        private readonly DatatablesHelper _datatablesHelper;

        public RendaVariavelController(DatatablesHelper datatablesHelper, BackendProxy proxy)
            : base(proxy)
        {
            _datatablesHelper = datatablesHelper;
        }

        [HttpGet]
        [Route("ativos")]
        [ExibirPeriodoAtualFilter]
        [ExibirAtalhosFilter]
        public IActionResult IndexAtivos()
        {
            return View();
        }

        [HttpGet]
        [Route("listar-ativos")]
        [FeedbackExceptionFilter("Ocorreu um erro ao obter os ativos.", TipoAcaoAoOcultarFeedback.Ocultar)]
        public async Task<IActionResult> Listar(string tipo)
        {
            var analiseSaida = await _proxy.ObterAnaliseAtivos();

            if (!analiseSaida.Sucesso)
                return new FeedbackResult(new Feedback(TipoFeedback.Erro, "Não foi possível obter os ativos.", analiseSaida.Mensagens));

            if (tipo == "DIV")
            {
                return PartialView("ListarAtivosDividendos", analiseSaida.Retorno.Where(x => x.CodigoTipo == (int)TipoConta.Acoes).ToList());
            }
            else
            {
                return PartialView("ListarAtivosFii", analiseSaida.Retorno.Where(x => x.CodigoTipo == (int)TipoConta.FII).ToList());
            }
        }

        [HttpGet]
        [Route("cadastrar-ativo")]
        public IActionResult CadastrarAtivo()
        {
            return PartialView("Manter", null);
        }

        [HttpPost]
        [Route("cadastrar-ativo")]
        [FeedbackExceptionFilter("Ocorreu um erro ao cadastrar o ativo.", TipoAcaoAoOcultarFeedback.Ocultar)]
        public async Task<IActionResult> CadastrarAtivo(ManterConta entrada)
        {
            if (entrada == null)
                return new FeedbackResult(new Feedback(TipoFeedback.Atencao, "As informações do ativo não foram preenchidas.", new[] { "Verifique se todas as informações do ativo foram preenchidas." }, TipoAcaoAoOcultarFeedback.Ocultar));

            var saida = await _proxy.CadastrarConta(entrada);

            if (!saida.Sucesso)
                return new FeedbackResult(new Feedback(TipoFeedback.Erro, "Não foi possível cadastrar o ativo.", saida.Mensagens));

            return new FeedbackResult(new Feedback(TipoFeedback.Sucesso, saida.Mensagens.First(), tipoAcao: TipoAcaoAoOcultarFeedback.OcultarMoldais));
        }

        [HttpGet]
        [Route("alterar-ativo")]
        [FeedbackExceptionFilter("Ocorreu um erro ao obter as informações do ativo.", TipoAcaoAoOcultarFeedback.Ocultar)]
        public async Task<IActionResult> AlterarAtivo(int id)
        {
            var saida = await _proxy.ObterContaPorId(id);

            if (!saida.Sucesso)
                return new FeedbackResult(new Feedback(TipoFeedback.Erro, "Não foi possível exibir as informações do ativo.", saida.Mensagens));

            return PartialView("Manter", saida.Retorno);
        }

        [HttpPost]
        [Route("alterar-ativo")]
        [FeedbackExceptionFilter("Ocorreu um erro ao alterar as informações do ativo.", TipoAcaoAoOcultarFeedback.Ocultar)]
        public async Task<IActionResult> AlterarAtivo(ManterConta entrada)
        {
            if (entrada == null)
                return new FeedbackResult(new Feedback(TipoFeedback.Atencao, "As informações do ativo não foram preenchidas.", new[] { "Verifique se todas as informações do ativo foram preenchidas." }, TipoAcaoAoOcultarFeedback.Ocultar));

            var saida = await _proxy.AlterarConta(entrada);

            return !saida.Sucesso
                ? new FeedbackResult(new Feedback(TipoFeedback.Erro, "Não foi possível alterar o ativo.", saida.Mensagens))
                : new FeedbackResult(new Feedback(TipoFeedback.Sucesso, saida.Mensagens.First(), tipoAcao: TipoAcaoAoOcultarFeedback.OcultarMoldais));
        }

        [HttpPost]
        [Route("excluir-ativo")]
        [FeedbackExceptionFilter("Ocorreu um erro ao excluir o ativo.", TipoAcaoAoOcultarFeedback.Ocultar)]
        public async Task<IActionResult> ExcluirAtivo(int id)
        {
            var saida = await _proxy.ExcluirConta(id);

            return !saida.Sucesso
                ? new FeedbackResult(new Feedback(TipoFeedback.Erro, "Não foi possível excluir o ativo.", saida.Mensagens))
                : new FeedbackResult(new Feedback(TipoFeedback.Sucesso, "Ativo excluído com sucesso."));
        }

        [HttpGet]
        [Route("obter-analise-por-ativo")]
        public async Task<IActionResult> ObterAnalisePorAtivo(int id, decimal valorCotacao = 0)
        {
            var saida = await _proxy.ObterAnaliseAtivo(id, valorCotacao);

            if (!saida.Sucesso)
                return new FeedbackResult(new Feedback(TipoFeedback.Erro, "Não foi possível obter a análise do ativo.", saida.Mensagens));

            return PartialView("PopupAcao", saida.Retorno);
        }

        [HttpGet]
        [Route("informar-valor-cotacao-por-acao")]
        public async Task<IActionResult> InformarValorCotacaoPorAcao(int id)
        {
            var saida = await _proxy.ObterContaPorId(id);

            if (!saida.Sucesso)
                return new FeedbackResult(new Feedback(TipoFeedback.Erro, "Não foi possível obter as informações do ativo.", saida.Mensagens));

            return PartialView("PopupValorCotacao", saida.Retorno);
        }
    }
}
