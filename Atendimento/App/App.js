angular.module('constants', [])
    .constant('baseURL', window.location.protocol + '//' + window.location.host)

var SSPApp = angular.module('SSPApp',
    ['ui.grid',
    'ui.grid.selection',
    'ui.grid.resizeColumns',
    'ui.grid.moveColumns',
    'ui.grid.grouping',
    'ui.grid.edit',
    'ui.grid.cellNav',
    'ui.grid.autoResize',
    'ui.grid.pinning',
    'ui.grid.exporter',
    'ui.bootstrap',
    'ngMessages',
    'ui.mask',
    'ui.validate',
    'ui.grid.autoResize',
    'ui.grid.pinning',
    //'ngJsTree',
    //'UrlService',        
    'SelectOneFromEntity',
    'frapontillo.bootstrap-switch',
    'ngCookies',
    'blockUI',
    'constants']);

SSPApp.config(function (blockUIConfig) {
    // Change the default overlay message
    blockUIConfig.message = 'Carregando...';
    blockUIConfig.template = "<div class=\"block-ui-overlay\"></div><div class=\"block-ui-message-container\" aria-live=\"assertive\" aria-atomic=\"true\"><div class=\"block-ui-message bg-primary\" ng-class=\"$_blockUiMessageClass\"  style=\"background-color: #337ab7;\"><i style=\"vertical-align: middle;\" class=\"fa fa-spinner fa-pulse fa-3x fa-fw\"></i> {{ state.message }}</div></div>";
});

SSPApp.config(function ($provide) {

    $provide.decorator('uibDatepickerPopupDirective', function ($delegate) {
        var directive = $delegate[0];
        var link = directive.link;

        directive.compile = function () {
            return function (scope, element, attrs) {
                link.apply(this, arguments);
                element.mask("99/99/9999");
            };
        };

        return $delegate;
    });

});



SSPApp.directive('maxDateValidator', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {
            ctrl.$validators.maxDateValidator = function (modelValue, viewValue) {
                if (viewValue == null || viewValue.trim() == '') {
                    return true;
                }

                console.log(attrs);
                var maxDateValidator = JSON.parse(attrs["maxDateValidator"]);

                var comp1 = maxDateValidator.maxDate.split('/');

                if (comp1.length !== 3) {
                    return false;
                }

                var d1 = parseInt(comp1[0], 10);
                var m1 = parseInt(comp1[1], 10);
                var y1 = parseInt(comp1[2], 10);
                var maxDate = new Date(y1, m1 - 1, d1);

                var comp = viewValue.split('/');

                if (comp.length !== 3) {
                    return false;
                }

                var d = parseInt(comp[0], 10);
                var m = parseInt(comp[1], 10);
                var y = parseInt(comp[2], 10);
                var date = new Date(y, m - 1, d);

                //console.log(maxDate.getDate(), date.getDate());

                return (maxDate.getFullYear() > date.getFullYear())
                || (maxDate.getFullYear() == date.getFullYear()
                    && maxDate.getMonth() > date.getMonth())
                || (maxDate.getFullYear() == date.getFullYear()
                    && maxDate.getMonth() == date.getMonth()
                    && maxDate.getDate() >= date.getDate());
            };
        }
    };
});

SSPApp.directive('isDateValidator', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {

            ctrl.$validators.isDateValidator = function (modelValue, viewValue) {
                if (viewValue == null || viewValue.trim() == '') {
                    return true;
                }

                var comp = viewValue.split('/');

                if (comp.length !== 3) {
                    return false;
                }

                var d = parseInt(comp[0], 10);
                var m = parseInt(comp[1], 10);
                var y = parseInt(comp[2], 10);
                var date = new Date(y, m - 1, d);
                return (date.getFullYear() == y && date.getMonth() + 1 == m && date.getDate() == d);
            };
        }
    };
});