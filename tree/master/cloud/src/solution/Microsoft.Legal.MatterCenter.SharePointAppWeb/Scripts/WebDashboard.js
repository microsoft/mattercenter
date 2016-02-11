/// <disable>JS3085.VariableDeclaredMultipleTimes,JS3058,JS3057,JS2064,JS2027,JS2032,JS2074,JS3092,JS2076,JS2024,JS2016,JS2026,JS3116,JS2005,JS2073,JS1003</disable>

(function () {
    "use strict";
    if (window.top !== window.self) {
        $("body").addClass("hideScrollSP");
    }
    // Solution to force a reload when page is loaded from back-forward cache to resolve issue in Safari Browser
    $(window).bind("pageshow", function (event) {
        if (event.originalEvent.persisted) {
            window.location.reload();
        }
    });
})();

$(document).on("click", ".matterName, .matterTitle", function (event) {
    "use strict";
    if (!event) {
        event = window.event;
        event.cancelBubble = true;
    }
    if (event.stopPropagation) {
        event.stopPropagation();
    }
});

$(document).ready(function (event) {
    "use strict";
    /* Set current page to 3 */
    oPageConfig.currentPage = 3;
    oGridConfig.bPageLoad = true;
    oCommonObject.sCurrentPage = oGlobalConstants.App_Name_Web_Dashboard;
    commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + "/" + oCommonObject.sCurrentPage, true);
    $("#MasterPageContainer").removeClass("MasterPageContent");
    $("#MasterPageForm > .mailpopupContainerBackground").remove();
    $("#MasterPageForm > .mailContainer").remove();

    // Logic to prevent multiple service call on resize of browser height
    oGridConfig.prevBrowserWidth = $(window).height();

    $(".navDiv").addClass("divPinnedMatters");
    $(".templates").attr({ "href": oGlobalConstants.Site_Url + oWebDashboardConstants.TemplatesURL, "target": "_parent" });
    $("#settingsLink").attr({ "href": oGlobalConstants.Site_Url + oGlobalConstants.SettingsPage, "target": "_parent" });
    $(".appName").find("a").attr({ "href": oGlobalConstants.Site_Url + oGlobalConstants.TenantWebDashboard, "target": "_parent" });
    $("#optionFileNameASCForMatters, #optionFileNameDESCForMatters").attr("data-sort-value", oGlobalConstants.Matter_Name);
    $("#optionCreatedDateASCForMatters, #optionCreatedDateDESCForMatters").attr("data-sort-value", oGlobalConstants.Open_Date);
    $("#optionModifiedDateASCForMatters, #optionModifiedDateDESCForMatters").attr("data-sort-value", oGlobalConstants.Last_Modified_Time);
    $("#optionFileNameASCForDocuments, #optionFileNameDESCForDocuments").attr("data-sort-value", oGlobalConstants.File_Name);
    $("#optionCreatedDateASCForDocuments, #optionCreatedDateDESCForDocuments").attr("data-sort-value", oGlobalConstants.Created_Date);
    $("#optionModifiedDateASCForDocuments, #optionModifiedDateDESCForDocuments").attr("data-sort-value", oGlobalConstants.Document_Last_Modified_Time);
    var arrClientParameters = getParameterByName("clientparameters").split("$|$");
    if (1 < arrClientParameters.length) {
        oSearchGlobal.sClientName = arrClientParameters[0];
        oSearchGlobal.sClientSiteUrl = arrClientParameters[1];
    } else if (1 === arrClientParameters.length && "" !== arrClientParameters[0]) {
        oSearchGlobal.bIsTenantCall = true;
    }
    var arrSectionParameter = getParameterByName("section");
    if (arrSectionParameter && "1" === arrSectionParameter) {                           // if section=1 in query string, load Matter view
        oGridConfig.isMatterView = true;
        $("#DisplayFindMattersResults").attr("class", "fallbackFlex");
        $("#DisplaySearchDocumentsResults").attr("class", "hide");
        $("#EmailDiv").css("display", "none");
    } else if (arrSectionParameter && "2" === arrSectionParameter) {                    // if section=2 in query string, load Documents view
        oGridConfig.isMatterView = false;
        $("#DisplayFindMattersResults").attr("class", "hide");
        $("#DisplaySearchDocumentsResults").attr("class", "fallbackFlex");
        $("#footer").addClass("documentFooter");
        $("#EmailDiv").css("display", "inline");
    }
    if (oGridConfig.isMatterView) {
        $("#searchMatterTab").addClass("selected");
        $("#searchDocumentTab").removeClass("selected");
    } else {
        $("#searchMatterTab").removeClass("selected");
        $("#searchDocumentTab").addClass("selected");
    }

    $("#feedbackandsupport").attr("href", oWebDashboardConstants.Feedback_and_Support);
    if ($(".pinnedSearch").hasClass("active")) {
        oGridConfig.currentView = 1;
    } else if ($(".col2").hasClass("active")) {
        oGridConfig.currentView = 2;
    } else {
        oGridConfig.currentView = 0;
    }

    $(document).on("click", ".quickLinksMenuButton", function (event) {
        $(".quickLinksMenuItems").toggle();
        closeAllPopupExcept("quickLinksMenuItems", event);
        event.stopPropagation();
    });
    $(document).on("click", ".matterPopup", function (event) {
        commonFunction.closeAllFilterExcept("matterPopup", event);
        event.stopPropagation();
    });
    $(document).on("click", ".documentPopup", function (event) {
        commonFunction.closeAllFilterExcept("documentPopup", event);
        event.stopPropagation();
    });

    $(".topNavMenuButton").click(function () {

        if ($(".responsiveMenu_topNavItems").is(":visible")) {
            $(".responsiveMenu_topNavItems").hide();
            $(".switchApp").css("top", "10px");
        } else {
            $(".responsiveMenu_topNavItems").show();
            $(".switchApp").css("top", "-147");
        }
    });
    $(".LeftContent").bind("DOMNodeInserted DOMSubtreeModified DOMNodeRemoved", function (event) {
        var leftContentHeight = $(".LeftContent").height();
        $(".sideNav").height(parseInt(leftContentHeight, 10) - 54);
    });
    $(".TopNavElements a").click(function () {
        $(".TopNavElements a").removeClass();
        $($(this).attr("href")).addClass("active");
    });
    $("#EmailDiv").click(function (event) {
        if (oGridConfig.arrSelectedGridData.length !== 0) {
            $("#EmailDiv").css("cursor", "pointer");
            $("#EmailDiv").css("color", "#333");
            showPopup(event);
        } else {
            $("#EmailDiv").css("cursor", "default");
            $("#EmailDiv").css("color", "#444");
        }
        $(".content a").not(this).hide("slow");
    });

    // for find document and find matter auto complete search
    $(document).on("keyup", ".filterSearchText", function (event) {
        oCommonObject.bHideAutoComplete = false;
        if ($(this).val()) {
            if (13 === event.which) {
                oCommonObject.bHideAutoComplete = true;
                // App Insight Event tracking for Textbox search
                var sCurrentEvent = (oGridConfig.isMatterView ? oGlobalConstants.Matter_Textbox_Search : oGlobalConstants.Document_Textbox_Search);
                commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + sCurrentEvent, true);
                oGridConfig.nGridPageNumber = 1;
                updateSortSettings(event);
                if (0 !== $(".autoCompleteSelected").length) {
                    var sSelectedContent = $(".autoCompleteSelected").text();
                    // Removal of unwanted HTML Tags
                    if (sSelectedContent && "" !== sSelectedContent) {
                        sSelectedContent = sSelectedContent.replace(/"<span class=\"highlightTerm\">"/g, "").replace(/"<\/span>"/g, "");
                    }
                    $(".filterSearchText").val(sSelectedContent);
                    $(".filterAutoComplete").html("").addClass("hide");
                }
                oGridConfig.isMatterView ? setGlobalRefiners() : $("#results").click();
                advancedSearch("grid", "tileContainer");
                advanceSearchDropdown();
            }
            if ("" !== $(".filterAutoComplete").html()) {
                if (0 === $(".autoCompleteSelected").length) {
                    if (40 === event.which) { $($(".autoCompleteRow")[0]).addClass("autoCompleteSelected"); }
                    if (38 === event.which) { $($(".autoCompleteRow")[parseInt($(".autoCompleteRow").length, 10) - 1]).addClass("autoCompleteSelected"); }

                } else {
                    var nPosition = $(".autoCompleteRow").filter(".autoCompleteSelected").index();

                    $($(".autoCompleteRow")[nPosition]).removeClass("autoCompleteSelected");
                    if (40 === event.which) {
                        if (parseInt($(".autoCompleteRow").length, 10) - 1 !== nPosition) {
                            nPosition = nPosition + 1;
                        } else {

                            $(".filterSearchText").focus();
                            $(".autoCompleteRow").removeClass("autoCompleteSelected");
                            return false;
                        }
                    }
                    if (38 === event.which) {
                        if (0 !== nPosition) {
                            nPosition = nPosition - 1;
                        } else {

                            $(".filterSearchText").focus();
                            $(".autoCompleteRow").removeClass("autoCompleteSelected");
                            return false;
                        }
                    }
                    $($(".autoCompleteRow")[nPosition]).addClass("autoCompleteSelected");
                }
            }
            if (40 !== event.which && 38 !== event.which && 13 !== event.which && "" !== $(this).val()) {
                autoComplete();
            }
        } else {
            oCommonObject.bHideAutoComplete = true;
            $(".filterAutoComplete").addClass("hide");
        }
    });

    $(".mailCartCloseIcon").click(function () {
        $(".popupContainerBackground").click();
    });

    /*Function to close the popup */
    $(".popupContainerBackground").not(".popUpContainer").click(function () {
        "use strict";
        $(".popupContainerBackground, .popUpContainer").addClass("hide");
        $(".popUpContainer").css("height", "");
    });
    $("#MatterCreation, #responsiveMenu_MatterCreation").attr("href", oWebDashboardConstants.Matter_Provision_App_Link);

    // For sorting the grid in matter view

    $(".sdBannerDropdown").click(function (event) {
        var $Element = oGridConfig.isMatterView ? $("#sdBannerDropdownForMatters") : $("#sdBannerDropdownForDocuments"), oSearchPopup = $("#filterSearch");
        commonFunction.openSortDropdown(".sdBannerDropdown", $Element, 1, event);
        if (oSearchPopup && oSearchPopup.is(":visible")) {
            $(".filterSearchDD").toggle();
        }
        event && event.stopPropagation();
        commonFunction.closeAllFilterExcept("sdBannerPanel", null);
    });

    $(".viewSwitcher").click(function (event) {
        commonFunction.closeAllFilterExcept("switchTab", null);
    });

    $(".sdBannerPanelItem").click(function (event) {
        oGridConfig.nGridPageNumber = 1;
        var sortText = this.innerHTML;
        var $Element = $(".sdBannerText");
        var sortTextWidth = sortText.length > 5 ? sortText.length < 9 ? 56 : (sortText.length - 2) * 8 : 35;
        $Element.val(sortText).css("width", sortTextWidth + "px")
                                        .attr("data-sort-value", $(this).attr("data-sort-value"))
                                        .attr("data-order", $(this).attr("data-order"))
                                        .attr("data-value", $(this).attr("data-value"))
                                        .attr("data-ss-value", $(this).attr("data-ss-value"));

        if (1 === parseInt($Element.attr("data-result"), 10)) {
            // call sorting for all search result
            oCommonObject.bCalledForSort = true;
            advancedSearch();
        } else if (2 === parseInt($Element.attr("data-result"), 10)) {
            // Clear the filter panel
            clearFilterPanel();

            // call sorting for recent document result.
            oGridConfig.isMatterView ? oCommonObject.getRecentMatters("recentMatters", 1, 1, false, event) : oCommonObject.getRecentDocuments("RecentDocumentContainer", 1, 1, false, event);
        } else {
            // call sorting for all pinned result 
            var sSelectedValue = $Element.attr("data-value");
            if (oGridConfig.arrPinnedData.length) {
                oGridConfig.isMatterView ? sortPinMatter("pinnedGrid_Grid", sSelectedValue) : sortPinDocument("pinnedGrid_Grid", sSelectedValue);
            }
        }
        $(".sdBannerPanel").addClass("hide");
        createResponsiveGrid();
    });

    // Initialized sort by dropdown
    var arrDropDownOptions = [oGlobalConstants.Sort_Type_None, oGlobalConstants.Sort_Type_Alphabetical_ASC, oGlobalConstants.Sort_Type_Alphabetical_DESC, oGlobalConstants.Sort_Type_Date_Created_ASC, oGlobalConstants.Sort_Type_Date_Created_DESC, oGlobalConstants.Sort_Type_Date_Modified_ASC, oGlobalConstants.Sort_Type_Date_Modified_DESC];
    $("#sdBannerDropdownForMatters .sdBannerPanelItem").each(function (index) {
        $(this).text(arrDropDownOptions[index]);
    });

    $("#sdBannerDropdownForDocuments .sdBannerPanelItem").each(function (index) {
        $(this).text(arrDropDownOptions[index]);
    });

    $("#RecentMattersHeader").click(function (event) {
        // App Insight Event tracking for recent matters
        commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Recent_Matters, true);
    });
    $("#PinnedMatters").click(function (event) {
        // App Insight Event tracking for pinned matters
        commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Pinned_Matters, true);
    });
    $("#PinnedDocuments").click(function (event) {
        // App Insight Event tracking for recent matters
        commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Pinned_Documents, true);
    });
    $("#RecentDocuments").click(function (event) {
        // App Insight Event tracking for recent matters
        commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Recent_Documents, true);
    });
});

function removeAttachment(thisObject) {
    "use strict";
    // App Insight Event tracking for removing mailcart documents
    commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Remove_From_Mail_Cart, true);
    var iIterator,
        iRetrieveIndex = parseInt($(thisObject).parents("li")[0].id),
        iSelectedTab;
    for (iIterator = 0; iIterator < $(thisObject).parents("li").siblings("li").length; iIterator++) {
        var iListItemSiblings = $(thisObject).parents("li").siblings("li")[iIterator].id;
        if (iListItemSiblings > $(thisObject).parents("li")[0].id) {
            $(thisObject).parents("li").siblings("li")[iIterator].id = iListItemSiblings - 1;
        }
    }
    if (1 === oGridConfig.currentView) {
        iSelectedTab = "#pinnedGrid_Grid";
        for (iIterator = 0; iIterator <= oGridConfig.arrPinnedData.length - 1; iIterator++) {
            if (oGridConfig.arrPinnedData[iIterator].DocumentUrl === oGridConfig.arrSelectedGridData[iRetrieveIndex].DocumentUrl || oGridConfig.arrPinnedData[iIterator].DocumentUrl === oGridConfig.arrSelectedGridData[iRetrieveIndex].Path) {
                removeCheck(iSelectedTab, iIterator);
            }
        }
    } else {
        iSelectedTab = 2 === oGridConfig.currentView ? "#RecentDocumentContainer_Grid" : "#grid_Grid";
        for (iIterator = 0; iIterator <= oGridConfig.arrGridData.length - 1; iIterator++) {
            if (oGridConfig.arrGridData[iIterator].Path === oGridConfig.arrSelectedGridData[iRetrieveIndex].DocumentUrl || oGridConfig.arrGridData[iIterator].Path === oGridConfig.arrSelectedGridData[iRetrieveIndex].Path) {
                removeCheck(iSelectedTab, iIterator);
            }
        }
    }

    if ($(iSelectedTab).find("#isSelectRowsActive").find("input[type='checkbox']:checked")) {
        $(iSelectedTab).find("#isSelectRowsActive").prop("checked", false);
    }
    oGridConfig.arrSelectedGridData.splice(iRetrieveIndex, 1);
    $(thisObject).parents("li").remove();
    var nCount = oGridConfig.arrSelectedGridData.length;
    displayAttachmentCount(nCount);
    if ($(".attachmentList").length === 0) {
        $(".popupContainerBackground, .popUpContainer").addClass("hide");
    }
}

function removeCheck(iSelectedTab, iIterator) {
    "use strict";
    var $this;
    if (!oGridConfig.isTileView) {

        $this = $("" + iSelectedTab + " tbody tr:nth-child(" + (iIterator + 1) + ") td div").find(".documentCheckBox");
        if ($this.is(":checked")) {
            $this.attr("checked", false);
        }
    } else {
        $this = $("" + iSelectedTab + " tbody tr:nth-child(" + (iIterator + 1) + ") td div");
        var mailCartIcon = $($this.find(".addToMailCart")).find(".mailCartRemoveIcon");
        if ($(mailCartIcon).length) {
            $(mailCartIcon).removeClass("mailCartRemoveIcon").addClass("floatContentLeft mailCartAddIcon").attr("src", "../Images/add-to-cart-666.png");
        } else {
            $(mailCartIcon).removeClass("floatContentLeft mailCartAddIcon").addClass("mailCartRemoveIcon").attr("src", "../Images/remove-from-cart-d24726.png");
        }
    }
}

function displayAttachmentCount(nCount) {
    "use strict";
    if (nCount === 1) {
        $(".popUpHeader .popUpHeaderTitle #EmailHeader").text("You have " + nCount + " attachment");
    } else {
        $(".popUpHeader .popUpHeaderTitle #EmailHeader").text("You have " + nCount + " attachments");
    }
    $("#EmailTextCount").text("(" + nCount + ")");
}

function placeDetailsPopup(element, event) {
    "use strict";
    (oGridConfig.isMatterView) ? $("#matterPopupPointerPosition").removeClass("matterPopupPointerFirst matterPopupPointerOffice flyoutPositioningDefault").addClass("matterPopupPointer") :
    $("#documentPopupPointerPosition").removeClass("matterPopupPointerFirst matterPopupPointerOffice flyoutPositioningDefault").addClass("matterPopupPointer");
    var popup = oGridConfig.isMatterView ? "matterPopup" : "documentPopup";
    var nAddTop = 0, left = 0;
    var nWidth = getWidth();
    var iContentHeight = $(".LeftContent").height();
    var iExpectedHeight = $(element).offset().top + $("." + popup).height();

    iExpectedHeight = iExpectedHeight + 95;
    nAddTop = (nWidth >= 1200) ? 129 : 94;
    left = 170;
    $("." + popup).css({ top: ($(element).offset().top - nAddTop), left: ($(element).offset().left + left) });

    if (("" !== oSearchGlobal.sClientName && "" !== oSearchGlobal.sClientSiteUrl) || oSearchGlobal.bIsTenantCall) {
        (iContentHeight < iExpectedHeight) ? updateIframeHeight(iExpectedHeight) : updateIframeHeight(iContentHeight);
    }

    $("." + popup).show();
    commonFunction.closeAllFilterExcept(popup);
    if (!event) {
        event = window.event;
        event.cancelBubble = true;
    }
    if (event.stopPropagation) {
        event.stopPropagation();
    }
}

function showDetailsPopup(element, event) {
    "use strict";
    (oGridConfig.isMatterView) ? $("#matterPopupLoading").removeClass("hide") : $("#documentPopupLoading").removeClass("hide");
    ////Commented this call for now, as this will be available post Document landing page changes
    ////oCommonObject.getDocumentLandingAssets(this);
    if ($(".filterSearchAdvance").css("display") === "block") {
        $(".filterSearchDD").toggle();
    }
    if (oGridConfig.isMatterView) {
        $(".matterPopupData").addClass("hide");
        showMatterDetailsPopup(element, event);
    } else {
        oCommonObject.getDocumentLandingAssets($(element).find(".nameColumn"));
        $(".documentPopupData").addClass("hide");
        showDocumentDetailsPopup(element, event);
    }
    placeDetailsPopup(element, event);
}

function showMatterDetailsPopup(element, event) {
    "use strict";
    var matterPopData = $(".matterPopupData");
    if (matterPopData.length) {
        var arrData = [],
                    matterName,
                    matterClientUrl,
                    matterClientName,
                    matterAreaOfLaw,
                    matterSubAreaOfLaw,
                    matterPracticeGroup,
                    matterOpenDate,
                    matterContentType,
                    readMore,
                    matterLandingPageUrl,
                    matterUrl,
                    matterClientID,
                    matterMatterID,
                    matterResponsibleAttorney,
                    matterOriginalName,
                    matterIsReadOnlyUser;
        var nRowIndex = $(element).parent()[0].rowIndex + (oGridConfig.nPinnedGridPageNumber * oGridConfig.itemsPerPage) - 1;

        if (1 === oGridConfig.currentView) {                // Pinned Matter view
            arrData = oGridConfig.arrPinnedData;
            //// Changes for URL consolidation
            matterName = (null !== arrData[nRowIndex].MatterGuid) ? oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].MatterGuid)) : oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].MatterName));
            matterClientName = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].MatterClient));
            matterClientUrl = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].MatterClientUrl));
            matterAreaOfLaw = oCommonObject.renderAsText(arrData[nRowIndex].MatterAreaOfLaw.slice(0, arrData[nRowIndex].MatterAreaOfLaw.trim().length - 1));
            matterSubAreaOfLaw = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].MatterSubAreaOfLaw));
            matterPracticeGroup = oCommonObject.renderAsText(arrData[nRowIndex].MatterPracticeGroup.slice(0, arrData[nRowIndex].MatterPracticeGroup.trim().length - 1));
            matterOpenDate = oCommonObject.renderAsText(formatDate(arrData[nRowIndex].MatterCreatedDate));
            matterContentType = oCommonObject.renderAsText(arrData[nRowIndex].MatterDescription);
            matterUrl = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].MatterUrl));
            matterClientID = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].MatterClientId.trim()));
            matterMatterID = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].MatterID.trim()));
            matterResponsibleAttorney = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].MatterResponsibleAttorney.trim()));
            //// Changes for URL consolidation
            matterOriginalName = arrData[nRowIndex].MatterName ? arrData[nRowIndex].MatterName.trim() : "NA";
            matterIsReadOnlyUser = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].HideUpload.trim()));
        } else {                                            // Recent Matter and All Matter view
            arrData = (oGridConfig.currentView) ? oGridConfig.arrRecentData : oGridConfig.arrGridData;
            //// Changes for URL consolidation
            matterName = (null !== arrData[nRowIndex][oGlobalConstants.Matter_GUID]) ? oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex][oGlobalConstants.Matter_GUID])) : oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex][oGlobalConstants.Matter_Name]));
            matterClientName = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex][oGlobalConstants.Client_Name]));
            matterClientUrl = oCommonObject.renderAsText(arrData[nRowIndex].SiteName);
            matterAreaOfLaw = oCommonObject.renderAsText(arrData[nRowIndex][oGlobalConstants.Area_Of_Law].slice(0, arrData[nRowIndex][oGlobalConstants.Area_Of_Law].trim().length - 1));
            matterSubAreaOfLaw = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex][oGlobalConstants.Sub_Area_Of_Law]));
            matterPracticeGroup = oCommonObject.renderAsText(arrData[nRowIndex][oGlobalConstants.Practice_Group].slice(0, arrData[nRowIndex][oGlobalConstants.Practice_Group].trim().length - 1));
            matterOpenDate = oCommonObject.renderAsText(formatDate(arrData[nRowIndex][oGlobalConstants.Open_Date]));
            matterContentType = oCommonObject.renderAsText(arrData[nRowIndex].Description);
            matterUrl = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].Path));
            matterClientID = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex][oGlobalConstants.Client_ID]));
            matterMatterID = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex][oGlobalConstants.Matter_ID]));
            matterResponsibleAttorney = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex][oGlobalConstants.Responsible_Attorney]));
            //// Changes for URL consolidation
            matterOriginalName = arrData[nRowIndex][oGlobalConstants.Matter_Name] ? arrData[nRowIndex][oGlobalConstants.Matter_Name].trim() : "NA";
            matterIsReadOnlyUser = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].IsReadOnlyUser.toString()));
        }
        readMore = matterClientUrl + "/" + matterName;
        // Trim semi colon from the end if it is present
        matterPracticeGroup = trimEndChar(matterPracticeGroup, ";");
        matterSubAreaOfLaw = trimEndChar(matterSubAreaOfLaw, ";");
        matterResponsibleAttorney = trimEndChar(matterResponsibleAttorney, ";");

        matterLandingPageUrl = readMore + "/" + matterName + oWebDashboardConstants.MatterLandingPageExtension;
        matterPopData.find(":nth-child(1)").attr({ "title": matterOriginalName, "data-mattername": matterName, "data-matterOriginalName": matterOriginalName, "data-client": matterClientUrl, "data-practicegroup": matterPracticeGroup, "data-areaoflaw": matterAreaOfLaw, "data-subareaoflaw": matterSubAreaOfLaw, "data-matterurl": readMore, "href": matterLandingPageUrl }).text(matterOriginalName);
        matterPopData.find(":nth-child(2)").attr({ "data-matterClientName": matterClientName }).html("<div class='fontWeight600 hideExtraContent contentInline' title='Client'>Client: </div><div class='matterRowValue hideExtraContent contentInline' title='" + matterClientName + "'>" + matterClientName + "</div>");
        matterPopData.find(":nth-child(3)").attr({ "data-matterClientID": matterClientID, "data-matterMatterID": matterMatterID }).html("<div class='fontWeight600 hideExtraContent contentInline' title='Client.Matter ID'>Client.Matter ID: </div><div class='matterRowValue hideExtraContent contentInline' title='" + matterClientID + oGlobalConstants.ClientID_MatterID_Separator + matterMatterID + "'>" + matterClientID + oGlobalConstants.ClientID_MatterID_Separator + matterMatterID + "</div>");
        matterPopData.find(":nth-child(4)").attr({ "data-matterAreaOfLaw": matterSubAreaOfLaw }).html("<div class='fontWeight600 hideExtraContent contentInline' title='Sub area of law'>Sub area of law: </div><div  title='" + matterSubAreaOfLaw + "' class='matterRowValue hideExtraContent contentInline'>" + matterSubAreaOfLaw + "</div>");
        matterPopData.find(":nth-child(5)").attr({ "data-matterResponsibleAttorney": matterResponsibleAttorney }).html("<div class='fontWeight600 hideExtraContent contentInline' title='Responsible Attorney'>Responsible Attorney: </div><div title='" + matterResponsibleAttorney + "' class='matterRowValue hideExtraContent contentInline'>" + matterResponsibleAttorney + "</div>");
        matterPopData.find("#matterLandingPage").html(oWebDashboardConstants.MatterDetailsAction);
        matterPopData.find("#uploadToMatter").attr({ "onclick": "uploadElement(" + nRowIndex + ", event);" }).html(oWebDashboardConstants.MatterUploadAction);
        if ("true" === matterIsReadOnlyUser) {
            matterPopData.find("#uploadToMatter").addClass("is-disabled").attr("disabled", "disabled");
        } else {
            matterPopData.find("#uploadToMatter").removeClass("is-disabled").removeAttr("disabled");
        }
        var sOneNoteUrl = matterClientUrl.replace(oGlobalConstants.Site_Url, "") + "/" + matterName + oGlobalConstants.OneNoteLibrary_Name_Suffix + "/" + matterName + "/" + matterName + oWebDashboardConstants.OneNoteTableOfContentsExtension + "$|$" + matterClientUrl.replace(oGlobalConstants.Site_Url, "") + "/" + matterName + "/" + matterName + "/" + matterName + oWebDashboardConstants.OneNoteTableOfContentsExtension
        , sMatterLandingPageUrl = matterClientUrl.replace(oGlobalConstants.Site_Url, "") + "/" + oGlobalConstants.Matter_Landing_Page_Repository + "/" + matterName + oWebDashboardConstants.MatterLandingPageExtension + "$|$" + matterClientUrl.replace(oGlobalConstants.Site_Url, "") + "/" + matterName + "/" + matterName + "/" + matterName + oWebDashboardConstants.MatterLandingPageExtension;
        // Check if the OneNote URL Exists or Not
        urlExists(matterClientUrl, matterName, sOneNoteUrl, sMatterLandingPageUrl, matterUrl);
        commonFunction.closeAllFilterExcept(".matterPopup");
        $("#matterLandingPage").off().on("click", function (event) {
            "use strict";
            var sMatterLink = $(this).attr("matter-link");
            if (sMatterLink) {
                window.open(sMatterLink, "_parent");
            }
        });
    }
}

function showDocumentDetailsPopup(element, event) {
    "use strict";
    var documentPopData = $(".documentPopupData");
    if (documentPopData.length) {
        var arrData = [],
                    documentName,
                    matterName,
                    documentClientUrl,
                    documentClientName,
                    documentID,
                    documentAuthor,
                    documentModifiedDate,
                    openDocument,
                    documentLandingPageUrl;
        var nRowIndex = $(element).parent()[0].rowIndex + (oGridConfig.nPinnedGridPageNumber * oGridConfig.itemsPerPage) - 1;

        if (1 === oGridConfig.currentView) {                            // Pinned Documents view
            arrData = oGridConfig.arrPinnedData;
            documentName = oCommonObject.renderStringContent(arrData[nRowIndex].DocumentName);
            matterName = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].DocumentMatter));
            documentClientUrl = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].DocumentClientUrl));
            documentClientName = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].DocumentClient));
            documentID = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex].DocumentID));
            documentAuthor = oCommonObject.renderStringContent(oCommonObject.renderAsText(trimEndChar($.trim(arrData[nRowIndex].DocumentOwner), ";")));
            documentModifiedDate = oCommonObject.renderStringContent(oCommonObject.renderAsText(formatDate(arrData[nRowIndex].DocumentModifiedDate)));
            openDocument = oCommonObject.renderAsText(arrData[nRowIndex].DocumentOWAUrl);
        } else {                                                        // Recent Documents and All Documents view
            arrData = (oGridConfig.currentView) ? oGridConfig.arrRecentData : oGridConfig.arrGridData;
            documentName = oCommonObject.renderStringContent(arrData[nRowIndex][oGlobalConstants.File_Name]);
            matterName = oCommonObject.renderStringContent((oCommonObject.renderAsText(arrData[nRowIndex][oGlobalConstants.Document_Matter_Name])));
            documentClientUrl = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex][oGlobalConstants.DocumentSPWebUrl]));
            documentClientName = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex][oGlobalConstants.Document_Client_Name]));
            documentID = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex][oGlobalConstants.Document_ID]));
            documentAuthor = oCommonObject.renderStringContent(oCommonObject.renderAsText(arrData[nRowIndex][oGlobalConstants.Author]));
            documentModifiedDate = oCommonObject.renderStringContent(oCommonObject.renderAsText(formatDate(arrData[nRowIndex][oGlobalConstants.Document_Last_Modified_Time])));
            openDocument = getDocumentUrl(arrData[nRowIndex]);
        }

        documentPopData.find(":nth-child(1)").attr({ "title": documentName, "data-mattername": matterName, "data-client": documentClientUrl, "href": documentLandingPageUrl }).text(documentName);
        documentPopData.find(":nth-child(2)").attr({ "data-documentClientName": documentClientUrl }).html("<div class='fontWeight600 hideExtraContent contentInline' title='Matter'>Matter: </div><div class='matterRowValue hideExtraContent contentInline' title='" + matterName + "'>" + matterName + "</div>");
        documentPopData.find(":nth-child(3)").attr({ "data-documentClientName": documentClientName }).html("<div class='fontWeight600 hideExtraContent contentInline' title='Client'>Client: </div><div class='matterRowValue hideExtraContent contentInline' title='" + documentClientName + "'>" + documentClientName + "</div>");
        documentPopData.find(":nth-child(4)").attr({ "data-documentID": documentID }).html("<div class='fontWeight600 hideExtraContent contentInline' title='Document ID'>Document ID: </div><div class='matterRowValue hideExtraContent contentInline' title='" + documentID + "'>" + documentID + "</div>");
        documentPopData.find(":nth-child(5)").attr({ "data-documentAuthor": documentAuthor }).html("<div class='fontWeight600 hideExtraContent contentInline' title='Author'>Author: </div><div class='matterRowValue hideExtraContent contentInline' title='" + documentAuthor + "'>" + documentAuthor + "</div>");
        documentPopData.find(":nth-child(6)").attr({ "data-documentModifiedDate": documentModifiedDate }).html("<div class='fontWeight600 hideExtraContent contentInline' title='Modified date'>Modified date: </div><div class='matterRowValue hideExtraContent contentInline' title='" + documentModifiedDate + "'>" + documentModifiedDate + "</div>");
        documentPopData.find("#openDocument").attr({ "data-link": openDocument, "onclick": "ViewMatterandGoToOneNote(event, oGlobalConstants.View_Document);" });
        documentPopData.find("#viewDocumentLandingPage").html(oWebDashboardConstants.DocumentDetailsAction);
        commonFunction.closeAllFilterExcept(".documentPopup");
        oCommonObject.updateDocumentLandingAttributes($(element).find(".nameColumn"));
    }
}

function placeMatterDetailsPopupTileView(element, event) {
    "use strict";
    var nAddTop = 0;
    var left = 0;
    var nWidth = getWidth();
    var iContentHeight = $(".LeftContent").height();
    var iExpectedHeight = $(element).offset().top + $(".matterPopup").height();
    if (703 >= nWidth || (703 < nWidth && 1199 >= nWidth)) {
        iExpectedHeight = iExpectedHeight + 95;
        oGridConfig.isTileView ? nAddTop = 75 : nAddTop = 50;
        oGridConfig.isTileView ? left = -35 : left = 0;
        if (703 >= nWidth) {
            oGridConfig.isTileView ? left = 10 : left = 0;
        }
        $(".matterPopup").css({ top: ($(element).offset().top + nAddTop), left: ($(element).offset().left + left) });
    }
    else {
        iExpectedHeight = iExpectedHeight + 10;
        left = 170;
        $(".matterPopup").css({ top: ($(element).offset().top - 118), left: ($(element).offset().left + left) });
    }

    if (("" !== oSearchGlobal.sClientName && "" !== oSearchGlobal.sClientSiteUrl) || oSearchGlobal.bIsTenantCall) {
        (iContentHeight < iExpectedHeight) ? updateIframeHeight(iExpectedHeight) : updateIframeHeight(iContentHeight);
    }

    /*---- For positioning fly out of matter-------------*/

    $("#matterPopupPointerPosition").removeClass("flyoutPositioningDefault flyoutPositioningForRightmost flyoutPositioningMobileView");
    if (1199 < nWidth) {
        positionFlyout(element, 0, 80, 100);
    } else if (703 < nWidth && 1199 >= nWidth) {
        positionFlyout(element, 40, 80, 90);
    } else {
        $("#matterPopupPointerPosition").addClass("flyoutPositioningMobileView matterPopupPointer").removeClass("matterPopupPointerFirst matterPopupPointerOffice");
        $(".matterPopup").css({ top: ($(element).offset().top + 60), left: ($(element).offset().left + $(element).width()) - 240 }).show();
    }

    $(".matterPopup").show();
    commonFunction.closeAllFilterExcept("matterPopup");
    if (!event) {
        event = window.event;
        event.cancelBubble = true;
    }
    if (event.stopPropagation) {
        event.stopPropagation();
    }
}

/* Returns the document URL. For documents supported by OWA, it returns OWA URL. Otherwise, the document path. */
function getDocumentUrl(oCurrentRow) {
    "use strict";
    var sDocumentPath, sDocumentExtension, sDocumentSPWebUrl;
    if (oCurrentRow) {
        sDocumentPath = trimEndChar(oCurrentRow.Path, "/");
        sDocumentExtension = oCurrentRow.FileExtension;
        sDocumentSPWebUrl = oCurrentRow.SPWebUrl;
        if (-1 < $.inArray(sDocumentExtension, oFilterGlobal.arrOWADocumentExtension) && sDocumentSPWebUrl) {
            sDocumentPath = commonFunction.getOWAUrl(sDocumentExtension, sDocumentSPWebUrl, sDocumentPath);
        }
    }
    return sDocumentPath;
}

function positionFlyout(element, contentDivOffsetLeft, leftOffsetForMatterPopupRightView, leftOffsetForMatterPopupLeftView) {
    "use strict";
    var sPointerPosition = null, sPointerPrevPostiton = null, nOffsetLeft, nOffsetTop;
    if (($("#contentDiv").width() - (($(element).offset().left) - ($("#contentDiv").offset().left) + ($(element).outerWidth())) + contentDivOffsetLeft) > ($(".matterPopup").width())) {
        // display right
        sPointerPosition = "flyoutPositioningDefault matterPopupPointerFirst";
        sPointerPrevPostiton = "matterPopupPointer matterPopupPointerOffice";
        nOffsetLeft = $(element).offset().left + $(element).width() - leftOffsetForMatterPopupRightView;
    } else {
        // left
        sPointerPosition = "flyoutPositioningForRightmost matterPopupPointer";
        sPointerPrevPostiton = "matterPopupPointerFirst matterPopupPointerOffice";
        nOffsetLeft = $(element).offset().left - $(element).width() - leftOffsetForMatterPopupLeftView;
    }
    nOffsetTop = $(element).offset().top - 15;
    $("#matterPopupPointerPosition").addClass(sPointerPosition).removeClass(sPointerPrevPostiton);
    $(".matterPopup").css({ top: nOffsetTop, left: nOffsetLeft }).show();
}

/**************************Advance Search****************************/

/* Search based on refiners in search box.*/
function getSortData() {
    "use strict";
    var oSortDetails = { ByProperty: "", Direction: 0 };

    var oSelectedDropdown = ".sdBannerText";
    oSortDetails.ByProperty = $(oSelectedDropdown).attr("data-sort-value");
    if ($(oSelectedDropdown).length) {
        oSortDetails.Direction = parseInt($(oSelectedDropdown).attr("data-order"), 10);
        oSearchGlobal.current_sorted_data_value = $(oSelectedDropdown).attr("data-value");
    }
    return oSortDetails;
}

function checkEmptyorWhitespace(input) {
    "use strict";
    if (/\S/.test(input)) {
        return input;
    }
    return oWebDashboardConstants.No_Data_String;
}

/*********************************AUTO COMPLETE FUNCTIONALITY**********************************/
$(document).on("click", ".filterAutoComplete", function (event) {
    "use strict";
    event.stopPropagation();
});

$(document).click(function (event) {
    "use strict";
    if ($(".filterSearchAdvance").css("display") === "block" && (event && event.target && event.target.className !== "ui-corner-all")) {
        $(".filterSearchDD").toggle();
    }
    if ("sdBannerDropdown" === event.target.className || "sdBannerText" === event.target.className || "sdBannerDD" === event.target.className) {
        closeAllPopupExcept("sdBannerPanel", event);
    }
    else {
        closeAllPopupExcept("", event);
    }
    $(".switchTab").css("display", "none");
    $(".quickLinksMenuItems").css("display", "none");
    event.stopPropagation();
});

function autoCompleteFailure(result) {
    "use strict";
    return false;
}

function autoComplete() {
    "use strict";
    var sDeployUrl = oCommonObject.getDeployedUrl();
    if ($(".filterSearchAdvance").is(":hidden")) {
        if (oGridConfig.isMatterView) {
            var sTerm = $(".filterSearchText").val(), sSearchTerm = sTerm && sTerm !== "" ? sTerm : "", FilterDetails, SearchDetails;
            if (sSearchTerm) {
                sSearchTerm = "(" + oGlobalConstants.Matter_Name + ":" + sSearchTerm + "* OR " + oGlobalConstants.Matter_ID + ":" + sSearchTerm + "*)";
                FilterDetails = oCommonObject.getSearchData();
                FilterDetails.ClientsList = selectCurrentClient(FilterDetails, sDeployUrl);
                SearchDetails = { "requestObject": { "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": sDeployUrl }, "searchObject": { "PageNumber": "1", "ItemsPerPage": "5", "SearchTerm": sSearchTerm, "Filters": FilterDetails } };
                oCommonObject.callSearchService("FindMatter", SearchDetails, autoCompleteSuccess, autoCompleteFailure);
            } else {
                $(".filterAutoComplete").addClass("hide").html("");
            }

        } else {
            var sSearchTerm = $(".filterSearchText").val() && $(".filterSearchText").val() !== "" ? $(".filterSearchText").val() : "";
            if (sSearchTerm) {
                sSearchTerm = "(FileName:" + sSearchTerm + "* OR " + oGlobalConstants.Document_ID + ":" + sSearchTerm + "*)";
                var FilterDetails = oCommonObject.getSearchData();
                FilterDetails.ClientsList = selectCurrentClient(FilterDetails, sDeployUrl);
                var SortDetails = getSortData();
                var SearchDetails = { "requestObject": { "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": sDeployUrl }, "searchObject": { "PageNumber": 1, "ItemsPerPage": "5", "SearchTerm": sSearchTerm, "Filters": FilterDetails, "Sort": SortDetails } };

                oCommonObject.callSearchService("FindDocument", SearchDetails, autoCompleteSuccess, autoCompleteFailure);
            } else {
                $(".filterAutoComplete").addClass("hide").html("");
            }
        }

    }
}

function selectCurrentClient(oFilterDetails, sDeployUrl) {
    "use strict";
    if ("" !== oSearchGlobal.sClientName) {
        if ("" === oFilterDetails.ClientsList[0]) {
            oFilterDetails.ClientsList = [];
        }
        oFilterDetails.ClientsList.push(oSearchGlobal.sClientSiteUrl);
    }
    return oFilterDetails.ClientsList;
}

function autoCompleteSuccess(result) {
    "use strict";
    if (!oCommonObject.bHideAutoComplete) {
        var splitResults, jsonResponse;
        jsonResponse = {
        };
        if (-1 !== result.indexOf("$|$")) {
            splitResults = result.split("$|$");
            if (splitResults[1] && parseInt(splitResults[1], 10) > 0) {
                jsonResponse = JSON.parse(splitResults[0]);
            }
        }
        if (jsonResponse && jsonResponse.length) {
            var sAssignedToMe, sTerm = $(".filterSearchText").val();
            commonFunction.closeAllFilterExcept("filterAutoComplete");
            if (oGridConfig.isMatterView) {
                var autoContainer = "<div id='autoComplete'>", autoCompleteItemContainer = "<div class='autoCompleteRow' data-searchterm='#SearchTerm#'>", regularSearchTerm = new RegExp("#SearchTerm#", "g"), lineItem, matterName, autoCompleteItemContainerClose = "#SearchTermClose#</div>", regularSearchTermClose = new RegExp("#SearchTermClose#", "g");
                for (lineItem in jsonResponse) {
                    if (jsonResponse.hasOwnProperty(lineItem)) {
                        matterName = jsonResponse[lineItem][oGlobalConstants.Matter_Name];
                        if (matterName) {
                            autoContainer = autoContainer + autoCompleteItemContainer.replace(regularSearchTerm, matterName + " (" + jsonResponse[lineItem][oGlobalConstants.Matter_ID] + ")") + autoCompleteItemContainerClose.replace(regularSearchTermClose, oCommonObject.highlightTerm(matterName + " (" + jsonResponse[lineItem][oGlobalConstants.Matter_ID] + ")", sTerm));
                        }
                    }
                }
                sAssignedToMe = oWebDashboardConstants.Search_Matters_Assigned_To_Me;
            } else {
                var oResponseData = JSON.parse(splitResults[0]);
                var autoContainer = "<div id='autoComplete'>";
                var autoCompleteItemContainer = "<div class='autoCompleteRow' data-searchterm='#SearchTerm#'>";
                var regularSearchTerm = new RegExp("#SearchTerm#", "g");
                var autoCompleteItemContainerClose = "#SearchTermClose#</div>",
                    regularSearchTermClose = new RegExp("#SearchTermClose#", "g");
                for (lineItem in oResponseData) {
                    var matterName = oCommonObject.ExtractFileTitle(oResponseData[lineItem][oGlobalConstants.File_Name]);
                    var docId = oResponseData[lineItem][oGlobalConstants.Document_ID];
                    if (0 < $.trim(docId).length) {
                        autoContainer = autoContainer + autoCompleteItemContainer.replace(regularSearchTerm, matterName + " (" + docId + ")") + autoCompleteItemContainerClose.replace(regularSearchTermClose, oCommonObject.highlightTerm(matterName + " (" + docId + ")", sTerm));
                    } else {
                        autoContainer = autoContainer + autoCompleteItemContainer.replace(regularSearchTerm, matterName) + autoCompleteItemContainerClose.replace(regularSearchTermClose, oCommonObject.highlightTerm(matterName, sTerm));
                    }
                }
                sAssignedToMe = oWebDashboardConstants.Search_Assigned_To_Me;
            }
            $(".filterAutoComplete").removeClass("hide").html(autoContainer);
            advanceSearchDropdown();
        }
        else {
            $(".filterAutoComplete").addClass("hide");
        }
    }
}

// Binding on onfocus event on search text box
$(document).on("click", ".autoCompleteRow", function () {
    "use strict";
    $(".autoCompleteRow").removeClass("autoCompleteSelected");
    $(this).addClass("autoCompleteSelected");

    var sSelectedContent = $(this).text();
    // Removal of unwanted HTML Tags
    if (sSelectedContent && "" !== sSelectedContent) {
        sSelectedContent = sSelectedContent.replace(/"<span class=\"highlightTerm\">"/g, "").replace(/"<\/span>"/g, "");
    }
    $(".filterSearchText").val(sSelectedContent);

    $(".filterAutoComplete").addClass("hide");
    advancedSearch("grid", "tileContainer");
    if (!oGridConfig.isMatterView) {
        $("#results").click();
    }
});

// Sort the grid
function sortPinDocument(container, sortByfield) {
    "use strict";
    if (sortByfield.length) {
        var headerobject = document.querySelector(".jsonGridHeader");
        if (headerobject) {
            var sSortOrder = parseInt($(".sdBannerText").attr("data-order"), 10) ? "desc" : "asc";
            headerobject.setAttribute("sortorder", sSortOrder);
            oGrid.sortJsonGrid(headerobject, container, sortByfield);
            headerobject.setAttribute("sortorder", "asc");
            sortPinnedArray(sSortOrder, sortByfield);
        }
    } else {
        onPinnedItemSuccess(oSearchGlobal.pinnedDocumentDataPassed, event);
    }
}

function setGlobalRefiners() {
    "use strict";
    oSearchGlobal.oFilterData = oCommonObject.getSearchData();
    var filterSearchText = $(".filterSearchText").val();
    oSearchGlobal.sSearchTerm = filterSearchText && filterSearchText !== "" ? filterSearchText : "";
}

commonFunction.switchThisApp = function (sSwitchAppName) {
    "use strict";
    if ("Search Documents" === sSwitchAppName) {
        $("#searchMatter").removeClass("selected");
        $("#searchDocument").addClass("selected");
    } else {
        $("#searchMatter").addClass("selected");
        $("#searchDocument").removeClass("selected");
    }
    $(".popupWait, #popupWaitLoadingImage, .loadingImage").addClass("hide");
};

$(window).on("resize", function (event) {
    "use strict";
    var nCurrentBrowserWidth = getWidth();
    if (oGridConfig.hasOwnProperty("prevBrowserWidth") && oGridConfig.prevBrowserWidth !== nCurrentBrowserWidth) {
        createResponsiveGrid();
        $(".matterPopup").hide();
        $(".documentPopup").hide();
        $(".switchApp").hide();
        $(".ui-datepicker").hide();
        $(".searchDocumentHeader .switchTab").html("").hide();
        $(".sdBannerPanel").addClass("hide");
    }
    if (850 < nCurrentBrowserWidth) {
        $(".mySearch, .pinnedSearch, .allSearch").show();
        $(".viewSwitcher").hide();
    } else {
        $(".mySearch, .pinnedSearch, .allSearch").not(".active").hide();
        $(".viewSwitcher").show();
    }
    if ($("#headerFlyout").is(":visible")) {
        $("#headerFlyout").css("width", "100%").css("width", "-=40px");
    }
});

function getWidth() {
    "use strict";
    var nWidth = 0;
    if (self.innerHeight) {
        nWidth = self.innerWidth;
    } else if (document.documentElement && document.documentElement.clientHeight) {
        nWidth = document.documentElement.clientWidth;
    } else if (document.body) {
        nWidth = document.body.clientWidth;
    }
    return nWidth;
}

function createResponsiveGrid() {
    "use strict";
    var isChrome = !!window.chrome;
    var isSafari = 0 < Object.prototype.toString.call(window.HTMLElement).indexOf("Constructor");
    var sSelectedTab = getCurrentSelectedTab();
    var arrGridHeader = $(sSelectedTab + " .jsonGridHeader");
    closeAllPopupExcept(""); //// Close all pop-ups
    var nCurrentWindowWidth = oCommonObject.getWidth();
    var arrGridWidthValues = oGridConfig.isMatterView ? oWebDashboardConstants.MatterColumnWidth.split(";") : oWebDashboardConstants.DocumentColumnWidth.split(";");
    //// Get last visible header and first hidden header, for responsiveness
    var lastHeader = getLastVisibleColumnHeader(arrGridHeader), firstHiddenHeader = $(arrGridHeader).filter(".hide").first();
    var lastHeaderIndex = lastHeader.index(), firstHiddenHeaderIndex = firstHiddenHeader.index();
    //// Get list of cells present in the last visible column, for responsiveness
    var oVisibleCellSet = $(".GridRow .jsonGridRow:nth-child(" + parseInt(lastHeaderIndex + 1) + ")"), oVisbleAlternateCellSet = $(".GridRowAlternate .jsonGridRow:nth-child(" + parseInt(lastHeaderIndex + 1) + ")");
    //// Get list of cells present in the first hidden column, for responsiveness
    var oHiddenCellSet = $(".GridRow .jsonGridRow:nth-child(" + parseInt(firstHiddenHeaderIndex + 1) + ")"), oHiddenAlternateCellSet = $(".GridRowAlternate .jsonGridRow:nth-child(" + parseInt(firstHiddenHeaderIndex + 1) + ")");
    var iMinHeaders = oGridConfig.isMatterView ? 1 : 3; // These are (minimum number of columns to show -1) for matters and documents view respectively
    if (((-1 < lastHeaderIndex) && (nCurrentWindowWidth < oGridConfig.nCurrentGridWidth) && (671 < nCurrentWindowWidth)) || ((iMinHeaders < lastHeaderIndex) && (nCurrentWindowWidth < oGridConfig.nCurrentGridWidth))) {  // As width for header and footer is fixed after window width of 671px, blocking responsive behavior after this point so as to avoid white space Or, when page is loaded first time having width <671 Or tabs are switched having width <671
        oGridConfig.nPreviousGridWidth = oGridConfig.nCurrentGridWidth;
        oGridConfig.nCurrentGridWidth -= arrGridWidthValues[lastHeaderIndex];
        oGridConfig.nGridWidth -= arrGridWidthValues[lastHeaderIndex];
        var sSelectedTab = getCurrentSelectedTab();
        $(sSelectedTab).css("maxWidth", oGridConfig.nGridWidth);
        $(sSelectedTab).css("width", 0);
        lastHeader.addClass("hide");
        oVisibleCellSet.addClass("hide");
        oVisbleAlternateCellSet.addClass("hide");
        $(window).trigger("resize");
    } else if ((-1 < firstHiddenHeaderIndex) && (nCurrentWindowWidth > oGridConfig.nPreviousGridWidth) && (oGridConfig.nCurrentGridWidth !== oGridConfig.nPreviousGridWidth)) {
        oGridConfig.nPreviousGridWidth = oGridConfig.nCurrentGridWidth;
        oGridConfig.nCurrentGridWidth += parseInt(arrGridWidthValues[firstHiddenHeaderIndex]);
        oGridConfig.nGridWidth += parseInt(arrGridWidthValues[firstHiddenHeaderIndex]);
        var sSelectedTab = getCurrentSelectedTab();
        $(sSelectedTab).css("maxWidth", oGridConfig.nGridWidth);
        $(sSelectedTab).css("width", 0);
        firstHiddenHeader.removeClass("hide") && oHiddenCellSet.removeClass("hide") && oHiddenAlternateCellSet.removeClass("hide");
        $(window).trigger("resize");
    }
}

// Function to get the last visible column header
function getLastVisibleColumnHeader(arrGridHeader) {
    "use strict";
    var lastVisibleHeader = arrGridHeader.not(".mandatory, .hide").last();
    if (lastVisibleHeader.length && (-1 < oWebDashboardConstants.NonDataColumns.indexOf(lastVisibleHeader[0].id))) {
        lastVisibleHeader = getLastVisibleColumnHeader(lastVisibleHeader.prev());
    }
    return lastVisibleHeader;
};

// Function to get the initial width of the grid view control
function getGridViewWidth() {
    "use strict";
    var arrGridWidthValues = oGridConfig.isMatterView ? oWebDashboardConstants.MatterColumnWidth.split(";") : oWebDashboardConstants.DocumentColumnWidth.split(";");
    oGridConfig.nGridWidth = arrGridWidthValues.reduce(function (previousValue, currentValue) { return parseInt(previousValue) + parseInt(currentValue); });
    oGridConfig.nCurrentGridWidth = oGridConfig.nPreviousGridWidth = oGridConfig.nGridWidth + oGridConfig.nWidthDiff;
};

// Function to get the selected tab
function getCurrentSelectedTab() {
    "use strict";
    var sSelectedTab;
    if (1 === oGridConfig.currentView) {            // Pinned View
        sSelectedTab = "#pinnedGrid_Grid";
    } else if (2 === oGridConfig.currentView) {     // Recent View
        sSelectedTab = (oGridConfig.isMatterView) ? "#recentMatters_Grid" : "#RecentDocumentContainer_Grid";
    } else {                                        // Search View
        sSelectedTab = "#grid_Grid";
    }
    return sSelectedTab;
}

// Function is used to retrieve parameter from the URL
function getParameterByName(name) {
    "use strict";
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function onURLExistSuccess(result) {
    "use strict";
    // Handle the true and false responses for the Matter Landing page and OneNote TOC
    if (result && result.Result) {
        var oSplitResults = result.Result.split("$|$");
        if (2 === oSplitResults.length) {
            var oOneNoteResult = oSplitResults[0].split("$#$");
            var oMatterLandingPageResult = oSplitResults[1].split("$#$");
            var anchorElement = document.createElement("a");
            anchorElement.href = result.oParam.sitename;
            if (oOneNoteResult && 2 === oOneNoteResult.length) {
                if ("true" === oOneNoteResult[0]) {
                    var oOneNoteLink = $("#OneNoteClick"), oOneNoteIcon = $("#OneNoteClick img");
                    if (oOneNoteLink.length && oOneNoteIcon.length) {
                        // Set the link to open the Matter's OneNote page in a new browser window
                        oOneNoteLink.attr({ "href": result.oParam.sitename + "/" + oWebDashboardConstants.WOPIFrameURL + anchorElement.protocol + "//" + anchorElement.hostname + oOneNoteResult[1], "target": "_parent", "class": "matterTitle applyCursor" });
                        oOneNoteIcon.attr({ "src": "../Images/one-note_666_29X29.png", "alt": "upload", "style": "" });
                    }
                }
            }
            $("#matterPopupLoading, #documentPopupLoading").addClass("hide");
            var oMatterPopData = $(".matterPopupData"), sMatterPageLink;
            // Set the link to the Matter Landing page or to the All Items page based on the value returned
            if (oMatterLandingPageResult && 2 === oMatterLandingPageResult.length) {
                sMatterPageLink = "true" === oMatterLandingPageResult[0] ? anchorElement.protocol + "//" + anchorElement.hostname + oMatterLandingPageResult[1] : result.oParam.matterLibraryURL;
            } else {
                sMatterPageLink = result.oParam.matterLibraryURL;
            }
            if (oMatterPopData[0]) {
                oMatterPopData.find("#matterLandingPage").attr({ "title": oWebDashboardConstants.MatterDetailsAction, "onclick": "ViewMatterandGoToOneNote(event, oGlobalConstants.View_Matter);", "matter-link": sMatterPageLink });
            }
            // Finally show the data
            oMatterPopData.removeClass("hide");
        } else {
            onURLExistFailure(result);
        }
    } else {
        onURLExistFailure(result);
    }
}

function onURLExistFailure(result) {
    "use strict";
    // Do not display the OneNote link
    $("#matterPopupLoading, #documentPopupLoading").addClass("hide");
    if (result && result.oParam && result.oParam.matterLibraryURL) {
        var oMatterPopData = $(".matterPopupData"),
            sMatterPageLink = result.oParam.matterLibraryURL;
        if (oMatterPopData[0]) {
            oMatterPopData.find("a.readMoreLink").attr({ "href": sMatterPageLink, "target": "_blank", "onclick": "ViewMatterandGoToOneNote(event, oGlobalConstants.View_Matter);" });
            oMatterPopData.find("a:nth-child(1)").attr({ "href": sMatterPageLink, "target": "_blank", "onclick": "ViewMatterandGoToOneNote(event, oGlobalConstants.View_Matter);" });
        }
    }
    // Finally show the data
    $(".matterPopupData").removeClass("hide");
}

// Function to check whether the OneNote exists
function urlExists(sClientURL, sMatterName, sOneNoteURL, sMatterLandingPageURL, sMatterLibraryURL) {
    "use strict";
    var sParameters = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": sClientURL }, requestedUrl: sOneNoteURL, requestedPageUrl: sMatterLandingPageURL }, oParam = { "sitename": sClientURL, "matterName": sMatterName, "matterLibraryURL": sMatterLibraryURL };
    oCommonObject.callSearchService("UrlExists", sParameters, onURLExistSuccess, onURLExistFailure, null, oParam);
}

// Returns the name of the function
function getFunctionName(oFunctionToBeProcressed) {
    "use strict";
    var ret = oFunctionToBeProcressed.toString();
    ret = ret.substr("function ".length);
    ret = ret.substr(0, ret.indexOf("("));
    return ret.trim();
}

// Set default sorting value to relevant
function updateSortSettings(event) {
    "use strict";
    var sortText, $Element = oGridConfig.isMatterView ? $("#sdBannerDropdownForMatters #optionNoneForMatters") : $("#sdBannerDropdownForDocuments #optionNoneForDocuments");

    $(".sdBannerText").attr("data-result", "1");
    $Element.attr("data-sort-value", "").attr("data-value", "").text(oGlobalConstants.Sort_Type_Relevant);
    sortText = $Element.text();
    var sortTextWidth = sortText.length > 5 ? sortText.length < 9 ? 56 : (sortText.length - 2) * 8 : 35;
    $(".sdBannerText").val(sortText).css("width", sortTextWidth + "px").attr({
        "data-sort-value": $Element.attr("data-sort-value"),
        "data-order": $Element.attr("data-order"),
        "data-value": $Element.attr("data-value"),
        "data-ss-value": $Element.attr("data-ss-value")
    });
}

// Sets the selected tab to active
function setActiveTab() {
    "use strict";
    if (2 === oGridConfig.currentView) { // Recent view
        $(".pinnedSearch .allSearch").removeClass("active");
        $(".mySearch").addClass("active");
    } else if (1 === oGridConfig.currentView) { // Pinned View
        $(".mySearch .allSearch").removeClass("active");
        $(".pinnedSearch").addClass("active");
    } else { // Search Results
        $(".pinnedSearch .mySearch").removeClass("active");
        $(".allSearch").addClass("active");
    }
}

// Updates the dropdown options
function updateDropdownOptions(option1, option2) {
    "use strict";
    setActiveTab();
    return "<div class='navDiv' onclick='changeNavigationTabs(this);'>" + option1 + "</div>" +
           "<div class='navDiv' onclick='changeNavigationTabs(this);'>" + option2 + "</div>";
}

$(document).on("click", ".viewSwitcher", function (event) {
    "use strict";
    var oHtmlDropDown = "", iTabPosition;
    var nWindowWidth = getWidth();
    if (1199 >= nWindowWidth) {
        if (2 === oGridConfig.currentView) { // Recent view
            oHtmlDropDown = oGridConfig.isMatterView ? updateDropdownOptions(arrOriginalTabOrder[1], arrOriginalTabOrder[2]) : updateDropdownOptions(arrOriginalTabOrder[4], arrOriginalTabOrder[5]);
        } else if (1 === oGridConfig.currentView) { // Pinned View
            oHtmlDropDown = oGridConfig.isMatterView ? updateDropdownOptions(arrOriginalTabOrder[0], arrOriginalTabOrder[1]) : updateDropdownOptions(arrOriginalTabOrder[3], arrOriginalTabOrder[4]);
        } else { // Search Results
            oHtmlDropDown = oGridConfig.isMatterView ? updateDropdownOptions(arrOriginalTabOrder[0], arrOriginalTabOrder[2]) : updateDropdownOptions(arrOriginalTabOrder[3], arrOriginalTabOrder[5]);
        }
        iTabPosition = parseInt($(".fallbackFlex .active").width(), 10) + 15;

        $(".searchDocumentHeader .switchTab").html("").append(oHtmlDropDown);

        $(".switchTab").css("left", parseInt(iTabPosition, 10)).toggle();
    }
    closeAllPopupExcept("navDiv", event);
    event.stopPropagation();
});

var arrOriginalTabOrder = [
    "My matters",
    "All matters",
    "Pinned matters",
    "My documents",
    "All documents",
    "Pinned documents",
];

function changeNavigationTabs(oElement) {
    "use strict";
    var oHeadingElements = $(".switchAppRow");
    var oHtmlDropDown = "";
    if (oGridConfig.isMatterView) {
        if (2 <= oHeadingElements.length) {
            if ($(oElement).text() === arrOriginalTabOrder[2]) { // that is Pinned Matters
                oGridConfig.currentView = 1;
                $(".pinnedSearch").show();
                $(".allSearch, .mySearch").hide();
                oHtmlDropDown = updateDropdownOptions(arrOriginalTabOrder[0], arrOriginalTabOrder[1]);
                if ("navDiv" !== oElement) {
                    togglePinnedView("#DisplayFindMattersResults .pinnedSearch", 1);
                }
            } else if ($(oElement).text() === arrOriginalTabOrder[0]) { // that is My Matters
                oGridConfig.currentView = 2;
                $(".mySearch").show();
                $(".allSearch, .pinnedSearch").hide();
                oHtmlDropDown = updateDropdownOptions(arrOriginalTabOrder[1], arrOriginalTabOrder[2]);
                if ("navDiv" !== oElement) {
                    togglePinnedView("#DisplayFindMattersResults .mySearch", 2);
                }
            } else { // that is All Matters
                oGridConfig.currentView = 0;
                $(".allSearch").show();
                $(".pinnedSearch, .mySearch").hide();
                oHtmlDropDown = updateDropdownOptions(arrOriginalTabOrder[0], arrOriginalTabOrder[2]);
                if ("navDiv" !== oElement) {
                    togglePinnedView("#DisplayFindMattersResults .allSearch", 0);
                }
            }
        }
    } else {
        if (3 <= oHeadingElements.length) {
            var oClientsTab = oHeadingElements[1];
            var oRecentMattersTab = oHeadingElements[0];
            var iPopupLeftPosition;

            if ($(oElement).text() === arrOriginalTabOrder[5]) { // that is Pinned documents
                oGridConfig.currentView = 1;
                $(".pinnedSearch").show();
                $(".allSearch, .mySearch").hide();
                oHtmlDropDown = updateDropdownOptions(arrOriginalTabOrder[3], arrOriginalTabOrder[4]);
                if ("navDiv" !== oElement) {
                    togglePinnedView("#DisplaySearchDocumentsResults .pinnedSearch", 1);
                }
            } else if ($(oElement).text() === arrOriginalTabOrder[3]) { // that is My documents
                oGridConfig.currentView = 2;
                $(".mySearch").show();
                $(".allSearch, .pinnedSearch").hide();
                oHtmlDropDown = updateDropdownOptions(arrOriginalTabOrder[4], arrOriginalTabOrder[5]);
                if ("navDiv" !== oElement) {
                    togglePinnedView("#DisplaySearchDocumentsResults .mySearch", 2);
                }

            } else { // that is All documents
                oGridConfig.currentView = 0;
                $(".allSearch").show();
                $(".pinnedSearch, .mySearch").hide();
                oHtmlDropDown = updateDropdownOptions(arrOriginalTabOrder[3], arrOriginalTabOrder[5]);
                if ("navDiv" !== oElement) {
                    togglePinnedView("#DisplaySearchDocumentsResults .allSearch", 0);
                }
            }
            $(".searchDocumentHeader .switchTab").html("").append(oHtmlDropDown);
        }
    }
}

// Function to perform operation before search result.
function beforeSearch(oFinalResult) {
    "use strict";
    var container = oFinalResult.oParam.container;
    if (oGridConfig.isMatterView) {
        $("#recentMatters").show();
    } else {
        // hide pinned grid and search results
        $("#RecentDocumentContainer").show();
    }
    $("#pinnedGrid, #grid").hide();
    var sGridName = oGridConfig.isMatterView ? "Matters" : "Documents";
    $("#" + container).html("<div class=\"centreContent\"><br /><img src=\"/Images/WindowsLoadingFast.GIF\" title=\"Loading\" alt=\"Loading...\" /><br /><br /> <div>Please wait while the " + sGridName + " load.</div><br /></div>");
}

// Function to count items to be displayed
function itemsCountsToBeDisplayed() {
    "use strict";
    var nWindowWidth = getWidth();
    oGridConfig.itemsPerPage = oWebDashboardConstants.ItemsPerPage;
}

// Function to sort pinned array
function sortPinnedArray(sSortOrder, sortByfield) {
    "use strict";
    oGridConfig.arrPinnedData.sort(function (oPrevious, oNext) {
        if ("asc" === sSortOrder) {
            return (oPrevious[sortByfield] > oNext[sortByfield]) ? 1 : ((oPrevious[sortByfield] < oNext[sortByfield]) ? -1 : 0);
        } else {
            return (oNext[sortByfield] > oPrevious[sortByfield]) ? 1 : ((oNext[sortByfield] < oPrevious[sortByfield]) ? -1 : 0);
        }
    });
}

