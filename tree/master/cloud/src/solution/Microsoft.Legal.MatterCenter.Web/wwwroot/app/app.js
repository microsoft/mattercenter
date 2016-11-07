'use strict';

angular.module('matterMain', [
  'ngResource',
  'ui.router',
  'AdalAngular',
  'ui.grid',
  'ui.grid.selection',
  'ui.bootstrap',
  'ui.grid.pagination',
  'ngSanitize',
  'ngAnimate',
  'ui.grid.infiniteScroll',
  'angulartics',
  'angulartics.azure',
  'ngAria'
])
.config(['$stateProvider', '$urlRouterProvider', '$locationProvider', '$httpProvider', 'adalAuthenticationServiceProvider',
    function ($stateProvider, $urlRouterProvider, $locationProvider, $httpProvider, adalProvider) {

        // For any unmatched url, send to /route1...
        $urlRouterProvider.otherwise(function ($injector, $location) {
            var $state = $injector.get("$state");
            $state.go("mc.navigation");
        });

        $stateProvider
       .state('mc', {
           url: '/',
           views: {
               "mainView": {
                   templateUrl: '/app/home.html',
                   controller: 'homeController as vm'
               }
           }, requireADLogin: true
       })
         .state('mc.navigation', {
             url: "^/navigation",
             views: {
                 "contentView": {
                     templateUrl: '/app/navigation.html',
                     controller: 'navigationController as vm'
                 }
             }, requireADLogin: true
         })
        .state('mc.matters', {
            url: "^/matters",
            views: {
                "contentView": {
                    templateUrl: '/app/matter/matters.html',
                    controller: 'mattersController as vm'
                }
            }, requireADLogin: true
        })
        .state('mc.documents', {
            url: "^/documents",
            views: {
                "contentView": {
                    templateUrl: '/app/document/documents.html',
                    controller: 'documentsController as vm'
                }
            }, requireADLogin: true
        })
        .state('mc.createMatter', {
            url: "^/createMatter",
            views: {
                "contentView": {
                    templateUrl: '/app/matter/createMatter.html',
                    controller: 'createMatterController as cm'
                }
            }, requireADLogin: true
        })
        .state('mc.documentdashboard', {
            url: "^/documentdashboard",
            views: {
                "contentView": {
                    templateUrl: '/app/dashboard/documentdashboard.html',
                    controller: 'DocumentDashBoardController as vm'
                }
            }, requireADLogin: true
        })
        .state('mc.matterdashboard', {
            url: "^/matterdashboard",
            views: {
                "contentView": {
                    templateUrl: '/app/dashboard/matterdashboard.html',
                    controller: 'MatterDashBoardController as vm'
                }
            }, requireADLogin: true
        })
        .state('mc.matterusers', {
            url: "^/matterusers",
            views: {
                "contentView": {
                    templateUrl: '/app/matter/matterusers.html',
                    controller: 'MatterUsersController as cm'
                }
            }, requireADLogin: true
        })
        .state('mc.settings', {
            url: "^/settings",
            views: {
                "contentView": {
                    templateUrl: '/app/dashboard/settings.html',
                    controller: 'SettingsController as vm'
                }
            }, requireADLogin: true
        })
         .state('mc.admin', {
             url: "^/admin",
             views: {
                 "contentView": {
                     templateUrl: '/app/admin/admin.html',
                     controller: 'AdminController as vm'
                 }
             }, requireADLogin: true
         })
        ;;

        //#region error handling.
        $httpProvider.interceptors.push(function ($q, $rootScope) {

            return {
                request: function (config) {

                    //the same config / modified config / a new config needs to be returned.
                    return config;
                },
                requestError: function (rejection) {

                    //Initializing error list
                    if ($rootScope.errorList == undefined) {
                        $rootScope.errorList = [];
                    }

                    $rootScope.errorList.push(rejection.data);
                    $scope.$broadcast('disableOverlay', "text");
                    //It has to return the rejection, simple reject call doesn't work
                    return $q.reject(rejection);
                },
                response: function (response) {

                    //the same response/modified/or a new one need to be returned.
                    return response;
                },
                responseError: function (rejection) {

                    //Initializing the error list
                    if ($rootScope.exceptionObj == undefined) {
                        $rootScope.exceptionObj = [];
                    }
                    if (rejection.data == undefined) {
                        rejection.data = {};
                        rejection.data.message = "Network issue. Unable to get the response from the server. ";
                    }
                    $rootScope.$broadcast('disableOverlay', "text");
                    //Adding to error list
                    //$rootScope.errorList.push(rejection.data);
                    angular.element('#myErrorModal').modal("show");
                    $rootScope.exceptionObj = rejection.data;
                    //It has to return the rejection, simple reject call doesn't work
                    return $q.reject(rejection);
                }
            };
        });

        //#endregion


        $locationProvider.html5Mode({
            enabled: false,
            requireBase: false
        });

        adalProvider.init(
         {
             instance: 'https://login.microsoftonline.com/',
             tenant: configs.uri.tenant,
             clientId: configs.ADAL.clientId,
             extraQueryParameter: 'nux=1',
             //cacheLocation: 'localStorage', // enable this for IE, as sessionStorage does not work for localhost.
         }, $httpProvider);


    }])
.directive('menuclose', function ($rootScope, $timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $(element).on('click', function (event) {
                if (event.target.className == "AppSwitcher OpenSwitcher" || event.target.className == "AppSwitcher OpenSwitcher hide" || event.target.className == "AppHeaderProfilePict") {

                } else {
                    $(".AppMenuFlyout").slideUp();
                    $(".topheader").css("z-index", "4");
                    $(".CloseSwitcher").addClass("hide");
                    $(".OpenSwitcher").removeClass("hide");
                    $(".MenuCaption").removeClass("hideMenuCaption");
                    $(".popcontent").css("display", 'none');
                    $rootScope.displayinfo = false;
                    $rootScope.dispinner = true;
                    $rootScope.contextualhelp = false;
                    $rootScope.dispcontextualhelpinner = true;
                    $timeout(function () { angular.element('.zindex6').css('z-index', '6'); }, 600);
                    $rootScope.$apply();
                }
            });
        }
    }
})
.run(function ($rootScope, $analytics) {
    $rootScope.setAuthenticatedUserContext = function () {
        appInsights.setAuthenticatedUserContext(configs.ADAL.authUserEmail);

    };
})
;
