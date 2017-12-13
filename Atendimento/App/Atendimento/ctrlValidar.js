SSPApp.controller('ctrlValidar', ['$scope', '$http', '$controller', '$uibModal', '$timeout', '$window', 'uiGridConstants', '$cookies', 'baseURL', function ($scope, $http, $controller, $uibModal, $timeout, $window, uiGridConstants, $cookies, baseURL) {

    $scope.listaValidacao = null;

    $scope.gridValidacao = {
        columnDefs: [
            {
                name: "EDITAR", displayName: '', enableCellEdit: false, minWidth: 60, maxWidth: 100, enableFiltering: false,
                cellTemplate: '<div style="text-align: center; padding: 5px 0 5px 0;"><button type="button" class="btn fa fa-edit bg-yellow" ng-click="grid.appScope.abrirDenuncia(row.entity.ano_denuncia,row.entity.numero_denuncia)"></button></div>'
            },
            { name: 'denuncia', displayName: 'Protocolo' },
            { name: 'cpf_denunciante', displayName: 'CPF do Denunciante' },
            { name: 'data_inclusao', displayName: 'Data de inclusão', type: 'date', cellFilter: "date: 'dd/MM/yyyy HH:mm:ss'" },
            { name: 'data_ocorrencia', displayName: 'Data da ocorrência', type: 'date', cellFilter: "date: 'dd/MM/yyyy'" },
            { name: 'data_resposta', displayName: 'Data da resposta', type: 'date', cellFilter: "date: 'dd/MM/yyyy HH:mm:ss'" },
            { name: 'orgao', displayName: 'Encaminhamento' },
            { name: 'tipo_crime', displayName: 'Tipo de crime' },
            { name: 'resposta', displayName: 'Resposta' },
            { name: 'status', displayName: 'Situação' },
        ],

        enableFiltering: true,
        enableRowSelection: false,
        enableColumnMenus: false,
        multiSelect: false,
        selectionRowHeaderWidth: 35,
        rowHeight: 35,
        height: 500,
    };

    $scope.carregarLista = function () {
        var user = $cookies.getObject('retorno');

        if (user != null) {
            var serv = baseURL + '/api/Atendimento/listarValidacao';
            var met = 'GET';
            var head = { 'Content-Type': 'application/json' };
            var data = { civil: user.FLAG_CIVIL, militar: user.FLAG_MILITAR };
            $scope.listaAnalise = null;

            $http({ method: met, url: serv, headers: head, params: data })
            .then(
                function successCallback(response) {
                    $scope.listaValidacao = response.data;
                    $scope.gridValidacao.data = response.data;
                }
                , function errorCallback(response) {
                    ajaxError(response.data);
                });
        }
        else {
            ajaxError('Não foi possível carregar a lista de validação.');
        }
    };

    // Atualiza as informações da denúncia que foi selecionada para ser aberta
    $scope.atualizarDenuncia = function (ano, num) {
        var user = $cookies.getObject('retorno');

        if (user != null) {
            var serv = baseURL + '/api/Atendimento/atualizarValidacao';
            var met = 'POST';
            var head = { 'Content-Type': 'application/json' };
            var data = { ano: ano, numero: num, idAtend: user.ID_USUARIO, civil: user.FLAG_CIVIL, militar: user.FLAG_MILITAR };

            $http({ method: met, url: serv, headers: head, params: data })
            .then(
                function successCallback(response) {
                    $cookies.put('anoValidar', ano, { path: '/' });
                    $cookies.put('numValidar', num, { path: '/' });
                    $window.location.href = baseURL + '/Atendimento/ValidaDenuncia';
                }
                , function errorCallback(response) {
                    ajaxError(response.data);
                });
        }
        else {
            ajaxError('Não foi possível abrir a denúncia.');
        }
    };

    $scope.abrirDenuncia = function (ano, numero) {
        var checkAno = $cookies.get('anoValidar');
        var checkNum = $cookies.get('numValidar');

        if (checkAno != null || checkNum != null) {
            if (checkAno == ano && checkNum == numero)
                $scope.atualizarDenuncia(ano, numero);
            else
                ajaxError('Finalize o trabalho da denúncia D' + checkNum + '/' + checkAno + ' para abrir uma nova.');
        }
        else {
            $scope.atualizarDenuncia(ano, numero);
        }
    };

    $scope.carregarLista();

}]);