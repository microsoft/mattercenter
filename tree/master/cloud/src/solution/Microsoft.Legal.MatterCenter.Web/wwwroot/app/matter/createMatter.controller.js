(function () {
    'use strict';

    angular.module("matterMain")
        .controller('createMatterController', ['$scope', '$state', '$stateParams', 'api',
        function ($scope, $state, $stateParams, api) {
            var cm = this;
            cm.clientNameList = [];
            cm.selectedClientName = null;
            var options = new Object;
            alert(cm.selectedClientName);
            function getTaxonomyDetails(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getTaxonomyDetails',
                    data: options,
                    success: callback
                });

            }

            options = {
                Client: {
                    Id: "123456",
                    Name: "Microsoft",
                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                },
                TermStoreDetails: {
                    TermGroup: "MatterCenterTerms",
                    TermSetName: "Clients",
                    CustomPropertyName: "ClientURL"
                }
            }
                
            
            getTaxonomyDetails(options, function (response) {
                console.log(response.clientTerms);
                cm.clientNameList = response.clientTerms;

            });

            $scope.SelectModal= function(){
            jQuery('#myModal').modal('show');
            }

        }]);

})();