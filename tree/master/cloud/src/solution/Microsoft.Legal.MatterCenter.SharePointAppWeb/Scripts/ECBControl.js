/// <disable>JS1003,JS2005,JS2023,JS2024,JS2027,JS2031,JS2032,JS2052,JS2053,JS2064,JS2073,JS2074,JS2076,JS3054,JS3056,JS3057,JS3058,JS3085,JS3092,JS3116</disable>

/* Object for ECB Control */
var oECBControl = (function () {
    "use strict";
    /* Common functions container */
    var commonFunctions = {};

    /* Function to generate the ECB menu for actions mentioned in the "sCurrentView" parameter */
    commonFunctions.generateECBControl = function (arrCurrentView, arrECBTitle) {
        var oECBActionsChunk = []
            , sULElement = "<ul class='ms-ContextualMenu'><div class='ContextualMenuWrapper'><img class='mattersiteloading hide' src='../Images/WindowsLoadingFast.GIF' alt='Loading data...' title='Please wait...' />{0}</div></ul>" //// HTML chunk of container for ECB menu
            , sLIElement = "<li class=\"ms-ContextualMenu-item\"><a class=\"ECBItem ms-ContextualMenu-link {0}\" title=\"{2}\">{1}</a></li>"; //// HTML chunk of item in ECB menu

        //// Generate the HTML chunk of items present in the ECB menu
        $.each(arrCurrentView, function (iCurrentIndex, sCurrentItem) {
            var arrActionClass = $.trim(sCurrentItem) ? $.trim(sCurrentItem).split(",") : ""; //// Separate the action name and class name from current item
            if (2 === arrActionClass.length) {
                var currLIElement = sLIElement.replace("{0}", arrActionClass[1]).replace(/\{1\}/g, arrActionClass[0]).replace("{2}", arrECBTitle[iCurrentIndex]);
                oECBActionsChunk.push(currLIElement);
            }
        });

        //// Generate the finalized HTML chunk of ECB menu
        var sECBActionsChunk = sULElement.replace("{0}", oECBActionsChunk.join(""));

        //// Populate the HTML chunk of ECB menu and bind the JS of fabric component
        $("#ECBControl").html(sECBActionsChunk).contextualMenu();
    };

    /* Return the functions which are exposed outside this JS file */
    return {
        generateECBControl: function (arrCurrentView, arrECBTitle) {
            commonFunctions.generateECBControl(arrCurrentView, arrECBTitle);
        }
    };
})();
