'use strict';
angular.module('SelectOneFromEntity', ['constants']).directive('selectonefromentity', ['$http', 'baseURL', function ($http, baseURL) {
    return { //<select-one-from-entity></select-one-from-entity>
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
            nullValue: '=',
            list1: '@',
            list2: '=',
            list3: '&'
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
                            console.log($scope.$parent);

                            if ($scope.$parent != null) {
                                if ($scope.$parent.$parent != null) {
                                    if ($scope.$parent.$parent.$parent != null) {
                                        if ($scope.$parent.$parent.$parent.$parent != null) {
                                            if ($scope.$parent.$parent.$parent.$parent.$parent != null) {
                                                if ($scope.$parent.$parent.$parent.$parent.$parent.hasOwnProperty("list" + selectOptions.entityName)) {
                                                    $scope.$parent.$parent.$parent.$parent.$parent["list" + selectOptions.entityName] = result.data.value;
                                                }

                                            }
                                            else if ($scope.$parent.$parent.$parent.$parent.hasOwnProperty("list" + selectOptions.entityName)) {
                                                $scope.$parent.$parent.$parent.$parent["list" + selectOptions.entityName] = result.data.value;
                                            }
                                        }
                                        else if ($scope.$parent.$parent.$parent.hasOwnProperty("list" + selectOptions.entityName)) {
                                            $scope.$parent.$parent.$parent["list" + selectOptions.entityName] = result.data.value;
                                        }
                                    }
                                    else if ($scope.$parent.$parent.hasOwnProperty("list" + selectOptions.entityName)) {
                                        $scope.$parent.$parent["list" + selectOptions.entityName] = result.data.value;
                                    }
                                }
                                else if ($scope.$parent.hasOwnProperty("list" + selectOptions.entityName)) {
                                    $scope.$parent["list" + selectOptions.entityName] = result.data.value;
                                }
                            }
                            
                            return $scope["list" + selectOptions.entityName] = result.data.value;

                        });
        }
    };
}]);