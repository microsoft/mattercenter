/// <reference path="documentheadertemplate.html" />
/// <reference path="documentheadertemplate.html" />
(function () {
    'use strict';

    var app = angular.module("matterMain");

    app.controller('documentsController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout',
        'matterResource', '$rootScope', 'uiGridConstants', '$location', '$http',
    function ($scope, $state, $interval, $stateParams, api, $timeout,
        documentResource, $rootScope, uiGridConstants, $location, $http) {
        var vm = this;
        vm.selected = undefined;
        // Onload show ui grid and hide error div
        //start
        vm.divuigrid = true;
        vm.nodata = false;
        //end

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
        vm.enableAttachment = true;
        vm.asyncCallCompleted = 0;

        //#region Grid Cell/Header Templates
        //Start
        vm.documentDropDown = false;
        vm.clientDropDown = false;
        vm.modifieddateDropDown = false;
        //End

        vm.gridOptions = {
            enableHorizontalScrollbar: 0,
            enableVerticalScrollbar: 0,
            enableGridMenu: true,
            enableRowHeaderSelection: true,
            enableRowSelection: true,
            enableSelectAll: true,
            multiSelect: true,
            columnDefs: [
                { field: 'documentName', displayName: 'Document', width: '20%', enableHiding: false, cellTemplate: '../app/document/DocumentCellTemplate.html', headerCellTemplate: '../app/document/DocumentHeaderTemplate.html' },
                { field: 'documentClientId', displayName: 'Client', width: '15%', enableCellEdit: true, headerCellTemplate: '../app/document/ClientHeaderTemplate.html' },
                { field: 'documentClientId', displayName: 'Client.Matter ID', width: '10%', headerTooltip: 'Click to sort by client.matterid', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.documentClientId}}.{{row.entity.documentMatterId}}</div>', enableCellEdit: true, },
                { field: 'documentModifiedDate', displayName: 'Modified Date', width: '10%', cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.documentModifiedDate}}"></div>', headerCellTemplate: '../app/document/ModifiedDateHeaderTemplate.html' },
                { field: 'documentOwner', displayName: 'Author', width: '20%', headerTooltip: 'Click to sort by document Author', visible: false },
                { field: 'documentVersion', displayName: 'Document Version', width: '5%', headerTooltip: 'Click to sort by version', visible: false },
                { field: 'documentCheckoutUser', displayName: 'Checked out to', width: '10%', headerTooltip: 'Click to sort by who has documents checked out', cellTemplate: '<div class="ngCellText">{{row.entity.documentCheckoutUser=="" ? "NA":row.entity.documentCheckoutUser}}</div>', visible: false },
                { field: 'documentCreatedDate', displayName: 'Created date', width: '10%', headerTooltip: 'Click to sort by created date', cellTemplate: '<div class="ui-grid-cell-contents" datefilter date="{{row.entity.documentCreatedDate}}"></div>', visible: false },
            ],
            enableColumnMenus: false,
            onRegisterApi: function (gridApi) {
                $scope.gridApi = gridApi;
                gridApi.core.on.columnVisibilityChanged($scope, function (changedColumn) {
                    $scope.columnChanged = { name: changedColumn.colDef.name, visible: changedColumn.colDef.visible };
                });
                gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                    vm.selectedRow = row.entity
                    //If the app is opened in outlook, then the below validation is going to be applied
                    if (vm.isOutlook && vm.showAttachment) {
                        vm.selectedRows = $scope.gridApi.selection.getSelectedRows();
                        if (vm.selectedRows && vm.selectedRows.length < 5) {
                            vm.enableAttachment = true
                            vm.showErrorAttachmentInfo = false;
                            vm.warningMessageText = '';
                        }
                        else {
                            vm.showErrorAttachmentInfo = true;
                            vm.enableAttachment = false;
                            vm.warningMessageText = configs.uploadMessages.maxAttachedMessage;
                        }
                    }
                });
                $scope.gridApi.core.on.sortChanged($scope, vm.sortChangedDocument);
                vm.sortChangedDocument($scope.gridApi.grid, [vm.gridOptions.columnDefs[1]]);

            }
        };

        //#region Code for attaching documents in compose more
        var isAppOpenedInOutlook = $location.absUrl().split('|')[0].split('=')[2];
        if (isAppOpenedInOutlook && isAppOpenedInOutlook === "Outlook") {
            vm.isOutlook = true;
        }
        if (vm.isOutlook) {
            Office.initialize = function (reason) {
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
                            vm.enableAttachment = true;
                            vm.gridOptions.columnDefs.splice(1, 7);
                        }
                    }
                }
            };

            vm.sendDocumentAsAttachment = function () {
                if (vm.selectedRows && vm.selectedRows.length) {
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
                    vm.asyncCallCompleted = vm.asyncCallCompleted + 1;
                    if (vm.asyncCallCompleted === vm.selectedRows.length) {
                        vm.showAttachmentProgress = false;
                        vm.asyncCallCompleted = 0;
                        notifyAttachmentResult();
                    }

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
            }
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


        //#region methods for getting,filtering,pin,unpin documents
        vm.searchDocument = function (val) {
            var searchRequest = {
                Client: {
                    Id: "123456",
                    Name: "Microsoft",
                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                },
                SearchObject: {
                    PageNumber: 1,
                    ItemsPerPage: 10,
                    SearchTerm: val,
                    Filters: {},
                    Sort: {
                        ByProperty: "LastModifiedTime",
                        Direction: 1
                    }
                }
            };
            return documentResource.get(searchRequest).$promise;
        }


        vm.search = function () {
            var searchRequest =
              {
                  Client: {
                      Id: "123456",
                      Name: "Microsoft",
                      Url: "https://msmatter.sharepoint.com/sites/catalog"
                  },
                  SearchObject: {
                      PageNumber: 1,
                      ItemsPerPage: 10,
                      SearchTerm: vm.searchTerm,
                      Filters: {},
                      Sort:
                              {
                                  ByProperty: "LastModifiedTime",
                                  Direction: 1
                              }
                  }
              };
            get(searchRequest, function (response) {
                vm.gridOptions.data = response.matterDataList;
            });
        }

        //SearchRequest Object
        var searchRequest = {
            Client: {
                Id: "123456",
                Name: "Microsoft",
                Url: "https://msmatter.sharepoint.com/sites/catalog"
            },
            SearchObject: {
                PageNumber: 1,
                ItemsPerPage: 10,
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
                            ByProperty: 'LastModifiedTime',
                            Direction: 1
                        }
            }
        };


        //For Searching Matter in GridHeader Menu
        //Start
        vm.searchDocumentGrid = function () {
            vm.lazyloader = false;

            searchRequest.SearchObject.SearchTerm = vm.searchTerm;
            searchRequest.SearchObject.Sort.ByProperty = "FileName";
            searchRequest.SearchObject.Sort.Direction = 1;
            get(searchRequest, function (response) {
                vm.lazyloader = true;
                vm.documents = response;
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            });
        }
        //End

        //For Searching client in GridHeader Menu
        //start
        vm.searchClient = function () {
            vm.lazyloader = false;
            searchRequest.SearchObject.SearchTerm = vm.searchClientTerm;
            searchRequest.SearchObject.Sort.ByProperty = "MCDocumentClientName";
            searchRequest.SearchObject.Sort.Direction = 1;
            get(searchRequest, function (response) {
                vm.lazyloader = true;
                vm.Clients = response;
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            });
        }
        //end

        //For filtering documentName 
        //Start
        vm.filterdocumentName = function (documentName) {
            vm.lazyloader = false;
            searchRequest.SearchObject.SearchTerm = documentName;
            searchRequest.SearchObject.Sort.ByProperty = "FileName";
            searchRequest.SearchObject.Sort.Direction = 1;
            get(searchRequest, function (response) {
                vm.lazyloader = true;
                vm.gridOptions.data = response;
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
                vm.documents = [];
            });

        }

        //End


        //For filtering Clientname 
        //Start
        vm.filterClientName = function (clientname) {
            vm.lazyloader = false;
            searchRequest.SearchObject.SearchTerm = clientname;
            searchRequest.SearchObject.Sort.ByProperty = "MCDocumentClientName";
            searchRequest.SearchObject.Sort.Direction = 1;
            get(searchRequest, function (response) {
                vm.lazyloader = true;
                vm.gridOptions.data = response;
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
                vm.Clients = [];
            });

        }

        //End


        //Code for filtering ModifiedDate
        //start
        vm.FilterModifiedDate = function () {
            vm.lazyloader = false;
            searchRequest.SearchObject.SearchTerm = "";
            searchRequest.SearchObject.Filters.DateFilters.ModifiedFromDate = vm.startdate;
            searchRequest.SearchObject.Filters.DateFilters.ModifiedToDate = vm.enddate;
            searchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
            searchRequest.SearchObject.Sort.Direction = 0;
            get(searchRequest, function (response) {
                vm.lazyloader = true;
                vm.gridOptions.data = response;
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
                vm.startdate = "";
                vm.enddate = "";
            });

        }

        //#endregion

        //Code written for displaying types in dropdown 
        //Start 
        vm.Documents = [{ Id: 1, Name: "All documents" }, { Id: 2, Name: "My documents" }, { Id: 3, Name: "Pinned documents" }];
        vm.ddlDocuments = vm.Documents[0];
        //End  

        vm.Pinnedobj = [];
        vm.getDocumentPinned = function () {

            var pinnedDocumentsRequest = {
                Id: "123456",
                Name: "Microsoft",
                Url: "https://msmatter.sharepoint.com/sites/catalog"
            }
            getPinnedDocuments(pinnedDocumentsRequest, function (pinresponse) {
                vm.Pinnedobj = [];
                for (var i = 0; i < pinresponse.length; i++) {
                    vm.Pinnedobj.push(pinresponse[i].documentName + "." + pinresponse[i].documentExtension);
                }
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            });
            return true;
        }

        vm.getDocumentPinned();

        //#region changing the grid based on the dropdown change 
        //Hits when the Dropdown changes 
        //Start 
        vm.GetDocuments = function (id) {
            vm.lazyloader = false;
            if (id == 1) {
                var AllDocRequest = {
                    Client: {
                        Id: "123456",
                        Name: "Microsoft",
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
                    },
                    SearchObject: {
                        PageNumber: 1,
                        ItemsPerPage: 10,
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
                        Sort: {
                            ByProperty: 'LastModifiedTime',
                            Direction: 1
                        }
                    }
                };
                get(AllDocRequest, function (response) {
                    vm.lazyloader = true;
                    if (response.errorCode == "404") {
                        vm.divuigrid = false;
                        vm.nodata = true;
                        vm.errorMessage = response.message;
                    } else {
                        vm.getDocumentPinned();
                        vm.divuigrid = true;
                        vm.nodata = false;
                        if (vm.Pinnedobj.length > 0) {
                            angular.forEach(vm.Pinnedobj, function (pinobj) {
                                angular.forEach(response, function (res) {
                                    if (pinobj == res.documentName) {
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
                    }
                });


            } else if (id == 2) {
                vm.lazyloader = false;
                var MyDocRequest = {
                    Client: {
                        Id: "123456",
                        Name: "Microsoft",
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
                    },
                    SearchObject: {
                        PageNumber: 1,
                        ItemsPerPage: 10,
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
                            FilterByMe: 1,
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
                };

                get(MyDocRequest, function (response) {
                    vm.lazyloader = true;
                    if (response.errorCode == "404") {
                        vm.divuigrid = false;
                        vm.nodata = true;
                        vm.errorMessage = response.message;
                    } else {
                        vm.getDocumentPinned();
                        vm.divuigrid = true;
                        vm.nodata = false;
                        if (vm.Pinnedobj.length > 0) {
                            angular.forEach(vm.Pinnedobj, function (pinobj) {
                                angular.forEach(response, function (res) {
                                    if (pinobj == res.documentName) {
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
                    }
                });
            } else if (id == 3) {
                vm.lazyloader = false;
                var pinnedMattersRequest = {
                    Id: "123456",
                    Name: "Microsoft",
                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                }
                getPinnedDocuments(pinnedMattersRequest, function (response) {
                    vm.lazyloader = true;
                    if (response.errorCode == "404") {
                        vm.divuigrid = false;
                        vm.nodata = true;
                        vm.errorMessage = response.message;
                    } else {
                        vm.getDocumentPinned();
                        vm.divuigrid = true;
                        vm.nodata = false;
                        angular.forEach(response, function (res) {
                            if (res.ismatterdone == undefined && !res.ismatterdone) {
                                res.MatterInfo = "Unpin this matter";
                                res.ismatterdone = true;
                            }
                        });
                        vm.gridOptions.data = response;
                    }
                });
            }
        }
        //End 


        //To run GetDocuments function on page load 
        vm.GetDocuments(vm.ddlDocuments.Id);
        //End 


        //#region For pin and unpin the matter
        //Written for unpinning the matter 
        //Start 
        vm.UnpinDocument = function (data) {
            var alldata = data.entity;
            var unpinRequest = {
                Client: {
                    Id: "123456",
                    Name: "Microsoft",
                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                },
                matterData: {
                    documentName: alldata.matterUrl,
                }
            }
            UnpinDocuments(unpinRequest, function (response) {
                if (response.isMatterUnPinned) {
                    $timeout(function () { vm.GetDocuments(vm.ddldocuments.Id); }, 500);
                    alert("Success");
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
                    Id: "123456",
                    Name: "Microsoft",
                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                },
                documentData: {
                    documentName: alldata.documentName,
                    DocumentVersion: alldata.DocumentVersion,
                    DocumentClient: alldata.DocumentClient,
                    DocumentClientId: alldata.DocumentClientId,
                    DocumentClientUrl: alldata.DocumentClientUrl,
                    DocumentMatter: alldata.DocumentMatter,
                    DocumentMatterId: alldata.DocumentMatterId,
                    DocumentOwner: alldata.DocumentOwner,
                    DocumentUrl: alldata.DocumentUrl,
                    DocumentOWAUrl: alldata.DocumentOWAUrl,
                    DocumentExtension: alldata.DocumentExtension,
                    DocumentCreatedDate: alldata.DocumentCreatedDate,
                    DocumentModifiedDate: alldata.DocumentModifiedDate,
                    DocumentCheckoutUser: alldata.DocumentCheckoutUser,
                    DocumentMatterUrl: alldata.DocumentMatterUrl,
                    DocumentParentUrl: alldata.DocumentParentUrl,
                    DocumentID: alldata.DocumentID
                }
            }
            pinDocuments(pinRequest, function (response) {
                if (response.isDocumentPinned) {
                    $timeout(function () { vm.GetDocuments(vm.ddlDocuments.Id); }, 500);
                    alert("Success");
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
                    vm.gridOptions.data = response;
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                }
            });
        }

        vm.sortChangedDocument = function (grid, sortColumns) {
            vm.divuigrid = false;
            vm.nodata = true;
            if (sortColumns.length != 0) {
                if (sortColumns[0].name == vm.gridOptions.columnDefs[0].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (localStorage.FileNameSort == undefined || localStorage.FileNameSort == "asc") {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "FileName";
                            SortRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            localStorage.FileNameSort = "desc";
                        } else {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "FileName";
                            SortRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            localStorage.FileNameSort = "asc";
                        }
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
                else if (sortColumns[0].name == vm.gridOptions.columnDefs[1].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (localStorage.DocumentClientSort == undefined || localStorage.DocumentClientSort == "asc") {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "MCDocumentClientName";
                            SortRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            localStorage.DocumentClientSort = "desc";
                        }
                        else {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "MCDocumentClientName";
                            SortRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            localStorage.DocumentClientSort = "asc";
                        }
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
                else if (sortColumns[0].name == vm.gridOptions.columnDefs[2].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (localStorage.DocumentClientIDSort == undefined || localStorage.DocumentClientIDSort == "asc") {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "MCDocumentClientID";
                            SortRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            localStorage.DocumentClientIDSort = "desc";
                        } else {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "MCDocumentClientID";
                            SortRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            localStorage.DocumentClientIDSort = "asc";
                        }

                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
                else if (sortColumns[0].name == vm.gridOptions.columnDefs[3].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (localStorage.ModiFiedDateSort == undefined || localStorage.ModiFiedDateSort == "asc") {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "MCModifiedDate";
                            SortRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            localStorage.ModiFiedDateSort = "desc";
                        } else {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "MCModifiedDate";
                            SortRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            localStorage.ModiFiedDateSort = "asc";
                        }

                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
                else if (sortColumns[0].name == vm.gridOptions.columnDefs[4].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (localStorage.AuthorSort == undefined || localStorage.AuthorSort == "asc") {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "MSITOfficeAuthor";
                            SortRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            localStorage.AuthorSort = "desc";
                        } else {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "MSITOfficeAuthor";
                            SortRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            localStorage.AuthorSort = "asc";
                        }
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
                else if (sortColumns[0].name == vm.gridOptions.columnDefs[5].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (localStorage.VersionSort == undefined || localStorage.VersionSort == "asc") {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "MCVersionNumber";
                            SortRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            localStorage.VersionSort = "desc";
                        } else {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "MCVersionNumber";
                            SortRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            localStorage.VersionSort = "desc";
                        }
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
                else if (sortColumns[0].name == vm.gridOptions.columnDefs[6].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (localStorage.CheckoutSort == undefined || localStorage.CheckoutSort == "asc") {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "MCCheckoutUser";
                            SortRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            localStorage.CheckoutSort = "desc"
                        } else {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "MCCheckoutUser";
                            SortRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            localStorage.CheckoutSort = "asc"
                        }
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
                else if (sortColumns[0].name == vm.gridOptions.columnDefs[7].name) {
                    if (sortColumns[0].sort != undefined) {
                        if (localStorage.CreatedSort == undefined || localStorage.CreatedSort == "asc") {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "Created";
                            SortRequest.SearchObject.Sort.Direction = 0;
                            vm.FilterByType();
                            localStorage.CreatedSort = "desc"
                        } else {
                            vm.lazyloader = false;
                            SortRequest.SearchObject.Sort.ByProperty = "Created";
                            SortRequest.SearchObject.Sort.Direction = 1;
                            vm.FilterByType();
                            localStorage.CreatedSort = "asc"
                        }
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                    }
                }
            } else {
                vm.lazyloader = false;
                SortRequest.SearchObject.Sort.ByProperty = "FileName";
                SortRequest.SearchObject.Sort.Direction = 0;
                vm.FilterByType();
            }
        }
        //#endregion

    }]);

    app.directive('popoverdoc', function () {
        return {
            restrict: 'AE',
            scope: { details: '@' },
            link: function (scope, element, attrs) {
                scope.$watch("details", function () {

                    var obj = eval('(' + attrs.details + ')');
                    var content = '<div class="">\
                                   <div class="FlyoutBoxContent" style="width: 350px;">\
                                      <div class="FlyoutContent">\
                                          <div class="ms-Callout-content FlyoutHeadingText">  ' + obj.documentName + ' </div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Matter:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.documentMatter + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Client:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.documentClient + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Document ID:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.documentID + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Author:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.documentOwner + '</div> \
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Modified date:</div>\
                                          <div class="ms-font-m FlyoutContent" datefilter date='+ obj.documentModifiedDate + '>' + obj.documentModifiedDate + '</div>\
                                       </div>\
                                       <button class="ms-Button ms-Button--primary ms-Callout-content" id="viewMatters"><a class="ms-Button-label" href="https://msmatter.sharepoint.com/sites/microsoft/SitePages/' + obj.matterGuid + '.aspx" target="_blank">Open document</a></button>\
                                       <button class="ms-Button ms-Button--primary ms-Callout-content" id="uploadToMatter"><a class="ms-Button-label" href="https://msmatter.sharepoint.com/sites/catalog/SitePages/documentDetails.aspx">View document details</a></button>\
                                    </div>\
                                </div>';
                    $(element).popover({
                        html: true,
                        trigger: 'click',
                        delay: 500,
                        content: content,
                    });
                });
            }
        }
    });

})();

