(function () {
    'use strict';

    angular.module("matterMain")
        .controller('navigationController', ['$state', '$stateParams', 'api','$rootScope',
        function ($state, $stateParams, api, $rootScope) {

            var vm = this;
            vm.SPOHomePage = configs.uri.SPOsiteURL + '/SitePages/MatterCenterHome.aspx';
            vm.menuClick = function () {
                var oAppMenuFlyout = $(".AppMenuFlyout");
                if (!(oAppMenuFlyout.is(":visible"))) {
                    //// Display the close icon and close the fly out
                    $(".OpenSwitcher").addClass("hide");
                    $(".CloseSwitcher").removeClass("hide");
                    $(".MenuCaption").addClass("hideMenuCaption");
                    oAppMenuFlyout.slideDown();
                } else {
                    oAppMenuFlyout.slideUp();
                    $(".CloseSwitcher").addClass("hide");
                    $(".OpenSwitcher").removeClass("hide");
                    $(".MenuCaption").removeClass("hideMenuCaption");
                }
            }

            //#region Global Variables
            vm.welcomeheader = false;
            $rootScope.displayinfo = false;
            $rootScope.dispinner = true;
            $rootScope.contextualhelp = false;
            $rootScope.dispcontextualhelpinner = true;
            //#endregion


            //#region for displaying the Personal Info 
            $rootScope.dispPersonal = function ($event) {
                $event.stopPropagation();
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
                $event.stopPropagation();
                if ($rootScope.dispcontextualhelpinner) {
                    $rootScope.contextualhelp = true;
                    $rootScope.dispcontextualhelpinner = false;
                } else {
                    $rootScope.contextualhelp          = false;
                    $rootScope.dispcontextualhelpinner = true;
                }
            }


            //#endregion

        }]);
})();
