/// <reference path="C:\SSP\WebDenunciaSSP\src\WebDenunciaSSP.Atendimento\Views/Consulta/ConsultarDenuncia.cshtml" />
"use strict";
var RegistroFormController;
SSPApp.controller("RegistroFormController", ['$scope', '$http', '$controller', '$uibModal', '$timeout', '$window', 'uiGridConstants', '$cookies', 'blockUI', 'baseURL', '$q', function ($scope, $http, $controller, $uibModal, $timeout, $window, uiGridConstants, $cookies, blockUI, baseURL, $q) {

    angular.extend(this, $controller('BaseListController', { $scope: $scope }));

    RegistroFormController = $scope;
    RegistroFormController.blockUI = blockUI;

    $scope.listTipoOrgao = [];
    $scope.listGrupo = [];
    $scope.listOrgao = [];
 
    $scope.isCPFValid = function (CPFValue) {

        if (CPFValue != undefined) {
            var cpf = CPFValue.replace(/[^\d]+/g, '');
            if (cpf === '') return false;
            // Elimina CPFs invalidos conhecidos    
            if (cpf.length !== 11 ||
                cpf === "00000000000" ||
                cpf === "11111111111" ||
                cpf === "22222222222" ||
                cpf === "33333333333" ||
                cpf === "44444444444" ||
                cpf === "55555555555" ||
                cpf === "66666666666" ||
                cpf === "77777777777" ||
                cpf === "88888888888" ||
                cpf === "99999999999")
                return false;
            // Valida 1o digito 
            var add = 0;
            for (i = 0; i < 9; i++)
                add += parseInt(cpf.charAt(i)) * (10 - i);
            var rev = 11 - (add % 11);
            if (rev === 10 || rev === 11)
                rev = 0;
            if (rev !== parseInt(cpf.charAt(9)))
                return false;
            // Valida 2o digito 
            add = 0;
            for (var i = 0; i < 10; i++)
                add += parseInt(cpf.charAt(i)) * (11 - i);
            rev = 11 - (add % 11);
            if (rev === 10 || rev === 11)
                rev = 0;
            if (rev !== parseInt(cpf.charAt(10)))
                return false;
            return true;
        }
    };

    $scope.isCPFused = function (CPFValue) {
        if (CPFValue != null) {
            var cpf = CPFValue.replace(/[^\d]+/g, '');

            if ($scope.RegistroUsuarioViewModel.usuarioID == 0 && cpf.length == 11) {
                var serv = baseURL + '/api/UsuarioAPI/verificaUsuarioCPF';
                var met = 'GET';
                var dados = { usuarioCPF: cpf };
                var head = { 'Content-Type': 'application/json' };
                
                $http({ method: met, url: serv, headers: head, params: dados })
                .then(
                    function successCallback(response) {
                        var retorno = response.data;
                        return retorno;
                    }
                    , function errorCallback(response) {
                        ajaxError(response.data);
                        return false;
                    });
            }
        }
    }

    $scope.isCaracterEspecial = function (event) {
        var retorno = true;
        var regex = new RegExp("^[0-9a-zA-Z\b]+$");

        var key = String.fromCharCode(event);
        // event = event.replace(/ +/g, "");

        if (event != "") {
            if (!regex.test(event.trim())) {
                // event.preventDefault();
                retorno = false;
            }
        }

        return  retorno;
    }

    $scope.ExibeAlertaForm = function () {       
        angular.forEach($scope.RegistroUsuarioViewModelForm.$error, function (field) {
            angular.forEach(field, function (errorField) {
                if (errorField.$setTouched) {
                    errorField.$setTouched();
                }
            })
        });
    }

    $scope.isCPFused = function (CPFValue) {
        
        if (CPFValue != null && CPFValue.replace(/[^\d]+/g, '').length == 11) {
            var cpf = CPFValue.replace(/[^\d]+/g, '');

            return $q(function (resolve, reject) {
                var serv = baseURL + '/api/UsuarioAPI/verificaUsuarioCPF';
                var met = 'GET';
                var dados = { usuarioCPF: cpf };
                var head = { 'Content-Type': 'application/json' };

                $http({ method: met, url: serv, headers: head, params: dados })
                .then(
                    function successCallback(response) {
                        var retorno = response.data;

                        if(retorno)
                        {
                            resolve();
                        }
                        else
                        {
                            reject();                        
                        }

                        
                    }
                    , function errorCallback(response) {
                        ajaxError(response.data);
                        reject();                        
                    });
            });
        }
        else
        {
            return true;
        }
    }



    $scope.save = function () {
        blockUI.start();
        blockUI.message('Salvando usuário...');

        if (!$scope.RegistroUsuarioViewModelForm.$valid) {
            $scope.ExibeAlertaForm();
            ajaxError('Preencha os campos obrigatorios marcados com *');
            blockUI.stop();
            return false;
        }

        if (!$scope.isCaracterEspecial($scope.RegistroUsuarioViewModel.usuarioLogin)) {
            $scope.ExibeAlertaForm();
            ajaxError('O campo Login não permite acentuação ou espaço');
            blockUI.stop();
            return false;
        }

        //if (!$scope.isCaracterEspecial($scope.RegistroUsuarioViewModel.usuarioNome)) { 
        //    $scope.ExibeAlertaForm();
        //    ajaxError(' O campo Nome não permite acentuação');
        //    return false;
        //}

        if (!$scope.isCPFValid($scope.RegistroUsuarioViewModel.usuarioCPF)) {
            $scope.ExibeAlertaForm();
            ajaxError('Digite um CPF válido');
            blockUI.stop();
            return false;
        }

        if (!$scope.RegistroUsuarioViewModelForm.usuarioEmail.$valid) {
            $scope.ExibeAlertaForm();
            ajaxError('Email inválido');
            blockUI.stop();
            return false;    
        }

        if ($scope.RegistroUsuarioViewModel.GrupoId == 0) {
            $scope.ExibeAlertaForm();
            ajaxError('Selecione o grupo que o usuário pertence.');
            blockUI.stop();
            return false;        
        }

        if ($scope.RegistroUsuarioViewModel.GrupoId == 6 || $scope.RegistroUsuarioViewModel.GrupoId == 7) {           
            //$scope.RegistroUsuarioViewModel.OrgaoId = RegistroFormController.selectedNode.idOrgao;

            //if ($scope.RegistroUsuarioViewModel.OrgaoId == undefined) {
            //    $scope.ExibeAlertaForm();
            //    ajaxError('Preencha os campos obrigatorios marcados com *')
            //    return false;
            //} else {
            //    $scope.RegistroUsuarioViewModel.OrgaoId = RegistroFormController.selectedNode.idOrgao;
            //}

            if (RegistroFormController.selOrgaos.length <= 0) {
                $scope.ExibeAlertaForm();
                ajaxError('Selecione pelo menos uma Unidade Policial para o usuário.');
                blockUI.stop();
                return false;
            } else {
                $scope.RegistroUsuarioViewModel.Orgaos = [];
                angular.forEach(RegistroFormController.selOrgaos, function (orgao) {
                    $scope.RegistroUsuarioViewModel.Orgaos.push(orgao);
                });
            }
        } else {
            $scope.RegistroUsuarioViewModel.OrgaoId = null;
            $scope.RegistroUsuarioViewModel.Orgaos = null;
        }

        var url = baseURL + '/api/UsuarioAPI/CriarUsuario';
        $http.post(url, $scope.RegistroUsuarioViewModel)
            .then(function success(response) {
                // console.log("Create Success", response);               
                // ajaxError(response.data)
                //  return false;

                blockUI.stop();
                BootstrapDialog.show({
                    message: response.data,
                    buttons: [
                        {
                        label: 'OK',
                        action: function (dialogItself) {
                            window.location.href = baseURL + '/Usuario/ListaUsuario';
                            return false;
                        }
                    }]
                });
                return false;
 
            }, function errorCallback(response) {
                ajaxError(response.data.Message);
                blockUI.stop();
                return false;
            });       
    };

    $scope.TipoUsuarioChanged = function(value){
        if ($scope.RegistroUsuarioViewModel.usuarioID == 0 || $scope.RegistroUsuarioViewModel.usuarioID == undefined) {
            console.log(value);
            $scope.RegistroUsuarioViewModel = {
                TipoUsuario: $scope.RegistroUsuarioViewModel.TipoUsuario
            };
        }
    };

    $scope.GrupoIdChanged = function () {
        $scope.RegistroUsuarioViewModel.OrgaoIdSubTipo == 0;
        $scope.CarregaOrgao();
        // $scope.RegistroUsuarioViewModel.OrgaoId == 0;       
    };

    $scope.OrgaoIdTipoChanged = function () {
        // console.log("Alterou OrgaoIdTipo" + $scope.RegistroUsuarioViewModel.OrgaoIdTipo);
        
        var url = baseURL + '/odata/SubtipoOrgao?$filter=ID_TIPO_ORGAO eq ' + $scope.RegistroUsuarioViewModel.OrgaoIdTipo;

        $http.get(url)
            .then(function (result) {
                $scope["listSubtipoOrgao"] = result.data.value;
        });
    };

    $scope.OrgaoIdSubTipoChanged = function () {
        // console.log("Alterou OrgaoIdSubTipoChanged" + $scope.RegistroUsuarioViewModel.OrgaoIdSubTipo);

        var url = baseURL + '/odata/Orgao?$filter=ID_SUBTIPO_ORGAO eq ' + $scope.RegistroUsuarioViewModel.OrgaoIdSubTipo;

        $http.get(url)
            .then(function (result) {
                $scope["listOrgao"] = result.data.value;
        });
    };

    $scope.OrgaoIdChanged = function () {
        console.log("Alterou OrgaoIdChanged");
    };    

    $scope.gridOptions = {
        saveScroll: true,
        enableColumnMenus: false,
        enableGridMenu: false,
        treeRowHeaderAlwaysVisible: false,
        showColumnFooter: false,
        enableColumnResizing: true,
        enableFiltering: true,
        //paginationPageSizes: [5, 10, 15],
        //paginationPageSize: 5,
        columnDefs : [

               { name: 'ID_USUARIO', displayName: 'ID_USUARIO', enableHiding: false, enableCellEdit: false, visible: false },
               { name: 'NOME_COMPLETO', displayName: 'Nome', enableCellEdit: true },
               { name: 'LOGIN', enableCellEdit: true },
                { name: 'EMAIL', enableCellEdit: true },
                 { name: 'CPF', displayName: 'CPF', enableCellEdit: true },
                  { name: 'RG', enableCellEdit: true },
                   { name: 'RG_UF', displayName: 'RG UF', enableCellEdit: true },
                    {
                        name: 'FLAG_ATIVO', displayName: 'Situação', enableCellEdit: true,
                        cellTemplate: '<div style="text-align: center;">{{ grid.appScope.formataAtivo(COL_FIELD); }} </div>',
                        filterHeaderTemplate: '<div class="ui-grid-filter-container" ng-repeat="colFilter in col.filters"><select class="form-control" ng-model="colFilter.term" ng-options="option.id as option.value for option in colFilter.options"></select></div>',
                        filter: {
                            term: null,
                            options: [{ id: null, value: 'Todos'}, { id: true, value: 'Ativo' }, { id: false, value: 'Inativo' }]
                        },
                    },

                   {
                       name: "Alterar", displayName: "", enableHiding: false, width: 150, enableFiltering: false, enableCellEdit: false,
                       cellTemplate: '<div><input type="button" ng-click="grid.appScope.Alterar(row.entity)" class="ui-grid-cell-contents link" title="Alterar" value="Alterar" /> </div>',
                   },
                   {
                       name: "Inativar", displayName: "", enableHiding: false, width: 150, enableFiltering: false, enableCellEdit: false,
                       cellTemplate: '<div><input type="button" ng-click="grid.appScope.InativarUsuario(row.entity)" class="ui-grid-cell-contents link" title="Inativar" value="Inativar" /> </div>',
                   },
                   {
                       name: "Resetar", displayName: "", enableHiding: false, width: 150, enableFiltering: false, enableCellEdit: false,
                       cellTemplate: '<div><input type="button" ng-click="grid.appScope.ResetarSenha(row.entity)" class="ui-grid-cell-contents link" title="Resetar Senha" value="Resetar Senha" /> </div>',
                   }

        ],

        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
            $scope.gridApi.core.on.sortChanged($scope, $scope.Search);


            $scope.Search();
        }          
    };

    $scope.formataAtivo = function (valor) {

        var Ativo;
        if (valor == true)
            Ativo = 'Ativo';
        else
            Ativo = 'Inativo';

        return Ativo;

    }

    $scope.Alterar = function (row) {
        var ID_USUARIO = row["ID_USUARIO"];
        window.location.href = baseURL + "/Usuario/Registro/" + ID_USUARIO;
    };

    $scope.ResetarSenha = function (row) {
        var user = $cookies.getObject('retorno');
        var data = { ID: row["ID_USUARIO"], CPF: row["CPF"], ID_ADMIN: user.ID_USUARIO, NOME_ADMIN: user.NOME_COMPLETO };

        BootstrapDialog.show({
            message: 'Tem certeza que deseja resetar a senha do usuário \n' +  row["NOME_COMPLETO"],

            buttons: [
                {
                label: 'Sim',
                cssClass: 'btn-primary',
                action: function (dialogItself) {
                    var url = baseURL + '/api/UsuarioAPI/ResetarSenhaUsuario';
                    $http.post(url, data)
                         .then(function success(response) {
                             //console.log("Create Success", response);
                             dialogItself.close();
                        });

                }
            }, {
                label: 'Não',
                action: function (dialogItself) {
                    dialogItself.close();
                }
            }]
        });

    };

    $scope.InativarUsuario = function (row) {
        var user = $cookies.getObject('retorno');
        var data = { ID: row["ID_USUARIO"], CPF: row["CPF"], ID_ADMIN: user.ID_USUARIO, NOME_ADMIN: user.NOME_COMPLETO };

        BootstrapDialog.show({
            message: 'Tem certeza que deseja inativar o usuário \n' + row["NOME_COMPLETO"],
            buttons: [
                {
                    label: 'Sim',
                    cssClass: 'btn-primary',
                    action: function (dialogItself) {
                        var url = baseURL + '/api/UsuarioAPI/InativarUsuario';
                        $http.post(url, data)
                            .then(function success(response) {
                                //console.log("Create Success", response);
                                $scope.ListaUsuarios();
                                dialogItself.close();
                            });
                    }
                },
                {
                    label: 'Não',
                    action: function (dialogItself) {
                        dialogItself.close();
                    }
                }]
        });
    };

    $scope.ListaUsuarios = function () {
        var url = baseURL + '/odata/Usuario';
        $http.get(url)
            .then(function successCallback(response) {
                //var str = JSON.parse(response.d);
                //  console.log("Create Success", response);

                $scope.gridOptions.data = response.data.value;
            }, function errorCallback(response) {
                console.log("Create Error", response);
            });
    };
        
    $scope.ListaUsuarios();

    $scope.Carregalistaorgao = function () {       
        var url = baseURL + '/odata/Orgao';

        $http.get(url).then(function (result) {
            $scope["listOrgao"] = result.data.value;
        });

        var urlSub = baseURL + '/odata/SubtipoOrgao';

        $http.get(urlSub).then(function (result) {
            $scope["listSubtipoOrgao"] = result.data.value;
        });
    };

    $scope.Carregalistaorgao();

    $scope.CurrentPageNumber = 1;
    $scope.TotalPages = 1;
    $scope.TotalPageResults = 10;

    $scope.GoToFirstPage = function () {
        $scope.CurrentPageNumber = 1;
        $scope.Search();
    };

    $scope.GoToPreviousPage = function () {
        $scope.CurrentPageNumber--;
        $scope.Search();
    };

    $scope.GoToPageNumber = function (pageNumber) {
        $scope.CurrentPageNumber = pageNumber;
        $scope.Search();
    };

    $scope.GoToNextPage = function () {
        $scope.CurrentPageNumber++;
        $scope.Search();
    };

    $scope.GoToLastPage = function () {
        $scope.CurrentPageNumber = $scope.TotalPages;
        $scope.Search();
    };

    $scope.BuildFilter = function () {
        var filterQuery = '';

        if (angular.isDefined($scope.filter)) {
      
        }

        return filterQuery.length > 0 ? '&$filter=' + filterQuery : "";
    };

    $scope.BuildOrderBy = function () {
        var orderBy = "";

        if ($scope.gridApi.grid.getColumnSorting().length > 0) {
            //if ($scope.gridApi.grid.getColumnSorting()[0].name == "MUNICIPIO") {
            //    orderBy += "&$orderby=MUNICIPIO/" + $scope.gridApi.grid.getColumnSorting()[0].name + " " + $scope.gridApi.grid.getColumnSorting()[0].sort.direction;                
            //}
            //else
            //{
            orderBy += "&$orderby=" + $scope.gridApi.grid.getColumnSorting()[0].name + " " + $scope.gridApi.grid.getColumnSorting()[0].sort.direction;
            //}
        }

        return orderBy;
    };

    $scope.BaseUrl2 = '/odata/Usuario';
    $scope.Search = function () {
        var url = $scope.BaseUrl2;
        url += '?$count=true'
        $scope.PageNumberValue = $scope.CurrentPageNumber;

        url += '&$skip=' + ($scope.CurrentPageNumber - 1) * $scope.TotalPageResults;
        url += '&$top=' + $scope.TotalPageResults;

        //url += '&$expand=MUNICIPIO'

        url += $scope.BuildFilter();
        url += $scope.BuildOrderBy();

        //url += "&$select=ID,PLACA_RESERVADA,PLACA_OFICIAL,DESTINO_ID,MUNICIPIO,ID_CONTROLE_RESPONSAVEL,STATUS,DATA_VENCIMENTO"

        //ShowLoading();
        $http.get(url)
            .success(function (result) {


                $scope.TotalResults = result["@odata.count"];
                $scope.TotalPages = Math.ceil(result["@odata.count"] / $scope.TotalPageResults);

                console.log(result);

                var data = result.value;

                //for (var item in data) {
                //    //console.log(item)
                //    data[item].STATUS_DESCRIPTION = $scope.GetDescriptionFromDictionary($scope.Dicionarios.Eventos, data[item].STATUS);

                //    data[item].RESPONSAVEL_SOLICITACAO = $scope.GetDescriptionFromDictionary($scope.Dicionarios.Responsavel, data[item].ID_CONTROLE_RESPONSAVEL);

                //    data[item].DESTINO = $scope.GetDescriptionFromDictionary($scope.Dicionarios.Destinos, data[item].DESTINO_ID);
                //    data[item].ID2 = data[item].ID;


                //    data[item].MUNICIPIO_DESCRIPTION = $scope.GetDescriptionFromDictionary($scope.Dicionarios.Municipios, data[item].MUNICIPIO_ID);

                //    if (data[item].DATA_VENCIMENTO == null || data[item].DATA_VENCIMENTO == "null") {
                //        data[item].DATA_VENCIMENTO = new Date(data[item].DATA_VENCIMENTO);
                //    }
                //}            
                $scope.gridOptions.data = data;
            }).finally(function () {
                //HideLoading();
            });
    };

    $scope.nodes = [];
    $scope.lista = [];
    $scope.idTipoPolicia = "2";  

    $scope.CarregaOrgao = function () {
        //console.log($scope.RegistroUsuarioViewModel);

        if ($scope.RegistroUsuarioViewModel.GrupoId == 6) // Civil
            $scope.idTipoPolicia = "2";

        if ($scope.RegistroUsuarioViewModel.GrupoId == 7) // militar
            $scope.idTipoPolicia = "3";

        var url = baseURL + '/api/UsuarioAPI/listarOrgaosPorTipo?idTipo=' + $scope.idTipoPolicia;

        $http.get(url)
            .then(function (result) {
                var list = result.data;
                //checkItem(list);
                checkOrgaos(list);
                $scope.nodes = list;
                //$scope.nodes.children = $scope.nodes.filhosOrgao;
            });
    };

    $scope.carregaOrgaosUsuario = function () {
        if ($scope.RegistroUsuarioViewModel.usuarioID > 0) {
            var serv = baseURL + '/api/UsuarioAPI/CarregarOrgaosUsuario/' + $scope.RegistroUsuarioViewModel.usuarioID;
            var met = 'GET';
            var head = { 'Content-Type': 'application/json' };
            $scope.listaAnalise = null;

            $http({ method: met, url: serv, headers: head })
            .then(
                function successCallback(response) {
                    $scope.RegistroUsuarioViewModel.Orgaos = response.data;
                    RegistroFormController.selOrgaos = response.data;
                    $scope.CarregaOrgao();
                }
                , function errorCallback(response) {
                    ajaxError(response.data);
                });
        }
    }

    function checkItem(list) {
        for (var i in list) {
            if (angular.isDefined(list[i])) {

                if (list[i].idOrgao == RegistroFormController.RegistroUsuarioViewModel.OrgaoId) {
                    list[i].checked = true;
                }

                if (list[i].children) {
                    checkItem(list[i].children);
                }
            }
        }
    };

    function checkOrgaos(lista) {
        angular.forEach(lista, function (orgao) {
            angular.forEach(RegistroFormController.RegistroUsuarioViewModel.Orgaos, function (userOrgao) {
                if (userOrgao == orgao.idOrgao)
                    orgao.checked = true;
            })

            if (orgao.children.length > 0)
                checkOrgaos(orgao.children);
        });
    };
  
    $scope.timeInMs = 0;

    var countUp = function () {
        $scope.timeInMs += 1000;
        $scope.carregaOrgaosUsuario();
        //$scope.CarregaOrgao();
       // $timeout(countUp, 1000);
    }

    $timeout(countUp, 1000);
}]);

////////////////////////////////////////////////////////////////////////////////
/// TREEVIEW  //////////////////////////////////////////////////////////////////
SSPApp.directive('treeView', function ($compile) {
    return {
        restrict: 'E',
        scope: {
            localNodes: '=model',
            selectedNode: '=',
            selOrgaos: '=',
            localClick: '&click'
        },
        link: function (scope, tElement, tAttrs, transclude) {
            scope.selectedNode = {};
            scope.selOrgaos = [];

            var maxLevels = (angular.isUndefined(tAttrs.maxlevels)) ? 10 : tAttrs.maxlevels;
            var hasCheckBox = (angular.isUndefined(tAttrs.checkbox)) ? false : true;

            scope.showItems = [];

            scope.showHide = function (ulId) {
                var hideThis = document.getElementById(ulId);
                var showHide = angular.element(hideThis).attr('class');
                angular.element(hideThis).attr('class', (showHide === 'show' ? 'hide' : 'show'));
            }

            scope.showIcon = function (node, list) {
                return scope.checkIfChildren(node, list);
            }

            scope.checkIfChildren = function (node,list) {
                //uncheckAll(list);

                //if (node.idOrgao == RegistroFormController.RegistroUsuarioViewModel.OrgaoId) {
                //    node.checked = true;
                //}

                return angular.isUndefined(node.children) || node.children.length > 0;
            }

            /////////////////////////////////////////////////
            /// SELECT ALL CHILDRENS
            // as seen at: http://jsfiddle.net/incutonez/D8vhb/5/
            function parentCheckChange(item, localNodes) {
                for (var i in localNodes) {
                    // localNodes[i].checked = false;
                    if (item.idOrgao != localNodes[i].idOrgao) {
                        localNodes[i].checked = false;
                    }                

                    if (localNodes[i].children) {
                        parentCheckChange(localNodes[i].children);
                    }
                }

                //for (var i in item.children) {                
                //    item.children[i].checked = false;

                //    if (item.children[i].children) {
                //        parentCheckChange(item.children[i]);
                //    }
                //}
            }

            function uncheckAll(list) {
                for (var i in list)
                {
                    if (angular.isDefined(list[i])) {
                        list[i].checked = false;

                        if (list[i].children) {
                            uncheckAll(list[i].children);
                        }
                    }
                }
            };

            scope.checkChange = function (node, localNodes) {
                scope.selectedNode = node;
                uncheckAll(localNodes);         
                node.checked = true;
            }

            scope.selectNode = function (node, localNodes) {
                if (node.checked) {
                    scope.selOrgaos.push(node.idOrgao);
                } else {
                    scope.selOrgaos = scope.selOrgaos.filter(
                        function (org) {
                            return org !== node.idOrgao
                        }
                    );
                }                
            }
            /////////////////////////////////////////////////

            function renderTreeView(collection, level, max, parent,orgaoid) {
                var text = '';
                text += '<li ng-repeat="n in ' + collection + '" >';
                text += '<span ng-show="showIcon(n,localNodes)" class="show-hide" ng-click=showHide(n.idOrgao)><i class="fa fa-plus-square"></i></span>';
                text += '<span ng-show="!showIcon(n,localNodes)" style="padding-right: 13px"></span>';

                if (hasCheckBox) {
                    //text += '<input class="tree-checkbox"  type=checkbox ng-model=n.checked ng-change=checkChange(n,localNodes)>';
                    text += '<input class="tree-checkbox"  type=checkbox ng-model=n.checked ng-change=selectNode(n,localNodes)>';
                }

                // text+= '<span class="edit" ng-click=localClick({node:n})><i class="fa fa-pencil"></i></span>'
                text += '<label>{{n.nomeOrgao}}</label>';

                if (level < max) {
                    text += '<ul id="{{n.idOrgao}}" class="hide" ng-if=checkIfChildren(n,localNodes)>' + renderTreeView('n.children', level + 1, max, 'n', orgaoid) + '</ul></li>';
                } else {
                    text += '</li>';
                }

                return text;
            }// end renderTreeView();

            try {
                var text = '<ul class="tree-view-wrapper">';
                text += renderTreeView('localNodes', 1, maxLevels, null, RegistroFormController.RegistroUsuarioViewModel.OrgaoId);
                text += '</ul>';

                tElement.html(text);
                $compile(tElement.contents())(scope);
            }
            catch (err) {
                tElement.html('<b>ERROR!!!</b> - ' + err);
                $compile(tElement.contents())(scope);
            }
        }
    };
});

