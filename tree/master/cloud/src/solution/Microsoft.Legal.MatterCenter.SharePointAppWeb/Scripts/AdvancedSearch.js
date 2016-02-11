/// <disable>JS3057,JS3058,JS2074,JS2076,JS3092,JS3054,JS2005,JS3056,JS2073,JS2024,JS2026,JS2032,JS2064,JS3116</disable>

var oGridConfig =
    {
        itemsPerPage: oWebDashboardConstants.ItemsPerPage,
        nGridPageNumber: 1,
        nPinnedGridPageNumber: 1,
        nGridTotalResults: 0,
        nPinnedGridTotalResults: 0,
        nCheckedRowCount: 0,
        isMatterView: true,
        isTileView: false,
        isMatterViewLoaded: false,
        isPinnedViewLoaded: false,
        arrGridData: [],
        arrRecentData: [],
        arrSelectedGridData: [],
        arrPinnedData: [],
        arrPinnedTemp: [],
        arrUnpinnedTemp: [],
        arrCheckedOutDoc: [],
        currentView: 0,          // 0: SearchResults, 1: PinnedView, 2: RecentView
        inWebDashboard: true,
        prevBrowserWidth: 0,
        bLoadPinnedData: false,
        nGridWidth: 0,
        nCurrentGridWidth: 0,
        nPreviousGridWidth: 0,
        nWidthDiff: 30,
        nAllMatterCount: 0,
        nAllDocumentCount: 0,
        bPageLoad: false,
        sSection: "?section="
    };

var oSearchGlobal = {
    siteURL: oGlobalConstants.Central_Repository_Url,
    serviceDMSURL: oGlobalConstants.Search_Service_Url,
    refinerHTMLTemplate: "<div class='refinerRow'><label class='refinerLabels'><input type='checkbox' class='refinerCheckBox' name='Client' value='#RefinerValue#' data-ClientURL='#RefinerClientURL#'/><div class='filterRefinerEntry' id='filterRefiner#RefinerValue#'>#RefinerValue#</div></label><div class='refinerSeparator'></div></div>",
    selectAllNone: "<div class='refinerRowLink refinerRowLinkRight'><a class='selectAllNone' href='javascript:void(0)' onclick='commonFunction.refinerSelectAll(this)'>Select All</a><div class='refinerSeparator'></div></div><div class='refinerRowLink'><a class='selectAllNone' href='javascript:void(0)' onclick='commonFunction.refinerSelectNone(this)'>Select None</a><div class='refinerSeparator'></div></div>",
    noRecordRow: "<div class='noRefiners#noRefinerVisible#'>No records found</div><div class='refinerSeparator'></div>",
    refinerOkButton: "<div class='filterOkButton'><div class='tileOk buttonOk' id='filterOk#SearchType#' onclick='commonFunction.refinerOkClick(\".filterSearch#SearchType#\")'>Ok</div></div>",
    refinerClientOkButton: "<div class='filterOkButton'><div class='tileOk' id='filterOk#SearchType#' onclick='commonFunction.refinerClientOkClick(\".filterSearch#SearchType#\")'>Ok</div></div>",
    refinerCancelButton: "<div class='filterOkButton'><div class='tileOk buttonCancel' id='filterOk#SearchType#' onclick='commonFunction.refinerCancelClick(this)'>Cancel</div></div>",
    regularRefiner: new RegExp("#RefinerValue#", "g"),
    regularRefinerType: new RegExp("#SearchType#", "g"),
    regularNoRefinerRow: new RegExp("#noRefinerVisible#", "g"),
    regularRefinerCount: new RegExp("#CountValue#", "g"),
    regularRefinerClientURL: new RegExp("#RefinerClientURL#", "g"),
    searchOption: 1,
    oDataArray: [],
    oFileArray: [],
    oUpload: [],
    sClientName: "",
    sClientSiteUrl: "",
    bIsTenantCall: false,
    pinnedMatterDataPassed: [],
    pinnedDocumentDataPassed: []
};

function closeAllPopupExcept(divClass, event) {
    "use strict";
    switch (divClass) {
        case "matterPopup":
            $(".filterPanel").hide();
            break;
        case "matterFolderWindow": $(".matterPopup").hide();
            $(".matterPopup .matterPopupData").find(":nth-child(10), :nth-child(11)").addClass("hide");
            $(".filterPanel").hide();
            break;
        case "filterPanel": $(".matterPopup").hide();
            $(".matterPopup .matterPopupData").find(":nth-child(10), :nth-child(11)").addClass("hide");
            break;
        default: break;
    }
    if (divClass !== "filterSearchAdvance" && divClass !== "refinerClient" && divClass !== "refinerPG" && divClass !== "refinerAOL" && divClass !== "ui-datepicker" && (event && event.target && event.target.className !== "ui-corner-all")) {
        $(".filterSearchAdvance").hide();
    }
    if ("sdBannerPanelItem" !== divClass && "sdBannerPanel" !== divClass && "sdBannerDD" !== divClass && "sdBannerText" !== divClass && "sdBannerDropdown" !== divClass) {
        $(".sdBannerPanel").addClass("hide");
    }
    if (divClass !== "refinerClient") {
        $(".refinerClient").hide();
    }
    if ("filterAutoComplete" !== divClass) {
        $(".filterAutoComplete").addClass("hide");
    }
    if ("switchApp" !== divClass) {
        $(".switchApp").hide();
    }
    if ("navDiv" !== divClass) {
        $(".switchTab").css("display", "none");
    }
    if ("quickLinksMenuItems" !== divClass) {
        $(".quickLinksMenuItems").css("display", "none");
    }

    if (event && "undefined" !== typeof event.stopPropagation) {
        divClass && event && event.stopPropagation();
    }
}

commonFunction.onClientSuccess = function (refiners) {
    "use strict";
    var refinerClientHTML = "<div class='refinerWrapper'>";
    var oClients = JSON.parse(refiners);
    if (!oClients.code) {
        var clientsList = oClients.ClientTerms;

        $.each(clientsList, function (iIterator, oClient) {
            refinerClientHTML = refinerClientHTML + oSearchGlobal.refinerHTMLTemplate.replace(oSearchGlobal.regularRefiner, oClient.Name).replace(oSearchGlobal.regularRefinerClientURL, oClient.Url);
            if ("" !== oSearchGlobal.sClientName && "" !== oSearchGlobal.sClientSiteUrl && oSearchGlobal.sClientSiteUrl.toLowerCase() === oClient.Url.toLowerCase()) {
                $(".refinerClientsText").attr({ "value": oClient.Name, "data-totalcount": 1, "data-selectedcount": 1, "disabled": "disabled" });
                $(".refinerClientDD").unbind("click").attr("disabled", "disabled");
            }
        });
        if (refinerClientHTML) {
            refinerClientHTML = oSearchGlobal.selectAllNone + refinerClientHTML + "</div>" + oSearchGlobal.noRecordRow.replace(oSearchGlobal.regularNoRefinerRow, "") + oSearchGlobal.refinerClientOkButton.replace(oSearchGlobal.regularRefinerType, "Clients") + oSearchGlobal.refinerCancelButton;
        } else {
            refinerClientHTML = oSearchGlobal.noRecordRow.replace(oSearchGlobal.regularNoRefinerRow, " noRefinersVisible");

        }
        $(".refinerClient").html(refinerClientHTML);
        $(".refinerClientsText").attr("data-totalcount", $(".filterSearchClients input[type='checkbox']").length);
        $(".refinerClientsText").attr("data-selectedcount", 0);
    } else {
        showCommonErrorPopUp(oClients.code);
    }
};

commonFunction.refinerClientOkClick = function (refinerId) {
    "use strict";
    var sSelectedText = "", sSelectedTextVal = "";
    $(refinerId + " .filterSearchRefinerText").attr("data-selectedcount", $(refinerId + " input:checked").length);
    $(refinerId + " input:checked").each(function () {
        if (sSelectedTextVal) {
            sSelectedText = sSelectedText + "$|$" + $(this).val();
            sSelectedTextVal = sSelectedTextVal + ", " + $(this).val(); //// Applied the fix for proper separator
        } else {
            sSelectedText = $(this).val();
            sSelectedTextVal = $(this).val();
        }
    });
    $(refinerId + " .filterSearchRefinerText").attr("data-selected", sSelectedText);
    $(refinerId + " .filterSearchRefinerText").val(sSelectedTextVal);
    commonFunction.closeAllFilterExcept("filterSearchAdvance");
};

function setRelevantIcon(event) {
    "use strict";
    // Check the item in the pinned item list and check whether this item is present in the results list.
    // For this we compare the document's path/ URL to ensure the documents are the same.
    // If yes, set the items icon to unpin.
    // If no, set the items icon to pin.
    if (oGridConfig.arrPinnedData && oGridConfig.arrGridData) {
        var nPinnedCount = 0, nPinnedLength = oGridConfig.arrPinnedData.length, nCount = 0, nLength = oGridConfig.arrGridData.length, sPathPropertyName, oPinnedItem,
       oGridContainer = 2 === oGridConfig.currentView ? oGridConfig.isMatterView ? "recentMatters" : "RecentDocumentContainer" : "grid";
        // Handle 0 scenario
        if (nPinnedLength === 0) {
            for (nCount = nLength - 1; nCount >= 0; nCount--) {
                sPathPropertyName = oGridConfig.isMatterView ? "MatterUrl" : "DocumentUrl";
                oPinnedItem = $("#" + oGridContainer + " img[data-rowindex=" + nCount + "]");
                if (oPinnedItem[0]) {
                    oPinnedItem.attr("src", "../Images/pin-666.png");
                    oPinnedItem.attr("onclick", "pinElement(" + nCount + ", this, event)");
                    oPinnedItem.attr("title", "pin");
                    oPinnedItem.attr("alt", "");
                    oPinnedItem.removeAttr("class");
                    oPinnedItem.addClass("pinIcon");
                }
            }
        } else {
            for (nPinnedCount = nPinnedLength - 1; nPinnedCount >= 0; nPinnedCount--) {
                for (nCount = nLength - 1; nCount >= 0; nCount--) {
                    sPathPropertyName = oGridConfig.isMatterView ? "MatterUrl" : "DocumentUrl";
                    var sCurrentPath = "", sDataItem = oGridConfig.arrGridData[nCount];
                    if (sDataItem && sDataItem.Path) {
                        if (oGridConfig.isMatterView) {
                            sCurrentPath = sDataItem.Path.toLowerCase();
                        } else {
                            sCurrentPath = decodeURIComponent(trimEndChar($.trim(sDataItem.Path), "/").toLowerCase());
                        }
                    }
                    oPinnedItem = $("#" + oGridContainer + " img[data-rowindex=" + nCount + "]");
                    if (oGridConfig.arrPinnedData[nPinnedCount][sPathPropertyName] && oGridConfig.arrPinnedData[nPinnedCount][sPathPropertyName].toLowerCase() === sCurrentPath) {
                        if (oPinnedItem[0]) {
                            oPinnedItem.attr("src", "../Images/unpin-666.png");
                            oPinnedItem.attr("onclick", "unPinElement('',this," + nPinnedCount + ", event)");
                            oPinnedItem.attr("title", "unpin");
                            oPinnedItem.attr("alt", "");
                            oPinnedItem.removeAttr("class");
                            oPinnedItem.addClass("unPinIcon");
                        }
                    } else if (oPinnedItem.attr("class") === "pinIcon" || oPinnedItem.attr("class") === "pinnedLoading floatContentLeft") {
                        oPinnedItem.attr("src", "../Images/pin-666.png");
                        oPinnedItem.attr("onclick", "pinElement(" + nCount + ", this, event)");
                        oPinnedItem.attr("title", "pin");
                        oPinnedItem.attr("alt", "");
                        oPinnedItem.removeAttr("class");
                        oPinnedItem.addClass("pinIcon");
                    }
                }
            }
        }
    }
}

function beforeAdvanceSearch() {
    "use strict";
    // Hide the Pinned Matter grid and hide recent matter grid
    $("#pinnedGrid, #RecentDocumentContainer, #recentMatters").hide();
    // show the All Results Grid
    $("#grid").show();
    // Remove the previous Active tab
    $(".active").removeClass("active");
    // Set the Active Tab
    $("#DisplayFindMattersResults .allSearch").addClass("active");
    $("#DisplaySearchDocumentsResults .allSearch").addClass("active");
    // Show Loading Image
    var sGridName = oGridConfig.isMatterView ? "Matters" : "Documents";
    $("#grid").html("<div class=\"centreContent\"><br /><img src=\"/Images/WindowsLoadingFast.GIF\" title=\"Loading\" alt=\"Loading...\" /><br /><br /> <div>Please wait while the " + sGridName + " load.</div><br /></div>");
}

function onSearchSuccess(oFinalResult, event) {
    "use strict";
    var nCurrentBrowserWidth = getWidth();
    if ($(".allSearch").hasClass("active")) {
        $(".allSearch").show();
        if (850 >= nCurrentBrowserWidth) {
            $(".mySearch, .pinnedSearch").not(".active").hide();
        }
    } else {
        $(".mySearch").show();
        if (850 >= nCurrentBrowserWidth) {
            $(".allSearch, .pinnedSearch").not(".active").hide();
        }
    }
    $(".filterAutoComplete").addClass("hide");
    if (oFinalResult && oFinalResult.Result && (-1 !== oFinalResult.Result.indexOf("$|$") || "undefined" !== typeof oFinalResult.Result.split("$|$"))) {
        var arrResultParts = oFinalResult.Result.split("$|$"),
            oResultData = {},
            nCountData = {},
            oRefinementData = {},
            arrColumnNames = [], arrGridHeaders = [], sAltRowColorValue = "rgb(255, 255, 255)", sCountHeaderID = "", sColumnID = "";
        oGridConfig.nCheckedRowCount = 0;
        oSearchGlobal.oUpload = [];
        if (arrResultParts[0]) {
            // Replacing any occurrence of \\\" with blank string, as encoding will convert \" to &quote; and decoding the same will return " only this will break JSON.parse, thus ensuring JSON.parse will not break in such scenario
            oResultData = JSON.parse(arrResultParts[0].replace(new RegExp('\\\\\\"', "g"), ""));    // Result Data
        }
        if (arrResultParts[1]) {
            nCountData = JSON.parse(arrResultParts[1]); // Count Data
            oGridConfig.nGridTotalResults = nCountData;
        }
        if (arrResultParts[2]) {
            oRefinementData = JSON.parse(arrResultParts[2]); // Filter Data
        }
        var nCount = 0, nLength = oResultData.length, arrResults = [];
        if (nLength && 0 < nLength && 0 < nCountData) {
            oGridConfig.isMatterViewLoaded = true;
            oGridConfig.bLoadPinnedData = false;
            oGridConfig.arrGridData = oResultData;
            for (nCount = 0; nCount < nLength; nCount++) {
                // for upload functionality
                if (oResultData[nCount] && oResultData[nCount].IsReadOnlyUser) {
                    if (oGridConfig.isMatterView) {
                        oSearchGlobal.oUpload.push("<img class='uploadImg hideUploadLeft' src='' onclick='' />");
                    } else if (oGridConfig.isTileView) {
                        oSearchGlobal.oUpload.push("<img class='uploadImg hideUpload' src='' onclick='' />");
                    }
                } else {
                    if (oGridConfig.isMatterView) {
                        oSearchGlobal.oUpload.push("<img title=\"Upload to this matter\" src=\"../Images/upload-666.png\" class=\"uploadImg uploadLeft\" onclick= \"uploadElement(#nIndex, event)\"/>");
                    } else if (oGridConfig.isTileView) {
                        oSearchGlobal.oUpload.push("<img  title='Upload to this matter' class='uploadImg'  onclick='populateFolderHierarchy(this); event.stopPropagation();' src='../Images/upload-666.png' />");
                    }
                }
            }

            var arrColumnJSON = oGrid.generateGridViewJSON();
            var arrHeaders = (oCommonObject.isMatterView) ? oWebDashboardConstants.MatterHeaderName.split(";") : oWebDashboardConstants.DocumentHeaderName.split(";");
            itemsCountsToBeDisplayed();
            var GridConfig = {
                container: oFinalResult.oParam.container,
                data: oResultData,
                gridName: "Search Result",
                gridHeader: arrHeaders,
                columnNames: arrColumnJSON,
                sortby: "",
                sortorder: "",
                sortType: String,
                initialsortorder: "",
                retainpageonsort: false,
                maxRows: oGridConfig.itemsPerPage,
                viewrecords: true,
                pagination: true,
                altRowColor: sAltRowColorValue,
                cellSpacing: 0
            };
            // Update the specific result count
            sCountHeaderID = oGridConfig.isMatterView ? "DisplayFindMattersResults" : "DisplaySearchDocumentsResults";
            if ("RecentDocumentContainer" === GridConfig.container || "recentMatters" === GridConfig.container) {
                sColumnID = "mySearch";
                oGridConfig.arrRecentData = oGridConfig.arrGridData;
            } else {
                sColumnID = "allSearch";
            }
            var oMatterCount = $("#" + sCountHeaderID + " ." + sColumnID + " .ResultNumbers");
            if (oMatterCount[0]) {
                oMatterCount.html(" (" + oGridConfig.nGridTotalResults + ")");
            }

            $("#" + oFinalResult.oParam.container).html("<div id=\"gridPaginationTD\"></div>");
            // Set the Pin or unpin icon
            new oGrid.JsonGrid(GridConfig);
            if ($(".pinnedSearch").hasClass("active")) {
                oGridConfig.currentView = 1;
            } else if ($(".mySearch").hasClass("active")) {
                oGridConfig.currentView = 2;
            } else {
                oGridConfig.currentView = 0;
            }
            if (oGridConfig.bPageLoad) {
                (getGridViewWidth) ? getGridViewWidth() : "";
            }
            if ("allSearch" === sColumnID) {
                (oGridConfig.bPageLoad) ? oGridConfig.isMatterView ? oGridConfig.nAllMatterCount = oGridConfig.nGridTotalResults : oGridConfig.nAllDocumentCount = oGridConfig.nGridTotalResults : "";
                (getGridViewWidth) ? getGridViewWidth() : "";
            }
            var sSelectedTab = getCurrentSelectedTab();
            $(sSelectedTab).css("maxWidth", oGridConfig.nGridWidth);
            $(sSelectedTab).css("width", 0);
            if (oGridConfig.isMatterViewLoaded && oGridConfig.isPinnedViewLoaded) {
                setRelevantIcon(event);
            }
        } else {
            oGridConfig.arrGridData.length = 0;
            sCountHeaderID = oGridConfig.isMatterView ? "DisplayFindMattersResults" : "DisplaySearchDocumentsResults"
            , sColumnID = "";
            if ("RecentDocumentContainer" === oFinalResult.oParam.container || "recentMatters" === oFinalResult.oParam.container) {
                sColumnID = "mySearch";
            } else {
                sColumnID = "allSearch";
            }
            if (0 === nLength || 0 === nCountData) {
                gridFailure(oFinalResult, oWebDashboardConstants.NoResults_Message, "." + sColumnID);
            } else {
                var oErrorPopUpData = JSON.parse(oFinalResult.Result);
                showCommonErrorPopUp(oErrorPopUpData.code);
            }
        }
        $("#grid .uploadImg").attr("style", "");
    } else {
        oGridConfig.arrGridData.length = 0;
        showCommonErrorPopUp(oResult);
    };
    createResponsiveGrid();
}

function onSearchFailure(oFinalResult) {
    "use strict";
    showCommonErrorPopUp(oFinalResult);
}

function gridFailure(oResult, sMessage, sColumnHeaderToUpdate) {
    "use strict";
    var oContainer = $("#" + oResult.oParam.container), sCountHeaderID = "";
    if (oContainer[0]) {
        oContainer.html("<div id=\"NoDataToDisplay\" class=\"centreContent\"><span id=\"NoDataMessage\" class=\"TextContent\">" + sMessage + "</span></div>");
    }
    // Update the count to 0
    sCountHeaderID = oGridConfig.isMatterView ? "DisplayFindMattersResults" : "DisplaySearchDocumentsResults";
    var oMatterCount = $("#" + sCountHeaderID + " " + sColumnHeaderToUpdate + " .ResultNumbers");
    if (oMatterCount[0]) {
        oMatterCount.html(" (0)");
    }
}

function advancedSearch(sContainer, event) {
    "use strict";
    var sMethodName = oGridConfig.isMatterView ? "FindMatter" : "FindDocument";
    oGridConfig.isMatterView ? $(".filterSearchText").attr({ "placeholder": "Search by matter name, matter ID, or keyword", "title": "Search by matter name, matter ID, or keyword" }) : $(".filterSearchText").attr({ "placeholder": "Search by document name, document ID, or keyword", "title": "Search by document name, document ID, or keyword" });
    var sRefinerString = oGridConfig.isMatterView ? oGlobalConstants.Matter_ID : oGlobalConstants.Document_ID;
    oGridConfig.currentView = "RecentDocumentContainer" === sContainer || "recentMatters" === sContainer ? 2 : 0;
    oSearchGlobal.clientDataLoaded = true;
    var sDeployUrl = oGlobalConstants.Central_Repository_Url;
    oCommonObject.isMatterView = oGridConfig.isMatterView;
    oSearchGlobal.searchOption = 0;
    if (1 === oGridConfig.nGridPageNumber && !oCommonObject.bCalledForSort) {
        oCommonObject.sSearchedKeyword = $(".filterSearchText").val().trim();
        var FilterDetails = oSearchGlobal.oFilterData = oCommonObject.getSearchData(event);
        if ("" !== oSearchGlobal.sClientName && "" !== oSearchGlobal.sClientSiteUrl) {
            if ("" === FilterDetails.ClientsList[0]) {
                FilterDetails.ClientsList = [];
            }
            FilterDetails.ClientsList.push(oSearchGlobal.sClientSiteUrl);
        }
        oCommonObject.sFilterDetails = FilterDetails;
    }
    oCommonObject.bCalledForSort = false;
    var sSearchTerm = oSearchGlobal.sSearchTerm = oCommonObject.sSearchedKeyword !== "" ? oCommonObject.formatSearchText(oCommonObject.sSearchedKeyword, sRefinerString) : "";
    var SortDetails = getSortData();
    itemsCountsToBeDisplayed();
    var SearchDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": sDeployUrl }, "searchObject": { "PageNumber": oGridConfig.nGridPageNumber, "ItemsPerPage": oGridConfig.itemsPerPage, "SearchTerm": sSearchTerm, "Filters": oCommonObject.sFilterDetails, "Sort": SortDetails } };
    var oParam = { "container": "grid", "isPage": 1, "pageNumber": 1 };
    oCommonObject.callSearchService(sMethodName, SearchDetails, onSearchSuccess, onSearchFailure, beforeAdvanceSearch, oParam);
    commonFunction.closeAllFilterExcept("", event);
    return false;
}

function requestPinnedMatter(container) {
    "use strict";
    var pinnedMatterDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oGlobalConstants.Central_Repository_Url } };
    var oParam = { "container": container };
    oCommonObject.callSearchService("FindUserPinnedMatter", pinnedMatterDetails, onPinnedItemSuccess, onPinnedItemFailure, pinnedMatterBeforeSend, oParam);
}

function requestPinnedDocument(container) {
    "use strict";
    var pinnedMatterDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oGlobalConstants.Central_Repository_Url } };
    var oParam = { "container": container };
    oCommonObject.callSearchService("FindUserPinnedDocument", pinnedMatterDetails, onPinnedItemSuccess, onPinnedItemFailure, pinnedMatterBeforeSend, oParam);
}

function onPinnedItemSuccess(oFinalResult, event) {
    "use strict";
    var sErrorPopUpDetails;
    oGridConfig.bLoadPinnedData = true;
    oGridConfig.isMatterView ? oSearchGlobal.pinnedMatterDataPassed = oFinalResult : oSearchGlobal.pinnedDocumentDataPassed = oFinalResult;
    if (oFinalResult && oFinalResult.Result && (-1 !== oFinalResult.Result.indexOf("$|$") || "undefined" !== typeof oFinalResult.Result.split("$|$"))) {
        var sImgShow = "<img title='Upload to this matter' src='../Images/upload-666.png' class='uploadImg uploadLeft' onclick= 'uploadElement(#nIndex, event)'/>",
            sImgHide = "<img title='Upload to this matter' src='../Images/upload-666.png' class='uploadImg hideUploadLeft' onclick= 'uploadElement(#nIndex, event)'/>",
            arrResultParts = oFinalResult.Result.split("$|$"),
            oResultData = {},
            nCountData = {},
            arrClientPinnedData = [],
            oRefinementData = {},
            sResultClientUrl = "",
            arrColumnNames = [], arrGridHeaders = [], sAltRowColorValue = "rgb(255, 255, 255)", sCountHeaderID = "";
        oGridConfig.nCheckedRowCount = 0;
        oSearchGlobal.oUpload = [];
        if (arrResultParts[0]) {
            oResultData = JSON.parse(arrResultParts[0]); // Result Data
        }
        if (arrResultParts[2]) {
            oRefinementData = JSON.parse(arrResultParts[2]); // Filter Data
        }
        var nCount = 0, nLength = oResultData.length, arrResults = [], oTemp, arrHideUpload = [];
        if (nLength > 0) {
            oGridConfig.isPinnedViewLoaded = true;
            nCountData = 0;
            for (nCount = 0; nCount < nLength; nCount++) {
                if (oResultData[nCount]) {
                    sResultClientUrl = oGridConfig.isMatterView ? oResultData[nCount].MatterClientUrl : oResultData[nCount].DocumentClientUrl;
                    sCountHeaderID = oGridConfig.isMatterView ? "DisplayFindMattersResults" : "DisplaySearchDocumentsResults";
                    if (oSearchGlobal.sClientSiteUrl && sResultClientUrl && sResultClientUrl.toLowerCase() !== oSearchGlobal.sClientSiteUrl.toLowerCase()) {
                        continue;
                    }
                    ++nCountData;
                    arrClientPinnedData.push(oResultData[nCount]);
                    // For Upload security functionality
                    if (oResultData[nCount].HideUpload) {
                        if ("false" === oResultData[nCount].HideUpload.toLowerCase()) {
                            arrHideUpload.push(false);
                        } else {
                            arrHideUpload.push(true);
                        }
                    }
                }
            }

            // Update the specific result count
            if ("" !== oSearchGlobal.sClientSiteUrl && "" !== oSearchGlobal.sClientName) {
                oGridConfig.nPinnedGridTotalResults = nCountData;
                oGridConfig.arrPinnedData = arrClientPinnedData;
            } else if (arrResultParts[1]) {
                oGridConfig.nPinnedGridTotalResults = JSON.parse(arrResultParts[1]); // Count Data
                oGridConfig.arrPinnedData = oResultData;
            }
            var oMatterCount = $("#" + sCountHeaderID + " .pinnedSearch .ResultNumbers");
            if (oMatterCount[0]) {
                oMatterCount.html(" (" + oGridConfig.nPinnedGridTotalResults + ")");
            }

            if (0 === nCountData) {
                oGridConfig.arrPinnedData.length = 0;
                gridFailure(oFinalResult, oWebDashboardConstants.NoResults_Message, ".pinnedSearch");
                return;
            }

            if (oGridConfig.isTileView) {
                arrColumnNames = [{ name: "Matter Name", width: 206, id: "Matter Name", align: "left", trimOnOverflow: false, formatter: "createTile" }];
                arrGridHeaders = [""];
                sAltRowColorValue = "rgb(255, 255, 255)";

            } else {
                var arrColumnJSON = oGrid.generateGridViewJSON();
                var arrHeaders = (oCommonObject.isMatterView) ? oWebDashboardConstants.MatterHeaderName.split(";") : oWebDashboardConstants.DocumentHeaderName.split(";");
            }
            itemsCountsToBeDisplayed();
            var GridConfig = {
                container: oFinalResult.oParam.container,
                data: oGridConfig.arrPinnedData,
                gridName: "Search Result",
                gridHeader: arrHeaders,
                columnNames: arrColumnJSON,
                sortby: "",
                sortorder: "",
                sortType: String,
                initialsortorder: "",
                retainpageonsort: false,
                maxRows: oGridConfig.itemsPerPage,
                viewrecords: true,
                pagination: true,
                altRowColor: sAltRowColorValue,
                cellSpacing: 0
            };
            $("#" + oFinalResult.oParam.container).html("<div id=\"gridPaginationTD\"></div>");
            var iPinnedLength = arrHideUpload.length;
            for (var bHide in arrHideUpload) {
                if (arrHideUpload[bHide]) {
                    oSearchGlobal.oUpload.push(sImgHide);
                } else {
                    oSearchGlobal.oUpload.push(sImgShow);
                }
            }

            new oGrid.JsonGrid(GridConfig);
            if ($(".pinnedSearch").hasClass("active")) {
                oGridConfig.currentView = 1;
            } else if ($(".mySearch").hasClass("active")) {
                oGridConfig.currentView = 2;
            } else {
                oGridConfig.currentView = 0;
            }
            var sSelectedTab = getCurrentSelectedTab();
            var sSelectedValue = $(".sdBannerText").attr("data-value");
            if (!oGridConfig.isMatterView) {
                // call sorting for all pinned result 
                if (sSelectedValue.length) {
                    sortPinDocument("pinnedGrid_Grid", sSelectedValue);
                }
            } else {
                // call sorting for all pinned result 
                if (sSelectedValue.length) {
                    sortPinMatter("pinnedGrid_Grid", sSelectedValue);
                }
            }
            if (1 === oGridConfig.currentView) {
                getGridViewWidth();
                createResponsiveGrid();
                $(sSelectedTab).css("maxWidth", oGridConfig.nGridWidth);
                $(sSelectedTab).css("width", 0);
            }
            // Set the Pin or unpin icon
            if (oGridConfig.isMatterViewLoaded && oGridConfig.isPinnedViewLoaded) {
                setRelevantIcon(event);
            }
        } else {
            sErrorPopUpDetails = JSON.parse(oFinalResult.Result);
            if ("0" === sErrorPopUpDetails.code) {
                oGridConfig.arrPinnedData.length = 0;
                oGridConfig.isMatterView ? gridFailure(oFinalResult, oWebDashboardConstants.NoPinnedMatters, ".pinnedSearch") : gridFailure(oFinalResult, oWebDashboardConstants.NoPinnedDocuments, ".pinnedSearch");
                // In case of removal of all pinned items, we need to update the result grid
                setRelevantIcon(event);
            } else {
                showCommonErrorPopUp(sErrorPopUpDetails.code);
            }
        }
        // hide upload icon on matter tile and also on the matter pop up of pinned matters
        var $CurrWebPinnedMatter = $("#pinnedGrid .uploadImg");
        $.each($CurrWebPinnedMatter, function (iIterator) {
            if (arrHideUpload[iIterator]) {
                $(this).addClass("hideUpload");
            } else {
                $(this).attr({ "onclick": "uploadElement(" + iIterator + ", event)", "src": "../Images/upload-666.png", "alt": "upload" }).removeClass("hideUpload");
                if (oGridConfig.isTileView) {
                    $(this).attr("style", "");
                }
            }

        });
    } else {
        oGridConfig.arrPinnedData.length = 0;
        sErrorPopUpDetails = JSON.parse(oFinalResult.Result);
        showCommonErrorPopUp(sErrorPopUpDetails.code);
    }
}

function onPinnedItemFailure(oResult) {
    "use strict";
    oGridConfig.arrPinnedData.length = 0;
    showCommonErrorPopUp(oResult);
}

function pinnedMatterBeforeSend() {
    "use strict";
    var sGridName = oGridConfig.isMatterView ? "Matters" : "Documents";
    $("#pinnedGrid").html("<div class=\"centreContent\"><img src=\"/Images/WindowsLoadingFast.GIF\" title=\"Loading\" alt=\"Loading...\" /><br /><br /> <span>Please wait while " + sGridName + " load.</span></div>");
}

// Function to send call to SharePoint, to update iFrame height
function updateIframeHeight(iHeight) {
    "use strict";
    var iFrameURL = oGlobalConstants.Site_Url;
    iHeight = (iHeight) ? !isNaN(iHeight) ? iHeight + "px" : iHeight : ($(".LeftContent").height() + 230) + "px"; // Adding 230px to incorporate the height of footer
    window.top.parent.postMessage(iHeight, iFrameURL);
}

// Function to send call to SharePoint, to update scroll to top
function scrollIframeTop() {
    "use strict";
    var iFrameURL = oGlobalConstants.Site_Url;
    var message = "scrolltop";
    window.top.parent.postMessage(message, iFrameURL);
}

// Function to send call to SharePoint, to update the Query string based upon current view
function updateQueryString() {
    "use strict";
    var sSectionValue = oGridConfig.isMatterView ? "1" : "2";
    var message = oGridConfig.sSection + sSectionValue;
    window.top.parent.postMessage(message, oGlobalConstants.Site_Url);
}

function getPinUnpinData(nElementIndex, oThisElement) {
    "use strict";
    var oPinnedItem = oGridConfig.arrGridData[nElementIndex], oParameters = {}, $Element = $(oThisElement).parent().parent().siblings().find(".uploadImg");
    var bHideUpload = true;
    if ($Element) {
        var uploadImage = $Element.attr("src");
        if (uploadImage) {
            bHideUpload = false;
        }
    }
    if (oPinnedItem) {
        if (oGridConfig.isMatterView) {
            oParameters = {
                "requestObject": {
                    "SPAppToken": oSharePointContext.SPAppToken,
                    "RefreshToken": oSharePointContext.RefreshToken
                },
                "client": {
                    "Url": oGlobalConstants.Central_Repository_Url
                },
                "matterData": {
                    "MatterClient": oPinnedItem[oGlobalConstants.Client_Name],
                    "MatterName": oPinnedItem[oGlobalConstants.Matter_Name],
                    "MatterDescription": oPinnedItem.Description,
                    "MatterCreatedDate": oPinnedItem[oGlobalConstants.Open_Date],
                    "MatterUrl": oPinnedItem.Path,
                    "MatterPracticeGroup": oPinnedItem[oGlobalConstants.Practice_Group],
                    "MatterAreaOfLaw": oPinnedItem[oGlobalConstants.Area_Of_Law],
                    "MatterSubAreaOfLaw": oPinnedItem[oGlobalConstants.Sub_Area_Of_Law],
                    "MatterClientUrl": oPinnedItem.SiteName,
                    "HideUpload": bHideUpload,
                    "MatterID": oPinnedItem[oGlobalConstants.Matter_ID],
                    "MatterClientId": oPinnedItem[oGlobalConstants.Client_ID],
                    "MatterResponsibleAttorney": oPinnedItem[oGlobalConstants.Responsible_Attorney],
                    "MatterModifiedDate": oPinnedItem[oGlobalConstants.Last_Modified_Time],
                    "MatterGuid": (null !== oPinnedItem[oGlobalConstants.Matter_GUID]) ? oPinnedItem[oGlobalConstants.Matter_GUID] : oPinnedItem[oGlobalConstants.Matter_Name]
                }
            };
        } else {
            var sFileName = oGridConfig.isMatterView ? oPinnedItem.Title : extractTitle(oPinnedItem.FileName);
            var sDocumentName = sFileName && sFileName !== "" ? sFileName : "NA", // Document Name
             sDocumentVersion = oPinnedItem.UIVersionStringOWSTEXT && oPinnedItem.UIVersionStringOWSTEXT !== "" ? oPinnedItem.UIVersionStringOWSTEXT : "NA",
             sDocumentClientName = oPinnedItem[oGlobalConstants.Document_Client_Name] && oPinnedItem[oGlobalConstants.Document_Client_Name] !== "" ? oPinnedItem[oGlobalConstants.Document_Client_Name] : "NA", // Client Name
             sDocumentClientId = oPinnedItem[oGlobalConstants.Document_Client_ID] && oPinnedItem[oGlobalConstants.Document_Client_ID] !== "" ? oPinnedItem[oGlobalConstants.Document_Client_ID] : "NA",
             sDocumentClientUrl = oPinnedItem.SPWebUrl && oPinnedItem.SPWebUrl !== "" ? decodeURIComponent(oPinnedItem.SPWebUrl) : "NA",
             sDocumentCreatedDate = oPinnedItem.Created && oPinnedItem.Created !== "" ? oPinnedItem.Created : "NA", // Open Date
             sDocumentMatter = oPinnedItem[oGlobalConstants.Document_Matter_Name] && oPinnedItem[oGlobalConstants.Document_Matter_Name] !== "" ? oPinnedItem[oGlobalConstants.Document_Matter_Name] : "NA", // Matter Name
             sDocumentMatterId = oPinnedItem[oGlobalConstants.Document_Matter_ID] && oPinnedItem[oGlobalConstants.Document_Matter_ID] !== "" ? oPinnedItem[oGlobalConstants.Document_Matter_ID] : "NA",
             sDocumentDateModified = oPinnedItem[oGlobalConstants.Document_Last_Modified_Time] && oPinnedItem[oGlobalConstants.Document_Last_Modified_Time] !== "" ? oPinnedItem[oGlobalConstants.Document_Last_Modified_Time] : "NA", // Date Modified
             sDocumentCheckoutUser = oPinnedItem[oGlobalConstants.Managed_Property_Document_CheckOutuser] ? oPinnedItem[oGlobalConstants.Managed_Property_Document_CheckOutuser] : "NA",
             sDocumentParentUrl = oPinnedItem.ParentLink ? decodeURIComponent(oPinnedItem.ParentLink.replace(oGlobalConstants.All_Items_Extension, "")) : "NA",
             sDocumentMatterUrl = oPinnedItem.SPWebUrl ? decodeURIComponent(oPinnedItem.SPWebUrl) : "NA",
             sLibraryName = sDocumentParentUrl.replace(sDocumentMatterUrl, "").split("/"),
             sDocumentID = oPinnedItem.dlcDocIdOWSText ? oPinnedItem.dlcDocIdOWSText : "NA",
             arrAuthor = oPinnedItem[oGlobalConstants.Author].split(";"),
             nIndex,
             sDocumentOwner,
             sAuthorList = "";
            sLibraryName = 1 < sLibraryName.length ? sLibraryName[1] : "";
            sDocumentMatterUrl = sDocumentMatterUrl + "/" + sLibraryName;
            if (arrAuthor.length > 0) {
                for (nIndex = 0; nIndex < arrAuthor.length; nIndex++) {
                    if (arrAuthor[nIndex].indexOf("<") === -1) {
                        sAuthorList += arrAuthor[nIndex] + ";";
                    }
                }
            }
            if (!oGridConfig.isMatterView) {
                if (0 < oPinnedItem[oGlobalConstants.Author].search("<")) {
                    sDocumentOwner = sAuthorList;
                } else {
                    sDocumentOwner = oPinnedItem[oGlobalConstants.Author] && oPinnedItem[oGlobalConstants.Author] !== "" ? oPinnedItem[oGlobalConstants.Author] : "NA"; // Author
                }
            }
            var sDocumentUrl = oPinnedItem.Path && oPinnedItem.Path !== "" ? decodeURIComponent(trimEndChar(oPinnedItem.Path, "/").toLowerCase()) : "NA"; // Path
            var sDocumentExtension = oPinnedItem.FileExtension && oPinnedItem.FileExtension !== "" ? oPinnedItem.FileExtension : "NA";
            var sDocumentOWAUrl = trimEndChar(sDocumentUrl, "/");
            var sDocumentSPWebUrl = oPinnedItem.SPWebUrl ? oPinnedItem.SPWebUrl : "NA";
            if (-1 < $.inArray(sDocumentExtension, oFilterGlobal.arrOWADocumentExtension) && sDocumentSPWebUrl) {
                sDocumentOWAUrl = decodeURIComponent(commonFunction.getOWAUrl(sDocumentExtension, sDocumentSPWebUrl, sDocumentOWAUrl));
            }
            oParameters = {
                "requestObject": {
                    "SPAppToken": oSharePointContext.SPAppToken,
                    "RefreshToken": oSharePointContext.RefreshToken
                },
                "client": {
                    "Url": oGlobalConstants.Central_Repository_Url
                },
                "documentData": {
                    "DocumentName": sDocumentName,
                    "DocumentVersion": sDocumentVersion,
                    "DocumentClient": sDocumentClientName,
                    "DocumentClientId": sDocumentClientId,
                    "DocumentClientUrl": sDocumentClientUrl,
                    "DocumentMatter": sDocumentMatter,
                    "DocumentMatterId": sDocumentMatterId,
                    "DocumentOwner": sDocumentOwner,
                    "DocumentUrl": sDocumentUrl,
                    "DocumentOWAUrl": sDocumentOWAUrl,
                    "DocumentExtension": sDocumentExtension,
                    "DocumentCreatedDate": sDocumentCreatedDate,
                    "DocumentModifiedDate": sDocumentDateModified,
                    "DocumentCheckoutUser": sDocumentCheckoutUser,
                    "DocumentMatterUrl": sDocumentMatterUrl,
                    "DocumentParentUrl": sDocumentParentUrl,
                    "DocumentID": sDocumentID
                }
            };
        }
    }
    return oParameters;
}

function onCreatePinMatterSuccess(oResultData, event) {
    "use strict";
    if ("True" === oResultData.Result) {
        // Reload the grid
        oGridConfig.isMatterView ? requestPinnedMatter("pinnedGrid") : requestPinnedDocument("pinnedGrid");
    } else {
        // Reset if Failure
        onCreatePinMatterFailure(oResultData, event);
    }
}

function onCreatePinMatterFailure(oResultData, event) {
    "use strict";
    // Reset if Failure
    var oPinnedItem = oResultData.oParam.element;
    if (oPinnedItem) {
        $(oPinnedItem).attr("src", "../Images/pin-666.png");
        $(oPinnedItem).attr("onclick", "pinElement(" + oResultData.oParam.nIndex + ", this, event)");
        $(oPinnedItem).attr("title", "pin");
        $(oPinnedItem).attr("alt", "");
        $(oPinnedItem).removeAttr("class");
        $(oPinnedItem).addClass("pinIcon");
    }
    var sErrorPopUpDetails = JSON.parse(oResultData.Result);
    showCommonErrorPopUp(sErrorPopUpDetails.code);
}

function pinElement(nIndex, oThisElement, event) {
    "use strict";
    var oPinnedItemParameters = getPinUnpinData(nIndex, oThisElement);
    oGridConfig.isMatterView ? oPinnedItemParameters.matterData.MatterCreatedDate = validateDateFormat(oPinnedItemParameters.matterData.MatterCreatedDate) : oPinnedItemParameters.documentData.DocumentCreatedDate = validateDateFormat(oPinnedItemParameters.documentData.DocumentCreatedDate);
    commonFunction.closeAllFilterExcept("");
    oGridConfig.arrPinnedTemp.push(oPinnedItemParameters);
    var oParam = { "container": "pinnedGrid", "element": event.target, "nIndex": nIndex };
    var sFunctionName = oGridConfig.isMatterView ? "PinMatterForUser" : "PinDocumentForUser";
    oCommonObject.callSearchService(sFunctionName, oPinnedItemParameters, onCreatePinMatterSuccess, onCreatePinMatterFailure, OnBeforePinUnpin, oParam);
    event.stopPropagation();
}

// Converts and return date to UTC format
function validateDateFormat(sDateToBeProcressed) {
    "use strict";
    var oDates = new Date(sDateToBeProcressed);
    if (isNaN(oDates)) {
        var arrSplitedDate = sDateToBeProcressed.replace(/[-]/g, "/");
        arrSplitedDate = arrSplitedDate.split("/");
        var sFormattedDate = arrSplitedDate[1] + "-" + arrSplitedDate[0] + "-" + arrSplitedDate[2];
        oDates = new Date(sFormattedDate);
    }
    return oDates.toISOString();
}

function OnBeforePinUnpin(oResultData) {
    "use strict";
    // Show the loading image for the unpinned item
    var oUnpinnedItem = oResultData.oParam.element;
    if (oUnpinnedItem) {
        $(oUnpinnedItem).attr("class", "pinnedLoading floatContentLeft");
        $(oUnpinnedItem).attr("src", "../Images/WindowsLoadingFast.GIF");
        $(oUnpinnedItem).attr("onclick", "return false;");
        $(oUnpinnedItem).attr("title", "loading...");
        $(oUnpinnedItem).attr("alt", "loading...");
    }
}

function onUnpinSuccess(oResultData, event) {
    "use strict";
    commonFunction.closeAllFilterExcept("");
    if ("True" === oResultData.Result) {
        oGridConfig.isMatterView ? requestPinnedMatter("pinnedGrid") : requestPinnedDocument("pinnedGrid");
    } else {
        // Reset if Failure
        onUnpinFailure(oResultData, event);
    }
}

function onUnpinFailure(oResultData, event) {
    "use strict";
    // Reset if Failure
    var oUnpinnedItem = oResultData.oParam.element;
    if (oUnpinnedItem) {
        $(oUnpinnedItem).attr("src", "../Images/unpin-666.png");
        $(oUnpinnedItem).attr("onclick", "unPinElement('',this," + oResultData.oParam.nIndex + ", event)");
        $(oUnpinnedItem).attr("title", "unpin");
        $(oUnpinnedItem).attr("alt", "");
        $(oUnpinnedItem).removeAttr("class");
        $(oUnpinnedItem).addClass("unPinIcon");
    }
    var sErrorPopUpDetails = JSON.parse(oResultData.Result);
    showCommonErrorPopUp(sErrorPopUpDetails.code);
}

function unPinElement(container, oItem, nIndex, event) {
    "use strict";
    var sParameters = {}, oParam = {}, sFunctionName, sDocumentUrl;
    commonFunction.closeAllFilterExcept("");
    if (oGridConfig.isMatterView) {
        sFunctionName = "RemovePinnedMatter";
        sParameters = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oGlobalConstants.Central_Repository_Url }, "matterData": { "MatterName": oGridConfig.arrPinnedData[nIndex].MatterUrl } }, oParam = { "container": container, "matterName": oGridConfig.arrPinnedData[nIndex].MatterName, "element": oItem };
        oParam = { "element": oItem, "nIndex": nIndex };
    } else {
        sFunctionName = "RemovePinnedDocument";
        sDocumentUrl = decodeURIComponent(trimEndChar($.trim(oGridConfig.arrPinnedData[nIndex].DocumentUrl), "/").toLowerCase());
        sDocumentUrl = oCommonObject.htmlEncode(sDocumentUrl);
        sParameters = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oGlobalConstants.Central_Repository_Url }, "documentData": { "DocumentName": oGridConfig.arrPinnedData[nIndex].DocumentUrl, "DocumentVersion": oGridConfig.arrPinnedData[nIndex].DocumentVersion, "DocumentClient": oGridConfig.arrPinnedData[nIndex].DocumentClient, "DocumentClientId": oGridConfig.arrPinnedData[nIndex].DocumentClientId, "DocumentMatter": oGridConfig.arrPinnedData[nIndex].DocumentMatter, "DocumentMatterId": oGridConfig.arrPinnedData[nIndex].DocumentMatterId, "DocumentOwner": oGridConfig.arrPinnedData[nIndex].DocumentOwner, "DocumentUrl": sDocumentUrl, "DocumentOWAUrl": oGridConfig.arrPinnedData[nIndex].DocumentOWAUrl, "DocumentExtension": oGridConfig.arrPinnedData[nIndex].DocumentExtension } };
        oParam = { "element": oItem, "nIndex": nIndex };
    }
    oCommonObject.callSearchService(sFunctionName, sParameters, onUnpinSuccess, onUnpinFailure, OnBeforePinUnpin, oParam);
    event.stopPropagation();
}

$(document).ready(function (event) {
    "use strict";
    $.ajax({
        url: "filterpanel.html?ver=25.0.0.0",
        success: function (response) {
            $(".bannerMatterWorkspace").html($(response).find(".commonBannerWrapper").html());
            $(".matterPopup").html($(response).find(".commonMatterPopup").html());
            $(".documentPopup").html($(response).find(".commonDocumentPopup").html());
            $("#mailBody").html($(response).find("#commonMailBody").html());
            $(".TryAgainLink").attr("href", "javascript:window.top.location.reload()");
            commonFunction.onLoadActions(true);
            commonFunction.setOWADocumentExtension();
            $("#filterAdvancedSearch").click(function (e) {
                // Reset the Search Results
                var oMatterResultNumber = $("#DisplayFindMattersResults .allSearch .ResultNumbers");
                var oDocumentResultNumber = $("#DisplaySearchDocumentsResults .allSearch .ResultNumbers");
                if (oMatterResultNumber[0] && oDocumentResultNumber[0]) {
                    oMatterResultNumber.html(" (0)");
                    oMatterResultNumber.html(" (0)");
                }
                // Set the page number back to 1
                oGridConfig.nGridPageNumber = 1;
            });
            if (oGridConfig.isMatterView) {
                $(".filterSearchPG, .filterSearchAOL").removeClass("hide");
                $(".filterSearchAuthor").addClass("hide");
            } else {
                $(".filterSearchPG, .filterSearchAOL").addClass("hide");
                $(".filterSearchAuthor").removeClass("hide");
            }
            var sHeaderID = oGridConfig.isMatterView ? "DisplayFindMattersResults" : "DisplaySearchDocumentsResults";
            togglePinnedView($("#" + sHeaderID + " .allSearch"), 0);        // load All view
            togglePinnedView($("#" + sHeaderID + " .pinnedSearch"), 1);     // load Pinned view
            togglePinnedView($("#" + sHeaderID + " .mySearch"), 2);         // load Recent view
            $(".sdBannerText").attr("data-result", "2");                    // Set Recent view as default view
            $(document).off("click", "#searchMatter");
            $(document).off("click", "#searchDocument");
            $("#searchMatter").on("click", function (event) {
                if ($(".switchAppRow.selected").text().toLowerCase() !== $(this).attr("data-attr").toLowerCase()) {
                    toggleView(true);
                }
            });
            $("#searchDocument").on("click", function () {
                if ($(".switchAppRow.selected").text().toLowerCase() !== $(this).attr("data-attr").toLowerCase()) {
                    toggleView(false);
                }
            });
            $("#searchMatterTab").on("click", function () {
                if ($("#headerFlyout .switchAppRow.selected").text().toLowerCase() !== $(this).attr("data-attr").toLowerCase()) {
                    toggleView(true);
                    closeHamburgerMenu();
                }
            });
            $("#searchDocumentTab").on("click", function () {
                if ($("#headerFlyout .switchAppRow.selected").text().toLowerCase() !== $(this).attr("data-attr").toLowerCase()) {
                    toggleView(false);
                    closeHamburgerMenu();
                }
            });
        }
    });
    jQuery.event.props.push("dataTransfer");
    $(document).click(function () {
        commonFunction.closeAllFilterExcept("");
    });
    $("#folderStructure .matterFolderPopupClose").click(function () {
        $(".folderStructureContent, .folderStructureTitle .matterFolderName").empty();
        $("#folderStructure").hide();
    });
    $(document).on("click", ".notificationContainer .notification .closeNotification,.notificationContainerForPopup .notification .closeNotification", function () {
        $(this).parent().remove();
    });
});

function formatDate(dDate) {
    "use strict";
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
    return date;
}

function hideOpenDate(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var sTileContent =
          "<div class=\"openDate hide\">" + cellValue + "</div>";
    return sTileContent;
}

function hideIsUploadStatus(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var sTileContent =
          "<div class=\"isUpload hide\">" + cellValue + "</div>";
    return sTileContent;
}