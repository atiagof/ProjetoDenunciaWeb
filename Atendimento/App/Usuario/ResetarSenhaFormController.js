"use strict";
var RegistroFormController;
SSPApp.controller("RegistroFormController", ['$scope', '$http', '$controller', '$uibModal', '$timeout', '$window', 'uiGridConstants', 'baseURL', function ($scope, $http, $controller, $uibModal, $timeout, $window, uiGridConstants, baseURL) {

    angular.extend(this, $controller('BaseListController', { $scope: $scope }));

    RegistroFormController = $scope;



    $scope.ResetarSenha = function () {

            
        if ($scope.RegistroUsuarioViewModel.NovaSenha == "") {

            return false;
            
        }


        if ($scope.RegistroUsuarioViewModel.NovaSenha != $scope.RegistroUsuarioViewModel.ConfirmaNovaSenha) {



            BootstrapDialog.show({
                message: '“A senha nova e a confirmação da senha não conferem, por favor, insira novamente a informação” ',

                buttons: [
                     {
                         label: 'OK',
                         action: function (dialogItself) {
                             dialogItself.close();
                         }
                     }]
            });

            return false;

        }


        //if (RegistroUsuarioViewModel)
        var url = baseURL + '/api/UsuarioAPI/ResetarSenha';
        $http.post(url, $scope.RegistroUsuarioViewModel)
                 .then(function success(response) {
                     console.log("Create Success", response);

                     BootstrapDialog.show({
                         message: 'Senha alterada com sucesso!',

                         buttons: [
                              {
                                label: 'OK',
                                action: function (dialogItself) {
                                    dialogItself.close();
                                }
                            }]
                     });

 
                 }, function errorCallback(response) {
                     console.log("Create Error", response);

                 });
       
    };





}]);

