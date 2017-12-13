'use strict';
angular.module('UrlService', []).service('UrlService', [function () {
    var service = {};

    service.GetParameter = function (parameterName) {
        var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < url.length; i++) {
            var urlparam = url[i].split('=');
            if (urlparam[0] == parameterName) {
                return urlparam[1];
            }
        }
    };

    service.GetReturnUrlOrDefault = function () {
        var returnUrl = service.GetParameter("ReturnUrl");

        if (angular.isUndefined(returnUrl) || returnUrl == null || returnUrl == "")
        {
            returnUrl = "/";
        }
        else
        {
            returnUrl = "/" + returnUrl;
        }

        return returnUrl;
    };

    return service;
}]);