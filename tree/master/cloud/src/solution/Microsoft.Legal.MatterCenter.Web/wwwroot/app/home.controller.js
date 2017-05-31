(function () {
    'use strict';
    var app = angular.module("matterMain");
    app.controller('homeController', ['$scope', '$state', '$stateParams', '$rootScope', 'api', 'homeResource', '$window', '$location',
            'adalAuthenticationService', '$timeout',
        function ($scope, $state, $stateParams, $rootScope, api, homeResource, $window, $location, adalService, $timeout) {
            var vm = this;
            //header
            vm.userName = adalService.userInfo.userName
            vm.loginUserEmail = adalService.userInfo.userName
            configs.ADAL.authUserEmail = adalService.userInfo.userName
            vm.fullName = adalService.userInfo.profile.given_name + ' ' + adalService.userInfo.profile.family_name
            vm.isAuthenticated = adalService.userInfo.isAuthenticated
            vm.smallPictureUrl = 'Images/MC_Profile_Switcher.png';
            vm.largePictureUrl = 'Images/mc_profile_switcher_72.png';
            vm.userProfileObjectId = adalService.userInfo.profile.oid;
            vm.navigation = uiconfigs.Navigation;
            vm.header = uiconfigs.Header;
            vm.globalSettings = configs.global;
            vm.isDevMode = configs.global.isDevMode;
            vm.lazyloaderhelp = true;
            $rootScope.setAuthenticatedUserContext();
            $rootScope.displayOverflow = "";
            $rootScope.appMenuFlyOut = false;
            $rootScope.flagAppMenuFlyOut = true;
            vm.helpData = [];

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

            function canCreateMatter(options, callback) {
                api({
                    resource: 'homeResource',
                    method: 'canCreateMatter',
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
            $rootScope.dispcontextualhelpinnerF1 = true;
            $rootScope.dispPersonal = function ($event) {
                $event.stopPropagation();
                $rootScope.contextualhelp = false;
                angular.element('.popcontent').css('display', 'none');
                angular.element('.dropdown').removeClass("open");
                $rootScope.dispcontextualhelpinner = true;
                angular.element('.mattersdrop').addClass('ng-hide');
                if ($rootScope.dispinner) {
                    if ($rootScope.pageIndex == 4 && $window.innerWidth < 675) {
                        angular.element('.zindex6').css('z-index', '2');
                    }
                    jQuery.a11yfy.assertiveAnnounce("Profile information popup opened with signout button");
                    $rootScope.displayinfo = true;
                    $rootScope.dispinner = false;
                } else {
                    $rootScope.displayinfo = false;
                    $rootScope.dispinner = true;
                    jQuery.a11yfy.assertiveAnnounce("Profile information popup closed ");
                    $timeout(function () { angular.element('#ProfileSwitcher').focus(); }, 500);
                    if ($rootScope.pageIndex == 4 && $window.innerWidth < 675) {
                        angular.element('.zindex6').css('z-index', '6');
                    }
                }
            }

            angular.element(document).ready(function () {
                $(document).on("keydown", openContextualHelpFlyout);
            });

            //This function will invoke if the user presses  F1 key or esc
            //If the user presses F1 key, show the help and if the user presses esc key, hide the
            //help pop up
            function openContextualHelpFlyout(e) {
                "use strict";
                // if key pressed is F1 then display the popup
                if (112 === e.keyCode) {
                    $rootScope.contextualhelp = true;
                    vm.help('');
                    $rootScope.dispcontextualhelpinnerF1 = false;
                    e.preventDefault();
                    e.stopPropagation();
                }
                //If the user presses esc key, hide the help pop up
                if (27 === e.keyCode) {
                    $rootScope.contextualhelp = false;
                    $rootScope.dispcontextualhelpinnerF1 = true;

                }
            };

            //#endregion

            //#region for displaying contextual help 
            //This event is going to fire when the user clicks on the help icon using the mouse
            $rootScope.dispContextualHelp = function ($event) {
                angular.element('.popcontent').css('display', 'none');
                angular.element('.dropdown').removeClass("open");
                vm.mattersdrop = false;
                vm.mattersdropinner = false;
                vm.documentsdrop = false;
                vm.docdropinner = true;
                $rootScope.displayinfo = false;
                $rootScope.dispinner = true;
                $event.stopPropagation();
                if ($rootScope.dispcontextualhelpinner) {
                    angular.element('.zindex6').css('z-index', '2');
                    $rootScope.contextualhelp = true;
                    $rootScope.dispcontextualhelpinner = false;
                } else {
                    $rootScope.contextualhelp = false;
                    $rootScope.dispcontextualhelpinner = true;
                    $timeout(function () { angular.element('.zindex6').css('z-index', '6'); }, 600);
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
                    vm.smallPictureUrl = response.largePictureUrl;
                    vm.largePictureUrl = response.largePictureUrl;
                });
            }

            vm.navigate = function(path)
            {
                $state.go(path);
                $rootScope.appMenuFlyOut = false;
                $rootScope.flagAppMenuFlyOut = true;
                jQuery.a11yfy.assertiveAnnounce("Collapsed matter center main menu");               
                $(".topheader").css("z-index", "4");
                $(".CloseSwitcher").addClass("hide");
                $(".OpenSwitcher").removeClass("hide");
                $(".MenuCaption").removeClass("hideMenuCaption");
            }

            vm.getUserProfilePicture();

            //#region Help
            $rootScope.help = function () {
                vm.helpData = [];
                vm.lazyloaderhelp = false;
                var helpRequestModel = {
                    Client:
                    {
                        Url: configs.global.repositoryUrl
                    },
                    SelectedPage: $rootScope.pageIndex
                };
                getHelp(helpRequestModel, function (response) {
                    vm.helpData = response;
                    vm.lazyloaderhelp = true;
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

                var windowLocation = $window.location.href;

                if (windowLocation != '' && typeof windowLocation != 'undefined') {
                    if ("section=2" == data && windowLocation.substring(windowLocation.indexOf("#") + 2, windowLocation.length) == 'documentdashboard') {
                        return false;
                    } else if ("section=1" == data && windowLocation.substring(windowLocation.indexOf("#") + 2, windowLocation.length) == 'matterdashboard') {
                        return false;
                    } else if ("settings" == data && windowLocation.substring(windowLocation.indexOf("#") + 2, windowLocation.length) == 'settings') {
                        return false;
                    }
                }

                if (data != "Settings") {
                    if (configs.global.isBackwardCompatible == false) {
                        $window.top.parent.location.href = configs.uri.SPOsiteURL + "/SitePages/MatterCenterHome.aspx?" + data;
                    }
                    else {
                        $window.top.parent.location.href = configs.global.repositoryUrl + "/SitePages/MatterCenterHome.aspx?" + data;
                    }

                } else {
                    if (configs.global.isBackwardCompatible == false) {
                        $window.top.parent.location.href = configs.uri.SPOsiteURL + "/SitePages/" + data + ".aspx";
                    }
                    else {
                        $window.top.parent.location.href = configs.global.repositoryUrl + "/SitePages/" + data + ".aspx";
                    }
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
            $rootScope.collapsedText = "Collapsed main menu";
            $rootScope.expandedText = "";
            vm.menuClick = function () {
                angular.element('.popcontent').css('display', 'none');
                angular.element('.dropdown').removeClass("open");
                if ($rootScope.flagAppMenuFlyOut) {
                    $rootScope.appMenuFlyOut = true;
                    jQuery.a11yfy.assertiveAnnounce("Expanded matter center main menu");
                    $rootScope.flagAppMenuFlyOut = false;
                    $(".OpenSwitcher").addClass("hide");
                    $(".CloseSwitcher").removeClass("hide");
                    $(".MenuCaption").addClass("hideMenuCaption");
                    $(".topheader").css("z-index", "8");
                } else {
                    $rootScope.appMenuFlyOut = false;
                    $rootScope.flagAppMenuFlyOut = true;

                    jQuery.a11yfy.assertiveAnnounce("Collapsed matter center main menu");

                    
                    $(".topheader").css("z-index", "4");
                    $(".CloseSwitcher").addClass("hide");
                    $(".OpenSwitcher").removeClass("hide");
                    $(".MenuCaption").removeClass("hideMenuCaption");
                }
            }

            vm.canLoginUserCreateMatter = false;
            vm.canCreateMatter = function () {
                var client = {
                    Url: configs.global.repositoryUrl
                }
                canCreateMatter(client, function (response) {
                    vm.canLoginUserCreateMatter = response.canCreateMatter;
                })
            }
            vm.canCreateMatter();

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
