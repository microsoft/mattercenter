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

var oListViewObject = {
    pageNumber: 1,
    searchResultCount: 0,
    itemsPerPage: oGlobalConstants.ListViewItemsPerPage,
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

//// #region Formatter functions for list view control

/* Function to generate check-box column for list view control*/
function loadCheckboxColumn(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var iCurrentIndex = parseInt(parseInt(oListViewObject.itemsPerPage, 10) * parseInt(oListViewObject.pageNumber, 10), 10) + parseInt(nIndex, 10);
    var sCheckBoxChunk = "<div class=\"checkBoxImage checkboxHoverImageContainer hide\" ></div><div class=\"gridCell ms-ChoiceField checkBox\"><input class=\"ms-ChoiceField-input\" id=\"demo-checkbox-unselected" + iCurrentIndex + "\" type=\"checkbox\"/><label class=\"ms-ChoiceField-field\" for=\"demo-checkbox-unselected" + iCurrentIndex + "\"></label></div>";
    return sCheckBoxChunk;
}

/* Function to generate Ellipsis column for list view control*/
function loadEllipsisColumn(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var sECBMenuTitle = oCommonObject.isMatterView ? oFindMatterConstants.ListViewECBMenuTitle : oFindDocumentConstants.ListViewECBMenuTitle;
    return "<div class='ms-Icon ms-Icon--ellipsis ellipsis' title='" + sECBMenuTitle + "'></div>";
}

/* Function to generate Title column for list view control*/
function loadNameColumn(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var sTitleProperty = "";
    oGridConfig.nColumnCounter = 1;
    //// If pinned data is requested (flag = 4) get the different set of field names
    if (4 === oCommonObject.iCurrentListViewData()) {
        sTitleProperty = oCommonObject.isMatterView ? oFindMatterConstants.ListViewPinnedTitleProperty : oFindDocumentConstants.ListViewPinnedTitleProperty;
    } else {
        sTitleProperty = oCommonObject.isMatterView ? oFindMatterConstants.ListViewTitleProperty : oFindDocumentConstants.ListViewTitleProperty;
    }
    var sTitleValue = ($.trim(rowObject[sTitleProperty])) ? $.trim(rowObject[sTitleProperty]) : "NA"
        , sDocumentPath, sDocumentExtension, sDocumentSPWebUrl;
    if (!oCommonObject.isMatterView) {
        //// If pinned data is requested (flag = 4) get the different set of field names
        if (4 === oCommonObject.iCurrentListViewData()) {
            sDocumentPath = rowObject[oFindDocumentConstants.ListViewDocumentOWAUrl];
        } else {
            sTitleValue = extractTitle(sTitleValue); //// Remove the extension from the document name only if current section is NOT pinned section
            // Document path URL will be available as a property of rowObject in case of pinned documents.
            sDocumentPath = trimEndChar(rowObject[oFindDocumentConstants.ListViewDocumentPath], "/"), sDocumentExtension = rowObject[oFindDocumentConstants.ListViewDocumentExtension], sDocumentSPWebUrl = rowObject[oFindDocumentConstants.ListViewDocumentSPWebUrl];
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

/* Function to generate non-specific columns for list view control*/
function genericColumn(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    oGridConfig.nColumnCounter++;
    var sCurrentValue = $.trim(cellValue) ? trimEndChar($.trim(cellValue), ";") : "NA";
    var sGenericColumnChunk = "<span class='gridCell ms-font-m dataColumn" + oGridConfig.nColumnCounter + "' title='" + sCurrentValue + "'>" + sCurrentValue + "</span>";
    return sGenericColumnChunk;
}

/* Function to generate Date column for list view control*/
function loadDateColumn(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    oGridConfig.nColumnCounter++;
    var sCurrentValue = $.trim(cellValue) ? cellValue : "NA"
    , orignalDateValue = sCurrentValue;
    var sDate = "NA" !== sCurrentValue ? oListView.formatDate(sCurrentValue) : "NA";
    var sGenericColumnChunk = "<span class='gridCell ms-font-m dataColumn" + oGridConfig.nColumnCounter + "' title='" + sDate + "' data-orignalDate='" + orignalDateValue + "'>" + sDate + "</span>";
    return sGenericColumnChunk;
}

/* Function to generate ClientMatterID column for list view control, content will be rendered as ClientID.MatterID*/
function loadClientMatterID_Column(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    oGridConfig.nColumnCounter++;
    var sMatterIDProperty = "", sClientMatterID = "";
    //// If pinned data is requested (flag = 4) get the different set of field names
    if (4 === oCommonObject.iCurrentListViewData()) {
        sMatterIDProperty = (oCommonObject.isMatterView) ? oFindMatterConstants.ListViewPinnedClientMatterIDPropertyName : oFindDocumentConstants.ListViewPinnedClientMatterIDPropertyName;
        sClientMatterID = $.trim(rowObject[sMatterIDProperty]) ? $.trim(rowObject[sMatterIDProperty]) : "NA";
    } else {
        sMatterIDProperty = (oCommonObject.isMatterView) ? oFindMatterConstants.ListViewMatterIDPropertyName : oFindDocumentConstants.ListViewMatterIDPropertyName;
        var sClientIDValue = $.trim(cellValue) ? cellValue : "NA"
      , sMatterIDValue = $.trim(rowObject[sMatterIDProperty]) ? $.trim(rowObject[sMatterIDProperty]) : "NA";
        sClientMatterID = sClientIDValue + "." + sMatterIDValue;
    }

    var sClientMatterIDChunk = "<span class='gridCell ms-font-m dataColumn" + oGridConfig.nColumnCounter + "' title='" + sClientMatterID + "'>" + sClientMatterID + "</span>";
    return sClientMatterIDChunk;
}

/* Function to modify checked out to column in case of Search Document list view */
function loadCheckedOutToUser(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    oGridConfig.nColumnCounter++;
    var sCurrentValue = $.trim(cellValue);
    sCurrentValue = sCurrentValue ? sCurrentValue : "NA";
    var sGenericColumnChunk = "<span class='gridCell ms-font-m dataColumn" + oGridConfig.nColumnCounter + "' title='" + sCurrentValue + "'>" + sCurrentValue + "</span>";
    return sGenericColumnChunk;
}

/* Function to generate Document Type Icon column in case of Search Document list view */
function loadDocumentTypeIcon(cellValue, rowObject, width, nIndex, event) {
    "use strict";
    var sExtension = "", iconSrc = "";
    if (4 === oCommonObject.iCurrentListViewData()) {
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

var oListView = (function () {
    "use strict";
    var oListViewCommonObject = {
        oListHeader: [],
        oListView: []
    },
    oCommonFunctions = {};

    //// #region List View Control

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
        $(sSelector).css({ "top": $("#listViewContainer").scrollTop() + 40 }); //// Add 40px in top of text flyout because we have 40px header width. 
        $(sSelector).removeClass("hide");
    };

    /* Function to bind events on list  view control */
    oCommonFunctions.addListViewControlBindings = function () {
        var oListViewContainer = $("#listViewContainer");
        if (oListViewContainer && oListViewContainer.length) {
            oListViewContainer.on("scroll", function () {
                oListView.bindScrollEvent();
            });
            var oLoadingImageContainer = $("#loadingImageContainer");
            if (oLoadingImageContainer && oLoadingImageContainer.length) {
                oListViewContainer.find(".lazyLoading").remove();
                var sLoadingImageChunk = oLoadingImageContainer.html();
                $("#listViewContainer_Grid").append(sLoadingImageChunk);
                oLoadingImageContainer.addClass("hide");
            }
        }
        $("#listViewContainer_Grid").find(".lazyLoading").addClass("hide");

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
                        if (4 === oCommonObject.iCurrentListViewData()) {
                            //// add code for pinned sections
                            oListViewObject.filterData = [];
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
                                        ($.inArray(sTrimmedValue, oListViewObject.filterData) === -1) && oListViewObject.filterData.push(sTrimmedValue);
                                    });
                                });
                            }
                            //// Server side sorting on the filter fly out data
                            oListViewObject.filterData.sort(function (previousValue, nextValue) {
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

    /* Function to generate the list view JSON */
    oCommonFunctions.generateListViewJSON = function () {
        var listViewJSON = [], arrColumnNames = [], arrColumnWidth = [], arrColumnFormatter = [];
        if (oCommonObject.isMatterView) {
            /* Get the data that is to be shown in Search Matter list view control */
            //// If pinned data is requested (flag = 4) get the different set of field names
            if (4 === oCommonObject.iCurrentListViewData()) {
                arrColumnNames = $.trim(oFindMatterConstants.ListViewPinnedColumnValueFields) ? $.trim(oFindMatterConstants.ListViewPinnedColumnValueFields).split(";") : "";
                oGridConfig.sortable = true;
            } else {
                arrColumnNames = $.trim(oFindMatterConstants.ListViewColumnValueFields) ? $.trim(oFindMatterConstants.ListViewColumnValueFields).split(";") : "";
                oGridConfig.sortable = false;
            }
            arrColumnWidth = $.trim(oFindMatterConstants.ListViewColumnWidth) ? $.trim(oFindMatterConstants.ListViewColumnWidth).split(";") : "";
            arrColumnFormatter = $.trim(oFindMatterConstants.ListViewColumnFomatter) ? $.trim(oFindMatterConstants.ListViewColumnFomatter).split(";") : "";
        } else {
            /* Get the data that is to be shown in Search Document list view control */
            //// If pinned data is requested (flag = 4) get the different set of field names
            if (4 === oCommonObject.iCurrentListViewData()) {
                arrColumnNames = $.trim(oFindDocumentConstants.ListViewPinnedColumnValueFields) ? $.trim(oFindDocumentConstants.ListViewPinnedColumnValueFields).split(";") : "";
                oGridConfig.sortable = true;
            } else {
                arrColumnNames = $.trim(oFindDocumentConstants.ListViewColumnValueFields) ? $.trim(oFindDocumentConstants.ListViewColumnValueFields).split(";") : "";
                oGridConfig.sortable = false;
            }
            arrColumnWidth = $.trim(oFindDocumentConstants.ListViewColumnWidth) ? $.trim(oFindDocumentConstants.ListViewColumnWidth).split(";") : "";
            arrColumnFormatter = $.trim(oFindDocumentConstants.ListViewColumnFomatter) ? $.trim(oFindDocumentConstants.ListViewColumnFomatter).split(";") : "";
        }
        if (arrColumnNames && arrColumnNames.length && arrColumnWidth && arrColumnWidth.length && arrColumnFormatter && arrColumnFormatter.length) {
            /* Generate the column structure that is to be shown in the list view control */
            $.each(arrColumnNames, function (iCurrentIndex, sCurrentValue) {
                var oCurrentItem = {};
                if ("ECB" === sCurrentValue) {
                    oCurrentItem = { name: sCurrentValue, width: arrColumnWidth[iCurrentIndex], id: sCurrentValue, align: "center", trimOnOverflow: false, formatter: arrColumnFormatter[iCurrentIndex], sortable: oGridConfig.sortable, sortType: String };
                } else {
                    oCurrentItem = { name: sCurrentValue, width: arrColumnWidth[iCurrentIndex], id: sCurrentValue, align: "left", trimOnOverflow: false, formatter: arrColumnFormatter[iCurrentIndex], sortable: oGridConfig.sortable, sortType: String };
                }
                listViewJSON.push(oCurrentItem);
            });
            return listViewJSON;
        }
        return [];
    };

    /* Function to load list view control */
    oCommonFunctions.createListView = function (oDataToLoad, bIsPageLoadData) {
        //// Get the list view JSON based on the current page
        var listViewJSON = oCommonFunctions.generateListViewJSON();
        if (listViewJSON && listViewJSON.length) {
            var arrHeaderData, arrHeaderDataTitle;
            if (oCommonObject.isMatterView) {
                arrHeaderData = oFindMatterConstants.ListViewHeaderName.split(";");
                arrHeaderDataTitle = oFindMatterConstants.ListViewHeaderNameTitle.split("$|$");
            } else {
                arrHeaderData = oFindDocumentConstants.ListViewHeaderName.split(";");
                arrHeaderDataTitle = oFindDocumentConstants.ListViewHeaderNameTitle.split("$|$");
            }
            var oHeaderNames = [], oHeaderFilterType = [];
            $.each(arrHeaderData, function (iItem, sCurrentValue) {
                var oCurrentHeader = sCurrentValue && sCurrentValue.split(",");
                oHeaderNames.push(oCurrentHeader[0]);
                oCommonObject.oHeaderFilterType.push(oCurrentHeader[1]);
            });
            var sortBy = (oCommonObject.isMatterView) ? oFindMatterConstants.ListViewTitleProperty : oFindDocumentConstants.ListViewTitleProperty;
            //// Generate the common JSON structure for generating the list view control
            var GridConfig = {
                container: "listViewContainer",
                data: oDataToLoad,
                gridName: "List View",
                gridHeader: oHeaderNames,
                gridHeaderTitle: arrHeaderDataTitle,
                columnNames: listViewJSON,
                sortby: "",
                sortorder: "asc",
                sortType: String,
                initialsortorder: "",
                retainpageonsort: false,
                maxRows: oListViewObject.itemsPerPage,
                viewrecords: true,
                pagination: false,
                cellSpacing: 0
            };
            if (bIsPageLoadData) {
                //// Generate the list view grid for page load data
                oCommonObject.isAllRowSelected = false;
                new LCADMS.JsonGrid(GridConfig);
                $(".jsonGridHeader").addClass("ms-font-m-plus");
                oListViewObject.nHeaderLeftOffset = $("#listViewContainer_Grid thead").length ? $("#listViewContainer_Grid thead")[0].offsetLeft : "";
            } else {
                //// Generate the list view table without header for lazy load data
                var oAdditionalProperties = {
                    currentPage: 0,
                    tblBody: $("#listViewContainer tbody")[0],
                    drilldown: false
                };

                //// Update the JSON object to extend additional properties for generating list view
                GridConfig = $.extend(GridConfig, oAdditionalProperties);
                new LCADMS.CreateHTMLTableRow(GridConfig);
                $("#listViewContainer_Grid").find(".lazyLoading").addClass("hide");

                //// Update the data present in the list view object to have data loaded by lazy load
                $.merge(LCADMS.gridObject[0].data, oDataToLoad);
                LCADMS.gridObject[0].maxRows = LCADMS.gridObject[0].data.length;
                oListViewObject.waitTillDataLoaded = false;
            }
            $(window).trigger("resize");
            oCommonObject.configureOnLoadView();
            oCommonFunctions.addListViewControlBindings();
        }
    };

    /* Function to load list view control */
    oCommonFunctions.loadListView = function (sDataToLoad, bIsPageLoadData) {
        var oDataToLoad = "", oListViewContainer = $("#listViewContainer_Grid");
        if ("null" !== sDataToLoad) {
            oDataToLoad = JSON.parse(sDataToLoad);
        } else {
            oDataToLoad = JSON.parse(oListViewCommonObject.sData);
        }
        if (4 === oCommonObject.iCurrentListViewData()) {
            $.each(oDataToLoad, function (key, value) {
                var sMatterId = oCommonObject.isMatterView ? oFindMatterConstants.ListViewPinnedMatterIDPropertyName : oFindDocumentConstants.ListViewPinnedMatterIDPropertyName
                , sClientId = oCommonObject.isMatterView ? oFindMatterConstants.ListViewPinnedMatterClientIDPropertyName : oFindDocumentConstants.ListViewPinnedDocumentClientIDPropertyName;
                value.CalClientMatterId = value[sClientId] + "." + value[sMatterId];
            });
            oCommonObject.pinnedData = oDataToLoad;

        }
        if (!(oListViewContainer.length && 4 === oCommonObject.iCurrentListViewData())) {
            oCommonFunctions.createListView(oDataToLoad, bIsPageLoadData);
        }
        oCommonFunctions.bindECB();
        oCommonObject.enableSearch();
    };

    /* Function to bind ECB control */
    oCommonFunctions.bindECB = function () {
        /* First unbind the click event and re-bind the ECB control to all the rows present in the list view control */
        $(".ellipsis").off().on("click", function (event) {
            var oCurrentRow = $(this).parents(".GridRow"), sECBHTMLChunk = {}, oCheckBoxList = {}; //// Get the parent row with class GridRow
            $(".ECBControlClass").remove();
            sECBHTMLChunk = $("#ECBControl").length && $("#ECBControl").clone() && $("#ECBControl").clone()[0];
            $(sECBHTMLChunk).removeClass("hide").addClass("ECBControlClass");
            $("#listViewContainer").append(sECBHTMLChunk);
            /* Function to bind ECB controls */
            oCommonObject.addECBBindings();
            var $AllListViewItems = $(".isSelectRowsActive");
            if ($AllListViewItems && $AllListViewItems.length && $AllListViewItems[0].checked) {
                $AllListViewItems.prop("checked", false);
                oCommonObject.isAllRowSelected = false;
            }
            var oListViewRows = $(".GridRow, .GridRowAlternate");
            if (oListViewRows.length) {
                oListViewRows.removeClass("is-selectedRow");
                $(".ellipsis").parent().removeClass("ellipsisBackground");
                $(".checkBoxImage").removeClass("checkboxHoverImageContainer checkboxImageContainer").addClass("hide");
                $(".checkBox").removeClass("hide");

                oCheckBoxList = oListViewRows.find(".ms-ChoiceField-input");
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
            oListViewObject.oCurrentMandatory = oCurrentRow.find(".mandatory"); //// Assign to global object so that it can used for reference from outer JS files as well
            if (oCommonObject.isMatterView) {
                updateECBforMatter(oContextualMenu, oListViewObject.oCurrentMandatory);
            } else {
                updateECBforDocument(oContextualMenu, oListViewObject.oCurrentMandatory);
            }
            oContextualMenu.find(".ms-ContextualMenu-link").removeClass("is-selected");
            //// Position the ECB fly out on the current ellipses icon and display the fly out
            var nDynamicWidth = oCommonObject.getWidth()
            , iEllipsesOffsetFromTop = $(this).position().top
            , iECBMenuOffsetFromTop = iEllipsesOffsetFromTop
            , iListViewOffsetFromTop = $("#listViewContainer").offset().top + $("#listViewContainer").height(); //// Adjust ECB menu relative to list view control (Kept commented for future purpose)
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
        if (4 !== oCommonObject.iCurrentListViewData()) {  //// No lazy loading for pinned section (flag = 4)
            if (!oListViewObject.waitTillDataLoaded && ($("#listViewContainer").scrollTop() + $("#listViewContainer").height() > ($("#listViewContainer_Grid").height() - (1 - (oGlobalConstants.ListViewLazyLoadingLimit / 100)) * $("#listViewContainer_Grid").height()))) {
                var iMaxCurrentData = parseInt(oListViewObject.itemsPerPage) * (oListViewObject.pageNumber);
                if (oListViewObject.searchResultCount > iMaxCurrentData) {
                    oListViewObject.waitTillDataLoaded = true; //// Throttle the next request till current service request is completed
                    $("#listViewContainer_Grid").find(".lazyLoading").removeClass("hide"); //// Display the loading image
                    $("#listViewContainer .jsonGridHeader").attr("disabled", "disabled").addClass("disableHeader");////disable the grid header for lazy loading
                    oListViewObject.pageNumber++;
                    if (oCommonObject.isMatterView) {
                        getSearchMatters("#listView", 1);
                    } else {
                        getSearchDocuments("#listView", 1);
                    }
                }
            }
        }
        $("#listViewContainer_Grid thead").css("left", oListViewObject.nHeaderLeftOffset - $("#listViewContainer").scrollLeft());
        $("#listViewContainer_Grid thead").width(($(document).width() - 37) + $("#listViewContainer").scrollLeft());       //// set header width according to document width(after Deducting 37px which is width of column picker notch) and scroll of list view container
        if (320 > window.innerWidth) {
            $("#listViewContainer_Grid thead").css("left", (-($("body").scrollLeft())) + oListViewObject.nHeaderLeftOffset); //// if there is horizontal scroll bar then set header left according to that scrollbar
        }
    };

    $(document).on("scroll", function (event) {
        $("#listViewContainer_Grid thead") && $("#listViewContainer_Grid thead").css("left", oListViewObject.nHeaderLeftOffset - $(document).scrollLeft());
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
        oCommonFunctions.adjustListViewHeight();
        oCommonObject.adjustCaretIcon();
        //// Adjust the position of column picker as per presence of scroll
        if ($("#listViewContainer").hasScrollBar()) {
            $("#columnPickerStrip, #columnPickerControl, #columnPickerContainer").removeClass("noScrollPadding").addClass("scrollPadding");
        } else {
            $("#columnPickerStrip, #columnPickerControl, #columnPickerContainer").removeClass("scrollPadding").addClass("noScrollPadding");
        }
        $("#listViewContainer_Grid thead").css("width", ($(document).width() - 37) + $("#listViewContainer").scrollLeft());  //// set header width according to document width(after Deducting 37px which is width of column picker notch) and scroll of list view container
        waitForFinalEvent(function () {
            "use strict";
            //// function call of change list view container height dynamically
            oCommonFunctions.applyMapContainerHeight();
        }, 500, "String");
    });

    oCommonFunctions.adjustListViewHeight = function () {
        var nCurrentWindowHeight = window.innerHeight;
        var nCurrentWindowWidth = window.innerWidth;
        //// check whether window height is less then 190px or not
        //// if yes then set container height to 155
        //// else set container height according to windows height 
        if (parseInt(nCurrentWindowHeight) <= 155) {
            $("#listViewContainer").height(55);
        } else {
            var oAttachButton = $("#attachDocuments"), iButtonHeight = 0;
            if (oAttachButton && oAttachButton.length) {
                iButtonHeight = (oAttachButton.attr("data-applicable")) ? 53 : 0;
            }
            //// If width is less than 320 px, then two scroll bars will appear and that's why we have this condition
            if (320 > nCurrentWindowWidth) {
                $("#listViewContainer").css("height", parseInt(nCurrentWindowHeight, 10) - (109 + iButtonHeight)); //// Deducting 109px from windows height in order to remove the height of App header section and height of app scrollbar
            } else {
                $("#listViewContainer").css("height", parseInt(nCurrentWindowHeight, 10) - (92 + iButtonHeight)); //// Deducting 92px from windows height in order to remove the height of App header section
            }
            $("#columnPickerStrip").css("height", parseInt(nCurrentWindowHeight, 10) - (111 + iButtonHeight)); //// Deducting 111px from windows height in order to remove the height of App header section and horizontal scrollbar for list view
        }
    };
    /* Function to set List View height dynamically according to window height*/
    oCommonFunctions.applyMapContainerHeight = function () {
        oGridConfig.nPreviousGridHeight = oGridConfig.nCurrentGridHeight;
        oGridConfig.nCurrentGridHeight = window.innerHeight;
        //// check height of previous grid and current grid
        //// oCommonObject.isServiceCallComplete used for check whether any service is currently running or not
        //// 4 != oCommonObject.iCurrentListViewData() used for check that there should not pinned matter/document data in list view 
        if (oGridConfig.nPreviousGridHeight !== oGridConfig.nCurrentGridHeight && 4 !== oCommonObject.iCurrentListViewData()) { // && oCommonObject.isServiceCallComplete) {            
            if (oGridConfig.nCurrentGridHeight >= parseInt(oGlobalConstants.ListView_BreakPoint, 10)) {
                if (oGridConfig.nPreviousGridHeight < parseInt(oGlobalConstants.ListView_BreakPoint, 10)) {
                    oCommonObject.abortRequest();
                    ////if height of container is more then 710px then load records according to height of browser window
                    //// equation : items per page = items per page * ( height of browser window / 710 ) + 1
                    //// In 710px height we can show 21 list item in page 
                    oCommonObject.isAllRowSelected = false;
                    oListViewObject.itemsPerPage = oListViewObject.itemsPerPage * (Math.floor((parseInt(oGridConfig.nCurrentGridHeight, 10) / parseInt(oGlobalConstants.ListView_BreakPoint, 10))) + 1);
                    oCommonObject.clearListViewContent();
                    // Update the list view for matters/documents
                    oCommonObject.updateListView();
                }
            } else if (oGridConfig.nPreviousGridHeight >= oGlobalConstants.ListView_BreakPoint || 0 === oGridConfig.nPreviousGridHeight) {
                ////oGridConfig.nPreviousGridHeight === 0 is checked for page load                
                //// set itemsperpage = 21                
                oCommonObject.abortRequest();
                oCommonObject.isAllRowSelected = false;
                oListViewObject.itemsPerPage = oGlobalConstants.ListViewItemsPerPage;
                if (1 === oListViewObject.pageNumber || oGridConfig.nCurrentGridHeight <= parseInt(oGlobalConstants.ListView_BreakPoint, 10)) {
                    oCommonObject.clearListViewContent();
                }
                if (4 === oCommonObject.iCurrentListViewData()) {
                    //// call pinned matters/documents
                    (oCommonObject.isMatterView) ? getPinnedMatters($("#listView")) : getPinnedDocument($("#listView"));
                } else {
                    oCommonObject.updateListView();
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

    /* Function to get the initial width of the list view control */
    oCommonFunctions.getListViewWidth = function () {
        var arrGridWidthValues = oCommonObject.isMatterView ? oFindMatterConstants.ListViewColumnWidth.split(";") : oFindDocumentConstants.ListViewColumnWidth.split(";");
        oGridConfig.nCurrentGridWidth = oGridConfig.nPreviousGridWidth = arrGridWidthValues.reduce(function (previousValue, currentValue) { return parseInt(previousValue) + parseInt(currentValue); }) + oGridConfig.nWidthDiff;
    };

    /* Function to load the ECB control and register ECB actions */
    oCommonFunctions.loadECBControl = function () {
        var arrCurrentView, arrECBTitle;
        /* Get the list of actions to be displayed in the ECB menu based on current page */
        if (oCommonObject.isMatterView) {
            arrCurrentView = $.trim(oFindMatterConstants.ListViewECBActions) ? $.trim(oFindMatterConstants.ListViewECBActions).split(";") : "";
            arrECBTitle = oFindMatterConstants.ListViewECBActionsTitle ? (oFindMatterConstants.ListViewECBActionsTitle).split("$|$") : "";
        } else {
            arrCurrentView = $.trim(oFindDocumentConstants.ListViewECBActions) ? $.trim(oFindDocumentConstants.ListViewECBActions).split(";") : "";
            arrECBTitle = oFindDocumentConstants.ListViewECBActionsTitle ? (oFindDocumentConstants.ListViewECBActionsTitle).split("$|$") : "";
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
    oCommonFunctions.highlightListViewRow = function ($RowElement, flag) {
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
            if (4 === oCommonObject.iCurrentListViewData()) {
                $(".sort").addClass("hide");
                if ("asc" === oThisObject.attr("sortorder")) {
                    oThisObject.find(".sort").html("&#x2191;").removeClass("hide");
                } else {
                    oThisObject.find(".sort").html("&#x2193;").removeClass("hide");
                }
                $(".isSelectRowsActive").prop("checked", false);
                oCommonFunctions.highlightListViewRow(oGridRow, false);
                $("#attachDocuments").addClass("is-disabled");
                oListView.bindECB();
            } else {
                var sRefinerName = $.trim(oThisObject.attr("id")) //// This is the name of the property on which the sort is to be applied
                    , oCurrentSortObject = oCommonObject.oSortDetails();
                if (oCurrentSortObject.ByProperty === sRefinerName) { //// If same column is clicked
                    oCurrentSortObject.Direction = (oCurrentSortObject.Direction) ? 0 : 1;
                } else {
                    oCurrentSortObject.Direction = 0; //// Ascending sort by default on first click
                }
                oCurrentSortObject.ByProperty = sRefinerName ? sRefinerName : "";
                oCommonObject.clearListViewContent();
                if (oCommonObject.isMatterView) {
                    getSearchMatters("#listView", 1);
                } else {
                    getSearchDocuments("#listView", 1);
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
                    oCommonFunctions.highlightListViewRow(oGridRow, false);
                } else {
                    oCommonFunctions.highlightListViewRow(oGridRow, true);
                }
            }
            oCommonObject.onListViewCheckboxClick();
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
        loadListView: function (sDataToLoad, bIsPageLoadData) {
            oCommonFunctions.loadListView(sDataToLoad, bIsPageLoadData);
        },
        scrollListView: function ($Element) {
            return oCommonFunctions.scrollListView($Element);
        },
        highlightListViewRow: function ($RowElement, bFlag) {
            oCommonFunctions.highlightListViewRow($RowElement, bFlag);
        },
        loadECBControl: function () {
            oCommonFunctions.loadECBControl();
        },
        getListViewWidth: function () {
            oCommonFunctions.getListViewWidth();
        },
        formatDate: function (dDate) {
            return oCommonFunctions.formatDate(dDate);
        },
        bindScrollEvent: function () {
            oCommonFunctions.bindScrollEvent();
        },
        adjustListViewHeight: function () {
            oCommonFunctions.adjustListViewHeight();
        },
        applyMapContainerHeight: function () {
            oCommonFunctions.applyMapContainerHeight();
        },
        generateListViewJSON: function () {
            return oCommonFunctions.generateListViewJSON();
        },
        bindECB: function () {
            return oCommonFunctions.bindECB();
        },
        addListViewControlBindings: function () {
            return oCommonFunctions.addListViewControlBindings();
        }
    });
})();
