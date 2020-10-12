var RendaVariavelAtivos = function () {

    var _carregarAtivos = function (tipo) {
        $.get(App.corrigirPathRota("/renda-variavel/listar-ativos?tipo=" + tipo), function (html) {
            if (tipo == "DIV") {
                App.desbloquear($("#portlet-dividendos"));
                $("#divValorDividendos").html(html);
                App.aplicarTabelaSelecionavel("#table-dividendos");
            } else {
                App.desbloquear($("#portlet-fii"));
                $("#divFiis").html(html);
                App.aplicarTabelaSelecionavel("#table-fii");
            }            

            $("button[class*='detalhar-ativo-" + tipo.toLowerCase() + "']").each(function () {
                let idConta = $(this).data("id-conta");

                $(this).click(function () {
                    Bufunfa.exibirAcao(idConta);
                });
            });

            $("button[class*='alterar-ativo-" + tipo.toLowerCase() + "']").each(function () {
                let idConta = $(this).data("id-conta");
                let tipo = $(this).data("tipo");

                $(this).click(function () {
                    _manterAtivo(idConta, tipo);
                });
            });

            $("button[class*='excluir-ativo-" + tipo.toLowerCase() + "']").each(function () {
                let idConta = $(this).data("id-conta");
                let tipo = $(this).data("tipo");

                $(this).click(function () {
                    AppModal.exibirConfirmacao("Deseja realmente excluir esse ativo?", "Sim", "Não", function () { _excluirAtivo(idConta, tipo); });
                });
            });

        }).done(function () {
            KTApp.initTooltips();
        }).fail(function (jqXhr) {
            let feedback = Feedback.converter(jqXhr.responseJSON);
            feedback.exibir();
        });
    };


    var _manterAtivo = function (id) {
        let cadastro = id === null || id === undefined || id === 0;

        AppModal.exibirPorRota((!cadastro ? App.corrigirPathRota("/renda-variavel/alterar-ativo?id=" + id) : App.corrigirPathRota("/renda-variavel/cadastrar-ativo")), function () {
            $("#form-manter-conta").validate({
                rules: {
                    iNome: {
                        required: true
                    },
                    sTipo: {
                        required: true
                    }
                },

                submitHandler: function () {

                    let ativo = {
                        Id: $("#iIdConta").val(),
                        Nome: $("#iNome").val(),
                        Tipo: $("#sTipo").val(),
                        ValorSaldoInicial: $("#iSaldoInicial").val(),
                        NomeInstituicao: $("#iNomeInstituicao").val(),
                        Numero: $("#iNumero").val(),
                        NumeroAgencia: $("#iNumeroAgencia").val(),
                        Ranking: $("#iRanking").length ? $("#iRanking").val() : null
                    };

                    $.post(App.corrigirPathRota(cadastro ? "/contas/cadastrar-conta" : "/contas/alterar-conta"), { entrada: ativo })
                        .done(function (feedbackResult) {
                            let feedback = Feedback.converter(feedbackResult);

                            if (feedback.tipo == "SUCESSO") {
                                AppModal.ocultar();
                                
                                feedback.exibir(function () {
                                    _carregarAtivos(ativo.Tipo == "4" ? "DIV" : "FII");
                                });
                            }
                            else
                                feedback.exibir();
                        })
                        .fail(function (jqXhr) {
                            let feedback = Feedback.converter(jqXhr.responseJSON);
                            feedback.exibir();
                        });
                }
            });

            $('#sTipo').select2({
                allowClear: false,
                placeholder: "Selecione um tipo",
                dropdownParent: $('.jc-bs3-container'),
                minimumResultsForSearch: -1
            }).on("change", function () {
                $(this).valid();
            });

            $('#iRanking').inputmask({
                mask: "9",
                repeat: 2,
                greedy: false
            });
        });
    };

    var _excluirAtivo = function (id, tipo) {
        $.post(App.corrigirPathRota("/renda-variavel/excluir-ativo?id=" + id), function (feedbackResult) {
            let feedback = Feedback.converter(feedbackResult);

            if (feedback.tipo == "SUCESSO") {
                _carregarAtivos(tipo);
            }

            feedback.exibir();
        })
            .fail(function (jqXhr) {
                let feedback = Feedback.converter(jqXhr.responseJSON);
                feedback.exibir();
            });
    };

    //== Public Functions
    return {
        init: function () {
            _carregarAtivos("DIV");
            _carregarAtivos("FII");


            $("#btn-cadastrar-renda-variavel").click(function () {
                _manterAtivo();
            });
        },

        atualizar: function () {
            _carregarAtivos("DIV");
            _carregarAtivos("FII");
        }
    };
}();

jQuery(document).ready(function () {
    RendaVariavelAtivos.init();
});