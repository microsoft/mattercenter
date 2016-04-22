'use strict';

angular.module('matterMain', [
  'ngResource',
  'ui.router',
  'AdalAngular',
  'ngTouch',
  'ui.grid'
])
.config(['$stateProvider', '$urlRouterProvider', '$locationProvider', '$httpProvider', 'adalAuthenticationServiceProvider',
    function ($stateProvider, $urlRouterProvider, $locationProvider, $httpProvider, adalProvider) {

          // For any unmatched url, send to /route1
            $urlRouterProvider.otherwise(function($injector, $location) {
                var $state = $injector.get("$state");
                $state.go("mc.navigation");
            });

			$stateProvider
           .state('mc', {
               url: '/',
               views: {
                   "view": {
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
		    });

			            $locationProvider.html5Mode({
                enabled: false,
                requireBase: false
            });
        
      var endpoints = {
          
      };
//'https://mattercenterservicenew.azurewebsites.net
        //http://localhost:58775
    adalProvider.init(
     {
         instance: 'https://login.microsoftonline.com/',
		 tenant: '72f988bf-86f1-41af-91ab-2d7cd011db47',
         clientId: '83b15df9-0dca-4137-a47f-cd69cffc3df8',
         extraQueryParameter: 'nux=1',
         cacheLocation: 'localStorage', // enable this for IE, as sessionStorage does not work for localhost.
         endpoints: endpoints
     }, $httpProvider);


  }])

