'use strict';

angular.module('matterMain', [
  'ngResource',
  'ui.router',
  'AdalAngular',
  'ui.grid',
  'ui.grid.selection',
  'ui.bootstrap',
  'ui.grid.pagination'
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
          });


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
             cacheLocation: 'localStorage', // enable this for IE, as sessionStorage does not work for localhost.
         }, $httpProvider);
         

    }]).directive('menuclose', function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                $(element).on('click', function (event) {
                    if (event.target.className == "AppSwitcher OpenSwitcher" || event.target.className == "AppSwitcher OpenSwitcher hide") {

                    } else {
                        $(".AppMenuFlyout").slideUp();
                        $(".CloseSwitcher").addClass("hide");
                        $(".OpenSwitcher").removeClass("hide");
                        $(".MenuCaption").removeClass("hideMenuCaption");
                    }
                });
            }
        }
    });

