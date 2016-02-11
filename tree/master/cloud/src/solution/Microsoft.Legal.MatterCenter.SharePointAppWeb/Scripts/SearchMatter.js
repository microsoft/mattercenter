/// <disable>JS1003,JS2032,JS2074,JS2076,JS2024,JS3116,JS3085,JS3058,JS3092,JS3057,JS3054,JS2073,JS2005,JS3056</disable>

var oSearchGlobal = {
    matterUrl: "",
    oServiceRequest: {},
    dataArray: [],    //// This and its occurrences needs to be removed from actual implementation, placed it for testing purpose       
};

//// #region Search Matters section

/* Function to be called before search matter */
function getSearchMattersBeforeSend(result) {
    "use strict";
    var container = result.oParam.container;
    $("#gridViewContainer_Grid").find(".lazyLoading").removeClass("hide");
    oCommonObject.isServiceCallComplete = false;
};

/* Function for success of search matter */
function getSearchMattersSuccess(result) {
    "use strict";
    $("#gridViewContainer_Grid").find(".lazyLoading").addClass("hide");
    $("#gridViewContainer .jsonGridHeader").removeAttr("disabled").removeClass("disableHeader"); //// Enable the grid header
    oCommonObject.isServiceCallComplete = true;
    if (result && $.trim(result.Result) && result.oParam) {
        var sData = result.Result.split("$|$")
            , bIsPageLoad;

        var searchMatters = JSON.parse(sData[0]);
        var totalResults = parseInt(sData[1], 10) || 0;
        if (!searchMatters.code || "0" === searchMatters.code) {
            //// Pinned matters exists for the current logged in user
            if ("undefined" !== typeof sData[1] && totalResults) {
                //// Check if the data is loaded for page load or lazy load
                if (1 === oGridViewObject.pageNumber) {
                    bIsPageLoad = true;
                    oGridViewObject.searchResultCount = totalResults;
                } else {
                    bIsPageLoad = false;
                }
                oGridView.loadGridView(sData[0], bIsPageLoad);
                oCommonObject.updatePinnedStatus();
                oCommonObject.updateSortingNotification();
                oCommonObject.enableSearch();
            } else {
                // No matters exists
                oGridView.loadGridView(sData[0], true);
                oCommonObject.updateSortingNotification(result.oParam.bIsFilterFlyoutCall);
                $("#loadingImageContainer").addClass("hide");
                $("#gridViewContainer_Grid tbody tr").removeClass("hide").addClass("invisible");
                $("#gridViewContainer_Grid tbody").append(oGlobalConstants.No_Results_Message);
                oGridViewObject.searchResultCount = 0;
                oCommonObject.enableSearch();
                oCommonObject.closeAllPopupExcept("", event);
            }
        } else {
            showCommonErrorPopUp(searchMatters.code);
        }
    }
}

/* Function for failure of search matter */
function getSearchMattersFailure(result) {
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
function getSearchMatters(oContainer, bIsFilterFlyoutCall) {
    "use strict";
    oCommonObject.disableSearch();
    var ofilterSearchText = oCommonObject.sSearchedKeyword
      , sSearchTerm = (ofilterSearchText && ofilterSearchText !== "") ? oCommonObject.formatSearchText(ofilterSearchText.trim(), oGlobalConstants.Matter_ID) : ""
      , oFilterDetails = { ClientsList: [], PGList: "", AOLList: "", FromDate: "", ToDate: "", FilterByMe: 1 }
      , oParam = { "container": oContainer, "bIsFilterFlyoutCall": bIsFilterFlyoutCall }
      , oSearchDetails;

    if (1 === oCommonObject.iCurrentGridViewData()) { //// If All Matters section (flag is 1) is selected, then set filter by me flag to 0
        oFilterDetails.FilterByMe = 0;
    }

    oCommonObject.getSortDetails(bIsFilterFlyoutCall);

    if (bIsFilterFlyoutCall) {
        $.extend(oFilterDetails, oCommonObject.oFlyoutFilters());
    }

    oSearchDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oCommonObject.getDeployedUrl() }, "searchObject": { "PageNumber": oGridViewObject.pageNumber, "ItemsPerPage": oGridViewObject.itemsPerPage, "SearchTerm": sSearchTerm, "Filters": oFilterDetails, "Sort": oCommonObject.oSortDetails() } };

    oCommonObject.callSearchService("FindMatter", oSearchDetails, getSearchMattersSuccess, getSearchMattersFailure, getSearchMattersBeforeSend, oParam);
}

//// #endregion

//// #region Pin Unpin functionality

/* Function for success of pin matter */
function pinMatterSuccess(result) {
    "use strict";
    if (result && result.Result && $.parseJSON(result.Result.toLowerCase())) {
        $(result.oParam.container).attr("data-ispinned", "true"); //// Update the property of current element       
        oCommonObject.userPinnedData.push(result.oParam.sCurrentMatterUrl);
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

/* Function for failure of pin matter */
function pinMatterFailure(result) {
    "use strict";
    oGridViewObject.waitTillDataLoaded = false;
    showCommonErrorPopUp(result.Result);
}

/* Function to call service to pin matter to user favorites */
function pinMatter(oCurrentRow) {
    "use strict";
    var sMatterName, sMatterID, sMatterDescription, sPracticeGroup, sAreaofLaw, sSubAreaofLaw, sResponsibleAttorney, sMatterCreatedDate, sClientName, sClientID, sClientUrl, sMatterUrl, bHideUpload, sLastModifiedDate, sMatterGuid;

    if (oCurrentRow && oCurrentRow.length) {
        //// Get the meta data required for pinning a matter
        sMatterName = oCurrentRow.attr("data-" + oGlobalConstants.Matter_Name + "");
        sMatterID = oCurrentRow.attr("data-" + oGlobalConstants.Matter_ID + "");
        sMatterDescription = oCurrentRow.attr("data-" + oGlobalConstants.Description + "");
        sPracticeGroup = oCurrentRow.attr("data-" + oGlobalConstants.Practice_Group + "");
        sAreaofLaw = oCurrentRow.attr("data-" + oGlobalConstants.Area_Of_Law + "");
        sSubAreaofLaw = oCurrentRow.attr("data-" + oGlobalConstants.Sub_Area_Of_Law + "");
        sResponsibleAttorney = oCurrentRow.attr("data-" + oGlobalConstants.Responsible_Attorney + "");
        sMatterCreatedDate = validateDateFormat(oCurrentRow.attr("data-" + oGlobalConstants.Open_Date + ""));
        sClientName = oCurrentRow.attr("data-" + oGlobalConstants.Client_Name + "");
        sClientID = oCurrentRow.attr("data-" + oGlobalConstants.Client_ID + "");
        sClientUrl = oCurrentRow.attr("data-" + oGlobalConstants.Site_Name + "");
        sMatterUrl = oCurrentRow.attr("data-" + oGlobalConstants.Path + "");
        sLastModifiedDate = oCurrentRow.attr("data-" + oGlobalConstants.Last_Modified_Time + "");
        bHideUpload = oCurrentRow.attr("data-" + oGlobalConstants.IsReadOnly + "");
        //// Changes for URL consolidation
        sMatterGuid = ("null" !== oCurrentRow.attr("data-" + oGlobalConstants.Matter_GUID + "")) ? oCurrentRow.attr("data-" + oGlobalConstants.Matter_GUID + "") : sMatterName;

        //// Generate the JSON structure of the input to service
        var oPinParameters = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oGlobalConstants.Central_Repository_Url }, "matterData": { "MatterName": sMatterName, "MatterDescription": sMatterDescription, "MatterCreatedDate": sMatterCreatedDate, "MatterUrl": sMatterUrl, "MatterPracticeGroup": sPracticeGroup, "MatterAreaOfLaw": sAreaofLaw, "MatterSubAreaOfLaw": sSubAreaofLaw, "MatterClientUrl": sClientUrl, "MatterClient": sClientName, "HideUpload": bHideUpload, "MatterID": sMatterID, "MatterClientId": sClientID, "MatterResponsibleAttorney": sResponsibleAttorney, "MatterModifiedDate": sLastModifiedDate, "MatterGuid": sMatterGuid } }
            , oParam = {
                "container": oCurrentRow, "sCurrentMatterUrl": sMatterUrl
            };

        oCommonObject.callSearchService("PinMatterForUser", oPinParameters, pinMatterSuccess, pinMatterFailure, null, oParam);
    }
}

/* Function for success of unpin matter */
function unpinMatterSuccess(result) {
    "use strict";
    if (result && result.Result && $.parseJSON(result.Result.toLowerCase())) {
        $(result.oParam.container).attr("data-ispinned", "false"); //// Update the property of current element
        /* Pop out the value of matter which is currently unpinned */
        oCommonObject.userPinnedData = $.grep(oCommonObject.userPinnedData, function (sCurrentValue) {
            return sCurrentValue !== result.oParam.sCurrentMatterUrl;
        });
        if (4 === oCommonObject.iCurrentGridViewData()) {
            oGridViewObject.pageNumber = 1; //// Reset the page number for grid view control
            $("#gridViewContainer").empty(); /// Remove the data from the grid view
            getPinnedMatters($("#pinnedMatters")); //// Refresh the grid view control to show new pinned data only in case of pinned section
        }
    } else if (-1 < result.indexOf("code")) { //// Display error pop up in case of exception
        var oErrorPopUpData = JSON.parse(result);
        showCommonErrorPopUp(oErrorPopUpData.code);
    } else {
        // Reason: Matter is not pinned. Hence, cannot be unpinned.
        $("#gridViewContainer").empty().html("<span class='noResultsText'>" + oFindMatterConstants.Failure_Invalid_Unpin + "</span>"); /// Remove the data from the grid view
        $("#loadingImageContainer").addClass("hide");
    }
}

/* Function for failure of unpin matter */
function unpinMatterFailure(result) {
    "use strict";
    oGridViewObject.waitTillDataLoaded = false;
    showCommonErrorPopUp(result.Result);
}

/* Function to call service to unpin matter to user favorites */
function unpinMatter(oCurrentRow) {
    "use strict";
    var sMatterUrl = "";
    if (oCurrentRow && oCurrentRow.length) {
        //// Get the meta data required for unpinning the matter
        if (4 === oCommonObject.iCurrentGridViewData()) {
            sMatterUrl = oCurrentRow.attr("data-matterurl");
        } else {
            sMatterUrl = oCurrentRow.attr("data-path");
        }

        //// Generate the JSON structure of the input to service
        var oUnpinParameters = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oGlobalConstants.Central_Repository_Url }, "matterData": { "MatterName": sMatterUrl } }, oParam = {
            "container": oCurrentRow, "sCurrentMatterUrl": sMatterUrl
        };

        oCommonObject.callSearchService("RemovePinnedMatter", oUnpinParameters, unpinMatterSuccess, unpinMatterFailure, null, oParam);
    }
}

/* Function to be called before getting pinned matters */
function getPinnedMatterBeforeSend(result) {
    "use strict";
    var container = result.oParam.container;
    $("#loadingImageContainer").removeClass("hide");
}

/* Function for success of getting pinned matters */
function getPinnedMatterSuccess(result) {
    "use strict";
    $("#gridViewContainer_Grid").find(".lazyLoading").addClass("hide");
    if (result && $.trim(result.Result)) {
        var sData = result.Result.split("$|$"); //// sData[0] = Pinned Matters collection, sData[1] = count of pinned matters
        var pinnedMatters = JSON.parse(sData[0]);
        oCommonObject.pinnedData = [];
        oCommonObject.pinnedData.push(sData[0]);
        if (!pinnedMatters.code || "0" === pinnedMatters.code) {
            //// Pinned matters exists for the current logged in user
            if ("undefined" !== typeof sData[1] && parseInt(sData[1], 10)) {
                oCommonObject.userPinnedData.length = 0; //// Refresh the array which holds the user pinned matter
                //// Add user's pinned matters name in the array
                $.each(pinnedMatters, function (iCurrentIndex, oCurrentObject) {
                    if (oCurrentObject) {
                        var sMatterUrl = $.trim(oCurrentObject.MatterUrl);
                        if (sMatterUrl) {
                            oCommonObject.userPinnedData.push(sMatterUrl);
                        }
                    }
                });
                oCommonObject.updatePinnedStatus();

                //// Show the pinned data in grid view only if, user selects from drop down
                (4 === oCommonObject.iCurrentGridViewData()) && oGridView.loadGridView(sData[0], true); //// Always pass true as no lazy loading expected in pinned section
            } else {
                // No pinned matters exists for user.
                sData[1] = 0;
                oCommonObject.userPinnedData.length = 0;
                (4 === oCommonObject.iCurrentGridViewData()) && $("#gridViewContainer").html("<span class='noResultsText'>" + oFindMatterConstants.Failure_No_Pinned_Matters + "</span>");
                $("#loadingImageContainer").addClass("hide");
            }
        } else {
            showCommonErrorPopUp(pinnedMatters.code);
        }
    }
}

/* Function for failure of getting pinned matters */
function getPinnedMatterFailure(result) {
    "use strict";
    oGridViewObject.waitTillDataLoaded = false;
    showCommonErrorPopUp(result.Result);
}

/* Function to get the pinned matters on page load */
function getPinnedMatters(oContainer) {
    "use strict";
    var oPinnedMatterDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oGlobalConstants.Central_Repository_Url } }
      , oParam = {
          "container": oContainer
      };

    oCommonObject.callSearchService("FindUserPinnedMatter", oPinnedMatterDetails, getPinnedMatterSuccess, getPinnedMatterFailure, getPinnedMatterBeforeSend, oParam);
}
//// #endregion

//// #region Common Functions
/* Function to update the ECB actions in the ECB menu */
function updateECBforMatter(oContextualMenu, oSelectedElement) {
    "use strict";
    var bIsReadOnlyUser = "", isPinned = "", sMatterUrl = "", matterName = "", siteName = "";
    var $Element = oSelectedElement;
    if ($Element) {
        if (4 === oCommonObject.iCurrentGridViewData()) {
            siteName = $Element.attr("data-matterclienturl") ? oCommonObject.renderAsText($Element.attr("data-matterclienturl").trim()) : "NA";
            bIsReadOnlyUser = oSelectedElement.attr("data-hideupload");
            isPinned = true;
            sMatterUrl = oSelectedElement.attr("data-matterurl");
            //// Changes for URL consolidation
            matterName = (null !== $Element.attr("data-MatterGuid")) ? $Element.attr("data-MatterGuid") : ($Element.attr("data-mattername") ? $Element.attr("data-mattername").trim() : "NA");
        } else {
            siteName = $Element.attr("data-" + oGlobalConstants.Site_Name) ? oCommonObject.renderAsText($Element.attr("data-" + oGlobalConstants.Site_Name).trim()) : "NA";
            bIsReadOnlyUser = oSelectedElement.attr("data-isreadonlyuser");
            isPinned = oSelectedElement.attr("data-ispinned");
            sMatterUrl = oSelectedElement.attr("data-path");
            //// Changes for URL consolidation
            matterName = ("null" !== $Element.attr("data-" + oGlobalConstants.Matter_GUID)) ? $Element.attr("data-" + oGlobalConstants.Matter_GUID) : ($Element.attr("data-" + oGlobalConstants.Matter_Name) ? $Element.attr("data-" + oGlobalConstants.Matter_Name).trim() : "NA");
        }
    }
    var sOneNoteUrl = siteName.replace(oGlobalConstants.Site_Url, "") + "/" + matterName + oGlobalConstants.OneNoteLibrary_Name_Suffix + "/" + matterName + "/" + matterName + oFindMatterConstants.OneNoteTableOfContentsExtension + "$|$" + siteName.replace(oGlobalConstants.Site_Url, "") + "/" + matterName + "/" + matterName + "/" + matterName + oFindMatterConstants.OneNoteTableOfContentsExtension
         , sMatterLandingPageUrl = siteName.replace(oGlobalConstants.Site_Url, "") + "/" + oGlobalConstants.Matter_Landing_Page_Repository + "/" + matterName + oFindMatterConstants.MatterLandingPageExtension + "$|$" + siteName.replace(oGlobalConstants.Site_Url, "") + "/" + matterName + "/" + matterName + "/" + matterName + oFindMatterConstants.MatterLandingPageExtension;

    //// Disable the upload option for read only user
    if (bIsReadOnlyUser && $.parseJSON(bIsReadOnlyUser)) {
        oContextualMenu.find(".upload").addClass("is-disabled");
        oContextualMenu.find(".upload").attr("disabled", "disabled");
    } else {
        oContextualMenu.find(".upload").removeClass("is-disabled");
        oContextualMenu.find(".upload").removeAttr("disabled");
    }
    //// Disable the pin/unpin options based on the status of pinned matter
    if (isPinned && $.parseJSON(isPinned)) {
        oContextualMenu.find(".pin").parent().addClass("removeECBItem");
        oContextualMenu.find(".unpin").parent().removeClass("removeECBItem");
    } else {
        oContextualMenu.find(".pin").parent().removeClass("removeECBItem");
        oContextualMenu.find(".unpin").parent().addClass("removeECBItem");
    }
    //// Set the URL for go to Matter Sites
    oCommonObject.checkMatterLandingURL(siteName, sOneNoteUrl, sMatterLandingPageUrl, sMatterUrl, oContextualMenu);
}

/* Converts and return date to UTC format */
function validateDateFormat(sDateToBeProcessed) {
    "use strict";
    var oDates = new Date(sDateToBeProcessed);
    if (isNaN(oDates)) {
        var arrSplitedDate = sDateToBeProcessed.replace(/[-]/g, "/");
        arrSplitedDate = arrSplitedDate.split("/");
        var sFormattedDate = arrSplitedDate[1] + "-" + arrSplitedDate[0] + "-" + arrSplitedDate[2];
        oDates = new Date(sFormattedDate);
    }
    return oDates.toISOString();
}

//// #endregion

//// #region events for upload popup and drill down on folders

// This region will be common for search matter and web dashboard. Is taken from filterPanel.js
var $MailBodyElement = $("#mailBody");

$MailBodyElement.on("dblclick", ".folderStructureContent ul li ul li", function (e) {
    "use strict";
    e.stopPropagation();
    e.stopImmediatePropagation();
    e.preventDefault();
    oUploadGlobal.oDrilldownParameter.nCurrentLevel++;
    commonFunction.drilldownNavigation(e);
    $(".folderStructureContent, .parentNode").addClass("folderStructureWithBreadcrumb");
});

$MailBodyElement.on("dragover", ".folderStructureContent ul li", function (e, ui) {
    "use strict";
    e.stopImmediatePropagation();
    $(this).addClass("folderDragOver");
});

$MailBodyElement.on("dragleave", ".folderStructureContent ul li", function (e, ui) {
    "use strict";
    $(this).removeClass("folderDragOver");
});

$MailBodyElement.on("click", "#rootLink", function () {
    "use strict";
    commonFunction.clearGlobalVariables();
    var htmlChunk = buildNestedList(oSearchGlobal.oFolderName, null);
    if ("undefined" !== typeof htmlChunk) {
        $(".folderStructureContent").html(htmlChunk);
    }
    if ("undefined" !== typeof bindDroppable) {
        bindDroppable($(".folderStructureContent ul li"));
    }
    $(".folderStructureContent, .parentNode").removeClass("folderStructureWithBreadcrumb");
});

$MailBodyElement.on("click", "#parentFolder", function (e) {
    "use strict";
    oUploadGlobal.oDrilldownParameter.nCurrentLevel--;
    commonFunction.drilldownNavigation(e);
});

/// Office.js Initialization 
(function () {
    "use strict";
    // The Office initialize function must be run each time a new page is loaded 
    if (oCommonObject.getParameterByName("appType") === oGlobalConstants.Querystring_Outlook) {
        Office.initialize = function (reason) {
        };
    }
})();

function attachmentTokenCallbackEmailClient(asyncResult, userContext) {
    "use strict";
    if ("succeeded" === asyncResult.status) {
        oSearchGlobal.oServiceRequest.attachmentToken = asyncResult.value;
        createMailPopup();
    } else {
        showNotification(oFindMatterConstants.Fail_Attachment_Token, "failNotification");
    }
}

// Function to check if string empty or not.
function checkEmptyorWhitespace(input) {
    "use strict";
    if (/\S/.test(input)) {
        return input;
    }
    return oFindMatterConstants.No_Subject_Mail;
}

/* Function to populate folder structure based for matter library */
function populateFolderHierarchy(element) {
    "use strict";
    $(".loadingImage").css("position", "absolute");

    var sMatterName, sMatterUrl, oMatterFolderDetails;
    //// Get the meta-data based on current view (flag = 4 for pinned view)
    if (4 === oCommonObject.iCurrentGridViewData()) {
        sMatterName = $(element).attr("data-mattername");
        sMatterUrl = $(element).attr("data-matterclienturl");
    } else {
        //// Changes for URL consolidation
        sMatterName = ($(element).attr("data-" + oGlobalConstants.Matter_Name)) ? $(element).attr("data-" + oGlobalConstants.Matter_Name).trim() : "NA";
        sMatterUrl = $(element).attr("data-sitename");
    }

    oMatterFolderDetails = {
        "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "matterData": { "MatterName": sMatterName, "MatterUrl": sMatterUrl }
    };

    oSearchGlobal.matterUrl = sMatterUrl;

    oCommonObject.callSearchService("GetFolderHierarchy", oMatterFolderDetails, folderHierarchySuccess, folderHierarchyFailure, folderHierarchyBeforeCall, $(element));
};

/* Function to display Matter flyout */
function showMatterDetailPopup(element, event, bUpload) {
    "use strict";
    var flyoutExist = $("#gridViewContainer .InfoFlyout");
    if (!(flyoutExist.length)) {
        var sFlyoutHTMLChunk = $(".InfoFlyout").length && $(".InfoFlyout").clone() && $(".InfoFlyout").clone()[0];
        $("#gridViewContainer").append(sFlyoutHTMLChunk);
        $(".FlyoutUrlTextbox").on("click", function (event) {
            event && event.stopPropagation();
        });
    }
    flyoutExist = $("#gridViewContainer .InfoFlyout"); //// Get the fly out again to get the updated chunk
    flyoutExist.find(".ms-Callout-inner").addClass("hide");
    flyoutExist.removeClass("hide");
    flyoutExist.find("#FlyoutPopupLoading").removeClass("hide");
    var matterPopData = $(".InfoFlyout .ms-Callout-inner"), $Element = $(element);
    if (matterPopData.length && $Element) {
        if (4 === oCommonObject.iCurrentGridViewData()) {
            //// Changes for URL consolidation
            var matterName = (null !== $Element.attr("data-MatterGuid")) ? $Element.attr("data-MatterGuid") : ($Element.attr("data - mattername") ? $Element.attr("data - mattername").trim() : "NA"),
                siteName = $Element.attr("data-matterclienturl") ? oCommonObject.renderAsText($Element.attr("data-matterclienturl").trim()) : "NA",
                clientName = $Element.attr("data-matterclient") ? oCommonObject.renderAsText($Element.attr("data-matterclient").trim()) : "NA",
                subAreaOfLaw = $Element.attr("data-mattersubareaoflaw") ? oCommonObject.renderAsText($Element.attr("data-mattersubareaoflaw").trim()) : "NA",
                matterURL = $Element.attr("data-matterclienturl") && matterName ? oCommonObject.renderAsText(siteName + "/" + matterName) : "NA",
                matterClientID = $Element.attr("data-matterclientid") ? oCommonObject.renderAsText($Element.attr("data-matterclientid").trim()) : "NA",
                matterMatterID = $Element.attr("data-matterid") ? oCommonObject.renderAsText($Element.attr("data-matterid").trim()) : "NA",
                matterResponsibleAttorney = $Element.attr("data-matterresponsibleattorney") ? oCommonObject.renderAsText($Element.attr("data-matterresponsibleattorney").trim()) : "NA",
                //// Changes for URL consolidation
                matterOriginalName = $Element.attr("data-mattername") ? oCommonObject.renderAsText($Element.attr("data-mattername").trim()) : "NA";
        } else {
            var matterName = ("null" !== $Element.attr("data-" + oGlobalConstants.Matter_GUID)) ? $Element.attr("data-" + oGlobalConstants.Matter_GUID) : ($Element.attr("data-" + oGlobalConstants.Matter_Name) ? $Element.attr("data-" + oGlobalConstants.Matter_Name).trim() : "NA"),
               siteName = $Element.attr("data-" + oGlobalConstants.Site_Name) ? oCommonObject.renderAsText($Element.attr("data-" + oGlobalConstants.Site_Name).trim()) : "NA",
               clientName = $Element.attr("data-" + oGlobalConstants.Client_Name) ? oCommonObject.renderAsText($Element.attr("data-" + oGlobalConstants.Client_Name).trim()) : "NA",
               subAreaOfLaw = $Element.attr("data-" + oGlobalConstants.Sub_Area_Of_Law) ? oCommonObject.renderAsText($Element.attr("data-" + oGlobalConstants.Sub_Area_Of_Law).trim()) : "NA",
               matterURL = $Element.attr("data-" + oGlobalConstants.Path) ? oCommonObject.renderAsText($Element.attr("data-" + oGlobalConstants.Path).trim()) : "NA";
            matterClientID = $Element.attr("data-" + oGlobalConstants.Client_ID) ? oCommonObject.renderAsText($Element.attr("data-" + oGlobalConstants.Client_ID).trim()) : "NA",
            matterMatterID = $Element.attr("data-" + oGlobalConstants.Matter_ID) ? oCommonObject.renderAsText($Element.attr("data-" + oGlobalConstants.Matter_ID).trim()) : "NA",
            matterResponsibleAttorney = $Element.attr("data-" + oGlobalConstants.Responsible_Attorney) ? oCommonObject.renderAsText($Element.attr("data-" + oGlobalConstants.Responsible_Attorney).trim()) : "NA",
            //// Changes for URL consolidation
            matterOriginalName = $Element.attr("data-" + oGlobalConstants.Matter_Name) ? oCommonObject.renderAsText($Element.attr("data-" + oGlobalConstants.Matter_Name).trim()) : "NA";
        }
        var documentLibraryURL = matterURL;
        documentLibraryURL = documentLibraryURL.replace(oFindMatterConstants.Document_Library_EndURL, "/");
        //// Trim semi colon from the end if it is present
        subAreaOfLaw = trimEndChar(subAreaOfLaw, ";");
        matterResponsibleAttorney = trimEndChar(matterResponsibleAttorney, ";");
        matterPopData.find(".FlyoutHeadingText").attr({ "title": matterOriginalName, "data-mattername": matterName, "data-OriginalName": matterOriginalName, "data-client": siteName, "data-subareaoflaw": subAreaOfLaw, "data-matterurl": siteName + "/" + matterName + "/" + matterName + ".aspx", "data-clientName": clientName }).text(matterOriginalName);
        matterPopData.find(".FlyoutClientName").attr({ "data-client": clientName }).html("<div class='fontWeight600 ms-font-m FlyoutContentHeading' title='Client'>Client: </div><div class='matterRowValue ms-font-m FlyoutContent' title='" + decodeURIComponent(clientName) + "'>" + decodeURIComponent(clientName) + "</div>");
        matterPopData.find(".FlyoutClientMatterID").attr({ "data-clientid": matterClientID, "data-matterid": matterMatterID }).html("<div class='fontWeight600 ms-font-m FlyoutContentHeading' title='Client.Matter ID'>Client.Matter ID: </div><div class='matterRowValue ms-font-m FlyoutContent' title='" + matterClientID + oGlobalConstants.ClientID_MatterID_Separator + matterMatterID + "'>" + matterClientID + oGlobalConstants.ClientID_MatterID_Separator + matterMatterID + "</div>");
        matterPopData.find(".FlyoutSubAreaoflaw").attr({ "data-practicegroup": subAreaOfLaw }).html("<div class='fontWeight600 ms-font-m FlyoutContentHeading' title='Sub area of law'>Sub area of law: </div><div class='matterRowValue ms-font-m FlyoutContent' title='" + subAreaOfLaw + "'>" + subAreaOfLaw + "</div>");
        matterResponsibleAttorney = matterResponsibleAttorney.replace(/;/g, ",");
        matterResponsibleAttorney = $.trim(matterResponsibleAttorney) && trimEndChar($.trim(matterResponsibleAttorney), ",");
        matterPopData.find(".FlyoutResponsibleAttorney").attr({ "data-matterresponsibleattorney": matterResponsibleAttorney }).html("<div class='fontWeight600 ms-font-m FlyoutContentHeading' title='Responsible attorney'>Responsible attorney: </div><div class='matterRowValue ms-font-m FlyoutContent' title='" + matterResponsibleAttorney + "'>" + matterResponsibleAttorney + "</div>");
        matterPopData.find(".FlyoutUrlHeading").text("Save the document to this location");
        matterPopData.find(".FlyoutUrlText").text("URL:");
        matterPopData.find(".FlyoutUrlTextbox").val(documentLibraryURL);
        $("#openDocument").addClass("hide");
        $("#viewDocumentLandingPage").addClass("hide");
        ////If outlook view then Show "upload to matter" Button else show textbox
        if (oCommonObject.getParameterByName("appType") === oGlobalConstants.Querystring_Office) {
            flyoutExist.find(".FlyoutUrlHeading").removeClass("hide");
            flyoutExist.find(".FlyoutUrl").removeClass("hide");
            flyoutExist.find("#uploadToMatter").addClass("hide");
        } else {
            flyoutExist.find(".FlyoutUrlHeading").addClass("hide");
            flyoutExist.find(".FlyoutUrl").addClass("hide");
            flyoutExist.find("#uploadToMatter").removeClass("hide");
            flyoutExist.find("#uploadToMatter").on("click", function () {
                //// Use oGridViewObject.oCurrentMandatory object for getting current mandatory column
                populateFolderHierarchy($Element);
            });
            var bIsReadOnlyUser = $Element.attr("data-isreadonlyuser");
            if (bIsReadOnlyUser === "true") {
                flyoutExist.find("#uploadToMatter").addClass("is-disabled");
                flyoutExist.find("#uploadToMatter").attr("disabled", "disabled");
            } else {
                flyoutExist.find("#uploadToMatter").removeClass("is-disabled");
                flyoutExist.find("#uploadToMatter").removeAttr("disabled");
            }
        }
        var sOneNoteUrl = siteName.replace(oGlobalConstants.Site_Url, "") + "/" + matterName + oGlobalConstants.OneNoteLibrary_Name_Suffix + "/" + matterName + "/" + matterName + oFindMatterConstants.OneNoteTableOfContentsExtension + "$|$" + siteName.replace(oGlobalConstants.Site_Url, "") + "/" + matterName + "/" + matterName + "/" + matterName + oFindMatterConstants.OneNoteTableOfContentsExtension
        , sMatterLandingPageUrl = siteName.replace(oGlobalConstants.Site_Url, "") + "/" + oGlobalConstants.Matter_Landing_Page_Repository + "/" + matterName + oFindMatterConstants.MatterLandingPageExtension + "$|$" + siteName.replace(oGlobalConstants.Site_Url, "") + "/" + matterName + "/" + matterName + "/" + matterName + oFindMatterConstants.MatterLandingPageExtension;
        placeMatterDetailsPopup(element, event);
        urlExists(siteName, matterName, sOneNoteUrl, sMatterLandingPageUrl, matterURL);
    }
}

/* Function to adjust position of flyout according to word view and outlook view*/
function placeMatterDetailsPopup(element, event) {
    "use strict";
    ////Following code is written in Java Script because we already have QueryString Implementation for word and outlook, and to avoid code Repetition in CSS.
    var nWindowWidth = $(window).width();
    var oFlyout = $("#gridViewContainer .InfoFlyout");
    if (nWindowWidth <= 645) {
        oFlyout.find(".FlyoutHeading").addClass("FlyoutHeadingWordMargin");
        oFlyout.find(".ms-Callout-inner").addClass("FlyoutBoxContent");
        oFlyout.find(".FlyoutBox").addClass("MatterFlyoutWordSize");
        oFlyout.find(".FlyoutContentHeading").css("float", "none");
        var flyoutArrowExist = $("#gridViewContainer .InfoFlyout").find(".flyoutToparrow");
        if (!flyoutArrowExist.length) {
            oFlyout.prepend("<div class='flyoutToparrow'></div>");
        }
        oFlyout.css({ top: ($(element).position().top + 25) });
        if (!(oCommonObject.getParameterByName("appType") === oGlobalConstants.Querystring_Office)) {
            oFlyout.find(".FlyoutBox").addClass("OutlookMatterFlyoutWordSize");
        }
    } else {
        oFlyout.find(".FlyoutBox").addClass("MatterFlyoutOutlookSize");
        var flyoutArrowExist = $("#gridViewContainer .InfoFlyout").find(".flyoutLeftarrow");
        if (!flyoutArrowExist.length) {
            oFlyout.prepend("<div class='flyoutLeftarrow'></div>");
            oFlyout.addClass("MatterFlyoutLeftMove");
        }
        if ($(element).offset().top < 220) {
            oFlyout.find(".flyoutLeftarrow").css("top", $(element).offset().top - 75);
            oFlyout.css({ "top": (($(element).position().top - $(element).offset().top)) + 83 });
        } else {
            oFlyout.css("top", Math.floor($(element).position().top) - 122);
            oFlyout.find(".flyoutLeftarrow").css("top", "128");                      // If we use ADD and REMOVE class CSS won't override calculated top
        }
        if (oCommonObject.getParameterByName("appType") === oGlobalConstants.Querystring_Office) {
            oFlyout.find(".FlyoutBox").addClass("WordMatterFlyoutOutlookSize");
        }
    }
    if (null != event) {
        event.stopPropagation();
    }
}

/*Function to handle check if URL exist or not*/
function urlExists(sClientURL, sMatterName, sOneNoteURL, sMatterLandingPageURL, sMatterLibraryURL) {
    "use strict";
    var sParameters = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": sClientURL }, requestedUrl: sOneNoteURL, requestedPageUrl: sMatterLandingPageURL }, oParam = { "sitename": sClientURL, "matterName": sMatterName, "matterLibraryURL": sMatterLibraryURL };
    oCommonObject.callSearchService("UrlExists", sParameters, onURLExistSuccess, onURLExistFailure, null, oParam);
}

/* Function to handle success call*/
function onURLExistSuccess(result) {
    "use strict";
    //// Handle the true and false responses for the Matter Landing page and OneNote TOC
    if (result && result.Result) {
        var oSplitResults = result.Result.split("$|$");
        if (2 === oSplitResults.length) {
            var oMatterLandingPageResult = oSplitResults[1].split("$#$");
            var anchorElement = document.createElement("a");
            anchorElement.href = result.oParam.sitename;
            var oMatterPopData = $(".InfoFlyout .ms-Callout-inner"), sMatterPageLink;
            //// Set the link to the Matter Landing page or to the All Items page based on the value returned
            if (oMatterLandingPageResult && 2 === oMatterLandingPageResult.length) {
                sMatterPageLink = "true" === oMatterLandingPageResult[0] ? anchorElement.protocol + "//" + anchorElement.hostname + oMatterLandingPageResult[1] : result.oParam.matterLibraryURL;
            } else {
                sMatterPageLink = result.oParam.matterLibraryURL;
            }
            if (oMatterPopData[0]) {
                oMatterPopData.find("#viewMatters").attr({ "data-matterlink": sMatterPageLink, "onclick": "viewMatter(this);" });
            }
            $("#FlyoutPopupLoading").addClass("hide");
            $("#gridViewContainer .InfoFlyout").find(".ms-Callout-inner").removeClass("hide");
        } else {
            onURLExistFailure(result);
        }
    }
}
/* Function to handle failed call*/
function onURLExistFailure(result) {
    "use strict";
    //// Do not display the OneNote link
    if (result && result.oParam && result.oParam.matterLibraryURL) {
        var oMatterPopData = $(".InfoFlyout .ms-Callout-inner"),
            sMatterPageLink = result.oParam.matterLibraryURL;
        if (oMatterPopData[0]) {
            oMatterPopData.find("#viewMatters").attr({ "data-matterlink": sMatterPageLink, "onclick": "ViewMatter(this);" });
        }
    }
    //// Finally show the data
    $("#FlyoutPopupLoading").addClass("hide");
    $("#gridViewContainer .InfoFlyout").find(".ms-Callout-inner").removeClass("hide");
}
//// #endregion

$(document).ready(function () {
    "use strict";
    oCommonObject.isMatterView = true; //// Flag to identify Search Matter page
    oCommonObject.sCurrentPage = oGlobalConstants.App_Name_Search_Matters;
    commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + "/" + oCommonObject.sCurrentPage, true);
    $(".AppHeader").removeClass("hide"); //// Display the app header
    $.event.props.push("dataTransfer");
    getContextualHelpData(1);
    $.ajax({
        url: "CommonControls.html?ver=25.0.0.0",
        success: function (response) {
            $("#HeaderPlaceHolder").html($(response).find("#commonSearchBar").html());
            $("#autoCompletePlaceHolder").attr("title", oFindMatterConstants.AutoComplete_Placeholder).text(oFindMatterConstants.AutoComplete_Placeholder);
            $("#gridView").html($(response).find("#gridViewContent").html());
            $(".textFlyoutContent").html($(response).find("#textFlyoutContent").html());
            $(".dateFlyoutContent").html($(response).find("#dateFlyoutContent").html());
            $(".clearFilterContent").html($(response).find("#clearFilterContent").html());
            $(".textFlyoutContent .clearFilterContent").find(".clearFilterText").attr("data-clearFilterType", "text");
            $(".dateFlyoutContent .clearFilterContent").find(".clearFilterText").attr("data-clearFilterType", "date");
            $(".InfoFlyout").html($(response).find(".commonFlyout").html());
            //// Generate the common drop down and place it on page
            oCommonObject.generateCommonDropdown("#searchPanelDropdown", oFindMatterConstants.SearchDropdownFields);
            //// Generate the column picker and place it on page
            oCommonObject.generateColumnPicker("#columnPickerPanel #columnPickerBlock #columnOptions", oFindMatterConstants.GridViewColumnPickerFields);
            oCommonObject.addDropdownBindings();
            oCommonObject.addAutoCompleteBindings();
            oCommonObject.addColumnPickerBindings();
            oGridView.loadECBControl();
            oGridView.adjustGridViewHeight();
            oGridView.applyMapContainerHeight();
            $(".ms-Dropdown-title, #gridViewPageHeader").attr("title", oGlobalConstants.My_Matters_Message).text(oGlobalConstants.My_Matters_Title); //// Set the drop down title and page header to My Matters on page load
            oCommonObject.addFilterFlyoutBindings();
        }
    });
    $.ajax({
        url: "filterpanel.html?ver=25.0.0.0",
        success: function (response) {
            $("#mailBody").html($(response).find("#commonMailBody").html());
            $("#popupFooterText").text(oGlobalConstants.Upload_Footer_Text);
        }
    });
    $(document).on("click", ".nameColumn", function (event) {
        showMatterDetailPopup(this, event, "false");
        oCommonObject.closeAllPopupExcept("InfoFlyout", event);
    });
});

