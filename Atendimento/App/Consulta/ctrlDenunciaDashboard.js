SSPApp.controller('ctrlConsultarMacro', ['$scope', '$http', '$controller', '$uibModal', '$timeout', '$window', 'uiGridConstants', '$cookies', 'baseURL', function ($scope, $http, $controller, $uibModal, $timeout, $window, uiGridConstants, $cookies, baseURL) {

    $scope.denuncia = null;

    // Recupera a informação da denúncia
    $scope.carregarDenuncia = function () {
        var user = $cookies.getObject('retorno');

        if (user != null) {
            var serv = baseURL + '/api/Consulta/abrirDenunciaMacro';
            var met = 'GET';
            var head = { 'Content-Type': 'application/json' };
            $scope.denuncia = null;

            $http({ method: met, url: serv, headers: head })
            .then(
                function successCallback(response) {
                    $scope.denuncia = response.data;

                    $scope.vazio = angular.equals({}, $scope.denuncia);
                }
                , function errorCallback(response) {

                    $scope.denuncia = {};
                    $scope.vazio = angular.equals({}, $scope.denuncia);
                });
        }
        else {
            ajaxError('Não foi possível abrir a Macro das denúncias.');
        }
    };

    // Função que irá ser chamada ao abrir a página
    $scope.carregarDenuncia();

}]);