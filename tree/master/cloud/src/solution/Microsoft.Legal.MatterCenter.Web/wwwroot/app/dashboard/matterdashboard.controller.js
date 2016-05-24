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
            $scope.checkClient = false;
            //#endregion


            //#region Variable to show matter count
            vm.isMatterView = false;
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
            vm.showclientdrop = function () {
                if (!$scope.clientdropvisible) {
                    $scope.clientdrop = true;
                    $scope.clientdropvisible = true;
                } else {
                    $scope.clientdrop = false;
                    $scope.clientdropvisible = false;
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

}


)();