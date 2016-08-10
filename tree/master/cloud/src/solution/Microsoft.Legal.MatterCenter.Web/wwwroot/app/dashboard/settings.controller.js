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

            //#region global variables
            $rootScope.bodyclass = "bodymain";
            $rootScope.profileClass = "hide";

            //#region for hiding the client details on load
            vm.showClientDetails = false;

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

            //#region for calling the getTaxonomydata function
            vm.getTaxonomyData();

            //#region for showing the selected clients Data
            vm.showSelectedClient = function (name) {
                vm.selected = name;
                vm.clientlist = false;
                vm.showClientDetails = true;
            }

        }]);
})();