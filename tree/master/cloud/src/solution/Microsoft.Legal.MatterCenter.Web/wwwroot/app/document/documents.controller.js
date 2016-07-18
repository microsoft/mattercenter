/// <reference path="documentheadertemplate.html" />
/// <reference path="documentheadertemplate.html" />
(function () {
    'use strict';

    var app = angular.module("matterMain");

    app.controller('documentsController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout',
        'documentResource', '$rootScope', 'uiGridConstants', '$location', '$http', '$templateCache', '$window', '$q', '$filter', 'commonFunctions',
    function ($scope, $state, $interval, $stateParams, api, $timeout,
        documentResource, $rootScope, uiGridConstants, $location, $http, $templateCache, $window, $q, $filter, commonFunctions) {
        var vm = this;
        vm.selected = undefined;
        vm.documentname = 'All Documents'
        vm.documentid = 1;
        vm.documentsdrop = false;
        vm.docdropinner = true;
        $rootScope.pageIndex = "2";
        $rootScope.bodyclass = "bodymain";
        // Onload show ui grid and hide error div
        //start
        vm.divuigrid = true;
        vm.nodata = false;
        //end

        //#region scopes for displaying and hiding filter icons
        //start
        vm.documentfilter = false;
        vm.moddatefilter = false;
        vm.createddatefilter = false;
        vm.clientfilter = false;
        vm.authorfilter = false;
        vm.checkoutfilter = false;
        //end

        //#region for showing the matters dropdown in resposive 
        vm.showdocdrop = function ($event) {
            $event.stopPropagation();
            if (vm.docdropinner) {
                vm.documentsdrop = true;
                vm.docdropinner = false;
            } else {
                vm.documentsdrop = false;
                vm.docdropinner = true;
            }
        }
        //#endregion

        //#region for closing all the dropdowns
        vm.closealldrops = function () {
            vm.documentsdrop = false;
            vm.docdropinner = true;
            vm.documentheader = true;
            vm.documentdateheader = true;
        }

        //#endregion

        //to hide lazyloader on load
        //start
        vm.lazyloader = true;
        //end        
        vm.bAttachDocumentFailed = false;
        vm.selectedRows = [];
        vm.showAttachmentProgress = false;
        vm.showAttachment = false;
        vm.attachButtonText = configs.uploadMessages.attachButtonText;
        vm.showPopUpHolder = false;
        vm.showErrorAttachmentInfo = false;
        vm.showFailedAtachments = false;
        vm.showSuccessAttachments = false;
        vm.failedFiles = [];
        vm.enableAttachment = false;
        vm.asyncCallCompleted = 0;

        //#region Grid Cell/Header Templates
        //Start
        vm.documentDropDown = false;
        vm.clientDropDown = false;
        vm.modifieddateDropDown = false;
        vm.AuthorDropDown = false;
        vm.CheckDropDown = false;
        vm.createddateDropDown = false;
        //End

        //For setting dynamic height to the grid
        vm.getTableHeight = function () {
            if (vm.isOutlook) {
                return {
                    height: ($window.innerHeight - 150) + "px"
                };
            }
            else {
                return {
                    height: ($window.innerHeight - 105) + "px"
                }
            }
        };

        $templateCache.put('coldefheadertemplate.html', "<div><div role='button' class='ui-grid-cell-contents ui-grid-header-cell-primary-focus' col-index='renderIndex'><span class='ui-grid-header-cell-label ng-binding' title='Click to sort by {{ col.colDef.displayName }}'>{{ col.colDef.displayName }}<span id='asc{{col.colDef.field}}' style='float:right;display:none' class='padl10px'>↑</span><span id='desc{{col.colDef.field}}' style='float:right;display:none' class='padlf10'>↓</span></span></div></div>");

        vm.gridOptions = {
            infiniteScrollDown: true,
            enableHorizontalScrollbar: 0,
            enableVerticalScrollbar: 0,
            enableGridMenu: true,
            enableRowHeaderSelection: false,
            enableRowSelection: true,
            enableSelectAll: true,
            multiSelect: true,
            columnDefs: [
                { field: 'checker', displayName: 'checked', width: '18', cellTemplate: '/app/document/DocumentTemplates/cellCheckboxTemplate.html', headerCellTemplate: '/app/document/DocumentTemplates/headerCheckboxTemplate.html', enableColumnMenu: false },
                { field: 'documentName', displayName: 'Document', width: '280', enableHiding: false, cellTemplate: '../app/document/DocumentTemplates/DocumentCellTemplate.html', headerCellTemplate: '../app/document/DocumentTemplates/DocumentHeaderTemplate.html' },
                { field: 'documentClient', displayName: 'Client', headerCellClass: 'gridclass', cellClass: 'gridclass', width: '200', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.documentClient=="" ? "NA":row.entity.documentClient}}</div>', enableCellEdit: true, headerCellTemplate: '../app/document/DocumentTemplates/ClientHeaderTemplate.html' },
                { field: 'documentClientId', displayName: 'Client.Matter ID', headerCellClass: 'gridclass', cellClass: 'gridclass', width: '150', headerCellTemplate: $templateCache.get('coldefheadertemplate.html'), cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.documentClientId==""?"NA":row.entity.documentClientId}}.{{row.entity.documentMatterId==""?"NA":row.entity.documentMatterId}}</div>', enableCellEdit: true, },
                { field: 'documentModifiedDate', displayName: 'Modified Date', headerCellClass: 'gridclass', cellClass: 'gridclass', width: '195', cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.documentModifiedDate}}"></div>', headerCellTemplate: '../app/document/DocumentTemplates/ModifiedDateHeaderTemplate.html' },
                { field: 'documentOwner', displayName: 'Author', width: '140', headerCellClass: 'gridclass', cellClass: 'gridclass', headerCellTemplate: '/app/document/DocumentTemplates/AuthorHeaderTemplate.html', visible: false },
                { field: 'documentVersion', displayName: 'Document Version', headerCellClass: 'gridclass', cellClass: 'gridclass', width: '200', headerCellTemplate: $templateCache.get('coldefheadertemplate.html'), visible: false },
                { field: 'documentCheckoutUser', displayName: 'Checked out to', headerCellClass: 'gridclass', cellClass: 'gridclass', width: '210', headerCellTemplate: '/app/document/DocumentTemplates/CheckOutHeaderTemplate.html', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.documentCheckoutUser=="" ? "NA":row.entity.documentCheckoutUser}}</div>', visible: false },
                { field: 'documentCreatedDate', displayName: 'Created date', headerCellClass: 'gridclass', cellClass: 'gridclass', width: '170', headerCellTemplate: '/app/document/DocumentTemplates/CreatedDateHeaderTemplate.html', cellTemplate: '<div class="ui-grid-cell-contents" datefilter date="{{row.entity.documentCreatedDate}}"></div>', visible: false },
            ],
            enableColumnMenus: false,
            onRegisterApi: function (gridApi) {
                $scope.gridApi = gridApi;
                gridApi.core.on.columnVisibilityChanged($scope, function (changedColumn) {
                    $scope.columnChanged = { name: changedColumn.colDef.name, visible: changedColumn.colDef.visible };
                });
                gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                    //vm.selectedRow = row.entity
                    vm.selectedRows = $scope.gridApi.selection.getSelectedRows();
                    var isRowPresent = $filter("filter")(vm.selectedRows, row.entity.documentID);
                    if (isRowPresent.length > 0) {
                        row.entity.checker = true;
                    }
                    else {
                        vm.checker = false;
                        row.entity.checker = false;
                    }
                    isOpenedInOutlook();

                });
                $scope.gridApi.core.on.sortChanged($scope, vm.sortChangedDocument);
                vm.sortChangedDocument($scope.gridApi.grid, [vm.gridOptions.columnDefs[1]]);
                //$scope.$watch('gridApi.grid.isScrollingVertically', vm.watchFunc);
                gridApi.infiniteScroll.on.needLoadMoreData($scope, vm.watchFunc);
                vm.setColumns();
            }
        };


        function isOpenedInOutlook() {
            //If the app is opened in outlook, then the below validation is going to be applied
            if (vm.isOutlook && vm.showAttachment) {
                if (vm.selectedRows.length > 0 && vm.selectedRows.length <= 5) {
                    vm.enableAttachment = true
                    vm.showErrorAttachmentInfo = false;
                    vm.warningMessageText = '';
                }
                else {
                    vm.enableAttachment = false;
                    if (vm.selectedRows.length > 5) {
                        vm.warningMessageText = configs.uploadMessages.maxAttachedMessage;
                        vm.showErrorAttachmentInfo = true;
                    }
                }
            }
        }

        var screenHeight = 0;
        vm.searchResultsLength = 0;

        //#region for setting the classes for ui-grid based on size
        vm.setColumns = function () {
            if ($window.innerWidth < 380) {
                $interval(function () {
                    angular.element('#documentgrid .ui-grid-viewport').addClass('viewport');
                    angular.element('#documentgrid .ui-grid-viewport').removeClass('viewportlg');
                }, 1000, 2);
            } else {
                $interval(function () {
                    angular.element('#documentgrid .ui-grid-viewport').removeClass('viewport');
                    angular.element('#documentgrid .ui-grid-viewport').addClass('viewportlg');
                }, 1000, 2);
            }
        }
        //#endregion

        //#region for setting the dynamic width to grid
        vm.setWidth = function () {
            var width = $window.innerWidth;
            angular.element(".ui-grid-viewport").css('max-width', width);
            angular.element(".ui-grid-render-container").css('max-width', width);
            screenHeight = $window.screen.availHeight;
            if (screenHeight <= 768) {
                vm.searchResultsLength = 17;
            } else if (screenHeight <= 1024 && screenHeight >= 769) {
                vm.searchResultsLength = 38;
            } else if (screenHeight <= 1080 && screenHeight >= 1025) {
                vm.searchResultsLength = 42;
            }
        };

        vm.setWidth();

        //#endregion

        //#region functionality for infinite scroll
        //start
        vm.pagenumber = 1;
        vm.responseNull = false;
        vm.watchFunc = function () {
            var promise = $q.defer();
            if (!vm.responseNull) {
                vm.lazyloader = false;
                vm.pagenumber = vm.pagenumber + 1;
                searchRequest.SearchObject.PageNumber = vm.pagenumber;
                searchRequest.SearchObject.SearchTerm = "";
                get(searchRequest, function (response) {
                    if (response == "") {
                        vm.lazyloader = true;
                        vm.responseNull = true;
                    } else {

                        vm.gridOptions.data = vm.gridOptions.data.concat(response);
                        vm.lazyloader = true;
                    }
                    promise.resolve();

                    $scope.gridApi.infiniteScroll.dataLoaded();
                    if (vm.checker) {
                        vm.toggleCheckerAll(vm.checker);
                    }
                });
            } else {
                vm.lazyloader = true;
            }
            return promise.promise;
        }
        //#endregion


        //#region Code for attaching documents in compose more
        Office.initialize = function (reason) {
        }
        var isAppOpenedInOutlook = $location.absUrl().split('|')[0].split('=')[2];
        if (isAppOpenedInOutlook && isAppOpenedInOutlook === "Outlook") {

            vm.isOutlook = true;
            //  vm.isOutlookAsAttachment(vm.isOutlook);
            // }
        }
        vm.isOutlook = true;
        //vm.appType=$location.search().AppType;
        // vm.isOutlook ? vm.isOutlookAsAttachment(vm.isOutlook) : "";
        vm.isOutlookAsAttachment = function (isOutlook) {
            if (isOutlook) {
                //Office.initialize = function (reason) {
                if (Office && Office.context && Office.context.mailbox && Office.context.mailbox.item) {
                    vm.showErrorAttachmentInfo = false;
                    //vm.showFailedAtachments = false;
                    vm.failedFiles = [];
                    vm.asyncCallCompleted = 0;
                    var oCurrentEmailItem = Office.context.mailbox.item.get_data();
                    var sEmailCreatedTime, sEmailModifiedTime;
                    if (oCurrentEmailItem && (oCurrentEmailItem.hasOwnProperty("$0_0") || oCurrentEmailItem.hasOwnProperty("_data$p$0"))) {
                        if (oCurrentEmailItem.hasOwnProperty("$0_0")) {
                            sEmailCreatedTime = oCurrentEmailItem.$0_0.dateTimeCreated;
                            sEmailModifiedTime = oCurrentEmailItem.$0_0.dateTimeModified;
                        } else {
                            sEmailCreatedTime = oCurrentEmailItem._data$p$0.dateTimeCreated;
                            sEmailModifiedTime = oCurrentEmailItem._data$p$0.dateTimeModified;
                        }
                        if (typeof (sEmailCreatedTime) === "undefined" && typeof (sEmailModifiedTime) === "undefined") {
                            vm.showAttachment = true;
                            vm.enableAttachment = false;
                            // vm.gridOptions.columnDefs.splice(1, 7);
                        }
                    }
                }
            }
        }
        // };
        vm.errorAttachDocument = false;
        vm.sendDocumentAsAttachment = function () {
            if (vm.selectedRows && vm.selectedRows.length <= 5) {
                vm.enableAttachment = true;
                vm.errorAttachDocument = false;;
                vm.asyncCallCompleted = 1;

                vm.showFailedAtachments = false;
                vm.failedFiles = [];
                vm.showPopUpHolder = true;
                vm.attachedProgressPopUp = true;
                vm.attachInProgressMessage = configs.uploadMessages.attachInProgressMessage.replace("{0}", 1);
                angular.forEach(vm.selectedRows, function (selRow) {
                    var documentPath = trimEndChar(selRow.documentOWAUrl, "/");
                    var documentName = '';
                    if (documentPath) {
                        documentPath = trimEndChar(documentPath.trim(), "/");
                        documentName = documentPath.substring(documentPath.lastIndexOf("/") + 1);
                        if (documentPath && documentName) {
                            vm.showAttachmentProgress = true;

                            sendAttachmentAsync(decodeURIComponent(documentPath), decodeURIComponent(documentName));
                        }
                    }
                });
            } else {
                vm.errorAttachDocument = true;
                vm.enableAttachment = false;
            }
        }


        /* Send asynchronous calls to send each document as attachment */
        function sendAttachmentAsync(sDocumentPath, sDocumentName) {
            Office.context.mailbox.item.addFileAttachmentAsync(sDocumentPath, sDocumentName, {
                asyncContext: {
                    sCurrentDocumentPath: sDocumentPath,
                    sCurrentDocumentName: sDocumentName
                }
            },
            function (asyncResult) {

                if (asyncResult.status === Office.AsyncResultStatus.Failed) {
                    vm.failedFiles.push(asyncResult.asyncContext.sCurrentDocumentName)
                    vm.showFailedAtachments = true;
                }

                //  vm.asyncCallBeforeCompleted = vm.asyncCallCompleted;
                $scope.$apply();
                if (vm.asyncCallCompleted === vm.selectedRows.length) {
                    vm.showAttachmentProgress = false;
                    notifyAttachmentResult();
                    //vm.asyncCallCompleted = 0;
                } else {
                    vm.asyncCallCompleted = vm.asyncCallCompleted + 1;
                }
                $scope.$apply();

            });
        }

        function notifyAttachmentResult() {
            if (vm.showFailedAtachments) {
                vm.showSuccessAttachments = false;
                vm.showFailedAtachments = true;
                vm.failedHeaderMessage = configs.uploadMessages.attachFailureMessage;
            } else {
                vm.showFailedAtachments = false;
                vm.showSuccessAttachments = true;
                vm.failedHeaderMessage = '';
                vm.failedFiles = [];
            }
            vm.showPopUpHolder = true;
        }
        function trimEndChar(sOrignalString, sCharToTrim) {
            "use strict";
            if (sOrignalString && sCharToTrim === sOrignalString.substr(-1)) {
                return sOrignalString.substr(0, sOrignalString.length - 1);
            }
            return sOrignalString;
        }

        vm.closeNotification = function () {
            vm.showPopUpHolder = false;
            vm.showSuccessAttachments = false;
        }


        //#endregion


        //#region Api Calls
        //search api call 
        function get(options, callback) {
            api({
                resource: 'documentResource',
                method: 'get',
                data: options,
                success: callback
            });
        }


        function getPinnedDocuments(options, callback) {
            api({
                resource: 'documentResource',
                method: 'getPinnedDocuments',
                data: options,
                success: callback
            });
        }


        //Callback function for pin 
        function pinDocuments(options, callback) {
            api({
                resource: 'documentResource',
                method: 'pinDocument',
                data: options,
                success: callback
            });
        }


        //Callback function for unpin 
        function UnpinDocuments(options, callback) {
            api({
                resource: 'documentResource',
                method: 'unPinDocument',
                data: options,
                success: callback
            });
        }
        //#endregion


        //Callback function for document assets 
        function GetAssets(options, callback) {
            api({
                resource: 'documentResource',
                method: 'getassets',
                data: options,
                success: callback
            });
        }
        //#endregion


        //#region methods for getting,filtering,pin,unpin documents

        //SearchRequest Object
        var searchRequest = {
            Client: {
                Url: configs.global.repositoryUrl
            },
            SearchObject: {
                PageNumber: 1,
                ItemsPerPage: vm.searchResultsLength,
                SearchTerm: '',
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
                            ByProperty: 'MCModifiedDate',
                            Direction: 1
                        }
            }
        };

        vm.searchDocument = function (val) {
            var finalSearchText = "";
            if (val != "") {
                finalSearchText = "(FileName:" + val + "* OR dlcDocIdOWSText:" + val + "*)"
            }
            vm.pagenumber = 1;
            searchRequest.SearchObject.PageNumber = vm.pagenumber;
            searchRequest.SearchObject.SearchTerm = finalSearchText;
            searchRequest.SearchObject.Sort.ByProperty = "FileName";
            searchRequest.SearchObject.Sort.Direction = 0;
            return documentResource.get(searchRequest).$promise;
        }

        vm.search = function () {
            vm.pagenumber = 1;
            vm.documentname = 'All Documents'
            vm.documentid = 1;
            vm.lazyloader = false;
            vm.divuigrid = false;
            vm.responseNull = false;
            var searchToText = '';
            var finalSearchText = '';
            if (vm.selected != "") {
                if (-1 !== vm.selected.indexOf(":")) {
                    finalSearchText = commonFunctions.searchFilter(vm.selected);
                } else {
                    finalSearchText = '("' + vm.selected + '*" OR FileName:"' + vm.selected + '*" OR dlcDocIdOWSText:"' + vm.selected + '*" OR MCDocumentClientName:"' + vm.selected + '*")';
                }
            }
            searchRequest.SearchObject.PageNumber = vm.pagenumber;
            searchRequest.SearchObject.SearchTerm = finalSearchText;
            searchRequest.SearchObject.Sort.ByProperty = "FileName";
            searchRequest.SearchObject.Sort.Direction = 0;
            get(searchRequest, function (response) {
                if (response == "") {
                    vm.gridOptions.data = response;
                    vm.lazyloader = true;
                    vm.divuigrid = true;
                    vm.nodata = true;
                } else {
                    vm.divuigrid = true;
                    vm.nodata = false;
                    vm.lazyloader = true;
                    vm.gridOptions.data = response;
                    searchRequest.SearchObject.Sort.ByProperty = "MCModifiedDate";
                }
            });
        }

        //#region for searching matter by property and searchterm
        vm.documentsearch = function (term, property, bool) {
            vm.lazyloader = false;
            vm.responseNull = false;
            searchRequest.SearchObject.PageNumber = 1;
            searchRequest.SearchObject.SearchTerm = term;
            searchRequest.SearchObject.Sort.ByProperty = property;
            searchRequest.SearchObject.Sort.Direction = 1;
            if (bool) {
                vm.documentheader = true;
                vm.divuigrid = false;
                searchRequest.SearchObject.SearchTerm = "";
                searchRequest.SearchObject.Sort.Direction = 1;
                if (property == "FileName") {
                    searchRequest.SearchObject.Sort.ByProperty = "MCModifiedDate";
                    searchRequest.SearchObject.Filters.Name = term;
                    vm.documentfilter = true;
                }
                else if (property == "MCDocumentClientName") {
                    searchRequest.SearchObject.Sort.ByProperty = "MCModifiedDate";
                    searchRequest.SearchObject.Filters.ClientName = term;
                    vm.clientfilter = true;
                }
                else if (property == "MSITOfficeAuthor") {
                    searchRequest.SearchObject.Filters.DocumentAuthor = term;
                    vm.authorfilter = true;
                } else if (property == "MCCheckoutUser") {
                    searchRequest.SearchObject.Filters.DocumentCheckoutUsers = term;
                    vm.checkoutfilter = true;
                }
                else {
                    searchRequest.SearchObject.Filters.DocumentAuthor = "";
                    searchRequest.SearchObject.Filters.DocumentCheckoutUsers = "";
                }
            }
            get(searchRequest, function (response) {
                if (response == "") {
                    if (bool) {
                        vm.gridOptions.data = response;
                        vm.nodata = true;
                    } else {
                        vm.details = response;
                        vm.nodata = false;
                        vm.filternodata = true;
                    }
                    vm.lazyloader = true;
                    vm.divuigrid = true;
                } else {
                    vm.divuigrid = true;
                    vm.nodata = false;
                    vm.lazyloader = true;
                    if (bool) {
                        vm.gridOptions.data = response;
                        vm.details = [];
                        if (!$scope.$$phase) {
                            $scope.$apply();
                        }
                    } else {
                        vm.details = response;
                        vm.filternodata = false;
                    }
                    searchRequest.SearchObject.SearchTerm = "";
                    searchRequest.SearchObject.Sort.ByProperty = "";
                }
            });
        }
        //#endregion

        //Code for filtering ModifiedDate
        //start
        vm.FilterModifiedDate = function (name) {
            vm.lazyloader = false;
            vm.divuigrid = false;
            searchRequest.SearchObject.PageNumber = 1;
            searchRequest.SearchObject.SearchTerm = "";
            if (name == "Modified Date") {
                searchRequest.SearchObject.Filters.DateFilters.ModifiedFromDate = vm.modstartdate.format("yyyy-MM-dd");
                searchRequest.SearchObject.Filters.DateFilters.ModifiedToDate = vm.modenddate.format("yyyy-MM-dd");
                vm.moddatefilter = true;
            }
            if (name == "Created Date") {
                searchRequest.SearchObject.Filters.DateFilters.CreatedFromDate = vm.startdate.format("yyyy-MM-dd");
                searchRequest.SearchObject.Filters.DateFilters.CreatedToDate = vm.enddate.format("yyyy-MM-dd");
                vm.createddatefilter = true;
            }
            searchRequest.SearchObject.Sort.ByProperty = "MCModifiedDate";
            searchRequest.SearchObject.Sort.Direction = 1;
            get(searchRequest, function (response) {
                if (response == "") {
                    vm.gridOptions.data = response;
                    vm.lazyloader = true;
                    vm.divuigrid = true;
                    vm.nodata = true;
                } else {
                    vm.divuigrid = true;
                    vm.nodata = false;
                    vm.lazyloader = true;
                    vm.gridOptions.data = response;
                }
            });

        }

        //#endregion

        //#region clearing all filters
        vm.clearFilters = function (property) {
            vm.documentheader = true;
            vm.documentdateheader = true;
            vm.lazyloader = false;
            if (property == "Document") {
                vm.searchTerm = "";
                searchRequest.SearchObject.SearchTerm = "";
                searchRequest.SearchObject.Filters.Name = "";
                searchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                vm.documentfilter = false;
            }
            else if (property == "Client") {
                vm.clientSearchTerm = "";
                searchRequest.SearchObject.Filters.ClientName = "";
                searchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                vm.clientfilter = false;
            }
            else if (property == "Checked out to") {
                vm.checkedSearchTerm = "";
                searchRequest.SearchObject.Filters.DocumentCheckoutUsers = "";
                vm.checkoutfilter = false;
            }
            else if (property == "Author") {
                vm.authorSearchTerm = ""
                searchRequest.SearchObject.Filters.DocumentAuthor = "";
                vm.authorfilter = false;
            }
            else if (property == "Modified Date") {
                searchRequest.SearchObject.Filters.DateFilters.ModifiedFromDate = "";
                searchRequest.SearchObject.Filters.DateFilters.ModifiedToDate = "";
                vm.modstartdate = "";
                vm.modenddate = "";
                vm.moddatefilter = false;
            } else {
                searchRequest.SearchObject.Filters.DateFilters.CreatedFromDate = "";
                searchRequest.SearchObject.Filters.DateFilters.CreatedToDate = "";
                vm.startDate = "";
                vm.endDate = "";
                vm.createddatefilter = false;
            }

            get(searchRequest, function (response) {
                if (response == "") {
                    vm.gridOptions.data = response;
                    vm.lazyloader = true;
                    vm.divuigrid = true;
                    vm.nodata = true;
                } else {
                    vm.divuigrid = true;
                    vm.nodata = false;
                    vm.lazyloader = true;
                    vm.gridOptions.data = response;
                }
            });
        }

        //#endregion


        //Code written for displaying types in dropdown 
        //Start 
        vm.Documents = [{ Id: 1, Name: "All documents" }, { Id: 2, Name: "My documents" }, { Id: 3, Name: "Pinned documents" }];
        vm.ddlDocuments = vm.Documents[0];
        //End  

        var pinnedDocumentsRequest = {
            Url: configs.global.repositoryUrl
        }

        //#region for setting the document name in dropdown
        vm.SetDocuments = function (id, name) {
            vm.documentname = name;
            vm.documentid = id;
            vm.GetDocuments(id);
        }

        //#endregion


        //#region changing the grid based on the dropdown change 
        //Hits when the Dropdown changes 
        //Start 
        vm.GetDocuments = function (id) {
            vm.selected = "";
            vm.searchTerm = "";
            vm.searchClientTerm = "";
            vm.startdate = "";
            vm.enddate = "";
            vm.sortexp = "";
            vm.sortby = "";
            vm.lazyloader = false;
            vm.divuigrid = false;
            if (id == 1) {
                vm.responseNull = false;
                searchRequest.SearchObject.PageNumber = 1;
                searchRequest.SearchObject.SearchTerm = "";
                searchRequest.SearchObject.Filters.FilterByMe = 0;
                get(searchRequest, function (response) {
                    if (response == "") {
                        vm.gridOptions.data = response;
                        vm.lazyloader = true;
                        vm.divuigrid = true;
                        vm.nodata = true;

                    } else {
                        if (vm.isOutlook) {
                            vm.isOutlookAsAttachment(vm.isOutlook);
                        }
                        vm.divuigrid = true;
                        vm.nodata = false;
                        vm.responseNull = false;
                        vm.pagenumber = 1;
                        searchRequest.SearchObject.PageNumber = 1;
                        searchRequest.SearchObject.SearchTerm = "";
                        searchRequest.SearchObject.Filters.FilterByMe = 0;
                        getPinnedDocuments(pinnedDocumentsRequest, function (pinresponse) {
                            if (pinresponse.length > 0) {
                                angular.forEach(pinresponse, function (pinobj) {
                                    angular.forEach(response, function (res) {
                                        if (pinobj.documentID == res.documentID) {
                                            if (res.ismatterdone == undefined && !res.ismatterdone) {
                                                res.MatterInfo = "Unpin this matter";
                                                res.ismatterdone = true;
                                            }
                                        }
                                    });
                                });
                                vm.gridOptions.data = response;
                                if (!$scope.$$phase) {
                                    $scope.$apply();
                                }
                            } else {
                                vm.gridOptions.data = response;
                                if (!$scope.$$phase) {
                                    $scope.$apply();
                                }
                            }
                        });
                        $timeout(function () { vm.lazyloader = true; }, 1000);
                    }
                });

            } else if (id == 2) {
                vm.lazyloader = false;
                vm.divuigrid = false;
                vm.responseNull = false;
                vm.pagenumber = 1;
                searchRequest.SearchObject.PageNumber = 1;
                searchRequest.SearchObject.SearchTerm = "";
                searchRequest.SearchObject.Filters.FilterByMe = 1;
                get(searchRequest, function (response) {
                    if (response == "") {
                        vm.gridOptions.data = response;
                        vm.lazyloader = true;
                        vm.divuigrid = true;
                        vm.nodata = true;
                    } else {
                        if (vm.isOutlook) {
                            vm.isOutlookAsAttachment(vm.isOutlook);
                        }
                        vm.divuigrid = true;
                        vm.nodata = false;
                        getPinnedDocuments(pinnedDocumentsRequest, function (pinresponse) {
                            if (pinresponse.length > 0) {
                                angular.forEach(pinresponse, function (pinobj) {
                                    angular.forEach(response, function (res) {
                                        if (pinobj.documentID == res.documentID) {
                                            if (res.ismatterdone == undefined && !res.ismatterdone) {
                                                res.MatterInfo = "Unpin this matter";
                                                res.ismatterdone = true;
                                            }
                                        }
                                    });
                                });
                                vm.gridOptions.data = response;
                                if (!$scope.$$phase) {
                                    $scope.$apply();
                                }
                            } else {
                                vm.gridOptions.data = response;
                                if (!$scope.$$phase) {
                                    $scope.$apply();
                                }
                            }
                            $timeout(function () { vm.lazyloader = true; }, 1000);
                        });
                    }
                });
            } else if (id == 3) {
                vm.lazyloader = false;
                vm.divuigrid = false;
                var pinnedMattersRequest = {
                    Url: configs.global.repositoryUrl
                }
                getPinnedDocuments(pinnedMattersRequest, function (response) {
                    if (response == "") {
                        vm.gridOptions.data = response;
                        vm.lazyloader = true;
                        vm.divuigrid = true;
                        vm.nodata = true;
                    } else {
                        if (vm.isOutlook) {
                            vm.isOutlookAsAttachment(vm.isOutlook);
                        }
                        vm.divuigrid = true;
                        vm.nodata = false;
                        angular.forEach(response, function (res) {
                            if (res.ismatterdone == undefined && !res.ismatterdone) {
                                res.MatterInfo = "Unpin this matter";
                                res.ismatterdone = true;
                            }
                        });
                        vm.gridOptions.data = response;
                        vm.lazyloader = true;
                    }
                });
            }
        }
        //End 


        //To run GetDocuments function on page load 
        vm.SetDocuments(vm.documentid, vm.documentname);
        //End 


        //#region For pin and unpin the matter
        //Written for unpinning the matter 
        //Start 
        vm.UnpinDocument = function (data) {
            var alldata = data.entity;
            var unpinRequest = {
                Client: {
                    Url: configs.global.repositoryUrl
                },
                DocumentData: {
                    DocumentUrl: alldata.documentUrl
                }
            }
            UnpinDocuments(unpinRequest, function (response) {
                if (response.isDocumentUnPinned) {
                    $timeout(function () { vm.SetDocuments(vm.documentid, vm.documentname); }, 500);
                }
            });
        }
        //End 


        //Written for pinning the matter 
        //Start 
        vm.PinMatter = function (data) {
            var alldata = data.entity;
            var pinRequest = {
                Client: {
                    Url: configs.global.repositoryUrl
                },
                documentData: {
                    DocumentName: alldata.documentName,
                    DocumentVersion: alldata.documentVersion,
                    DocumentClient: alldata.documentClient,
                    DocumentClientId: alldata.documentClientId,
                    DocumentClientUrl: alldata.documentClientUrl,
                    DocumentMatter: alldata.documentMatter,
                    DocumentMatterId: alldata.documentMatterId,
                    DocumentOwner: alldata.documentOwner,
                    DocumentUrl: alldata.documentUrl,
                    DocumentOWAUrl: alldata.documentOWAUrl,
                    DocumentExtension: alldata.documentExtension,
                    DocumentCreatedDate: alldata.documentCreatedDate,
                    DocumentModifiedDate: alldata.documentModifiedDate,
                    DocumentCheckoutUser: alldata.documentCheckoutUser,
                    DocumentMatterUrl: alldata.documentMatterUrl,
                    DocumentParentUrl: alldata.documentParentUrl,
                    DocumentID: alldata.documentID,
                    DocumentIconUrl: alldata.documentIconUrl,
                    PinType: "unpin"
                }
            }
            pinDocuments(pinRequest, function (response) {
                if (response.isDocumentPinned) {
                    $timeout(function () { vm.SetDocuments(vm.documentid, vm.documentname); }, 500);
                }
            });
        }
        //#endregion 


        vm.menuClick = function () {
            var oAppMenuFlyout = $(".AppMenuFlyout");
            if (!(oAppMenuFlyout.is(":visible"))) {
                //// Display the close icon and close the fly out 
                $(".OpenSwitcher").addClass("hide");
                $(".CloseSwitcher").removeClass("hide");
                $(".MenuCaption").addClass("hideMenuCaption");
                oAppMenuFlyout.slideDown();
            } else {
                oAppMenuFlyout.slideUp();
                $(".CloseSwitcher").addClass("hide");
                $(".OpenSwitcher").removeClass("hide");
                $(".MenuCaption").removeClass("hideMenuCaption");
            }
        }

        //#region  For datepickers in modifiedheadertemplate
        //Angular Datepicker Starts here
        //Start for modified date 
        vm.moddateOptions = {
            formatYear: 'yy',
            maxDate: new Date()
        };


        vm.modenddateOptions = {
            formatYear: 'yy',
            maxDate: new Date()
        }

        $scope.$watch('vm.modstartdate', function (newval, oldval) {
            vm.modenddateOptions.minDate = newval;
        });


        vm.modStartDate = function ($event) {
            if ($event) {
                $event.preventDefault();
                $event.stopPropagation();
            }
            this.modifiedStartDate = true;
        };
        vm.modEndDate = function ($event) {
            if ($event) {
                $event.preventDefault();
                $event.stopPropagation();
            }
            this.modifiedenddate = true;
        };

        vm.modifiedStartDate = false;
        vm.modifiedenddate = false;

        vm.disabled = function (date, mode) {
            return (mode === 'day' && (date.getDay() != 0));
        };

        //End


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

        vm.disabled = function (date, mode) {
            return (mode === 'day' && (date.getDay() != 0));
        };



        //#endregion

        //#region Custom Sorting functionality
        //Start

        vm.FilterByType = function () {
            get(searchRequest, function (response) {
                vm.lazyloader = true;
                if (response.errorCode == "404") {
                    vm.divuigrid = false;
                    vm.nodata = true;

                } else {
                    vm.divuigrid = true;
                    vm.nodata = false;
                    vm.gridOptions.data = response;
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                }
            });
        }

        vm.sortby = "";
        vm.sortexp = "";
        vm.showSortExp = function () {
            if (vm.sortexp != "" || vm.sortexp != undefined || vm.sortby != "" || vm.sortby != undefined) {
                if (vm.sortby == "asc") {
                    angular.element("#desc" + vm.sortexp).css("display", "none");
                } else {
                    angular.element("#asc" + vm.sortexp).css("display", "none");
                }
                angular.element("#" + vm.sortby + vm.sortexp).css("display", "block");
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            }
        }

        vm.sortChangedDocument = function (grid, sortColumns) {
            vm.divuigrid = false;
            vm.responseNull = false;
            if (sortColumns.length != 0) {
                if (sortColumns[0].name == vm.gridOptions.columnDefs[1].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (vm.FileNameSort == undefined || vm.FileNameSort == "asc") {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "FileName";
                            searchRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            vm.FileNameSort = "desc";
                            vm.sortby = "asc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        } else {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "FileName";
                            searchRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            vm.FileNameSort = "asc";
                            vm.sortby = "desc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        }
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
                else if (sortColumns[0].name == vm.gridOptions.columnDefs[2].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (vm.DocumentClientSort == undefined || vm.DocumentClientSort == "asc") {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "MCDocumentClientName";
                            searchRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            vm.DocumentClientSort = "desc";
                            vm.sortby = "asc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        }
                        else {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "MCDocumentClientName";
                            searchRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            vm.DocumentClientSort = "asc";
                            vm.sortby = "desc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        }
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
                else if (sortColumns[0].name == vm.gridOptions.columnDefs[3].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (vm.DocumentClientIDSort == undefined || vm.DocumentClientIDSort == "asc") {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "MCDocumentClientID";
                            searchRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            vm.DocumentClientIDSort = "desc";
                            vm.sortby = "asc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        } else {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "MCDocumentClientID";
                            searchRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            vm.DocumentClientIDSort = "asc";
                            vm.sortby = "desc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        }

                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
                else if (sortColumns[0].name == vm.gridOptions.columnDefs[4].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (vm.ModiFiedDateSort == undefined || vm.ModiFiedDateSort == "asc") {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "MCModifiedDate";
                            searchRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            vm.ModiFiedDateSort = "desc";
                            vm.sortby = "asc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        } else {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "MCModifiedDate";
                            searchRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            vm.ModiFiedDateSort = "asc";
                            vm.sortby = "desc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        }

                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
                else if (sortColumns[0].name == vm.gridOptions.columnDefs[5].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (vm.AuthorSort == undefined || vm.AuthorSort == "asc") {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "MSITOfficeAuthor";
                            searchRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            vm.AuthorSort = "desc";
                            vm.sortby = "asc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        } else {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "MSITOfficeAuthor";
                            searchRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            vm.AuthorSort = "asc";
                            vm.sortby = "desc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        }
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
                else if (sortColumns[0].name == vm.gridOptions.columnDefs[6].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (vm.VersionSort == undefined || vm.VersionSort == "asc") {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "MCVersionNumber";
                            searchRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            vm.VersionSort = "desc";
                            vm.sortby = "asc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        } else {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "MCVersionNumber";
                            searchRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            vm.VersionSort = "asc";
                            vm.sortby = "desc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        }
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
                else if (sortColumns[0].name == vm.gridOptions.columnDefs[7].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (vm.CheckoutSort == undefined || vm.CheckoutSort == "asc") {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "MCCheckoutUser";
                            searchRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            vm.CheckoutSort = "desc";
                            vm.sortby = "asc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        } else {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "MCCheckoutUser";
                            searchRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            vm.CheckoutSort = "asc";
                            vm.sortby = "desc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        }
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
                else if (sortColumns[0].name == vm.gridOptions.columnDefs[8].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (vm.CreatedSort == undefined || vm.CreatedSort == "asc") {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "Created";
                            searchRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            vm.CreatedSort = "desc";
                            vm.sortby = "asc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        } else {
                            vm.lazyloader = false;
                            vm.pagenumber = 1;
                            searchRequest.SearchObject.PageNumber = 1;
                            searchRequest.SearchObject.Sort.ByProperty = "Created";
                            searchRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            vm.CreatedSort = "asc";
                            vm.sortby = "desc";
                            vm.sortexp = sortColumns[0].field;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        }
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
            } else {
                vm.lazyloader = false;
                searchRequest.SearchObject.Sort.ByProperty = "FileName";
                searchRequest.SearchObject.Sort.Direction = 0;
                vm.FilterByType();
            }
        }
        //#endregion


        //#region setting the grid options when window is resized

        angular.element($window).bind('resize', function () {
            angular.element('#documentgrid .ui-grid').css('height', $window.innerHeight - 110);
            if ($window.innerWidth < 380) {
                angular.element('#documentgrid .ui-grid-viewport').addClass('viewport');
                angular.element('#documentgrid .ui-grid-viewport').removeClass('viewportlg');
            } else {
                angular.element('#documentgrid .ui-grid-viewport').removeClass('viewport');
                angular.element('#documentgrid .ui-grid-viewport').addClass('viewportlg');
            }
        });

        //#endregion

        //#region
        vm.typeheadselect = function (index, selected) {
            vm.documentname = 'All Documents'
            vm.documentid = 1;
            var searchToText = '';
            var finalSearchText = "";
            if (selected != "") {
                searchToText = selected.replace("(", ",")
                searchToText = searchToText.replace(")", "")
                var firstText = searchToText.split(',')[0]
                var secondText = searchToText.split(',')[1]
                var finalSearchText = '(FileName:"' + firstText.trim() + '" OR dlcDocIdOWSText:"' + firstText.trim() + '"OR MCDocumentClientName:"' + firstText.trim() + '")';
            }
            searchRequest.SearchObject.SearchTerm = finalSearchText;
            searchRequest.SearchObject.Sort.Direction = 0;
            vm.FilterByType();
        }

        //#endregion

        //function to check all checkboxes inside grid
        vm.toggleCheckerAll = function (checked) {
            for (var i = 0; i < vm.gridOptions.data.length; i++) {
                vm.gridOptions.data[i].checker = checked;
                if (checked) {
                    //    vm.cartelements.push(vm.documentGridOptions.data[i]);
                    vm.documentsCheckedCount = vm.gridOptions.data.length;
                    vm.selectedRows = vm.gridOptions.data;

                }
                else {
                    vm.documentsCheckedCount = 0;
                }
                if (checked) {
                    $scope.gridApi.selection.selectAllRows();
                }
                else {
                    $scope.gridApi.selection.clearSelectedRows();
                    vm.selectedRows = [];
                    vm.showErrorAttachmentInfo = false;

                }
            }
            isOpenedInOutlook();
            $scope.$apply();

        };

        //vm.toggleChecker = function (checked, rowinfo) {
        //    console.log(checked);
        //    console.log(rowinfo);
        //    $scope.gridApi.selection.selectRow(rowinfo);
        //}

        //vm.toggleChecker = function (checked, rowinfo) {
        //    if (checked) {
        //        if (vm.documentsCheckedCount >= 0) {
        //            vm.documentsCheckedCount = parseInt(vm.documentsCheckedCount, 10) + 1;
        //        }
        //       // vm.cartelements.push(rowinfo);
        //    }
        //    else {
        //        vm.documentsCheckedCount = parseInt(vm.documentsCheckedCount, 10) - 1
        //        var rows = vm.gridApi.core.getVisibleRows(vm.gridApi.grid),
        //            allChecked = true;
        //        for (var r = 0; r < rows.length; r++) {
        //            if (rows[r].entity.checker !== true) {
        //                allChecked = false;
        //                break;
        //            }
        //        }
        //        $("#chkAllDocCheckBox").prop('checked', allChecked);
        //    }
        //};


        vm.assetsuccess = false;
        vm.getDocumentAssets = function (row) {
            vm.assetsuccess = false;
            var Client = {
                Id: row.entity.documentUrl.replace(configs.uri.SPOsiteURL, ""),
                Name: row.entity.documentMatterUrl.replace(configs.uri.SPOsiteURL, ""),
                Url: row.entity.documentClientUrl
            }
            GetAssets(Client, function (response) {
                vm.listguid = response.listInternalName;
                vm.docguid = response.documentGuid;
                vm.assetsuccess = true;
            });
        }

        vm.gotoDocumentUrl = function (url) {
            if (vm.assetsuccess) {
                $window.open(configs.global.repositoryUrl + "/SitePages/documentDetails.aspx?client=" + url.replace(configs.uri.SPOsiteURL, "") + "&listguid=" + vm.listguid + "&docguid=" + vm.docguid, "_blank");
            } else {
                $timeout(function () { $window.open(configs.global.repositoryUrl + "/SitePages/documentDetails.aspx?client=" + url.replace(configs.uri.SPOsiteURL, "") + "&listguid=" + vm.listguid + "&docguid=" + vm.docguid, "_blank"); }, 1500);
            }
        }

        //#region For displaying and setting the position of the filters name wise
        vm.documentheader = true;
        vm.documentdateheader = true;
        vm.searchexp = "";
        vm.filtername = "";

        vm.openDocumentHeader = function ($event, name) {
            vm.filternodata = false;
            vm.details = [];
            var dimensions = $event.target.getBoundingClientRect();
            var top = dimensions.top + 30;
            var left = dimensions.left - 224;
            angular.element('.documentheader').css({ 'top': top, 'left': left });
            angular.element('.documentheaderdates').css({ 'top': top, 'left': left });
            if (name == "Document") {
                vm.searchexp = "FileName";
                vm.filtername = "Document";
            }
            if (name == "client") {
                vm.searchexp = "MCDocumentClientName";
                vm.filtername = "Client";
            }
            if (name == "Author") {
                vm.searchexp = "MSITOfficeAuthor";
                vm.filtername = "Author";
            }
            if (name == "checkout") {
                vm.searchexp = "MCCheckoutUser";
                vm.filtername = "Checked out to";
            }
            if (name == "ModifiedDate") {
                vm.filtername = "Modified Date";
            }
            if (name == "CreatedDate") {
                vm.filtername = "Created Date";
            }
            $timeout(function () {
                if (name == 'ModifiedDate' || name == 'CreatedDate') {
                    vm.documentdateheader = false;
                }
                else {
                    vm.documentheader = false;
                }
            },
            600);
            if (!$scope.$$phase) {
                $scope.$apply();
            }
        }
        //#endregion

    }]);

    app.filter('unique', function () {
        return function (collection, keyname) {
            var output = [],
                keys = [];

            angular.forEach(collection, function (item) {
                var key = item[keyname];
                if (keys.indexOf(key) === -1) {
                    keys.push(key);
                    output.push(item);
                }
            });
            return output;
        };
    });

})();

