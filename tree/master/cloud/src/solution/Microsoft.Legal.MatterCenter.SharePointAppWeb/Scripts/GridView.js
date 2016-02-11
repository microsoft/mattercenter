/// <disable>JS1003,JS2005,JS2024,JS2027,JS2031,JS2032,JS2052,JS2053,JS2064,JS2073,JS2074,JS2076,JS2085,JS3054,JS3057,JS3085,JS3092,JS3116,JS3056,JS3058</disable>

var oGridConfig = {
    nColumnCounter: 1,
    nGridWidth: 0,
    nCurrentGridWidth: 0,
    nPreviousGridWidth: 0,
    nCurrentGridHeight: 0,
    nPreviousGridHeight: 0,
    nWidthDiff: 50,
    sortable: "",
};

var oGridViewObject = {
    pageNumber: 1,
    searchResultCount: 0,
    itemsPerPage: oGlobalConstants.GridViewItemsPerPage,
    waitTillDataLoaded: false,
    oCurrentMandatory: "",
    filterData: [],
    nHeaderLeftOffset: 0,
    bDefaultSelectionChanged: false,
    arrUserSelectedColumns: [],
    isPageLoad: 0
};

/* Global functions */

/* Function to detect if vertical scroll bar is present for a particular element */
(function ($) {
    $.fn.hasScrollBar = function () {
        return this.get(0).scrollHeight > this.height();
    };
})(
/// <disable>JS3058</disable>
jQuery);

var toggleView = function (isMatterView) {
    "use strict";
    oCommonObject.isMatterView = isMatterView;
};

$(document).on("click", function () {
    "use strict";
    $(".ms-ContextualMenu").removeClass("is-open");
});

//// #region Formatter functions for grid view control

/* Function to generate check-box column for grid view control*/
function loadCheckboxColumn(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var iCurrentIndex = parseInt(parseInt(oGridViewObject.itemsPerPage, 10) * parseInt(oGridViewObject.pageNumber, 10), 10) + parseInt(nIndex, 10);
    var sCheckBoxChunk = "<div class=\"checkBoxImage checkboxHoverImageContainer hide\" ></div><div class=\"gridCell ms-ChoiceField checkBox\"><input class=\"ms-ChoiceField-input\" id=\"demo-checkbox-unselected" + iCurrentIndex + "\" type=\"checkbox\"/><label class=\"ms-ChoiceField-field\" for=\"demo-checkbox-unselected" + iCurrentIndex + "\"></label></div>";
    return sCheckBoxChunk;
}

/* Function to generate Ellipsis column for grid view control*/
function loadEllipsisColumn(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var sECBMenuTitle = oCommonObject.isMatterView ? oFindMatterConstants.GridViewECBMenuTitle : oFindDocumentConstants.GridViewECBMenuTitle;
    return "<div class='ms-Icon ms-Icon--ellipsis ellipsis' title='" + sECBMenuTitle + "'></div>";
}

/* Function to generate Title column for grid view control*/
function loadNameColumn(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var sTitleProperty = "";
    oGridConfig.nColumnCounter = 1;
    //// If pinned data is requested (flag = 4) get the different set of field names
    if (4 === oCommonObject.iCurrentGridViewData()) {
        sTitleProperty = oCommonObject.isMatterView ? oFindMatterConstants.GridViewPinnedTitleProperty : oFindDocumentConstants.GridViewPinnedTitleProperty;
    } else {
        sTitleProperty = oCommonObject.isMatterView ? oFindMatterConstants.GridViewTitleProperty : oFindDocumentConstants.GridViewTitleProperty;
    }
    var sTitleValue = ($.trim(rowObject[sTitleProperty])) ? $.trim(rowObject[sTitleProperty]) : "NA"
        , sDocumentPath, sDocumentExtension, sDocumentSPWebUrl;
    if (!oCommonObject.isMatterView) {
        //// If pinned data is requested (flag = 4) get the different set of field names
        if (4 === oCommonObject.iCurrentGridViewData()) {
            sDocumentPath = rowObject[oFindDocumentConstants.GridViewDocumentOWAUrl];
        } else {
            sTitleValue = extractTitle(sTitleValue); //// Remove the extension from the document name only if current section is NOT pinned section
            // Document path URL will be available as a property of rowObject in case of pinned documents.
            sDocumentPath = trimEndChar(rowObject[oFindDocumentConstants.GridViewDocumentPath], "/"), sDocumentExtension = rowObject[oFindDocumentConstants.GridViewDocumentExtension], sDocumentSPWebUrl = rowObject[oFindDocumentConstants.GridViewDocumentSPWebUrl];
            if (-1 < $.inArray(sDocumentExtension, oFilterGlobal.arrOWADocumentExtension) && sDocumentSPWebUrl) {
                sDocumentPath = commonFunction.getOWAUrl(sDocumentExtension, sDocumentSPWebUrl, sDocumentPath);
            }
        }
    }
    //// Code chunk to put all returned properties as data- attributes
    var sHTMLChunk = "<div class=\"gridCell nameColumn mandatory ms-font-m ms-font-weight-semibold\" title=\"" + sTitleValue;
    $.each(rowObject, function (iCurrentItem, sCurrentValue) {
        sCurrentValue = ("string" === typeof sCurrentValue) ? sCurrentValue.replace(/"/g, "") : sCurrentValue;
        sHTMLChunk += "\" data-" + iCurrentItem.toLowerCase() + " = \"" + sCurrentValue;
    });
    oCommonObject.isMatterView ? sHTMLChunk += "\"><span class=\"content\">" + sTitleValue + "</span></div>" : sHTMLChunk += "\"><span class=\"content\"><a target=\"_blank\" onclick=\"javascript:return false;\" href=" + sDocumentPath + ">" + sTitleValue + "</a></span></div>";
    return sHTMLChunk;
}

/* Function to generate non-specific columns for grid view control*/
function genericColumn(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    oGridConfig.nColumnCounter++;
    var sCurrentValue = $.trim(cellValue) ? trimEndChar($.trim(cellValue), ";") : "NA";
    var sGenericColumnChunk = "<span class='gridCell ms-font-m dataColumn" + oGridConfig.nColumnCounter + "' title='" + sCurrentValue + "'>" + sCurrentValue + "</span>";
    return sGenericColumnChunk;
}

/* Function to generate Date column for grid view control*/
function loadDateColumn(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    oGridConfig.nColumnCounter++;
    var sCurrentValue = $.trim(cellValue) ? cellValue : "NA"
    , orignalDateValue = sCurrentValue;
    var sDate = "NA" !== sCurrentValue ? oGridView.formatDate(sCurrentValue) : "NA";
    var sGenericColumnChunk = "<span class='gridCell ms-font-m dataColumn" + oGridConfig.nColumnCounter + "' title='" + sDate + "' data-orignalDate='" + orignalDateValue + "'>" + sDate + "</span>";
    return sGenericColumnChunk;
}

/* Function to generate ClientMatterID column for grid view control, content will be rendered as ClientID.MatterID*/
function loadClientMatterID_Column(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    oGridConfig.nColumnCounter++;
    var sMatterIDProperty = "", sClientMatterID = "";
    //// If pinned data is requested (flag = 4) get the different set of field names
    if (4 === oCommonObject.iCurrentGridViewData()) {
        sMatterIDProperty = (oCommonObject.isMatterView) ? oFindMatterConstants.GridViewPinnedClientMatterIDPropertyName : oFindDocumentConstants.GridViewPinnedClientMatterIDPropertyName;
        sClientMatterID = $.trim(rowObject[sMatterIDProperty]) ? $.trim(rowObject[sMatterIDProperty]) : "NA";
    } else {
        sMatterIDProperty = (oCommonObject.isMatterView) ? oFindMatterConstants.GridViewMatterIDPropertyName : oFindDocumentConstants.GridViewMatterIDPropertyName;
        var sClientIDValue = $.trim(cellValue) ? cellValue : "NA"
      , sMatterIDValue = $.trim(rowObject[sMatterIDProperty]) ? $.trim(rowObject[sMatterIDProperty]) : "NA";
        sClientMatterID = sClientIDValue + "." + sMatterIDValue;
    }

    var sClientMatterIDChunk = "<span class='gridCell ms-font-m dataColumn" + oGridConfig.nColumnCounter + "' title='" + sClientMatterID + "'>" + sClientMatterID + "</span>";
    return sClientMatterIDChunk;
}

/* Function to modify checked out to column in case of Search Document grid view */
function loadCheckedOutToUser(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    oGridConfig.nColumnCounter++;
    var sCurrentValue = $.trim(cellValue);
    sCurrentValue = sCurrentValue ? sCurrentValue : "NA";
    var sGenericColumnChunk = "<span class='gridCell ms-font-m dataColumn" + oGridConfig.nColumnCounter + "' title='" + sCurrentValue + "'>" + sCurrentValue + "</span>";
    return sGenericColumnChunk;
}

/* Function to generate Document Type Icon column in case of Search Document grid view */
function loadDocumentTypeIcon(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var sExtension = "", iconSrc = "";
    if (4 === oCommonObject.iCurrentGridViewData()) {
        sExtension = (rowObject && rowObject.DocumentExtension) ? rowObject.DocumentExtension : "";
    } else {
        sExtension = (rowObject && rowObject.FileExtension) ? rowObject.FileExtension : "";
    }
    iconSrc = oCommonObject.getIconSource(sExtension);
    return "<img class='docTypeIcon' id='" + nIndex + "docTypeIcon' src='" + iconSrc + "' alt='' onerror='errorImage(this);'>";
}

/* Function to open matter landing page */
function viewMatter(oElement) {
    "use strict";
    var sMatterURL = $(oElement).attr("data-matterlink");
    oCommonObject.openWindow(sMatterURL, "_blank");
}

//// #endregion

var oGridView = (function () {
    "use strict";
    var oGridViewCommonObject = {
        oGridHeader: [],
        oGridView: []
    },
    oCommonFunctions = {};

    //// #region Grid View Control

    /* Position the filtering fly out */
    oCommonFunctions.positionFilteringFlyout = function (oThisObject, sSelector) {
        var iCaretIconOffset = oThisObject.position().left;
        var iFlyoutLeftPosition = iCaretIconOffset;
        if (iFlyoutLeftPosition > 250) {            //// We checked whether select column is first column or not by getting position of caret icon
            $(sSelector).css({ "left": iFlyoutLeftPosition - $(sSelector).width() + 10 }); //// for aligning filter flyout we add 10px to left because we have caret icon with 10px width
        } else {
            $(sSelector).css({ "left": "0px" });
        }
        //// Set position of text flyout dynamically
        $(sSelector).css({ "top": $("#gridViewContainer").scrollTop() + 40 }); //// Add 40px in top of text flyout because we have 40px header width. 
        $(sSelector).removeClass("hide");
    };

    /* Function to bind events on list  view control */
    oCommonFunctions.addGridViewControlBindings = function () {
        var oGridViewContainer = $("#gridViewContainer");
        if (oGridViewContainer && oGridViewContainer.length) {
            oGridViewContainer.on("scroll", function () {
                oGridView.bindScrollEvent();
            });
            var oLoadingImageContainer = $("#loadingImageContainer");
            if (oLoadingImageContainer && oLoadingImageContainer.length) {
                oGridViewContainer.find(".lazyLoading").remove();
                var sLoadingImageChunk = oLoadingImageContainer.html();
                $("#gridViewContainer_Grid").append(sLoadingImageChunk);
                oLoadingImageContainer.addClass("hide");
            }
        }
        $("#gridViewContainer_Grid").find(".lazyLoading").addClass("hide");

        /* Bind the event for getting values for given column */
        $(".jsonGridHeader .ms-Icon--caretDown").off().on("click", function (event) {
            oCommonObject.arrUniqueRefinerData().length = 0; //// Refresh the array of values for filter fly out
            oCommonObject.closeAllPopupExcept("", event);
            $(".filterFlyoutSection .ms-Label").css("display", "inline"); //// remove fabric's inline style
            $(".filterFlyoutSearchText").val(""); //// Clear the text from the search box
            oCommonObject.iFilterFlyoutPageNumber(); //// Reset the page number for filter fly out data to 1
            oCommonObject.moveFilterFlyout();
            $("#filterResultsContainer").empty(); //// Clear the older filter data
            var oThisObject = $(this), oCurrentHeader = oThisObject.parent();
            if (oCurrentHeader.length) {
                var sRefinerName = oCurrentHeader.attr("id")
                    , sFilterFlyoutType = oCurrentHeader.attr("filterFlyOutType");
                if ($.trim(sRefinerName) && "NA" !== sFilterFlyoutType) {
                    //// Set the clear filter text and title attribute for the filter
                    var oHeaderTitle = oCurrentHeader.find(".headerTitle")
                        , oClearText = $(".clearText")
                        , oClearFilterTitle = $(".clearFilterTitle");
                    //// Set the title for clear text filters
                    if (oHeaderTitle.length && oClearText.length && oClearFilterTitle.length) {
                        oClearFilterTitle.text(oHeaderTitle.text());
                        oClearText.attr("title", $(oClearText[0]).text());
                    }
                    //// Processing for date filters
                    if ("date" === sFilterFlyoutType) {
                        $("#dateFlyoutContent").attr({ "data-refinername": sRefinerName, "data-filterflyouttype": sFilterFlyoutType });
                        oCommonObject.setDateFilters(sRefinerName, sFilterFlyoutType);
                        oCommonFunctions.positionFilteringFlyout(oThisObject, "#dateFlyoutContent");
                    } else { //// Processing for single and multi text filters
                        $("#textFlyoutContent").attr({ "data-refinername": sRefinerName, "data-filterflyouttype": sFilterFlyoutType });
                        oCommonFunctions.positionFilteringFlyout(oThisObject, "#textFlyoutContent");
                        //// If current section is pinned section
                        if (4 === oCommonObject.iCurrentGridViewData()) {
                            //// add code for pinned sections
                            oGridViewObject.filterData = [];
                            var filter = oCommonObject.isMatterView ? oCommonObject.oPinnedFlyoutFilters.oSearchMatterFilters : oCommonObject.oPinnedFlyoutFilters.oSearchDocumentFilters;
                            if (Object.keys(filter).length) {
                                oCommonObject.filterData(filter, true, sRefinerName);
                            } else {
                                var oData = oCommonObject.pinnedFilterData.length ? oCommonObject.pinnedFilterData : oCommonObject.pinnedData;
                                $.each(oData, function (iCount, sCurrentValue) {
                                    var arrSplitValues = [];
                                    if ($.trim(sCurrentValue[sRefinerName])) {
                                        arrSplitValues = sCurrentValue[sRefinerName] && sCurrentValue[sRefinerName].split(";");
                                    }
                                    $.each(arrSplitValues, function (iCurrentCount, sValue) {
                                        var sTrimmedValue = $.trim(sValue);
                                        ($.inArray(sTrimmedValue, oGridViewObject.filterData) === -1) && oGridViewObject.filterData.push(sTrimmedValue);
                                    });
                                });
                            }
                            //// Server side sorting on the filter fly out data
                            oGridViewObject.filterData.sort(function (previousValue, nextValue) {
                                if (isNaN(previousValue) || isNaN(nextValue)) {
                                    return previousValue > nextValue ? 1 : -1;
                                }
                                return previousValue - nextValue;
                            });
                        } else { //// Processing for My and All sections
                            var sFilterSearchTerm = oCommonObject.formatFilterSearchTerm(sRefinerName);
                        }
                        oCommonObject.isSearchText = 1;
                        oCommonObject.highlightSelectedFilters(sRefinerName, sFilterFlyoutType);
                    }
                }
            }
            event && event.stopPropagation();
        });
        $(".jsonGridHeader").hover(function () {
            if ("NA" !== $(this).attr("filterFlyOutType")) {
                $(this).find(".ms-Icon--caretDown").css("opacity", "1");
            }
        }, function () {
            $(this).find(".ms-Icon--caretDown").css("opacity", "0");
        });
    };

    /* Function to generate the grid view JSON */
    oCommonFunctions.generateGridViewJSON = function () {
        var gridViewJSON = [], arrColumnNames = [], arrColumnWidth = [], arrColumnFormatter = [];
        if (oCommonObject.isMatterView) {
            /* Get the data that is to be shown in Search Matter grid view control */
            //// If pinned data is requested (flag = 4) get the different set of field names
            if (4 === oCommonObject.iCurrentGridViewData()) {
                arrColumnNames = $.trim(oFindMatterConstants.GridViewPinnedColumnValueFields) ? $.trim(oFindMatterConstants.GridViewPinnedColumnValueFields).split(";") : "";
                oGridConfig.sortable = true;
            } else {
                arrColumnNames = $.trim(oFindMatterConstants.GridViewColumnValueFields) ? $.trim(oFindMatterConstants.GridViewColumnValueFields).split(";") : "";
                oGridConfig.sortable = false;
            }
            arrColumnWidth = $.trim(oFindMatterConstants.GridViewColumnWidth) ? $.trim(oFindMatterConstants.GridViewColumnWidth).split(";") : "";
            arrColumnFormatter = $.trim(oFindMatterConstants.GridViewColumnFomatter) ? $.trim(oFindMatterConstants.GridViewColumnFomatter).split(";") : "";
        } else {
            /* Get the data that is to be shown in Search Document grid view control */
            //// If pinned data is requested (flag = 4) get the different set of field names
            if (4 === oCommonObject.iCurrentGridViewData()) {
                arrColumnNames = $.trim(oFindDocumentConstants.GridViewPinnedColumnValueFields) ? $.trim(oFindDocumentConstants.GridViewPinnedColumnValueFields).split(";") : "";
                oGridConfig.sortable = true;
            } else {
                arrColumnNames = $.trim(oFindDocumentConstants.GridViewColumnValueFields) ? $.trim(oFindDocumentConstants.GridViewColumnValueFields).split(";") : "";
                oGridConfig.sortable = false;
            }
            arrColumnWidth = $.trim(oFindDocumentConstants.GridViewColumnWidth) ? $.trim(oFindDocumentConstants.GridViewColumnWidth).split(";") : "";
            arrColumnFormatter = $.trim(oFindDocumentConstants.GridViewColumnFomatter) ? $.trim(oFindDocumentConstants.GridViewColumnFomatter).split(";") : "";
        }
        if (arrColumnNames && arrColumnNames.length && arrColumnWidth && arrColumnWidth.length && arrColumnFormatter && arrColumnFormatter.length) {
            /* Generate the column structure that is to be shown in the grid view control */
            $.each(arrColumnNames, function (iCurrentIndex, sCurrentValue) {
                var oCurrentItem = {};
                if ("ECB" === sCurrentValue) {
                    oCurrentItem = { name: sCurrentValue, width: arrColumnWidth[iCurrentIndex], id: sCurrentValue, align: "center", trimOnOverflow: false, formatter: arrColumnFormatter[iCurrentIndex], sortable: oGridConfig.sortable, sortType: String };
                } else {
                    oCurrentItem = { name: sCurrentValue, width: arrColumnWidth[iCurrentIndex], id: sCurrentValue, align: "left", trimOnOverflow: false, formatter: arrColumnFormatter[iCurrentIndex], sortable: oGridConfig.sortable, sortType: String };
                }
                gridViewJSON.push(oCurrentItem);
            });
            return gridViewJSON;
        }
        return [];
    };

    /* Function to load grid view control */
    oCommonFunctions.createGridView = function (oDataToLoad, bIsPageLoadData) {
        //// Get the grid view JSON based on the current page
        var gridViewJSON = oCommonFunctions.generateGridViewJSON();
        if (gridViewJSON && gridViewJSON.length) {
            var arrHeaderData, arrHeaderDataTitle;
            if (oCommonObject.isMatterView) {
                arrHeaderData = oFindMatterConstants.GridViewHeaderName.split(";");
                arrHeaderDataTitle = oFindMatterConstants.GridViewHeaderNameTitle.split("$|$");
            } else {
                arrHeaderData = oFindDocumentConstants.GridViewHeaderName.split(";");
                arrHeaderDataTitle = oFindDocumentConstants.GridViewHeaderNameTitle.split("$|$");
            }
            var oHeaderNames = [], oHeaderFilterType = [];
            $.each(arrHeaderData, function (iItem, sCurrentValue) {
                var oCurrentHeader = sCurrentValue && sCurrentValue.split(",");
                oHeaderNames.push(oCurrentHeader[0]);
                oCommonObject.oHeaderFilterType.push(oCurrentHeader[1]);
            });
            var sortBy = (oCommonObject.isMatterView) ? oFindMatterConstants.GridViewTitleProperty : oFindDocumentConstants.GridViewTitleProperty;
            //// Generate the common JSON structure for generating the grid view control
            var GridConfig = {
                container: "gridViewContainer",
                data: oDataToLoad,
                gridName: "Grid View",
                gridHeader: oHeaderNames,
                gridHeaderTitle: arrHeaderDataTitle,
                columnNames: gridViewJSON,
                sortby: "",
                sortorder: "asc",
                sortType: String,
                initialsortorder: "",
                retainpageonsort: false,
                maxRows: oGridViewObject.itemsPerPage,
                viewrecords: true,
                pagination: false,
                cellSpacing: 0
            };
            if (bIsPageLoadData) {
                //// Generate the grid view grid for page load data
                oCommonObject.isAllRowSelected = false;
                new oGrid.JsonGrid(GridConfig);
                $(".jsonGridHeader").addClass("ms-font-m-plus");
                oGridViewObject.nHeaderLeftOffset = $("#gridViewContainer_Grid thead").length ? $("#gridViewContainer_Grid thead")[0].offsetLeft : "";
            } else {
                //// Generate the grid view table without header for lazy load data
                var oAdditionalProperties = {
                    currentPage: 0,
                    tblBody: $("#gridViewContainer tbody")[0],
                    drilldown: false
                };

                //// Update the JSON object to extend additional properties for generating grid view
                GridConfig = $.extend(GridConfig, oAdditionalProperties);
                new oGrid.CreateHTMLTableRow(GridConfig);
                $("#gridViewContainer_Grid").find(".lazyLoading").addClass("hide");

                //// Update the data present in the grid view object to have data loaded by lazy load
                $.merge(oGrid.gridObject[0].data, oDataToLoad);
                oGrid.gridObject[0].maxRows = oGrid.gridObject[0].data.length;
                oGridViewObject.waitTillDataLoaded = false;
            }
            $(window).trigger("resize");
            oCommonObject.configureOnLoadView();
            oCommonFunctions.addGridViewControlBindings();
        }
    };

    /* Function to load grid view control */
    oCommonFunctions.loadGridView = function (sDataToLoad, bIsPageLoadData) {
        var oDataToLoad = "", oGridViewContainer = $("#gridViewContainer_Grid");
        if ("null" !== sDataToLoad) {
            oDataToLoad = JSON.parse(sDataToLoad);
        } else {
            oDataToLoad = JSON.parse(oGridViewCommonObject.sData);
        }
        if (4 === oCommonObject.iCurrentGridViewData()) {
            $.each(oDataToLoad, function (key, value) {
                var sMatterId = oCommonObject.isMatterView ? oFindMatterConstants.GridViewPinnedMatterIDPropertyName : oFindDocumentConstants.GridViewPinnedMatterIDPropertyName
                , sClientId = oCommonObject.isMatterView ? oFindMatterConstants.GridViewPinnedMatterClientIDPropertyName : oFindDocumentConstants.GridViewPinnedDocumentClientIDPropertyName;
                value.CalClientMatterId = value[sClientId] + "." + value[sMatterId];
            });
            oCommonObject.pinnedData = oDataToLoad;

        }
        if (!(oGridViewContainer.length && 4 === oCommonObject.iCurrentGridViewData())) {
            oCommonFunctions.createGridView(oDataToLoad, bIsPageLoadData);
        }
        oCommonFunctions.bindECB();
        oCommonObject.enableSearch();
    };

    /* Function to bind ECB control */
    oCommonFunctions.bindECB = function () {
        /* First unbind the click event and re-bind the ECB control to all the rows present in the grid view control */
        $(".ellipsis").off().on("click", function (event) {
            var oCurrentRow = $(this).parents(".GridRow"), sECBHTMLChunk = {}, oCheckBoxList = {}; //// Get the parent row with class GridRow
            $(".ECBControlClass").remove();
            sECBHTMLChunk = $("#ECBControl").length && $("#ECBControl").clone() && $("#ECBControl").clone()[0];
            $(sECBHTMLChunk).removeClass("hide").addClass("ECBControlClass");
            $("#gridViewContainer").append(sECBHTMLChunk);
            /* Function to bind ECB controls */
            oCommonObject.addECBBindings();
            var $AllGridViewItems = $(".isSelectRowsActive");
            if ($AllGridViewItems && $AllGridViewItems.length && $AllGridViewItems[0].checked) {
                $AllGridViewItems.prop("checked", false);
                oCommonObject.isAllRowSelected = false;
            }
            var oGridViewRows = $(".GridRow, .GridRowAlternate");
            if (oGridViewRows.length) {
                oGridViewRows.removeClass("is-selectedRow");
                $(".ellipsis").parent().removeClass("ellipsisBackground");
                $(".checkBoxImage").removeClass("checkboxHoverImageContainer checkboxImageContainer").addClass("hide");
                $(".checkBox").removeClass("hide");

                oCheckBoxList = oGridViewRows.find(".ms-ChoiceField-input");
                if (oCheckBoxList && oCheckBoxList.length) {
                    $.each(oCheckBoxList, function (iCurrentItem, sCurrentValue) {
                        sCurrentValue.checked = false;
                    });
                }
                var oCurrentRow = $(this).parent().parent();
                if (oCurrentRow.length) {
                    oCurrentRow.addClass("is-selectedRow");
                    $(this).parent().addClass("ellipsisBackground");
                    var currentCheckbox = $(this).parent().parent().find(".ms-ChoiceField-input");
                    if (currentCheckbox && currentCheckbox.length) {
                        currentCheckbox[0].checked = true;
                        oCurrentRow.find(".checkBoxImage").removeClass("hide checkboxHoverImageContainer").addClass("checkboxImageContainer");
                        oCurrentRow.find(".checkBox").addClass("hide");
                        $("#attachDocuments").removeClass("is-disabled");
                    }
                }
            }
            !(oCurrentRow && oCurrentRow.length) ? oCurrentRow = $(this).parents(".GridRowAlternate") : ""; //// if GridRow is not present check for GridRowAlternate
            var oContextualMenu = $(".ms-ContextualMenu");
            oGridViewObject.oCurrentMandatory = oCurrentRow.find(".mandatory"); //// Assign to global object so that it can used for reference from outer JS files as well
            if (oCommonObject.isMatterView) {
                updateECBforMatter(oContextualMenu, oGridViewObject.oCurrentMandatory);
            } else {
                updateECBforDocument(oContextualMenu, oGridViewObject.oCurrentMandatory);
            }
            oContextualMenu.find(".ms-ContextualMenu-link").removeClass("is-selected");
            //// Position the ECB fly out on the current ellipses icon and display the fly out
            var nDynamicWidth = oCommonObject.getWidth()
            , iEllipsesOffsetFromTop = $(this).position().top
            , iECBMenuOffsetFromTop = iEllipsesOffsetFromTop
            , iGridViewOffsetFromTop = $("#gridViewContainer").offset().top + $("#gridViewContainer").height(); //// Adjust ECB menu relative to grid view control (Kept commented for future purpose)
            nDynamicWidth = (450 < nDynamicWidth) ? $(this).position().left - 5 : (parseInt($(this).position().left + 10, 10) - parseInt(oContextualMenu.width(), 10)) + 15;
            $("#ECBControl").css({
                "left": nDynamicWidth, "top": iEllipsesOffsetFromTop + 23
            }); //// Add 10px to adjust the as per height of ellipses
            oContextualMenu.addClass("is-open");
            oCommonObject.closeAllPopupExcept("ms-ContextualMenu", event); //// Close all pop-ups except ECB menu            
            (event) ? event.stopPropagation() : "";
        });
    };

    /* Function to bind the scroll event on the element */
    oCommonFunctions.bindScrollEvent = function () {
        if (4 !== oCommonObject.iCurrentGridViewData()) {  //// No lazy loading for pinned section (flag = 4)
            if (!oGridViewObject.waitTillDataLoaded && ($("#gridViewContainer").scrollTop() + $("#gridViewContainer").height() > ($("#gridViewContainer_Grid").height() - (1 - (oGlobalConstants.GridViewLazyLoadingLimit / 100)) * $("#gridViewContainer_Grid").height()))) {
                var iMaxCurrentData = parseInt(oGridViewObject.itemsPerPage) * (oGridViewObject.pageNumber);
                if (oGridViewObject.searchResultCount > iMaxCurrentData) {
                    oGridViewObject.waitTillDataLoaded = true; //// Throttle the next request till current service request is completed
                    $("#gridViewContainer_Grid").find(".lazyLoading").removeClass("hide"); //// Display the loading image
                    $("#gridViewContainer .jsonGridHeader").attr("disabled", "disabled").addClass("disableHeader");////disable the grid header for lazy loading
                    oGridViewObject.pageNumber++;
                    if (oCommonObject.isMatterView) {
                        getSearchMatters("#gridView", 1);
                    } else {
                        getSearchDocuments("#gridView", 1);
                    }
                }
            }
        }
        $("#gridViewContainer_Grid thead").css("left", oGridViewObject.nHeaderLeftOffset - $("#gridViewContainer").scrollLeft());
        $("#gridViewContainer_Grid thead").width(($(document).width() - 37) + $("#gridViewContainer").scrollLeft());       //// set header width according to document width(after Deducting 37px which is width of column picker notch) and scroll of grid view container
        if (320 > window.innerWidth) {
            $("#gridViewContainer_Grid thead").css("left", (-($("body").scrollLeft())) + oGridViewObject.nHeaderLeftOffset); //// if there is horizontal scroll bar then set header left according to that scrollbar
        }
    };

    $(document).on("scroll", function (event) {
        $("#gridViewContainer_Grid thead") && $("#gridViewContainer_Grid thead").css("left", oGridViewObject.nHeaderLeftOffset - $(document).scrollLeft());
    });

    /* Function to trigger event after window resize */
    var waitForFinalEvent = (function () {
        "use strict";
        var timers = {
        };
        return function (callback, ms, uniqueId) {
            if (timers[uniqueId]) {
                clearTimeout(timers[uniqueId]);
            }
            timers[uniqueId] = setTimeout(callback, ms);
        };
    })();
    //// #endregion

    $(window).on("resize", function (event) {
        "use strict";
        oCommonObject.closeAllPopupExcept("", event); //// Close all pop-ups
        oCommonFunctions.adjustGridViewHeight();
        oCommonObject.adjustCaretIcon();
        //// Adjust the position of column picker as per presence of scroll
        if ($("#gridViewContainer").hasScrollBar()) {
            $("#columnPickerStrip, #columnPickerControl, #columnPickerContainer").removeClass("noScrollPadding").addClass("scrollPadding");
        } else {
            $("#columnPickerStrip, #columnPickerControl, #columnPickerContainer").removeClass("scrollPadding").addClass("noScrollPadding");
        }
        $("#gridViewContainer_Grid thead").css("width", ($(document).width() - 37) + $("#gridViewContainer").scrollLeft());  //// set header width according to document width(after Deducting 37px which is width of column picker notch) and scroll of grid view container
        waitForFinalEvent(function () {
            "use strict";
            //// function call of change grid view container height dynamically
            oCommonFunctions.applyMapContainerHeight();
        }, 500, "String");
    });

    oCommonFunctions.adjustGridViewHeight = function () {
        var nCurrentWindowHeight = window.innerHeight;
        var nCurrentWindowWidth = window.innerWidth;
        //// check whether window height is less then 190px or not
        //// if yes then set container height to 155
        //// else set container height according to windows height 
        if (parseInt(nCurrentWindowHeight) <= 155) {
            $("#gridViewContainer").height(55);
        } else {
            var oAttachButton = $("#attachDocuments"), iButtonHeight = 0;
            if (oAttachButton && oAttachButton.length) {
                iButtonHeight = (oAttachButton.attr("data-applicable")) ? 53 : 0;
            }
            //// If width is less than 320 px, then two scroll bars will appear and that's why we have this condition
            if (320 > nCurrentWindowWidth) {
                $("#gridViewContainer").css("height", parseInt(nCurrentWindowHeight, 10) - (109 + iButtonHeight)); //// Deducting 109px from windows height in order to remove the height of App header section and height of app scrollbar
            } else {
                $("#gridViewContainer").css("height", parseInt(nCurrentWindowHeight, 10) - (92 + iButtonHeight)); //// Deducting 92px from windows height in order to remove the height of App header section
            }
            $("#columnPickerStrip").css("height", parseInt(nCurrentWindowHeight, 10) - (111 + iButtonHeight)); //// Deducting 111px from windows height in order to remove the height of App header section and horizontal scrollbar for grid view
        }
    };
    /* Function to set Grid View height dynamically according to window height*/
    oCommonFunctions.applyMapContainerHeight = function () {
        oGridConfig.nPreviousGridHeight = oGridConfig.nCurrentGridHeight;
        oGridConfig.nCurrentGridHeight = window.innerHeight;
        //// check height of previous grid and current grid
        //// oCommonObject.isServiceCallComplete used for check whether any service is currently running or not
        //// 4 != oCommonObject.iCurrentGridViewData() used for check that there should not pinned matter/document data in grid view
        if (oGridConfig.nPreviousGridHeight !== oGridConfig.nCurrentGridHeight && 4 !== oCommonObject.iCurrentGridViewData()) { // && oCommonObject.isServiceCallComplete) {            
            if (oGridConfig.nCurrentGridHeight >= parseInt(oGlobalConstants.GridView_BreakPoint, 10)) {
                if (oGridConfig.nPreviousGridHeight < parseInt(oGlobalConstants.GridView_BreakPoint, 10)) {
                    oCommonObject.abortRequest();
                    ////if height of container is more then 710px then load records according to height of browser window
                    //// equation : items per page = items per page * ( height of browser window / 710 ) + 1
                    //// In 710px height we can show 21 list item in page 
                    oCommonObject.isAllRowSelected = false;
                    oGridViewObject.itemsPerPage = oGridViewObject.itemsPerPage * (Math.floor((parseInt(oGridConfig.nCurrentGridHeight, 10) / parseInt(oGlobalConstants.GridView_BreakPoint, 10))) + 1);
                    oCommonObject.clearGridViewContent();
                    // Update the grid view for matters/documents
                    oCommonObject.updateGridView();
                }
            } else if (oGridConfig.nPreviousGridHeight >= oGlobalConstants.GridView_BreakPoint || 0 === oGridConfig.nPreviousGridHeight) {
                ////oGridConfig.nPreviousGridHeight === 0 is checked for page load                
                //// set itemsperpage = 21                
                oCommonObject.abortRequest();
                oCommonObject.isAllRowSelected = false;
                oGridViewObject.itemsPerPage = oGlobalConstants.GridViewItemsPerPage;
                if (1 === oGridViewObject.pageNumber || oGridConfig.nCurrentGridHeight <= parseInt(oGlobalConstants.GridView_BreakPoint, 10)) {
                    oCommonObject.clearGridViewContent();
                }
                if (4 === oCommonObject.iCurrentGridViewData()) {
                    //// call pinned matters/documents
                    (oCommonObject.isMatterView) ? getPinnedMatters($("#gridView")) : getPinnedDocument($("#gridView"));
                } else {
                    oCommonObject.updateGridView();
                }
            }
        }
        //// If window height goes below 460px, dynamically updating the height of column picker panel. If not, removing in-line height, which will in turn apply height from css
        if (460 >= parseInt(oGridConfig.nCurrentGridHeight)) {
            $("#columnPickerBlock").height(parseInt(oGridConfig.nCurrentGridHeight) - 160);    //// Deducting 160px from windows height in order to remove the height of App header section and 40px margin of column picker panel
            $("#columnPickerPanel").height(parseInt(oGridConfig.nCurrentGridHeight) - 120); //// Deducting 120px from windows height in order to remove the height of App header section
            $("#ContextualHelp .ContextualHelpContainer").height(parseInt(oGridConfig.nCurrentGridHeight) - 20); //// Deducting 20px from windows height in order to avoid app scroll bar
        } else {
            $("#columnPickerPanel,#columnPickerBlock,#ContextualHelp .ContextualHelpContainer").css("height", "");
        }
    };

    /* Function to get the initial width of the grid view control */
    oCommonFunctions.getGridViewWidth = function () {
        var arrGridWidthValues = oCommonObject.isMatterView ? oFindMatterConstants.GridViewColumnWidth.split(";") : oFindDocumentConstants.GridViewColumnWidth.split(";");
        oGridConfig.nCurrentGridWidth = oGridConfig.nPreviousGridWidth = arrGridWidthValues.reduce(function (previousValue, currentValue) { return parseInt(previousValue) + parseInt(currentValue); }) + oGridConfig.nWidthDiff;
    };

    /* Function to load the ECB control and register ECB actions */
    oCommonFunctions.loadECBControl = function () {
        var arrCurrentView, arrECBTitle;
        /* Get the list of actions to be displayed in the ECB menu based on current page */
        if (oCommonObject.isMatterView) {
            arrCurrentView = $.trim(oFindMatterConstants.GridViewECBActions) ? $.trim(oFindMatterConstants.GridViewECBActions).split(";") : "";
            arrECBTitle = oFindMatterConstants.GridViewECBActionsTitle ? (oFindMatterConstants.GridViewECBActionsTitle).split("$|$") : "";
        } else {
            arrCurrentView = $.trim(oFindDocumentConstants.GridViewECBActions) ? $.trim(oFindDocumentConstants.GridViewECBActions).split(";") : "";
            arrECBTitle = oFindDocumentConstants.GridViewECBActionsTitle ? (oFindDocumentConstants.GridViewECBActionsTitle).split("$|$") : "";
        }

        /* Generate the ECB control based on the current page */
        (arrCurrentView && arrECBTitle && arrCurrentView.length == arrECBTitle.length) ? oECBControl.generateECBControl(arrCurrentView, arrECBTitle) : "";

        /* Function to bind ECB controls */
        oCommonObject.addECBBindings();
    };

    /* Function to convert the date in specific format For now only MMM DD, YYYY format is supported*/
    oCommonFunctions.formatDate = function (dDate) {
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
    };
    oCommonFunctions.highlightGridViewRow = function ($RowElement, flag) {
        var oGridRow = {}, oCheckBox = {}, oCheckImage = {}, oDocumentCheckbox = {};
        oGridRow = $RowElement;
        oCheckBox = oGridRow.find(".checkBox");
        oCheckImage = oGridRow.find(".checkBoxImage");
        oDocumentCheckbox = oGridRow.find(".ms-ChoiceField-input");
        if (flag) {
            oGridRow.addClass("is-selectedRow");
            oGridRow.find(".ellipsis").parent().addClass("ellipsisBackground");
            if (oDocumentCheckbox && oDocumentCheckbox.length) {
                oDocumentCheckbox.prop("checked", true);
                oCheckBox.addClass("hide");
                oCheckImage.removeClass("checkboxHoverImageContainer hide").addClass("checkboxImageContainer");
            }
        } else {
            oGridRow.removeClass("is-selectedRow");
            oGridRow.find(".ellipsis").parent().removeClass("ellipsisBackground");
            if (oDocumentCheckbox && oDocumentCheckbox.length) {
                oDocumentCheckbox.prop("checked", false);
                oGridRow.find(".checkboxHoverImageContainer").addClass("hide");
                oCheckBox.removeClass("hide");
                oCheckImage.removeClass("checkboxHoverImageContainer checkboxImageContainer").addClass("hide");
            }
        }
        var oSelectedRows = $(".is-selectedRow");
        (oSelectedRows && oSelectedRows.length) ? $("#attachDocuments").removeClass("is-disabled") : $("#attachDocuments").addClass("is-disabled");
    };

    $(document).ready(function () {
        /* Display the header */
        $(".AppHeader").removeClass("hide");
        $(document).on("click", ".jsonGridHeader:not('#DocType, #CheckBox ,#ECB')", function () {
            var oThisObject = $(this), oGridRow = $(".GridRow, .GridRowAlternate");
            oCommonObject.isAllRowSelected = false;
            if (4 === oCommonObject.iCurrentGridViewData()) {
                $(".sort").addClass("hide");
                if ("asc" === oThisObject.attr("sortorder")) {
                    oThisObject.find(".sort").html("&#x2191;").removeClass("hide");
                } else {
                    oThisObject.find(".sort").html("&#x2193;").removeClass("hide");
                }
                $(".isSelectRowsActive").prop("checked", false);
                oCommonFunctions.highlightGridViewRow(oGridRow, false);
                $("#attachDocuments").addClass("is-disabled");
                oGridView.bindECB();
            } else {
                var sRefinerName = $.trim(oThisObject.attr("id")) //// This is the name of the property on which the sort is to be applied
                    , oCurrentSortObject = oCommonObject.oSortDetails();
                if (oCurrentSortObject.ByProperty === sRefinerName) { //// If same column is clicked
                    oCurrentSortObject.Direction = (oCurrentSortObject.Direction) ? 0 : 1;
                } else {
                    oCurrentSortObject.Direction = 0; //// Ascending sort by default on first click
                }
                oCurrentSortObject.ByProperty = sRefinerName ? sRefinerName : "";
                oCommonObject.clearGridViewContent();
                if (oCommonObject.isMatterView) {
                    getSearchMatters("#gridView", 1);
                } else {
                    getSearchDocuments("#gridView", 1);
                }
            }
        });
        $(document).on("click", ".GridRow, .GridRowAlternate", function (event) {
            var oDocumentCheckbox = {}, oCheckBox = {}, oCheckImage = {}, oGridRow = $(this);
            if (event && event.target && (0 > event.target.className.indexOf("field"))) {
                if (oCommonObject.isMatterView && !oGridRow.hasClass("is-selectedRow")) {
                    $(".GridRow, .GridRowAlternate").removeClass("is-selectedRow");
                    $(".ellipsis").parent().removeClass("ellipsisBackground");
                }
                if (oGridRow.hasClass("is-selectedRow")) {
                    oCommonFunctions.highlightGridViewRow(oGridRow, false);
                } else {
                    oCommonFunctions.highlightGridViewRow(oGridRow, true);
                }
            }
            oCommonObject.onGridViewCheckboxClick();
        });

        $(document).on("click", ".checkboxHoverImageContainer", function (event) {
            var oGridRow = $(this).parent().parent();
            if (oGridRow && oGridRow.length) {
                oGridRow.click();
            }
            event ? event.stopPropagation() : "";
        });

        $(document).on("mouseover", ".GridRow, .GridRowAlternate", function (event) {
            var oGridRow = $(this);
            oGridRow.find(".checkBox").addClass("hide");
            oGridRow.find(".checkBoxImage").removeClass("hide checkboxImageContainer").addClass("checkboxHoverImageContainer");
            oGridRow.find(".ellipsis").parent().removeClass("ellipsisBackground").addClass("ellipsisHoverBackground");

        }).on("mouseout", ".GridRow, .GridRowAlternate", function (event) {
            var oGridRow = $(this);
            oGridRow.find(".ellipsis").parent().removeClass("ellipsisHoverBackground");
            if (oGridRow.hasClass("is-selectedRow")) {
                oGridRow.find(".checkBoxImage").removeClass("checkboxHoverImageContainer hide").addClass("checkboxImageContainer");
                oGridRow.find(".ellipsis").parent().removeClass("ellipsisHoverBackground").addClass("ellipsisBackground");
            } else {
                oGridRow.find(".checkBoxImage").addClass("hide");
                oGridRow.find(".checkBox").removeClass("hide");
            }
        });

        $(document).on("click", ".searchPanelDropdownOption", function (event) {
            $(".searchPanelDropdownOption").removeClass("selectedDropdownOption");
            $(this).addClass("selectedDropdownOption");
        });
    });

    return ({
        loadGridView: function (sDataToLoad, bIsPageLoadData) {
            oCommonFunctions.loadGridView(sDataToLoad, bIsPageLoadData);
        },
        scrollGridView: function ($Element) {
            return oCommonFunctions.scrollGridView($Element);
        },
        highlightGridViewRow: function ($RowElement, bFlag) {
            oCommonFunctions.highlightGridViewRow($RowElement, bFlag);
        },
        loadECBControl: function () {
            oCommonFunctions.loadECBControl();
        },
        getGridViewWidth: function () {
            oCommonFunctions.getGridViewWidth();
        },
        formatDate: function (dDate) {
            return oCommonFunctions.formatDate(dDate);
        },
        bindScrollEvent: function () {
            oCommonFunctions.bindScrollEvent();
        },
        adjustGridViewHeight: function () {
            oCommonFunctions.adjustGridViewHeight();
        },
        applyMapContainerHeight: function () {
            oCommonFunctions.applyMapContainerHeight();
        },
        generateGridViewJSON: function () {
            return oCommonFunctions.generateGridViewJSON();
        },
        bindECB: function () {
            return oCommonFunctions.bindECB();
        },
        addGridViewControlBindings: function () {
            return oCommonFunctions.addGridViewControlBindings();
        }
    });
})();
