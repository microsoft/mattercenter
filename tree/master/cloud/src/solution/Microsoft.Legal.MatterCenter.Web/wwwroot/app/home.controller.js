(function () {
    'use strict';

    angular.module("matterMain")
        .controller('homeController', ['$scope', '$state', '$stateParams', 
        function ($scope, $state, $stateParams) {
            var vm = this;
			//header
			 var vm = this;
            vm.userName =  "Wilson Reddy Gajarla"
			// configs.name;
            vm.isDelegate = false;
            vm.isAdmin = true;
            vm.collapse = false;

            vm.showToUser = false;
            vm.featureEnabled = true;
            

		    if($state.current.name === 'mc')
            $state.go('mc.navigation');


       }]);
})();
