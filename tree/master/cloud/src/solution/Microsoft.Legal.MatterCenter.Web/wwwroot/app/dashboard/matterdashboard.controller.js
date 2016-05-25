(function () {
    'use strict;'
    var app = angular.module("matterMain");
    app.controller('MatterDashBoardController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'matterDashBoardResource', '$rootScope', 'uiGridConstants', '$location', '$http',
        function matterDashBoardController($scope, $state, $interval, $stateParams, api, $timeout, matterDashBoardResource, $rootScope, uiGridConstants, $location, $http) {
            var vm = this;            
            $scope.downwarddrop = true;
            $scope.upwarddrop = false;
            $scope.loadLocation = false;
            $scope.AuthornoResults = false;
            $scope.clientdrop = false;
            $scope.clientdropvisible = false;
            $scope.pgdrop = false;
            $scope.pgdropvisible = false;
            $scope.aoldrop = false;
            $scope.aoldropvisible = false;            
            $scope.checkClient = false;
            //#endregion
            //#region Variable to show matter count            
            vm.allMatterCount = 0;
            vm.myMatterCount = 0;
            vm.pinMatterCount = 0;
            
            //#endregion            

            var gridOptions = {
                paginationPageSize: 10,
                enableGridMenu: false,
                enableRowHeaderSelection: false,
                enableRowSelection: true,
                enableSelectAll: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableFiltering: false
            }

            //#region Matter             
            //#region Matter Grid functionality
            vm.matterGridOptions = {
                paginationPageSize: gridOptions.paginationPageSize,
                enableGridMenu: gridOptions.enableGridMenu,
                enableRowHeaderSelection: gridOptions.enableRowHeaderSelection,
                enableRowSelection: gridOptions.enableRowSelection,
                enableSelectAll: gridOptions.enableSelectAll,
                multiSelect: gridOptions.multiSelect,
                enableFiltering: gridOptions.enableFiltering,
                columnDefs: [
                    { field: 'matterName', displayName: 'Matter', cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.matterModifiedDate}}"></div>' },
                    { field: 'matterClient', displayName: 'Client', cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.matterModifiedDate}}"></div>' },
                    { field: 'matterClientId', displayName: 'Client.MatterID', headerTooltip: 'Click to sort by client.matterid', enableCellEdit: true, },
                    { field: 'matterModifiedDate', displayName: 'Modified Date', cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.matterModifiedDate}}"></div>' },
                    { field: 'matterResponsibleAttorney', headerTooltip: 'Click to sort by attorney', displayName: 'Responsible attorney', visible: false },
                    { field: 'matterSubAreaOfLaw', headerTooltip: 'Click to sort by sub area of law', displayName: 'Sub area of law', visible: false },
                    { field: 'matterCreatedDate', headerTooltip: 'Click to sort by matter open date', displayName: 'Open date' },
                ],
                onRegisterApi: function (gridApi) {
                    vm.gridApi = gridApi;
                    //Set the selected row of the grid to selectedRow property of the controller
                    gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                        vm.selectedRow = row.entity
                    });
                }
            }
            //#endregion

            var optionsForClientGroup = {
                Client: {
                    Url: "https://msmatter.sharepoint.com/sites/microsoft"
                },
                TermStoreDetails: {
                    TermGroup: "MatterCenterTerms",
                    TermSetName: "Clients",
                    CustomPropertyName: "ClientURL"
                }
            };

            //input parameters building here for all the api's
            var optionsForPracticeGroup = {
                Client: {
                    Url: "https://msmatter.sharepoint.com/sites/microsoft"
                },
                TermStoreDetails: {
                    TermGroup: "MatterCenterTerms",
                    TermSetName: "Practice Groups",
                    CustomPropertyName: "ContentTypeName",
                    DocumentTemplatesName: "DocumentTemplates"
                }
            }

            function getTaxonomyDetailsForClient(optionsForClientGroup, callback) {
                api({
                    resource: 'matterDashBoardResource',
                    method: 'getTaxonomyDetails',
                    data: optionsForClientGroup,
                    success: callback
                });
            }

            function getTaxonomyDetailsForPractice(optionsForPracticeGroup, callback) {
                api({
                    resource: 'matterDashBoardResource',
                    method: 'getTaxonomyDetails',
                    data: optionsForPracticeGroup,
                    success: callback
                });
            }
            
            getTaxonomyDetailsForClient(optionsForClientGroup, function (response) {
                vm.clients = response.clientTerms;
            });

            getTaxonomyDetailsForPractice(optionsForPracticeGroup, function (response) {
                vm.practiceGroups = response.pgTerms;
                vm.aolTerms = [];
                angular.forEach(response.pgTerms, function (pgTerm) {
                    angular.forEach(pgTerm.areaTerms, function (areaterm) {
                        vm.aolTerms.push(areaterm);
                    });
                })
            });

            vm.checkAll = function (checkAll, type) {
                if (type === 'client') {
                    angular.forEach(vm.clients, function (client) {
                        client.Selected = checkAll;
                    });
                }
                if (type === 'pg') {
                    angular.forEach(vm.practiceGroups, function (pg) {
                        pg.Selected = checkAll;
                    });
                }
                if (type === 'aol') {
                    angular.forEach(vm.aolTerms, function (aol) {
                        aol.Selected = checkAll;
                    });
                }
            }          

            //#region This event is going to fire when the user clicks on "OK" button in the filter panel
            vm.filterSearchOK = function (type) {
                if (type === 'client') {
                    vm.selectedClients = '';
                    angular.forEach(vm.clients, function (client) {
                        if (client.Selected) {
                            vm.selectedClients = vm.selectedClients + client.name + ","
                        }
                    });
                    $scope.clientdrop = false;
                    $scope.clientdropvisible = false;
                }
                if (type === 'pg') {
                    vm.selectedPGs = '';
                    vm.selectedAOLs = '';
                    angular.forEach(vm.practiceGroups, function (pg) {
                        if (pg.Selected) {
                            vm.selectedPGs = vm.selectedPGs + pg.termName + ","
                            //For each of the selected pg's select corresponding aol check boxes automatically and update the aol
                            //textbox accordingly
                            angular.forEach(pg.areaTerms, function (areaterm) {
                                areaterm.Selected = true;
                                vm.selectedAOLs = vm.selectedAOLs + areaterm.termName + ","                                
                            });
                        }
                    });
                    
                    $scope.pgdrop = false;
                    $scope.pgdropvisible = false;
                }

                if (type === 'aol') {
                    vm.selectedAOLs = '';
                    angular.forEach(vm.aolTerms, function (aol) {
                        if (aol.Selected) {
                            vm.selectedAOLs = vm.selectedAOLs + aol.termName + ","
                        }
                    });
                    $scope.aoldrop = false;
                    $scope.aoldropvisible = false;
                }
            }
            //#region This event is going to fire when the user clicks on "Cancel" button in the filter panel
            vm.filterSearchCancel = function (type) {
                $scope.clientdrop = false;
                $scope.clientdropvisible = false;
                $scope.pgdrop = false;
                $scope.pgdropvisible = false;
                $scope.aoldrop = false;
                $scope.aoldropvisible = false;
            }

            
            //#endregion

            //vm.getMatters();
            //vm.getPinnedMatters();
            //vm.getMyMatters();            
            //vm.getPracticeGroups()

            //#endregion 
            //#region Closing and Opening searchbar dropdowns
            vm.showupward = function () {
                $scope.searchdrop = true;
                $scope.downwarddrop = false;
                $scope.upwarddrop = true;
            }
            vm.showdownward = function () {
                $scope.searchdrop = false;
                $scope.upwarddrop = false;
                $scope.downwarddrop = true;
            }
            //#endregion
            //#region Angular Datepicker Starts here
            //Start
            $scope.dateOptions = {
                formatYear: 'yy',
                maxDate: new Date()
            };
            $scope.enddateOptions = {
                formatYear: 'yy',
                maxDate: new Date()
            }
            $scope.$watch('startdate', function (newval, oldval) {
                $scope.enddateOptions.minDate = newval;
            });
            $scope.openStartDate = function ($event) {
                if ($event) {
                    $event.preventDefault();
                    $event.stopPropagation();
                }
                this.openedStartDate = true;
            };
            $scope.openEndDate = function ($event) {
                if ($event) {
                    $event.preventDefault();
                    $event.stopPropagation();
                }
                this.openedEndDate = true;
            };
            $scope.openedStartDate = false;
            $scope.openedEndDate = false;
            //#endregion
            //#region showing and hiding client dropdown
            vm.showClientDrop = function () {
                if (!$scope.clientdropvisible) {
                    $scope.clientdrop = true;
                    $scope.clientdropvisible = true;
                    $scope.pgdrop = false;
                    $scope.pgdropvisible = false;
                    $scope.aoldrop = false;
                    $scope.aoldropvisible = false;
                } else {
                    $scope.clientdrop = false;
                    $scope.clientdropvisible = false;
                    $scope.pgdrop = false;
                    $scope.pgdropvisible = false;
                    $scope.aoldrop = false;
                    $scope.aoldropvisible = false;
                }
            }
            //#endregion

            //#region showing and hiding practice group dropdown
            vm.showPracticegroupDrop = function () {
                if (!$scope.pgdropvisible) {
                    $scope.pgdrop = true;
                    $scope.pgdropvisible = true;
                    $scope.clientdrop = false;
                    $scope.clientdropvisible = false;
                    $scope.aoldrop = false;
                    $scope.aoldropvisible = false;
                } else {                   
                    $scope.clientdrop = false;
                    $scope.clientdropvisible = false;
                    $scope.pgdrop = false;
                    $scope.pgdropvisible = false;
                    $scope.aoldrop = false;
                    $scope.aoldropvisible = false;
                }
            }
            //#endregion

            //#region showing and hiding area of law dropdown
            vm.showAreaofLawDrop = function () {
                if (!$scope.aoldropvisible) {
                    $scope.aoldrop = true;
                    $scope.aoldropvisible = true;
                    $scope.clientdrop = false;
                    $scope.clientdropvisible = false;
                    $scope.pgdrop = false;
                    $scope.pgdropvisible = false;
                } else {
                    $scope.clientdrop = false;
                    $scope.clientdropvisible = false;
                    $scope.pgdrop = false;
                    $scope.pgdropvisible = false;
                    $scope.aoldrop = false;
                    $scope.aoldropvisible = false;
                }
            }
            //#endregion
        }
    ]);
    app.directive("toggletab", function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                $(element).find('a').click(function (e) {
                    e.preventDefault()
                    $(this).tab('show')
                })
            }

        }
    });

})();