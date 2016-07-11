(function () {
    'use strict';

    var app = angular.module("matterMain");

    app.controller('mattersController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'matterResource', '$rootScope', 'uiGridConstants', '$location', '$http', '$window', '$parse', '$templateCache', '$q', '$filter', 'commonFunctions',
        function ($scope, $state, $interval, $stateParams, api, $timeout, matterResource, $rootScope, uiGridConstants, $location, $http, $window, $parse, $templateCache, $q, $filter, commonFunctions) {
            var vm = this;
            vm.selected = '';
            vm.selectedRow = {
                matterClientUrl: '',
                matterName: '',
                matterGuid: ''
            };
            vm.mattername = "All Matters";
            vm.sortname = "";
            vm.mattersdrop = false;
            vm.mattersdropinner = true;
            //$rootScope.bodyclass = "bodymain";
            //This value is for displaying the help
            $rootScope.pageIndex = "1";
            //#region Onload show ui grid and hide error div
            //start
            vm.divuigrid = true;
            vm.nodata = false;
            //#endregion

            //#region To hide lazyloader on load
            //start
            vm.lazyloader = true;
            //#endregion

            //#region Assigning scopes for Dropdowns in headers
            //Start
            vm.matterDropDowm = false;
            vm.clientDropDowm = false;
            vm.modifieddateDropDowm = false;
            vm.attorneyDropDowm = false;
            vm.arealawDropDowm = false;
            vm.opendateDropDown = false;
            //End
            Office.initialize = function (reason) {
                // vm.initOutlook();
            };
            $scope.initOfficeLibrary = function () {

            };


            $templateCache.put('coldefheadertemplate.html', "<div><div role='button' class='ui-grid-cell-contents ui-grid-header-cell-primary-focus' col-index='renderIndex'><span class='ui-grid-header-cell-label ng-binding' title='Click to sort by {{ col.colDef.displayName }}'>{{ col.colDef.displayName }}<span id='asc{{col.colDef.field}}' style='float:right;display:none' class='padl10px'>↑</span><span id='desc{{col.colDef.field}}' style='float:right;display:none' class='padlf10'>↓</span></span></div></div>");

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
                columnDefs: [
                     { field: 'matterName', displayName: 'Matter', enableHiding: false, width: "275", cellTemplate: '../app/matter/MatterTemplates/MatterCellTemplate.html', headerCellTemplate: '../app/matter/MatterTemplates/MatterHeaderTemplate.html' },
                     { field: 'matterClient', displayName: 'Client', enableCellEdit: true, width: "200", headerCellTemplate: '../app/matter/MatterTemplates/ClientHeaderTemplate.html' },
                     { field: 'matterClientId', displayName: 'Client.MatterID', width: "150", headerCellTemplate: $templateCache.get('coldefheadertemplate.html'), cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.matterClientId}}.{{row.entity.matterID}}</div>', enableCellEdit: true, },
                     { field: 'matterModifiedDate', displayName: 'Modified Date', width: "195", cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.matterModifiedDate}}"></div>', headerCellTemplate: '../app/matter/MatterTemplates/ModifiedDateTemplate.html' },
                     { field: 'matterResponsibleAttorney', headerCellTemplate: '../app/matter/MatterTemplates/ResponsibleAttorneyHeaderTemplate.html', width: "250", displayName: 'Responsible attorney', visible: false },
                     { field: 'matterSubAreaOfLaw', headerCellTemplate: '../app/matter/MatterTemplates/AreaofLawHeaderTemplate.html', width: "210", displayName: 'Sub area of law', visible: false },
                     { field: 'matterCreatedDate', headerCellTemplate: '../app/matter/MatterTemplates/OpenDateTemplate.html', width: "170", displayName: 'Open date', cellTemplate: '<div class="ui-grid-cell-contents" datefilter date="{{row.entity.matterCreatedDate}}"></div>', visible: false },
                ],
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
                    $scope.gridApi.core.on.sortChanged($scope, $scope.sortChanged);
                    $scope.sortChanged($scope.gridApi.grid, [vm.gridOptions.columnDefs[1]]);
                    $scope.$watch('gridApi.grid.isScrollingVertically', vm.watchFuncscroll);
                    gridApi.infiniteScroll.on.needLoadMoreData($scope, vm.watchFunc);
                }
            };

            //#endregion

            vm.watchFuncscroll = function () {
                //var element = 0;
                //var topelement = angular.element('.ui-grid-viewport').scrollTop();
                //angular.element('.popcontent').css({ 'top': element - topelement });
                //if (!$scope.$$phase) {
                //    $scope.$apply();
                //}
            }


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

            //#region for setting the dynamic width to grid
            vm.setWidth = function () {
                var width = $window.innerWidth;
                angular.element(".ui-grid-viewport").css('max-width', width);
                angular.element(".ui-grid-render-container").css('max-width', width);
            };

            vm.setWidth();

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
                var attachmentName = "", sAttachmentFileName = "", bHasEML = false, attachmentType = "", sContentType = "", sExtension = "", iconSrc = "";
                vm.allAttachmentDetails = []
                var individualAttachment = {};
                //For just email
                individualAttachment.attachmentId = Office.context.mailbox.item.itemId;
                individualAttachment.counter = nIDCounter;
                individualAttachment.attachmentFileName = Office.context.mailbox.item.subject;
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
                    ItemsPerPage: "17",
                    SearchTerm: "",
                    Filters: {
                        AOLList: "",
                        ClientsList: [],
                        FilterByMe: 0,
                        FromDate: "",
                        PGList: "",
                        ToDate: "",
                    },
                    Sort:
                            {
                                ByProperty: "LastModifiedTime",
                                Direction: 1
                            }
                }
            }

            //#endregion


            //#region For filtering the grid when clicked on search button 
            vm.searchMatter = function (val) {
                var finalSearchText = "";
                if (val != "") {
                    finalSearchText = "(MCMatterName:" + val + "* OR MCMatterID:" + val + "*)";
                }
                vm.pagenumber = 1;
                searchRequest.SearchObject.PageNumber = vm.pagenumber;
                searchRequest.SearchObject.SearchTerm = finalSearchText;
                searchRequest.SearchObject.Sort.Direction = 1;
                return matterResource.get(searchRequest).$promise;
            }

            vm.search = function () {
                vm.matterid = 1;
                vm.mattername = "All Matters";
                vm.pagenumber = 1;
                var searchToText = '';
                var finalSearchText = '';
                if (vm.selected != "") {
                    if (vm.selected.indexOf("(") > -1) {
                        searchToText = vm.selected.replace("(", ",")
                        searchToText = searchToText.replace(")", "")
                        var firstText = searchToText.split(',')[0]
                        var secondText = searchToText.split(',')[1]
                        finalSearchText = '(MCMatterName:"' + firstText.trim() + '" AND MCMatterID:"' + secondText.trim() + '")'
                    } else {
                        finalSearchText = commonFunctions.searchFilter(vm.selected);
                    }
                }
                searchRequest.SearchObject.SearchTerm = finalSearchText;
                searchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
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


            //#region for setting the mattername in dropdown
            vm.SetMatters = function (id, name) {
                vm.mattername = name;
                vm.GetMatters(id);
                vm.matterid = id;
            }

            //#endregion
            //#region for searching matter by property and searchterm
            vm.mattersearch = function (term, property, bool) {
                vm.lazyloader = false;
                searchRequest.SearchObject.SearchTerm = term;
                searchRequest.SearchObject.Sort.ByProperty = property;
                if (bool) {
                    searchRequest.SearchObject.Sort.Direction = 1;
                    if (property == "MCResponsibleAttorney") {
                        vm.attorneyproperty = term;
                        searchRequest.SearchObject.SearchTerm = "";
                        searchRequest.SearchObject.Filters.ResponsibleAttorneys = term;
                    } else if (property == "MCSubAreaofLaw") {
                        searchRequest.SearchObject.SearchTerm = "";
                        searchRequest.SearchObject.Filters.SubareaOfLaw = term;
                    }
                    else {
                        searchRequest.SearchObject.Filters.ResponsibleAttorneys = "";
                        searchRequest.SearchObject.Filters.SubareaOfLaw = "";
                    }
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
                        if (bool) {
                            vm.gridOptions.data = response;
                            vm.details = [];
                            if (!$scope.$$phase) {
                                $scope.$apply();
                            }
                        } else {
                            vm.details = response;
                        }
                        searchRequest.SearchObject.SearchTerm = "";
                        searchRequest.SearchObject.Sort.ByProperty = "";
                    }
                });
            }
            //#endregion


            //#region Code for filtering ModifiedDate
            //start
            vm.FilterModifiedDate = function () {
                vm.lazyloader = false;
                vm.divuigrid = false;
                var ModifiedDateRequest =
                  {
                      Client: {
                          Url: configs.global.repositoryUrl
                      },
                      SearchObject: {
                          PageNumber: 1,
                          ItemsPerPage: 10,
                          SearchTerm: "",
                          Filters: {
                              AOLList: "",
                              ClientName: "",
                              ClientsList: [],
                              DateFilters: { CreatedFromDate: "", CreatedToDate: "", ModifiedFromDate: vm.startdate.format("dd/MM/yyyy"), ModifiedToDate: vm.enddate.format("dd/MM/yyyy"), OpenDateFrom: "", OpenDateTo: "" },
                              DocumentAuthor: [],
                              DocumentCheckoutUsers: [],
                              FilterByMe: 1,
                              FromDate: "",
                              Name: "",
                              PGList: "",
                              ResponsibleAttorneys: [],
                              SubareaOfLaw: "",
                              ToDate: ""
                          },
                          Sort:
                                  {
                                      ByProperty: "LastModifiedTime",
                                      Direction: 0
                                  }
                      }
                  };
                get(ModifiedDateRequest, function (response) {
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
                        vm.startDate = "";
                        vm.endDate = "";
                    }
                });

            }

            //#endregion

            //#region Code written for displaying types in dropdown 
            //Start 
            vm.Matters = [{ Id: 1, Name: "All Matters" }, { Id: 2, Name: "My Matters" }, { Id: 3, Name: "Pinned Matters" }];
            vm.ddlMatters = vm.Matters[0];
            //#endregion  

            //#region Hits when the Dropdown changes 
            //Start 
            vm.GetMatters = function (id) {
                vm.selected = "";
                vm.searchTerm = "";
                vm.searchClientTerm = "";
                vm.startdate = "";
                vm.enddate = "";
                vm.sortexp = "";
                vm.sortby = "";
                vm.lazyloader = false;
                vm.divuigrid = false;
                var pinnedMattersRequest = {
                    Url: configs.global.repositoryUrl
                }
                if (id == 1) {
                    vm.pagenumber = 1;
                    vm.responseNull = false;
                    searchRequest.SearchObject.PageNumber = 1;
                    searchRequest.SearchObject.SearchTerm = "";
                    searchRequest.SearchObject.Filters.FilterByMe = 0;
                    searchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                    get(searchRequest, function (response) {
                        if (response == "") {
                            vm.gridOptions.data = response;
                            vm.lazyloader = true;
                            vm.divuigrid = true;
                            vm.nodata = true;
                        } else {
                            getPinnedMatters(pinnedMattersRequest, function (pinnedResponse) {
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
                        $timeout(function () {
                            vm.lazyloader = true;
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }, 1000);
                    });
                } else if (id == 2) {
                    vm.lazyloader = false;
                    vm.divuigrid = false;
                    vm.pagenumber = 1;
                    vm.responseNull = false;
                    searchRequest.SearchObject.PageNumber = 1;
                    searchRequest.SearchObject.SearchTerm = "";
                    searchRequest.SearchObject.Filters.FilterByMe = 1;
                    searchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                    get(searchRequest, function (response) {
                        if (response == "") {
                            vm.gridOptions.data = response;
                            vm.lazyloader = true;
                            vm.divuigrid = true;
                            vm.nodata = true;
                        } else {
                            getPinnedMatters(pinnedMattersRequest, function (pinnedResponse) {
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
                            });
                        }
                    });
                } else if (id == 3) {
                    vm.lazyloader = false;
                    vm.divuigrid = false;
                    var pinnedMattersRequest = {
                        Url: configs.global.repositoryUrl
                    }
                    getPinnedMatters(pinnedMattersRequest, function (response) {
                        if (response == "") {
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
                        }
                    });
                }
            }
            //#endregion 


            //#region To run GetMatters function on page load 
            vm.GetMatters(vm.ddlMatters.Id);
            //End 


            //#region Written for unpinning the matter 
            //Start 
            vm.UnpinMatter = function (data) {
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
                        $timeout(vm.SetMatters(vm.matterid, vm.mattername), 500);
                        alert("Success");
                    }
                });
            }
            //End 


            //#region Written for pinning the matter 
            //Start 
            vm.PinMatter = function (data) {
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
                        $timeout(vm.SetMatters(vm.matterid, vm.mattername), 500);
                        alert("Success");
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
            //Start
            vm.dateOptions = {

                formatYear: 'yy',
                maxDate: new Date()
            };


            vm.enddateOptions = {
                formatYear: 'yy',
                maxDate: new Date()
            }

            $scope.$watch('startdate', function (newval, oldval) {
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
                    if (response == "") {
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
                                searchRequest.SearchObject.Sort.ByProperty = "MCMatterName";
                                searchRequest.SearchObject.Sort.Direction = 0;
                                vm.FilterByType();
                                vm.MatterNameSort = "desc"; vm.sortby = "asc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            } else {
                                vm.pagenumber = 1;
                                vm.lazyloader = false;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "MCMatterName";
                                searchRequest.SearchObject.Sort.Direction = 1;
                                vm.FilterByType();
                                vm.MatterNameSort = "asc"; vm.sortby = "desc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }
                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[1].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.ClientSort == undefined || vm.ClientSort == "asc") {
                                vm.pagenumber = 1;
                                vm.lazyloader = false;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "MCClientName";
                                searchRequest.SearchObject.Sort.Direction = 0;
                                vm.FilterByType();
                                vm.ClientSort = "desc"; vm.sortby = "asc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }
                            else {
                                vm.pagenumber = 1;
                                vm.lazyloader = false;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "MCClientName";
                                searchRequest.SearchObject.Sort.Direction = 1;
                                vm.FilterByType();
                                vm.ClientSort = "asc"; vm.sortby = "desc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }
                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[2].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.ClientIDSort == undefined || vm.ClientIDSort == "asc") {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "MCClientID";
                                searchRequest.SearchObject.Sort.Direction = 0;
                                vm.FilterByType();
                                vm.ClientIDSort = "desc"; vm.sortby = "asc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            } else {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "MCClientID";
                                searchRequest.SearchObject.Sort.Direction = 1;
                                vm.FilterByType();
                                vm.ClientIDSort = "asc"; vm.sortby = "desc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }

                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[3].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.ModiFiedTimeSort == undefined || vm.ModiFiedTimeSort == "asc") {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                                searchRequest.SearchObject.Sort.Direction = 0;
                                vm.FilterByType();
                                vm.ModiFiedTimeSort = "desc"; vm.sortby = "asc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            } else {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                                searchRequest.SearchObject.Sort.Direction = 1;
                                vm.FilterByType();
                                vm.ModiFiedTimeSort = "asc"; vm.sortby = "desc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }

                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[4].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.ResAttoSort == undefined || vm.ResAttoSort == "asc") {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "MCResponsibleAttorney";
                                searchRequest.SearchObject.Sort.Direction = 0;
                                vm.FilterByType();
                                vm.ResAttoSort = "desc"; vm.sortby = "asc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            } else {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "MCResponsibleAttorney";
                                searchRequest.SearchObject.Sort.Direction = 1;
                                vm.FilterByType();
                                vm.ResAttoSort = "asc"; vm.sortby = "desc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }
                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[5].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.SubAreaSort == undefined || vm.SubAreaSort == "asc") {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "MCSubAreaofLaw";
                                searchRequest.SearchObject.Sort.Direction = 0;
                                vm.FilterByType();
                                vm.SubAreaSort = "desc"; vm.sortby = "asc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            } else {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "MCSubAreaofLaw";
                                searchRequest.SearchObject.Sort.Direction = 1;
                                vm.FilterByType();
                                vm.SubAreaSort = "asc"; vm.sortby = "desc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }
                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[6].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (vm.OpenDateSort == undefined || vm.OpenDateSort == "asc") {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "MCOpenDate";
                                searchRequest.SearchObject.Sort.Direction = 0;
                                vm.FilterByType();
                                vm.OpenDateSort = "desc"; vm.sortby = "asc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            } else {
                                vm.lazyloader = false;
                                vm.pagenumber = 1;
                                searchRequest.SearchObject.PageNumber = 1;
                                searchRequest.SearchObject.Sort.ByProperty = "MCOpenDate";
                                searchRequest.SearchObject.Sort.Direction = 1;
                                vm.FilterByType();
                                vm.OpenDateSort = "asc"; vm.sortby = "desc";
                                vm.sortexp = sortColumns[0].field;
                                $interval(function () { vm.showSortExp(); }, 1200, 3);
                            }

                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }
                    }
                } else {
                    //if (vm.MatterNameSort == undefined || vm.MatterNameSort == "asc") {
                    //    vm.pagenumber = 1;
                    //    vm.lazyloader = false;
                    //    searchRequest.SearchObject.PageNumber = 1;
                    //    searchRequest.SearchObject.Sort.ByProperty = "MCMatterName";
                    //    searchRequest.SearchObject.Sort.Direction = 0;
                    //    vm.FilterByType();
                    //    vm.MatterNameSort = "desc"; vm.sortby = "asc";
                    //    vm.sortexp = "matterName";
                    //    $interval(function () { vm.showSortExp(); }, 1200, 3);
                    //} else {
                    //    vm.pagenumber = 1;
                    //    vm.lazyloader = false;
                    //    searchRequest.SearchObject.PageNumber = 1;
                    //    searchRequest.SearchObject.Sort.ByProperty = "MCMatterName";
                    //    searchRequest.SearchObject.Sort.Direction = 1;
                    //    vm.FilterByType();
                    //    vm.MatterNameSort = "asc"; vm.sortby = "desc";
                    //    vm.sortexp = "matterName";
                    //    $interval(function () { vm.showSortExp(); }, 1200, 3);
                    //}
                }
            }
            //#endregion

            //#region setting the grid options when window is resized

            angular.element($window).bind('resize', function () {
                if ($window.innerWidth < 360) {
                    vm.gridOptions.enableHorizontalScrollbar = false;
                    vm.gridOptions.enablePaginationControls = false;
                    vm.gridOptions.columnDefs = [{ field: 'matterName', displayName: 'Matter', enableHiding: false, width: "275", cellTemplate: '../app/matter/MatterTemplates/MatterCellTemplate.html', headerCellTemplate: '../app/matter/MatterTemplates/MatterHeaderTemplate.html' }];
                    $scope.$apply();
                } else {
                    //vm.gridOptions = {
                    //    //paginationPageSizes: [10, 50, 100],
                    //    //paginationPageSize: 10,
                    //    enableHorizontalScrollbar: 0,
                    //    enableVerticalScrollbar: 1,
                    //    enableGridMenu: true,
                    //    enableRowHeaderSelection: false,
                    //    enableRowSelection: true,
                    //    enableSelectAll: false,
                    //    multiSelect: false,
                    //    columnDefs: [
                    //         { field: 'matterName', displayName: 'Matter', enableHiding: false, width: "245", cellTemplate: '../app/matter/MatterTemplates/MatterCellTemplate.html', headerCellTemplate: '../app/matter/MatterTemplates/MatterHeaderTemplate.html' },
                    //         { field: 'matterClient', displayName: 'Client', enableCellEdit: true, width: "200", headerCellTemplate: '../app/matter/MatterTemplates/ClientHeaderTemplate.html' },
                    //         { field: 'matterClientId', displayName: 'Client.MatterID', width: "150", headerCellTemplate: $templateCache.get('coldefheadertemplate.html'), cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.matterClientId}}.{{row.entity.matterID}}</div>', enableCellEdit: true, },
                    //         { field: 'matterModifiedDate', displayName: 'Modified Date', width: "195", cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.matterModifiedDate}}"></div>', headerCellTemplate: '../app/matter/MatterTemplates/ModifiedDateTemplate.html' },
                    //         { field: 'matterResponsibleAttorney', headerCellTemplate: '../app/matter/MatterTemplates/ResponsibleAttorneyHeaderTemplate.html', width: "250", displayName: 'Responsible attorney', visible: false },
                    //         { field: 'matterSubAreaOfLaw', headerCellTemplate: '../app/matter/MatterTemplates/AreaofLawHeaderTemplate.html', width: "210", displayName: 'Sub area of law', visible: false },
                    //         { field: 'matterCreatedDate', headerCellTemplate: '../app/matter/MatterTemplates/OpenDateTemplate.html', width: "170", displayName: 'Open date', cellTemplate: '<div class="ui-grid-cell-contents" datefilter date="{{row.entity.matterCreatedDate}}"></div>', visible: false },
                    //    ],
                    //    enableColumnMenus: false,
                    //    onRegisterApi: function (gridApi) {
                    //        $scope.gridApi = gridApi;
                    //        gridApi.core.on.columnVisibilityChanged($scope, function (changedColumn) {
                    //            $scope.columnChanged = { name: changedColumn.colDef.name, visible: changedColumn.colDef.visible };
                    //        });
                    //        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                    //            vm.selectedRow = row.entity
                    //        });
                    //        $scope.gridApi.core.on.sortChanged($scope, $scope.sortChanged);
                    //        $scope.sortChanged($scope.gridApi.grid, [vm.gridOptions.columnDefs[1]]);
                    //        $scope.$watch('gridApi.grid.isScrollingVertically', vm.watchFunc);
                    //    }
                    //};
                }
            });

            //#endregion

            //#region
            vm.typeheadselect = function (index, selected) {
                vm.matterid = 1;
                vm.mattername = "All Matters";
                var searchToText = '';
                var finalSearchText = '';
                if (selected != "") {
                    searchToText = selected.replace("(", ",")
                    searchToText = searchToText.replace(")", "")
                    var firstText = searchToText.split(',')[0]
                    var secondText = searchToText.split(',')[1]
                    var finalSearchText = '(MCMatterName:"' + firstText.trim() + '" AND MCMatterID:"' + secondText.trim() + '")'
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
                                    if (!child.active) { setActiveItem(child); }
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

            vm.matterheader = true;
            vm.matterdateheader = true;
            vm.searchexp = "";
            vm.searchName = "";
            vm.filtername = "";

            vm.openMatterHeader = function ($event, name) {
                vm.searchTerm = "";
                vm.details = [];
                var dimensions = $event.target.getBoundingClientRect();
                var top = dimensions.top + 30;
                var left = dimensions.left - 254;
                angular.element('.matterheader').css({ 'top': top, 'left': left });
                angular.element('.matterheaderdates').css({ 'top': top, 'left': left });
                if (name == "matter") {
                    vm.searchexp = "MCMatterName";
                    vm.searchName = vm.searchTerm;
                    vm.filtername = "Matter";
                }
                if (name == "client") {
                    vm.searchexp = "MCClientName";
                    vm.searchName = "MCClientName:" + vm.searchTerm + "*(MCMatterName:* OR MCMatterID:* OR MCClientName:*)";
                    vm.filtername = "Client";
                }
                if (name == "Attorney") {
                    vm.searchexp = "MCResponsibleAttorney";
                    vm.searchName = "MCResponsibleAttorney:" + vm.searchTerm + "*(MCMatterName:* OR MCMatterID:* OR MCClientName:*)";
                    vm.filtername = "Responsible Attorney";
                }
                if (name == "AreaOfLaw") {
                    vm.searchexp = "MCSubAreaofLaw";
                    vm.searchName = "MCSubAreaofLaw:" + vm.searchTerm + "*(MCMatterName:* OR MCMatterID:* OR MCClientName:*)";
                    vm.filtername = "Area of Law";
                }
                if (name == "ModifiedDate") {
                    vm.filtername = "Modified Date";
                }
                if (name == "OpenDate") {
                    vm.filtername = "Open Date";
                }
                $timeout(function () {
                    if (name == 'ModifiedDate' || name == 'OpenDate')
                    {
                        vm.matterdateheader = false;
                    }
                    else{
                        vm.matterheader = false;
                    }
                },
                500);
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            }

        }]);
})();

