angular.module("uib/template/tabs/custom_tabset.html", []).run(["$templateCache", function ($templateCache) {
    $templateCache.put("uib/template/tabs/custom_tabset.html",
      "<div>\n" +
      "  <ul class=\"nav nav-{{tabset.type || 'tabs'}} col-xs-12 col-sm-12 col-md-2 col-lg-2 \" ng-class=\"{'nav-stacked': vertical, 'nav-justified': justified}\" ng-transclude></ul>\n" +
      "  <div class=\"tab-content col-xs-12 col-sm-12 col-md-10 col-lg-10  \">\n" +
      "    <div class=\"tab-pane\"\n" +
      "         ng-repeat=\"tab in tabset.tabs\"\n" +
      "         ng-class=\"{active: tabset.active === tab.index}\"\n" +
      "         uib-tab-content-transclude=\"tab\">\n" +
      "    </div>\n" +
      "  </div>\n" +
      "</div>\n" +
      "");
}]);