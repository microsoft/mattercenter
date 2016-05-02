(function () {
    'use strict';

    angular.module("matterMain")
        .controller('createMatterController', ['$scope', '$state', '$stateParams', 'api',
        function ($scope, $state, $stateParams, api) {
            var cm = this;

            cm.clientNameList = [];
            cm.selectedClientName = null;
            cm.clientId = null;
            cm.popupContainerBackground = "Show";
            cm.popupContainer = "hide";
            cm.areaOfLawTerms = [];
            cm.subAreaOfLawTerms = [];
            var optionsForClientGroup = new Object;
            var optionsForPracticeGroup = new Object;

            function getTaxonomyDetailsForClient(optionsForClientGroup, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getTaxonomyDetails',
                    data: optionsForClientGroup,
                    success: callback
                });

            }

            function getTaxonomyDetailsForPractice(optionsForPracticeGroup, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getTaxonomyDetails',
                    data: optionsForPracticeGroup,
                    success: callback
                });

            }

            optionsForClientGroup = {
                Client: {

                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                },
                TermStoreDetails: {
                    TermGroup: "MatterCenterTerms",
                    TermSetName: "Clients",
                    CustomPropertyName: "ClientURL"
                }
            }

            optionsForPracticeGroup = {
                Client: {

                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                },
                TermStoreDetails: {
                    TermGroup: "MatterCenterTerms",
                    TermSetName: "Practice Groups",
                    CustomPropertyName: "ClientURL"
                }
            }

            //call back function for getting the clientNamesList
            getTaxonomyDetailsForClient(optionsForClientGroup, function (response) {


                cm.clientNameList = response.clientTerms;
                cm.popupContainerBackground = "hide";// jQuery('#myModal').modal('show');


            });

            cm.selectMatterType = function () {
                cm.popupContainerBackground = "Show";
                cm.popupContainer = "Show";
                getTaxonomyDetailsForPractice(optionsForPracticeGroup, function (response) {
                    cm.pracitceGroupList = response.pgTerms;
                });
            }


            cm.selectMatterTypePopUpClose = function () {
                alert();
                cm.popupContainerBackground = "hide";
                cm.popupContainer = "hide";
            }

            cm.getSelectedClientValue = function (client) {

                cm.clientId = cm.selectedClientName;

            }
            cm.getSelectedPracticeGroupValue = function () {


                if (cm.selectedPracticeGroup != null) {
                    cm.areaOfLawTerms = cm.selectedPracticeGroup.areaTerms;
                    cm.subAreaOfLawTerms = cm.selectedPracticeGroup.areaTerms[0].subareaTerms;
                    cm.activeSubAOLTerm = cm.selectedPracticeGroup.areaTerms[0].subareaTerms[0];
                    cm.activeAOLTerm = cm.selectedPracticeGroup.areaTerms[0];
                } else {
                    cm.areaOfLawTerms = cm.subAreaOfLawTerms = null;

                }

            }


            cm.selectedAreaOfLawTerm = function (areaOfLawTerm) {

                cm.activeAOLTerm = areaOfLawTerm;

                cm.subAreaOfLawTerms = areaOfLawTerm.subareaTerms;
                cm.activeSubAOLTerm = areaOfLawTerm.subareaTerms[0];

            }

            cm.selectedSubAreaOfLawTerm = function (subAreaOfLawTerm) {
                cm.activeSubAOLTerm = subAreaOfLawTerm;
            }

            //$scope.SelectModal= function(){
            //   // jQuery('#myModal').modal('show');
            //}

        }]);

})();