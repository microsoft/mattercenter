'use strict';

angular.module('matterMain')
  .config(['$stateProvider', function ($stateProvider) {

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
		    })
  }]);