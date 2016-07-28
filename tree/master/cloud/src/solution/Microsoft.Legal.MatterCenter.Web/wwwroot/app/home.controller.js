(function () {
    'use strict';
    var app = angular.module("matterMain");
    app.controller('homeController', ['$scope', '$state', '$stateParams', '$rootScope', 'api', 'homeResource', '$window', '$location',
            'adalAuthenticationService',
        function ($scope, $state, $stateParams, $rootScope, api, homeResource, $window, $location, adalService) {
            var vm = this;
            //header
            vm.userName = adalService.userInfo.userName
            vm.loginUserEmail = adalService.userInfo.userName
            vm.fullName = adalService.userInfo.profile.given_name + ' ' + adalService.userInfo.profile.family_name
            vm.isAuthenticated = adalService.userInfo.isAuthenticated
            vm.smallPictureUrl = 'Images/MC_Profile_Switcher.png';
            vm.largePictureUrl = 'Images/MC_Profile_Switcher.png';
            vm.userProfileObjectId = adalService.userInfo.profile.oid;
            vm.contextualHelpHeader = uiconfigs.Home.ContextualHelpHeader;
            vm.ContextualHelpBottomText = uiconfigs.Home.ContextualHelpBottomText;
            vm.MatterCenterSupportLinkText = uiconfigs.Home.MatterCenterSupportLinkText;


            //Callback function for help 
            function getHelp(options, callback) {
                api({
                    resource: 'homeResource',
                    method: 'getHelp',
                    data: options,
                    success: callback
                });
            }

            function getUserProfilePicture(options, callback) {
                api({
                    resource: 'homeResource',
                    method: 'getUserProfilePicture',
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

            vm.getUserProfilePicture = function () {

                var client = {
                    Url: configs.uri.SPOsiteURL
                }

                getUserProfilePicture(client, function (response) {
                    vm.smallPictureUrl = response.smallPictureUrl;
                    vm.largePictureUrl = response.largePictureUrl;
                });
            }

            vm.getUserProfilePicture();

            //#region Help
            vm.help = function () {
                var helpRequestModel = {
                    Client:
                    {
                        Url: configs.global.repositoryUrl
                    },
                    SelectedPage: $rootScope.pageIndex
                };
                getHelp(helpRequestModel, function (response) {
                    vm.helpData = response;
                });
            }
            //#endregion

            //#region showing and hiding hamburger icon
            vm.showHamburger = true;
            vm.showClose = false;
            vm.showHeaderFlyout = false;
            vm.showHeaderBackground = false;
            vm.showHamburgerIcon = function () {
                vm.showHamburger = true;
                vm.showClose = false;
                vm.showHeaderFlyout = false;
                vm.showHeaderBackground = false;
            }

            vm.showCloseIcon = function () {
                vm.showHamburger = true;
                vm.showClose = true;
                vm.showHeaderFlyout = true;
                vm.showHeaderBackground = true;
            }

            //#endregion

            //#region navigating to the url based on menu click
            vm.navigateUrl = function (data) {
                if (data != "Settings") {
                    $window.top.parent.location.href = configs.uri.SPOsiteURL + "/SitePages/MatterCenterHomev1.aspx?" + data;
                } else {
                    $window.top.parent.location.href = configs.uri.SPOsiteURL + "/SitePages/" + data + ".aspx";
                }
            }

            //#endregion

            vm.mainheadersearch = "";
            //#region navigates to the url with the input entered in the main search as parameter
            vm.mainHeaderClick = function () {
                if (vm.mainheadersearch != "") {
                    $window.top.parent.location.href = configs.uri.SPOsiteURL + "/search/Pages/results.aspx?k=" + vm.mainheadersearch;
                }
            }
            //#endregion

            //#region setting the current year
            var date = new Date();
            vm.currentyear = date.getFullYear();
            //#endregion

        }]);

    app.factory("commonFunctions", function () {
        return {
            searchFilter: function (searchTerm) {
                if (-1 !== searchTerm.indexOf(":")) {
                    var arrTerm = searchTerm.split(":"), sManagedProperty = "";
                    if (arrTerm.length && 2 === arrTerm.length && arrTerm[0] && arrTerm[1]) {
                        sManagedProperty = arrTerm[1].trim(); // Removal of White Space
                        var sPropertyName = arrTerm[0].trim(); // Removal of White Space
                        searchTerm = "(" + sPropertyName + ":\"" + sManagedProperty + "*\")";
                    }
                }
                return searchTerm;
            }
        }
    });
})();
