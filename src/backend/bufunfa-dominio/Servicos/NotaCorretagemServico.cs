using JNogueira.Bufunfa.Dominio.Comandos;
using JNogueira.Bufunfa.Dominio.Entidades;
using JNogueira.Bufunfa.Dominio.Interfaces.Comandos;
using JNogueira.Bufunfa.Dominio.Interfaces.Dados;
using JNogueira.Bufunfa.Dominio.Resources;
using JNogueira.NotifiqueMe;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JNogueira.Bufunfa.Dominio.Servicos
{
    public class NotaCorretagemServico : Notificavel
    {
        private readonly ContaServico _contaServico;
        private readonly IContaRepositorio _contaRepositorio;
        private readonly ILancamentoRepositorio _lancamentoRepositorio;
        private readonly INotaCorretagemRepositorio _notaCorretagemRepositorio;
        private readonly IUnitOfWork _uow;

        public NotaCorretagemServico(
            ContaServico contaServico,
            IContaRepositorio contaRepositorio,
            ILancamentoRepositorio lancamentoRepositorio,
            INotaCorretagemRepositorio notaCorretagemRepositorio,
            IUnitOfWork uow)
        {
            _contaRepositorio          = contaRepositorio;
            _contaServico              = contaServico;
            _lancamentoRepositorio     = lancamentoRepositorio;
            _notaCorretagemRepositorio = notaCorretagemRepositorio;
            _uow                       = uow;
        }

        public async Task<ISaida> ObterNotaPorId(int idNota, int idUsuario)
        {
            this.NotificarSeMenorOuIgualA(idNota, 0, NotaCorretagemMensagem.Id_Nota_Invalido);
            this.NotificarSeMenorOuIgualA(idUsuario, 0, Mensagem.Id_Usuario_Invalido);

            if (this.Invalido)
                return new Saida(false, this.Mensagens, null);

            var nota = await _notaCorretagemRepositorio.ObterPorId(idNota);

            if (nota == null)
                return new Saida(true, new[] { NotaCorretagemMensagem.Id_Nota_Nao_Existe }, null);

            // Verifica se a nota pertece ao usuário informado.
            this.NotificarSeDiferentes(nota.IdUsuario, idUsuario, NotaCorretagemMensagem.Nota_Nao_Pertence_Usuario);

            return this.Invalido
                ? new Saida(false, this.Mensagens, null)
                : new Saida(true, new[] { NotaCorretagemMensagem.Nota_Encontrada_Com_Sucesso }, new NotaCorretagemSaida(nota));
        }

        //public async Task<ISaida> ProcurarLancamentos(ProcurarLancamentoEntrada entrada)
        //{
        //    // Verifica se os parâmetros para a procura foram informadas corretamente
        //    return entrada.Invalido
        //        ? new Saida(false, entrada.Mensagens, null)
        //        : await _lancamentoRepositorio.Procurar(entrada);
        //}

        public async Task<ISaida> CadastrarNota(NotaCorretagemEntrada entrada)
        {
            // Verifica se as informações para cadastro foram informadas corretamente
            if (entrada.Invalido)
                return new Saida(false, entrada.Mensagens, null);

            // Verifica se a conta existe a partir do ID informado.
            this.NotificarSeFalso(await _contaRepositorio.VerificarExistenciaPorId(entrada.IdUsuario, entrada.IdConta), ContaMensagem.Id_Conta_Nao_Existe);

            if (this.Invalido)
                return new Saida(false, this.Mensagens, null);

            foreach (var idAtivoVenda in entrada.Lancamentos.Where(x => x.TipoNegociacao == TipoNegociacaoAtivo.Venda).Select(x => x.IdAtivo).Distinct())
            {
                // Verifica se a quantidade de ações/cotas vendidas é superior a quantidade em carteira para o ativo
                var ativoVenda = await _contaRepositorio.ObterPorId(idAtivoVenda);

                if (ativoVenda == null)
                {
                    this.AdicionarNotificacao($"O ativo de ID {idAtivoVenda} não existe.");
                    continue;
                }

                var qtdAcoesVendasNaNota = entrada.Lancamentos.Where(x => x.TipoNegociacao == TipoNegociacaoAtivo.Venda && x.IdAtivo == idAtivoVenda).Sum(x => x.Quantidade);

                var operacoes = await _lancamentoRepositorio.ObterPorPeriodo(idAtivoVenda, new DateTime(2019, 1, 1), DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59));

                var qtdAcoesCompradas = operacoes.Where(x => x.IdCategoria == (int)TipoCategoriaEspecial.CompraAcoes).Sum(x => x.QtdRendaVariavel.HasValue ? x.QtdRendaVariavel.Value : 0);
                var qtdAcoesVendidas = operacoes.Where(x => x.IdCategoria == (int)TipoCategoriaEspecial.VendaAcoes).Sum(x => x.QtdRendaVariavel.HasValue ? x.QtdRendaVariavel.Value : 0);

                var qtdAcoesDisponivel = qtdAcoesCompradas - qtdAcoesVendidas;

                this.NotificarSeVerdadeiro(qtdAcoesVendasNaNota > qtdAcoesDisponivel, $"O ativo {ativoVenda} possui disponível em carteira, {qtdAcoesDisponivel} {(ativoVenda.Tipo == TipoConta.Acoes ? "ações" : "cotas")}, por esse motivo não é possível vender {qtdAcoesVendasNaNota} {(ativoVenda.Tipo == TipoConta.Acoes ? "ações" : "cotas")}");
            }

            var contaNota = (ContaSaida)(await _contaServico.ObterContaPorId(entrada.IdConta, entrada.IdUsuario)).Retorno;

            var totalCompra = entrada.Lancamentos.Where(x => x.TipoNegociacao == TipoNegociacaoAtivo.Compra).Sum(x => x.Quantidade * x.ValorPrecoUnitario);

            // Verifica se existe saldo disponível na conta para efetuar as compras existentes na nota.
            this.NotificarSeMaiorQue(totalCompra, contaNota.ValorSaldoAtual.Value, $"O saldo atual disponível ({contaNota.ValorSaldoAtual?.ToString("C2")}) na conta {contaNota.Nome} é insuficiente para realizar as compras existentes na nota.");

            if (this.Invalido)
                return new Saida(false, this.Mensagens, null);

            var nota = new NotaCorretagem(entrada);

            await _notaCorretagemRepositorio.Inserir(nota);

            await _uow.Commit();

            return new Saida(true, new[] { NotaCorretagemMensagem.Nota_Cadastrada_Com_Sucesso }, new NotaCorretagemSaida(await _notaCorretagemRepositorio.ObterPorId(nota.Id)));
        }

        public async Task<ISaida> AlterarNota(int idNota, NotaCorretagemEntrada entrada)
        {
            this.NotificarSeMenorOuIgualA(idNota, 0, NotaCorretagemMensagem.Id_Nota_Invalido);

            if (this.Invalido)
                return new Saida(false, this.Mensagens, null);

            // Verifica se as informações para alteração foram informadas corretamente
            if (entrada.Invalido)
                return new Saida(false, entrada.Mensagens, null);

            var nota = await _notaCorretagemRepositorio.ObterPorId(idNota);

            // Verifica se a nota existe
            this.NotificarSeNulo(nota, NotaCorretagemMensagem.Id_Nota_Nao_Existe);

            if (this.Invalido)
                return new Saida(false, this.Mensagens, null);

            // Verifica se a nota pertece ao usuário informado.
            this.NotificarSeDiferentes(nota.IdUsuario, entrada.IdUsuario, NotaCorretagemMensagem.Nota_Alterar_Nao_Pertence_Usuario);

            if (this.Invalido)
                return new Saida(false, this.Mensagens, null);

            nota.Alterar(entrada);

            _notaCorretagemRepositorio.Atualizar(nota);

            await _uow.Commit();

            return new Saida(true, new[] { NotaCorretagemMensagem.Nota_Alterada_Com_Sucesso }, new NotaCorretagemSaida(await _notaCorretagemRepositorio.ObterPorId(nota.Id)));
        }

        //public async Task<ISaida> ExcluirLancamento(int idLancamento, int idUsuario)
        //{
        //    this.NotificarSeMenorOuIgualA(idLancamento, 0, LancamentoMensagem.Id_Lancamento_Invalido);
        //    this.NotificarSeMenorOuIgualA(idUsuario, 0, Mensagem.Id_Usuario_Invalido);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    var lancamento = await _lancamentoRepositorio.ObterPorId(idLancamento);

        //    // Verifica se o lançamento existe
        //    this.NotificarSeNulo(lancamento, LancamentoMensagem.Id_Lancamento_Nao_Existe);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    // Verifica se o lançamento pertece ao usuário informado.
        //    this.NotificarSeDiferentes(lancamento.IdUsuario, idUsuario, LancamentoMensagem.Lancamento_Excluir_Nao_Pertence_Usuario);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    foreach (var anexo in lancamento.Anexos)
        //    {
        //        // Exclui os anexos do banco de dados e os arquivos do Google Drive
        //        await _anexoRepositorio.Deletar(anexo);
        //    }

        //    // Verifica se o lançamento está associado ao pagamento de uma fatura de cartão de crédito.
        //    if (lancamento.Categoria.Id == (int)TipoCategoriaEspecial.PagamentoFaturaCartao)
        //    {
        //        var fatura = await _faturaRepositorio.ObterPorLancamento(lancamento.Id);

        //        if (fatura != null)
        //        {
        //            var parcelas = await _parcelaRepositorio.ObterPorFatura(fatura.Id);

        //            foreach(var parcela in parcelas)
        //            {
        //                parcela.DesfazerLancamento();
        //            }
        //        }
        //    }

        //    // Caso o lançamento esteja associado a uma parcela, a parcela deve ser reaberta.
        //    if (lancamento.IdParcela.HasValue)
        //    {
        //        var parcela = await _parcelaRepositorio.ObterPorId(lancamento.IdParcela.Value);

        //        parcela?.DesfazerLancamento();
        //    }

        //    if (string.IsNullOrEmpty(lancamento.IdTransferencia))
        //    {
        //        _lancamentoRepositorio.Deletar(lancamento);
        //    }
        //    // Caso o lançamento pertence a uma transferência, todos os lançamentos relacionados a transferência também serão excluídos.
        //    else
        //    {
        //        var lancamentosTransferencia = await _lancamentoRepositorio.ObterPorIdTransferencia(lancamento.IdTransferencia);

        //        foreach (var item in lancamentosTransferencia)
        //            _lancamentoRepositorio.Deletar(item);
        //    }

        //    await _uow.Commit();

        //    return new Saida(true, new[] { LancamentoMensagem.Lancamento_Excluido_Com_Sucesso }, new LancamentoSaida(lancamento));
        //}

        //public async Task<ISaida> ObterAnexoPorId(int idAnexo, int idUsuario)
        //{
        //    this.NotificarSeMenorOuIgualA(idAnexo, 0, LancamentoAnexoMensagem.Id_Anexo_Invalido);
        //    this.NotificarSeMenorOuIgualA(idUsuario, 0, Mensagem.Id_Usuario_Invalido);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    var anexo = await _anexoRepositorio.ObterPorId(idAnexo);

        //    // Verifica se o anexo existe
        //    this.NotificarSeNulo(anexo, LancamentoAnexoMensagem.Id_Anexo_Nao_Existe);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    // Verifica se o anexo pertece a um lançamento do usuário informado.
        //    this.NotificarSeDiferentes(anexo.Lancamento.IdUsuario, idUsuario, LancamentoAnexoMensagem.Anexo_Download_Nao_Pertence_Usuario);

        //    return this.Invalido
        //        ? new Saida(false, this.Mensagens, null)
        //        : new Saida(true, new[] { LancamentoAnexoMensagem.Anexo_Encontrado_Com_Sucesso }, new LancamentoAnexoSaida(anexo));
        //}

        //public async Task<ISaida> CadastrarAnexo(int idLancamento, LancamentoAnexoEntrada entrada)
        //{
        //    // Verifica se as informações para cadastro foram informadas corretamente
        //    if (entrada.Invalido)
        //        return new Saida(false, entrada.Mensagens, null);

        //    var lancamento = await _lancamentoRepositorio.ObterPorId(idLancamento);

        //    // Verifica se o lançamento existe
        //    this.NotificarSeNulo(lancamento, LancamentoMensagem.Id_Lancamento_Nao_Existe);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    // Insere as informações do anexo no banco de dados e realiza o upload do arquivo para o Google Drive
        //    var anexo = await _anexoRepositorio.Inserir(idLancamento, lancamento.Data, entrada);

        //    if (_anexoRepositorio.Invalido)
        //        return new Saida(false, _anexoRepositorio.Mensagens, null);

        //    await _uow.Commit();

        //    return new Saida(true, new[] { LancamentoAnexoMensagem.Anexo_Cadastrado_Com_Sucesso }, new LancamentoAnexoSaida(anexo));
        //}

        //public async Task<ISaida> ExcluirAnexo(int idAnexo, int idUsuario)
        //{
        //    this.NotificarSeMenorOuIgualA(idAnexo, 0, LancamentoAnexoMensagem.Id_Anexo_Invalido);
        //    this.NotificarSeMenorOuIgualA(idUsuario, 0, Mensagem.Id_Usuario_Invalido);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    var anexo = await _anexoRepositorio.ObterPorId(idAnexo);

        //    // Verifica se o anexo existe
        //    this.NotificarSeNulo(anexo, LancamentoAnexoMensagem.Id_Anexo_Nao_Existe);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    // Verifica se o anexo pertece a um lançamento ao usuário informado.
        //    this.NotificarSeDiferentes(anexo.Lancamento.IdUsuario, idUsuario, LancamentoAnexoMensagem.Anexo_Excluir_Nao_Pertence_Usuario);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    // Exclui o anexo do banco de dados e também o arquivo do Google Drive.
        //    await _anexoRepositorio.Deletar(anexo);

        //    if (_anexoRepositorio.Invalido)
        //        return new Saida(false, _anexoRepositorio.Mensagens, null);

        //    await _uow.Commit();

        //    return new Saida(true, new[] { LancamentoAnexoMensagem.Anexo_Excluido_Com_Sucesso }, new LancamentoAnexoSaida(anexo));
        //}

        //public async Task<ISaida> CadastrarDetalhe(int idLancamento, LancamentoDetalheEntrada entrada)
        //{
        //    // Verifica se as informações para cadastro foram informadas corretamente
        //    if (entrada.Invalido)
        //        return new Saida(false, entrada.Mensagens, null);

        //    var lancamento = await _lancamentoRepositorio.ObterPorId(idLancamento);

        //    // Verifica se o lançamento existe
        //    this.NotificarSeNulo(lancamento, LancamentoMensagem.Id_Lancamento_Nao_Existe);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    // Verifica se o valor do detalhe é maior que o valor do lançamento
        //    this.NotificarSeMaiorOuIgualA(entrada.Valor, lancamento.Valor, LancamentoDetalheMensagem.Valor_Detalhe_Maior_Ou_Igual_Valor_Lancamento);

        //    // Verifica se a categoria do detalhe é a mesma categoria informada para o lançamento.
        //    this.NotificarSeIguais(entrada.IdCategoria, lancamento.IdCategoria, LancamentoDetalheMensagem.Id_Categoria_Igual_Categoria_Lancamento);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    // Verifica se a soma dos valores dos detalhes do lançamento somado ao valor do detalhe é maior que o valor do lançamento
        //    this.NotificarSeMaiorQue(lancamento.Detalhes.Sum(x => x.Valor) + entrada.Valor, lancamento.Valor, LancamentoDetalheMensagem.Soma_Detalhes_Mais_Valor_Detalhe_Maior_Valor_Lancamento);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    var detalhe = new LancamentoDetalhe(idLancamento, entrada);

        //    await _detalheRepositorio.Inserir(detalhe);

        //    await _uow.Commit();

        //    return new Saida(true, new[] { LancamentoDetalheMensagem.Detalhe_Cadastrado_Com_Sucesso }, new LancamentoDetalheSaida(await _detalheRepositorio.ObterPorId(detalhe.Id)));
        //}

        //public async Task<ISaida> ExcluirDetalhe(int idDetalhe, int idUsuario)
        //{
        //    this.NotificarSeMenorOuIgualA(idDetalhe, 0, LancamentoDetalheMensagem.Id_Detalhe_Invalido);
        //    this.NotificarSeMenorOuIgualA(idUsuario, 0, Mensagem.Id_Usuario_Invalido);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    var detalhe = await _detalheRepositorio.ObterPorId(idDetalhe);

        //    // Verifica se o detalhe existe
        //    this.NotificarSeNulo(detalhe, LancamentoDetalheMensagem.Id_Detalhe_Nao_Existe);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    // Verifica se o detalhe pertece a um lançamento do usuário informado.
        //    this.NotificarSeDiferentes(detalhe.Lancamento.IdUsuario, idUsuario, LancamentoDetalheMensagem.Detalhe_Excluir_Nao_Pertence_Usuario);

        //    if (this.Invalido)
        //        return new Saida(false, this.Mensagens, null);

        //    _detalheRepositorio.Deletar(detalhe);

        //    await _uow.Commit();

        //    return new Saida(true, new[] { LancamentoDetalheMensagem.Detalhe_Excluido_Com_Sucesso }, new LancamentoDetalheSaida(detalhe));
        //}
    }
}
