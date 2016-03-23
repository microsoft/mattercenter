/// <disable>JS3057,JS3058,JS2074,JS2076,JS3092,JS3054,JS2005,JS3056,JS2073,JS2024,JS2026,JS2032,JS2064,JS3116,JS3053</disable>

var oUploadGlobal = {
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

function closeAllPopup() {
    "use strict";
    $(".popUpCloseIcon").click();
    $(".errorPopUp").addClass("hide");
    oUploadGlobal.arrFiles = [];
    oUploadGlobal.arrContent = [];
    oUploadGlobal.sNotificationMsg = "";
    if ("object" === typeof commonFunction && jQuery.isFunction(commonFunction.clearGlobalVariables)) {
        commonFunction.clearGlobalVariables();
    }
    $(".folderStructureContent, .parentNode").removeClass("folderStructureWithBreadcrumb");
}

$(document).mousedown(function (e) {
    "use strict";
    if ($(e.target).is(".mailpopupContainerBackground")) {
        closeAllPopup();
    }

    if ($(e.target).is(".mailContainer")) {
        $(".errorPopUp").addClass("hide");
        oUploadGlobal.arrFiles = [];
        oUploadGlobal.arrContent = [];
    }
});

function showPopupNotification(sMsg, resultClass) {
    "use strict";
    var sContent = "";
    sContent = "<div class='notification " + resultClass + "'>" + sMsg + "</div>";
    $(".mailContainer .notification").remove();
    $(".mailContainer").prepend(sContent);
}

function buildNestedList(treeNodes, rootId) {
    "use strict";
    var nodesByParent = {};
    $.each(treeNodes, function (iIndex, node) {
        if (!(node.parenturl in nodesByParent)) { nodesByParent[node.parenturl] = []; }
        nodesByParent[node.parenturl].push(node);
    });

    function buildTree(children) {
        var $container = $("<ul>"),
            sDataClient = "data-client",
            sDataFolderName = "data-foldername",
            sDataParentName = "data-parentname";
        if (!children) { return; }
        $.each(children, function (iIndex, child) {
            if (null !== child.parenturl) {
                if (null !== oUploadGlobal.oDrilldownParameter.sCurrentParentUrl && oUploadGlobal.oDrilldownParameter.sCurrentParentUrl === child.parenturl) {
                    $("<li>", { html: "<div class='treeNodes' title='" + child.name + "'><img class='folderIcon' src='" + oGlobalConstants.Site_Url + oGlobalConstants.Layout_Folder_Icon + "' align='center' alt='" + child.name + "' data-foldername='" + child.url + "' data-parentname='" + child.parenturl + "'/>" + child.name + "</div>" })
                    .css({ display: "inline-block", width: "90%", padding: "0" })
                    .attr(sDataClient, oUploadGlobal.oDrilldownParameter.sRootUrl)
                    .attr(sDataFolderName, child.url)
                    .attr(sDataParentName, child.parenturl)
                    .addClass("childNode")
                    .appendTo($container)
                    .append(buildTree(nodesByParent[child.url]));
                } else if (null !== oUploadGlobal.oDrilldownParameter.sCurrentParentUrl && oUploadGlobal.oDrilldownParameter.sCurrentParentUrl === child.url) {
                    $("<li>", { html: "<div class='treeNodes hide' title='" + child.name + "'><img class='folderIcon' src='" + oGlobalConstants.Site_Url + oGlobalConstants.Layout_Folder_Icon + "' align='center' alt='" + child.name + "' data-foldername='" + child.url + "' data-parentname='" + child.parenturl + "'/>" + child.name + "</div>" })
                    .css({ display: "inline-block", width: "100%", padding: "0" })
                    .attr(sDataClient, oUploadGlobal.oDrilldownParameter.sRootUrl)
                    .attr(sDataFolderName, child.url)
                    .attr(sDataParentName, child.parenturl)
                    .addClass("parentNode")
                    .appendTo($container)
                    .append(buildTree(nodesByParent[child.url]));
                }
            } else {
                oUploadGlobal.oDrilldownParameter.sRootUrl = child.url;
                oUploadGlobal.oDrilldownParameter.sCurrentParentUrl = child.url;
                oUploadGlobal.oDrilldownParameter.nCurrentLevel++;
                $("<li>", { html: "<div class='treeNodes hide' title='" + child.name + "'><img class='folderIcon' src='" + oGlobalConstants.Site_Url + oGlobalConstants.Layout_Folder_Icon + "' align='center' alt='" + child.name + "' data-foldername='" + child.url + "' data-parentname='" + child.parenturl + "'/>" + child.name + "</div>" })
                .css({ display: "inline-block", width: "100%", padding: "0" })
                .attr(sDataClient, oUploadGlobal.oDrilldownParameter.sRootUrl)
                .attr(sDataFolderName, child.url)
                .attr(sDataParentName, child.parenturl)
                .addClass("parentNode")
                .appendTo($container)
                .append(buildTree(nodesByParent[child.url]));
            }
        });
        return $container;
    }
    return buildTree(nodesByParent[rootId]);
}

function folderHierarchySuccess(result) {
    "use strict";
    // Dynamically decrease the height of the popup
    oCommonObject.updateUploadPopupHeight(false);
    oCommonObject.getContentCheckConfigurations(oSearchGlobal.matterUrl);
    var nTemp = 0,
        oMatter = result.oParam,
        treeNodes = JSON.parse(result.Result),
        htmlChunk;
    if (0 < $(".notification").length) {
        $(".notificationContainerForPopup").find("div:first").remove();
    }
    $(".popupWait, .loadingImage").addClass("hide");
    result = result.Result;
    oSearchGlobal.oFolderName = treeNodes;
    if (treeNodes.code) {
        showCommonErrorPopUp(treeNodes.code);
        return;
    }
    htmlChunk = buildNestedList(treeNodes, null);
    $(".folderStructureContent").html(htmlChunk);

    // Making icons non draggable on upload popup
    $(".mailContainer img").attr("draggable", "false");
    $("#attachmentHeader").removeClass("hide");
    if (oCommonObject.getParameterByName("appType") === oGlobalConstants.Querystring_Outlook) {
        uploadMailClient();
    } else {
        $(".maildata").html("<div class='noFilesUploaded'>" + oGlobalConstants.No_Files_Uploaded + "</div>");
    }
    if (oMatter && oMatter[0]) {
        var sOriginalName, sMatterGUID;
        if (4 === oCommonObject.iCurrentGridViewData()) {
            sOriginalName = $(oMatter[0]).attr("data-mattername");
            sMatterGUID = $(oMatter[0]).attr("data-matterguid");
        } else {
            sOriginalName = $(oMatter[0]).attr("data-" + oGlobalConstants.Matter_Name);
            sMatterGUID = $(oMatter[0]).attr("data-" + oGlobalConstants.Matter_GUID);
        }
        oCommonObject.addUploadAttributes(sOriginalName, sMatterGUID);
        if ("undefined" !== typeof oMatter[0]) {
            $("#rootLink").attr("title", oMatter[0].innerHTML);
        }

        bindDroppable($(".folderStructureContent ul li"));
    }
    $("#mailBody").on("drop", ".folderStructureContent ul li", function (e, ui) {
        var sDataClient = "data-client",
            sDataFolderName = "data-foldername",
            clientRelativeUrl = "",
            folderUrl = "",
            isOverwrite = "",
            count = 0;
        e.stopPropagation();
        e.stopImmediatePropagation();
        e.preventDefault();
        if ($(this).hasClass("folderDragOver")) {
            $(this).removeClass("folderDragOver");
        }
        if ($(this).parent().parent().hasClass("folderDragOver")) {
            $(this).parent().parent().removeClass("folderDragOver");
        }

        if (e.dataTransfer && e.dataTransfer.files && 0 !== e.dataTransfer.files.length) {
            oSearchGlobal.files = e.dataTransfer.files;
            oSearchGlobal.dataArray.length = 0;

            clientRelativeUrl = "undefined" !== typeof $(e.target).attr(sDataClient) ? $(e.target).attr(sDataClient) : $(e.target).parent().attr(sDataClient);
            folderUrl = "undefined" !== typeof $(e.target).attr(sDataFolderName) ? $(e.target).attr(sDataFolderName) : $(e.target).parent().attr(sDataFolderName);
            isOverwrite = "False";
            for (count = 0; count < e.dataTransfer.files.count; count++) {
                oUploadGlobal.arrOverwrite[count] = "False";
            }
            // upload dropped file
            uploadFile(clientRelativeUrl, folderUrl, isOverwrite);
        } else {
            // upload mail/attachment
            uploadAttachment(e, ui);
        }
    });

    $(".mailpopupContainerBackground, .mailContainer, .commonMailBody").removeClass("hide");

}

function bindDroppable($Element) {
    "use strict";
    $Element.droppable({
        greedy: true,
        tolerance: "pointer",
    });

    $(".parentNode").on("dropover", function (event, ui) {
        var first = $(".childNode:first");
        var last = $(".childNode:last");
        if (first.length && last.length) {
            if ((event.originalEvent.pageX > first.offset().left) &&
                (event.originalEvent.pageY > first.offset().top) &&
                (event.originalEvent.pageX < (last.offset().left + last.width())) &&
                (event.originalEvent.pageY < (last.offset().top + last.height()))) {
                $(this).removeClass("folderDragOver");
            } else {
                $(this).addClass("folderDragOver");
                $(".childNode").removeClass("folderDragOver");
            }
        }
    }).on("dropout", function () {
        $(this).removeClass("folderDragOver");
    });

    $(".childNode").on("dropover", function () {
        $(".parentNode, .childNode").removeClass("folderDragOver");
        $(this).addClass("folderDragOver");
    }).on("dropout", function () {
        $(this).removeClass("folderDragOver");
    });
}

$(".popUpCloseIcon").click(function () {
    "use strict";
    $(".mailContainer .notification").remove();
    $(".mailpopupContainerBackground, .mailContainer").addClass("hide");
    $(".errorPopUp").addClass("hide");
    $(".folderStructureContent, .parentNode").removeClass("folderStructureWithBreadcrumb");
    if ("undefined" !== typeof commonFunction.clearGlobalVariables) {
        commonFunction.clearGlobalVariables();
    }
});

function folderHierarchyBeforeCall() {
    "use strict";
    $(".popupWait, .loadingImage").removeClass("hide");
}

function folderHierarchyFailure(result) {
    "use strict";
    $(".popupWait, .loadingImage").addClass("hide");
    showCommonErrorPopUp(result);
}

function populateFolderHierarchy(element) {
    "use strict";
    $(".loadingImage").css("position", "absolute");
    var matterName = $(element).siblings(".matterTitle").attr("data-mattername"), matterUrl = $(element).siblings(".matterTitle").attr("data-client"), MatterFolderDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "matterData": { "MatterName": matterName, "MatterUrl": matterUrl } };
    oSearchGlobal.matterUrl = matterUrl;
    var oMatter = $(element).siblings(".matterTitle");
    oCommonObject.callSearchService("GetFolderHierarchy", MatterFolderDetails, folderHierarchySuccess, folderHierarchyFailure, folderHierarchyBeforeCall, oMatter);
}

// Upload Mail
function uploadMailClient() {
    "use strict";
    if (Office.context.mailbox) {
        // Init mail app
        oSearchGlobal.oServiceRequest.attachmentToken = "";
        oSearchGlobal.oServiceRequest.ewsUrl = Office.context.mailbox.ewsUrl;
        oSearchGlobal.oServiceRequest.subject = Office.context.mailbox.item.subject;
        oSearchGlobal.oServiceRequest.MailId = Office.context.mailbox.item.itemId;
        oSearchGlobal.oServiceRequest.attachments = new Array();
        var iCounter = 0;
        if (Office.context.mailbox.item.attachments) {
            var attachmentLength = Office.context.mailbox.item.attachments.length;
            for (iCounter = 0; iCounter < attachmentLength; iCounter++) {
                oSearchGlobal.oServiceRequest.attachments[iCounter] = JSON.parse(JSON.stringify(Office.context.mailbox.item.attachments[iCounter]._data$p$0));
            }
            Office.context.mailbox.getCallbackTokenAsync(attachmentTokenCallbackEmailClient);
            showAttachmentLoading();
        } else {
            $(".maildata").html("<div class='noFilesUploaded'>" + oGlobalConstants.No_Files_Uploaded + "</div>");
        }
    }
}

function showAttachmentLoading() {
    "use strict";
    var sHTMLChunk = "";
    sHTMLChunk += "<div class='attachmentSection'><img alt='Loading' class='attachmentListLoading' src='../images/loading_metro.gif'/></div>";
    $(".maildata").html(sHTMLChunk);
}

function createMailPopup() {
    "use strict";
    var sHTMLChunk = "", sImageChunk = "", nIDCounter = 0;
    sHTMLChunk += "<div class='attachmentSection'>";
    var attachmentName = "", sAttachmentFileName = "", bHasEML = false, attachmentType = "", sContentType = "", sExtension = "", iconSrc = "";
    var mailSubject = checkEmptyorWhitespace(oSearchGlobal.oServiceRequest.subject);
    mailSubject = mailSubject.replace(oUploadGlobal.regularExtraSpace, "").replace(oUploadGlobal.regularInvalidCharacter, "").replace(oUploadGlobal.regularInvalidRule, ".").replace(oUploadGlobal.regularStartEnd, "");

    sHTMLChunk += "<div class='attachment'>";
    sHTMLChunk += "<div class='attachmentName mailName'><img id='" + nIDCounter + "editIcon' class='editUploadIcon' src='../Images/edit-666.png' title='Edit attachment name' onclick='editAttachment(this," + nIDCounter + ")' alt='Edit'/><img id='" + nIDCounter + "saveIcon' class='saveUploadIcon hide' src='../Images/save-666.png' title='Save attachment name' onclick='saveAttachment(this," + nIDCounter + ")' alt='Save'/><div id='" + nIDCounter + "attachment-Status' class='uploadSuccessStatus'></div><img class='attachIcon' id='" + nIDCounter + "attachIcon' src='../Images/attach_icon-15x15_666.png' alt='attachment icon'><input type='text' maxlength='128' id='" + nIDCounter + "attachmentText' class='popupName editPopupContent hide' /><div id='" + nIDCounter + "attachment' class='popupName' draggable='true' ondragstart='dragStart(this);' data-attachmentId= '" + Office.context.mailbox.item.itemId + "' title='" + mailSubject + "'>" + mailSubject + "</div><div id='" + nIDCounter + "attachment-Message' class='uploadSuccessMessage'></div>";
    sHTMLChunk += "</div>";
    sHTMLChunk += "</div>";

    for (var attachment in oSearchGlobal.oServiceRequest.attachments) {
        bHasEML = false;
        nIDCounter++;
        attachmentName = checkEmptyorWhitespace(oSearchGlobal.oServiceRequest.attachments[attachment].name);
        attachmentName = attachmentName.replace(oUploadGlobal.regularExtraSpace, "").replace(oUploadGlobal.regularInvalidCharacter, "").replace(oUploadGlobal.regularInvalidRule, ".").replace(oUploadGlobal.regularStartEnd, "");
        if (attachmentName.lastIndexOf(".eml") === attachmentName.length - 4) {
            sAttachmentFileName = attachmentName.substring(0, attachmentName.lastIndexOf(".eml"));
            bHasEML = true;
        } else {
            sAttachmentFileName = attachmentName;
        }
        attachmentType = oSearchGlobal.oServiceRequest.attachments[attachment].hasOwnProperty("attachmentType") ? oSearchGlobal.oServiceRequest.attachments[attachment].attachmentType : "";
        sContentType = oSearchGlobal.oServiceRequest.attachments[attachment].hasOwnProperty("contentType") ? oSearchGlobal.oServiceRequest.attachments[attachment].contentType : "";
        sExtension = -1 < attachmentName.lastIndexOf(".") ? attachmentName.substring(attachmentName.lastIndexOf(".") + 1) : 1 === parseInt(attachmentType) ? "msg" : "";
        iconSrc = oCommonObject.getIconSource(sExtension);

        if (-1 < sContentType.indexOf(oGlobalConstants.Image_ContentType)) {
            sImageChunk += "<div class='attachment'>";
            sImageChunk += "<div class='attachmentName'><img id='" + nIDCounter + "editIcon' class='editUploadIcon' src='../Images/edit-666.png' title='Edit Attachment Name' onclick='editAttachment(this," + nIDCounter + ")' alt='Edit'/><img id='" + nIDCounter + "saveIcon' class='saveUploadIcon hide' src='../Images/save-666.png' title='Save Attachment Name' onclick='saveAttachment(this," + nIDCounter + ")' alt='Save'/><div id='" + nIDCounter + "attachment-Status' class='uploadSuccessStatus'></div><img class='attachIcon' id='" + nIDCounter + "attachIcon' src='" + iconSrc + "' alt='' onerror='errorImage(this);'><input type='text' maxlength='128' id='" + nIDCounter + "attachmentText' class='popupName editPopupContent hide' /><div id='" + nIDCounter + "attachment' class='popupName' draggable='true' ondragstart='dragStart(this);' data-attachmentId='" + oSearchGlobal.oServiceRequest.attachments[attachment].id + "' data-originalName='" + attachmentName + "' title='" + sAttachmentFileName + "' attachmentType='" + attachmentType + "' hasEML='" + bHasEML + "'>" + sAttachmentFileName + " </div><div id='" + nIDCounter + "attachment-Message' class='uploadSuccessMessage'></div>";
            sImageChunk += "</div>";
            sImageChunk += "</div>";
        } else {
            sHTMLChunk += "<div class='attachment'>";
            sHTMLChunk += "<div class='attachmentName'><img id='" + nIDCounter + "editIcon' class='editUploadIcon' src='../Images/edit-666.png' title='Edit Attachment Name' onclick='editAttachment(this," + nIDCounter + ")' alt='Edit'/><img id='" + nIDCounter + "saveIcon' class='saveUploadIcon hide' src='../Images/save-666.png' title='Save Attachment Name' onclick='saveAttachment(this," + nIDCounter + ")' alt='Save'/><div id='" + nIDCounter + "attachment-Status' class='uploadSuccessStatus'></div><img class='attachIcon' id='" + nIDCounter + "attachIcon' src='" + iconSrc + "' alt='' onerror='errorImage(this);'><input type='text' maxlength='128' id='" + nIDCounter + "attachmentText' class='popupName editPopupContent hide' /><div id='" + nIDCounter + "attachment' class='popupName' draggable='true' ondragstart='dragStart(this);' data-attachmentId='" + oSearchGlobal.oServiceRequest.attachments[attachment].id + "' data-originalName='" + attachmentName + "' title='" + sAttachmentFileName + "' attachmentType='" + attachmentType + "' hasEML='" + bHasEML + "'>" + sAttachmentFileName + " </div><div id='" + nIDCounter + "attachment-Message' class='uploadSuccessMessage'></div>";
            sHTMLChunk += "</div>";
            sHTMLChunk += "</div>";
        }
    }

    if (sImageChunk) {
        sHTMLChunk += sImageChunk;
    }

    sHTMLChunk += "</div>";

    $(".maildata").html(sHTMLChunk);
    // Making icons non draggable on upload popup
    $(".mailContainer img").attr("draggable", "false");
    $(".popupName").draggable({
        helper: "clone",
        start: function (event, ui) {
            $(this).draggable("option", "cursorAt", {
                left: Math.floor(this.clientWidth / 2),
            });
        }
    });
}

function dragStart(ev) {
    "use strict";
    oUploadGlobal.src = ev;
}

// Function to upload local file
function uploadFile(clientRelativeUrl, folderUrl, isOverwrite) {
    "use strict";
    if (clientRelativeUrl && folderUrl) {
        // App Insight Event tracking for Attachment upload
        commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Local_Upload, true);
        //// Check if the loading image is already present or not. If the loading image is not present, then append it
        var oFolderStructureContent = $(".folderStructureContent");
        var sContent = "";
        if (!oFolderStructureContent.find(".uploadDocumentLoading").length) { oFolderStructureContent.append("<img class=\"uploadDocumentLoading\" src=\"/images/loading_metro.gif\" alt=''/>"); }
        //// Consolidate the output element. 
        $("body #form2").remove();
        $("body").append("<form id='form2' class='hide' runat='server' enctype='multipart/form-data'></form>");
        var form = document.querySelector("#form2");
        var data = new FormData(form);

        var sDocumentLibraryName = $("#mailContent").attr("data-originalname");
        data.append("SPAppToken", oSharePointContext.SPAppToken);
        data.append("ClientUrl", oGlobalConstants.Site_Url + clientRelativeUrl.substring(0, clientRelativeUrl.lastIndexOf("/")));
        data.append("RefreshToken", oSharePointContext.RefreshToken);
        data.append("FolderName", folderUrl);
        data.append("AllowContentCheck", oUploadGlobal.bAllowContentCheck);
        if ("undefined" !== typeof sDocumentLibraryName) {
            data.append("DocumentLibraryName", sDocumentLibraryName);
        }

        var nCounter = 0;
        for (nCounter = 0; nCounter < oSearchGlobal.files.length; nCounter++) {
            if (oSearchGlobal.files[nCounter]) {
                data.append(oSearchGlobal.files[nCounter].name, oSearchGlobal.files[nCounter]);
                data.append("Overwrite" + nCounter, isOverwrite);
            }
        }

        oUploadGlobal.iActiveUploadRequest++;
        oUploadGlobal.oXHR.onreadystatechange = function () {
            $(".mailContainer .notification .closeNotification").click();
            if (4 === oUploadGlobal.oXHR.readyState && 200 === oUploadGlobal.oXHR.status && oUploadGlobal.oXHR.responseText) {
                oUploadGlobal.iActiveUploadRequest--;
                if (0 === oUploadGlobal.iActiveUploadRequest) {
                    $(".uploadDocumentLoading").remove();
                }
                var arrResponse = $("<div/>").html(oUploadGlobal.oXHR.responseText).text().split("$|$");
                var overwriteExists = false;
                var errorOccured = false;
                var sOptionContent = "";
                for (var responseCount = 0; responseCount < arrResponse.length - 1; responseCount++) {
                    var response = arrResponse[responseCount].split(":::");
                    var errorResponse = arrResponse[responseCount].split("$$$");
                    if (1 < errorResponse.length) {
                        sContent = "<div class='notification warningNotification'> <img id = 'warningImg' src = '../Images/warning-message.png' alt='Warning Message'/> <div id = 'overWriteDocumentName'>" + errorResponse[0] + "</div><div class='askForOverwrite'> <input type='button' id = 'overWriteOk' data-operation ='ignore' value='" + oGlobalConstants.Upload_Ok_Button + "' onClick='oCommonObject.localOverWriteDocument(this);' /></div></div>";
                        oCommonObject.updateUploadPopupHeight(true);
                        $(".notificationContainerForPopup").append(sContent);

                        //// Making icons non draggable on upload popup
                        $(".mailContainer img").attr("draggable", "false");
                        oUploadGlobal.sNotificationMsg = "";
                        errorOccured = true;
                    } else {
                        if (response.length > 1) {
                            var nIDCounter = $(".maildata .attachmentSection").length;
                            var sExtension = response[0].substring(response[0].lastIndexOf(".") + 1);
                            oUploadGlobal.arrOverwrite[responseCount] = "False";
                            sContent = "<div class='attachment'>";
                            var sFolderName = oCommonObject.getRootFolderForMatter(response[1]);
                            sContent += "<div class='attachmentName mailName'><div id='" + nIDCounter + "attachment-Status' class=\"uploadSuccessStatus\"><img class=\"uploadSuccessStatus\" src=\"/images/success-message.png\" alt='Uploaded successfully'/></div><img class='attachIcon' id='" + nIDCounter + "attachIcon' src='" + oGlobalConstants.Image_Document_Icon.replace("{0}", sExtension) + "' alt='attachment icon' onerror='errorImage(this);'><div id='" + nIDCounter + "attachment' title='" + response[0] + "' class='popupName popupSelect'>" + response[0] + "</div><div id='" + nIDCounter + "attachment-Message' class='uploadSuccessMessage' title='" + sFolderName + "'>(" + sFolderName + ")</div>";
                            sContent += "</div>";
                            sContent += "</div>";

                            if (0 === nIDCounter) {
                                $(".maildata").html("<div class='attachmentSection'></div>");
                            }
                            $(".maildata .attachmentSection").append(sContent);
                            oCommonObject.showNotification(oGlobalConstants.Upload_Success_Notification, "successNotification ms-font-weight-semibold");
                            oUploadGlobal.sNotificationMsg = "";
                            //// Making icons non draggable on upload popup
                            $(".mailContainer img").attr("draggable", "false");
                        } else {
                            oUploadGlobal.sClientRelativeUrl = clientRelativeUrl;
                            oUploadGlobal.sFolderUrl = folderUrl;
                            oUploadGlobal.arrOverwrite[responseCount] = "True";
                            var duplicateNotification = response[0].split("@@@");
                            var contentCheckPerformed = response[0].split("|||");
                            //// Update the content as per the logic.
                            var fileName = "undefined" !== typeof oSearchGlobal.files[responseCount] && oSearchGlobal.files[responseCount].name ? oSearchGlobal.files[responseCount].name.trim() : "",
                            bAppendEnabled = oCommonObject.overwriteConfiguration(fileName);
                            oUploadGlobal.bAppendOptionEnabled = bAppendEnabled;

                            //// True means show append button and False means hide append button
                            var sAppendContent = bAppendEnabled ? "<input type='button' id = 'overWriteAppend' data-operation='append' title='" + oGlobalConstants.Upload_Append_Button_Tooltip + "'  value='" + oGlobalConstants.Upload_Append_Button + "' onClick='oCommonObject.localOverWriteDocument(this);' />" : "";
                            var sContentCheckChunk = "";
                            if (oUploadGlobal.bAllowContentCheck && duplicateNotification[1] && "TRUE" === duplicateNotification[1].toUpperCase()) {
                                sContentCheckChunk = "<input type='button' id = 'contentCheck' title='" + oGlobalConstants.Upload_Content_Check_Tooltip + "' value='" + oGlobalConstants.Upload_ContentCheck_Button + "' data-operation='contentCheck' onClick='oCommonObject.localOverWriteDocument(this); oCommonObject.contentCheckNotification(true);' />";
                            }

                            //// Capture First message from page and preserve for later usage.
                            if ("undefined" !== typeof oUploadGlobal.sNotificationMsg && "" === oUploadGlobal.sNotificationMsg) {
                                oUploadGlobal.sNotificationMsg = duplicateNotification[0];
                            }

                            if (duplicateNotification[1]) {
                                //// Potential duplicate found, show notification to perform content check or overwrite or append
                                sOptionContent = sContentCheckChunk + "<input type='button' id = 'overWriteYes' title= '" + oGlobalConstants.Upload_Overwrite_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Overwrite_Button + "' data-operation='overwrite' onClick='oCommonObject.localOverWriteDocument(this);'/>" + sAppendContent + "<input type='button' id = 'overWriteNo' title='" + oGlobalConstants.Upload_Cancel_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Cancel_Button + "' data-operation='ignore' onClick='oCommonObject.localOverWriteDocument(this);'/>";
                                sContent = oCommonObject.getNotificationContent(duplicateNotification[0], "", sOptionContent);
                            } else if (contentCheckPerformed[1]) {
                                //// Content Check is performed, show notification to overwrite or append
                                $(".notification").remove();
                                oCommonObject.updateUploadPopupHeight(false);
                                sContentCheckChunk = ("TRUE" === contentCheckPerformed[1].toUpperCase()) ? sContentCheckChunk : "";
                                sOptionContent = sContentCheckChunk + "<input type='button' id = 'overWriteYes' title= '" + oGlobalConstants.Upload_Overwrite_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Overwrite_Button + "' data-operation='overwrite' onClick='oCommonObject.localOverWriteDocument(this);'/>" + sAppendContent + "<input type='button' id = 'overWriteNo' title='" + oGlobalConstants.Upload_Cancel_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Cancel_Button + "' data-operation='ignore' onClick='oCommonObject.localOverWriteDocument(this);'/>";
                                sContent = oCommonObject.getNotificationContent(oUploadGlobal.sNotificationMsg, contentCheckPerformed[0], sOptionContent);

                                //// clear previous stored notification
                                oUploadGlobal.sNotificationMsg = "";
                            }
                            oCommonObject.updateUploadPopupHeight(true);
                            $(".notificationContainerForPopup").append(sContent);

                            //// Making icons non draggable on upload popup
                            $(".mailContainer img").attr("draggable", "false");
                            oUploadGlobal.arrFiles.push(oSearchGlobal.files[responseCount]);
                            overwriteExists = true;
                        }
                    }
                }
            } else if (4 === oUploadGlobal.oXHR.readyState && 200 !== oUploadGlobal.oXHR.status) {
                // TODO: if upload fails, display file name and red cross icon
                oUploadGlobal.iActiveUploadRequest--;
                if (0 === oUploadGlobal.iActiveUploadRequest) {
                    $(".uploadDocumentLoading").remove();
                }
            }
            if (overwriteExists || errorOccured) {
                //// Dynamically increase the height of the popup
                oCommonObject.updateUploadPopupHeight(true);
            }
        };
        oUploadGlobal.oXHR.open("POST", "UploadFile.aspx");
        oUploadGlobal.oXHR.setRequestHeader("RequestValidationToken", oMasterGlobal.Tokens);
        oUploadGlobal.oXHR.send(data);
    }
}

// Function to upload attachment/Email from Outlook
function uploadAttachment(e, ui) {
    "use strict";
    // Get folder URL and attachment ID.
    e.stopPropagation();
    e.preventDefault();
    var arrSelectedFolderUrl = [],
        arrSelectedAttachment = [],
        arrTempAttachments = [],
        iAttachmentPos = 0,
        srcElement = ("undefined" === typeof ui) ? (($(oUploadGlobal.src).length) ? oUploadGlobal.src : false) : ui.draggable[0],
        targetElement = "undefined" === typeof e.target.attributes["data-foldername"] ? $(e.target).parent()[0] : e.target,
        sClientURL = targetElement.attributes["data-client"].value,
        sDocumentLibraryName = $("#mailContent").attr("data-originalname"), //// Pass GUID
        oParam = { source: srcElement, target: targetElement, arrTempAttachments: [], arrSelectedFolderUrl: [], arrMailFolderURL: [], isMail: false, documentLibraryName: sDocumentLibraryName },
        isMail = $(srcElement).parent().hasClass("mailName");
    if (!srcElement) {
        return;
    }
    arrSelectedFolderUrl.push(targetElement.attributes["data-foldername"].value);
    arrSelectedAttachment.push(srcElement.attributes["data-attachmentId"].value);

    // If mail selected than call to service.
    if (isMail) {
        var arrMailFolderURL = [];
        arrMailFolderURL.push(targetElement.attributes["data-foldername"].value);
        oParam.arrMailFolderURL = arrMailFolderURL;
        oParam.isMail = true;
        oSearchGlobal.oServiceRequest.subject = srcElement.innerHTML.trim() + ".eml";
        requestMailUpload(arrMailFolderURL, oParam, false, false);
    } else if (0 !== arrSelectedAttachment.length && 0 !== arrSelectedFolderUrl.length && arrSelectedAttachment.length === arrSelectedFolderUrl.length) {
        // Popup out unselected attachment ID from Global Array.
        for (var attachment in oSearchGlobal.oServiceRequest.attachments) {
            if (-1 !== arrSelectedAttachment.indexOf(oSearchGlobal.oServiceRequest.attachments[attachment].id)) {
                if ("true" === $(srcElement).attr("hasEML")) {
                    oSearchGlobal.oServiceRequest.attachments[attachment].name = srcElement.innerHTML.trim() + ".eml";
                } else {
                    oSearchGlobal.oServiceRequest.attachments[attachment].name = srcElement.innerHTML.trim();
                }
                oSearchGlobal.oServiceRequest.attachments[attachment].originalName = srcElement.attributes["data-originalName"].value;
                arrTempAttachments[iAttachmentPos] = oSearchGlobal.oServiceRequest.attachments[attachment];
            }
        }
        oParam.arrTempAttachments = arrTempAttachments;
        oParam.arrSelectedFolderUrl = arrSelectedFolderUrl;
        requestAttachmentUpload(arrTempAttachments, arrSelectedFolderUrl, oParam, false, false);
    }
}

/* Function to retrieve the user's choice to overwrite a document or not and depending on that either overwrite that document or not */
function overWriteDocument(oDocument) {
    "use strict";
    var $askForOverwrite = $(".askForOverwrite"),
    overWriteYes = $askForOverwrite.find("#overWriteYes")[0],
    overWriteNo = $askForOverwrite.find("#overWriteNo")[0],
    overWriteAppend = $askForOverwrite.find("#overWriteAppend")[0],
    overWriteContentCheck = $askForOverwrite.find("#contentCheck")[0];

    if ("undefined" !== typeof overWriteYes) { overWriteYes.disabled = true; }
    if ("undefined" !== typeof overWriteNo) { overWriteNo.disabled = true; }
    if ("undefined" !== typeof overWriteAppend) { overWriteAppend.disabled = true; }
    if ("undefined" !== typeof overWriteContentCheck) { overWriteContentCheck = true; }

    if ("overWriteNo" === $(oDocument).attr("id")) {
        if ("undefined" !== typeof overWriteYes) { overWriteYes.disabled = false; }
        if ("undefined" !== typeof overWriteNo) { overWriteNo.disabled = false; }
        if ("undefined" !== typeof overWriteAppend) { overWriteAppend.disabled = false; }
        if ("undefined" !== typeof overWriteContentCheck) { overWriteContentCheck = false; }
        $(oDocument).parents(".notification").remove();
        oUploadGlobal.sNotificationMsg = "";
    }
    var oUploadParameter = oUploadGlobal.hasOwnProperty("oUploadParameter") && oUploadGlobal.oUploadParameter.length ? oUploadGlobal.oUploadParameter.pop() : "",
        isUploadParameterHasTempAttachmentsProp = oUploadParameter.hasOwnProperty("arrTempAttachments"),
        isUploadParameterHasSelectedFolderUrlProp = oUploadParameter.hasOwnProperty("arrSelectedFolderUrl"),
        isUploadParameterHasIsMailProp = oUploadParameter.hasOwnProperty("isMail"),
        isUploadParameterHasMailFolderURLProp = oUploadParameter.hasOwnProperty("arrMailFolderURL"),
        sOperation = $(oDocument).attr("data-operation");

    oUploadGlobal.sNotificationMsg = ("contentCheck" !== sOperation) ? "" : oUploadGlobal.sNotificationMsg;

    if ("overwrite" === sOperation) {
        if (isUploadParameterHasIsMailProp && oUploadParameter.isMail) {
            if (isUploadParameterHasMailFolderURLProp) {
                requestMailUpload(oUploadParameter.arrMailFolderURL, oUploadParameter, true);
            }
        } else {
            if (isUploadParameterHasTempAttachmentsProp && isUploadParameterHasSelectedFolderUrlProp) {
                requestAttachmentUpload(oUploadParameter.arrTempAttachments, oUploadParameter.arrSelectedFolderUrl, oUploadParameter, true);
            }
        }
    } else if ("append" === sOperation) {
        var date = new Date();
        date = date.toISOString();
        var reg = new RegExp(":", "g");
        date = date.replace(reg, "_").replace(".", "_");
        if (isUploadParameterHasIsMailProp && oUploadParameter.isMail) {
            var subject = oSearchGlobal.oServiceRequest.subject;
            var subjectNameWithoutExt = subject.substring(0, subject.lastIndexOf("."));
            var extMail = subject.substr(subject.lastIndexOf(".") + 1);
            oSearchGlobal.oServiceRequest.subject = subjectNameWithoutExt + "_" + date + "." + extMail;
            if (isUploadParameterHasMailFolderURLProp) {
                requestMailUpload(oUploadParameter.arrMailFolderURL, oUploadParameter, true);
            }
        } else {
            if (isUploadParameterHasTempAttachmentsProp) {
                for (var attachment in oUploadParameter.arrTempAttachments) {
                    var fileNameWithExt = oUploadParameter.arrTempAttachments[attachment].name;
                    if (-1 !== fileNameWithExt.lastIndexOf(".")) {
                        var fileNameWithoutExt = fileNameWithExt.substring(0, fileNameWithExt.lastIndexOf("."));
                        var ext = fileNameWithExt.substr(fileNameWithExt.lastIndexOf(".") + 1);
                        oUploadParameter.arrTempAttachments[attachment].name = fileNameWithoutExt + "_" + date + "." + ext;
                    } else {
                        oUploadParameter.arrTempAttachments[attachment].name = fileNameWithExt + "_" + date;
                    }
                }
            }
            if (isUploadParameterHasTempAttachmentsProp && isUploadParameterHasSelectedFolderUrlProp) {
                requestAttachmentUpload(oUploadParameter.arrTempAttachments, oUploadParameter.arrSelectedFolderUrl, oUploadParameter, true);
            }
        }
    } else if ("contentCheck" === sOperation) {
        if (isUploadParameterHasIsMailProp && oUploadParameter.isMail) {
            if (isUploadParameterHasMailFolderURLProp) {
                requestMailUpload(oUploadParameter.arrMailFolderURL, oUploadParameter, false, true);
            }
        } else {
            if (isUploadParameterHasTempAttachmentsProp && isUploadParameterHasSelectedFolderUrlProp) {
                requestAttachmentUpload(oUploadParameter.arrTempAttachments, oUploadParameter.arrSelectedFolderUrl, oUploadParameter, false, true);
            }
        }
    }

    if (0 < $(".notification").length) {
        if ("undefined" !== typeof overWriteYes) { overWriteYes.disabled = false; }
        if ("undefined" !== typeof overWriteNo) { overWriteNo.disabled = false; }
        if ("undefined" !== typeof overWriteAppend) { overWriteAppend.disabled = false; }
        if ("undefined" !== typeof overWriteContentCheck) { overWriteContentCheck = false; }
        $(oDocument).parents(".notification").remove();
    }
    //// Dynamically decrease the height of the popup
    oCommonObject.updateUploadPopupHeight(false);
}

function onUploadSuccess(result) {
    "use strict";
    $(".mailContainer .notification .closeNotification").click();
    oUploadGlobal.iActiveUploadRequest--;
    var successStatusId = result.oParam.source.id + "-Status";
    var successMessageId = result.oParam.source.id + "-Message";
    var count = result.oParam.source.id.split("attachment")[0];
    var target = result.oParam.target;
    var folderName = target.attributes["data-foldername"].value.substring(target.attributes["data-foldername"].value.lastIndexOf("/") + 1);
    var sOptionContent = "";
    if ("True" === result.Result) {
        if (0 === oUploadGlobal.iActiveUploadRequest) {
            $(".uploadDocumentLoading").remove();
        }

        $("#" + successStatusId).empty();
        $("#" + successStatusId).append("<img  class=\"uploadSuccessStatus\" src=\"/images/success-message.png\" alt='Uploaded successfully'/>");
        var thisAttachment = $("#" + count + "attachment");
        if (result.oParam.isMail) {
            var subject = oSearchGlobal.oServiceRequest.subject;
            subject = subject.substring(0, subject.lastIndexOf("."));
            thisAttachment.html(subject);
            thisAttachment.attr("title", subject);
        } else {
            var extEmlOrMsg = result.oParam.arrTempAttachments[0].name.substr(result.oParam.arrTempAttachments[0].name.lastIndexOf(".") + 1);
            if ("eml" === extEmlOrMsg || "msg" === extEmlOrMsg) {
                thisAttachment.html(result.oParam.arrTempAttachments[0].name.substring(0, result.oParam.arrTempAttachments[0].name.lastIndexOf(".")));
                thisAttachment.attr("title", result.oParam.arrTempAttachments[0].name.substring(0, result.oParam.arrTempAttachments[0].name.lastIndexOf(".")));
            } else {
                thisAttachment.html(result.oParam.arrTempAttachments[0].name);
                thisAttachment.attr("title", result.oParam.arrTempAttachments[0].name);
            }
        }
        //// Making icons non draggable on upload popup
        $(".mailContainer img").attr("draggable", "false");
        var sFolderName = oCommonObject.getRootFolderForMatter(folderName);
        $("#" + successMessageId).attr("title", sFolderName).text("(" + sFolderName + ")").removeClass("uploadFailedMessage hide").addClass("uploadSuccessMessage");
        $("#" + count + "editIcon, #" + count + "saveIcon").addClass("hide");
        oCommonObject.showNotification(oGlobalConstants.Upload_Success_Notification, "successNotification ms-font-weight-semibold");
    } else if ("False" !== result.Result) {
        var errorResponse = result.Result.split("$$$");
        oUploadGlobal.oUploadParameter.push(result.oParam);
        var sContent = "";
        if (1 < errorResponse.length) {
            sOptionContent = "<input type='button' id = 'overWriteOk' value='" + oGlobalConstants.Upload_Ok_Button + "' data-operation='ignore' onClick='overWriteDocument(this);' />";
            sContent = oCommonObject.getNotificationContent(errorResponse[0], "", sOptionContent);
        } else {
            //// update the content as per the logic
            var selectedOverwriteConfiguration = oGlobalConstants.Overwrite_Config_Property.trim().toLocaleUpperCase(),
                bAppendEnabled = false,
                fileExtension = "undefined" !== typeof result.oParam.arrTempAttachments[0] && result.oParam.arrTempAttachments[0].name ? result.oParam.arrTempAttachments[0].name.trim().substring(result.oParam.arrTempAttachments[0].name.trim().lastIndexOf(".") + 1) : "";
            var isEmail = result.oParam.isMail ? true : (1 === parseInt(result.oParam.source.attributes.attachmentType.value) || "eml" === fileExtension) ? true : false;
            var duplicateNotification = result.Result.split("@@@");
            var contentCheckPerformed = result.Result.split("|||");

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

            //// True means show append button and False means hide append button
            var sAppendContent = bAppendEnabled ? "<input type='button' id = 'overWriteAppend' data-operation='append' value='" + oGlobalConstants.Upload_Append_Button + "' onClick='overWriteDocument(this);' />" : "";

            var sContentCheckChunk = "";
            if (oUploadGlobal.bAllowContentCheck && duplicateNotification[1] && "TRUE" === duplicateNotification[1].toUpperCase()) {
                sContentCheckChunk = "<input type='button' id = 'contentCheck' title='" + oGlobalConstants.Upload_Content_Check_Tooltip + "' value='" + oGlobalConstants.Upload_ContentCheck_Button + "' data-operation='contentCheck' onClick='overWriteDocument(this); oCommonObject.contentCheckNotification(false);' />";
            }

            if (!oUploadGlobal.sNotificationMsg.trim()) {
                oUploadGlobal.sNotificationMsg = duplicateNotification[0];
            }

            if (duplicateNotification[1]) {
                sOptionContent = sContentCheckChunk + "<input type='button' id = 'overWriteYes' title= '" + oGlobalConstants.Upload_Overwrite_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Overwrite_Button + "' data-operation='overwrite' onClick='overWriteDocument(this);'/>" + sAppendContent + "<input type='button' id = 'overWriteNo' title='" + oGlobalConstants.Upload_Cancel_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Cancel_Button + "' data-operation='ignore' onClick='overWriteDocument(this);'/>";
                sContent = oCommonObject.getNotificationContent(duplicateNotification[0], "", sOptionContent);
            } else if (contentCheckPerformed[1] && !oUploadGlobal.bIsAbortedCC) {
                //// Content Check is performed , remove notification message set by contentCheckNotification()
                $(".notification").remove();
                oCommonObject.updateUploadPopupHeight(false);
                if (contentCheckPerformed[1]) {
                    sContentCheckChunk = ("TRUE" === contentCheckPerformed[1].toUpperCase()) ? sContentCheckChunk : "";
                    sOptionContent = "<input type='button' id = 'overWriteYes' title= '" + oGlobalConstants.Upload_Overwrite_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Overwrite_Button + "' data-operation='overwrite' onClick='overWriteDocument(this);'/>" + sAppendContent + "<input type='button' id = 'overWriteNo' title='" + oGlobalConstants.Upload_Cancel_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Cancel_Button + "' data-operation='ignore' onClick='overWriteDocument(this);'/>";

                    sContent = oCommonObject.getNotificationContent(oUploadGlobal.sNotificationMsg, contentCheckPerformed[0], sOptionContent);
                    //// clear previous stored notification
                    oUploadGlobal.sNotificationMsg = "";
                }
            }
            oUploadGlobal.bIsAbortedCC = false;
        }
        //// Dynamically increase the height of the popup
        oCommonObject.updateUploadPopupHeight(true);
        $(".notificationContainerForPopup").append(sContent);

        //// Making icons non draggable on upload popup
        $(".mailContainer img").attr("draggable", "false");
    } else {
        sOptionContent = "<input type='button' id = 'overWriteOk' title='" + oGlobalConstants.Upload_Cancel_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Ok_Button + "' data-operation='ignore' onClick='oCommonObject.localOverWriteDocument(this);' />";

        sContent = oCommonObject.getNotificationContent(oGlobalConstants.Upload_Failed_Message, "", sOptionContent);
        $(".notificationContainerForPopup").append(sContent);
        //// Making icons non draggable on upload popup
        $(".mailContainer img").attr("draggable", "false");
        $("#" + successMessageId).attr("title", oGlobalConstants.Upload_Failed_Marker).text(oGlobalConstants.Upload_Failed_Marker).addClass("uploadFailedMessage").removeClass("uploadSuccessMessage");
    }
    if (0 === oUploadGlobal.iActiveUploadRequest) {
        $(".uploadDocumentLoading").remove();
    }
}

function onUploadFailure(result) {
    "use strict";
    var failureStatusId = result.oParam.id + "-Status";
    oUploadGlobal.iActiveUploadRequest--;
    if (0 === oUploadGlobal.iActiveUploadRequest) {
        $(".uploadDocumentLoading").remove();
    }
    showPopupNotification(oFindMatterConstants.Mail_Upload_Failure, "failNotification");
}

function onUploadCallBefore() {
    "use strict";
    var oFolderStructureContent = $(".folderStructureContent");
    // Check if the loading image is already present or not. If the loading image is not present, then append it
    if (!oFolderStructureContent.find(".uploadDocumentLoading").length) { oFolderStructureContent.append("<img class=\"uploadDocumentLoading\" src=\"/images/loading_metro.gif\" alt=''/>"); }
    oUploadGlobal.iActiveUploadRequest++;
}

function requestMailUpload(mailfolderUrl, oParam, isOverwrite, isContentCheck) {
    "use strict";
    var sParameters = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oSearchGlobal.matterUrl }, "serviceRequest": { "AttachmentToken": oSearchGlobal.oServiceRequest.attachmentToken, "EwsUrl": oSearchGlobal.oServiceRequest.ewsUrl, "Attachments": oSearchGlobal.oServiceRequest.attachments, "MailId": oSearchGlobal.oServiceRequest.MailId, "FolderPath": mailfolderUrl, "Subject": oSearchGlobal.oServiceRequest.subject, "Overwrite": isOverwrite, "PerformContentCheck": isContentCheck, "DocumentLibraryName": oParam.documentLibraryName, "AllowContentCheck": oUploadGlobal.bAllowContentCheck } };
    oCommonObject.callSearchService("UploadMail", sParameters, onUploadSuccess, onUploadFailure, onUploadCallBefore, oParam);
}

function requestAttachmentUpload(arrTempAttachments, arrSelectedFolderUrl, oParam, isOverwrite, isContentCheck) {
    "use strict";
    var sParameters = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oSearchGlobal.matterUrl }, "serviceRequest": { "AttachmentToken": oSearchGlobal.oServiceRequest.attachmentToken, "EwsUrl": oSearchGlobal.oServiceRequest.ewsUrl, "Attachments": arrTempAttachments, "MailId": oSearchGlobal.oServiceRequest.MailId, "FolderPath": arrSelectedFolderUrl, "Subject": oSearchGlobal.oServiceRequest.subject, "Overwrite": isOverwrite, "PerformContentCheck": isContentCheck, "DocumentLibraryName": oParam.documentLibraryName, "AllowContentCheck": oUploadGlobal.bAllowContentCheck } };
    oCommonObject.callSearchService("UploadAttachment", sParameters, onUploadSuccess, onUploadFailure, onUploadCallBefore, oParam);
}

function openFileBrowser() {
    "use strict";
    var fileExplorer = $("#fileExplorer");
    var files = [];
    var tempFiles = fileExplorer[0].files;
    fileExplorer.click();
    var selectedFiles = fileExplorer[0].files;
    var isOverwrite = "False";
    if (selectedFiles.length > 0 && tempFiles !== selectedFiles) {
        oSearchGlobal.files = fileExplorer[0].files;
        var folderURL = oSearchGlobal.oFolderName[0].url;
        // upload browsed file(s) to root folder
        uploadFile(folderURL, folderURL, isOverwrite);
    }
};

function editAttachment(oElement, count) {
    "use strict";
    // App Insight Event tracking for edit Attachment name
    commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Rename_Mail_Attachment, true);
    var thisAttachment = $("#" + count + "attachment");
    var thisAttachmentText = $("#" + count + "attachmentText");
    var thisEditIcon = $("#" + count + "editIcon");
    var thisSaveIcon = $("#" + count + "saveIcon");
    var thisAttachIcon = $("#" + count + "attachIcon");
    var attachmentText = thisAttachment[0].innerHTML;
    var thisStatusMessage = $("#" + count + "attachment-Message");

    if (thisSaveIcon.hasClass("hide")) {
        thisSaveIcon.removeClass("hide");
        thisEditIcon.addClass("hide");
        thisAttachIcon.addClass("hide");
        thisStatusMessage.attr("title", "").empty().addClass("hide");
    }

    if (thisAttachmentText.hasClass("hide")) {
        thisAttachmentText.removeClass("hide");
        thisAttachment.addClass("hide");
        thisAttachmentText.val(attachmentText);
    }
};

function saveAttachment(oElement, count) {
    "use strict";
    // App Insight Event tracking for saving updated attachment name
    commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Save_Renamed_Mail_Attachment, true);
    var thisAttachment = $("#" + count + "attachment");
    var thisAttachmentText = $("#" + count + "attachmentText");
    var thisEditIcon = $("#" + count + "editIcon");
    var thisSaveIcon = $("#" + count + "saveIcon");
    var thisAttachIcon = $("#" + count + "attachIcon");
    var attachmentText = thisAttachmentText[0].value.trim();
    var oldText = thisAttachment[0].innerHTML;
    if (!oUploadGlobal.regularInvalidCharacter.test(attachmentText) && !oUploadGlobal.regularExtraSpace.test(attachmentText) && !oUploadGlobal.regularInvalidRule.test(attachmentText) && !oUploadGlobal.regularStartEnd.test(attachmentText)) {
        if (thisEditIcon.hasClass("hide")) {
            thisEditIcon.removeClass("hide");
            thisAttachIcon.removeClass("hide");
            thisSaveIcon.addClass("hide");
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
        $(".errorPopUp").addClass("hide");
    } else {
        oUploadGlobal.regularInvalidCharacter.lastIndex = 0;
        showErrorNotification(thisAttachmentText, "Invalid character");
    }
};

function showErrorNotification(sElement, sMsg) {
    "use strict";
    var windowWidth = oCommonObject.getWidth();
    var removeTop = 75, removeLeft = 115;
    var posLeft = "50px";
    var triangleTopPos = 48;
    var errorBoxHeight = 62;
    var iLeftPos = $(sElement).offset().left
    , iTopPos = $(sElement).offset().top
    , iCurrentWidth = $(sElement).outerWidth();
    iLeftPos = parseInt(iLeftPos, 10) + parseInt(iCurrentWidth, 10) - 20;
    iTopPos = parseInt(iTopPos, 10) - 20;
    var errorPopUp = $("#mainContainer .errorPopUp");


    errorPopUp.css("left", iLeftPos - removeLeft).css("top", iTopPos - removeTop).removeClass("hide").find(".errText").text(sMsg);
    $(".errorPopUp .errTriangleBorder").css("left", posLeft);
    $(".errorPopUp .errTriangle").css("left", posLeft);
    $(".errorPopUp .errTriangleBorder").css("top", "calc(50% - -" + triangleTopPos + "px)");
    $(".errorPopUp .errTriangle").css("top", "calc(50% - -" + (triangleTopPos + 1) + "px)");
    $(".errorPopUp .errText").css("min-height", errorBoxHeight + "px");
}

// Function to close the upload success notification
$(document).on("click", ".mailContainer .notification .closeNotification", function () {
    "use strict";
    var minRequiredHeight = 283, updatedHeight = $(".mailContainer").height() - 30;
    $(".mailContainer .successNotification").remove();
    if (minRequiredHeight <= updatedHeight) {
        $(".mailContainer").height(updatedHeight);  // Adjusting the height of the popup, post removing notification
    }
});
