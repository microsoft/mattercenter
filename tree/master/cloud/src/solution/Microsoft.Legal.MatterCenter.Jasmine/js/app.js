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
  'ui.grid.infiniteScroll'
])
.config(['$stateProvider', '$urlRouterProvider', '$locationProvider', '$httpProvider', 'adalAuthenticationServiceProvider',
    function ($stateProvider, $urlRouterProvider, $locationProvider, $httpProvider, adalProvider) {

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
        ;;



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
         }, $httpProvider);


    }])
.directive('menuclose', function ($rootScope) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $(element).on('click', function (event) {
                if (event.target.className == "AppSwitcher OpenSwitcher" || event.target.className == "AppSwitcher OpenSwitcher hide" || event.target.className == "AppHeaderProfilePict") {

                } else {
                    $(".AppMenuFlyout").slideUp();
                    $(".CloseSwitcher").addClass("hide");
                    $(".OpenSwitcher").removeClass("hide");
                    $(".MenuCaption").removeClass("hideMenuCaption");
                    $(".popcontent").css("display", 'none');
                    $rootScope.displayinfo = false;
                    $rootScope.dispinner = true;
                    $rootScope.contextualhelp = false;
                    $rootScope.dispcontextualhelpinner = true;
                    $rootScope.$apply();
                }
            });
        }
    }
})

.run(function ($rootScope) {
    $rootScope.logEvent = function (eventName) {
        var appType = configs.appInsights.appType;
        eventName = appType + "/" + eventName;
        appInsights.trackEvent(eventName);
        appInsights.setAuthenticatedUserContext(configs.ADAL.authUserEmail)
    };
});
