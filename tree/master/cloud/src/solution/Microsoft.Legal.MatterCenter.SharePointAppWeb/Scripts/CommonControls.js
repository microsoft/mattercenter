/// <disable>JS1003,JS2032,JS2074,JS2076,JS2024,JS3116,JS3085,JS3058,JS3092,JS3057,JS3054,JS2073,JS2005,JS3056</disable>

//// JavaScript file for configurable components present in Matter Center

/* Object for drop down control */
var oDropdownControl = (function () {
    "use strict";
    /* Common functions container */
    var commonFunctions = {};

    /* Function to generate the ECB menu for actions mentioned in the "sCurrentView" parameter */
    commonFunctions.generateDropdownControl = function (sDropdownSelector, arrCurrentView) {
        var oDropdownTerms = []
            , sSelectElement = "<div class='ms-Dropdown'><select class='ms-Dropdown-select'>{0}</select></div>" //// HTML chunk of container for drop down menu
            , sOptionElement = "<option>{0}</option>"
            , oDropdownContainer = $(sDropdownSelector);

        //// Generate the HTML chunk of items present in the drop down menu
        $.each(arrCurrentView, function (iIterator, oCurrentItem) {
            var sDropdownItem = $.trim(oCurrentItem) ? $.trim(oCurrentItem).split(",") : ""; //// Separate the action name and class name from current item
            if (sDropdownItem) {
                var sDropdownClass = sOptionElement.replace("{0}", sDropdownItem[0]);
                oDropdownTerms.push(sDropdownClass);
            }
        });

        //// Generate the finalized HTML chunk of drop down menu
        var sDropDownChunk = sSelectElement.replace("{0}", oDropdownTerms.join(""));

        //// Populate the HTML chunk of ECB menu and bind the JS of fabric component
        oDropdownContainer.html(sDropDownChunk).find(".ms-Dropdown").dropdown();
        oDropdownContainer.find(".ms-Dropdown .ms-Dropdown-title").addClass("ms-font-weight-semilight");
        //// Attach class to each drop down element, here we will need to re-iterate as the HTML chunk needs to be ready before attaching class
        var oDropDownItems = oDropdownContainer.find(".ms-Dropdown-items .ms-Dropdown-item");
        $.each(arrCurrentView, function (iIterator, oCurrentItem) {
            var sDropdownItem = $.trim(oCurrentItem) ? $.trim(oCurrentItem).split(",") : ""; //// Separate the action name and class name from current item
            if (2 === sDropdownItem.length) {
                if ("mydata" === sDropdownItem[1]) {
                    $(oDropDownItems[iIterator]).addClass(sDropdownItem[1] + " ms-font-weight-semilight searchPanelDropdownOption selectedDropdownOption");
                } else {
                    $(oDropDownItems[iIterator]).addClass(sDropdownItem[1] + " ms-font-weight-semilight searchPanelDropdownOption");
                }
            }
        });
        oCommonObject.updateDropDownToolTip();
    };

    /* Return the functions which are exposed outside this JS file */
    return {
        generateDropdownControl: function (sDropdownSelector, arrCurrentView) {
            commonFunctions.generateDropdownControl(sDropdownSelector, arrCurrentView);
        }
    };
})();

var oColumnPickerControl = (function () {
    "use strict";
    /* Common functions container */
    var commonFunctions = {};

    /* Function to generate the ECB menu for actions mentioned in the "sCurrentView" parameter */
    commonFunctions.generateColumnPickerControl = function (sColumnPickerSelector, arrCurrentView) {
        var sColumnPickerChunk = "", tempChunk = "";
        var sCheckBoxChunk = oGlobalConstants.ColumnPickerChunk;
        var sAllColumnHeader = sCheckBoxChunk.replace(/\{0}/g, "").replace(/\{1}/g, oGlobalConstants.GridViewAllColumnOption).replace(/\{2}/g, null).replace(/\{3}/g, "");
        sColumnPickerChunk = sAllColumnHeader;
        $.each(arrCurrentView, function (iIterator, oCurrentItem) {
            tempChunk = sCheckBoxChunk.replace(/\{0}/g, iIterator).replace(/\{1}/g, oCurrentItem).replace(/\{2}/g, iIterator);
            sColumnPickerChunk += (0 === iIterator) ? tempChunk.replace(/\{3}/g, "defaultSelection") : tempChunk.replace(/\{3}/g, "");
        });
        $(sColumnPickerSelector).html(sColumnPickerChunk);
        $(sColumnPickerSelector).find(".defaultSelection").prop("checked", true);
    };

    /* Return the functions which are exposed outside this JS file */
    return {
        generateColumnPickerControl: function (sColumnPickerSelector, arrCurrentView) {
            commonFunctions.generateColumnPickerControl(sColumnPickerSelector, arrCurrentView);
        }
    };
})();

/* Function to hide the common control on resize and click events */
$(document).on("click", function (event) {
    "use strict";
    var sCurrentItemClass = (event.target && event.target.className) ? event.target.className : "";
    var sCurrentItemId = (event.target && event.target.id) ? event.target.id : "";
    if (-1 < sCurrentItemClass.indexOf("columnOptionName") || -1 < sCurrentItemClass.indexOf("ms-ChoiceField-input") || "columnOptions" === sCurrentItemId) {
        oCommonObject.closeAllPopupExcept("columnPickerPanel", event);
    } else if (-1 < sCurrentItemClass.indexOf("FlyoutContent") || -1 < sCurrentItemClass.indexOf("FlyoutBoxContent") || -1 < sCurrentItemClass.indexOf("flyoutLeftarrow")) {
        oCommonObject.closeAllPopupExcept("InfoFlyout", event);
    } else {
        oCommonObject.closeAllPopupExcept("", event);  //// Close all pop-ups
    }
});

/*Function to stop click event when enter is pressed on Chrome*/
$(document).on("keypress", function (e) {
    "use strict";
    if (e.which === 13) { // Checks for the enter key
        e.preventDefault(); // Stops Chrome from triggering the button to be clicked
    }
});

$(window).on("resize", function (event) {
    "use strict";
    $(".ms-Dropdown").removeClass("ms-Dropdown--open");
    $(".autoCompletePanel").empty().addClass("hide");
});
