(function () {
    'use strict';

    var app = angular.module("matterMain");

    app.controller('mattersController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'matterResource', '$rootScope', 'uiGridConstants', '$location', '$http', '$window', '$parse', '$templateCache', '$q', '$filter', 'commonFunctions', '$animate',
        function ($scope, $state, $interval, $stateParams, api, $timeout, matterResource, $rootScope, uiGridConstants, $location, $http, $window, $parse, $templateCache, $q, $filter, commonFunctions, $animate) {
            var vm = this;
            vm.selected = '';
            vm.selectedRow = {
                matterClientUrl: '',
                matterName: '',
                matterGuid: ''
            };
            //#region dynamic contents
            vm.navigationContent = uiconfigs.Navigation;
            vm.configSearchContent = configs.search;
            vm.matterConfigContent = uiconfigs.Matters;
            vm.configsUri = configs.uri;
            vm.globalSettings = configs.global;
            vm.header = uiconfigs.Header;
            vm.center = configs.search.Schema.toLowerCase();
            //#region for setting the dynamic width to grid
            var screenHeight = 0;
            vm.searchResultsLength = 0;
            //#end region
            vm.mattername = "" + vm.matterConfigContent.Dropdown1Item2 + "";
            vm.sortname = "";
            vm.mattersdrop = false;
            vm.mattersdropinner = true;
            $rootScope.bodyclass = "bodymain";
            $rootScope.profileClass = "";
            $rootScope.displayOverflow = "";
            //This value is for displaying the help
            $rootScope.pageIndex = "1";
            //#region Onload show ui grid and hide error div
            //start
            vm.divuigrid = true;
            //vm.nodata = false;
            vm.filternodata = false;
            vm.matterid = 2;

            vm.sortby = "desc";
            vm.sortexp = "matterModifiedDate";
            //#endregion

            //#region To hide lazyloader on load
            //start
            vm.lazyloader = true;
            vm.lazyloaderFilter = true;
            //#endregion

            //#region scopes for displaying and hiding filter icons
            //start
            vm.matterfilter = false;
            vm.moddatefilter = false;
            vm.opendatefilter = false;
            vm.clientfilter = false;
            vm.areafilter = false;
            vm.areaoflawfilter = false;
            vm.subareafilter = false;
            vm.attorneyfilter = false;
            vm.practiceGroupfilter = false;
            vm.projectIDfilter = false;
            vm.showfiltericon = vm.configSearchContent.ManagedPropertyLastModifiedTime;
            //end

            //#region Assigning scopes for Dropdowns in headers
            //Start
            vm.matterDropDowm = false;
            vm.clientDropDowm = false;
            vm.modifieddateDropDowm = false;
            vm.attorneyDropDowm = false;
            vm.arealawDropDowm = false;
            vm.subArealawDropDowm = false;
            vm.practiceGroupDropDown = false;
            vm.projectIDDropDown = false;
            vm.opendateDropDown = false;
            //End
            Office.initialize = function (reason) {
                // vm.initOutlook();
            };
            $scope.initOfficeLibrary = function () {

            };

            //#region for setting the dynamic width to grid
            vm.setWidth = function () {
                var width = $window.innerWidth;
                angular.element(".ui-grid-viewport").css('max-width', width);
                angular.element(".ui-grid-render-container").css('max-width', width);
                screenHeight = $window.screen.availHeight;
                if (screenHeight <= 768) {
                    vm.searchResultsLength = 20;
                } else if (screenHeight <= 1024 && screenHeight >= 769) {
                    vm.searchResultsLength = 30;
                } else if (screenHeight <= 1080 && screenHeight >= 1025) {
                    vm.searchResultsLength = 40;
                }
            };

            vm.setWidth();

            //#endregion

            //For setting dynamic height to the grid
            vm.getTableHeight = function () {
                return {
                    height: ($window.innerHeight - 100) + "px"
                };
            };

            $templateCache.put('coldefheadertemplate.html', "<div><div role='button' class='ui-grid-cell-contents ui-grid-header-cell-primary-focus' col-index='renderIndex'><span class='ui-grid-header-cell-label ng-binding' title='Click to sort by'>{{ col.colDef.displayName }}<span id='asc{{col.colDef.field}}' style='float:right;display:none' class='padl10px'>↑</span><span id='desc{{col.colDef.field}}' style='float:right;display:none' class='padlf10'>↓</span></span></div></div>");

            var columnDefs1 = [];
            angular.forEach(configs.search.searchColumnsUIPickerForMatter, function (value, key) {
                if (key == "matterName") {
                    if (value.displayInUI == true && value.position != -1) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterConfigContent.GridColumn1Header,
                            enableHiding: false,
                            width: "275",
                            cellTemplate: '../app/matter/MatterTemplates/MatterCellTemplate.html',
                            headerCellTemplate: '../app/matter/MatterTemplates/MatterHeaderTemplate.html',
                            position: value.position,

                        });
                    }
                }
                if (key == "matterClient") {
                    if (value.displayInUI == true && value.position != -1) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterConfigContent.GridColumn2Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            enableCellEdit: true,
                            width: "200",
                            headerCellTemplate: '../app/matter/MatterTemplates/ClientHeaderTemplate.html',
                            position: value.position,
                            visible: value.defaultVisibleInGrid
                        });
                    }
                }
                if (key == "matterClientId") {
                    if (value.displayInUI == true && value.position != -1) {
                        var cellTemplateContent = "";
                        if (!vm.globalSettings.isBackwardCompatible) {
                            cellTemplateContent = '<div class="ui-grid-cell-contents" >{{row.entity.matterClientId}}.{{row.entity.matterID}}</div>';
                        }
                        else {
                            cellTemplateContent = '<div class="ui-grid-cell-contents" >{{row.entity.matterID}}</div>';
                        }
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterConfigContent.GridColumn3Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "150",
                            headerCellTemplate: $templateCache.get('coldefheadertemplate.html'),
                            cellTemplate: cellTemplateContent,
                            enableCellEdit: true,
                            position: value.position,
                            visible: value.defaultVisibleInGrid
                        });

                    }

                }
                if (key == "matterModifiedDate") {
                    if (value.displayInUI == true && value.position != -1) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterConfigContent.GridColumn4Header,
                            width: "195",
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.matterModifiedDate}}"></div>',
                            headerCellTemplate: '../app/matter/MatterTemplates/ModifiedDateTemplate.html',
                            position: value.position,
                            visible: value.defaultVisibleInGrid
                        });

                    }
                }
                if (key == "matterResponsibleAttorney") {
                    if (value.displayInUI == true && value.position != -1) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterConfigContent.GridColumn5Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            headerCellTemplate: '../app/matter/MatterTemplates/ResponsibleAttorneyHeaderTemplate.html',
                            width: "250",
                            position: value.position,
                            visible: value.defaultVisibleInGrid
                        });

                    }
                }
                if (key == "matterSubAreaOfLaw") {
                    if (value.displayInUI == true && value.position != -1) {

                        var columnName = "";
                        var cellTemplate = "";
                        if (!vm.globalSettings.isBackwardCompatible) {
                            columnName = vm.matterConfigContent.GridColumn6Header
                            cellTemplate = '../app/matter/MatterTemplates/AreaofLawHeaderTemplate.html';
                        }
                        else {
                            columnName = vm.matterConfigContent.GridColumn4Header;
                            cellTemplate = '../app/matter/MatterTemplates/SubAreaofLawHeaderTemplate.html';
                        }

                        columnDefs1.push({
                            field: key,
                            displayName: columnName,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.matterSubAreaOfLaw==""?"NA":row.entity.matterSubAreaOfLaw}}</div>',
                            headerCellTemplate: cellTemplate,
                            width: "210",
                            position: value.position,
                            visible: value.defaultVisibleInGrid
                        });

                    }
                }
                if (key == "matterCreatedDate") {
                    var columnName = "";
                    if (!vm.globalSettings.isBackwardCompatible) {
                        columnName = vm.matterConfigContent.GridColumn7Header
                    }
                    else {
                        columnName = vm.matterConfigContent.GridColumn5Header
                    }
                    if (value.displayInUI == true && value.position != -1) {
                        columnDefs1.push({
                            field: key,
                            displayName: columnName,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            headerCellTemplate: '../app/matter/MatterTemplates/OpenDateTemplate.html',
                            width: "170",
                            cellTemplate: '<div class="ui-grid-cell-contents" datefilter date="{{row.entity.matterCreatedDate}}"></div>',
                            position: value.position,
                            visible: value.defaultVisibleInGrid
                        });
                    }
                }
                if (key == "matterDescription" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterConfigContent.GridColumn8Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "210",
                            position: value.position,
                            visible: value.defaultVisibleInGrid
                        });

                    }
                }
                if (key == "matterUrl" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterConfigContent.GridColumn9Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "210",
                            position: value.position,
                            visible: value.defaultVisibleInGrid
                        });

                    }
                }
                if (key == "matterClientUrl" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterConfigContent.GridColumn10Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "210",
                            position: value.position,
                            visible: value.defaultVisibleInGrid
                        });

                    }
                }
                if (key == "matterPracticeGroup" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterConfigContent.GridColumn2Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "210",
                            position: value.position,
                            headerCellTemplate: "../app/matter/MatterTemplates/PracticeGroupHeaderTemplate.html",
                            visible: value.defaultVisibleInGrid
                        });

                    }
                }

                if (key == "matterAreaOfLaw" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterConfigContent.GridColumn3Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            headerCellTemplate: '../app/matter/MatterTemplates/AreaofLawHeaderTemplate.html',
                            width: "210",
                            visible: value.defaultVisibleInGrid,
                            position: value.position
                        });

                    }
                }
                if (key == "hideUpload" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterConfigContent.GridColumn13Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "210",
                            visible: value.defaultVisibleInGrid,
                            position: value.position
                        });

                    }
                }
                if (key == "matterID" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterConfigContent.GridColumn6Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "210",
                            visible: value.defaultVisibleInGrid,
                            headerCellTemplate: "../app/matter/MatterTemplates/ProjectIDHeaderTemplate.html",
                            position: value.position
                        });
                    }
                }
                if (key == "matterGuid" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterConfigContent.GridColumn15Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "210",
                            visible: value.defaultVisibleInGrid,
                            position: value.position
                        });

                    }
                }

            });
            function getSortFunction(fieldName) {
                return function (col1, col2) {
                    return parseInt(col1[fieldName]) - parseInt(col2[fieldName]);
                }
            }
            columnDefs1.sort(getSortFunction("position"));

            //#region Setting the options for grid

            vm.gridOptions = {
                //paginationPageSizes: [10, 50, 100],
                //paginationPageSize: 10,
                infiniteScrollDown: true,
                enableHorizontalScrollbar: 0,
                enableVerticalScrollbar: 1,
                enableGridMenu: true,
                enableRowHeaderSelection: false,
                enableRowSelection: true,
                enableSelectAll: false,
                multiSelect: false,
                virtualizationThreshold: vm.searchResultsLength,
                columnDefs: columnDefs1,
                enableColumnMenus: false,
                onRegisterApi: function (gridApi) {
                    $scope.gridApi = gridApi;
                    gridApi.core.on.columnVisibilityChanged($scope, function (changedColumn) {
                        $scope.columnChanged = { name: changedColumn.colDef.name, visible: changedColumn.colDef.visible };
                    });
                    gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                        //vm.selectedRow = row.entity
                        vm.selectedRow.matterName = row.entity.matterName
                        vm.selectedRow.matterClientUrl = row.entity.matterClientUrl
                        vm.selectedRow.matterGuid = row.entity.matterGuid
                    });
                    $animate.enabled(gridApi.grid.element, false);
                    $scope.gridApi.core.on.sortChanged($scope, $scope.sortChanged);
                    $scope.sortChanged($scope.gridApi.grid, [vm.gridOptions.columnDefs[1]]);
                    $scope.$watch('gridApi.grid.isScrollingVertically', vm.watchFuncscroll);
                    gridApi.infiniteScroll.on.needLoadMoreData($scope, vm.watchFunc);
                    vm.setColumns();
                }
            };

            //#endregion

            vm.watchFuncscroll = function () {
                //var element = 0;
                ////var topelement = angular.element('.ui-grid-viewport').scrollTop();
                //angular.element('.popcontent').css("display","none");
                //if (!$scope.$$phase) {
                //    $scope.$apply();
                //}
            }

            //#region for setting the classes for ui-grid based on size
            vm.setColumns = function () {
                if ($window.innerWidth < 360) {
                    $interval(function () {
                        angular.element('#mattergrid .ui-grid-viewport').addClass('viewport');
                        angular.element('#mattergrid .ui-grid-viewport').removeClass('viewportlg');
                    }, 1000, 2);
                } else {
                    $interval(function () {
                        angular.element('#mattergrid .ui-grid-viewport').removeClass('viewport');
                        angular.element('#mattergrid .ui-grid-viewport').addClass('viewportlg');
                    }, 1000, 2);
                }
            }
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
                            vm.lazyloader = true;
                            vm.gridOptions.data = vm.gridOptions.data.concat(response);
                        }
                        promise.resolve();
                        $scope.gridApi.infiniteScroll.dataLoaded();
                    });
                } else {
                    vm.lazyloader = true;
                }
                return promise.promise;
            }
            //#endregion


            //#endregion

            //#region Setting the api calls 
            //search api call 
            function get(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'get',
                    data: options,
                    success: callback
                });
            }


            function getPinnedMatters(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getPinnedMatters',
                    data: options,
                    success: callback
                });
            }


            //Callback function for pin 
            function PinMatters(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'PinMatters',
                    data: options,
                    success: callback
                });
            }


            //Callback function for unpin 
            function UnpinMatters(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'UnpinMatters',
                    data: options,
                    success: callback
                });
            }


            //#region Code for Upload functionality
            vm.docUpLoadSuccess = false;
            vm.mailUpLoadSuccess = false;
            vm.loadingAttachments = false;
            vm.IsDupliacteDocument = false;
            vm.IsNonIdenticalContent = false;
            vm.showLoading = false;
            //Callback function for folder hierarchy 
            function getFolderHierarchy(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getFolderHierarchy',
                    data: options,
                    success: callback
                });
            }

            vm.getFolderHierarchy = function (matterName, matterUrl, matterGUID) {

                if ((matterName && matterName !== "") && (matterUrl && matterUrl !== "") && (matterGUID && matterGUID !== "")) {

                    vm.selectedRow.matterName = matterName;
                    vm.selectedRow.matterClientUrl = matterUrl;
                    vm.selectedRow.matterGuid = matterGUID;
                }


                vm.allAttachmentDetails = [];
                var matterData = {
                    MatterName: vm.selectedRow.matterName,
                    MatterUrl: vm.selectedRow.matterClientUrl
                };
                vm.getContentCheckConfigurations(vm.selectedRow.matterClientUrl);
                getFolderHierarchy(matterData, function (response) {
                    vm.foldersList = response.foldersList;
                    vm.uploadedFiles = [];

                    function getNestedChildren(arr, parent) {
                        var parentList = []
                        for (var i in arr) {
                            if (arr[i].parentURL == parent) {
                                var children = getNestedChildren(arr, arr[i].url)

                                if (children.length) {
                                    arr[i].children = children;
                                    arr[i].active = parent == null ? true : false;
                                }

                                parentList.push(arr[i]);

                            }
                        }
                        return parentList
                    }

                    vm.foldersList = getNestedChildren(vm.foldersList, null);
                    if (vm.foldersList[0] !== null) { vm.showSelectedFolderTree(vm.foldersList[0]); }

                    jQuery('#UploadMatterModal').modal("show");
                    //Initialize Officejs library                     
                    //Office.initialize = function (reason) {
                    //     vm.initOutlook();
                    //};
                    vm.initOutlook();
                    vm.lazyloader = true;
                });
            }

            //#region drop method will handle the file upload scenario for both email and attachment

            //Helper method which will handle mail or doc upload. This method will be called from inside vm.handleDrop
            function mailOrDocUpload(targetDrop, sourceFile, isOverwrite, performContentCheck, draggedFile, sOperation) {
                vm.isLoadingFromDesktopStarted = true;
                var attachments = [];
                var attachmentsArray = {};
                var mailId = '';

                if (sourceFile.isEmail && sourceFile.isEmail === "true") {

                    attachments = [];
                    mailId = Office.context.mailbox.item.itemId;
                    for (var iCounter = 0; iCounter < vm.allAttachmentDetails.length; iCounter++) {
                        attachmentsArray = {};
                        attachmentsArray.attachmentType = 0;
                        attachmentsArray.name = vm.allAttachmentDetails[iCounter].attachmentFileName;
                        attachmentsArray.isInline = false;
                        attachmentsArray.contentType = vm.allAttachmentDetails[iCounter].contentType;
                        attachmentsArray.attachmentType = vm.allAttachmentDetails[iCounter].attachmentType;
                        attachmentsArray.id = vm.allAttachmentDetails[iCounter].attachmentId;
                        attachmentsArray.size = vm.allAttachmentDetails[iCounter].size;
                        attachments.push(attachmentsArray);
                    }
                }
                else {
                    attachments = [];
                    attachmentsArray.attachmentType = 0;
                    attachmentsArray.name = sourceFile.title;
                    attachmentsArray.originalName = sourceFile.title;
                    attachmentsArray.isInline = false;
                    attachmentsArray.contentType = sourceFile.contentType;
                    attachmentsArray.id = sourceFile.attachmentId;
                    attachmentsArray.size = sourceFile.size;
                    attachments.push(attachmentsArray);
                    mailId = Office.context.mailbox.item.itemId;
                }
                var folders = [];
                folders.push(targetDrop.url);
                var attachmentRequestVM = {
                    Client: {
                        Url: vm.selectedRow.matterClientUrl
                    },
                    ServiceRequest: {
                        AttachmentToken: vm.attachmentToken,
                        FolderPath: folders,
                        EwsUrl: vm.ewsUrl,
                        DocumentLibraryName: vm.selectedRow.matterName,
                        MailId: mailId,
                        PerformContentCheck: performContentCheck,
                        Overwrite: isOverwrite,
                        Subject: vm.subject + ".eml",
                        AllowContentCheck: vm.oUploadGlobal.bAllowContentCheck,
                        Attachments: attachments
                    }
                }

                if (undefined !== sOperation && sOperation == "append") {
                    var date = new Date();
                    date = date.toISOString();
                    var reg = new RegExp(":", "g");
                    date = date.replace(reg, "_").replace(".", "_");
                    if (sourceFile.isEmail && sourceFile.isEmail === "true") {
                        var subject = vm.subject + ".eml";
                        var subjectNameWithoutExt = subject.substring(0, subject.lastIndexOf("."));
                        var extMail = subject.substr(subject.lastIndexOf(".") + 1);
                        attachmentRequestVM.ServiceRequest.Subject = subjectNameWithoutExt + "_" + date + "." + extMail;
                    }
                    else {

                        for (var attachment in attachments) {
                            var fileNameWithExt = attachments[attachment].name;
                            if (-1 !== fileNameWithExt.lastIndexOf(".")) {
                                var fileNameWithoutExt = fileNameWithExt.substring(0, fileNameWithExt.lastIndexOf("."));
                                var ext = fileNameWithExt.substr(fileNameWithExt.lastIndexOf(".") + 1);
                                attachments[attachment].name = fileNameWithoutExt + "_" + date + "." + ext;
                            } else {
                                attachments[attachment].name = fileNameWithExt + "_" + date;
                            }
                        }
                        attachmentRequestVM.ServiceRequest.Attachments = attachments;
                    }
                }

                if (sourceFile.isEmail && sourceFile.isEmail === "true") {
                    vm.uploadEmail(attachmentRequestVM, draggedFile);
                }
                if (sourceFile.isEmail && sourceFile.isEmail === "false") {
                    vm.uploadAttachment(attachmentRequestVM, draggedFile);
                }
            }

            //This function will handle the files that has been dragged from the outlook
            vm.handleOutlookDrop = function (targetDrop, sourceFile) {
                vm.oUploadGlobal.successBanner = false;
                vm.targetDrop = targetDrop;
                vm.sourceFile = sourceFile;
                sourceFile.uploadSuccess = false;
                var isOverwrite = false;//Todo: Need to get from the config.js
                var performContentCheck = false;//Todo: Need to get from the config.js
                vm.isLoadingFromDesktopStarted = true;
                var draggedFile = $filter("filter")(vm.allAttachmentDetails, sourceFile.attachmentId)[0];
                mailOrDocUpload(targetDrop, sourceFile, isOverwrite, performContentCheck, draggedFile);
            }

            //This function will handle the files that has been dragged from the user desktop
            vm.ducplicateSourceFile = [];
            vm.handleDesktopDrop = function (targetDropUrl, sourceFiles, isOverwrite) {
                vm.oUploadGlobal.successBanner = false;
                vm.isLoadingFromDesktopStarted = true;
                // vm.files = sourceFiles.files;
                var fd = new FormData();
                fd.append('targetDropUrl', targetDropUrl);
                fd.append('folderUrl', targetDropUrl)
                fd.append('documentLibraryName', vm.selectedRow.matterName)
                fd.append('clientUrl', vm.selectedRow.matterClientUrl);
                fd.append('AllowContentCheck', vm.oUploadGlobal.bAllowContentCheck);
                var nCount = 0;
                angular.forEach(sourceFiles, function (file) {
                    fd.append('file', file);
                    fd.append("Overwrite" + nCount++, isOverwrite);
                });

                $http.post("/api/v1/document/uploadfiles", fd, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined },
                    timeout: vm.oUploadGlobal.canceler.promise
                }).then(function (response) {
                    vm.isLoadingFromDesktopStarted = false;
                    if (response.status == 200) {
                        if (response.data.length !== 0) {
                            var tempFile = [];
                            for (var i = 0; i < response.data.length; i++) {
                                if (!response.data[i].isError) {
                                    response.data[i].dropFolder = response.data[i].dropFolder == vm.selectedRow.matterGuid ? vm.selectedRow.matterName : response.data[i].dropFolder;
                                    vm.uploadedFiles.push(response.data[i]);
                                    tempFile.push(response.data[i]);
                                    vm.oUploadGlobal.successBanner = (tempFile.length == sourceFiles.length) ? true : false;
                                    vm.ducplicateSourceFile = vm.ducplicateSourceFile.filter(function (item) {
                                        return item.fileName !== response.data[i].fileName;
                                    });
                                } else {
                                    if (response.data[i].code == "DuplicateDocument" || response.data[i].code == "IdenticalContent") {
                                        vm.IsDupliacteDocument = true;
                                        if (response.data[i].value.split("|")[1]) {
                                            response.data[i].contentCheck = response.data[i].value.split("|")[1];
                                            response.data[i].saveLatestVersion = "True";
                                            response.data[i].cancel = "True";
                                            response.data[i].append = vm.overwriteConfiguration(response.data[i].fileName);
                                            response.data[i].value = response.data[i].value.split("|")[0];
                                            response.data[i].fileType = "remotefile";
                                            vm.ducplicateSourceFile.push(response.data[i]);
                                            vm.oUploadGlobal.arrFiles.push(vm.files[i]);
                                            vm.oUploadGlobal.successBanner = false;
                                        }
                                        else {
                                            var file = $filter("filter")(vm.ducplicateSourceFile, response.data[i].fileName);
                                            if (file.length > 0) {
                                                file[0].value = file[0].value + "<br/><br/>" + response.data[i].value;
                                                file[0].saveLatestVersion = "True";
                                                file[0].cancel = "True";
                                                file[0].contentCheck = "False";
                                            }
                                        }

                                    }
                                    else {
                                        vm.IsDupliacteDocument = true;
                                        response.data[i].ok = "True";
                                        response.data[i].value = "The file <b >" + response.data[i].fileName + " </b> is failed to upload";
                                        vm.ducplicateSourceFile.push(response.data[i]);
                                    }
                                }
                            }

                        }
                    } else {
                        //To Do error handling implementation
                    }
                }).catch(function (response) {
                    vm.isLoadingFromDesktopStarted = false;
                    console.error('Gists error', response.status, response.data);
                })

            }
            vm.uploadedFiles = [];

            //#endregion

            //#region Mail Upload Methods

            //Call back function when the mail gets uploaded
            vm.uploadEmail = function (attachmentRequestVM, droppedAttachedFile) {
                uploadEmail(attachmentRequestVM, function (response) {
                    vm.showLoading = false;
                    var target = vm.targetDrop;
                    var source = vm.sourceFile;
                    //If the mail upload is success
                    if (response.code === "OK" && response.value === "Attachment upload success") {

                        var subject = Office.context.mailbox.item.subject;
                        subject = subject.substring(0, subject.lastIndexOf("."));
                        vm.mailUpLoadSuccess = true;
                        vm.mailUploadedFile = subject;
                        vm.mailUploadedFolder = vm.targetDrop.name;
                        vm.isLoadingFromDesktopStarted = false;
                        droppedAttachedFile.uploadSuccess = true;
                        //removeDraggableDirective();
                        vm.oUploadGlobal.successBanner = droppedAttachedFile.uploadSuccess ? true : false;
                    }
                        //If the mail upload is not success
                    else if (response.code === "DuplicateDocument") {
                        vm.IsDupliacteDocument = true; //ToDo:Set it to false on mail upload dialog open
                        vm.IsNonIdenticalContent = false;

                        var selectedOverwriteConfiguration = configs.uploadMessages.overwrite_Config_Property.trim().toLocaleUpperCase(),
                        bAppendEnabled = false,
                        fileExtension = "undefined" !== typeof source && source.title ? source.title.trim().substring(source.title.trim().lastIndexOf(".") + 1) : "";
                        var isEmail = droppedAttachedFile.isEmail ? true : (1 === parseInt(droppedAttachedFile.attachmentType) || "eml" === fileExtension) ? true : false;
                        bAppendEnabled = attachmentEmailOverwriteConfiguration(selectedOverwriteConfiguration, isEmail);
                        response.contentCheck = response.value.split("|")[1];
                        response.value = response.value.split("|")[0];
                        response.saveLatestVersion = "True";
                        response.cancel = "True";
                        response.append = bAppendEnabled;
                        var duplicFile = response;
                        duplicFile.droppedUrl = target;
                        duplicFile.source = source;
                        duplicFile.fileType = "attacheddocument"
                        vm.ducplicateSourceFile.push(duplicFile);
                    }
                    else if (response.code === "NonIdenticalContent") {
                        vm.IsNonIdenticalContent = true; //ToDo:Set it to false on mail upload dialog open
                        vm.IsDupliacteDocument = false;
                    }
                    else if (response.code === "IdenticalContent") {
                        var dupliFile = vm.ducplicateSourceFile[0];
                        dupliFile.value = dupliFile.value + "<br/><br/>" + response.value;
                        dupliFile.saveLatestVersion = "True";
                        dupliFile.cancel = "True";
                        dupliFile.append = true;
                        dupliFile.contentCheck = "False";
                    }
                    console.log(response);
                    vm.isLoadingFromDesktopStarted = false;
                });
            }


            function attachmentEmailOverwriteConfiguration(selectedOverwriteConfiguration, isEmail) {
                var bAppendEnabled = false;
                switch (selectedOverwriteConfiguration) {
                    case "BOTH":
                        bAppendEnabled = true;
                        break;
                    case "DOCUMENT ONLY":
                        bAppendEnabled = isEmail ? false : true;
                        break;
                    default:
                        bAppendEnabled = isEmail ? true : false;
                        break;
                }
                return bAppendEnabled;

            }

            ////Call Web API method for upload mail
            function uploadEmail(attachmentRequestVM, callback) {
                api({
                    resource: 'matterResource',
                    method: 'uploadEmail',
                    data: attachmentRequestVM,
                    success: callback
                });
            }

            //#endregion

            //#region Call back function when attachment gets uploaded
            vm.uploadAttachment = function (attachmentRequestVM, droppedAttachedFile) {
                vm.oUploadGlobal.successBanner = false;
                uploadAttachment(attachmentRequestVM, function (response) {
                    vm.isLoadingFromDesktopStarted = false;
                    vm.showLoading = false;
                    vm.oUploadGlobal.iActiveUploadRequest--;
                    var target = vm.targetDrop;
                    var source = vm.sourceFile;
                    //If the upload is success
                    if (response.code === "OK" && response.value === "Attachment upload success") {
                        vm.IsDupliacteDocument = false;
                        vm.IsNonIdenticalContent = false;
                        vm.docUpLoadSuccess = true;
                        if (vm.oUploadGlobal.iActiveUploadRequest === 0) {
                            //ToDo: Remove the animated image
                        }
                        var extEmailOrMsg = vm.sourceFile.title.substr(vm.sourceFile.title.lastIndexOf(".") + 1);
                        if (extEmailOrMsg === "eml" || extEmailOrMsg === "msg") {
                            vm.docUploadedFolder = vm.sourceFile.title.substring(0, vm.sourceFile.title.lastIndexOf("."));

                        }
                        else {
                            vm.targetDrop.name = vm.targetDrop.name == vm.selectedRow.matterGuid ? vm.selectedRow.matterName : vm.targetDrop.name;

                        }
                        droppedAttachedFile.uploadedFolder = vm.targetDrop.name;
                        vm.docUploadedFolder = vm.targetDrop.name;
                        droppedAttachedFile.uploadSuccess = true;
                        console.log(droppedAttachedFile.counter);
                        vm.oUploadGlobal.successBanner = droppedAttachedFile.uploadSuccess ? true : false;

                        //removeDraggableDirective();

                    }
                        //If the attachment upload is not success
                    else if (response.code === "DuplicateDocument") {

                        vm.IsDupliacteDocument = true; //ToDo:Set it to false on mail upload dialog open
                        vm.IsNonIdenticalContent = false;
                        var selectedOverwriteConfiguration = configs.uploadMessages.overwrite_Config_Property.trim().toLocaleUpperCase(),
                        bAppendEnabled = false,
                        fileExtension = "undefined" !== typeof source && source.title ? source.title.trim().substring(source.title.trim().lastIndexOf(".") + 1) : "";
                        var isEmail = droppedAttachedFile.isEmail ? true : (1 === parseInt(droppedAttachedFile.attachmentType) || "eml" === fileExtension) ? true : false;
                        bAppendEnabled = attachmentEmailOverwriteConfiguration(selectedOverwriteConfiguration, isEmail);
                        response.contentCheck = response.value.split("|")[1];
                        response.value = response.value.split("|")[0];
                        response.saveLatestVersion = "True";
                        response.cancel = "True";
                        response.append = bAppendEnabled;
                        var duplicFile = response;
                        duplicFile.droppedUrl = target;
                        duplicFile.source = source;
                        duplicFile.fileType = "attacheddocument";
                        vm.ducplicateSourceFile.push(duplicFile);
                    }
                        //NonIdenticalContent
                    else if (response.code === "NonIdenticalContent") {
                        vm.IsNonIdenticalContent = true; //ToDo:Set it to false on mail upload dialog open
                        vm.IsDupliacteDocument = false;
                    }
                    else if (response.code === "IdenticalContent") {
                        var dupliFile = vm.ducplicateSourceFile[0];
                        dupliFile.value = dupliFile.value + "<br/><br/>" + response.value;
                        dupliFile.saveLatestVersion = "True";
                        dupliFile.cancel = "True";
                        dupliFile.contentCheck = "False";
                    }
                });
            }

            //Call Web API method for upload attachement
            function uploadAttachment(attachmentRequestVM, callback) {
                api({
                    resource: 'matterResource',
                    method: 'uploadAttachment',
                    data: attachmentRequestVM,
                    success: callback
                });
            }

            //Remove the draggable directive after successful file upload
            function removeDraggableDirective() {
                var divElement = angular.element(jQuery("#" + vm.sourceFile.id));
                //divElement.removeAttr("draggable");
            }

            //Remove the draggable directive after successful file upload
            function addDraggableDirective() {
                var divElement = angular.element(document.querySelector("#" + vm.sourceFile.id));
                divElement.removeAttr("draggable");
            }
            //#endregion



            vm.editAttachment = function (element, event) {

                //ToDo: Use Angular data binding functionality
                var editIcon = $("#" + event.target.id);
                var rowIndex = event.target.id.charAt(0);
                var saveIcon = $("#" + rowIndex + "saveIcon");
                var attachIcon = $("#" + rowIndex + "attachIcon");
                var thisAttachment = $("#" + rowIndex + "attachment");
                var thisAttachmentText = $("#" + rowIndex + "attachmentText");
                var attachmentText = thisAttachment[0].innerHTML;

                if (saveIcon.hasClass("hide")) {
                    saveIcon.removeClass("hide");
                    editIcon.addClass("hide");
                    attachIcon.addClass("hide");
                }

                if (thisAttachmentText.hasClass("hide")) {
                    thisAttachmentText.removeClass("hide");
                    thisAttachment.addClass("hide");
                    thisAttachmentText.val(attachmentText);
                }
            }

            vm.saveAttachment = function (element, event) {
                //ToDo: Use Angular data binding functionality
                var saveIcon = $("#" + event.target.id);
                var rowIndex = event.target.id.charAt(0);
                var editIcon = $("#" + rowIndex + "editIcon");
                var thisAttachment = $("#" + rowIndex + "attachment");
                var thisAttachmentText = $("#" + rowIndex + "attachmentText");
                var attachIcon = $("#" + rowIndex + "attachIcon");
                var attachmentText = thisAttachmentText[0].value.trim();
                var oldText = thisAttachment[0].innerHTML;
                if (!vm.oUploadGlobal.regularInvalidCharacter.test(attachmentText) &&
                    !vm.oUploadGlobal.regularExtraSpace.test(attachmentText) &&
                    !vm.oUploadGlobal.regularInvalidRule.test(attachmentText) &&
                    !vm.oUploadGlobal.regularStartEnd.test(attachmentText)) {
                    if (editIcon.hasClass("hide")) {
                        editIcon.removeClass("hide");
                        attachIcon.removeClass("hide");
                        saveIcon.addClass("hide");
                    }
                    if (thisAttachment.hasClass("hide")) {
                        thisAttachment.removeClass("hide");
                        thisAttachmentText.addClass("hide");
                        if ("" === attachmentText) {
                            thisAttachment.html(oldText);
                            thisAttachment.attr("title", oldText);
                        } else {
                            thisAttachment.html(attachmentText);
                            thisAttachment.attr("title", attachmentText);
                        }
                    }
                    //ToDo:$(".errorPopUp").addClass("hide");

                }
                else {
                    vm.oUploadGlobal.regularInvalidCharacter.lastIndex = 0;
                    //ToDo:showErrorNotification(thisAttachmentText, "Invalid character");
                }
            }

            //#region  Methods for Error Notifications Dialogs

            //Methods for over writing the document
            vm.overWriteDocument = function (operation) {
                if (operation === "overwrite") {
                    jQuery('#overWriteNo').hide();
                    vm.showLoading = true;
                    vm.IsDupliacteDocument = false;
                    vm.IsNonIdenticalContent = false;
                    mailOrDocUpload(vm.targetDrop, vm.sourceFile, vm.IsDupliacteDocument, vm.IsNonIdenticalContent);
                }
                else if (operation === "contentCheck") {
                    vm.showLoading = true;
                }
                else if (operation === "append") {
                    vm.showLoading = true;
                }
            }

            //Method for closing the notification dialog
            vm.closeNotificationDialog = function () {
                vm.IsDupliacteDocument = false;
                vm.IsNonIdenticalContent = false;
                vm.showLoading = false;
                jQuery('#overWriteNo').hide();
            }

            //#endregion


            vm.Openuploadmodal = function (matterName, matterUrl, matterGUID) {
                vm.lazyloader = false;
                vm.getFolderHierarchy(matterName, matterUrl, matterGUID);
                vm.oUploadGlobal.successBanner = false;
                vm.isLoadingFromDesktopStarted = false;
            }


            vm.oUploadGlobal = {
                regularInvalidCharacter: new RegExp("[\*\?\|\\\t/:\"\"'<>#{}%~&]", "g"),
                regularStartEnd: new RegExp("^[\. ]|[\. ]$", "g"),
                regularExtraSpace: new RegExp(" {2,}", "g"),
                regularInvalidRule: new RegExp("[\.]{2,}", "g"),
                oUploadParameter: [],
                sClientRelativeUrl: "",
                sFolderUrl: "",
                arrContent: [],
                arrFiles: [],
                arrOverwrite: [],
                src: [],
                iActiveUploadRequest: 0,
                oDrilldownParameter: { nCurrentLevel: 0, sCurrentParentUrl: "", sRootUrl: "" },
                sNotificationMsg: "",
                bAppendOptionEnabled: false,
                oXHR: new XMLHttpRequest(),
                bIsAbortedCC: false,
                bAllowContentCheck: false,
                canceler: $q.defer(),
                successBanner: false
            };

            vm.attachmentTokenCallbackEmailClient = function (asyncResult, userContext) {
                "use strict";
                if (asyncResult.status === "succeeded") {
                    vm.attachmentToken = asyncResult.value;
                    vm.createMailPopup();
                    vm.mailUpLoadSuccess = false;
                    vm.mailUploadedFile = null;
                    vm.mailUploadedFolder = null;;
                    vm.loadingAttachments = false;
                    $scope.$apply();
                } else {
                    //showNotification(oFindMatterConstants.Fail_Attachment_Token, "failNotification");
                }
            }

            vm.getIconSource = function (sExtension) {
                var uploadImageDocumentIcon = configs.uri.SPOsiteURL + configs.uploadMessages.uploadImageDocumentIcon;
                var iconSrc = uploadImageDocumentIcon.replace("{0}", sExtension);
                iconSrc = (-1 < configs.uploadMessages.uploadPNGIconExtensions.indexOf(sExtension)) ?
                                iconSrc.substring(0, uploadImageDocumentIcon.lastIndexOf(".") + 1) + "png" : iconSrc;
                return iconSrc;
            }

            vm.checkEmptyorWhitespace = function (input) {
                "use strict";
                if (/\S/.test(input)) {
                    return input;
                }
                return oFindMatterConstants.No_Subject_Mail;
            }



            vm.initOutlook = function () {

                vm.IsDupliacteDocument = false;
                if (Office.context && Office.context.mailbox) {
                    vm.loadingAttachments = true;
                    vm.attachmentToken = '';
                    vm.ewsUrl = Office.context.mailbox.ewsUrl;
                    vm.subject = Office.context.mailbox.item.subject;
                    vm.mailId = Office.context.mailbox.item.itemId;
                    vm.attachments = new Array();
                    var iCounter = 0;
                    if (Office.context.mailbox.item.attachments) {
                        var attachmentsLength = Office.context.mailbox.item.attachments.length;
                        for (iCounter = 0; iCounter < attachmentsLength; iCounter++) {
                            if (Office.context.mailbox.item.attachments[iCounter].hasOwnProperty("$0_0")) {
                                vm.attachments[iCounter] = JSON.parse(JSON.stringify(Office.context.mailbox.item.attachments[iCounter].$0_0));
                            }
                            else if (Office.context.mailbox.item.attachments[iCounter].hasOwnProperty("_data$p$0")) {
                                vm.attachments[iCounter] = JSON.parse(JSON.stringify(Office.context.mailbox.item.attachments[iCounter]._data$p$0));
                            }
                        }
                        Office.context.mailbox.getCallbackTokenAsync(vm.attachmentTokenCallbackEmailClient);
                    }
                }
            }


            vm.createMailPopup = function () {
                var sImageChunk = "", nIDCounter = 0;
                var attachmentName = "", mailSubject = "", sAttachmentFileName = "", bHasEML = false, attachmentType = "", sContentType = "", sExtension = "", iconSrc = "";
                vm.allAttachmentDetails = []
                var individualAttachment = {};
                //For just email
                individualAttachment.attachmentId = Office.context.mailbox.item.itemId;
                individualAttachment.counter = nIDCounter;
                console.log("mailSubject");
                //var mailSubject = checkEmptyorWhitespace(Office.context.mailbox.item.subject);
                //mailSubject = mailSubject.replace(oUploadGlobal.regularExtraSpace, "").replace(oUploadGlobal.regularInvalidCharacter, "").replace(oUploadGlobal.regularInvalidRule, ".").replace(oUploadGlobal.regularStartEnd, "");
                mailSubject = vm.checkEmptyorWhitespace(Office.context.mailbox.item.subject);
                console.log(mailSubject);
                mailSubject = mailSubject.replace(vm.oUploadGlobal.regularExtraSpace, "")
                                            .replace(vm.oUploadGlobal.regularInvalidCharacter, "")
                                            .replace(vm.oUploadGlobal.regularInvalidRule, ".")
                                            .replace(vm.oUploadGlobal.regularStartEnd, "");
                console.log(mailSubject);
                vm.subject = mailSubject;
                //Office.context.mailbox.item.subject=mailSubject;
                individualAttachment.attachmentFileName = mailSubject;
                individualAttachment.isEmail = true;
                individualAttachment.uploadSuccess = false;
                vm.allAttachmentDetails.push(individualAttachment);
                //For all attachments in the current email
                for (var attachment in vm.attachments) {
                    individualAttachment = {};
                    bHasEML = false;
                    nIDCounter++;
                    attachmentName = vm.checkEmptyorWhitespace(vm.attachments[attachment].name);
                    attachmentName = attachmentName.replace(vm.oUploadGlobal.regularExtraSpace, "")
                                                .replace(vm.oUploadGlobal.regularInvalidCharacter, "")
                                                .replace(vm.oUploadGlobal.regularInvalidRule, ".")
                                                .replace(vm.oUploadGlobal.regularStartEnd, "");
                    if (attachmentName.lastIndexOf(".eml") === attachmentName.length - 4) {
                        sAttachmentFileName = attachmentName.substring(0, attachmentName.lastIndexOf(".eml"));
                        bHasEML = true;
                    } else {
                        sAttachmentFileName = attachmentName;
                    }

                    var attachmentType = vm.attachments[attachment].hasOwnProperty("attachmentType") ? vm.attachments[attachment].attachmentType : "";
                    var sContentType = vm.attachments[attachment].hasOwnProperty("contentType") ? vm.attachments[attachment].contentType : "";
                    var sExtension = -1 < attachmentName.lastIndexOf(".") ? attachmentName.substring(attachmentName.lastIndexOf(".") + 1) : 1 === parseInt(attachmentType) ? "msg" : "";
                    var iconSrc = vm.getIconSource(sExtension);
                    //if (-1 < sContentType.indexOf(configs.Upload.ImageContentType)) {
                    //}
                    //else{

                    //}
                    individualAttachment.contentType = sContentType;
                    individualAttachment.attachmentId = vm.attachments[attachment].id;
                    individualAttachment.counter = nIDCounter;
                    individualAttachment.attachmentFileName = sAttachmentFileName;
                    individualAttachment.bHasEML = bHasEML;
                    individualAttachment.attachmentType = attachmentType;
                    individualAttachment.iconSrc = iconSrc;
                    individualAttachment.extension = sExtension;
                    individualAttachment.isEmail = false;
                    individualAttachment.uploadSuccess = false;
                    individualAttachment.uploadedFolder = null;
                    individualAttachment.size = vm.attachments[attachment].size;
                    individualAttachment.attachmentType = attachmentType;
                    vm.allAttachmentDetails.push(individualAttachment);

                }


            }
            //#endregion

            //#region Request object for the GetMattersMethod
            var searchRequest = {
                Client: {
                    Url: configs.global.repositoryUrl
                },
                SearchObject: {
                    PageNumber: 1,
                    ItemsPerPage: vm.searchResultsLength,
                    SearchTerm: "",
                    IsUnique: false,
                    UniqueColumnName: '',
                    FilterValue: '',
                    Filters: {
                        AOLList: [],
                        ClientName: "",
                        ClientsList: [],
                        PGList: [],
                        DateFilters: {
                            CreatedFromDate: "", CreatedToDate: "", ModifiedFromDate: "", ModifiedToDate: "", OpenDateFrom: "", OpenDateTo: ""
                        },
                        DocumentAuthor: "",
                        DocumentCheckoutUsers: "",
                        FilterByMe: 1,
                        FromDate: "",
                        Name: "",                        
                        ResponsibleAttorneys: "",
                        SubareaOfLaw: "",
                        ToDate: "",
                        ProjectName: "",
                        ProjectID: "",
                        PracticeGroup: "",
                        AreaOfLaw: ""
                    },
                    Sort:
                            {
                                ByProperty: "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "",
                                ByColumn: "ModifiedFromDate",
                                Direction: 1
                            }
                }
            }

            //#endregion
            vm.filterSearch = function (val) {
                if (val.length > 3) {

                    searchRequest.SearchObject.IsUnique = true;
                    searchRequest.SearchObject.FilterValue = val;
                    if (vm.searchexp == vm.configSearchContent.ManagedPropertyMatterName) {
                        searchRequest.SearchObject.UniqueColumnName = vm.configSearchContent.ManagedPropertyMatterName
                        vm.mattersearch("" + vm.configSearchContent.ManagedPropertyMatterName + ":" + val + "*(" + vm.configSearchContent.ManagedPropertyMatterName + ":* OR " + vm.configSearchContent.ManagedPropertyMatterId + ":* OR " + vm.configSearchContent.ManagedPropertyClientName + ":*)", vm.searchexp, false);
                    }
                    else if (vm.searchexp == vm.configSearchContent.ManagedPropertyClientName && !vm.globalSettings.isBackwardCompatible) {
                        searchRequest.SearchObject.UniqueColumnName = vm.configSearchContent.ManagedPropertyClientName
                        vm.mattersearch("" + vm.configSearchContent.ManagedPropertyClientName + ":" + vm.clientSearchTerm + "*(" + vm.configSearchContent.ManagedPropertyMatterName + ":* OR " + vm.configSearchContent.ManagedPropertyMatterId + ":* OR " + vm.configSearchContent.ManagedPropertyClientName + ":*)", vm.searchexp, false);
                    }
                    else if (vm.searchexp == vm.configSearchContent.ManagedPropertyPracticeGroup) {
                        searchRequest.SearchObject.UniqueColumnName = vm.configSearchContent.ManagedPropertyPracticeGroup
                        vm.mattersearch("" + vm.configSearchContent.ManagedPropertyPracticeGroup + ":" + vm.practiceGroupSearchTerm + "*(" + vm.configSearchContent.ManagedPropertyMatterName + ":* OR " + vm.configSearchContent.ManagedPropertyMatterId + ":* OR " + vm.configSearchContent.ManagedPropertyClientName + ":*)", vm.searchexp, false);
                    }
                    else if (vm.searchexp == vm.configSearchContent.ManagedPropertyResponsibleAttorney) {
                        searchRequest.SearchObject.UniqueColumnName = vm.configSearchContent.ManagedPropertyResponsibleAttorney
                        vm.mattersearch("" + vm.configSearchContent.ManagedPropertyResponsibleAttorney + ":" + vm.attorneySearchTerm + "*(" + vm.configSearchContent.ManagedPropertyMatterName + ":* OR " + vm.configSearchContent.ManagedPropertyMatterId + ":* OR " + vm.configSearchContent.ManagedPropertyClientName + ":*)", vm.searchexp, false);
                    }
                    else if (vm.searchexp == vm.configSearchContent.ManagedPropertySubAreaOfLaw) {
                        searchRequest.SearchObject.UniqueColumnName = vm.configSearchContent.ManagedPropertySubAreaOfLaw
                        vm.mattersearch("" + vm.configSearchContent.ManagedPropertySubAreaOfLaw + ":" + vm.subAreaOfLawSearchTerm + "*(" + vm.configSearchContent.ManagedPropertyMatterName + ":* OR " + vm.configSearchContent.ManagedPropertyMatterId + ":* OR " + vm.configSearchContent.ManagedPropertyClientName + ":*)", vm.searchexp, false);
                    }
                    else if (vm.searchexp == vm.configSearchContent.ManagedPropertyAreaOfLaw) {
                        searchRequest.SearchObject.UniqueColumnName = vm.configSearchContent.ManagedPropertyAreaOfLaw
                        vm.mattersearch("" + vm.configSearchContent.ManagedPropertyAreaOfLaw + ":" + vm.areaOfLawSearchTerm + "*(" + vm.configSearchContent.ManagedPropertyMatterName + ":* OR " + vm.configSearchContent.ManagedPropertyMatterId + ":* OR " + vm.configSearchContent.ManagedPropertyClientName + ":*)", vm.searchexp, false);
                    }
                    else if (vm.searchexp == vm.configSearchContent.ManagedPropertyMatterId) {
                        searchRequest.SearchObject.UniqueColumnName = vm.configSearchContent.ManagedPropertyMatterId
                        vm.mattersearch("" + vm.configSearchContent.ManagedPropertyMatterId + ":" + vm.projectIDSearchTerm + "*(" + vm.configSearchContent.ManagedPropertyMatterName + ":* OR " + vm.configSearchContent.ManagedPropertyMatterId + ":* OR " + vm.configSearchContent.ManagedPropertyClientName + ":*)", vm.searchexp, false);
                    }
                }
            }

            //#region For filtering the grid when clicked on search button
            vm.searchMatter = function (val) {
                var finalSearchText = "";
                if (val != "") {
                    finalSearchText = "(" + vm.configSearchContent.ManagedPropertyMatterName + ":" + val + "* OR " + vm.configSearchContent.ManagedPropertyMatterId + ":" + val + "*)";
                }
                var searchMatterRequest = {
                    Client: {
                        Url: configs.global.repositoryUrl
                    },
                    SearchObject: {
                        PageNumber: 1,
                        ItemsPerPage: 5,
                        SearchTerm: finalSearchText,
                        Filters: {
                            AOLList: [],
                            ClientName: "",
                            ClientsList: [],
                            DateFilters: {
                                CreatedFromDate: "", CreatedToDate: "", ModifiedFromDate: "", ModifiedToDate: "", OpenDateFrom: "", OpenDateTo: ""
                            },
                            DocumentAuthor: "",
                            DocumentCheckoutUsers: "",
                            FilterByMe: 1,
                            FromDate: "",
                            Name: "",
                            PGList: [],
                            ResponsibleAttorneys: "",
                            SubareaOfLaw: "",
                            ToDate: ""
                        },
                        Sort:
                                {
                                    ByProperty: "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "",
                                    Direction: 1,
                                    ByColumn: "ModifiedFromDate",
                                }
                    }
                }
                return matterResource.get(searchMatterRequest).$promise;
            }

            vm.search = function () {
                vm.lazyloader = false;
                vm.divuigrid = false;
                vm.nodata = false;
                searchRequest.SearchObject.ItemsPerPage = vm.searchResultsLength;
                if (vm.matterid == 3) {
                    vm.matterid = 1;
                    vm.mattername = "" + vm.matterConfigContent.Dropdown1Item1 + "";
                }
                vm.pagenumber = 1;
                var searchToText = '';
                var finalSearchText = '';
                if (vm.selected != "") {
                    if (vm.selected.indexOf("(") > -1) {
                        searchToText = vm.selected.replace("(", ",")
                        searchToText = searchToText.replace(")", "")
                        var firstText = searchToText.split(',')[0]
                        var secondText = searchToText.split(',')[1]
                        finalSearchText = '(' + vm.configSearchContent.ManagedPropertyMatterName + ':"' + firstText.trim() + '" AND ' + vm.configSearchContent.ManagedPropertyMatterId + ':"' + secondText.trim() + '")';
                    } else {
                        finalSearchText = commonFunctions.searchFilter(vm.selected);
                    }
                }
                searchRequest.SearchObject.SearchTerm = finalSearchText;
                searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                searchRequest.SearchObject.Sort.Direction = 1;
                get(searchRequest, function (response) {
                    if (response == "" || response.errorCode == "500" || response.lenth == 0) {
                        vm.gridOptions.data = response;
                        vm.lazyloader = true;
                        vm.divuigrid = true;
                        vm.nodata = true;
                        $interval(function () { vm.showSortExp(); }, 2000, 3);
                    } else {
                        vm.gridOptions.data = response;
                        vm.divuigrid = true;
                        vm.nodata = false;
                        vm.lazyloader = true;
                        $interval(function () { vm.showSortExp(); }, 2000, 3);
                    }
                });
            }
            //#endregion


            //#region for setting the mattername in dropdown
            vm.SetMatters = function (id, name) {
                vm.clearAllFilter();
                vm.mattername = name;
                vm.GetMatters(id);
                vm.matterid = id;
            }

            //#endregion
            //#region for searching matter by property and searchterm
            vm.mattersearch = function (term, property, bool) {
                vm.lazyloaderFilter = false;
                vm.filternodata = false;
                searchRequest.SearchObject.PageNumber = 1;
                searchRequest.SearchObject.SearchTerm = term;
                searchRequest.SearchObject.Sort.ByProperty = property;
                searchRequest.SearchObject.Sort.Direction = 0;
                if (bool) {
                    vm.matterheader = true;
                    vm.divuigrid = false;
                    searchRequest.SearchObject.SearchTerm = "";
                    searchRequest.SearchObject.ItemsPerPage = vm.searchResultsLength;
                    if (property == "" + vm.configSearchContent.ManagedPropertyResponsibleAttorney + "") {
                        vm.attorneyproperty = term;
                        searchRequest.SearchObject.Filters.ResponsibleAttorneys = term;
                        searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                        vm.attorneyfilter = true;
                    }
                    else if (property == "" + vm.configSearchContent.ManagedPropertySubAreaOfLaw + "") {
                        searchRequest.SearchObject.Filters.SubareaOfLaw = term.trim();
                        searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                        if (!vm.globalSettings.isBackwardCompatible) {
                            vm.areafilter = true;
                        } else {
                            vm.subareafilter = true;
                        }
                    }
                    else if (property == "" + vm.configSearchContent.ManagedPropertyMatterName + "") {
                        searchRequest.SearchObject.Filters.Name = term;
                        searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                        vm.matterfilter = true;
                    }
                    else if (property == "" + vm.configSearchContent.ManagedPropertyClientName  + "" && !vm.globalSettings.isBackwardCompatible) {
                        searchRequest.SearchObject.Filters.ClientName = term;
                        searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                        vm.clientfilter = true;
                        vm.areaoflawfilter = true
                    }
                    else if (property == "" + vm.configSearchContent.ManagedPropertyAreaOfLaw + "" && vm.globalSettings.isBackwardCompatible) {
                        searchRequest.SearchObject.Filters.AreaOfLaw = term.trim();
                        searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                        vm.areaoflawfilter = true;
                    }
                    else if (property == "" + vm.configSearchContent.ManagedPropertyPracticeGroup + "") {
                        searchRequest.SearchObject.Filters.PracticeGroup = term.trim();
                        searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                        vm.practiceGroupfilter = true;
                    }
                    else if (property == "" + vm.configSearchContent.ManagedPropertyMatterId + "") {
                        searchRequest.SearchObject.Filters.ProjectID = term.trim();
                        searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                        vm.projectIDfilter = true;
                    }
                } else {
                    searchRequest.SearchObject.ItemsPerPage = 50;
                    if (property == "" + vm.configSearchContent.ManagedPropertyResponsibleAttorney + "") {
                        searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyResponsibleAttorney + "";
                        searchRequest.SearchObject.Sort.Direction = 0;
                    } else if (property == "" + vm.configSearchContent.ManagedPropertySubAreaOfLaw + "") {
                        searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertySubAreaOfLaw + "";
                        searchRequest.SearchObject.Sort.Direction = 0;
                    } else if (property == "" + vm.configSearchContent.ManagedPropertyAreaOfLaw + "") {
                        searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyAreaOfLaw + "";
                        searchRequest.SearchObject.Sort.Direction = 0;
                    } else if (property == "" + vm.configSearchContent.ManagedPropertyPracticeGroup + "") {
                        searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyPracticeGroup + "";
                        searchRequest.SearchObject.Sort.Direction = 0;
                    } else if (property == "" + vm.configSearchContent.ManagedPropertyMatterId + "") {
                        searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyMatterId + "";
                        searchRequest.SearchObject.Sort.Direction = 0;
                    } else {
                        searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                        searchRequest.SearchObject.Sort.Direction = 1;
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
                            searchRequest.SearchObject.IsUnique = false;
                            searchRequest.SearchObject.FilterValue = '';
                            searchRequest.SearchObject.UniqueColumnName = '';
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
                            if (!$scope.$$phase) {
                                $scope.$apply();
                            }
                        } else {
                            vm.details = response;
                            vm.filternodata = false;
                            searchRequest.SearchObject.IsUnique = false;
                            searchRequest.SearchObject.FilterValue = '';
                            searchRequest.SearchObject.UniqueColumnName = '';
                        }
                        searchRequest.SearchObject.SearchTerm = "";
                        searchRequest.SearchObject.Sort.ByProperty = "";
                        $interval(function () { vm.showSortExp(); }, 2000, 3);
                    }
                });
            }
            //#endregion


            //#region Code for filtering ModifiedDate
            //start
            vm.FilterModifiedDate = function (name) {
                vm.lazyloader = false;
                vm.divuigrid = false;
                searchRequest.SearchObject.PageNumber = 1;
                searchRequest.SearchObject.SearchTerm = "";
                if (name == "Modified Date") {
                    searchRequest.SearchObject.Filters.DateFilters.ModifiedFromDate = $filter('date')(vm.modstartdate, "yyyy-MM-ddT00:00:00") + "Z";
                    searchRequest.SearchObject.Filters.DateFilters.ModifiedToDate = $filter('date')(vm.modenddate, "yyyy-MM-ddT23:59:59") + "Z";
                    vm.moddatefilter = true;
                }
                if (name == "Open Date" || name == "Created Date") {
                    searchRequest.SearchObject.Filters.DateFilters.OpenDateFrom = $filter('date')(vm.startDate, "yyyy-MM-ddT00:00:00") + "Z";
                    searchRequest.SearchObject.Filters.DateFilters.OpenDateTo = $filter('date')(vm.endDate, "yyyy-MM-ddT23:59:59") + "Z";
                    vm.opendatefilter = true;
                }
                searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                searchRequest.SearchObject.Sort.Direction = 1;
                get(searchRequest, function (response) {
                    if (response == "") {
                        vm.gridOptions.data = response;
                        vm.lazyloader = true;
                        vm.divuigrid = true;
                        vm.nodata = true;
                        $interval(function () { vm.showSortExp(); }, 2500, 3);
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                        vm.lazyloader = true;
                        vm.gridOptions.data = response;
                        $interval(function () { vm.showSortExp(); }, 2500, 3);
                    }
                });

            }

            vm.clearAllFilter = function () {

                vm.matterdateheader = true;
                vm.matterheader = true;
                vm.lazyloader = false;
                vm.divuigrid = false;
                vm.nodata = false;
                vm.responseNull = false;
                vm.pagenumber = 1;
                searchRequest.SearchObject.SearchTerm = '';
                searchRequest.SearchObject.ItemsPerPage = vm.searchResultsLength;
                searchRequest.SearchObject.PageNumber = vm.pagenumber;
                searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                searchRequest.SearchObject.Sort.Direction = 1;
                
                searchRequest.SearchObject.Filters.ResponsibleAttorneys = "";
                vm.attorneyfilter = false;

                searchRequest.SearchObject.FilterValue = '';
                searchRequest.SearchObject.IsUnique = false;
                searchRequest.SearchObject.UniqueColumnName = '';

                vm.areaSearchTerm = "";
                searchRequest.SearchObject.Filters.SubareaOfLaw = "";
                vm.areafilter = false;

                vm.projectIDSearchTerm = "";
                searchRequest.SearchObject.Filters.ProjectID = "";
                vm.projectIDfilter = false;

                searchRequest.SearchObject.SearchTerm = "";
                searchRequest.SearchObject.Filters.Name = "";
                vm.matterfilter = false;

                vm.clientSearchTerm = ""
                searchRequest.SearchObject.Filters.ClientName = "";
                vm.clientfilter = false;
                vm.areaoflawfilter = false;

                vm.practiceGroupSearchTerm = ""
                searchRequest.SearchObject.Filters.PracticeGroup = "";               
                vm.practiceGroupfilter = false;

                searchRequest.SearchObject.Filters.DateFilters.ModifiedFromDate = "";
                searchRequest.SearchObject.Filters.DateFilters.ModifiedToDate = "";
                vm.modstartdate = "";
                vm.modenddate = "";
                vm.moddatefilter = false;
          
                searchRequest.SearchObject.Filters.SubareaOfLaw = "";
                vm.subAreaOfLawSearchTerm = "";
                vm.subareafilter = false;

                searchRequest.SearchObject.Filters.AreaOfLaw = "";
                vm.areaOfLawSearchTerm = "";
                vm.areaoflawfilter = false;
                vm.areafilter = false;

                searchRequest.SearchObject.Filters.DateFilters.OpenDateFrom = "";
                searchRequest.SearchObject.Filters.DateFilters.OpenDateTo = "";
                vm.startDate = "";
                vm.endDate = "";
                vm.opendatefilter = false;
            }

            //#endregion

            //#region clearing all filters
            vm.clearFilters = function (property) {
                vm.matterdateheader = true;
                vm.matterheader = true;
                vm.lazyloader = false;
                vm.divuigrid = false;
                vm.nodata = false;
                vm.responseNull = false;
                vm.pagenumber = 1;
                searchRequest.SearchObject.SearchTerm = '';
                searchRequest.SearchObject.ItemsPerPage = vm.searchResultsLength;
                searchRequest.SearchObject.PageNumber = vm.pagenumber;
                searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                searchRequest.SearchObject.Sort.Direction = 1;
                if (property == vm.matterConfigContent.GridColumn5Header) {
                    if (!vm.globalSettings.isBackwardCompatible) {
                        vm.attorneySearchTerm = "";
                        searchRequest.SearchObject.Filters.ResponsibleAttorneys = "";
                        vm.attorneyfilter = false;
                    }
                    else {
                        searchRequest.SearchObject.Filters.DateFilters.OpenDateFrom = "";
                        searchRequest.SearchObject.Filters.DateFilters.OpenDateTo = "";
                        vm.startDate = "";
                        vm.endDate = "";
                        vm.opendatefilter = false;
                    }
                }
                else if (property == vm.matterConfigContent.GridColumn6Header && !vm.globalSettings.isBackwardCompatible) {
                    vm.areaSearchTerm = "";
                    searchRequest.SearchObject.Filters.SubareaOfLaw = "";
                    vm.areafilter = false;
                }
                else if (property == vm.matterConfigContent.GridColumn6Header && vm.globalSettings.isBackwardCompatible) {
                    vm.projectIDSearchTerm = "";
                    searchRequest.SearchObject.Filters.ProjectID = "";
                    vm.projectIDfilter = false;
                }
                else if (property == vm.matterConfigContent.GridColumn1Header) {
                    vm.searchTerm = "";
                    searchRequest.SearchObject.SearchTerm = "";
                    searchRequest.SearchObject.Filters.Name = "";
                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                    vm.matterfilter = false;
                }
                else if (property == vm.matterConfigContent.GridColumn2Header && !vm.globalSettings.isBackwardCompatible) {
                    vm.clientSearchTerm = ""
                    searchRequest.SearchObject.Filters.ClientName = "";
                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                    vm.clientfilter = false;
                    vm.areaoflawfilter = false;
                }
                else if (property == vm.matterConfigContent.GridColumn2Header && vm.globalSettings.isBackwardCompatible) {
                    vm.practiceGroupSearchTerm = ""
                    searchRequest.SearchObject.Filters.PracticeGroup = "";
                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                    vm.practiceGroupfilter = false;
                }
                else if (property == vm.matterConfigContent.GridColumn4Header) {
                    if (!vm.globalSettings.isBackwardCompatible) {
                        searchRequest.SearchObject.Filters.DateFilters.ModifiedFromDate = "";
                        searchRequest.SearchObject.Filters.DateFilters.ModifiedToDate = "";
                        vm.modstartdate = "";
                        vm.modenddate = "";
                        vm.moddatefilter = false;
                    } else {
                        searchRequest.SearchObject.Filters.SubareaOfLaw = "";
                        vm.subAreaOfLawSearchTerm = "";
                        searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                        vm.subareafilter = false;
                    }
                }
                else if (property == vm.matterConfigContent.GridColumn3Header) {
                    searchRequest.SearchObject.Filters.AreaOfLaw = "";
                    vm.areaOfLawSearchTerm = "";
                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                    vm.areaoflawfilter = false;
                    vm.areafilter = false;
                }
                else {
                    searchRequest.SearchObject.Filters.DateFilters.OpenDateFrom = "";
                    searchRequest.SearchObject.Filters.DateFilters.OpenDateTo = "";
                    vm.startDate = "";
                    vm.endDate = "";
                    vm.opendatefilter = false;
                }

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

            //#endregion

            //#region Code written for displaying types in dropdown 
            //Start 
            vm.Matters = [{ Id: 1, Name: "" + vm.matterConfigContent.Dropdown1Item1 + "" }, { Id: 2, Name: "" + vm.matterConfigContent.Dropdown1Item2 + "" }, { Id: 3, Name: "" + vm.matterConfigContent.Dropdown1Item3 + "" }];
            vm.ddlMatters = vm.Matters[1];
            //#endregion  

            //#region Hits when the Dropdown changes 
            //Start 
            vm.GetMatters = function (id) {
                vm.selected = "";
                vm.searchTerm = "";
                vm.searchClientTerm = "";
                vm.startDate = "";
                vm.endDate = "";
                //vm.sortexp = "";
                //vm.sortby = "";
                vm.lazyloader = false;
                vm.divuigrid = false;
                vm.gridOptions.data = [];
                var pinnedMattersRequest = {
                    Url: configs.global.repositoryUrl
                }
                if (id == 1) {
                    vm.pagenumber = 1;
                    vm.responseNull = false;
                    searchRequest.SearchObject.PageNumber = 1;
                    searchRequest.SearchObject.SearchTerm = "";
                    searchRequest.SearchObject.Filters.FilterByMe = 0;
                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                    searchRequest.SearchObject.Sort.ByColumn = "MatterModifiedDate";
                    get(searchRequest, function (response) {
                        if (response == "" || response.errorCode == "500") {
                            vm.gridOptions.data = response;
                            vm.lazyloader = true;
                            vm.divuigrid = true;
                            vm.nodata = true;
                        } else {
                            getPinnedMatters(searchRequest, function (pinnedResponse) {
                                if (pinnedResponse && pinnedResponse.length > 0) {
                                    angular.forEach(pinnedResponse, function (pinobj) {
                                        angular.forEach(response, function (res) {
                                            //Check if the pinned matter name is equal to search matter name
                                            if (pinobj.matterName == res.matterName) {
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
                        }
                        vm.nodata = false;
                        $timeout(function () {
                            vm.lazyloader = true;
                            vm.divuigrid = true;
                        }, 1000);
                        $interval(function () { vm.showSortExp(); }, 2000, 3);
                    });
                } else if (id == 2) {
                    vm.lazyloader = false;
                    vm.divuigrid = false;
                    vm.pagenumber = 1;
                    vm.responseNull = false;
                    searchRequest.SearchObject.PageNumber = 1;
                    searchRequest.SearchObject.SearchTerm = "";
                    searchRequest.SearchObject.Filters.FilterByMe = 1;
                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                    searchRequest.SearchObject.Sort.ByColumn = "MatterModifiedDate";
                    get(searchRequest, function (response) {
                        if (response == "" || response.errorCode == "500") {
                            vm.gridOptions.data = response;
                            vm.lazyloader = true;
                            vm.divuigrid = true;
                            vm.nodata = true;
                        } else {
                            getPinnedMatters(searchRequest, function (pinnedResponse) {
                                if (pinnedResponse && pinnedResponse.length > 0) {
                                    angular.forEach(pinnedResponse, function (pinobj) {
                                        angular.forEach(response, function (res) {
                                            //Check if the pinned matter name is equal to search matter name
                                            if (pinobj.matterName == res.matterName) {
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
                                vm.lazyloader = true;
                                vm.divuigrid = true;
                                vm.nodata = false;
                                $interval(function () { vm.showSortExp(); }, 2000, 3);
                            });
                        }
                    });
                } else if (id == 3) {
                    vm.lazyloader = false;
                    vm.divuigrid = false;
                    var pinnedMattersRequest = {
                        Url: configs.global.repositoryUrl
                    }
                    searchRequest.SearchObject.Sort.ByColumn = "MatterModifiedDate";
                    getPinnedMatters(searchRequest, function (response) {
                        if (response == "" || response.errorCode == "500") {
                            vm.gridOptions.data = response;
                            vm.lazyloader = true;
                            vm.divuigrid = true;
                            vm.nodata = true;
                        } else {
                            angular.forEach(response, function (res) {
                                if (res.ismatterdone == undefined && !res.ismatterdone) {
                                    res.MatterInfo = "Unpin this matter";
                                    res.ismatterdone = true;
                                }
                            });
                            vm.gridOptions.data = response;
                            vm.lazyloader = true;
                            vm.divuigrid = true;
                            vm.nodata = false;
                            $interval(function () { vm.showSortExp(); }, 1000, 3);
                        }
                    });
                }
            }
            //#endregion 


            //#region To run GetMatters function on page load 
            vm.SetMatters(vm.matterid, vm.mattername);
            //End 


            //#region Written for unpinning the matter 
            //Start 
            vm.UnpinMatter = function (data) {
                vm.lazyloader = false;
                vm.divuigrid = false;
                var alldata = data.entity;
                var unpinRequest = {
                    Client: {
                        Url: configs.global.repositoryUrl
                    },
                    matterData: {
                        matterName: alldata.matterUrl,
                    }
                }
                UnpinMatters(unpinRequest, function (response) {
                    if (response.isMatterUnPinned) {
                        $timeout(function () { vm.SetMatters(vm.matterid, vm.mattername); $interval(function () { vm.showSortExp(); }, 5000, 3); }, 500);
                        //alert("Success");
                    }
                });
            }
            //End 


            //#region Written for pinning the matter 
            //Start 
            vm.PinMatter = function (data) {
                vm.lazyloader = false;
                vm.divuigrid = false;
                var alldata = data.entity;
                var pinRequest = {
                    Client: {
                        Url: configs.global.repositoryUrl
                    },
                    matterData: {
                        matterName: alldata.matterName,
                        matterDescription: alldata.matterDescription,
                        matterCreatedDate: alldata.matterCreatedDate,
                        matterUrl: alldata.matterUrl,
                        matterPracticeGroup: alldata.matterPracticeGroup,
                        matterAreaOfLaw: alldata.matterAreaOfLaw,
                        matterSubAreaOfLaw: alldata.matterSubAreaOfLaw,
                        matterClientUrl: alldata.matterClientUrl,
                        matterClient: alldata.matterClient,
                        matterClientId: alldata.matterClientId,
                        hideUpload: alldata.hideUpload,
                        matterID: alldata.matterID,
                        matterResponsibleAttorney: alldata.matterResponsibleAttorney,
                        matterModifiedDate: alldata.matterModifiedDate,
                        matterGuid: alldata.matterGuid
                    }
                }
                PinMatters(pinRequest, function (response) {
                    if (response.isMatterPinned) {
                        $timeout(function () { vm.SetMatters(vm.matterid, vm.mattername); $interval(function () { vm.showSortExp(); }, 5000, 3); }, 500);
                        //alert("Success");
                    }
                });
            }
            //#endregion


            //#region To display modal up in center of the screen...
            //Start 


            vm.reposition = function () {
                var modal = $(this)

                var dialog = modal.find('.modal-dialog');
                modal.css('display', 'block');
                // Dividing by two centers the modal exactly, but dividing by three  
                // or four works better for larger screens. 
                dialog.css("margin-top", Math.max(0, (jQuery(window).height() - dialog.height()) / 2));
            }
            // Reposition when a modal is shown 
            jQuery('.modal').on('show.bs.modal', vm.reposition);
            // Reposition when the window is resized 
            jQuery(window).on('resize', function () {
                jQuery('.modal:visible').each(vm.reposition);
            });

            $timeout(vm.reposition(), 100);
            //#endregion 

            //#region For making menu visbible and hide
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
            //#endregion

            $rootScope.breadcrumb = true;
            $rootScope.foldercontent = false;

            vm.hideBreadCrumb = function () {
                $rootScope.breadcrumb = true;
                $rootScope.foldercontent = false;

            }

            //#region Angular Datepicker Starts here
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


            //Start for open date options
            vm.dateOptions = {
                formatYear: 'yy',
                maxDate: new Date()
            };


            vm.enddateOptions = {
                formatYear: 'yy',
                maxDate: new Date()
            }

            $scope.$watch('vm.startDate', function (newval, oldval) {
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
                vm.lazyloader = true;
                if (vm.matterid == 3) {
                    var pinnedMattersRequest = {
                        Url: configs.global.repositoryUrl
                    }
                    getPinnedMatters(searchRequest, function (response) {

                        if (response == "" || response.errorCode == "500") {
                            vm.gridOptions.data = response;
                            vm.divuigrid = false;
                            vm.nodata = true;
                            $scope.errorMessage = response.message;
                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                            angular.forEach(response, function (res) {
                                if (res.ismatterdone == undefined && !res.ismatterdone) {
                                    res.MatterInfo = "Unpin this matter";
                                    res.ismatterdone = true;
                                }
                            });
                            vm.gridOptions.data = response;
                            if (!$scope.$$phase) {
                                $scope.$apply();
                            }

                        }
                    });
                }
                else {
                    get(searchRequest, function (response) {
                        //  vm.lazyloader = true;
                        if (response == "" || response.errorCode == "500") {
                            vm.gridOptions.data = response;
                            vm.divuigrid = false;
                            vm.nodata = true;
                            $scope.errorMessage = response.message;
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
            }

            vm.sortby = "desc";
            vm.sortexp = "matterModifiedDate";
            vm.showSortExp = function () {
                if (vm.sortby == "asc") {
                    angular.element("#desc" + vm.sortexp).css("display", "none");
                } else {
                    angular.element("#asc" + vm.sortexp).css("display", "none");
                }
                var elm = angular.element("#" + vm.sortby + vm.sortexp);
                if (elm != undefined) {
                    elm.css("display", "block");
                }
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            }

            vm.sortby = "desc";
            vm.sortexp = "matterModifiedDate";
            $interval(function () { vm.showSortExp(); }, 3000, 3);

            $scope.sortChanged = function (grid, sortColumns) {
                vm.divuigrid = false;
                vm.responseNull = false;
                searchRequest.SearchObject.SearchTerm = "";
                if (sortColumns.length != 0 && sortColumns[0] != undefined) {
                    if (sortColumns[0].name == vm.gridOptions.columnDefs[0].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.MatterNameSort == undefined || vm.MatterNameSort == "asc") {
                                vm.pagenumber = 1;
                                vm.lazyloader = false;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyMatterName + "";
                                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                                searchRequest.SearchObject.Sort.Direction = 0;
                                vm.FilterByType();
                                vm.MatterNameSort = "desc"; vm.sortby = "asc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            } else {
                                vm.pagenumber = 1;
                                vm.lazyloader = false;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyMatterName + "";
                                searchRequest.SearchObject.Sort.Direction = 1;
                                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                                vm.FilterByType();
                                vm.MatterNameSort = "asc"; vm.sortby = "desc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }
                        } else {
                            vm.divuigrid = true;
                            //vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[1].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.ClientSort == undefined || vm.ClientSort == "asc") {
                                vm.pagenumber = 1;
                                vm.lazyloader = false;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyClientName + "";
                                searchRequest.SearchObject.Sort.Direction = 0;
                                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                                vm.FilterByType();
                                vm.ClientSort = "desc"; vm.sortby = "asc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }
                            else {
                                vm.pagenumber = 1;
                                vm.lazyloader = false;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyClientName + "";
                                searchRequest.SearchObject.Sort.Direction = 1;
                                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                                vm.FilterByType();
                                vm.ClientSort = "asc"; vm.sortby = "desc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }
                        } else {
                            vm.divuigrid = true;
                            //vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[2].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.ClientIDSort == undefined || vm.ClientIDSort == "asc") {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                if (!vm.globalSettings.isBackwardCompatible) {
                                    searchRequest.SearchObject.Sort.ByProperty = "MCClientID";
                                } else {
                                    searchRequest.SearchObject.Sort.ByProperty = vm.configSearchContent.ManagedPropertyAreaOfLaw;
                                }
                                searchRequest.SearchObject.Sort.Direction = 0;
                                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                                vm.FilterByType();
                                vm.ClientIDSort = "desc"; vm.sortby = "asc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            } else {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                if (!vm.globalSettings.isBackwardCompatible) {
                                    searchRequest.SearchObject.Sort.ByProperty = "MCClientID";
                                } else {
                                    searchRequest.SearchObject.Sort.ByProperty = vm.configSearchContent.ManagedPropertyAreaOfLaw;
                                }
                                searchRequest.SearchObject.Sort.Direction = 1;
                                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                                vm.FilterByType();
                                vm.ClientIDSort = "asc"; vm.sortby = "desc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }

                        } else {
                            vm.divuigrid = true;
                            //vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[3].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.ModiFiedTimeSort == undefined || vm.ModiFiedTimeSort == "asc") {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                if (!vm.globalSettings.isBackwardCompatible) {
                                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                                } else {
                                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertySubAreaOfLaw + "";
                                }
                                searchRequest.SearchObject.Sort.Direction = 0;
                                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                                vm.FilterByType();
                                vm.ModiFiedTimeSort = "desc"; vm.sortby = "asc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            } else {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                if (!vm.globalSettings.isBackwardCompatible) {
                                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyLastModifiedTime + "";
                                } else {
                                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertySubAreaOfLaw + "";
                                }
                                searchRequest.SearchObject.Sort.Direction = 1;
                                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                                vm.FilterByType();
                                vm.ModiFiedTimeSort = "asc"; vm.sortby = "desc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }

                        } else {
                            vm.divuigrid = true;
                            //vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[4].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.ResAttoSort == undefined || vm.ResAttoSort == "asc") {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                if (!vm.globalSettings.isBackwardCompatible) {
                                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyResponsibleAttorney + "";
                                } else {
                                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyOpenDate + "";
                                }
                                searchRequest.SearchObject.Sort.Direction = 0;
                                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                                vm.FilterByType();
                                vm.ResAttoSort = "desc"; vm.sortby = "asc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            } else {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                if (!vm.globalSettings.isBackwardCompatible) {
                                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyResponsibleAttorney + "";
                                } else {
                                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyOpenDate + "";
                                }
                                searchRequest.SearchObject.Sort.Direction = 1;
                                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                                vm.FilterByType();
                                vm.ResAttoSort = "asc"; vm.sortby = "desc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1500, 3);
                            }
                        } else {
                            vm.divuigrid = true;
                            //vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[5].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.SubAreaSort == undefined || vm.SubAreaSort == "asc") {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertySubAreaOfLaw + "";
                                searchRequest.SearchObject.Sort.Direction = 0;
                                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                                vm.FilterByType();
                                vm.SubAreaSort = "desc"; vm.sortby = "asc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            } else {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertySubAreaOfLaw + "";
                                searchRequest.SearchObject.Sort.Direction = 1;
                                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                                vm.FilterByType();
                                vm.SubAreaSort = "asc"; vm.sortby = "desc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }
                        } else {
                            vm.divuigrid = true;
                            //vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[6].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.OpenDateSort == undefined || vm.OpenDateSort == "asc") {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyOpenDate + "";
                                searchRequest.SearchObject.Sort.Direction = 0;
                                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                                vm.FilterByType();
                                vm.OpenDateSort = "desc"; vm.sortby = "asc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            } else {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyOpenDate + "";
                                searchRequest.SearchObject.Sort.Direction = 1;
                                searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                                vm.FilterByType();
                                vm.OpenDateSort = "asc"; vm.sortby = "desc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }

                        } else {
                            vm.divuigrid = true;
                            //vm.nodata = false;
                        }
                    }
                } else {

                    vm.pagenumber = 1;
                    vm.lazyloader = false;
                    searchRequest.SearchObject.PageNumber = 1;
                    searchRequest.SearchObject.Sort.ByProperty = "" + vm.configSearchContent.ManagedPropertyMatterName + "";
                    searchRequest.SearchObject.Sort.Direction = 1;
                    searchRequest.SearchObject.Sort.ByColumn = sortColumns[0].name;
                    vm.FilterByType();
                    vm.MatterNameSort = "asc"; vm.sortby = "desc";
                    vm.sortexp = "matterName";
                    $interval(function () { vm.showSortExp(); }, 1200, 3);

                }
            }
            //#endregion

            //#region setting the grid options when window is resized

            angular.element($window).bind('resize', function () {
                angular.element('#mattergrid .ui-grid').css('height', $window.innerHeight - 115);
                if ($window.innerWidth < 360) {
                    angular.element('#mattergrid .ui-grid-viewport').addClass('viewport');
                    angular.element('#mattergrid .ui-grid-viewport').removeClass('viewportlg');
                    angular.element('.ui-grid-menu-mid').css('height', $window.innerHeight - 300 + 'px !important');
                } else {
                    angular.element('#mattergrid .ui-grid-viewport').removeClass('viewport');
                    angular.element('#mattergrid .ui-grid-viewport').addClass('viewportlg');
                    angular.element('.ui-grid-menu-mid').css('height', $window.innerHeight - 300 + 'px !important');
                }
                if (vm.nodata && $window.innerHeight < 375) {
                    angular.element('#mattergrid .ui-grid-viewport').removeClass('viewportlg');
                    angular.element('#mattergrid .ui-grid-viewport').css("overflow-x","hidden!important");
                } else {
                    angular.element('#mattergrid .ui-grid-viewport').css("overflow-x", "auto!important");
                    angular.element('#mattergrid .ui-grid-viewport').addClass('viewportlg');
                }
            });

            //#endregion

            //#region
            vm.typeheadselect = function (index, selected) {
                if (vm.matterid == 3) {
                    vm.matterid = 1;
                    vm.mattername = "" + vm.matterConfigContent.Dropdown1Item1 + "";
                }
                var searchToText = '';
                var finalSearchText = '';
                if (selected != "") {
                    searchToText = selected.replace("(", ",")
                    searchToText = searchToText.replace(")", "")
                    var firstText = searchToText.split(',')[0]
                    var secondText = searchToText.split(',')[1]
                    var finalSearchText = '(' + vm.configSearchContent.ManagedPropertyMatterName + ':"' + firstText.trim() + '" AND ' + vm.configSearchContent.ManagedPropertyMatterId + ':"' + secondText.trim() + '")'
                }
                searchRequest.SearchObject.SearchTerm = finalSearchText;
                searchRequest.SearchObject.Sort.Direction = 0;
                vm.FilterByType();
            }

            //#endregion

            //#region for showing the matters dropdown in resposive 
            vm.showmatterdrop = function ($event) {
                $event.stopPropagation();
                if (vm.mattersdropinner) {
                    vm.mattersdrop = true;
                    vm.mattersdropinner = false;
                } else {
                    vm.mattersdrop = false;
                    vm.mattersdropinner = true;
                }
            }
            //#endregion

            //#region for closing all the dropdowns
            vm.closealldrops = function () {
                vm.mattersdrop = false;
                vm.mattersdropinner = true;
                vm.matterheader = true;
                vm.matterdateheader = true;
                angular.element('.ui-grid-icon-menu').addClass('showExpandIcon');
                angular.element('.ui-grid-icon-menu').removeClass('closeColumnPicker');
            }

            //#endregion

            //#region To getContentCheckConfigurations
            //start

            function getContentCheckConfigurations(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getDefaultMatterConfigurations',
                    data: options,
                    success: callback
                });
            }
            vm.getContentCheckConfigurations = function (siteCollectionPath) {
                siteCollectionPath = JSON.stringify(siteCollectionPath);
                getContentCheckConfigurations(siteCollectionPath, function (response) {
                    if (!response.isError) {
                        var defaultMatterConfig = JSON.parse(response.code);
                        vm.oUploadGlobal.bAllowContentCheck = defaultMatterConfig.IsContentCheck;

                    } else {
                        vm.oUploadGlobal.bAllowContentCheck = false;
                    }

                });

            }
            //#region To expand and collapse the folder tree structure in upload
            vm.showSelectedFolderTree = function (folder) {
                function setActiveItem(item) {
                    if (item.children !== null) {
                        angular.forEach(item.children, function (child) {
                            if (item.parentURL !== null) {
                                if (item.active) {
                                    child.active = child.active ? false : true;
                                    if (!child.active) {
                                        setActiveItem(child);
                                    }
                                } else {
                                    child.active = false;
                                    setActiveItem(child);
                                }
                            }
                            else {
                                child.active = child.active ? false : true;
                                if (!child.active) {
                                    setActiveItem(child);
                                }
                            }
                        });
                    }

                }
                setActiveItem(folder);

            }
            //#endRegion
            //#region To do contentcheck or save as latestversion
            vm.localOverWriteDocument = function (duplicateFile, sOperation) {
                if (duplicateFile.fileType == "remotefile") {
                    if ("contentCheck" === sOperation) {
                        vm.files = [vm.oUploadGlobal.arrFiles[vm.oUploadGlobal.arrFiles.length - 1]];
                    } else {
                        vm.files = [vm.oUploadGlobal.arrFiles.pop()];
                        duplicateFile.cancel = null;
                    }

                    var nOperation = "";
                    if ("ignore" !== sOperation) {
                        switch (sOperation) {
                            case "overwrite":
                                nOperation = "0";
                                break;
                            case "append":
                                nOperation = "1";
                                break;
                            case "contentCheck":
                                nOperation = "2";
                                break;
                            case "cancelContentCheck":
                                nOperation = "3";
                                break;
                        }
                        // uploadFile(oUploadGlobal.sClientRelativeUrl, oUploadGlobal.sFolderUrl, nOperation);

                        vm.handleDesktopDrop(vm.clientRelativeUrl, vm.files, nOperation);



                    } else {
                        duplicateFile.cancel = "False";
                        if (vm.ducplicateSourceFile.length > 0) {
                            vm.ducplicateSourceFile.pop();
                        }
                    }
                }
                else if (duplicateFile.fileType == "attacheddocument") {
                    console.log("dragged attached obj");
                    console.log(duplicateFile);
                    var draggedFile = $filter("filter")(vm.allAttachmentDetails, vm.sourceFile.attachmentId)[0];
                    if ("contentCheck" === sOperation) {
                        mailOrDocUpload(vm.targetDrop, vm.sourceFile, false, true, draggedFile);


                    } else if ("overwrite" === sOperation) {

                        duplicateFile.cancel = null; vm.ducplicateSourceFile.pop();
                        mailOrDocUpload(vm.targetDrop, vm.sourceFile, true, undefined, draggedFile);
                    }
                    else if ("append" === sOperation) {

                        if (vm.sourceFile.isEmail && vm.sourceFile.isEmail === "true") {
                            mailOrDocUpload(vm.targetDrop, vm.sourceFile, true, false, draggedFile, sOperation)
                        }
                        if (vm.sourceFile.isEmail && vm.sourceFile.isEmail === "false") {
                            mailOrDocUpload(vm.targetDrop, vm.sourceFile, true, false, draggedFile, sOperation)
                        }
                        duplicateFile.cancel = null;
                    }
                    else {
                        duplicateFile.cancel = "False";
                        if (vm.ducplicateSourceFile.length > 0) {
                            vm.ducplicateSourceFile.pop();
                        }
                    }
                }
                else {
                    vm.ducplicateSourceFile = vm.ducplicateSourceFile.filter(function (item) {
                        return item.fileName !== duplicateFile.fileName;
                    });
                }
            }

            // Function to configure time stamp
            vm.overwriteConfiguration = function (fileName) {
                // Update the content as per the logic.
                var selectedOverwriteConfiguration = configs.uploadMessages.overwrite_Config_Property.trim().toLocaleUpperCase(),
                    fileExtension = fileName.trim().substring(fileName.trim().lastIndexOf(".") + 1),
                    bAppendEnabled = false;

                switch (selectedOverwriteConfiguration) {
                    case "BOTH":
                        bAppendEnabled = true;
                        break;
                    case "DOCUMENT ONLY":
                        bAppendEnabled = "eml" === fileExtension || "msg" === fileExtension ? false : true;
                        break;
                    default:
                        bAppendEnabled = "eml" === fileExtension || "msg" === fileExtension ? true : false;
                        break;
                }
                return bAppendEnabled;
            }

            vm.contentCheckNotification = function (file, isLocalUpload) {
                file.contentCheck = "contentCheck";
                file.saveLatestVersion = "False";
                file.cancel = "False";
                if (file.append) {
                    file.append = false;
                }

            }
            vm.abortContentCheck = function (file, isLocalUpload) {
                "use strict";
                if (isLocalUpload) {
                    vm.oUploadGlobal.canceler.resolve();
                    vm.oUploadGlobal.canceler = $q.defer();
                }
                file.contentCheck = null;
                file.saveLatestVersion = "True";
                file.value = file.value + "<br/><div>" + configs.uploadMessages.content_Check_Abort + "</div>";
                file.cancel = "True";

            }

            vm.closeSuccessBanner = function () {
                vm.oUploadGlobal.successBanner = false;
            }


            $scope.errorImage = function (image) {
                "use strict";
                if (image && image.iconSrc && image.iconSrc != "") {

                }
            }


            //#region For displaying and setting the position of the filters name wise
            vm.matterheader = true;
            vm.matterdateheader = true;
            vm.searchexp = "";
            vm.filtername = "";

            vm.openMatterHeader = function ($event, name) {
                vm.filternodata = false;
                vm.details = [];
                var dimensions = $event.target.getBoundingClientRect();
                var top = dimensions.top + 30;
                var left = dimensions.left - 224;
                angular.element('.matterheader').css({
                    'top': top, 'left': left
                });
                angular.element('.matterheaderdates').css({
                    'top': top, 'left': left
                });
                if (name === vm.matterConfigContent.GridColumn1Header) {
                    vm.searchexp = "" + vm.configSearchContent.ManagedPropertyMatterName + "";
                    vm.filtername = vm.matterConfigContent.GridColumn1Header;
                }
                if (name === vm.matterConfigContent.GridColumn2Header && !vm.globalSettings.isBackwardCompatible) {
                    vm.searchexp = "" + vm.configSearchContent.ManagedPropertyClientName + "";
                    vm.filtername = vm.matterConfigContent.GridColumn2Header;
                }
                if (name === vm.matterConfigContent.GridColumn2Header && vm.globalSettings.isBackwardCompatible) {
                    vm.searchexp = "" + vm.configSearchContent.ManagedPropertyPracticeGroup + "";
                    vm.filtername = vm.matterConfigContent.GridColumn2Header;
                }
                if (name === vm.matterConfigContent.GridColumn3Header) {
                    vm.searchexp = "" + vm.configSearchContent.ManagedPropertyAreaOfLaw + "";
                    vm.filtername = vm.matterConfigContent.GridColumn3Header;
                }
                if (name === vm.matterConfigContent.GridColumn5Header) {
                    if (!vm.globalSettings.isBackwardCompatible) {
                        vm.searchexp = "" + vm.configSearchContent.ManagedPropertyResponsibleAttorney + "";
                        vm.filtername = vm.matterConfigContent.GridColumn5Header;
                    }
                    else {
                        vm.filtername = vm.matterConfigContent.GridColumn5Header;
                    }
                }
                //AOL
                if (name === vm.matterConfigContent.GridColumn6Header && !vm.globalSettings.isBackwardCompatible) {
                    vm.searchexp = "" + vm.configSearchContent.ManagedPropertySubAreaOfLaw + "";
                    vm.filtername = vm.matterConfigContent.GridColumn6Header;
                }
                if (name === vm.matterConfigContent.GridColumn6Header && vm.globalSettings.isBackwardCompatible) {
                    vm.searchexp = "" + vm.configSearchContent.ManagedPropertyMatterId + "";
                    vm.filtername = vm.matterConfigContent.GridColumn6Header;
                }
                if (name === vm.matterConfigContent.GridColumn4Header && vm.globalSettings.isBackwardCompatible) {
                    vm.searchexp = "" + vm.configSearchContent.ManagedPropertySubAreaOfLaw + "";
                    vm.filtername = vm.matterConfigContent.GridColumn4Header;
                }
                if (name === vm.matterConfigContent.GridColumn7Header) {
                    vm.filtername = vm.matterConfigContent.GridColumn7Header;
                }
                $timeout(function () {
                    if (name == vm.matterConfigContent.GridColumn4Header && !vm.globalSettings.isBackwardCompatible
                        || name == vm.matterConfigContent.GridColumn7Header && !vm.globalSettings.isBackwardCompatible
                        || name == vm.matterConfigContent.GridColumn5Header && vm.globalSettings.isBackwardCompatible) {
                        vm.matterdateheader = false;
                    }
                    else {
                        vm.matterheader = false;
                    }
                },
                500);
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            }
            //#endregion

            //#region filtering the values as per the name
            //start
            vm.filtermatter = function (value) {
                var searchTerm = "";
                if (vm.filtername == vm.matterConfigContent.GridColumn1Header) {
                    searchTerm = vm.searchTerm.toLowerCase();
                }
                else if (vm.filtername == vm.matterConfigContent.GridColumn2Header && !vm.globalSettings.isBackwardCompatible) {
                    searchTerm = vm.clientSearchTerm.toLowerCase();
                }
                else if (vm.filtername == vm.matterConfigContent.GridColumn2Header && vm.globalSettings.isBackwardCompatible) {
                    searchTerm = vm.practiceGroupSearchTerm.toLowerCase();
                }
                else if (vm.filtername == vm.matterConfigContent.GridColumn5Header) {
                    searchTerm = vm.attorneySearchTerm.toLowerCase();
                }
                else if (vm.filtername == vm.matterConfigContent.GridColumn6Header && !vm.globalSettings.isBackwardCompatible) {
                    searchTerm = vm.areaSearchTerm.toLowerCase();
                }
                else if (vm.filtername == vm.matterConfigContent.GridColumn6Header && vm.globalSettings.isBackwardCompatible) {
                    searchTerm = vm.projectIDSearchTerm.toLowerCase();
                }
                else if (vm.filtername == vm.matterConfigContent.GridColumn3Header && vm.globalSettings.isBackwardCompatible) {
                    searchTerm = vm.areaOfLawSearchTerm.toLowerCase();
                }
                else if (vm.filtername == vm.matterConfigContent.GridColumn4Header) {
                    searchTerm = vm.subAreaOfLawSearchTerm.toLowerCase();
                }
                var arrayItem = value.split(';');
                var arrelements = [];
                angular.forEach(arrayItem, function (item) {
                    var lowerItem = item.toLowerCase();
                    if (-1 !== lowerItem.indexOf(searchTerm)) {
                        arrelements.push(item);
                    }
                });
                return arrelements.toString();
            }

            //end
            //#endregion

            //#region for opening view matters url in new window
            vm.viewMatterDetails = function (url, guid) {
                var viewmatterurl = url + '/SitePages/' + guid + '.aspx';
                window.open(viewmatterurl, 'viewmatterwindow', 'toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=650,height=500')
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

