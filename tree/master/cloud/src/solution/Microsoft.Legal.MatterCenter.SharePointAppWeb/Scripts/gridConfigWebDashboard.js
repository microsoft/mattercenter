/// <disable>JS3057,JS3058,JS2074,JS2076,JS3092,JS3054,JS2005,JS3056,JS2073,JS2024,JS2026,JS2032,JS3116,JS3053</disable>
// Function to replace to default error image.
function errorImage(image) {
    "use strict";
    if ($(image).attr("data-error")) {
        $(image).attr("src", oGlobalConstants.Image_General_Document).attr("data-error", "true");
    } else {
        $(image).attr("src", $(image).attr("src").substring(0, $(image).attr("src").lastIndexOf(".") + 1) + "png").attr("data-error", "true");
    }
}

function toggleView(isMatterView) {
    "use strict";
    oGridConfig.isTileView = false;
    oGridConfig.bPageLoad = true;
    oGridConfig.isMatterView = isMatterView;
    updateQueryString && updateQueryString();
    oGridConfig.nGridPageNumber = 1;
    advanceSearchDropdown();
    // Matter Count and Sort Filter
    var sMatterResultDisplayValue = isMatterView ? "fallbackFlex" : "hide";
    var sMatterResultSortValue = isMatterView ? "inline" : "none";
    // Document Count and Sort Filter
    var sDocumentResultDisplayValue = isMatterView ? "hide" : "fallbackFlex";
    // Display the Mail cart and Separator
    var sDisplayValue = isMatterView ? "none" : "inline";
    // Show or hide based on the dropdown value
    $("#DisplayFindMattersResults").attr("class", sMatterResultDisplayValue);
    $("#DisplaySearchDocumentsResults").attr("class", sDocumentResultDisplayValue);
    $("#EmailDiv").css("display", sDisplayValue);
    $(".separator").css("display", sDisplayValue);
    // Remove the previous Active tab
    $(".active").removeClass("active");
    // Set the Active Tab
    $("#DisplayFindMattersResults .pinnedSearch").addClass("active");
    $("#DisplayFindMattersResults .pinnedSearch .ResultNumbers, #DisplayFindMattersResults .mySearch .ResultNumbers, #DisplayFindMattersResults .allSearch .ResultNumbers").html(" (0)");
    $("#DisplaySearchDocumentsResults .pinnedSearch").addClass("active");
    $("#DisplaySearchDocumentsResults .pinnedSearch .ResultNumbers,#DisplaySearchDocumentsResults .mySearch .ResultNumbers, #DisplaySearchDocumentsResults .allSearch .ResultNumbers").html(" (0)");
    // Hide the pinned matter section and show the search results tab
    var oGridContainer = $("#grid");
    var oPinnedContainer = $("#pinnedGrid");
    if (oGridContainer[0] && oPinnedContainer[0]) {
        $("#pinnedGrid").html("");
        $("#grid").html("");
        oGridContainer.hide();
        oPinnedContainer.show();
    }
    // Clear filter panel
    clearFilterPanel();
    $(".filterSearchText").val("");
    advancedSearch();
    var sHeaderID = oGridConfig.isMatterView ? "DisplayFindMattersResults" : "DisplaySearchDocumentsResults";
    togglePinnedView($("#" + sHeaderID + " .pinnedSearch"), 1);			// Calling this to get pinned data and update count
    togglePinnedView($("#" + sHeaderID + " .mySearch"), 2);             // Calling this to get recent data and update count
    isMatterView ? $(".filterSearchText").attr({ "placeholder": "Search by matter name, matter ID, or keyword", "title": "Search by matter name, matter ID, or keyword" }) : $(".filterSearchText").attr({ "placeholder": "Search by document name, document ID, or keyword", "title": "Search by document name, document ID, or keyword" });
    // hide Practice group and Area of law in Search Document view.
    if (oGridConfig.isMatterView) {
        $(".filterSearchPG, .filterSearchAOL").removeClass("hide");
        $(".filterSearchAuthor").addClass("hide");
        $(".selected").removeClass("selected");
        $("#searchMatterTab").addClass("selected");
    } else {
        $(".filterSearchPG, .filterSearchAOL").addClass("hide");
        $(".filterSearchAuthor").removeClass("hide");
        $(".selected").removeClass("selected");
        $("#searchDocumentTab").addClass("selected");
        $("#footer").addClass("documentFooter");
    }
    oCommonObject.bindAutocomplete("#txtDocumentAuthor", true);
    $(document).on("keypress, input", "#txtDocumentAuthor", function () {
        $(this).removeAttr("data-resolved");
    });
    $(".sdBannerText").attr("data-result", "2");				// Set Recent as default view
    $("#optionNoneForDocuments, #optionNoneForMatters").attr("data-sort-value", "").attr("data-value", "").text(oGlobalConstants.Sort_Type_None);
    $("#sdBannerDropdownForDocuments, #sdBannerDropdownForMatters").addClass("hide");
    if (isMatterView) {
        $(".extraTabHolder").removeClass("searchResults, pinnedDocuments");
        $(".extraTabHolder").addClass("pinnedMatters");
        $(".navDiv").removeClass("divPinnedDocuments, divSearchResults");
        $(".navDiv").addClass("divPinnedMatters");
    } else {
        $(".extraTabHolder").removeClass("searchResults, pinnedMatters");
        $(".extraTabHolder").addClass("pinnedDocuments");
        $(".navDiv").removeClass("divPinnedMatters, divSearchResults");
        $(".navDiv").addClass("divPinnedDocuments");
    }
}

function togglePinnedView(oCurrentTab, currentView) {
    "use strict";
    oGridConfig.currentView = currentView;
    oGridConfig.nGridPageNumber = 1;
    if (2 !== oGridConfig.currentView) {
        $("#optionNoneForDocuments").show();
        var oGridContainer = $("#grid");
        var oPinnedContainer = $("#pinnedGrid");
        var oRecentDocumentContainer = $("#RecentDocumentContainer");
        var oRecentMatterContainer = $("#recentMatters");
        if (oGridContainer[0] && oPinnedContainer[0]) {
            var sortText, $Element = oGridConfig.isMatterView ? $("#sdBannerDropdownForMatters #optionNoneForMatters") : $("#sdBannerDropdownForDocuments #optionNoneForDocuments");
            if (oGridConfig.currentView) {
                // pinned view
                oGridContainer.hide();
                oRecentDocumentContainer.hide();
                oRecentMatterContainer.hide();
                oPinnedContainer.show();
                $(".sdBannerText").attr("data-result", "0"); // default make it pinned view
                $("#optionNoneForDocuments, #optionNoneForMatters").attr("data-sort-value", "").attr("data-value", "").text(oGlobalConstants.Sort_Type_None);

                sortText = $Element.text(oGlobalConstants.Sort_Type_None).text();
            } else {
                $("#grid").html("");
                oGridContainer.show();
                oPinnedContainer.hide();
                oRecentDocumentContainer.hide();
                oRecentMatterContainer.hide();
                $(".sdBannerText").attr("data-result", "1");
                $("#optionNoneForDocuments, #optionNoneForMatters").attr("data-sort-value", "").attr("data-value", "").text(oGlobalConstants.Sort_Type_Relevant);

                sortText = $Element.text(oGlobalConstants.Sort_Type_Relevant).text();
            }
            var sortTextWidth = sortText.length > 5 ? sortText.length < 9 ? 56 : (sortText.length - 1) * 8 : 35;
            $(".sdBannerText").val(sortText).css("width", sortTextWidth + "px")
                                                .attr("data-sort-value", $Element.attr("data-sort-value"))
                                                .attr("data-order", $Element.attr("data-order"))
                                                .attr("data-value", $Element.attr("data-value"))
                                                .attr("data-ss-value", $Element.attr("data-ss-value"));
        }
        oGridConfig.currentView ? oGridConfig.isMatterView ? requestPinnedMatter("pinnedGrid") : requestPinnedDocument("pinnedGrid") : advancedSearch();
    } else if (2 === oGridConfig.currentView) {
        ////
        // update oGridConfig isRecentView
        // check if it for find document or find matter
        // update the data result present in sort by drop down
        // select default sorting order
        // call the sorting value
        ////
        // update sorting value for document
        $(".sdBannerText").attr("data-result", "2");
        if (oGridConfig.isMatterView) {
            // Set modified date desc as Display none for matter
            $("#optionNoneForMatters").attr("data-sort-value", $("#optionModifiedDateDESCForMatters").attr("data-sort-value")).attr("data-order", $("#optionModifiedDateDESCForMatters").attr("data-order")).attr("data-value", $("#optionModifiedDateDESCForMatters").attr("data-value")).text(oGlobalConstants.Sort_Type_None).show().click();
        } else {
            // Hide none option
            $("#optionNoneForDocuments").hide();
            // Set sort by dropdown options to modified date desc
            $("#optionModifiedDateDESCForDocuments").click();
        }
    }
    if (oGridConfig.currentView) {                  // If toggling to my or pinned view, update count for all search results to total result count
        clearFilterPanel();
        if (oGridConfig.isMatterView) {
            $("#DisplayFindMattersResults .allSearch .ResultNumbers").html(" (" + oGridConfig.nAllMatterCount + ")");
        } else {
            $("#DisplaySearchDocumentsResults .allSearch .ResultNumbers").html(" (" + oGridConfig.nAllDocumentCount + ")");
        }
    }
    // Remove the previous active tab and add active class to this tab
    if ($(".active")[0]) {
        $(".active").removeClass("active");
    }
    $(oCurrentTab).addClass("active");
}

function onGridCheckboxClick(nIndex, event) {
    "use strict";
    // App Insight Event tracking for
    commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Add_To_Mail_Cart, true);
    var nCount = oGridConfig.arrSelectedGridData.length - 1,
        isArrayElement = false,
        iSelectedTab,
        nTopRowCount;
    while (nCount !== -1) {
        if (1 === oGridConfig.currentView) {
            iSelectedTab = "#pinnedGrid_Grid";
            if (oGridConfig.arrSelectedGridData[nCount].DocumentUrl) {
                if (oCommonObject.getDecodeURIComponent(oGridConfig.arrSelectedGridData[nCount].DocumentUrl) === oCommonObject.getDecodeURIComponent(oGridConfig.arrPinnedData[nIndex].DocumentUrl)) {
                    isArrayElement = true;
                    break;
                }
            } else {
                if (oCommonObject.getDecodeURIComponent(oGridConfig.arrSelectedGridData[nCount].Path) === oCommonObject.getDecodeURIComponent(oGridConfig.arrPinnedData[nIndex].DocumentUrl)) {
                    isArrayElement = true;
                    break;
                }
            }
        } else {
            iSelectedTab = 2 === oGridConfig.currentView ? "#RecentDocumentContainer_Grid" : "#grid_Grid";
            if (oGridConfig.arrSelectedGridData[nCount].DocumentUrl) {
                if (oCommonObject.getDecodeURIComponent(oGridConfig.arrSelectedGridData[nCount].DocumentUrl) === oCommonObject.getDecodeURIComponent(oGridConfig.arrGridData[nIndex].Path)) {
                    isArrayElement = true;
                    break;
                }
            } else {
                if (oCommonObject.getDecodeURIComponent(oGridConfig.arrSelectedGridData[nCount].Path) === oCommonObject.getDecodeURIComponent(oGridConfig.arrGridData[nIndex].Path)) {
                    isArrayElement = true;
                    break;
                }
            }
        }

        nCount--;
    }
    if (isArrayElement) {
        oGridConfig.arrSelectedGridData.splice(nCount, 1);
        if ($(iSelectedTab).find("#isSelectRowsActive").find("input[type='checkbox']:checked")) {
            $(iSelectedTab).find("#isSelectRowsActive").prop("checked", false);
        }
    } else {
        if (1 === oGridConfig.currentView) {
            oGridConfig.arrSelectedGridData.push(oGridConfig.arrPinnedData[nIndex]);
        } else {
            oGridConfig.arrSelectedGridData.push(oGridConfig.arrGridData[nIndex]);
        }
    }
    if (oGridConfig.arrSelectedGridData.length !== 0) {
        $("#EmailDiv").css("cursor", "pointer");
        $("#EmailDiv").css("color", "#333");
    } else {
        $("#EmailDiv").css("cursor", "default");
        $("#EmailDiv").css("color", "#444");
    }
    $("#EmailTextCount").text("(" + oGridConfig.arrSelectedGridData.length + ")");

    nTopRowCount = 1 === oGridConfig.currentView ? oGridConfig.arrPinnedData.length : oGridConfig.arrGridData.length;

    if ($(iSelectedTab).find(".jsonGridRow").find("input[type='checkbox']:checked").length === nTopRowCount) {
        $(iSelectedTab).find("#isSelectRowsActive").prop("checked", true);
    }
}

// Function to sort pinned result
function sortPinMatter(container, sortByfield) {
    "use strict";
    if (sortByfield.length) {
        var item, arrHideUpload = [], sProperty;
        var headerobject = document.querySelector(".jsonGridHeader");

        if (headerobject) {
            var sSortOrder = parseInt($(".sdBannerText").attr("data-order"), 10) ? "desc" : "asc";
            headerobject.setAttribute("sortorder", sSortOrder);
            oGrid.sortJsonGrid(headerobject, container, sortByfield);
            headerobject.setAttribute("sortorder", "asc");

            var gridObjectPosition = oGrid.gridName.indexOf("pinnedGrid_Grid");
            var currentGridConfig = oGrid.gridObject[gridObjectPosition];
            var pinnedMatters = currentGridConfig && currentGridConfig.data;
            // hide upload icon on matter tile and also on the matter pop up of pinned matters
            var $CurrWebPinnedMatter = $("#pinnedGrid .uploadImg");
            $.each($CurrWebPinnedMatter, function (iIterator) {
                if (pinnedMatters[iIterator].HideUpload.toLowerCase() !== "false") {
                    $(this).attr("class", "hideUpload");
                } else {
                    $(this).attr({ "onclick": "uploadElement(" + iIterator + ", event)", "src": "../Images/upload-666.png", "alt": "upload" });
                    $(this).attr("style", "");
                    $(this).removeClass("hideUpload").removeClass("hideUploadLeft");
                }
            });
            sortPinnedArray(sSortOrder, sortByfield);
        }
    } else {
        onPinnedItemSuccess(oSearchGlobal.pinnedMatterDataPassed, event);
    }
}

//// #region Formatter functions for Web Dashboard grid view
// Function to add Pin column in Web dashboard grid view
function loadPinIcon(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var sPinChunk = (oGridConfig.bLoadPinnedData) ? "<img title=\"Unpin\" src=\"../Images/unpin-666.png\" class=\"unPinIcon unPinIconLeft\" onclick=\"unPinElement('',this," + nIndex + ", event)\"  data-RowIndex = " + nIndex + " alt=\"\"/>" : "<img title=\"Pin\" src=\"../Images/pin-666.png\" class=\"pinIcon pinIconLeft\"  onclick=\"pinElement(" + nIndex + ", this, event)\" data-RowIndex = " + nIndex + " alt=\"\" />";
    sPinChunk = "<div>" + sPinChunk + "</div>";
    return sPinChunk;
}

// Function to add Upload column in Matter view
function loadUploadIcon(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var sUploadImageDiv = oSearchGlobal.oUpload[nIndex] ? oSearchGlobal.oUpload[nIndex].replace("#nIndex", nIndex) : "";
    return "<div class=\"iconPadding\">" + sUploadImageDiv + "</div>";
}

// Function to add check box column in document view
function addCheckBox(cellValue, rowObject, width, nIndex) {
    "use strict";
    var nCount,
        isChecked = "",
        iSelectedTab,
        nTopRowCount;
    var nSelectedRowCount = oGridConfig.arrSelectedGridData.length - 1;
    width = width - 10;         // Reducing 10, in order to incorporate 10px padding in the column
    if (1 === oGridConfig.currentView) {
        iSelectedTab = "#pinnedGrid_Grid";
        nTopRowCount = oGridConfig.arrPinnedData.length;
    } else if (2 === oGridConfig.currentView) {
        iSelectedTab = "#RecentDocumentContainer_Grid";
        nTopRowCount = oGridConfig.arrGridData.length;
    } else {
        iSelectedTab = "#grid_Grid";
        nTopRowCount = oGridConfig.arrGridData.length;
    }
    for (nCount = 0; nCount <= nSelectedRowCount; nCount++) {
        var sCurrentPath = oGridConfig.arrSelectedGridData[nCount] && oCommonObject.getDecodeURIComponent(oGridConfig.arrSelectedGridData[nCount].Path);
        var sPinnedDocumentPath = (oGridConfig.arrSelectedGridData[nCount] && oGridConfig.arrSelectedGridData[nCount].DocumentUrl) ? oCommonObject.getDecodeURIComponent(oGridConfig.arrSelectedGridData[nCount].DocumentUrl) : "";
        var rowObjectPath = rowObject.Path ? oCommonObject.getDecodeURIComponent(rowObject.Path) : oCommonObject.getDecodeURIComponent(rowObject.DocumentUrl);
        sCurrentPath = sCurrentPath ? sCurrentPath : sPinnedDocumentPath;
        if ((sCurrentPath && rowObjectPath && sCurrentPath === rowObjectPath)) {
            isChecked = "checked";
            oGridConfig.nCheckedRowCount++;
            if (oGridConfig.nCheckedRowCount === nTopRowCount) {
                if ($(iSelectedTab).find("#isSelectRowsActive").find("input[type='checkbox']")) {
                    $(iSelectedTab).find("#isSelectRowsActive").prop("checked", true);
                    oGridConfig.nCheckedRowCount = 0;
                }
            } else {
                if ($(iSelectedTab).find("#isSelectRowsActive").find("input[type='checkbox']")) {
                    $(iSelectedTab).find("#isSelectRowsActive").prop("checked", false);
                }
            }
            break;
        }
    }
    ////Following section is kept commented, as need to work on implementing fabric checkbox
    ////var iCounter = oGridConfig.currentView + 1;
    ////var iCurrentIndex = parseInt(parseInt(oGridConfig.itemsPerPage, 10) * parseInt(oGridConfig.nGridPageNumber, 10), 10) + parseInt(nIndex + 1, 10);
    var sCheckBoxChunk =
        "<div style=\"width:" + width + "\">"
       + "<input type=\"checkbox\" " + isChecked + " class=\"documentCheckBox floatcontentLeft\" onChange=\"onGridCheckboxClick(" + nIndex + ",event)\"/></div>";

    ////"<div class=\"checkBoxImage checkboxHoverImageContainer hide\" ></div><div class=\"ms-ChoiceField\" style=\"width:" + width + "\"><input class=\"ms-ChoiceField-input\" id=\"" + iCounter + "demo-checkbox-unselected" + iCurrentIndex + "\" type=\"checkbox\"/><label class=\"ms-ChoiceField-field\" for=\"" + iCounter + "demo-checkbox-unselected" + iCurrentIndex + "\" onclick=\"onGridCheckboxClick(" + nIndex + ",event)\"></label></div>";
    //// + "<input type=\"checkbox\" " + isChecked + " class=\"documentCheckBox floatcontentLeft\" onChange=\"onGridCheckboxClick(" + nIndex + ",event)\"/></div>";
    return sCheckBoxChunk;
}

// Function to append hyper link to document name in tile view
function loadGenericColumn(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    width = width - 10;         // Reducing 10, in order to incorporate 10px padding in the column.
    var sCurrentValue = $.trim(cellValue) ? trimEndChar($.trim(cellValue), ";") : "NA";
    var sHTMLChunk = "<div title=\"" + sCurrentValue + "\" style=\"width:" + width + "\" class=\"ellipses\" ><span>" + sCurrentValue + "</span></div>";
    return sHTMLChunk;
}

// Function to generate Title column
function loadNameColumn(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var sTitleProperty = "";
    width = width - 10;         // Reducing 10, in order to incorporate 10px padding in the column.
    // If pinned data is requested.
    if (oGridConfig.bLoadPinnedData) {
        sTitleProperty = oCommonObject.isMatterView ? oWebDashboardConstants.MatterPinnedTitleProperty : oWebDashboardConstants.DocumentPinnedTitleProperty;
    } else {
        sTitleProperty = oCommonObject.isMatterView ? oWebDashboardConstants.MatterTitleProperty : oWebDashboardConstants.DocumentTitleProperty;
    }
    var sTitleValue = ($.trim(rowObject[sTitleProperty])) ? $.trim(rowObject[sTitleProperty]) : "NA"
        , sDocumentPath, sDocumentExtension, sDocumentSPWebUrl;
    if (!oCommonObject.isMatterView) {
        // If pinned data is requested
        if (oGridConfig.bLoadPinnedData) {
            sDocumentPath = rowObject[oWebDashboardConstants.DocumentOWAUrl];
        } else {
            sTitleValue = extractTitle(sTitleValue); // Remove the extension from the document name only if current section is NOT pinned section
            // Document path URL will be available as a property of rowObject in case of pinned documents.
            sDocumentPath = trimEndChar(rowObject[oWebDashboardConstants.DocumentPath], "/"), sDocumentExtension = rowObject[oWebDashboardConstants.DocumentExtension], sDocumentSPWebUrl = rowObject[oWebDashboardConstants.DocumentSPWebUrl];
            if (-1 < $.inArray(sDocumentExtension, oFilterGlobal.arrOWADocumentExtension) && sDocumentSPWebUrl) {
                sDocumentPath = commonFunction.getOWAUrl(sDocumentExtension, sDocumentSPWebUrl, sDocumentPath);
            }
        }
    }
    // Code chunk to put all returned properties as data- attributes
    var sHTMLChunk = "<div class=\"ellipses nameColumn mandatory\" style=\"width: " + width + "\" title=\"" + sTitleValue;
    $.each(rowObject, function (iCurrentItem, sCurrentValue) {
        sCurrentValue = ("string" === typeof sCurrentValue) ? sCurrentValue.replace(/"/g, "") : sCurrentValue;
        sHTMLChunk += "\" data-" + iCurrentItem.toLowerCase() + " = \"" + sCurrentValue;
    });
    sHTMLChunk += "\"><span class=\"content\">" + sTitleValue + "</span></div>";
    return sHTMLChunk;
}

// Function to generate ClientMatterID column, content will be rendered as ClientID.MatterID
function generateClientMatterID(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var sMatterIDProperty = "", sClientMatterID = "", sMatterIDValue = "";
    var sClientIDValue = $.trim(cellValue) ? cellValue : "NA";
    // If pinned data is requested
    if (oGridConfig.bLoadPinnedData) {
        sMatterIDProperty = (oCommonObject.isMatterView) ? oWebDashboardConstants.PinnedMatterIDPropertyNameForMatter : oWebDashboardConstants.PinnedMatterIDPropertyNameForDocument;
    } else {
        sMatterIDProperty = (oCommonObject.isMatterView) ? oWebDashboardConstants.MatterIDPropertyForMatter : oWebDashboardConstants.MatterIDPropertyForDocument;
    }
    sMatterIDValue = $.trim(rowObject[sMatterIDProperty]) ? $.trim(rowObject[sMatterIDProperty]) : "NA";
    sClientMatterID = sClientIDValue + oGlobalConstants.ClientID_MatterID_Separator + sMatterIDValue;
    var sClientMatterIDChunk = "<div class='ellipses'><span title='" + sClientMatterID + "'>" + sClientMatterID + "</span></div>";
    return sClientMatterIDChunk;
}

// Function to generate Document Type Icon column in case of Search Document grid view
function loadDocumentTypeIcon(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var sExtension = "", iconSrc = "";
    if ($.trim(cellValue)) {
        sExtension = $.trim(cellValue);
    } else if (oGridConfig.bLoadPinnedData) {
        sExtension = (rowObject && rowObject.DocumentExtension) ? rowObject.DocumentExtension : "";
    } else {
        sExtension = (rowObject && rowObject.FileExtension) ? rowObject.FileExtension : "";
    }
    iconSrc = oCommonObject.getIconSource(sExtension);
    return "<img class='docTypeIcon mandatory' id='" + nIndex + "docTypeIcon' src='" + iconSrc + "' alt='' onerror='errorImage(this);'>";
}

// Function to generate Date column
function changeDateFormat(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var sCurrentValue = $.trim(cellValue) ? $.trim(cellValue) : "NA";
    var sDateValue = "NA" !== sCurrentValue ? formatDate(sCurrentValue) : "NA";
    var sHTMLChunk = "<div title='" + sDateValue + "' data-orignalDate='" + sDateValue + "'><span>" + sDateValue + "</span></div>";
    return sHTMLChunk;
}

//// #endregion