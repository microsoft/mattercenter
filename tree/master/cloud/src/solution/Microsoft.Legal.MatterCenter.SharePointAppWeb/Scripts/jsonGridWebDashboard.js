/// <disable>JS3057,JS3058,JS2074,JS2076,JS3092,JS3054,JS2005,JS3056,JS2073,JS2024,JS2026,JS2032,JS2064,JS3053, JS3116</disable>
var selectAllFlagIs = false,
    nClickCount = 0;
if (typeof oGrid === "undefined") {
    var oGrid = {};
}
oGrid.gridName = [];
oGrid.gridObject = [];

var oUploadGlobal = {
    sClientRelativeUrl: "",
    sFolderUrl: "",
    arrContent: [],
    arrFiles: [],
    arrOverwrite: [],
    oDrilldownParameter: { nCurrentLevel: 0, sCurrentParentUrl: "", sRootUrl: "" },
    oXHR: new XMLHttpRequest(),
    sMatterURL: "",
    sNotificationMsg: ""
};

function renderConsumptionDot(cellValue, rowJsonObject) {
    "use strict";
    if ("NA" === cellValue) {
        return "<span>" + cellValue + "</span><div class=\"dvCnsp dv30Cnsp\"></div>";
    } else if (cellValue < 30) {
        return "<span>" + cellValue + "% </span><div class=\"dvCnsp dv30Cnsp\"></div>";
    } else if (cellValue >= 30 && cellValue < 60) {
        return "<span>" + cellValue + "% </span><div class=\"dvCnsp dv60Cnsp\"></div>";
    } else {
        return "<span>" + cellValue + "% </span><div class=\"dvCnsp dv70Cnsp\"></div>";
    }
}
function InsertCommasOnly(cellvalue, rowJsonObject) {
    "use strict";
    return cellvalue;
}
function InsertPercentages(cellvalue, rowJsonObject) {
    "use strict";
    return cellvalue;
}
function cleanNumberFormatting(sData) {
    "use strict";
    return sData.replace("$", "").replace("%", "").replace(/,/gi, "");
}
function hasClass(el, name) {
    "use strict";
    if (el) {
        return new RegExp("(\\s|^)" + name + "(\\s|$)").test(el.className);
    }
}
function addClass(el, name) {
    "use strict";
    if (el) {
        if (!hasClass(el, name)) {
            el.className += (el.className ? " " : "") + name;
        }
    }
}
function removeClass(el, name) {
    "use strict";
    if (el) {
        if (hasClass(el, name)) {
            el.className = el.className.replace(new RegExp("(\\s|^)" + name + "(\\s|$)"), " ").replace(/^\s+|\s+$/g, "");
        }
    }
}
function sortBy(field, reverse, primer) {
    "use strict";
    var key = function (gridRow) {
        "use strict";

        return getFunctionName(primer) === "Date" ? gridRow[field] : primer ? primer(gridRow[field]) : gridRow[field];
    };
    return function (gridRow1, gridRow2) {
        "use strict";
        var sortValue1 = getFunctionName(primer) === "Date" ? validateDateFormat(key(gridRow1)) : key(gridRow1);
        var sortValue2 = getFunctionName(primer) === "Date" ? validateDateFormat(key(gridRow2)) : key(gridRow2);
        if (primer === parseFloat || primer === parseInt) {
            if (String(sortValue1) === "NaN") {
                sortValue1 = 0;
            }
            if (String(sortValue2) === "NaN") {
                sortValue2 = 0;
            }
        }
        return ((sortValue1 < sortValue2) ? -1 : (sortValue1 > sortValue2) ? +1 : 0) * [-1, 1][+!!reverse];
    };
}
function clone(obj) {
    "use strict";
    // Handle the 3 simple types, and null or undefined
    var copy;
    var iIterator = 0;
    var attr;
    if (null === obj || "object" !== typeof obj) {
        return obj;
    }
    var len = obj.length;
    // Handle Date
    if (obj instanceof Date) {
        copy = new Date();
        copy.setTime(obj.getTime());
        return copy;
    }
    // Handle Array
    if (obj instanceof Array) {
        copy = [];
        for (iIterator = 0; iIterator < len; iIterator++) {
            copy[iIterator] = clone(obj[iIterator]);
        }
        return copy;
    }
    // Handle Object
    if (obj instanceof Object) {
        copy = {};
        for (attr in obj) {
            if (obj.hasOwnProperty(attr)) {
                copy[attr] = clone(obj[attr]);
            }
        }
        return copy;
    }
    throw new Error("Unable to copy obj! Its type isn't supported.");
}
oGrid.getAdjustedRowChunk = function (inputData, width) {
    "use strict";
    return "<div class=\"jsonGridOverflow\" title=\"" + inputData + "\" style=\"width: " + width + "px;\"><span>" + inputData + "</span></div>";
};
oGrid.getAdjustedRowChunkAndToolTip = function (inputData, width) {
    "use strict";
    width = width.replace("%", "");
    var windowWidth = document.querySelector("#reportHead").offsetWidth * (parseInt(width) / 100);
    return "<span class=\"jsonGridOverflow\" title=\"" + inputData + "\" style=\"width: " + (windowWidth - 40) + "px;\">" + inputData + "</span>";
};
oGrid.setViewRecords = function (currentGridConfig) {
    "use strict";
    var ELE = document.getElementById(currentGridConfig.gridName + "_ViewRecords");
    if ($(".pinnedSearch").hasClass("active")) {
        oGridConfig.currentView = 1;
    } else if ($(".mySearch").hasClass("active")) {
        oGridConfig.currentView = 2;
    } else {
        oGridConfig.currentView = 0;
    }

    var startItem = 1 === oGridConfig.currentView ? (currentGridConfig.currentPage * currentGridConfig.maxRows + 1) : ((oGridConfig.nGridPageNumber - 1) * oGridConfig.itemsPerPage + 1),
        endItem = 1 === oGridConfig.currentView ? ((currentGridConfig.currentPage + 1) * currentGridConfig.maxRows) : ((oGridConfig.nGridPageNumber) * oGridConfig.itemsPerPage),
        lastPage = 1 === oGridConfig.currentView ? currentGridConfig.data.length : oGridConfig.nGridTotalResults;
    endItem = endItem > lastPage ? lastPage : endItem;
    oGridConfig.nPinnedGridPageNumber = currentGridConfig.currentPage;
    if ($(ELE)) {
        $(ELE).find(".PaginateSpan").text(InsertCommasOnly(startItem) + " - " + InsertCommasOnly(endItem));
    }
};
oGrid.populateGrid = function (currentGridConfig) {
    "use strict";
    var htmlGridObject = currentGridConfig.gridObject;
    if (htmlGridObject) {
        var numberOfRows = currentGridConfig.tblBody.rows.length;
        var rowCounter = 0;
        for (rowCounter = 0; rowCounter < numberOfRows; rowCounter += 1) {
            currentGridConfig.tblBody.deleteRow(-1);
        }
        oGrid.CreateHTMLTableRow(currentGridConfig);
    }
};
oGrid.disablePrev = function (gridname) {
    "use strict";
    var prev = document.getElementById(gridname + "_Prev");
    var first = document.getElementById(gridname + "_First");
    addClass(prev, "click-disabled");
    addClass(first, "click-disabled");
    if (prev !== null) {
        prev.setAttribute("src", "../Images/pagination-back-disabled.png");
    }
    if (first !== null) {
        first.setAttribute("src", "../Images/pagination-back-disabled.png");
    }
};
oGrid.enablePrev = function (gridname) {
    "use strict";
    var prev = document.getElementById(gridname + "_Prev");
    var first = document.getElementById(gridname + "_First");
    removeClass(prev, "click-disabled");
    removeClass(first, "click-disabled");
    if (prev !== null) {
        prev.setAttribute("src", "../Images/pagination-back-enabled.png");
    }
    if (first !== null) {
        first.setAttribute("src", "../Images/pagination-back-Enabled.png");
    }
};
oGrid.disableNext = function (gridname) {
    "use strict";
    var next = document.getElementById(gridname + "_Next");
    var last = document.getElementById(gridname + "_Last");
    addClass(next, "click-disabled");
    addClass(last, "click-disabled");
    next.setAttribute("src", "../Images/pagination-front-disabled.png");
    last.setAttribute("src", "../Images/pagination-front-disabled.png");
};
oGrid.enableNext = function (gridname) {
    "use strict";
    var next = document.getElementById(gridname + "_Next");
    var last = document.getElementById(gridname + "_Last");
    removeClass(next, "click-disabled");
    removeClass(last, "click-disabled");
    next.setAttribute("src", "../Images/pagination-front-enabled.png");
    last.setAttribute("src", "../Images/pagination-front-Enabled.png");
};
oGrid.goLast = function (ELE, gridName) {
    "use strict";
    if (!hasClass(ELE, "click-disabled")) {
        if (oGrid.gridName.length > 0) {
            var gridObjectPosition = oGrid.gridName.indexOf(gridName);
            if (-1 < gridObjectPosition) {
                var currentGridConfig = oGrid.gridObject[gridObjectPosition];
                currentGridConfig.currentPage = currentGridConfig.totalPages;
                oGrid.setViewRecords(currentGridConfig);
                oGrid.disableNext(currentGridConfig.gridName);
                oGrid.enablePrev(currentGridConfig.gridName);
                oGrid.populateGrid(currentGridConfig);

                createResponsiveGrid();
            }
        }
    }
};
oGrid.goFirst = function (ELE, gridName) {
    "use strict";
    if (!hasClass(ELE, "click-disabled")) {
        if (oGrid.gridName.length > 0) {
            var gridObjectPosition = oGrid.gridName.indexOf(gridName);
            if (-1 < gridObjectPosition) {
                var currentGridConfig = oGrid.gridObject[gridObjectPosition];
                currentGridConfig.currentPage = 0;
                oGrid.setViewRecords(currentGridConfig);
                oGrid.disablePrev(currentGridConfig.gridName);
                oGrid.enableNext(currentGridConfig.gridName);
                oGrid.populateGrid(currentGridConfig);

                createResponsiveGrid();
            }
        }
    }
};
oGrid.goPrevious = function (ELE, gridName) {
    "use strict";
    if (!hasClass(ELE, "click-disabled")) {
        if (oGrid.gridName.length > 0) {
            var gridObjectPosition = oGrid.gridName.indexOf(gridName);
            if (-1 < gridObjectPosition) {
                var currentGridConfig = oGrid.gridObject[gridObjectPosition];
                currentGridConfig.currentPage -= 1;
                oGrid.setViewRecords(currentGridConfig);
                if (!currentGridConfig.currentPage) {
                    oGrid.disablePrev(currentGridConfig.gridName);
                    oGrid.enableNext(currentGridConfig.gridName);
                }
                if (currentGridConfig.currentPage > 0) {
                    oGrid.enableNext(currentGridConfig.gridName);
                }
                oGrid.populateGrid(currentGridConfig);

                createResponsiveGrid();
            }
        }
    }
};

oGrid.goNext = function (ELE, gridName) {
    "use strict";
    if (!hasClass(ELE, "click-disabled")) {
        if (oGrid.gridName.length > 0) {
            var gridObjectPosition = oGrid.gridName.indexOf(gridName);
            if (-1 < gridObjectPosition) {
                var currentGridConfig = oGrid.gridObject[gridObjectPosition];
                currentGridConfig.currentPage += 1;
                oGrid.setViewRecords(currentGridConfig);
                if (currentGridConfig.currentPage > 0) {
                    oGrid.enablePrev(currentGridConfig.gridName);
                }
                if (currentGridConfig.currentPage === currentGridConfig.totalPages) {
                    oGrid.disableNext(currentGridConfig.gridName);
                    oGrid.enablePrev(currentGridConfig.gridName);
                }
                oGrid.populateGrid(currentGridConfig);

                createResponsiveGrid();
            }
        }
    }
};
oGrid.sortJsonGrid = function (cellObject, gridName, fieldName) {
    "use strict";
    if (oGrid.gridName.length > 0) {
        var gridObjectPosition = oGrid.gridName.indexOf(gridName);
        if (-1 < gridObjectPosition) {
            var sortOrder = cellObject.getAttribute("sortorder"),
                sortFlag = false;
            if ("asc" === sortOrder) {
                sortFlag = true;
                cellObject.setAttribute("sortorder", "desc");
            } else {
                cellObject.setAttribute("sortorder", "asc");
            }
            var currentGridConfig = oGrid.gridObject[gridObjectPosition],
                columnCounter,
                sortType = String;

            for (columnCounter in currentGridConfig.columnNames) {
                if (currentGridConfig.columnNames[columnCounter].name === fieldName && currentGridConfig.columnNames[columnCounter].sortType) {
                    if (currentGridConfig.columnNames[columnCounter].sortAttribute) {
                        fieldName = currentGridConfig.columnNames[columnCounter].sortAttribute;
                    }

                    sortType = currentGridConfig.columnNames[columnCounter].sortType;
                }
            }

            currentGridConfig.data.sort(sortBy(fieldName, sortFlag, sortType));
            if (!currentGridConfig.retainpageonsort && currentGridConfig.totalPages) {
                currentGridConfig.currentPage = 0;
                oGrid.setViewRecords(currentGridConfig);
                oGrid.disablePrev(currentGridConfig.gridName);
                oGrid.enableNext(currentGridConfig.gridName);
            }

            oGrid.populateGrid(currentGridConfig);
        }
    }
};
oGrid.CreatePaginationControl = function (jsonGridConfiguration) {
    "use strict";
    var row = jsonGridConfiguration.tblFoot.insertRow(-1),
        cell = row.insertCell(0),
        paginationDiv = document.createElement("div"),
        firstDiv = document.createElement("div"),
        prevDiv = document.createElement("div"),
        viewRecordDiv = document.createElement("div"),
        viewRecordSpan = document.createElement("span"),
        nextDiv = document.createElement("div"),
        lastDiv = document.createElement("div"),
        nTempGridPage;

    cell.colSpan = jsonGridConfiguration.columnNames.length;
    var sFunctionName = oGridConfig.isMatterView ? "oCommonObject.getRecentMatters" : "oCommonObject.getRecentDocuments",
        sContainer = oGridConfig.isMatterView ? "recentMatters" : "RecentDocumentContainer";
    // Handle the Pinned Matter grid and the All Results grid.
    //  First Page and Previous Page.
    if (2 === oGridConfig.currentView) {
        if (1 === oGridConfig.nGridPageNumber) {
            firstDiv.innerHTML = "<img title=\"First\" id=\"" + jsonGridConfiguration.gridName + "_First\"  class=\"first cur-pointer click-disabled\"  src = \"../Images/pagination-back-disabled.png\" active=\"0\" /></img>";
            prevDiv.innerHTML = "<img title=\"Previous\" id=\"" + jsonGridConfiguration.gridName + "_Prev\"  class=\"prev cur-pointer click-disabled\"  src = \"../Images/pagination-back-disabled.png\" active=\"0\" /></img>";
        } else {
            nTempGridPage = oGridConfig.nGridPageNumber - 1;
            firstDiv.innerHTML = "<img title=\"First\" id=\"" + jsonGridConfiguration.gridName + "_First\"  class=\"first paginationIcons cursorHand\"  src = \"../Images/pagination-back-Enabled.png\" active=\"1\" onclick=\"oGridConfig.nGridPageNumber = 1;" + sFunctionName + "('" + sContainer + "', 1,  1, true);scrollIframeTop();\"/></img>";
            prevDiv.innerHTML = "<img title=\"Previous\" id=\"" + jsonGridConfiguration.gridName + "_Prev\"  class=\"prev paginationIcons cursorHand\"  src = \"../Images/pagination-back-enabled.png\" active=\"1\" onclick=\"oGridConfig.nGridPageNumber = " + nTempGridPage + ";" + sFunctionName + "('" + sContainer + "'," + nTempGridPage + ",  1, true);scrollIframeTop();\"/></img>";
        }

    } else if (1 === oGridConfig.currentView) {
        firstDiv.innerHTML = '<img title="First" id="' + jsonGridConfiguration.gridName + '_First"  class="first cur-pointer click-disabled"  src = "../Images/pagination-back-disabled.png" active="1" onclick="oGrid.goFirst(this,\'' + jsonGridConfiguration.gridName + '\');" />';
        firstDiv.className = "jsongrid-first";

        prevDiv.innerHTML = '<img title="Previous" id="' + jsonGridConfiguration.gridName + '_Prev"  active="1" class="prev cur-pointer middle click-disabled" src = "../Images/pagination-back-disabled.png" onclick="oGrid.goPrevious(this,\'' + jsonGridConfiguration.gridName + '\');return false;" />';
        prevDiv.className = "jsongrid-prev";
    } else {
        if (1 === oGridConfig.nGridPageNumber) {
            firstDiv.innerHTML = "<img title=\"First\" id=\"" + jsonGridConfiguration.gridName + "_First\"  class=\"first cur-pointer click-disabled\"  src = \"../Images/pagination-back-disabled.png\" active=\"0\" /></img>";
            prevDiv.innerHTML = "<img title=\"Previous\" id=\"" + jsonGridConfiguration.gridName + "_Prev\"  class=\"prev cur-pointer click-disabled\"  src = \"../Images/pagination-back-disabled.png\" active=\"0\" /></img>";
        } else {
            nTempGridPage = oGridConfig.nGridPageNumber - 1;
            firstDiv.innerHTML = "<img title=\"First\" id=\"" + jsonGridConfiguration.gridName + "_First\"  class=\"first paginationIcons cursorHand\"  src = \"../Images/pagination-back-Enabled.png\" active=\"1\" onclick=\"oGridConfig.nGridPageNumber = 1;advancedSearch('grid', 'tileContainer');scrollIframeTop();\"/></img>";
            prevDiv.innerHTML = "<img title=\"Previous\" id=\"" + jsonGridConfiguration.gridName + "_Prev\"  class=\"prev paginationIcons cursorHand\"  src = \"../Images/pagination-back-enabled.png\" active=\"1\" onclick=\"oGridConfig.nGridPageNumber = " + nTempGridPage + ";advancedSearch('grid', 'tileContainer');scrollIframeTop();\"/></img>";
        }
    }
    viewRecordSpan.className = "PaginateSpan";
    viewRecordDiv.appendChild(viewRecordSpan);
    // View Records
    viewRecordDiv.id = jsonGridConfiguration.gridName + "_ViewRecords";
    viewRecordDiv.className = "ViewRecordDiv moveToMiddle";
    if (!jsonGridConfiguration.viewrecords) {
        viewRecordDiv.style.visibility = "hidden";
    }

    // Handle the Pinned Matter grid and the All Results grid.
    // Next Page and Last Page.
    var nTempGridLastPage = null;
    if (2 === oGridConfig.currentView) {
        if (oGridConfig.nGridPageNumber === (Math.floor((parseInt(oGridConfig.nGridTotalResults, 10) - 1) / oGridConfig.itemsPerPage) + 1)) {
            nextDiv.innerHTML = "<img title=\"Next\" id=\"" + jsonGridConfiguration.gridName + "_Next\"  class=\"next cur-pointer click-disabled\"  active=\"0\"  src = \"../Images/pagination-front-disabled.png\" />";
            lastDiv.innerHTML = "<img title=\"Last\" id=\"" + jsonGridConfiguration.gridName + "_Last\"  class=\"last cur-pointer click-disabled\"  active=\"0\"  src = \"../Images/pagination-front-disabled.png\" />";
        } else {
            nTempGridPage = oGridConfig.nGridPageNumber + 1;
            nTempGridLastPage = (Math.floor((parseInt(oGridConfig.nGridTotalResults, 10) - 1) / oGridConfig.itemsPerPage) + 1);
            nextDiv.innerHTML = "<img title=\"Next\" id=\"" + jsonGridConfiguration.gridName + "_Next\"  class=\"next paginationIcons cursorHand\"  active=\"1\"  src = \"../Images/pagination-front-enabled.png\" onclick=\"oGridConfig.nGridPageNumber = " + nTempGridPage + ";" + sFunctionName + "('" + sContainer + "'," + nTempGridPage + ",  1, true);scrollIframeTop();\" />";
            lastDiv.innerHTML = "<img title=\"Last\" id=\"" + jsonGridConfiguration.gridName + "_Last\"  class=\"next paginationIcons cursorHand\"  active=\"1\"  src = \"../Images/pagination-front-Enabled.png\" onclick=\"oGridConfig.nGridPageNumber = " + nTempGridLastPage + ";" + sFunctionName + "('" + sContainer + "'," + nTempGridLastPage + ",  1, true);scrollIframeTop();\" />";
        }

    } else if (1 === oGridConfig.currentView) {
        nextDiv.innerHTML = '<img title="Next" id="' + jsonGridConfiguration.gridName + '_Next"  class="next cur-pointer middle"  active="0"  src = "../Images/pagination-front-enabled.png" onclick="oGrid.goNext(this,\'' + jsonGridConfiguration.gridName + '\'); return false;" />';
        nextDiv.className = "jsongrid-next";

        lastDiv.innerHTML = '<img title="Last" id="' + jsonGridConfiguration.gridName + '_Last"  class="last cur-pointer"  active="0" src = "../Images/pagination-front-Enabled.png" onclick="oGrid.goLast(this,\'' + jsonGridConfiguration.gridName + '\')"/>';
        lastDiv.className = "jsongrid-last";
    } else {
        if (oGridConfig.nGridPageNumber === (Math.floor((parseInt(oGridConfig.nGridTotalResults, 10) - 1) / oGridConfig.itemsPerPage) + 1)) {
            nextDiv.innerHTML = "<img title=\"Next\" id=\"" + jsonGridConfiguration.gridName + "_Next\"  class=\"next cur-pointer click-disabled\"  active=\"0\"  src = \"../Images/pagination-front-disabled.png\" />";
            lastDiv.innerHTML = "<img title=\"Last\" id=\"" + jsonGridConfiguration.gridName + "_Last\"  class=\"last cur-pointer click-disabled\"  active=\"0\"  src = \"../Images/pagination-front-disabled.png\" />";
        } else {
            nTempGridPage = oGridConfig.nGridPageNumber + 1;
            nTempGridLastPage = (Math.floor((parseInt(oGridConfig.nGridTotalResults, 10) - 1) / oGridConfig.itemsPerPage) + 1);
            nextDiv.innerHTML = "<img title=\"Next\" id=\"" + jsonGridConfiguration.gridName + "_Next\"  class=\"next paginationIcons cursorHand\"  active=\"1\"  src = \"../Images/pagination-front-enabled.png\" onclick=\"oGridConfig.nGridPageNumber = " + nTempGridPage + ";advancedSearch('grid', 'tileContainer');scrollIframeTop();\" />";
            lastDiv.innerHTML = "<img title=\"Last\" id=\"" + jsonGridConfiguration.gridName + "_Last\"  class=\"next paginationIcons cursorHand\"  active=\"1\"  src = \"../Images/pagination-front-Enabled.png\" onclick=\"oGridConfig.nGridPageNumber = " + nTempGridLastPage + ";advancedSearch('grid', 'tileContainer');scrollIframeTop();\" />";
        }
    }

    paginationDiv.className = "jsonGridFooter";
    paginationDiv.appendChild(firstDiv);
    paginationDiv.appendChild(prevDiv);
    paginationDiv.appendChild(viewRecordDiv);
    paginationDiv.appendChild(nextDiv);
    paginationDiv.appendChild(lastDiv);
    var testDiv = document.getElementById("gridPaginationTD");
    testDiv.appendChild(paginationDiv);
    cell.appendChild(paginationDiv);
    new oGrid.setViewRecords(jsonGridConfiguration);
    if (1 === oGridConfig.currentView) {
        if (jsonGridConfiguration.maxRows && jsonGridConfiguration.maxRows >= jsonGridConfiguration.data.length) {
            $("#" + jsonGridConfiguration.gridName + "_Last").addClass("click-disabled");
            oGrid.disableNext(jsonGridConfiguration.gridName);

        }
    }
};
oGrid.CreateHTMLTableRow = function (jsonGridConfiguration, rowPosition) {
    "use strict";
    var startIndex = jsonGridConfiguration.currentPage * jsonGridConfiguration.maxRows;
    var endIndex = (startIndex + jsonGridConfiguration.maxRows) <= jsonGridConfiguration.data.length ? (startIndex + jsonGridConfiguration.maxRows) : jsonGridConfiguration.data.length;
    endIndex = endIndex <= 0 ? jsonGridConfiguration.data.length : endIndex;
    var cell = null, cellCounter, drillCounter = 0, cellblank,
        numberofColumns = jsonGridConfiguration.columnNames.length;
    for (this.rowPosition = startIndex; this.rowPosition < endIndex; this.rowPosition += 1) {
        var row = jsonGridConfiguration.tblBody.insertRow(-1);
        if (this.rowPosition % 2) {
            row.setAttribute("class", "GridRowAlternate");
        } else {
            row.setAttribute("class", "GridRow");
        }
        var iCount = 0;
        var oDrillObject = [];
        if (jsonGridConfiguration.drilldown) {
            if (jsonGridConfiguration.drilldownAt === "<") {
                cell = row.insertCell(0);
                drillCounter = 1;
                cell.setAttribute("class", "jsonGridRow DrillExpandIcon");
                cell.parentNode.setAttribute("onclick", 'LoadNextRow(this,"' + jsonGridConfiguration.data[this.rowPosition].C1 + '","' + jsonGridConfiguration.container + '")');
                cellblank = row.insertCell(1);
                cellblank.setAttribute("style", "width: 30px;");
            }
        }
        for (cellCounter = 0; cellCounter < numberofColumns; cellCounter += 1) {
            cell = row.insertCell(cellCounter + drillCounter);
            cell.setAttribute("class", "jsonGridRow");
            cell.style.textAlign = jsonGridConfiguration.columnNames[cellCounter].align;

            // Doing this to avoid overlapping of last column values and the scroll bar
            if (jsonGridConfiguration.columnNames[cellCounter].noOverlap) {
                if (cellCounter === numberofColumns - 1) {
                    cell.style.paddingRight = 20 + "px";
                }
            }
            var strWidth = jsonGridConfiguration.columnNames[cellCounter].width.toString();
            //// Reducing 10px from width, to accommodate 10px left padding at individual cell level, as per red lines
            if (strWidth.indexOf("%") !== -1) {
                cell.style.width = jsonGridConfiguration.columnNames[cellCounter].width - 10;
                cell.style.minWidth = jsonGridConfiguration.columnNames[cellCounter].width - 10;
                cell.style.maxWidth = jsonGridConfiguration.columnNames[cellCounter].width - 10;
            } else {
                if ("doctype" !== jsonGridConfiguration.gridHeader[cellCounter].toLowerCase()) {
                    cell.style.width = jsonGridConfiguration.columnNames[cellCounter].width - 10 + "px";
                    cell.style.minWidth = jsonGridConfiguration.columnNames[cellCounter].width - 10 + "px";
                    cell.style.maxWidth = jsonGridConfiguration.columnNames[cellCounter].width - 10 + "px";
                }
            }
            if ("matter" === jsonGridConfiguration.gridHeader[cellCounter].toLowerCase() || "document" === jsonGridConfiguration.gridHeader[cellCounter].toLowerCase()) {
                cell.setAttribute("onclick", "showDetailsPopup(this, event);");
            }
            if ("check box" === jsonGridConfiguration.gridHeader[cellCounter].toLowerCase()) {
                cell.setAttribute("class", "jsonGridRow checkBox");
                cell.style.width = jsonGridConfiguration.columnNames[cellCounter].width + "px";
                cell.style.minWidth = jsonGridConfiguration.columnNames[cellCounter].width + "px";
                cell.style.maxWidth = jsonGridConfiguration.columnNames[cellCounter].width + "px";
            }
            if (jsonGridConfiguration.columnNames[cellCounter].paddingRight) {
                cell.style.paddingRight = jsonGridConfiguration.columnNames[cellCounter].paddingRight + "px";
            }
            if (jsonGridConfiguration.altRowColor && this.rowPosition % 2 !== 0) {
                cell.style.backgroundColor = jsonGridConfiguration.altRowColor;
            }
            if (!jsonGridConfiguration.columnNames[cellCounter].formatter) {
                if (jsonGridConfiguration.columnNames[cellCounter].trimOnOverflow) {
                    cell.innerHTML = oGrid.getAdjustedRowChunk((jsonGridConfiguration.data[this.rowPosition][jsonGridConfiguration.columnNames[cellCounter].name] || "NA"), jsonGridConfiguration.columnNames[cellCounter].width);
                } else {
                    if (jsonGridConfiguration.columnNames[cellCounter].trimOnOverflowAndShowToolTip) {
                        cell.innerHTML = (jsonGridConfiguration.data[this.rowPosition][jsonGridConfiguration.columnNames[cellCounter].name] || "NA");
                    } else {
                        cell.innerHTML = (jsonGridConfiguration.data[this.rowPosition][jsonGridConfiguration.columnNames[cellCounter].name] || "NA");
                    }
                }
            } else {
                var sFormattedChunk = window[jsonGridConfiguration.columnNames[cellCounter].formatter](jsonGridConfiguration.data[this.rowPosition][jsonGridConfiguration.columnNames[cellCounter].name], jsonGridConfiguration.data[this.rowPosition], jsonGridConfiguration.columnNames[cellCounter].width, this.rowPosition);
                cell.innerHTML = sFormattedChunk;
            }
            if (jsonGridConfiguration.columnNames[cellCounter].style) {
                oGrid.applyStyleToObject(cell, jsonGridConfiguration.columnNames[cellCounter].style);
            }
        }
        if (jsonGridConfiguration.drilldown) {
            if (jsonGridConfiguration.drilldownAt === ">") {
                cell = row.insertCell(numberofColumns);
                cell.setAttribute("class", "jsonGridRow WebdingArrow");
                cell.parentNode.setAttribute("onclick", 'showHideDrill(this,"' + jsonGridConfiguration.data[this.rowPosition].C1 + '")');
            }
            var row1 = jsonGridConfiguration.tblBody.insertRow(-1);
            row1.setAttribute("class", "itemHide");
            cell = row1.insertCell(0);
            cell.setAttribute("colspan", jsonGridConfiguration.columnNames.length + 1);
        }
    }

    // check each time for select all item value
    if (selectAllFlagIs) {
        // select all the rows
        var oCheckBox = $(".jsonGridRow").find("input[type='checkbox']");
        var className;
        if (oCheckBox[0]) {
            className = oCheckBox[0].className;
        }
        $("." + className).prop("checked", true);
    }
};

oGrid.CreateEndRow = function (JsonGridConfiguration) {
    "use strict";
    if (JsonGridConfiguration.tblBody && JsonGridConfiguration.appendEndRow) {
        $(JsonGridConfiguration.tblBody).append(JsonGridConfiguration.endRowChunk);
    }
};


oGrid.CreateHTMLTableWithHeader = function (jsonGridConfiguration) {
    "use strict";
    var tHead = jsonGridConfiguration.tblHead,
        row = tHead.insertRow(-1),
        cell = null,
        LoopCounter, drillCounter = 0,
        numberOfHeaderColumns = jsonGridConfiguration.gridHeader.length;
    if (jsonGridConfiguration.drilldown && jsonGridConfiguration.drilldownAt === "<") {
        cell = row.insertCell(0);
        drillCounter = 1;
        cell.setAttribute("class", "jsonGridHeader");
    }
    for (LoopCounter = 0; LoopCounter < (numberOfHeaderColumns) ; LoopCounter += 1) {
        cell = row.insertCell(LoopCounter + drillCounter);
        cell.setAttribute("id", jsonGridConfiguration.columnNames[LoopCounter].id);
        cell.setAttribute("class", "jsonGridHeader");
        cell.style.width = jsonGridConfiguration.columnNames[LoopCounter].width + "px";
        //// Reducing 10px from width, to accommodate 10px left padding at individual header level, as per red lines
        if ("DocType" !== jsonGridConfiguration.gridHeader[LoopCounter]) {
            cell.style.maxWidth = jsonGridConfiguration.columnNames[LoopCounter].width - 10 + "px";
            cell.style.minWidth = jsonGridConfiguration.columnNames[LoopCounter].width - 10 + "px";
        }
        if (LoopCounter + 1 === numberOfHeaderColumns) {
            cell.style.paddingRight = 15 + "px";
        }
        cell.style.textAlign = jsonGridConfiguration.columnNames[LoopCounter].align;
        if (jsonGridConfiguration.columnNames[LoopCounter].sortable) {
            cell.setAttribute("onclick", 'oGrid.sortJsonGrid(this,"' + jsonGridConfiguration.container + '_Grid","' + jsonGridConfiguration.columnNames[LoopCounter].name + '")');
            cell.style.cursor = "pointer";
            if (jsonGridConfiguration.columnNames[LoopCounter].name === jsonGridConfiguration.sortby) {
                if (jsonGridConfiguration.sortorder === "asc") {
                    cell.setAttribute("sortorder", "desc");
                } else {
                    cell.setAttribute("sortorder", "asc");
                }
            } else {
                cell.setAttribute("sortorder", jsonGridConfiguration.initialsortorder);
            }
        } else {
            cell.style.cursor = "default";
        }

        // add the header with check box
        if ("check box" === jsonGridConfiguration.gridHeader[LoopCounter].toLowerCase() && 0 === LoopCounter) {
            var oCheckBox, className, iIterator = 0;
            cell.setAttribute("class", "jsonGridHeader mandatory");
            cell.innerHTML =
                "<div class=\"floatContentLeft\">"
                + "<input id=\"isSelectRowsActive\" class=\"documentCheckbox\" type=\"checkbox\"/>"
                + "</div>"
                + "<div class=\"floatContentLeft threeQuarterContentWidth\" ></div>";
            $(document.body).on("change", "#isSelectRowsActive", function () {
                var iSelectedTab;
                if (1 === oGridConfig.currentView) {
                    iSelectedTab = "#pinnedGrid_Grid";
                } else if (2 === oGridConfig.currentView) {
                    iSelectedTab = "#RecentDocumentContainer_Grid";
                } else {
                    iSelectedTab = "#grid_Grid";
                }
                // checked
                // check if the check box is already selected or not??
                if (this.checked) {
                    // print the starting index and end index of that row..
                    // and add the check mark to all the rows..

                    oCheckBox = $("" + iSelectedTab + "").find(".jsonGridRow").find("input[type='checkbox']");
                    className = oCheckBox[0].className.split(" ");
                    for (iIterator = 0; iIterator <= oCheckBox.length - 1; iIterator++) {
                        if (!oCheckBox[iIterator].checked) {
                            oCheckBox[iIterator].checked = true;
                            $(iSelectedTab + " tbody tr:nth-child(" + (iIterator + 1) + ")").find("input[type='checkbox']").trigger("change");
                        }
                    }
                    selectAllFlagIs = true;
                } else {
                    // unchecked
                    oCheckBox = $("" + iSelectedTab + "").find(".jsonGridRow").find("input[type='checkbox']");
                    className = oCheckBox[0].className.split(" ");
                    for (iIterator = 0; iIterator <= oCheckBox.length - 1; iIterator++) {
                        if (oCheckBox[iIterator].checked) {
                            oCheckBox[iIterator].checked = false;
                            $(iSelectedTab + " tbody tr:nth-child(" + (iIterator + 1) + ")").find("input[type='checkbox']").trigger("change");
                        }
                    }
                    selectAllFlagIs = false;
                }
            });
        } else if ("pin" === jsonGridConfiguration.gridHeader[LoopCounter].toLowerCase() || "upload" === jsonGridConfiguration.gridHeader[LoopCounter].toLowerCase()) {
            cell.setAttribute("class", "jsonGridHeader mandatory");
            cell.innerHTML = "";
        } else if ("doctype" === jsonGridConfiguration.gridHeader[LoopCounter].toLowerCase()) {
            cell.setAttribute("class", "jsonGridHeader mandatory");
            cell.innerHTML = "<img class='docTypeIconHeader' id='docTypeIcon' src='" + oGlobalConstants.Image_General_Document + "' alt='Document type icon'>";
        } else if ("matter" === jsonGridConfiguration.gridHeader[LoopCounter].toLowerCase() || "document" === jsonGridConfiguration.gridHeader[LoopCounter].toLowerCase()) {
            cell.setAttribute("class", "jsonGridHeader mandatory");
            cell.innerHTML = jsonGridConfiguration.gridHeader[LoopCounter];
        } else {
            cell.innerHTML = jsonGridConfiguration.gridHeader[LoopCounter];
        }
    }
    if (jsonGridConfiguration.drilldown && jsonGridConfiguration.drilldownAt === ">") {
        cell = row.insertCell(numberOfHeaderColumns);
        drillCounter = 1;
        cell.setAttribute("class", "jsonGridHeader");
    }
    if (jsonGridConfiguration.drilldown) {
        row.setAttribute("style", "display:none");
    }
};
oGrid.applyStyleToObject = function (oGridObject, oStyleObject) {
    "use strict";
    var oStyles = Object.keys(oStyleObject);
    var iCounter = 0;
    for (iCounter; iCounter < oStyles.length; iCounter += 1) {
        try {
            oGridObject.style[oStyles[iCounter]] = oStyleObject[oStyles[iCounter]];
        } catch (e) {
        }
    }
};
oGrid.CreateHTMLTable = function (jsonGridConfiguration) {
    "use strict";
    var grid = document.createElement("table");
    grid.cellPadding = jsonGridConfiguration.cellPadding;
    grid.cellSpacing = jsonGridConfiguration.cellSpacing;
    grid.setAttribute("id", jsonGridConfiguration.gridName);
    if (typeof (jsonGridConfiguration.container) === "string") {
        grid.setAttribute("class", "jsonGrid");
    } else {
        grid.setAttribute("class", "InnerJsonGrid");
    }
    if (jsonGridConfiguration.style) {
        oGrid.applyStyleToObject(grid, jsonGridConfiguration.style);
    }
    jsonGridConfiguration.containerObject.appendChild(grid);
    return grid;
};
oGrid.JsonGrid = function (gridConfigOptions) {
    "use strict";
    var attr;
    var tBody;
    var checkKeys;
    if (typeof (gridConfigOptions.container) === "string") {
        this.containerObject = document.getElementById(gridConfigOptions.container);
    } else {
        this.containerObject = gridConfigOptions.container;
    }
    if (gridConfigOptions.data.length <= 0) {
        return false;
    }
    if (this.containerObject) {
        this.gridOptions = {
            caption: "",
            container: "",
            gridName: "",
            data: [],
            gridHeader: [],
            columnNames: [],
            style: {},
            altRowColor: "",
            sortby: "",
            sortorder: "",
            sortType: "",
            initialsortorder: "",
            retainpageonsort: false,
            maxRows: 0,
            viewrecords: true,
            pagination: true,
            cellSpacing: 0,
            cellPadding: 0,
            appendEndRow: false,
            endRowChunk: "",
            drilldownConfig: null
        };
        for (attr in gridConfigOptions) {
            if (attr !== "drilldownConfig") {
                this.gridOptions[attr] = clone(gridConfigOptions[attr]);
            } else {
                this.gridOptions.drilldownConfig = gridConfigOptions["drilldownConfig"];
            }
        }
        if (!this.gridOptions.pagination) {
            this.gridOptions.maxRows = this.gridOptions.data.length;
        }
        this.gridOptions.containerObject = this.containerObject;
        this.gridOptions.currentPage = 0;
        if (this.gridOptions.maxRows > 0) {
            this.gridOptions.totalPages = Math.ceil(this.gridOptions.data.length / this.gridOptions.maxRows) - 1;
        } else {
            this.gridOptions.totalPages = 0;
        }
        this.gridOptions.gridName = this.gridOptions.container + "_Grid";
        if (this.gridOptions.gridHeader.length !== this.gridOptions.columnNames.length) {
            return false;
        }

        for (this.LoopCounter = 0; this.LoopCounter < this.gridOptions.columnNames.length; this.LoopCounter += 1) {
            if (!this.gridOptions.data[0].hasOwnProperty([this.gridOptions.columnNames[this.LoopCounter].name].toString()) && !("pin" === [this.gridOptions.columnNames[this.LoopCounter].name].toString().toLowerCase() || "upload" === [this.gridOptions.columnNames[this.LoopCounter].name].toString().toLowerCase() || "checkbox" === [this.gridOptions.columnNames[this.LoopCounter].name].toString().toLowerCase() || "doctype" === [this.gridOptions.columnNames[this.LoopCounter].name].toString().toLowerCase())) {
                return false;
            }
        }
        if (this.gridOptions.sortby) {

            if (this.gridOptions.sortorder === "asc") {

                this.gridOptions.data.sort(sortBy(this.gridOptions.sortby, true, this.gridOptions.sortType));

            }
            if (this.gridOptions.sortorder === "desc") {

                this.gridOptions.data.sort(sortby(this.gridOptions.sortby, false, this.gridOptions.sortType));

            }
        }
        this.gridObject = oGrid.CreateHTMLTable(this.gridOptions);
        this.gridOptions.tblHead = this.gridObject.createTHead();
        this.gridOptions.tblFoot = this.gridObject.createTFoot();
        tBody = document.createElement("tbody");
        this.gridObject.appendChild(tBody);
        this.gridOptions.tblBody = tBody;
        this.gridOptions.gridObject = this.gridObject;
        new oGrid.CreateHTMLTableWithHeader(this.gridOptions);
        new oGrid.CreateHTMLTableRow(this.gridOptions);
        if (this.gridOptions.pagination) {
            new oGrid.CreatePaginationControl(this.gridOptions);
        }

        var indexPositionOfCurrentGrid = oGrid.gridName.indexOf(this.gridOptions.gridName);
        if (indexPositionOfCurrentGrid > -1) {
            oGrid.gridObject[indexPositionOfCurrentGrid] = this.gridOptions;
        } else {
            oGrid.gridName.push(this.gridOptions.gridName);
            oGrid.gridObject.push(this.gridOptions);
        }
        this.containerObject = null;
        this.gridObject = null;
        this.LoopCounter = null;
    }
};

/* Function to generate the grid view JSON */
oGrid.generateGridViewJSON = function () {
    "use strict";
    var gridViewJSON = [], arrColumnNames = [], arrColumnWidth = [], arrColumnFormatter = [];
    if (oGridConfig.isMatterView) {
        /* Get the data that is to be shown in Search Matter grid view control */
        //// If pinned data is requested (flag = 4) get the different set of field names
        if (oGridConfig.bLoadPinnedData) {
            arrColumnNames = $.trim(oWebDashboardConstants.MatterPinnedColumnValueFields) ? $.trim(oWebDashboardConstants.MatterPinnedColumnValueFields).split(";") : "";
        } else {
            arrColumnNames = $.trim(oWebDashboardConstants.MatterColumnValueFields) ? $.trim(oWebDashboardConstants.MatterColumnValueFields).split(";") : "";
        }
        arrColumnWidth = $.trim(oWebDashboardConstants.MatterColumnWidth) ? $.trim(oWebDashboardConstants.MatterColumnWidth).split(";") : "";
        arrColumnFormatter = $.trim(oWebDashboardConstants.MatterColumnFormatter) ? $.trim(oWebDashboardConstants.MatterColumnFormatter).split(";") : "";
    } else {
        /* Get the data that is to be shown in Search Document grid view control */
        //// If pinned data is requested (flag = 4) get the different set of field names
        if (oGridConfig.bLoadPinnedData) {
            arrColumnNames = $.trim(oWebDashboardConstants.DocumentPinnedColumnValueFields) ? $.trim(oWebDashboardConstants.DocumentPinnedColumnValueFields).split(";") : "";
        } else {
            arrColumnNames = $.trim(oWebDashboardConstants.DocumentColumnValueFields) ? $.trim(oWebDashboardConstants.DocumentColumnValueFields).split(";") : "";
        }
        arrColumnWidth = $.trim(oWebDashboardConstants.DocumentColumnWidth) ? $.trim(oWebDashboardConstants.DocumentColumnWidth).split(";") : "";
        arrColumnFormatter = $.trim(oWebDashboardConstants.DocumentColumnFormatter) ? $.trim(oWebDashboardConstants.DocumentColumnFormatter).split(";") : "";
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

function createRadio(cellValue, rowObject, width) {
    "use strict";
    return '<input type="radio" name="createDoc" id="DMS_' + rowObject.Name + '" value="' + cellValue + '"/>';
}

function createDocLink(cellValue, rowObject, width) {
    "use strict";
    return "<a class=\"docLink\" href=\"" + rowObject.Path + "\">" + cellValue + "</a>";
}

function showHideDrill(oObject) {
    "use strict";
    if (oObject) {
        var firstChild = oObject.children[0];
        if (firstChild && WinJS.Utilities.hasClass(firstChild, "DrillExpandIcon")) {
            WinJS.Utilities.removeClass(firstChild, "DrillExpandIcon");
            WinJS.Utilities.addClass(firstChild, "DrillCollapseIcon");
            if (oObject && oObject.nextSibling) {
                removeClass(oObject.nextSibling, "itemHide");
                addClass(oObject, "expandedRow");
            }
        } else {
            WinJS.Utilities.removeClass(firstChild, "DrillCollapseIcon");
            WinJS.Utilities.addClass(firstChild, "DrillExpandIcon");
            if (oObject && oObject.nextSibling) {
                addClass(oObject.nextSibling, "itemHide");
                removeClass(oObject, "expandedRow");
            }
        }
    }
}

function createPinMatter(cellValue, rowObject, width) {
    "use strict";
    return "<div><span class='matterTitle' data-matterName = '" + oCommonObject.renderAsText(cellValue) + "'>" + oCommonObject.renderAsText(cellValue) + "</span><br /><span class='matterMetadata'>" + oCommonObject.renderAsText(rowObject.MatterContentType) + "</span><br /><span class='matterMetadata'>" + oCommonObject.renderAsText(rowObject.MatterCreatedDate) + "</span><img  class='uploadImg'  alt='Unpin' src='../Images/unpin-666.png' onclick = 'unpinMatter(this)'/><img class='uploadImg' src='../Images/upload-666.png' title='Upload documents' onclick= 'uploadElement()'/></div>";
}

function uploadElement(nIndex, event) {
    "use strict";
    var matterName, matterUrl, oMatterFolderDetails, matterClientUrl, matterGuid, oMatter = {};
    if (oGridConfig.currentView) {
        if (1 === oGridConfig.currentView) {
            matterName = oGridConfig.arrPinnedData[nIndex].MatterName;
            matterUrl = oGridConfig.arrPinnedData[nIndex].MatterClientUrl;
            matterGuid = oGridConfig.arrPinnedData[nIndex].MatterGuid;
        } else {
            matterName = oGridConfig.arrRecentData[nIndex][oGlobalConstants.Matter_Name];
            matterUrl = oGridConfig.arrRecentData[nIndex].SiteName;
            matterGuid = oGridConfig.arrRecentData[nIndex][oGlobalConstants.Matter_GUID];
        }
    } else {
        matterName = oGridConfig.arrGridData[nIndex][oGlobalConstants.Matter_Name];
        matterUrl = oGridConfig.arrGridData[nIndex].SiteName;
        matterGuid = oGridConfig.arrGridData[nIndex][oGlobalConstants.Matter_GUID];
    }
    oUploadGlobal.sMatterURL = matterUrl;
    oMatterFolderDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "matterData": { "MatterName": matterName, "MatterUrl": matterUrl } };
    oMatter = { "MatterGuid": matterGuid, "OriginalName": matterName };
    oCommonObject.callSearchService("GetFolderHierarchy", oMatterFolderDetails, folderHierarchySuccess, folderHierarchyFailure, folderHierarchyBeforeCall, oMatter);
    matterClientUrl = 1 === oGridConfig.currentView ? oGridConfig.arrPinnedData[nIndex].MatterUrl : oGridConfig.arrGridData[nIndex].Path;
    $("#matterLibraryURL").attr("href", matterClientUrl).attr("target", "_blank").text(matterName).attr("title", matterName);
    event.stopPropagation();
    commonFunction.closeAllFilterExcept(".folderStructure");
}

function folderHierarchySuccess(result) {
    "use strict";
    // Dynamically decrease the height of the popup
    oCommonObject.updateUploadPopupHeight(false);
    oCommonObject.getContentCheckConfigurations(oUploadGlobal.sMatterURL);
    $(".popupWait, .loadingImage").addClass("hide");
    var oTreeNodes = JSON.parse(result.Result), sHTMLChunk, oMatter = result.oParam;
    oSearchGlobal.oFolderName = oTreeNodes;
    if (oTreeNodes.code) {
        showCommonErrorPopUp(oTreeNodes.code);
        return;
    }
    sHTMLChunk = buildNestedList(oTreeNodes, null);
    $(".folderStructureContent").html(sHTMLChunk);

    // Making icons non draggable on upload popup
    $(".mailContainer img").attr("draggable", "false");
    $("#attachmentHeader").removeClass("hide");
    $(".maildata").html("<div class='noFilesUploaded'>" + oGlobalConstants.No_Files_Uploaded + "</div>");
    if ("undefined" !== typeof result.oParam.matterData) {
        $("#rootLink").attr("title", result.oParam.matterData.MatterName);
    }
    $(".folderStructureContent ul li ul li").droppable();
    if (oMatter) {
        oCommonObject.addUploadAttributes(oMatter.OriginalName, oMatter.MatterGuid);
    }
    $("#mailBody").on("drop", ".folderStructureContent ul li", function (e, ui) {
        e.stopPropagation();
        e.stopImmediatePropagation();
        e.preventDefault();
        $(this).removeClass("folderDragOver");
        if (e.dataTransfer && e.dataTransfer.files && 0 !== e.dataTransfer.files.length) {
            oSearchGlobal.oFileArray = e.dataTransfer.files;
            oSearchGlobal.oDataArray.length = 0;

            var sClientRelativeUrl = "undefined" !== typeof $(e.target).attr("data-client") ? $(e.target).attr("data-client") : $(e.target).parent().attr("data-client");
            var sFolderUrl = "undefined" !== typeof $(e.target).attr("data-foldername") ? $(e.target).attr("data-foldername") : $(e.target).parent().attr("data-foldername");
            var isOverwrite = "False";
            uploadFile(sClientRelativeUrl, sFolderUrl, isOverwrite);
        }
    });

    $(".popupContainerBackground, .mailContainer").removeClass("hide");
    if (("" !== oSearchGlobal.sClientName && "" !== oSearchGlobal.sClientSiteUrl) || oSearchGlobal.bIsTenantCall) {
        $(".mailContainer").addClass("popupSP");
    }
}

$(document).mousedown(function (e) {
    "use strict";
    if ($(e.target).is(".popupContainerBackground")) {
        closeAllPopup();
    }
});

$(document).mousedown(function (e) {
    "use strict";
    if ($(e.target).is(".mailContainer")) {
        $(".errorPopUp").addClass("hide");
        oUploadGlobal.arrFiles = [];
        oUploadGlobal.arrContent = [];
    }
});

$(".popUpCloseIcon").click(function () {
    "use strict";
    $(".mailContainer .notification").remove();
    $(".popupContainerBackground, .mailContainer").addClass("hide");
    $(".errorPopUp").addClass("hide");
    $(".popUpContainer").css("height", "");
    commonFunction.clearGlobalVariables();
    $(".folderStructureContent, .parentNode").removeClass("folderStructureWithBreadcrumb");
});

function closeAllPopup() {
    "use strict";
    $(".popUpCloseIcon").click();
    $(".errorPopUp").addClass("hide");
    oUploadGlobal.arrFiles = [];
    oUploadGlobal.arrContent = [];
    commonFunction.clearGlobalVariables();
    $(".folderStructureContent, .parentNode").removeClass("folderStructureWithBreadcrumb");
}

function folderHierarchyBeforeCall() {
    "use strict";
    $(".loadingImage").css("position", "absolute");
    $(".popupWait, .loadingImage").removeClass("hide");
}

function folderHierarchyFailure(result) {
    "use strict";
    $(".popupWait, .loadingImage").addClass("hide");
}

function buildNestedList(treeNodes, rootId) {
    "use strict";
    var nodesByParent = {};
    $.each(treeNodes, function (iIndex, node) {
        if (!(node.parenturl in nodesByParent)) { nodesByParent[node.parenturl] = []; }
        nodesByParent[node.parenturl].push(node);
    });

    function buildTree(children) {
        var $container = $("<ul>");
        if (!children) { return; }
        $.each(children, function (iIndex, child) {
            if (null !== child.parenturl) {
                if (null !== oUploadGlobal.oDrilldownParameter.sCurrentParentUrl && oUploadGlobal.oDrilldownParameter.sCurrentParentUrl === child.parenturl) {
                    $("<li>", { html: "<div class='treeNodes' title='" + child.name + "'><img class='folderIcon' src='" + oGlobalConstants.Site_Url + "/_layouts/15/images/folder.gif' align='center' alt='" + child.name + "' data-foldername='" + child.url + "' data-parentname='" + child.parenturl + "'/>" + child.name + "</div>" })
                    .css({ display: "inline-block", width: "90%", padding: "0" })
                    .attr("data-client", oUploadGlobal.oDrilldownParameter.sRootUrl)
                    .attr("data-foldername", child.url)
                    .attr("data-parentname", child.parenturl)
                    .appendTo($container)
                    .append(buildTree(nodesByParent[child.url]));
                } else if (null !== oUploadGlobal.oDrilldownParameter.sCurrentParentUrl && oUploadGlobal.oDrilldownParameter.sCurrentParentUrl === child.url) {
                    $("<li>", { html: "<div class='treeNodes hide' title='" + child.name + "'><img class='folderIcon' src='" + oGlobalConstants.Site_Url + "/_layouts/15/images/folder.gif' align='center' alt='" + child.name + "' data-foldername='" + child.url + "' data-parentname='" + child.parenturl + "'/>" + child.name + "</div>" })
                    .css({ display: "inline-block", width: "100%", padding: "0" })
                    .attr("data-client", oUploadGlobal.oDrilldownParameter.sRootUrl)
                    .attr("data-foldername", child.url)
                    .attr("data-parentname", child.parenturl)
                    .addClass("parentNode")
                    .appendTo($container)
                    .append(buildTree(nodesByParent[child.url]));
                }
            } else {
                oUploadGlobal.oDrilldownParameter.sRootUrl = child.url;
                oUploadGlobal.oDrilldownParameter.sCurrentParentUrl = child.url;
                oUploadGlobal.oDrilldownParameter.nCurrentLevel++;
                $("<li>", { html: "<div class='treeNodes hide' title='" + child.name + "'><img class='folderIcon' src='" + oGlobalConstants.Site_Url + "/_layouts/15/images/folder.gif' align='center' alt='" + child.name + "' data-foldername='" + child.url + "' data-parentname='" + child.parenturl + "'/>" + child.name + "</div>" })
                .css({ display: "inline-block", width: "100%", padding: "0" })
                .attr("data-client", oUploadGlobal.oDrilldownParameter.sRootUrl)
                .attr("data-foldername", child.url)
                .attr("data-parentname", child.parenturl)
                .addClass("parentNode")
                .appendTo($container)
                .append(buildTree(nodesByParent[child.url]));
            }
        });
        return $container;
    }
    return buildTree(nodesByParent[rootId]);
}

function uploadFile(sClientRelativeUrl, sFolderUrl, isOverwrite) {
    "use strict";
    if (sClientRelativeUrl && sFolderUrl) {
        // App Insight Event tracking for Attachment Upload
        commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Local_Upload, true);
        // Check if the loading image is already present or not. If the loading image is not present, then append it
        var oNotificationContainerForMailPopup = $(".notificationContainerForMailPopup");
        if (!oNotificationContainerForMailPopup.find(".uploadDocumentLoading").length) { oNotificationContainerForMailPopup.append("<img class=\"uploadDocumentLoading\" src=\"/images/loading_metro.gif\" />"); }
        // Consolidate the output element. 
        $("body #form2").remove();
        $("body").append("<form id='form2' class='hide' runat='server' enctype='multipart/form-data'></form>");
        var form = document.querySelector("#form2"),
            data = new FormData(form),
            documentLibraryName = $("#mailContent").attr("data-originalname");

        data.append("SPAppToken", oSharePointContext.SPAppToken);
        data.append("ClientUrl", oGlobalConstants.Site_Url + sClientRelativeUrl.substring(0, sClientRelativeUrl.lastIndexOf("/")));
        data.append("RefreshToken", oSharePointContext.RefreshToken);
        data.append("FolderName", sFolderUrl);
        if ("undefined" !== typeof documentLibraryName) {
            data.append("DocumentLibraryName", documentLibraryName);
            data.append("AllowContentCheck", oUploadGlobal.bAllowContentCheck);
        }

        var nCounter = 0;
        for (nCounter = 0; nCounter < oSearchGlobal.oFileArray.length; nCounter++) {
            if (oSearchGlobal.oFileArray[nCounter]) {
                data.append(oSearchGlobal.oFileArray[nCounter].name, oSearchGlobal.oFileArray[nCounter]);
                data.append("Overwrite" + nCounter, isOverwrite);
            }
        }

        oUploadGlobal.oXHR.onreadystatechange = function () {
            if (4 === oUploadGlobal.oXHR.readyState && 200 === oUploadGlobal.oXHR.status && oUploadGlobal.oXHR.responseText) {
                $(".uploadDocumentLoading").remove();
                var arrResponse = $("<div/>").html(oUploadGlobal.oXHR.responseText).text().split("$|$");
                var overwriteExists = false;
                var errorOccured = false;
                var sOptionContent = "";
                for (var responseCount = 0; responseCount < arrResponse.length - 1; responseCount++) {
                    var response = arrResponse[responseCount].split(":::");
                    var errorResponse = arrResponse[responseCount].split("$$$");
                    if (1 < errorResponse.length) {
                        sContent = "<div class='notification uploadDocumentNotification warningNotification'> <img id = 'warningImg' src = '../Images/warning-message.png'/> <div class='overWriteTextContainer'><div id = 'overWriteDocumentNameWebdashboard'>" + errorResponse[0] + "</div><div class='askForOverwrite'> <input type='button' id = 'overWriteOk' value='" + oGlobalConstants.Upload_Ok_Button + "' data-operation='ignore'  onClick='oCommonObject.localOverWriteDocument(this);' /></div></div></div>";
                        oCommonObject.updateUploadPopupHeight(true);
                        $(".notificationContainerForMailPopup").append(sContent);
                        // Making icons non draggable on upload popup
                        $(".mailContainer img").attr("draggable", "false");
                        oUploadGlobal.sNotificationMsg = "";
                        errorOccured = true;
                    } else {
                        var sContent = "";
                        var sExtension = response[0].substring(response[0].lastIndexOf(".") + 1);
                        if (response.length > 1) {
                            var nIDCounter = $(".maildata .attachmentSection").length;
                            oUploadGlobal.arrOverwrite[responseCount] = "False";
                            sContent = "<div class='attachment'>";
                            var sFolderName = oCommonObject.getRootFolderForMatter(response[1]);
                            sContent += "<div class='attachmentName mailName'><div id='" + nIDCounter + "attachment-Status'><img class=\"uploadSuccessStatus\" src=\"/images/success-message.png\" /></div><img class='attachIcon' id='" + nIDCounter + "attachIcon' src='" + oGlobalConstants.Image_Document_Icon.replace("{0}", sExtension) + "' alt='attachment icon' onerror='errorImage(this);'><div id='" + nIDCounter + "attachment' title='" + response[0] + "' class='popupName popupSelect'>" + response[0] + "</div><div id='" + nIDCounter + "attachment-Message' class='uploadSuccessMessage' title='" + sFolderName + "'>(" + sFolderName + ")</div>";

                            sContent += "</div>";
                            sContent += "</div>";

                            if (0 === nIDCounter) {
                                $(".maildata").html("<div class='attachmentSection'></div>");
                            }
                            $(".maildata .attachmentSection").append(sContent);
                            oCommonObject.showNotification(oGlobalConstants.Upload_Success_Notification, "successNotification ms-font-weight-semibold");
                            oUploadGlobal.sNotificationMsg = "";
                            // Making icons non draggable on upload popup
                            $(".mailContainer img").attr("draggable", "false");
                        } else {
                            oUploadGlobal.sClientRelativeUrl = sClientRelativeUrl;
                            oUploadGlobal.sFolderUrl = sFolderUrl;
                            oUploadGlobal.arrOverwrite[responseCount] = "True";

                            var duplicateNotification = response[0].split("@@@");
                            var contentCheckPerformed = response[0].split("|||");

                            // update the content as per the logic
                            var fileName = "undefined" !== typeof oSearchGlobal.oFileArray[responseCount] && oSearchGlobal.oFileArray[responseCount].name ? oSearchGlobal.oFileArray[responseCount].name.trim() : "",
                            bAppendEnabled = oCommonObject.overwriteConfiguration(fileName),
                            // True means show append button and False means hide append button
                            sAppendContent = bAppendEnabled ? "<input type='button' id = 'overWriteAppend' data-operation='append' title='" + oGlobalConstants.Upload_Append_Button_Tooltip + "'  value='" + oGlobalConstants.Upload_Append_Button + "' onClick='oCommonObject.localOverWriteDocument(this);' />" : "";

                            var sContentCheckChunk = "";
                            if (oUploadGlobal.bAllowContentCheck && duplicateNotification[1] && "TRUE" === duplicateNotification[1].toUpperCase()) {
                                sContentCheckChunk = "<input type='button' id = 'contentCheck' title='" + oGlobalConstants.Upload_Content_Check_Tooltip + "' value='" + oGlobalConstants.Upload_ContentCheck_Button + "' data-operation='contentCheck' onClick='oCommonObject.localOverWriteDocument(this); oCommonObject.contentCheckNotification(true);' />";
                            }

                            //// Capture First message from page and preserve for later usage.
                            if ("undefined" !== typeof oUploadGlobal.sNotificationMsg && "" === oUploadGlobal.sNotificationMsg) {
                                oUploadGlobal.sNotificationMsg = duplicateNotification[0];
                            }

                            if (duplicateNotification[1]) {
                                //// Potential duplicate found, show notification to perform content check or overwrite or append
                                sOptionContent = sContentCheckChunk + "<input type='button' id = 'overWriteYes' title= '" + oGlobalConstants.Upload_Overwrite_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Overwrite_Button + "' data-operation='overwrite' onClick='oCommonObject.localOverWriteDocument(this);'/>" + sAppendContent + "<input type='button' id = 'overWriteNo' title='" + oGlobalConstants.Upload_Cancel_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Cancel_Button + "' data-operation='ignore' onClick='oCommonObject.localOverWriteDocument(this);'/>";
                                sContent = oCommonObject.getNotificationContent(duplicateNotification[0], "", sOptionContent);
                            } else if (contentCheckPerformed[1]) {
                                //// Content Check is performed, show notification to overwrite or append
                                $(".notification").remove();
                                oCommonObject.updateUploadPopupHeight(false);
                                sContentCheckChunk = ("TRUE" === contentCheckPerformed[1].toUpperCase()) ? sContentCheckChunk : "";
                                sOptionContent = sContentCheckChunk + "<input type='button' id = 'overWriteYes' title= '" + oGlobalConstants.Upload_Overwrite_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Overwrite_Button + "' data-operation='overwrite' onClick='oCommonObject.localOverWriteDocument(this);'/>" + sAppendContent + "<input type='button' id = 'overWriteNo' title='" + oGlobalConstants.Upload_Cancel_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Cancel_Button + "' data-operation='ignore' onClick='oCommonObject.localOverWriteDocument(this);'/>";
                                sContent = oCommonObject.getNotificationContent(oUploadGlobal.sNotificationMsg, contentCheckPerformed[0], sOptionContent);

                                //// clear previous stored notification
                                oUploadGlobal.sNotificationMsg = "";
                            }
                            oCommonObject.updateUploadPopupHeight(true);
                            $(".notificationContainerForMailPopup").append(sContent);
                            // Making icons non draggable on upload pop-up
                            $(".mailContainer img").attr("draggable", "false");
                            oUploadGlobal.arrFiles.push(oSearchGlobal.oFileArray[responseCount]);
                            overwriteExists = true;
                        }
                    }
                }
                oCommonObject.updateNotificationPosition();
            } else if (4 === oUploadGlobal.oXHR.readyState && 200 !== oUploadGlobal.oXHR.status) {
                // TODO: if upload fails, display file name and red cross icon
                $(".uploadDocumentLoading").remove();
            }
            if (overwriteExists || errorOccured) {
                // Dynamically increase the height of the popup
                oCommonObject.updateUploadPopupHeight(true);
            }
        };
        oUploadGlobal.oXHR.open("POST", "UploadFile.aspx");
        oUploadGlobal.oXHR.setRequestHeader("RequestValidationToken", oMasterGlobal.Tokens);
        oUploadGlobal.oXHR.send(data);
    } else {
        // Could not upload because of issue with data
    }
}

function showPopupNotification(sMsg, resultClass) {
    "use strict";
    var sContent = "";
    sContent = "<div class='notification " + resultClass + "'>" + sMsg + "</div>";
    $(".mailContainer .notification").remove();
    $(".mailContainer").prepend(sContent);
}

// Function to close the upload success notification
$(document).on("click", ".mailContainer .notification .closeNotification", function () {
    "use strict";
    var minRequiredHeight = 283, updatedHeight = $(".mailContainer").height() - 30;
    $(".mailContainer .successNotification").remove();
    if (minRequiredHeight <= updatedHeight) {
        $(".mailContainer").height(updatedHeight);  // Adjusting the height of the popup, post removing notification        
    }
});