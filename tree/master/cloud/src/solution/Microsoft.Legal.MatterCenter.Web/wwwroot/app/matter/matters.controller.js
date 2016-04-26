(function () {
    'use strict';

    angular.module("matterMain")
        .controller('mattersController', ['$scope', '$state', '$stateParams', 'api',
      function ($scope, $state, $stateParams, api) {
	    var vm = this;
		var options= new Object;

		function getMatters(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getMatters',
                    data: options,
                    success: callback
                });
            } 
			
		getMatters(options, function (response) {
                   vm.title = response.title;

                });   
			
		function getPinnedMatters(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getPinnedMatters',
                    data: options,
                    success: callback
                });
            } 
		
		options = {	Id: "123456", 
							Name: "Microsoft", 
							Url: "https://msmatter.sharepoint.com/sites/catalog"
						};	
                        
		getPinnedMatters(options, function (response) {
                   vm.matters = response.userPinnedMattersList;

                });          

        }]);
})();
