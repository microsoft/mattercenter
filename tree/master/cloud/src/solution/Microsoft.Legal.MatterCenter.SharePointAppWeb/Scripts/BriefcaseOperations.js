/// <disable>JS2032,JS2074,JS1003,JS2024,JS2076,JS3058,JS3057,JS3092,JS3056,JS3116</disable>


var oGlobalConstants = {
    sUpdateDocument: oLegalBriefcaseConstants.Update_Copy_Message,
    sCheckOut: oLegalBriefcaseConstants.Checkout_Message,
    sUpdateDocumentSuccess: oLegalBriefcaseConstants.Update_Document_Success,
    sCheckOutSuccess: oLegalBriefcaseConstants.Checkout_Success,
    sCheckOutFailure: oLegalBriefcaseConstants.Checkout_Failure,
    sCheckIn: oLegalBriefcaseConstants.CheckIn_Message,
    sCheckInSuccess: oLegalBriefcaseConstants.CheckIn_Success,
    sGenericSuccess: oLegalBriefcaseConstants.Generic_Success_Message,
    sGenericFailure: oLegalBriefcaseConstants.Generic_Failure_Message,
    sRelaxMsg: oLegalBriefcaseConstants.Relax_Message,
    sUpdateDocumentFailure: oLegalBriefcaseConstants.Update_Document_Failure,
    sUpdateDocumentPartial: oLegalBriefcaseConstants.Update_Document_Partial,
    sCheckOutPartial: oLegalBriefcaseConstants.CheckOut_Partial_Message,
    sAlreadyCheckOut: oLegalBriefcaseConstants.CheckIn_Already_Checkout,
    sDetach: oLegalBriefcaseConstants.Detach_Document_Message,
    sDetachSuccess: oLegalBriefcaseConstants.Detach_Document_Success,
    sDetachFailure: oLegalBriefcaseConstants.Detach_Document_Failure,
    sDetachQuestion: oLegalBriefcaseConstants.Detach_Document_Question,
    sNotSupported: oLegalBriefcaseConstants.Not_Supported_Message,
    sCheckInPartial: oLegalBriefcaseConstants.CheckIn_Partial,
    sVersionInfoFailure: oLegalBriefcaseConstants.Version_Info_Failed,
    sPartialDetach: oLegalBriefcaseConstants.Detach_Partial,
    sGoodNews: oLegalBriefcaseConstants.Good_News_Message,
    sUpdateQuestion: oLegalBriefcaseConstants.Update_Document_Question,
    sQuestion: oLegalBriefcaseConstants.Question,
    sErrorMessage: "Some Error Occurred",
    sWrongParameter: "Invalid Parameters are passed!",
    sUnSupportedList: "<ul class=\"UnSupportedList\"></ul>",
    iUpdateOperation: 1,
    iCheckOutOperation: 2,
    iDetachOperation: 3,
    iCheckInOperation: 4,
    iCurrentOperation: 0,
    itemId: null,
    listId: null,
};

var briefcaseDetails = {};

var appInsights;
// Function is used to make service calls
function syncBriefcase(itemId, listId, operationType) {
    "use strict";
    itemId = itemId.split(",");
    if (itemId && listId) {
        var briefcaseDetails = { "requestObject": { "SPAppToken": "", "RefreshToken": oSharePointContext.RefreshToken }, "syncDetails": { "ListId": listId, "ItemId": itemId, "Operation": operationType } };
        oCommonObject.callLegalBriefcase("SyncBriefcase", briefcaseDetails, onSuccess, onFailure, null, null);
    }
}


// On success function of Sync briefcase
function onSuccess(result) {
    "use strict";
    removeLoadingClasses();
    var status = "true";
    var oResult = JSON.parse(result);
    if (oResult.Status) {
        status = oResult.Status;
        if ("object" === typeof (oResult.Status)) {
            if (oResult.ErrorMessage && "" !== oResult.ErrorMessage) {
                status = "false";
            } else {
                status = "true";
            }
        }
    }
    if ("true" === status) {
        var bFailed = checkFailed(oResult);
        switch (oGlobalConstants.iCurrentOperation) {
            case 1:
                if (!bFailed) {
                    displayMessage(oGlobalConstants.sUpdateDocumentSuccess, true);
                } else {
                    displayMessage(oGlobalConstants.sUpdateDocumentPartial, true);
                }
                break;
            case 2:
                if (!bFailed) {
                    displayMessage(oGlobalConstants.sCheckOutSuccess, true);
                } else {
                    displayMessage(oGlobalConstants.sCheckOutPartial, true);
                }
                break;
            case 3:
                if (!bFailed) {
                    displayMessage(oGlobalConstants.sDetachSuccess, true);
                } else {
                    displayMessage(oGlobalConstants.sPartialDetach, true);
                }
                break;
        }
    } else {
        if (oResult.ErrorMessage && "" !== oResult.ErrorMessage) {
            displayMessage(oResult.ErrorMessage, false);
        } else {
            displayNotSupported();
        }
    }
}

// Failure function of Sync Briefcase
function onFailure(result) {
    "use strict";
    switch (oGlobalConstants.iCurrentOperation) {
        case 1:
            displayMessage(oGlobalConstants.sUpdateDocumentFailure, false);
            break;

        case 2:
            displayMessage(oGlobalConstants.sCheckOutFailure, false);
            break;

        case 3:
            displayMessage(oGlobalConstants.sDetachFailure, false);
            break;
    }
}


// Function is used to perform check for multiple files
function checkFailed(oResult) {
    "use strict";
    var bFailed = false, iIterator = 0, iLength = oResult.Status.length;
    for (iIterator = 0; iIterator < iLength; iIterator++) {
        if (!oResult.Status[iIterator]) {
            bFailed = true;
        }
    }
    return bFailed;
}


// Function is used to get version of the files that have been requested for check in by user
function getVersion(itemId, listId, operationType) {
    "use strict";
    var arrItemId = itemId.split(",");
    if (arrItemId && listId) {
        var briefcaseDetails = { "requestObject": { "SPAppToken": "", "RefreshToken": oSharePointContext.RefreshToken }, "syncDetails": { "ListId": listId, "ItemId": arrItemId, "Operation": operationType } };
        oCommonObject.callLegalBriefcase("GetVersion", briefcaseDetails, onVersionSuccess, onVersionFailure, null, null);
    }
}

// Success function of version check
function onVersionSuccess(result) {
    "use strict";
    removeLoadingClasses();
    var versionInfo = JSON.parse(result);
    if (versionInfo.Status) {
        if ("false" !== versionInfo.Status) {
            displayVersion(versionInfo);
        } else {
            displayNotSupported();
        }
    } else {
        if ("Already Checked Out" === versionInfo.ErrorMessage) {
            displayMessage(oGlobalConstants.sAlreadyCheckOut, false);
        } else {
            displayMessage(versionInfo.ErrorMessage, false);
        }
    }
}

// Failure function of version check
function onVersionFailure(result) {
    "use strict";
    displayMessage(oGlobalConstants.sVersionInfoFailure, false);
}


// Function is used to display version
function displayVersion(oVersionObject) {
    "use strict";
    var iIterator = 0, iLength = oVersionObject.RelativeURL.length;
    var documentId = oGlobalConstants.itemId;
    var arrDocumentId = documentId.split(",");
    briefcaseDetails = { "requestObject": { "SPAppToken": "", "RefreshToken": oSharePointContext.RefreshToken }, "briefcaseDetails": [], "versionInfo": null, "comments": null, "retainCheckout": null, "convenienceCopy": null };
    var oBriefCase = { "DocumentId": null, "DocumentUrl": null };
    if (1 === iLength) {
        if (oVersionObject.IsMajorVersion[0] && oVersionObject.IsMinorVersion[0]) {
            var sMinorVersion = oVersionObject.CurrentMajorVersion[0] + "." + (parseInt(oVersionObject.CurrentMinorVersion[0], 10) + 1);
            var sMajorVersion = (parseInt((oVersionObject.CurrentMajorVersion[0]), 10) + 1) + ".0";
            if (1 <= parseInt(oVersionObject.CurrentMinorVersion[0], 10)) {
                var sOverwriteVersion = oVersionObject.CurrentMajorVersion[0] + "." + (parseInt(oVersionObject.CurrentMinorVersion[0], 10));
                $(".OverwriteVersion").text(sOverwriteVersion + " Overwrite the current minor version");
            } else {
                $($(".CheckInOptions")[2]).hide();
                $(".OverwriteVersion").hide();
            }
            $(".MinorVersion").text(sMinorVersion + " Minor version (draft)");
            $(".MajorVersion").text(sMajorVersion + " Major version (publish)");
        } else {
            $(".VersionDetails").hide();
        }
        oBriefCase.DocumentId = arrDocumentId[0];
        oBriefCase.DocumentUrl = oVersionObject.RelativeURL[0];
        briefcaseDetails.briefcaseDetails.push(oBriefCase);
    } else {
        var iCount = 0, iMinorVersionCount = 0;
        for (iIterator = 0; iIterator < iLength; iIterator++) {
            briefcaseDetails.briefcaseDetails.push({ "DocumentId": arrDocumentId[iIterator], "DocumentUrl": oVersionObject.RelativeURL[iIterator] });
            oBriefCase.DocumentId = null;
            oBriefCase.DocumentUrl = null;
            if (oVersionObject.IsMajorVersion[iIterator] && oVersionObject.IsMinorVersion[iIterator]) {
                iCount++;
            }
            if (1 <= parseInt(oVersionObject.CurrentMinorVersion[iIterator], 10)) {
                iMinorVersionCount++;
            }
        }
        if (!(iCount === iLength)) {
            $(".VersionDetails").hide();
        } else {
            if (iMinorVersionCount !== iLength) {
                $($(".CheckInOptions")[2]).hide();
            }
        }
    }
    $(".loadingImage").hide();
    $(".Container").hide();
    $(".VersionDiv").show();
}


// Function is used to check in document
function checkInDocument() {
    "use strict";
    briefcaseDetails.versionInfo = $("input[name=confirmVersion]:checked").val();
    if (!(briefcaseDetails.versionInfo)) {
        briefcaseDetails.versionInfo = 0;
    }
    briefcaseDetails.comments = $("#CheckInComments").val();
    briefcaseDetails.retainCheckout = ($("input[name=confirm]:checked").val() === "true");
    briefcaseDetails.convenienceCopy = ($("input[name=confirmConvenience]:checked").val() === "true");
    oCommonObject.callLegalBriefcase("SendToMatter", briefcaseDetails, onCheckInSuccess, onCheckInFailure, null, null);
    displayLoading(oGlobalConstants.sCheckIn, true);
}

// Function called on Check in document success
function onCheckInSuccess(result) {
    "use strict";
    removeLoadingClasses();
    var oResponse = JSON.parse(result);
    if (oResponse.ErrorMessage) {
        displayMessage(oResponse.ErrorMessage, false);
    } else {
        var iIterator = 0, iLength = oResponse.Status.length, bDisplayErrorMessage = false, oListChunk = "";

        for (iIterator = 0; iIterator < iLength; iIterator++) {
            if (!oResponse.Status[iIterator]) {
                bDisplayErrorMessage = true;
            }
        }
        if (bDisplayErrorMessage) {
            displayNotSupported();
        } else {
            displayMessage(oGlobalConstants.sCheckInSuccess, true);
        }
    }
}

// Function called on check in document failure
function onCheckInFailure(result) {
    "use strict";
    displayMessage(oGlobalConstants.sErrorMessage, false);
}



// Function is used to retrieve parameter from the URL
function getParameterByName(name) {
    "use strict";
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

// Function is used to global parameters like Item Id, List Id and Item URL
function setGlobalParameters() {
    "use strict";
    oGlobalConstants.itemId = getParameterByName("ItemID");
    if (!oGlobalConstants.itemId) {
        oGlobalConstants.itemId = getParameterByName("SPListItemId");
    }
    oGlobalConstants.itemUrl = getParameterByName("ItemURL");
    oGlobalConstants.listId = decodeURIComponent(getParameterByName("ListId"));
    if (!oGlobalConstants.listId) {
        oGlobalConstants.listId = getParameterByName("SPListId");
    }
    oGlobalConstants.listId = oGlobalConstants.listId.replace("{", "").replace("}", "");
}

// Function is used to check type of operation requested by the user
function checkOperation() {
    "use strict";
    setGlobalParameters();
    var operation = getParameterByName("Operation");

    switch (operation) {
        case "DMSCheckIn":
            oGlobalConstants.iCurrentOperation = oGlobalConstants.iCheckInOperation;
            displayLoading(oGlobalConstants.sCheckIn, true);
            getVersion(oGlobalConstants.itemId, oGlobalConstants.listId, oGlobalConstants.iCheckInOperation);
            break;
        case "DMSCheckOut":
            oGlobalConstants.iCurrentOperation = oGlobalConstants.iCheckOutOperation;
            syncBriefcase(oGlobalConstants.itemId, oGlobalConstants.listId, oGlobalConstants.iCheckOutOperation);
            break;
        case "DetachFromDMS":
            oGlobalConstants.iCurrentOperation = oGlobalConstants.iDetachOperation;
            displayLoading(oGlobalConstants.sDetach, false);
            syncBriefcase(oGlobalConstants.itemId, oGlobalConstants.listId, oGlobalConstants.iDetachOperation);
            break;
        case "UpdateCopy":
            oGlobalConstants.iCurrentOperation = oGlobalConstants.iUpdateOperation;
            displayLoading(oGlobalConstants.sUpdateDocument, false);
            syncBriefcase(oGlobalConstants.itemId, oGlobalConstants.listId, oGlobalConstants.iUpdateOperation);
            break;
        default:
            displayMessage(oGlobalConstants.sWrongParameter, false);
            break;
    }
}


// Function is used to get response for check in functionality
function getResponse() {
    "use strict";
    var bAnswer = false;
    var sResponse = $("input[name=confirm]:checked").val();
    if ("Yes" === sResponse) {
        bAnswer = true;
    }
    return bAnswer;
}


// Function is used to display loading on the UI
function displayLoading(sMsg, bIsVersion) {
    "use strict";
    $(".BigMessage").text(oGlobalConstants.sRelaxMsg);
    $(".BigMessage").addClass("LoadingBigMessage");
    $(".Question").text("" + sMsg);
    $(".Question").addClass("LoadingQuestion");
    $(".Response").hide();
    $(".loadingImage").show();
    if (bIsVersion) {
        $(".VersionDiv").hide();
        $(".Container").show();
    } else {
        $(".ButtonContainer").hide();
    }
}

// Function to remove the classes added to div while displaying loading popup
function removeLoadingClasses() {
    "use strict";
    $(".BigMessage").removeClass("LoadingBigMessage");
    $(".Question").removeClass("LoadingQuestion");
}

// Function is used to display message on the UI
function displayMessage(sMsg, bSuccess) {
    "use strict";
    if (bSuccess) {
        if (2 === oGlobalConstants.iCurrentOperation) {
            $(".BigMessage").text(oGlobalConstants.sGoodNews);
        } else {
            $(".BigMessage").text(oGlobalConstants.sGenericSuccess);
        }
    } else {
        $(".BigMessage").text(oGlobalConstants.sGenericFailure);
    }
    $(".BigMessage").addClass("LoadingBigMessage");
    $(".Question").text("" + sMsg);
    $(".Question").addClass("LoadingQuestion");
    $(".loadingImage").hide();
    $(".ButtonContainer").hide();
}

// Function is used to make call for performing check for normal documents
function checkNormalDocuments(operationType) {
    "use strict";
    setGlobalParameters();
    oGlobalConstants.iCurrentOperation = operationType;
    var itemId = oGlobalConstants.itemId.split(",");
    var listId = oGlobalConstants.listId;
    if (itemId && listId) {
        var briefcaseDetails = { "requestObject": { "SPAppToken": "", "RefreshToken": oSharePointContext.RefreshToken }, "syncDetails": { "ListId": listId, "ItemId": itemId, "Operation": operationType } };
        oCommonObject.callLegalBriefcase("CheckNormalDocuments", briefcaseDetails, onCheckSuccess, onCheckFailure, null, null);
    }
}

// Function called on success of check normal documents function
function onCheckSuccess(result) {
    "use strict";
    removeLoadingClasses();
    var bResult = JSON.parse(result);
    if (bResult) {
        displayQuestion();
    } else {
        displayNotSupported();
    }
}


// Function called on failure of check normal documents function
function onCheckFailure(result) {
    "use strict";
    displayMessage(oGlobalConstants.sErrorMessage, false);
}


// Document ready function
$(document).ready(function () {
    "use strict";
    var operation = getParameterByName("Operation");
    if (operation) {
        var sBigMessage = "", sQuestion = "", bIsQuestion = true, bIsSubmitButton = false, bCallCheckOperation = true, bDisplayLoading = true, bIsContainer = true;
        switch (operation) {
            case "DMSCheckIn":
                bIsContainer = false;
                break;
            case "DMSCheckOut":
                sBigMessage = oGlobalConstants.sRelaxMsg;
                sQuestion = oGlobalConstants.sCheckOut;
                break;
            case "DetachFromDMS":
                displayLoading(oGlobalConstants.sCheckIn, false);
                $(".Container").show();
                checkNormalDocuments(oGlobalConstants.iDetachOperation);
                return;
            case "UpdateCopy":
                displayLoading(oGlobalConstants.sCheckIn, false);
                $(".Container").show();
                checkNormalDocuments(oGlobalConstants.iUpdateOperation);
                return;
        }
        if (bIsContainer) {
            $(".BigMessage").html("" + sBigMessage);
            $(".Question").html("" + sQuestion);
            $(".BigMessage").addClass("LoadingBigMessage");
            $(".Question").addClass("LoadingQuestion");
            if (bIsSubmitButton) {
                $(".ButtonContainer").show();
            }
            if (bDisplayLoading) {
                $(".loadingImage").show();
            }
            $(".Container").show();
            if (bCallCheckOperation) {
                checkOperation();
            }
        } else {
            $(".VersionDiv").show();
            checkOperation();
        }
    }
});

// Function is used to display question information on popup for Detach and Update functionality
function displayQuestion() {
    "use strict";
    $(".loadingImage").hide();
    if (1 === oGlobalConstants.iCurrentOperation) {
        $(".BigMessage").html("" + oGlobalConstants.sUpdateQuestion);
    } else if (3 === oGlobalConstants.iCurrentOperation) {
        $(".BigMessage").html("" + oGlobalConstants.sDetachQuestion);
    }
    $(".Question").html("" + oGlobalConstants.sQuestion);
    $(".ButtonContainer").show();
    $(".Container").show();
}


// Function is used to display not supported message
function displayNotSupported() {
    "use strict";
    $(".BigMessage").html(oGlobalConstants.sNotSupported);
    $(".Question").hide();
    $(".loadingImage").hide();
    $(".NotSupportedButton").show();
}

// Function is used to toggle display of Convenience copy option
function toggleConvenienceOption() {
    "use strict";
    "true" === $("input[name=confirm]:checked").val() ? $("#ConvenienceCopy").hide() : $("#ConvenienceCopy").show();
}

// Function is used to close popup
function closePopup() {
    "use strict";
    closeParentDialog(true);
}


// Function is used to close popup dialog
function closeParentDialog(refresh) {
    "use strict";
    var target = parent.postMessage ? parent : (parent.document.postMessage ? parent.document : "undefined");
    if (refresh) {
        target.postMessage("CloseCustomActionDialogRefresh", "*");
    } else {
        target.postMessage("CloseCustomActionDialogNoRefresh", "*");
    }
}