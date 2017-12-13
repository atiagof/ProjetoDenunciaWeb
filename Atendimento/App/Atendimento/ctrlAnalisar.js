SSPApp.controller('ctrlAnalisar', ['$scope', '$http', '$controller', '$uibModal', '$timeout', '$window', 'uiGridConstants', '$cookies', 'baseURL', function ($scope, $http, $controller, $uibModal, $timeout, $window, uiGridConstants, $cookies, baseURL) {

    $scope.listaAnalise = null;

    $scope.gridAnalise = {
        columnDefs: [
            {
                name: "EDITAR", displayName: '', enableCellEdit: false, minWidth: 40, maxWidth: 60, enableFiltering: false,
                cellTemplate: '<div style="text-align: center; padding: 5px 0 5px 0;"><button type="button" class="btn fa fa-edit bg-yellow" ng-click="grid.appScope.abrirDenuncia(row.entity.ano_denuncia,row.entity.numero_denuncia)"></button></div>'
            },
            { name: 'denuncia', displayName: 'Protocolo', minWidth: 60, Width: 60 },
            { name: 'cpf_denunciante', displayName: 'CPF do Denunciante' },
            { name: 'data_inclusao', displayName: 'Data de inclusão', type: 'date', cellFilter: "date: 'dd/MM/yyyy'" },
            { name: 'tipo_crime', displayName: 'Tipo de crime' },
            { name: 'status', displayName: 'Situação' },
            { name: 'just_reativo', displayName: 'Reativação' }
        ],

        enableFiltering: true,
        enableRowSelection: false,
        enableColumnMenus: false,
        multiSelect: false,
        selectionRowHeaderWidth: 35,
        rowHeight: 35,
        height: 500,
    };

    // Carrega a lista de denúncias que necessitam ser analisadas
    $scope.carregarLista = function () {
        var user = $cookies.getObject('retorno');

        if (user != null)
        {
            var serv = baseURL + '/api/Atendimento/listarAnalise';
            var met = 'GET';
            var head = { 'Content-Type': 'application/json' };
            var data = { civil: user.FLAG_CIVIL, militar: user.FLAG_MILITAR };
            $scope.listaAnalise = null;

            $http({ method: met, url: serv, headers: head, params: data })
            .then(
                function successCallback(response) {
                    $scope.listaAnalise = response.data;
                    $scope.gridAnalise.data = response.data;
                }
                , function errorCallback(response) {
                    ajaxError(response.data);
                });
        }
        else
        {
            ajaxError('Não foi possível carregar a lista de denúncias.');
        }
    };

    // Atualiza as informações da denúncia que foi selecionada para ser aberta
    $scope.atualizarDenuncia = function (ano, num) {
        var user = $cookies.getObject('retorno');

        if (user != null)
        {
            var serv = baseURL + '/api/Atendimento/atualizarAnalise';
            var met = 'POST';
            var head = { 'Content-Type': 'application/json' };
            var data = { ano: ano, numero: num, idAtend: user.ID_USUARIO, civil: user.FLAG_CIVIL, militar: user.FLAG_MILITAR };

            $http({ method: met, url: serv, headers: head, params: data })
            .then(
                function successCallback(response) {
                    $cookies.put('anoAnalise', ano, { path: '/' });
                    $cookies.put('numAnalise', num, { path: '/' });
                    $window.location.href = baseURL + '/Atendimento/AnaliseDenuncia';
                }
                , function errorCallback(response) {
                    ajaxError(response.data);
                });
        }
        else
        {
            ajaxError('Não foi possível abrir a denúncia.');
        }
    };

    $scope.abrirDenuncia = function (ano, numero) {
        var checkAno = $cookies.get('anoAnalise');
        var checkNum = $cookies.get('numAnalise');

        if (checkAno != null || checkNum != null)
        {
            if (checkAno == ano && checkNum == numero)
                $scope.atualizarDenuncia(ano, numero);
            else
                ajaxError('Finalize o trabalho da denúncia D' + checkNum + '/' + checkAno + ' para abrir uma nova.');
        }
        else
        {
            $scope.atualizarDenuncia(ano, numero);
        }
    };

    $scope.carregarLista();

}]);