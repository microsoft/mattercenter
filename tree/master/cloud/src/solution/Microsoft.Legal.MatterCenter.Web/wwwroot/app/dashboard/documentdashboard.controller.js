
/// <reference path="../../lib/jquery/dist/jquery.js" />
///
(function () {
    'use strict;'
    var app = angular.module("matterMain");
    app.controller('DocumentDashBoardController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'documentDashBoardResource', '$rootScope', 'uiGridConstants', '$location', '$http', 'commonFunctions', '$window', '$filter','$anchorScroll',
    function documentDashBoardController($scope, $state, $interval, $stateParams, api, $timeout, documentDashBoardResource, $rootScope, uiGridConstants, $location, $http, commonFunctions, $window, $filter, $anchorScroll) {
            //#region For declaring variables.
            var vm = this;
            vm.selected = undefined;
            vm.selectedAuthor = undefined;
            //#region Global Variable
            vm.documentdrop = false;
            vm.downwarddrop = true;
            vm.upwarddrop = false;
            vm.loadLocation = false;
            vm.AuthornoResults = false;
            vm.documentDashboardConfigs = uiconfigs.DocumentDashboard;
            vm.documentConfigContent = uiconfigs.Documents;
            vm.configSearchContent = configs.search;
            vm.center = configs.search.Schema.toLowerCase();
            vm.globalSettings = configs.global;
            vm.clientdrop = false;
            vm.lazyloaderdocumentclient = true;
            vm.clientdropvisible = false;
            vm.checkClient = false;
            vm.sortbydrop = false;
            vm.sortbydropvisible = false;
            vm.sortbytext = 'Relevant';
            vm.documentsCheckedCount = 0;
            vm.enable = true;
            vm.totalrecords = 0;
            $rootScope.bodyclass = "bodymain";
            $rootScope.profileClass = "hide";
            $rootScope.displayOverflow = "display";
            vm.nodata = false;
            vm.isDisplayMessage = false;
            vm.displayMessage = '';
            vm.tabClicked = "All Documents";
            vm.showNavTab = false;
            vm.showInnerNav = true;
            vm.selectedTab = vm.documentDashboardConfigs.Tab1HeaderText;
            vm.popupContainer = true;
            //#endregion

            //#region Variable to show document count
            vm.allDocumentCount = 0;
            vm.myDocumentCount = 0;
            vm.pinDocumentCount = 0;
            vm.selectedTabInfo = vm.documentDashboardConfigs.Tab2HeaderText + " (" + vm.myDocumentCount + ")";
            vm.selectedDocuments = [];
            //#endregion

            //#endregion Declaring Variables.

            //#region Closing all dropdowns on click of page
            vm.closealldrops = function ($event) {
                if ($event !== null) {
                    $event.stopPropagation();
                }
                vm.searchdrop = false;
                vm.downwarddrop = true;
                vm.upwarddrop = false;
                vm.clientdrop = false;
                vm.clientdropvisible = false;
                vm.sortbydrop = false;
                vm.sortbydropvisible = false;
                vm.showNavTab = false;
                vm.showInnerNav = true;
                vm.openedStartDate = false;
                vm.openedEndDate = false;
                angular.element('.popcontent').css('display', 'none');
            }
            //#endregion

            //#region Closing and hiding innerdropdowns of search box
            vm.hideinnerdashboarddrop = function ($event) {
                $event.stopPropagation();
                vm.clientdrop = false;
                vm.clientdropvisible = false;
                $timeout(function () { angular.element('#matterClients').focus(); }, 500);
                
            }

            vm.collapseDateControls = function () {
                vm.openedStartDate = true;
                vm.openedEndDate = true;
                vm.openedStartDate = false;
                vm.openedEndDate = false;
            }
            //#endregion

            //#region Declaration of GridObject with properties to display.
            var gridOptions = {
                paginationPageSize: 30,
                enableGridMenu: false,
                enableRowHeaderSelection: false,
                enableRowSelection: false,
                enableSelectAll: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableFiltering: false
            }
            //#endregion

            //#region To get the column header name
            vm.switchFuction = function (columnName) {
                var displayColumn;
                switch (columnName) {
                    case "GridColumn1Header":
                        displayColumn = vm.documentDashboardConfigs.GridColumn1Header;
                        break;
                    case "GridColumn2Header":
                        displayColumn = vm.documentDashboardConfigs.GridColumn2Header;
                        break;
                    case "GridColumn3Header":
                        displayColumn = vm.documentDashboardConfigs.GridColumn3Header;
                        break;
                    case "GridColumn4Header":
                        displayColumn = vm.documentDashboardConfigs.GridColumn4Header;
                        break;
                    case "GridColumn5Header":
                        displayColumn = vm.documentDashboardConfigs.GridColumn5Header;
                        break;
                    case "GridColumn6Header":
                        displayColumn = vm.documentDashboardConfigs.GridColumn6Header;
                        break;
                    case "GridColumn7Header":
                        displayColumn = vm.documentDashboardConfigs.GridColumn7Header;
                        break;
                    case "GridColumn8Header":
                        displayColumn = vm.documentDashboardConfigs.GridColumn8Header;
                        break;
                    default:
                        displayColumn = '';
                        break;
                }

                return displayColumn;
            };
            //#endregion

            //#region to announce document type to Jaws tool.
            vm.getDocumentName = function (docExtension) {
                var docName = '';
                switch (docExtension) {
                    case 'docx':
                        docName = 'Word document';
                        break;
                    case 'eml':
                        docName = 'Email document';
                        break;
                    case 'xls':
                        docName = 'Excel document';
                        break;
                    case 'txt':
                        docName = 'Text document';
                        break;
                    case 'pdf':
                        docName = 'PDF document';
                        break;
                    case 'zip':
                        docName = 'Zip file document';
                        break;
                    default:
                        docName = docExtension
                        break;
                }

                return docName;
            }
            //#endregion

            //#region To get the column schema and populate in column collection for grid with sorting of column display
            var columnDefs1 = [];
            columnDefs1.push({
                field: 'checker',
                displayName: 'checked',
                width: '2.5%',
                cellTemplate: '/app/dashboard/cellCheckboxTemplate.html',
                headerCellTemplate: '/app/dashboard/headerCheckboxTemplate.html',
                enableColumnMenu: false,
                position: 0
            });

            angular.forEach(configs.search.searchColumnsUIPickerForDocument, function (value, key) {

                if (value.displayInDashboard == true && value.position != -1) {
                    columnDefs1.push({
                        field: value.keyName,
                        displayName: vm.switchFuction(value.displayName),
                        width: value.dashboardwidth,
                        enableHiding: false,
                        cellTemplate: value.dashboardcellTemplate,
                        position: value.position,
                        cellClass: value.dashboardCellClass,
                        headerCellClass: value.dashboardHeaderCellClass,
                        enableColumnMenu: false
                    });
                }
            });
            //Declaring column collection object. 
            // Collection requires as columns defination will be read through appsettings files and - 
            // - number of columns is dynemic (not fixed) and reduced code redundancy and easy to read and understand.
            columnDefs1.push({
                field: 'pin',
                width: '6%',
                displayName: 'Pin/Unpin',
                cellTemplate: '<div class="ui-grid-cell-contents pad0 pull-right"><img aria-label="This image button will allow the user to pin or unpin the current document" title={{row.entity.pinType}} src="../Images/{{row.entity.pinType}}-666.png" ng-click="grid.appScope.vm.pinorunpin($event, row.entity)"/></div>',
                enableColumnMenu: false,
                position: 75
            });

            //Sorting the column as per appsetting columns defination.
            function getSortFunction(fieldName) {
                return function (col1, col2) {
                    return parseInt(col1[fieldName]) - parseInt(col2[fieldName]);
                }
            }
            columnDefs1.sort(getSortFunction("position"));
            //#endregion

            //#region Document Grid Option object declaration.
            vm.documentGridOptions = {
                enableHorizontalScrollbar: 0,
                enableVerticalScrollbar: 0,
                enableSorting: false,
                paginationPageSize: gridOptions.paginationPageSize,
                enablePagination: false,
                enableGridMenu: gridOptions.enableGridMenu,
                enableRowHeaderSelection: gridOptions.enableRowHeaderSelection,
                enableRowSelection: gridOptions.enableRowSelection,
                enableSelectAll: gridOptions.enableSelectAll,
                multiSelect: gridOptions.multiSelect,
                enableFiltering: gridOptions.enableFiltering,
                columnDefs: columnDefs1,

                onRegisterApi: function (gridApi) {
                    vm.gridApi = gridApi;
                    $scope.gridApi = gridApi;
                    //Set the selected row of the grid to selectedRow property of the controller
                    gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                    });
                }
            }
            //#endregion

            //#region for client taxonomy
            var optionsForClientGroup = {
                Client: {
                    Url: configs.global.repositoryUrl
                },
                TermStoreDetails: {
                    TermGroup: configs.taxonomy.termGroup,
                    TermSetName: configs.taxonomy.clientTermSetName,
                    CustomPropertyName: configs.taxonomy.clientCustomPropertiesURL,
                }
            };
            //#endregion

            //#region Cart functionality

            vm.cartelements = [];
            //#region Functionality to check all checkboxes inside grid
            vm.toggleChecker = function (checked, rowinfo) {
                vm.documentsCheckedCount = 0;
                allChecked = false;
                vm.cartelements = [];
                angular.forEach(vm.gridApi.grid.rows, function (row) {
                    if (row.entity.checker) {
                        vm.documentsCheckedCount = vm.documentsCheckedCount + 1;
                        row.entity.mailCartSelected = false;
                        vm.cartelements.push(row.entity);
                    }
                });
                if (vm.gridApi.grid.rows.length == vm.documentsCheckedCount) {
                    allChecked = true;
                }
                $("#chkAllDocCheckBox").prop('checked', allChecked);
            };
            //#endregion

            //#region Functionality to removing  attachemnt and elements from cart
            vm.removeAttachment = function (obj) {
                var index = 0

                for (var i = 0; i < vm.cartelements.length ; i++) {
                    if (vm.cartelements[i].documentCreatedDate == obj.documentCreatedDate && vm.cartelements[i].documentName == obj.documentName) {
                        index = i;
                        break;
                    }
                }
                if (vm.cartelements.length > 0) {
                    vm.cartelements.splice(index, 1);
                }

                angular.forEach(vm.gridApi.grid.rows, function (row) {
                    if (row.entity.checker && row.entity.documentCreatedDate == obj.documentCreatedDate && row.entity.documentName == obj.documentName) {
                        row.entity.checker = false;
                        row.entity.mailCartSelected = false;
                    }
                });

                if (vm.documentsCheckedCount > 0) {
                    vm.documentsCheckedCount = parseInt(vm.documentsCheckedCount, 10) - 1;
                }

                if (vm.cartelements.length == 0) {
                    vm.documentsCheckedCount = 0;
                    jQuery('#UploadMatterModal').modal("hide");
                }
            }
            //#endregion

            //#region Functionality to check all checkboxes inside grid
            vm.toggleCheckerAll = function (checked, event) {                
                vm.cartelements = [];
                vm.documentsCheckedCount = 0;
                for (var i = 0; i < vm.documentGridOptions.data.length; i++) {
                    vm.documentGridOptions.data[i].checker = checked;
                    if (checked) {
                        vm.cartelements.push(vm.documentGridOptions.data[i]);
                        vm.documentsCheckedCount = vm.documentGridOptions.data.length;
                        vm.selectedRows = vm.documentGridOptions.data;
                    }
                }
            };
            //#endregion

            //#region Functionality to show mail cart model
            vm.showMailCartModal = function () {
                if (vm.documentsCheckedCount > 0) {
                    angular.element('#UploadMatterModal').modal("show");
                    angular.forEach(vm.cartelements, function (cartItem) {
                        cartItem.mailCartSelected = false;
                    });
                    vm.displayMessage = '';
                    vm.isDisplayMessage = false;
                }
            }
            //#endregion

            //#region Event is going to fire when the user clicks on "Email as attachment" or "Email as link" in the modal window
            //Event suffice requirement of downloading attached files and open it directly in outlook. (Outlook client needs to be installed)
            vm.downloadEmailAsAttachment = function (downloadAttachmentsAsEmail) {
                //Get all the documents which are checked
                var i = 0;
                vm.displayMessage = '';
                angular.forEach(vm.cartelements, function (selectedDocument) {

                    if (selectedDocument.mailCartSelected) {
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

                    var oEmailRelativePath = '';
                    var sFileURLs = ""
                    angular.forEach(vm.selectedDocuments, function (selectedDocument) {

                        var docUrl = selectedDocument.documentUrl;
                        if (selectedDocument.documentUrl.indexOf("WopiFrame.aspx") > 0) {
                            docUrl = selectedDocument.documentParentUrl.replace("/Forms/AllItems.aspx", "") + '/' + selectedDocument.documentName + '.' + selectedDocument.documentExtension
                        }
                        oEmailRelativePath = trimEndChar(unescape(docUrl));
                        oEmailRelativePath = oEmailRelativePath.replace(configs.uri.SPOsiteURL, "$") + "$";
                        if (selectedDocument.documentClientUrl) {
                            sFileURLs += selectedDocument.documentClientUrl.replace("/Forms/AllItems.aspx", "") + oEmailRelativePath + selectedDocument.documentName + ";";
                        }
                        else {
                            sFileURLs += selectedDocument.documentTeamUrl.replace("/Forms/AllItems.aspx", "") + oEmailRelativePath + selectedDocument.documentName + ";";
                        }
                    });

                    var mailAttachmentDetailsRequest = {
                        FullUrl: sFileURLs,
                        IsAttachmentCall: downloadAttachmentsAsEmail
                    }

                    downloadAttachmentsAsStream(mailAttachmentDetailsRequest, function (response) {
                        var fileName = response.fileName
                        var blob = new Blob([response.fileAttachment.result], { type: "text/eml" });


                        if (window.navigator.msSaveOrOpenBlob) {
                            window.navigator.msSaveOrOpenBlob(blob, fileName);
                        } else {
                            var element = window.document.createElement("a");
                            element.href = window.URL.createObjectURL(blob);
                            element.download = fileName;
                            document.body.appendChild(element);
                            element.click();
                            document.body.removeChild(element);
                        }
                        //Once we get the response, stop the progress
                        angular.forEach(vm.cartelements, function (document) {
                            angular.element("#document-" + i).css("display", "none");
                            i = i + 1;
                        });
                        //clear the selectedDocuments array
                        vm.selectedDocuments = [];
                        //enable the vm.enable so that user can click the link again
                        vm.enable = true;
                        vm.isDisplayMessage = true;
                        if (downloadAttachmentsAsEmail) {
                            vm.displayMessage = "Selected documents has been saved as an attachment in draft email in Outlook."
                        }
                        else {
                            vm.displayMessage = "Selected documents has been saved as link in draft email in Outlook."
                        }

                    })
                }
                else {
                    vm.enable = true;
                    vm.isDisplayMessage = true;
                    vm.displayMessage = "Please select document."
                }
            }
            //#endregion

            //#regionFunction to take care triming end characters.
            function trimEndChar(sOrignalString, sCharToTrim) {
                "use strict";
                if (sOrignalString && sCharToTrim === sOrignalString.substr(-1)) {
                    return sOrignalString.substr(0, sOrignalString.length - 1);
                }
                return sOrignalString;
            }
            //#endregion Cart functionality

            //#region API calls to get document information

            //api call to get all documents
            function get(options, callback) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'get',
                    data: options,
                    success: callback
                });
            }

            //API call to get document count.
            function getDocumentCounts(options, callback) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'getDocumentCounts',
                    data: options,
                    success: callback
                });
            }

            //API call to get download attachment as stream in outlook.
            function downloadAttachmentsAsStream(options, callback) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'downloadattachmentsasstream',
                    data: options,
                    success: callback
                });
            }

            //API call to get download attachment 
            function downloadAttachments(options, callback) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'downloadAttachments',
                    data: options,
                    success: callback
                });
            }

            //API call to get all pinned documents
            function getPinDocuments(options, callback) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'getPinnedDocuments',
                    data: options,
                    success: callback
                });
            }

            //API call to get documents of the current logged in user
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

            //Callback function to get texonomy details for clients.
            function getTaxonomyDetailsForClient(optionsForClientGroup, callback) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'getTaxonomyDetails',
                    data: optionsForClientGroup,
                    success: callback
                });
            }

            //#endregion API calls to get document information

            //#region Callback function for document assets 
            function GetAssets(options, callbackOnSuccess, callbackOnError) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'getassets',
                    data: options,
                    success: callbackOnSuccess,
                    error: callbackOnError
                });
            }
            //#endregion

            //#region Callback function for document assets 
            function getUsers(optionsForUsers, callback) {
                api({
                    resource: 'documentDashBoardResource',
                    method: 'getUsers',
                    data: optionsForUsers,
                    success: callback
                });
            }
            //#endregion

            //#region Declaring request object
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
                          Direction: 1,
                          ByColumn: ""
                      }
                }
            }
            //#endregion

            //#region Functionality to showing documents as Pin or Unpin.
            vm.showDocumentAsPinOrUnpin = function (response, searchRequest) {
                documentRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                getPinDocuments(searchRequest, function (pinnedResponse) {
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

                    }
                    vm.documentGridOptions.data = response;
                    vm.getDocumentCounts();
                });
            }
            //#endregion

            vm.selected = "";
            vm.searchClicked = false;
            //#region Search functionality at global level.
            vm.search = function () {
                vm.searchClicked = true;
                angular.element('#allDocuments').addClass("active");
                angular.element('#myDocuments').removeClass("active");
                angular.element('#pinDocuments').removeClass("active");
                vm.pagenumber = 1;
                vm.displaypagination = false;
                vm.documentname = 'All Documents'
                vm.documentid = 1;
                vm.nodata = false;
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.responseNull = false;
                vm.tabClicked = "All Documents"
                vm.selectedTab = vm.documentDashboardConfigs.Tab2HeaderText;
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
                vm.selectedTab = vm.documentDashboardConfigs.Tab2HeaderText;
                documentRequest.SearchObject.Filters.FilterByMe = 0;
                vm.pagenumber = 1;
                vm.sortbytext = vm.documentDashboardConfigs.DropDownOptionText;
                documentRequest.SearchObject.PageNumber = vm.pagenumber;
                documentRequest.SearchObject.SearchTerm = finalSearchText;
                documentRequest.SearchObject.Sort.ByProperty = "FileName";
                documentRequest.SearchObject.Sort.Direction = 0;
                get(documentRequest, function (response) {
                    if (response == "" || response.length == 0) {
                        vm.getDocumentCounts();
                        vm.documentGridOptions.data = response;
                    } else {
                        vm.showDocumentAsPinOrUnpin(response, documentRequest);
                    }
                });
            }
            //#endregion

            //#region Request for search documents object
            vm.searchDocument = function (val) {
                var searchUserRequest = {
                    Client: {
                        Url: configs.global.repositoryUrl
                    },
                    SearchObject: {
                        SearchTerm: val
                    }
                };

                return documentDashBoardResource.getUsers(searchUserRequest).$promise;
            }
            //#endregion

            //#region  Searching document files
            vm.searchDocumentFile = function (val) {
                var searchDocumentRequest = {
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
                              Direction: 1,
                              ByColumn: ""
                          }
                    }
                }
                var finalSearchText = "";
                if (val != "") {
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
                vm.pagenumber = 1;
                searchDocumentRequest.SearchObject.PageNumber = vm.pagenumber;
                searchDocumentRequest.SearchObject.SearchTerm = finalSearchText;
                searchDocumentRequest.SearchObject.Sort.ByProperty = "FileName";
                searchDocumentRequest.SearchObject.Sort.Direction = 0;
                return documentDashBoardResource.get(searchDocumentRequest).$promise;
            }
            //#endregion

            //#region Functionality to get suggestions as user type search value.
            vm.typeheadselect = function (index, selected) {
                vm.searchClicked = true;
                angular.element('#allDocuments').addClass("active");
                angular.element('#myDocuments').removeClass("active");
                angular.element('#pinDocuments').removeClass("active");
                vm.pagenumber = 1;
                vm.displaypagination = false;
                vm.documentname = 'All Documents'
                vm.documentid = 1;
                vm.nodata = false;
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.responseNull = false;
                vm.tabClicked = "All Documents"
                vm.selectedTab = vm.documentDashboardConfigs.Tab2HeaderText;
                var searchToText = '';
                var finalSearchText = '';
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
                vm.pagenumber = 1;
                vm.sortbytext = vm.documentDashboardConfigs.DropDownOptionText;
                documentRequest.SearchObject.Sort.ByProperty = "";
                documentRequest.SearchObject.PageNumber = 1;
                documentRequest.SearchObject.Filters.FilterByMe = 0;
                documentRequest.SearchObject.SearchTerm = finalSearchText;
                documentRequest.SearchObject.Sort.Direction = 0;
                vm.FilterByType();
            }

            vm.authortypeheadselect = function (index, selected) {
                vm.selectedAuthor = index.name;
            }
            //#endregion

            //#reion This function will get counts for all matters, my matters and pinned matters
            vm.getDocumentCounts = function () {
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.displaypagination = false;
                getDocumentCounts(documentRequest, function (response) {
                    vm.allDocumentCount = response.allDocumentCounts;
                    if (!vm.searchClicked) {
                        vm.myDocumentCount = response.myDocumentCounts;
                        vm.pinDocumentCount = response.pinnedDocumentCounts;
                    }

                    if (vm.tabClicked.toLowerCase() == vm.documentDashboardConfigs.Tab1HeaderText.toLowerCase()) {
                        vm.totalrecords = response.myDocumentCounts;
                    }
                    if (vm.tabClicked.toLowerCase() == vm.documentDashboardConfigs.Tab2HeaderText.toLowerCase()) {
                        vm.totalrecords = response.allDocumentCounts;
                    }
                    if (vm.tabClicked.toLowerCase() == vm.documentDashboardConfigs.Tab3HeaderText.toLowerCase()) {
                        vm.totalrecords = response.pinnedDocumentCounts;
                    }
                    if (vm.searchClicked) {
                        vm.totalrecords = response.allDocumentCounts;
                    }
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }

                    if (vm.selectedTab == vm.documentDashboardConfigs.Tab1HeaderText && !vm.searchClicked) {
                        vm.selectedTabInfo = vm.documentDashboardConfigs.Tab1HeaderText + " (" + response.myDocumentCounts + ")";
                    } else if (vm.selectedTab == vm.documentDashboardConfigs.Tab2HeaderText) {
                        vm.selectedTabInfo = vm.documentDashboardConfigs.Tab2HeaderText + " (" + response.allDocumentCounts + ")";
                    } else if (vm.selectedTab == vm.documentDashboardConfigs.Tab3HeaderText && !vm.searchClicked) {
                        vm.selectedTabInfo = vm.documentDashboardConfigs.Tab3HeaderText + " (" + response.pinnedDocumentCounts + ")";
                    }
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                    vm.pagination();
                    if (response == "" || (vm.selectedTab == vm.documentDashboardConfigs.Tab2HeaderText && response.allDocumentCounts == 0) ||
                        (vm.selectedTab == vm.documentDashboardConfigs.Tab1HeaderText && response.myDocumentCounts == 0 && !vm.searchClicked) ||
                        (vm.selectedTab == vm.documentDashboardConfigs.Tab3HeaderText && response.pinnedDocumentCounts == 0 && !vm.searchClickedtype)) {
                        vm.lazyloaderdashboard = true;
                        vm.divuigrid = false;
                        vm.nodata = true;
                    } else {
                        vm.lazyloaderdashboard = true;
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                });
            }
            //#endregion

            //#region Function to get the documents based on search term
            vm.getDocuments = function () {
                vm.documentGridOptions.data = [];
                vm.tabClicked = "All Documents";
                vm.selectedTab = vm.documentDashboardConfigs.Tab2HeaderText;
                vm.sortbytext = vm.documentDashboardConfigs.DropDownOptionText;
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.nodata = false;
                vm.displaypagination = false;
                var pinnedDocumentsRequest = {
                    Url: configs.global.repositoryUrl
                }
                vm.pagenumber = 1;
                documentRequest.SearchObject.PageNumber = 1;
                documentRequest.SearchObject.Filters.FilterByMe = 0;
                documentRequest.SearchObject.Sort.ByProperty = "";
                documentRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                documentRequest.SearchObject.SearchTerm = "";
                get(documentRequest, function (response) {
                    //We need to call pinned api to determine whether a matter is pinned or not
                    vm.showDocumentAsPinOrUnpin(response, documentRequest);
                });
            }
            //#endregion

            //#region Function to get the documents which are pinned by user
            vm.getPinnedDocuments = function () {
                vm.searchClicked = false;
                vm.documentGridOptions.data = [];
                vm.tabClicked = "Pinned Documents";
                vm.selectedTab = vm.documentDashboardConfigs.Tab3HeaderText;
                vm.sortbytext = vm.documentDashboardConfigs.DrpDownOption1Text;
                vm.displaypagination = false;
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.nodata = false;
                vm.selectedClients = "";
                vm.selected = "";
                angular.element("input[name='clients']:checkbox").attr('checked', false);
                vm.selectedAuthor = "";
                vm.startDate = "";
                vm.endDate = "";
                vm.pagenumber = 1;
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
                documentRequest.SearchObject.PageNumber = 1;
                documentRequest.SearchObject.Filters.ClientsList = [];
                documentRequest.SearchObject.Filters.DocumentAuthor = "";
                documentRequest.SearchObject.Filters.FromDate = "";
                documentRequest.SearchObject.Filters.ToDate = "";
                documentRequest.SearchObject.Sort.ByProperty = "DocumentName";
                documentRequest.SearchObject.Sort.Direction = 1;
                documentRequest.SearchObject.Sort.ByColumn = "DocumentName";
                documentRequest.SearchObject.Sort.SortAndFilterPinnedData = true;
                var client = {
                    //ToDo: Need to read from config.js
                    Url: configs.global.repositoryUrl
                }

                getPinDocuments(documentRequest, function (response) {
                    if (response && response.length > 0) {
                        vm.documentGridOptions.data = response;
                        documentRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                        documentRequest.SearchObject.Sort.ByProperty = "";
                        documentRequest.SearchObject.Sort.Direction = 1;
                        documentRequest.SearchObject.Sort.ByColumn = "";
                        vm.getDocumentCounts();
                        vm.totalrecords = vm.pinDocumentCount
                    }
                    else {
                        documentRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                        documentRequest.SearchObject.Sort.ByProperty = "";
                        documentRequest.SearchObject.Sort.Direction = 1;
                        documentRequest.SearchObject.Sort.ByColumn = "";
                        vm.getDocumentCounts();
                        vm.totalrecords = 0;
                    }
                });
            }
            //#endregion    	   

            //#region Function to get the documents based on login user
            vm.getMyDocuments = function () {
                vm.selected = "";
                vm.searchClicked = false;
                vm.documentGridOptions.data = [];
                vm.tabClicked = "My Documents";
                vm.selectedTab = vm.documentDashboardConfigs.Tab1HeaderText;
                vm.sortbytext = vm.documentDashboardConfigs.DrpDownOption4Text + ' ↓';
                vm.lazyloaderdashboard = false;
                vm.displaypagination = false;
                vm.divuigrid = false;
                vm.nodata = false;
                documentRequest.SearchObject.Filters.ClientsList = [];
                documentRequest.SearchObject.Filters.DocumentAuthor = "";
                documentRequest.SearchObject.Filters.FromDate = "";
                documentRequest.SearchObject.Filters.ToDate = "";
                documentRequest.SearchObject.PageNumber = 1;
                documentRequest.SearchObject.Filters.FilterByMe = 1;
                documentRequest.SearchObject.Sort.ByProperty = "MCModifiedDate";
                documentRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                documentRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                documentRequest.SearchObject.SearchTerm = "";
                vm.startDate = "";
                vm.endDate = "";
                get(documentRequest, function (response) {
                    if (response == "" || response.length == 0) {
                        vm.getDocumentCounts();
                    } else {
                        getPinDocuments(documentRequest, function (pinnedResponse) {
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
                                vm.getDocumentCounts();

                            }
                            else {
                                vm.documentGridOptions.data = response;
                                vm.getDocumentCounts();
                            }
                        });
                    }
                });
            }
            //#endregion

            $timeout(function () { vm.getMyDocuments() }, 500);

            //#region This function will pin or unpin the document based on the image button clicked
            vm.pinorunpin = function (e, currentRowData) {
                vm.popupContainer = false;
                if (e.currentTarget.src.toLowerCase().indexOf("images/pin-666.png") > 0) {
                    jQuery.a11yfy.assertiveAnnounce("Pinning document " + currentRowData.documentName)
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
                            documentMatter: currentRowData.documentMatterName,
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
                            documentIconUrl: currentRowData.documentIconUrl,
                            documentPracticeGroup: currentRowData.documentPracticeGroup,
                            pinType: 'unpin'
                        }
                    }
                    pinDocuments(pinRequest, function (response) {
                        if (response.isDocumentPinned) {
                            jQuery.a11yfy.assertiveAnnounce("Successfully pinned document " + currentRowData.documentName)
                            e.currentTarget.src = "../images/unpin-666.png";
                            e.currentTarget.title = "unpin";
                            vm.pinDocumentCount = parseInt(vm.pinDocumentCount, 10) + 1;
                        }
                        vm.popupContainer = true;
                    });
                }
                else if (e.currentTarget.src.toLowerCase().indexOf("images/unpin-666.png") > 0) {
                    e.currentTarget.src = "../Images/loadingGreen.gif";
                    jQuery.a11yfy.assertiveAnnounce("Unpinning document " + currentRowData.documentName)
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
                            jQuery.a11yfy.assertiveAnnounce("Successfully unpinned document " + currentRowData.documentName)
                            vm.pinDocumentCount = parseInt(vm.pinDocumentCount, 10) - 1;
                            if (vm.tabClicked.toLowerCase().indexOf("pinned") >= 0) {
                                e.currentTarget.src = "../images/unpin-666.png";
                                e.currentTarget.title = "unpin";
                                vm.documentGridOptions.data.splice(vm.documentGridOptions.data.indexOf(currentRowData), 1)
                            }
                            else {
                                e.currentTarget.src = "../images/pin-666.png";
                                e.currentTarget.title = "pin";
                            }
                            if (vm.pinDocumentCount == 0) {
                                vm.divuigrid = false;
                                vm.nodata = true;
                                vm.displaypagination = false;
                            }
                        }
                        vm.popupContainer = true;
                    });
                }
            }
            //#endregion

            //#region This event is going to fire when the user clicks onm "Select All" and "UnSelect All" links
            vm.checkAll = function (checkAll, type, $event) {
                $event.stopPropagation();
                var checkAnnounc = checkAll ? "checked" : "unchecked";
                if (type === vm.documentDashboardConfigs.AdvSearchLabel1FunctionParameterText) {
                    angular.forEach(vm.clients, function (client) {
                        client.Selected = checkAll;
                    });
                    jQuery.a11yfy.assertiveAnnounce("all clients are " + checkAnnounc);
                }
            }
            //#endregion

            //#region Closing and Opening searchbar dropdowns
            vm.showupward = function ($event) {
                $event.stopPropagation();
                vm.searchdrop = true;
                vm.downwarddrop = false;
                vm.upwarddrop = true;
                jQuery.a11yfy.assertiveAnnounce("Collapsing advance search section");
                $timeout(function () { angular.element('#matterClients').focus(); }, 500);
            }

            vm.showdownward = function ($event) {
                $event.stopPropagation();
                vm.searchdrop = false;
                vm.upwarddrop = false;
                vm.downwarddrop = true;
                jQuery.a11yfy.assertiveAnnounce("Exapnding advance search section");

            }
            //#endregion

            //#region Showing and Hiding the sortby dropdown
            vm.showsortby = function ($event) {
                $event.stopPropagation();
                if (!vm.sortbydropvisible) {
                    jQuery.a11yfy.assertiveAnnounce("Expanding context menu");
                    vm.sortbydrop = true;
                    vm.sortbydropvisible = true;
                    $timeout(function () { angular.element('#menuSortBy').focus() }, 500);
                } else {
                    jQuery.a11yfy.assertiveAnnounce("Collapsing context menu");
                    vm.sortbydrop = false;
                    vm.sortbydropvisible = false;
                    $timeout(function () { angular.element('#sortIconCombo').focus() }, 500);
                }
            }
            //#endregion

            //#region For declaring startdate and enddate variable.
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
            //#endregion

            //#region Functionality to open start date selection template.
            vm.openStartDate = function ($event) {
                if ($event) {
                    $event.preventDefault();
                    $event.stopPropagation();
                }
                if (vm.endDate !== '' && vm.endDate !== undefined) {
                    vm.dateOptions.maxDate = vm.endDate;
                }
                vm.openedStartDate = vm.openedStartDate ? false : true;
                vm.openedEndDate = false;

            };
            //#endregion

            //#region Functionality to open end date selection template.
            vm.openEndDate = function ($event) {
                if ($event) {
                    $event.preventDefault();
                    $event.stopPropagation();
                }
                vm.openedEndDate = vm.openedEndDate ? false : true;
                vm.openedStartDate = false;
            };
            //#endregion
            vm.openedStartDate = false;
            vm.openedEndDate = false;

            //#region Functionality to get result as per selection of created date.
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
                            else if (new Date(year, month - 1, day) > vm.dateOptions.maxDate && new Date(year, month - 1, day) <= new Date()) {
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
            //#endregion

            //#region Showing and hiding client dropdown
            vm.showclientdrop = function ($event) {
                $event.stopPropagation();
                if (!vm.clientdropvisible) {
                    jQuery.a11yfy.assertiveAnnounce("Expanding the clients list popup");
                    if (vm.clients === undefined) {
                        jQuery.a11yfy.assertiveAnnounce("Loading the clients");
                        vm.lazyloaderdocumentclient = false;
                        getTaxonomyDetailsForClient(optionsForClientGroup, function (response) {
                            vm.clients = response.clientTerms;
                            vm.clientdrop = true;
                            vm.clientdropvisible = true;
                            jQuery.a11yfy.assertiveAnnounce("clients list loaded");
                            if (vm.selectedClients !== undefined && vm.selectedClients.length > 0) {
                                vm.customSelection(vm.documentDashboardConfigs.AdvSearchLabel1FunctionParameterText);
                            }
                            vm.lazyloaderdocumentclient = true;
                        });
                    }
                    else {
                        if (vm.selectedClients !== undefined && vm.selectedClients.length > 0) {
                            vm.customSelection(vm.documentDashboardConfigs.AdvSearchLabel1FunctionParameterText);
                        }
                        vm.clientdrop = true;
                        vm.clientdropvisible = true;
                    }
                } else if (vm.clientdropvisible && $event.type === "keyup") {
                    vm.customSelection(vm.documentDashboardConfigs.AdvSearchLabel1FunctionParameterText);
                } else {
                    jQuery.a11yfy.assertiveAnnounce("Collapsing the clients list popup");
                    vm.clientdrop = false;
                    vm.clientdropvisible = false;
                    vm.lazyloaderdocumentclient = true;
                }
            }
            //#endregion

            //#Region : Function handle the keyup events in advanced search to check and unchecked user selection.
            vm.customSelection = function (type) {
                if (type !== undefined && type === vm.documentDashboardConfigs.AdvSearchLabel1FunctionParameterText) {
                    var selectdClients = vm.selectedClients.split(',');
                    angular.forEach(vm.clients, function (client) {
                        client.Selected = false;
                        angular.forEach(selectdClients, function (clientInput) {
                            if (clientInput.toString().length > 0 && client.name.toString().toLowerCase().indexOf(clientInput.toString().toLowerCase()) !== -1) {
                                client.Selected = true;
                                jQuery.a11yfy.assertiveAnnounce(client.name + "checked");
                            }
                        })
                    });
                }
            }
            //#endRegion

            //#region This event is going to fire when the user clicks on "Cancel" button in the filter panel
            vm.filterSearchCancel = function (type) {
                if (vm.selectedClientsForCancel !== undefined && vm.selectedClientsForCancel.toString().length > 0) {
                    vm.selectedClients = vm.selectedClientsForCancel;
                    angular.forEach(vm.clients, function (client) {
                        if (vm.selectedClients.indexOf(client.name) > 0) {
                            client.Selected = true;
                        }
                    });
                }
                vm.clientdrop = false;
                vm.clientdropvisible = false;
                vm.lazyloaderdocumentclient = true;
            }
            //#endregion

            //#region For Sorting by Alphebatical or Created date
            vm.FilterByType = function () {
                vm.beforeSortingAccessibilityMessage(documentRequest);
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.nodata = false;
                vm.displaypagination = false;
                if (vm.tabClicked == "Pinned Documents") {
                    documentRequest.SearchObject.Sort.SortAndFilterPinnedData = true;
                    getPinDocuments(documentRequest, function (response) {
                        documentRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                        documentRequest.SearchObject.Sort.ByProperty = "";
                        documentRequest.SearchObject.Sort.Direction = 1;
                        documentRequest.SearchObject.Sort.ByColumn = "";
                        if (response && response.length > 0) {
                            vm.documentGridOptions.data = response;
                            vm.getDocumentCounts();
                            vm.beforeSortingAccessibilityMessage(documentRequest);
                        }
                        else {
                            vm.getDocumentCounts()
                        }
                    });

                } else {
                    documentRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                    get(documentRequest, function (response) {
                        if (response && response.length > 0) {
                            vm.documentGridOptions.data = response;
                            vm.errorMessage = response.message;
                            vm.getDocumentCounts();

                        } else {
                            vm.showDocumentAsPinOrUnpin(response, documentRequest);
                            vm.beforeSortingAccessibilityMessage(documentRequest);
                            if (!$scope.$$phase) {
                                $scope.$apply();
                            }
                        }
                    });
                }
            }
            //#endregion

            //#region For Sorting by column name and ascending and desending direction
            vm.sortExpression = function (byProperty, byColumn, sortDirection) {
                documentRequest.SearchObject.Sort.ByProperty = byProperty;
                documentRequest.SearchObject.Sort.Direction = sortDirection;
                documentRequest.SearchObject.Sort.ByColumn = byColumn;
                vm.FilterByType();
            }

            vm.sortyby = function (sortexp, data) {
                vm.sortbytext = data;
                vm.sortbydrop = false;
                if (vm.tabClicked !== "Pinned Documents") {
                    if (sortexp == 'AlphabeticalUp') {
                        vm.sortExpression(vm.configSearchContent.ManagedPropertyFileName, vm.configSearchContent.ManagedPropertyFileName, 0);
                    }
                    else if (sortexp == 'AlphabeticalDown') {
                        vm.sortExpression(vm.configSearchContent.ManagedPropertyFileName, vm.configSearchContent.ManagedPropertyFileName, 1);
                    }
                    else if (sortexp == 'CreateddateUp') {
                        vm.sortExpression(vm.configSearchContent.ManagedPropertyCreated, vm.configSearchContent.ManagedPropertyCreated, 0);
                    }
                    else if (sortexp == 'CreateddateDown') {
                        vm.sortExpression(vm.configSearchContent.ManagedPropertyCreated, vm.configSearchContent.ManagedPropertyCreated, 1);
                    }
                    else if (sortexp == 'ModifieddateUp') {
                        vm.sortExpression(vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime, vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime, 0);
                    }
                    else if (sortexp == 'ModifieddateDown') {
                        vm.sortExpression(vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime, vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime, 1);
                    }
                    else {
                        vm.sortExpression(vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime, vm.configSearchContent.ManagedPropertyDocumentLastModifiedTime, 1);
                    }
                }
                else {
                    if (sortexp == 'AlphabeticalUp') {
                        vm.sortExpression("DocumentName", "DocumentName", 0);
                    }
                    else if (sortexp == 'AlphabeticalDown') {
                        vm.sortExpression("DocumentName", "DocumentName", 1);
                    }
                    else if (sortexp == 'CreateddateUp') {
                        vm.sortExpression("DocumentCreatedDate", "DocumentCreatedDate", 0);
                    }
                    else if (sortexp == 'CreateddateDown') {
                        vm.sortExpression("DocumentCreatedDate", "DocumentCreatedDate", 1);
                    }
                    else if (sortexp == 'ModifieddateUp') {
                        vm.sortExpression("DocumentModifiedDate", "DocumentModifiedDate", 0);
                    }
                    else if (sortexp == 'ModifieddateDown') {
                        vm.sortExpression("DocumentModifiedDate", "DocumentModifiedDate", 1);
                    }
                    else {
                        vm.sortExpression("DocumentModifiedDate", "DocumentModifiedDate", 1);
                    }
                }
            }
            //#endregion


            //Declaring variables for pagination
            vm.first = 1;
            vm.last = gridOptions.paginationPageSize;
            vm.total = 0;
            vm.pagenumber = 1;
            vm.fromtopage = vm.first + " - " + vm.last;
            vm.displaypagination = false;

            //#region Functionality to implement pagination in grid.
            vm.pagination = function () {
                vm.divuigrid = false;
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
                vm.setWidthtoPagination();
                if (vm.totalrecords == 0) {
                    vm.displaypagination = false;
                    vm.divuigrid = false;
                } else {
                    vm.divuigrid = true;
                    vm.displaypagination = true;
                    $interval(function () { vm.setPaginationHeight() }, 500, vm.divuigrid);
                }
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            };
            //#endregion

            //#region Functionality to get records on next page in grid.
            vm.next = function () {
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.displaypagination = false;
                if (vm.last < vm.totalrecords) {
                    vm.documentGridOptions.data = [];
                    vm.first = vm.first + gridOptions.paginationPageSize;
                    vm.last = vm.last + gridOptions.paginationPageSize;
                    vm.total = vm.totalrecords - gridOptions.paginationPageSize;
                    if (vm.last > vm.totalrecords) {
                        vm.fromtopage = vm.first + " - " + vm.totalrecords;
                    } else {
                        vm.fromtopage = vm.first + " - " + vm.last;
                    }
                    vm.setWidthtoPagination();
                    vm.pagenumber = vm.pagenumber + 1;
                    documentRequest.SearchObject.PageNumber = vm.pagenumber;
                    documentRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                    get(documentRequest, function (response) {
                        vm.lazyloader = true;
                        if (response.errorCode == "404") {
                            vm.lazyloaderdashboard = true;
                            vm.divuigrid = false;
                            vm.nodata = true;
                            vm.errorMessage = response.message;
                        } else {
                            documentRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                            getPinDocuments(documentRequest, function (pinnedResponse) {
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
                                }
                                else {
                                    vm.documentGridOptions.data = response;
                                }
                                vm.lazyloaderdashboard = true;
                                vm.divuigrid = true;
                                vm.displaypagination = true;
                                $interval(function () { vm.setPaginationHeight() }, 300, vm.divuigrid);
                                $anchorScroll();
                            });
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

            //#region Functionality to get records on previous page in grid.
            vm.prev = function () {
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.displaypagination = false;
                if (vm.last > gridOptions.paginationPageSize) {
                    vm.documentGridOptions.data = [];
                    vm.first = vm.first - gridOptions.paginationPageSize;
                    vm.last = vm.last - gridOptions.paginationPageSize;
                    vm.pagenumber = vm.pagenumber - 1;
                    vm.fromtopage = vm.first + " - " + vm.last;
                    vm.setWidthtoPagination();
                    documentRequest.SearchObject.PageNumber = vm.pagenumber;
                    documentRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                    get(documentRequest, function (response) {
                        if (response.errorCode == "404") {
                            vm.divuigrid = false;
                            vm.nodata = true;
                            vm.errorMessage = response.message;
                            vm.lazyloaderdashboard = true;
                        } else {
                            documentRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                            getPinDocuments(documentRequest, function (pinnedResponse) {
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
                                    vm.lazyloaderdashboard = true;
                                    vm.divuigrid = true;
                                }
                                else {
                                    vm.documentGridOptions.data = response;
                                    vm.lazyloaderdashboard = true;
                                }
                                vm.displaypagination = true;
                                $interval(function () { vm.setPaginationHeight() }, 300, vm.divuigrid);
                                $anchorScroll();
                            });
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

            //#region This event is going to fire when the user clicks on "OK" button in the filter panel
            vm.filterSearchOK = function (type) {
                if (type === vm.documentDashboardConfigs.AdvSearchLabel1FunctionParameterText) {
                    vm.selectedClients = '';
                    angular.forEach(vm.clients, function (client) {
                        if (client.Selected) {
                            vm.selectedClients = vm.selectedClients + client.name + ","
                        }
                    });
                    vm.selectedClients = vm.selectedClients.slice(0, vm.selectedClients.length - 1);
                    vm.selectedClientsForCancel = vm.selectedClients;
                    vm.clientdrop = false;
                    vm.clientdropvisible = false;
                }
            }
            //#endregion

            //#region Calling the document assets api
            vm.assetsuccess = false;
            vm.getDocumentAssets = function (row) {
                vm.assetsuccess = false;
                vm.closealldrops(null);
                var Client = {
                    Id: (row.entity.documentParentUrl.replace("/Forms/AllItems.aspx", "") + "/" + row.entity.documentName + "." + row.entity.documentExtension).replace(configs.uri.SPOsiteURL, ""),
                    Name: row.entity.documentMatterUrl.replace(configs.uri.SPOsiteURL, "").replace(".aspx", "").replace("/sitepages/", "/"),
                    Url: row.entity.documentClientUrl
                }
                GetAssets(Client, function (response) {
                    vm.listguid = response.listInternalName;
                    vm.docguid = response.documentGuid;
                    vm.assetsuccess = true;
                }, function (data) {
                    vm.listguid = "";
                    vm.docguid = "";
                    vm.assetsuccess = true;
                });
            }
            //#endregion

            //#region get document URL
            vm.gotoDocumentUrl = function (url) {
                if (vm.assetsuccess) {
                    $window.open(configs.global.repositoryUrl + "/SitePages/documentDetails.aspx?client=" + url.replace(configs.uri.SPOsiteURL, "") + "&listguid=" + vm.listguid + "&docguid=" + vm.docguid, "_parent");
                } else {
                    $timeout(function () { $window.open(configs.global.repositoryUrl + "/SitePages/documentDetails.aspx?client=" + url.replace(configs.uri.SPOsiteURL, "") + "&listguid=" + vm.listguid + "&docguid=" + vm.docguid, "_parent"); }, 1500);
                }
            }

            //#region Functionality to implement search result.
            vm.getSearchResults = function () {
                angular.element('#allDocuments').addClass("active");
                angular.element('#myDocuments').removeClass("active");
                angular.element('#pinDocuments').removeClass("active");
                vm.searchClicked = true;
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.searchdrop = false;
                vm.upwarddrop = false;
                vm.downwarddrop = true;
                vm.displaypagination = false;
                vm.nodata = false;
                var clientArray = [];
                var author = "";
                var areaOfLaw = "";
                var startdate = "";
                var enddate = "";
                vm.tabClicked = 'All Documents';
                vm.selectedTab = vm.documentDashboardConfigs.Tab2HeaderText;
                if (vm.selectedClients != "" && vm.selectedClients != undefined) {
                    if (!vm.globalSettings.isBackwardCompatible) {
                        clientArray = vm.selectedClients.split(',');
                    }
                    else {
                        areaOfLaw = vm.selectedClients;
                    }
                }
                if (vm.startDate != "" && vm.startDate != undefined) {
                    startdate = $filter('date')(vm.startDate, "yyyy-MM-ddT00:00:00") + "Z";
                }
                if (vm.endDate != "" && vm.endDate != undefined) {
                    enddate = $filter('date')(vm.endDate, "yyyy-MM-ddT23:59:59") + "Z";
                }
                if (vm.selectedAuthor != "" && vm.selectedAuthor != undefined) {
                    author = vm.selectedAuthor;
                }
                if (vm.selected == "") {
                    documentRequest.SearchObject.SearchTerm = "";
                }
                documentRequest.SearchObject.Filters.FilterByMe = 0;
                vm.sortbytext = vm.documentDashboardConfigs.DropDownOptionText;
                documentRequest.SearchObject.Sort.ByProperty = "";
                documentRequest.SearchObject.Filters.AreaOfLaw = areaOfLaw;
                documentRequest.SearchObject.Filters.ClientsList = clientArray;
                documentRequest.SearchObject.Filters.DocumentAuthor = author;
                documentRequest.SearchObject.Filters.FromDate = startdate;
                documentRequest.SearchObject.Filters.ToDate = enddate;
                documentRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                get(documentRequest, function (response) {
                    if (response == "" || response.length == 0) {
                        vm.errorMessage = response.message;
                        vm.getDocumentCounts();
                    } else {
                        vm.showDocumentAsPinOrUnpin(response, documentRequest);
                        if (!$scope.$$phase) {
                            $scope.$apply();
                        }
                    }
                });
            }
            //#endregion

            //#region Showing hidden tabs in responsive
            vm.showDocTabs = function ($event) {
                $event.stopPropagation();
                if (vm.showInnerNav) {
                    vm.showNavTab = true;
                    vm.showInnerNav = false;
                }
            }
            //#endregion

            //#region To show selected tab
            vm.showSelectedTabs = function (name, count) {
                vm.selectedTab = name;
                vm.selectedTabInfo = vm.selectedTab + " (" + count + ")";
                if (name == vm.documentDashboardConfigs.Tab1HeaderText) {
                    vm.getMyDocuments();
                }
                else if (name == vm.documentDashboardConfigs.Tab2HeaderText) {
                    vm.getDocuments();
                } else {
                    vm.getPinnedDocuments();
                }
            }
            //#endregion
            vm.ariaMessage = function (message) {
                jQuery.a11yfy.assertiveAnnounce(message);
            }
            //#region Exporting to Excel Test
            vm.export = function () {
                var exportMatterSearchRequest = {
                    Client: {
                        //ToDo: Need to read from config.js
                        Url: configs.global.repositoryUrl
                    },
                    SearchObject: {
                        PageNumber: 1,
                        ItemsPerPage: 500,
                        SearchTerm: documentRequest.SearchObject.SearchTerm,
                        Filters: {
                            ClientsList: documentRequest.SearchObject.Filters.ClientsList,
                            FromDate: documentRequest.SearchObject.Filters.FromDate,
                            ToDate: documentRequest.SearchObject.Filters.ToDate,
                            DocumentAuthor: documentRequest.SearchObject.Filters.DocumentAuthor,
                            FilterByMe: documentRequest.SearchObject.Filters.FilterByMe
                        },
                        Sort:
                          {
                              ByProperty: documentRequest.SearchObject.Sort.ByProperty,
                              Direction: documentRequest.SearchObject.Sort.Direction,
                              ByColumn: documentRequest.SearchObject.Sort.ByColumn
                          }
                    }
                };
                if (vm.tabClicked != "Pinned Documents") {
                    exportMatterSearchRequest.SearchObject.Sort.SortAndFilterPinnedData = false;
                    get(exportMatterSearchRequest, function (response) {
                        if (response == "" || response.length == 0) {
                        } else {
                            vm.exportDate = response;

                            $timeout(function () {
                                $("#exportable").table2excel({
                                    // exclude CSS class
                                    exclude: ".noExl",
                                    name: "Documents",
                                    filename: "Documents" //do not include extension
                                });
                            }, 1000);
                        }
                    });
                } else {
                    var pinnedMattersRequest = {
                        Url: configs.global.repositoryUrl//ToDo: Read from config.js
                    }
                    exportMatterSearchRequest.SearchObject.Sort.SortAndFilterPinnedData = true;
                    getPinDocuments(exportMatterSearchRequest, function (response) {
                        if (response == "" || response.length == 0) {

                        } else {
                            vm.exportDate = response;

                            $timeout(function () {
                                $("#exportable").table2excel({
                                    // exclude CSS class
                                    exclude: ".noExl",
                                    name: "Documents",
                                    filename: "Documents" //do not include extension
                                });
                            }, 1000);
                        }
                    });
                }
            }
            //#endregion

            //#region To display modal up in center of the screen..
            vm.reposition = function () {
                var modal = $(this)

                var dialog = modal.find('.modal-dialog');
                modal.css('display', 'block');
                // Dividing by two centers the modal exactly, but dividing by three  
                // or four works better for larger screens. 
                dialog.css("margin-top", Math.max(0, (screen.height - dialog.height()) / 4));
            }

            // Reposition when a modal is shown 
            jQuery('.modal').on('show.bs.modal', vm.reposition);

            // Reposition when the window is resized 
            jQuery(window).on('resize', function () {
                jQuery('.modal:visible').each(vm.reposition);
            });

            $timeout(vm.reposition(), 100);

            //Angular element resize function.
            // When we minimize the window and window innerwidth is less than 867, it keeps open current active tab and -
            // other two tabs (like My Document, Pinned Documents) will be open on clicking ellipsis(...)
            angular.element($window).bind('resize', function () {
                if ($window.innerWidth > 867) {
                    vm.showNavTab = false;
                    vm.showInnerNav = true;
                }
            });

            //#region for stting the height of the pagination
            vm.setPaginationHeight = function () {
                var height = angular.element(".ui-grid-canvas").height();
                angular.element('.jsonGridFooter').css("top", height + 150);
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            }
            //#endregion

            //#region to set the dynamic width for the pagination
            vm.setWidthtoPagination = function () {
                var txt = vm.fromtopage;
                if (txt.length <= 5) {
                    angular.element('.docFromToPageWidth').css("min-width", "43px");
                } else if (txt.length <= 7) {
                    angular.element('.docFromToPageWidth').css("min-width", "64px");
                } else if (txt.length <= 9) {
                    angular.element('.docFromToPageWidth').css("min-width", "84px");
                }
            }
            //#endregion

            //Binding click event to div
            angular.element('#mainDivContainer').bind('click', function (event) {
                // Check if we have not clicked on the search box
                if (!($(event.target).parents().addBack().is('.dropdown-menu'))) {
                    // Hide/collapse your search box, autocomplete or whatever you need to do
                    angular.element('.dropdown-menu').hide('');
                }
            });

            vm.beforeSortingAccessibilityMessage = function (searchRequest) {
                if (searchRequest.SearchObject.Sort.Direction == 0) {
                    jQuery.a11yfy.assertiveAnnounce("sorting data by " + searchRequest.SearchObject.Sort.ByColumn + " in ascending order");
                } else if (searchRequest.SearchObject.Sort.Direction == 1) {
                    jQuery.a11yfy.assertiveAnnounce("sorting data by " + searchRequest.SearchObject.Sort.ByColumn + " in descending order");
                }
            }
            vm.afterSortingAccessibilityMessage = function (searchRequest) {
                if (searchRequest.SearchObject.Sort.Direction == 0) {
                    jQuery.a11yfy.assertiveAnnounce("sorted data by " + searchRequest.SearchObject.Sort.ByColumn + " in ascending order");
                } else if (searchRequest.SearchObject.Sort.Direction == 1) {
                    jQuery.a11yfy.assertiveAnnounce("sorted data by " + searchRequest.SearchObject.Sort.ByColumn + " in descending order");
                }
            }
            //#region For removing the active class from the tabs that are not selected
            vm.hideTabs = function ($event) {
                if (!vm.lazyloaderdashboard) {
                    if (vm.selectedTab == vm.documentDashboardConfigs.Tab1HeaderText) {
                        angular.element('#allDocuments').removeClass("active");
                        angular.element('#myDocuments').addClass("active");
                        angular.element('#pinDocuments').removeClass("active");
                    } else if (vm.selectedTab == vm.documentDashboardConfigs.Tab2HeaderText) {
                        angular.element('#allDocuments').addClass("active");
                        angular.element('#myDocuments').removeClass("active");
                        angular.element('#pinDocuments').removeClass("active");
                    } else {
                        angular.element('#allDocuments').removeClass("active");
                        angular.element('#myDocuments').removeClass("active");
                        angular.element('#pinDocuments').addClass("active");
                    }
                }
            }
            //#endregion


            vm.toggleCheckerForKeyDown = function (temp, currentRow, event) {
                temp = temp ? false : true;
                currentRow.checker = temp;
            }

            vm.pageLoadCompleted = function () {
                jQuery.a11yfy.assertiveAnnounce("Documents dashboard page loaded successfully");
            }
        }
    ]);
})();