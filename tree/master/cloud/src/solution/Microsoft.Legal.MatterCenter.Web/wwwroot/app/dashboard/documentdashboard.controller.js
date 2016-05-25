(function () {
    'use strict;'
    var app = angular.module("matterMain");
    app.controller('DocumentDashBoardController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'documentDashBoardResource', '$rootScope', 'uiGridConstants', '$location', '$http',
        function documentDashBoardController($scope, $state, $interval, $stateParams, api, $timeout, documentDashBoardResource, $rootScope, uiGridConstants, $location, $http) {
            var vm = this;

            //#region Global Variables
            vm.documentdrop = false;
            vm.downwarddrop = true;
            vm.upwarddrop = false;
            vm.loadLocation = false;
            vm.AuthornoResults = false;
            vm.clientdrop = false;
            vm.clientdropvisible = false;
            vm.checkClient = false;
            vm.sortbydrop = false;
            vm.sortbydropvisible = false;
            vm.sortbytext = 'Relevant';
            //#endregion

            //#region Variable to show document count
            
            vm.allDocumentCount = 0;
            vm.myDocumentCount = 0;
            vm.pinDocumentCount = 0;
            //#endregion

            //#region closing all dropdowns on click of page
            vm.closealldrops = function ($event) {
                $event.stopPropagation();
                vm.searchdrop = false;
                vm.downwarddrop = true;
                vm.upwarddrop = false;
                vm.clientdrop = false;
                vm.clientdropvisible = false;
                vm.sortbydrop = false;
                vm.sortbydropvisible = false;
            }
            //#endregion

            //#region closing and hiding innerdropdowns of search box
            vm.hideinnerdashboarddrop = function ($event) {
                $event.stopPropagation();
                vm.clientdrop = false;
                vm.clientdropvisible = false;
            }
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
                    { field: 'documentIconUrl', displayName: 'Icon', width: '2%', cellTemplate: '<div class="ui-grid-cell-contents"><img src="{{row.entity.documentIconUrl}}"/></div>', headerCellTemplate: '<div class="ui-grid-cell-contents"><img class="docTypeIconHeader" id="docTypeIcon" style="padding:0" alt="Document type icon" src="https://msmatter.sharepoint.com/_layouts/15/images/generaldocument.png"></div>', enableColumnMenu: false },
    	            { field: 'documentName', displayName: 'Document', width: '20%', cellTemplate:'/app/dashboard/DocumentDashboardCellTemplate.html', enableColumnMenu: false },
                    { field: 'documentClientId', displayName: 'Client', width: '15%', cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.documentClientId}}</div>', enableColumnMenu: false },
                    { field: 'documentOwner', displayName: 'Author', width: '14%', enableColumnMenu: false },
                    { field: 'documentModifiedDate', displayName: 'Modified date', width: '20%', enableColumnMenu: false },
                    { field: 'documentId', displayName: 'Document ID', width: '10%', enableColumnMenu: false },
                    { field: 'documentVersion', displayName: 'Version', width: '6%', enableColumnMenu: false },
                    { field: 'pin', width: '5%', cellTemplate: '<div class="ui-grid-cell-contents pad0"><img src="../Images/pin-666.png"/></div>', enableColumnMenu: false }
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
                    resource: 'documentDashBoardResource',
                    method: 'get',
                    data: options,
                    success: callback
                });
            }

            //api call to get all pinned documents
            function getPinDocuments(options, callback) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'getPinnedDocuments',
                    data: options,
                    success: callback
                });
            }

            //api call to get documents of the current logged in user
            function myDocuments(options, callback) {
                api({
                    resource: 'documentDashBoardResource',
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
            
            $timeout(vm.getDocuments(),500);
            //$timeout(vm.getPinnedDocuments(),500);
            //$timeout(vm.getMyDocuments(),700);
            
            //#endregion

            //#region Closing and Opening searchbar dropdowns
            vm.showupward = function ($event) {
                $event.stopPropagation();
                vm.searchdrop = true;
                vm.downwarddrop = false;
                vm.upwarddrop = true;
            }

            vm.showdownward = function ($event) {
                $event.stopPropagation();
                vm.searchdrop = false;
                vm.upwarddrop = false;
                vm.downwarddrop = true;
            }
            //#endregion

            //#region Showing and Hiding the sortby dropdown
            vm.showsortby = function ($event) {
                $event.stopPropagation();
                if (!vm.sortbydropvisible) {
                    vm.sortbydrop = true;
                    vm.sortbydropvisible = true;
                } else {
                    vm.sortbydrop = false;
                    vm.sortbydropvisible = false;
                }
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
            vm.showclientdrop = function ($event) {
                $event.stopPropagation();
                if (!vm.clientdropvisible) {
                    vm.clientdrop = true;
                    vm.clientdropvisible = true;
                } else {
                    vm.clientdrop = false;
                    vm.clientdropvisible = false;
                }
            }


            //#endregion

            //#region For Sorting by Alphebatical or Created date

            vm.sortyby = function (data) {
                vm.sortbytext = data;
                vm.sortbydrop = false;
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