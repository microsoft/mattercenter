/// <disable>JS1003,JS2032,JS2074,JS2076,JS2024,JS3116,JS3085,JS3058,JS3092,JS3057,JS3054,JS2073,JS2005,JS3056</disable>

var oDocumentConstants = {
    bAttachDocumentFailed: false,
    iAsyncCallsCompleted: 0
};

//// Office.js Initialization 
(function () {
    "use strict";
    //// The Office initialize function must be run each time a new page is loaded 
    if (oCommonObject.getParameterByName("appType") === oGlobalConstants.Querystring_Outlook) {
        Office.initialize = function (reason) {
            enableAttachIfComposeMode();
        };
    }
})();

/* Function to notify the user about success/failure for attachment documents */
function notifyAttachmentResult() {
    "use strict";
    $(".warningPopUpHolder, .attachedSuccessPopUp, .attachedProgressPopUp, .attachedFailedPopUp").addClass("hide");
    if (oDocumentConstants.bAttachDocumentFailed) {
        ////show failure message
        $(".attachedFailedPopUp").removeClass("hide");
    } else {
        //// show success message
        $(".attachedSuccessPopUp").removeClass("hide");
    }
    $(".warningPopUpHolder").removeClass("hide");
    var oAllSelector = $("#CheckBox .ms-ChoiceField-input");
    if (oAllSelector && oAllSelector.length) {
        oAllSelector[0].checked = false;
    }
}

/* Send asynchronous calls to send each document as attachment */
function sendAttachmentAsync(sDocumentPath, sDocumentName) {
    "use strict";
    Office.context.mailbox.item.addFileAttachmentAsync(sDocumentPath, sDocumentName, {
        asyncContext: {
            sCurrentDocumentPath: sDocumentPath, sCurrentDocumentName: sDocumentName
        }
    },
    function (asyncResult) {
        if (asyncResult.status === Office.AsyncResultStatus.Failed) {
            $(".failureDocumentList").append("<div title=\"" + asyncResult.asyncContext.sCurrentDocumentName + "\" class=\"documentList\">" + asyncResult.asyncContext.sCurrentDocumentName + "</div>");
            oDocumentConstants.bAttachDocumentFailed = true;
        }
        oDocumentConstants.iAsyncCallsCompleted++;
        if ($(".is-selectedRow").length === oDocumentConstants.iAsyncCallsCompleted) {
            notifyAttachmentResult();
        } else {
            $("#currentDocumentCount").text(parseInt(oDocumentConstants.iAsyncCallsCompleted, 10) + 1);
        }
    });
}

/* Send selected documents as attachments to email */
function sendDocumentAsAttachment(event) {
    "use strict";
    $(".errorAttachDocument").addClass("hide");
    $(".failureDocumentList").empty();
    var oSelectedRows = $(".is-selectedRow");
    if (oSelectedRows && oSelectedRows.length) {
        if (5 < oSelectedRows.length) {
            $(".errorAttachDocument").removeClass("hide");
        } else {
            //// Display the in progress pop up
            $(".attachedProgressPopUp, .warningPopUpHolder").removeClass("hide");
            ////$("#totalDocumentCount").text(oSelectedRows.length);
            $(".progressPopUpDetails").html(oFindDocumentConstants.AttachInProgressMessage.replace("{0}", oSelectedRows.length));
            $("#currentDocumentCount").text(1);
            //// Send the call to attach the document
            var sDocumentURL = (4 === oCommonObject.iCurrentGridViewData()) ? "data-" + oGlobalConstants.GridViewPinnedDocumentUrl : "data-" + oGlobalConstants.Path
                , oNameColumn = $(".is-selectedRow .nameColumn")
                , sUrlArray = [];
            if (oNameColumn.length) {
                $.each(oNameColumn, function () {
                    var sCurrentDocumentPath = trimEndChar($(this).attr(sDocumentURL), "/"), sCurrentDocumentName;
                    if (sCurrentDocumentPath) {
                        sCurrentDocumentPath = trimEndChar(sCurrentDocumentPath.trim(), "/");
                        sCurrentDocumentName = sCurrentDocumentPath.substring(sCurrentDocumentPath.lastIndexOf("/") + 1);
                        if (sCurrentDocumentPath && sCurrentDocumentName) {
                            sendAttachmentAsync(decodeURIComponent(sCurrentDocumentPath), decodeURIComponent(sCurrentDocumentName));
                        }
                    }
                });
            }
        }
        event && event.stopPropagation();
    }
}

/* Function to enable attach document if email is opened in compose mode */
function enableAttachIfComposeMode() {
    "use strict";
    if (Office.context && Office.context.mailbox && Office.context.mailbox.item) {
        //// If email is opened in Compose mode then show the Send as attachment button
        var oCurrentEmailItem = Office.context.mailbox.item.get_data();
        if (oCurrentEmailItem && oCurrentEmailItem.$0_0) {
            var sEmailCreatedTime = oCurrentEmailItem.$0_0.dateTimeCreated
                , sEmailModifiedTime = oCurrentEmailItem.$0_0.dateTimeModified;
            //// Created time and Modified time are undefined if email is opened in compose mode
            if ("undefined" === typeof (sEmailCreatedTime) && "undefined" === typeof (sEmailModifiedTime)) {
                $("#attachDocuments").attr("data-applicable", true);
                $("#attachDocuments").click(function (event) {
                    var bIsDisabled = $(this).hasClass("is-disabled");
                    if (!bIsDisabled) {
                        //// Log event
                        commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oFindDocumentConstants.Event_Name_Attach_Document, true);
                        oDocumentConstants.bAttachDocumentFailed = false;
                        oDocumentConstants.iAsyncCallsCompleted = 0;
                        sendDocumentAsAttachment(event);
                    }
                    event && event.preventDefault();
                });
            }
        }
    }
}

/* Function to update dynamic text on page */
function updateDynamicText() {
    "use strict";
    $(".warningMessageText").text(oFindDocumentConstants.MaxAttachedMessage).attr("title", oFindDocumentConstants.MaxAttachedMessage);
    $(".warningPopUpDetails").text(oFindDocumentConstants.AttachSuccessMessage).attr("title", oFindDocumentConstants.AttachSuccessMessage);
    $(".attachedPopUpDetails").text(oFindDocumentConstants.AttachFailureMessage).attr("title", oFindDocumentConstants.AttachFailureMessage);
    ////$(".progressPopUpDetails").html(oFindDocumentConstants.AttachInProgressMessage);
    $("#attachButtonText").text(oFindDocumentConstants.AttachButtonText).attr("title", oFindDocumentConstants.AttachButtonText);
}

/* Function to be called before search matter */
function getSearchDocumentsBeforeSend(result) {
    "use strict";
    var container = result.oParam.container;
    $("#gridViewContainer_Grid").find(".lazyLoading").removeClass("hide");
    oCommonObject.isServiceCallComplete = false;
};

/* Function for success of search document */
function getSearchDocumentsSuccess(result) {
    "use strict";
    $("#gridViewContainer_Grid").find(".lazyLoading").addClass("hide");
    $("#gridViewContainer .jsonGridHeader").removeAttr("disabled").removeClass("disableHeader"); //// Enable the grid header
    oCommonObject.isServiceCallComplete = true;
    if (result && $.trim(result.Result) && result.oParam) {
        var sData = result.Result.split("$|$")
          , bIsPageLoad;

        var searchDocuments = JSON.parse(sData[0]);
        var totalResults = parseInt(sData[1], 10) || 0;
        if (!searchDocuments.code || "0" === searchDocuments.code) {
            //// Pinned matters exists for the current logged in user
            if ("undefined" !== typeof sData[1] && totalResults) {
                //// Check if the data is loaded for page load or lazy load
                if (1 === oGridViewObject.pageNumber) {
                    bIsPageLoad = true;
                    oGridViewObject.searchResultCount = totalResults;
                    if ($("#attachDocuments").attr("data-applicable")) {
                        $("#attachDocuments").removeClass("hide");
                    }
                    $("#attachDocuments").addClass("is-disabled");
                } else {
                    bIsPageLoad = false;
                }
                oGridView.loadGridView(sData[0], bIsPageLoad);
                oCommonObject.updatePinnedStatus();
                oCommonObject.updateSortingNotification();
                oCommonObject.enableSearch();
            } else {
                // No documents exists
                oGridView.loadGridView(sData[0], true);
                $("#attachDocuments").addClass("is-disabled");
                oCommonObject.updateSortingNotification();
                $("#loadingImageContainer").addClass("hide");
                $("#gridViewContainer_Grid tbody tr").removeClass("hide").addClass("invisible");
                $("#gridViewContainer_Grid tbody").append(oGlobalConstants.No_Results_Message);
                oGridViewObject.searchResultCount = 0;
                oCommonObject.enableSearch();
                oCommonObject.closeAllPopupExcept("", event);
            }
        } else {
            showCommonErrorPopUp(searchDocuments.code);
        }
    }
}

/* Function for failure of search document */
function getSearchDocumentsFailure(result) {
    "use strict";
    oCommonObject.isServiceCallComplete = true;
    oGridViewObject.waitTillDataLoaded = false;
    if (result.Result && result.Result.statusText && "abort" === result.Result.statusText) {
        return;
    }
    oCommonObject.enableSearch();
    showCommonErrorPopUp(result.Result);
}

/* Function to get the matters on page load */
function getSearchDocuments(oContainer, bIsFilterFlyoutCall) {
    "use strict";
    oCommonObject.disableSearch();
    var ofilterSearchText = oCommonObject.sSearchedKeyword
      , sSearchTerm = (ofilterSearchText && ofilterSearchText !== "") ? oCommonObject.formatSearchText(ofilterSearchText.trim(), oGlobalConstants.Document_ID) : ""
      , oFilterDetails = { ClientsList: [], FromDate: "", ToDate: "", FilterByMe: 1 }
      , oParam = { "container": oContainer, "bIsFilterFlyoutCall": bIsFilterFlyoutCall }
      , oSearchDetails;

    if (1 === oCommonObject.iCurrentGridViewData()) { //// If All Documents section (flag is 1) is selected, then set filter by me flag to 0
        oFilterDetails.FilterByMe = 0;
    }

    oCommonObject.getSortDetails(bIsFilterFlyoutCall);

    if (bIsFilterFlyoutCall) {
        $.extend(oFilterDetails, oCommonObject.oFlyoutFilters());
    }

    oSearchDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oCommonObject.getDeployedUrl() }, "searchObject": { "PageNumber": oGridViewObject.pageNumber, "ItemsPerPage": oGridViewObject.itemsPerPage, "SearchTerm": sSearchTerm, "Filters": oFilterDetails, "Sort": oCommonObject.oSortDetails() } };

    oCommonObject.callSearchService("FindDocument", oSearchDetails, getSearchDocumentsSuccess, getSearchDocumentsFailure, getSearchDocumentsBeforeSend, oParam);
}

//// #endregion

//// #region Pin Unpin functionality

/* Function for failure of pin document */
function pinDocumentFailure(result) {
    "use strict";
    oGridViewObject.waitTillDataLoaded = false;
    showCommonErrorPopUp(result.Result);
}

/* Function for success of pin document */
function pinDocumentSuccess(result) {
    "use strict";
    if (result && result.Result && $.parseJSON(result.Result.toLowerCase())) {
        $(result.oParam.container).attr("data-ispinned", "true"); //// Update the property of current element
        oCommonObject.userPinnedData.push(result.oParam.sCurrentDocumentUrl);
        oCommonObject.updatePinnedStatus();
    } else if (-1 < result.indexOf("code")) { //// Display error pop up in case of exception
        var oErrorPopUpData = JSON.parse(result);
        showCommonErrorPopUp(oErrorPopUpData.code);
    } else {
        // Reason: Something went wrong
        $("#gridViewContainer").empty().html("<span class='noResultsText'>" + oGlobalConstants.Failure_Message + "</span>"); /// Remove the data from the grid view
        $("#loadingImageContainer").addClass("hide");
    }
}

/* Function to call service to pin matter to user favorites */
function pinDocument(oCurrentRow) {
    "use strict";
    var sDocumentName, sDocumentVersion, sDocumentClient, sDocumentClientID, sDocumentClientUrl, sDocumentMatter, sDocumentMatterID, sDocumentOwner, sDocumentUrl, sDocumentOWAUrl, sDocumentExtension, sDocumentCreatedDate, sDocumentModifiedDate, sDocumentCheckoutUser, sDocumentMatterUrl, sDocumentParentUrl, sDocumentID, sLibraryName;
    if (oCurrentRow && oCurrentRow.length) {
        //// Get the meta data required for pinning a document
        sDocumentName = oCurrentRow.text() ? oCurrentRow.text() : "NA";
        sDocumentVersion = oCurrentRow.attr("data-" + oGlobalConstants.Managed_Property_Document_Version + "") ? checkNull(oCurrentRow.attr("data-" + oGlobalConstants.Managed_Property_Document_Version + "")) : "NA";
        sDocumentClient = oCurrentRow.attr("data-" + oGlobalConstants.Document_Client_Name + "") ? checkNull(oCurrentRow.attr("data-" + oGlobalConstants.Document_Client_Name + "")) : "NA";
        sDocumentClientID = oCurrentRow.attr("data-" + oGlobalConstants.Document_Client_ID + "") ? checkNull(oCurrentRow.attr("data-" + oGlobalConstants.Document_Client_ID + "").replace(/\(/g, "").replace(/\)/g, "")) : "NA";
        sDocumentClientUrl = oCurrentRow.attr("data-" + oGlobalConstants.SP_Web_Url + "") ? decodeURIComponent(checkNull(oCurrentRow.attr("data-" + oGlobalConstants.SP_Web_Url + ""))) : "NA";
        sDocumentMatter = oCurrentRow.attr("data-" + oGlobalConstants.Document_Matter_Name + "") ? checkNull(oCurrentRow.attr("data-" + oGlobalConstants.Document_Matter_Name + "")) : "NA";
        sDocumentMatterID = oCurrentRow.attr("data-" + oGlobalConstants.Document_Matter_ID + "") ? checkNull(oCurrentRow.attr("data-" + oGlobalConstants.Document_Matter_ID + "").replace(/\(/g, "").replace(/\)/g, "")) : "NA";
        sDocumentOwner = oCurrentRow.attr("data-" + oGlobalConstants.Author + "") ? checkNull(oCurrentRow.attr("data-" + oGlobalConstants.Author + "")) : "NA";
        sDocumentUrl = oCurrentRow.attr("data-" + oGlobalConstants.Path + "") ? checkNull(decodeURIComponent(trimEndChar(oCurrentRow.attr("data-" + oGlobalConstants.Path + ""), "/").toLowerCase())) : "NA";
        sDocumentExtension = oCurrentRow.attr("data-" + oGlobalConstants.File_Extension + "") ? checkNull(oCurrentRow.attr("data-" + oGlobalConstants.File_Extension + "")) : "NA";
        sDocumentCreatedDate = oCurrentRow.attr("data-" + oGlobalConstants.Created + "") ? checkNull(oCurrentRow.attr("data-" + oGlobalConstants.Created + "")) : "NA";
        sDocumentModifiedDate = oCurrentRow.attr("data-" + oGlobalConstants.Document_Last_Modified_Time + "") ? checkNull(oCurrentRow.attr("data-" + oGlobalConstants.Document_Last_Modified_Time + "")) : "NA";
        sDocumentOWAUrl = getDocumentUrl(oCurrentRow);
        sDocumentCheckoutUser = oCurrentRow.attr("data-" + oGlobalConstants.Managed_Property_Document_CheckOutuser + "") ? checkNull(oCurrentRow.attr("data-" + oGlobalConstants.Managed_Property_Document_CheckOutuser + "")) : "NA";
        sDocumentParentUrl = oCurrentRow.attr("data-" + oGlobalConstants.Parent_Link + "") ? decodeURIComponent(checkNull(oCurrentRow.attr("data-" + oGlobalConstants.Parent_Link + "")).replace(oGlobalConstants.All_Items_Extension, "")) : "NA";
        sDocumentMatterUrl = oCurrentRow.attr("data-" + oGlobalConstants.SP_Web_Url + "") ? checkNull(oCurrentRow.attr("data-" + oGlobalConstants.SP_Web_Url + "")) : "NA";
        sLibraryName = sDocumentParentUrl.replace(sDocumentMatterUrl, "").split("/")[1];
        sDocumentMatterUrl = decodeURIComponent(sDocumentMatterUrl + "/" + sLibraryName);
        sDocumentID = oCurrentRow.attr("data-" + oGlobalConstants.Document_ID + "") ? checkNull(oCurrentRow.attr("data-" + oGlobalConstants.Document_ID + "")) : "NA";

        //// Generate the JSON structure of the input to service
        var oPinParameters = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oGlobalConstants.Central_Repository_Url }, "documentData": { "DocumentName": sDocumentName, "DocumentVersion": sDocumentVersion, "DocumentClient": sDocumentClient, "DocumentClientId": sDocumentClientID, "DocumentClientUrl": sDocumentClientUrl, "DocumentMatter": sDocumentMatter, "DocumentMatterId": sDocumentMatterID, "DocumentOwner": sDocumentOwner, "DocumentUrl": sDocumentUrl, "DocumentOWAUrl": sDocumentOWAUrl, "DocumentExtension": sDocumentExtension, "DocumentCreatedDate": sDocumentCreatedDate, "DocumentModifiedDate": sDocumentModifiedDate, "DocumentCheckoutUser": sDocumentCheckoutUser, "DocumentMatterUrl": sDocumentMatterUrl, "DocumentParentUrl": sDocumentParentUrl, "DocumentID": sDocumentID } }, oParam = { "container": oCurrentRow, "sCurrentDocumentUrl": sDocumentUrl };
        oCommonObject.callSearchService("PinDocumentForUser", oPinParameters, pinDocumentSuccess, pinDocumentFailure, null, oParam);
    }
}

/* Function to check for null as string */
function checkNull(sOriginalString) {
    "use strict";
    if ("null" === sOriginalString) {
        sOriginalString = "NA";
    }
    return sOriginalString;
}

/* Function for success of unpin matter */
function unpinDocumentSuccess(result) {
    "use strict";
    if (result && result.Result && $.parseJSON(result.Result.toLowerCase())) {
        $(result.oParam.container).attr("data-ispinned", "false"); //// Update the property of current element
        /* Pop out the value of document which is currently unpinned */
        oCommonObject.userPinnedData = $.grep(oCommonObject.userPinnedData, function (sCurrentValue) {
            return sCurrentValue !== result.oParam.sDocumentUrl;
        });
        if (4 === oCommonObject.iCurrentGridViewData()) {
            oGridViewObject.pageNumber = 1; //// Reset the page number for grid view control
            $("#gridViewContainer").empty(); /// Remove the data from the grid view
            getPinnedDocument($("#pinnedDocuments")); //// Refresh the grid view control to show new pinned data only in case of pinned section
        }
    } else if (-1 < result.indexOf("code")) { //// Display error pop up in case of exception
        var oErrorPopUpData = JSON.parse(result);
        showCommonErrorPopUp(oErrorPopUpData.code);
    } else {
        // Reason: Matter is not pinned. Hence, cannot be unpinned.
        $("#gridViewContainer").empty().html("<span class='noResultsText'>" + oFindDocumentConstants.Failure_Invalid_Unpin + "</span>"); /// Remove the data from the grid view
        $("#loadingImageContainer").addClass("hide");
    }
}

/* Function for failure of unpin matter */
function unpinDocumentFailure(result) {
    "use strict";
    oGridViewObject.waitTillDataLoaded = false;
    showCommonErrorPopUp(result.Result);
}

/* Function to call service to unpin matter to user favorites */
function unpinDocument(oCurrentRow) {
    "use strict";
    var sDocumentUrl = "";
    if (oCurrentRow && oCurrentRow.length) {
        //// Get the meta data required for unpinning the matter
        if (4 === oCommonObject.iCurrentGridViewData()) {
            sDocumentUrl = oCurrentRow.attr("data-" + oGlobalConstants.GridViewPinnedDocumentUrl);
        } else {
            sDocumentUrl = oCurrentRow.attr("data-" + oGlobalConstants.Path);
        }
        sDocumentUrl = decodeURIComponent(trimEndChar($.trim(sDocumentUrl), "/").toLowerCase());
        sDocumentUrl = oCommonObject.htmlEncode(sDocumentUrl);
        //// Generate the JSON structure of the input to service
        var oUnpinParameters = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oGlobalConstants.Central_Repository_Url }, "documentData": { "DocumentUrl": sDocumentUrl } }, oParam = { "container": oCurrentRow, "sCurrentDocumentUrl": sDocumentUrl };

        oCommonObject.callSearchService("RemovePinnedDocument", oUnpinParameters, unpinDocumentSuccess, unpinDocumentFailure, null, oParam);
    }
}

/* Function to be called before getting pinned documents */
function getPinnedDocumentBeforeSend(result) {
    "use strict";
    var container = result.oParam.container;
    $("#loadingImageContainer").removeClass("hide");
}

/* Function for success of getting pinned document */
function getPinnedDocumentSuccess(result) {
    "use strict";
    $("#gridViewContainer_Grid").find(".lazyLoading").addClass("hide");
    if (result && $.trim(result.Result)) {
        var sData = result.Result.split("$|$"); //// sData[0] = Pinned Documents collection, sData[1] = count of pinned documents
        var pinnedDocuments = JSON.parse(sData[0]);
        if (!pinnedDocuments.code || "0" === pinnedDocuments.code) {
            //// Pinned documents exists for the current logged in user
            oCommonObject.pinnedData = [];
            oCommonObject.pinnedData.push(sData[0]);
            if ("undefined" !== typeof sData[1] && parseInt(sData[1], 10)) {
                oCommonObject.userPinnedData.length = 0; //// Refresh the array which holds the user pinned matter
                //// Add user's pinned documents name in the array
                $.each(pinnedDocuments, function (iCurrentIndex, oCurrentObject) {
                    if (oCurrentObject) {
                        var sDocumentUrl = $.trim(oCurrentObject.DocumentUrl);
                        if (sDocumentUrl) {
                            oCommonObject.userPinnedData.push(sDocumentUrl);
                        }
                    }
                });
                oCommonObject.updatePinnedStatus();

                //// Show the pinned data in grid view only if, user selects from drop down
                (4 === oCommonObject.iCurrentGridViewData()) && oGridView.loadGridView(sData[0], true); //// Always pass true as no lazy loading expected in pinned section
                $("#attachDocuments").addClass("is-disabled");
            } else {
                // No pinned documents exists for user.
                sData[1] = 0;
                oCommonObject.userPinnedData.length = 0;
                (4 === oCommonObject.iCurrentGridViewData()) && $("#gridViewContainer").html("<span class='noResultsText'>" + oFindDocumentConstants.No_Pinned_Documents + "</span>");
                $("#loadingImageContainer").addClass("hide");
            }
        } else {
            showCommonErrorPopUp(pinnedDocuments.code);
        }
    }
}

/* Function for failure of getting pinned document */
function getPinnedDocumentFailure(result) {
    "use strict";
    oGridViewObject.waitTillDataLoaded = false;
    showCommonErrorPopUp(result.Result);
}

/* Function to get the pinned documents on page load */
function getPinnedDocument(oContainer) {
    "use strict";
    var oPinnedDocumentDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oGlobalConstants.Central_Repository_Url } }
      , oParam = { "container": oContainer };

    oCommonObject.callSearchService("FindUserPinnedDocument", oPinnedDocumentDetails, getPinnedDocumentSuccess, getPinnedDocumentFailure, getPinnedDocumentBeforeSend, oParam);
}
////#endregion

//// #region Common Functions
/* Returns the document URL. For documents supported by OWA, it returns OWA URL. Otherwise, the document path. */
function getDocumentUrl(oCurrentRow) {
    "use strict";
    var sDocumentPath, sDocumentExtension, sDocumentSPWebUrl;
    if (oCurrentRow && oCurrentRow.length) {
        sDocumentPath = trimEndChar(oCurrentRow.attr("data-" + oGlobalConstants.Path), "/");
        sDocumentExtension = oCurrentRow.attr("data-" + oGlobalConstants.File_Extension);
        sDocumentSPWebUrl = oCurrentRow.attr("data-" + oGlobalConstants.SP_Web_Url);
        if (-1 < $.inArray(sDocumentExtension, oFilterGlobal.arrOWADocumentExtension) && sDocumentSPWebUrl) {
            sDocumentPath = commonFunction.getOWAUrl(sDocumentExtension, sDocumentSPWebUrl, sDocumentPath);
        }
    }
    return sDocumentPath;
}

/* Function to update the ECB actions in the ECB menu */
function updateECBforDocument(oContextualMenu, oSelectedElement) {
    "use strict";
    var sClientUrl = "", sMatterName = "", sParentLink = "", isPinned = "";

    if (4 === oCommonObject.iCurrentGridViewData()) { //// Get different field values for pinned view (flag = 4)
        sClientUrl = oSelectedElement.attr("data-documentclienturl"); //// These properties will not come from resource files as these are key names of pinned JSON object which will not change
        sParentLink = oSelectedElement.attr("data-documentparenturl");
        isPinned = true;
    } else {
        sClientUrl = oSelectedElement.attr("data-" + oGlobalConstants.SP_Web_Url);
        sParentLink = oSelectedElement.attr("data-" + oGlobalConstants.Parent_Link);
        isPinned = oSelectedElement.attr("data-ispinned");
    }
    //// Changes for URL consolidation
    sMatterName = sParentLink.replace(sClientUrl, "");
    sMatterName = sMatterName && sMatterName.split("/");
    sMatterName = sMatterName[1] ? sMatterName[1] : "";
    //// Disable the pin/unpin options based on the status of pinned document
    if (isPinned && $.parseJSON(isPinned)) {
        oContextualMenu.find(".pin").parent().addClass("removeECBItem");
        oContextualMenu.find(".unpin").parent().removeClass("removeECBItem");
    } else {
        oContextualMenu.find(".pin").parent().removeClass("removeECBItem");
        oContextualMenu.find(".unpin").parent().addClass("removeECBItem");
    }

    //// Update the location of matter landing page for go to Matter Sites action
    if ($.trim(sMatterName)) {
        var sAbsoluteClientUrl = sClientUrl.replace(oGlobalConstants.Site_Url, "")
            , sLandingPageName = "/" + sMatterName + ".aspx"
            , sMatterLandingPageUrl = sAbsoluteClientUrl + "/" + oGlobalConstants.Matter_Landing_Page_Repository + sLandingPageName + "$|$"
                                  + sAbsoluteClientUrl + "/" + sMatterName + "/" + sMatterName + sLandingPageName;

        //// Set the URL for go to Matter Sites
        oCommonObject.checkMatterLandingURL(sClientUrl, sMatterLandingPageUrl, sMatterLandingPageUrl, sParentLink, oContextualMenu);
    } else {
        var oGoToMatterSitesElement = oContextualMenu.find(".gotoMatterSites");
        //// Set the hyper link property of go to Matter Sites
        oGoToMatterSitesElement.attr({ "data-matterlink": sParentLink, "onclick": "viewMatter(this);" });
    }
}

/* Function to display Document flyout */
function showMatterDetailPopup(element, event, bUpload) {
    "use strict";
    var flyoutExist = $("#gridViewContainer .InfoFlyout");
    if (!flyoutExist.length) {
        var sFlyoutHTMLChunk = $(".InfoFlyout").length && $(".InfoFlyout").clone() && $(".InfoFlyout").clone()[0];
        $("#gridViewContainer").append(sFlyoutHTMLChunk);
    }
    $("#gridViewContainer .InfoFlyout").find(".ms-Callout-inner").addClass("hide");
    $("#gridViewContainer .InfoFlyout").removeClass("hide");
    $("#gridViewContainer .InfoFlyout").find("#FlyoutPopupLoading").removeClass("hide");
    var matterPopData = $(".InfoFlyout .ms-Callout-inner"), $Element = $(element);
    if (matterPopData.length && $Element) {
        if (4 === oCommonObject.iCurrentGridViewData()) {
            var documentName = $Element.attr("title") ? $Element.attr("title").trim() : "NA",
                matterName = $Element.attr("data-documentmatter") ? oCommonObject.renderAsText($Element.attr("data-documentmatter").trim()) : "NA",
                clientName = $Element.attr("data-documentclient") ? oCommonObject.renderAsText($Element.attr("data-documentclient").trim()) : "NA",
                  author = $Element.attr("data-documentowner") ? oCommonObject.renderAsText($Element.attr("data-documentowner").trim()) : "NA",
                documentUrl = $Element.attr("data-" + oFindDocumentConstants.GridViewDocumentOWAUrl) ? oCommonObject.renderAsText($Element.attr("data-" + oFindDocumentConstants.GridViewDocumentOWAUrl).trim()) : "",
                dateModified = $Element.attr("data-documentmodifieddate") ? oCommonObject.renderAsText($Element.attr("data-documentmodifieddate").trim()) : "NA",
                documentID = $Element.attr("data-documentid") ? oCommonObject.renderAsText($Element.attr("data-documentid").trim()) : "NA";
        } else {
            var documentName = $Element.attr("title") ? $Element.attr("title").trim() : "NA",
                matterName = $Element.attr("data-" + oGlobalConstants.Document_Matter_Name) ? oCommonObject.renderAsText($Element.attr("data-" + oGlobalConstants.Document_Matter_Name).trim()) : "NA",
                documentPath = $($Element.find("a")[0]).attr("href").trim(),
            clientName = $Element.attr("data-" + oGlobalConstants.Document_Client_Name) ? oCommonObject.renderAsText($Element.attr("data-" + oGlobalConstants.Document_Client_Name).trim()) : "NA",
            author = $Element.attr("data-" + oGlobalConstants.Author) ? oCommonObject.renderAsText($Element.attr("data-" + oGlobalConstants.Author).trim()) : "NA",
            documentUrl = documentPath ? oCommonObject.renderAsText(documentPath) : "",
            dateModified = $Element.attr("data-" + oGlobalConstants.Document_Last_Modified_Time) ? oCommonObject.renderAsText($Element.attr("data-" + oGlobalConstants.Document_Last_Modified_Time).trim()) : "NA",
            documentID = $Element.attr("data-" + oGlobalConstants.Document_ID) ? oCommonObject.renderAsText($Element.attr("data-" + oGlobalConstants.Document_ID).trim()) : "NA";
        }
        var libraryName = documentUrl.split("/");
        //// Add html chunk of flyout.
        matterPopData.find(".FlyoutHeadingText").text(documentName);
        matterPopData.find("#openDocument").attr({ "data-link": trimEndChar(documentUrl.trim(), "/"), "data-documentname": documentName, "data-clientName": clientName });
        matterPopData.find(".FlyoutMatterName").attr({ "data-matter": matterName }).html("<div class='fontWeight600 ms-font-m FlyoutContentHeading' title='Matter'>Matter: </div><div class='matterRowValue ms-font-m FlyoutContent' title='" + decodeURIComponent(matterName) + "'>" + decodeURIComponent(matterName) + "</div>");
        matterPopData.find(".FlyoutClientName").attr({ "data-client": clientName }).html("<div class='fontWeight600 ms-font-m FlyoutContentHeading' title='Client'>Client: </div><div class='matterRowValue ms-font-m FlyoutContent' title='" + decodeURIComponent(clientName) + "'>" + decodeURIComponent(clientName) + "</div>");
        matterPopData.find(".FlyoutDocumentID").attr({ "data-clientid": documentID, "data-matterid": documentID }).html("<div class='fontWeight600 ms-font-m FlyoutContentHeading' title='Document ID'>Document ID: </div><div class='matterRowValue ms-font-m FlyoutContent' title='" + documentID + "'>" + documentID + "</div>");
        matterPopData.find(".FlyoutAuthor").attr({ "data-matterAuthor": author }).html("<div class='fontWeight600 ms-font-m FlyoutContentHeading' title='Author'>Author: </div><div class='matterRowValue ms-font-m FlyoutContent' title='" + author + "'>" + author + "</div>");
        matterPopData.find(".FlyoutCreatedDate").attr({ "data-modifieddate": oGridView.formatDate(dateModified) }).html("<div class='fontWeight600 ms-font-m FlyoutContentHeading' title='Modified date'>Modified date: </div><div class='matterRowValue ms-font-m FlyoutContent' title='" + oGridView.formatDate(dateModified) + "'>" + oGridView.formatDate(dateModified) + "</div>");
        $("#gridViewContainer .InfoFlyout").find(".FlyoutUrlHeading").addClass("hide");
        $("#gridViewContainer .InfoFlyout").find(".FlyoutUrl").addClass("hide");
        $("#gridViewContainer .InfoFlyout").find("#uploadToMatter").addClass("hide");
        $("#gridViewContainer .InfoFlyout").find("#viewMatters").addClass("hide");

        //// Set the attributes for view document landing button
        oCommonObject.updateDocumentLandingAttributes($Element);
    }
    placeMatterDetailsPopup(element, event);
};

/* Function to adjust position of flyout according to word view and outlook view*/
function placeMatterDetailsPopup(element, event) {
    "use strict";
    ////Check if current view is word view and set data accordingly 
    ////Following code is written in Java Script because we already have QueryString Implementation for word and outlook, and to avoid code Repetition in CSS.
    var nWindowWidth = $(window).width();
    if (nWindowWidth <= 660) {
        $("#gridViewContainer .InfoFlyout").find(".FlyoutContentHeading").css("float", "none");
        $("#gridViewContainer .InfoFlyout").find(".FlyoutHeading").addClass("FlyoutHeadingWordMargin");
        $("#gridViewContainer .InfoFlyout").find(".ms-Callout-inner").addClass("FlyoutBoxContent");
        $("#gridViewContainer .InfoFlyout").find(".FlyoutBox").addClass("DocumentFlyoutWordSize");
        var flyoutArrowExist = $("#gridViewContainer .InfoFlyout").find(".flyoutToparrow");
        if (!flyoutArrowExist.length) {
            $("#gridViewContainer .InfoFlyout").prepend("<div class='flyoutToparrow'></div>");
        }
        $("#gridViewContainer .InfoFlyout").css({ top: ($(element).position().top + 20) });
    } else {
        $("#gridViewContainer .InfoFlyout").find(".FlyoutBox").addClass("DocumentFlyoutOutlookSize");
        var flyoutArrowExist = $("#gridViewContainer .InfoFlyout").find(".flyoutLeftarrow");
        if (!flyoutArrowExist.length) {
            $("#gridViewContainer .InfoFlyout").prepend("<div class='flyoutLeftarrow'></div>");
            $("#gridViewContainer .InfoFlyout").addClass("DocumentFlyoutLeftMove");
        }
        if ($(element).offset().top < 220) {
            $("#gridViewContainer .InfoFlyout").find(".flyoutLeftarrow").css("top", $(element).offset().top - 75);
            $("#gridViewContainer .InfoFlyout").css({ "top": (($(element).position().top - $(element).offset().top)) + 83 });
        } else {
            $("#gridViewContainer .InfoFlyout").find(".flyoutLeftarrow").css("top", "128");                 // If we use ADD and REMOVE class CSS won't override calculated top
            $("#gridViewContainer .InfoFlyout").css("top", Math.floor($(element).position().top) - 122);
        }
    }
    event.stopPropagation();
};

////#endregion

$(document).ready(function () {
    "use strict";
    oCommonObject.isMatterView = false; //// Flag to identify Search Document page
    oCommonObject.sCurrentPage = oGlobalConstants.App_Name_Search_Documents;
    commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + "/" + oCommonObject.sCurrentPage, true);
    $(".AppHeader").removeClass("hide"); //// Display the app header
    getContextualHelpData(2);
    $.ajax({
        url: "CommonControls.html?ver=25.0.0.0",
        success: function (response) {
            $("#HeaderPlaceHolder").html($(response).find("#commonSearchBar").html());
            $("#autoCompletePlaceHolder").attr("title", oFindDocumentConstants.AutoComplete_Placeholder).text(oFindDocumentConstants.AutoComplete_Placeholder);
            $("#gridView").html($(response).find("#gridViewContent").html());
            $("#SendToOneDrive").html($(response).find("#buttonContent").html());
            $("#EmailDocumentLinks").html($(response).find("#buttonContent").html());
            $("#SendToOneDrive .ms-Button-label").text("Send to OneDrive");
            $("#EmailDocumentLinks .ms-Button-label").text("Email document links");
            $(".textFlyoutContent").html($(response).find("#textFlyoutContent").html());
            $(".dateFlyoutContent").html($(response).find("#dateFlyoutContent").html());
            $(".clearFilterContent").html($(response).find("#clearFilterContent").html());
            $(".textFlyoutContent .clearFilterContent").find(".clearFilterText").attr("data-clearFilterType", "text");
            $(".dateFlyoutContent .clearFilterContent").find(".clearFilterText").attr("data-clearFilterType", "date");
            $(".InfoFlyout").html($(response).find(".commonFlyout").html());
            commonFunction.setOWADocumentExtension();
            //// Generate the common drop down and place it on page
            oCommonObject.generateCommonDropdown("#searchPanelDropdown", oFindDocumentConstants.SearchDropdownFields);
            //// Generate the column picker and place it on page
            oCommonObject.generateColumnPicker("#columnPickerPanel #columnPickerBlock #columnOptions", oFindDocumentConstants.GridViewColumnPickerFields);
            oCommonObject.addDropdownBindings();
            oCommonObject.addAutoCompleteBindings();
            oCommonObject.addColumnPickerBindings();
            oGridView.loadECBControl();
            oGridView.adjustGridViewHeight();
            oGridView.applyMapContainerHeight();
            $(".ms-Dropdown-title, #gridViewPageHeader").attr("title", oGlobalConstants.My_Documents_Message).text(oGlobalConstants.My_Documents_Title); //// Set the drop down title and page header to My Matters on page load
            oCommonObject.addFilterFlyoutBindings();
        }
    });
    $(document).on("click", ".nameColumn", function (event) {
        showMatterDetailPopup(this, event, "false");
        oCommonObject.getDocumentLandingAssets(this);
        oCommonObject.closeAllPopupExcept("InfoFlyout", event);
    });
    $(document).on("click", ".warningPopUpCloseIconContainer , .attachedPopUpCloseIconContainer", function (event) {
        $(".warningPopUpHolder, .attachedSuccessPopUp, .attachedProgressPopUp, .attachedFailedPopUp").addClass("hide");
        var oGridRow = $(".GridRow, .GridRowAlternate");
        oGridView.highlightGridViewRow(oGridRow, false);
        $("#attachDocuments").addClass("is-disabled");
    });
    $(document).on("click", ".errorAttachDocument", function (event) {
        $(this).addClass("hide");
    });
    $(document).on("click", ".warningPopUpContainer", function (event) {
        oCommonObject.closeAllPopupExcept("warningPopUpHolder", event);
        event && event.stopPropagation();
    });
    updateDynamicText();
});
