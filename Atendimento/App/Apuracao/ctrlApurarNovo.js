var ctrlApurarNovo;
SSPApp.controller('ctrlApurarNovo', ['$scope', '$http', '$controller', '$uibModal', '$timeout', '$window', 'uiGridConstants', '$cookies', 'baseURL', function ($scope, $http, $controller, $uibModal, $timeout, $window, uiGridConstants, $cookies, baseURL) {
    angular.extend(this, $controller('BaseListController', { $scope: $scope }));

    ctrlApurarNovo = $scope;
    $scope.Expand = '&$expand=tipo_crime,orgao';
    $scope.gridOptions.enableFiltering = true;

    $scope.gridOptions.columnDefs = [
        {
            name: "EDITAR", displayName: '', enableCellEdit: false, minWidth: 40, maxWidth: 60, enableFiltering: false,
            cellTemplate: '<div style="text-align: center; padding: 5px 0 5px 0;"><button type="button" class="btn fa fa-edit bg-yellow" ng-click="grid.appScope.abrirDenuncia(row.entity.ano_denuncia,row.entity.numero_denuncia)"></button></div>'
        },
            { name: 'denuncia', displayName: 'Protocolo', minWidth: 60, Width: 60 },
            { name: 'cpf_denunciante', displayName: 'CPF do Denunciante' },
            { name: 'data_inclusao', filterHeaderTemplate : $scope.defaultDateFilterTemplate, displayName: 'Data de inclusão', type: 'date', cellFilter: "date: 'dd/MM/yyyy HH:mm:ss'" },
            { name: 'data_ocorrencia', displayName: 'Data de ocorrência', type: 'date', cellFilter: "date: 'dd/MM/yyyy'" },
            { name: 'tipo_crime.DESCRICAO_TIPO_CRIME', displayName: 'Assunto' },
            { name: 'orgao.NOME_ORGAO', displayName: 'Encaminhamento' },
            { name: 'motivo_retorno', displayName: 'Retorno' },
            { name: 'status', displayName: 'Situação' },
    ];


    $scope.listaApuracao = null;

    $scope.gridApuracao = {
        columnDefs: [
           
        ],
               
        enableFiltering: true,
        enableRowSelection: false,
        enableColumnMenus: false,
        multiSelect: false,
        selectionRowHeaderWidth: 35,
        rowHeight: 35,
        height: 500,
    };

    //$scope.carregarLista = function () {
    //    var user = $cookies.getObject('retorno');

    //    if (user != null) {
    //        var serv = baseURL + '/odata/DenunciaApuracaoModel';
    //        var met = 'GET';
    //        var head = { 'Content-Type': 'application/json' };            
    //        $scope.listaApuracao = null;

    //        $http({ method: met, url: serv, headers: head })
    //        .then(
    //            function successCallback(response) {
    //                $scope.listaApuracao = response.data;
    //                $scope.gridApuracao.data = response.data;
    //            }
    //            , function errorCallback(response) {
    //                ajaxError(response.data);
    //            });
    //    }
    //    else {
    //        ajaxError('Não foi possível carregar a lista de denúncias.');
    //    }
    //};

    // Atualiza as informações da denúncia que foi selecionada para ser aberta
    $scope.atualizarDenuncia = function (ano, num) {

        var user = $cookies.getObject('retorno');

        if (user != null) {
            var serv = baseURL + '/api/Apuracao/atualizarApuracao';
            var met = 'POST';
            var head = { 'Content-Type': 'application/json' };
            var data = { ano: ano, numero: num, idAtend: user.data.ID_USUARIO, civil: user.data.FLAG_CIVIL, militar: user.data.FLAG_MILITAR };

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

    //$scope.carregarLista();

}]);