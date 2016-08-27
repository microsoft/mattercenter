/// <disable>JS1003,JS2005,JS2024,JS2027,JS2031,JS2032,JS2052,JS2053,JS2064,JS2073,JS2074,JS2076,JS2085,JS3054,JS3057,JS3085,JS3116,JS3056,JS3058,JS3092</disable>
/* Matter Center constants */


var oMatterLandingHtmlConstants = {
    "matterInfoHtml": "<div class='documentLoadingIcon hide'><img class=\"loadingIcon\" src=\"@@LoadingImage\" alt=\"Loading\"/></div><div class=\"errorPopUpHolder hide\"><div class=\"errorPopupBackground\"></div><div class=\"errorPopUpContainer errorPopUpCenter\"><img title=\"Close\" class=\"errorPopUpCloseIcon popUpFloatRight\" alt=\"Close\" src=\"@@PopupCloseIcon\"><div class=\"errorPopUpMessage\"><span id=\"genericMessage\">Something went wrong</span><div class=\"clear\"></div><div id=\"expandCollapse\"><span class=\"inlineElement\" id=\"expandMessage\">+</span><span id=\"collapseMessage\" class=\"inlineElement hide\">-</span><span>Click here for details</span></div><div class=\"clear\"></div><span id=\"errorMessage\" class=\"hide ellipsis\"></span></div></div></div><div title='@@MatterName' class=\"matterName\">@@MatterName</div><div class=\"matterLink\"><a class=\"matterLinkPart\" title='My Matters' href=\"@@FindMatterUrl\" target=\"_self\"> @@MyMatters </a> > <a class=\"matterLinkPart\" title='@@MatterName' href=\"@@MatterUrl\" target=\"_self\"> @@MatterName </a> </div><div class=\"matterAction\"><div class=\"matterView\"><div class=\"matterProfileTitle\">@@Label1Tab1HeaderText</div><div class=\"matterDescriptionTitle changeSection\">@@Label2Tab2HeaderText</div></div>@@PinChunk</div><div class=\"clear\"></div><div class=\"matterDescriptionBody hide\"><div><img class=\"loadingIcon\" alt=\"Loading Image\" src=\"@@LoadingImage\" /></div></div><div class=\"matterProfileBody\"><div class=\"matterDetails\"><img class=\"loadingIcon\" alt=\"Loading Image\" src=\"@@LoadingImage\" /></div><div class=\"clear\"></div></div>",
    "taskSectionHtml": "<div class=\"taskHeading\"> <div class=\"headingText\" title='Task'> Task </div> <div class=\"taskOption\"> <a href=\"@@TaskLink\" onclick=\"LogEvent(appInsightsMatterLandingText + appInsightsOperations.Task)\" title= \"View / Edit\" target =\"_self\">View / Edit </a></div> </div> <div class=\"taskBoard\"><img class=\"loadingIcon\" src=\"@@LoadingImage\" alt=\"Loading\"/></div>",
    "calendarSectionHtml": "<div class=\"calenderHeading\"> <div title='Calendar' class=\"headingText\"> Calendar Events </div><div class=\"calenderOption\"><a href=\"@@TaskLink\" title= \"View / Edit\" target =\"_self\" onclick=\"LogEvent(appInsightsMatterLandingText + appInsightsOperations.Calendar)\" >View / Edit </a></div></div><div class=\"eventBoard\"><img class=\"loadingIcon\" src=\"@@LoadingImage\" alt=\"Loading\"/></div>",
    "rssSectionHtml": "<div class=\"taskHeading\"><div title='Related News (RSS)' class=\"headingText\"> Related News (RSS) </div></div><div class=\"clear\"></div>",
    "taskItemHtml": "<div class='taskTitle'><a href='@@TaskUrl' onclick=\"LogEvent(appInsightsMatterLandingText + appInsightsOperations.TaskItem)\" target='_self'></a></div><div class='taskDescription' title='@@TaskDescription'><div>@@TaskDescription</div></div>",
    "calendarItemHtml": "<div class='eventItem'><div class='eventImage'><div class='eventMonth'>@@EventMonth</div><div class='eventDate'>@@EventDay</div></div> <div class='eventBody'><a href='@@EventUrl' onclick=\"LogEvent(appInsightsMatterLandingText + appInsightsOperations.CalendarEvent)\" target='_self'></a><div class='eventDuration'>@@EventTime</div> <div class='eventDescription' title='@@EventDescription'><div>@@EventDescription</div></div> </div> </div>",
    "userItemHtml": "<div class=\"teamFlyoutDataRow\">@@Username</div>",
    "hierarchyHtml": "<div title='@@MatterName' id=\"documentLibraryTitle\">@@MatterName</div>",
    "manageUsersHtml": "<div title='Manage users' class=\"manageUsers\"><a href=\"javascript:createModalDialog(true)\" onclick=\"LogEvent(appInsightsMatterLandingText + appInsightsOperations.ManageTeam)\">Manage users</a></div>",
    "noTasksHtml": "<div class='emptyItems'>@@NoTasksMessage</div>",
    "pinHtml": "<div class=\"pinIcon\"><div title=\"Pin\" id=\"PinMatter\" class=\"hide\" onclick=\"pinUnpinMatter()\"><img id=\"pinImg\" alt=\"Pin\" class=\"hide\" src=\"@@PinIcon\"><img id=\"unPinImg\" alt=\"Unpin\" class=\"hide\" src=\"@@UnpinIcon\"></div><img class=\"loadingIcon\" alt=\"Loading Image\" src=\"@@LoadingImage\"></div>",
    "matterInformationItemHtml": "<div class='matterDetailsColumn  @@matterDetailClass'><div class='matterDetailTitle'>@@PropertyName</div>  <div class='matterDetailText' title='@@PropertyValue'><div>@@PropertyValue</div></div></div>",
    "oneNoteHtml": "<a href=\"@@OneNoteLink\" target=\"_self\" title=\"Go to Matter OneNote\" onclick=\"LogEvent(appInsightsMatterLandingText + appInsightsOperations.OneNote)\"><img src=\"@@OneNoteIcon\" alt=\"Go to Matter OneNote\" />Go to Matter OneNote</a>",
    "oneNoteLoadingHtml": "<img class=\"loadingIcon\" alt=\"Loading Image\" src=\"@@LoadingImage\" />",
    "arrowHtml": "<span class=\"arrowHolder\"><img src=\"@@ArrowIcon\" class=\"arrowImg\" alt=\":\" /></span>"
};

/* Suffix for all the document library */
var libraryNameSuffix = {
    "oneNoteSuffix": "_OneNote",
    "taskSuffix": "_Task",
    "calendarSuffix": "_Calendar"
};

/* Assets location */
var oMatterLandingAssetsLocation = {
    oneNoteIcon: "images/OneNoteIcon.png",
    pinIcon: "images/Pin.png",
    userListIcon: "images/userList.png",
    unPinIcon: "images/Unpin.png",
    popupCloseIcon: "images/close-666.png",
    arrowImage: "/_layouts/15/images/spcommon.png"
};

/* Common objects used through out the code */
var oMatterLandingCommonObjects = {
    windowHeight: 0,
    oTaskData: {},
    nFooterPosition: 0,
    oCalendarData: {},
    tasksListQuery: "<View><Query> <Where><Eq><FieldRef Name='AssignedTo' /><Value Type='Integer'><UserID /></Value></Eq></Where> </Query> <ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='FileRef' /><FieldRef Name='Body' /><FieldRef Name='AssignedTo' /></ViewFields> <RowLimit>3</RowLimit></View>",
    calendarListQuery: "<View><Query><Where><Geq><FieldRef Name='EndDate' /><Value Type='DateTime'><Today /></Value></Geq></Where><OrderBy><FieldRef Name='EventDate' /></OrderBy> </Query> <ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='FileRef' /><FieldRef Name='EventDate' /><FieldRef Name='EndDate' /><FieldRef Name='Description' /><FieldRef Name='fAllDayEvent' /></ViewFields><RowLimit>3</RowLimit></View>",
    pinListQuery: "<View><Query><Where><Eq><FieldRef Name='UserAlias'/><Value Type='Text'>@@Username</Value></Eq></Where></Query><ViewFields> <FieldRef Name='UserPinDetails' /></ViewFields></View>",
    oMatterCenterAssetsLocation: "SiteAssets/Matter Center Assets/Matter Landing Assets/",
    hierarchyLibraryName: "Matter Library",
};

/* Azure page URLs */
var azurePages = {
    "managePermission": "[[[Manage Permission Relative Url]]]"
};

/* Modal Dialog object */
var oGlobalModalDialog = {
    width: 770,
    height: 500,
    url: oCommonLinks.AzureSiteUrl + azurePages.managePermission,
    ManageUserTitle: "Manage users",
    AddUserTitle: "Add users"
};

/* Holds the information required to pin the matter */
var oMatterDetails = {
    "ClientName": {}, "ClientID": {}, "MatterID": {}, "PracticeGroup": {}, "AreaOfLaw": {}, "ResponsibleAttorney": {}, "OpenDate": {},
    "MatterClientUrl": {}, "MatterUrl": {}
};

/* Messages for no items for different sections */
var noItemsMessage = {
    "noTasks": "There are no tasks created",
    "errorMsgTask": "Error occurred while trying to fetch tasks",
    "errorMsgCalendar": "Error occurred while trying to fetch events",
    "noEvents": "There are no active events",
    "noDescription": "There is no description for this matter",
    "errorRetrieveFails": "Error while trying to retrieve matter pin/unpin status",
    "errorMatterInfo": "Error while trying to retrieve matter information",
    "errorMatterDesc": "Error while trying to retrieve matter description",
    "errorPinUnpinData": "Error while trying to pin/unpin matter",
};

/* Property name mapping for the matter landing page */
var oPropertyNameMapping = {
    "MatterName": "Matter Name",
    "ClientName": "Client Name",
    "AreaOfLaw": "Area of Law",
    "ClientID": "Client Id",
    "MatterID": "Matter Id",
    "PracticeGroup": "Practice Group",
    "ResponsibleAttorney": "Responsible Attorney",
    "Description": "Description"
};

/* Regular expresssion for extracting date part from the data in calendar list */
var regularExpressions = {
    "extractDate": "/([\d]+:[\d]{2})(:[\d]{2})(.*)/, \"$1$3\""
};

/* Default text for application insights */
var appInsightsMatterLandingText = "Matter Landing Page/";

/* Operations for application insights tracking */
var appInsightsOperations = {
    "Pin": "Pin Matter",
    "Unpin": "Unpin Matter",
    "OneNote": "Go to OneNote",
    "Users": "View team members",
    "PageLoad": "Open Page",
    "Search": "Search",
    "MatterLink": "Web Matters",
    "DocumentLink": "Web Documents",
    "SettingLink": "Settings Page",
    "ManageTeam": "Manage team members",
    "MatterDescription": "Matter Description",
    "MatterInfo": "Matter Information",
    "Menu": "Menu option",
    "Calendar": "Calendar List",
    "Task": "Task List",
    "CalendarEvent": "Calendar Item",
    "TaskItem": "Task Event",
    "FeedbackSupport": "Feedback & Support",
    "PrivacyCookies": "Privacy & cookies",
    "TermsOfUse": "Terms of use",
    "Microsoft": "Microsoft"
};

/* List column names */
var listColumnNames = {
    "Title": "Title",
    "Body": "Body",
    "ItemUrl": "FileRef",
    "ID": "ID",
    "EventDate": "EventDate",
    "AllDayEvent": "fAllDayEvent",
    "EndDate": "EndDate",
    "Description": "Description"
};

/* Matter Detail classes */
var oMatterDetailClasses = {
    "ClientName": "clientName",
    "MatterID": "clientMatterID",
    "PracticeGroup": "practiceGroup",
    "AreaOfLaw": "areaOfLaw",
    "ResponsibleAttorney": "responsibleAttorney"

};

/* Declare global objects to be used */
var oPinProperties = {}, iCountVal = 0;
oGlobalConstants.sClientIdAndMatterIdSeperator = ".";
oGlobalConstants.sGetTitle = "_api/web/lists/getByTitle('";
oGlobalConstants.sGetProperties = "')/RootFolder/properties";
oGlobalConstants.sUrlExtension = "/Forms/AllItems.aspx";
oGlobalConstants.dispFormUrl = "/DispForm.aspx?ID=";
oGlobalConstants.sNoBlockedUserMessage = "No conflicted users";
oGlobalConstants.sNoTeamMemberMessage = "No Team members";
oGlobalConstants.iCounter = 0;
oGlobalConstants.iCheckFullPermissionCounter = 0;
oGlobalConstants.iRetryAttempts = 10;
oGlobalConstants.iTeamMembersCount = 0;
oGlobalConstants.iBlockedUsersCount = 0;
oGlobalConstants.sBlockedUsersChunk = "";
oGlobalConstants.sUsersChunk = "";
oGlobalConstants.arrDisplayUserName = [];
oGlobalConstants.arrDisplayBlockUserName = [];
oGlobalConstants.iPresenceCount = 100000;
oGlobalConstants.sProvisionMatterGroupName = "Provision Matter Users";
oGlobalConstants.sListName = "UserPinnedMatter";
oGlobalConstants.sPinColumn = "UserPinDetails";
oGlobalConstants.MatterPinnedDetails = "";
oGlobalConstants.collListItem = null;
oGlobalConstants.oList = null;
oGlobalConstants.sMatterIdAndClientIdTitle = "Client & Matter ID";
oGlobalConstants.oneNoteExtension = ".onetoc2";
oGlobalConstants.wopiFrameUrl = "/_layouts/WopiFrame.aspx?sourcedoc=";
oGlobalConstants.Go_To_OneNote = "GoToOneNote";
oGlobalConstants.sAppName = "ProvisionDMSMatter";
oGlobalConstants.sSendMailListName = "SendMail";
oGlobalConstants.sEffectivePermissionUrl = "{0}/_api/Web/lists/getbytitle('{1}')/EffectiveBasePermissions";
oGlobalConstants.matterCenterMatterList = "MatterCenterMatters";
// Declare the element for pin
var oElement = null;
//window.location.pathname.split('/').pop().replace('.aspx', '')
/* Function to get matter name in case if GUID is not present */
function getMatterName() {
    return ("undefined" === typeof (documentLibraryGUID)) ? documentLibraryName : documentLibraryGUID;
    //return documentLibraryGUID;
}

/* Document ready function */
$(document).ready(function () {
	

    "use strict";
    displayHeaderAndFooterContent();
    var clientUrl = _spPageContextInfo.webServerRelativeUrl + "/";
    LogEvent(appInsightsMatterLandingText + appInsightsOperations.PageLoad);
    /* Make a call to display content on the page */
    //documentLibraryGUID = window.location.pathname.split('/').pop().replace('.aspx', '')
    displayContent();
    /* Register the click event for Matter Information section */
   

    $("#matterLink").click(function (event) {
        LogEvent(appInsightsMatterLandingText + appInsightsOperations.MatterLink);
    });
    $("#documentLink").click(function (event) {
        LogEvent(appInsightsMatterLandingText + appInsightsOperations.DocumentLink);
    });
    $("#settingsLink").click(function (event) {
        LogEvent(appInsightsMatterLandingText + appInsightsOperations.SettingLink);
    });
    $("#footerLogo").click(function () {
        LogEvent(appInsightsMatterLandingText + appInsightsOperations.Microsoft);
    });

    $("#feedbackSupport").click(function () {
        LogEvent(appInsightsOperations.FeedbackSupport);
    });

    $("#privacyLink").click(function () {
        LogEvent(appInsightsOperations.PrivacyCookies);
    });

    $("#termsOfUse").click(function () {
        LogEvent(appInsightsOperations.TermsOfUse);
    });
    /* Register the click event for user icon */
    $(".userIcon").click(function (event) {
        $(".selectedUserIcon").removeClass("hide");
        $(".popupBackground").removeClass("hide");
        $(".teamFlyout").removeClass("hide");
        increasePopUpheight();
        LogEvent(appInsightsMatterLandingText + appInsightsOperations.Users);
        event && event.stopPropagation();
    });

    /* Register the click event for selected user icon section */
    $(".selectedUserIcon").click(function () {
        $(this).addClass("hide");
        $(".popupBackground").addClass("hide");
        $(".teamFlyout").addClass("hide");
    });

    /* Register the click event for menu icon section */
    $("#menu").click(function (event) {
        increasePopUpheight();
    });

    /* Register the click event for team user section */
    $(".teamSection").click(function () {
        $(".blockedUserSection").removeClass("selectedSection");
        $(".teamFlyoutData").removeClass("hide");
        $(".blockedFlyoutData").addClass("hide");
        $(this).addClass("selectedSection");
    });

    /* Register the click event for blocked user section */
    $(".blockedUserSection").click(function () {
        $(".teamSection").removeClass("selectedSection");
        $(".teamFlyoutData").addClass("hide");
        $(".blockedFlyoutData").removeClass("hide");
        $(this).addClass("selectedSection");

    });

    /* Register the click event for background */
    $(".popupBackground , .errorPopupBackground").on("click", function (event) {
        closeAllPopup(event);
    });

    /* Set max height on the basis of height of the window */
    $(".teamFlyoutData").css("max-height", oMatterLandingCommonObjects.windowHeight - 343);

    /* Register the click event for search icon */
    $(".searchIcon").on("click", function (event) {
        var sSearchText = $("#searchText").val().trim();
        if (sSearchText && "" !== sSearchText) {
            LogEvent(appInsightsMatterLandingText + appInsightsOperations.Search);
        }
    });

    /* Register the click event for error popup expand message option */
    $("#expandMessage, #collapseMessage").click(function () {
        if ($("#errorMessage").is(":visible")) {
            $("#errorMessage, #collapseMessage").addClass("hide");
            $("#expandMessage").removeClass("hide");
        } else {
            $("#errorMessage, #collapseMessage").removeClass("hide");
            $("#expandMessage").addClass("hide");
        }
    });

    // Register the close event of error popup
    $(".errorPopUpCloseIcon").on("click", function (event) {
        "use strict";
        $(".errorPopUpHolder").addClass("hide");
    });

    

    // Make a call to SharePoint functions
    ExecuteOrDelayUntilScriptLoaded(function () {
        var $task = $("#taskPane");
        var $calendar = $("#calenderPane");
        if ($task) {
            retrieveListItems(libraryNameSuffix.taskSuffix, oMatterLandingCommonObjects.tasksListQuery, false);
        }
        if ($calendar) {
            retrieveListItems(libraryNameSuffix.calendarSuffix, oMatterLandingCommonObjects.calendarListQuery, true);
        }
        getMetaDataProperties(clientUrl, documentLibraryName);
        getUserData();
        checkUserExistsInSharePointGroup();
        getCurrentUserTitle();
        retrievePinListItems();
    }, "sp.js");
    
});
$(window).on("resize", function (event) {
    adjustFooter();

});


/* Close the popup if clicked outside the selected area */
function closeAllPopup(event) {
    "use strict";
    $(".selectedUserIcon").addClass("hide");
    $(".teamFlyout").addClass("hide");
    $(".popupBackground").addClass("hide");
    $(".closeIcon").addClass("hide");
    $(".errorPopUpHolder").addClass("hide");
    // Check if div is open then close the div
    if ($(".menuFlyout").is(":visible")) {
        $(".menuFlyout").slideUp();
    }
}

/* Function to adjust footer position*/
function adjustFooter() {
    // Height of content box + margin of content box and height of sharepoint ribbon (183px)
    var oDifference = 183;
    var oFooterPosition = $("#contentBox").height() + oDifference;
    if ($(window).height() >= oFooterPosition) {
        $("#matterCenterFooter").css("margin-top", $(window).height() - oFooterPosition);
    }
    increasePopUpheight();
}

/* Increase Height of teamflyout */
function increasePopUpheight() {
    "use strict";
    /* Change the height of the display popup as per the position of the footer  */
    var iDocumentHeight = $("#s4-workspace")[0].scrollHeight;
    // Set popup background height to document height - distance between ribbon and usericon (81)
    $(".teamFlyout").css("height", iDocumentHeight - 81);
    $(".popupBackground").css("height", iDocumentHeight);
    $(".errorPopupBackground").css("height", iDocumentHeight);
}

/* Set ellipsis for the descriptions */
function setEllipsis($childContainer, itemID) {
    var oChild = $childContainer.eq(itemID);
    var oParent = $childContainer.eq(itemID).parent().height();
    while (oChild.outerHeight() > oParent) {
        oChild.text(function (index, text) {
            return text.replace(/.{5}$/, "...");
        });
    }

}

/* Create modal dialog for displaying the dialog box with manage permission page opened in it */
function createModalDialog(isEdit) {
    "use strict";
    var title, clientUrl;
    closeAllPopup(null);
    title = oGlobalModalDialog.ManageUserTitle;
    clientUrl = _spPageContextInfo.webAbsoluteUrl;
    var querystring = "&IsEdit=true&clientUrl=" + clientUrl + "&matterName=" + documentLibraryName + "&IsDlg=1";
    querystring = encodeURIComponent(querystring);
    var options = SP.UI.$create_DialogOptions();
    options.title = title;
    options.url = oGlobalModalDialog.url + "?" + querystring;

    options.width = oGlobalModalDialog.width;
    options.height = oGlobalModalDialog.height;
    if ("addEventListener" in window) { // All browsers except IE before version 9
        window.removeEventListener("message", receiveMessage, true);
        window.addEventListener("message", receiveMessage, true);
    } else if ("attachEvent" in window) { // IE before version 9
        window.detachEvent("onmessage", receiveMessage);
        window.attachEvent("onmessage", receiveMessage);
    }
    SP.UI.ModalDialog.showModalDialog(options);
}

/* For closing the dialog box */
function receiveMessage(event) {
    "use strict";
    var iValidIframeUrl = $("iframe[class=ms-dlgFrame]").attr("src"); // URL of Edit Matter page in i-Frame
    var anchorElement = document.createElement("a");                      // Temporary element to get the host name
    iValidIframeUrl = (iValidIframeUrl) ? iValidIframeUrl.split("?")[0] : "";
    anchorElement.href = iValidIframeUrl;
    var iValidIframeHost = anchorElement.protocol + "//" + anchorElement.hostname;

    if (event.origin !== iValidIframeHost) {
        return;
    }
    if (event.data) {
        if ("true" === event.data) {
            RefreshOnDialogClose();
        }
    }
}

/* Attaches all the content on the page */
function displayContent() {
   "use strict";    
   var url = "/sites/catalog/SiteAssets/Matter Center Assets/Common Assets/Scripts/uiconfigforspo.js";
   $.getScript( url, function() {
	    /* Remove the hierarchy div if it already exists */
	    var hierarchyDiv = $("#documentLibraryTitle");
	    if (hierarchyDiv) {
	        $(hierarchyDiv).remove();
	    }
	
	    var listUrl = _spPageContextInfo.webAbsoluteUrl + "/Lists";
	    //alert(uiconfigs.MatterLanding.Label1Tab1HeaderText);
	
	   oMatterLandingCommonObjects.hierarchyLibraryName = uiconfigs.MatterLanding.Label19Section4Text;
	    /* Set the matter information html */
	    var pinChunk = oMatterLandingHtmlConstants.pinHtml.replace("@@PinIcon", oCommonLinks.sCatalogSite + oMatterLandingCommonObjects.oMatterCenterAssetsLocation + oMatterLandingAssetsLocation.pinIcon);
	    pinChunk = pinChunk.replace("@@LoadingImage", oCommonLinks.sCatalogSite + oCommonLinks.oMatterCenterAssetsLocation + oCommonAssets.loadingImage);
	    pinChunk = pinChunk.replace("@@UnpinIcon", oCommonLinks.sCatalogSite + oMatterLandingCommonObjects.oMatterCenterAssetsLocation + oMatterLandingAssetsLocation.unPinIcon);
	    var sMatterInformationText = oMatterLandingHtmlConstants.matterInfoHtml.replace("@@PinChunk", pinChunk);
	    sMatterInformationText = sMatterInformationText.replace("@@FindMatterUrl", oFooterLinks.dashboard + oFooterLinks.matterDocumentsQueryString);
	    sMatterInformationText = sMatterInformationText.replace("@@MatterUrl", _spPageContextInfo.siteServerRelativeUrl + "/" + getMatterName() + oGlobalConstants.sUrlExtension);
	    sMatterInformationText = sMatterInformationText.replace(/@@MatterName/g, documentLibraryName);
	    sMatterInformationText = sMatterInformationText.replace(/@@LoadingImage/g, oCommonLinks.sCatalogSite + oCommonLinks.oMatterCenterAssetsLocation + oCommonAssets.loadingImage);
	    sMatterInformationText = sMatterInformationText.replace(/@@PopupCloseIcon/g, oCommonLinks.sCatalogSite + oMatterLandingCommonObjects.oMatterCenterAssetsLocation + oMatterLandingAssetsLocation.popupCloseIcon);
		
		sMatterInformationText  = sMatterInformationText.replace("@@Label1Tab1HeaderText", uiconfigs.MatterLanding.Label1Tab1HeaderText); 
		sMatterInformationText  = sMatterInformationText.replace("@@Label2Tab2HeaderText", uiconfigs.MatterLanding.Label2Tab2HeaderText);
		sMatterInformationText  = sMatterInformationText.replace("@@MyMatters", uiconfigs.MatterLanding.Label18MenuText);
        sMatterInformationText = sMatterInformationText.replace("title='My Matters'", "title='" + uiconfigs.MatterLanding.Label18MenuText + "'");
		

			    	    	    	    	    	    
	    $("#matterInfo").html(sMatterInformationText);
	    $("#matterInfo").parent().append(oMatterLandingHtmlConstants.hierarchyHtml.replace(/@@MatterName/g, oMatterLandingCommonObjects.hierarchyLibraryName));
	
	    /* Set the task panel html text */
	    var sTaskPanelText = oMatterLandingHtmlConstants.taskSectionHtml;
	    sTaskPanelText = sTaskPanelText.replace("@@LoadingImage", oCommonLinks.sCatalogSite + oCommonLinks.oMatterCenterAssetsLocation + oCommonAssets.loadingImage);
	    sTaskPanelText = sTaskPanelText.replace("@@TaskLink", listUrl + getMatterName() + libraryNameSuffix.taskSuffix);
	    sTaskPanelText = sTaskPanelText.replace("title='Task'> Task </div>", "title='" + uiconfigs.MatterLanding.Label14TaskeTitleText + "'> "+ uiconfigs.MatterLanding.Label14TaskeTitleText+" </div>");
	    sTaskPanelText = sTaskPanelText.replace("View / Edit </a>", "" + uiconfigs.MatterLanding.Label17EditTitleText + "</a>");
	    sTaskPanelText = sTaskPanelText.replace("title= \"View / Edit\"" , "title=\"" + uiconfigs.MatterLanding.Label17EditTitleText+ "\"");

	    
	    $("#taskPane").html(sTaskPanelText);
	    
	    noItemsMessage.noTasks =  uiconfigs.MatterLanding.ErrMsg1NoTask;
		noItemsMessage.errorMsgTask = uiconfigs.MatterLanding.ErrMsg2FetchTask;
		noItemsMessage.errorMsgCalendar = uiconfigs.MatterLanding.ErrMsg3CreateEvent; 
		noItemsMessage.noEvents = uiconfigs.MatterLanding.ErrMsg4ActiveEvents; 
		noItemsMessage.noDescription = uiconfigs.MatterLanding.ErrMsg5NoMatterDescription;
		noItemsMessage.errorRetrieveFails = uiconfigs.MatterLanding.ErrMsg6ForPinUnpinMatters; 
		noItemsMessage.errorMatterInfo = uiconfigs.MatterLanding.ErrMsg7MatterInformation;
		noItemsMessage.errorMatterDesc = uiconfigs.MatterLanding.ErrMsg8MatterDescription; 
		noItemsMessage.errorPinUnpinData = uiconfigs.MatterLanding.ErrMsg9NoPinUnpinMatter;
		
		oPropertyNameMapping.MatterName =  uiconfigs.MatterLanding.Label1MenuText;
		oPropertyNameMapping.ClientName = uiconfigs.MatterLanding.Label3Tab1Column1Text;
		oPropertyNameMapping.AreaOfLaw = uiconfigs.MatterLanding.Label6Tab1Column4Text;
		oPropertyNameMapping.ClientID = uiconfigs.MatterLanding.Label10Section1Text;
		oPropertyNameMapping.MatterID =  uiconfigs.MatterLanding.Label9Section1Text;
		oPropertyNameMapping.PracticeGroup= uiconfigs.MatterLanding.Label5Tab1Column3Text;
		oPropertyNameMapping.ResponsibleAttorney = uiconfigs.MatterLanding.Label7Tab1Column5Text;
		oPropertyNameMapping.Description = uiconfigs.MatterLanding.Label11Section1Text;
		
        oGlobalConstants.sMatterIdAndClientIdTitle  = uiconfigs.MatterLanding.Label4Tab1Column2Text;

        /* Set the calendar panel html text */
	    var sCalendarPanelText = oMatterLandingHtmlConstants.calendarSectionHtml;
	    sCalendarPanelText = sCalendarPanelText.replace("@@LoadingImage", oCommonLinks.sCatalogSite + oCommonLinks.oMatterCenterAssetsLocation + oCommonAssets.loadingImage);
	    sCalendarPanelText = sCalendarPanelText.replace("@@TaskLink", listUrl + getMatterName() + libraryNameSuffix.calendarSuffix);
	    sCalendarPanelText = sCalendarPanelText.replace("Calendar Events </div>", "" + uiconfigs.MatterLanding.Label15CalenderTitleText + " </div>");
        sCalendarPanelText = sCalendarPanelText.replace("title= \"View / Edit\"" , "title=\"" + uiconfigs.MatterLanding.Label17EditTitleText+ "\"");
        sCalendarPanelText = sCalendarPanelText.replace("View / Edit </a>", "" + uiconfigs.MatterLanding.Label17EditTitleText + "</a>");


	    $("#calendarPane").html(sCalendarPanelText);
	
	    /* Set the rss panel html text */
	    var sRSSText = oMatterLandingHtmlConstants.rssSectionHtml;
	    sRSSText = sRSSText.replace("Related News (RSS) </div>", "" + uiconfigs.MatterLanding.Label16RSSFeedTitleText+ "</a>");
        sRSSText = sRSSText.replace("title='Related News (RSS)'", "title='" +  uiconfigs.MatterLanding.Label16RSSFeedTitleText + "'");


	    $("#rssPane").html(sRSSText);
	
	    /* Add loading image to OneNote section */
	    $("#oneNotePane").html(oMatterLandingHtmlConstants.oneNoteLoadingHtml.replace("@@LoadingImage", 
	    	oCommonLinks.sCatalogSite + oCommonLinks.oMatterCenterAssetsLocation + oCommonAssets.loadingImage));
	    // Cache the pin element
    	oElement = $("#PinMatter");
		$(".matterDescriptionTitle, .matterProfileTitle").click(function () {
	        var className = $(this)[0].className;
	        if (-1 !== className.indexOf("matterDescriptionTitle")) {
	            $(".matterDescriptionTitle").removeClass("changeSection");
	            $(".matterProfileTitle").addClass("changeSection");
	            $(".matterDescriptionBody").removeClass("hide");
	            $(".matterProfileBody").addClass("hide");
	            LogEvent(appInsightsMatterLandingText + appInsightsOperations.MatterDescription);
	      	}
			else {
	            $(".matterDescriptionTitle").addClass("changeSection");
	            $(".matterProfileTitle").removeClass("changeSection");
	            $(".matterDescriptionBody").addClass("hide");
	            $(".matterProfileBody").removeClass("hide");
	            LogEvent(appInsightsMatterLandingText + appInsightsOperations.MatterInfo);
        	}
    	});
    	
    	$("#matterLink").text(uiconfigs.MatterLanding.Menu1Option1Text);
    	$("#matterLink").attr('title', uiconfigs.MatterLanding.Menu1Option1Text);
    	
    	$("#documentLink").text(uiconfigs.MatterLanding.Menu1Option2Text);
    	$("#documentLink").attr('title', uiconfigs.MatterLanding.Menu1Option2Text);

    	$("#settingsLink").text(uiconfigs.MatterLanding.Menu1Option3Text);
    	$("#settingsLink").attr('title', uiconfigs.MatterLanding.Menu1Option3Text);
    	
    	$(".iconText .mainText")[0].innerHTML =  uiconfigs.MatterLanding.MenuImageUpperCaption;
    	$(".iconText .subText")[0].innerHTML =  uiconfigs.MatterLanding.MenuImageLowerCaption;
    	
    	//$("#searchText").attr('placeholder', uiconfigs.MatterLanding.Menu1Option3Text);

    	


    });
}

// Replace query string key with value 
function replaceQueryStringAndGet(url, key, value) {
    "use strict";
    var regExp = new RegExp("([?|&])" + key + "=.*?(&|$)", "i");
    var separator = url.indexOf("?") !== -1 ? "&" : "?";
    if (url.match(regExp)) {
        return url.replace(regExp, "$1" + key + "=" + value + "$2");
    } else {
        return url + separator + key + "=" + value;
    }
}

// Display folder navigation
function folderNavigation() {
    "use strict";
    // Call the function for post render
    function onPostRender(renderCtx) {
        if (renderCtx.rootFolder) {
            var listUrl = decodeURIComponent(renderCtx.listUrlDir), navigationItems = [], rootFolder = decodeURIComponent(renderCtx.rootFolder), index = 0, folderPath, pathArray = [], currentFolderUrl = "";
            var rootNavItem = {
                title: oMatterLandingCommonObjects.hierarchyLibraryName, //// renderCtx.ListTitle Using static text Matter Library
                url: replaceQueryStringAndGet(document.location.href, "RootFolder", listUrl)
            };

            if ("" === renderCtx.rootFolder || rootFolder.toLowerCase() == listUrl.toLowerCase()) {
                navigationItems.push(rootNavItem);

            } else {
                // Get the folder path excluding list url, removing list url will give us path relative to current list url 
                folderPath = rootFolder.toLowerCase().indexOf(listUrl.toLowerCase()) == 0 ? rootFolder.substr(listUrl.length) : rootFolder;
                pathArray = folderPath.split("/");
                currentFolderUrl = listUrl;

                navigationItems.push(rootNavItem);

                for (index = 0; index < pathArray.length; index++) {
                    if ("" === pathArray[index]) {
                        continue;
                    }
                    var lastItem = index == pathArray.length - 1;
                    currentFolderUrl += "/" + pathArray[index];
                    var item = {
                        title: pathArray[index],
                        url: lastItem ? "" : replaceQueryStringAndGet(document.location.href, "RootFolder", encodeURIComponent(currentFolderUrl))
                    };
                    navigationItems.push(item);
                }
            }

            /* Call the render function when dom content are loaded */
            if (document.addEventListener) {
                document.addEventListener("DOMContentLoaded", function () { RenderItems(renderCtx.wpq, navigationItems); }, false);
            } else {
                document.attachEvent("onreadystatechange", function () {
                    RenderItems(renderCtx.wpq, navigationItems);
                });
            }
        }
    }


    // Add a div and then render navigation items inside span 
    function RenderItems(webPartId, navigationItems) {
        if (0 === navigationItems.length) {
            return;
        } else if (1 === navigationItems.length) {
            navigationItems[0].url = "";
        }

        var folderNavDivClass = "documentLibraryTitle", hierarchyItemsClass = "hierarchyItems", webpartDivId = "WebPart" + webPartId;
        // Div is added beneath the header to show folder navigation 
        var folderNavDiv = document.getElementById(folderNavDivClass);
        if (null !== folderNavDiv) {
            folderNavDiv.parentNode.removeChild(folderNavDiv);
            folderNavDiv = null;
        }

        folderNavDiv = document.createElement("div");
        folderNavDiv.setAttribute("id", folderNavDivClass);
        $("#matterInfo").parent().append(folderNavDiv);
        folderNavDiv = document.getElementById(folderNavDivClass);
        var span = null, anchor = null;
        var iStartIndex = 0, iLength = navigationItems.length;
        if (2 < iLength) {
            while (2 < iLength) {
                iStartIndex++;
                iLength--;
            }
        }
        // Display items on the page
        for (var index = iStartIndex; index < navigationItems.length; index++) {
            if ("" === navigationItems[index].url) {
                span = document.createElement("span");
                span.innerHTML = navigationItems[index].title;
                span.setAttribute("class", hierarchyItemsClass);
                span.setAttribute("title", navigationItems[index].title);
                folderNavDiv.appendChild(span);
            } else {
                span = document.createElement("span");
                anchor = document.createElement("a");
                anchor.setAttribute("href", navigationItems[index].url);
                anchor.innerHTML = navigationItems[index].title;
                span.setAttribute("class", hierarchyItemsClass);
                anchor.setAttribute("title", navigationItems[index].title);
                span.setAttribute("title", navigationItems[index].title);
                span.appendChild(anchor);
                folderNavDiv.appendChild(span);
            }

            // Add arrow (>) to separate navigation items, except the last one 
            if (index != navigationItems.length - 1) {
                span = document.createElement("span");
                span.innerHTML = oMatterLandingHtmlConstants.arrowHtml.replace("@@ArrowIcon", oCommonLinks.sCatalogSite + oMatterLandingCommonObjects.oMatterCenterAssetsLocation + oMatterLandingAssetsLocation.arrowImage);
                folderNavDiv.appendChild(span);
            }
        }

    }

    // Register the post render event of list view web part with our custom function
    function _registerTemplate() {
        var viewContext = {};
        viewContext.Templates = {};
        viewContext.OnPostRender = onPostRender;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(viewContext);
    }

    // Delay the execution of the script until client templates gets loaded 
    ExecuteOrDelayUntilScriptLoaded(_registerTemplate, "clienttemplates.js");
};

// Call the function to display the navigation items
folderNavigation();

// Get metadata associated with matter on the page
function getMetaDataProperties(sClientPath, sMatterName) {
    "use strict";
    var sDomainAddress = window.location.protocol + "//" + window.location.hostname;
    var iFlag, oPropertiesList;
    oPinProperties["MatterClientUrl"] = sDomainAddress + sClientPath;
    oMatterDetails.MatterClientUrl = oPinProperties["MatterClientUrl"];
    oPinProperties["MatterUrl"] = oPinProperties["MatterClientUrl"] + getMatterName() + oGlobalConstants.sUrlExtension;
    oMatterDetails.MatterUrl = oPinProperties["MatterUrl"];
    iFlag = 0;
    oPropertiesList = ["ClientName", "ClientID", "MatterID", "PracticeGroup", "AreaOfLaw", "ResponsibleAttorney"];
    var clientUrl = sDomainAddress + sClientPath + oGlobalConstants.sGetTitle + sMatterName + oGlobalConstants.sGetProperties;
    getPropertiesForMatter(clientUrl, iFlag, oPropertiesList);

    iFlag = 1;
    oPropertiesList = ["Description"];
    clientUrl = sDomainAddress + sClientPath + oGlobalConstants.sGetTitle + sMatterName + "')";
    getPropertiesForMatter(clientUrl, iFlag, oPropertiesList);

    // Update the OneNote Link
    urlExists();
}

// Make a call to get the properties from the Document library
function getPropertiesForMatter(clientUrl, iFlag, oPropertiesList) {
    "use strict";
    $.ajax({
        type: "GET",
        url: clientUrl,  // URL from where the XML code of metadata properties is retrieved
        contentType: "text/xml",
        dataType: "text",
        success: function (xml) {
            processXML(xml, iFlag, oPropertiesList);  // Call the processXML function to process the XML code and get the properties and display it
        },
        error: function (response) {
            if (0 === iFlag) {
                onQueryFailed("MatterData");
            } else {
                onQueryFailed("MatterDescription");
            }
        }
    });
}
function processXML(xml, iFlag, oPropertiesList) {
    "use strict";
    var oProcessedXml, iLength, oElements, iterator, iPropertiesLength, iPropertyIterator, oPinPropertiesList, sTextContent, sClientId;
    oProcessedXml = $.parseXML(xml);
    var oMatterInfoCount = 0;
    oPinPropertiesList = ["MatterName", "ClientName", "AreaOfLaw", "PracticeGroup", "SubAreaOfLaw", "ResponsibleAttorney", "TeamMembers", "IsMatter", "OpenDate", "ClientID", "MatterID", "BlockedUploadUsers", "ClientName", "BlockedUsers", "LastItemModifiedDate", "MatterCenterUserEmails", "ResponsibleAttorneyEmail"];  // Properties that are required for Pinning matter
    iLength = oProcessedXml.firstChild.lastChild.firstChild.childElementCount;
    oElements = oProcessedXml.firstChild.lastChild.firstChild;
    iterator = 0;
    iPropertiesLength = oPropertiesList.length;
    iPropertyIterator = 0;
    $(".matterDetails .loadingIcon").hide();
    for (iPropertyIterator = 0; iPropertyIterator < iPropertiesLength; iPropertyIterator++) {
        for (iterator = 0; iterator < iLength; iterator++) {
            if (oElements.childNodes[iterator].localName === oPropertiesList[iPropertyIterator]) {  // Comparing XML properties with the properties we required
                if (0 === iFlag) {
                    sTextContent = oElements.childNodes[iterator].textContent;
                    if (("AreaOfLaw").toLowerCase() === (oPropertiesList[iPropertyIterator]).toLowerCase() && sTextContent || ("PracticeGroup").toLowerCase() === (oPropertiesList[iPropertyIterator]).toLowerCase() && sTextContent || ("ResponsibleAttorney").toLowerCase() === (oPropertiesList[iPropertyIterator]).toLowerCase() && sTextContent) {
                        if (sTextContent.lastIndexOf(";") === sTextContent.length - 1) {
                            sTextContent = sTextContent.substring(0, sTextContent.length - 1);  // Use substring to remove ; at the end of the string
                        } else if (sTextContent.lastIndexOf(";") === sTextContent.length - 2) {
                            sTextContent = sTextContent.substring(0, sTextContent.length - 2);  // Use substring to remove ; at the end of the string
                        }
                        if (("ResponsibleAttorney").toLowerCase() === (oPropertiesList[iPropertyIterator]).toLowerCase() && sTextContent) {
                            var arrResponsibleAttorney = sTextContent.split(";");
                            arrResponsibleAttorney = $.map(arrResponsibleAttorney, function (n) { return n.trim(); });
                            sTextContent = unique(arrResponsibleAttorney).toString();
                        }
                    } else if (("ClientID").toLowerCase() === (oPropertiesList[iPropertyIterator]).toLowerCase()) {
                        sClientId = sTextContent;
                        continue;
                    } else if (("MatterID").toLowerCase() === (oPropertiesList[iPropertyIterator]).toLowerCase()) {
                        sTextContent = sClientId + oGlobalConstants.sClientIdAndMatterIdSeperator + sTextContent;
                        oPropertyNameMapping[oPropertiesList[iPropertyIterator]] = oGlobalConstants.sMatterIdAndClientIdTitle;
                    }
                    $(".matterDetails").append(oMatterLandingHtmlConstants.matterInformationItemHtml.replace("@@PropertyName", oPropertyNameMapping[oPropertiesList[iPropertyIterator]]).replace(/@@PropertyValue/g, sTextContent).replace("@@matterDetailClass", oMatterDetailClasses[oPropertiesList[iPropertyIterator]]));
                    setEllipsis($(".matterDetailText div"), oMatterInfoCount);
                    oMatterInfoCount++;
                }
                if (1 === iFlag) {
                    var sContent = (oElements.childNodes[iterator].textContent).trim() ? oElements.childNodes[iterator].textContent.trim() : noItemsMessage.noDescription;
                    $(".matterDescriptionBody div").attr("title", sContent).empty().append(sContent);
                    setEllipsis($(".matterDescriptionBody div"), 0);
                    oMatterDetails[oPropertiesList[iPropertyIterator]] = oElements.childNodes[iterator].textContent;
                    oPinProperties[oPropertiesList[iPropertyIterator]] = oElements.childNodes[iterator].textContent; // Add Description to pin properties list 
                }
            }
        }
    }

    // Store all the properties in variable that will be further used for pinning matter
    iPropertiesLength = oPinPropertiesList.length;
    for (iPropertyIterator = 0; iPropertyIterator < iPropertiesLength; iPropertyIterator++) {
        for (iterator = 0; iterator < iLength; iterator++) {
            if (oElements.childNodes[iterator].localName === oPinPropertiesList[iPropertyIterator]) { // Comparing XML properties with the properties we required 
                oMatterDetails[oPinPropertiesList[iPropertyIterator]] = oElements.childNodes[iterator].textContent;
                oPinProperties[oPinPropertiesList[iPropertyIterator]] = oElements.childNodes[iterator].textContent;
            }
        }
    }

    iCountVal++;
    if (2 <= iCountVal) {
        if (oElement) {
            oElement.disabled = false;
        }
    }
}

/* Get the unique items from the list */
function unique(list) {
    "use strict";
    var result = [];
    $.each(list, function (iIndex, item) {
        if (-1 === $.inArray(item, result)) {
            result.push(item);
        }
    });
    return result;
}


// Function is used to get user data to be displayed in the user list popup
function getUserData() {
    "use strict";
    if (!oPinProperties.TeamMembers && oGlobalConstants.iRetryAttempts > oGlobalConstants.iCounter) {
        oGlobalConstants.iCounter++;
        setTimeout(function () { getUserData(); }, 1000);
    } else {
        var clientContext = new SP.ClientContext.get_current();
        var arrBlockUserName, arrUserNames, arrTeamMembers = [], arrUsersIds = [], arrResponsibleAttorney = [];
        if (oPinProperties.MatterCenterUserEmails) {
            arrTeamMembers = htmlDecode(oPinProperties.MatterCenterUserEmails).split("$|$");
        } else {
            arrTeamMembers = trimEndChar(htmlDecode(oPinProperties.TeamMembers.trim()), ";").split(";");
        }
        if (oPinProperties.ResponsibleAttorneyEmail) {
            arrResponsibleAttorney = trimEndChar(htmlDecode(oPinProperties.ResponsibleAttorneyEmail.trim()), ";").split(";");
        } else if (oPinProperties.ResponsibleAttorney) {
            arrResponsibleAttorney = trimEndChar(htmlDecode(oPinProperties.ResponsibleAttorney.trim()), ";").split(";");
        }
        arrUserNames = $.merge(arrTeamMembers, arrResponsibleAttorney);
        if (oPinProperties.BlockedUsers && trimEndChar(htmlDecode(oPinProperties.BlockedUsers.trim()), ";").length > 0) {
            arrBlockUserName = trimEndChar(htmlDecode(oPinProperties.BlockedUsers.trim()), ";").split(";");
            arrBlockUserName = unique($.map(arrBlockUserName, function (item) { return item.trim(); }));
        }
        //// Added all users in single dimensional array
        if (arrUserNames) {
            $.each(arrUserNames, function (key, value) {
                if (value) {
                    var arrUserName = value.split(";");
                    $.each(arrUserName, function (key, sUserName) {
                        if (sUserName) {
                            arrUsersIds.push(sUserName.trim());
                        }
                    });
                }
            });
            arrUsersIds = unique($.map(arrUsersIds, function (item) { return item.trim() !== "" ? item.trim() : null; }));
            oGlobalConstants.iTeamMembersCount = arrUsersIds.length;
            $(".teamSection .userNumber").text("(" + oGlobalConstants.iTeamMembersCount + ")");
            $(".teamFlyoutData").find(".loadingIcon").addClass("hide");
            $.each(arrUsersIds, function (key, value) {
                getUserEmail(clientContext, value, false);
            });
        }
        if (arrBlockUserName) {
            oGlobalConstants.iBlockedUsersCount = arrBlockUserName.length;
            $(".blockedUserSection .userNumber").text("(" + oGlobalConstants.iBlockedUsersCount + ")");
            $(".blockedFlyoutData").find(".loadingIcon").addClass("hide");
            $.each(arrBlockUserName, function (key, value) {
                if (value) {
                    getUserEmail(clientContext, value, true);
                } else {
                    oGlobalConstants.iBlockedUsersCount--;
                }
            });
        }

        if (!oGlobalConstants.iTeamMembersCount) {
            $(".teamFlyoutData").append("<div class='teamFlyoutDataRow'>" + oGlobalConstants.sNoTeamMemberMessage + "</div>");
        }

        if (!oGlobalConstants.iBlockedUsersCount) {
            $(".blockedFlyoutData").empty().append("<div class='teamFlyoutDataRow'>" + oGlobalConstants.sNoBlockedUserMessage + "</div>");
        }
    }
}


// Function is used to get and assign user email ID to lync sip presence
function getUserEmail(clientContext, loginName, isBlockedUser) {
    "use strict";
    var oUser = clientContext.get_web().ensureUser(loginName);
    clientContext.load(oUser);
    clientContext.executeQueryAsync(function () {
        if (isBlockedUser) {
            oGlobalConstants.arrDisplayBlockUserName.push({ "username": oUser.get_title(), "email": oUser.get_email() });
            oGlobalConstants.iBlockedUsersCount--;
            if (!oGlobalConstants.iBlockedUsersCount) {
                displayUserDetails(oGlobalConstants.arrDisplayBlockUserName, false);
            }
        } else {
            oGlobalConstants.arrDisplayUserName.push({ "username": oUser.get_title(), "email": oUser.get_email() });
            oGlobalConstants.iTeamMembersCount--;
            if (!oGlobalConstants.iTeamMembersCount) {
                displayUserDetails(oGlobalConstants.arrDisplayUserName, true);
            }
        }

        if (!oGlobalConstants.iTeamMembersCount && !oGlobalConstants.iBlockedUsersCount) {
            $(".blockedFlyoutData").addClass("hide");
        }
        ProcessImn();
    },
    function () { //  Do nothing.
    });
}

// Function to trim trailing special character if present
function trimEndChar(sOrignalString, sCharToTrim) {
    "use strict";
    if (sCharToTrim === sOrignalString.substr(-1)) {
        return sOrignalString.substr(0, sOrignalString.length - 1);
    }
    return sOrignalString;
}

// Function to display Team Members and Conflicted user on flyout
function displayUserDetails(oUserArray, IsTeam) {
    "use strict";
    oUserArray = oUserArray.sort(function (sPrevious, sNext) {
        if (sPrevious.username.toLowerCase() === sNext.username.toLowerCase()) {
            return 0;
        } else {
            return (sPrevious.username.toLowerCase() < sNext.username.toLowerCase()) ? -1 : 1;
        }
    });
    oGlobalConstants.sUsersChunk = "";
    $.each(oUserArray, function (key, val) {
        oGlobalConstants.sUsersChunk += "<span class=\"ms-verticalAlignTop ms-noWrap teamFlyoutDataRow\"><span class=\"ms-imnSpan\"><a class=\"ms-imnlink ms-spimn-presenceLink\" onclick='WriteDocEngagementLog(\"DocModifiedByPresenceClick\", \"ODModifiedByPresenceClick\"); IMNImageOnClick(event);return false;' href=\"#\"><span class=\"ms-spimn-presenceWrapper ms-imnImg ms-spimn-imgSize-10x10\"><img name=\"imnmark\" title=\"\" class=\"ms-spimn-img ms-spimn-presence-disconnected-10x10x32\" id=\"imn_" + (oGlobalConstants.iPresenceCount++) + ",type=sip\" alt=\"No presence information\" src=\"/_layouts/15/images/spimn.png\" sip=\"" + val.email + "\" showofflinepawn=\"1\"></span></a><a href=\"sip:" + val.email + "\">" + val.username + "</a></span></span>";
    });
    if (IsTeam) {
        $(".teamFlyoutData").append(oGlobalConstants.sUsersChunk);
    } else {
        $(".blockedFlyoutData").append(oGlobalConstants.sUsersChunk);
    }
}

// Function to check if user is exist of SharePoint Group
function checkUserExistsInSharePointGroup() {
    "use strict";
    var sUrl = oCommonLinks.sCatalogSite + oGlobalConstants.sEffectivePermissionUrl.replace("{0}", oGlobalConstants.matterCenterMatterList);
    $.ajax({
        url: sUrl,
        type: "GET",
        dataType: "json",
        headers: {
            Accept: "application/json;odata=verbose"
        },
        success: function (oData) {
            onPermissionSuccess(oData);
        },
        error: function () {
            onFailure();
        }
    });
}

// Function to check logged in user has permission on list
function onPermissionSuccess(oData) {
    "use strict";
    if (oData) {
        var permission = new SP.BasePermissions();
        permission.initPropertiesFromJson(oData.d.EffectiveBasePermissions);
        var bHasPermission = permission.has(SP.PermissionKind.editListItems);
        if (bHasPermission) {
            checkIfUserHasFullPermissions();
        }
    }
}

// Function for when group and user is load failed
function onFailure() {
    "use strict";
    return false; // DO nothing
}

// Make a call to get the permissions of all the users on the document library
function checkIfUserHasFullPermissions() {
    "use strict";
    if ((!oPinProperties.MatterClientUrl) && oGlobalConstants.iRetryAttemps > oGlobalConstants.iCheckFullPermissionCounter) {
        oGlobalConstants.iCheckFullPermissionCounter++;
        setTimeout(function () {
            checkIfUserHasFullPermissions();
        }, 1000);
    } else {
        $.ajax({
            type: "GET",
            url: oPinProperties.MatterClientUrl + "_api/web/lists/getbytitle('" + documentLibraryName + "')/EffectiveBasePermissions",
            dataType: "json",
            headers: {
                Accept: "application/json;odata=verbose"
            },
            success: function (oData) {
                // Call the processXML function to process the XML code and get the properties and display it
                onFullPermissionSuccess(oData);
            },
            error: function () {
                return false;
            }
        });
    }
}
function onFullPermissionSuccess(oData) {
    "use strict";
    var permission = new SP.BasePermissions();
    permission.initPropertiesFromJson(oData.d.EffectiveBasePermissions);
    var bHasPermission = permission.has(SP.PermissionKind.managePermissions);
    if (bHasPermission) {
        $(".teamFlyoutData").prepend(oMatterLandingHtmlConstants.manageUsersHtml);
    }
}

// Process the user xml retrieved for Matter Library
function processUserXML(xml) {
    "use strict";
    var oProcessedXml, iLength, oElements, iterator, iPropertiesLength, iPropertyIterator, oPinPropertiesList, sTextContent, sClientId;
    oProcessedXml = $.parseXML(xml);
    oPinPropertiesList = ["Name"];
    if (oProcessedXml.firstChild.lastChild.lastChild.firstChild && oProcessedXml.firstChild.lastChild.lastChild.firstChild.childElementCount) {
        iLength = oProcessedXml.firstChild.lastChild.lastChild.firstChild.childElementCount;
        oElements = oProcessedXml.firstChild.lastChild.lastChild.firstChild;
        iterator = 0;
        iPropertiesLength = oPinPropertiesList.length;
        iPropertyIterator = 0;
        for (iPropertyIterator = 0; iPropertyIterator < iPropertiesLength; iPropertyIterator++) {
            for (iterator = 0; iterator < iLength; iterator++) {
                if (oElements.childNodes[iterator].localName === oPinPropertiesList[iPropertyIterator]) {
                    if ("Full Control" === oElements.childNodes[iterator].textContent) {
                        $(".teamFlyoutData").prepend(oMatterLandingHtmlConstants.manageUsersHtml);
                    }
                }
            }
        }
    }
}

// Retrieve items from the calendar when page is loaded
function retrieveListItems(listSuffix, listQuery, isCalender) {
    "use strict";
    var listName = documentLibraryName + listSuffix;
    var context = new SP.ClientContext.get_current();
    var web = context.get_web();
    var list = web.get_lists().getByTitle(listName);
    var camlQuery = new SP.CamlQuery();
    camlQuery.set_viewXml(listQuery);
    if (isCalender) {
        oMatterLandingCommonObjects.oCalendarData = list.getItems(camlQuery);
        context.load(oMatterLandingCommonObjects.oCalendarData);
        context.executeQueryAsync(Function.createDelegate(this, function () { onQuerySucceed(); }), Function.createDelegate(this, function () {
            onQueryFailed("Calendar");
        }));
    } else {
        oMatterLandingCommonObjects.oTaskData = list.getItems(camlQuery);
        context.load(oMatterLandingCommonObjects.oTaskData);
        context.executeQueryAsync(Function.createDelegate(this, function () { taskSuccess(); }), Function.createDelegate(this, function () {
            onQueryFailed("Task");
        }));
    }
}

// Task success function
function taskSuccess() {
    "use strict";
    $(".taskBoard").empty();
    var taskCount = 0;
    var listEnumerator = oMatterLandingCommonObjects.oTaskData.getEnumerator();
    while (listEnumerator.moveNext()) {
        var currentItem = listEnumerator.get_current();
        var title = currentItem.get_item(listColumnNames.Title);
        var body = currentItem.get_item(listColumnNames.Body);
        if (body) {
            var oTask = $(body);
            if (0 < $(oTask).length) {
                body = htmlDecode($(oTask).text().trim());
            }
            if ("" === body) {
                body = oGlobalConstants.sNotApplicable;
            }
        } else {
            body = oGlobalConstants.sNotApplicable;
        }
        var url = currentItem.get_item(listColumnNames.ItemUrl);
        url = url.replace(url.substring(url.lastIndexOf("/"), url.length), "");
        url += oGlobalConstants.dispFormUrl + currentItem.get_item(listColumnNames.ID);
        var taskItem = oMatterLandingHtmlConstants.taskItemHtml.replace(/@@TaskDescription/g, body);
        taskItem = taskItem.replace("@@TaskUrl", url);
        var oTaskBody = $(taskItem);
        oTaskBody.find("a").text(title).attr("title", title);
        $(".taskBoard").append(oTaskBody);
        setEllipsis($(".taskDescription div"), taskCount);
        taskCount++;
    }
    if (0 === taskCount) {
        $(".taskBoard").empty().append(oMatterLandingHtmlConstants.noTasksHtml.replace("@@NoTasksMessage", noItemsMessage.noTasks));
    }
    adjustFooter();
}

/* Decodes the html value */
function htmlDecode(value) {
    "use strict";
    return $("<div/>").html(value).text();
}


// Function to be called when event items are retrieved
function onQuerySucceed() {
    "use strict";
    var title = "", eventCount = 0, taskLibrary = "", oDescription = null, timeString = "";
    $(".eventBoard").empty();
    var months = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];
    var listEnumerator = oMatterLandingCommonObjects.oCalendarData.getEnumerator();
    while (listEnumerator.moveNext()) {
        var currentItem = listEnumerator.get_current();
        var month = months[getEventMonth(currentItem.get_item(listColumnNames.EventDate))];
        var isFullDay = currentItem.get_item(listColumnNames.AllDayEvent);
        var day = appendZero(getEventDay(currentItem.get_item(listColumnNames.EventDate)));
        var endMonth = months[getEventMonth(currentItem.get_item(listColumnNames.EndDate))];
        var endDay = appendZero(getEventDay(currentItem.get_item(listColumnNames.EndDate)));
        var startTime = getTime(currentItem.get_item(listColumnNames.EventDate));
        var endtime = getTime(currentItem.get_item(listColumnNames.EndDate));
        var url = currentItem.get_item(listColumnNames.ItemUrl);
        url = url.replace(url.substring(url.lastIndexOf("/"), url.length), "");
        url += oGlobalConstants.dispFormUrl + currentItem.get_item(listColumnNames.ID);
        title = currentItem.get_item(listColumnNames.Title);
        var description = currentItem.get_item(listColumnNames.Description);
        if (description) {
            oDescription = $(description);
            if (0 < $(oDescription).length) {
                description = htmlDecode($(oDescription).text().trim());
            }
            /* Check if description is empty */
            if ("" === description) {
                description = oGlobalConstants.sNotApplicable;
            }
        } else {
            description = oGlobalConstants.sNotApplicable;
        }
        // Building event div including the data we got from our list
        if (isFullDay) {
            timeString = "All Day";
        } else {
            timeString = startTime + " - " + endtime + " " + endMonth + " " + endDay;
        }

        var calendarItem = oMatterLandingHtmlConstants.calendarItemHtml.replace("@@EventMonth", month);
        calendarItem = calendarItem.replace("@@EventDay", day);
        calendarItem = calendarItem.replace("@@EventUrl", url);
        calendarItem = calendarItem.replace("@@EventTime", timeString);
        calendarItem = calendarItem.replace(/@@EventDescription/g, description);
        var oCalendarData = $(calendarItem);
        oCalendarData.find(".eventBody a").text(title).attr("title", title);
        $(".eventBoard").append(oCalendarData);
        setEllipsis($(".eventDescription div"), eventCount);
        eventCount++;
    }
    if (0 === eventCount) {
        $(".eventBoard").empty().append(oMatterLandingHtmlConstants.noTasksHtml.replace("@@NoTasksMessage", noItemsMessage.noEvents));
    }
    adjustFooter();
}

// Gets event day in PST form
function getEventDay(date) {
    "use strict";
    var updatedDate;
    if (date) {
        updatedDate = new Date(date);
        return updatedDate.getUTCDate();
    }
}

// Gets event month in PST form
function getEventMonth(date) {
    "use strict";
    var updatedDate;
    if (date) {
        updatedDate = new Date(date);
        return updatedDate.getUTCMonth();
    }
}


/* Appends zero to the day if it is less than 10 */
function appendZero(day) {
    "use strict";
    return day < 10 ? "0" + day : "" + day;
}

/* Get time from the retrieved date */
function getTime(date) {
    "use strict";
    var time = "";
    if (date) {
        var oDate = new Date(date);
        time = getFormattedHours(oDate.getUTCHours()) + ":" + appendZero(oDate.getUTCMinutes()) + " " + getAmOrPm(oDate.getUTCHours());
    }
    return time;
}

// Get hours in AM PM format
function getFormattedHours(hours) {
    "use strict";
    var formattedHours = hours;
    if (hours > 12) {
        formattedHours = hours % 12;
        if (0 === formattedHours) {
            formattedHours = 12;
        }
    } else if (0 === hours) {
        formattedHours = 12;
    }
    formattedHours = appendZero(formattedHours);
    return formattedHours;
}

/* Returns AM or PM based on the hours of the time */
function getAmOrPm(hours) {
    "use strict";
    return hours < 12 ? "AM" : "PM";
}

// Failure function handler
function onQueryFailed(sOperationType) {
    // To do: handle failure scenario
    if ("Calendar" === sOperationType) {
        $(".eventBoard").empty().append(oMatterLandingHtmlConstants.noTasksHtml.replace("@@NoTasksMessage", noItemsMessage.errorMsgCalendar));
    } else if ("Task" === sOperationType) {
        $(".taskBoard").empty().append(oMatterLandingHtmlConstants.noTasksHtml.replace("@@NoTasksMessage", noItemsMessage.errorMsgTask));
    } else if (sOperationType === "MatterData") {
        $(".matterProfileBody .matterDetails").empty().text(noItemsMessage.errorMatterInfo);
    } else if (sOperationType === "MatterDescription") {
        $(".matterDescriptionBody").empty().text(noItemsMessage.errorMatterDesc);
    } else if (oGlobalConstants.sOperationPin === sOperationType || oGlobalConstants.sOperationUnPin === sOperationType || "RetrieveItems" === sOperationType) {
        $(".errorPopUpHolder").removeClass("hide");
        $(".pinIcon .loadingIcon").addClass("hide");
        if (oGlobalConstants.sOperationPin === sOperationType || oGlobalConstants.sOperationUnPin === sOperationType) {
            $("#errorMessage").empty().text(noItemsMessage.errorPinUnpinData);
        } else if ("RetrieveItems" === sOperationType) {
            $("#errorMessage").empty().text(noItemsMessage.errorRetrieveFails);
        }
    }
    adjustFooter();
}


// Retrieve the pin list items
function retrievePinListItems() {
    "use strict";
    // Wait for getCurrentUserTitle to complete
    if ("undefined" === typeof window.oGlobalConstants.sUserLoginName || window.oGlobalConstants.sUserLoginName === null) {
        setTimeout(function () {
            retrievePinListItems();
        }, 1000);
    } else {
        if (window.oGlobalConstants.sUserLoginName) {
            oGlobalConstants.clientContext = new SP.ClientContext(oCommonLinks.sCatalogSite);
            oGlobalConstants.oList = oGlobalConstants.clientContext.get_web().get_lists().getByTitle(oGlobalConstants.sListName);
            var camlQuery = new SP.CamlQuery();
            camlQuery.set_viewXml(oMatterLandingCommonObjects.pinListQuery.replace("@@Username", oGlobalConstants.sUserLoginName));
            oGlobalConstants.collListItem = oGlobalConstants.oList.getItems(camlQuery);
            oGlobalConstants.clientContext.load(oGlobalConstants.collListItem);
            oGlobalConstants.clientContext.executeQueryAsync(Function.createDelegate(this, function () { pinItemsSuccess(); }), Function.createDelegate(this, function () {
                onQueryFailed("RetrieveItems");
            }));
        } else {
            onQueryFailed(oGlobalConstants.sOperationPin);
        }
    }
}

// Success function when pin items are retrieved
function pinItemsSuccess() {
    "use strict";
    var iCount = 0, oListItem, oPinnedObject;
    var listItemEnumerator = oGlobalConstants.collListItem.getEnumerator();
    while (listItemEnumerator.moveNext()) {
        iCount++;
        oListItem = listItemEnumerator.get_current();
        oGlobalConstants.MatterPinnedDetails = oListItem.get_item(oGlobalConstants.sPinColumn);
    }
    if ("" !== oGlobalConstants.sOperationType) {
        if (0 === iCount) {
            if (oGlobalConstants.sOperationPin === oGlobalConstants.sOperationType) {
                oPinnedObject = getPinnedObject();
                createListItemAndPin(oPinnedObject);
            }
        } else {
            if (oGlobalConstants.sOperationPin === oGlobalConstants.sOperationType) {
                oPinnedObject = getPinnedObject();
                updateList(oPinnedObject);
            } else if (oGlobalConstants.sOperationUnpin === oGlobalConstants.sOperationType) {
                oPinnedObject = getPinnedObject();
                removeList(oPinnedObject);
            }
        }
    } else {
        setTextForPinOperation(oGlobalConstants.sOperationPin);
        $(".pinIcon .loadingIcon").addClass("hide");
        $("#PinMatter").removeClass("hide");
    }

    if (0 < iCount) {
        checkAlreadyPinned(oGlobalConstants.MatterPinnedDetails);
    }
}

// Get the pinned object
function getPinnedObject() {
    "use strict";
    var sMatterClientUrl = oMatterDetails.MatterClientUrl;
    sMatterClientUrl = sMatterClientUrl.substring(0, sMatterClientUrl.length - 1);
    var sPinnedObject = "\"" + oMatterDetails.MatterUrl + "\": {\n\t \"MatterName\": \"" +
                        oMatterDetails.MatterName + "\", \n\t \"MatterDescription\": \"" +
                        oMatterDetails.Description + "\", \n\t \"MatterCreatedDate\": \"" +
                        oMatterDetails.OpenDate + "\", \n\t \"MatterUrl\": \"" +
                        oMatterDetails.MatterUrl + "\", \n\t \"MatterPracticeGroup\": \"" +
                        oMatterDetails.PracticeGroup + "\", \n\t \"MatterAreaOfLaw\": \"" +
                        oMatterDetails.AreaOfLaw + "\", \n\t \"MatterSubAreaOfLaw\": \"" +
                        oMatterDetails.SubAreaOfLaw + "\", \n\t \"MatterClientUrl\": \"" +
                        sMatterClientUrl + "\", \n\t \"MatterClient\": \"" +
                        oMatterDetails.ClientName + "\", \n\t \"MatterClientId\": \"" +
                        oMatterDetails.ClientID + "\", \n\t \"MatterID\": \"" +
                        oMatterDetails.MatterID + "\", \n\t \"MatterModifiedDate\": \"" +
                        oMatterDetails.LastItemModifiedDate + "\", \n\t \"MatterResponsibleAttorney\": \"" +
                        oMatterDetails.ResponsibleAttorney + "\", \n\t \"MatterGUID\": \"" +
                        getMatterName() + "\", \n\t \"HideUpload\": \"";
    var sHideUpload = "true";
    var sBlockedUploadUsers = "";
    if (oMatterDetails.BlockedUploadUsers && oMatterDetails.BlockedUploadUsers.trim()) {
        sBlockedUploadUsers = oMatterDetails.BlockedUploadUsers.toString();
    }
    if ("" !== sBlockedUploadUsers) {
        if (-1 !== sBlockedUploadUsers.indexOf(oGlobalConstants.sCurrentUserTitle)) {
            sHideUpload = "false";
        }
    } else {
        sHideUpload = "false";
    }
    sPinnedObject += sHideUpload + "\" \n }";
    return sPinnedObject;
}

// Create new list item and pin the data
function createListItemAndPin(oPinnedObject) {
    "use strict";
    var clientContext = new SP.ClientContext(oCommonLinks.sCatalogSite);
    var oList = clientContext.get_web().get_lists().getByTitle(oGlobalConstants.sListName);
    var itemCreateInfo = new SP.ListItemCreationInformation();
    oGlobalConstants.oListItem = oList.addItem(itemCreateInfo);
    var oPinnedBody = "{" + oPinnedObject + "}";
    oGlobalConstants.oListItem.set_item(oGlobalConstants.sUserAliasColumn, oGlobalConstants.sUserLoginName);
    oGlobalConstants.oListItem.set_item(oGlobalConstants.sPinColumn, oPinnedBody);
    oGlobalConstants.oListItem.update();
    if (oElement) {
        setTextForPinOperation(oGlobalConstants.sOperationUnpin);
    }
    clientContext.load(oGlobalConstants.oListItem);
    clientContext.executeQueryAsync(Function.createDelegate(this, function () { onListItemCreated(oGlobalConstants.oListItem, clientContext); }), Function.createDelegate(this, function () {
        onQueryFailed(oGlobalConstants.sOperationPin);
    }));
}

// Update the value of pinned object inside the list for the current user
function updateList(oPinnedObject) {
    "use strict";
    var listItemEnumerator = oGlobalConstants.collListItem.getEnumerator(), oListItem;
    while (listItemEnumerator.moveNext()) {
        oListItem = listItemEnumerator.get_current();
        oGlobalConstants.MatterPinnedDetails = oListItem.get_item(oGlobalConstants.sPinColumn);
    }
    var pinnedMattersList = decodeURIComponent(oGlobalConstants.MatterPinnedDetails.toLowerCase());
    var matterUrl = decodeURIComponent(oMatterDetails.MatterUrl.toLowerCase());
    var iPresent = pinnedMattersList.search(matterUrl);
    if (0 <= iPresent) {
        setTextForPinOperation(oGlobalConstants.sOperationUnpin);
        $(".pinIcon .loadingIcon").addClass("hide");
        $("#PinMatter").removeClass("hide");
    } else {
        var valueIndex = oGlobalConstants.MatterPinnedDetails.lastIndexOf("}");
        oGlobalConstants.MatterPinnedDetails = oGlobalConstants.MatterPinnedDetails.substring(0, valueIndex);
        oGlobalConstants.MatterPinnedDetails += ", \n" + oPinnedObject + "\n }";
        oListItem.set_item(oGlobalConstants.sPinColumn, oGlobalConstants.MatterPinnedDetails);
        if (oElement) {
            setTextForPinOperation(oGlobalConstants.sOperationUnpin);
        }
        oListItem.update();
        oGlobalConstants.clientContext.executeQueryAsync(Function.createDelegate(this, function () { onListItemCreated(null, null); }), Function.createDelegate(this, function () {
            onQueryFailed(oGlobalConstants.sOperationPin);
        }));
    }
}

// Remove the list item after the last item has been deleted from the pinned object
function removeList(oPinnedObject) {
    "use strict";
    var listItemEnumerator = oGlobalConstants.collListItem.getEnumerator(), oListItem;
    while (listItemEnumerator.moveNext()) {
        oListItem = listItemEnumerator.get_current();
        oGlobalConstants.MatterPinnedDetails = oListItem.get_item(oGlobalConstants.sPinColumn);
    }
    var deletion = JSON.parse(oGlobalConstants.MatterPinnedDetails);
    var matterUrl = decodeURIComponent(oMatterDetails.MatterUrl.toLowerCase());
    $.each(deletion, function (key, val) {
        if (decodeURIComponent(key.toLowerCase()) === matterUrl) {
            delete deletion[key];
            return false;
        }
    });
    var newMatterPinnedDetails = JSON.stringify(deletion);
    oListItem.set_item(oGlobalConstants.sPinColumn, newMatterPinnedDetails);
    oListItem.update();
    if (oElement) {
        setTextForPinOperation(oGlobalConstants.sOperationPin);
    }
    var iCountValue = 0;
    for (var key in deletion) {
        if (deletion.hasOwnProperty(key)) {
            iCountValue++;
        }
    }
    if (0 === iCountValue) {
        oListItem.deleteObject();
    }
    oGlobalConstants.clientContext.executeQueryAsync(Function.createDelegate(this, function () { onListItemDeleted(); }), Function.createDelegate(this, function () {
        onQueryFailed(oGlobalConstants.sOperationUnpin);
    }));
}

// Function to be called on list item deleted
function onListItemDeleted() {
    "use strict";
    if (oElement) {
        setTextForPinOperation(oGlobalConstants.sOperationPin);
        $(".pinIcon .loadingIcon").addClass("hide");
        $("#PinMatter").removeClass("hide");
    }
}

/* Function to execute on success of breaking the permission of list item */
function onBreakPermissionSuccess(objListItem) {
    "use strict";
    if (oElement) {
        setTextForPinOperation(oGlobalConstants.sOperationUnpin);
        $(".pinIcon .loadingIcon").addClass("hide");
        $("#PinMatter").removeClass("hide");
    }
};

// Function called on list item created
function onListItemCreated(oListItem, clientContext) {
    "use strict";
    if (!oListItem && !clientContext) {
        if (oElement) {
            setTextForPinOperation(oGlobalConstants.sOperationUnpin);
            $(".pinIcon .loadingIcon").addClass("hide");
            $("#PinMatter").removeClass("hide");
        }
    } else {
        var objListItem = oListItem;
        var collRoleDefinitionBinding = SP.RoleDefinitionBindingCollection.newObject(clientContext);
        collRoleDefinitionBinding.add(clientContext.get_web().get_roleDefinitions().getByType(SP.RoleType.reader));
        var oUser = clientContext.get_web().ensureUser(_spPageContextInfo.userLoginName);
        objListItem.breakRoleInheritance(false);
        objListItem.get_roleAssignments().add(oUser, collRoleDefinitionBinding);
        objListItem.update();
        clientContext.load(oUser);
        clientContext.load(objListItem);
        clientContext.executeQueryAsync(function () { onBreakPermissionSuccess(objListItem); }, function () {
            onQueryFailed(oGlobalConstants.sOperationPin);
        });
    }
}

// Set text for the pin un pin operation
function setTextForPinOperation(sOperationType) {
    "use strict";
    var oImgElements = $("#PinMatter img");
    var arrImgSrc = [];
    var iterator = 0, iCount = oImgElements.length;
    for (iterator = 0; iterator < iCount; iterator++) {
        arrImgSrc.push(oImgElements[iterator].src);
    }
    $("#PinMatter").attr("title", sOperationType);
    var sHtml = "<img src='" + arrImgSrc[0] + "' alt='Pin' class=\"hide pinImg\"><img src='" + arrImgSrc[1] + "' class=\"hide unPinImg\"  alt='Unpin'>";
    $("#PinMatter").html(sHtml);
    if (sOperationType === oGlobalConstants.sOperationPin) {
        $(".unPinImg").addClass("hide");
        $(".pinImg").removeClass("hide");
    } else {
        $(".pinImg").addClass("hide");
        $(".unPinImg").removeClass("hide");
    }
}

// Make a call based on the title of the icon
function pinUnpinMatter(event) {
    "use strict";
    $(".pinIcon .loadingIcon").removeClass("hide");
    $("#PinMatter").addClass("hide");
   // if( $(".pinImg").cl)
      //  $(".pinImg").addClass("hide");
  // $(".unPinImg").removeClass("hide");

    if (oElement) {
        if (oGlobalConstants.sOperationPin === $(oElement).attr("title")) {
            LogEvent(appInsightsMatterLandingText + appInsightsOperations.Pin);
            pinMatter();
        } else {
            LogEvent(appInsightsMatterLandingText + appInsightsOperations.Unpin);
            unPinMatter();
        }
    }
    event && event.stopPropagation();
}

// Make a call to pin the matter
function pinMatter() {
    "use strict";
    oGlobalConstants.sOperationType = oGlobalConstants.sOperationPin;
    retrievePinListItems();
}

// Make a call to unpin the matter
function unPinMatter() {
    "use strict";
    oGlobalConstants.sOperationType = oGlobalConstants.sOperationUnpin;
    retrievePinListItems();
}

// Check if matter is already pinned
function checkAlreadyPinned(sMatterDetails) {
    "use strict";
    var pinnedMattersList = decodeURIComponent(sMatterDetails.toLowerCase());
    var matterUrl = decodeURIComponent(oMatterDetails.MatterUrl.toLowerCase());
    var iPresent = pinnedMattersList.search(matterUrl);
    if (oElement) {
        if (0 > iPresent) {
            setTextForPinOperation(oGlobalConstants.sOperationPin);
        } else {
            setTextForPinOperation(oGlobalConstants.sOperationUnpin);
        }
    }
}

// Check if one note exists
function urlExists() {
    "use strict";
    // Get the link from the data-href attribute
    var oOneNoteLink = $("#oneNotePane"), arrOneNoteURL;
    var sDocumentLibraryOriginalName = getMatterName();
    var sOneNoteURL = _spPageContextInfo.siteServerRelativeUrl + oGlobalConstants.wopiFrameUrl + _spPageContextInfo.siteServerRelativeUrl + "/" + sDocumentLibraryOriginalName + libraryNameSuffix.oneNoteSuffix + "/" + documentLibraryName + "/" + sDocumentLibraryOriginalName + oGlobalConstants.oneNoteExtension;
    if (sOneNoteURL && "" !== sOneNoteURL) {
        arrOneNoteURL = sOneNoteURL.split("sourcedoc=");
        if (arrOneNoteURL.length && arrOneNoteURL[1]) {
            // AJAX Call to check if the item exists
            $.ajax({
                url: arrOneNoteURL[1],
                error: function () {
                    // Failure - Hide the link
                    oOneNoteLink.addClass("hide");
                    adjustFooter();
                },
                success: function () {
                    // Success - Show the link and update the href
                    oGlobalConstants.sEventName = oGlobalConstants.Go_To_OneNote;
                    $(oOneNoteLink).empty();
                    $(oOneNoteLink).html(oMatterLandingHtmlConstants.oneNoteHtml.replace("@@OneNoteLink", sOneNoteURL).replace("@@OneNoteIcon", oCommonLinks.sCatalogSite + oMatterLandingCommonObjects.oMatterCenterAssetsLocation + oMatterLandingAssetsLocation.oneNoteIcon));
                    adjustFooter();
                },
            });
        } else {
            // Hide the icon
            oOneNoteLink.addClass("hide");
        }
    } else {
        // Hide the icon
        oOneNoteLink.addClass("hide");
    }
}

/* Log events in application insights */
// Function to log application insight event
function LogEvent(eventName) {
    "use strict";
    var appInsights = initializeApplicationInsights();
    appInsights.trackEvent(eventName);
}