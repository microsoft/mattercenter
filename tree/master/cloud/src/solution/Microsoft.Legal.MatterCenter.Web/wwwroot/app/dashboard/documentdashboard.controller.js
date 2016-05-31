(function () {
    'use strict;'
    var app = angular.module("matterMain");
    app.controller('DocumentDashBoardController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'documentDashBoardResource', '$rootScope', 'uiGridConstants', '$location', '$http',
        function documentDashBoardController($scope, $state, $interval, $stateParams, api, $timeout, documentDashBoardResource, $rootScope, uiGridConstants, $location, $http) {
            var vm = this;
            vm.selected = undefined;
            vm.selectedAuthor = undefined;
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
            vm.documentsCheckedCount = 0;
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
                enableHorizontalScrollbar: 0,
                enableVerticalScrollbar: 0,
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
    	            { field: 'documentName', displayName: 'Document', width: '20%', cellTemplate: '/app/dashboard/DocumentDashboardCellTemplate.html', enableColumnMenu: false },
                    { field: 'documentClientId', displayName: 'Client', width: '15%', cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.documentClientId}}</div>', enableColumnMenu: false },
                    { field: 'documentOwner', displayName: 'Author', width: '14%', enableColumnMenu: false },
                    { field: 'documentModifiedDate', displayName: 'Modified date', width: '20%', enableColumnMenu: false },
                    { field: 'documentID', displayName: 'Document ID', width: '10%', cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.documentID==""?"NA":row.entity.documentID}}</div>', enableColumnMenu: false },
                    { field: 'documentVersion', displayName: 'Version', width: '6%', enableColumnMenu: false },
                    { field: 'pin', width: '5%', cellTemplate: '<div class="ui-grid-cell-contents pad0"><img src="../Images/pin-666.png" ng-click="grid.appScope.vm.pinorunpin($event, row.entity)"/></div>', enableColumnMenu: false }
                ],
                onRegisterApi: function (gridApi) {
                    vm.gridApi = gridApi;
                    //Set the selected row of the grid to selectedRow property of the controller
                    gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                        vm.selectedRow = row.entity
                    });
                }
            }

            //#region for client taxonomy
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


            //#endregion

            vm.showMailCartModal = function () {
                if (vm.documentsCheckedCount > 0) {
                    jQuery('#UploadMatterModal').modal("show");
                }
            }

            //#region Cart functionality
            vm.cartelements = [];

            //function to toggle check all 
            vm.toggleChecker = function (checked, rowinfo) {
                if (checked) {
                    vm.documentsCheckedCount = parseInt(vm.documentsCheckedCount, 10) + 1;
                    vm.cartelements.push(rowinfo);
                }
                else {
                    vm.documentsCheckedCount = parseInt(vm.documentsCheckedCount, 10) - 1
                    var rows = vm.gridApi.core.getVisibleRows(vm.gridApi.grid),
                        allChecked = true;
                    for (var r = 0; r < rows.length; r++) {
                        if (rows[r].entity.checker !== true) {
                            allChecked = false;
                            break;
                        }
                    }
                    $("#chkAllDocCheckBox").prop('checked', allChecked);
                }
            };

            //Removing elements from cart
            vm.removeAttachment = function (obj) {
                angular.forEach(vm.cartelements, function (element) {
                    if (element.documentID == obj.documentID) {
                        if (jQuery("#doc" + obj.documentID).is(":checked")) {
                            jQuery("#doc" + obj.documentID).prop("checked", false);
                        }
                        vm.documentsCheckedCount = parseInt(vm.documentsCheckedCount, 10) - 1;
                        vm.cartelements.pop(element);
                        if (vm.cartelements.length == 0) {
                            vm.documentsCheckedCount = 0;
                        }
                    }
                });

            }

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

            //Callback function for pin 
            function pinDocuments(options, callback) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'pinDocument',
                    data: options,
                    success: callback
                });
            }


            //Callback function for unpin 
            function UnpinDocuments(options, callback) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'unPinDocument',
                    data: options,
                    success: callback
                });
            }

            function getTaxonomyDetailsForClient(optionsForClientGroup, callback) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'getTaxonomyDetails',
                    data: optionsForClientGroup,
                    success: callback
                });
            }

            //#endregion

            //#region function to get the documents based on search term
            vm.getDocuments = function () {
                var documentRequest = {
                    Client: {
                        //ToDo: Need to read from config.js
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
                    },
                    SearchObject: {
                        PageNumber: 1,
                        ItemsPerPage: gridOptions.paginationPageSize,
                        SearchTerm: "",
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
            vm.getMyDocuments = function () {
                var documentRequest = {
                    Client: {
                        //ToDo: Need to read from config.js
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
                    },
                    SearchObject: {
                        PageNumber: 1,
                        ItemsPerPage: gridOptions.paginationPageSize,
                        SearchTerm: "",
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

            $timeout(vm.getDocuments(), 700);
            //vm.getPinnedDocuments();
            //vm.getMyDocuments();

            //#endregion

            //This function will pin or unpin the matter based on the image button clicked
            vm.pinorunpin = function (e, currentRowData) {

                if (e.currentTarget.src.toLowerCase().indexOf("images/pin-666.png") > 0) {
                    e.currentTarget.src = "../Images/loadingGreen.gif";
                    var pinRequest = {
                        Client: {
                            Url: "https://msmatter.sharepoint.com/sites/catalog"//ToDo: Need to read from config.js file
                        },
                        documentData: {
                            documentName: currentRowData.documentName,
                            DocumentVersion: currentRowData.DocumentVersion,
                            DocumentClient: currentRowData.DocumentClient,
                            DocumentClientId: currentRowData.DocumentClientId,
                            DocumentClientUrl: currentRowData.DocumentClientUrl,
                            DocumentMatter: currentRowData.DocumentMatter,
                            DocumentMatterId: currentRowData.DocumentMatterId,
                            DocumentOwner: currentRowData.DocumentOwner,
                            DocumentUrl: currentRowData.DocumentUrl,
                            DocumentOWAUrl: currentRowData.DocumentOWAUrl,
                            DocumentExtension: currentRowData.DocumentExtension,
                            DocumentCreatedDate: currentRowData.DocumentCreatedDate,
                            DocumentModifiedDate: currentRowData.DocumentModifiedDate,
                            DocumentCheckoutUser: currentRowData.DocumentCheckoutUser,
                            DocumentMatterUrl: currentRowData.DocumentMatterUrl,
                            DocumentParentUrl: currentRowData.DocumentParentUrl,
                            DocumentID: currentRowData.DocumentID
                        }
                    }
                    pinDocuments(pinRequest, function (response) {
                        if (response.isDocumentPinned) {
                            e.currentTarget.src = "../images/unpin-666.png";
                            vm.pinMatterCount = parseInt(vm.pinMatterCount, 10) + 1;
                        }
                    });
                }
                else if (e.currentTarget.src.toLowerCase().indexOf("images/unpin-666.png") > 0) {
                    e.currentTarget.src = "../Images/loadingGreen.gif";
                    var unpinRequest = {
                        Client: {
                            Url: "https://msmatter.sharepoint.com/sites/catalog"//ToDo: Need to read from config.js file
                        },
                        matterData: {
                            documentName: currentRowData.matterUrl,
                        }
                    }
                    UnpinDocuments(unpinRequest, function (response) {
                        if (response.isMatterUnPinned) {
                            e.currentTarget.src = "../images/pin-666.png";
                            vm.pinMatterCount = parseInt(vm.pinMatterCount, 10) - 1;
                            vm.matterGridOptions.data.splice(vm.matterGridOptions.data.indexOf(currentRowData), 1)
                        }
                    });
                }

            }

            //#region This event is going to file when the user clicks onm "Select All" and "UnSelect All" links
            vm.checkAll = function (checkAll, type, $event) {
                $event.stopPropagation();
                if (type === 'client') {
                    angular.forEach(vm.clients, function (client) {
                        client.Selected = checkAll;
                    });
                }
            }
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
            vm.dateOptions = {

                formatYear: 'yy',
                maxDate: new Date()
            };


            vm.enddateOptions = {
                formatYear: 'yy',
                maxDate: new Date()
            }

            $scope.$watch('vm.startdate', function (newval, oldval) {
                vm.enddateOptions.minDate = newval;
            });


            vm.openStartDate = function ($event) {
                if ($event) {
                    $event.preventDefault();
                    $event.stopPropagation();
                }
                this.openedStartDate = true;
            };
            vm.openEndDate = function ($event) {
                if ($event) {
                    $event.preventDefault();
                    $event.stopPropagation();
                }
                this.openedEndDate = true;
            };

            vm.openedStartDate = false;
            vm.openedEndDate = false;
            //#endregion

            //#region showing and hiding client dropdown
            vm.showclientdrop = function ($event) {
                $event.stopPropagation();
                if (!vm.clientdropvisible) {
                    if (vm.clients === undefined) {
                        getTaxonomyDetailsForClient(optionsForClientGroup, function (response) {
                            vm.clients = response.clientTerms;
                        });
                    }
                    vm.clientdrop = true;
                    vm.clientdropvisible = true;
                } else {
                    vm.clientdrop = false;
                    vm.clientdropvisible = false;
                }
            }


            //#endregion

            //#region For Sorting by Alphebatical or Created date
            var SortRequest = {
                Client: {
                    Id: "123456",
                    Name: "Microsoft",
                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                },
                SearchObject: {
                    PageNumber: 1,
                    ItemsPerPage: 10,
                    SearchTerm: "",
                    Filters: {
                        ClientName: "",
                        ClientsList: [],
                        PGList: [],
                        AOLList: [],
                        DateFilters: {
                            CreatedFromDate: "",
                            CreatedToDate: "",
                            ModifiedFromDate: "",
                            ModifiedToDate: "",
                            OpenDateFrom: "",
                            OpenDateTo: ""
                        },
                        DocumentAuthor: "",
                        DocumentCheckoutUsers: "",
                        FilterByMe: 0,
                        FromDate: "",
                        Name: "",
                        ResponsibleAttorneys: "",
                        SubareaOfLaw: "",
                        ToDate: ""
                    },
                    Sort:
                            {
                                ByProperty: '',
                                Direction: 0
                            }
                }
            }

            vm.FilterByType = function () {
                get(SortRequest, function (response) {
                    vm.lazyloader = true;
                    if (response.errorCode == "404") {
                        vm.divuigrid = false;
                        vm.nodata = true;
                        vm.errorMessage = response.message;
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                        vm.documentGridOptions.data = response;
                        if (!$scope.$$phase) {
                            $scope.$apply();
                        }
                    }
                });
            }


            vm.sortyby = function (sortexp, data) {
                vm.sortbytext = data;
                vm.sortbydrop = false;
                if (sortexp == 'AlphabeticalUp') {
                    vm.lazyloader = false;
                    SortRequest.SearchObject.Sort.ByProperty = "FileName";
                    SortRequest.SearchObject.Sort.Direction = 0;
                    vm.FilterByType();
                } else if (sortexp == 'AlphabeticalDown') {
                    vm.lazyloader = false;
                    SortRequest.SearchObject.Sort.ByProperty = "FileName";
                    SortRequest.SearchObject.Sort.Direction = 1;
                    vm.FilterByType();
                } else if (sortexp == 'CreateddateUp') {
                    vm.lazyloader = false;
                    SortRequest.SearchObject.Sort.ByProperty = "Created";
                    SortRequest.SearchObject.Sort.Direction = 0;
                    vm.FilterByType();
                }
                else if (sortexp == 'CreateddateDown') {
                    vm.lazyloader = false;
                    SortRequest.SearchObject.Sort.ByProperty = "Created";
                    SortRequest.SearchObject.Sort.Direction = 1;
                    vm.FilterByType();
                }
                else if (sortexp == 'ModifieddateUp') {
                    vm.lazyloader = false;
                    SortRequest.SearchObject.Sort.ByProperty = "MCModifiedDate";
                    SortRequest.SearchObject.Sort.Direction = 0;
                    vm.FilterByType();
                }
                else if (sortexp == 'ModifieddateDown') {
                    vm.lazyloader = false;
                    SortRequest.SearchObject.Sort.ByProperty = "MCModifiedDate";
                    SortRequest.SearchObject.Sort.Direction = 1;
                    vm.FilterByType();
                }
                else {
                    vm.lazyloader = false;
                    SortRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                    SortRequest.SearchObject.Sort.Direction = 1;
                    vm.FilterByType();
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

    app.directive('infopopover', function () {
        return {
            restrict: 'AE',
            link: function (scope, element, attrs) {
                var content = "<div class='cartelementpopup' style='width:250px'>\
                                   <div class='checkOutMetadata marginTop10'>When sending to your OneDrive the DMS version will be checked out to you until you check it back in.</div>\
                               </div>";
                $(element).popover({
                    html: true,
                    trigger: 'click',
                    delay: 500,
                    content: content,
                });
            }
        }
    });
}


)();