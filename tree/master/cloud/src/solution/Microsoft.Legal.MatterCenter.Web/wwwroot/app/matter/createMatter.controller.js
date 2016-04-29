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
            var optionsForPracticeGroup=new Object;
            
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
                
            
            getTaxonomyDetailsForClient(optionsForClientGroup, function (response) {
                
               
                cm.clientNameList = response.clientTerms;
                cm.popupContainerBackground = "hide";// jQuery('#myModal').modal('show');
                //alert(cm.popupContainerBackground);

            });
            
            cm.selectMatterType = function () {
                cm.popupContainerBackground = "Show";
                cm.popupContainer = "Show";
                getTaxonomyDetailsForPractice(optionsForPracticeGroup, function (response) {
                  
                  //  console.log(response.pgTerms);
                    cm.pracitceGroupList = response.pgTerms;
                    

                });
                
            }

            cm.getSelectedClientValue = function (client) {
              //  console.log(cm.selectedClientName);
                cm.clientId = cm.selectedClientName;
                //alert(client.name+","+client.id);
            }
            cm.getSelectedPracticeGroupValue = function () {

                console.log(cm.selectedPracticeGroup);
                cm.areaOfLawTerms = cm.selectedPracticeGroup.areaTerms;
               // var aolContent = " <div class='popUpMDContent ' ng-repeat='areaTerm in cm.selectedPracticeGroup.areaTerms' data-value='0' documents='' data-foldernamesaol='Emails;' data-areaoflaw-id=''></div>";
               
            }


            cm.selectedAreaOfLawTerm = function (areaOfLawTerm) {
                cm.aolSelectedClass = "popUpSelected";
                // alert(areaOfLawTerm.termName);
                cm.subAreaOfLawTerms = areaOfLawTerm.subareaTerms;

            }

            //$scope.SelectModal= function(){
            //   // jQuery('#myModal').modal('show');
            //}

        }]);

})();