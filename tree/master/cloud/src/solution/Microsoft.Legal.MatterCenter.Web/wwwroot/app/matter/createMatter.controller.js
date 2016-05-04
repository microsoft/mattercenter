(function () {
    'use strict';

    var app=  angular.module("matterMain");
        app.controller('createMatterController', ['$scope', '$state', '$stateParams', 'api',
        function ($scope, $state, $stateParams, api) {
            var cm = this;
            var oPageOneState={
                ClientValue: [],
                ClientId: "",
                MatterName: "",
                MatterId: "",
                MatterDescription: "",
                ContentTypes: [],
                matterMandatory: "",
                oAreaOfLawTerms: [],
                oSubAreaOfLawTerms: [],
                oSelectedDocumentTypeLawTerms:[]
            }
            cm.clientId = "";
            cm.selectedClientName = "";
            cm.matterName="";
            cm.matterId="";
            cm.matterDescription="";

            cm.clientNameList = [];           
            cm.areaOfLawTerms = [];
            cm.subAreaOfLawTerms = [];        
            cm.documentTypeLawTerms = [];
            cm.selectedDocumentTypeLawTerms=[];
            cm.activeAOLTerm = null;
            cm.activeSubAOLTerm = null;
            cm.activeDocumentTypeLawTerm = null;
            cm.popupContainerBackground = "Show";
            cm.popupContainer = "hide";

            cm.sectionName = "snOpenMatter";
            cm.removeDTItem = false;
            cm.primaryMatterType = cm.errorPopUp = false;

            var optionsForClientGroup = new Object;
            var optionsForPracticeGroup = new Object;
            var optionsForCheckMatterName = new Object;
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

            function getCheckValidMatterName(optionsForCheckMatterName, callback) {
                api({
                    resource: 'matterResource',
                    method: 'checkMatterExists',
                    data: optionsForCheckMatterName,
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
                    CustomPropertyName: "ContentTypeName",
                    DocumentTemplatesName:"DocumentTemplates"
                }
            }

            optionsForCheckMatterName={
                Client: {

                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                },
                Matter: {
                    MatterGuid: "",
                    Name:cm.matterName.trim()
                }
            }


            

            //call back function for getting the clientNamesList
            getTaxonomyDetailsForClient(optionsForClientGroup, function (response) {
                console.log(response);

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
                if (cm.popupContainer == "Show") {
                    cm.popupContainerBackground = "hide";
                    cm.popupContainer = "hide";
                } 
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
                    cm.errorPopUp = false;
                } else {
                    cm.areaOfLawTerms = cm.subAreaOfLawTerms = null;

                }

            }

            //function to get the subAOL items on selection of AOLTerm
            cm.selectAreaOfLawTerm = function (areaOfLawTerm) {
                cm.errorPopUp = false;
                cm.activeAOLTerm = areaOfLawTerm;

                cm.subAreaOfLawTerms = areaOfLawTerm.subareaTerms;
                cm.activeSubAOLTerm = areaOfLawTerm.subareaTerms[0];

            }
            ////function to for seclection of subAOL items 
            cm.selectSubAreaOfLawTerm = function (subAreaOfLawTerm) {
                cm.errorPopUp = false;
                cm.activeSubAOLTerm = subAreaOfLawTerm;
                
            }

            cm.selectDocumentTemplateTypeLawTerm = function (documentTemplateTypeLawTerm) {
                // alert(documentTemplateTypeLawTerm);
                if (documentTemplateTypeLawTerm != null) {
                    cm.errorPopUp = false;;
                    cm.removeDTItem = true;
                    cm.activeDocumentTypeLawTerm = documentTemplateTypeLawTerm;
                    cm.primaryMatterType = true;
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
                        console.log(cm.selectedPracticeGroup);
                        console.log(cm.activeAOLTerm);
                        console.log(cm.activeSubAOLTerm);
                       
                        cm.documentTypeLawTerms.push(cm.activeSubAOLTerm);
                        cm.activeDocumentTypeLawTerm = null;
                        //cm.primaryMatterType = true; alert(cm.primaryMatterType);
                      //  cm.activeSubAOLTerm = null;
                    }
                }
            }

            cm.removeFromDocumentTemplate = function () {
                //  alert(cm.activeDocumentTypeLawTerm);
                if (cm.removeDTItem) {
                    var index = cm.documentTypeLawTerms.indexOf(cm.activeDocumentTypeLawTerm);
                    cm.documentTypeLawTerms.splice(index, 1);
                    cm.removeDTItem = false;
                    cm.primaryMatterType = false;
                    cm.activeDocumentTypeLawTerm = null;
                }
               
            }
            cm.checkValidMatterName = function () {
                optionsForCheckMatterName.Matter.Name = cm.matterName.trim();
                getCheckValidMatterName(optionsForCheckMatterName, function (response) {
                    if (response.code != 200) {
                        alert("Matter Name already Exists");
                    } else {
                        alert("success");
                    }
                });

            }

            cm.navigateToSecondSection = function (sectionName) {
                if ("" !== cm.selectedClientName.trim() && null!==cm.selectedClientName) {
                    if ("" !== cm.clientId.trim() && null !== cm.clientId) {
                        if ("" != cm.matterName.trim() && null !== cm.matterName) {
                            if ("" !== cm.matterId.trim() && null != cm.matterId)
                            {
                                if ("" !== cm.matterDescription.trim() && null !== cm.matterDescription) {
                                    alert(cm.selectedDocumentTypeLawTerms.length);
                                    if (cm.selectedDocumentTypeLawTerms.length > 0) {

                                        oPageOneState.ClientValue.push({ ClientName: cm.clientNameList });
                                        oPageOneState.ClientId = cm.clientId.trim();
                                        oPageOneState.MatterTitle = cm.matterName.trim();
                                        oPageOneState.MatterId = cm.matterId.trim();
                                        oPageOneState.MatterDescription = cm.matterDescription.trim();
                                        oPageOneState.oAreaOfLawTerms = cm.areaOfLawTerms;
                                        oPageOneState.oSubAreaOfLawTerms = cm.subAreaOfLawTerms;
                                        oPageOneState.oSelectedDocumentTypeLawTerms = cm.selectedDocumentTypeLawTerms;
                                        localStorage.setItem('oPageOneData', JSON.stringify(oPageOneState));
                                        cm.sectionName = sectionName;
                                    }
                                }
                                else {
                                    alert(cm.matterDescription);
                                }
                            }
                            else {
                                alert(cm.matterId);
                            }

                        }
                        else {
                            alert(cm.matterName);
                        }

                    }
                    else {
                        alert(cm.clientId);
                    }
                }
                else {
                    alert(cm.selectedClientName);
                }
               
                //if (sectionName == "snConflictCheck") {
                //    if (cm.clientId != null && cm.selectedClientName != null){

                //        if()
                //    }
                //        && (cm.matterName != null || cm.matterName.trim() != '') && (cm.matterId != null || cm.matterId.trim() != '') && (cm.matterDescription != null || cm.matterDescription.trim() != '')) {
                //        cm.sectionName = sectionName;

                //    }
                //    else {
                      
                //        alert("Incomplete");
                //    }
                //}
                //else {
                //    cm.sectionName = sectionName;
                //}
            }
           
            cm.saveDocumentTemplates = function () {
               
                if (cm.primaryMatterType) {
                    cm.errorPopUp = false;
                    angular.forEach(cm.documentTypeLawTerms, function (term) {
                        var primaryType = false;
                        //For loop
                        if (cm.activeDocumentTypeLawTerm.id == term.id) {// this line will check whether the data is existing or not
                            primaryType = true;
                        }
                        term.primaryMatterType = primaryType;
                        cm.popupContainerBackground = "hide";
                        cm.popupContainer = "hide";
                       
                    });

                    cm.selectedDocumentTypeLawTerms = cm.documentTypeLawTerms;
                }
                else {
                    cm.errorPopUp=true;
                }
            }
            //$scope.SelectModal= function(){
            //   // jQuery('#myModal').modal('show');
            //}

        }]);

        app.filter('getAssociatedDocumentTemplatesCount', function () {
            return function (input, splitChar) {

                return input.split(splitChar).length;;
            }
        });
      


       
})();