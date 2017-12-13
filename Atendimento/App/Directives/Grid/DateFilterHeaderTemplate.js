'use strict';
angular.module('DateFilterHeaderTemplate', ['constants']).directive('DateFilterHeaderTemplate', [function () {
    return { 
        restrict: 'A',
        require: 'ngModel',
        scope: {
            name: '=',
            label: '=',
            ngModel: '=',
            selectOptions: '=',
            fvalid: '=fieldValid',
            finvalid: '=filedInvalid',
            ivalid: '=iconValid',
            iinvalid: '=iconInvalid',
            isRequired: '=',
            nullValue: '='
        },
        templateUrl: function (elem, attr) {
            var selectOptions = JSON.parse(attr.selectoptions);
            return baseURL + '/Directive/SelectOneFromEntity?entityName=' + selectOptions.entityName
            + '&textField=' + selectOptions.textField + '&valueField=' + selectOptions.valueField;
        },
        controller: function ($scope, $element, $attrs) {
            var selectOptions = JSON.parse($attrs.selectoptions);
            var url = baseURL + '/odata/' + selectOptions.entityName;
            if (angular.isDefined(selectOptions.orderByField) && selectOptions.orderByField !== "") {
                url += '?$orderby=' + selectOptions.orderByField;
            }
            $http.get(url)
                        .then(function (result) {

                            return $scope["list" + selectOptions.entityName] = result.data.value;
                        });
        }
    };
}]);