var BaseListController;
SSPApp.controller('BaseListController', ['$scope', 'uiGridConstants', 'i18nService', '$http', '$uibModal', 'baseURL', function ($scope, uiGridConstants, i18nService, $http, $uibModal, baseURL) {
    i18nService.setCurrentLang('pt-br');

    BaseListController = $scope;
    $scope.defaultFilterTemplate = " <div class=\"ui-grid-filter-container\" ng-repeat=\"colFilter in col.filters\" ng-class=\"{'ui-grid-filter-cancel-button-hidden' : colFilter.disableCancelFilterButton === true }\"><div ng-if=\"colFilter.type !== 'select'\"><input type=\"text\" class=\"ui-grid-filter-input ui-grid-filter-input-{{$index}}\" ng-model=\"colFilter.term\" ng-model-options=\"{'updateOn': 'default blur', debounce : {'default' : 1000 , 'blur' : 0 } }\" ng-attr-placeholder=\"{{colFilter.placeholder || ''}}\" aria-label=\"{{colFilter.ariaLabel || aria.defaultFilterLabel}}\"><div role=\"button\" class=\"ui-grid-filter-button\" ng-click=\"removeFilter(colFilter, $index)\" ng-if=\"!colFilter.disableCancelFilterButton\" ng-disabled=\"colFilter.term === undefined || colFilter.term === null || colFilter.term === ''\" ng-show=\"colFilter.term !== undefined && colFilter.term !== null && colFilter.term !== ''\"><i class=\"ui-grid-icon-cancel\" ui-grid-one-bind-aria-label=\"aria.removeFilter\">&nbsp;</i></div></div><div ng-if=\"colFilter.type === 'select'\"><select class=\"ui-grid-filter-select ui-grid-filter-input-{{$index}}\" ng-model=\"colFilter.term\" ng-attr-placeholder=\"{{colFilter.placeholder || aria.defaultFilterLabel}}\" aria-label=\"{{colFilter.ariaLabel || ''}}\" ng-options=\"option.value as option.label for option in colFilter.selectOptions\"><option value=\"\"></option></select><div role=\"button\" class=\"ui-grid-filter-button-select\" ng-click=\"removeFilter(colFilter, $index)\" ng-if=\"!colFilter.disableCancelFilterButton\" ng-disabled=\"colFilter.term === undefined || colFilter.term === null || colFilter.term === ''\" ng-show=\"colFilter.term !== undefined && colFilter.term != null\"><i class=\"ui-grid-icon-cancel\" ui-grid-one-bind-aria-label=\"aria.removeFilter\">&nbsp;</i></div></div></div>";
    $scope.defaultDateFilterTemplate = '<div class="ui-grid-filter-container" ng-repeat="colFilter in col.filters"><input datepicker-options="{showWeeks:false}"     close-text="Fechar" ng-click="isopened = true;" is-open="isopened" uib-datepicker-popup="dd/MM/yyyy" ng-model="colFilter.term" type="text" /></div>';

    $scope.defaultDateFilterTemplate = '<div class="ui-grid-filter-container" ng-repeat="colFilter in col.filters"><label>{{colFilter.term}}</label><button ng-click="">...</button></div>';



    $scope.uiGridConstants = uiGridConstants;
    //$scope.FormTemplateUrl = '/' + $scope.EntityName + '/Form?layout=_LayoutModal';
    $scope.FormTemplateController = 'BaseFormController';    

    $scope.Expand = '';

    $scope.ODataFilter = '';

    $scope.gridOptions = {
        useExternalFiltering: true,
        useExternalSorting:true,
        enableRowHashing: false,        
        saveScroll: true,
        fastWatch: false,
        enableGridMenu: false,
        enableRowHeaderSelection: true,
        multiSelect: false,
        modifierKeysToMultiSelect: false,
        enableFullRowSelection: true,
        treeRowHeaderAlwaysVisible: false,
        showColumnFooter: false,
        enableColumnResizing: true,
        minimumColumnSize: 200,
        enableHorizontalScrollbar: true,
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
            $scope.gridApi.core.on.sortChanged($scope, function (sortColumns) {
                $scope.Search();
            });

            $scope.gridApi.core.on.filterChanged($scope, function () {
                $scope.SelectedPage = 1;
                $scope.Search();
                return false;
            });
        }
    };

    // Configuração do Grid para exportação
    // *Obs: Necessário colocar o grid como hidden no html da view
    $scope.gridExport = {
        useExternalFiltering: true,
        useExternalSorting: true,
        enableRowHashing: false,
        saveScroll: true,
        fastWatch: false,
        enableGridMenu: false,
        enableRowHeaderSelection: true,
        multiSelect: false,
        modifierKeysToMultiSelect: false,
        enableFullRowSelection: true,
        treeRowHeaderAlwaysVisible: false,
        showColumnFooter: false,
        enableColumnResizing: true,
        minimumColumnSize: 200,
        enableHorizontalScrollbar: true,
        onRegisterApi: function (gridApi) {

            $scope.gridExportApi = gridApi;
        }
    };


    //Row Entity Definition
    $scope.gridOptions.rowIdentity = function (row) {
        return row.Id;
    };
    $scope.gridOptions.getRowIdentity = function (row) {
        return row.Id;
    };
    //Row Entity 

    //Start Filter
    $scope.InitFilterValues = function () {
        $scope.filter = new Object();

        //var d = new Date();

        //$scope.filter.DataBatismoPropostoInicio = new Date(d.getFullYear(), d.getMonth(), 1);
        //$scope.filter.DataBatismoPropostoFim = new Date(d.getFullYear(), d.getMonth() + 1, 0);
    };
    $scope.InitFilterValues();

    //Start Pagination

    $scope.ItemsPerPage = 30;

    $scope.FirstPage = function () {
        $scope.SelectedPage = 1;
        $scope.Search();
    };
    $scope.PreviousPage = function () {
        $scope.SelectedPage--;
        $scope.Search();
    };
    $scope.GoToPage = function () {
        $scope.SelectedPage = $scope.GoToPageValue;
        $scope.Search();
    };
    $scope.NextPage = function () {
        $scope.SelectedPage++;
        $scope.Search();
    };
    $scope.LastPage = function () {
        $scope.SelectedPage = $scope.TotalPages;
        $scope.Search();
    };
    //End Pagination

    $scope.BuildODataFilter = function () {

    };

    //Start Search - Busca os dados exibidos no Grid
    $scope.Search = function () {
        console.log($scope.EntityName);

        var url = baseURL + "/odata/" + $scope.EntityName;

        $scope.GoToPageValue = $scope.SelectedPage;

        url += '?$count=true&$skip=' + ($scope.SelectedPage - 1) * $scope.ItemsPerPage + "&$top=" + $scope.ItemsPerPage;

        if (angular.isDefined($scope.SelectFields) && $scope.SelectFields != "") {
            url += '&$select=' + $scope.SelectFields;
        }
        if (angular.isDefined($scope.gridApi)) {
            var filter = "";
            for(var key in $scope.gridApi.grid.columns)
            {
                if ($scope.gridApi.grid.columns[key].enableFiltering == true
                    && angular.isDefined($scope.gridApi.grid.columns[key].filters[0].term)
                    && $scope.gridApi.grid.columns[key].filters[0].term != null)
                {                    
                    switch($scope.gridApi.grid.columns[key].colDef.type)
                    {
                        case "string":
                            if ($scope.gridApi.grid.columns[key].filters[0].term.length >= 3) {
                                if (filter.length > 0) {
                                    filter += " and "
                                }

                                filter += "contains(" + $scope.gridApi.grid.columns[key].colDef.name.replace('.', '/') + ",'" + $scope.gridApi.grid.columns[key].filters[0].term + "')";
                            }
                            break;
                        case "number":
                        case "numberStr":
                            if ($scope.gridApi.grid.columns[key].filters[0].term.length >= 1) {
                                if (filter.length > 0) {
                                    filter += " and "
                                }

                                filter += $scope.gridApi.grid.columns[key].colDef.name.replace('.', '/') + " eq " + $scope.gridApi.grid.columns[key].filters[0].term;
                            }
                            break;
                    }                    
                }
            }

            if(filter.length > 0)
            {
                url += '&$filter=' + filter;
            }
        }
        //url += $scope.
        //ToDo: Implementar filtro dinâmico
        //if ($scope.filter && $scope.filter.Name && $scope.filter.Name.length > 0) {
        //    url += "&$filter=contains(Nome,'" + $scope.filter.Name + "')";
        //}

        url += $scope.ODataFilter;


        if (angular.isDefined($scope.gridApi)) {
            if ($scope.gridApi.grid.getColumnSorting().length > 0) {
                url += "&$orderby="
                for (var key in $scope.gridApi.grid.getColumnSorting()) {
                    url += $scope.gridApi.grid.getColumnSorting()[key].name.replace('.', '/');
                    url += " " + $scope.gridApi.grid.getColumnSorting()[key].sort.direction;
                }
            }
        }

        url = url + $scope.Expand;

        $http.get(url)
        .success(function (result) {

            $scope.TotalResults = result["@odata.count"];
            $scope.TotalPages = Math.ceil($scope.TotalResults / $scope.ItemsPerPage);

            console.log(result);
            response = result;

            $scope.gridOptions.data = "";

            if (angular.isDefined($scope.gridApi)) {
                $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.ALL);
            }
            $scope.gridOptions.data = result.value;

            if ($scope.gridOptions.data.length == 0)
            {
                ajaxError('Não foram encontrado registros para essa consulta.');
            }
        }).error(function (data, status) {
            ajaxError('Erro ao realizar consulta consulta.');
        });
    };
    //End Search

    //Start SearchExportação - Busca os dados exibidos na Exportação para Excel/PDF
    $scope.SearchExportacao = function () {
        console.log($scope.EntityName);

        $scope.BuildODataFilter();

        var url = baseURL + "/odata/" + $scope.EntityName;

        $scope.GoToPageValue = $scope.SelectedPage;

        url += '?$count=true';

        if (angular.isDefined($scope.SelectFields) && $scope.SelectFields != "") {
            url += '&$select=' + $scope.SelectFields;
        }
        if (angular.isDefined($scope.gridApi)) {
            var filter = "";
            for(var key in $scope.gridApi.grid.columns)
            {
                if ($scope.gridApi.grid.columns[key].enableFiltering == true
                    && angular.isDefined($scope.gridApi.grid.columns[key].filters[0].term)
                    && $scope.gridApi.grid.columns[key].filters[0].term != null)
                {                    
                    switch($scope.gridApi.grid.columns[key].colDef.type)
                    {
                        case "string":
                            if ($scope.gridApi.grid.columns[key].filters[0].term.length >= 3) {
                                if (filter.length > 0) {
                                    filter += " and "
                                }

                                filter += "contains(" + $scope.gridApi.grid.columns[key].colDef.name.replace('.', '/') + ",'" + $scope.gridApi.grid.columns[key].filters[0].term + "')";
                            }
                            break;
                        case "number":
                        case "numberStr":
                            if ($scope.gridApi.grid.columns[key].filters[0].term.length >= 1) {
                                if (filter.length > 0) {
                                    filter += " and "
                                }

                                filter += $scope.gridApi.grid.columns[key].colDef.name.replace('.', '/') + " eq " + $scope.gridApi.grid.columns[key].filters[0].term;
                            }
                            break;
                    }                    
                }
            }

            if(filter.length > 0)
            {
                url += '&$filter=' + filter;
            }
        }
        //url += $scope.
        //ToDo: Implementar filtro dinâmico
        //if ($scope.filter && $scope.filter.Name && $scope.filter.Name.length > 0) {
        //    url += "&$filter=contains(Nome,'" + $scope.filter.Name + "')";
        //}

        url += $scope.ODataFilter;


        if (angular.isDefined($scope.gridApi)) {
            if ($scope.gridApi.grid.getColumnSorting().length > 0) {
                url += "&$orderby="
                for (var key in $scope.gridApi.grid.getColumnSorting()) {
                    url += $scope.gridApi.grid.getColumnSorting()[key].name.replace('.', '/');
                    url += " " + $scope.gridApi.grid.getColumnSorting()[key].sort.direction;
                }
            }
        }

        url = url + $scope.Expand;
        $http.get(url)
        .success(function (result) {

            

            
            response = result;

            $scope.gridExport.data = "";

            if (angular.isDefined($scope.gridApi)) {
                $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.ALL);
            }
            $scope.gridExport.data = result.value;

            if ($scope.gridExport.data.length == 0)
            {
                ajaxError('Não foram encontrado registros para essa consulta.');
            }
        }).error(function (data, status) {
            ajaxError('Erro ao realizar consulta consulta.');
        });
    };
    //End Search


    //Start CRUD
    //Create
    $scope.Create = function () {
        $scope.EditId = 0;

        $scope.modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/' + $scope.PageController + '/Formulario?layout=_LayoutFormModal',//$scope.FormTemplateUrl,
            controller: $scope.FormTemplateController,
            size: 'lg',
            closable: false,
            backdrop: 'static',
            scope: $scope
        });
    };
    //Read
    $scope.Details = function () {

    };
    //Update
    $scope.Edit = function () {
        $scope.EditId = new Number($scope.gridApi.selection.getSelectedRows()[0].Id);
        
        $scope.modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/' + $scope.PageController + '/Formulario?layout=_LayoutFormModal' + '&Id=' + $scope.EditId,
            controller: $scope.FormTemplateController,
            size: 'lg',
            closable: false,
            backdrop: 'static',
            scope: $scope
        });
    };

    //Delete
    $scope.Delete = function () {

        BootstrapDialog.confirm({
            title: $scope["Title"],
            message: 'Tem certeza que deseja deletar o registro(s) selecionado(s)?',
            type: BootstrapDialog.TYPE_WARNING, // <-- Default value is BootstrapDialog.TYPE_PRIMARY
            closable: false, // <-- Default value is false
            draggable: true, // <-- Default value is false
            btnCancelLabel: 'Cancelar', // <-- Default value is 'Cancel',
            btnOKLabel: 'Confirmar', // <-- Default value is 'OK',
            btnOKClass: 'btn-warning', // <-- If you didn't specify it, dialog type will be used,
            callback: function (result) {
                // result will be true if button was click, while it will be false if users close the dialog directly.
                if (result) {
                    // Simple POST request example (passing data) :
                    //$http.post('/Membro/Delete', {id : $scope.Membro.MembroId}).
                    //  then(function (response) {
                    var id = new Number($scope.gridApi.selection.getSelectedRows()[0].Id);

                    $http.delete("/api/" + $scope.EntityName + '/' + id)
                    .then(function (result) {
                        $scope.Search();                       
                    });
                    //}).fail(function() {
                    //    BootstrapDialog.show({
                    //                type: 'type-success',
                    //                title: 'ERRO AO TENTAR DELETAR MEMBRO',
                    //                closable: false,
                    //                message: 'Houve um erro no servidor, por favor tente novamente.',
                    //                buttons: [{
                    //                    label: 'Entendi',
                    //                    action: function (dialog) {
                    //                        dialog.close();
                    //                    }
                    //                }]
                    //            });
                    //});

                    // this callback will be called asynchronously
                    // when the response is available
                    //}, function (response) {
                    //    BootstrapDialog.show({
                    //        type: 'type-success',
                    //        title: 'ERRO AO TENTAR DELETAR MEMBRO',
                    //        closable: false,
                    //        message: 'Houve um erro no servidor, por favor tente novamente.',
                    //        buttons: [{
                    //            label: 'Entendi',
                    //            action: function (dialog) {
                    //                dialog.close();
                    //            }
                    //        }]
                    //    });

                    // called asynchronously if an error occurs
                    // or server returns response with an error status.
                    //});
                } else {

                }
            }
        });
    };
    //End CRUD

    //Callback Server
    $scope.Save = function () {

    };
    //End Callback Server


    $scope.SelectedPage = 1;

    $scope.DateFormat = "dd/MM/yyyy";    

    //$scope.load = setInterval(function () {
    //    if (angular.isDefined($scope.EntityName)) {
    //        $scope.Search();
    //        clearInterval($scope.load);
    //    }       
    //}, 500);
}]);