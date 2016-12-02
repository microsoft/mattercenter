/// <reference path="documentheadertemplate.html" />
(function () {
    'use strict';

    var app = angular.module("matterMain");

    app.controller('documentsController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout',
        'documentResource', '$rootScope', 'uiGridConstants', '$location', '$http', '$templateCache', '$window', '$q', '$filter', 'commonFunctions', '$animate',
    function ($scope, $state, $interval, $stateParams, api, $timeout,
        documentResource, $rootScope, uiGridConstants, $location, $http, $templateCache, $window, $q, $filter, commonFunctions, $animate) {
        var vm = this;
        vm.selected = undefined;
        //#region dynamic content
        vm.navigationContent = uiconfigs.Navigation;
        vm.header = uiconfigs.Header;
        vm.documentConfigContent = uiconfigs.Documents;
        vm.uploadMessages = uiconfigs.uploadMessages;
        vm.configSearchContent = configs.search;
        vm.globalSettings = configs.global;
        //#end region
        vm.documentname = 'My Documents'
        vm.documentid = 2;
        vm.documentsdrop = false;
        vm.docdropinner = true;
        $rootScope.pageIndex = "2";
        //To load the Contextual help data
        $rootScope.help();
        $rootScope.bodyclass = "bodymain";
        $rootScope.displayOverflow = "";
        $rootScope.profileClass = "";
        //To get all results on filtered column for filter.
        vm.previousDocFileNameValue = '';
        vm.previousDocClientNameValue = '';
        vm.previousDocMatterNameValue = '';
        vm.previousDocPracticeGroupValue = '';
        vm.previousDocAuthorValue = '';
        vm.previousDocCheckOutUserValue = '';
        vm.assetsuccess = false;
        // Onload show ui grid and hide error div
        //start
        vm.divuigrid = false;
        vm.nodata = false;
        var screenHeight = 0;
        vm.searchResultsLength = 0;
        vm.lazyloaderFilter = true;
        vm.sortby = "desc";
        vm.sortexp = "documentName"
        //end
        //#region for checking whether the app is opened in outlook
        var isAppOpenedInOutlook = $location.absUrl();
        if (isAppOpenedInOutlook.indexOf("Outlook") > -1) {

            vm.isOutlook = true;
            //  vm.isOutlookAsAttachment(vm.isOutlook);
            // }
        }
        //#endregion

        //#region scopes for displaying and hiding filter icons
        //start
        vm.documentfilter = false;
        vm.moddatefilter = false;
        vm.createddatefilter = false;
        vm.clientfilter = false;
        vm.authorfilter = false;
        vm.projectNamefilter = false;
        vm.practiceGroupfilter = false;
        vm.checkoutfilter = false;
        vm.pinnedorunpinned = false;
        //end

        //#region for showing the matters dropdown in resposive 
        vm.showdocdrop = function ($event) {
            $event.stopPropagation();
            $rootScope.displayinfo = false;
            $rootScope.dispinner = true;
            $rootScope.contextualhelp = false;
            $rootScope.dispcontextualhelpinner = true;
            if (vm.docdropinner) {
                vm.documentsdrop = true;
                vm.docdropinner = false;
            } else {
                vm.documentsdrop = false;
                vm.docdropinner = true;
            }
        }
        //#endregion

        //#region For clearing all the sort filters 
        vm.clearAllFiltersofSort = function () {
            angular.element('[id^="asc"]').hide();
            angular.element('[id^="desc"]').hide();
            vm.FileNameSort = undefined;
            vm.DocumentClientSort = undefined;
            vm.DocumentClientIDSort = undefined;
            vm.ModiFiedDateSort = undefined;
            vm.AuthorSort = undefined;
            vm.DocumentPracticeGroupSort = undefined;
            vm.VersionSort = undefined;
            vm.CheckoutSort = undefined;
            vm.CreatedSort = undefined;
        }

        //#endregion

        //#region for closing all the dropdowns
        vm.closealldrops = function () {
            vm.documentsdrop = false;
            vm.docdropinner = true;
            vm.documentheader = true;
            vm.documentdateheader = true;
            angular.element('.ui-grid-icon-menu').addClass('showExpandIcon');
            angular.element('.ui-grid-icon-menu').removeClass('closeColumnPicker');
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
        vm.attachButtonText = vm.uploadMessages.attachButtonText;
        vm.showPopUpHolder = false;
        vm.showErrorAttachmentInfo = false;
        vm.showFailedAtachments = false;
        vm.showFailedAtachmentsBlock = false;
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
        vm.practiceGroupDropDown = false;
        vm.projectNameDropDown = false;
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
                    height: ($window.innerHeight - 100) + "px"
                }
            }
        };

        vm.switchFuction = function (columnName) {
            var displayColumn = [];
            switch (columnName) {
                case "GridColumn1Header":
                    displayColumn[0] = vm.documentConfigContent.GridColumn1Header;
                    displayColumn[1] = vm.documentConfigContent.GridColumn1HeaderTitle;
                    break;
                case "GridColumn2Header":
                    displayColumn[0] = vm.documentConfigContent.GridColumn2Header;
                    displayColumn[1] = vm.documentConfigContent.GridColumn2HeaderTitle;
                    break;
                case "GridColumn3Header":
                    displayColumn[0] = vm.documentConfigContent.GridColumn3Header;
                    displayColumn[1] = vm.documentConfigContent.GridColumn3HeaderTitle;
                    break;
                case "GridColumn4Header":
                    displayColumn[0] = vm.documentConfigContent.GridColumn4Header;
                    displayColumn[1] = vm.documentConfigContent.GridColumn4HeaderTitle;
                    break;
                case "GridColumn5Header":
                    displayColumn[0] = vm.documentConfigContent.GridColumn5Header;
                    displayColumn[1] = vm.documentConfigContent.GridColumn5HeaderTitle;
                    break;
                case "GridColumn6Header":
                    displayColumn[0] = vm.documentConfigContent.GridColumn6Header;
                    displayColumn[1] = vm.documentConfigContent.GridColumn6HeaderTitle;
                    break;
                case "GridColumn7Header":
                    displayColumn[0] = vm.documentConfigContent.GridColumn7Header;
                    displayColumn[1] = vm.documentConfigContent.GridColumn7HeaderTitle;
                    break;
                case "GridColumn8Header":
                    displayColumn[0] = vm.documentConfigContent.GridColumn8Header;
                    displayColumn[1] = vm.documentConfigContent.GridColumn8HeaderTitle;
                    break;
                default:
                    displayColumn = '';
                    displayColumn[1] = '';
                    break;
            }
            return displayColumn;
        };

        $templateCache.put('coldefheadertemplate.html', "<div><div role='button' class='ui-grid-cell-contents ui-grid-header-cell-primary-focus' col-index='renderIndex'><span class='ui-grid-header-cell-label ng-binding' title='Click to sort by'>{{ col.colDef.displayName }}<span id='asc{{col.colDef.field}}' style='float:right;display:none' class='padl10px'>↑</span><span id='desc{{col.colDef.field}}' style='float:right;display:none' class='padlf10'>↓</span></span></div></div>");
        var columnDefs1 = [];
        columnDefs1.push({
            field: 'checker',
            displayName: 'checked',
            enableHiding: false,
            width: '20',
            cellTemplate: '/app/document/DocumentTemplates/cellCheckboxTemplate.html',
            headerCellTemplate: '/app/document/DocumentTemplates/headerCheckboxTemplate.html',
            position: 0
        });
        angular.forEach(configs.search.searchColumnsUIPickerForDocument, function (value, key) {

            if (value.displayInUI == true && value.position != -1) {
                var displaycolVal = vm.switchFuction(value.displayName);
                columnDefs1.push({
                    field: key,
                    displayName: displaycolVal[0],
                    width: value.width,
                    enableHiding: value.enableHiding,
                    cellTemplate: value.cellTemplate,
                    headerCellTemplate: value.headerCellTemplate == "Custom" ? $templateCache.get('coldefheadertemplate.html').replace('Click to sort by', displaycolVal[1]) : value.headerCellTemplate,
                    position: value.position,
                    cellClass: value.cellClass,
                    headerCellClass: value.headerCellClass,
                    visible: value.defaultVisibleInGrid,
                    suppressRemoveSort: true
                });
            }
        });

        function getSortFunction(fieldName) {
            return function (col1, col2) {
                return parseInt(col1[fieldName]) - parseInt(col2[fieldName]);
            }
        }
        columnDefs1.sort(getSortFunction("position"));

        vm.gridOptions = {
            infiniteScrollDown: true,
            //infiniteScrollPercentage: 50,
            infiniteScrollRowsFromEnd: 10,
            enableHorizontalScrollbar: 0,
            enableVerticalScrollbar: 1,
            enableGridMenu: true,
            enableRowHeaderSelection: false,
            enableRowSelection: true,
            enableSelectAll: true,
            multiSelect: true,
            columnDefs: columnDefs1,
            enableColumnMenus: false,
            onRegisterApi: function (gridApi) {
                $scope.gridApi = gridApi;
                gridApi.core.on.columnVisibilityChanged($scope, function (changedColumn) {
                    $scope.columnChanged = { name: changedColumn.colDef.name, visible: changedColumn.colDef.visible };
                });
                gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                    //vm.selectedRow = row.entity
                    vm.selectedRows = $scope.gridApi.selection.getSelectedRows();
                    var isRowPresent = $filter("filter")(vm.selectedRows, row.entity.docGuid);
                    if (isRowPresent.length > 0) {
                        row.entity.checker = true;
                    }
                    else {
                        vm.checker = false;
                        row.entity.checker = false;
                    }
                    isOpenedInOutlook();
                });
                $animate.enabled(gridApi.grid.element, false);
                $scope.gridApi.core.on.sortChanged($scope, $scope.sortChangedDocument);
                $scope.sortChangedDocument($scope.gridApi.grid, [vm.gridOptions.columnDefs[1]]);
                $scope.$watch('gridApi.grid.isScrollingVertically', vm.watchFuncScroll);
                $scope.gridApi.infiniteScroll.on.needLoadMoreData($scope, vm.watchFunc);
                //vm.setColumns();
            }
        };

        vm.watchFuncScroll = function () {

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
                        vm.warningMessageText = vm.uploadMessages.maxAttachedMessage;
                        vm.showErrorAttachmentInfo = true;
                    }
                }
            }
        }

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
                vm.searchResultsLength = 42;
            } else if (screenHeight <= 1080 && screenHeight >= 1025) {
                vm.searchResultsLength = 50;
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
                return promise.promise;
            } else {
                vm.lazyloader = true;
                // $scope.gridApi.infiniteScroll.dataLoaded();
            }
        }
        //#endregion


        //#region Code for attaching documents in compose more
        Office.initialize = function (reason) {
        }
        //vm.isOutlook = true;
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
                vm.showFailedAtachmentsBlock = false;
                vm.failedFiles = [];
                vm.showPopUpHolder = true;
                vm.attachedProgressPopUp = true;               
                angular.forEach(vm.selectedRows, function (selRow) {
                    var docUrl = selRow.documentOWAUrl;
                    if (selRow.documentOWAUrl.indexOf("WopiFrame.aspx") > 0) {
                        docUrl = selRow.documentParentUrl + '/' + selRow.documentName + '.' + selRow.documentExtension
                    }
                    var documentPath = trimEndChar(docUrl, "/");
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
                vm.showFailedAtachmentsBlock = true;
                vm.failedHeaderMessage = vm.uploadMessages.attachFailureMessage;
            } else {
                vm.showFailedAtachmentsBlock = false;
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
                IsUnique: false,
                UniqueColumnName: '',
                FilterValue: '',
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
                    ProjectName: "",
                    SubareaOfLaw: "",
                    ProjectID: "",
                    PracticeGroup: "",
                    ToDate: ""
                },
                Sort:
                        {
                            ByProperty: '' + vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime + '',
                            Direction: 1
                        }
            }
        };

        vm.searchDocument = function (val) {
            var finalSearchText = "";
            if (val != "") {
                //finalSearchText = "(" + vm.configSearchContent.ManagedPropertyFileName + ":" + val + "* OR " + vm.configSearchContent.ManagedPropertyDocumentId + ":" + val + "*)"
                if (val.indexOf("(") == 0 && val.indexOf(")") == val.length - 1) {
                    finalSearchText = "(" + vm.configSearchContent.ManagedPropertyFileName + ":\"" + val + "*\" OR " + vm.configSearchContent.ManagedPropertyDocumentId + ":\"" + val + "*\")";
                }
                else if (val.lastIndexOf("(") > 0 && val.lastIndexOf(")") == val.length - 1) {
                    var documentName = val.substring(0, val.lastIndexOf("(") - 1);
                    var documentID = val.substring(val.lastIndexOf("("), val.lastIndexOf(")") + 1);
                    finalSearchText = '(' + vm.configSearchContent.ManagedPropertyFileName + ":\"" + documentName.trim() + "*\" OR " + vm.configSearchContent.ManagedPropertyDocumentId + ":\"" + documentID.trim() + "*\")";
                }
                else {
                    finalSearchText = "(" + vm.configSearchContent.ManagedPropertyFileName + ":\"" + val.trim() + "*\" OR " + vm.configSearchContent.ManagedPropertyDocumentId + ":\"" + val.trim() + "*\")";
                }
            }
            var searchDocumentRequest = {
                Client: {
                    Url: configs.global.repositoryUrl
                },
                SearchObject: {
                    PageNumber: 1,
                    ItemsPerPage: 5,
                    SearchTerm: finalSearchText,
                    IsUnique: false,
                    UniqueColumnName: '',
                    FilterValue: '',
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
                        ProjectName: "",
                        PracticeGroup: "",
                        ProjectID: "",
                        SubareaOfLaw: "",
                        ToDate: ""
                    },
                    Sort:
                            {
                                ByProperty: '' + vm.configSearchContent.ManagedPropertyFileName + '',
                                Direction: 0,
                                ByColumn: "DocumentName",
                                SortAndFilterPinnedData: false
                            }
                }
            };
            if (vm.documentid == 2) {
                searchDocumentRequest.SearchObject.Filters.FilterByMe = 1;
            } else {
                searchDocumentRequest.SearchObject.Filters.FilterByMe = 0;
            }
            return documentResource.get(searchDocumentRequest).$promise;
        }

        vm.search = function () {
            vm.clearAllFilterForSearch();
            vm.pagenumber = 1;
            if (vm.documentid == 3) {
                vm.documentname = 'All Documents'
                vm.documentid = 1;
            }
            if (vm.documentid == 2) {
                searchRequest.SearchObject.Filters.FilterByMe = 1;
            } else {
                searchRequest.SearchObject.Filters.FilterByMe = 0;
            }
            vm.lazyloader = false;
            //vm.divuigrid = false;
            vm.responseNull = false;
            vm.gridOptions.data = [];
            var searchToText = '';
            var finalSearchText = '';
            if (vm.selected != "") {
                if (-1 !== vm.selected.indexOf(":")) {
                    finalSearchText = commonFunctions.searchFilter(vm.selected);
                }
                else if (vm.selected.indexOf("(") == 0 && vm.selected.indexOf(")") == vm.selected.length - 1) {
                    finalSearchText = '(' + vm.configSearchContent.ManagedPropertyFileName + ':"' + vm.selected.trim() + '*" OR ' + vm.configSearchContent.ManagedPropertyDocumentId + ':"' + vm.selected.trim() + '*" OR ' + vm.configSearchContent.ManagedPropertyDocumentClientName + ':"' + vm.selected.trim() + '*")';
                }
                else if (vm.selected.lastIndexOf("(") > 0 && vm.selected.lastIndexOf(")") == vm.selected.length - 1) {
                    var documentName = vm.selected.substring(0, vm.selected.lastIndexOf("(") - 1);
                    var documentID = vm.selected.substring(vm.selected.lastIndexOf("("), vm.selected.lastIndexOf(")") + 1);
                    finalSearchText = '(' + vm.configSearchContent.ManagedPropertyFileName + ":\"" + documentName.trim() + "*\" OR " + vm.configSearchContent.ManagedPropertyDocumentId + ":\"" + documentID.trim() + "*\" OR " + vm.configSearchContent.ManagedPropertyDocumentClientName + ":\"" + vm.selected.trim() + "*\")";
                }
                else {
                    finalSearchText = "(" + vm.configSearchContent.ManagedPropertyFileName + ":\"" + vm.selected.trim() + "*\" OR " + vm.configSearchContent.ManagedPropertyDocumentId + ":\"" + vm.selected.trim() + "*\" OR " + vm.configSearchContent.ManagedPropertyDocumentClientName + ":\"" + vm.selected.trim() + "*\")"
                }
            }
            searchRequest.SearchObject.PageNumber = vm.pagenumber;
            searchRequest.SearchObject.SearchTerm = finalSearchText;
            //searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyFileName + "";
            //searchRequest.SearchObject.Sort.Direction = 0;
            get(searchRequest, function (response) {
                if (response == "") {
                    vm.gridOptions.data = response;
                    vm.lazyloader = true;
                    //vm.divuigrid = true;
                    vm.nodata = true;
                    $interval(function () { vm.showSortExp(); }, 2000, 3);
                } else {
                    //vm.divuigrid = true;
                    vm.nodata = false;
                    vm.lazyloader = true;
                    vm.gridOptions.data = response;
                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime + "";
                    $interval(function () { vm.showSortExp(); }, 2000, 3);
                }
            });
        }

        vm.filterSearch = function (val) {
            if (val.length > 3) {

                searchRequest.SearchObject.IsUnique = true;
                searchRequest.SearchObject.FilterValue = val;

                if (vm.searchexp == vm.configSearchContent.ManagedPropertyFileName) {
                    searchRequest.SearchObject.IsUnique = false;
                    searchRequest.SearchObject.FilterValue = '';
                    searchRequest.SearchObject.UniqueColumnName = '';
                    vm.documentsearch("" + vm.configSearchContent.ManagedPropertyFileName + ":" + val + "*(* OR " + vm.configSearchContent.ManagedPropertyFileName + ":* OR " + vm.configSearchContent.ManagedPropertyDocumentId + ":* OR " + vm.configSearchContent.ManagedPropertyDocumentClientName + ":*)", vm.searchexp, false);
                }
                else if (vm.searchexp == vm.configSearchContent.ManagedPropertyDocumentClientName) {
                    searchRequest.SearchObject.UniqueColumnName = vm.configSearchContent.ManagedPropertyDocumentClientName;
                    vm.documentsearch("" + vm.configSearchContent.ManagedPropertyDocumentClientName + ":" + val + "*(* OR " + vm.configSearchContent.ManagedPropertyFileName + ":* OR " + vm.configSearchContent.ManagedPropertyDocumentId + ":* OR " + vm.configSearchContent.ManagedPropertyDocumentClientName + ":*)", vm.searchexp, false);
                }
                else if (vm.searchexp == vm.configSearchContent.ManagedPropertyMatterName) {
                    searchRequest.SearchObject.UniqueColumnName = vm.configSearchContent.ManagedPropertyMatterName;
                    vm.documentsearch("" + vm.configSearchContent.ManagedPropertyMatterName + ":" + val + "*(* OR " + vm.configSearchContent.ManagedPropertyFileName + ":* OR " + vm.configSearchContent.ManagedPropertyDocumentId + ":* OR " + vm.configSearchContent.ManagedPropertyDocumentClientName + ":*)", vm.searchexp, false);
                }
                else if (vm.searchexp == vm.configSearchContent.ManagedPropertyAuthor) {
                    searchRequest.SearchObject.UniqueColumnName = vm.configSearchContent.ManagedPropertyAuthor;
                    vm.documentsearch("" + vm.configSearchContent.ManagedPropertyAuthor + ":" + val + "*(* OR " + vm.configSearchContent.ManagedPropertyFileName + ":* OR " + vm.configSearchContent.ManagedPropertyDocumentId + ":* OR " + vm.configSearchContent.ManagedPropertyDocumentClientName + ":*)", vm.searchexp, false)
                }
                else if (vm.searchexp == vm.configSearchContent.ManagedPropertyPracticeGroup) {
                    searchRequest.SearchObject.UniqueColumnName = vm.configSearchContent.ManagedPropertyPracticeGroup;
                    vm.documentsearch("" + vm.configSearchContent.ManagedPropertyPracticeGroup + ":" + val + "*(* OR " + vm.configSearchContent.ManagedPropertyFileName + ":* OR " + vm.configSearchContent.ManagedPropertyDocumentId + ":* OR " + vm.configSearchContent.ManagedPropertyDocumentClientName + ":*)", vm.searchexp, false)
                }
                else if (vm.searchexp == vm.configSearchContent.ManagedPropertyDocumentCheckOutUser) {
                    searchRequest.SearchObject.UniqueColumnName = vm.configSearchContent.ManagedPropertyDocumentCheckOutUser;
                    vm.documentsearch("" + vm.configSearchContent.ManagedPropertyDocumentCheckOutUser + ":" + val + "*(* OR " + vm.configSearchContent.ManagedPropertyFileName + ":* OR " + vm.configSearchContent.ManagedPropertyDocumentId + ":* OR " + vm.configSearchContent.ManagedPropertyDocumentClientName + ":*)", vm.searchexp, false)
                }

            }
        }

        //#region for searching matter by property and searchterm
        vm.documentsearch = function (term, property, bool) {
            vm.lazyloaderFilter = false;
            vm.responseNull = false;
            searchRequest.SearchObject.PageNumber = 1;
            vm.filternodata = false;
            //searchRequest.SearchObject.SearchTerm = term;
            searchRequest.SearchObject.Sort.ByProperty = property;
            searchRequest.SearchObject.Sort.Direction = 1;
            if (bool) {
                vm.documentheader = true;
                //vm.divuigrid = false;
                vm.lazyloader = false;
                //searchRequest.SearchObject.SearchTerm = "";
                searchRequest.SearchObject.Sort.Direction = 1;
                if (property == "" + vm.configSearchContent.ManagedPropertyFileName + "") {
                    vm.searchTerm = term;
                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime + "";
                    searchRequest.SearchObject.Filters.Name = term;
                    vm.documentfilter = true;
                }
                else if (property == "" + vm.configSearchContent.ManagedPropertyDocumentClientName + "") {
                    vm.clientSearchTerm = term;
                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime + "";
                    searchRequest.SearchObject.Filters.ClientName = term;
                    vm.clientfilter = true;
                }
                else if (property == "" + vm.configSearchContent.ManagedPropertyMatterName + "") {
                    vm.projectSearchTerm = term;
                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime + "";
                    searchRequest.SearchObject.Filters.ProjectName = term;
                    vm.projectNamefilter = true;
                }
                else if (property == "" + vm.configSearchContent.ManagedPropertyAuthor + "") {
                    vm.authorSearchTerm = term;
                    searchRequest.SearchObject.Filters.DocumentAuthor = term;
                    vm.authorfilter = true;
                }
                else if (property == "" + vm.configSearchContent.ManagedPropertyPracticeGroup + "") {
                    vm.practiceGroupSearchTerm = term;
                    searchRequest.SearchObject.Filters.PracticeGroup = term;
                    vm.practiceGroupfilter = true;
                }
                else if (property == "" + vm.configSearchContent.ManagedPropertyDocumentCheckOutUser + "") {
                    vm.checkedSearchTerm = term;
                    searchRequest.SearchObject.Filters.DocumentCheckoutUsers = term;
                    vm.checkoutfilter = true;
                }
                else {
                    searchRequest.SearchObject.Filters.DocumentAuthor = "";
                    searchRequest.SearchObject.Filters.DocumentCheckoutUsers = "";
                }
            }
            else {
                searchRequest.SearchObject.SearchTerm = term;
                if (!searchRequest.SearchObject.IsUnique) {
                    searchRequest.SearchObject.IsUnique = true;
                    searchRequest.SearchObject.FilterValue = term.substring(term.indexOf(":") + 1, term.indexOf("*"));
                    searchRequest.SearchObject.UniqueColumnName = property;
                }

                if (property == "" + vm.configSearchContent.ManagedPropertyFileName + "") {
                    if (searchRequest.SearchObject.Filters.Name != '') {
                        vm.previousDocFileNameValue = searchRequest.SearchObject.Filters.Name;
                        searchRequest.SearchObject.Filters.Name = '';
                    }
                }
                else if (property == "" + vm.configSearchContent.ManagedPropertyDocumentClientName + "") {
                    if (searchRequest.SearchObject.Filters.ClientName != '') {
                        vm.previousDocClientNameValue = searchRequest.SearchObject.Filters.ClientName;
                        searchRequest.SearchObject.Filters.ClientName = '';
                    }
                }
                else if (property == "" + vm.configSearchContent.ManagedPropertyMatterName + "") {
                    if (searchRequest.SearchObject.Filters.ProjectName != '') {
                        vm.previousDocMatterNameValue = searchRequest.SearchObject.Filters.ProjectName;
                        searchRequest.SearchObject.Filters.ProjectName = '';
                    }
                }
                else if (property == "" + vm.configSearchContent.ManagedPropertyAuthor + "") {
                    if (searchRequest.SearchObject.Filters.DocumentAuthor != '') {
                        vm.previousDocAuthorValue = searchRequest.SearchObject.Filters.DocumentAuthor;
                        searchRequest.SearchObject.Filters.DocumentAuthor = '';
                    }
                }
                else if (property == "" + vm.configSearchContent.ManagedPropertyPracticeGroup + "") {
                    if (searchRequest.SearchObject.Filters.PracticeGroup != '') {
                        vm.previousDocPracticeGroupValue = searchRequest.SearchObject.Filters.PracticeGroup;
                        searchRequest.SearchObject.Filters.PracticeGroup = '';
                    }
                }
                else if (property == "" + vm.configSearchContent.ManagedPropertyDocumentCheckOutUser + "") {
                    if (searchRequest.SearchObject.Filters.DocumentCheckoutUsers != '') {
                        vm.previousDocCheckOutUserValue = searchRequest.SearchObject.Filters.DocumentCheckoutUsers;
                        searchRequest.SearchObject.Filters.DocumentCheckoutUsers = '';
                    }
                }
            }

            if (vm.documentid === 3) {
                searchRequest.SearchObject.Sort.SortAndFilterPinnedData = true;
                getPinnedDocuments(searchRequest, function (response) {
                    searchRequest.SearchObject.SearchTerm = '';
                    if (response == "") {
                        if (bool) {
                            vm.gridOptions.data = response;
                            vm.nodata = true;
                            vm.lazyloader = true;
                        } else {
                            vm.details = response;
                            //vm.nodata = false;
                            vm.filternodata = true;
                            searchRequest.SearchObject.IsUnique = false;
                            searchRequest.SearchObject.FilterValue = '';
                            searchRequest.SearchObject.UniqueColumnName = '';
                            vm.SetPreviousFilterVlaues();
                        }
                        vm.lazyloaderFilter = true;
                        vm.divuigrid = true;
                        $interval(function () { vm.showSortExp(); }, 2000, 3);
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                        vm.lazyloaderFilter = true;
                        if (bool) {
                            vm.gridOptions.data = response;
                            vm.details = [];
                            vm.lazyloader = true;
                            if (!$scope.$$phase) {
                                $scope.$apply();
                            }
                        } else {
                            vm.details = response;
                            vm.filternodata = false;
                            searchRequest.SearchObject.IsUnique = false;
                            searchRequest.SearchObject.FilterValue = '';
                            searchRequest.SearchObject.UniqueColumnName = '';
                            vm.SetPreviousFilterVlaues();
                        }
                        //searchRequest.SearchObject.SearchTerm = "";
                        searchRequest.SearchObject.Sort.ByProperty = "";
                        $interval(function () { vm.showSortExp(); }, 2000, 3);
                    }
                });
            } else {
                searchRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                get(searchRequest, function (response) {
                    searchRequest.SearchObject.SearchTerm = '';
                    if (response == "") {
                        if (bool) {
                            vm.gridOptions.data = response;
                            vm.nodata = true;
                            vm.lazyloader = true;
                        } else {
                            vm.details = response;
                            //vm.nodata = false;
                            vm.filternodata = true;
                            searchRequest.SearchObject.IsUnique = false;
                            searchRequest.SearchObject.FilterValue = '';
                            searchRequest.SearchObject.UniqueColumnName = '';
                            vm.SetPreviousFilterVlaues();
                        }
                        vm.lazyloaderFilter = true;
                        vm.divuigrid = true;
                        $interval(function () { vm.showSortExp(); }, 2000, 3);
                    } else {
                        vm.divuigrid = true;
                        //vm.nodata = false;
                        vm.lazyloaderFilter = true;
                        if (bool) {
                            vm.gridOptions.data = response;
                            vm.details = [];
                            vm.lazyloader = true;
                            if (!$scope.$$phase) {
                                $scope.$apply();
                            }
                        } else {
                            vm.details = response;
                            vm.filternodata = false;
                            searchRequest.SearchObject.IsUnique = false;
                            searchRequest.SearchObject.FilterValue = '';
                            searchRequest.SearchObject.UniqueColumnName = '';
                            vm.SetPreviousFilterVlaues();
                        }
                        //searchRequest.SearchObject.SearchTerm = "";
                        searchRequest.SearchObject.Sort.ByProperty = "";
                        $interval(function () { vm.showSortExp(); }, 2000, 3);
                    }
                });
            }
        }
        //#endregion

        vm.SetPreviousFilterVlaues = function () {
            if (vm.previousDocFileNameValue != '') {
                searchRequest.SearchObject.Filters.Name = vm.previousDocFileNameValue;
                vm.previousDocFileNameValue = '';
            }
            if (vm.previousDocPracticeGroupValue != '') {
                searchRequest.SearchObject.Filters.PracticeGroup = vm.previousDocPracticeGroupValue;
                vm.previousDocPracticeGroupValue = '';
            }
            if (vm.previousDocMatterNameValue != '') {
                searchRequest.SearchObject.Filters.ProjectName = vm.previousDocMatterNameValue;
                vm.previousDocMatterNameValue = '';
            }
            if (vm.previousDocClientNameValue != '') {
                searchRequest.SearchObject.Filters.ClientName = vm.previousDocClientNameValue;
                vm.previousDocClientNameValue = '';
            }
            if (vm.previousDocAuthorValue != '') {
                searchRequest.SearchObject.Filters.DocumentAuthor = vm.previousDocAuthorValue;
                vm.previousDocAuthorValue = '';
            }
            if (vm.previousSubAreaOfLawValue != '') {
                searchRequest.SearchObject.Filters.SubareaOfLaw = vm.previousSubAreaOfLawValue;
                vm.previousSubAreaOfLawValue = '';
            }
            if (vm.previousDocCheckOutUserValue != '') {
                searchRequest.SearchObject.Filters.DocumentCheckoutUsers = vm.previousDocCheckOutUserValue;
                vm.previousDocCheckOutUserValue = '';
            }
        }

        //Code for filtering ModifiedDate
        //start
        vm.FilterModifiedDate = function (name) {
            if (vm.startDate != "" || vm.endDate != "" || vm.modStartDate != "" || vm.modEndDate != "") {
                vm.documentdateheader = false;
                vm.lazyloader = false;
                vm.gridOptions.data = [];
                //vm.divuigrid = false;
                searchRequest.SearchObject.PageNumber = 1;
                searchRequest.SearchObject.SearchTerm = "";
                if (name == "Modified Date") {
                    if (vm.modStartDate != undefined) {
                        if (vm.modStartDate != "") {
                            searchRequest.SearchObject.Filters.DateFilters.ModifiedFromDate = $filter('date')(vm.modStartDate, "yyyy-MM-ddT00:00:00") + "Z";
                        }
                    } else {
                        searchRequest.SearchObject.Filters.DateFilters.ModifiedFromDate = "";
                    }
                    if (vm.modEndDate != undefined) {
                        if (vm.modEndDate != "") {
                            searchRequest.SearchObject.Filters.DateFilters.ModifiedToDate = $filter('date')(vm.modEndDate, "yyyy-MM-ddT23:59:59") + "Z";
                        }
                    } else {
                        searchRequest.SearchObject.Filters.DateFilters.ModifiedToDate = "";
                    }
                    vm.moddatefilter = true;
                }
                if (name == "Created Date") {
                    if (vm.startDate != undefined) {
                        if (vm.startDate != "") {
                            searchRequest.SearchObject.Filters.DateFilters.CreatedFromDate = $filter('date')(vm.startDate, "yyyy-MM-ddT00:00:00") + "Z";
                        }
                    } else {
                        searchRequest.SearchObject.Filters.DateFilters.CreatedFromDate = "";
                    }
                    if (vm.endDate != undefined) {
                        if (vm.endDate != "") {
                            searchRequest.SearchObject.Filters.DateFilters.CreatedToDate = $filter('date')(vm.endDate, "yyyy-MM-ddT23:59:59") + "Z";
                        }
                    } else {
                        searchRequest.SearchObject.Filters.DateFilters.CreatedToDate = "";
                    }
                    vm.createddatefilter = true;
                }
                searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime + "";
                searchRequest.SearchObject.Sort.Direction = 1;
                if ((vm.modStartDate == undefined && vm.modEndDate == undefined) || (vm.modStartDate == "" && vm.modEndDate == "") || (vm.modStartDate == undefined && vm.modEndDate == "") || (vm.modStartDate == undefined && vm.modEndDate == "")) {
                    vm.moddatefilter = false;
                }
                if ((vm.startDate == undefined && vm.endDate == undefined) || (vm.startDate == "" && vm.endDate == "") || (vm.startDate == undefined && vm.endDate == "") || (vm.startDate == "" && vm.endDate == undefined)) {
                    vm.createddatefilter = false;
                }
                if (vm.documentid === 3) {
                    searchRequest.SearchObject.Sort.SortAndFilterPinnedData = true;
                    getPinnedDocuments(searchRequest, function (response) {
                        if (response == "") {
                            vm.gridOptions.data = response;
                            vm.lazyloader = true;
                            vm.divuigrid = true;
                            vm.nodata = true;
                            $interval(function () { vm.showSortExp(); }, 2000, 3);
                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                            vm.lazyloader = true;
                            vm.gridOptions.data = response;
                            $interval(function () { vm.showSortExp(); }, 2000, 3);
                        }
                    });
                } else {
                    searchRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                    get(searchRequest, function (response) {
                        if (response == "") {
                            vm.gridOptions.data = response;
                            vm.lazyloader = true;
                            //vm.divuigrid = true;
                            vm.nodata = true;
                            $interval(function () { vm.showSortExp(); }, 2000, 3);
                        } else {
                            //vm.divuigrid = true;
                            vm.nodata = false;
                            vm.lazyloader = true;
                            vm.gridOptions.data = response;
                            $interval(function () { vm.showSortExp(); }, 2000, 3);
                        }
                    });
                }
            } else {
                vm.documentdateheader = true;
            }
        }

        vm.clearAllFilter = function () {

            vm.documentfilter = false;
            vm.searchTerm = '';
            searchRequest.SearchObject.Filters.Name = '';

            vm.clientfilter = false;
            vm.clientSearchTerm = '';
            searchRequest.SearchObject.Filters.ClientName = '';

            vm.projectNamefilter = false;
            vm.projectSearchTerm = '';
            searchRequest.SearchObject.Filters.ProjectName = '';

            vm.checkoutfilter = false;
            vm.checkedSearchTerm = '';
            searchRequest.SearchObject.Filters.DocumentCheckoutUsers = '';

            vm.authorfilter = false;
            vm.authorSearchTerm = '';
            searchRequest.SearchObject.Filters.DocumentAuthor = '';

            vm.practiceGroupfilter = false;
            vm.practiceGroupSearchTerm = '';
            searchRequest.SearchObject.Filters.PracticeGroup = '';

            vm.moddatefilter = false;
            vm.modStartDate = '';
            vm.modEndDate = '';
            searchRequest.SearchObject.Filters.DateFilters.ModifiedFromDate = '';
            searchRequest.SearchObject.Filters.DateFilters.ModifiedToDate = '';

            vm.documentheader = true;
            vm.documentdateheader = true;

            vm.createddatefilter = false;
            vm.startDate = '';
            vm.endDate = '';
            searchRequest.SearchObject.Filters.DateFilters.CreatedFromDate = '';
            searchRequest.SearchObject.Filters.DateFilters.CreatedToDate = '';

            searchRequest.SearchObject.FilterValue = '';
            searchRequest.SearchObject.IsUnique = false;
            searchRequest.SearchObject.UniqueColumnName = '';

            vm.previousDocFileNameValue = '';
            vm.previousDocClientNameValue = '';
            vm.previousDocMatterNameValue = '';
            vm.previousDocPracticeGroupValue = '';
            vm.previousDocAuthorValue = '';
            vm.previousDocCheckOutUserValue = '';
        }

        //#endregion

        //#region clearing all filters
        vm.clearFilters = function (property) {
            vm.documentheader = true;
            vm.documentdateheader = true;
            vm.lazyloader = false;
            vm.nodata = false;
            vm.pagenumber = 1;
            searchRequest.SearchObject.PageNumber = vm.pagenumber;
            if (property === vm.documentConfigContent.GridColumn1Header) {
                vm.searchTerm = "";
                //searchRequest.SearchObject.SearchTerm = "";
                searchRequest.SearchObject.Filters.Name = "";
                searchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                vm.documentfilter = false;
            }
            else if (property == vm.documentConfigContent.GridColumn2Header) {
                vm.clientSearchTerm = "";
                searchRequest.SearchObject.Filters.ClientName = "";
                searchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                vm.clientfilter = false;
            }
            else if (property == vm.documentConfigContent.GridColumn3Header) {
                vm.projectSearchTerm = "";
                searchRequest.SearchObject.Filters.ProjectName = "";
                searchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                vm.projectNamefilter = false;
            }
            else if (property == vm.documentConfigContent.GridColumn7Header) {
                vm.checkedSearchTerm = "";
                searchRequest.SearchObject.Filters.DocumentCheckoutUsers = "";
                vm.checkoutfilter = false;
            }
            else if (property == vm.documentConfigContent.GridColumn4Header && vm.globalSettings.isBackwardCompatible) {
                vm.authorSearchTerm = ""
                searchRequest.SearchObject.Filters.DocumentAuthor = "";
                vm.authorfilter = false;
            }
            else if (property == vm.documentConfigContent.GridColumn5Header && !vm.globalSettings.isBackwardCompatible) {
                vm.authorSearchTerm = ""
                searchRequest.SearchObject.Filters.DocumentAuthor = "";
                vm.authorfilter = false;
            }
            else if (property == vm.documentConfigContent.GridColumn6Header && vm.globalSettings.isBackwardCompatible) {
                vm.practiceGroupSearchTerm = ""
                searchRequest.SearchObject.Filters.PracticeGroup = "";
                vm.practiceGroupfilter = false;
            } //For Date Columns
            else if (property == vm.documentConfigContent.GridColumn5Header && vm.globalSettings.isBackwardCompatible) {
                searchRequest.SearchObject.Filters.DateFilters.ModifiedFromDate = "";
                searchRequest.SearchObject.Filters.DateFilters.ModifiedToDate = "";
                vm.modStartDate = "";
                vm.modEndDate = "";
                vm.moddatefilter = false;
                searchRequest.SearchObject.Filters.DateFilters.CreatedFromDate = "";
                searchRequest.SearchObject.Filters.DateFilters.CreatedToDate = "";
                vm.startDate = "";
                vm.endDate = "";
                vm.createddatefilter = false;
                vm.modDateOptions.maxDate = new Date();
                vm.dateOptions.maxDate = new Date();
            }
            else if (!vm.globalSettings.isBackwardCompatible && property == vm.documentConfigContent.GridColumn4Header) {
                searchRequest.SearchObject.Filters.DateFilters.ModifiedFromDate = "";
                searchRequest.SearchObject.Filters.DateFilters.ModifiedToDate = "";
                vm.modStartDate = "";
                vm.modEndDate = "";
                vm.moddatefilter = false;
                vm.modDateOptions.maxDate = new Date();
            } else if (!vm.globalSettings.isBackwardCompatible && property == vm.documentConfigContent.GridColumn8Header) {
                searchRequest.SearchObject.Filters.DateFilters.CreatedFromDate = "";
                searchRequest.SearchObject.Filters.DateFilters.CreatedToDate = "";
                vm.startDate = "";
                vm.endDate = "";
                vm.createddatefilter = false;
                vm.dateOptions.maxDate = new Date();
            }
            if (vm.documentid === 3) {
                searchRequest.SearchObject.Sort.SortAndFilterPinnedData = true;
                getPinnedDocuments(searchRequest, function (response) {
                    if (response == "") {
                        vm.gridOptions.data = response;
                        vm.lazyloader = true;
                        vm.divuigrid = true;
                        vm.nodata = true;
                        $interval(function () { vm.showSortExp(); }, 2000, 3);
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                        vm.lazyloader = true;
                        vm.gridOptions.data = response;
                        $interval(function () { vm.showSortExp(); }, 2000, 3);
                    }
                });
            } else {
                searchRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                get(searchRequest, function (response) {
                    if (response == "") {
                        vm.gridOptions.data = response;
                        vm.lazyloader = true;
                        vm.divuigrid = true;
                        vm.nodata = true;
                        $interval(function () { vm.showSortExp(); }, 2000, 3);
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                        vm.lazyloader = true;
                        vm.gridOptions.data = response;
                        $interval(function () { vm.showSortExp(); }, 2000, 3);
                    }
                });
            }
        }

        //#endregion


        //Code written for displaying types in dropdown 
        //Start 
        vm.Documents = [{ Id: 1, Name: "All documents" }, { Id: 2, Name: "My documents" }, { Id: 3, Name: "Pinned documents" }];
        vm.ddlDocuments = vm.Documents[1];
        //End  

        var pinnedDocumentsRequest = {
            Url: configs.global.repositoryUrl
        }

        //#region for setting the document name in dropdown
        vm.SetDocuments = function (id, name) {
            vm.pinnedorunpinned = false;
            vm.clearAllFilter();
            vm.clearAllFiltersofSort();
            vm.documentname = name;
            vm.documentid = id;
            vm.GetDocuments(id);
        }

        //#endregion


        //#region changing the grid based on the dropdown change 
        //Hits when the Dropdown changes 
        //Start 
        vm.GetDocuments = function (id) {
            if (!vm.pinnedorunpinned) {
                vm.selected = "";
                vm.searchTerm = "";
                vm.searchClientTerm = "";
                vm.startDate = "";
                vm.endDate = "";
                vm.lazyloader = false;
                //vm.divuigrid = false;
                vm.nodata = false;
                vm.gridOptions.data = [];
                vm.clearAllFiltersofSort();
            }
            if (id == 1) {
                //vm.divuigrid = false;
                searchRequest.SearchObject.PageNumber = 1;
                if (!vm.pinnedorunpinned) {
                    vm.responseNull = false;
                    searchRequest.SearchObject.SearchTerm = "";
                    searchRequest.SearchObject.Sort.ByProperty = vm.configSearchContent.ManagedPropertyFileName;
                    searchRequest.SearchObject.Sort.Direction = 0;
                    searchRequest.SearchObject.Filters.FilterByMe = 0;
                    searchRequest.SearchObject.Sort.ByColumn = "documentName";
                    vm.sortby = "asc";
                    vm.sortexp = "documentName";
                    vm.FileNameSort = "desc";
                }
                get(searchRequest, function (response) {
                    if (response == "") {
                        vm.gridOptions.data = response;
                        vm.lazyloader = true;
                        //vm.divuigrid = true;
                        vm.nodata = true;

                    } else {
                        if (vm.isOutlook) {
                            vm.isOutlookAsAttachment(vm.isOutlook);
                        }
                        vm.divuigrid = true;
                        vm.nodata = false;
                        vm.responseNull = false;
                        //vm.pagenumber = 1;
                        //searchRequest.SearchObject.PageNumber = 1;
                        //searchRequest.SearchObject.SearchTerm = "";
                        //searchRequest.SearchObject.Filters.FilterByMe = 0;
                        //searchRequest.SearchObject.Sort.ByColumn = "documentName";
                        searchRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                        getPinnedDocuments(searchRequest, function (pinresponse) {
                            if (pinresponse.length > 0) {
                                angular.forEach(pinresponse, function (pinobj) {
                                    angular.forEach(response, function (res) {
                                        var pinnedUrl = pinobj.documentParentUrl + "/" + pinobj.documentName
                                        var searchDocumentUrl = res.documentParentUrl + "/" + res.documentName
                                        if (pinnedUrl == searchDocumentUrl) {
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
                            $timeout(function () { vm.lazyloader = true; }, 800, angular.element(".ui-grid-row").css('visibility') != 'hidden');
                            $interval(function () { vm.showSortExp(); }, 2500, 3);
                        });
                    }
                });

            } else if (id == 2) {
                vm.lazyloader = false;
                vm.responseNull = false;
                vm.pagenumber = 1;
                searchRequest.SearchObject.PageNumber = 1;
                if (!vm.pinnedorunpinned) {
                    searchRequest.SearchObject.SearchTerm = "";
                    searchRequest.SearchObject.Filters.FilterByMe = 1;
                    searchRequest.SearchObject.Sort.ByColumn = "documentModifiedDate";
                    searchRequest.SearchObject.Sort.ByProperty = vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime;
                    searchRequest.SearchObject.Sort.Direction = 1;
                    if (!vm.globalSettings.isBackwardCompatible) {
                        vm.sortby = "asc";
                        vm.sortexp = "documentModifiedDate";
                        vm.ModiFiedDateSort = "asc";
                    }
                }
                get(searchRequest, function (response) {
                    if (response == "" && response.length == 0) {
                        vm.gridOptions.data = response;
                        vm.lazyloader = true;
                        vm.divuigrid = true;
                        vm.nodata = true;
                    } else {
                        if (vm.isOutlook) {
                            vm.isOutlookAsAttachment(vm.isOutlook);
                        }
                        //vm.divuigrid = true;
                        //vm.nodata = false;
                        searchRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                        getPinnedDocuments(searchRequest, function (pinresponse) {
                            if (pinresponse.length > 0) {
                                angular.forEach(pinresponse, function (pinobj) {
                                    angular.forEach(response, function (res) {
                                        var pinnedUrl = pinobj.documentParentUrl + "/" + pinobj.documentName
                                        var searchDocumentUrl = res.documentParentUrl + "/" + res.documentName
                                        if (pinnedUrl == searchDocumentUrl) {
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
                            vm.divuigrid = true;
                            $timeout(function () { vm.lazyloader = true; }, 800, angular.element(".ui-grid-canvas").css('visibility') != 'hidden');
                            if (!vm.globalSettings.isBackwardCompatible) {
                                $interval(function () { vm.showSortExp(); }, 3000, angular.element(".ui-grid-canvas").css('visibility') != 'hidden');
                            }
                        });
                    }
                });
            } else if (id == 3) {
                vm.lazyloader = false;
                //vm.divuigrid = false;
                if (!vm.pinnedorunpinned) {
                    var pinnedMattersRequest = {
                        Url: configs.global.repositoryUrl
                    }
                    searchRequest.SearchObject.Sort.ByColumn = '';
                    searchRequest.SearchObject.Sort.ByProperty = '';
                    searchRequest.SearchObject.Sort.Direction = 0;
                    searchRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                }
                getPinnedDocuments(searchRequest, function (response) {
                    if (response == "") {
                        vm.gridOptions.data = response;
                        vm.lazyloader = true;
                        //vm.divuigrid = true;
                        vm.nodata = true;
                    } else {
                        if (vm.isOutlook) {
                            vm.isOutlookAsAttachment(vm.isOutlook);
                        }
                        //vm.divuigrid = true;
                        vm.nodata = false;
                        angular.forEach(response, function (res) {
                            if (res.ismatterdone == undefined && !res.ismatterdone) {
                                res.MatterInfo = "Unpin this matter";
                                res.ismatterdone = true;
                            }
                        });
                        vm.gridOptions.data = response;
                        vm.lazyloader = true;
                        //$interval(function () { vm.showSortExp(); }, 2000, 3);
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
            vm.divuigrid = false;
            vm.pinnedorunpinned = true;
            vm.lazyloader = false;
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
                    $timeout(function () { vm.GetDocuments(vm.documentid); $interval(function () { vm.showSortExp(); }, 5000, 3); }, 500);
                }
            });
        }
        //End 


        //Written for pinning the matter 
        //Start 
        vm.PinMatter = function (data) {
            vm.pinnedorunpinned = true;
            vm.divuigrid = false;
            vm.lazyloader = false;
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
                    DocumentMatter: alldata.documentMatterName,
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
                    DocumentPracticeGroup: alldata.documentPracticeGroup,
                    PinType: "unpin"
                }
            }
            pinDocuments(pinRequest, function (response) {
                if (response.isDocumentPinned) {
                    $timeout(function () { vm.GetDocuments(vm.documentid); $interval(function () { vm.showSortExp(); }, 5000, 3); }, 500);
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
        vm.modDateOptions = {
            formatYear: 'yy',
            maxDate: new Date()
        };


        vm.modEndDateOptions = {
            formatYear: 'yy',
            maxDate: new Date()
        }

        $scope.$watch('vm.modStartDate', function (newval, oldval) {
            vm.modEndDateOptions.minDate = newval;
        });


        vm.openModStartDate = function ($event) {
            if ($event) {
                $event.preventDefault();
                $event.stopPropagation();
            }

            if (vm.modEndDate !== '' && vm.modEndDate !== undefined) {
                vm.modDateOptions.maxDate = vm.modEndDate;
            }
            this.modifiedStartDate = true;
        };
        vm.openModEndDate = function ($event) {
            if ($event) {
                $event.preventDefault();
                $event.stopPropagation();
            }
            this.modifiedEndDate = true;
        };

        vm.modifiedStartDate = false;
        vm.modifiedEndDate = false;

        vm.changeOnModifiedDate = function ($event) {
            if ($event.keyCode == '13' || $event.keyCode == '9') {

                var modelValue = $event.target.attributes['ng-model'].value;

                if (!/^\d{1,2}\/\d{1,2}\/\d{4}$/.test($event.target.value)) {
                    if (modelValue == 'vm.modStartDate') {
                        vm.modStartDate = new Date();
                        $event.target.value = vm.modStartDate;
                    } else {
                        vm.modEndDate = new Date();
                        $event.target.value = vm.modEndDate;
                    }
                }
                else {
                    var parts = $event.target.value.split("/");
                    var day = parseInt(parts[1], 10);
                    var month = parseInt(parts[0], 10);
                    var year = parseInt(parts[2], 10);
                    if (modelValue == 'vm.modStartDate') {
                        if (vm.modEndDate !== '' && new Date(year, month - 1, day) > vm.modEndDate) {
                            vm.modStartDate = vm.modEndDate;
                            vm.modDateOptions.maxDate = vm.modStartDate;
                        }
                        else if (new Date(year, month - 1, day) > vm.modDateOptions.maxDate && new Date(year, month - 1, day) <= new Date())
                        {
                            vm.modStartDate = new Date(year, month - 1, day);
                            vm.modEndDate = vm.modStartDate;
                            vm.modDateOptions.maxDate = vm.modStartDate;
                        } else if (new Date(year, month - 1, day) > new Date() && vm.modDateOptions.maxDate <= new Date()) {
                            vm.modStartDate = vm.modDateOptions.maxDate;
                            $event.target.value = vm.modStartDate;
                        } else if (new Date(year, month - 1, day) > new Date()) {
                            vm.modStartDate = new Date();
                            vm.modDateOptions.maxDate = vm.modStartDate;
                            $event.target.value = vm.modStartDate;
                        }
                    } else if (modelValue == 'vm.modEndDate' && new Date(year, month - 1, day) > new Date()) {
                        vm.modEndDate = new Date();
                        $event.target.value = vm.modEndDate;
                    }
                }
            }
        };

        vm.disabled = function (date, mode) {
            return (mode === 'day' && (date.getDay() != 0));
        };

        //End


        //Start
        vm.dateOptions = {
            formatYear: 'yy',
            maxDate: new Date()
        };


        vm.endDateOptions = {
            formatYear: 'yy',
            maxDate: new Date()
        }

        $scope.$watch('vm.startDate', function (newval, oldval) {
            vm.endDateOptions.minDate = newval;
        });


        vm.openStartDate = function ($event) {
            if ($event) {
                $event.preventDefault();
                $event.stopPropagation();
            }
            if (vm.endDate !== '' && vm.endDate !== undefined) {
                vm.dateOptions.maxDate = vm.endDate;
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

        vm.changeOnCreateDate = function ($event) {
            if ($event.keyCode == '13' || $event.keyCode == '9') {

                var modelValue = $event.target.attributes['ng-model'].value;

                if (!/^\d{1,2}\/\d{1,2}\/\d{4}$/.test($event.target.value)) {
                    if (modelValue == 'vm.startDate') {
                        vm.startDate = new Date();
                        $event.target.value = vm.startDate;
                    } else {
                        vm.endDate = new Date();
                        $event.target.value = vm.endDate;
                    }
                }
                else {
                    var parts = $event.target.value.split("/");
                    var day = parseInt(parts[1], 10);
                    var month = parseInt(parts[0], 10);
                    var year = parseInt(parts[2], 10);
                    if (modelValue == 'vm.startDate') {
                        if (vm.endDate !== '' && new Date(year, month - 1, day) > vm.endDate) {
                            vm.startDate = vm.endDate;
                            vm.dateOptions.maxDate = vm.startDate;
                        }
                        else if (new Date(year, month - 1, day) > vm.dateOptions.maxDate && new Date(year, month - 1, day) <= new Date())
                        {
                            vm.startDate = new Date(year, month - 1, day);
                            vm.endDate = vm.startDate;
                            vm.dateOptions.maxDate = vm.startDate;
                        }
                        else if (new Date(year, month - 1, day) > new Date() && vm.dateOptions.maxDate <= new Date()) {
                            vm.startDate = vm.dateOptions.maxDate;
                            $event.target.value = vm.startDate;
                        } else if (new Date(year, month - 1, day) > new Date()) {
                            vm.startDate = new Date();
                            vm.dateOptions.maxDate = vm.startDate;
                            $event.target.value = vm.startDate;
                        }
                    } else if (modelValue == 'vm.endDate' && new Date(year, month - 1, day) > new Date()) {
                        vm.endDate = new Date();
                        $event.target.value = vm.endDate;
                    }
                }
            }
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
            if (vm.documentid == 3) {
                var pinnedDocumentsRequest = {
                    Url: configs.global.repositoryUrl
                }
                searchRequest.SearchObject.Sort.SortAndFilterPinnedData = true;
                vm.gridOptions.data = [];
                getPinnedDocuments(searchRequest, function (response) {

                    if (response == "" || response.errorCode == "500") {
                        vm.gridOptions.data = response;
                        vm.divuigrid = true;
                        vm.nodata = true;
                        $scope.errorMessage = response.message;
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                        angular.forEach(response, function (res) {
                            if (res.ismatterdone == undefined && !res.ismatterdone) {
                                res.MatterInfo = "Unpin this document";
                                res.ismatterdone = true;
                            }
                        });
                        vm.gridOptions.data = response;
                        //if (!$scope.$$phase) {
                        //    $scope.$apply();
                        //}
                        vm.lazyloader = true;
                    }
                    searchRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                });
            }
            else {
                get(searchRequest, function (response) {
                    if (response.errorCode == "404") {
                        vm.divuigrid = false;
                        vm.nodata = true;

                    } else {
                        getPinnedDocuments(searchRequest, function (pinresponse) {
                            if (pinresponse.length > 0) {
                                angular.forEach(pinresponse, function (pinobj) {
                                    angular.forEach(response, function (res) {
                                        var pinnedUrl = pinobj.documentParentUrl + "/" + pinobj.documentName
                                        var searchDocumentUrl = res.documentParentUrl + "/" + res.documentName
                                        if (pinnedUrl == searchDocumentUrl) {
                                            if (res.ismatterdone == undefined && !res.ismatterdone) {
                                                res.MatterInfo = "Unpin this matter";
                                                res.ismatterdone = true;
                                            }
                                        }
                                    });
                                });
                                vm.gridOptions.data = response;
                                vm.divuigrid = true;
                                if (!$scope.$$phase) {
                                    $scope.$apply();
                                }
                            } else {
                                vm.gridOptions.data = response;
                                vm.divuigrid = true;
                                vm.lazyloader = true;
                                if (!$scope.$$phase) {
                                    $scope.$apply();
                                }
                            }
                            //$timeout(function () { vm.lazyloader = true; }, 800, angular.element(".ui-grid-row").css('visibility') != 'hidden');

                        });
                    }
                });
            }
        }

        //vm.sortby = "desc";
        //vm.sortexp = "documentName";
        vm.showSortExp = function () {
            //angular.element('[id^="asc"]').hide();
            //angular.element('[id^="desc"]').hide();
            if (vm.sortby == "asc") {
                angular.element("#desc" + vm.sortexp).css("display", "none");
            } else {
                angular.element("#asc" + vm.sortexp).css("display", "none");
            }
            var elm = angular.element("#" + vm.sortby + vm.sortexp);
            if (elm != undefined) {
                elm.css("display", "block");
            }
            //$timeout(function () { vm.divuigrid = true; }, 800, true);
            vm.divuigrid = true;
            if (!$scope.$$phase) {
                $scope.$apply();
            }
        }


        //$interval(function () { vm.showSortExp(); }, 2500, 3);

        //#region for sorting in ascending
        vm.documentSortBy = function (byproperty, direction, bycolumn, sortexp, sortby) {
            //vm.lazyloader = true;
            vm.pagenumber = 1;
            searchRequest.SearchObject.PageNumber = 1;
            searchRequest.SearchObject.Sort.ByProperty = byproperty;
            searchRequest.SearchObject.Sort.Direction = direction;
            searchRequest.SearchObject.Sort.ByColumn = bycolumn;
            vm.FilterByType();
            vm.sortby = sortby;
            vm.sortexp = sortexp;
            $interval(function () { vm.showSortExp(); }, 1500, 3);
        }

        vm.clearFilterValuesOnSorting = function () {
            if (vm.documentfilter == false && vm.clientfilter == false && vm.projectNamefilter == false &&
                vm.checkoutfilter == false && vm.authorfilter == false && vm.practiceGroupfilter == false &&
                vm.moddatefilter == false && vm.createddatefilter == false) {
                vm.clearAllFilter();
            }
            else {
                if (vm.documentfilter == false) {
                    vm.searchTerm = '';
                }
                if (vm.clientfilter == false) {
                    vm.clientSearchTerm = '';
                }
                if (vm.projectNamefilter == false) {
                    vm.projectSearchTerm = '';
                }
                if (vm.checkoutfilter == false) {
                    vm.checkedSearchTerm = '';
                }
                if (vm.authorfilter == false) {
                    vm.authorSearchTerm = '';
                }
                if (vm.practiceGroupfilter == false) {
                    vm.practiceGroupSearchTerm = '';
                }
                if (vm.moddatefilter == false) {
                    vm.modStartDate = '';
                    vm.modEndDate = '';
                }
                if (vm.createddatefilter == false) {
                    vm.startDate = '';
                    vm.endDate = '';
                }
            }
        }

        $scope.sortChangedDocument = function (grid, sortColumns) {
            vm.responseNull = false;
            //vm.clearAllFiltersofSort();
            vm.clearFilterValuesOnSorting();
            $timeout(function () { vm.documentdateheader = true; vm.documentheader = true; }, 1);
            $scope.gridApi.infiniteScroll.resetScroll();
            if (sortColumns.length != 0) {
                if (sortColumns[0].name != undefined) {
                    vm.lazyloader = false;
                    vm.divuigrid = false;
                    if (sortColumns[0].name.trim().toLowerCase() == configs.search.searchColumnsUIPickerForDocument.documentName.keyName.trim().toLowerCase()) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.FileNameSort == undefined || vm.FileNameSort == "asc") {
                                vm.FileNameSort = "desc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyFileName, 0, sortColumns[0].name, sortColumns[0].field, "asc");
                            } else {
                                vm.FileNameSort = "asc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyFileName, 1, sortColumns[0].name, sortColumns[0].field, "desc");
                            }
                        } else {
                            vm.divuigrid = true;
                            vm.lazyloader = true;
                        }
                    }
                    else if (sortColumns[0].name.trim().toLowerCase() == configs.search.searchColumnsUIPickerForDocument.documentClient.keyName.trim().toLowerCase()) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.DocumentClientSort == undefined || vm.DocumentClientSort == "asc") {
                                vm.DocumentClientSort = "desc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyDocumentClientName, 0, sortColumns[0].name, sortColumns[0].field, "asc");
                            }
                            else {
                                vm.DocumentClientSort = "asc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyDocumentClientName, 1, sortColumns[0].name, sortColumns[0].field, "desc");
                            }
                        } else {
                            vm.divuigrid = true;
                        }
                    }
                    else if (sortColumns[0].name.trim().toLowerCase() == configs.search.searchColumnsUIPickerForDocument.documentClientId.keyName.trim().toLowerCase()) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.DocumentClientIDSort == undefined || vm.DocumentClientIDSort == "asc") {
                                vm.DocumentClientIDSort = "desc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyDocumentClientId, 0, sortColumns[0].name, sortColumns[0].field, "asc");
                            } else {
                                vm.DocumentClientIDSort = "asc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyDocumentClientId, 1, sortColumns[0].name, sortColumns[0].field, "desc");
                            }
                        } else {
                            vm.divuigrid = true;
                        }
                    }
                    else if (sortColumns[0].name.trim().toLowerCase() == configs.search.searchColumnsUIPickerForDocument.documentModifiedDate.keyName.trim().toLowerCase()) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.ModiFiedDateSort == undefined || vm.ModiFiedDateSort == "asc") {
                                vm.ModiFiedDateSort = "desc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime, 0, sortColumns[0].name, sortColumns[0].field, "asc");
                            } else {
                                vm.ModiFiedDateSort = "asc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime, 1, sortColumns[0].name, sortColumns[0].field, "desc");
                            }
                        } else {
                            vm.divuigrid = true;
                        }
                    }
                    else if (sortColumns[0].name.trim().toLowerCase() == configs.search.searchColumnsUIPickerForDocument.documentOwner.keyName.trim().toLowerCase()) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.AuthorSort == undefined || vm.AuthorSort == "asc") {
                                vm.AuthorSort = "desc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyAuthor, 0, sortColumns[0].name, sortColumns[0].field, "asc");
                            } else {
                                vm.AuthorSort = "asc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyAuthor, 1, sortColumns[0].name, sortColumns[0].field, "desc");
                            }
                        } else {
                            vm.divuigrid = true;
                        }
                    }
                    else if (sortColumns[0].name.trim().toLowerCase() == configs.search.searchColumnsUIPickerForDocument.documentPracticeGroup.keyName.trim().toLowerCase()) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.DocumentPracticeGroupSort == undefined || vm.DocumentPracticeGroupSort == "asc") {
                                vm.DocumentPracticeGroupSort = "desc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyPracticeGroup, 0, sortColumns[0].name, sortColumns[0].field, "asc");
                            }
                            else {
                                vm.DocumentPracticeGroupSort = "asc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyPracticeGroup, 1, sortColumns[0].name, sortColumns[0].field, "desc");
                            }
                        } else {
                            vm.divuigrid = true;
                        }
                    }
                    else if (sortColumns[0].name.trim().toLowerCase() == configs.search.searchColumnsUIPickerForDocument.documentVersion.keyName.trim().toLowerCase()) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.VersionSort == undefined || vm.VersionSort == "asc") {
                                vm.VersionSort = "desc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyDocumentVersion, 0, sortColumns[0].name, sortColumns[0].field, "asc");
                            } else {
                                vm.VersionSort = "asc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyDocumentVersion, 1, sortColumns[0].name, sortColumns[0].field, "desc");
                            }
                        } else {
                            vm.divuigrid = true;
                        }
                    }
                    else if (sortColumns[0].name.trim().toLowerCase() == configs.search.searchColumnsUIPickerForDocument.documentCheckoutUser.keyName.trim().toLowerCase()) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.CheckoutSort == undefined || vm.CheckoutSort == "asc") {
                                vm.CheckoutSort = "desc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyDocumentCheckOutUser, 0, sortColumns[0].name, sortColumns[0].field, "asc");
                            } else {
                                vm.CheckoutSort = "asc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyDocumentCheckOutUser, 1, sortColumns[0].name, sortColumns[0].field, "desc");
                            }
                        } else {
                            vm.divuigrid = true;
                        }
                    }
                    else if (sortColumns[0].name.trim().toLowerCase() == configs.search.searchColumnsUIPickerForDocument.documentCreatedDate.keyName.trim().toLowerCase()) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.CreatedSort == undefined || vm.CreatedSort == "asc") {
                                vm.CreatedSort = "desc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyCreated, 0, sortColumns[0].name, sortColumns[0].field, "asc");
                            } else {
                                vm.CreatedSort = "asc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyCreated, 1, sortColumns[0].name, sortColumns[0].field, "desc");
                            }
                        } else {
                            vm.divuigrid = true;
                        }
                    }
                    else if (sortColumns[0].name.trim().toLowerCase() == configs.search.searchColumnsUIPickerForDocument.documentMatterName.keyName.trim().toLowerCase()) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.CreatedSort == undefined || vm.CreatedSort == "asc") {
                                vm.CreatedSort = "desc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyMatterName, 0, sortColumns[0].name, sortColumns[0].field, "asc");
                            } else {
                                vm.CreatedSort = "asc";
                                vm.documentSortBy(vm.configSearchContent.ManagedPropertyMatterName, 1, sortColumns[0].name, sortColumns[0].field, "desc");
                            }
                        } else {
                            vm.divuigrid = true;
                        }
                    }
                } else {
                    vm.lazyloader = true;
                    vm.divuigrid = true;
                }
            } else {
                vm.pagenumber = 1;
                searchRequest.SearchObject.PageNumber = 1;
                vm.lazyloader = true;
                vm.divuigrid = true;
                searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyFileName + "";
                searchRequest.SearchObject.Sort.Direction = 0;
                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                vm.FilterByType();
            }
        }
        //#endregion


        //#region setting the grid options when window is resized

        angular.element($window).bind('resize', function () {
            angular.element('#documentgrid .ui-grid').css('height', $window.innerHeight - 93);
            angular.element('.ui-grid-icon-menu').addClass('showExpandIcon');
            angular.element('.ui-grid-icon-menu').removeClass('closeColumnPicker');
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
            vm.clearAllFilterForSearch();
            vm.lazyloader = false;
            if (vm.documentid == 3) {
                vm.documentname = 'All Documents'
                vm.documentid = 1;
            }
            var searchToText = '';
            var finalSearchText = "";
            if (selected != "") {
                if (selected.lastIndexOf("(") > 0 && selected.lastIndexOf(")") == selected.length - 1) {
                    var documentName = selected.substring(0, selected.lastIndexOf("(") - 1);
                    var documentID = selected.substring(selected.lastIndexOf("("), selected.lastIndexOf(")") + 1);
                    finalSearchText = '(' + vm.configSearchContent.ManagedPropertyFileName + ":\"" + documentName.trim() + "\" AND " + vm.configSearchContent.ManagedPropertyDocumentId + ":\"" + documentID.trim() + "\")";
                }
                else if (selected.indexOf("(") == 0 && selected.indexOf(")") == selected.length - 1) {
                    finalSearchText = '(' + vm.configSearchContent.ManagedPropertyDocumentId + ':"' + selected.trim() + '")';
                }
                else {
                    finalSearchText = "(" + vm.configSearchContent.ManagedPropertyFileName + ":\"" + selected.trim() + "*\" OR " + vm.configSearchContent.ManagedPropertyDocumentId + ":\"" + selected.trim() + "*\")";
                }
            }
            if (vm.documentid == 2) {
                searchRequest.SearchObject.Filters.FilterByMe = 1;
            } else {
                searchRequest.SearchObject.Filters.FilterByMe = 0;
            }
            searchRequest.SearchObject.SearchTerm = finalSearchText;
            searchRequest.SearchObject.Sort.Direction = 0;
            vm.FilterByType();
            vm.lazyloader = true;
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



        vm.getDocumentAssets = function (row) {
            vm.assetsuccess = false;
            var Client = {
                Id: (row.entity.documentParentUrl.replace("/Forms/AllItems.aspx", "") + "/" + row.entity.documentName + "." + row.entity.documentExtension).replace(configs.uri.SPOsiteURL, ""),
                Name: row.entity.documentMatterUrl.replace(configs.uri.SPOsiteURL, "").replace(".aspx", "").replace("/sitepages/", "/"),
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
                $window.open(configs.global.repositoryUrl + "/SitePages/documentDetails.aspx?client=" + url.replace(configs.uri.SPOsiteURL, "") + "&listguid=" + vm.listguid + "&docguid=" + vm.docguid, 'viewmatterwindow', 'toolbar=no,location=yes,status=no,menubar=no,scrollbars=yes,resizable=yes,width=850,height=500');
            } else {
                $timeout(function () { $window.open(configs.global.repositoryUrl + "/SitePages/documentDetails.aspx?client=" + url.replace(configs.uri.SPOsiteURL, "") + "&listguid=" + vm.listguid + "&docguid=" + vm.docguid, 'viewmatterwindow', 'toolbar=no,location=yes,status=no,menubar=no,scrollbars=yes,resizable=yes,width=850,height=500'); }, 1500);
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

            vm.clearFilterValuesOnSorting();
            //Document
            if (name === vm.documentConfigContent.GridColumn1Header) {
                vm.searchexp = "" + vm.configSearchContent.ManagedPropertyFileName + "";
                vm.filtername = vm.documentConfigContent.GridColumn1Header;
            }
            //ClientName
            if (name === vm.documentConfigContent.GridColumn2Header) {
                vm.searchexp = "" + vm.configSearchContent.ManagedPropertyDocumentClientName + "";
                vm.filtername = vm.documentConfigContent.GridColumn2Header;
            }

            //Project Name
            if (name === vm.documentConfigContent.GridColumn3Header) {
                vm.searchexp = "" + vm.configSearchContent.ManagedPropertyMatterName + "";
                vm.filtername = vm.documentConfigContent.GridColumn3Header;
            }
            //Author
            if (name === vm.documentConfigContent.GridColumn4Header && vm.globalSettings.isBackwardCompatible) {
                vm.searchexp = "" + vm.configSearchContent.ManagedPropertyAuthor + "";
                vm.filtername = vm.documentConfigContent.GridColumn4Header;
            }
            if (name === vm.documentConfigContent.GridColumn5Header && !vm.globalSettings.isBackwardCompatible) {
                vm.searchexp = "" + vm.configSearchContent.ManagedPropertyAuthor + "";
                vm.filtername = vm.documentConfigContent.GridColumn5Header;
            }
            //for Practice Group
            if (name === vm.documentConfigContent.GridColumn6Header) {
                vm.searchexp = "" + vm.configSearchContent.ManagedPropertyPracticeGroup + "";
                vm.filtername = vm.documentConfigContent.GridColumn6Header;
            }
            //Check out to
            if (name === vm.documentConfigContent.GridColumn7Header) {
                vm.searchexp = "" + vm.configSearchContent.ManagedPropertyDocumentCheckOutUser + "";
                vm.filtername = vm.documentConfigContent.GridColumn7Header;
            }
            //Modified Date
            if (name === vm.documentConfigContent.GridColumn4Header && !vm.globalSettings.isBackwardCompatible) {
                vm.filtername = vm.documentConfigContent.GridColumn4Header;
            }

            if (name === vm.documentConfigContent.GridColumn5Header && vm.globalSettings.isBackwardCompatible) {
                vm.filtername = vm.documentConfigContent.GridColumn5Header;
            }
            //Created Date
            if (name == vm.documentConfigContent.GridColumn8Header) {
                vm.filtername = vm.documentConfigContent.GridColumn8Header;
            }
            vm.dateOptions.maxDate = new Date();
            vm.modDateOptions.maxDate = new Date();
            $timeout(function () {
                if (name == vm.documentConfigContent.GridColumn5Header && vm.globalSettings.isBackwardCompatible || name == vm.documentConfigContent.GridColumn8Header || name == vm.documentConfigContent.GridColumn4Header && !vm.globalSettings.isBackwardCompatible) {
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

        //#region for opening view documents url in new window
        vm.viewDocumentMatter = function (url) {
            window.open(url, 'viewmatterwindow', 'toolbar=no,location=yes,status=no,menubar=no,scrollbars=yes,resizable=yes,width=850,height=500')
        }
        //#endregion
        $rootScope.$on('disableOverlay', function (event, data) {
            vm.lazyloader = true;
        });
        //#region for clearing all the column filter search textboxes 
        vm.clearAllFilterForSearch = function () {
            vm.searchTerm = "";
            searchRequest.SearchObject.SearchTerm = "";
            searchRequest.SearchObject.Filters.Name = "";
            vm.documentfilter = false;
            vm.clientSearchTerm = "";
            searchRequest.SearchObject.Filters.ClientName = "";
            vm.clientfilter = false;
            vm.projectSearchTerm = "";
            searchRequest.SearchObject.Filters.ProjectName = "";
            vm.projectNamefilter = false;
            vm.checkedSearchTerm = "";
            searchRequest.SearchObject.Filters.DocumentCheckoutUsers = "";
            vm.checkoutfilter = false;
            vm.authorSearchTerm = ""
            searchRequest.SearchObject.Filters.DocumentAuthor = "";
            vm.authorfilter = false;
            vm.practiceGroupSearchTerm = ""
            searchRequest.SearchObject.Filters.PracticeGroup = "";
            vm.practiceGroupfilter = false;
            searchRequest.SearchObject.Filters.DateFilters.ModifiedFromDate = "";
            searchRequest.SearchObject.Filters.DateFilters.ModifiedToDate = "";
            vm.modStartDate = "";
            vm.modEndDate = "";
            vm.moddatefilter = false;
            searchRequest.SearchObject.Filters.DateFilters.CreatedFromDate = "";
            searchRequest.SearchObject.Filters.DateFilters.CreatedToDate = "";
            vm.startDate = "";
            vm.endDate = "";
            vm.createddatefilter = false;
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