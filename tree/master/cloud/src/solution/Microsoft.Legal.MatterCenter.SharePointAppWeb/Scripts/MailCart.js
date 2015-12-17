/// <disable>JS3057,JS3058,JS2074,JS2076,JS3092,JS3054,JS2005,JS3056,JS2073,JS2024,JS2026,JS2032,JS2064,JS3116</disable>
var iDocumentCount = 0,
    sOneDriveURL = "",
    iOverWriteDivCount = 0,
    iTotalDocumentCount = 0,
    iParentNodeID = null,
    errorDocumentNumber = -1,
    sOneDriveErrorMessage = "",
    arrCheckOutFailDocument = [];
function downloadEmail(bAttachmentCall) {
    "use strict";
    // App Insight Event tracking for Email as attachment
    bAttachmentCall ? (commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Email_Attachment, true)) : commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Email_Link, true);
    var arrMatches = modifySelectedFileURLS(), matterDetails;
    if (arrMatches.length > 0) {
        // Prevent the click events if there is at least one document selected.
        // Add a return false to each of the links when any link is clicked.
        $(".sendEmailOptions").each(function () {
            "use strict";
            var oCurrentLink = $(this);
            if (oCurrentLink[0]) {
                oCurrentLink.attr("onclick", "return false;" + oCurrentLink.attr("onclick"));
            }
        });
        var oEmailSelectedData = arrMatches,
            sFileURLs = "",
            nCount, oEmailRelativePath,
            nEmailSelectedDataLength = oEmailSelectedData.length - 1,
            beforeEmail;

        for (nCount = 0; nCount <= nEmailSelectedDataLength; nCount++) {
            if (oEmailSelectedData[nCount].DocumentUrl) {
                oEmailRelativePath = unescape(oEmailSelectedData[nCount].DocumentUrl);
            } else {
                oEmailRelativePath = unescape(oEmailSelectedData[nCount].Path);
            }
            oEmailRelativePath = trimEndChar(oEmailRelativePath, "/");
            oEmailRelativePath = oEmailRelativePath.replace(oGlobalConstants.Site_Url, "$") + "$";
            if (oEmailSelectedData[nCount].DocumentClientUrl) {
                sFileURLs += oEmailSelectedData[nCount].DocumentTeamUrl + oEmailRelativePath + oEmailSelectedData[nCount].FileName + ";";
            } else {
                sFileURLs += oEmailSelectedData[nCount].SiteName + oEmailRelativePath + oEmailSelectedData[nCount].FileName + ";";
            }
        }
        beforeEmail = { "oParam": { "FullUrl": sFileURLs } };
        matterDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "mailAttachmentDetails": { "FullUrl": sFileURLs, "IsAttachmentCall": bAttachmentCall } };
        $.ajax({
            type: "POST",
            url: oGlobalConstants.Legal_Briefcase_Service_Url + "SaveEmail",
            data: JSON.stringify(matterDetails),
            contentType: "application/json; charset=utf-8",
            headers: { "RequestValidationToken": oMasterGlobal.Tokens },
            begin: beforeLegalBriefcaseOperations(beforeEmail),
            success: function (result) {
                result = encodeURIComponent(result);
                var url = "/pages/downloadEmail.aspx";
                var form = $("<form class=\"mailForm\" action=\"" + url + "\" method=\"post\">" +
                  "<input type=hidden name=\"mailContent\" id=\"mailContent\" value=\"" + result + "\"/>" +
                   "<input type=hidden name=\"requestToken\" id=\"requestToken\" value=\"" + oMasterGlobal.Tokens + "\"/>" +
                  "</form>");
                $("body").append(form);
                $(form).submit();
                var iDocuments = $(".attachmentList").length, iIterator = 0;
                for (iIterator = 0; iIterator < iDocuments; iIterator++) {
                    $(".emailDocumentNames #" + iIterator + " img.mailCheck").css("display", "inline");
                    $(".emailDocumentNames #" + iIterator + " .sendingDocumentImg").css("display", "none");
                }
                // Remove the return false to enable the links again
                $(".sendEmailOptions").each(function () {
                    "use strict";
                    var oCurrentLink = $(this);
                    if (oCurrentLink[0]) {
                        oCurrentLink.attr("onclick", oCurrentLink.attr("onclick").replace(/return false;/g, ""));
                    }
                });
                $(".mailForm").remove();
            }
        });
    }
}

/* Common function to retrieve selected documents in the Mail Cart popup */
function modifySelectedFileURLS() {
    "use strict";
    var query = $("img.mailCheck[data-checked='true']").map(function () { return $(this).attr("data-value"); }),
            arrMatches = [], iIterate;

    arrMatches = $.grep(oGridConfig.arrSelectedGridData, function (selectedDocument) {
        for (iIterate = 0; iIterate < query.length; iIterate++) {
            if ((selectedDocument.DocumentUrl) && (decodeURI(trimEndChar(selectedDocument.DocumentUrl, "/").toUpperCase()) === decodeURI(query[iIterate].toUpperCase()))) {
                selectedDocument.DocumentUrl = query[iIterate];
                arrMatches.push(selectedDocument);
                return true;
            } else if ((selectedDocument.Path) && (decodeURI(trimEndChar(selectedDocument.Path, "/").toUpperCase()) === decodeURI(query[iIterate].toUpperCase()))) {
                selectedDocument.Path = query[iIterate];
                arrMatches.push(selectedDocument);
                return true;
            }
        }
    });
    return arrMatches;
}


/* Function to display Look up pop up */
function showPopup() {
    "use strict";
    var mailContainer = $(".popUpContainer");
    // Enable to onclick event.
    $(".sendEmailOptions").each(function () {
        "use strict";
        var oCurrentLink = $(this);
        if (oCurrentLink[0]) {
            oCurrentLink.attr("onclick", oCurrentLink.attr("onclick").replace(/return false;/g, ""));
        }
    });
    if ($(".checkOutPopup")) {
        $(".checkOutPopup").hide();
    }
    if ($(".notification")) {
        $(".notification").remove();
    }
    $(".popUpContainer .popUpBody ul").empty();
    var nCount = 0, oEmailRelativePath, fileURL = "", oParam, matterDetails;

    for (nCount = 0; nCount <= oGridConfig.arrSelectedGridData.length - 1; nCount++) {
        if (oGridConfig.arrSelectedGridData[nCount].DocumentUrl) {
            oEmailRelativePath = unescape(oGridConfig.arrSelectedGridData[nCount].DocumentUrl);
        } else {
            oEmailRelativePath = unescape(oGridConfig.arrSelectedGridData[nCount].Path);
        }
        oEmailRelativePath = trimEndChar(oEmailRelativePath, "/");
        oEmailRelativePath = oEmailRelativePath.replace(oGlobalConstants.Site_Url, "$") + ";";
        if (oGridConfig.arrSelectedGridData[nCount].DocumentClientUrl) {
            fileURL += oGridConfig.arrSelectedGridData[nCount].DocumentClientUrl + oEmailRelativePath;
        } else {
            fileURL += oGridConfig.arrSelectedGridData[nCount].SiteName + oEmailRelativePath;
        }
    }
    oParam = { "oSelectedGridData": oGridConfig.arrSelectedGridData, "nCount": nCount };
    matterDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "mailAttachmentDetails": { "FullUrl": fileURL } };

    oCommonObject.callLegalBriefcase("CheckedOutDocumentByUser", matterDetails, onSearchCheckedOutDocumentsSuccess, onSearchCheckedOutDocumentsFailure, beforeSearchCheckedOutDocuments, oParam);
    displayAttachmentCount(nCount);
    $(".popupContainerBackground, .popUpContainer").removeClass("hide");
    oGridConfig.inWebDashboard && mailContainer.addClass("placePopup");
    if (("undefined" !== typeof oSearchGlobal.sClientName && "undefined" !== typeof oSearchGlobal.sClientSiteUrl && "undefined" !== typeof oSearchGlobal.bIsTenantCall) && (("" !== oSearchGlobal.sClientName && "" !== oSearchGlobal.sClientSiteUrl) || oSearchGlobal.bIsTenantCall)) {
        $(".popUpContainer").addClass("popupSP");
    }
}
/* Hides the loading image shown on Mail cart popup*/
function hideLoadingIcon() {
    "use strict";
    $("#mailCartLoadingImage, #mailCartLoadingImage .LoadingImage").hide();
}

/* Function to show the loading image while the check out status of the documents is being retrieved */
function beforeSearchCheckedOutDocuments() {
    "use strict";
    $("#mailCartLoadingImage").show();
    $("#mailCartLoadingImage .LoadingImage").show();
}

/* Function to display the check out status of the documents */
function onSearchCheckedOutDocumentsSuccess(result, event) {
    "use strict";
    hideLoadingIcon();
    var sDocumentName = "", arrDocumentUrl = null, sDocumentUrl = null, iCount, oEmailRelativePath, iErrorCode, iTimeStamp, sHtmlChunk;
    oGridConfig.arrCheckedOutDoc = [];
    for (iCount = 0; iCount < result.oParam.nCount; iCount++) {
        if (0 <= result.Result[iCount].indexOf("#|#")) {
            var arrErrorPopUpData = JSON.parse(result.Result[iCount]).code.split("#|#");
            iErrorCode = arrErrorPopUpData[0];
            iTimeStamp = arrErrorPopUpData[1];
            if (oGridConfig.arrSelectedGridData[iCount].DocumentUrl) {
                sDocumentName = oGridConfig.arrSelectedGridData[iCount].DocumentName + "." + oGridConfig.arrSelectedGridData[iCount].DocumentExtension;
            } else {
                sDocumentName = oGridConfig.arrSelectedGridData[iCount].FileName;
            }

            sHtmlChunk = oGlobalConstants.Info_Popup_Error_Message + "<br><span class = \\'checkOut\\' id = \\'errorCode\\' title = \\' " + iErrorCode + "\\'>Error Code:\\'" + iErrorCode + "\\'</span><br><span class =\\'checkOut\\' id = \\'errorTimestamp\\' title = \\'" + iTimeStamp + "\\'>Timestamp:\\'" + iTimeStamp + "\\'</span>";
            $(".popUpContainer .popUpBody ul").append("<li class=\"attachmentList\" id=\"" + iCount + "\"><img alt=\"checkbox\" src = \"../Images/checkbox.png\" disabled class=\"mailCheck disabledCheckbox\" data-checked=\"false\" data-value=\" \"/><img class = \"sendingDocumentImg\" src = \"../Images/loadingcirclests16.gif\"/><span title = \"" + sDocumentName + "\">" + sDocumentName + "</span><img src=\"../Images/icon-delete.png\" class=\"attachmentRemove\" title=\"Remove attachment\" onclick = \"removeAttachment(this)\" /><img src=\"../Images/information-blue.png\" class=\"checkOutInfo\" onclick = \"checkOutInfo(this, event," + iCount + ",\'" + sHtmlChunk + "\');\" /><div class=\"popupText\"><span class = \"checkOut\" id = \"fileNotAvailable\" title=\"" + oGlobalConstants.File_Not_Available + "\">" + oGlobalConstants.File_Not_Available + "</span></br></div></li>");
        } else {
            if (result.oParam.oSelectedGridData[iCount].DocumentUrl) {
                arrDocumentUrl = trimEndChar(result.oParam.oSelectedGridData[iCount].DocumentUrl, "/").split("/");
                sDocumentName = result.oParam.oSelectedGridData[iCount].DocumentName + "." + result.oParam.oSelectedGridData[iCount].DocumentExtension;
                arrDocumentUrl[arrDocumentUrl.length - 1] = encodeURI(sDocumentName);
                sDocumentUrl = arrDocumentUrl.join("/");
                if (result.Result[iCount] === "unavailable") {
                    $(".popUpContainer .popUpBody ul").append("<li class=\"attachmentList\" id=\"" + iCount + "\"><img alt=\"checkbox\" src = \"../Images/checkbox.png\" disabled class=\"mailCheck disabledCheckbox\" data-checked=\"false\" data-value=\"" + sDocumentUrl + "\"/><img class = \"sendingDocumentImg\" src = \"../Images/loadingcirclests16.gif\"/><span title = \"" + sDocumentName + "\">" + sDocumentName + "</span><img src=\"../Images/icon-delete.png\" class=\"attachmentRemove\" title=\"Remove attachment\" onclick = \"removeAttachment(this)\" /><img src=\"../Images/information-blue.png\" class=\"checkOutInfo\" onclick = \"checkOutInfo(this, event," + iCount + ",\'" + oGlobalConstants.Info_Popup_Document_Check_Out + "\');\" /><div class=\"popupText\"><span class = \"checkOut\" id = \"fileNotFound\" title=\"" + oGlobalConstants.Checked_Out_File_Not_Found + "\">" + oGlobalConstants.Checked_Out_File_Not_Found + "</span><br></br><span class = \"checkOut\" id = \"fileMovedDeleted\" title = \"" + oGlobalConstants.Checked_Out_File_Not_Found_Moved_Deleted + "\">" + oGlobalConstants.Checked_Out_File_Not_Found_Moved_Deleted + "</span></div></li>");
                } else {
                    if (result.Result[iCount] === "") {
                        $(".popUpContainer .popUpBody ul").append("<li class=\"attachmentList\" id=\"" + iCount + "\"><img alt=\"checkbox\" src = \"../Images/checkbox.png\" class=\"mailCheck\" data-checked=\"false\" data-value=\"" + sDocumentUrl + "\"/><img class = \"sendingDocumentImg\" src = \"../Images/loadingcirclests16.gif\"/><span title = \"" + sDocumentName + "\">" + sDocumentName + "</span><img src=\"../Images/icon-delete.png\" class=\"attachmentRemove\"  title=\"Remove attachment\" onclick = \"removeAttachment(this)\" /><img src=\"../Images/information-blue.png\" class=\"checkOutInfo\" onclick = \"checkOutInfo(this, event," + iCount + ",\'" + oGlobalConstants.Info_Popup_Document_Check_Out + "\');\" /><div class=\"popupText\"><span class = \"checkOut\" id = \"isNotCheckedOut\" title=\"" + oGlobalConstants.Document_Checked_Out_Message + "\">" + oGlobalConstants.Document_Checked_Out_Message + "</span></div></li>");
                    } else {
                        //// Add document URL in array which are already checkout
                        oGridConfig.arrCheckedOutDoc.push(decodeURIComponent(sDocumentUrl));
                        $(".popUpContainer .popUpBody ul").append("<li class=\"attachmentList\" id=\"" + iCount + "\"><img alt=\"checkbox\" src = \"../Images/checkbox.png\" class=\"mailCheck\" data-checked=\"false\" data-value=\"" + sDocumentUrl + "\"/><img class = \"sendingDocumentImg\" src = \"../Images/loadingcirclests16.gif\"/><span title = \"" + sDocumentName + "\">" + sDocumentName + "</span><img src=\"../Images/icon-delete.png\" class=\"attachmentRemove\"  title=\"Remove attachment\"  onclick = \"removeAttachment(this)\" /><img  class=\"checkOutInfo\" onclick = \"checkOutInfo(this, event," + iCount + ",\'" + oGlobalConstants.Info_Popup_Document_Already_Check_Out + "\');\" src=\"../Images/information-blue.png\"/><div class=\"popupText\"><span class = \"checkOut\" id = \"convenienceCopy\" title=\"" + oGlobalConstants.Convenience_copy_available + "\"> " + oGlobalConstants.Convenience_copy_available + " </span><br></br><span class = \"checkOut\" id = \"checkedOutBy\" title=\"" + "(Checked out by " + result.Result[iCount] + ")" + "\"> (Checked out by " + result.Result[iCount] + ")</span></div></li>");
                    }
                }
            } else {
                arrDocumentUrl = trimEndChar(result.oParam.oSelectedGridData[iCount].Path, "/").split("/");
                sDocumentName = result.oParam.oSelectedGridData[iCount].FileName;
                arrDocumentUrl[arrDocumentUrl.length - 1] = encodeURI(sDocumentName);
                sDocumentUrl = arrDocumentUrl.join("/");

                if (result.Result[iCount] === "unavailable") {
                    $(".popUpContainer .popUpBody ul").append("<li class=\"attachmentList\" id=\"" + iCount + "\"><img alt=\"checkbox\" src = \"../Images/checkbox.png\" disabled class=\"mailCheck disabledCheckbox\" data-checked=\"false\" data-value=\"" + sDocumentUrl + "\"/><img class = \"sendingDocumentImg\" src = \"../Images/loadingcirclests16.gif\"/><span title = \"" + sDocumentName + "\">" + sDocumentName + "</span><img src=\"../Images/icon-delete.png\" class=\"attachmentRemove\" title=\"Remove attachment\" onclick = \"removeAttachment(this)\" /><img src=\"../Images/information-blue.png\" class=\"checkOutInfo\" onclick = \"checkOutInfo(this, event," + iCount + ",\'" + oGlobalConstants.Info_Popup_Document_Check_Out + "\');\" /><div class=\"popupText\"><span class = \"checkOut\" id = \"fileNotFound\" title=\"" + oGlobalConstants.Checked_Out_File_Not_Found + "\">" + oGlobalConstants.Checked_Out_File_Not_Found + "</span><br></br><span class = \"checkOut\" id = \"fileMovedDeleted\" title = \"" + oGlobalConstants.Checked_Out_File_Not_Found_Moved_Deleted + "\">" + oGlobalConstants.Checked_Out_File_Not_Found_Moved_Deleted + "</span></div></li>");
                } else {
                    if (result.Result[iCount] === "") {
                        $(".popUpContainer .popUpBody ul").append("<li class=\"attachmentList\" id=\"" + iCount + "\"><img alt=\"checkbox\" src = \"../Images/checkbox.png\" class=\"mailCheck\" data-checked=\"false\" data-value=\"" + sDocumentUrl + "\"/><img class = \"sendingDocumentImg\" src = \"../Images/loadingcirclests16.gif\"/><span title = \"" + sDocumentName + "\">" + sDocumentName + "</span><img src=\"../Images/icon-delete.png\" class=\"attachmentRemove\"  title=\"Remove attachment\" onclick = \"removeAttachment(this)\" /><img src=\"../Images/information-blue.png\" class=\"checkOutInfo\" onclick = \"checkOutInfo(this, event," + iCount + ",\'" + oGlobalConstants.Info_Popup_Document_Check_Out + "\');\" /><div class=\"popupText\"><span class = \"checkOut\" id = \"isNotCheckedOut\" title=\"" + oGlobalConstants.Document_Checked_Out_Message + "\">" + oGlobalConstants.Document_Checked_Out_Message + "</span></div></li>");
                    } else {
                        //// Add document URL in array which are already checkout
                        oGridConfig.arrCheckedOutDoc.push(decodeURIComponent(sDocumentUrl));
                        $(".popUpContainer .popUpBody ul").append("<li class=\"attachmentList\" id=\"" + iCount + "\"><img alt=\"checkbox\" src = \"../Images/checkbox.png\" class=\"mailCheck\" data-checked=\"false\" data-value=\"" + sDocumentUrl + "\"/><img class = \"sendingDocumentImg\" src = \"../Images/loadingcirclests16.gif\"/><span title = \"" + sDocumentName + "\">" + sDocumentName + "</span><img src=\"../Images/icon-delete.png\" class=\"attachmentRemove\"  title=\"Remove attachment\"  onclick = \"removeAttachment(this)\" /><img  class=\"checkOutInfo\" onclick = \"checkOutInfo(this, event," + iCount + ",\'" + oGlobalConstants.Info_Popup_Document_Already_Check_Out + "\');\" src=\"../Images/information-blue.png\"/><div class=\"popupText\"><span class = \"checkOut\" id = \"convenienceCopy\" title=\"" + oGlobalConstants.Convenience_copy_available + "\">" + oGlobalConstants.Convenience_copy_available + "</span><br></br><span class = \"checkOut\" id = \"checkedOutBy\" title=\"" + "(Checked out by " + result.Result[iCount] + ")" + "\"> (Checked out by " + result.Result[iCount] + ")</span></div></li>");
                    }
                }
            }
        }
    }
}
function onSearchErrorCheckedOutDocumentsSuccess(result, event) {
    "use strict";
    hideLoadingIcon();
    var sDocumentName = "", arrDocumentUrl = null, sDocumentUrl = null, iCount, oEmailRelativePath, iErrorCode, iTimeStamp, sHtmlChunk;
    if (0 <= result.Result[0].indexOf("#|#")) {
        var arrErrorPopUpData = JSON.parse(result.Result[iCount]).code.split("#|#");
        iErrorCode = arrErrorPopUpData[0];
        iTimeStamp = arrErrorPopUpData[1];
        $(".emailDocumentNames #" + errorDocumentNumber + " img.mailCheck").css("display", "inline");
        $(".emailDocumentNames #" + errorDocumentNumber + " .sendingDocumentImg").css("display", "none");

        if (oGridConfig.arrSelectedGridData[errorDocumentNumber].DocumentUrl) {
            sDocumentName = oGridConfig.arrSelectedGridData[errorDocumentNumber].DocumentName + "." + oGridConfig.arrSelectedGridData[errorDocumentNumber].DocumentExtension;
        } else {
            sDocumentName = oGridConfig.arrSelectedGridData[errorDocumentNumber].FileName;
        }

        sHtmlChunk = oGlobalConstants.Info_Popup_Error_Message + "<br><span class = \\'checkOut\\' id = \\'errorCode\\' title = \\' " + iErrorCode + "\\'>Error Code:\\'" + iErrorCode + "\\'</span><br><span class =\\'checkOut\\' id = \\'errorTimestamp\\' title = \\'" + iTimeStamp + "\\'>Timestamp:\\'" + iTimeStamp + "\\'</span>";
        $("ul li").eq(errorDocumentNumber)[0].innerHTML = "<img alt=\"checkbox\" src = \"../Images/checkbox.png\" disabled class=\"mailCheck disabledCheckbox\" data-checked=\"false\" data-value=\" \"/><img class = \"sendingDocumentImg\" src = \"../Images/loadingcirclests16.gif\"/><span title = \"" + sDocumentName + "\">" + sDocumentName + "</span><img src=\"../Images/icon-delete.png\" class=\"attachmentRemove\" title=\"Remove attachment\" onclick = \"removeAttachment(this)\" /><img src=\"../Images/information-blue.png\" class=\"checkOutInfo\" onclick = \"checkOutInfo(this, event," + errorDocumentNumber + ",\'" + sHtmlChunk + "\');\" /><div class=\"popupText\"><span class = \"checkOut\" id = \"fileNotAvailable\" title=\"" + oGlobalConstants.File_Not_Available + "\">" + oGlobalConstants.File_Not_Available + "</span></br></div></li>";

    } else {
        if (result.oParam.oSelectedGridData[0].DocumentUrl) {
            arrDocumentUrl = trimEndChar(result.oParam.oSelectedGridData[0].DocumentUrl, "/").split("/");
            sDocumentName = result.oParam.oSelectedGridData[0].DocumentName + "." + result.oParam.oSelectedGridData[0].DocumentExtension;
            arrDocumentUrl[arrDocumentUrl.length - 1] = encodeURI(sDocumentName);
            sDocumentUrl = arrDocumentUrl.join("/");

            if (result.Result[0] === "unavailable") {
                $("ul li").eq(errorDocumentNumber)[0].innerHTML = "<img alt=\"checkbox\" src = \"../Images/checkbox.png\" disabled class=\"mailCheck disabledCheckbox\" data-checked=\"false\" data-value=\"" + sDocumentUrl + "\"/><img class = \"sendingDocumentImg\" src = \"../Images/loadingcirclests16.gif\"/><span title = \"" + sDocumentName + "\">" + sDocumentName + "</span><img src=\"../Images/icon-delete.png\" class=\"attachmentRemove\" title=\"Remove attachment\" onclick = \"removeAttachment(this)\" /><img src=\"../Images/information-blue.png\" class=\"checkOutInfo\" onclick = \"checkOutInfo(this, event," + errorDocumentNumber + ",\'" + oGlobalConstants.Info_Popup_Document_Check_Out + "\');\" /><div class=\"popupText\"><span class = \"checkOut\" id = \"fileNotFound\" title=\"" + oGlobalConstants.Checked_Out_File_Not_Found + "\">" + oGlobalConstants.Checked_Out_File_Not_Found + "</span><br></br><span class = \"checkOut\" id = \"fileMovedDeleted\" title = \"" + oGlobalConstants.Checked_Out_File_Not_Found_Moved_Deleted + "\">" + oGlobalConstants.Checked_Out_File_Not_Found_Moved_Deleted + "</span></div>";
            } else {
                if (result.Result[0] === "") {
                    $("ul li").eq(errorDocumentNumber)[0].innerHTML = "<img alt=\"checkbox\" src = \"../Images/checkbox.png\" class=\"mailCheck\" data-checked=\"false\" data-value=\"" + sDocumentUrl + "\"/><img class = \"sendingDocumentImg\" src = \"../Images/loadingcirclests16.gif\"/><span title = \"" + sDocumentName + "\">" + sDocumentName + "</span><img src=\"../Images/icon-delete.png\" class=\"attachmentRemove\"  title=\"Remove attachment\" onclick = \"removeAttachment(this)\" /><img src=\"../Images/information-blue.png\" class=\"checkOutInfo\" onclick = \"checkOutInfo(this, event," + errorDocumentNumber + ",\'" + oGlobalConstants.Info_Popup_Document_Check_Out + "\');\" /><div class=\"popupText\"><span class = \"checkOut\" id = \"isNotCheckedOut\" title=\"" + oGlobalConstants.Document_Checked_Out_Message + "\">" + oGlobalConstants.Document_Checked_Out_Message + "</span></div>";
                } else {
                    $("ul li").eq(errorDocumentNumber)[0].innerHTML = "<img alt=\"checkbox\" src = \"../Images/checkbox.png\" class=\"mailCheck\" data-checked=\"false\" data-value=\"" + sDocumentUrl + "\"/><img class = \"sendingDocumentImg\" src = \"../Images/loadingcirclests16.gif\"/><span title = \"" + sDocumentName + "\">" + sDocumentName + "</span><img src=\"../Images/icon-delete.png\" class=\"attachmentRemove\"  title=\"Remove attachment\"  onclick = \"removeAttachment(this)\" /><img  class=\"checkOutInfo\" onclick = \"checkOutInfo(this, event," + errorDocumentNumber + ",\'" + oGlobalConstants.Info_Popup_Document_Already_Check_Out + "\');\" src=\"../Images/information-blue.png\"/><div class=\"popupText\"><span class = \"checkOut\" id = \"convenienceCopy\" title=\"" + oGlobalConstants.Convenience_copy_available + "\"> " + oGlobalConstants.Convenience_copy_available + " </span><br></br><span class = \"checkOut\" id = \"checkedOutBy\" title=\"" + "(Checked out by " + result.Result[0] + ")" + "\"> (Checked out by " + result.Result[0] + ")</span></div>";
                }
            }
        } else {
            arrDocumentUrl = trimEndChar(result.oParam.oSelectedGridData[0].Path, "/").split("/");
            sDocumentName = result.oParam.oSelectedGridData[0].FileName;
            arrDocumentUrl[arrDocumentUrl.length - 1] = encodeURI(sDocumentName);
            sDocumentUrl = arrDocumentUrl.join("/");

            if (result.Result[0] === "unavailable") {
                $("ul li").eq(errorDocumentNumber)[0].innerHTML = "<img alt=\"checkbox\" src = \"../Images/checkbox.png\" disabled class=\"mailCheck disabledCheckbox\" data-checked=\"false\" data-value=\"" + sDocumentUrl + "\"/><img class = \"sendingDocumentImg\" src = \"../Images/loadingcirclests16.gif\"/><span title = \"" + sDocumentName + "\">" + sDocumentName + "</span><img src=\"../Images/icon-delete.png\" class=\"attachmentRemove\" title=\"Remove attachment\" onclick = \"removeAttachment(this)\" /><img src=\"../Images/information-blue.png\" class=\"checkOutInfo\" onclick = \"checkOutInfo(this, event," + errorDocumentNumber + ",\'" + oGlobalConstants.Info_Popup_Document_Check_Out + "\');\" /><div class=\"popupText\"><span class = \"checkOut\" id = \"fileNotFound\" title=\"" + oGlobalConstants.Checked_Out_File_Not_Found + "\">" + oGlobalConstants.Checked_Out_File_Not_Found + "</span><br></br><span class = \"checkOut\" id = \"fileMovedDeleted\" title = \"" + oGlobalConstants.Checked_Out_File_Not_Found_Moved_Deleted + "\">" + oGlobalConstants.Checked_Out_File_Not_Found_Moved_Deleted + "</span></div>";
            } else {
                if (result.Result[0] === "") {
                    $("ul li").eq(errorDocumentNumber)[0].innerHTML = "<img alt=\"checkbox\" src = \"../Images/checkbox.png\" class=\"mailCheck\" data-checked=\"false\" data-value=\"" + sDocumentUrl + "\"/><img class = \"sendingDocumentImg\" src = \"../Images/loadingcirclests16.gif\"/><span title = \"" + sDocumentName + "\">" + sDocumentName + "</span><img src=\"../Images/icon-delete.png\" class=\"attachmentRemove\"  title=\"Remove attachment\" onclick = \"removeAttachment(this)\" /><img src=\"../Images/information-blue.png\" class=\"checkOutInfo\" onclick = \"checkOutInfo(this, event," + errorDocumentNumber + ",\'" + oGlobalConstants.Info_Popup_Document_Check_Out + "\');\" /><div class=\"popupText\"><span class = \"checkOut\" id = \"isNotCheckedOut\" title=\"" + oGlobalConstants.Document_Checked_Out_Message + "\">" + oGlobalConstants.Document_Checked_Out_Message + "</span></div>";
                } else {
                    $("ul li").eq(errorDocumentNumber)[0].innerHTML = "<img alt=\"checkbox\" src = \"../Images/checkbox.png\" class=\"mailCheck\" data-checked=\"false\" data-value=\"" + sDocumentUrl + "\"/><img class = \"sendingDocumentImg\" src = \"../Images/loadingcirclests16.gif\"/><span title = \"" + sDocumentName + "\">" + sDocumentName + "</span><img src=\"../Images/icon-delete.png\" class=\"attachmentRemove\"  title=\"Remove attachment\"  onclick = \"removeAttachment(this)\" /><img  class=\"checkOutInfo\" onclick = \"checkOutInfo(this, event," + errorDocumentNumber + ",\'" + oGlobalConstants.Info_Popup_Document_Already_Check_Out + "\');\" src=\"../Images/information-blue.png\"/><div class=\"popupText\"><span class = \"checkOut\" id = \"convenienceCopy\" title=\"" + oGlobalConstants.Convenience_copy_available + "\"> " + oGlobalConstants.Convenience_copy_available + " </span><br></br><span class = \"checkOut\" id = \"checkedOutBy\" title=\"" + "(Checked out by " + result.Result[0] + ")" + "\"> (Checked out by " + result.Result[0] + ")</span></div>";
                }
            }
        }
    }
    errorDocumentNumber = -1;
}
/* Function to display failure notification in case the check-out status of the documents cannot be retrieved */
function onSearchCheckedOutDocumentsFailure(result, event) {
    "use strict";
    $(".popUpHeader .notificationContainerForPopup").empty();
    var sFailContent = '<div class="notification failNotification">' + '<img alt="warning" id = "warningImg" src = "../Images/warning-message.png"/> Failed to retrieve document check-out status. <span class="closeFailNotification">x</span></div>';
    $(".popUpHeader .notificationContainerForPopup").append(sFailContent);
    hideLoadingIcon();
}

/* Function to show the check out status information of each document */
function checkOutInfo(element, event, attachmentNumber, message) {
    "use strict";
    var sContent = "";
    $(".checkOutMetadata").text("");
    $(".checkOutMetadata").html(message);
    $(".ErrorTryAgainLink").attr({ href: "javascript:void(0);", onclick: "tryAgainShowPopup(" + attachmentNumber + ")" });
    var documentList = $(element)[0].parentNode.parentNode;
    var scrollTopValue = (documentList.scrollTop !== 0 ? documentList.scrollTop : documentList.parentNode.scrollTop);
    $(".checkOutPopup").css({ top: $(element)[0].offsetTop - scrollTopValue - 34 });
    $(".checkOutPopup").show();
    iParentNodeID = $(element).parent()[0].id;
    commonFunction.closeAllFilterExcept("checkOutPopup", event);
    if (!event) {
        event = window.event;
        event.cancelBubble = true;
    }
    if (event.stopPropagation) {
        event.stopPropagation();
    }
}

function tryAgainShowPopup(attachmentNumber) {
    "use strict";
    errorDocumentNumber = attachmentNumber;
    $(".popUpCloseIcon").remove();
    $(".checkOutPopup").hide();
    $(".notification").remove();
    $(".emailDocumentNames #" + attachmentNumber + " img.mailCheck").css("display", "none");
    $(".emailDocumentNames #" + attachmentNumber + " .sendingDocumentImg").css("display", "inline");
    var oEmailRelativePath, fileURL = "", oParam, matterDetails, data = [];
    data[0] = oGridConfig.arrSelectedGridData[attachmentNumber];
    if (data.DocumentUrl) {
        oEmailRelativePath = unescape(data[0].DocumentUrl);
    } else {
        oEmailRelativePath = unescape(data[0].Path);
    }
    oEmailRelativePath = trimEndChar(oEmailRelativePath, "/");
    oEmailRelativePath = oEmailRelativePath.replace(oGlobalConstants.Site_Url, "$") + ";";
    if (data.DocumentClientUrl) {
        fileURL += data.DocumentClientUrl + oEmailRelativePath;
    } else {
        fileURL += oGridConfig.arrSelectedGridData[attachmentNumber].SiteName + oEmailRelativePath;
    }
    oParam = { "oSelectedGridData": data, "nCount": 1 };
    matterDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "mailAttachmentDetails": { "FullUrl": fileURL } };

    oCommonObject.callLegalBriefcase("CheckedOutDocumentByUser", matterDetails, onSearchErrorCheckedOutDocumentsSuccess, onSearchCheckedOutDocumentsFailure, null, oParam);
    $(".popupContainerBackground, .popUpContainer").removeClass("hide");

    if (("undefined" !== typeof oSearchGlobal.sClientName && "undefined" !== typeof oSearchGlobal.sClientSiteUrl && "undefined" !== typeof oSearchGlobal.bIsTenantCall) && (("" !== oSearchGlobal.sClientName && "" !== oSearchGlobal.sClientSiteUrl) || oSearchGlobal.bIsTenantCall)) {
        $(".popUpContainer").addClass("popupSP");
    }
}

/* Function to send the documents to user's One Drive */
function sendToBriefcase() {
    "use strict";
    var oParam, matterDetails, arrMatches;

    iOverWriteDivCount = 0;
    iDocumentCount = 0;

    arrMatches = modifySelectedFileURLS();

    if (arrMatches.length > 0) {
        // Prevent the click events if there is at least one document selected.
        // Add a return false to each of the links when any link is clicked.
        $(".sendEmailOptions").each(function () {
            "use strict";
            var oCurrentLink = $(this);
            if (oCurrentLink[0]) {
                oCurrentLink.attr("onclick", "return false;" + oCurrentLink.attr("onclick"));
            }
        });
        var sFileURLs = "", iCount, oEmailRelativePath;
        iTotalDocumentCount = arrMatches.length;

        for (iCount = 0; iCount < arrMatches.length; iCount++) {
            if (arrMatches[iCount].DocumentUrl) {
                oEmailRelativePath = decodeURIComponent(unescape(arrMatches[iCount].DocumentUrl));
            } else {
                oEmailRelativePath = decodeURIComponent(unescape(arrMatches[iCount].Path));
            }
            oEmailRelativePath = trimEndChar(oEmailRelativePath, "/");
            oEmailRelativePath = oEmailRelativePath.replace(decodeURIComponent(oGlobalConstants.Site_Url), "$") + ";";
            sFileURLs += arrMatches[iCount].DocumentClientUrl ? decodeURIComponent(arrMatches[iCount].DocumentClientUrl) + oEmailRelativePath : arrMatches[iCount].SiteName + oEmailRelativePath;
        }

        matterDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "mailAttachmentDetails": { "FullUrl": sFileURLs, "IsOverwrite": 0 }, "doCheckOut": true };
        oParam = { "iCount": 2, "FullUrl": sFileURLs };
        oCommonObject.callLegalBriefcase("SendToBriefcase", matterDetails, onSearchDocumentsSuccess, onSearchDocumentsFailure, beforeLegalBriefcaseOperations, oParam);
    }
}

/* Function to retrieve the URL of the documents if already existing in the user's One Drive and/or display the success notification in case of successfully sending out all the new documents to user's One Drive */
function onSearchDocumentsSuccess(result) {
    "use strict";
    if ($(".notification")) {
        $(".notification").remove();
    }
    var iDocuments = $(".attachmentList").length, iIterator = 0,
        oParsedResultDoc = $.parseJSON(result.Result), sResultDoc = oParsedResultDoc.value;
    if (!oParsedResultDoc.code || "0" === oParsedResultDoc.code) {
        var arrResultDoc = sResultDoc.lastIndexOf(";") === sResultDoc.length - 1 ? sResultDoc.substr(0, (sResultDoc.length - 1)).split(";") : sResultDoc.split(";"),
        iCount = result.oParam.iCount, oParam, matterDetails;
        for (iIterator = 0; iIterator < iDocuments; iIterator++) {
            $(".emailDocumentNames #" + iIterator + " img.mailCheck").css("display", "inline");
            $(".emailDocumentNames #" + iIterator + " .sendingDocumentImg").css("display", "none");
        }
        if ("false" !== arrResultDoc[0]) {
            iDocumentCount += parseInt(arrResultDoc[1], 10);
            sOneDriveURL = arrResultDoc[0];

            if (iCount === arrResultDoc.length - 1) {
                showNotification(iDocumentCount, sOneDriveURL, "successNotification", iTotalDocumentCount, 0);
            }
            while (iCount < arrResultDoc.length - 1) {
                iCount++;
                onSearchExistingDocumentsSuccess({ "iCount": iCount, "FullUrl": arrResultDoc[iCount] });

            }
        } else {    // In case collection is not initialized display error notification.        
            showNotification(iDocuments, arrResultDoc[1], "failNotification", iDocuments, 1);
        }
    } else {
        sOneDriveErrorMessage = sResultDoc; //// to be used in case of error in configuration of OneDrive
        showNotification(iDocuments, oParsedResultDoc.code, "failNotification", iDocuments, 2);
    }
}

/* Function to display the failure notification in case of failure in sending the documents to the user's One Drive */
function onSearchDocumentsFailure(result) {
    "use strict";
    showNotification(iDocumentCount, sOneDriveURL, "failNotification", iTotalDocumentCount, 0);
}

/* Function to display notification to user for every document that is already existing in the user's One Drive */
function onSearchExistingDocumentsSuccess(result) {
    "use strict";

    var iDocuments = $(".attachmentList").length, iIterator = 0, sContent;
    for (iIterator = 0; iIterator < iDocuments; iIterator++) {
        $(".emailDocumentNames #" + iIterator + " img.mailCheck").css("display", "inline");
        $(".emailDocumentNames #" + iIterator + " .sendingDocumentImg").css("display", "none");
    }

    iOverWriteDivCount++;
    if ("" !== result.FullUrl) {
        if (iOverWriteDivCount > 1) {
            sContent = '<div class="notification warningNotification hide" id = "' + iOverWriteDivCount + '"> <div class="notificationContent"><img id = "warningImg" src = "../Images/warning-message.png"/></div> <div class="overWriteTextContainer"><span id = "overWriteDocumentName"> ' + result.FullUrl.split("$")[1].substring(result.FullUrl.split("$")[1].lastIndexOf("/") + 1) + "</span> already exists in your OneDrive. Do you want to overwrite it?" + '<br/><span class="askForOverwrite"> <input type="button" id = "overWriteYes" value="Yes" onclick="overWriteDocument(this);" data-url = "' + result.FullUrl + '"> <input type="button" id = "overWriteNo" value="No" onclick="overWriteDocument(this);"> </span> </div></div>';
        } else {
            sContent = '<div class="notification warningNotification" id = "' + iOverWriteDivCount + '"> <div class="notificationContent"><img id = "warningImg" src = "../Images/warning-message.png"/></div> <div class="overWriteTextContainer"><span id = "overWriteDocumentName"> ' + result.FullUrl.split("$")[1].substring(result.FullUrl.split("$")[1].lastIndexOf("/") + 1) + "</span> already exists in your OneDrive. Do you want to overwrite it?" + '<br/><span class="askForOverwrite"> <input type="button" id = "overWriteYes" value="Yes" onclick="overWriteDocument(this);" data-url = "' + result.FullUrl + '"> <input type="button" id = "overWriteNo" value="No" onclick="overWriteDocument(this);"> </span> </div></div>';
        }
        $(".popUpHeader .notificationContainerForPopup").append(sContent);
    }
}

/* Function to retrieve the user's choice to overwrite a document or not and depending on that either overwrite that document or not */
function overWriteDocument(oDocument) {
    "use strict";
    var sNotificationPopup = oDocument.parentNode.parentNode.parentNode.id;
    $(".notificationContainerForPopup #" + sNotificationPopup + " .askForOverwrite #overWriteYes")[0].disabled = true;
    $(".notificationContainerForPopup #" + sNotificationPopup + " .askForOverwrite #overWriteNo")[0].disabled = true;
    var sDocumentURL = "", oMatterDetails = {}, oParam = { "divId": sNotificationPopup, "FullUrl": "" };
    if ("Yes" === oDocument.value) {
        sDocumentURL = oDocument.getAttribute("data-url");
        oParam.FullUrl = sDocumentURL;
        oMatterDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "mailAttachmentDetails": { "FullUrl": sDocumentURL, "IsOverwrite": 1 }, "doCheckOut": true };

        oCommonObject.callLegalBriefcase("SendToBriefcase", oMatterDetails, onOverWriteDocumentSuccess, onOverWriteDocumentFailure, beforeLegalBriefcaseOperations, oParam);
    } else {
        sDocumentURL = $(oDocument).parent().find("#overWriteYes").attr("data-url");
        var sDocumentFullUrl = oGlobalConstants.Site_Url + sDocumentURL.split("$")[1], sDocumentName = "";
        oParam.FullUrl = sDocumentURL;
        if (-1 === $.inArray(sDocumentFullUrl, oGridConfig.arrCheckedOutDoc)) {
            oMatterDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "mailAttachmentDetails": { "FullUrl": sDocumentURL, "IsOverwrite": 1 }, "doCheckOut": true };
            sDocumentName = sDocumentFullUrl.split("/");
            arrCheckOutFailDocument.push(sDocumentName[sDocumentName.length - 1]);
            oCommonObject.callLegalBriefcase("DiscardCheckOutChanges", oMatterDetails, onOverWriteDocumentCancelSuccess, onOverWriteDocumentCancelFail, null, null);
        }
        var notificationContainer = $(".notificationContainerForPopup #" + sNotificationPopup);
        if (notificationContainer.length !== 0) {
            notificationContainer.remove();
        }
        if ($(".warningNotification").length === 0) {
            showNotification(iDocumentCount, sOneDriveURL, "successNotification", iTotalDocumentCount, 0);
        } else {
            var iTempCount = parseInt(sNotificationPopup, 10) + 1;
            $(".notificationContainerForPopup #" + iTempCount).show();
        }
    }

}

function onOverWriteDocumentCancelSuccess() {
    "use strict";
    arrCheckOutFailDocument.pop();
}

// Display error message while checkout fails
function onOverWriteDocumentCancelFail() {
    "use strict";
    var sDocument = arrCheckOutFailDocument.pop();
    showNotification(iDocumentCount, sDocument, "failNotification", 0, 3);  // Use 0 for number of document to display and use 3 for display check out fail notification 
}

/* On successfully overwriting a document, this function will display the next document overwrite notification or the success notification depending on the number of documents left to be asked to the user to be overwritten or not */
function onOverWriteDocumentSuccess(result) {
    "use strict";
    if (result) {
        var oParsedResult = JSON.parse(result.Result);
        if (!oParsedResult.code || "0" === oParsedResult.code) {
            var iDocuments = $(".attachmentList").length, iIterator = 0, notificationContainer;
            for (iIterator = 0; iIterator < iDocuments; iIterator++) {
                $(".emailDocumentNames #" + iIterator + " img.mailCheck").css("display", "inline");
                $(".emailDocumentNames #" + iIterator + " .sendingDocumentImg").css("display", "none");
            }

            iDocumentCount += 1;
            notificationContainer = $(".notificationContainerForPopup #" + result.oParam.divId);

            if (notificationContainer.length !== 0) {
                notificationContainer.remove();
            }
            if ($(".warningNotification").length === 0) {
                sOneDriveURL = $.parseJSON(result.Result).value.split(";")[0];
                showNotification(iDocumentCount, sOneDriveURL, "successNotification", iTotalDocumentCount, 0);
            } else {
                var iTempCount = parseInt(result.oParam.divId, 10) + 1;
                $(".notificationContainerForPopup #" + iTempCount).show();
            }
        } else {
            $(".notification").remove();
            showNotification(iDocuments, oParsedResult.code, "failNotification", iDocuments, 2);
        }
    }
}

/* Click event for close notification popup*/
$(document).on("click", ".closeFailNotification", function () {
    "use strict";
    $(this).parent().remove();
});

/* Function to display failure notification if the document was not successfully overwritten */
function onOverWriteDocumentFailure(result) {
    "use strict";
    if ($(".warningNotification").length > 0) {
        $(".warningNotification").remove();
    }
    showNotification(iDocumentCount, sOneDriveURL, "failNotification", iTotalDocumentCount, 0);
    return "false";
}

/* Function to display loading images next to the documents in the Mail Cart for which the Send to Briefcase or overwrite functionality is being carried out */
function beforeLegalBriefcaseOperations(result) {
    "use strict";
    var iIterateFiles = 0, iIterator = 0,
        arrFileURLs = result.oParam.FullUrl.split(";"),
        iDocuments = $(".attachmentList").length,
        arrMatches = [];

    arrMatches = oGridConfig.arrSelectedGridData;

    if (arrMatches.length > 0) {
        var sFileURLs = "", iCount, oEmailRelativePath;
        for (iCount = 0; iCount < arrMatches.length; iCount++) {
            if (arrMatches[iCount].DocumentUrl) {
                oEmailRelativePath = unescape(arrMatches[iCount].DocumentUrl);
            } else {
                oEmailRelativePath = unescape(arrMatches[iCount].Path);
            }
            oEmailRelativePath = trimEndChar(oEmailRelativePath, "/");
            oEmailRelativePath = oEmailRelativePath.replace(oGlobalConstants.Site_Url, "$") + "$";
            if (arrMatches[iCount].DocumentClientUrl) {
                sFileURLs += arrMatches[iCount].DocumentTeamUrl + oEmailRelativePath + arrMatches[iCount].FileName + ";";
            } else {
                sFileURLs += arrMatches[iCount].SiteName + oEmailRelativePath + arrMatches[iCount].FileName + ";";
            }
        }
    }

    var arrSelectedFileURLs = sFileURLs.split(";");

    for (iIterateFiles = 0; iIterateFiles < arrFileURLs.length; iIterateFiles++) {
        if (arrFileURLs[iIterateFiles] !== "") {
            for (iIterator = 0; iIterator < iDocuments; iIterator++) {
                if (arrSelectedFileURLs[iIterator] === arrFileURLs[iIterateFiles]) {
                    $(".emailDocumentNames #" + iIterator + " img.mailCheck").css("display", "none");
                    $(".emailDocumentNames #" + iIterator + " .sendingDocumentImg").css("display", "inline");
                }
            }
        }
    }
}

/* Function to display success/failure notification */
function showNotification(iSentDocumentCount, sMsg, resultClass, iTotalDocument, bUninitializedError) {
    "use strict";
    // Remove the return false to enable the links again
    $(".sendEmailOptions").each(function () {
        "use strict";
        var oCurrentLink = $(this);
        if (oCurrentLink[0]) {
            oCurrentLink.attr("onclick", oCurrentLink.attr("onclick").replace(/return false;/g, ""));
        }
    });

    $("#legalBriefcaseLoadingImage").hide();
    var sContent = "", sFailContent = "";
    if (iSentDocumentCount === 1) {
        sContent = '<div class="notification successNotification successNotificationPopup">' + '<div><img id = "warningImg" src = "../Images/success-message.png"/></div>' + iSentDocumentCount + ' File sent to your OneDrive. You can <span class="briefcaseURL" onclick="window.open(\'' + sMsg + "/" + oGlobalConstants.Legal_Briefcase_Folder_Name + '\', \'_parent\')">view here </span><span class="closeNotification">x</span></div>';
    } else {
        sContent = '<div class="notification successNotification successNotificationPopup">' + '<div><img id = "warningImg" src = "../Images/success-message.png"/></div>' + iSentDocumentCount + ' Files sent to your OneDrive. You can <span class="briefcaseURL" onclick="window.open(\'' + sMsg + "/" + oGlobalConstants.Legal_Briefcase_Folder_Name + '\', \'_parent\')">view here </span><span class="closeNotification">x</span></div>';
    }
    // Do not show the success message for 0 documents uploaded
    if (0 < iSentDocumentCount) {
        $(".popUpHeader .notificationContainerForPopup").append(sContent);
    }
    if (resultClass === "failNotification") {
        $(".successNotification").hide();
        if (1 === bUninitializedError) {    // In case user's OneDrive is not set up
            sFailContent = '<div class="notification ' + resultClass + '">' + '<img id = "warningImg" src = "../Images/warning-message.png"/>' + oGlobalConstants.ErrorMessageSendToOneDrive + '<span class="closeFailNotification">x</span></div>';
        } else if (2 === bUninitializedError) { //// In case of exception occurred in service
            var sErrorCode = sMsg && sMsg.split("#|#") && sMsg.split("#|#")[0];
            if (sErrorCode === oGlobalConstants.ErrorCodeOneDriveNotConfigured) {
                sFailContent = '<div class="notification ' + resultClass + '">' + '<img id = "warningImg" src = "../Images/warning-message.png"/>' + oGlobalConstants.ErrorMessageOneDriveNotConfigured.replace("{0}", sOneDriveErrorMessage) + '<span class="closeFailNotification">x</span></div>';
            } else {
                sFailContent = '<div class="notification ' + resultClass + '">' + '<img id = "warningImg" src = "../Images/warning-message.png"/>' + oGlobalConstants.Send_To_OneDrive_Error_Message + sErrorCode + '<span class="closeFailNotification">x</span></div>';
            }
            $(".attachmentList .mailCheck").show(); //// need to perform show hide as previous implementation does not allow to use add and remove class
            $(".attachmentList .sendingDocumentImg").hide();
        } else if (3 === bUninitializedError) {  //// need to show error while failed to checkout
            sFailContent = '<div class="notification ' + resultClass + '">' + '<img id = "warningImg" src = "../Images/warning-message.png"/>' + oGlobalConstants.Check_Out_Document_Fail + " " + sMsg + '<span class="closeFailNotification">x</span></div>';
        } else {
            var iFailedDocument = iTotalDocument - iSentDocumentCount;
            sFailContent = '<div class="notification ' + resultClass + '">' + '<img id = "warningImg" src = "../Images/warning-message.png"/> Failed to send ' + iFailedDocument + ' document to OneDrive. <span class="closeFailNotification">x</span></div>';
        }
        $(".popUpHeader .notificationContainerForPopup").append(sFailContent);
    }
}

$(document).ready(function () {
    "use strict";
    $(document).on("click", ".mailCheck", function () {
        // Set the image to checked if unchecked and vice versa on click.
        // data-checked attribute will be true or false based on the image.
        var oCheckboxImage = $(this);
        if (oCheckboxImage[0]) {
            var sDataChecked = oCheckboxImage.attr("data-checked");
            if (sDataChecked && "false" === sDataChecked) {
                oCheckboxImage.attr({ "src": "../Images/checkbox-selected.png", "data-checked": "true" });
            } else {
                oCheckboxImage.attr({ "src": "../Images/checkbox.png", "data-checked": "false" });
            }
        }
    });
});


$(window).on("resize", function () {
    "use strict";
    // hide infoPopup
    $(".checkOutPopup").hide();
});