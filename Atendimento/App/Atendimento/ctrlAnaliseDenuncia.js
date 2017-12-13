SSPApp.controller('ctrlAnaliseDenuncia', ['$scope', '$http', '$controller', '$uibModal', '$timeout', '$window', 'uiGridConstants', '$cookies', 'baseURL', function ($scope, $http, $controller, $uibModal, $timeout, $window, uiGridConstants, $cookies, baseURL) {

    $scope.denuncia = null;
    $scope.vazio = null;
    $scope.descarte = '';
    $scope.lim_descarte = 1000;
    $scope.orgaos = null;
    $scope.encOrgao = {};
    $scope.motivo = {};

    // Carrega as informações de acordo com os dados passados
    $scope.carregar = function () {
        var ano = $cookies.get('anoAnalise');
        var numero = $cookies.get('numAnalise');

        if (ano != null && numero != null) {
            $scope.ano = ano;
            $scope.numero = numero;
            $scope.carregarMotivos();
            $scope.carregarDenuncia(ano, numero);
        }
        else {
            $scope.vazio = true;
        }
    }

    // Recupera a informação da denúncia
    $scope.carregarDenuncia = function (ano, num) {
        var user = $cookies.getObject('retorno');

        if (user != null) {
            var serv = baseURL + '/api/Atendimento/abrirDenuncia';
            var met = 'GET';
            var head = { 'Content-Type': 'application/json' };
            var data = { ano: ano, numero: num, civil: user.FLAG_CIVIL, militar: user.FLAG_MILITAR }
            $scope.denuncia = null;

            $http({ method: met, url: serv, headers: head, params: data })
            .then(
                function successCallback(response) {
                    $scope.denuncia = response.data;
                    $scope.vazio = angular.equals({}, $scope.denuncia);
                    $scope.carregarOrgaos();
                }
                , function errorCallback(response) {
                    ajaxError(response.data);
                    $scope.denuncia = {};
                    $scope.vazio = angular.equals({}, $scope.denuncia);
                });
        }
        else {
            ajaxError('Não foi possível abrir a denúncia.');
        }
    };

    // carregar anexos
    $scope.carregarDenunciaAnexo = function (ano, num) {
        var user = $cookies.getObject('retorno');

        if (user != null) {
            var serv = baseURL + '/odata/DenunciaAnexo?$filter=ANO_DENUNCIA eq ' + ano + ' and NUM_DENUNCIA eq ' + num;''
            var met = 'GET';
            var head = { 'Content-Type': 'application/json' };
            var data = { ano: ano, numero: num, civil: user.FLAG_CIVIL, militar: user.FLAG_MILITAR }
            
            $http({ method: met, url: serv, headers: head})
            .then(
                function successCallback(response) {
                    $scope.anexos = response.data.value;
                }
                , function errorCallback(response) {
                    ajaxError(response.data.value);
                });
        }
        else {
            ajaxError('Não foi possível abrir a denúncia.');
        }
    };

    // Recupera a lista de orgãos
    $scope.carregarOrgaos = function () {
        var user = $cookies.getObject('retorno');

        if (user != null)
        {
            var serv = '';
            var met = 'GET';
            var head = { 'Content-Type': 'application/json' };
            var civil = user.FLAG_CIVIL;
            var militar = user.FLAG_MILITAR;
            $scope.orgaos = null;

            if (civil)
            {
                serv = baseURL + '/api/Atendimento/listarDelegacias';

                if ($scope.denuncia.id_orgao > 0)
                    $scope.encOrgao.ID = $scope.denuncia.id_orgao;
            }

            if (militar)
            {
                serv = baseURL + '/api/Atendimento/listarBatalhoes';

                if ($scope.denuncia.id_orgao > 0)
                    $scope.encOrgao.ID = $scope.denuncia.id_orgao;
            }

            $http({ method: met, url: serv, headers: head })
            .then(
                function successCallback(response) {
                    $scope.orgaos = response.data;
                }
                , function errorCallback(response) {
                    $scope.orgaos = null;
                    ajaxError(response.data);
                });
        }
        else {
            ajaxError('Não foi possível carregar os orgãos.')
        }
    }

    // Recupera a lista de motivos de descarte
    $scope.carregarMotivos = function () {

    }

    // Alteração do motivo do descarte
    $scope.selMotivo = function () {
        $scope.descarte = $scope.motivo.descricao;
    }

    // Função para exibir o anexo dentro do modal
    $scope.exibirAnexo = function (desc_anexo) {
        if (desc_anexo != null) {
            $scope.anexo_modal = desc_anexo;

            $('#anexoModal').modal({
            
            });
        }
        else {
            ajaxError('Não foi possível visualizar o arquivo.');
        }
    }

    // Função para realizar o download do Anexo
    $scope.baixarAnexo = function (down_anexo) {
        if (down_anexo != null) {
            var filePath = baseURL + '/' + down_anexo.descricao;
            var met = 'GET';
            var response = 'arraybuffer';
            var file = down_anexo.nome + down_anexo.extensao;

            $http({ method: met, url: filePath, responseType: response, downfile: file })
            .then(
                function successCallback(response)
                {
                    var data = response.data;
                    var octetStreamMime = 'application/octet-stream';
                    var success = false;

                    // Get the headers
                    headers = response.headers();

                    // Get the filename from the x-filename header or default to "download.bin"
                    //var filename = headers['x-filename'] || 'download.bin';
                    var filename = response.config.downfile;

                    // Determine the content type from the header or default to "application/octet-stream"
                    var contentType = headers['content-type'] || octetStreamMime;

                    try {
                        // Try using msSaveBlob if supported
                        console.log("Trying saveBlob method ...");
                        var blob = new Blob([data], { type: contentType });
                        if (navigator.msSaveBlob)
                            navigator.msSaveBlob(blob, filename);
                        else {
                            // Try using other saveBlob implementations, if available
                            var saveBlob = navigator.webkitSaveBlob || navigator.mozSaveBlob || navigator.saveBlob;
                            if (saveBlob === undefined) throw "Not supported";
                            saveBlob(blob, filename);
                        }
                        console.log("saveBlob succeeded");
                        success = true;
                    } catch (ex) {
                        console.log("saveBlob method failed with the following exception:");
                        console.log(ex);
                    }

                    if (!success) {
                        // Get the blob url creator
                        var urlCreator = window.URL || window.webkitURL || window.mozURL || window.msURL;
                        if (urlCreator) {
                            // Try to use a download link
                            var link = document.createElement('a');
                            if ('download' in link) {
                                // Try to simulate a click
                                try {
                                    // Prepare a blob URL
                                    console.log("Trying download link method with simulated click ...");
                                    var blob = new Blob([data], { type: contentType });
                                    var url = urlCreator.createObjectURL(blob);
                                    link.setAttribute('href', url);

                                    // Set the download attribute (Supported in Chrome 14+ / Firefox 20+)
                                    link.setAttribute("download", filename);

                                    // Simulate clicking the download link
                                    var event = document.createEvent('MouseEvents');
                                    event.initMouseEvent('click', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
                                    link.dispatchEvent(event);
                                    console.log("Download link method with simulated click succeeded");
                                    success = true;

                                } catch (ex) {
                                    console.log("Download link method with simulated click failed with the following exception:");
                                    console.log(ex);
                                }
                            }

                            if (!success) {
                                // Fallback to window.location method
                                try {
                                    // Prepare a blob URL
                                    // Use application/octet-stream when using window.location to force download
                                    console.log("Trying download link method with window.location ...");
                                    var blob = new Blob([data], { type: octetStreamMime });
                                    var url = urlCreator.createObjectURL(blob);
                                    window.location = url;
                                    console.log("Download link method with window.location succeeded");
                                    success = true;
                                } catch (ex) {
                                    console.log("Download link method with window.location failed with the following exception:");
                                    console.log(ex);
                                }
                            }

                        }
                    }

                    if (!success) {
                        // Fallback to window.open method
                        console.log("No methods worked for saving the arraybuffer, using last resort window.open");
                        window.open(httpPath, '_blank', '');
                    }
                }
                , function errorCallback(data, status)
                {
                    ajaxError('Download do arquivo retornou erro: ' + status);
                }
            );
        }
        else {
            ajaxError('Não foi possível realizar o download do arquivo.');
        }
    }

    // Função para checar o limite do preenchimento do motivo do descarte
    $scope.checarMotivo = function () {
        if ($scope.descarte.length > $scope.lim_descarte)
            $scope.descarte = $scope.descarte.slice(0, $scope.lim_descarte);
    }

    // Função para verificar se o usuário é o mesmo marcado na denúncia (true caso igual, false caso diferente)
    $scope.verificarDenuncia = function () {
        var user = $cookies.getObject('retorno');

        if (user != null) {
            if ($scope.denuncia.usuario_atendimento.toUpperCase() != user.NOME.toUpperCase())
                return true;
            else
                return false;
        }
        else {
            ajaxError('Não foi possível verificar a denúncia.');
        }
    }

    $scope.verificarDescarte = function () {
        if ($scope.verificarDenuncia()) {
            $('#descModal').modal('hide');
            $('#checkDesc').modal({});
        }
        else {
            $scope.descartarDenuncia();
        }
    }

    $scope.verificarEncaminhamento = function () {
        if ($scope.verificarDenuncia()) {
            $('#checkEnc').modal({});
        }
        else {
            $scope.encaminharDenuncia();
        }
    }

    // Realiza o encaminhamento para o orgão mencionado
    $scope.encaminharDenuncia = function () {
        var user = $cookies.getObject('retorno');

        if (user != null)
        {
            if ($scope.encOrgao == null || $scope.encOrgao.ID <= 0 || $scope.encOrgao.ID == null) {
                ajaxError('Selecione a unidade policial que receberá a denúncia!');
            }
            else {
                var serv = baseURL + '/api/Atendimento/encaminharDenuncia';
                var met = 'POST';
                var head = { 'Content-Type': 'application/json' };
                var data = { ano: $scope.ano, numero: $scope.numero, idAtend: user.ID_USUARIO, idOrgao: $scope.encOrgao.ID, civil: user.FLAG_CIVIL, militar: user.FLAG_MILITAR };

                $http({ method: met, url: serv, headers: head, params: data })
                .then(
                    function successCallback(response) {
                        $scope.retornar();
                    }
                    , function errorCallback(response) {
                        ajaxError(response.data);
                    });
            }
        }
        else {
            ajaxError('Não foi possível encaminhar a denúncia.')
        }
    };

    // Marca a denúncia como descartada para entrar na lista de validação
    $scope.descartarDenuncia = function () {
        var user = $cookies.getObject('retorno');

        if (user != null)
        {
            if ($scope.descarte == null || $scope.descarte.trim() == '') {
                ajaxError('Para que a denúncia seja descartada, é necessário preencher a justificativa!');
            }
            else {
                var serv = baseURL + '/api/Atendimento/descartarDenuncia';
                var met = 'POST';
                var head = { 'Content-Type': 'application/json' };
                var data = { ano: $scope.ano, numero: $scope.numero, idAtend: user.ID_USUARIO, just: $scope.descarte, civil: user.FLAG_CIVIL, militar: user.FLAG_MILITAR };
                $http({ method: met, url: serv, headers: head, params: data })
                .then(
                    function successCallback(response) {
                        $scope.retornar();
                    }
                    , function errorCallback(response) {
                        ajaxError(response.data);
                    });
            }
        }
        else
        {
            ajaxError('Não foi possível descartar a denúncia.');
        }
    }

    // Retorna o usuário para a lista de análise
    $scope.retornar = function () {
        $cookies.remove('anoAnalise', { path: '/' });
        $cookies.remove('numAnalise', { path: '/' });
        $window.location.href = baseURL + '/Atendimento/Analisar';
    }

    // Gera o conteúdo da exportação em PDF da denúncia
    $scope.exportar = function () {
        if ($scope.denuncia != null) {
            var conteudo = [];
            var protocolo = 'D' + $scope.numero + $scope.ano;

            // Verificando a opção de sigilo do Denunciante
            var sigilo = '';
            if ($scope.denuncia.denunciante.sigilo == 'Sim')
                sigilo = ' OPTOU ';
            else
                sigilo = ' NÃO OPTOU ';

            // Verificando o Tipo de crime selecionado
            var tipo_crime = '';
            if ($scope.denuncia.flag_tipo_crime)
                tipo_crime = $scope.denuncia.tipo_crime + ' - ' + $scope.denuncia.outro_tipo_crime;
            else
                tipo_crime = $scope.denuncia.tipo_crime;

            // Verificando o Local de fuga selecionado
            var local_fuga = '';
            if ($scope.denuncia.flag_local_fuga)
                local_fuga = $scope.denuncia.local_fuga + ' - ' + $scope.denuncia.descricao_local_fuga;
            else
                local_fuga = $scope.denuncia.local_fuga;

            // Cabeçalho
            conteudo.push({ image: 'brasaoSP', width: 70, height: 77, absolutePosition: { x: 20, y: 25 } });
            conteudo.push({ text: 'GOVERNO DO ESTADO DE SÃO PAULO', style: 'gov_header' });
            conteudo.push({ text: 'SECRETARIA DE ESTADO DA SEGURANÇA PÚBLICA', style: 'ssp_header' });
            conteudo.push({ text: 'DEPA - Delegacia Eletrônica de Proteção Animal', style: 'depa_header' });
            conteudo.push({ text: ' ', style: 'den_space1' });

            // Conteúdo
            conteudo.push({ columns: [{ text: 'Relatório da Denúncia', style: 'den_titleleft' }, { text: new Date().toLocaleDateString(), style: 'den_titleright' }] });
            conteudo.push({ canvas: [{ type: 'line', lineWidth: 2, x1: 0, y1: 5, x2: 516, y2: 5 }] });
            conteudo.push({ text: ' ', style: 'den_space1' });
            conteudo.push({ text: 'Protocolo: ' + protocolo, style: 'den_protocolo' });
            conteudo.push({ text: ' ', style: 'den_space1' });
            conteudo.push({ text: 'O denunciante' + sigilo + 'pelo sigilo dos seus dados pessoais!', style: 'den_sigilo' });
            conteudo.push({ text: ' ', style: 'den_space1' });
            conteudo.push({ text: 'Ocorrência', style: 'den_title' });
            conteudo.push({ canvas: [{ type: 'line', lineWidth: 2, x1: 0, y1: 5, x2: 516, y2: 5 }] });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({
                columns: [
                    { text: [{ text: 'Data do fato: ', bold: true }, { text: $scope.denuncia.data_ocorrencia }], width: '*' },
                    { text: [{ text: 'Hora aproximada: ', bold: true }, { text: $scope.denuncia.periodo }], width: '*' }
                ]
            });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({ text: [{ text: 'Tipo de crime: ', bold: true }, { text: tipo_crime }] });
            conteudo.push({ text: ' ', style: 'den_space2' });

            // Animais
            angular.forEach($scope.denuncia.animais, function (animal, key) {
                conteudo.push({
                    columns: [
                        { text: [{ text: 'Classificação do animal: ', bold: true }, { text: animal.descricao }], width: '*' },
                        { text: [{ text: 'Quantidade: ', bold: true }, { text: animal.quantidade }], width: '*' }
                    ]
                });
                conteudo.push({ text: ' ', style: 'den_space2' });
            });

            conteudo.push({ text: [{ text: 'Relato do fato: ', bold: true }, { text: $scope.denuncia.relato }], style: 'just_text' });
            conteudo.push({ text: ' ', style: 'den_space3' });

            // Endereço do fato
            conteudo.push({ text: 'Endereço', style: 'den_subtitle' });
            conteudo.push({ canvas: [{ type: 'line', color: '#366696' , lineWidth: 1, x1: 0, y1: 5, x2: 516, y2: 5 }] });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({
                columns: [
                    { text: [{ text: 'Tipo de endereço: ', bold: true }, { text: $scope.denuncia.tipo_endereco }], width: '*' },
                    { text: [{ text: 'CEP: ', bold: true }, { text: $scope.denuncia.cep == null ? '' : $scope.denuncia.cep }], width: '*' }
                ]
            });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({
                columns: [
                    { text: [{ text: 'Endereço: ', bold: true }, { text: $scope.denuncia.endereco }], width: '*' },
                    { text: [{ text: 'Número: ', bold: true }, { text: $scope.denuncia.endereco_numero }], width: '*' }
                ]
            });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({
                columns: [
                    { text: [{ text: 'Complemento: ', bold: true }, { text: $scope.denuncia.complemento == null ? '' : $scope.denuncia.complemento }], width: '*' },
                    { text: [{ text: 'Bairro: ', bold: true }, { text: $scope.denuncia.bairro == null ? '' : $scope.denuncia.bairro }], width: '*' }
                ]
            });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({
                columns: [
                    { text: [{ text: 'Cidade: ', bold: true }, { text: $scope.denuncia.cidade }], width: '*' },
                    { text: [{ text: 'Estado: ', bold: true }, { text: $scope.denuncia.estado }], width: '*' }
                ]
            });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({ text: [{ text: 'Ponto de referência: ', bold: true }, { text: $scope.denuncia.endereco_referencia }] });
            conteudo.push({ text: ' ', style: 'den_space3' });

            // Veículos envolvidos
            angular.forEach($scope.denuncia.veiculos, function (veiculo, key) {
                var indVeiculo = key + 1;
                conteudo.push({ text: 'Veículo ' + indVeiculo, style: 'den_subtitle' });
                conteudo.push({ canvas: [{ type: 'line', color: '#366696', lineWidth: 1, x1: 0, y1: 5, x2: 516, y2: 5 }] });
                conteudo.push({ text: ' ', style: 'den_space2' });
                conteudo.push({
                    columns: [
                        { text: [{ text: 'Tipo de veículo: ', bold: true }, { text: veiculo.tipo_veiculo }], width: '*' },
                        { text: [{ text: 'Número da placa: ', bold: true }, { text: veiculo.placa }], width: '*' }
                    ]
                });
                conteudo.push({ text: ' ', style: 'den_space2' });
                conteudo.push({
                    columns: [
                        { text: [{ text: 'Marca: ', bold: true }, { text: veiculo.marca }], width: '*' },
                        { text: [{ text: 'Cor: ', bold: true }, { text: veiculo.cor_veiculo }], width: '*' }
                    ]
                });
                conteudo.push({ text: ' ', style: 'den_space2' });
                conteudo.push({ text: [{ text: 'Modelo: ', bold: true }, { text: veiculo.modelo }] });
                conteudo.push({ text: ' ', style: 'den_space2' });
                conteudo.push({ text: [{ text: 'Observação: ', bold: true }, { text: veiculo.observacoes }], style: 'just_text' });
                conteudo.push({ text: ' ', style: 'den_space3' });
            });

            // Informações adicionais
            conteudo.push({ text: 'Informações adicionais', style: 'den_subtitle' });
            conteudo.push({ canvas: [{ type: 'line', color: '#366696', lineWidth: 1, x1: 0, y1: 5, x2: 516, y2: 5 }] });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({ text: [{ text: 'Foi registrado um Boletim de ocorrência? ', bold: true }, { text: $scope.denuncia.flag_boletim }] });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({ text: [{ text: 'O denunciado porta arma de fogo? ', bold: true }, { text: $scope.denuncia.flag_porte_arma }] });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({ text: [{ text: 'Há possibilidade de ocorrer fuga do local onde o denunciado se encontra? ', bold: true }, { text: local_fuga }] });
            conteudo.push({ text: ' ', style: 'den_space1' });

            // Denunciante
            conteudo.push({ text: 'Denunciante', style: 'den_title' });
            conteudo.push({ canvas: [{ type: 'line', lineWidth: 2, x1: 0, y1: 5, x2: 516, y2: 5 }] });
            conteudo.push({ text: ' ', style: 'den_space3' });
            conteudo.push({ text: 'Informações do(a) Denunciante', style: 'den_subtitle' });
            conteudo.push({ canvas: [{ type: 'line', color: '#366696', lineWidth: 1, x1: 0, y1: 5, x2: 516, y2: 5 }] });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({
                columns: [
                    { text: [{ text: 'Nome: ', bold: true }, { text: $scope.denuncia.denunciante.nome }], width: '*' },
                    { text: [{ text: 'Sexo: ', bold: true }, { text: $scope.denuncia.denunciante.sexo }], width: '*' }
                ]
            });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({
                columns: [
                    { text: [{ text: 'Telefone fixo: ', bold: true }, { text: $scope.denuncia.denunciante.tel_residencial }], width: '*' },
                    { text: [{ text: 'Telefone celular: ', bold: true }, { text: $scope.denuncia.denunciante.tel_celular }], width: '*' }
                ]
            });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({ text: [{ text: 'Endereço de e-mail: ', bold: true }, { text: $scope.denuncia.denunciante.email }] });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({
                columns: [
                    { text: [{ text: 'CPF: ', bold: true }, { text: $scope.denuncia.denunciante.CPF }], width: '*' },
                    { text: [{ text: 'RG: ', bold: true }, { text: $scope.denuncia.denunciante.RG }], width: '*' }
                ]
            });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({
                columns: [
                    { text: [{ text: 'UF emissor: ', bold: true }, { text: $scope.denuncia.denunciante.RG_UF }], width: '*' },
                    { text: [{ text: 'Naturalidade: ', bold: true }, { text: $scope.denuncia.denunciante.naturalidade }], width: '*' }
                ]
            });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({
                columns: [
                    { text: [{ text: 'Data de nascimento: ', bold: true }, { text: $scope.denuncia.denunciante.data_nascimento }], width: '*' },
                    { text: [{ text: 'Estado civil: ', bold: true }, { text: $scope.denuncia.denunciante.estado_civil }], width: '*' }
                ]
            });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({ text: [{ text: 'Profissão: ', bold: true }, { text: $scope.denuncia.denunciante.profissao }] });
            conteudo.push({ text: ' ', style: 'den_space3' });
            conteudo.push({ text: 'Endereço', style: 'den_subtitle' });
            conteudo.push({ canvas: [{ type: 'line', color: '#366696', lineWidth: 1, x1: 0, y1: 5, x2: 516, y2: 5 }] });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({
                columns: [
                    { text: [{ text: 'CEP: ', bold: true }, { text: $scope.denuncia.denunciante.CEP }], width: '*' },
                    { text: [{ text: 'Endereço: ', bold: true }, { text: $scope.denuncia.denunciante.endereco }], width: '*' }
                ]
            });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({
                columns: [
                    { text: [{ text: 'Número: ', bold: true }, { text: $scope.denuncia.denunciante.numero }], width: '*' },
                    { text: [{ text: 'Complemento: ', bold: true }, { text: $scope.denuncia.denunciante.complemento }], width: '*' }
                ]
            });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({
                columns: [
                    { text: [{ text: 'Bairro: ', bold: true }, { text: $scope.denuncia.denunciante.bairro }], width: '*' },
                    { text: [{ text: 'Cidade: ', bold: true }, { text: $scope.denuncia.denunciante.cidade }], width: '*' }
                ]
            });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({
                columns: [
                    { text: [{ text: 'Estado: ', bold: true }, { text: $scope.denuncia.denunciante.estado }], width: '*' },
                    { text: [{ text: 'Ponto de referência: ', bold: true }, { text: $scope.denuncia.denunciante.referencia }], width: '*' }
                ]
            });
            conteudo.push({ text: ' ', style: 'den_space1' });

            // Denunciados
            angular.forEach($scope.denuncia.denunciados, function (denunciado, key) {
                var indDenunciado = key + 1;
                conteudo.push({ text: 'Denunciado ' + indDenunciado, style: 'den_title' });
                conteudo.push({ canvas: [{ type: 'line', lineWidth: 2, x1: 0, y1: 5, x2: 516, y2: 5 }] });
                conteudo.push({ text: ' ', style: 'den_space2' });
                conteudo.push({
                    columns: [
                        { text: [{ text: 'Nome: ', bold: true }, { text: denunciado.nome }], width: '*' },
                        { text: [{ text: 'Apelido: ', bold: true }, { text: denunciado.apelido }], width: '*' }
                    ]
                });
                conteudo.push({ text: ' ', style: 'den_space3' });
                conteudo.push({ text: 'Características', style: 'den_subtitle' });
                conteudo.push({ canvas: [{ type: 'line', color: '#366696', lineWidth: 1, x1: 0, y1: 5, x2: 516, y2: 5 }] });
                conteudo.push({ text: ' ', style: 'den_space2' });
                conteudo.push({
                    columns: [
                        { text: [{ text: 'Sexo: ', bold: true }, { text: denunciado.sexo }], width: '*' },
                        { text: [{ text: 'Idade: ', bold: true }, { text: denunciado.idade }], width: '*' }
                    ]
                });
                conteudo.push({ text: ' ', style: 'den_space2' });
                conteudo.push({
                    columns: [
                        { text: [{ text: 'Estatura: ', bold: true }, { text: denunciado.estatura }], width: '*' },
                        { text: [{ text: 'Tipo físico: ', bold: true }, { text: denunciado.tipo_fisico }], width: '*' }
                    ]
                });
                conteudo.push({ text: ' ', style: 'den_space2' });
                conteudo.push({
                    columns: [
                        { text: [{ text: 'Cor da pele: ', bold: true }, { text: denunciado.cor_pele }], width: '*' },
                        { text: [{ text: 'Cor dos olhos: ', bold: true }, { text: denunciado.cor_olhos }], width: '*' }
                    ]
                });
                conteudo.push({ text: ' ', style: 'den_space2' });

                if (denunciado.flag_tipo_cabelo)
                    conteudo.push({
                        columns: [
                            { text: [{ text: 'Tipo de cabelo: ', bold: true }, { text: denunciado.tipo_cabelo }], width: '*' },
                            { text: [{ text: 'Descrição do tipo de cabelo: ', bold: true }, { text: denunciado.desc_tipo_cabelo }], width: '*' }
                        ]
                    });
                else
                    conteudo.push({ text: [{ text: 'Tipo de cabelo: ', bold: true }, { text: denunciado.tipo_cabelo }] });
                conteudo.push({ text: ' ', style: 'den_space2' });

                if (denunciado.flag_cor_cabelo)
                    conteudo.push({
                        columns: [
                            { text: [{ text: 'Cor do cabelo: ', bold: true }, { text: denunciado.cor_cabelo }], width: '*' },
                            { text: [{ text: 'Descrição da cor do cabelo: ', bold: true }, { text: denunciado.desc_cor_cabelo }], width: '*' }
                        ]
                    });
                else
                    conteudo.push({ text: [{ text: 'Cor do cabelo: ', bold: true }, { text: denunciado.cor_cabelo }] });
                conteudo.push({ text: ' ', style: 'den_space2' });

                if (denunciado.flag_cicatriz)
                    conteudo.push({
                        columns: [
                            { text: [{ text: 'Tem cicatriz? ', bold: true }, { text: denunciado.cicatriz }], width: '*' },
                            { text: [{ text: 'Descrição cicatriz: ', bold: true }, { text: denunciado.desc_cicatriz }], width: '*' }
                        ]
                    });
                else
                    conteudo.push({ text: [{ text: 'Tem cicatriz? ', bold: true }, { text:denunciado.cicatriz  }] });
                conteudo.push({ text: ' ', style: 'den_space2' });

                if (denunciado.flag_tatuagem)
                    conteudo.push({
                        columns: [
                            { text: [{ text: 'Tem tatuagem? ', bold: true }, { text: denunciado.tatuagem }], width: '*' },
                            { text: [{ text: 'Descrição tatuagem: ', bold: true }, { text: denunciado.desc_tatuagem }], width: '*' }
                        ]
                    });
                else
                    conteudo.push({ text: [{ text: 'Tem tatuagem? ', bold: true }, { text: denunciado.tatuagem }] });
                conteudo.push({ text: ' ', style: 'den_space3' });

                conteudo.push({ text: 'Endereço', style: 'den_subtitle' });
                conteudo.push({ canvas: [{ type: 'line', color: '#366696', lineWidth: 1, x1: 0, y1: 5, x2: 516, y2: 5 }] });
                conteudo.push({ text: ' ', style: 'den_space2' });
                conteudo.push({
                    columns: [
                        { text: [{ text: 'Tipo de endereço: ', bold: true }, { text: denunciado.tipo_endereco }], width: '*' },
                        { text: [{ text: 'CEP: ', bold: true }, { text: denunciado.CEP }], width: '*' }
                    ]
                });
                conteudo.push({ text: ' ', style: 'den_space2' });
                conteudo.push({
                    columns: [
                        { text: [{ text: 'Endereço: ', bold: true }, { text: denunciado.endereco }], width: '*' },
                        { text: [{ text: 'Número: ', bold: true }, { text: denunciado.numero }], width: '*' }
                    ]
                });
                conteudo.push({ text: ' ', style: 'den_space2' });
                conteudo.push({
                    columns: [
                        { text: [{ text: 'Complemento: ', bold: true }, { text: denunciado.complemento }], width: '*' },
                        { text: [{ text: 'Bairro: ', bold: true }, { text: denunciado.bairro }], width: '*' }
                    ]
                });
                conteudo.push({ text: ' ', style: 'den_space2' });
                conteudo.push({
                    columns: [
                        { text: [{ text: 'Cidade: ', bold: true }, { text: denunciado.cidade }], width: '*' },
                        { text: [{ text: 'Estado: ', bold: true }, { text: denunciado.estado }], width: '*' }
                    ]
                });
                conteudo.push({ text: ' ', style: 'den_space2' });
                conteudo.push({ text: [{ text: 'Ponto de referência: ', bold: true }, { text: denunciado.endereco_referencia }] });
                conteudo.push({ text: ' ', style: 'den_space1' });
            });

            // Encaminhamento
            conteudo.push({ text: 'Encaminhamento', style: 'den_title' });
            conteudo.push({ canvas: [{ type: 'line', lineWidth: 2, x1: 0, y1: 5, x2: 516, y2: 5 }] });
            conteudo.push({ text: ' ', style: 'den_space2' });
            conteudo.push({ text: [{ text: 'Atendente que está analisando a denúncia: ', bold: true }, { text: $scope.denuncia.usuario_atendimento }] });
            conteudo.push({ text: ' ', style: 'den_space2' });

            if ($scope.denuncia.status == 'Reativada') {
                conteudo.push({ text: [{ text: 'Atendente que reativou a denúncia: ', bold: true }, { text: $scope.denuncia.usuario_reativo }] });
                conteudo.push({ text: ' ', style: 'den_space2' });
                conteudo.push({ text: [{ text: 'Justificativa da reativação: ', bold: true }, { text: $scope.denuncia.just_reativo }], style: 'just_text' });
                conteudo.push({ text: ' ', style: 'den_space2' });
            }

            conteudo.push({ text: [{ text: 'Unidade policial: ', bold: true }, { text: $scope.denuncia.orgao }] });
            conteudo.push({ text: ' ', style: 'den_space1' });

            // Gerando as informações do PDF
            $scope.exportDen = {
                content: conteudo,
                images: { brasaoSP: 'data:image/png;base64,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' },
                styles: {
                    gov_header: { bold: true, alignment: 'center', fontSize: 18 },
                    ssp_header: { alignment: 'center', fontSize: 12 },
                    depa_header: { bold: true, alignment: 'center', fontSize: 14 },
                    den_header: { bold: true, fontSize: 18 },
                    den_titleleft: { bold: true, fontSize: 18, alignment: 'left' },
                    den_titleright: { bold: true, fontSize: 18, alignment: 'right' },
                    den_protocolo: { bold: true, fontSize: 14 },
                    den_title: { bold: true, fontSize: 18 },
                    den_subtitle: { bold: true, fontSize: 16, color: '#366696' },
                    den_sigilo: { bold: true, fontSize: 14, color: '#FF3333', alignment: 'center' },
                    den_space1: { fontSize: 20 },
                    den_space2: { fontSize: 6 },
                    den_space3: { fontSize: 10 },
                    just_text: { alignment: 'justify' }
                }
            };

            // Enviando o download do PDF ao cliente
            pdfMake.createPdf($scope.exportDen).download('Atendimento - ' + protocolo +'.pdf');
        }
    }

    // Função que irá ser chamada ao abrir a página
    $scope.carregar();

}]);