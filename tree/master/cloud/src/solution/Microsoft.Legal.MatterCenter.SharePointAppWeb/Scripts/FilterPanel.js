/// <disable>JS1003,JS2076,JS2032,JS2064,JS2074,JS2024,JS2026,JS2005,JS3085,JS3116,JS3092,JS3057,JS3058,JS3056</disable>
/* Common JS file for filter panel */

var oFilterGlobal = {
    siteURL: oGlobalConstants.Central_Repository_Url,
    refinerHTMLTemplate: "<div class='refinerRow'><label class='refinerLabels'><input type='checkbox' class='refinerCheckBox' name='Client' value='#RefinerValue#' data-PG='PGValue' /><div class='filterRefinerEntry' id='filterRefiner#RefinerValue#'>#RefinerValue#</div></label><div class='refinerSeparator'></div></div>",
    regularRefiner: new RegExp("#RefinerValue#", "g"),
    selectAllNone: "<div class='refinerRowLink refinerRowLinkRight'><a class='selectAllNone' href='javascript:void(0)' onclick='commonFunction.refinerSelectAll(this)'>Select All</a></div><div class='refinerRowLink'><a class='selectAllNone' href='javascript:void(0)' onclick='commonFunction.refinerSelectNone(this)'>Select None</a><div class='refinerSeparator'></div></div>",
    noRecordRow: "<div class='noRefiners#noRefinerVisible#'>No records found</div><div class='refinerSeparator'></div>",
    regularNoRefinerRow: new RegExp("#noRefinerVisible#", "g"),
    refinerOkButton: "<div class='filterOkButton'><div class='tileOk buttonOk' id='filterOk#SearchType#' onclick='commonFunction.refinerOkClick(\".filterSearch#SearchType#\")'>Ok</div></div>",
    regularRefinerType: new RegExp("#SearchType#", "g"),
    refinerCancelButton: "<div class='filterOkButton'><div class='tileOk buttonCancel' id='filterOk#SearchType#' onclick='commonFunction.refinerCancelClick(this)'>Cancel</div></div>",
    clientsList: "",
    oTermStoreData: "",
    isVisible: false,
    arrOWADocumentExtension: [],
    arrVisioFileExtension: []
},
oJQueryBindings = {
    $ClientText: "",
    $PracticeGroupText: "",
    $AreaOfLawText: "",
    $SearchMatter: "",
    $SearchDocument: "",
    $SwitchApp: "",
    $SpecificName: "",
    $DocumentContainers: "",
    $IsVisible: "",
},
commonFunction = {},

oPageConfig = {
    currentPage: 0
};

/* Function to show all options */
commonFunction.showAllOptions = function ($ShowAll) {
    "use strict";
    $ShowAll.children(".refinerRow").show();
};

/* Function to check selected check boxes */
commonFunction.checkSelected = function (arrSelectedText, $SelectedCheckbox) {
    "use strict";
    $.each(arrSelectedText, function (index, value) {
        var oRelevantRow = $SelectedCheckbox.find(".refinerRow").find("input[type=checkbox][value='" + value.trim() + "']");
        if (oRelevantRow.length) {
            oRelevantRow.prop("checked", "checked");
        }
    });
};

/* Function to reset to saved state */
commonFunction.resetToSavedState = function ($ResetThis) {
    "use strict";
    if ($ResetThis.parent().children(".filterSearchRefinerText").attr("data-selected")) {
        var sResetText = $ResetThis.parent().children(".filterSearchRefinerText").attr("data-selected").split("$|$"), totalCount = $ResetThis.parent().children(".filterSearchRefinerText").attr("data-totalcount");
        $ResetThis.children(".refinerRow").children("input[type=checkbox]").removeAttr("checked");
        if (totalCount && 0 < (parseInt(totalCount, 10))) {
            $ResetThis.children(".noRefiners").hide();
            $ResetThis.children(".refinerRowLink").show();
        } else {
            $ResetThis.children(".noRefiners").show();
            $ResetThis.children(".refinerRowLink").hide();
        }
        $ResetThis.parent().children(".filterSearchRefinerText").val(sResetText.join(", "));
        commonFunction.checkSelected(sResetText, $ResetThis);
        commonFunction.showAllOptions($ResetThis);
    }
};

/* Function to close all pop up except the specified pop up */
commonFunction.closeAllFilterExcept = function (sCloseThis, event) {
    "use strict";
    if ("filterSearchAdvance" !== sCloseThis && "refinerClient" !== sCloseThis && "refinerPG" !== sCloseThis && "refinerAOL" !== sCloseThis && "ui-datepicker" !== sCloseThis && (oGridConfig.isMatterView || (event && event.target && event.target.className !== "ui-corner-all"))) {
        $(".filterSearchAdvance").hide();
    }
    if ("refinerClient" !== sCloseThis) {
        $(".refinerClient").hide();
    }
    if ("refinerPG" !== sCloseThis) {
        $(".refinerPG").hide();
    }
    if ("refinerAOL" !== sCloseThis) {
        $(".refinerAOL").hide();
    }
    if ("matterPopup" !== sCloseThis) {
        $(".matterPopup").hide();
    }
    if ("documentPopup" !== sCloseThis) {
        $(".documentPopup").hide();
    }
    if ("filterPanel" !== sCloseThis) {
        $(".filterPanel").hide();
    }
    if ("filterAutoComplete" !== sCloseThis) {
        $(".filterAutoComplete").addClass("hide");
    }
    if ("switchApp" !== sCloseThis) {
        $(".switchApp").hide();
    }
    if ("checkOutPopup" !== sCloseThis) {
        $(".checkOutPopup").hide();
    }
    if ("switchTab" !== sCloseThis) {
        $(".switchTab").hide();
    }
    if ("sdBannerPanel" !== sCloseThis) {
        $(".sdBannerPanel").addClass("hide");
    }
    sCloseThis && event && event.stopPropagation();
};

/* Function to search on key up */
function filterKeyUp(eventKeyUp, oSearchObject) {
    "use strict";
    var arrSearch = $(oSearchObject).val().split(",")
    , sTrimmedText = arrSearch[arrSearch.length - 1].trim()
    , $SearchThis = $(oSearchObject).parent().children(".filterSearchRefinerDDL");
    if (arrSearch[0]) {
        if ($SearchThis.is(":hidden")) {
            $(oSearchObject).parent().children(".filterSearchRefinerDDL").width($(oSearchObject).width());
            commonFunction.showAllOptions($SearchThis);
            $(oSearchObject).parent().children(".filterSearchRefinerDDL").toggle();
            eventKeyUp.stopPropagation();
        }
        $SearchThis.find(".refinerRow").find("input[type=checkbox]").removeAttr("checked");
        $SearchThis.find(".refinerRow").hide();
        commonFunction.checkSelected(arrSearch, $SearchThis);
        if (sTrimmedText) {
            var $RelavantSearch = $SearchThis.find(".refinerRow").find("input[type=checkbox]").filter(function () {
                return $(this).attr("value").toLowerCase().indexOf(sTrimmedText.toLowerCase()) > -1;
            });
            if ($RelavantSearch.length) {
                $RelavantSearch.prop("checked", "checked");
                $RelavantSearch.parent().parent().show();
                $SearchThis.children(".refinerWrapper").show();
                $SearchThis.children(".noRefiners").hide();
                $SearchThis.children(".refinerRowLink").show();
            } else {
                $SearchThis.children(".refinerWrapper").hide();
                $SearchThis.children(".noRefiners").show();
                $SearchThis.children(".refinerRowLink").hide();
            }
        } else {
            commonFunction.showAllOptions($(".refinerWrapper"));
        }
    } else {
        commonFunction.showAllOptions($(".refinerWrapper"));
        commonFunction.refinerSelectNone($SearchThis);
        $(".noRefiners").hide();
        $(".refinerRowLink").show();
    }
    eventKeyUp.stopPropagation();
}

/* Functions for fetching client from Term store */
commonFunction.onClientSuccess = function (refiners) {
    "use strict";

    var refinerClientHTML = oFilterGlobal.selectAllNone;
    refinerClientHTML = "<div class='refinerWrapper'>";
    var sClientTileHTML = "";
    var oClients = JSON.parse(refiners);
    oFilterGlobal.clientsList = JSON.parse(JSON.stringify(oClients.ClientTerms));
    $.each(oFilterGlobal.clientsList, function (iIndex, val) {
        refinerClientHTML = refinerClientHTML + oFilterGlobal.refinerHTMLTemplate.replace(oFilterGlobal.regularRefiner, val.Name);
    });
    if (refinerClientHTML) {
        refinerClientHTML = refinerClientHTML + "</div>" + oFilterGlobal.noRecordRow.replace(oFilterGlobal.regularNoRefinerRow, "") + oFilterGlobal.refinerOkButton.replace(oFilterGlobal.regularRefinerType, "Clients") + oFilterGlobal.refinerCancelButton;
    } else {
        refinerClientHTML = oFilterGlobal.noRecordRow.replace(oFilterGlobal.regularNoRefinerRow, " noRefinersVisible");
    }
    $(".refinerClient").length && $(".refinerClient").html(refinerClientHTML);
    oJQueryBindings.$ClientText.attr("data-totalcount", $(".filterSearchClients input[type='checkbox']").length);
    oJQueryBindings.$ClientText.attr("data-selectedcount", 0);
};

/* Function to show loading image before call to service */
commonFunction.onBeforeSend = function (refiners) {
    "use strict";
    $(".tileContainer").html("<div class='loadingDiv'><img src='/Images/WindowsLoadingFast.GIF' title='Loading' class='LoadingImage' alt='Loading...' /><br /><br /> <span class='loadingMessageTop'>" + oGlobalConstants.Loading_Clients + "</span></div>");
};

/* Functions for fetching Practice Groups and Area of Laws */
commonFunction.onPGSuccess = function (refiners) {
    "use strict";
    var oPracticeGroup = JSON.parse(refiners);
    if (!oPracticeGroup.code) {
        var sPGHtml = oFilterGlobal.selectAllNone + "<div class='refinerWrapper'>";
        var sAOLHtml = oFilterGlobal.selectAllNone + "<div class='refinerWrapper'>";
        oFilterGlobal.oTermStoreData = JSON.parse(refiners);

        if (oPracticeGroup.PGTerms) {
            $.each(oPracticeGroup.PGTerms, function (iIndex, sValue) {
                var $AOLObject = sValue;

                sPGHtml = sPGHtml + oFilterGlobal.refinerHTMLTemplate.replace(oFilterGlobal.regularRefiner, $AOLObject.TermName);

                $.each($AOLObject.AreaTerms, function (iCount, sTerm) {
                    sAOLHtml = sAOLHtml + oFilterGlobal.refinerHTMLTemplate.replace(oFilterGlobal.regularRefiner, sTerm.TermName).replace("PGValue", $AOLObject.TermName);
                });
            });
        }
        if (sPGHtml) {
            sPGHtml += "</div>" + oFilterGlobal.noRecordRow.replace(oFilterGlobal.regularNoRefinerRow, "") + oFilterGlobal.refinerOkButton.replace(oFilterGlobal.regularRefinerType, "PG") + oFilterGlobal.refinerCancelButton;
        } else {
            sPGHtml = oFilterGlobal.noRecordRow.replace(oFilterGlobal.regularNoRefinerRow, " noRefinersVisible");
        }
        if (sAOLHtml) {
            sAOLHtml += "</div>" + oFilterGlobal.noRecordRow.replace(oFilterGlobal.regularNoRefinerRow, "") + oFilterGlobal.refinerOkButton.replace(oFilterGlobal.regularRefinerType, "AOL") + oFilterGlobal.refinerCancelButton;
        } else {
            sAOLHtml = oFilterGlobal.noRecordRow.replace(oFilterGlobal.regularNoRefinerRow, " noRefinersVisible");
        }
        $(".refinerPG").length && $(".refinerPG").html(sPGHtml);
        $(".refinerAOL").length && $(".refinerAOL").html(sAOLHtml);
        oJQueryBindings.$PracticeGroupText.attr("data-totalcount", $(".filterSearchPG input[type='checkbox']").length);
        oJQueryBindings.$AreaOfLawText.attr("data-totalcount", $(".filterSearchAOL input[type='checkbox']").length);
        oJQueryBindings.$PracticeGroupText.attr("data-selectedcount", 0);
        oJQueryBindings.$AreaOfLawText.attr("data-selectedcount", 0);
    } else {
        showCommonErrorPopUp(oPracticeGroup.code);
    }
};

/* Common failure function */
commonFunction.onRefinerFailure = function (result, sContainer) {
    "use strict";
    $(sContainer).html(result.d);
    showCommonErrorPopUp(result);
};

/* Function to select all the check box */
commonFunction.refinerSelectAll = function (sSelectLink) {
    "use strict";
    $(sSelectLink).parent().parent().children().find("input[type=checkbox]").prop("checked", "checked");
};
/* Function to clear check box selection */
commonFunction.refinerSelectNone = function (sSelectLink) {
    "use strict";
    $(sSelectLink).parent().parent().children().find("input[type=checkbox]").removeAttr("checked");
};
/* Function to execute on click on Cancel */
commonFunction.refinerCancelClick = function (sCancelButton) {
    "use strict";
    var $CancelThis = $(sCancelButton).parent().parent();
    commonFunction.resetToSavedState($CancelThis);
    $CancelThis.hide();
};
/* Function to execute on click of OK */
commonFunction.refinerOkClick = function (sRefinerId, event) {
    "use strict";
    var sSelectedText = "", sSelectedTextVal = "";
    $(sRefinerId + " .filterSearchRefinerText").attr("data-selectedcount", $(sRefinerId + " input:checked").length);
    $(sRefinerId + " input:checked").each(function () {
        if (sSelectedText) {
            sSelectedText = sSelectedText + "$|$" + $(this).val();
            sSelectedTextVal = sSelectedTextVal + ", " + $(this).val();
        } else {
            sSelectedText = $(this).val();
            sSelectedTextVal = $(this).val();
        }
    });
    $(sRefinerId + " .filterSearchRefinerText").attr("data-selected", sSelectedText);
    $(sRefinerId + " .filterSearchRefinerText").val(sSelectedTextVal);
    commonFunction.closeAllFilterExcept("filterSearchAdvance", event);
    /* Code to cascade the Area of Laws */
    if (".filterSearchPG" === sRefinerId) {  //// If OK clicked on Practice Group filter
        if (sSelectedText) {                 //// If at least one practice group is selected
            var $AOLObject = $(".refinerAOL .refinerWrapper");
            $AOLObject.find("input[type='checkbox']").removeAttr("checked"); //// Uncheck all the AOL
            var arrPracticeGroup = sSelectedText.split("$|$");
            $.each(arrPracticeGroup, function (index, val) {    //// Check the AOL for selected practice group
                $AOLObject.find("input[type='checkbox'][data-pg='" + val + "']").prop("checked", "checked");
            });
        } else {
            commonFunction.refinerSelectNone(".refinerAOL .buttonCancel");
        }
        commonFunction.refinerOkClick(".filterSearchAOL", event);
    }
    /* Code to cascade the Area of Laws */
};

/* Function to bind all JQuery elements */
commonFunction.JQueryBindings = function () {
    "use strict";
    oJQueryBindings.$ClientText = $(".refinerClientsText");
    oJQueryBindings.$PracticeGroupText = $(".refinerPGText");
    oJQueryBindings.$AreaOfLawText = $(".refinerAOLText");
    oJQueryBindings.$SearchMatter = $("#searchMatter");
    oJQueryBindings.$SearchDocument = $("#searchDocument");
    oJQueryBindings.$SwitchApp = $(".switchApp");
    oJQueryBindings.$SpecificName = $(".specificName");
    oJQueryBindings.$IsVisible = false;
};

commonFunction.openAdvanceSearch = function (sSelectorThis, $Element, iFlag, event) {
    "use strict";
    var sSelector = sSelectorThis.replace("DD", "")
    , sSelectorString = sSelector.replace(".", "");
    $(sSelector).find(".refinerWrapper").show();
    $(sSelector).find(".refinerRow").show();
    $(sSelector).find(".noRefiners").hide();
    if ($(sSelector).find(".noRefiners").is(":visible")) {
        $(sSelector).find(".noRefiners").css("display", "none");
    }
    if ($(sSelector).find(".refinerRowLink").is(":hidden")) {
        $(sSelector).find(".refinerRowLink").show();
    }
    commonFunction.closeAllFilterExcept(sSelectorString, event);
    $(sSelector).width($Element.width());
    iFlag && commonFunction.resetToSavedState($(sSelector));
    $(sSelector).toggle();
    event.stopPropagation();
};

commonFunction.openSortDropdown = function (sSelectorThis, $Element, iFlag, event) {
    "use strict";
    if ("block" === $Element.css("display")) {
        $Element.addClass("hide");
    } else {
        $Element.removeClass("hide");
    }
};

commonFunction.switchThisApp = function (sAppName) {
    "use strict";
    $(".popupWait, .loadingImage").removeClass("hide");
    $(".loadingImage").css("position", "absolute");
    "use strict";
    $(".swicthApp").toggle();
    var sQueryParameter = commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + sAppName, false);
    switch (sAppName) {
        case oGlobalConstants.App_Name_Search_Matters:
            window.location.href = oGlobalConstants.ServicePath_FindMatter + sQueryParameter;
            break;
        case oGlobalConstants.App_Name_Search_Documents:
            window.location.href = oGlobalConstants.ServicePath_FindDocument + sQueryParameter;
            break;
        case oGlobalConstants.App_Name_Provision_Matters:
            window.location.href = oGlobalConstants.ServicePath_MatterProvision + sQueryParameter;
            break;
        case oGlobalConstants.App_Name_App_Landing_Page:
            window.location.href = oGlobalConstants.ServicePath_AppLandingPage + sQueryParameter;
            break;
    }
};

//// Function to log App Insight events and set query parameter in URL on redirection
commonFunction.AppLogEvent = function (sAppName, bToLog) {
    "use strict";
    var sQueryParameter = "",
        sGroupName = oGlobalConstants.Browser,
        parameterKey = "appType",
        appType = oCommonObject.getParameterByName(parameterKey),
        eventName = "";

    if (null != appType) {
        switch (appType) {
            case oGlobalConstants.Querystring_Office:
                sGroupName = oGlobalConstants.Word;
                sQueryParameter = "?" + parameterKey + "=" + oGlobalConstants.Querystring_Office;
                break;
            case oGlobalConstants.Querystring_OWA:
                sGroupName = oGlobalConstants.OWA;
                sQueryParameter = "?" + parameterKey + "=" + oGlobalConstants.Querystring_OWA;
                break;
            case oGlobalConstants.Querystring_Outlook:
                sQueryParameter = "?" + parameterKey + "=" + oGlobalConstants.Querystring_Outlook;
                sGroupName = oGlobalConstants.Outlook;
                break;
        }
    }
    eventName = sGroupName + "/" + sAppName;
    (bToLog) ? oCommonObject.logEvent(eventName) : "";
    return sQueryParameter;
};

// Function to trim trailing special character if present
function trimEndChar(sOrignalString, sCharToTrim) {
    "use strict";
    if (sOrignalString && sCharToTrim === sOrignalString.substr(-1)) {
        return sOrignalString.substr(0, sOrignalString.length - 1);
    }
    return sOrignalString;
}

function advanceSearchDropdown() {
    "use strict";
    if ($("#filterSearchDown").is(":visible")) {
        $(".filterSearchDD").toggle();
    }
}

commonFunction.onLoadActions = function (isServiceCall) {
    "use strict";
    //// #region JQuery Bindings
    commonFunction.JQueryBindings();
    //// #endregion

    // #region Date Picker
    $("#refinerFromText, .imageDatePickerFrom").datepicker({
        dateFormat: "yy-mm-dd",
        changeMonth: true,
        changeYear: true,
        yearRange: "-70:+70",
        constrainInput: true,
        duration: "",
        gotoCurrent: true,
        maxDate: "+0D",
        onClose: function (selectedDate) {
            $("#refinerToText, .imageDatePickerTo").datepicker("option", "minDate", selectedDate);
        }
    }).datepicker();
    $("#refinerToText, .imageDatePickerTo").datepicker({
        dateFormat: "yy-mm-dd",
        changeMonth: true,
        changeYear: true,
        yearRange: "-70:+10",
        constrainInput: true,
        duration: "",
        gotoCurrent: true,
        maxDate: "+0D",
        onClose: function (selectedDate) {
            $("#refinerFromText, .imageDatePickerFrom").datepicker("option", "maxDate", selectedDate || "-1D");
        }
    }).datepicker();
    $(".ui-datepicker").click(function (event) {
        commonFunction.closeAllFilterExcept("ui-datepicker", event);
        event.stopPropagation();
    });
    //// #endregion

    //// #region Binding
    // #region App Switch
    $(".specificName").click(function (event) {
        oJQueryBindings.$SwitchApp.toggle();
        event.bubbles = false;
        return false;
    });

    // #endregion
    $(".filterSearchDD").click(function (event) {
        // App Insight Event tracking for Advance Search
        var eventName = oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Open_Advance_Search;
        if ($("#filterSearchDown").is(":visible")) {
            eventName = oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Close_Advance_Search;
        }
        commonFunction.AppLogEvent(eventName, true);
        commonFunction.closeAllFilterExcept("filterSearchAdvance", event);
        if ($(".closeFailSavedSearchNotification").is(":visible")) {
            $(".closeFailSavedSearchNotification").parent().remove();
        }
        $(".filterSearchAdvance").toggle();
        $(".quickLinksMenuItems").hide();
        $(".filterSearchDD").toggle();
        $(".sdBannerPanel").addClass("hide");
        event.stopPropagation();
    });
    //// #region Close All Pop up except specified popup
    $(".filterSearchAdvance").click(function (event) {
        commonFunction.closeAllFilterExcept("filterSearchAdvance", event);
    });
    $(".refinerClient").click(function (event) {
        commonFunction.closeAllFilterExcept("refinerClient", event);
    });
    $(".refinerPG").click(function (event) {
        commonFunction.closeAllFilterExcept("refinerPG", event);
    });
    $(".refinerAOL").click(function (event) {
        commonFunction.closeAllFilterExcept("refinerAOL", event);
    });
    oJQueryBindings.$PracticeGroupText.click(function (event) {
        commonFunction.closeAllFilterExcept("refinerPG", event);
    });
    oJQueryBindings.$AreaOfLawText.click(function (event) {
        commonFunction.closeAllFilterExcept("refinerAOL", event);
    });
    $("#filterAdvancedSearch").click(function (event) {
        if ($(".filterSearchAdvance").css("display") === "block") {
            $(".filterSearchDD").toggle();
        }
        commonFunction.removeClasses();
        // Change the default sort setting to relevant
        if ("function" === typeof (updateSortSettings)) {
            oGridConfig.nGridPageNumber = 1;
            updateSortSettings(event);
        }
        advancedSearch("#allMatters", "tileContainer", event);
        commonFunction.closeAllFilterExcept("", event);
        closeAllPopupExcept("", event);
    });
    $(".filterSearchImg").click(function (event) {
        oCommonObject.bHideAutoComplete = true;
        // App Insight Event tracking for Text box search
        var sCurrentEvent = (oGridConfig.isMatterView ? oGlobalConstants.Matter_Textbox_Search : oGlobalConstants.Document_Textbox_Search);
        commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + sCurrentEvent, true);
        if ($(".filterSearchAdvance").css("display") === "block") {
            $(".filterSearchDD").toggle();
        }
        commonFunction.removeClasses();
        // Change the default sort setting to relevant
        if ("function" === typeof (updateSortSettings)) {
            oGridConfig.nGridPageNumber = 1;
            updateSortSettings(event);
        }
        advancedSearch("#allMatters", "tileContainer", event);
        commonFunction.closeAllFilterExcept("", event);
        closeAllPopupExcept("", event);
    });
    oJQueryBindings.$ClientText.click(function (event) {
        commonFunction.closeAllFilterExcept("refinerClient", event);
    });
    $(document).on("input", ".filterSearchRefinerText", function () {
        if (!oJQueryBindings.$ClientText.val()) {
            $(".refinerWrapper").find("input[type=checkbox]").removeAttr("checked");
        }
    });
    $(".filterSearchText").click(function (event) {
        commonFunction.closeAllFilterExcept("filterSearchAdvance", event);
    });
    //// endregion
    $(document).on("keyup", ".filterSearchRefinerText", function (event) {
        $(".refinerWrapper").show();
        $(".buttonOk").removeAttr("disabled");
        if (!oJQueryBindings.$ClientText[0].validity.patternMismatch && !oJQueryBindings.$PracticeGroupText[0].validity.patternMismatch && !oJQueryBindings.$AreaOfLawText[0].validity.patternMismatch) {
            filterKeyUp(event, this);
        } else {
            $(".refinerWrapper").hide();
            $(".refinerRowLink").hide();
            $(".noRefiners").show();
            $(".buttonOk").attr("disabled", "disabled");
        }
        event.stopPropagation();
    });
    // functions for switching app 

    $(document).on("click", ".appSwitch", function (event) {
        oFilterGlobal.isVisible ? $(".appSwitch").removeClass("appSwitchMenushow").find(".appSwitcherName").removeClass("hide") : $(".appSwitch").addClass("appSwitchMenushow").find(".appSwitcherName").addClass("hide");
        oFilterGlobal.isVisible = !(oFilterGlobal.isVisible);
        $(".matterPopup .documentPopup").hide();
        $(".appSwitchMenu").toggleClass("hide");
        event.stopPropagation();
    });

    // End of function for switching app.
    //// #endregion
    //// #region Advance search pop up
    /* Common function change */
    $(".refinerClientDD").click(function (event) {
        commonFunction.openAdvanceSearch(".refinerClientDD", oJQueryBindings.$ClientText, 1, event);
    });
    $(".refinerPGDD").click(function (event) {
        commonFunction.openAdvanceSearch(".refinerPGDD", oJQueryBindings.$PracticeGroupText, 1, event);
    });
    $(".refinerAOLDD").click(function (event) {
        commonFunction.openAdvanceSearch(".refinerAOLDD", oJQueryBindings.$AreaOfLawText, 0, event);
    });

    $(document).on("click", ".switchAppRow, .switchAppLink", function () {
        if ($(".appSwitcherName span").text().toLowerCase() !== $(this).attr("data-attr").toLowerCase()) {
            $(".popupWait, .loadingImage").removeClass("hide");
            $(".loadingImage").css("position", "absolute");
            commonFunction.switchThisApp($(this).attr("data-attr"));
        }
    });

    //// #endregion
    ////#region Service Calls
    if (isServiceCall) {
        /* Call to fetch Clients from Term Store */
        var matterDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oFilterGlobal.siteURL }, "details": { "TermGroup": oGlobalConstants.Common_Term_Set_Group_Name, "TermSetName": oGlobalConstants.Client_Term_Set_Name, "CustomPropertyName": oGlobalConstants.Client_Custom_Properties_Url } };
        oCommonObject.callProvisioningService("GetTaxonomyData", matterDetails, commonFunction.onClientSuccess, commonFunction.onRefinerFailure, commonFunction.onBeforeSend);
        /* Call to fetch Practice Groups and Area of Laws */
        var PGDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oFilterGlobal.siteURL }, "details": { "TermGroup": oGlobalConstants.Common_Term_Set_Group_Name, "TermSetName": oGlobalConstants.Practice_Group_Term_Set_Name, "CustomPropertyName": oGlobalConstants.Practice_Group_Custom_Properties } };
        oCommonObject.callProvisioningService("GetTaxonomyData", PGDetails, commonFunction.onPGSuccess, commonFunction.onRefinerFailure);
    }
    ////#endregion

    // #region events for upload popup and drill down on folders
    var $MailBodyElement = $("#mailBody");
    $MailBodyElement.on("dblclick", ".folderStructureContent ul li ul li", function (e) {
        e.stopPropagation();
        e.stopImmediatePropagation();
        e.preventDefault();
        oUploadGlobal.oDrilldownParameter.nCurrentLevel++;
        commonFunction.drilldownNavigation(e);
        $(".folderStructureContent, .parentNode").addClass("folderStructureWithBreadcrumb");
    });

    $MailBodyElement.on("dragover", ".folderStructureContent ul li", function (e, ui) {
        e.stopImmediatePropagation();
        $(this).addClass("folderDragOver");
    });

    $MailBodyElement.on("dragleave", ".folderStructureContent ul li", function (e, ui) {
        $(this).removeClass("folderDragOver");
    });

    $MailBodyElement.on("click", "#rootLink", function () {
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
        oUploadGlobal.oDrilldownParameter.nCurrentLevel--;
        commonFunction.drilldownNavigation(e);
    });
    // #endregion
    $(document).on("click", ".ErrorPopUpCloseIcon, .ErrorPopupBackground", function () {
        $(".ErrorPopUpHolder").addClass("hide");
    });
    $(document).on("click", ".TryAgainLink", function () {
        $(".popupWait").removeClass("hide").addClass("ErrorPopUpIndex");
    });
    $(".AdminLink").attr("href", oGlobalConstants.Admin_Support_URL);
};

//// Function to redirect user for authentication in case of Token Request Failure exception
function reauthenticateUser() {
    "use strict";
    //// Token Request failed exception occurred, thus do not show error pop up, re-authenticate the user
    var windowLocation = window.location,
        sCurrentWindowUrl = windowLocation.protocol + "//" + windowLocation.host + windowLocation.pathname,
        sQueryParameter = commonFunction.AppLogEvent("", false);
    sCurrentWindowUrl = sCurrentWindowUrl + "?{StandardTokens}" + "?" + oGlobalConstants.TokenRequestFailedQueryString + "=true" + sQueryParameter;
    //// AppRedirect page + Client ID + Redirect URI + Parameters
    window.location.href = oGlobalConstants.App_Redirect_URL + "?client_id=" + oGlobalConstants.ClientID + "&redirect_uri=" + sCurrentWindowUrl + encodeURIComponent(windowLocation.search);
}

// Function to show common Error Pop up
function showCommonErrorPopUp(sErrorPopUpData) {
    "use strict";
    if (("undefined" !== typeof oSearchGlobal && "undefined" !== typeof oSearchGlobal.sClientName && "undefined" !== typeof oSearchGlobal.sClientSiteUrl && "undefined" !== typeof oSearchGlobal.bIsTenantCall) && (("" !== oSearchGlobal.sClientName && "" !== oSearchGlobal.sClientSiteUrl) || oSearchGlobal.bIsTenantCall)) {
        $(".ErrorPopUpContainer").addClass("ErrorPopupSP");
    }
    var iErrorCode = "NA", iTimeStamp = "NA";
    if (sErrorPopUpData && "string" === typeof sErrorPopUpData && (-1 < sErrorPopUpData.indexOf("#|#"))) {
        var arrErrorPopUpData = sErrorPopUpData.split("#|#");
        if (arrErrorPopUpData[0] && arrErrorPopUpData[1]) {
            iErrorCode = arrErrorPopUpData[0];
            iTimeStamp = arrErrorPopUpData[1];
        }
    }
    $("#errorCode").text(iErrorCode);
    $("#errorTimeStamp").text(iTimeStamp);
    if (oGlobalConstants.TokenRequestFailedErrorCode === iErrorCode) {
        reauthenticateUser();
    } else {
        //// Some other exception occurred
        $(".ErrorPopUpHolder").removeClass("hide");
        $(".ErrorMessageText").html(oGlobalConstants.Error_Pop_Up_Message);
        if (window.top === window.self) {
            $(".TryAgainLink").attr("href", "javascript:window.top.location.reload()");
        } else {
            $(".TryAgainLink").attr({ "href": window.location.href, "target": "_self" });
        }
    }
}

// Function is used to remove classes that are applied during save searches
commonFunction.removeClasses = function () {
    "use strict";
    if ("none" !== $(".pinnedSearchesContainer").css("display")) {
        $(".rightContentContainer").removeClass("sSSortDDMinimized sSSortDD");
        $(".PinnedSearchesGridHeaderLabel").removeClass("sSHeaderLabelMinimized");
        $(".sdBanner").removeClass("sSSdBanner");
        $(".sdBannerPanel").removeClass("sSBannerPanel");
    }
    $(".rightContentContainer").addClass("sortDDAllResult");
};

// #region methods for drill down on folders - being used in find matter and web dashboard 
commonFunction.clearGlobalVariables = function () {
    "use strict";
    oUploadGlobal.oDrilldownParameter.nCurrentLevel = 0;
    $("#breadcrumb").addClass("hide");
};

commonFunction.drilldownNavigation = function (e) {
    "use strict";
    var sParentUrl = "undefined" === typeof $(e.target).parent().attr("data-parentname") ? $(e.target).attr("data-parentname") : $(e.target).parent().attr("data-parentname");
    var sCurrentUrl = "undefined" === typeof $(e.target).parent().attr("data-foldername") ? $(e.target).attr("data-foldername") : $(e.target).parent().attr("data-foldername");
    var arrCurrentFolder = [], oParentElement = {}, oCurrentElement = {};

    oUploadGlobal.oDrilldownParameter.sCurrentParentUrl = sCurrentUrl;
    $.each(oSearchGlobal.oFolderName, function (iIndex, child) {
        if ((null !== child.url && child.url === oUploadGlobal.oDrilldownParameter.sCurrentParentUrl) || (null !== child.parenturl && child.parenturl === oUploadGlobal.oDrilldownParameter.sCurrentParentUrl)) {
            arrCurrentFolder.push(child);
        }
        if (null !== child.url && child.url === oUploadGlobal.oDrilldownParameter.sCurrentParentUrl) {
            oCurrentElement = child;
        } else if (sParentUrl === child.url) {
            oParentElement = child;
        }
    });

    if (1 < oUploadGlobal.oDrilldownParameter.nCurrentLevel) {
        $("#parentFolder").html(oParentElement.name).attr("data-foldername", oParentElement.url).attr("data-parentname", oParentElement.parenturl).attr("title", oParentElement.name);
        $("#currentFolder").html(oCurrentElement.name).attr("title", oCurrentElement.name);
        $("#breadcrumb").removeClass("hide");
    } else {
        $("#breadcrumb").addClass("hide");
    }

    if (2 === oUploadGlobal.oDrilldownParameter.nCurrentLevel) {
        $("#parentFolder").hide();
        $("#breadcrumbSeparator").hide();
    } else {
        $("#parentFolder").show();
        $("#breadcrumbSeparator").show();
    }

    var $HtmlChunk = buildNestedList(arrCurrentFolder, sParentUrl);
    if ("undefined" !== typeof $HtmlChunk) {
        $(".folderStructureContent").html($HtmlChunk);
    }

    if ("undefined" !== typeof bindDroppable) {
        bindDroppable($(".folderStructureContent ul li"));
    }
};

// removes the extension from the file name
function extractTitle(fileName) {
    "use strict";
    return (fileName) ? fileName.substring(0, fileName.lastIndexOf(".")) : "";
}

// This function will clear Filter Panel
function clearFilterPanel() {
    "use strict";
    // Clear Search term
    $(".filterSearchText").val("");

    // Clear Clients
    $(".refinerClient .refinerRowLink:last a").click();
    $(".refinerClient .filterOkButton:first > .tileOk").click();

    // Clear PG
    $(".refinerPG .refinerRowLink:last a").click();
    $(".refinerPG .filterOkButton:first > .tileOk").click();

    // Clear AOL
    $(".filterSearchAOL .refinerRowLink:last a").click();
    $(".refinerAOL .filterOkButton:first> .tileOk").click();

    // Clear DOC Author
    $(".filterSearchAdvance .filterSearchAuthor .refinerRight .filterSearchRefinerText").val("");

    // Clear From Date
    $(".filterSearchAdvance .filterSearchDate .refinerRight #refinerFromText").val("");

    // Clear To Date
    $(".filterSearchAdvance .filterSearchDate .refinerRight #refinerToText").val("");

    // Clear global filter object
    if (oGridConfig.isMatterView) {
        oSearchGlobal.oFilterData = { AOLList: "", PGList: "", ClientsList: [], FromDate: "", ToDate: "", DocumentAuthor: "", FilterByMe: oSearchGlobal.searchOption };
    } else {
        oSearchGlobal.oFilterData = { ClientsList: [], FromDate: "", ToDate: "", DocumentAuthor: "", FilterByMe: oSearchGlobal.searchOption };
    }
    oSearchGlobal.sSearchTerm = "";
}

function ViewMatterandGoToOneNote(event, eventName) {
    "use strict";
    // App Insight Event tracking for View Matter and Go To OneNote
    commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + eventName, true);
    event.stopPropagation();
}
// #endregion

// Error handling for page load
$(document).ready(function () {
    "use strict";
    /* Open Contextual help flyout */
    $(document).on("keydown", openContextualHelpFlyout);
    if ("undefined" !== typeof oErrorObject && oErrorObject.isErrorOccured) {
        if ("undefined" !== oErrorObject.errorPopupData.code && "string" === typeof oErrorObject.errorPopupData.code && (-1 < oErrorObject.errorPopupData.code.indexOf("#|#"))) {
            showCommonErrorPopUp(oErrorObject.errorPopupData.code);
        }
    }
    /* Hide the fly outs if clicked outside */
    $(document).on("click", function (event) {
        if (oFilterGlobal.isVisible) {
            $(".appSwitch").removeClass("appSwitchMenushow").find(".appSwitcherName").removeClass("hide");
            oFilterGlobal.isVisible = !(oFilterGlobal.isVisible);
            $(".appSwitchMenu").toggleClass("hide");
            event.stopPropagation();
        }
        var oAppMenuFlyout = $(".AppMenuFlyout");
        if (oAppMenuFlyout.is(":visible")) {
            oAppMenuFlyout.slideUp();
            $(".OpenSwitcher").removeClass("hide");
            $(".MenuCaption").removeClass("hideMenuCaption");
            $(".CloseSwitcher").addClass("hide");
        }
        $(".PersonaFlyout").slideUp();

        var oContextualHelpFlyout = $(".ContextualHelpContainer");
        if (oContextualHelpFlyout.is(":visible")) {
            oContextualHelpFlyout.hide();
        }
    });

    $(".ContextualHelpContainer").click(function (event) {
        event ? event.stopPropagation() : "";
    });
    $(".ContextualHelpLogo").click(function (event) {
        commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Contextual_help, true);
        $(".PersonaFlyout").slideUp();
        showContextualHelpFlyout(event);
        event ? event.stopPropagation() : "";
    });

    /* Hide/Show fly out based on the click on the hamburger icon */
    $(".AppSwitcher").on("click", function (event) {
        oCommonObject.closeAllPopupExcept("", event); //// Close all pop-ups except App Menu flyout
        var oAppMenuFlyout = $(".AppMenuFlyout");
        if (!(oAppMenuFlyout.is(":visible"))) {
            //// Display the close icon and close the fly out
            $(".OpenSwitcher").addClass("hide");
            $(".CloseSwitcher").removeClass("hide");
            $(".MenuCaption").addClass("hideMenuCaption");
            oAppMenuFlyout.slideDown();
        } else {
            $(".CloseSwitcher").addClass("hide");
            $(".OpenSwitcher").removeClass("hide");
            $(".MenuCaption").removeClass("hideMenuCaption");
        }
        event ? event.stopPropagation() : "";
    });
    /*Show/Hide persona flyout*/
    $("#AppHeaderPersona").on("click", function (event) {
        commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Profile_switch, true);
        oCommonObject.closeAllPopupExcept("PersonaFlyout", event); //// Close all pop-ups except Persona Flyout
        var oPersonaFlyout = $(".PersonaFlyout");
        if (oPersonaFlyout.is(":visible")) {
            oPersonaFlyout.slideUp();
        } else {
            oPersonaFlyout.slideDown();
        }
        event ? event.stopPropagation() : "";
    });
    /* Update the links from the resource files */
    $(".MatterSitesLink").on("click", function () {
        commonFunction.switchThisApp(oGlobalConstants.App_Name_Search_Matters);
    });
    $(".UploadLink").on("click", function () {
        commonFunction.switchThisApp(oGlobalConstants.App_Name_Search_Matters);
    });
    $(".MatterDocumentsLink").on("click", function () {
        commonFunction.switchThisApp(oGlobalConstants.App_Name_Search_Documents);
    });
    $(".DelveLink").on("click", function () {
        if ($("#autoCompleteText").val().trim()) {
            $(".DelveLink").attr("href", oGlobalConstants.Delve_Link + encodeURIComponent($("#autoCompleteText").val().trim()));
        } else if (oGlobalConstants.Delve_Link) {
            $(".DelveLink").attr("href", oGlobalConstants.Delve_Link.split("?")[0]);
        }
        commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oGlobalConstants.App_Name_Search_Documents + oGlobalConstants.Delve, true);
    });
    $(".HomeLink").on("click", function () {
        commonFunction.switchThisApp(oGlobalConstants.App_Name_App_Landing_Page);
    });
    $(document).on("click", ".ErrorPopUpCloseIcon, .ErrorPopupBackground", function () {
        $(".ErrorPopUpHolder").addClass("hide");
    });

    $(".MatterCenterSupportLink").on("click", function (event) {
        commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Matter_Center_Support, true);
    });
});

// Function to replace to default error image.
function errorImage(image) {
    "use strict";
    if ($(image).attr("data-error")) {
        $(image).attr("src", oGlobalConstants.Image_General_Document).attr("data-error", "true");
    } else {
        $(image).attr("src", $(image).attr("src").substring(0, $(image).attr("src").lastIndexOf(".") + 1) + "png").attr("data-error", "true");
    }
}

// Sets the object with list of OWA document extensions and Visio extensions
commonFunction.setOWADocumentExtension = function () {
    "use strict";
    var arrOWAExtension, arrVisioExtension;
    if (oGlobalConstants.OWA_Document_Extensions) {
        arrOWAExtension = oGlobalConstants.OWA_Document_Extensions.split(",");
        if (arrOWAExtension.length) {
            oFilterGlobal.arrOWADocumentExtension = commonFunction.trimArrayElements(arrOWAExtension);
        }
    }
    if (oGlobalConstants.OWA_Visio_Extensions) {
        arrVisioExtension = oGlobalConstants.OWA_Visio_Extensions.split(",");
        if (arrVisioExtension.length) {
            oFilterGlobal.arrVisioFileExtension = commonFunction.trimArrayElements(arrVisioExtension);
        }
    }
};

// Creates the OWA URL for document if supported by OWA. Else, returns the document path
commonFunction.getOWAUrl = function (documentExtension, documentSPWebUrl, documentPath) {
    "use strict";
    var sOWADocumentUrl, anchorElement = document.createElement("a"), sDocumentPath = encodeURIComponent(documentPath);
    anchorElement.href = sDocumentPath;
    if (-1 < $.inArray(documentExtension, oFilterGlobal.arrVisioFileExtension)) {
        sOWADocumentUrl = documentSPWebUrl + "/" + oGlobalConstants.VisioWebAccessUrl.replace("{0}", sDocumentPath.replace(anchorElement.protocol + "//" + anchorElement.hostname, ""));
    } else {
        sOWADocumentUrl = documentSPWebUrl + "/" + oGlobalConstants.OWA_WOPIFrameUrl.replace("{0}", sDocumentPath.replace(anchorElement.protocol + "//" + anchorElement.hostname, ""));
    }
    return sOWADocumentUrl;
};

// Trims the elements of array
commonFunction.trimArrayElements = function (arrElements) {
    "use strict";
    var arrTrimmedElements = [];
    arrTrimmedElements = $.map(arrElements, function (item) {
        return $.trim(item);
    });
    return arrTrimmedElements;
};

$(".closeContextualHelpFlyout").click(function () {
    "use strict";
    $(".ContextualHelpContainer").hide();
});

function getContextualHelpData(iCurrentPage) {
    "use strict";
    var sParameters = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oGlobalConstants.Central_Repository_Url }, "selectedPage": iCurrentPage }, oParam = 5;
    oCommonObject.callSearchService("FetchContextualHelpContent", sParameters, onFetchContextualHelpContentSuccess, onFetchContextualHelpContentFailure, beforeFetchingContextualHelpContent, oParam);
}

function showContextualHelpFlyout(event) {
    "use strict";
    oCommonObject.closeAllPopupExcept("ContextualHelpContainer", event); //// Close all pop-ups except Contextual Menu
    $(".ContextualHelpContainer").show();
}

function beforeFetchingContextualHelpContent() {
    "use strict";
    $(".contextualHelpSections").html("<div class=\"contextualHelpLoadingImageContainer\"><img src=\"/Images/WindowsLoadingFast.GIF\" title=\"Loading\" class=\"contextualHelpLoadingImage\" alt=\"Loading...\" /><div class=\"contextualHelpLoadingText\" >" + oGlobalConstants.Load_Contextual_Help_Context + "</span></div>");
}

function onFetchContextualHelpContentSuccess(dataPassed) {
    "use strict";
    var sHtmlSectionContainer = "";
    if (dataPassed.Result) {
        var result = JSON.parse(dataPassed.Result),
            index = 0,
            sortedContent = {},
            sHtmlSectionLinks = "", selectedSection = "", sHtmlSectionHeader = "", currLinkTitle = "", currLinkUrl = "", currSectionTitle = "", numberOfColumns = "";
        for (index = 0; index < result.length; index++) {
            currSectionTitle = result[index] && result[index].hasOwnProperty("ContextSection") && result[index].ContextSection.hasOwnProperty("SectionTitle") ? result[index].ContextSection.SectionTitle.trim() : "NA";
            numberOfColumns = result[index] && result[index].hasOwnProperty("ContextSection") && result[index].ContextSection.hasOwnProperty("NumberOfColumns") ? result[index].ContextSection.NumberOfColumns.trim() : "1";
            currLinkTitle = result[index].hasOwnProperty("LinkTitle") ? result[index].LinkTitle.trim() : "NA";
            currLinkUrl = result[index].hasOwnProperty("LinkURL") ? result[index].LinkURL.trim() : "NA";

            if (selectedSection.toLocaleLowerCase() !== currSectionTitle.toLocaleLowerCase()) {
                // this is a new section
                selectedSection = currSectionTitle;
                if ("" !== sHtmlSectionHeader && "" !== typeof sHtmlSectionLinks) {
                    sHtmlSectionContainer = sHtmlSectionContainer + "<div class='ContextualHelpSubSection' id='popularSearchesSection'>" + sHtmlSectionHeader + sHtmlSectionLinks + "</div>";
                }
                sHtmlSectionLinks = "", sHtmlSectionHeader = "";
                sHtmlSectionHeader = " <div class='ContextualHelpSectionHeader' title='" + currSectionTitle + "' id='" + currSectionTitle + "'>" + currSectionTitle + "</div>";
            }
            var displayNumberOfColumns = "1" === numberOfColumns ? "displaySingleColumn" : "displayDoubleColumns";
            sHtmlSectionLinks = sHtmlSectionLinks + "<div class='ContextualHelpSectionData " + displayNumberOfColumns + "' title='" + currLinkTitle + "'><a href='" + currLinkUrl + "'  target='_blank'>" + currLinkTitle + "</a></div>";
        }
        if ("" !== sHtmlSectionHeader && "" !== typeof sHtmlSectionLinks) {
            sHtmlSectionContainer = sHtmlSectionContainer + "<div class='ContextualHelpSubSection' id='popularSearchesSection'>" + sHtmlSectionHeader + sHtmlSectionLinks + "</div>";
        } else {
            sHtmlSectionContainer = oGlobalConstants.No_Help_Links_Message;
        }
    } else {
        sHtmlSectionContainer = oGlobalConstants.No_Help_Links_Message;
    }
    $(".contextualHelpSections").html(sHtmlSectionContainer);
}

function onFetchContextualHelpContentFailure(dataFailed) {
    "use strict";
    var sResult = dataFailed.Result;
    $(".contextualHelpSections").html(sResult);
}

function openContextualHelpFlyout(e) {
    "use strict";
    // if key pressed is F1 then display the popup
    if (112 === e.keyCode) {
        $(".ContextualHelpLogo").click();
        e.preventDefault();
        e.stopPropagation();
    }
};

$(window).on("resize", function () {
    "use strict";
    $(".ContextualHelpContainer").hide();
    $(".PersonaFlyout").hide();
});

$("#openHamburger, #closeHamburger").click(function () {
    "use strict";
    var oElement = $("#headerFlyout");
    if (oElement.length) {
        if (oElement.is(":visible")) {
            oElement.slideUp();
            $(".matterCenterHeaderBackground").addClass("hide");
            $("#closeHamburger").addClass("hide");
            $("#openHamburger").removeClass("hide");
            $("#matterCenterHomeText").removeClass("buffer");
        } else {
            oElement.slideDown();
            $(".matterCenterHeaderBackground").removeClass("hide");
            $("#closeHamburger").removeClass("hide");
            $("#matterCenterHomeText").addClass("buffer");
        }
    }
});

$("#matterCenterHomeText").click(function () {
    "use strict";
    window.open(oGlobalConstants.Site_Url + oGlobalConstants.TenantWebDashboard, "_parent");
});

$(".matterCenterHeaderBackground").click(function () {
    "use strict";
    closeHamburgerMenu();
});

function closeHamburgerMenu() {
    "use strict";
    $("#openHamburger, #closeHamburger").click();
}

$("#searchLink").on("click", function (event) {
    "use strict";
    commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Enterprise_Search, true);
    var sSearchText = $("#searchText").val().trim();
    var sSearchPage = "";
    if ("undefined" !== typeof oWebDashboardConstants) {
        sSearchPage = oWebDashboardConstants.Search_Page_URL;
    }
    if ("undefined" !== typeof oSettingsConstants) {
        sSearchPage = oSettingsConstants.Search_Page_URL;
    }
    var sDelimiter = "$|$";
    if (sSearchText) {
        var sSearchPageUrl = oGlobalConstants.Site_Url + sSearchPage.replace("{0}", encodeURIComponent(sSearchText));
        var message = "enterprise_search" + sDelimiter + sSearchPageUrl;
        window.top.parent.location.href = sSearchPageUrl;
    }
});

/* Register the keypress event for search text box */
$("#searchText").on("keypress", function (event) {
    "use strict";
    if (event.which === 13) {
        $("#searchLink").click();
    }
});

function delveIt() {
    "use strict";
    if ($("#autoCompleteText").val().trim()) {
        window.open(oGlobalConstants.Delve_Link + encodeURIComponent($("#autoCompleteText").val().trim()));
    } else if (oGlobalConstants.Delve_Link) {
        window.open(oGlobalConstants.Delve_Link.split("?")[0]);
    }
    commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oGlobalConstants.App_Name_Search_Documents + oGlobalConstants.Delve, true);
}