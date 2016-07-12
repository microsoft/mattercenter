(function () {
    'use strict;'
    var app = angular.module("matterMain");
    app.controller('DocumentDashBoardController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'documentDashBoardResource', '$rootScope', 'uiGridConstants', '$location', '$http','commonFunctions',
        function documentDashBoardController($scope, $state, $interval, $stateParams, api, $timeout, documentDashBoardResource, $rootScope, uiGridConstants, $location, $http,commonFunctions) {
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
            vm.enable = true;
            vm.totalrecords = 0;
            //#endregion

            //#region Variable to show document count

            vm.allDocumentCount = 0;
            vm.myDocumentCount = 0;
            vm.pinDocumentCount = 0;
            vm.selectedDocuments = [];
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
                paginationPageSize: 6,
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
                    { field: 'documentIconUrl', displayName: 'Icon', width: '2%', cellTemplate: '<div class="ui-grid-cell-contents"><img src="{{row.entity.documentIconUrl}}"/></div>', headerCellTemplate: '<div class="ui-grid-cell-contents"><img class="docTypeIconHeader" id="docTypeIcon" style="padding:0" alt="Document type icon" src=' + configs.uri.SPOsiteURL + '"/_layouts/15/images/generaldocument.png"></div>', enableColumnMenu: false },
    	            { field: 'documentName', displayName: 'Document', width: '20%', cellTemplate: '/app/dashboard/DocumentDashboardCellTemplate.html', enableColumnMenu: false },
                    { field: 'documentClientId', displayName: 'Client', width: '15%', cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.documentClientId}}</div>', enableColumnMenu: false },
                    { field: 'documentOwner', displayName: 'Author', width: '14%', enableColumnMenu: false },
                    { field: 'documentModifiedDate', displayName: 'Modified date', width: '20%', enableColumnMenu: false },
                    { field: 'documentID', displayName: 'Document ID', width: '10%', cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.documentID==""?"NA":row.entity.documentID}}</div>', enableColumnMenu: false },
                    { field: 'documentVersion', displayName: 'Version', width: '6%', enableColumnMenu: false },
                    { field: 'pin', width: '5%', displayName: '', cellTemplate: '<div class="ui-grid-cell-contents pad0"><img src="../Images/{{row.entity.pinType}}-666.png" ng-click="grid.appScope.vm.pinorunpin($event, row.entity)"/></div>', enableColumnMenu: false }
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
                    Url: configs.global.repositoryUrl
                },
                TermStoreDetails: {
                    TermGroup: "MatterCenterTerms",
                    TermSetName: "Clients",
                    CustomPropertyName: "ClientURL"
                }
            };
            //#endregion



            //#region Cart functionality
            vm.cartelements = [];
            //function to toggle check all 
            vm.toggleChecker = function (checked, rowinfo) {
                if (checked) {
                    if (vm.documentsCheckedCount >= 0) {
                        vm.documentsCheckedCount = parseInt(vm.documentsCheckedCount, 10) + 1;
                    }
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
                    if (checked) {
                        vm.cartelements.push(vm.documentGridOptions.data[i]);
                        vm.documentsCheckedCount = vm.documentGridOptions.data.length;
                    }
                    else {
                        vm.cartelements = [];
                        vm.documentsCheckedCount = 0;
                    }
                }

            };

            vm.showMailCartModal = function () {
                if (vm.documentsCheckedCount > 0) {
                    jQuery('#UploadMatterModal').modal("show");
                }
            }

            //Event is going to fire when the user clicks on "Email as attachment" or "Email as link" in the modal window
            vm.downloadEmailAsAttachment = function (downloadAttachmentsAsEmail) {
                //Get all the documents which are checked
                var i = 0;
                angular.forEach(vm.cartelements, function (selectedDocument) {

                    if (selectedDocument.selected) {
                        vm.selectedDocuments.push(selectedDocument);
                        //Display progress icon for each checked item
                        angular.element("#document-" + i).css("display", "block");
                    }
                    i = i + 1;
                });
                i = 0;
                if (vm.selectedDocuments.length > 0) {
                    //Disbale the click event of the button. Once we get the response from the server, enable the click event again
                    vm.enable = false;
                }
                else {
                    vm.enable = true;
                }
                var oEmailRelativePath = '';
                var sFileURLs = ""
                angular.forEach(vm.selectedDocuments, function (selectedDocument) {
                    oEmailRelativePath = trimEndChar(unescape(selectedDocument.documentUrl));
                    oEmailRelativePath = oEmailRelativePath.replace(configs.uri.SPOsiteURL, "$") + "$";
                    if (selectedDocument.documentClientUrl) {
                        sFileURLs += selectedDocument.documentClientUrl + oEmailRelativePath + selectedDocument.documentName + ";";
                    }
                    else {
                        sFileURLs += selectedDocument.documentTeamUrl + oEmailRelativePath + selectedDocument.documentName + ";";
                    }
                });

                var mailAttachmentDetailsRequest = {
                    FullUrl: sFileURLs,
                    IsAttachmentCall: downloadAttachmentsAsEmail
                }

                downloadAttachmentsAsStream(mailAttachmentDetailsRequest, function (response) {
                    var result = encodeURIComponent(response);
                    //Once we get the response, stop the progress
                    angular.forEach(vm.cartelements, function (document) {
                        angular.element("#document-" + i).css("display", "none");
                        i = i + 1;
                    });
                    //clear the selectedDocuments array
                    vm.selectedDocuments = [];
                    //enable the vm.enable so that user can click the link again
                    vm.enable = true;
                })
            }

            function trimEndChar(sOrignalString, sCharToTrim) {
                "use strict";
                if (sOrignalString && sCharToTrim === sOrignalString.substr(-1)) {
                    return sOrignalString.substr(0, sOrignalString.length - 1);
                }
                return sOrignalString;
            }


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

            function getDocumentCounts(options, callback) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'getDocumentCounts',
                    data: options,
                    success: callback
                });
            }

            function downloadAttachmentsAsStream(options, callback) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'downloadattachmentsasstream',
                    data: options,
                    success: callback
                });
            }

            function downloadAttachments(options, callback) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'downloadAttachments',
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

            //#region request object
            var documentRequest = {
                Client: {
                    //ToDo: Need to read from config.js
                    Url: configs.global.repositoryUrl
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
                        FilterByMe: 0
                    },
                    Sort:
                      {
                          ByProperty: "LastModifiedTime",
                          Direction: 1
                      }
                }
            }

            //#endregion


            //#reion This function will get counts for all matters, my matters and pinned matters
            vm.getDocumentCounts = function () {
                vm.lazyloaderdashboard = false;
                documentRequest.SearchObject.PageNumber = 1;
                documentRequest.SearchObject.Filters.FilterByMe = 0;
                documentRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                documentRequest.SearchObject.SearchTerm = "";
                getDocumentCounts(documentRequest, function (response) {
                    vm.allDocumentCount = response.allDocumentCounts;
                    vm.myDocumentCount = response.myDocumentCounts;
                    vm.pinDocumentCount = response.pinnedDocumentCounts;
                    vm.lazyloaderdashboard = true;
                    vm.totalrecords = response.allDocumentCounts;
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                    vm.pagination();
                });
            }
            //#endregion




            //#region function to get the documents based on search term
            vm.getDocuments = function () {

                var pinnedDocumentsRequest = {
                    Url: configs.global.repositoryUrl
                }
                documentRequest.SearchObject.PageNumber = 1;
                documentRequest.SearchObject.Filters.FilterByMe = 0;
                documentRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                documentRequest.SearchObject.SearchTerm = "";
                get(documentRequest, function (response) {
                    //We need to call pinned api to determine whether a matter is pinned or not
                    getPinDocuments(pinnedDocumentsRequest, function (pinnedResponse) {
                        if (pinnedResponse && pinnedResponse.length > 0) {
                            vm.pinDocumentCount = pinnedResponse.length;
                            angular.forEach(pinnedResponse, function (pinobj) {
                                angular.forEach(response, function (res) {
                                    if (pinobj.documentName == res.documentName) {
                                        if (res.isDocumentDone == undefined && !res.isDocumentDone) {
                                            res.isDocumentDone = true;
                                            res.pinType = "unpin"
                                        }
                                    }
                                });
                            });
                            vm.documentGridOptions.data = response;
                            //vm.allDocumentCount = response.length;
                            vm.totalrecords = vm.allDocumentCount;
                            vm.pagination();
                        }
                        else {
                            vm.documentGridOptions.data = response;
                            //vm.allDocumentCount = response.length;
                            vm.totalrecords = vm.allDocumentCount;
                            vm.pagination();
                        }
                    });

                });
            }
            //#endregion

            //#region function to get the documents which are pinned by user
            vm.getPinnedDocuments = function () {
                var client = {
                    //ToDo: Need to read from config.js
                    Url: configs.global.repositoryUrl
                }

                getPinDocuments(client, function (response) {
                    if (response) {
                        vm.documentGridOptions.data = response;
                        //vm.pinDocumentCount = response.length;
                        vm.totalrecords = vm.pinDocumentCount;
                        vm.pagination();
                    }
                });
            }
            //#endregion    	   

            //#region function to get the documents based on login user
            vm.getMyDocuments = function () {
                documentRequest.SearchObject.PageNumber = 1;
                documentRequest.SearchObject.Filters.FilterByMe = 1;
                documentRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                documentRequest.SearchObject.SearchTerm = "";
                get(documentRequest, function (response) {
                    vm.documentGridOptions.data = response;
                    //vm.myDocumentCount = response.length;
                    vm.totalrecords = vm.myDocumentCount;
                    vm.pagination();
                });
            }
            //#endregion

            //$timeout(vm.getDocumentCounts(), 800);
            $interval(function () { vm.getDocumentCounts() }, 800, 3);
            $timeout(vm.getDocuments(), 700);

            //#region This function will pin or unpin the document based on the image button clicked
            vm.pinorunpin = function (e, currentRowData) {

                if (e.currentTarget.src.toLowerCase().indexOf("images/pin-666.png") > 0) {
                    e.currentTarget.src = "../Images/loadingGreen.gif";
                    var pinRequest = {
                        Client: {
                            Url: configs.global.repositoryUrl
                        },
                        documentData: {
                            documentName: currentRowData.documentName,
                            documentVersion: currentRowData.documentVersion,
                            documentClient: currentRowData.documentClient,
                            documentClientId: currentRowData.documentClientId,
                            documentClientUrl: currentRowData.documentClientUrl,
                            documentMatter: currentRowData.documentMatter,
                            documentMatterId: currentRowData.documentMatterId,
                            documentOwner: currentRowData.documentOwner,
                            documentUrl: currentRowData.documentUrl,
                            documentOWAUrl: currentRowData.documentOWAUrl,
                            documentExtension: currentRowData.documentExtension,
                            documentCreatedDate: currentRowData.documentCreatedDate,
                            documentModifiedDate: currentRowData.documentModifiedDate,
                            documentCheckoutUser: currentRowData.documentCheckoutUser,
                            documentMatterUrl: currentRowData.documentMatterUrl,
                            documentParentUrl: currentRowData.documentParentUrl,
                            documentID: currentRowData.documentID,
                            pinType: 'unpin'
                        }
                    }
                    pinDocuments(pinRequest, function (response) {
                        if (response.isDocumentPinned) {
                            e.currentTarget.src = "../images/unpin-666.png";
                            vm.pinDocumentCount = parseInt(vm.pinDocumentCount, 10) + 1;
                        }
                    });
                }
                else if (e.currentTarget.src.toLowerCase().indexOf("images/unpin-666.png") > 0) {
                    e.currentTarget.src = "../Images/loadingGreen.gif";
                    var unpinRequest = {
                        Client: {
                            Url: configs.global.repositoryUrl
                        },
                        documentData: {
                            documentUrl: currentRowData.documentUrl,
                        }
                    }
                    UnpinDocuments(unpinRequest, function (response) {
                        if (response.isDocumentUnPinned) {
                            e.currentTarget.src = "../images/pin-666.png";
                            vm.pinDocumentCount = parseInt(vm.pinDocumentCount, 10) - 1;
                            vm.documentGridOptions.data.splice(vm.documentGridOptions.data.indexOf(currentRowData), 1)
                        }
                    });
                }
            }
            //#endregion

            //#region This event is going to fire when the user clicks onm "Select All" and "UnSelect All" links
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
                    Url: configs.global.repositoryUrl
                },
                SearchObject: {
                    PageNumber: 1,
                    ItemsPerPage: gridOptions.paginationPageSize,
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
                                ByProperty: 'LastModifiedTime',
                                Direction: 1
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

            //#region Pagination

            vm.first = 1;
            vm.last = gridOptions.paginationPageSize;
            vm.total = 0;
            vm.pagenumber = 1;
            vm.fromtopage = vm.first + " - " + vm.last;
            vm.displaypagination = false;

            vm.pagination = function () {
                vm.first = 1;
                vm.last = gridOptions.paginationPageSize;
                vm.total = 0;
                vm.pagenumber = 1;
                vm.fromtopage = vm.first + " - " + vm.last;
                vm.displaypagination = false;
                vm.total = vm.totalrecords - gridOptions.paginationPageSize;
                if (vm.totalrecords > gridOptions.paginationPageSize) {
                    vm.fromtopage = vm.first + " - " + vm.last;
                }
                else {
                    if (vm.total < gridOptions.paginationPageSize) { vm.fromtopage = vm.first + " - " + vm.totalrecords; } else {
                        vm.fromtopage = vm.first + " - " + vm.last;
                    }
                }

                if (vm.totalrecords == 0) {
                    vm.displaypagination = false;
                } else {
                    vm.displaypagination = true;
                }
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            };

            vm.next = function () {
                if (vm.last < vm.totalrecords) {
                    vm.first = vm.first + gridOptions.paginationPageSize;
                    vm.last = vm.last + gridOptions.paginationPageSize;
                    vm.total = vm.totalrecords - gridOptions.paginationPageSize;
                    if (vm.total < gridOptions.paginationPageSize) {
                        vm.fromtopage = vm.first + " - " + vm.totalrecords;
                    } else {
                        vm.fromtopage = vm.first + " - " + vm.last;
                    }
                    vm.pagenumber = vm.pagenumber + 1;
                    documentRequest.SearchObject.PageNumber = vm.pagenumber;
                    documentRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                    get(documentRequest, function (response) {
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
                } else {
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                }
            };

            vm.prev = function () {
                if (vm.last > gridOptions.paginationPageSize) {
                    vm.first = vm.first - gridOptions.paginationPageSize;
                    vm.last = vm.last - gridOptions.paginationPageSize;
                    vm.pagenumber = vm.pagenumber - 1;
                    vm.fromtopage = vm.first + " - " + vm.last;
                    documentRequest.SearchObject.PageNumber = vm.pagenumber;
                    documentRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                    get(documentRequest, function (response) {
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
                } else {
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                }
            };

            //#endregion
        }
    ]);
}


)();