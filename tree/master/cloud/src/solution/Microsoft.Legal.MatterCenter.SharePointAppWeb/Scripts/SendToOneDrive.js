/// <disable>JS2074,JS3116,JS3092,JS2024,JS2076,JS3058,JS2032,JS2064,JS3056</disable>
var appInsights = "";

var oGlobalConstants = {
    sRelaxMsg: "Sit back and Relax!",
    sSendToOneDriveMsg: "We are sending document(s) to OneDrive",
    sOverWriteMsg: "<b>@@FileName</b> already exists in your OneDrive. <br> Do you want to overwrite it?",
    sGenericSuccessMsg: "Success!",
    sGenericErrorMsg: "Failure!",
    sSuccessMsg: "Documents sent to OneDrive!",
    sFailureMsg: "Some Error Occurred",
    sCheckOutFailMsg: "Error occurred while checking out the document",
    sInvalidFilesMsg: "Please select documents only",
    bOverwrite: false,
    arrOverwriteFiles: [],
    iCurrent: 0,
    bCheckedOut: false
};

// Function is used to read parameter from the URL
function getParameterByName(name) {
    "use strict";
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}


// Function is used to make call to service for retrieving the URL of the selected items
function callService(listId, listItemId, currentURL) {
    "use strict";
    var requestObject = { "requestObject": { "SPAppToken": "", "RefreshToken": oSharePointContext.RefreshToken }, "listId": listId, "listItemId": listItemId, "currentLocation": currentURL };
    oCommonObject.callLegalBriefcase("GetFileRelativeLocation", requestObject, onRelativeLocationSuccess, onFailure, null, null);
}

function onRelativeLocationSuccess(msg) {
    "use strict";
    var relativeLocations = msg;
    oGlobalConstants.bCheckedOut = false;
    if ("false" !== relativeLocations && relativeLocations) {
        var requestData = formulateRequest(relativeLocations, 0);
        oCommonObject.callLegalBriefcase("CheckedOutDocumentByUser", requestData, onCheckedOutDataSuccess, DiscardFailure, null, null);
        callSendToOneDrive(requestData, onSendToOneDriveSuccess);
    } else {
        displayErrorMessage(msg);
    }
}

// Success function to be called when ajax request successfully executed
function onCheckedOutDataSuccess(msg) {
    "use strict";
    if (msg && "" !== msg[0]) {
        oGlobalConstants.bCheckedOut = true;
    }
}

// Failure function to be called when ajax request is not executed
function onFailure(msg) {
    "use strict";
    displayErrorMessage("Error");
}

// Document ready function
$(document).ready(function () {
    "use strict";
    displayLoading();
    var listItemId = [getParameterByName("SPListItemId")];
    var listId = getParameterByName("SPListId");
    var currentURL = getParentUrl();
    currentURL = currentURL.substring(0, currentURL.indexOf("/_layouts"));
    if ("" === listId.trim()) {
        var currentListItemId = listItemId[0];
        listItemId[0] = listItemId[0] && listItemId[0].split("$|$") && listItemId[0].split("$|$")[0];
        listId = currentListItemId && currentListItemId.split("$|$") && currentListItemId.split("$|$")[1];
        currentURL = currentListItemId && currentListItemId.split("$|$") && currentListItemId.split("$|$")[2];
    }
    listId = listId.replace("{", "").replace("}", "");
    callService(listId, listItemId, currentURL);
});

// Function is used to get URL of the parent window
function getParentUrl() {
    "use strict";
    var isInIframe = (parent !== window),
        parentUrl = null;

    if (isInIframe) {
        parentUrl = document.referrer;
    }
    return parentUrl;
}

// Function is used to display error message 
function displayErrorMessage(sMsg) {
    "use strict";
    $(".BigMessage").html(oGlobalConstants.sGenericErrorMsg);
    if ("false" === sMsg) {
        $(".Question").html(oGlobalConstants.sInvalidFilesMsg);
    } else {
        $(".Question").html(oGlobalConstants.sFailureMsg);
    }
    $(".LoadingImage").hide();
}

// Function is used to formulate response for the service
function formulateRequest(sServiceResponse, iOverWrite) {
    "use strict";
    var requestObject = {
        "requestObject": { "SPAppToken": "", "RefreshToken": oSharePointContext.RefreshToken }, "mailAttachmentDetails": { "FullUrl": sServiceResponse, "IsOverwrite": iOverWrite }, "doCheckOut": true
    };
    return requestObject;
}

// Function is used to call send to One Drive
function callSendToOneDrive(requestObject, successFunction) {
    "use strict";
    oCommonObject.callLegalBriefcase("SendToBriefcase", requestObject, successFunction, onFailure, null, null);
}

function onSendToOneDriveSuccess(msg) {
    "use strict";
    var sResponse = JSON.parse(msg);
    if (0 !== parseInt(sResponse.code, 10)) {
        displayErrorMessage("Error");
    } else {
        var sValue = sResponse.value, arrOverwriteFiles = [];
        oGlobalConstants.arrOverwriteFiles = checkOverwriteStatus(sValue);
        if (0 !== oGlobalConstants.arrOverwriteFiles.length) {
            processOverwriteFiles();
        }
    }
}

// Function to check whether all the requests are executed 
function checkOverwriteStatus(sValue) {
    "use strict";
    if (sValue) {
        var arrValues = sValue.split(";");
        if (2 < arrValues.length) {
            if (arrValues[1] === arrValues[2]) {
                displaySuccessMessage();
                return [];
            } else {
                var iterator = 3, iCount = arrValues.length, arrOverwriteFiles = [];
                for (iterator = 3; iterator < iCount; iterator++) {
                    arrOverwriteFiles.push(arrValues[iterator]);
                }
                return arrOverwriteFiles;
            }
        }
        return [];
    } else {
        return [];
    }
}

// Function is used to display success message
function displaySuccessMessage() {
    "use strict";
    $(".BigMessage").html(oGlobalConstants.sGenericSuccessMsg).show();
    $(".LoadingImage").hide();
    $(".Question").html(oGlobalConstants.sSuccessMsg).show();
    $(".ButtonContainer").hide();
}

// Function is used to display loading message on the popup for Send to One Drive
function displayLoading() {
    "use strict";
    $(".BigMessage").text(oGlobalConstants.sRelaxMsg);
    $(".BigMessage").addClass("LoadingBigMessage").show();
    $(".Question").text(oGlobalConstants.sSendToOneDriveMsg);
    $(".Question").addClass("LoadingQuestion");
    $(".LoadingImage").show();
    $(".ButtonContainer").hide();
    $(".Container").show();
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

// Function is used to process over written files
function processOverwriteFiles() {
    "use strict";
    displayOverwriteMessage();
}

// Function is used to display over write message
function displayOverwriteMessage() {
    "use strict";
    $(".LoadingImage").hide();
    var sFileName = getFileName(oGlobalConstants.iCurrent);
    $(".Question").html(oGlobalConstants.sOverWriteMsg.replace("@@FileName", sFileName));
    $(".ButtonContainer").show();
    $(".ButtonContainer").addClass("AdditionalMargin");
    $(".BigMessage").hide();
}

// Function is used to return file name from the URL
function getFileName(iCurrent) {
    "use strict";
    var sFileName = oGlobalConstants.arrOverwriteFiles[iCurrent];
    sFileName = sFileName.substring(sFileName.lastIndexOf("/") + 1, sFileName.length);
    return sFileName;
}

// Function to be called on click of over write button
function overWriteFiles(bChoice) {
    "use strict";
    if (bChoice && (oGlobalConstants.arrOverwriteFiles.length - 1) > oGlobalConstants.iCurrent) {
        displayLoading();
        callSendToOneDrive(formulateRequest(oGlobalConstants.arrOverwriteFiles[oGlobalConstants.iCurrent], 1), checkOverwriteComplete);
    } else {
        oGlobalConstants.iCurrent++;
        if ((oGlobalConstants.arrOverwriteFiles.length - 1) > oGlobalConstants.iCurrent) {
            processOverwriteFiles();
        } else {
            if (!oGlobalConstants.bCheckedOut) {
                oCommonObject.callLegalBriefcase("DiscardCheckOutChanges", formulateRequest(oGlobalConstants.arrOverwriteFiles[0], 0), DiscardSuccess, DiscardFailure, null, null);
            } else {
                var target = parent.postMessage ? parent : (parent.document.postMessage ? parent.document : "undefined");
                target.postMessage("CloseOneDriveActionDialog", "*");
                closePopup();
            }
        }
    }
}

// Function to handle success scenario of successful checkout document
function DiscardSuccess(msg) {
    "use strict";
    var target = parent.postMessage ? parent : (parent.document.postMessage ? parent.document : "undefined");
    target.postMessage("CloseOneDriveActionDialog", "*");
    closePopup();
}

// Function to handle failure  scenario of successful checkout document
function DiscardFailure(msg) {
    "use strict";
    $(".ButtonContainer").hide();
    $(".Question").hide();
    $(".BigMessage").show().html(oGlobalConstants.sGenericErrorMsg);
    $(".Question").show().html(oGlobalConstants.sCheckOutFailMsg);
    $(".LoadingImage").hide();
}

// Function to be called when over write of the document is complete
function checkOverwriteComplete(msg) {
    "use strict";
    var oResponse = JSON.parse(msg);
    if (0 === parseInt(oResponse.code, 10)) {
        oGlobalConstants.iCurrent++;
        if ((oGlobalConstants.arrOverwriteFiles.length - 1) > oGlobalConstants.iCurrent) {
            processOverwriteFiles();
        } else {
            displaySuccessMessage();
        }
    } else {
        displayErrorMessage("Error");
    }
}

