(function () {
    'use strict;'
    var app = angular.module("matterMain");
    app.controller('DashBoardController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'dashBoardResource', '$rootScope', 'uiGridConstants', '$location', '$http',
        function matterDashBoard($scope, $state, $interval, $stateParams, api, $timeout, dashBoardResource, $rootScope, uiGridConstants, $location, $http) {
            var vm = this;

            //#region Global Variables
            $scope.documentdrop = false;
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

            //#region Variable to show document count
            vm.isDocumentView = true;
            vm.allDocumentCount = 0;
            vm.myDocumentCount = 0;
            vm.pinDocumentCount = 0;
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

            //#region Document 

            //#region Document Grid Functionality
            vm.documentGridOptions = {
                paginationPageSize: gridOptions.paginationPageSize,
                enablePagination: false,
                enableGridMenu: gridOptions.enableGridMenu,
                enableRowHeaderSelection: gridOptions.enableRowHeaderSelection,
                enableRowSelection: gridOptions.enableRowSelection,
                enableSelectAll: gridOptions.enableSelectAll,
                multiSelect: gridOptions.multiSelect,
                enableFiltering: gridOptions.enableFiltering,
                columnDefs: [
                    { field: 'checker', displayName: 'checked', width: '2%', cellTemplate: '/app/dashboard/cellCheckboxTemplate.html', headerCellTemplate: '/app/dashboard/headerCheckboxTemplate.html', enableColumnMenu: false },
                    { field: 'documentIconUrl', displayName: 'Icon', width: '3%', cellTemplate: '<div class="ui-grid-cell-contents"><img src="{{row.entity.documentIconUrl}}"/></div>', headerCellTemplate: '<div class="ui-grid-cell-contents"><img class="docTypeIconHeader" id="docTypeIcon" style="padding:0" alt="Document type icon" src="https://msmatter.sharepoint.com/_layouts/15/images/generaldocument.png"></div>', enableColumnMenu: false },
    	            { field: 'documentName', displayName: 'Document', width: '25%', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.documentName}}</div>', enableColumnMenu: false },
                    { field: 'documentClientId', displayName: 'Client', width: '15%', cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.documentClientId}}</div>', enableColumnMenu: false },
                    { field: 'documentOwner', displayName: 'Author', width: '14%', enableColumnMenu: false },
                    { field: 'documentModifiedDate', displayName: 'Modified date', width: '20%', enableColumnMenu: false },
                    { field: 'documentId', displayName: 'Document ID', width: '10%', enableColumnMenu: false },
                    { field: 'documentVersion', displayName: 'Version', width: '6%', enableColumnMenu: false },
                    { field: 'pin', width: '5%', cellTemplate: '<div class="ui-grid-cell-contents"><img src="../Images/pin-666.png"/></div>', enableColumnMenu: false }
                ],
                onRegisterApi: function (gridApi) {
                    vm.gridApi = gridApi;
                    //Set the selected row of the grid to selectedRow property of the controller
                    gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                        vm.selectedRow = row.entity
                    });
                }
            }

            //function to toggle check all 
            vm.toggleChecker = function (checked) {
                var rows = vm.gridApi.core.getVisibleRows(vm.gridApi.grid),
                    allChecked = true;

                for (var r = 0; r < rows.length; r++) {
                    if (rows[r].entity.checker !== true) {
                        allChecked = false;
                        break;
                    }
                }
                $("#chkAllDocCheckBox").prop('checked', allChecked);
            };

            //function to check all checkboxes inside grid
            vm.toggleCheckerAll = function (checked) {
                for (var i = 0; i < vm.documentGridOptions.data.length; i++) {
                    vm.documentGridOptions.data[i].checker = checked;
                }
            };
            //#endregion

            //#region api call to get document information
            //api call to get all documents
            function get(options, callback) {
                api({
                    resource: 'documentResource',
                    method: 'get',
                    data: options,
                    success: callback
                });
            }

            //api call to get all pinned documents
            function getPinDocuments(options, callback) {
                api({
                    resource: 'documentResource',
                    method: 'getPinnedDocuments',
                    data: options,
                    success: callback
                });
            }

            //api call to get documents of the current logged in user
            function myDocuments(options, callback) {
                api({
                    resource: 'documentResource',
                    method: 'get',
                    data: options,
                    success: callback
                });
            }
            //#endregion

            //#region function to get the documents based on search term
            vm.getDocuments = function (searchTerm) {
                var documentRequest = {
                    Client: {
                        //ToDo: Need to read from config.js
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
                    },
                    SearchObject: {
                        PageNumber: 1,
                        ItemsPerPage: gridOptions.paginationPageSize,
                        SearchTerm: searchTerm,
                        Filters: {
                            ClientsList: [],
                            FromDate: "",
                            ToDate: "",
                            DocumentAuthor: "",
                            //FilterByMe: 1
                        },
                        Sort:
                          {
                              ByProperty: "LastModifiedTime",
                              Direction: 1
                          }
                    }
                }
                get(documentRequest, function (response) {
                    vm.documentGridOptions.data = response;
                    vm.allDocumentCount = response.length;


                });
            }
            //#endregion

            //#region function to get the documents which are pinned by user
            vm.getPinnedDocuments = function () {
                var client = {
                    //ToDo: Need to read from config.js
                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                }

                getPinDocuments(client, function (response) {
                    if (response) {
                        vm.documentGridOptions.data = response;
                        vm.pinDocumentCount = response.length;
                    }
                });
            }
            //#endregion    	   

            //#region function to get the documents based on login user
            vm.getMyDocuments = function (searchTerm) {
                var documentRequest = {
                    Client: {
                        //ToDo: Need to read from config.js
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
                    },
                    SearchObject: {
                        PageNumber: 1,
                        ItemsPerPage: gridOptions.paginationPageSize,
                        SearchTerm: searchTerm,
                        Filters: {
                            ClientsList: [],
                            FromDate: "",
                            ToDate: "",
                            DocumentAuthor: "",
                            FilterByMe: 1
                        },
                        Sort: {
                            ByProperty: "LastModifiedTime",
                            Direction: 1
                        }
                    }
                }
                get(documentRequest, function (response) {
                    vm.documentGridOptions.data = response;
                    vm.myDocumentCount = response.length;

                });
            }
            //#endregion

            //Call all document related api if view is document
            if (vm.isDocumentView) {
                vm.getDocuments();
                vm.getPinnedDocuments();
                vm.getMyDocuments();
            }
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