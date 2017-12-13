SSPApp.controller('ctrlMenu', ['$scope', '$http', '$controller', '$uibModal', '$timeout', '$window', 'uiGridConstants', '$cookies', 'baseURL', function ($scope, $http, $controller, $uibModal, $timeout, $window, uiGridConstants, $cookies, baseURL) {

    $scope.usuario = null;
    $scope.atendimento = null;
    $scope.apuracao = null;
    $scope.civil = null;
    $scope.militar = null;

    // Verificar o grupo do usuário e setar as variáveis de acesso e organização do sistema conforme o grupo
    $scope.carregarAcesso = function () {
        var user = $cookies.getObject('retorno');
        $scope.atendimento = false;
        $scope.apuracao = false;
        $scope.civil = false;
        $scope.militar = false;

        if (user != null)
        {
            $scope.usuario = user;
        }
    }

    $scope.logout = function () {
        var serv = baseURL + '/api/UsuarioAPI/Logout'
        var met = 'POST';
        var head = { 'Content-Type': 'application/json' };

        $http({ method: met, url: serv, headers: head })
            .then(
            function successCallback(response) {
                // Remover todos os cookies
                var cks = $cookies.getAll();
                angular.forEach(cks, function (v, k) {
                    $cookies.remove(k, { path: '/' });
                });
                // Remove as informações do usuário logado
                $scope.usuario = null;
                window.location.href = baseURL + '/Atendimento/Index';
            }
            , function errorCallback(response) {
                ajaxError(response.data);
            });
    }

    $scope.carregarAcesso();

}]);