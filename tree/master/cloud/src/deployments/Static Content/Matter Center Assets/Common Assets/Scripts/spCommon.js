/// <disable>JS1003,JS2005,JS2016,JS2024,JS2026,JS2027,JS2031,JS2032,JS2052,JS2053,JS2064,JS2073,JS2074,JS2076,JS3054,JS3056,JS3057,JS3058,JS3085,JS3092,JS3116</disable>
//// Above exclusion list is signed off as per the Modern Cop Exclusion list.

/* Common chunk for header and footer section */
var oCommonHtmlChunk = {
    "headerSectionHtml": "<div class=\"popupBackground hide\"></div><div id=\"menu\"><img src=\"@@HamburgerIcon\" title=\"View links\" /></div><div class=\"searchZone\"><a id=\"mcIcon\" href = \"@@DashboardLink\" title = \"Matter Center Home\" target=\"_self\"><img src='@@MCIcon' title='Matter Center' alt= 'Matter Center'/></a><div class=\"iconText\"><div class=\"mainText\"></div><div class=\"subText\"></div></div><div class=\"userIcon\"><img src=\"@@TeamMembersIcon\" title=\"Team Members\" /></div><div class=\"searchBox\"><input id=\"searchText\" type=\"text\" placeholder=\"Search SharePoint\"/><img class=\"searchIcon\" src=\"@@SearchIcon\" alt=\"\"/></div></div><div class=\"closeIcon hide\"><img src=\"@@CloseIcon\" title=\"Close\" /></div><div class=\"menuFlyout\"><div class=\"menuFlyoutColumn\"><a id=\"matterLink\" href=\"@@MattersLink\"class=\"flyoutColumnData\" target=\"_self\" title = \"Matters\"> Matters</a><a id=\"documentLink\" href=\"@@DocumentsLink\"class=\"flyoutColumnData\" target=\"_self\" title = \"Documents\"> Documents</a><a id=\"settingsLink\" href=\"@@SettingsLink\"class=\"flyoutColumnData\" target=\"_self\" title = \"Settings\">Settings</a></div></div><div class=\"selectedUserIcon hide\"><img src=\"@@TeamMembersIcon\" title=\"Team Members\" /></div><div class=\"teamFlyout hide\"><div class=\"teamFlyoutHeader\"><div class=\"teamSection selectedSection\">TEAM<span class=\"userNumber\">(0)</span> </div><div class=\"blockedUserSection\">CONFLICTED<span class=\"userNumber\">(0)</span> </div></div><div class=\"teamFlyoutData\"><img class=\"loadingIcon\" src=\"@@LoadingImage\" alt=\"Loading\"/></div><div class=\"blockedFlyoutData hide\"><img class=\"loadingIcon\" src=\"@@LoadingImage\" alt=\"Loading\"/></div></div>",
    "headerSectionHtmlBackwardCompatible": "<div class=\"popupBackground hide\"></div><div id=\"menu\"><img src=\"@@HamburgerIcon\" title=\"View links\" /></div><div class=\"searchZone\"><a id=\"mcIcon\" href = \"@@DashboardLink\" title = \"Project Center Home\" target=\"_self\"><img src='@@MCIcon' title='Project Center' alt= 'Project Center'/></a><div class=\"iconText\"><div class=\"mainText\"></div><div class=\"subText\"></div></div><div class=\"userIcon\"><img src=\"@@TeamMembersIcon\" title=\"Team Members\" /></div><div class=\"searchBox\"><input id=\"searchText\" type=\"text\" placeholder=\"Search SharePoint\"/><img class=\"searchIcon\" src=\"@@SearchIcon\" alt=\"\"/></div></div><div class=\"closeIcon hide\"><img src=\"@@CloseIcon\" title=\"Close\" /></div><div class=\"menuFlyout\"><div class=\"menuFlyoutColumn\"><a id=\"matterLink\" href=\"@@MattersLink\"class=\"flyoutColumnData\" target=\"_self\" title = \"Matters\"> Matters</a><a id=\"documentLink\" href=\"@@DocumentsLink\"class=\"flyoutColumnData\" target=\"_self\" title = \"Documents\"> Documents</a><a id=\"settingsLink\" href=\"@@SettingsLink\"class=\"flyoutColumnData\" target=\"_self\" title = \"Settings\">Settings</a></div></div><div class=\"selectedUserIcon hide\"><img src=\"@@TeamMembersIcon\" title=\"Team Members\" /></div><div class=\"teamFlyout hide\"><div class=\"teamFlyoutHeader\"><div class=\"teamSection selectedSection\">TEAM<span class=\"userNumber\">(0)</span> </div><div class=\"blockedUserSection\">CONFLICTED<span class=\"userNumber\">(0)</span> </div></div><div class=\"teamFlyoutData\"><img class=\"loadingIcon\" src=\"@@LoadingImage\" alt=\"Loading\"/></div><div class=\"blockedFlyoutData hide\"><img class=\"loadingIcon\" src=\"@@LoadingImage\" alt=\"Loading\"/></div></div>",
    "footerSectionHtml": "<div id=\"footerContainer\"><div class=\"footerLogo\"><a target=\"_self\" href=\"@@FooterLink\" id=\"footerLogoLink\"><img src=\"@@MicrosoftLogo\" alt=\"Microsoft\" title =\"Microsoft\"/></a></div><section class=\"footerLink\"><a id=\"feedbackSupport\" href=\"@@Feedback\" target =\"_self\" class=\"linkText\" title=\"Feedback & Support\">Feedback & Support</a><a id=\"privacyLink\" href=\"@@Privacy\" target =\"_self\" class=\"linkText\" title=\"Privacy & Cookies\">Privacy & Cookies</a> <a id=\"termsOfUse\" href=\"@@Terms\" class=\"linkText\" target =\"_self\"title=\"Terms of Use\">Terms of Use</a><span> &copy; @@Year Microsoft</span></section></div>"
};

/* Footer links for the page */
var oFooterLinks = {
    "feedback": "[Enter URL for Support, e.g. mailto:support@supportsite.com]",
    "termsOfUse": "[Enter URL for Terms of use, e.g. termofuse.supportsite.com]",
    "privacy": "[Enter URL for Privacy terms, e.g. privacy.supportsite.com]",
    "searchPageURL": "/search/Pages/results.aspx?k=@@searchText",
    "dashboardDocumentsQueryString": "?section=2",
    "matterDocumentsQueryString": "?section=1",
    "dashboard": "[[[Tenant Web Dashboard Url]]]",
    "settingsPage": "[[[Tenant Settings Url]]]",
    "logoLink": "[[[Logo link]]]"
},

/* Common objects*/
oCommonLinks = {
    "AzureSiteUrl": "[[[Azure Site Url]]]",
    "sCatalogSite": "[[[Catalog site relative Url]]]/",
    "applicationInsightsId": "[[[App Insights ID]]]",
    "oMatterCenterAssetsLocation": "[[[Common Assets Url]]]",
};

/* Assets location */
var oCommonAssets = {
    searchIcon: "images/searchIcon.png",
    hamburgerIcon: "images/HamburgerIcon.png",
    microsoftLogo: "images/microsoftLogo.png",
    userListIcon: "images/userList.png",
    closeIcon: "images/closeIcon.png",
    matterCenterIcon: "images/mcIcon.png",
    loadingImage: "images/WindowsLoadingFast.gif"
};

var oGlobalConstants = {
    sOperationUnpin: "Unpin",
    sOperationPin: "Pin",
    sUserAliasColumn: "UserAlias",
    sOperationType: "",
    currentUser: null,
    sDateFormat: "MMMM dd, yyyy",
    sEventName: "",
    clientContext: null,
    sNotApplicable: "NA", 
    sCurrentUserEmail:"",
    sEmailName : "EmailSupport.eml"
};

/* Attaches all the content on the page */
function displayHeaderAndFooterContent(isBackwardCompatible) {
    "use strict";
    var sAssetsLocation = "../"+ oCommonLinks.oMatterCenterAssetsLocation;
    /* Set the header html text */
    var sHeaderText = '';
    if(isBackwardCompatible){
    	sHeaderText = oCommonHtmlChunk.headerSectionHtmlBackwardCompatible.replace("@@SearchIcon", sAssetsLocation + oCommonAssets.searchIcon).replace(/@@LoadingImage/g, sAssetsLocation + oCommonAssets.loadingImage).replace("@@HamburgerIcon", sAssetsLocation + oCommonAssets.hamburgerIcon).replace("@@CloseIcon", sAssetsLocation + oCommonAssets.closeIcon).replace(/@@TeamMembersIcon/g, sAssetsLocation + oCommonAssets.userListIcon).replace("@@MCIcon", sAssetsLocation + oCommonAssets.matterCenterIcon).replace("@@SettingsLink", oFooterLinks.settingsPage).replace("@@DashboardLink", oFooterLinks.dashboard).replace("@@DocumentsLink", oFooterLinks.dashboard + oFooterLinks.dashboardDocumentsQueryString).replace("@@MattersLink", oFooterLinks.dashboard + oFooterLinks.matterDocumentsQueryString);
    }
    else{    
    	sHeaderText = oCommonHtmlChunk.headerSectionHtml.replace("@@SearchIcon", sAssetsLocation + oCommonAssets.searchIcon).replace(/@@LoadingImage/g, sAssetsLocation + oCommonAssets.loadingImage).replace("@@HamburgerIcon", sAssetsLocation + oCommonAssets.hamburgerIcon).replace("@@CloseIcon", sAssetsLocation + oCommonAssets.closeIcon).replace(/@@TeamMembersIcon/g, sAssetsLocation + oCommonAssets.userListIcon).replace("@@MCIcon", sAssetsLocation + oCommonAssets.matterCenterIcon).replace("@@SettingsLink", oFooterLinks.settingsPage).replace("@@DashboardLink", oFooterLinks.dashboard).replace("@@DocumentsLink", oFooterLinks.dashboard + oFooterLinks.dashboardDocumentsQueryString).replace("@@MattersLink", oFooterLinks.dashboard + oFooterLinks.matterDocumentsQueryString);
 	}
	$("#matterCenterHeader").html(sHeaderText);

    /* Set the rss panel html text */
    var sFooterText = oCommonHtmlChunk.footerSectionHtml.replace("@@Year", (new Date).getFullYear());
    sFooterText = sFooterText.replace("@@MicrosoftLogo", sAssetsLocation + oCommonAssets.microsoftLogo).replace("@@Feedback", oFooterLinks.feedback).replace("@@Terms", oFooterLinks.termsOfUse).replace("@@Privacy", oFooterLinks.privacy).replace("@@FooterLink", oFooterLinks.logoLink);
    $("#matterCenterFooter").html(sFooterText);

    applyBindings();
}

/* Apply event bindings once the data is appended on the page */
function applyBindings() {
    "use strict";
    /* Register the click event for hamburger menu items */
    $("#menu").click(function (event) {
        if (!$(".menuFlyout").is(":visible")) {
            $(".closeIcon").removeClass("hide");
            $(".popupBackground").removeClass("hide");
            $(".menuFlyout").slideDown();
        }
    });

    /* Register the click event for close icon on menu items */
    $(".closeIcon").click(function () {
        if ($(".menuFlyout").is(":visible")) {
            $(this).addClass("hide");
            $(".menuFlyout").slideUp();
            $(".popupBackground").addClass("hide");
        }
    });

    /* Register the click event for search icon */
    $(".searchIcon").on("click", function (event) {
        var sSearchText = $("#searchText").val().trim();
        if (sSearchText && "" !== sSearchText) {
            var sSearchPageUrl = oFooterLinks.searchPageURL.replace("@@searchText", sSearchText);
            window.location = sSearchPageUrl;
        }
    });

    /* Register the keypress event for search textbbox */
    $("#searchText").on("keypress", function (event) {
        if (event.which === 13) {
            $(".searchIcon").click();
            event && event.preventDefault();
        }
    });
}

/* Get the title of the current user */
function getCurrentUserTitle() {
    "use strict";
    if (!oGlobalConstants.sUserLoginName || !oGlobalConstants.sCurrentUserTitle) {
        var clientContext = new SP.ClientContext.get_current();
        var web = clientContext.get_web();
        oGlobalConstants.currentUser = web.get_currentUser();
        clientContext.load(oGlobalConstants.currentUser);
        clientContext.executeQueryAsync(Function.createDelegate(this, function () {
            oGlobalConstants.sCurrentUserTitle = oGlobalConstants.currentUser.get_title();
            oGlobalConstants.sCurrentUserEmail = oGlobalConstants.currentUser.get_email();
            var sAlias = oGlobalConstants.currentUser.get_email();
            if (sAlias) {
                oGlobalConstants.sUserLoginName = getUserLoginName(sAlias);
            }
            else {
                oGlobalConstants.sUserLoginName = getUserLoginName(oGlobalConstants.currentUser.get_loginName());
            }
        }),
        Function.createDelegate(this, function () {
            oGlobalConstants.sCurrentUserTitle = "";
            oGlobalConstants.sUserLoginName = "";
        }));
    }
}


function EscapeSingleQuotes(sProjectName) {
    "use strict";
    return sProjectName.replace(/'/g, "''");
}


/* Extract the login name of the user */
function getUserLoginName(sEmailId) {
    "use strict";
    var sUserPinName = "";
    if (sEmailId) {
        if (-1 === sEmailId.lastIndexOf("@")) { //// user is on premise
            sUserPinName = sEmailId.substring(sEmailId.lastIndexOf("\\") + 1, sEmailId.length);
        }
        else {
            sUserPinName = sEmailId.substring(sEmailId.lastIndexOf("|") + 1, sEmailId.lastIndexOf("@"));
        }
    }
    return sUserPinName;
}

// Initialize Application Insights object
function initializeApplicationInsights() {
    "use strict";
    /// <disable>JS3047,JS2017</disable>
    var appInsights = window.appInsights || function (config) {
        function r(config) { t[config] = function () { var i = arguments; t.queue.push(function () { t[config].apply(t, i) }) } } var t = { config: config }, u = document, e = window, o = "script", s = u.createElement(o), i, f; for (s.src = config.url || "//az416426.vo.msecnd.net/scripts/a/ai.0.js", u.getElementsByTagName(o)[0].parentNode.appendChild(s), t.cookie = u.cookie, t.queue = [], i = ["Event", "Exception", "Metric", "PageView", "Trace"]; i.length;) r("track" + i.pop()); return r("setAuthenticatedUserContext"), r("clearAuthenticatedUserContext"), config.disableExceptionTracking || (i = "onerror", r("_" + i), f = e[i], e[i] = function (config, r, u, e, o) { var s = f && f(config, r, u, e, o); return s !== !0 && t["_" + i](config, r, u, e, o), s }), t
    }({
        instrumentationKey: oCommonLinks.applicationInsightsId
    });
    window.appInsights = appInsights;
    appInsights.trackPageView();
    return appInsights;
}