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
            cm.documentTypeLawTerms = [];
            cm.activeAOLTerm = null;
            cm.activeSubAOLTerm = null;
            cm.activeDocumentTypeLawTerm = null;
            cm.sectionName = "snOpenMatter";
            cm.removeDTItem = false;
            
           
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

            //calls this function when selectType button clicks
            cm.selectMatterType = function () {
                cm.popupContainerBackground = "Show";
                if (cm.pracitceGroupList == null) {
                    getTaxonomyDetailsForPractice(optionsForPracticeGroup, function (response) {
                        cm.pracitceGroupList = response.pgTerms;
                        cm.popupContainer = "Show";
                        cm.popupContainerBackground = "Show";
                    });
                }
                else {
                    cm.popupContainer = "Show";
                    cm.popupContainerBackground = "Show";
                }
            }

            //function for closing the popup
            cm.selectMatterTypePopUpClose = function () {               
                cm.popupContainerBackground = "hide";
                cm.popupContainer = "hide";
            }
            //function to get the clientId from ClientName dropdown
            cm.getSelectedClientValue = function (client) {
                
                cm.clientId = cm.selectedClientName;

            }
            //function to get the all AOL and SAOL terms
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

            //function to get the subAOL items on selection of AOLTerm
            cm.selectAreaOfLawTerm = function (areaOfLawTerm) {

                cm.activeAOLTerm = areaOfLawTerm;

                cm.subAreaOfLawTerms = areaOfLawTerm.subareaTerms;
                cm.activeSubAOLTerm = areaOfLawTerm.subareaTerms[0];

            }
            ////function to for seclection of subAOL items 
            cm.selectSubAreaOfLawTerm = function (subAreaOfLawTerm) {
                cm.activeSubAOLTerm = subAreaOfLawTerm;
                
            }

            cm.selectDocumentTemplateTypeLawTerm = function (documentTemplateTypeLawTerm) {
                // alert(documentTemplateTypeLawTerm);
                if (documentTemplateTypeLawTerm != null) {
                    cm.removeDTItem = true;
                    cm.activeDocumentTypeLawTerm = documentTemplateTypeLawTerm;
                }

            }

            cm.addToDocumentTemplate = function () {
                var   isThisNewDocTemplate = true;
                if (cm.activeSubAOLTerm != null) {
                    angular.forEach(cm.documentTypeLawTerms, function (term) { //For loop
                        if (cm.activeSubAOLTerm.id == term.id) {// this line will check whether the data is existing or not

                            isThisNewDocTemplate = false;
                        }
                    });
                    if (isThisNewDocTemplate) {

                        cm.documentTypeLawTerms.push(cm.activeSubAOLTerm);
                        cm.activeSubAOLTerm = null;

                    }
                }
            }

            cm.removeFromDocumentTemplate = function () {
                //  alert(cm.activeDocumentTypeLawTerm);
                if (cm.removeDTItem) {
                    var index = cm.documentTypeLawTerms.indexOf(cm.activeDocumentTypeLawTerm);
                    cm.documentTypeLawTerms.splice(index, 1);
                    cm.removeDTItem = false;
                    cm.activeDocumentTypeLawTerm = null;
                }
               
            }


            cm.navigateToSecondSection = function (sectionName) {
                if (sectionName == "snConflictCheck") {
                    if (cm.clientId != null && cm.selectedClientName != null && (cm.matterName != null || cm.matterName.trim() != '') && (cm.matterId != null || cm.matterId.trim() != '') && (cm.matterDescription != null || cm.matterDescription.trim() != '')) {
                        cm.sectionName = sectionName;

                    }
                    else {
                        alert("Incomplete");
                    }
                }
                else {
                    cm.sectionName = sectionName;
                }
            }
           
            cm.saveDocumentTemplates = function () {

            }
            //$scope.SelectModal= function(){
            //   // jQuery('#myModal').modal('show');
            //}

        }]);

})();