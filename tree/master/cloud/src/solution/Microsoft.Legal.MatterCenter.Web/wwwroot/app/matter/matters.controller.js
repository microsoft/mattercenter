(function () {
    'use strict';

    var app = angular.module("matterMain");

    app.controller('mattersController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'matterResource', '$rootScope', 'uiGridConstants', '$location', '$http', '$window',
        function ($scope, $state, $interval, $stateParams, api, $timeout, matterResource, $rootScope, uiGridConstants, $location, $http, $window) {
            var vm = this;
            vm.selected = undefined;

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
            //End

            $scope.initOfficeLibrary = function () {

            };


            //#region Setting the options for grid
            vm.gridOptions = {
                //paginationPageSizes: [10, 50, 100],
                //paginationPageSize: 10,
                enableHorizontalScrollbar: 0,
                enableVerticalScrollbar: 1,
                enableGridMenu: true,
                enableRowHeaderSelection: false,
                enableRowSelection: true,
                enableSelectAll: false,
                multiSelect: false,
                columnDefs: [
                     { field: 'matterName', displayName: 'Matter', enableHiding: false, width: "20%", cellTemplate: '../app/matter/MatterCellTemplate.html', headerCellTemplate: '../app/matter/MatterHeaderTemplate.html' },
                     { field: 'matterClient', displayName: 'Client', enableCellEdit: true, width: "15%", headerCellTemplate: '../app/matter/ClientHeaderTemplate.html' },
                     { field: 'matterClientId', displayName: 'Client.MatterID', width: "15%", headerTooltip: 'Click to sort by client.matterid', cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.matterClientId}}.{{row.entity.matterID}}</div>', enableCellEdit: true, },
                     { field: 'matterModifiedDate', displayName: 'Modified Date', width: "10%", cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.matterModifiedDate}}"></div>', headerCellTemplate: '../app/matter/ModifiedDateTemplate.html' },
                     { field: 'matterResponsibleAttorney', headerTooltip: 'Click to sort by attorney', width: "15%", displayName: 'Responsible attorney', visible: false },
                     { field: 'matterSubAreaOfLaw', headerTooltip: 'Click to sort by sub area of law', width: "15%", displayName: 'Sub area of law', visible: false },
                     { field: 'matterCreatedDate', headerTooltip: 'Click to sort by matter open date', width: "15%", displayName: 'Open date', cellTemplate: '<div class="ui-grid-cell-contents" datefilter date="{{row.entity.matterCreatedDate}}"></div>', visible: false },
                ],
                enableColumnMenus: false,
                onRegisterApi: function (gridApi) {
                    $scope.gridApi = gridApi;
                    gridApi.core.on.columnVisibilityChanged($scope, function (changedColumn) {
                        $scope.columnChanged = { name: changedColumn.colDef.name, visible: changedColumn.colDef.visible };
                    });
                    gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                        vm.selectedRow = row.entity
                    });
                    $scope.gridApi.core.on.sortChanged($scope, $scope.sortChanged);
                    $scope.sortChanged($scope.gridApi.grid, [vm.gridOptions.columnDefs[1]]);
                    $scope.$watch('gridApi.grid.isScrollingVertically', watchFunc);
                    function watchFunc(newData) {
                        if (newData === true) {
                            $rootScope.$broadcast('scrolled');
                        }
                    }
                }
            };
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
            //#endregion


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

            vm.getFolderHierarchy = function () {
                var matterData = {
                    MatterName: vm.selectedRow.matterName,
                    MatterUrl: "https://msmatter.sharepoint.com/sites/microsoft"
                };
                getFolderHierarchy(matterData, function (response) {
                    vm.foldersList = response.foldersList;
                    jQuery('#UploadMatterModal').modal("show");
                    //Initialize Officejs library                     
                    Office.initialize = function (reason) {

                    };
                    vm.initOutlook();
                });
            }

            //#region drop method will handle the file upload scenario for both email and attachment

            //Helper method which will handle mail or doc upload. This method will be called from inside vm.handleDrop
            function mailOrDocUpload(targetDrop, sourceFile, isOverwrite, performContentCheck) {
                var attachments = [];
                var attachmentsArray = {};
                var mailId = '';

                if (sourceFile.isEmail && sourceFile.isEmail === "true") {
                    attachments = vm.allAttachmentDetails
                    mailId = Office.context.mailbox.item.itemId;
                    for (var iCounter = 0; iCounter < vm.allAttachmentDetails.length; iCounter++) {
                        attachments = [];
                        attachmentsArray.attachmentType = 0;
                        attachmentsArray.name = vm.allAttachmentDetails[iCounter].attachmentFileName;
                        attachmentsArray.isInline = false;
                        attachmentsArray.contentType = vm.allAttachmentDetails[iCounter].contentType;
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
                        Url: "https://msmatter.sharepoint.com/sites/microsoft"
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
                        AllowContentCheck: performContentCheck,
                        Attachments: attachments
                    }
                }

                if (sourceFile.isEmail && sourceFile.isEmail === "true") {
                    vm.uploadEmail(attachmentRequestVM);
                }
                if (sourceFile.isEmail && sourceFile.isEmail === "false") {
                    vm.uploadAttachment(attachmentRequestVM);
                }
            }

            //This function will handle the files that has been dragged from the outlook
            vm.handleOutlookDrop = function (targetDrop, sourceFile) {
                vm.targetDrop = targetDrop;
                vm.sourceFile = sourceFile;
                var isOverwrite = false;//Todo: Need to get from the config.js
                var performContentCheck = false;//Todo: Need to get from the config.js
                vm.showLoading = true;
                mailOrDocUpload(targetDrop, sourceFile, isOverwrite, performContentCheck);
            }

            //This function will handle the files that has been dragged from the user desktop
            vm.handleDesktopDrop = function (targetDrop, sourceFiles) {
                vm.isLoadingFromDesktopStarted = true;
                vm.files = sourceFiles.files;
                var fd = new FormData();
                fd.append('targetDropUrl', targetDrop.url);
                fd.append('folderUrl', targetDrop.url)
                fd.append('documentLibraryName', vm.selectedRow.matterName)
                fd.append('clientUrl', 'https://msmatter.sharepoint.com/sites/microsoft');
                angular.forEach(vm.files, function (file) {
                    fd.append('file', file);
                })

                $http.post("/api/v1/document/uploadfiles", fd, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined }
                }).then(function (response) {
                    vm.isLoadingFromDesktopStarted = false;
                    console.log(response.data);
                    vm.uploadedFiles = response.data;
                }).catch(function (response) {
                    vm.isLoadingFromDesktopStarted = false;
                    console.error('Gists error', response.status, response.data);
                })

            }


            //#endregion

            //#region Mail Upload Methods

            //Call back function when the mail gets uploaded
            vm.uploadEmail = function (attachmentRequestVM) {
                uploadEmail(attachmentRequestVM, function (response) {
                    vm.showLoading = false
                    //If the mail upload is success
                    if (response.code === "OK" && response.value === "Attachment upload success") {
                        vm.mailUpLoadSuccess = true;
                        var subject = Office.context.mailbox.item.subject;
                        subject = subject.substring(0, subject.lastIndexOf("."));
                        vm.mailUploadedFile = subject;
                        vm.mailUploadedFolder = vm.targetDrop.name;
                        //removeDraggableDirective();
                    }
                        //If the mail upload is not success
                    else if (response.code === "DuplicateDocument") {
                        vm.IsDupliacteDocument = true; //ToDo:Set it to false on mail upload dialog open
                        vm.IsNonIdenticalContent = false;
                    }
                    else if (response.code === "NonIdenticalContent") {
                        vm.IsNonIdenticalContent = true; //ToDo:Set it to false on mail upload dialog open
                        vm.IsDupliacteDocument = false;
                    }
                    console.log(response);
                });
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
            vm.uploadAttachment = function (attachmentRequestVM) {
                uploadAttachment(attachmentRequestVM, function (response) {
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
                            vm.docUploadedFolder = vm.targetDrop.name;
                        }
                        //removeDraggableDirective();

                    }
                        //If the attachment upload is not success
                    else if (response.code === "DuplicateDocument") {
                        vm.IsDupliacteDocument = true; //ToDo:Set it to false on mail upload dialog open
                        vm.IsNonIdenticalContent = false;
                    }
                        //NonIdenticalContent
                    else if (response.code === "NonIdenticalContent") {
                        vm.IsNonIdenticalContent = true; //ToDo:Set it to false on mail upload dialog open
                        vm.IsDupliacteDocument = false;
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


            $scope.Openuploadmodal = function () {
                vm.getFolderHierarchy();
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
                bAllowContentCheck: false
            };

            vm.attachmentTokenCallbackEmailClient = function (asyncResult, userContext) {
                "use strict";
                if (asyncResult.status === "succeeded") {
                    vm.attachmentToken = asyncResult.value;
                    vm.createMailPopup();
                    vm.loadingAttachments = false;
                } else {
                    //showNotification(oFindMatterConstants.Fail_Attachment_Token, "failNotification");
                }
            }

            vm.getIconSource = function (sExtension) {
                //var iconSrc = configs.Upload.ImageDocumentIcon.replace("{0}", sExtension);
                //iconSrc = (-1 < configs.Upload.PNGIconExtensions.indexOf(sExtension)) ?
                //                iconSrc.substring(0, configs.Upload.ImageDocumentIcon.lastIndexOf(".") + 1) + "png" : iconSrc;
                //return iconSrc;
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
                vm.loadingAttachments = true;
                var sImageChunk = "", nIDCounter = 0;
                var attachmentName = "", sAttachmentFileName = "", bHasEML = false, attachmentType = "", sContentType = "", sExtension = "", iconSrc = "";
                vm.allAttachmentDetails = []
                var individualAttachment = {};
                //For just email
                individualAttachment.attachmentId = Office.context.mailbox.item.itemId;
                individualAttachment.counter = nIDCounter;
                individualAttachment.attachmentFileName = Office.context.mailbox.item.subject;
                individualAttachment.isEmail = true;
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
                    var iconSrc = ''; //vm.getIconSource(sExtension);
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
                    individualAttachment.size = vm.attachments[attachment].size
                    vm.allAttachmentDetails.push(individualAttachment);
                }
            }
            //#endregion


            //#region For filtering the grid when clicked on search button 
            vm.searchMatter = function (val) {
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
                          SearchTerm: val,
                          Filters: {},
                          Sort:
                                  {
                                      ByProperty: "LastModifiedTime",
                                      Direction: 1
                                  }
                      }
                  };
                return matterResource.get(searchRequest).$promise;
            }


            vm.search = function () {
                vm.lazyloader = false;
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
                          SearchTerm: vm.selected,
                          Filters: {},
                          Sort:
                                  {
                                      ByProperty: "LastModifiedTime",
                                      Direction: 1
                                  }
                      }
                  };
                get(searchRequest, function (response) {
                    vm.lazyloader = true;
                    vm.gridOptions.data = response;
                });
            }
            //#endregion

            //#region Searchrequest object
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
                                     Direction: 0
                                 }
                     }
                 };

            //#endregion

            //#region For Searching Matter in GridHeader Menu
            //Start
            vm.searchMatterGrid = function () {
                vm.lazyloader = false;
                searchRequest.SearchObject.SearchTerm = vm.searchTerm;
                searchRequest.SearchObject.Sort.ByProperty = "MCMatterName";
                get(searchRequest, function (response) {
                    vm.lazyloader = true;
                    vm.matters = response;
                });
            }
            //#endregion

            //#region For Searching client in GridHeader Menu
            //start
            vm.searchClient = function () {
                vm.lazyloader = false;
                searchRequest.SearchObject.SearchTerm = vm.searchClientTerm;
                searchRequest.SearchObject.Sort.ByProperty = "MCClientName";
                get(searchRequest, function (response) {
                    vm.lazyloader = true;
                    vm.Clients = response;
                });
            }
            //#endregion

            //#region For filtering mattername 
            //Start
            vm.filterMatterName = function (mattername) {
                vm.lazyloader = false;
                searchRequest.SearchObject.SearchTerm = mattername;
                searchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                searchRequest.SearchObject.Sort.Direction = 1;
                get(searchRequest, function (response) {
                    vm.lazyloader = true;
                    vm.gridOptions.data = response;
                    vm.matters = [];
                });

            }
            //#endregion


            //#region For filtering Clientname 
            //Start
            vm.filterClientName = function (clientname) {
                vm.lazyloader = false;
                searchRequest.SearchObject.SearchTerm = clientname;
                searchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                searchRequest.SearchObject.Sort.Direction = 1;
                get(searchRequest, function (response) {
                    vm.lazyloader = true;
                    vm.gridOptions.data = response;
                    vm.Clients = [];
                });

            }

            //#endregion


            //#region Code for filtering ModifiedDate
            //start
            vm.FilterModifiedDate = function () {
                vm.lazyloader = false;
                var ModifiedDateRequest =
                  {
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
                              AOLList: "",
                              ClientName: "",
                              ClientsList: [],
                              DateFilters: { CreatedFromDate: "", CreatedToDate: "", ModifiedFromDate: "05/02/2016", ModifiedToDate: "05/06/2016", OpenDateFrom: "", OpenDateTo: "" },
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
                    vm.lazyloader = true;
                    vm.gridOptions.data = response;
                    vm.startDate = "";
                    vm.endDate = "";
                });

            }

            //#endregion

            //#region Code written for displaying types in dropdown 
            //Start 
            vm.Matters = [{ Id: 1, Name: "All Matters" }, { Id: 2, Name: "My Matters" }, { Id: 3, Name: "Pinned Matters" }];
            vm.ddlMatters = vm.Matters[0];


            //#endregion  

            vm.Pinnedobj = [];
            vm.getMatterPinned = function () {

                var pinnedMattersRequest = {
                    Id: "123456",
                    Name: "Microsoft",
                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                }
                getPinnedMatters(pinnedMattersRequest, function (response) {
                    vm.Pinnedobj.push(response);
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                });
                return true;
            }

            vm.getMatterPinned();

            //#region Request object for the GetMattersMethod
            var MattersRequest = {
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


            //#region Hits when the Dropdown changes 
            //Start 
            vm.GetMatters = function (id) {
                vm.lazyloader = false;
                if (id == 1) {
                    MattersRequest.SearchObject.Filters.FilterByMe = 0;
                    get(MattersRequest, function (response) {
                        vm.lazyloader = true;
                        if (response.errorCode == "404") {
                            vm.divuigrid = false;
                            vm.nodata = true;
                            $scope.errorMessage = response.message;
                        } else {
                            vm.getMatterPinned();
                            vm.divuigrid = true;
                            vm.nodata = false;
                            if (vm.Pinnedobj.length > 0) {
                                angular.forEach(vm.Pinnedobj, function (pinobj) {
                                    angular.forEach(response, function (res) {
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
                        }
                    });
                } else if (id == 2) {
                    vm.lazyloader = false;
                    MattersRequest.SearchObject.Filters.FilterByMe = 1;
                    get(MattersRequest, function (response) {
                        vm.lazyloader = true;
                        if (response.errorCode == "404") {
                            vm.divuigrid = false;
                            vm.nodata = true;
                            $scope.errorMessage = response.message;
                        } else {
                            vm.getMatterPinned();
                            vm.divuigrid = true;
                            vm.nodata = false;
                            if (vm.Pinnedobj.length > 0) {
                                angular.forEach(vm.Pinnedobj, function (pinobj) {
                                    angular.forEach(response, function (res) {
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
                        }
                    });
                } else if (id == 3) {
                    vm.lazyloader = false;
                    var pinnedMattersRequest = {
                        Id: "123456",
                        Name: "Microsoft",
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
                    }
                    getPinnedMatters(pinnedMattersRequest, function (response) {
                        vm.lazyloader = true;
                        if (response.errorCode == "404") {
                            vm.divuigrid = false;
                            vm.nodata = true;
                            $scope.errorMessage = response.message;
                        } else {
                            vm.getMatterPinned();
                            vm.divuigrid = true;
                            vm.nodata = false;
                            angular.forEach(response.matterDataList, function (res) {
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
                        Id: "123456",
                        Name: "Microsoft",
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
                    },
                    matterData: {
                        matterName: alldata.matterUrl,
                    }
                }
                UnpinMatters(unpinRequest, function (response) {
                    if (response.isMatterUnPinned) {
                        $timeout(function () { vm.GetMatters(vm.ddlMatters.Id); }, 500);
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
                        Id: "123456",
                        Name: "Microsoft",
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
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
                        $timeout(function () { vm.GetMatters(vm.ddlMatters.Id); }, 500);
                        alert("Success");
                    }
                });
            }
            //#endregion


            //#region To display modal up in center of the screen...
            //Start 
            function reposition() {
                var modal = $(this),
                dialog = modal.find('.modal-dialog');
                modal.css('display', 'block');
                // Dividing by two centers the modal exactly, but dividing by three  
                // or four works better for larger screens. 
                dialog.css("margin-top", Math.max(0, (jQuery(window).height() - dialog.height()) / 2));
            }
            // Reposition when a modal is shown 
            jQuery('.modal').on('show.bs.modal', reposition);
            // Reposition when the window is resized 
            jQuery(window).on('resize', function () {
                jQuery('.modal:visible').each(reposition);
            });

            $timeout(reposition(), 100);
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
                        AOLList: "",
                        ClientsList: [],
                        FilterByMe: 1,
                        FromDate: "",
                        PGList: "",
                        ToDate: ""
                    },
                    Sort:
                            {
                                ByProperty: '',
                                Direction: 0
                            }
                }
            }

            $scope.FilterByType = function () {
                get(SortRequest, function (response) {
                    vm.lazyloader = true;
                    if (response.errorCode == "404") {
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

            $scope.sortChanged = function (grid, sortColumns) {
                vm.divuigrid = false;
                vm.nodata = true;
                if (sortColumns.length != 0) {
                    if (sortColumns[0].name == vm.gridOptions.columnDefs[0].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (localStorage.MatterNameSort == undefined || localStorage.MatterNameSort == "asc") {
                                vm.lazyloader = false;
                                SortRequest.SearchObject.Sort.ByProperty = "MCMatterName";
                                SortRequest.SearchObject.Sort.Direction = 0;
                                $scope.FilterByType();
                                localStorage.MatterNameSort = "desc";
                            } else {
                                vm.lazyloader = false;
                                SortRequest.SearchObject.Sort.ByProperty = "MCMatterName";
                                SortRequest.SearchObject.Sort.Direction = 1;
                                $scope.FilterByType();
                                localStorage.MatterNameSort = "asc";
                            }
                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[1].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (localStorage.ClientSort == undefined || localStorage.ClientSort == "asc") {
                                vm.lazyloader = false;
                                SortRequest.SearchObject.Sort.ByProperty = "MCClientName";
                                SortRequest.SearchObject.Sort.Direction = 0;
                                $scope.FilterByType();
                                localStorage.ClientSort = "desc";
                            }
                            else {
                                vm.lazyloader = false;
                                SortRequest.SearchObject.Sort.ByProperty = "MCClientName";
                                SortRequest.SearchObject.Sort.Direction = 1;
                                $scope.FilterByType();
                                localStorage.ClientSort = "asc";
                            }
                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[2].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (localStorage.ClientIDSort == undefined || localStorage.ClientIDSort == "asc") {
                                vm.lazyloader = false;
                                SortRequest.SearchObject.Sort.ByProperty = "MCClientID";
                                SortRequest.SearchObject.Sort.Direction = 0;
                                $scope.FilterByType();
                                localStorage.ClientIDSort = "desc";
                            } else {
                                vm.lazyloader = false;
                                SortRequest.SearchObject.Sort.ByProperty = "MCClientID";
                                SortRequest.SearchObject.Sort.Direction = 1;
                                $scope.FilterByType();
                                localStorage.ClientIDSort = "asc";
                            }

                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[3].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (localStorage.ModiFiedTimeSort == undefined || localStorage.ModiFiedTimeSort == "asc") {
                                vm.lazyloader = false;
                                SortRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                                SortRequest.SearchObject.Sort.Direction = 0;
                                $scope.FilterByType();
                                localStorage.ModiFiedTimeSort = "desc";
                            } else {
                                vm.lazyloader = false;
                                SortRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                                SortRequest.SearchObject.Sort.Direction = 1;
                                $scope.FilterByType();
                                localStorage.ModiFiedTimeSort = "asc";
                            }

                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[4].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (localStorage.ResAttoSort == undefined || localStorage.ResAttoSort == "asc") {
                                vm.lazyloader = false;
                                SortRequest.SearchObject.Sort.ByProperty = "MCResponsibleAttorney";
                                SortRequest.SearchObject.Sort.Direction = 0;
                                $scope.FilterByType();
                                localStorage.ResAttoSort = "desc";
                            } else {
                                vm.lazyloader = false;
                                SortRequest.SearchObject.Sort.ByProperty = "MCResponsibleAttorney";
                                SortRequest.SearchObject.Sort.Direction = 1;
                                $scope.FilterByType();
                                localStorage.ResAttoSort = "asc";
                            }
                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[5].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (localStorage.SubAreaSort == undefined || localStorage.SubAreaSort == "asc") {
                                vm.lazyloader = false;
                                SortRequest.SearchObject.Sort.ByProperty = "MCSubAreaofLaw";
                                SortRequest.SearchObject.Sort.Direction = 0;
                                $scope.FilterByType();
                                localStorage.SubAreaSort = "desc";
                            } else {
                                vm.lazyloader = false;
                                SortRequest.SearchObject.Sort.ByProperty = "MCSubAreaofLaw";
                                SortRequest.SearchObject.Sort.Direction = 1;
                                $scope.FilterByType();
                                localStorage.SubAreaSort = "desc";
                            }
                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }
                    }
                    else if (sortColumns[0].name == vm.gridOptions.columnDefs[6].name) {
                        if (sortColumns[0].sort != undefined) {
                            if (localStorage.OpenDateSort == undefined || localStorage.OpenDateSort == "asc") {
                                vm.lazyloader = false;
                                SortRequest.SearchObject.Sort.ByProperty = "MCOpenDate";
                                SortRequest.SearchObject.Sort.Direction = 0;
                                $scope.FilterByType();
                                localStorage.OpenDateSort = "desc"
                            } else {
                                vm.lazyloader = false;
                                SortRequest.SearchObject.Sort.ByProperty = "MCOpenDate";
                                SortRequest.SearchObject.Sort.Direction = 1;
                                $scope.FilterByType();
                                localStorage.OpenDateSort = "asc"
                            }
                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                        }
                    }
                } else {
                    vm.lazyloader = false;
                    SortRequest.SearchObject.Sort.ByProperty = "MCMatterName";
                    SortRequest.SearchObject.Sort.Direction = 0;
                    $scope.FilterByType();
                }
            }
            //#endregion

            //#region setting the grid options when window is resized

            angular.element($window).bind('resize', function () {
                if ($window.innerWidth < 340) {
                    vm.gridOptions.enableGridMenu = false;
                    vm.gridOptions.enablePaginationControls = false;
                    vm.gridOptions.columnDefs = [{ field: 'matterName', displayName: 'Matter', enableHiding: false, width: "100%", cellTemplate: '../app/matter/MatterCellTemplate.html', headerCellTemplate: '../app/matter/MatterHeaderTemplate.html' }];
                    $scope.$apply();
                } else {
                    vm.gridOptions = {
                        paginationPageSizes: [10, 50, 100],
                        paginationPageSize: 10,
                        enableGridMenu: true,
                        enableRowHeaderSelection: false,
                        enableRowSelection: true,
                        enableSelectAll: false,
                        multiSelect: false,
                        enablePaginationControls: true,
                        columnDefs: [
                             { field: 'matterName', displayName: 'Matter', enableHiding: false, width: "20%", cellTemplate: '../app/matter/MatterCellTemplate.html', headerCellTemplate: '../app/matter/MatterHeaderTemplate.html' },
                             { field: 'matterClient', displayName: 'Client', enableCellEdit: true, width: "15%", headerCellTemplate: '../app/matter/ClientHeaderTemplate.html' },
                             { field: 'matterClientId', displayName: 'Client.MatterID', width: "15%", headerTooltip: 'Click to sort by client.matterid', cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.matterClientId}}.{{row.entity.matterID}}</div>', enableCellEdit: true, },
                             { field: 'matterModifiedDate', displayName: 'Modified Date', width: "10%", cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.matterModifiedDate}}"></div>', headerCellTemplate: '../app/matter/ModifiedDateTemplate.html' },
                             { field: 'matterResponsibleAttorney', headerTooltip: 'Click to sort by attorney', width: "15%", displayName: 'Responsible attorney', visible: false },
                             { field: 'matterSubAreaOfLaw', headerTooltip: 'Click to sort by sub area of law', width: "15%", displayName: 'Sub area of law', visible: false },
                             { field: 'matterCreatedDate', headerTooltip: 'Click to sort by matter open date', width: "15%", displayName: 'Open date', cellTemplate: '<div class="ui-grid-cell-contents" datefilter date="{{row.entity.matterCreatedDate}}"></div>', visible: false },
                        ],
                        enableColumnMenus: false,
                        onRegisterApi: function (gridApi) {
                            $scope.gridApi = gridApi;
                            gridApi.core.on.columnVisibilityChanged($scope, function (changedColumn) {
                                $scope.columnChanged = { name: changedColumn.colDef.name, visible: changedColumn.colDef.visible };
                            });
                            gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                                vm.selectedRow = row.entity
                            });
                            $scope.gridApi.core.on.sortChanged($scope, $scope.sortChanged);
                            $scope.sortChanged($scope.gridApi.grid, [vm.gridOptions.columnDefs[1]]);
                        }
                    };
                }
            });

            //#endregion

        }]);

    app.directive('popover', function () {
        return {
            restrict: 'AE',
            scope: { details: '@' },
            link: function (scope, element, attrs) {
                scope.$watch("details", function () {
                    var obj = eval('(' + attrs.details + ')');
                    var content = '<div class="">\
                                   <div class="FlyoutBoxContent" style="width: 350px;">\
                                      <div class="FlyoutContent FlyoutHeading">\
                                          <div class="ms-Callout-content FlyoutHeadingText">  ' + obj.matterName + ' </div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Client:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterClient + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Client.Matter ID:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterClientId + '.' + obj.matterID + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Sub area of law:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterSubAreaOfLaw + '</div> \
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Responsible attorney:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterResponsibleAttorney + '</div>\
                                       </div>\
                                       <button class="ms-Button ms-Button--primary ms-Callout-content" id="viewMatters"><a class="ms-Button-label" href="https://msmatter.sharepoint.com/sites/microsoft/SitePages/' + obj.matterGuid + '.aspx" target="_blank">View matter details</a></button>\
                                       <button class="ms-Button ms-Button--primary ms-Callout-content" id="uploadToMatter"><a class="ms-Button-label" onclick="Openuploadmodal(\'' + obj.matterName + '\',\'' + obj.matterUrl + '\')" type="button">Upload to a matter</a></button>\
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


    app.directive('showbreadcrumb', function ($rootScope) {
        return {
            restrict: 'AE',
            link: function (scope, element, attrs) {
                $(element).find('ul li ul li').on("dblclick", function (e) {
                    $rootScope.breadcrumb = false;
                    $rootScope.foldercontent = true;
                    $rootScope.$apply();
                    var text = $(this).find('div').attr('title');
                    $('#breadcrumb #currentFolder').html(text);
                });

            }
        }
    });

    app.directive('datefilter', function () {
        return {
            restrict: 'A',
            scope: { date: '@' },
            link: function (scope, element, attrs) {
                scope.$watch("date", function () {
                    var dDate = attrs.date;
                    var oDates = new Date(dDate), months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], date = "", oLocalDate = dDate;
                    if (isNaN(oDates)) {
                        var arrSplitedDate = dDate.replace(/[-]/g, "/");
                        arrSplitedDate = arrSplitedDate.split("/");
                        dDate = arrSplitedDate[1] + "-" + arrSplitedDate[0] + "-" + arrSplitedDate[2];
                        oDates = new Date(dDate);
                    }
                    if (0 > oLocalDate.indexOf("Z")) {
                        date += months[parseInt(oDates.getMonth(), 10)] + " ";
                        date += oDates.getDate() + ", ";
                        date += oDates.getFullYear();
                    } else {
                        date += months[parseInt(oDates.getUTCMonth(), 10)] + " ";
                        date += oDates.getUTCDate() + ", ";
                        date += oDates.getUTCFullYear();
                    }
                    $(element).html(date);
                    return date;
                });
            }
        }
    });

})();

function Openuploadmodal(mattername, matterurl) {
     jQuery('#UploadMatterModal').modal("show");
    console.log(mattername);
    console.log(matterurl);
}