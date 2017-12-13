"use strict";
var BaseFormController;
SSPApp.controller("BaseFormController", ['$scope', '$http', function ($scope, $http) {
        BaseFormController = $scope;
        $scope.isMobile = false;
        $scope.isSaved = false; 
        
}]);