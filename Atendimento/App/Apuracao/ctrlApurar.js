SSPApp.controller('ctrlApurar', ['$scope', '$http', '$controller', '$uibModal', '$timeout', '$window', 'uiGridConstants', '$cookies', 'baseURL', function ($scope, $http, $controller, $uibModal, $timeout, $window, uiGridConstants, $cookies, baseURL) {

    $scope.listaApuracao = null;

    $scope.changeBackground = function(value)
    {
        if (value >= 5 && value < 10)
            return 'expirarWarn';

        if (value >= 10)
            return 'expirado';

        return 'expirarOk';
    }

    $scope.gridApuracao = {
        columnDefs: [
           {
               name: "EDITAR", displayName: '', enableCellEdit: false, minWidth: 40, maxWidth: 60, enableFiltering: false,
               cellTemplate: '<div style="text-align: center; padding: 5px 0 5px 0;"><button type="button" class="btn fa fa-edit bg-yellow" ng-click="grid.appScope.abrirDenuncia(row.entity.ano_denuncia,row.entity.numero_denuncia)"></button></div>'
           },
            { name: 'denuncia', displayName: 'Protocolo', minWidth: 60, Width: 60, cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) { return $scope.changeBackground(row.entity.tempo_expirar); } },
            { name: 'cpf_denunciante', displayName: 'CPF do Denunciante', cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) { return $scope.changeBackground(row.entity.tempo_expirar); } },
            { name: 'data_inclusao', displayName: 'Data de inclusão', type: 'date', cellFilter: "date: 'dd/MM/yyyy HH:mm:ss'", cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) { return $scope.changeBackground(row.entity.tempo_expirar); } },
            { name: 'data_ocorrencia', displayName: 'Data de ocorrência', type: 'date', cellFilter: "date: 'dd/MM/yyyy'", cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) { return $scope.changeBackground(row.entity.tempo_expirar); } },
            { name: 'data_expiracao', displayName: 'Data de expiração', type: 'date', cellFilter: "date: 'dd/MM/yyyy'", cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) { return $scope.changeBackground(row.entity.tempo_expirar); } },
            { name: 'tipo_crime', displayName: 'Tipo de crime', cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) { return $scope.changeBackground(row.entity.tempo_expirar); } },
            { name: 'orgao', displayName: 'Encaminhamento', cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) { return $scope.changeBackground(row.entity.tempo_expirar); } },
            { name: 'motivo_retorno', displayName: 'Retorno', cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) { return $scope.changeBackground(row.entity.tempo_expirar); } },
            { name: 'status', displayName: 'Situação', cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) { return $scope.changeBackground(row.entity.tempo_expirar); } },
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
            var serv = baseURL + '/api/Apuracao/listarApuracao';
            var met = 'GET';
            var head = { 'Content-Type': 'application/json' };
            //var data = { idOrgao: user.ID_ORGAO, civil: user.FLAG_CIVIL, militar: user.FLAG_MILITAR };
            var data = { idUsuario: user.ID_USUARIO, civil: user.FLAG_CIVIL, militar: user.FLAG_MILITAR };
            $scope.listaApuracao = null;

            $http({ method: met, url: serv, headers: head, params: data })
            .then(
                function successCallback(response) {
                    $scope.listaApuracao = response.data;
                    $scope.gridApuracao.data = response.data;
                }
                , function errorCallback(response) {
                    ajaxError(response.data);
                });
        }
        else {
            ajaxError('Não foi possível carregar a lista de denúncias.');
        }
    };

    // Atualiza as informações da denúncia que foi selecionada para ser aberta
    $scope.atualizarDenuncia = function (ano, num) {

        var user = $cookies.getObject('retorno');

        if (user != null) {
            var serv = baseURL + '/api/Apuracao/atualizarApuracao';
            var met = 'POST';
            var head = { 'Content-Type': 'application/json' };
            var data = { ano: ano, numero: num, idAtend: user.ID_USUARIO, civil: user.FLAG_CIVIL, militar: user.FLAG_MILITAR };

            $http({ method: met, url: serv, headers: head, params: data })
            .then(
                function successCallback(response) {
                    $cookies.put('anoApurar', ano, { path: '/' });
                    $cookies.put('numApurar', num, { path: '/' });
                    $window.location.href = baseURL + '/Apuracao/ApuraDenuncia';
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
        var checkAno = $cookies.get('anoApurar');
        var checkNum = $cookies.get('numApurar');

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