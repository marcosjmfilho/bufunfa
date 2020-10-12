using JNogueira.Bufunfa.Api.Swagger;
using JNogueira.Bufunfa.Api.Swagger.Exemplos;
using JNogueira.Bufunfa.Api.ViewModels;
using JNogueira.Bufunfa.Dominio.Comandos;
using JNogueira.Bufunfa.Dominio.Interfaces.Comandos;
using JNogueira.Bufunfa.Dominio.Interfaces.Dados;
using JNogueira.Bufunfa.Dominio.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace JNogueira.Bufunfa.Api.Controllers
{
    [Authorize("Bearer")]
    [Produces("application/json")]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erro não tratado encontrado. (Internal Server Error)", typeof(Response))]
    [SwaggerResponseExample((int)HttpStatusCode.InternalServerError, typeof(InternalServerErrorApiResponse))]
    [SwaggerResponse((int)HttpStatusCode.NotFound, "Endereço não encontrado. (Not found)", typeof(Response))]
    [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(NotFoundApiResponse))]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Acesso negado. Token de autenticação não encontrado. (Unauthorized)", typeof(Response))]
    [SwaggerResponseExample((int)HttpStatusCode.Unauthorized, typeof(UnauthorizedApiResponse))]
    [SwaggerResponse((int)HttpStatusCode.Forbidden, "Acesso negado. Sem permissão de acesso a funcionalidade. (Forbidden)", typeof(Response))]
    [SwaggerResponseExample((int)HttpStatusCode.Forbidden, typeof(ForbiddenApiResponse))]
    [SwaggerTag("Permite a gestão e consulta das notas de corretagem.")]
    public class NotaCorretagemController : BaseController
    {
        private readonly NotaCorretagemServico _notaCorretagemServico;

        public NotaCorretagemController(NotaCorretagemServico notaCorretagemServico)
        {
            _notaCorretagemServico = notaCorretagemServico;
        }

        /// <summary>
        /// Obtém uma nota a partir do seu ID
        /// </summary>
        [HttpGet]
        [Route("nota-corretagem/obter-por-id")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Nota de corretagem encontrada.", typeof(Response))]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(ObterNotaPorIdResponseExemplo))]
        public async Task<IActionResult> ObterNotaPorId([FromQuery, SwaggerParameter("ID da nota.", Required = true)] int idNota)
        {
            return new ApiResult(await _notaCorretagemServico.ObterNotaPorId(idNota, base.ObterIdUsuarioClaim()));
        }

        ///// <summary>
        ///// Realiza uma procura por lançamentos a partir dos parâmetros informados
        ///// </summary>
        //[HttpPost]
        //[Consumes("application/json")]
        //[Route("lancamento/procurar")]
        //[SwaggerRequestExample(typeof(ProcurarLancamentoViewModel), typeof(ProcurarLancamentoRequestExemplo))]
        //[SwaggerResponse((int)HttpStatusCode.OK, "Resultado da procura por lançamentos.", typeof(Response))]
        //[SwaggerResponseExample((int)HttpStatusCode.OK, typeof(ProcurarLancamentoResponseExemplo))]
        //public async Task<IActionResult> Procurar([FromBody, SwaggerParameter("Parâmetros utilizados para realizar a procura.", Required = true)] ProcurarLancamentoViewModel model)
        //{
        //    var entrada = new ProcurarLancamentoEntrada(
        //        base.ObterIdUsuarioClaim(),
        //        model.IdConta,
        //        model.IdCategoria,
        //        model.IdPessoa,
        //        model.DataInicio,
        //        model.DataFim,
        //        model.OrdenarPor,
        //        model.OrdenarSentido,
        //        model.PaginaIndex,
        //        model.PaginaTamanho
        //    );

        //    return new ApiResult(await _notaCorretagemServico.ProcurarLancamentos(entrada));
        //}

        /// <summary>
        /// Realiza o cadastro de uma nota de corretagem.
        /// </summary>
        [Consumes("application/json")]
        [HttpPost]
        [Route("nota-corretagem/cadastrar")]
        [SwaggerRequestExample(typeof(NotaCorretagemViewModel), typeof(NotaCorretagemRequestExemplo))]
        [SwaggerResponse((int)HttpStatusCode.OK, "Nota de corretagem cadastrada com sucesso.", typeof(Response))]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CadastrarNotaResponseExemplo))]
        public async Task<IActionResult> CadastrarNota([FromBody, SwaggerParameter("Informações de cadastro da nota.", Required = true)] NotaCorretagemViewModel model)
        {
            var entrada = new NotaCorretagemEntrada(
                base.ObterIdUsuarioClaim(),
                model.IdConta.Value,
                model.DataPregao,
                model.Numero,
                model.Lancamentos?.Select(x => new LancamentoNotaEntrada(x.IdAtivo, x.Quantidade, x.ValorPrecoUnitario, x.TipoNegociacao, x.Observacao)).ToArray(),
                model.ValorTaxaLiquidacao,
                model.ValorTaxaRegistro,
                model.ValorTaxaTermo,
                model.ValorTaxaAna,
                model.ValorEmolumentos,
                model.ValorTaxaCorretagem,
                model.ValorIss,
                model.ValorIrrf,
                model.ValorOutrasTaxas,
                model.Observacao);

            return new ApiResult(await _notaCorretagemServico.CadastrarNota(entrada));
        }

        /// <summary>
        /// Realiza a alteração de uma nota.
        /// </summary>
        [HttpPut]
        [Consumes("application/json")]
        [Route("nota-corretagem/alterar")]
        [SwaggerRequestExample(typeof(NotaCorretagemViewModel), typeof(NotaCorretagemRequestExemplo))]
        [SwaggerResponse((int)HttpStatusCode.OK, "Nota de corretagem alterada com sucesso.", typeof(Response))]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(AlterarNotaResponseExemplo))]
        public async Task<IActionResult> AlterarNota(
            [FromQuery, SwaggerParameter("ID da nota.", Required = true)] int idNota,
            [FromBody, SwaggerParameter("Informações para alteração de uma nota.", Required = true)] NotaCorretagemViewModel model)
        {
            var entrada = new NotaCorretagemEntrada(
                base.ObterIdUsuarioClaim(),
                model.IdConta.Value,
                model.DataPregao,
                model.Numero,
                model.Lancamentos?.Select(x => new LancamentoNotaEntrada(x.IdAtivo, x.Quantidade, x.ValorPrecoUnitario, x.TipoNegociacao, x.Observacao)).ToArray(),
                model.ValorTaxaLiquidacao,
                model.ValorTaxaRegistro,
                model.ValorTaxaTermo,
                model.ValorTaxaAna,
                model.ValorEmolumentos,
                model.ValorTaxaCorretagem,
                model.ValorIss,
                model.ValorIrrf,
                model.ValorOutrasTaxas,
                model.Observacao);

            return new ApiResult(await _notaCorretagemServico.AlterarNota(idNota, entrada));
        }

        ///// <summary>
        ///// Realiza a exclusão de um lançamento.
        ///// </summary>
        //[HttpDelete]
        //[Route("lancamento/excluir")]
        //[SwaggerResponse((int)HttpStatusCode.OK, "Lançamento excluído com sucesso.", typeof(Response))]
        //[SwaggerResponseExample((int)HttpStatusCode.OK, typeof(ExcluirLancamentoResponseExemplo))]
        //public async Task<ISaida> ExcluirLancamento([FromQuery, SwaggerParameter("ID do lançamento que deverá ser excluído.", Required = true)] int idLancamento)
        //{
        //    return await _notaCorretagemServico.ExcluirLancamento(idLancamento, base.ObterIdUsuarioClaim());
        //}

        ///// <summary>
        ///// Realiza a exclusão de um anexo.
        ///// </summary>
        //[HttpDelete]
        //[Route("lancamento/excluir-anexo")]
        //[SwaggerResponse((int)HttpStatusCode.OK, "Anexo excluído com sucesso.", typeof(Response))]
        //[SwaggerResponseExample((int)HttpStatusCode.OK, typeof(ExcluirAnexoResponseExemplo))]
        //public async Task<IActionResult> ExcluirAnexo([FromQuery, SwaggerParameter("ID do anexo que deverá ser excluído.", Required = true)] int idAnexo)
        //{
        //    return new ApiResult(await _notaCorretagemServico.ExcluirAnexo(idAnexo, base.ObterIdUsuarioClaim()));
        //}

        ///// <summary>
        ///// Realiza a exclusão de um detalhe.
        ///// </summary>
        //[HttpDelete]
        //[Route("lancamento/excluir-detalhe")]
        //[SwaggerResponse((int)HttpStatusCode.OK, "Detalhe excluído com sucesso.", typeof(Response))]
        //[SwaggerResponseExample((int)HttpStatusCode.OK, typeof(ExcluirDetalheResponseExemplo))]
        //public async Task<IActionResult> ExcluirDetalhe([FromQuery, SwaggerParameter("ID do detalhe que deverá ser excluído.", Required = true)] int idDetalhe)
        //{
        //    return new ApiResult(await _notaCorretagemServico.ExcluirDetalhe(idDetalhe, base.ObterIdUsuarioClaim()));
        //}
    }
}
