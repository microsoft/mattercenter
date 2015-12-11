/// <disable>JS1003,JS2005,JS2016,JS2023,JS2024,JS2026,JS2027,JS2031,JS2032,JS2052,JS2053,JS2064,JS2073,JS2074,JS2076,JS3054,JS3056,JS3057,JS3058,JS3085,JS3092,JS3116</disable>
var oPinProperties = {}, iCountVal = 0;
oGlobalConstants.sErrorMessage = "<span class='PropertyTitle DescriptionTitle'>Some Error Occurred :( </span>";
oGlobalConstants.sClientIdAndMatterIdSeperator = ".";
oGlobalConstants.sGetTitle = "_api/web/lists/getByTitle('";
oGlobalConstants.sGetProperties = "')/RootFolder/properties";
oGlobalConstants.sUrlExtension = "/Forms/AllItems.aspx";
oGlobalConstants.sUserLoginName = "";
oGlobalConstants.sOperationType = "";
oGlobalConstants.sCurrentUserTitle = "";
oGlobalConstants.MatterPinnedDetails = "";
oGlobalConstants.sOperationUnpin = "Unpin";
oGlobalConstants.sOperationPin = "Pin";
oGlobalConstants.sMatterIdAndClientIdTitle = "Client & Matter ID";
oGlobalConstants.sGoToOneNoteTitle = "Go to OneNote";
oGlobalConstants.sShareTitle = "Share";
oGlobalConstants.sPinColumn = "UserPinDetails";
oGlobalConstants.sUserAliasColumn = "UserAlias";
oGlobalConstants.sListName = "UserPinnedMatter";
oGlobalConstants.applicationInsightsId = "[[[App Insights ID]]]";
oGlobalConstants.eventName = "";
oGlobalConstants.Pin_Matter = "PinMatter";
oGlobalConstants.Unpin_Matter = "UnpinMatter";
oGlobalConstants.Share_Matter = "ShareMatter";
oGlobalConstants.Go_To_OneNote = "GoToOneNote";
oGlobalConstants.sProvisionMatterGroupName = "Provision Matter Users";
oGlobalConstants.sCatalogSiteUrl = "[[[Catalog site Url]]]";
oGlobalConstants.sEditUserLink = "Edit users";
oGlobalConstants.sAddUserLink = "Add users";
oGlobalConstants.sNoBlockedUserMessage = "No blocked users";
oGlobalConstants.sNoTeamMemberMessage = "No Team members";
oGlobalConstants.iCounter = 0;
oGlobalConstants.iCheckFullPermissionCounter = 0;
oGlobalConstants.iRetryAttemps = 10;
oGlobalConstants.iTeamMembersCount = 0;
oGlobalConstants.iBlockedUsersCount = 0;
oGlobalConstants.sBlockedUsersChunk = "";
oGlobalConstants.sUsersChunk = "";
oGlobalConstants.iPresenceCount = 100000;
oGlobalConstants.sWebDashboardUrl = "[[[Tenant Web Dashboard Url]]]";
oGlobalConstants.sNA = "NA";
oGlobalConstants.oListItem = null;
oGlobalConstants.sAppName = "ProvisionDMSMatter";
oGlobalConstants.sSendMailListName = "SendMail";
oGlobalConstants.sEffectivePermissionUrl = "/{0}/_api/Web/lists/getbytitle('{1}')/EffectiveBasePermissions";

var oGlobalModalDialog = {
    width: 770,
    height: 500,
    url: "[[[Manage Permission Url]]]",
    EditUserTitle: "Edit users",
    AddUserTitle: "Add users"
};

$(document).ready(function () {
    "use strict";
    showLoadingImage();
    var fileUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/sp.js";
    $("#IconBar").find("a").attr({ "href": oGlobalConstants.sWebDashboardUrl });
    $.getScript(fileUrl, function () {
        ExecuteOrDelayUntilScriptLoaded(function () {
            getCurrentUserTitle();
            getUserData();
            checkUserExistsInSharePointGroup();
        }, "sp.js");

    });
    if ('addEventListener' in window) {
        window.addEventListener('message', receiveMessage, true);
    } else if ('attachEvent' in window) { // IE
        window.attachEvent('onmessage', receiveMessage);
    }
    /* Check if edit links are present, if so then remove them */
    if (0 < $("#EditLinks").length) {
        $("#EditLinks").remove();
    }
});


var oPropertyNameMapping = {
    "MatterName": "Matter Name",
    "ClientName": "Client Name",
    "AreaOfLaw": "Area of Law",
    "ClientID": "Client Id",
    "MatterID": "Matter Id",
    "PracticeGroup": "Practice Group",
    "ResponsibleAttorney": "Responsible Attorney",
    "Description": "Description"
};


// Disable the pin button until the status is not retrieved for current matter
var oElement = document.getElementById("PinMatter");
if (oElement) {
    oElement.disabled = true;
}

// Function to get current user title
function getCurrentUserTitle() {
    "use strict";
    var clientContext = SP.ClientContext.get_current();
    var web = clientContext.get_web();
    oGlobalConstants.currentUser = web.get_currentUser();
    clientContext.load(oGlobalConstants.currentUser);
    clientContext.executeQueryAsync(Function.createDelegate(this, function () {
        window.oGlobalConstants.sCurrentUserTitle = window.oGlobalConstants.currentUser.get_title();
        if (!window.oGlobalConstants.sUserLoginName) {
            var alias = oGlobalConstants.currentUser.get_email();
            if (alias) {
                window.oGlobalConstants.sUserLoginName = getUserLoginName(alias);
            }
            else {
                window.oGlobalConstants.sUserLoginName = getUserLoginName(oGlobalConstants.currentUser.get_loginName());
            }
            ExecuteOrDelayUntilScriptLoaded(function () { retrieveListItems(); }, "sp.js");
        }
    }), Function.createDelegate(this, onQueryFailed("UserInformation")));
}

// Function to process XML code and get appropriate metadata properties and display those to content editor web part of metadata properties
function processXML(xml, sStyleId, iFlag, oPropertiesList) {
    "use strict";
    var oProcessedXml, iLength, oElements, iterator, iPropertiesLength, iPropertyIterator, oPinPropertiesList, sTextContent, sClientId;
    oProcessedXml = $.parseXML(xml);
    oPinPropertiesList = ["MatterName", "ClientName", "AreaOfLaw", "PracticeGroup", "SubAreaOfLaw", "ResponsibleAttorney", "TeamMembers", "IsMatter", "OpenDate", "ClientID", "MatterID", "BlockedUploadUsers", "ClientName", "BlockedUsers", "LastItemModifiedDate"];  // Properties that are required for Pinning matter
    iLength = oProcessedXml.firstChild.lastChild.firstChild.childElementCount;
    oElements = oProcessedXml.firstChild.lastChild.firstChild;
    iterator = 0;
    iPropertiesLength = oPropertiesList.length;
    iPropertyIterator = 0;
    for (iPropertyIterator = 0; iPropertyIterator < iPropertiesLength; iPropertyIterator++) {
        for (iterator = 0; iterator < iLength; iterator++) {
            if (oElements.childNodes[iterator].localName === oPropertiesList[iPropertyIterator]) {  // Comparing XML properties with the properties we required
                if (0 === iFlag) {
                    sTextContent = oElements.childNodes[iterator].textContent;
                    if ("AreaOfLaw" === oPropertiesList[iPropertyIterator] && sTextContent || "PracticeGroup" === oPropertiesList[iPropertyIterator] && sTextContent || "ResponsibleAttorney" === oPropertiesList[iPropertyIterator] && sTextContent) {
                        if (sTextContent.lastIndexOf(";") === sTextContent.length - 1) {
                            sTextContent = sTextContent.substring(0, sTextContent.length - 1);
                        } else if (sTextContent.lastIndexOf(";") === sTextContent.length - 2) {
                            sTextContent = sTextContent.substring(0, sTextContent.length - 2);
                        }
                        if ("ResponsibleAttorney" === oPropertiesList[iPropertyIterator] && sTextContent) {
                            var arrResponsibleAttorney = sTextContent.split(";");
                            arrResponsibleAttorney = $.map(arrResponsibleAttorney, function (n) { return n.trim(); });
                            sTextContent = unique(arrResponsibleAttorney).toString();
                        }

                    } else if ("OpenDate" === oPropertiesList[iPropertyIterator]) {
                        var date = oElements.childNodes[iterator].textContent;
                        var dateArray = date.slice(0, 10).split("-");
                        sTextContent = dateArray[2] + "/" + dateArray[1] + "/" + dateArray[0];
                    } else if ("ClientID" === oPropertiesList[iPropertyIterator]) {
                        sClientId = sTextContent;
                        continue;
                    } else if ("MatterID" === oPropertiesList[iPropertyIterator]) {
                        sTextContent = sClientId + oGlobalConstants.sClientIdAndMatterIdSeperator + sTextContent;
                        oPropertyNameMapping[oPropertiesList[iPropertyIterator]] = oGlobalConstants.sMatterIdAndClientIdTitle;
                    }
                    $("#" + sStyleId).append("<div class='MetaData'><div class='PropertyTitle DescriptionTitle'> " +
                        oPropertyNameMapping[oPropertiesList[iPropertyIterator]] + "</div>  <div class='PropertyContent'>" + sTextContent + "</div></div>");  // Display the metadata property on content editor web part
                }
                if (1 === iFlag) {
                    $("#MetaDataProperties+.MetaData").remove();
                    var sContent = (oElements.childNodes[iterator].textContent) ? oElements.childNodes[iterator].textContent : oGlobalConstants.sNA;
                    $("#" + sStyleId).parent().append("<div class='MetaData'><div class='PropertyTitle DescriptionTitle'> " + oPropertyNameMapping[oPropertiesList[iPropertyIterator]] + "</div>  <div class='PropertyContent'>" + sContent + "</div></div>").find("br").remove();
                    localStorage.setItem(oPropertiesList[iPropertyIterator], oElements.childNodes[iterator].textContent);
                    oPinProperties[oPropertiesList[iPropertyIterator]] = oElements.childNodes[iterator].textContent; // Add Description to pin properties list 
                }
            }
        }
    }
    // Store all the properties in variable that will be further used for pinning matter
    iPropertiesLength = oPinPropertiesList.length;
    for (iPropertyIterator = 0; iPropertyIterator < iPropertiesLength; iPropertyIterator++) {
        for (iterator = 0; iterator < iLength; iterator++) {
            if (oElements.childNodes[iterator].localName === oPinPropertiesList[iPropertyIterator]) { // Comparing XML properties with the properties we required 
                localStorage.setItem(oPinPropertiesList[iPropertyIterator], oElements.childNodes[iterator].textContent);
                oPinProperties[oPinPropertiesList[iPropertyIterator]] = oElements.childNodes[iterator].textContent;
            }
        }
    }

    iCountVal++;
    if (2 <= iCountVal) {
        if (oElement) {
            oElement.disabled = false;
        }
    }

}

// Function is used to make ajax requests
function callAJAXFunction(clientUrl, iFlag, oPropertiesList, sStyleId) {
    "use strict";
    $.ajax({
        type: "GET",
        url: clientUrl,  // URL from where the XML code of metadata properties is retrieved
        contentType: "text/xml",
        dataType: "text",
        success: function (xml) {
            processXML(xml, sStyleId, iFlag, oPropertiesList);  // Call the processXML function to process the XML code and get the properties and display it
        },
        error: function () {
            $("#" + sStyleId).append(oGlobalConstants.sErrorMessage);  // If error occurs, display error message on web part
        }
    });
}

// Function to get the data from Matter Landing page to process and display metadata properties to content editor web part
function getMetaDataProperties(sClientPath, sMatterName, sStyleId) {
    "use strict";
    var sDomainAddress = window.location.protocol + "//" + window.location.hostname;
    var iFlag, oPropertiesList;
    oPinProperties["MatterClientUrl"] = sDomainAddress + sClientPath;
    localStorage.setItem("MatterClientUrl", oPinProperties["MatterClientUrl"]);
    oPinProperties["MatterUrl"] = oPinProperties["MatterClientUrl"] + sMatterName + oGlobalConstants.sUrlExtension;
    localStorage.setItem("MatterUrl", oPinProperties["MatterUrl"]);
    $("#" + sStyleId).html("");
    iFlag = 0;
    oPropertiesList = ["MatterName", "ClientName", "ClientID", "MatterID", "PracticeGroup", "AreaOfLaw", "ResponsibleAttorney"];
    var clientUrl = sDomainAddress + sClientPath + oGlobalConstants.sGetTitle + sMatterName + oGlobalConstants.sGetProperties;
    callAJAXFunction(clientUrl, iFlag, oPropertiesList, sStyleId);

    iFlag = 1;
    oPropertiesList = ["Description"];
    clientUrl = sDomainAddress + sClientPath + oGlobalConstants.sGetTitle + sMatterName + "')";
    callAJAXFunction(clientUrl, iFlag, oPropertiesList, sStyleId);
    // Update the OneNote Link
    urlExists();
}

// Function is used to retrieve list items
function retrieveListItems() {
    "use strict";
    oGlobalConstants.clientContext = new SP.ClientContext(oGlobalConstants.sCatalogSite);
    oGlobalConstants.oList = oGlobalConstants.clientContext.get_web().get_lists().getByTitle(oGlobalConstants.sListName);
    var camlQuery = new SP.CamlQuery();
    camlQuery.set_viewXml("<View><Query><Where><Eq><FieldRef Name='" + oGlobalConstants.sUserAliasColumn + "'/> <Value Type='Text'>" + oGlobalConstants.sUserLoginName + "</Value></Eq></Where></Query><ViewFields> <FieldRef Name='" + oGlobalConstants.sPinColumn + "' /></ViewFields></View>");
    oGlobalConstants.collListItem = oGlobalConstants.oList.getItems(camlQuery);
    oGlobalConstants.clientContext.load(oGlobalConstants.collListItem);
    oGlobalConstants.clientContext.executeQueryAsync(Function.createDelegate(this, function () { onQuerySucceeded(); }), Function.createDelegate(this, onQueryFailed("RetrieveItems")));
}

// Success function to be called when all the items are retrieved from the list
function onQuerySucceeded() {
    "use strict";
    var iCount = 0, oListItem, oPinnedObject;
    var listItemEnumerator = oGlobalConstants.collListItem.getEnumerator();
    while (listItemEnumerator.moveNext()) {
        iCount++;
        oListItem = listItemEnumerator.get_current();
        oGlobalConstants.MatterPinnedDetails = oListItem.get_item(oGlobalConstants.sPinColumn);
    }
    if ("" !== oGlobalConstants.sOperationType) {
        if (0 === iCount) {
            if (oGlobalConstants.sOperationPin === oGlobalConstants.sOperationType) {
                oPinnedObject = getPinnedObject();
                createListItemAndPin(oPinnedObject);
            }
        } else {
            if (oGlobalConstants.sOperationPin === oGlobalConstants.sOperationType) {
                oPinnedObject = getPinnedObject();
                updateList(oPinnedObject);
            } else if (oGlobalConstants.sOperationUnpin === oGlobalConstants.sOperationType) {
                oPinnedObject = getPinnedObject();
                removeList(oPinnedObject);
            }
        }
    }

    if (0 < iCount) {
        checkAlreadyPinned(oGlobalConstants.MatterPinnedDetails);
    }
}

// Function is used to get Pinned object
function getPinnedObject() {
    "use strict";
    var sMatterClientUrl = localStorage.getItem("MatterClientUrl");
    sMatterClientUrl = sMatterClientUrl.substring(0, sMatterClientUrl.length - 1);
    var sPinnedObject = "\"" + localStorage.getItem("MatterUrl") + "\": {\n\t \"MatterName\": \"" +
                        localStorage.getItem("MatterName") + "\", \n\t \"MatterDescription\": \"" +
                        localStorage.getItem("Description") + "\", \n\t \"MatterCreatedDate\": \"" +
                        localStorage.getItem("OpenDate") + "\", \n\t \"MatterUrl\": \"" +
                        localStorage.getItem("MatterUrl") + "\", \n\t \"MatterPracticeGroup\": \"" +
                        localStorage.getItem("PracticeGroup") + "\", \n\t \"MatterAreaOfLaw\": \"" +
                        localStorage.getItem("AreaOfLaw") + "\", \n\t \"MatterSubAreaOfLaw\": \"" +
                        localStorage.getItem("SubAreaOfLaw") + "\", \n\t \"MatterClientUrl\": \"" +
                        sMatterClientUrl + "\", \n\t \"MatterClient\": \"" +
                        localStorage.getItem("ClientName") + "\", \n\t \"MatterClientId\": \"" +
                        localStorage.getItem("ClientID") + "\", \n\t \"MatterID\": \"" +
                        localStorage.getItem("MatterID") + "\", \n\t \"MatterModifiedDate\": \"" +
                        localStorage.getItem("LastItemModifiedDate") + "\", \n\t \"MatterResponsibleAttorney\": \"" +
                        localStorage.getItem("ResponsibleAttorney") + "\", \n\t \"HideUpload\": \"";
    var sHideUpload = "true";
    var sBlockedUploadUsers = "";
    if (localStorage.getItem("BlockedUploadUsers") && localStorage.getItem("BlockedUploadUsers").trim()) {
        sBlockedUploadUsers = localStorage.getItem("BlockedUploadUsers").toString();
    }
    if ("" !== sBlockedUploadUsers) {
        if (-1 !== sBlockedUploadUsers.indexOf(oGlobalConstants.sCurrentUserTitle)) {
            sHideUpload = "false";
        }
    } else {
        sHideUpload = "false";
    }
    sPinnedObject += sHideUpload + "\" \n }";
    return sPinnedObject;
}

// Function is used to check whether the matter is already pinned or not
function checkAlreadyPinned(sMatterDetails) {
    "use strict";
    var pinnedMattersList = decodeURIComponent(sMatterDetails.toLowerCase());
    var matterUrl = decodeURIComponent(localStorage.getItem("MatterUrl").toLowerCase());
    var iPresent = pinnedMattersList.search(matterUrl);
    if (oElement) {
        if (0 > iPresent) {
            setTextForPinOperation(oGlobalConstants.sOperationPin);
        } else {
            setTextForPinOperation(oGlobalConstants.sOperationUnpin);
        }
    }
}

// Function called on click of Pin
function pinMatter() {
    "use strict";
    oGlobalConstants.sOperationType = oGlobalConstants.sOperationPin;
    retrieveListItems();
}

// Function called on click of Un pin
function unPinMatter() {
    "use strict";
    oGlobalConstants.sOperationType = oGlobalConstants.sOperationUnpin;
    retrieveListItems();
}

/* Function to execute on success of breaking the permission of list item */
function onBreakPermissionSuccess(objListItem) {
    "use strict";
    if (oElement) {
        setTextForPinOperation(oGlobalConstants.sOperationUnpin);
    }
};

// Function to be called request failure
function onQueryFailed(sOperationType) {
    "use strict";
    if ("UserInformation" === sOperationType) {
        $("#MetaDataProperties").html = oGlobalConstants.sErrorMessage;
    }
}

// Function to create List Item And Pin
function createListItemAndPin(oPinnedObject) {
    "use strict";
    var clientContext = new SP.ClientContext(oGlobalConstants.sCatalogSite);
    var oList = clientContext.get_web().get_lists().getByTitle(oGlobalConstants.sListName);
    var itemCreateInfo = new SP.ListItemCreationInformation();
    oGlobalConstants.oListItem = oList.addItem(itemCreateInfo);
    var oPinnedBody = "{" + oPinnedObject + "}";
    oGlobalConstants.oListItem.set_item(oGlobalConstants.sUserAliasColumn, oGlobalConstants.sUserLoginName);
    oGlobalConstants.oListItem.set_item(oGlobalConstants.sPinColumn, oPinnedBody);
    oGlobalConstants.oListItem.update();
    clientContext.load(oGlobalConstants.oListItem);
    clientContext.executeQueryAsync(Function.createDelegate(this, function () { onListItemCreated(oGlobalConstants.oListItem, clientContext); }), Function.createDelegate(this, function () { onQueryFailed(oGlobalConstants.sOperationPin); }));
}

// Function called on list item created
function onListItemCreated(oListItem, clientContext) {
    "use strict";
    if (!oListItem && !clientContext) {
        if (oElement) {
            setTextForPinOperation(oGlobalConstants.sOperationUnpin);
        }
    } else {
        var objListItem = oListItem;
        var collRoleDefinitionBinding = SP.RoleDefinitionBindingCollection.newObject(clientContext);
        collRoleDefinitionBinding.add(clientContext.get_web().get_roleDefinitions().getByType(SP.RoleType.reader));
        var oUser = clientContext.get_web().ensureUser(_spPageContextInfo.userLoginName);
        objListItem.breakRoleInheritance(false);
        objListItem.get_roleAssignments().add(oUser, collRoleDefinitionBinding);
        objListItem.update();
        clientContext.load(oUser);
        clientContext.load(objListItem);
        clientContext.executeQueryAsync(function () { onBreakPermissionSuccess(objListItem); }, function () { /* Do nothing */ });
    }
}

// Update the value of pinned object inside the list for the current user
function updateList(oPinnedObject) {
    "use strict";
    var listItemEnumerator = oGlobalConstants.collListItem.getEnumerator(), oListItem;
    while (listItemEnumerator.moveNext()) {
        oListItem = listItemEnumerator.get_current();
        oGlobalConstants.MatterPinnedDetails = oListItem.get_item(oGlobalConstants.sPinColumn);
    }
    var pinnedMattersList = decodeURIComponent(oGlobalConstants.MatterPinnedDetails.toLowerCase());
    var matterUrl = decodeURIComponent(localStorage.getItem("MatterUrl").toLowerCase());
    var iPresent = pinnedMattersList.search(matterUrl);
    if (0 <= iPresent) {
        setTextForPinOperation(oGlobalConstants.sOperationUnpin);
    } else {
        var valueIndex = oGlobalConstants.MatterPinnedDetails.lastIndexOf("}");
        oGlobalConstants.MatterPinnedDetails = oGlobalConstants.MatterPinnedDetails.substring(0, valueIndex);
        oGlobalConstants.MatterPinnedDetails += ", \n" + oPinnedObject + "\n }";
        oListItem.set_item(oGlobalConstants.sPinColumn, oGlobalConstants.MatterPinnedDetails);
        if (oElement) {
            setTextForPinOperation(oGlobalConstants.sOperationUnpin);
        }
        oListItem.update();
        oGlobalConstants.clientContext.executeQueryAsync(Function.createDelegate(this, function () { onListItemCreated(null, null); }), Function.createDelegate(this, function () { onQueryFailed(oGlobalConstants.sOperationPin); }));
    }
}

// Function to update the list item after removing the pinned matter
function removeList(oPinnedObject) {
    "use strict";
    var listItemEnumerator = oGlobalConstants.collListItem.getEnumerator(), oListItem;
    while (listItemEnumerator.moveNext()) {
        oListItem = listItemEnumerator.get_current();
        oGlobalConstants.MatterPinnedDetails = oListItem.get_item(oGlobalConstants.sPinColumn);
    }

    var deletion = JSON.parse(oGlobalConstants.MatterPinnedDetails);
    var matterUrl = decodeURIComponent(localStorage.getItem("MatterUrl").toLowerCase());
    $.each(deletion, function (key, val) {
        if (decodeURIComponent(key.toLowerCase()) === matterUrl) {
            delete deletion[key];
            return false;
        }
    });
    var newMatterPinnedDetails = JSON.stringify(deletion);
    oListItem.set_item(oGlobalConstants.sPinColumn, newMatterPinnedDetails);
    oListItem.update();
    if (oElement) {
        setTextForPinOperation(oGlobalConstants.sOperationPin);
    }
    var iCountValue = 0;
    for (var key in deletion) {
        if (deletion.hasOwnProperty(key)) {
            iCountValue++;
        }
    }
    if (0 === iCountValue) {
        oListItem.deleteObject();
    }
    oGlobalConstants.clientContext.executeQueryAsync(Function.createDelegate(this, function () { onListItemDeleted(); }), Function.createDelegate(this, function () { onQueryFailed(oGlobalConstants.sOperationUnpin); }));
}

// Function to be called on list item deleted or pinned matter removed
function onListItemDeleted() {
    "use strict";
    if (oElement) {
        setTextForPinOperation(oGlobalConstants.sOperationPin);
    }
}

// Function to be called on click of Div
function pinUnpinMatter() {
    "use strict";
    if (oElement) {
        if (oGlobalConstants.sOperationPin === $(oElement).text()) {
            oGlobalConstants.eventName = oGlobalConstants.Pin_Matter;
            LogEvent();
            pinMatter();
        } else {
            oGlobalConstants.eventName = oGlobalConstants.Unpin_Matter;
            LogEvent();
            unPinMatter();
        }
    }
}

// Function is used to Show Share Matter Popup
function showPopup(sListID, sMatterName) {
    "use strict";
    oGlobalConstants.eventName = oGlobalConstants.Share_Matter;
    LogEvent();
    var sSiteCollectionURL = window.location.protocol + "//" + window.location.host + _spPageContextInfo.siteServerRelativeUrl;
    var sPermissionPopupURL = sSiteCollectionURL + "/_layouts/15/aclinv.aspx?List={" + sListID + "}&IsDlg=1";
    var sTitle = oGlobalConstants.sShareTitle + " " + sMatterName;
    var options = { title: sTitle, width: 800, height: 300, url: sPermissionPopupURL };
    // Show popup
    SP.UI.ModalDialog.showModalDialog(options);
}


$("#assign").click(function () {
    "use strict";
    $("#block").addClass("unselected").removeClass("selected");
    $("#assign").addClass("selected").removeClass("unselected");
    $("#blockUserDetails").hide();
    $("#userDetails").show();
});

$("#block").click(function () {
    "use strict";
    $("#assign").addClass("unselected").removeClass("selected");
    $("#block").addClass("selected").removeClass("unselected");
    $("#blockUserDetails").show();
    $("#userDetails").hide();
});

// Function is used to set the width of the web parts on the page
function setWidth() {
    "use strict";
    var oWebPartId = [
    "OuterLeftCell",
    "OuterRightCell",
    "MiddleLeftCell",
    "MiddleMiddleCell"
    ];

    var oWebPartWidth = [
        "60%",
        "40%",
        "1%",
        "99%"
    ];

    var iterator, iLength = oWebPartId.length, oWebPartElements;
    for (iterator = 0 ; iterator < iLength; iterator++) {
        oWebPartElements = document.getElementById(oWebPartId[iterator]);
        if (oWebPartElements) {
            oWebPartElements.width = oWebPartWidth[iterator];
        }
    }
}

// Call function to set width of the web parts
setWidth();


// Set title for the operations
$("#GoToOneNote").parent()[0].title = oGlobalConstants.sGoToOneNoteTitle;
$("#Share")[0].title = oGlobalConstants.sShareTitle;

// Function is used to set text for Pin and unpin operation
function setTextForPinOperation(sOperationType) {
    "use strict";
    var oImgElements = $("#PinMatter img");
    var arrImgSrc = [];
    var iterator = 0, iCount = oImgElements.length;
    for (iterator = 0; iterator < iCount; iterator++) {
        arrImgSrc.push(oImgElements[iterator].src);
    }
    $("#PinMatter").attr("title", sOperationType);
    var sHtml = "<img src='" + arrImgSrc[0] + "' alt='Pin' id = 'pinImg'><img src='" + arrImgSrc[1] + "' id = 'unPinImg' alt='Unpin'>" + sOperationType;
    $("#PinMatter").html(sHtml);
    if (sOperationType === oGlobalConstants.sOperationPin) {
        $("#pinImg").css("display", "inline");
        $("#unPinImg").css("display", "none");
    } else {
        $("#pinImg").css("display", "none");
        $("#unPinImg").css("display", "inline");
    }
}

// Function is used to retrieve user login name from the email address
function getUserLoginName(sEmailId) {
    "use strict";
    var sUserPinName = "";
    if (sEmailId) {
        if (sEmailId.lastIndexOf("@") == -1) { // User is on premise
            sUserPinName = sEmailId.substring(sEmailId.lastIndexOf("\\") + 1, sEmailId.length);
        }
        else {
            sUserPinName = sEmailId.substring(sEmailId.lastIndexOf("|") + 1, sEmailId.lastIndexOf("@"));
        }
    }
    return sUserPinName;
}

// Function to check whether the OneNote exists
function urlExists() {
    "use strict";
    // Get the link from the data-href attribute
    var oOneNoteLink = $("#GoToOneNote"), oOneNoteIcon = $("#GoToOneNote img"), arrOneNoteURL;
    if (oOneNoteLink[0] && oOneNoteIcon[0]) {
        var sOneNoteURL = oOneNoteLink.attr("data-href");
        if (sOneNoteURL && "" !== sOneNoteURL) {
            arrOneNoteURL = sOneNoteURL.split("sourcedoc=");
            if (arrOneNoteURL.length && arrOneNoteURL[1]) {
                // AJAX Call to check if the item exists
                $.ajax({
                    url: arrOneNoteURL[1],
                    error: function () {
                        // Failure - Hide the link
                        oOneNoteLink.hide();
                    },
                    success: function () {
                        // Success - Show the link and update the href
                        oGlobalConstants.eventName = oGlobalConstants.Go_To_OneNote;
                        $("#OneNote").click(function () {
                            LogEvent();
                        });
                        $("#GoToOneNote").attr({ "href": oOneNoteLink.attr("data-href") });
                        $("#GoToOneNote img").attr({ "src": oOneNoteIcon.attr("src").replace("WindowsLoadingFast.GIF", "One-Note-27X27-666.png"), "alt": oGlobalConstants.sGoToOneNoteTitle });
                        $("#GoToOneNote").get(0).lastChild.nodeValue = oGlobalConstants.sGoToOneNoteTitle;
                    },
                });
            } else {
                // Hide the icon
                oOneNoteLink.hide();
            }
        } else {
            // Hide the icon
            oOneNoteLink.hide();
        }
    }
}

// Function to log application insight event
function LogEvent() {
    "use strict";
    // Suppressing warning since the following code is in minified form and fetched from Azure portal
    /// <disable>JS3047,JS2017</disable>
    var appInsights = window.appInsights || function (config) {
        function r(config) { t[config] = function () { var i = arguments; t.queue.push(function () { t[config].apply(t, i) }) } } var t = { config: config }, u = document, e = window, o = "script", s = u.createElement(o), i, f; for (s.src = config.url || "//az416426.vo.msecnd.net/scripts/a/ai.0.js", u.getElementsByTagName(o)[0].parentNode.appendChild(s), t.cookie = u.cookie, t.queue = [], i = ["Event", "Exception", "Metric", "PageView", "Trace"]; i.length;) r("track" + i.pop()); return r("setAuthenticatedUserContext"), r("clearAuthenticatedUserContext"), config.disableExceptionTracking || (i = "onerror", r("_" + i), f = e[i], e[i] = function (config, r, u, e, o) { var s = f && f(config, r, u, e, o); return s !== !0 && t["_" + i](config, r, u, e, o), s }), t
    }({
        instrumentationKey: oCommonLinks.applicationInsightsId
    });
    window.appInsights = appInsights;
    appInsights.trackPageView();
    appInsights.trackEvent(oGlobalConstants.eventName);
}

moveElement();

// Function is used to move element to top most div
function moveElement() {
    "use strict";
    var oFooterElement = $("#mainDivContainer");
    $(oFooterElement).hide();
    $("#contentRow").append(oFooterElement);
    $("#contentRow #mainDivContainer").show();
}

// Function is used to get and assign user email ID to lync sip presence
function getUserEmail(clientContext, loginName, isBlockedUser) {
    "use strict";
    var oUser = clientContext.get_web().ensureUser(loginName);
    clientContext.load(oUser);
    clientContext.executeQueryAsync(function () {
        if (isBlockedUser) {
            oGlobalConstants.sBlockedUsersChunk += "<span class=\"ms-verticalAlignTop ms-noWrap\" style=\"display: block\"><span class=\"ms-imnSpan\"><a class=\"ms-imnlink ms-spimn-presenceLink\" onclick='WriteDocEngagementLog(\"DocModifiedByPresenceClick\", \"ODModifiedByPresenceClick\"); IMNImageOnClick(event);return false;' href=\"#\"><span class=\"ms-spimn-presenceWrapper ms-imnImg ms-spimn-imgSize-10x10\"><img name=\"imnmark\" title=\"\" class=\"ms-spimn-img ms-spimn-presence-disconnected-10x10x32\" id=\"imn_" + (oGlobalConstants.iPresenceCount++) + ",type=sip\" alt=\"No presence information\" src=\"/_layouts/15/images/spimn.png\" sip=\"" + oUser.get_email() + "\" showofflinepawn=\"1\"></span></a>" + loginName + "</span>";
            oGlobalConstants.iBlockedUsersCount--;
            if (!oGlobalConstants.iBlockedUsersCount) {
                $("#blockUserDetails").empty().append(oGlobalConstants.sBlockedUsersChunk);
            }
        } else {
            oGlobalConstants.sUsersChunk += "<span class=\"ms-verticalAlignTop ms-noWrap\" style=\"display: block\"><span class=\"ms-imnSpan\"><a class=\"ms-imnlink ms-spimn-presenceLink\" onclick='WriteDocEngagementLog(\"DocModifiedByPresenceClick\", \"ODModifiedByPresenceClick\"); IMNImageOnClick(event);return false;' href=\"#\"><span class=\"ms-spimn-presenceWrapper ms-imnImg ms-spimn-imgSize-10x10\"><img name=\"imnmark\" title=\"\" class=\"ms-spimn-img ms-spimn-presence-disconnected-10x10x32\" id=\"imn_" + (oGlobalConstants.iPresenceCount++) + ",type=sip\" alt=\"No presence information\" src=\"/_layouts/15/images/spimn.png\" sip=\"" + oUser.get_email() + "\" showofflinepawn=\"1\"></span></a>" + loginName + "</span>";
            oGlobalConstants.iTeamMembersCount--;
            if (!oGlobalConstants.iTeamMembersCount) {
                $("#userDetails").empty().append(oGlobalConstants.sUsersChunk);
            }
        }

        if (!oGlobalConstants.iTeamMembersCount && !oGlobalConstants.iBlockedUsersCount) {
            $("#blockUserDetails").hide();
            $("#userDetails, #blockUserDetails").css("height", $("#userDetails").height());
        }
        ProcessImn();
    },
    function (sender, args) {
    });

}

// Function is used to get user data 
function getUserData() {
    "use strict";
    if (!oPinProperties.TeamMembers && oGlobalConstants.iRetryAttemps > oGlobalConstants.iCounter) {
        oGlobalConstants.iCounter++;
        setTimeout(function () { getUserData(); }, 1000);
    }
    else {
        var clientContext = SP.ClientContext.get_current();
        var arrBlockUserName, arrUserName;
        if (oPinProperties.TeamMembers || oPinProperties.ResponsibleAttorney) {
            arrUserName = $.merge(trimEndChar(oPinProperties.TeamMembers.trim(), ";").split(';'), trimEndChar(oPinProperties.ResponsibleAttorney.trim(), ";").split(';'));
            arrUserName = unique($.map(arrUserName, function (item) { return item.trim(); }));
        }
        if (oPinProperties.BlockedUser || blockUser) {
            arrBlockUserName = oPinProperties.BlockedUser ? trimEndChar(oPinProperties.BlockedUser.trim(), ";").split(';') : trimEndChar(blockUser.trim(), ";").split(';');
            arrBlockUserName = unique($.map(arrBlockUserName, function (item) { return item.trim(); }));
        }
        if (arrUserName) {
            oGlobalConstants.iTeamMembersCount = arrUserName.length;
            $.each(arrUserName, function (key, value) {
                if (value) {
                    getUserEmail(clientContext, value, false);
                } else {
                    oGlobalConstants.iTeamMembersCount--;
                }
            });
        }
        if (arrBlockUserName) {
            oGlobalConstants.iBlockedUsersCount = arrBlockUserName.length;
            $.each(arrBlockUserName, function (key, value) {
                if (value) {
                    getUserEmail(clientContext, value, true);
                } else {
                    oGlobalConstants.iBlockedUsersCount--;
                }
            });
        }

        if (!oGlobalConstants.iTeamMembersCount) {
            $("#userDetails").empty().append("<div class='noUsers'>" + oGlobalConstants.sNoTeamMemberMessage + "</div>");
        }

        if (!oGlobalConstants.iBlockedUsersCount) {
            $("#blockUserDetails").empty().append("<div class='noUsers'>" + oGlobalConstants.sNoBlockedUserMessage + "</div>");
        }
    }
}

// Function to remove duplicate values from array 
function unique(list) {
    "use strict";
    var result = [];
    $.each(list, function (iIndex, item) {
        if ($.inArray(item, result) == -1) {
            result.push(item);
        }
    });
    return result;
}


// Function to trim trailing special character if present
function trimEndChar(sOrignalString, sCharToTrim) {
    "use strict";
    if (sCharToTrim === sOrignalString.substr(-1)) {
        return sOrignalString.substr(0, sOrignalString.length - 1);
    }
    return sOrignalString;
}


// Function to check if user is exist of SharePoint Group
function checkUserExistsInSharePointGroup() {
    "use strict";    
    var sUrl = oGlobalConstants.sCatalogSiteUrl + oGlobalConstants.sEffectivePermissionUrl.replace("{0}", oGlobalConstants.sAppName).replace("{1}", oGlobalConstants.sSendMailListName);
    $.ajax({
        url: sUrl,
        type: "GET",
        dataType: "json",
        headers: {
            Accept: "application/json;odata=verbose"
        },
        success: function (oData) {            
            onPermissionSuccess(oData);
        },
        error: function () {            
            onFailure();
        }
    });
}

// Function to check logged in user has permission on list
function onPermissionSuccess(oData) {
    "use strict";
    if (oData) {
        var permission = new SP.BasePermissions();
        permission.initPropertiesFromJson(oData.d.EffectiveBasePermissions);
        var bHasPermission = permission.has(SP.PermissionKind.editListItems);
        if (bHasPermission) {
            checkIfUserHasFullPermissions();
        }
    }
}

// Function for when group and user is load failed
function onFailure() {
    "use strict";
    return false; // Kept for future purpose
}

// Function to create modal dialog box which will show Edit/Add user page in Iframe 
function createModalDialog(isEdit) {
    "use strict";
    var title, clientUrl, matterName;
    if (oPinProperties.MatterClientUrl && oPinProperties.MatterName) {
        clientUrl = trimEndChar(oPinProperties.MatterClientUrl, "/");
        matterName = oPinProperties.MatterName;
        if (isEdit) {
            title = oGlobalModalDialog.EditUserTitle;
        }
        else {
            title = oGlobalModalDialog.AddUserTitle;
        }
        var querystring = "&IsEdit=" + isEdit + "&" + "clientUrl=" + clientUrl + "&" + "matterName=" + matterName;
        querystring = encodeURIComponent(querystring);
        var options = SP.UI.$create_DialogOptions();
        options.title = title;
        options.url = oGlobalModalDialog.url + "?" + querystring;
        options.width = oGlobalModalDialog.width;
        options.height = oGlobalModalDialog.height;
        if ('addEventListener' in window) { // all browsers except IE before version 9
            window.removeEventListener('message', receiveMessage, true);
            window.addEventListener('message', receiveMessage, true);
        } else if ('attachEvent' in window) { // IE before version 9
            window.detachEvent('onmessage', receiveMessage);
            window.attachEvent('onmessage', receiveMessage);
        }
        SP.UI.ModalDialog.showModalDialog(options);
    }
}

// Function to show loading image
function showLoadingImage() {
    "use strict";
    $("#EditLinks").remove();
    $("#userDetails,#blockUserDetails").empty().append("<div class='loadingImage'><img src='" + oGlobalConstants.sCatalogSiteUrl + "/SiteAssets/Matter%20Center%20Assets/Matter%20Landing%20Assets/WindowsLoadingFast.GIF' alt='loading image'/></div>");
}

// Function to register event from Cross domain
function receiveMessage(event) {
    "use strict";
    var iValidIframeUrl = $("iframe[class=ms-dlgFrame]").attr("src"); // URL of Edit Matter page in i-Frame
    var anchorElement = document.createElement('a');                      // Temporary element to get the host name
    iValidIframeUrl = (iValidIframeUrl) ? iValidIframeUrl.split("?")[0] : "";
    anchorElement.href = iValidIframeUrl;
    var iValidIframeHost = anchorElement.protocol + '//' + anchorElement.hostname;

    if (event.origin !== iValidIframeHost) {
        return;
    }
    if (event.data) {
        if ("true" === event.data) {
            RefreshOnDialogClose();
        }
    }
}

function checkIfUserHasFullPermissions() {
    "use strict";
    if ((!oPinProperties.MatterClientUrl || !oPinProperties.MatterName) && oGlobalConstants.iRetryAttemps > oGlobalConstants.iCheckFullPermissionCounter) {
        oGlobalConstants.iCheckFullPermissionCounter++;
        setTimeout(function () { checkIfUserHasFullPermissions(); }, 1000);
    } else {
        $.ajax({
            type: "GET",
            url: oPinProperties.MatterClientUrl + "/_api/web/lists/getbytitle('" + oPinProperties.MatterName + "')/RoleAssignments/GetByPrincipalId('" + _spPageContextInfo.userId + "')/RoleDefinitionBindings?$select=Name&$filter=Name eq 'Full Control'",  // URL from where the XML code of metadata properties is retrieved
            contentType: "text/xml",
            dataType: "text",
            success: function (xml) {
                processUserXML(xml);  // Call the processXML function to process the XML code and get the properties and display it
            },
            error: function () {
                return false;
            }
        });
    }
}


function processUserXML(xml) {
    "use strict";
    var oProcessedXml, iLength, oElements, iterator, iPropertiesLength, iPropertyIterator, oPinPropertiesList, sTextContent, sClientId;
    var oProcessedXml = $.parseXML(xml);
    oPinPropertiesList = ["Name"];
    if (oProcessedXml.firstChild.lastChild.lastChild.firstChild && oProcessedXml.firstChild.lastChild.lastChild.firstChild.childElementCount) {
        iLength = oProcessedXml.firstChild.lastChild.lastChild.firstChild.childElementCount;
        oElements = oProcessedXml.firstChild.lastChild.lastChild.firstChild;
        iterator = 0;
        iPropertiesLength = oPinPropertiesList.length;
        iPropertyIterator = 0;
        for (iPropertyIterator = 0; iPropertyIterator < iPropertiesLength; iPropertyIterator++) {
            for (iterator = 0; iterator < iLength; iterator++) {
                if (oElements.childNodes[iterator].localName === oPinPropertiesList[iPropertyIterator]) {
                    if (oElements.childNodes[iterator].textContent === 'Full Control') {
                        $(".allUsers").append('<div id= "EditLinks"> <a href= "JavaScript:createModalDialog(false)">' + oGlobalConstants.sAddUserLink + '</a> <a href= "JavaScript:createModalDialog(true)">' + oGlobalConstants.sEditUserLink + '</a></div>');
                    }
                }
            }
        }
    }
}
