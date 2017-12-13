// Code goes here
//var app = angular.module('SSPapp', ['ui.bootstrap']);


////////////////////////////////////////////////////////////////////////////////
/// TREEVIEW  //////////////////////////////////////////////////////////////////
SSPApp.directive('treeView', function ($compile) {
  return {
    restrict : 'E',
    scope : {
      localNodes : '=model',
      localClick : '&click'
    },
    link : function (scope, tElement, tAttrs, transclude) {
      
      var maxLevels = (angular.isUndefined(tAttrs.maxlevels)) ? 10 : tAttrs.maxlevels; 
      var hasCheckBox = (angular.isUndefined(tAttrs.checkbox)) ? false : true;
      scope.showItems = [];
      
      scope.showHide = function(ulId) {
        var hideThis = document.getElementById(ulId);
        var showHide = angular.element(hideThis).attr('class');
        angular.element(hideThis).attr('class', (showHide === 'show' ? 'hide' : 'show'));
      }
      
      scope.showIcon = function(node) {
          return scope.checkIfChildren(node);
      }
      
      scope.checkIfChildren = function(node) {
          return angular.isUndefined(node.children) || node.children.length > 0;
      }

      /////////////////////////////////////////////////
      /// SELECT ALL CHILDRENS
      // as seen at: http://jsfiddle.net/incutonez/D8vhb/5/
      function parentCheckChange(item) {
        for (var i in item.children) {
            item.children[i].checked = item.checked;

          if (item.children[i].children) {
            parentCheckChange(item.children[i]);
          }
        }
      }


      //function parentCheckChangefalse(item, localNodes) {

        
      //    for (var i in localNodes) {

      //        if (item.idOrgao != localNodes[i].idOrgao) {
      //            localNodes[i].children[i].checked = false;
      //        }

      //        if (localNodes[i].children[i].children) {
      //            parentCheckChange(localNodes[i].children[i]);
      //        }
      //    }
      //}
     
      scope.checkChange = function (node, localNodes) {

         // parentCheckChangefalse(node, localNodes);

        if (node.children) {
          parentCheckChange(node);
        }
      }
      /////////////////////////////////////////////////

      function renderTreeView(collection, level, max) {
        var text = '';
        text += '<li ng-repeat="n in ' + collection + '" >';
        text += '<span ng-show="showIcon(n)" class="show-hide" ng-click=showHide(n.idOrgao)><i class="fa fa-plus-square"></i></span>';
        text += '<span ng-show="!showIcon(n)" style="padding-right: 13px"></span>';
       
        if (hasCheckBox) {
            text += '<input class="tree-checkbox" type=checkbox ng-model=n.checked ng-change=checkChange(n,localNodes)>';
        }
        

       // text+= '<span class="edit" ng-click=localClick({node:n})><i class="fa fa-pencil"></i></span>'

        
        text += '<label>{{n.nomeOrgao}}</label>';
       
        if (level < max) {
            text += '<ul id="{{n.idOrgao}}" class="hide" ng-if=checkIfChildren(n)>' + renderTreeView('n.children', level + 1, max) + '</ul></li>';
        } else {
          text += '</li>';
        }
        
        return text;
      }// end renderTreeView();
      
      try {
        var text = '<ul class="tree-view-wrapper">';
        text += renderTreeView('localNodes', 1, maxLevels);
        text += '</ul>';
        tElement.html(text);
        $compile(tElement.contents())(scope);
      }
      catch(err) {
        tElement.html('<b>ERROR!!!</b> - ' + err);
        $compile(tElement.contents())(scope);
      }
    }
  };
});



//SSPApp.controller('x', ['$scope', function ($scope) {
SSPApp.controller("x", ['$scope', '$http', '$controller', '$uibModal', '$timeout', '$window', 'uiGridConstants', 'baseURL', function ($scope, $http, $controller, $uibModal, $timeout, $window, uiGridConstants, baseURL) {


    $scope.nodes = [];
    $scope.lista = [];

   // var url =  'http://localhost:63196/odata/Orgao?$filter=ID_SUBTIPO_ORGAO eq 2';

    var url =  'http://localhost:63196/api/UsuarioAPI/listarOrgaosPorTipo?idTipo=2';

    $http.get(url)
                .then(function (result) {

                    $scope.nodes = result.data;

                    $scope.nodes.children = $scope.nodes.filhosOrgao;
                });

  
}]);
