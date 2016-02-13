/// <disable>JS3092, JS3116, JS2074, JS2073, JS2076, JS3058, JS2064, JS2053, JS3053, JS3057, JS3054, JS2032</disable>
if (typeof oGrid === "undefined") {
    var oGrid = {};
}
oGrid.gridName = [];
oGrid.gridObject = [];

function renderConsumptionDot(cellValue, rowJsonObject) {
    "use strict";
    if (cellValue === "NA") {
        return "<span>" + cellValue + "</span><div class='dvCnsp dv30Cnsp'></div>";
    } else if (cellValue < 30) {
        return "<span>" + cellValue + "% </span><div class='dvCnsp dv30Cnsp'></div>";
    } else if (cellValue >= 30 && cellValue < 60) {
        return "<span>" + cellValue + "% </span><div class='dvCnsp dv60Cnsp'></div>";
    } else {
        return "<span>" + cellValue + "% </span><div class='dvCnsp dv70Cnsp'></div>";
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
function hasClass(el, customname) {
    "use strict";
    if (el) {
        return new RegExp("(\\s|^)" + customname + "(\\s|$)").test(el.className);
    }
}
function addClass(el, customname) {
    "use strict";
    if (el) {
        if (!hasClass(el, customname)) { el.className += (el.className ? " " : "") + customname; }
    }
}
function removeClass(el, customname) {
    "use strict";
    if (el) {
        if (hasClass(el, customname)) {
            el.className = el.className.replace(new RegExp("(\\s|^)" + customname + "(\\s|$)"), " ").replace(/^\s+|\s+$/g, "");
        }
    }
}
function sortBy(field, reverse, primer) {
    "use strict";
    var key = function (x) {
        return primer ? primer(x[field]) : x[field];
    };
    return function (a, b) {
        var A = key(a), B = key(b);
        if (primer === parseFloat || primer === parseInt) {
            if (String(A) === "NaN") {
                A = 0;
            }
            if (String(B) === "NaN") {
                B = 0;
            }
        }
        return ((A < B) ? -1 : (A > B) ? +1 : 0) * [-1, 1][+!!reverse];
    };
}
function clone(obj) {
    "use strict";
    // Handle the 3 simple types, and null or undefined
    var copy;
    var iIterator = 0;
    var attr;
    if (obj === null || typeof obj !== "object") {
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
    return "<div class='jsonGridOverflow' title='" + inputData + "' style='width: " + width + "px;'><span class='PaginateSpan'>" + inputData + "<span></div>";
};
oGrid.getAdjustedRowChunkAndToolTip = function (inputData, width) {
    "use strict";
    width = width.replace("%", "");
    var windowWidth = document.querySelector("#reportHead").offsetWidth * (parseInt(width) / 100);
    return "<span class='jsonGridOverflow' title='" + inputData + "' style='width: " + (windowWidth - 40) + "px;'>" + inputData + "</span>";
};
oGrid.setViewRecords = function (currentGridConfig) {
    "use strict";
    var ELE = document.getElementById(currentGridConfig.gridName + "_ViewRecords");
    var startItem = currentGridConfig.currentPage * currentGridConfig.maxRows + 1;
    var endItem = (currentGridConfig.currentPage + 1) * currentGridConfig.maxRows;
    var lastPage = currentGridConfig.data.length;
    endItem = endItem > lastPage ? lastPage : endItem;
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
    prev.setAttribute("src", "../Images/pagination-back-disabled.png");
    first.setAttribute("src", "../Images/pagination-back-disabled.png");
};
oGrid.enablePrev = function (gridname) {
    "use strict";
    var prev = document.getElementById(gridname + "_Prev");
    var first = document.getElementById(gridname + "_First");
    removeClass(prev, "click-disabled");
    removeClass(first, "click-disabled");
    prev.setAttribute("src", "../Images/pagination-back-enabled.png");
    first.setAttribute("src", "../Images/pagination-back-Enabled.png");
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
            }
        }
    }
};
oGrid.sortJsonGrid = function (cellObject, gridName, fieldName) {
    "use strict";
    if (oGrid.gridName.length > 0) {
        var gridObjectPosition = oGrid.gridName.indexOf(gridName);
        if (-1 < gridObjectPosition) {
            var sortOrder = cellObject.getAttribute("sortorder");
            var sortFlag = true;
            if (sortOrder === "asc") {
                sortFlag = false;
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
    /* Hide the columns which are not selected using column picker */
    var oHeaderArray = $(".jsonGridHeader");
    $.each(oHeaderArray, function (sCurrentIndex, oCurrentValue) {
        if ($(oCurrentValue).hasClass("hide")) {
            var oVisibleCellSet = $(".GridRow .jsonGridRow:nth-child(" + parseInt(sCurrentIndex + 1) + ")")
              , oVisbleAlternateCellSet = $(".GridRowAlternate .jsonGridRow:nth-child(" + parseInt(sCurrentIndex + 1) + ")");
            oVisibleCellSet.addClass("hide");
            oVisbleAlternateCellSet.addClass("hide");
        }
    });
};
oGrid.CreatePaginationControl = function (jsonGridConfiguration) {
    "use strict";
    var row = jsonGridConfiguration.tblFoot.insertRow(-1);
    var cell = row.insertCell(0);
    var paginationDiv = document.createElement("div");
    var firstDiv = document.createElement("div");
    var prevDiv = document.createElement("div");
    var viewRecordDiv = document.createElement("div");
    var viewRecordSpan = document.createElement("span");
    viewRecordSpan.className = "PaginateSpan";
    var nextDiv = document.createElement("div");
    var lastDiv = document.createElement("div");

    cell.colSpan = jsonGridConfiguration.columnNames.length;
    firstDiv.innerHTML = '<img title="First" id="' + jsonGridConfiguration.gridName + '_First"  class="first cur-pointer click-disabled"  src = "../Images/pagination-back-disabled.png" active="1" onclick="oGrid.goFirst(this,\'' + jsonGridConfiguration.gridName + '\');" />';
    firstDiv.className = "jsongrid-first";
    prevDiv.innerHTML = '<img title="Previous" id="' + jsonGridConfiguration.gridName + '_Prev"  active="1" class="prev cur-pointer middle click-disabled" src = "../Images/pagination-back-disabled.png" onclick="oGrid.goPrevious(this,\'' + jsonGridConfiguration.gridName + '\');return false;" />';
    prevDiv.className = "jsongrid-prev";
    viewRecordDiv.appendChild(viewRecordSpan);
    viewRecordDiv.id = jsonGridConfiguration.gridName + "_ViewRecords";
    viewRecordDiv.className = "ViewRecordDiv";
    if (!jsonGridConfiguration.viewrecords) {
        viewRecordDiv.style.visibility = "hidden";
    }


    nextDiv.innerHTML = '<img title="Next" id="' + jsonGridConfiguration.gridName + '_Next"  class="next cur-pointer middle"  active="0"  src = "../Images/pagination-front-enabled.png" onclick="oGrid.goNext(this,\'' + jsonGridConfiguration.gridName + '\'); return false;" />';
    nextDiv.className = "jsongrid-next";

    lastDiv.innerHTML = '<img title="Last" id="' + jsonGridConfiguration.gridName + '_Last"  class="last cur-pointer"  active="0" src = "../Images/pagination-front-Enabled.png" onclick="oGrid.goLast(this,\'' + jsonGridConfiguration.gridName + '\')"/>';
    lastDiv.className = "jsongrid-last";

    paginationDiv.className = "jsonGridFooter pagination";
    paginationDiv.appendChild(firstDiv);
    paginationDiv.appendChild(prevDiv);
    paginationDiv.appendChild(viewRecordDiv);
    paginationDiv.appendChild(nextDiv);
    paginationDiv.appendChild(lastDiv);
    var testDiv = document.getElementById("gridPaginationTD");
    testDiv.appendChild(paginationDiv);
    cell.appendChild(paginationDiv);
    new oGrid.setViewRecords(jsonGridConfiguration);
    if (jsonGridConfiguration.maxRows && jsonGridConfiguration.maxRows >= jsonGridConfiguration.data.length) {
        var nextImg = document.querySelector("#" + jsonGridConfiguration.gridName + "_Next");
        $("#" + jsonGridConfiguration.gridName + "_Last").addClass("click-disabled");
        // WinJS.Utilities.addClass(nextImg, "click-disabled");
        oGrid.disableNext(jsonGridConfiguration.gridName);

    }

};
oGrid.CreateHTMLTableRow = function (jsonGridConfiguration, rowPosition) {
    "use strict";
    var startIndex = jsonGridConfiguration.currentPage * jsonGridConfiguration.maxRows;
    var endIndex = (startIndex + jsonGridConfiguration.maxRows) <= jsonGridConfiguration.data.length ? (startIndex + jsonGridConfiguration.maxRows) : jsonGridConfiguration.data.length;
    endIndex = endIndex <= 0 ? jsonGridConfiguration.data.length : endIndex;
    var cell = null, cellblank = null;
    var cellCounter, drillCounter = 0;
    var numberofColumns = jsonGridConfiguration.columnNames.length;
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
                cell.parentNode.setAttribute("onclick", "LoadNextRow(this,'" + jsonGridConfiguration.data[this.rowPosition].C1 + "','" + jsonGridConfiguration.container + "')");
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
            if (strWidth.indexOf("%") !== -1) {
                cell.style.width = jsonGridConfiguration.columnNames[cellCounter].width - 10;
                cell.style.minWidth = jsonGridConfiguration.columnNames[cellCounter].width - 10;
                cell.style.maxWidth = jsonGridConfiguration.columnNames[cellCounter].width - 10;
            } else {
                if ("DocType" !== jsonGridConfiguration.gridHeader[cellCounter]) {
                    cell.style.width = jsonGridConfiguration.columnNames[cellCounter].width - 10 + "px";
                    cell.style.minWidth = jsonGridConfiguration.columnNames[cellCounter].width - 10 + "px";
                    cell.style.maxWidth = jsonGridConfiguration.columnNames[cellCounter].width - 10 + "px";
                }
            }
            if ("Check Box" === jsonGridConfiguration.gridHeader[cellCounter] || "ECB" === jsonGridConfiguration.gridHeader[cellCounter]) {
                cell.style.width = jsonGridConfiguration.columnNames[cellCounter].width + "px";
                cell.style.minWidth = jsonGridConfiguration.columnNames[cellCounter].width + "px";
                cell.style.maxWidth = jsonGridConfiguration.columnNames[cellCounter].width + "px";
            }
            if (jsonGridConfiguration.columnNames[cellCounter].paddingRight) {
                cell.style.paddingRight = jsonGridConfiguration.columnNames[cellCounter].paddingRight + "px";
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
                // WinJS.Utilities.setInnerHTMLUnsafe(cell, sFormattedChunk);
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


                cell.parentNode.setAttribute("onclick", "showHideDrill(this,'" + jsonGridConfiguration.data[this.rowPosition].C1 + "')");

            }
            var row1 = jsonGridConfiguration.tblBody.insertRow(-1);
            row1.setAttribute("class", "itemHide");
            cell = row1.insertCell(0);

            cell.setAttribute("colspan", jsonGridConfiguration.columnNames.length + 1);
        }
    }
    //// check each time for select all item value
    if (oCommonObject.isAllRowSelected) {
        //// select all the rows
        var oGridRow = $(".GridRow, .GridRowAlternate");
        oGridView.highlightGridViewRow(oGridRow, true);
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
    var tHead = jsonGridConfiguration.tblHead;
    var row = tHead.insertRow(-1);

    var cell = null;
    var LoopCounter, drillCounter = 0;
    var numberOfHeaderColumns = jsonGridConfiguration.gridHeader.length;
    var arrColumnNames = null;
    arrColumnNames = oCommonObject.isMatterView ? $.trim(oFindMatterConstants.GridViewHeaderName).split(";") : $.trim(oFindDocumentConstants.GridViewHeaderName).split(";");
    arrColumnNames = $.each(arrColumnNames, function (iCounter) {
        arrColumnNames[iCounter] = this.substr(this.indexOf(",") + 1);
    });
    if (jsonGridConfiguration.drilldown && jsonGridConfiguration.drilldownAt === "<") {
        cell = row.insertCell(0);
        drillCounter = 1;
        cell.setAttribute("class", "jsonGridHeader");
    }
    for (LoopCounter = 0; LoopCounter < (numberOfHeaderColumns) ; LoopCounter += 1) {
        cell = row.insertCell(LoopCounter + drillCounter);
        cell.setAttribute("id", jsonGridConfiguration.columnNames[LoopCounter].id);
        cell.setAttribute("class", "jsonGridHeader");
        if (jsonGridConfiguration.gridHeaderTitle[LoopCounter] && "" !== jsonGridConfiguration.gridHeaderTitle[LoopCounter]) {
            cell.setAttribute("title", jsonGridConfiguration.gridHeaderTitle[LoopCounter]);
        }
        if ("DocType" !== jsonGridConfiguration.gridHeader[LoopCounter]) {
            cell.style.maxWidth = jsonGridConfiguration.columnNames[LoopCounter].width - 10 + "px";
            cell.style.minWidth = jsonGridConfiguration.columnNames[LoopCounter].width - 10 + "px";
        }
        if ("ECB" === jsonGridConfiguration.gridHeader[LoopCounter]) {
            cell.style.maxWidth = jsonGridConfiguration.columnNames[LoopCounter].width + "px";
            cell.style.minWidth = jsonGridConfiguration.columnNames[LoopCounter].width + "px";
        }
        if (LoopCounter === 0 && "Check Box" !== jsonGridConfiguration.gridHeader[LoopCounter]) {
            cell.style.paddingLeft = 5 + "px";
        }
        if (LoopCounter + 1 === numberOfHeaderColumns) {
            cell.style.paddingRight = 15 + "px";
        }
        cell.style.textAlign = jsonGridConfiguration.columnNames[LoopCounter].align;
        if (jsonGridConfiguration.columnNames[LoopCounter].sortable && !("CheckBox" === [jsonGridConfiguration.columnNames[LoopCounter].name].toString() || "ECB" === [jsonGridConfiguration.columnNames[LoopCounter].name].toString() || "DocType" === [jsonGridConfiguration.columnNames[LoopCounter].name].toString())) {
            cell.setAttribute("onclick", "oGrid.sortJsonGrid(this,'" + jsonGridConfiguration.container + "_Grid','" + jsonGridConfiguration.columnNames[LoopCounter].name + "')");
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
            cell.style.cursor = "pointer";
        }
        //// add the header with check box
        if ("Check Box" === jsonGridConfiguration.gridHeader[LoopCounter]) {
            var oCheckBox, className, iIterator = 0;
            cell.style.maxWidth = jsonGridConfiguration.columnNames[LoopCounter].width + "px";
            cell.innerHTML =
                "<div class=\"floatContentLeft\">"
                + "<div class=\"ms-ChoiceField\"><input class=\"ms-ChoiceField-input isSelectRowsActive checkBox\" id=\"demo-checkbox-unselected\" type=\"checkbox\"><label class=\"ms-ChoiceField-field\" for=\"demo-checkbox-unselected\"></label></div>"
                + "</div>";

            $(document.body).on("change", ".isSelectRowsActive", function () {
                //// checked
                //// check if the check box is already selected or not??
                var oGridRow = $(".GridRow, .GridRowAlternate");
                if (this.checked) {
                    oGridView.highlightGridViewRow(oGridRow, true);
                    oCommonObject.isAllRowSelected = true;
                } else {
                    oGridView.highlightGridViewRow(oGridRow, false);
                    oCommonObject.isAllRowSelected = false;
                }
            });
        } else if ("ECB" === jsonGridConfiguration.gridHeader[LoopCounter]) {
            cell.innerHTML = "";
        } else if ("DocType" === jsonGridConfiguration.gridHeader[LoopCounter]) {
            cell.innerHTML = "<img class='docTypeIconHeader' id='docTypeIcon' src='" + oGlobalConstants.Image_General_Document + "' alt='Document type icon'>";
        } else {
            if ("NA" === arrColumnNames[LoopCounter]) {
                cell.innerHTML = "<span class=\"headerTitle\">" + jsonGridConfiguration.gridHeader[LoopCounter] + "</span>" + "<span id=\"sort" + jsonGridConfiguration.columnNames[LoopCounter].id + "\" class = \"sort hide\">&#x2191;</span><i class=\"ms-Icon ms-Icon--filter hide\"></i>";
            } else {
                cell.innerHTML = "<span class=\"headerTitle\">" + jsonGridConfiguration.gridHeader[LoopCounter] + "</span>" + "<span id=\"sort" + jsonGridConfiguration.columnNames[LoopCounter].id + "\" class = \"sort hide\">&#x2191;</span><i class=\"ms-Icon ms-Icon--filter hide\"></i>" + "<i class='ms-Icon ms-Icon--caretDown' title='Open " + jsonGridConfiguration.gridHeader[LoopCounter] + " sort and filter menu'></i>";
            }
            cell.setAttribute("filterFlyOutType", oCommonObject.oHeaderFilterType[LoopCounter]);
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
            gridHeaderTitle: [],
            columnNames: [],
            style: {},
            altRowColor: "",
            sortby: "",
            sortorder: "asc",
            sortType: "",
            initialsortorder: "asc",
            retainpageonsort: true,
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
        if (this.gridOptions.gridHeader.length !== this.gridOptions.columnNames.length || this.gridOptions.gridHeader.length !== this.gridOptions.gridHeaderTitle.length) {
            return false;
        }

        for (this.LoopCounter = 0; this.LoopCounter < this.gridOptions.columnNames.length; this.LoopCounter += 1) {
            if (!this.gridOptions.data[0].hasOwnProperty([this.gridOptions.columnNames[this.LoopCounter].name].toString()) && !("CheckBox" === [this.gridOptions.columnNames[this.LoopCounter].name].toString() || "ECB" === [this.gridOptions.columnNames[this.LoopCounter].name].toString() || "DocType" === [this.gridOptions.columnNames[this.LoopCounter].name].toString())) {
                return false;
            }
        }
        if (this.gridOptions.sortby) {

            if (this.gridOptions.sortorder === "asc") {

                this.gridOptions.data.sort(sortBy(this.gridOptions.sortby, true, this.gridOptions.sortType));

            }
            if (this.gridOptions.sortorder === "desc") {

                this.gridOptions.data.sort(sortBy(this.gridOptions.sortby, false, this.gridOptions.sortType));

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

function createRadio(cellValue, rowObject, width) {
    "use strict";
    return "<input type='radio' name='createDoc' id='DMS_" + rowObject.Name + "' value='" + cellValue + "'/>";
}
function createDocLink(cellValue, rowObject, width) {
    "use strict";
    return "<a class='docLink' href='" + rowObject.Path + "'>" + cellValue + "</a>";
}
function showHideDrill(oObject) {
    "use strict";
    if (oObject) {
        var firstChild = oObject.children[0];
        if (firstChild && WinJS.Utilities.hasClass(firstChild, "DrillExpandIcon")) {
            if (oObject && oObject.nextSibling) {
                removeClass(oObject.nextSibling, "itemHide");
                addClass(oObject, "expandedRow");
            }
        } else {
            if (oObject && oObject.nextSibling) {
                addClass(oObject.nextSibling, "itemHide");
                removeClass(oObject, "expandedRow");
            }
        }
    }
}

function createPinMatter(cellValue, rowObject, width) {
    "use strict";
    return "<div class='pinMatterTile' onclick='showMatterDetailPopup(this,\"Pinned Matters\",event)' ><span title='" + oCommonObject.renderAsText(cellValue) + "' class='matterTitle' data-mattername = '" + oCommonObject.renderAsText(cellValue) + "' data-client='" + oCommonObject.renderAsText(rowObject.MatterClientUrl) + "' data-matterurl='" + oCommonObject.renderAsText(rowObject.MatterUrl) + "' data-practicegroup='" + oCommonObject.renderAsText(rowObject.MatterPracticeGroup) + "' data-areaoflaw='" + oCommonObject.renderAsText(rowObject.MatterAreaOfLaw) + "' data-subareaoflaw='" + oCommonObject.renderAsText(rowObject.MatterSubAreaOfLaw) + "'data-matterResponsibleAttorney='" + (oCommonObject.renderAsText(rowObject.MatterResponsibleAttorney) || "NA") + "' data-clientID='" + (oCommonObject.renderAsText(rowObject.MatterClientId) || "NA") + "' data-matterID= '" + (oCommonObject.renderAsText(rowObject.MatterID) || "NA") + "' >" + oCommonObject.renderAsText(cellValue) + "</span><br /><span class='matterMetadata clientIDMatterID' title='" + (oCommonObject.renderAsText(rowObject.MatterClientId) || "NA") + oGlobalConstants.ClientID_MatterID_Separator + (oCommonObject.renderAsText(rowObject.MatterID) || "NA") + "'>" + (oCommonObject.renderAsText(rowObject.MatterClientId) || "NA") + oGlobalConstants.ClientID_MatterID_Separator + (oCommonObject.renderAsText(rowObject.MatterID) || "NA") + "</span><br /><img  class='iconUnpin' title='Unpin' src='../Images/unpin-666.png' alt='unpin' onclick = 'unpinMatter(this);event.stopPropagation();'/><img alt='upload' class='uploadImg' title='Upload' src='../Images/upload-666.png' onclick='populateFolderHierarchy(this); event.stopPropagation();' /></div>";
}