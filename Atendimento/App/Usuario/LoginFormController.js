
"use strict";
var LoginFormController;

SSPApp.controller('LoginFormController', ['$scope', '$http', '$controller', '$uibModal', '$timeout', '$window', 'uiGridConstants', '$cookies', 'baseURL', function ($scope, $http, $controller, $uibModal, $timeout, $window, uiGridConstants, $cookies, baseURL) {
    angular.extend(this, $controller('BaseListController', { $scope: $scope }));
    LoginFormController = $scope;

    $scope.Logar = function () {
        var url = baseURL + '/api/UsuarioAPI/Logar'

        $http.post(url, $scope.RegistroUsuarioViewModel)
            .then(function success(response) {
                console.log("Create Success", response);

                if (response.statusText == "OK") {
                    $cookies.putObject('retorno', response.data, { path: '/' });
                    window.location.href = baseURL + "/Atendimento/Index";
                } else {
                    ajaxError(response.data);
                }
            }, function errorCallback(response) {
                console.log("Create Error", response);
                ajaxError(response.data);
            });
    };
}]);