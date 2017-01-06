(function () {

    'use strict';
    function heading() {
        return {
            restrict: "E",
            scope: {
                employeeNames: '=employeeNames',
                navUrl: '=navUrl',
                navUrlText: '=navUrlText'
            },
            template: '<div>'
                        + '<div class="brand">'
                        + '<img src="../../images/TEALS_logo.png"/>'
                        + '</div>'
                        + '<div class="pull-right user-name">'
                        + '<div style="display:inline-block">{{employeeNames}}</div>'

                        + '<div style="display:inline-block">&nbsp; &nbsp;<a href="https://www.yammer.com/microsoft.com/#/threads/inGroup?type=in_group&feedId=7384139&view=all" target="_blank">Feedback </a></div>'

                        + '</br><a class="pull-right nav-url" target="_parent" href="{{navUrl}}">{{navUrlText}}</a>'
                        + '</div>'
                        + '<div class="clearfix"></div>'
                    + '</div>'
        }
    }

    var rmAppDirective = angular.module('matterMain');
    rmAppDirective.directive('heading', [heading]);

})();