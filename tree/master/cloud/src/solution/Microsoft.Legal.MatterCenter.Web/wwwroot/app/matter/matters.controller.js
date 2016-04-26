(function () {
    'use strict';

    angular.module("matterMain")
        .controller('mattersController', ['$scope', '$state', '$stateParams', 'api',
      function ($scope, $state, $stateParams, api) {
	    var vm = this;
          var options = new Object;
          vm.gridOptions = {
              columnDefs: [{ field: 'matterName', displayName: 'Matter', enableCellEdit: true },
                          { field: 'matterClient', displayName: 'Client', enableCellEdit: true },
                          //matterID
                 { field: 'matterClientId', displayName: 'Client.MatterID', cellTemplate: '<div class="ngCellText">{{row.entity.matterClientId}}.{{row.entity.matterID}}</div>', enableCellEdit: true, },
                  { field: 'matterModifiedDate', displayName: 'Modified Date', type: 'date', cellFilter: 'date:\'MMM dd,yyyy\'' },
              ],
              enableColumnMenus: false
          };


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
		
          options = {
              Id: "123456",
							Name: "Microsoft", 
							Url: "https://msmatter.sharepoint.com/sites/catalog"
						};	
                        
		getPinnedMatters(options, function (response) {
              //vm.matters = response.userPinnedMattersList;
              vm.gridOptions.data = response.userPinnedMattersList;


                });          

        }]);
})();
