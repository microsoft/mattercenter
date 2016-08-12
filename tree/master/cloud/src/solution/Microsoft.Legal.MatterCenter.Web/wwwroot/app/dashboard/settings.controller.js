(function () {
    'use strict;'
    var app = angular.module("matterMain");
    app.controller('SettingsController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'settingsResource', '$rootScope', 'uiGridConstants', '$location', '$http', '$q', '$filter', 'commonFunctions',
        function settingsController($scope, $state, $interval, $stateParams, api, $timeout, settingsResource, $rootScope, uiGridConstants, $location, $http, $q, $filter, commonFunctions) {
            var vm = this;
            //#region API to get the client taxonomy 
            function getTaxonomyDetails(optionsForClientGroup, callback) {
                api({
                    resource: 'settingsResource',
                    method: 'getTaxonomyData',
                    data: optionsForGroup,
                    success: callback
                });
            }

            //API call to get roles that are configured in the system
            function getRoles(options, callback) {
                api({
                    resource: 'settingsResource',
                    method: 'getRoles',
                    data: options,
                    success: callback
                });
            }
            //API call to get permission levels that are configured in the system
            function getPermissionLevels(options, callback) {
                api({
                    resource: 'settingsResource',
                    method: 'getPermissionLevels',
                    data: options,
                    success: callback
                });
            }

            //API call to get default configurations 
            function getDefaultConfigurations(options, callback) {
                api({
                    resource: 'settingsResource',
                    method: 'getDefaultConfigurations',
                    data: JSON.stringify(options),
                    success: callback
                });
            }

            //#region global variables
            $rootScope.bodyclass = "";
            $rootScope.profileClass = "hide";
            vm.assignPermissionTeams = [{ assignedUser: '', assignedRole: '', assignedPermission: '', assigneTeamRowNumber: 1 }];
            vm.assignRoles = [];
            vm.assignPermissions = [];
            //#endregion

            //#region for hiding the client details on load
            vm.showClientDetails = false;
            //#endregion

            //#region requestobject for getting the taxonomy data
            var optionsForGroup = {
                Client: {
                    Url: configs.global.repositoryUrl
                },
                TermStoreDetails: {
                    TermGroup: "MatterCenterTerms",
                    TermSetName: "Clients",
                    CustomPropertyName: "ClientURL"
                }
            };
            //#endregion

            vm.clientlist = false;
            vm.nodata = false;
            vm.lazyloader = false;
            //#region function for getting the client details
            vm.taxonomydata = [];
            vm.getTaxonomyData = function () {
                vm.lazyloader = false;
                getTaxonomyDetails(optionsForGroup, function (response) {
                    if (response != "") {
                        vm.clientlist = true;
                        vm.nodata = false;
                        vm.taxonomydata = response.clientTerms;
                        vm.lazyloader = true;
                    } else {
                        vm.clientlist = false;
                        vm.nodata = true;
                        vm.lazyloader = true;
                    }
                });
            }
            //#endregion


            //#region setting the request object for getting roles and permission levels
            var rolesRequest = new Object;
            rolesRequest = {
                Url: configs.global.repositoryUrl
            }

            var permissionsRequest = new Object;
            permissionsRequest = {
                Url: configs.global.repositoryUrl
            }

            //#region for calling getting the role details
            vm.getRolesData = function () {
                vm.lazyloader = false;
                getRoles(rolesRequest, function (response) {
                    if (response != "") {
                        vm.clientlist = true;
                        vm.nodata = false;
                        vm.assignRoles = response;
                        vm.lazyloader = true;
                    } else {
                        vm.clientlist = false;
                        vm.nodata = true;
                        vm.lazyloader = true;
                    }
                });
            }
            //#endregion

            //#region for calling getting the role details
            vm.getPermissionsData = function () {
                vm.lazyloader = false;
                getPermissionLevels(permissionsRequest, function (response) {
                    if (response != "") {
                        vm.clientlist = true;
                        vm.nodata = false;
                        vm.assignPermissions = response;
                        vm.lazyloader = true;
                    } else {
                        vm.clientlist = false;
                        vm.nodata = true;
                        vm.lazyloader = true;
                    }
                });
            }
            //#endregion

            //#region for calling the functions on page load
            vm.getTaxonomyData();
            vm.getRolesData();
            vm.getPermissionsData();
            //#endregion


            //#region for getting the users based on search values
            vm.searchUsers = function (val) {
                var searchUserRequest = {
                    Client: {
                        //Need to get the matter url from query string
                        Url: configs.global.repositoryUrl
                    },
                    SearchObject: {
                        SearchTerm: val
                    }
                };
                return settingsResource.getUsers(searchUserRequest).$promise;
            }
            //#endregion

            var siteCollectionPath = "";
            //#region for showing the selected clients Data
            vm.showSelectedClient = function (name,url) {
                vm.lazyloader = false;
                vm.selected = name;
                siteCollectionPath = url;
                getDefaultConfigurations(siteCollectionPath, function (response) {
                    if (response != "") {
                        vm.configurations = JSON.parse(response.code);
                        vm.setClientData(vm.configurations);
                        vm.showrole="Yes";
                        vm.showmatterid = "Datetime";
                        vm.nodata = false;
                        vm.lazyloader = true;
                        vm.clientlist = false;
                        vm.showClientDetails = true;
                    } else {
                        vm.nodata = true;
                        vm.lazyloader = true;
                    }
                });
            }
            //#endregion

            //#region for setting the selected client values to ng-models
            vm.setClientData = function (data) {
                vm.mattername = data.DefaultMatterName;
                vm.matterid = data.DefaultMatterId;
                if (data.IsRestrictedAccessSelected) {
                    vm.assignteam = "Yes";
                } else {
                    vm.assignteam = "No";
                }
                if (data.IsCalendarSelected) {
                    vm.calendar = "Yes";
                } else {
                    vm.calendar = "No";
                }
                if (data.IsRSSSelected) {
                    vm.rss = "Yes";
                } else {
                    vm.rss = "No";
                }
                if (data.IsEmailOptionSelected) {
                    vm.email = "Yes";
                } else {
                    vm.email = "No";
                }
                if (data.IsTaskSelected) {
                    vm.tasks = "Yes";
                } else {
                    vm.tasks = "No";
                }
                if (data.IsMatterDescriptionMandatory) {
                    vm.matterdesc = "Yes";
                } else {
                    vm.matterdesc = "No";
                }
                if (data.IsConflictCheck) {
                    vm.conflict = "Yes";
                } else {
                    vm.conflict = "No";
                }
                if (data.IsMatterDescriptionMandatory) {
                    vm.matterdesc = "Yes";
                } else {
                    vm.matterdesc = "No";
                }
            }
            //#endregion

            //#region for showing the settings div
            vm.showSetting = function () {
                vm.clientlist = true;
                vm.showClientDetails = false;
            }
            //#endregion

            //#region for adding more users to assign permissions
            vm.addNewAssignPermissions = function () {
                var newItemNo = vm.assignPermissionTeams.length + 1;
                vm.assignPermissionTeams.push({ 'assigneTeamRowNumber': newItemNo, 'assignedRole': vm.assignRoles[0], 'assignedPermission': vm.assignPermissions[0] });
            };
            //#endregion

            //#region for removing the added users
            vm.removeAssignPermissionsRow = function (index) {
                var remainingRows = vm.assignPermissionTeams.length;
                if (1 < remainingRows) {
                    vm.assignPermissionTeams.splice(index, 1);

                }
            };
            //#endregion


            //#region for selecting the typehead values into the input with additional changes
            vm.onSelect = function ($item, $model, $label, value, fucnValue, $event) {
                if ($item && $item.name !== "No results found") {
                    if (value == "team") {
                        $label.assignedUser = $item.name + '(' + $item.email + ')';
                    }
                    if (-1 == cm.oSiteUsers.indexOf($item.email)) {
                        vm.oSiteUsers.push($item.email);
                    }
                }
                else {
                }
            }
            //#endregion
        }]);
})();