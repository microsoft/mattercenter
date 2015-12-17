/// <disable>JS1003,JS2005,JS2023,JS2024,JS2027,JS2031,JS2032,JS2052,JS2053,JS2064,JS2073,JS2074,JS2076,JS2085,JS3054,JS3056,JS3057,JS3058,JS3085,JS3092,JS3116</disable>
//// Above exclusion list is signed off as per the Modern Cop Exclusion list.

var oHomeObject = (function () {
    "use strict";

    // #region Common Objects
    var oJQueryObjects = {
        $UploadAttachments: "",
        $MattersContainer: "",
        $DocumentsContainer: "",
        $CreateMatter: "",
        $MatterCenterSupportLink: "",
        $LearnMoreLink: "",
        $ProfileSwitcher: "",
        $ContextualHelp: "",
        $Home: ""
    },
    commonFunctions = {};
    // #endregion

    /* Function to bind the click events on the Primary and Secondary Link Containers */
    commonFunctions.SwitchApp = function (oCurrentElement, sCurrentSelector, sAppNameToSwitch) {
        oCurrentElement.find(sCurrentSelector).on("click", function () {
            commonFunction.switchThisApp(sAppNameToSwitch);
        });
    };

    /* Function to bind the click events for App Insight Events tracking */
    commonFunctions.AppInsightEventsLogging = function (oCurrentElement, sEventToLog, isNoServiceCall) {
        oCurrentElement.on("click", function () {
            commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + sEventToLog, isNoServiceCall);
        });
    };

    $(document).ready(function () {

        /////* Set current page to 0 */
        oCommonObject.sCurrentPage = oGlobalConstants.Home_page;
        commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + "/" + oGlobalConstants.Home_page, true);
        /* Get Contextual Help Data */
        getContextualHelpData(0);

        /* Display the header */
        $(".AppHeader").removeClass("hide");

        // #region JQuery Bindings    
        oJQueryObjects.$UploadAttachments = $(".UploadAttachments");
        oJQueryObjects.$MattersContainer = $(".MattersContainer");
        oJQueryObjects.$DocumentsContainer = $(".DocumentsContainer");
        oJQueryObjects.$CreateMatter = $(".CreateMatter");
        oJQueryObjects.$MatterCenterSupportLink = $(".MatterCenterSupportLink");
        oJQueryObjects.$LearnMoreLink = $(".LearnMoreLink");
        oJQueryObjects.$ProfileSwitcher = $("#ProfileSwitcher");
        oJQueryObjects.$ContextualHelp = $("#ContextualHelp");
        oJQueryObjects.$Home = $(".Home");
        // #endregion

        /* Update the links from the resource files */
        oJQueryObjects.$LearnMoreLink.attr({ "href": oGlobalConstants.Learn_More_Link, "target": "_blank" });
        oJQueryObjects.$MatterCenterSupportLink.attr("href", "mailto:" + oGlobalConstants.Matter_Center_Support_Email);
        $(".MatterDashboardLink").attr({ "href": oGlobalConstants.Site_Url + oGlobalConstants.TenantWebDashboard, "target": "_blank" });

        /* Move the page specific content in the header section */
        var $HeaderPlaceHolder = $("#HeaderPlaceHolderContent");
        $("#HeaderPlaceHolder").html($HeaderPlaceHolder.html());
        $(".AppLogoContainer").removeClass("hide");
        $HeaderPlaceHolder.html("");

        /* Remove the information panel on click of dismiss link */
        $(".DismissLink").on("click", function () {
            $(".WelcomeBar").empty().addClass("RemoveWelcomeBar");
        });

        // #region Bind the click events on the Primary and Secondary Link Containers
        commonFunctions.SwitchApp(oJQueryObjects.$MattersContainer, ".FigureCaption, img", oGlobalConstants.App_Name_Search_Matters);
        commonFunctions.SwitchApp(oJQueryObjects.$DocumentsContainer, ".FigureCaption, img", oGlobalConstants.App_Name_Search_Documents);
        commonFunctions.SwitchApp(oJQueryObjects.$UploadAttachments, ".SecondaryLinksImg, .SecondaryLinksText", oGlobalConstants.App_Name_Search_Matters);
        commonFunctions.SwitchApp(oJQueryObjects.$CreateMatter, ".SecondaryLinksImg, .SecondaryLinksText", oGlobalConstants.App_Name_Provision_Matters);
        // #endregion

        // #region Bind the click events for App Insight Events tracking
        commonFunctions.AppInsightEventsLogging(oJQueryObjects.$LearnMoreLink, oGlobalConstants.Learn_More_link_Name, true);
        commonFunctions.AppInsightEventsLogging(oJQueryObjects.$Home, "/" + oGlobalConstants.Home_page, true);
        commonFunctions.AppInsightEventsLogging(oJQueryObjects.$UploadAttachments, oGlobalConstants.Upload_Attachments, true);
        // #endregion

        $(".AppLogoContainer").on("click", function () {
            // App Insight Event tracking for Web dashboard
            commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oGlobalConstants.App_Name_Web_Dashboard, true);
            commonFunction.switchThisApp(oGlobalConstants.App_Name_App_Landing_Page);
        });
    });
})();

/// Office.js Initialization 
(function () {
    "use strict";
    // The Office initialize function must be run each time a new page is loaded 
    if (oCommonObject.getParameterByName("appType") === oGlobalConstants.Querystring_Outlook) {
        Office.initialize = function (reason) {
        };
    }
})();