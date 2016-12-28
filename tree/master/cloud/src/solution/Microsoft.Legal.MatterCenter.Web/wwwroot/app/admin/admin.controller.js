(function () {
    'use strict;'
    var app = angular.module("matterMain");
    app.controller('AdminController', ['$scope', '$state', '$stateParams', 'api', 'adminResource', '$filter', '$window', '$rootScope', '$location',
        function adminController($scope, $state, $stateParams, api, adminResource, $filter, $window, $rootScope, $location) {
            var vm = this;
            vm.popupContainerBackground = "hide";
            vm.adminContent = uiconfigs.Admin;

            function get(filterForGet, callback) {
                api({
                    resource: 'adminResource',
                    method: 'Get',
                    data: JSON.stringify(filterForGet),
                    success: callback
                });
            }

            function getconfigsforspo(filterForSPO, callback) {
                api({
                    resource: 'adminResource',
                    method: 'getconfigsforspo',
                    data: JSON.stringify(filterForSPO),
                    success: callback
                });
            }

            vm.updateLabels = function (caseNo) {
                vm.popupContainerBackground = "Show";
                if (caseNo == 1) {
                    var filterForGet = "";
                    get(filterForGet, function (response) {
                        vm.popupContainerBackground = "hide";
                        if (response.isError) {
                            angular.element('#errorDiv').modal('show');
                            console.log(response);
                            vm.errorMsg = "Some error occured in the system."; vm.successMsg = "";
                        } else {
                            console.log(response);
                            angular.element('#errorDiv').modal('show');
                            vm.errorMsg = ""; vm.successMsg = "Updated Successfully.";
                        }
                    });
                } else if (caseNo == 2) {
                    var filterForSPO = "";
                    getconfigsforspo(filterForSPO, function (response) {
                        vm.popupContainerBackground = "hide";
                        if (response.isError) {
                            angular.element('#errorDiv').modal('show');
                            console.log(response);
                            vm.errorMsg = "Some error occured in the system."; vm.successMsg = "";
                        } else {
                            angular.element('#errorDiv').modal('show');
                            vm.errorMsg = ""; vm.successMsg = "Updated Successfully.";
                        }
                    });
                }
            }
            $rootScope.$on('disableOverlay', function (event, data) {
                vm.popupContainerBackground = "hide";
            });

        }]);
})();