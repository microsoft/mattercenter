(function () {
    'use strict';

    angular.module("matterMain")
        .controller('homeController', ['$scope', '$state', '$stateParams', '$rootScope', 'api', 'homeResource','adalAuthenticationService',
        function ($scope, $state, $stateParams, $rootScope, api, homeResource, adalService) {
            var vm = this;
            //header
            vm.userName = adalService.userInfo.userName
            vm.loginUserEmail = adalService.userInfo.userName
            vm.fullName = adalService.userInfo.profile.given_name + ' ' + adalService.userInfo.profile.family_name
            vm.isAuthenticated = adalService.userInfo.isAuthenticated
            // configs.name;
            vm.isDelegate = false;
            vm.isAdmin = true;
            vm.collapse = false;

            vm.showToUser = false;
            vm.featureEnabled = true;

            //Callback function for help 
            function getHelp(options, callback) {
                api({
                    resource: 'homeResource',
                    method: 'getHelp',
                    data: options,
                    success: callback
                });
            }

            //#endregion

            if ($state.current.name === 'mc')
                $state.go('mc.navigation');

            //#region for displaying the Personal Info 
            $rootScope.displayinfo = false;
            $rootScope.dispinner = true;
            $rootScope.contextualhelp = false;
            $rootScope.dispcontextualhelpinner = true;
            $rootScope.dispPersonal = function ($event) {
                $event.stopPropagation();
                $rootScope.contextualhelp = false;
                $rootScope.dispcontextualhelpinner = true;
                if ($rootScope.dispinner) {
                    $rootScope.displayinfo = true;
                    $rootScope.dispinner = false;
                } else {
                    $rootScope.displayinfo = false;
                    $rootScope.dispinner = true;
                }
            }
            //#endregion

            //#region for displaying contextual help 
            $rootScope.dispContextualHelp = function ($event) {
                
                $rootScope.displayinfo = false;
                $rootScope.dispinner = true;
                $event.stopPropagation();
                if ($rootScope.dispcontextualhelpinner) {
                    $rootScope.contextualhelp = true;
                    vm.help('');
                    $rootScope.dispcontextualhelpinner = false;
                } else {
                    $rootScope.contextualhelp = false;
                    $rootScope.dispcontextualhelpinner = true;
                }
            }
            //#endregion
            $rootScope.pageIndex = "0";
            vm.signOut = function () {
                adalService.logOut();
            }

            //#region Help
            vm.help = function () {
                var helpRequestModel = {
                    Client:
                    {
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
                    },
                    SelectedPage:  $rootScope.pageIndex
                };
                getHelp(helpRequestModel, function (response) {
                    vm.helpData = response;
                });
            }
            //#endregion
        }]);
})();
