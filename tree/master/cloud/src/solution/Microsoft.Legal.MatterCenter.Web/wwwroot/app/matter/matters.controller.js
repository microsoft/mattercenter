(function () {
    'use strict';

    var app = angular.module("matterMain");

    app.controller('mattersController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'matterResource', '$rootScope', 'uiGridConstants', '$location', '$http',
function ($scope, $state, $interval, $stateParams, api, $timeout, matterResource, $rootScope, uiGridConstants, $location, $http) {
    var vm = this;
    vm.selected = undefined;
    // Onload show ui grid and hide error div
    //start
    $scope.divuigrid = true;
    $scope.nodata = false;
    //end

    //to hide lazyloader on load
    //start
    $scope.lazyloader = true;
    //end

    //Assigning html for Matterheadertemplate
    //Start
    $scope.matterDropDowm = false;
    var MatterHeaderTemplate = "<div ng-mouseenter='grid.appScope.matterDropDowm = true' ng-mouseleave='grid.appScope.matterDropDowm = false'>\
    <div class='dropdown keep-open' style='float:right;' ng-show='grid.appScope.matterDropDowm'>\
        <a href='javascript:;' dropdown class='prisma-header-dropdown-anchor id='acombo' dropdown-toggle' type='button' data-toggle='dropdown'>\
            <img src='../images/icon-combobox.png'/>\
        </a>\
        <div class='dropdown-menu flyoutWrapper dropdown-menu-right' ng-click='$event.stopPropagation();' role='menu'  aria-labelledby='acombo'>\
         <div class='input-group'>\
           <input class='form-control' ng-model='grid.appScope.vm.searchTerm' placeholder='Search'/> \
                <div class='input-group-btn'>\
                 <button type='button' ng-click='grid.appScope.vm.searchMatterGrid()' class='btn btn-default' ><img src='../images/search-15x15-666.png' /></button>\
                 </div>\
         </div>\
         <hr/> \
            <div class='clearFilterText' data-clearfiltertype='text'>\
            <div class='clearFiltersIcon'>\
                <img src='../Images/Filters_30px_X_30px_active_color_666.png' alt='clear' title='Clear filters'>\
            </div>\
            <div class='ms-font-m ms-font-weight-semilight clearText' title='Clear filters from Matter'><span>Clear filters from </span><span class='clearFilterTitle'>Matter</span></div>\
             <div id='filterResultsContainer'><div class='filterValueLabels ms-font-m ms-font-weight-semilight' ng-repeat='matter in grid.appScope.matters' ng-click='grid.appScope.filterMatterName(matter.matterName)'>{{matter.matterName}}</div> </div>\
        </div>\
        </div>\
    </div>\
    <div role='button' style='padding-left: 11px;' class='ui-grid-cell-contents ui-grid-header-cell-primary-focus' col-index='renderIndex'>\
        <span class='ui-grid-header-cell-label ng-binding' title='Click to sort by matter name'>{{ col.colDef.displayName }}</span>\
        <span ui-grid-visible='col.sort.direction' aria-label='{{getSortDirectionAriaLabel()}}' class='ui-grid-invisible'><sub ui-grid-visible='isSortPriorityVisible()' class='ui-grid-sort-priority-number'>{{col.sort.priority + 1}}</sub></span>\
        <span class='sort pull-right' ng-show='grid.appScope.sortMCMatterName'>↑</span>\
        <span ng-show='grid.appScope.sortDownMCMatterName' class='sort pull-right'>↓</span>\
    </div>\
    <div class='ui-grid-column-menu-button ng-scope' ng-if='grid.options.enableColumnMenus && !col.isRowHeader && col.colDef.enableColumnMenu !== false' ng-click='grid.appScope.toggleMenu($event)' ng-class='{'ui-grid-column-menu-button-last-col': isLastCol}'>\
        <i  title='' aria-hidden='true' class='ui-grid-icon-up-dir'>&nbsp;</i>\
    </div>\
</div>"
    //End


    //Assigning html for Clientheadertemplate
    //Start
    $scope.clientDropDowm = false;
    var ClientHeaderTemplate = "<div ng-mouseenter='grid.appScope.clientDropDowm = true' ng-mouseleave='grid.appScope.clientDropDowm = false'>\
    <div class='dropdown keep-open' style='float:right;' ng-show='grid.appScope.clientDropDowm'>\
        <a href='javascript:;' dropdown class='prisma-header-dropdown-anchor id='acombo' dropdown-toggle' type='button' data-toggle='dropdown'>\
            <img src='../images/icon-combobox.png'/>\
        </a>\
        <div class='dropdown-menu flyoutWrapper dropdown-menu-right' ng-click='$event.stopPropagation();' role='menu'  aria-labelledby='acombo'>\
         <div class='input-group'>\
           <input class='form-control' ng-model='grid.appScope.vm.searchClientTerm' placeholder='Search'/> \
                <div class='input-group-btn'>\
                 <button type='button' ng-click='grid.appScope.vm.searchClient()' class='btn btn-default' ><img src='../images/search-15x15-666.png' /></button>\
                 </div>\
         </div>\
         <hr/> \
            <div class='clearFilterText' data-clearfiltertype='text'>\
            <div class='clearFiltersIcon'>\
                <img src='../Images/Filters_30px_X_30px_active_color_666.png' alt='clear' title='Clear filters'>\
            </div>\
            <div class='ms-font-m ms-font-weight-semilight clearText' title='Clear filters from Client'><span>Clear filters from </span><span class='clearFilterTitle'>Client</span></div>\
             <div id='filterResultsContainer'><div class='filterValueLabels ms-font-m ms-font-weight-semilight' ng-repeat='client in grid.appScope.Clients' ng-click='grid.appScope.filterClientName(Client.matterClient)'>{{client.matterClient}}</div> </div>\
        </div>\
        </div>\
    </div>\
    <div role='button' class='ui-grid-cell-contents ui-grid-header-cell-primary-focus' col-index='renderIndex'>\
        <span class='ui-grid-header-cell-label ng-binding' title='Click to sort by client name'>{{ col.colDef.displayName }}</span>\
        <span ui-grid-visible='col.sort.direction' aria-label='Sort None' class='ui-grid-invisible'></span>\
        <span class='sort pull-right' ng-show='grid.appScope.sortMCClient'>↑</span>\
        <span ng-show='grid.appScope.sortDownMCClient' class='sort pull-right'>↓</span>\
    </div>\
    <div class='ui-grid-column-menu-button ng-scope' ng-if='grid.options.enableColumnMenus && !col.isRowHeader && col.colDef.enableColumnMenu !== false' ng-click='grid.appScope.toggleMenu($event)' ng-class='{'ui-grid-column-menu-button-last-col': isLastCol}'>\
        <i  title='' aria-hidden='true' class='ui-grid-icon-up-dir'>&nbsp;</i>\
    </div>\
</div>"
    //End

    //Assigning html for ModifiedDateheadertemplate
    //Start
    $scope.modifieddateDropDowm = false;
    var ModifiedDateheadertemplate = "<div ng-mouseenter='grid.appScope.modifieddateDropDowm = true' ng-mouseleave='grid.appScope.modifieddateDropDowm = false'>\
    <div class='dropdown keep-open' style='float:right;' ng-show='grid.appScope.modifieddateDropDowm'>\
        <a href='javascript:;' dropdown class='prisma-header-dropdown-anchor id='acombo' dropdown-toggle' type='button' data-toggle='dropdown'>\
            <img src='../images/icon-combobox.png'/>\
        </a>\
        <div class='dropdown-menu flyoutWrapper dropdown-menu-right' ng-click='$event.stopPropagation();' role='menu'  aria-labelledby='acombo'>\
         <div class='input-group'>\
             <input type='text' placeholder='Start mm/dd/yyyy' class='calendar form-control'\
                    uib-datepicker-popup='MM/dd/yyyy'\
                    ng-model='grid.appScope.$parent.startdate'\
                    is-open='grid.appScope.openedStartDate' \
                    datepicker-options='grid.appScope.dateOptions'\
                    ng-required='true' close-text='Close'\
                    alt-input-formats='altInputFormats'/>\
                <span class='input-group-btn'>\
                    <button type='button' class='btn btn-default' ng-click='grid.appScope.openStartDate()'><i class='glyphicon glyphicon-calendar'></i></button>\
                </span>\
          </div>\
          <div class='input-group' style='margin-top:5px'>\
             <input type='text' placeholder='End mm/dd/yyyy' class='calendar form-control'\
                    uib-datepicker-popup='MM/dd/yyyy'\
                    ng-model='grid.appScope.$parent.enddate'\
                    is-open='grid.appScope.openedEndDate' \
                    datepicker-options='grid.appScope.enddateOptions'\
                    ng-required='true' close-text='Close'\
                    alt-input-formats='altInputFormats' />\
                <span class='input-group-btn'>\
                    <button type='button' class='btn btn-default' ng-click='grid.appScope.openEndDate()'><i class='glyphicon glyphicon-calendar'></i></button>\
                </span>\
          </div>\
          <div class='okDateButton' id='btnOK' ng-click='grid.appScope.FilterModifiedDate()'>Ok</div>\
          <hr/> \
            <div class='clearFilterText' data-clearfiltertype='text'>\
            <div class='clearFiltersIcon'>\
                <img src='../Images/Filters_30px_X_30px_active_color_666.png' alt='clear' title='Clear filters'>\
            </div>\
            <div class='ms-font-m ms-font-weight-semilight clearText' title='Clear filters from Modified Date'><span>Clear filters from </span><span class='clearFilterTitle'>Modified Date</span></div>\
             <div id='filterResultsContainer'><div class='filterValueLabels ms-font-m ms-font-weight-semilight' ng-repeat='client in grid.appScope.Clients' ng-click='grid.appScope.filterClientName(Client.matterClient)'>{{client.matterClient}}</div> </div>\
        </div>\
        </div>\
    </div>\
    <div role='button' class='ui-grid-cell-contents ui-grid-header-cell-primary-focus' col-index='renderIndex'>\
        <span class='ui-grid-header-cell-label ng-binding' title='Click to sort last modified date'>{{ col.colDef.displayName }}</span>\
        <span ui-grid-visible='col.sort.direction' aria-label='Sort None' class='ui-grid-invisible'></span>\
        <span class='sort pull-right' ng-show='grid.appScope.sortMCModifiedDate'>↑</span>\
        <span ng-show='grid.appScope.sortDownMCModifiedDate' class='sort pull-right'>↓</span>\
    </div>\
    <div class='ui-grid-column-menu-button ng-scope' ng-if='grid.options.enableColumnMenus && !col.isRowHeader && col.colDef.enableColumnMenu !== false' ng-click='grid.appScope.toggleMenu($event)' ng-class='{'ui-grid-column-menu-button-last-col': isLastCol}'>\
        <i  title='' aria-hidden='true' class='ui-grid-icon-up-dir'>&nbsp;</i>\
    </div>\
</div>"
    //End

    //Assigning html for celltemplate
    //Start
    var matterCellTemplate = "<div class='row'>\
    <div class='col-xs-7 col-sm-9 col-md-9 col-lg-10' id='matterPopup'>\
        <a popover type='button' class='btn btn-link col-xs-12 col-sm-12' style='text-align:left' details={{row.entity}} data-toggle='popover' data-container='body' data-placement='right' type='button' data-html='true' href='' > {{row.entity.matterName}} </a>\
        <div class='popover-content'></div>\
    </div>\
    <div class='col-xs-5 col-sm-3 col-md-3 col-lg-2 text-right'>\
        <div class='dropdown'>\
            <a class='btn-link dropdown-toggle ms-Icon ms-Icon--ellipsis ellipsis UiGrid-a' type='button' data-toggle='dropdown'></a><ul style='margin:0;padding:0' class='dropdown-menu'>\
                <li class='ms-ContextualMenu-item' ng-click='grid.appScope.Openuploadmodal()'><a class='ECBItem ms-ContextualMenu-link upload'>Upload to this Matter</a></li>\
                <li class='ms-ContextualMenu-item'><a class='ECBItem ms-ContextualMenu-link upload' href='https://msmatter.sharepoint.com/sites/microsoft/SitePages/{{row.entity.matterGuid}}.aspx' target='_blank'>View Matter Details</a></li>\
                <li class='ms-ContextualMenu-item'>\
                    <a class='ECBItem ms-ContextualMenu-link upload' href='https://msmatter.sharepoint.com/sites/microsoft/' target='_blank'>\
                        Go to Matter OneNote\
                    </a>\
                </li>\
                <li class='ms-ContextualMenu-item' ng-click='row.entity.MatterInfo===undefined?grid.appScope.PinMatter(row):grid.appScope.UnpinMatter(row)'><a class='ECBItem ms-ContextualMenu-link upload'>{{row.entity.MatterInfo===undefined?'Pin this Matter':'Unpin this matter'}}</a></li>\
            </ul>\
        </div>\
    </div>\
</div>";
    //End

    $scope.initOfficeLibrary = function () {

    };

    vm.gridOptions = {
        paginationPageSizes: [6, 50, 100],
        paginationPageSize: 6,
        enableGridMenu: true,
        enableRowHeaderSelection: false,
        enableRowSelection: true,
        enableSelectAll: false,
        multiSelect: false,
        columnDefs: [
            { field: 'matterName', displayName: 'Matter', enableHiding: false, cellTemplate: matterCellTemplate, headerCellTemplate: MatterHeaderTemplate },
            { field: 'matterClient', displayName: 'Client', enableCellEdit: true, headerCellTemplate: ClientHeaderTemplate },
            { field: 'matterClientId', displayName: 'Client.MatterID', headerTooltip: 'Click to sort by client.matterid', cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.matterClientId}}.{{row.entity.matterID}}</div>', enableCellEdit: true, },
            { field: 'matterModifiedDate', displayName: 'Modified Date', cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.matterModifiedDate}}"></div>', headerCellTemplate: ModifiedDateheadertemplate },
            { field: 'matterResponsibleAttorney', headerTooltip: 'Click to sort by attorney', displayName: 'Responsible attorney', visible: false },
            { field: 'matterSubAreaOfLaw', headerTooltip: 'Click to sort by sub area of law', displayName: 'Sub area of law', visible: false },
            { field: 'matterCreatedDate', headerTooltip: 'Click to sort by matter open date', displayName: 'Open date', cellTemplate: '<div class="ui-grid-cell-contents" datefilter date="{{row.entity.matterCreatedDate}}"></div>', visible: false },
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
        vm.files = sourceFiles.files;
        var fd = new FormData();
        angular.forEach(vm.files, function (file) {
            fd.append('file', file);
            fd.append('targetDropUrl', targetDrop.url);
            fd.append('folderUrl', targetDrop.url)
            fd.append('documentLibraryName', targetDrop.name)
            fd.append('clientUrl', 'https://msmatter.sharepoint.com/sites/microsoft"');
        })

        $http.post("/api/v1/document/uploadfiles", fd, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        });
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


    //For Searching Matter in GridHeader Menu
    //Start
    vm.searchMatterGrid = function () {
        $scope.lazyloader = false;

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
                              ByProperty: "MCMatterName",
                              Direction: 0
                          }
              }
          };
        get(searchRequest, function (response) {
            $scope.lazyloader = true;
            $scope.matters = response;
        });
    }
    //End

    //For Searching client in GridHeader Menu
    //start
    vm.searchClient = function () {
        $scope.lazyloader = false;

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
                  SearchTerm: vm.searchClientTerm,
                  Filters: {},
                  Sort:
                          {
                              ByProperty: "MCClientName",
                              Direction: 0
                          }
              }
          };
        get(searchRequest, function (response) {
            $scope.lazyloader = true;
            $scope.Clients = response;
        });
    }
    //end

    //For filtering mattername 
    //Start
    $scope.filterMatterName = function (mattername) {
        $scope.lazyloader = false;
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
                  SearchTerm: mattername,
                  Filters: {},
                  Sort:
                          {
                              ByProperty: "LastModifiedTime",
                              Direction: 1
                          }
              }
          };
        get(searchRequest, function (response) {
            $scope.lazyloader = true;
            vm.gridOptions.data = response;
            $scope.matters = [];
        });

    }

    //End


    //For filtering Clientname 
    //Start
    $scope.filterClientName = function (clientname) {
        $scope.lazyloader = false;
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
                  SearchTerm: clientname,
                  Filters: {},
                  Sort:
                          {
                              ByProperty: "LastModifiedTime",
                              Direction: 1
                          }
              }
          };
        get(searchRequest, function (response) {
            $scope.lazyloader = true;
            vm.gridOptions.data = response;
            $scope.Clients = [];
        });

    }

    //End


    //Code for filtering ModifiedDate
    //start
    $scope.FilterModifiedDate = function () {
        $scope.lazyloader = false;
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
                  SearchTerm: "",
                  Filters: {
                      OLList: "",
                      ClientName: "",
                      ClientsList: [],
                      DateFilters: { CreatedFromDate: "", CreatedToDate: "", ModifiedFromDate: $scope.startdate, ModifiedToDate: $scope.enddate, OpenDateFrom: "", OpenDateTo: "" },
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
        get(searchRequest, function (response) {
            $scope.lazyloader = true;
            vm.gridOptions.data = response;
            $scope.startdate = "";
            $scope.enddate = "";
        });

    }

    //end

    //Code written for displaying types in dropdown 
    //Start 
    $scope.Matters = [{ Id: 1, Name: "All Matters" }, { Id: 2, Name: "My Matters" }, { Id: 3, Name: "Pinned Matters" }];
    $scope.ddlMatters = $scope.Matters[0];


    //End  

    $scope.Pinnedobj = [];
    $scope.getMatterPinned = function () {

        var pinnedMattersRequest = {
            Id: "123456",
            Name: "Microsoft",
            Url: "https://msmatter.sharepoint.com/sites/catalog"
        }
        getPinnedMatters(pinnedMattersRequest, function (response) {
            for (var i = 0; i < response.matterDataList.length; i++) {
                $scope.Pinnedobj.push(response.matterDataList[i]);
            }
        });
        return true;
    }

    $scope.getMatterPinned();


    //Hits when the Dropdown changes 
    //Start 
    $scope.GetMatters = function (id) {
        $scope.lazyloader = false;
        if (id == 1) {
            var AllMattersRequest = {
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


            get(AllMattersRequest, function (response) {
                $scope.lazyloader = true;
                if (response.errorCode == "404") {
                    $scope.divuigrid = false;
                    $scope.nodata = true;
                    $scope.errorMessage = response.message;
                } else {
                    $scope.getMatterPinned();
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                    if ($scope.Pinnedobj.length > 0) {
                        angular.forEach($scope.Pinnedobj, function (pinobj) {
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
                        console.log(response);
                    } else {
                        vm.gridOptions.data = response;
                    }
                }
            });


        } else if (id == 2) {
            $scope.lazyloader = false;
            var MyMattersRequest = {
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
                                ByProperty: "LastModifiedTime",
                                Direction: 1
                            }
                }
            }


            get(MyMattersRequest, function (response) {
                $scope.lazyloader = true;
                if (response.errorCode == "404") {
                    $scope.divuigrid = false;
                    $scope.nodata = true;
                    $scope.errorMessage = response.message;
                } else {
                    $scope.getMatterPinned();
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                    if ($scope.Pinnedobj.length > 0) {
                        angular.forEach($scope.Pinnedobj, function (pinobj) {
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
                    } else {
                        vm.gridOptions.data = response;
                    }
                }
            });
        } else if (id == 3) {
            $scope.lazyloader = false;
            var pinnedMattersRequest = {
                Id: "123456",
                Name: "Microsoft",
                Url: "https://msmatter.sharepoint.com/sites/catalog"
            }
            getPinnedMatters(pinnedMattersRequest, function (response) {
                $scope.lazyloader = true;
                if (response.errorCode == "404") {
                    $scope.divuigrid = false;
                    $scope.nodata = true;
                    $scope.errorMessage = response.message;
                } else {
                    $scope.getMatterPinned();
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                    angular.forEach(response.matterDataList, function (res) {
                        if (res.ismatterdone == undefined && !res.ismatterdone) {
                            res.MatterInfo = "Unpin this matter";
                            res.ismatterdone = true;
                        }
                    });
                    vm.gridOptions.data = response.matterDataList;
                }
            });
        }
    }
    //End 


    //To run GetMatters function on page load 
    $scope.GetMatters($scope.ddlMatters.Id);
    //End 


    //Written for unpinning the matter 
    //Start 
    $scope.UnpinMatter = function (data) {
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
                $timeout(function () { $scope.GetMatters($scope.ddlMatters.Id); }, 500);
                alert("Success");
            }
        });
    }
    //End 


    //Written for pinning the matter 
    //Start 
    $scope.PinMatter = function (data) {
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
                $timeout(function () { $scope.GetMatters($scope.ddlMatters.Id); }, 500);
                alert("Success");
            }
        });
    }
    //End 









    //To display modal up in center of the screen...
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
    //End 
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
    $rootScope.breadcrumb = true;
    $rootScope.foldercontent = false;

    $scope.hideBreadCrumb = function () {
        $rootScope.breadcrumb = true;
        $rootScope.foldercontent = false;

    }

    //Angular Datepicker Starts here
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

    $scope.disabled = function (date, mode) {
        return (mode === 'day' && (date.getDay() != 0));
    };



    //End

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
            $scope.lazyloader = true;
            if (response.errorCode == "404") {
                $scope.divuigrid = false;
                $scope.nodata = true;
                $scope.errorMessage = response.message;
            } else {
                $scope.divuigrid = true;
                $scope.nodata = false;
                vm.gridOptions.data = response;
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            }
        });
    }

    $scope.sortChanged = function (grid, sortColumns) {
        $scope.divuigrid = false;
        $scope.nodata = true;
        if (sortColumns.length != 0) {
            if (sortColumns[0].name == vm.gridOptions.columnDefs[0].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.MatterNameSort == undefined || localStorage.MatterNameSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCMatterName";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.MatterNameSort = "desc";
                    } else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCMatterName";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.MatterNameSort = "asc";
                    }
                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
            else if (sortColumns[0].name == vm.gridOptions.columnDefs[1].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.ClientSort == undefined || localStorage.ClientSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCClientName";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.ClientSort = "desc";
                    }
                    else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCClientName";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.ClientSort = "asc";
                    }
                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
            else if (sortColumns[0].name == vm.gridOptions.columnDefs[2].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.ClientIDSort == undefined || localStorage.ClientIDSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCClientID";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.ClientIDSort = "desc";
                    } else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCClientID";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.ClientIDSort = "asc";
                    }

                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
            else if (sortColumns[0].name == vm.gridOptions.columnDefs[3].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.ModiFiedTimeSort == undefined || localStorage.ModiFiedTimeSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.ModiFiedTimeSort = "desc";
                    } else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.ModiFiedTimeSort = "asc";
                    }

                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
            else if (sortColumns[0].name == vm.gridOptions.columnDefs[4].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.ResAttoSort == undefined || localStorage.ResAttoSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCResponsibleAttorney";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.ResAttoSort = "desc";
                    } else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCResponsibleAttorney";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.ResAttoSort = "asc";
                    }
                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
            else if (sortColumns[0].name == vm.gridOptions.columnDefs[5].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.SubAreaSort == undefined || localStorage.SubAreaSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCSubAreaofLaw";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.SubAreaSort = "desc";
                    } else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCSubAreaofLaw";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.SubAreaSort = "desc";
                    }
                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
            else if (sortColumns[0].name == vm.gridOptions.columnDefs[6].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.OpenDateSort == undefined || localStorage.OpenDateSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCOpenDate";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.OpenDateSort = "desc"
                    } else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCOpenDate";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.OpenDateSort = "asc"
                    }
                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
        } else {
            $scope.lazyloader = false;
            SortRequest.SearchObject.Sort.ByProperty = "MCMatterName";
            SortRequest.SearchObject.Sort.Direction = 0;
            $scope.FilterByType();
        }
    }
    //#endregion

    $scope.$watch('grid.options.totalItems', function (newval, oldval) {
        console.log(newval);
    });

}]);

    app.directive('popover', function () {
        return {
            restrict: 'AE',
            scope: { details: '@' },
            link: function (scope, element, attrs) {
                scope.$watch("details", function () {
                    var obj = eval('(' + attrs.details + ')');
                    var content = '<div class="">\
                                   <div class="FlyoutBoxContent">\
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
                                       <button class="ms-Button ms-Button--primary ms-Callout-content" id="uploadToMatter"><a class="ms-Button-label" onclick="$scope.Openuploadmodal()" type="button">Upload to a matter</a></button>\
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

