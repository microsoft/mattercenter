/// <disable>JS1003,JS2076,JS2032,JS2064,JS2074,JS2024,JS2026,JS2005,JS3085,JS3116,JS3092,JS3057,JS3058,JS3056</disable>
/* Common file to hold all the common JavaScript functions */

var oServiceGlobal = {
    oAjaxRequest: null
};

var oCommonObject = (function () {
    "use strict";
    var commonConstants = {
        sProvisioningServiceLocation: oGlobalConstants.Matter_Provision_Service_Url,  //// URL for dev site (Development purpose)
        sSearchServiceLocation: oGlobalConstants.Search_Service_Url,  //// URL for dev site (Development purpose)
        sLegalBriefcaseLocation: oGlobalConstants.Legal_Briefcase_Service_Url,
        sServiceLocation: "",
        oUsers: [], //// Array which will holds the JSON response of users's request
        oSiteUser: [], //// Array which will hold all the set of valid resolved users
        isMatterView: true,
        iCurrentGridViewData: 2, //// Represents current data that grid view is holding on page load (All Matters/Documents: 1, My Matters/Documents: 2, Recent Matters/Documents: 3, Pinned Matters/Documents: 4)
        userPinnedData: [],
        pinnedData: [],
        pinnedFilterData: [],
        arrUniqueRefinerData: [],
        isServiceCallComplete: true,
        sCurrentPage: oGlobalConstants.Home_page,
        iColumnDataTotalResults: 0,
        iFilterFlyoutPageNumber: 1,
        iTypingTimer: 0,////timer identifier
        isSearchText: 0,
        isAllRowSelected: false,
        isFilterFlyoutVisible: 0,
        oHeaderFilterType: [],
        oSortDetails: {
            ByProperty: "",
            Direction: 0
        },
        arrCurrentRefiner: [],
        valueSeperator: "; ",
        bHideAutoComplete: false,
        iDocumentDataCounter: 0,
        sSearchedKeyword: "",
        sFilterDetails: "",
        bCalledForSort: false,
        sDocumentFullName: "",
        /* Wrapper function to call specified method of the service with specified parameters, success and failure functions */
        callService: function (sWebMethodName, oParameters, onSuccess, onFailure, onBeforeSend, oParam) {
            var sWebMethodUrl = commonConstants.sServiceLocation + sWebMethodName
            , sData = JSON.stringify(oParameters);
            oServiceGlobal.oAjaxRequest = $.ajax({
                type: "POST",
                url: sWebMethodUrl,
                data: sData,
                contentType: "application/json; charset=utf-8",
                headers: { "RequestValidationToken": oMasterGlobal.Tokens }, // Get the request validation token from the master page
                success: function (result) {
                    if ("function" === typeof (onSuccess)) {
                        if (oParam) {
                            var oFinalResult = { "Result": result, "oParam": oParam };
                            onSuccess(oFinalResult);
                        } else {
                            onSuccess(result);
                        }
                    };
                },
                beforeSend: function (result) {
                    commonConstants.logAppInsight(sWebMethodName);
                    if ("function" === typeof (onBeforeSend)) {
                        if (oParam) {
                            var oFinalResult = { "Result": result, "oParam": oParam };
                            onBeforeSend(oFinalResult);
                        } else {
                            onBeforeSend(result);
                        }
                    }
                },
                error: function (result) {
                    if ("function" === typeof (onFailure)) {
                        if (oParam) {
                            var oFinalResult = { "Result": result, "oParam": oParam };
                            onFailure(oFinalResult);
                        } else {
                            onFailure(result);
                        }
                    }
                }
            });
        },

        /* Function to log to app insight */
        logAppInsight: function (eventName) {
            if (appInsights) {
                switch (eventName) {
                    case "PinDocumentForUser":
                        eventName = oGlobalConstants.Pin_Document;
                        break;
                    case "RemovePinnedDocument":
                        eventName = oGlobalConstants.Unpin_Document;
                        break;
                    case "PinMatterForUser":
                        eventName = oGlobalConstants.Pin_Matter;
                        break;
                    case "RemovePinnedMatter":
                        eventName = oGlobalConstants.Unpin_Matter;
                        break;
                    case "SendToBriefcase":
                        eventName = oGlobalConstants.Send_to_OneDrive;
                        break;
                    case "UploadMail", "UploadAttachment":
                        eventName = oGlobalConstants.Mail_Attachment_Upload;
                        break;
                    case "CreateMatter":
                        eventName = oGlobalConstants.Create_Matter;
                        break;
                    default:
                        eventName = "";
                        break;
                }
                if (eventName) {
                    commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + eventName, true);
                }
            }
        },
        highlightTerm: function (sResult, sTerm) {
            if (sResult && sTerm && "" !== sResult && "" !== sTerm) {
                var termTemplate = "<span class='highlightTerm'>" + sTerm + "</span>",
                    escapeChars = ["(", ")", "+"];
                //// Replace the term with the highlighted version
                // This will match all instances of the corresponding term, irrespective of case
                escapeChars.forEach(function (char) { sTerm = sTerm.replace(char, "\\" + char); });
                var sTermRegex = new RegExp(sTerm, "ig");
                sResult = sResult.replace(sTermRegex, termTemplate);
            }
            return sResult;
        },
        formatSearchText: function (sSearchTerm, sRefinerString) {
            // Search Matters
            var sRefinerName, sClientRefiner;
            if (oGlobalConstants.Matter_ID === sRefinerString) {
                sRefinerName = oGlobalConstants.Matter_Name;
                sClientRefiner = oGlobalConstants.Client_Name;
            } else {
                sRefinerName = oGlobalConstants.File_Name;
                sClientRefiner = oGlobalConstants.Document_Client_Name;
            }
            // Format is Title (MatterID)
            if (-1 !== sSearchTerm.indexOf("(")) {
                // Separate the title from the ID
                var sTitle, sMatterID;
                // Check if Document ID exist for the Document
                if (commonConstants.sDocumentFullName && null !== commonConstants.sDocumentFullName && "" !== commonConstants.sDocumentFullName) {
                    sSearchTerm = commonConstants.sDocumentFullName;
                }

                if ("null" !== oCommonObject.sDocID) {
                    sTitle = sSearchTerm.substring(0, sSearchTerm.lastIndexOf("(")); // To take full document name in the case that document name contains parenthesis e.g. 'xyz (2)'
                    sMatterID = sSearchTerm.substring(sSearchTerm.lastIndexOf("("));
                } else {
                    sTitle = sSearchTerm; // If no document name exist then assign the sTitle only to sSearchTerm
                }

                // Check if the string is in the correct format, otherwise do not format
                if (sTitle) {
                    sTitle = sTitle.trim(); // Removal of White Space
                    if (sTitle && sMatterID) {
                        sMatterID = sMatterID.replace(/([()])/g, "").trim(); // Removal of all brackets from the ID
                        sSearchTerm = "(" + sRefinerName + ":\"" + sTitle + "\" AND " + sRefinerString + ":\"" + sMatterID + "\")";
                    } else {
                        sSearchTerm = "(" + sRefinerName + ":\"" + sTitle + "\")"; // Set search term if document id doesn't exist for particular document
                    }
                }
            } else {
                if (-1 !== sSearchTerm.indexOf(":")) {
                    var arrTerm = sSearchTerm.split(":"), sClientID = "", sMatterID = "", sClientName = "", sMatterName = "", sManagedProperty = "", sModifiedDate = "", sCreatedDate = "";
                    if (arrTerm.length && 2 === arrTerm.length && arrTerm[0] && arrTerm[1]) {
                        // Check for the Existence of ClientID: Search Term
                        if (oGlobalConstants.Client_Custom_Properties_Id.toLowerCase() === arrTerm[0].toLowerCase()) {
                            sClientID = arrTerm[1].trim(); // Removal of White Space
                            var sPropertyName = oCommonObject.isMatterView ? oGlobalConstants.Matter_Client_Custom_Properties_Id : oGlobalConstants.Document_Client_ID;
                            sSearchTerm = "(" + sPropertyName + ":" + sClientID + "*)"; // Replace ClientID with MatterClientID alias
                        } else if (oGlobalConstants.Matter_ID_Alias.toLowerCase() === arrTerm[0].toLowerCase()) {
                            sMatterID = arrTerm[1].trim(); // Removal of White Space
                            var sPropertyName = oCommonObject.isMatterView ? oGlobalConstants.Matter_ID : oGlobalConstants.Document_Matter_ID;
                            sSearchTerm = "(" + sPropertyName + ":" + sMatterID + "*)"; // Replace MatterID with respective managed property
                        } else if (oGlobalConstants.Client_Name_Search_Term.toLowerCase() === arrTerm[0].toLowerCase()) {
                            sClientName = arrTerm[1].trim(); // Removal of White Space
                            var sPropertyName = oCommonObject.isMatterView ? oGlobalConstants.Client_Name : oGlobalConstants.Document_Client_Name;
                            sSearchTerm = "(" + sPropertyName + ":" + sClientName + "*)"; // Replace ClientName with respective managed property
                        } else if (oGlobalConstants.Matter_Name_Search_Term.toLowerCase() === arrTerm[0].toLowerCase()) {
                            sMatterName = arrTerm[1].trim(); // Removal of White Space
                            var sPropertyName = oCommonObject.isMatterView ? oGlobalConstants.Matter_Name : oGlobalConstants.Document_Matter_Name;
                            sSearchTerm = "(" + sPropertyName + ":" + sMatterName + "*)"; // Replace MatterName with respective managed property
                        } else if (oGlobalConstants.Document_Last_Modified_Time.toLowerCase() === arrTerm[0].toLowerCase() || oGlobalConstants.Last_Modified_Time.toLowerCase() === arrTerm[0].toLowerCase()) {
                            sModifiedDate = arrTerm[1].trim(); // Removal of White Space
                            var sPropertyName = oCommonObject.isMatterView ? oGlobalConstants.Last_Modified_Time : oGlobalConstants.Document_Last_Modified_Time;
                            sSearchTerm = "(" + sPropertyName + ":" + sModifiedDate + ")"; // Replace Modified date with respective managed property without * sign

                        } else if (oGlobalConstants.Created_Date.toLowerCase() === arrTerm[0].toLowerCase() || oGlobalConstants.Open_Date.toLowerCase() === arrTerm[0].toLowerCase()) {
                            sCreatedDate = arrTerm[1].trim(); // Removal of White Space
                            var sPropertyName = oCommonObject.isMatterView ? oGlobalConstants.Open_Date : oGlobalConstants.Created_Date;
                            sSearchTerm = "(" + sPropertyName + ":" + sCreatedDate + ")"; // Replace Created Date with respective managed property without * sign

                        } else { // Managed properties search even if full text is not typed
                            sManagedProperty = arrTerm[1].trim(); // Removal of White Space
                            var sPropertyName = arrTerm[0].trim(); // Removal of White Space
                            sSearchTerm = "(" + sPropertyName + ":\"" + sManagedProperty + "*\")";
                        }
                    }
                } else {
                    if (!sSearchTerm || sSearchTerm.length <= 0) {
                        sSearchTerm = "*";
                    } else {
                        sSearchTerm = "\"" + sSearchTerm + "*\"";
                    }
                    if ((oGridConfig && !oGridConfig.isMatterView && oCommonObject.bHideAutoComplete) || (!oGridConfig.inWebDashboard && !oCommonObject.isMatterView)) {
                        sSearchTerm = "(" + sSearchTerm + " OR " + sRefinerName + ":" + sSearchTerm + " OR " + sRefinerString + ":" + sSearchTerm + " OR " + sClientRefiner + ":" + sSearchTerm + ")"; // Adding search term to support free text search
                    } else {
                        sSearchTerm = "(" + sRefinerName + ":" + sSearchTerm + " OR " + sRefinerString + ":" + sSearchTerm + " OR " + sClientRefiner + ":" + sSearchTerm + ")";
                    }
                }
            }
            return sSearchTerm;
        },
        spClientCall: function (FilterDetails) {
            if (oSearchGlobal.sClientName && "" !== oSearchGlobal.sClientName && "" !== oSearchGlobal.sClientSiteUrl) {
                if ("" === FilterDetails.ClientsList[0]) {
                    FilterDetails.ClientsList = [];
                }
                FilterDetails.ClientsList.push(oSearchGlobal.sClientSiteUrl);
            }
        },
        getRecentDocuments: function (container, pageNumber, isPage, source, event) {
            var sSearchTerm = $(".filterSearchText").val() && $(".filterSearchText").val() !== "" ? oCommonObject.formatSearchText($(".filterSearchText").val().trim(), oGlobalConstants.Document_ID) : "", FilterDetails;
            if (source) {
                FilterDetails = oSearchGlobal.oFilterData;
                sSearchTerm = oSearchGlobal.sSearchTerm;
            } else {
                FilterDetails = oCommonObject.getSearchData(event);
            }

            commonConstants.spClientCall(FilterDetails);
            // Get the current users data always
            FilterDetails.FilterByMe = 1;
            var oSortDetails = getSortData();

            var oItemPerPage = null, sFunBeforeSearch = null, sFunOnSuccess = null, sFunOnFailure = null, oParam = null;
            if (oGridConfig.inWebDashboard) {
                // WebDashboard
                itemsCountsToBeDisplayed();
                oItemPerPage = oGridConfig.itemsPerPage;
                sFunOnSuccess = onSearchSuccess;
                sFunOnFailure = onSearchFailure;
                oParam = { "container": container, "isPage": isPage, "pageNumber": pageNumber, "itemsPerPage": oSearchGlobal.itemsPerPage };
            } else {
                // FindDocument
                oItemPerPage = oSearchGlobal.itemsPerPage;
                sFunOnSuccess = onSuccess;
                sFunOnFailure = onFailure;
                oParam = { "container": container, "isPage": isPage, "pageNumber": pageNumber };
            }
            var SearchDetails = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oCommonObject.getDeployedUrl() }, "searchObject": { "PageNumber": pageNumber, "ItemsPerPage": oItemPerPage, "SearchTerm": sSearchTerm, "Filters": FilterDetails, "Sort": oSortDetails } };
            oCommonObject.callSearchService("FindDocument", SearchDetails, sFunOnSuccess, sFunOnFailure, beforeSearch, oParam);
        },
        getRecentMatters: function (container, pageNumber, isPage, source, event) {
            var ofilterSearchText = $(".filterSearchText").val();
            var sSearchTerm = ofilterSearchText && ofilterSearchText !== "" ? ofilterSearchText : "", FilterDetails, oFilterDetails = { ClientsList: [], PGList: "", AOLList: "", FromDate: "", ToDate: "" }, oParam, SearchDetails;
            oSearchGlobal.clientDataLoaded = false;
            if (source) {
                FilterDetails = oSearchGlobal.oFilterData;
                sSearchTerm = oSearchGlobal.sSearchTerm;
            } else {
                FilterDetails = oCommonObject.getSearchData(event);
            }

            commonConstants.spClientCall(FilterDetails);
            if (FilterDetails.ClientsList.length && !FilterDetails.ClientsList[0]) {
                FilterDetails.ClientsList = oFilterDetails.ClientsList.join(",").split(",");
            }
            FilterDetails.FilterByMe = 1; // Get the current users data always
            var oSortDetails = getSortData();
            var oItemPerPage = null, sFunBeforeSearch = null;
            if (oGridConfig.inWebDashboard) { // FindMatter
                itemsCountsToBeDisplayed();
                oItemPerPage = oGridConfig.itemsPerPage;
                sFunBeforeSearch = beforeSearch;
            } else { // WebDashboard
                oItemPerPage = oSearchGlobal.itemsPerPage;
                sFunBeforeSearch = beforeRequestMatter;
            }

            oParam = {
                "container": container, "tileContainer": "tileContainer", "pageNumber": pageNumber, "itemsPerPage": oSearchGlobal.itemsPerPage, "fromPagination": true
            };

            SearchDetails = {
                "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oCommonObject.getDeployedUrl() }, "searchObject": { "PageNumber": pageNumber, "ItemsPerPage": oItemPerPage, "SearchTerm": sSearchTerm, "Filters": FilterDetails, "Sort": oSortDetails }, "isDocument": false
            };
            oCommonObject.callSearchService("FindMatter", SearchDetails, onSearchSuccess, onSearchFailure, sFunBeforeSearch, oParam);
        },
        // Function to get URL on which app is deployed
        getDeployedUrl: function () {
            return oGlobalConstants.Central_Repository_Url;
        },
        // Function to get the appSwitcher from Master Page to Current Page
        getAppSwitcher: function (sAppName) {
            var oCommonAppSwitch = $(".commonAppSwitch"), oLocalAppSwicther = $(".appSwitch");
            if (oCommonAppSwitch && oLocalAppSwicther) {
                var appSwitcherHTML = oCommonAppSwitch.html();
                if (appSwitcherHTML) {
                    oLocalAppSwicther.html(appSwitcherHTML);
                    oCommonAppSwitch.empty();
                    oLocalAppSwicther.find(".appSwitcherName") ? oLocalAppSwicther.find(".appSwitcherName span").text(sAppName) : "";
                }
            }
        },
        // Encode special HTML characters, so text is safe when building HTML dynamically        
        renderAsText: function renderAsText(text) {
            var renderedText = "";
            if ("string" === typeof text) {
                // Search for HTML special characters, convert to HTML entities
                renderedText = $("<div/>").text(text).html();
            }
            return renderedText;

        },
        // Function to get all refiners data
        getSearchData: function getSearchData(event) {
            var sSearchTerm, sClientSelection, sPGSelection, sAOLSelection, fromDate, toDate, boolIsDateValid, sAuthor;

            // Get the Search Term if Any.
            // Get the Client Name from the selected Value if Any.

            sSearchTerm = $(".filterSearchText").val() && $(".filterSearchText").val() !== "" ? oCommonObject.formatSearchText($(".filterSearchText").val().trim(), oGlobalConstants.Document_ID) : "";
            sClientSelection = ($(".filterSearchRefinerText.refinerClientsText").attr("data-selected") && $(".filterSearchRefinerText.refinerClientsText").attr("data-selected") !== "" ? $(".filterSearchRefinerText.refinerClientsText").attr("data-selected") : "").split("$|$");

            fromDate = $("#refinerFromText").val();
            toDate = $("#refinerToText").val();

            boolIsDateValid = oCommonObject.checkDateValid(fromDate, toDate);
            if (!boolIsDateValid) {
                $("#refinerDateError").show();
            } else {
                $("#refinerDateError").hide();

                var oFilterPanelDetails = oGridConfig.isMatterView ? { ClientsList: [], PGList: [""], AOLList: [""], FromDate: "", ToDate: "", FilterByMe: "" } : {
                    ClientsList: [], FromDate: "", ToDate: "", DocumentAuthor: "", FilterByMe: ""
                };
                if (oGridConfig.isMatterView) {
                    var oPGList = $(".filterSearchRefinerText.refinerPGText").attr("data-selected"),
                        AOLList = $(".filterSearchRefinerText.refinerAOLText").attr("data-selected");
                    oFilterPanelDetails.PGList = (oPGList && oPGList !== "" ? oPGList : "").split("$|$");
                    oFilterPanelDetails.AOLList = (AOLList && AOLList !== "" ? AOLList : "").split("$|$");
                } else {
                    sAuthor = $("#txtDocumentAuthor").val();
                    sAuthor = trimEndChar(sAuthor.trim(), ";");
                    oFilterPanelDetails.DocumentAuthor = sAuthor;
                }
                // Append time to "From" part of date if not null
                if ($.trim(fromDate)) {
                    fromDate += oGlobalConstants.From_Date_Append_Time;
                }
                // Append time to "To" part of date if not null
                if ($.trim(toDate)) {
                    toDate += oGlobalConstants.To_Date_Append_Time;
                }
                oFilterPanelDetails.ClientsList = sClientSelection;
                oFilterPanelDetails.FromDate = fromDate;
                oFilterPanelDetails.ToDate = toDate;
                oFilterPanelDetails.FilterByMe = oSearchGlobal.searchOption;
                closeAllPopupExcept("filterAutoComplete", event);
                return oFilterPanelDetails;
            }
        },

        //// Function to check type of the app based on URL
        getParameterByName: function (sParamterName) {
            sParamterName = sParamterName.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var oRegex = new RegExp("[\\?&]" + sParamterName + "=([^&#]*)", "i"),
                oResults = oRegex.exec(location.search);
            return oResults === null ? "" : decodeURIComponent(oResults[1].replace(/\+/g, " "));
        },

        // Function to validate to date and from date
        checkDateValid: function checkDateValid(fromDateText, toDateText) {
            if (fromDateText && toDateText) {
                var fromDate = new Date(fromDateText.replace(/-/g, "/"));
                var toDate = new Date(toDateText.replace(/-/g, "/"));
                if (fromDate <= toDate) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return true;
            }
        },
        //// Extract file title from file name. E.g. input: "abc.txt", Output: "abc"
        ExtractFileTitle: function ExtractFileName(fileNameWithExtn) {
            var nExtnPosition = fileNameWithExtn ? fileNameWithExtn.lastIndexOf(".") : fileNameWithExtn;
            return -1 < nExtnPosition && fileNameWithExtn ? fileNameWithExtn.substring(0, nExtnPosition) : fileNameWithExtn;
        },
        // App Insight Event tracking function
        logEvent: function (eventName) {
            if (appInsights && eventName) {
                appInsights.trackEvent(eventName);
            }
        },
        // Update CSS for success notification
        updateNotificationPosition: function () {
            "use strict";
            var oSuccessNotification = $(".successNotification");
            var oNotificationContainer = oGridConfig.inWebDashboard ? $(".notificationContainerForMailPopup") : $(".notificationContainerForPopup");
            if (oNotificationContainer.children().length && oSuccessNotification.length) {
                oSuccessNotification.remove();
            }
        },

        // Display Success notification
        showNotification: function (sMsg, resultClass) {
            "use strict";
            // Check if success notification is not present; then only display notification
            var oNotificationContainer = oGridConfig.inWebDashboard ? $(".notificationContainerForMailPopup") : $(".notificationContainerForPopup");
            var nIsSuccessNotificationPresent = $(".successNotification").length;
            if (!nIsSuccessNotificationPresent) {
                var sContent = "";
                sContent = "<div class='notification " + resultClass + "'>" + sMsg + "<span class='closeNotification'>x</span></div>";
                if (0 === oNotificationContainer.children().length) {   // Increase height only when overwrite notification is not present
                    $(".mailContainer").height($(".mailContainer").height() + 30);  // Adjusting the height of the popup, post removing notification    
                }
                $(".mailContainer").prepend(sContent);
                oCommonObject.updateNotificationPosition();
            }
        },

        // Function to overwrite local files 
        localOverWriteDocument: function (oDocument) {
            var $askForOverwrite = $(".askForOverwrite"),
                overWriteYes = $askForOverwrite.find("#overWriteYes")[0],
                overWriteNo = $askForOverwrite.find("#overWriteNo")[0],
                overWriteAppend = $askForOverwrite.find("#overWriteAppend")[0],
                overWriteContentCheck = $askForOverwrite.find("#contentCheck")[0];

            if ("undefined" !== typeof overWriteYes) {
                overWriteYes.disabled = true;
            }
            if ("undefined" !== typeof overWriteNo) {
                overWriteNo.disabled = true;
            }
            if ("undefined" !== typeof overWriteAppend) {
                overWriteAppend.disabled = true;
            }
            if ("undefined" !== typeof overWriteContentCheck) {
                overWriteContentCheck.disabled = true;
            }

            if ("overWriteNo" === $(oDocument).attr("id")) {
                if ("undefined" !== typeof overWriteYes) {
                    overWriteYes.disabled = false;
                }
                if ("undefined" !== typeof overWriteNo) {
                    overWriteNo.disabled = false;
                }
                if ("undefined" !== typeof overWriteAppend) {
                    overWriteAppend.disabled = false;
                }
                if ("undefined" !== typeof overWriteContentCheck) {
                    overWriteContentCheck.disabled = false;
                }
                oUploadGlobal.sNotificationMsg && (oUploadGlobal.sNotificationMsg = "");
                $(oDocument).parents(".notification").remove();
            }

            var sOperation = $(oDocument).attr("data-operation");

            if ("contentCheck" === sOperation) {
                oSearchGlobal.files = [oUploadGlobal.arrFiles[oUploadGlobal.arrFiles.length - 1]];
            } else {
                if (oGridConfig.inWebDashboard) {
                    oSearchGlobal.oFileArray = [oUploadGlobal.arrFiles.pop()];
                } else {
                    oSearchGlobal.files = [oUploadGlobal.arrFiles.pop()];
                }
            }

            var nOperation = "";
            if ("ignore" !== sOperation) {
                switch (sOperation) {
                    case "overwrite":
                        nOperation = "0";
                        break;
                    case "append":
                        nOperation = "1";
                        break;
                    case "contentCheck":
                        nOperation = "2";
                        break;
                    case "cancelContentCheck":
                        nOperation = "3";
                        break;
                }
                uploadFile(oUploadGlobal.sClientRelativeUrl, oUploadGlobal.sFolderUrl, nOperation);
            }
            if (0 < $(".notification").length) {
                if ("undefined" !== typeof overWriteYes) {
                    overWriteYes.disabled = false;
                }
                if ("undefined" !== typeof overWriteNo) {
                    overWriteNo.disabled = false;
                }
                if ("undefined" !== typeof overWriteAppend) {
                    overWriteAppend.disabled = false;
                }
                if ("undefined" !== typeof overWriteContentCheck) {
                    overWriteContentCheck.disabled = false;
                }
                $(oDocument).parents(".notification").remove();
            }
            //// Dynamically decrease the height of the popup
            oCommonObject.updateUploadPopupHeight(false);
            oCommonObject.updateNotificationPosition();
        },

        // Function to configure time stamp
        overwriteConfiguration: function (fileName) {
            // Update the content as per the logic.
            var selectedOverwriteConfiguration = oGlobalConstants.Overwrite_Config_Property.trim().toLocaleUpperCase(),
                fileExtension = fileName.trim().substring(fileName.trim().lastIndexOf(".") + 1),
                bAppendEnabled = false;

            switch (selectedOverwriteConfiguration) {
                case "BOTH":
                    bAppendEnabled = true;
                    break;
                case "DOCUMENT ONLY":
                    bAppendEnabled = "eml" === fileExtension || "msg" === fileExtension ? false : true;
                    break;
                default:
                    bAppendEnabled = "eml" === fileExtension || "msg" === fileExtension ? true : false;
                    break;
            }
            return bAppendEnabled;
        },
        // Increase and decrease the mail cart popup container
        updateUploadPopupHeight: function (increaseHeight) {
            var oNotificationContainer = oGridConfig.inWebDashboard ? $(".mailContainer >.notificationContainerForMailPopup") : $(".notificationContainerForPopup"),
                mailContainer = $(".mailContainer"),
                overWriteNotification = $(".notification.warningNotification");
            if (increaseHeight) {
                // Dynamically increase the height of the popup
                if (!overWriteNotification.length) {
                    oNotificationContainer.css("height", oNotificationContainer.height() + 80);
                    oGridConfig.inWebDashboard ? mailContainer.css({ "height": mailContainer.height() + 80 }).addClass("placePopupWithHeight") : mailContainer.css("height", mailContainer.height() + 68);
                }
            } else {
                // Dynamically decrease the height of the popup
                if (!overWriteNotification.length || (0 === overWriteNotification.length && !$('oNotificationContainer[style*="height"]').length && !$('mailContainer[style*="height"]').length)) {
                    oNotificationContainer.css("height", "");
                    oGridConfig.inWebDashboard ? mailContainer.css({ "height": "" }).addClass("placePopup") : mailContainer.css("height", "");
                }
            }
        },
        // Function to bind the jQuery UI auto-complete to the jQuery object of passed selector
        bindAutocomplete: function (sSelector, bIsMultiUser) {
            var $AutoComplete = $(sSelector);
            if ($AutoComplete) {
                $AutoComplete.autocomplete({
                    minLength: oGlobalConstants.Minlength_For_PeoplePicker, //// Minimum length of string for which the service request will be sent to fetch the users
                    source: function (request, response) {
                        var $SelectedObject = $(sSelector); //// Need to get the jQuery object again as on typing, the $AutoComplete object is out of scope
                        var sSearchTerm = (bIsMultiUser) ? $.trim($(request.term.split(";")).last()[0]) : $.trim(request.term); //// If multi-user text box, extract the last term for filtering
                        $(".ui-autocomplete").hide();
                        var matterDetails = {
                            "requestObject": {
                                "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
                            }, "client": {
                                "Url": oGlobalConstants.Central_Repository_Url
                            }, "searchObject": {
                                "SearchTerm": sSearchTerm
                            }
                        };
                        if (sSearchTerm) { //// If user has entered something apart from white space
                            $.ajax({
                                type: "POST",
                                url: oGlobalConstants.Matter_Provision_Service_Url + "GetUsers",
                                data: JSON.stringify(matterDetails),
                                contentType: "application/json; charset=utf-8",
                                headers: {
                                    "RequestValidationToken": oMasterGlobal.Tokens
                                },
                                success: function (result) {
                                    commonConstants.oUsers = JSON.parse(result);
                                    /* Store valid site users in array for further use */
                                    $.each(commonConstants.oUsers, function (key, item) {
                                        item.Email && commonConstants.oSiteUser.push(item.Email);
                                    });
                                    if (!commonConstants.oUsers.code) {
                                        var sConflictIdentified = "undefined" !== typeof (oMatterProvisionObject) ? oMatterProvisionObject.sConflictScenario() : "undefined" !== typeof (oManagePermissionsOnMatter) ? oManagePermissionsOnMatter.sConflictScenario() : "False";
                                        /* Generate the JSON object that will be passed to the auto complete function */
                                        var arrUsers = $.map(commonConstants.oUsers, function (item) {
                                            if ("True" === sConflictIdentified) {
                                                if ("User" === item.EntityType) {
                                                    return {
                                                        label: item.Name,
                                                        Email: item.Email,
                                                        OriginalEmail: item.EntityData.Email
                                                    };
                                                }
                                            } else {
                                                return {
                                                    label: item.Name,
                                                    Email: item.Email,
                                                    OriginalEmail: item.EntityData.Email
                                                };
                                            }
                                        });
                                        if (!arrUsers.length) {
                                            var arrToPush = [];
                                            arrToPush.label = oGlobalConstants.People_Picker_No_Results;
                                            arrToPush.Email = "";
                                            arrUsers.push(arrToPush);
                                        }
                                        response(arrUsers);
                                    } else {
                                        if (!(oMatterProvisionObject) || (commonConstants.oUsers.code && "string" === typeof commonConstants.oUsers.code && (-1 < commonConstants.oUsers.code.indexOf("#|#")))) {
                                            showCommonErrorPopUp(commonConstants.oUsers.code);
                                        } else {
                                            oMatterProvisionObject && oMatterProvisionObject.showErrorNotification(sSelector, commonConstants.oUsers.value);
                                        }
                                    }
                                    $SelectedObject.removeClass("ui-autocomplete-loading"); //// Remove the loading image explicitly once done with auto complete operations
                                },
                                error: function (result) {
                                    return false;
                                }
                            });
                        } else {
                            $SelectedObject.removeClass("ui-autocomplete-loading"); //// Remove the loading image explicitly once done with auto complete operations
                        }
                    },
                    focus: function (event, ui) {
                        //// prevent value inserted on focus
                        return false;
                    },
                    select: function (event, ui) {
                        if (ui && ui.item && !($.trim(ui.item.Email))) {
                            return false;
                        }
                        $(event.target).attr("data-resolved", "1");
                        if (bIsMultiUser) {
                            var terms = $.trim(this.value) ? $.trim(this.value).split(";") : [];
                            //// remove the current input
                            terms.pop();
                            //// add the selected item
                            terms.push(ui.item.value + " (" + ui.item.OriginalEmail + ")");
                            //// add placeholder to get the semicolon-and-space at the end
                            terms.push("");
                            terms = $.map(terms, function (item) {
                                return ($.trim(item));
                            });
                            this.value = terms.join("; ");
                            return false;
                        } else if (sSelector === "#txtConflictCheckBy") {
                            this.value = ui.item.value + " (" + ui.item.OriginalEmail + ");";
                            return false;
                        }
                    }
                }).data("uiAutocomplete")._renderItem = function (ul, item) {
                    var sUserTitle, sTooltip;
                    // Check if there is a user to be returned then assign title else for no record results keep it empty
                    if (item.OriginalEmail) {
                        sUserTitle = item.title;
                        sTooltip = item.OriginalEmail;
                    } else {
                        sUserTitle = "";
                        sTooltip = "";
                    }
                    return $("<li>")
                    .append("<a title='" + sTooltip + "'>" + item.label + "</a>")
                    .appendTo(ul);
                };
            }
        },

        getUserName: function (sUserEmails, bIsName) {
            "use strict";
            var arrUserNames = [], sEmail = "", oEmailRegex = new RegExp(oGlobalConstants.Email_Validation_Regex);
            if (sUserEmails && null !== sUserEmails && "" !== sUserEmails) {
                arrUserNames = sUserEmails.split(";");
                for (var iIterator = 0; iIterator < arrUserNames.length - 1; iIterator++) {
                    if (arrUserNames[iIterator] && null !== arrUserNames[iIterator] && "" !== arrUserNames[iIterator]) {
                        if (-1 !== arrUserNames[iIterator].lastIndexOf("(")) {
                            sEmail = $.trim(arrUserNames[iIterator].substring(arrUserNames[iIterator].lastIndexOf("(") + 1, arrUserNames[iIterator].lastIndexOf(")")));
                            if (oEmailRegex.test(sEmail)) {
                                arrUserNames[iIterator] = bIsName ? $.trim(arrUserNames[iIterator].substring(0, arrUserNames[iIterator].lastIndexOf("("))) : sEmail;
                            }
                        }
                    }
                }
            }
            return arrUserNames;
        },

        // Function to get current width of the window
        getWidth: function () {
            var nWidth = 0;
            if (self.innerHeight) {
                nWidth = self.innerWidth;
            } else if (document.documentElement && document.documentElement.clientHeight) {
                nWidth = document.documentElement.clientWidth;
            } else if (document.body) {
                nWidth = document.body.clientWidth;
            }
            return nWidth;
        },
        /* Function to update the URL of go to Matter Sites ECB action */
        checkMatterLandingURLSuccess: function (result) {
            //// Handle the true and false responses for the Matter Landing page
            if (result && result.Result && result.oParam) {

                //// Show the go to Matter Sites action and remove the chunk for loading image
                var oGoToMatterSitesElement = result.oParam.currentECBMenuContainer.find(".gotoMatterSites");
                var gotoOneNoteOption = result.oParam.currentECBMenuContainer.find(".gotoOneNote");
                result.oParam.currentECBMenuContainer.find(".ms-ContextualMenu-item").removeClass("hide");
                var anchorElement = document.createElement("a"); //// Dummy element to get the host name and protocol properties
                anchorElement.href = result.oParam.clientUrl; //// Set the HREF attribute of dummy element to client URL
                $(".mattersiteloading").addClass("hide");

                var oSplitResults = result.Result.split("$|$");
                if (2 === oSplitResults.length) {
                    var oOneNoteResult = oSplitResults[0].split("$#$")
                    , oMatterLandingPageResult = oSplitResults[1].split("$#$")
                  , sMatterPageLink = result.oParam.matterLibraryURL;  // Set the link to the All Items page
                    if (oOneNoteResult && 2 === oOneNoteResult.length) {
                        if ("true" === oOneNoteResult[0]) {
                            if (gotoOneNoteOption && gotoOneNoteOption.length) {
                                gotoOneNoteOption.attr({ "href": anchorElement.href + "/" + oFindMatterConstants.WOPIFrameURL + anchorElement.protocol + "//" + anchorElement.hostname + oOneNoteResult[1], "target": "_blank" });
                                gotoOneNoteOption.removeClass("is-disabled").removeAttr("disabled");
                            }
                        }
                    } else {
                        //// If OneNote URL not exist then disabled option
                        gotoOneNoteOption.addClass("is-disabled").attr("disabled", "disabled");
                        gotoOneNoteOption.removeAttr("href");
                        gotoOneNoteOption.removeAttr("target");
                    }


                    //// Set the link to the Matter Landing page if oMatterLandingPageResult[0]  is true
                    if (oMatterLandingPageResult && 2 === oMatterLandingPageResult.length && (oMatterLandingPageResult[0] && $.parseJSON(oMatterLandingPageResult[0]))) {
                        sMatterPageLink = anchorElement.protocol + "//" + anchorElement.hostname + oMatterLandingPageResult[1];
                    }

                    //// Set the hyper link property of go to Matter Sites
                    oGoToMatterSitesElement.attr({ "data-matterlink": sMatterPageLink, "onclick": "viewMatter(this);" });
                } else {
                    commonConstants.checkMatterLandingURLFailure(result); //// Set the path to matter library
                }
            } else {
                commonConstants.checkMatterLandingURLFailure(result); //// Set the path to matter library
            }
        },
        /* Function to set the path of go to Matter Sites to the location of matter landing page */
        checkMatterLandingURLFailure: function (result) {
            //// Set the URL to document library
            if (result && result.oParam && result.oParam.matterLibraryURL && result.oParam.currentECBMenuContainer) {
                result.oParam.currentECBMenuContainer.find(".gotoMatterSites").attr({ "href": result.oParam.matterLibraryURL, "target": "_blank" });
            }
        },
        /* Function to check whether the Matter Landing page exists */
        checkMatterLandingURL: function (sClientURL, sOneNoteURL, sMatterLandingPageURL, sMatterLibraryURL, oContextualMenu) {
            //// Hide the go to Matter Sites action and display the loading image
            oContextualMenu.find(".ms-ContextualMenu-item").addClass("hide");
            $(".mattersiteloading").removeClass("hide");

            var oCheckMatterLandingParameters = { "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": sClientURL }, requestedUrl: sOneNoteURL, requestedPageUrl: sMatterLandingPageURL }
              , oParam = {
                  "clientUrl": sClientURL, "matterLibraryURL": sMatterLibraryURL, "currentECBMenuContainer": oContextualMenu
              };

            oCommonObject.callSearchService("UrlExists", oCheckMatterLandingParameters, commonConstants.checkMatterLandingURLSuccess, commonConstants.checkMatterLandingURLFailure, null, oParam);
        },
        /* Function to update pinned status of the matter */
        updatePinnedStatus: function () {
            //// Get the URL of matter
            var oMandatoryObject = $(".mandatory"); //// Get all the mandatory columns from the grid view control
            var iStartPosition = parseInt(parseInt(oGridViewObject.itemsPerPage, 10) * (parseInt(oGridViewObject.pageNumber, 10) - 1), 10)
              , iLengthToTraverse = oMandatoryObject.length * parseInt(oGridViewObject.pageNumber, 10),
            arrDataToCompare = $.map(oCommonObject.userPinnedData, function (item) {
                return decodeURIComponent(trimEndChar($.trim(item), "/").toLowerCase());
            });
            /* Update the status only for latest data load in the grid view */
            for (var iIterator = iStartPosition; iIterator < iLengthToTraverse ; iIterator++) {
                var oCurrentObject = $(oMandatoryObject[iIterator]); //// Get the current mandatory object
                var sCurrentPath = oCurrentObject.attr("data-" + oGlobalConstants.Path); //// Get the matter URL
                //// Check if matter is already pinned and set the attribute accordingly
                if (-1 === $.inArray(decodeURIComponent(trimEndChar($.trim(sCurrentPath), "/").toLowerCase()), arrDataToCompare)) {
                    oCurrentObject.attr("data-ispinned", "false");
                } else {
                    oCurrentObject.attr("data-ispinned", "true");
                }
            }
        },
        /* Function to generate common drop down control */
        generateCommonDropdown: function (sDropdownSelector, sDropdownFields) {
            var sDropdownData = [];
            sDropdownData = $.trim(sDropdownFields) ? $.trim(sDropdownFields).split(";") : "";
            oDropdownControl.generateDropdownControl("#searchPanelDropdown", sDropdownData);
        },
        /* Function to generate common drop down control */
        generateColumnPicker: function (sColumnPickerSelector, sColumnPickerFields) {
            var sColumnPickerData = [];
            sColumnPickerData = $.trim(sColumnPickerFields) ? $.trim(sColumnPickerFields).split(";") : "";
            oColumnPickerControl.generateColumnPickerControl(sColumnPickerSelector, sColumnPickerData);
        },
        /* Function to update configurations and refresh grid view */
        updateAndRefreshGridView: function (oCurrentElement, bIsSearchCall) {
            var oPageHeader = $("#gridViewPageHeader");
            if (oPageHeader.length) {
                oPageHeader.text(oCurrentElement.text()); //// Set the page header to current selection
                //// Set the tool tip of the current section
                switch (oCurrentElement.text()) {
                    case oGlobalConstants.Pinned_Matters_Title:
                        oPageHeader.attr("title", oGlobalConstants.Pinned_Matters_Message);
                        $(".ms-Dropdown-title").attr("title", oGlobalConstants.Pinned_Matters_Message);
                        break;
                    case oGlobalConstants.All_Matters_Title:
                        oPageHeader.attr("title", oGlobalConstants.All_Matters_Message);
                        $(".ms-Dropdown-title").attr("title", oGlobalConstants.All_Matters_Message);
                        break;
                    case oGlobalConstants.My_Matters_Title:
                        oPageHeader.attr("title", oGlobalConstants.My_Matters_Message);
                        $(".ms-Dropdown-title").attr("title", oGlobalConstants.My_Matters_Message);
                        break;
                    case oGlobalConstants.Pinned_Documents_Title:
                        oPageHeader.attr("title", oGlobalConstants.Pinned_Documents_Message);
                        $(".ms-Dropdown-title").attr("title", oGlobalConstants.Pinned_Documents_Message);
                        break;
                    case oGlobalConstants.All_Documents_Title:
                        oPageHeader.attr("title", oGlobalConstants.All_Documents_Message);
                        $(".ms-Dropdown-title").attr("title", oGlobalConstants.All_Documents_Message);
                        break;
                    case oGlobalConstants.My_Documents_Title:
                        oPageHeader.attr("title", oGlobalConstants.My_Documents_Message);
                        $(".ms-Dropdown-title").attr("title", oGlobalConstants.My_Documents_Message);
                        break;
                }
            }
            commonConstants.abortRequest();
            commonConstants.clearGridViewContent();
            commonConstants.clearFlyOutFilters(true); //// Clear server side filters
            commonConstants.clearClientFlyOutFilters(); //// Clear client side filters
            (oCommonObject.isMatterView) ? getPinnedMatters($("#gridView")) : getPinnedDocument($("#gridView"));
            if (bIsSearchCall) {
                (oCommonObject.isMatterView) ? getSearchMatters($("#gridView"), 0) : getSearchDocuments($("#gridView"), 0);
            }
            $("#autoCompleteText").focus().blur();
            $("#attachDocuments").addClass("is-disabled");
        },

        /* Aborts the previously made AJAX call */
        abortRequest: function () {
            "use strict";
            if (!oCommonObject.isServiceCallComplete) {
                if (oServiceGlobal.oAjaxRequest && 4 !== oServiceGlobal.oAjaxRequest.readyState) {
                    oServiceGlobal.oAjaxRequest.abort();
                    oServiceGlobal.oAjaxRequest = null;
                }
            }
        },
        /* Function to set tool tip of common drop down*/
        updateDropDownToolTip: function () {
            var dropDownItem = $(".ms-Dropdown-item");
            $.each(dropDownItem, function (iIterator) {
                if ($(dropDownItem[iIterator]).length) {
                    switch ($(dropDownItem[iIterator]).text()) {
                        case oGlobalConstants.Pinned_Documents_Title:
                            $(dropDownItem[iIterator]).attr("title", oGlobalConstants.Pinned_Documents_Message);
                            break;
                        case oGlobalConstants.All_Documents_Title:
                            $(dropDownItem[iIterator]).attr("title", oGlobalConstants.All_Documents_Message);
                            break;
                        case oGlobalConstants.My_Documents_Title:
                            $(dropDownItem[iIterator]).attr("title", oGlobalConstants.My_Documents_Message);
                            break;
                        case oGlobalConstants.Pinned_Matters_Title:
                            $(dropDownItem[iIterator]).attr("title", oGlobalConstants.Pinned_Matters_Message);
                            break;
                        case oGlobalConstants.All_Matters_Title:
                            $(dropDownItem[iIterator]).attr("title", oGlobalConstants.All_Matters_Message);
                            break;
                        case oGlobalConstants.My_Matters_Title:
                            $(dropDownItem[iIterator]).attr("title", oGlobalConstants.My_Matters_Message);
                            break;
                    }
                }
            });
        },
        /* Function to bind events to common drop down control */
        addDropdownBindings: function () {
            /* Functionality of All Matters */
            $(".alldata").on("click", function () {
                oCommonObject.isAllRowSelected = false;
                $("#autoCompleteText").val("");
                oCommonObject.sSearchedKeyword = "";
                commonConstants.iCurrentGridViewData = 1; //// Set the flag for search data
                commonConstants.arrCurrentRefiner.length = 0; //// Remove the current selected filters
                commonConstants.updateAndRefreshGridView($(this), true); //// Update the configuration and refresh the grid view
            });

            /* Functionality of My Matters */
            $(".mydata").on("click", function () {
                oCommonObject.isAllRowSelected = false;
                $("#autoCompleteText").val("");
                oCommonObject.sSearchedKeyword = "";
                commonConstants.iCurrentGridViewData = 2; //// Set the flag for search data
                commonConstants.arrCurrentRefiner.length = 0; //// Remove the current selected filters
                commonConstants.updateAndRefreshGridView($(this), true); //// Update the configuration and refresh the grid view
            });

            /* Functionality of Recent Matters */
            $(".recentdata").on("click", function () {
                oCommonObject.isAllRowSelected = false;
                $("#autoCompleteText").val("");
                oCommonObject.sSearchedKeyword = "";
                commonConstants.iCurrentGridViewData = 3; //// Set the flag for search data
                commonConstants.arrCurrentRefiner.length = 0; //// Remove the current selected filters
                commonConstants.updateAndRefreshGridView($(this), true); //// Update the configuration and refresh the grid view
            });

            /* Functionality of Pinned Matters */
            $(".pinneddata").on("click", function () {
                oCommonObject.isAllRowSelected = false;
                $("#autoCompleteText").val("");
                oCommonObject.sSearchedKeyword = "";
                commonConstants.iCurrentGridViewData = 4; //// Set the flag for search data
                commonConstants.arrCurrentRefiner.length = 0; //// Remove the current selected filters
                commonConstants.updateAndRefreshGridView($(this), false); //// Update the configuration and refresh the grid view
            });

            /* Function to remove the extra styles attached by fabric */
            $(".ms-Dropdown").on("click", function () {
                $(this).find(".ms-Dropdown-item").removeClass("ms-Dropdown-item--selected");
            });

            /* Function to open the drop down menu */
            $("#searchPanelDropdown .ms-Dropdown-title").click(function (event) {
                commonConstants.closeAllPopupExcept("ms-Dropdown", event);     //// Close all pop-ups except App Switcher
            });

            /* Function to open the drop down menu on click of glyph icon */
            $("#glyphIcon .ms-Icon--caretDown").on("click", function (event) {
                $(".ms-Dropdown").toggleClass("ms-Dropdown--open");
                commonConstants.closeAllPopupExcept("ms-Dropdown", event);     //// Close all pop-ups except App Switcher
                event && event.stopPropagation();
            });
        },
        autoCompleteSearch: function () {
            if ($(".autoCompletePanel").html()) {
                var oSelectedAutoCompleteRow = $(".autoCompleteSelected");
                // Pick document id of the selected document in the row
                oCommonObject.sDocID = oSelectedAutoCompleteRow.attr("data-docid");
                // If any suggestion is selected from the auto complete panel set the text in the auto complete text box
                if (0 !== oSelectedAutoCompleteRow.length) {
                    $("#autoCompleteText").val(oSelectedAutoCompleteRow.text());
                    $(".autoCompletePanel").empty().addClass("hide");
                }
            }
            $("#searchIcon").click(); //// Trigger search Icon click to search for Matters/Documents
            return false;
        },
        /* Function to bind events for auto complete control */
        addAutoCompleteBindings: function () {
            var $textField = $(".ms-TextField");

            /* Bind JS of fabric component */
            $textField.textField();

            /* Event for Enter event on Chrome*/
            $("#MasterPageForm").on("submit", function (e) {
                //// If auto complete panel has some results
                oCommonObject.autoCompleteSearch();
                return false;
            });

            /* Event for search text box input, calling auto complete */
            $("#autoCompleteText").on("keyup", function (e) {
                commonConstants.bHideAutoComplete = false;
                if ($(this).val()) {
                    if (13 === e.which) {
                        commonConstants.bHideAutoComplete = true;
                        oCommonObject.autoCompleteSearch();
                        return false;
                    }
                    var $autoCompleteRow = $(".autoCompleteRow");
                    if ($(".autoCompletePanel").html()) {
                        // If no suggestion is selected
                        if (0 === $(".autoCompleteSelected").length) {
                            if (40 === e.which) {
                                $($autoCompleteRow[0]).addClass("autoCompleteSelected");
                            }
                            if (38 === e.which) {
                                $($autoCompleteRow[parseInt($autoCompleteRow.length, 10) - 1]).addClass("autoCompleteSelected");
                            }
                        } else {
                            // If no suggestion is selected
                            var position = $autoCompleteRow.filter(".autoCompleteSelected").index();
                            $($autoCompleteRow[position]).removeClass("autoCompleteSelected");
                            // On key up event
                            if (40 === e.which) {
                                // Check to see if selected suggestion is not the last one in list
                                if (parseInt($autoCompleteRow.length, 10) - 1 !== position) {
                                    position = position + 1;
                                } else {
                                    $("#autoCompleteText").focus();
                                    $autoCompleteRow.removeClass("autoCompleteSelected");
                                    return false;
                                }
                            }
                            // On key down event
                            if (38 === e.which) {
                                // Check to see if selected suggestion is not the first one in the list
                                if (0 !== position) {
                                    position = position - 1;
                                } else {
                                    $("#autoCompleteText").focus();
                                    $autoCompleteRow.removeClass("autoCompleteSelected");
                                    return false;
                                }
                            }
                            $($autoCompleteRow[position]).addClass("autoCompleteSelected");
                        }
                    }
                    if (40 !== e.which && 38 !== e.which && 13 !== e.which && "" !== $(this).val()) {
                        commonConstants.getAutoCompleteData();
                    }
                } else {
                    commonConstants.bHideAutoComplete = true;
                    $(".autoCompletePanel").addClass("hide");
                }
            });

            //// Binding on onfocus event on auto complete text box
            $(document).on("focus", "#autoCompleteText", function (event) {
                $(".autoCompleteRow").removeClass("autoCompleteSelected");
                commonConstants.closeAllPopupExcept("", event);
            });

            // Binding click event on auto complete suggestions
            $(document).on("click", ".autoCompleteRow", function (event) {
                $("#autoCompleteText").click().val($(this).text()); //// Set the selected text in the auto complete text box
                commonConstants.closeAllPopupExcept("", event); //// Close all popups
                oCommonObject.sDocID = $(this).attr("data-docid");
                commonConstants.sDocumentFullName = $(this).attr("data-searchterm");
                $("#searchIcon").click(); //// Trigger search Icon click to search for Matters/Documents
                event && event.stopPropagation();
            });

            /* Binding click event on search text box */
            $textField.on("click", function (event) {
                $textField.find(".ms-Label").hide();
                $("#autoCompleteText").focus();
                event && event.stopPropagation();
            });

            /* Function to execute on click of Search Icon */
            $("#searchIcon").on("click", function () {
                commonConstants.bHideAutoComplete = true;
                if (!$(".ms-Icon--search")[0].disabled) {
                    oGridViewObject.pageNumber = 1; //// Reset the page number for grid view control
                    if (4 === oCommonObject.iCurrentGridViewData()) { //// Check the flag for search data if current view is pinned view
                        commonConstants.iCurrentGridViewData = 1;
                        $(".searchPanelDropdownOption").removeClass("selectedDropdownOption");
                        $(".alldata").addClass("selectedDropdownOption");
                        if (oCommonObject.isMatterView) {
                            $(".ms-Dropdown-title, #gridViewPageHeader").text("All Matters").attr("title", oGlobalConstants.All_Matters_Message); //// Set the drop down title to All Matters                 
                        } else {
                            $(".ms-Dropdown-title, #gridViewPageHeader").text("All Documents").attr("title", oGlobalConstants.All_Documents_Message); //// Set the drop down title to All Matters
                        }
                    }
                    oCommonObject.isAllRowSelected = false;
                    commonConstants.arrCurrentRefiner.length = 0; //// Remove the current selected filters
                    commonConstants.clearFlyOutFilters(true);
                    if (!$("#autoCompleteText").val().trim().length) {
                        $("#autoCompletePlaceHolder").css("display", "inline");
                    }
                    $("#gridViewContainer").off("scroll").empty(); /// Remove the data from the grid view
                    $("#loadingImageContainer").removeClass("hide"); //// Hide the loading image of grid view control
                    oCommonObject.sSearchedKeyword = $("#autoCompleteText").length ? $("#autoCompleteText").val() : "";
                    if (oCommonObject.isMatterView) {
                        //// Get data for Search Matters
                        getSearchMatters($("#gridView"), 1);
                    } else {
                        //// Get data for Search Documents
                        getSearchDocuments($("#gridView"), 1);
                    }
                }
            });
        },
        /* Function to bind events for column picker control */
        addColumnPickerBindings: function () {
            var $columnPickerIcon = $("#columnPickerIcon");
            var oContainer = "columnPickerPanel";
            var arrCheckBox = $("#" + oContainer + "").find("input[type='checkbox']");
            var nCheckBoxCount = arrCheckBox.length;

            $columnPickerIcon.on("click", function (event) {
                commonConstants.closeAllPopupExcept("columnPickerPanel", event);     //// Close all pop-ups except Column Picker
                $columnPickerIcon.toggleClass("selected");
                var imageSrc = $columnPickerIcon.hasClass("selected") ? "../Images/Column_Picker_Close.png" : "../Images/Column_Picker_Expand.png";
                $columnPickerIcon.attr("src", imageSrc);
                $("#columnPickerPanel").toggle("slide", { direction: "right" }).toggleClass("hide");
                $("#columnPickerStrip").toggleClass("hide");
                event && event.stopPropagation();
            });

            $(document.body).on("change", "#options-checkbox-unselected", function () {
                var iIterator = 0;
                var bCurrentItemStatus = $("#options-checkbox-unselected")[0].checked;
                oGridViewObject && (oGridViewObject.bDefaultSelectionChanged = true);
                //// Starting the iteration from position 2 as Name column is to be selected always and it is present in position 1
                for (iIterator = 2; iIterator <= nCheckBoxCount - 1; iIterator++) {
                    arrCheckBox[iIterator].checked = this.checked;
                    commonConstants.toggleColumns(iIterator - 1, bCurrentItemStatus);
                }
            });

            $("#" + oContainer).on("click", ".defaultSelection", function (event) {
                event.stopPropagation();
                event.preventDefault();
            });
        },
        /* Function to perform column option selection */
        onColumnPickerCheckboxClick: function (iIndex, event) {
            if (iIndex) {
                oGridViewObject && (oGridViewObject.bDefaultSelectionChanged = true);
                var bCurrentItemStatus = $("#options-checkbox-unselected" + iIndex)[0].checked;
                var $AllColumnOption = $("#columnOptions").find("#options-checkbox-unselected");
                commonConstants.toggleColumns(iIndex, bCurrentItemStatus);
                if ($AllColumnOption[0].checked) {
                    $AllColumnOption.prop("checked", false);
                }
                var nSelectedColumnCount = $("#columnOptions").find("input[type='checkbox']:checked").length;
                var nTotalColumnCount = $("#columnOptions").find("input[type='checkbox']").length;
                if (nTotalColumnCount - 1 === nSelectedColumnCount) {
                    $AllColumnOption.prop("checked", true);
                }
                event ? event.stopPropagation() : "";
            }
        },
        /* Function to perform grid view item selection */
        onGridViewCheckboxClick: function () {
            var $AllGridViewItems = $(".isSelectRowsActive");
            if ($AllGridViewItems && $AllGridViewItems.length && $AllGridViewItems[0].checked) {
                $AllGridViewItems.prop("checked", false);
                oCommonObject.isAllRowSelected = false;
            }
            var nSelectedColumnCount = $(".jsonGrid").find(".is-selectedRow").length;
            var nTotalColumnCount = $(".gridCell").find("input[type='checkbox']").length;
            if (nTotalColumnCount && (nSelectedColumnCount === nTotalColumnCount)) {
                $AllGridViewItems.prop("checked", true);
                oCommonObject.isAllRowSelected = true;
            }
        },
        /* Function to show/hide grid view columns based upon user selection */
        toggleColumns: function (iIndex, bCurrentItemStatus) {
            var nHeaderOptionMapping = oCommonObject.isMatterView ? iIndex + oFindMatterConstants.GridViewNonDataColumns.split(";").length + 1 : iIndex + oFindDocumentConstants.GridViewNonDataColumns.split(";").length + 1;
            if (bCurrentItemStatus) {
                $(".dataColumn" + (parseInt(iIndex) + 1)).parent().removeClass("hide");
                $(".jsonGridHeader:nth-child(" + nHeaderOptionMapping + ")").removeClass("hide");
            } else {
                $(".dataColumn" + (parseInt(iIndex) + 1)).parent().addClass("hide");
                $(".jsonGridHeader:nth-child(" + nHeaderOptionMapping + ")").addClass("hide");
            }
            oGridViewObject.arrUserSelectedColumns = [];
            $.each($("#columnOptions").find("input[type='checkbox']:checked").siblings(), function (index, value) {
                $(value).text().trim() && oGridViewObject.arrUserSelectedColumns.push($(value).text().trim());
            });
            /* Adjust the caret down icon for last column in the grid view */
            commonConstants.adjustCaretIcon();
        },
        /* Function for failure of auto complete */
        getAutoCompleteDataFailure: function (result) {
            oGridViewObject.waitTillDataLoaded = false;
            var oResult = JSON.parse(result);
            showCommonErrorPopUp(oResult.Result);
        },
        /* Function for success of auto complete */
        getAutoCompleteDataSuccess: function (result) {
            if (!commonConstants.bHideAutoComplete) {
                var splitResults, jsonResponse;
                jsonResponse = {
                };
                if (-1 !== result.indexOf("$|$")) {
                    splitResults = result.split("$|$");
                    if (splitResults[1] && parseInt(splitResults[1], 10) > 0) {
                        jsonResponse = JSON.parse(splitResults[0]);
                    }
                }
                commonFunction.closeAllFilterExcept("autoCompletePanel");
                oCommonObject.sDocID = "null"; // Stores document id if document id exist for particular document

                var autoContainer = "<div id='autoComplete'>"
                    , autoCompleteItemContainer = "<div class=\"autoCompleteRow\" title=\"#SearchTerm#\" data-docid=\"#DocID#\" data-searchterm=\"#SearchTerm#\">"
                    , regularSearchTerm = new RegExp("#SearchTerm#", "g")
                    , regularDocID = new RegExp("#DocID#", "g")
                    , lineItem
                    , autoCompleteItemContainerClose = "#SearchTermClose#</div>"
                    , regularSearchTermClose = new RegExp("#SearchTermClose#", "g")
                    , sTerm = $("#autoCompleteText").length ? $("#autoCompleteText").val() : "";

                if (jsonResponse && jsonResponse.length) {
                    for (lineItem in jsonResponse) {
                        if (jsonResponse.hasOwnProperty(lineItem)) {
                            /* Differ the implementation based on Search Matters or Search Documents */
                            if (oCommonObject.isMatterView) { //// Manipulation for Search Matters
                                var matterName = jsonResponse[lineItem][oGlobalConstants.Matter_Name];
                                autoContainer = autoContainer + autoCompleteItemContainer.replace(regularSearchTerm, matterName + " (" + jsonResponse[lineItem][oGlobalConstants.Matter_ID] + ")") + autoCompleteItemContainerClose.replace(regularSearchTermClose, oCommonObject.highlightTerm(matterName + " (" + jsonResponse[lineItem][oGlobalConstants.Matter_ID] + ")", sTerm));
                            } else {//// Manipulation for Search Documents
                                var docName = oCommonObject.ExtractFileTitle(jsonResponse[lineItem][oGlobalConstants.File_Name]);
                                var docId = jsonResponse[lineItem][oGlobalConstants.Document_ID];
                                if (0 < $.trim(docId).length) {
                                    autoContainer = autoContainer + autoCompleteItemContainer.replace(regularSearchTerm, jsonResponse[lineItem][oGlobalConstants.File_Name] + " (" + docId + ")").replace(regularDocID, docId) + autoCompleteItemContainerClose.replace(regularSearchTermClose, oCommonObject.highlightTerm(docName + " (" + docId + ")", sTerm));
                                } else {
                                    autoContainer = autoContainer + autoCompleteItemContainer.replace(regularSearchTerm, jsonResponse[lineItem][oGlobalConstants.File_Name]).replace(regularDocID, docId) + autoCompleteItemContainerClose.replace(regularSearchTermClose, oCommonObject.highlightTerm(docName, sTerm));
                                }
                            }
                        }
                    }
                    $(".autoCompletePanel").removeClass("hide").html(autoContainer);
                } else {
                    //// Display no results in auto complete panel  /* Do not remove commented code; kept for future purpose */
                    //// autoContainer = autoContainer + "<div class='searchOptions' data-option='1' onclick='return false;'>" + oGlobalConstants.No_Results_Autocomplete + "</div>";
                    $(".autoCompletePanel").addClass("hide").empty();
                }

                advanceSearchDropdown();
            }
        },
        /* Function to get the auto complete results */
        getAutoCompleteData: function () {
            var ofilterSearchText = $("#autoCompleteText").length ? $("#autoCompleteText").val() : ""
                , sSearchTerm = ""
                , oFilterDetails = {}
                , oSortDetails = { ByProperty: "", Direction: 0 }
                , oSearchDetails, sRefinerForName, sRefinerForID, sServiceCallName;

            //// Update the refiners and configurations based on Search Matters or Search Documents
            if (oCommonObject.isMatterView) {
                oFilterDetails = {
                    ClientsList: [], PGList: "", AOLList: "", FromDate: "", ToDate: "", FilterByMe: 1
                };
                sRefinerForName = oGlobalConstants.Matter_Name;
                sRefinerForID = oGlobalConstants.Matter_ID;
                sServiceCallName = "FindMatter";
            } else {
                oFilterDetails = {
                    ClientsList: [], FromDate: "", ToDate: "", DocumentAuthor: "", FilterByMe: 1
                };
                sRefinerForName = oGlobalConstants.File_Name;
                sRefinerForID = oGlobalConstants.Document_ID;
                sServiceCallName = "FindDocument";
            }

            if (2 !== oCommonObject.iCurrentGridViewData()) { //// If All Matters/Documents section (flag is 1) is selected, then set filter by me flag to 0
                oFilterDetails.FilterByMe = 0;
            }

            if (2 === oCommonObject.iCurrentGridViewData()) { //// If My Matters/Documents section (flag is 2) is selected, then update the Sort filter
                oSortDetails.ByProperty = oCommonObject.isMatterView ? oGlobalConstants.Last_Modified_Time : oGlobalConstants.Document_Last_Modified_Time;
                oSortDetails.Direction = 1;
            }

            if (ofilterSearchText) {
                sSearchTerm = "(" + sRefinerForName + ":" + ofilterSearchText + "* OR " + sRefinerForID + ":" + ofilterSearchText + "*)";

                oSearchDetails = {
                    "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": oCommonObject.getDeployedUrl() }, "searchObject": {
                        "PageNumber": "1", "ItemsPerPage": "5", "SearchTerm": sSearchTerm, "Filters": oFilterDetails, "Sort": oSortDetails
                    }
                };

                oCommonObject.callSearchService(sServiceCallName, oSearchDetails, oCommonObject.getAutoCompleteDataSuccess, oCommonObject.getAutoCompleteDataFailure, null, null);
            } else {
                $(".autoCompletePanel").addClass("hide").empty();
            }
        },
        /* Function to get the source path for the icon */
        getIconSource: function (sExtension) {
            var iconSrc = oGlobalConstants.Image_Document_Icon.replace("{0}", sExtension);
            iconSrc = (-1 < oGlobalConstants.PNG_Icon_Extensions.indexOf(sExtension)) ? iconSrc.substring(0, oGlobalConstants.Image_Document_Icon.lastIndexOf(".") + 1) + "png" : iconSrc;
            return iconSrc;
        },
        /* Function to close all pop up */
        closeAllPopupExcept: function (divClass, event) {
            var sCurrentItemClass = (event.target && event.target.className) ? event.target.className : ""
            , sCurrentItemId = (event.target && event.target.id) ? event.target.id : "";

            if ("AppMenuFlyout" !== divClass) {
                var $AppMenuFlyout = $(".AppMenuFlyout");
                if ($AppMenuFlyout.is(":visible")) {
                    $AppMenuFlyout.slideUp();
                    $(".OpenSwitcher").removeClass("hide");
                    $(".MenuCaption").removeClass("hideMenuCaption");
                    $(".CloseSwitcher").addClass("hide");
                }
            }
            if ("ContextualHelpContainer" !== divClass) {
                var $ContextualHelpFlyout = $(".ContextualHelpContainer");
                if ($ContextualHelpFlyout.is(":visible")) {
                    $ContextualHelpFlyout.hide();
                }
            }
            if ("ms-ContextualMenu" !== divClass) {
                $(".ms-ContextualMenu").removeClass("is-open");
            }
            if ("errorAttachDocument" !== divClass) {
                $(".errorAttachDocument").addClass("hide");
            }
            if ("ms-Dropdown" !== divClass) {
                var $ViewDropDown = $(".ms-Dropdown");
                if ($ViewDropDown.hasClass("ms-Dropdown--open")) {
                    $ViewDropDown.removeClass("ms-Dropdown--open");
                }
            }
            if ("autoCompletePanel" !== divClass) {
                $(".autoCompletePanel").addClass("hide");   //// Remove the results from auto complete panel and hide it
            }
            if ("PersonaFlyout" !== divClass) {
                $(".PersonaFlyout").slideUp();
            }
            if ("textFlyoutContent" !== divClass) {
                $(".flyoutWrapper").css("min-height", 0);
                $("#textFlyoutContent").addClass("hide");
            }
            if ("dateFlyoutContent" !== divClass) {
                $("#dateFlyoutContent").addClass("hide");
            }
            if ("columnPickerPanel" !== divClass) {
                if (!$("#columnPickerPanel").hasClass("hide")) {
                    $("#columnPickerIcon").click();
                }
            }
            if ("InfoFlyout" !== divClass) {
                $("#gridViewContainer .InfoFlyout").remove();
            }
        },
        /* Function to add bindings for ECB control */
        addECBBindings: function () {
            /* Functionality of Pin */
            $(".pin").on("click", function () {
                //// Use oGridViewObject.oCurrentMandatory object for getting current mandatory column
                if (oCommonObject.isMatterView) {
                    //// Pin Matter for User
                    pinMatter(oGridViewObject.oCurrentMandatory);
                } else {
                    //// Pin Document for User
                    pinDocument(oGridViewObject.oCurrentMandatory);
                }
            });

            /* Functionality of Unpin */
            $(".unpin").on("click", function () {
                //// Use oGridViewObject.oCurrentMandatory object for getting current mandatory column
                if (oCommonObject.isMatterView) {
                    //// Unpin Matter for User
                    unpinMatter(oGridViewObject.oCurrentMandatory);
                } else {
                    //// Unpin Document for User
                    unpinDocument(oGridViewObject.oCurrentMandatory);
                }
            });

            /* Functionality of Upload */
            $(".upload").on("click", function () {
                //// Use oGridViewObject.oCurrentMandatory object for getting current mandatory column
                if (!($(this).attr("disabled"))) {
                    $(".notification").remove();
                    populateFolderHierarchy(oGridViewObject.oCurrentMandatory);
                }
            });

            /* Functionality of Go to Matter Sites */
            $(".gotoMatterSites").on("click", function () {
                //// Use oGridViewObject.oCurrentMandatory object for getting current mandatory column
                commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Go_To_Matter_Sites, true);
            });

            /* Functionality of Matter Info */
            $(".matterinfo").on("click", function () {
                //// Use oGridViewObject.oCurrentMandatory object for getting current mandatory column
                commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Matter_Info, true);
            });

            /* Functionality of Document Info */
            $(".documentinfo").on("click", function () {
                //// Use oGridViewObject.oCurrentMandatory object for getting current mandatory column
                commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Document_Info, true);
            });

            /* Functionality of Edit Document */
            $(".editdocument").on("click", function () {
                //// Get the mandatory element from where we can get the meta data associated with the matter
                commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Edit_Document, true);
                var sDocumentPath = "";
                if (4 === oCommonObject.iCurrentGridViewData()) {
                    sDocumentPath = oGridViewObject.oCurrentMandatory.attr("data-documentowaurl");
                } else {
                    sDocumentPath = getDocumentUrl(oGridViewObject.oCurrentMandatory);
                }
                commonConstants.openWindow(sDocumentPath, "_blank");
            });
        },
        /* Function to populate data inside filter fly out */
        populateFilterHTML: function (sRefinerName, sFilterFlyoutType, arrRefinerUniqueValues, bScroll) {

            var sResultsChunk = "", oResultsCollection = [];
            //// Check the length of arrRefinerUniqueValues if its between 1 to 6 then set the height of the flyout filter accordingly
            if (oGlobalConstants.Minimum_Window_Size < window.innerHeight) {
                var windowheight = window.innerHeight - parseInt(oGlobalConstants.Space_Above_Filter_Flyout);
                if (1 <= arrRefinerUniqueValues.length && 6 >= arrRefinerUniqueValues.length) {
                    var defaultheight = parseInt(oGlobalConstants.Default_Record_Height)
                    , defultFilterHeight = parseInt(oGlobalConstants.Default_Flyout_Wrapper_Height)
                    , iflyoutHeight = defaultheight * (arrRefinerUniqueValues.length ? arrRefinerUniqueValues.length : 1)
                    , iflyoutWrapperHeight = defultFilterHeight + iflyoutHeight
                    , ifinalFlyoutHeight = iflyoutWrapperHeight < windowheight ? iflyoutWrapperHeight : windowheight
                    , ifilterResultHeight = defaultheight + (ifinalFlyoutHeight - defultFilterHeight);
                    $(".flyoutWrapper").css("min-height", ifinalFlyoutHeight);
                    $("#filterResultsContainer").css({ "max-height": ifilterResultHeight });
                }
                //// Check the length of arrRefinerUniqueValues if its greater than 6 set the flyout height to maximum
                if (7 <= arrRefinerUniqueValues.length) {
                    var ifinalFlyoutHeight = oGlobalConstants.Maximum_Flyout_Height < windowheight ? oGlobalConstants.Maximum_Flyout_Height : windowheight;
                    $(".flyoutWrapper").css("min-height", ifinalFlyoutHeight);
                    $("#filterResultsContainer").css({ "max-height": (ifinalFlyoutHeight - parseInt(oGlobalConstants.Minimum_Flyout_Height)) });
                }
            } else {
                $(".flyoutWrapper").css("min-height", oGlobalConstants.Small_WindowSize_Flyout_Height);
                $("#filterResultsContainer").css({ "max-height": oGlobalConstants.Small_WindowSize_Flyout_Container_Height });
            }
            //// singleselect: Single select filtering, multiselect: Multi select filtering, date: Filtering for date column
            if ("singleselect" === sFilterFlyoutType.toLowerCase()) {
                sResultsChunk = "<div class='filterValueLabels ms-font-m ms-font-weight-semilight' id='filterValue{0}'><span title='{1}'>{1}</span></div>";
            } else if ("multiselect" === sFilterFlyoutType.toLowerCase()) {
                sResultsChunk = "<div class='ms-ChoiceField filterValueLabels'><input id='filterValueCheckbox{0}' class='ms-ChoiceField-input filterValueCheckbox' type='checkbox' /><label for='filterValueCheckbox{0}' class='ms-ChoiceField-field filterValueCheckbox'><span class='ms-Label' title='{1}'>{1}</span></label></div>";
            }
            //// Populate the filter fly out with data for refiner
            if (0 === arrRefinerUniqueValues.length) {
                oResultsCollection.push("<div title=\"" + oGlobalConstants.FilterFlyout_NoResult + "\" class=\"ms-font-m filterFlyOutNoResults\"><span>" + oGlobalConstants.FilterFlyout_NoResult + "</span></div>");
            } else {
                $.each(arrRefinerUniqueValues, function (iIndex, sCurrentItem) {
                    var sCurrentValue = $.trim(sCurrentItem);
                    /* Commented code kept if there is a requirement to remove extension from filter fly out in future */
                    ////if ("undefined" !== typeof (oFindDocumentConstants) && (oFindDocumentConstants.GridViewTitleProperty === result.oParam.refinerValue || oFindDocumentConstants.GridViewPinnedTitleProperty === result.oParam.refinerValue)) {
                    ////    sCurrentValue = extractTitle(sCurrentValue);
                    ////} 
                    if (sCurrentValue) {
                        oResultsCollection.push(sResultsChunk.replace(/\{0\}/g, iIndex).replace(/\{1\}/g, sCurrentValue));
                    }
                });
            }

            //// Add empty elements for having scroll bar
            var iUniqueValueLength = oResultsCollection && oResultsCollection.length;
            if (10 > iUniqueValueLength && 0 < arrRefinerUniqueValues.length) {
                oResultsCollection.push("<div class=\"bufferElements\"></div>");
            }

            var sResultsCollectionChunk = oResultsCollection.join("");
            $("#filterResultsContainer").html(sResultsCollectionChunk);
            if (bScroll) {
                var topScroll = $("#filterResultsContainer").scrollTop();
                $("#filterResultsContainer").scrollTop(topScroll - 3);
            }
            commonConstants.waitTillFilterDataLoaded = false;
            commonConstants.bindScrollOnFilterFlyout();
            //// Highlight the selected filters
            commonConstants.highlightSelectedFilters(sRefinerName, sFilterFlyoutType);
        },
        /* Function to be called before getting data for specific column */
        getColumnDataOnBefore: function (result) {
            //// result.oParam.refinerValue will have the name of refiner
            var oLoadingChunk = "<div class=\"filterflyoutLoading\"><img title=\"Loading...\" alt=\"Loading...\" src=\"../Images/WindowsLoadingFast.GIF\"></div>";
            var oLoadingImage = $("#filterResultsContainer").find(".filterflyoutLoading");
            if (0 === oLoadingImage.length) {
                $("#filterResultsContainer").append(oLoadingChunk);
            }
        },
        /* Failure function to get the data for specific column */
        getColumnDataFailure: function (result) {
            commonConstants.waitTillFilterDataLoaded = false;
            var oResult = JSON.parse(result);
            showCommonErrorPopUp(oResult.Result);
        },
        /* Success function to get the data for specific column */
        getColumnDataSuccess: function (result) {
            //// result.oParam.refinerValue will have the name of refiner
            if (result && $.trim(result.Result)) {
                var sData = result.Result.split("$|$")
                  , bIsPageLoad;

                var searchResults = JSON.parse(sData[0]);
                var totalResults = parseInt(sData[1], 10) || 0;
                if (!searchResults.code || "0" === searchResults.code) {
                    //// Pinned matters exists for the current logged in user
                    if ("undefined" !== typeof sData[1] && totalResults) {
                        commonConstants.iColumnDataTotalResults = totalResults;
                        //// Get the data for the refiner i.e. selected column
                        var oRefinerData = $.map(searchResults, function (sCurrentItem) {
                            return trimEndChar($.trim(sCurrentItem[result.oParam.refinerValue]), ";");
                        });
                        var uniqueDataLength = commonConstants.arrUniqueRefinerData.length;
                        //// Get the unique values from the refiner array
                        /* Separate out individual entries for multiuser field */
                        $.each(oRefinerData, function (iCount, sCurrentValue) {
                            var arrSplitValues = [];
                            if ($.trim(sCurrentValue)) {
                                arrSplitValues = sCurrentValue.split(";");
                            }
                            $.each(arrSplitValues, function (iCurrentCount, sValue) {
                                var sTrimmedValue = $.trim(sValue);
                                ($.inArray(sTrimmedValue, commonConstants.arrUniqueRefinerData) === -1) && commonConstants.arrUniqueRefinerData.push(sTrimmedValue);
                            });
                        });
                        var bScroll = uniqueDataLength === commonConstants.arrUniqueRefinerData.length;
                        //// Get specific value for multivalued text
                        var sCurrentFilterSearchTerm = $(".filterFlyoutSearchText").length ? $(".filterFlyoutSearchText").val() : "";
                        if ($.trim(sCurrentFilterSearchTerm)) {
                            var oSplitSearchTerm = sCurrentFilterSearchTerm.split(";")
                                , oClientFilteredArray = [];
                            if (oSplitSearchTerm.length) {
                                $.each(oSplitSearchTerm, function (sCurrentIndex, sCurrentValue) {
                                    sCurrentValue = $.trim(sCurrentValue).toLowerCase();
                                    $.each(commonConstants.arrUniqueRefinerData, function (sIndex, sValue) {
                                        var sLowerValue = sValue.toLowerCase();
                                        if (-1 !== sLowerValue.indexOf(sCurrentValue) && -1 === commonConstants.hasValue(sValue, oClientFilteredArray)) {
                                            oClientFilteredArray.push(sValue);
                                        }
                                    });
                                });
                                commonConstants.arrUniqueRefinerData = oClientFilteredArray;
                            }
                        }
                        commonConstants.populateFilterHTML(result.oParam.refinerValue, result.oParam.filterFlyoutType, commonConstants.arrUniqueRefinerData, bScroll);
                    } else {
                        // No documents exists
                        sData[1] = 0;
                        $("#filterResultsContainer").html("<div title=\"" + oGlobalConstants.FilterFlyout_NoResult + "\" class=\"ms-font-m filterFlyOutNoResults\"><span>" + oGlobalConstants.FilterFlyout_NoResult + "</span></div>");
                    }
                } else {
                    showCommonErrorPopUp(searchResults.code);
                }
            }
        },
        hasValue: function (sValue, oFilterArray) {
            var nIndex = -1;
            var result = nIndex;
            $.each(oFilterArray, function (index, value) {
                if (result === nIndex && value.toLowerCase() === sValue.toLowerCase()) {
                    result = index;
                }
            });
            return result;
        },
        /* Function to get the data for specific column */
        getColumnData: function (sRefinerValue, sFilterFlyoutType, sFilterSearchTerm) {
            "use strict";
            var currentView = oCommonObject.isMatterView ? oGlobalConstants.Matter_ID : oGlobalConstants.Document_ID;
            sFilterSearchTerm = sFilterSearchTerm + oCommonObject.formatSearchText(oCommonObject.sSearchedKeyword, currentView);
            var oFilterDetails = {
                ClientsList: [], PGList: "", AOLList: "", FromDate: "", ToDate: "", DocumentAuthor: [], FilterByMe: 1
            };
            var oParam = { "refinerValue": sRefinerValue, "filterFlyoutType": sFilterFlyoutType }
            , oSortDetails = { ByProperty: sRefinerValue, Direction: 0 } //// Used for server side sorting           
            , oSearchDetails;

            if (1 === oCommonObject.iCurrentGridViewData()) { //// If All Matters section (flag is 1) is selected, then set filter by me flag to 0
                oFilterDetails.FilterByMe = 0;
            }

            //// Add the column filters in the filter object
            $.extend(oFilterDetails, oCommonObject.oFlyoutFilters());

            var oCurrentFilter = jQuery.extend(true, {}, oFilterDetails); //// This is used so as not to update the current Filter

            //// Remove the current filter based on the refiner added
            oCurrentFilter = commonConstants.updateFlyoutFilters(sRefinerValue, sFilterFlyoutType, true, oCurrentFilter, null);

            var sServiceCallName = (oCommonObject.isMatterView) ? "FindMatter" : "FindDocument";
            oSearchDetails = {
                "requestObject": {
                    "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
                }, "client": {
                    "Url": oCommonObject.getDeployedUrl()
                }, "searchObject": {
                    "PageNumber": commonConstants.iFilterFlyoutPageNumber, "ItemsPerPage": oGlobalConstants.FilterFlyoutItemsPerCall, "SearchTerm": sFilterSearchTerm, "Filters": oCurrentFilter, "Sort": oSortDetails
                }
            };
            oCommonObject.callSearchService(sServiceCallName, oSearchDetails, commonConstants.getColumnDataSuccess, commonConstants.getColumnDataFailure, commonConstants.getColumnDataOnBefore, oParam);
        },
        /* Function to format the date which is required by KQL */
        formatDateForKQL: function (sCurrentDate) {
            if ($.trim(sCurrentDate)) {
                sCurrentDate = $.datepicker.formatDate("yy-mm-dd", new Date(sCurrentDate)); //// yy-mm-dd is the required format for KQL to work, hence this will not come from resource file
            } else {
                sCurrentDate = "";
            }
            return sCurrentDate;
        },
        flyoutSearchClick: function () {
            oCommonObject.isAllRowSelected = false;
            oCommonObject.iFilterFlyoutPageNumber(); //// Reset the page number of filter fly out to 1
            $("#filterResultsContainer").empty();
            oCommonObject.isSearchText = 0;
            var oFilterFlyOut = $("#textFlyoutContent");
            if (oFilterFlyOut.length) {
                var sRefinerName = oFilterFlyOut.attr("data-refinername")
                , sFilterFlyoutType = oFilterFlyOut.attr("data-filterflyouttype");
                if (4 === oCommonObject.iCurrentGridViewData()) {
                    var sCurrentFilterText = $(".filterFlyoutSearchText").val().toLowerCase(); //// Current text in search panel
                    oGridViewObject.filterData; /// unfiltered data from the filter fly out
                    var arrFilteredData = [];
                    $.each(oGridViewObject.filterData, function (iCount, sCurrentValue) {
                        if (-1 < sCurrentValue.toLowerCase().indexOf(sCurrentFilterText)) {
                            arrFilteredData.push(sCurrentValue);
                        }
                        oCommonObject.populateFilterHTML(sRefinerName, sFilterFlyoutType, arrFilteredData);
                    });
                } else {
                    if ($.trim(sRefinerName) && $.trim(sFilterFlyoutType)) {
                        oCommonObject.arrUniqueRefinerData().length = 0; //// Refresh the array of values for filter fly out
                        var sFilterSearchTerm = oCommonObject.formatFilterSearchTerm(sRefinerName);
                        commonConstants.getColumnData(sRefinerName, sFilterFlyoutType, sFilterSearchTerm);
                    }
                }
            }
        },
        /* Function to bind events to filter fly out */
        addFilterFlyoutBindings: function () {
            //// Function to clear selected filters from fly out
            $("#dateFlyoutContent .clearFilterText, #textFlyoutContent .clearFilterText").on("click", function () {
                $("#textFlyoutContent, #dateFlyoutContent").addClass("hide"); //// Hide the filter fly out
                var oThisObject = $(this)
                    , sCurrentFlyOutType = oThisObject && oThisObject.attr("data-clearfiltertype")
                    , oFilterFlyOut
                    , oGridRow = {};
                if (4 === oCommonObject.iCurrentGridViewData()) {
                    if ("text" === sCurrentFlyOutType) {
                        if (!oCommonObject.isMatterView) {
                            delete oPinnedFlyoutFilters.oSearchDocumentFilters[$("#textFlyoutContent").attr("data-refinername")];
                            commonConstants.filterData(oPinnedFlyoutFilters.oSearchDocumentFilters, false, sRefinerName);
                        } else {
                            delete oPinnedFlyoutFilters.oSearchMatterFilters[$("#textFlyoutContent").attr("data-refinername")];
                            commonConstants.filterData(oPinnedFlyoutFilters.oSearchMatterFilters, false, sRefinerName);
                        }
                    } else if ("date" === sCurrentFlyOutType) {
                        if (!oCommonObject.isMatterView) {
                            delete oPinnedFlyoutFilters.oSearchDocumentFilters[$("#dateFlyoutContent").attr("data-refinername")];
                            commonConstants.filterData(oPinnedFlyoutFilters.oSearchDocumentFilters, false, sRefinerName);
                        } else {
                            delete oPinnedFlyoutFilters.oSearchMatterFilters[$("#dateFlyoutContent").attr("data-refinername")];
                            commonConstants.filterData(oPinnedFlyoutFilters.oSearchMatterFilters, false, sRefinerName);
                        }
                    }
                    oGridRow = $(".GridRow, .GridRowAlternate");
                    oGridView.highlightGridViewRow(oGridRow, false);
                    $("#attachDocuments").addClass("is-disabled");
                } else {
                    if ("text" === sCurrentFlyOutType) {
                        oFilterFlyOut = $("#textFlyoutContent");
                    } else if ("date" === sCurrentFlyOutType) {
                        oFilterFlyOut = $("#dateFlyoutContent");
                    }
                    if (oFilterFlyOut.length) {
                        var sRefinerName = oFilterFlyOut.attr("data-refinername")
                            , sFilterFlyoutType = oFilterFlyOut.attr("data-filterflyouttype");
                        commonConstants.updateFlyoutFilters(sRefinerName, sFilterFlyoutType, true, oFlyoutFilters, oThisObject);
                    }
                    /* Pop out the value of current refiner  */
                    commonConstants.arrCurrentRefiner = $.grep(commonConstants.arrCurrentRefiner, function (sCurrentValue) {
                        return sCurrentValue !== sRefinerName;
                    });
                    commonConstants.clearGridViewContent();
                    (oCommonObject.isMatterView) ? getSearchMatters($("#gridView"), 1) : getSearchDocuments($("#gridView"), 1);
                }
            });

            //// Function to perform a search inside filter fly out
            $("#filterSearchIcon .ms-Icon--search").off().on("click", function (event) {
                commonConstants.flyoutSearchClick();
                event && event.stopPropagation();
            });

            //// Function to search in text box on click of enter
            $(".filterFlyoutSearchText").off().on("keyup", function (event) {
                if (13 === event.keyCode) {//// If enter key is pressed
                    commonConstants.flyoutSearchClick();
                } else {
                    var doneTypingInterval = 1000;  ////time in ms,  1 second
                    ////On keyup, start the countdown     
                    var nSearchTextLength = $(".filterFlyoutSearchText").val().trim().length;
                    if (3 <= nSearchTextLength) {
                        clearTimeout(commonConstants.iTypingTimer);
                        commonConstants.iTypingTimer = setTimeout(commonConstants.flyoutSearchClick, doneTypingInterval);
                    }
                }

            });

            //// Function to search on click of values in filter fly out
            $(document).on("click", ".filterValueLabels", function (event) {
                if ((4 === oCommonObject.iCurrentGridViewData()) || (event && event.target && event.target.className.indexOf("filterValueCheckbox") < 0)) { //// If click is not triggered by fabric component
                    var oFilterFlyOut = $("#textFlyoutContent")
                        , oThisObject = $(this);
                    if (oFilterFlyOut.length && oThisObject.length) {
                        var sRefinerName = oFilterFlyOut.attr("data-refinername")
                            , sFilterFlyoutType = oFilterFlyOut.attr("data-filterflyouttype");
                        if (-1 === $.inArray(sRefinerName, commonConstants.arrCurrentRefiner)) {
                            commonConstants.arrCurrentRefiner.push(sRefinerName);
                        }
                        if ($.trim(sRefinerName) && $.trim(sFilterFlyoutType)) {
                            if (oCommonObject.isMatterView) {
                                ////#region Search Matters
                                if (4 === oCommonObject.iCurrentGridViewData()) {
                                    if ("singleselect" === sFilterFlyoutType.toLowerCase()) {
                                        switch (sRefinerName) {
                                            case oFindMatterConstants.GridViewPinnedTitleProperty:
                                                oPinnedFlyoutFilters.oSearchMatterFilters.MatterName = oThisObject.find("span").attr("title");
                                                commonConstants.filterData(oPinnedFlyoutFilters.oSearchMatterFilters, false, sRefinerName);
                                                break;
                                            case oFindMatterConstants.FilterRefinerPinnedClientName:
                                                oPinnedFlyoutFilters.oSearchMatterFilters.MatterClient = oThisObject.find("span").attr("title");
                                                commonConstants.filterData(oPinnedFlyoutFilters.oSearchMatterFilters, false, sRefinerName);
                                                break;
                                            case oFindMatterConstants.FilterRefinerPinnedSubAreaofLaw:
                                                oPinnedFlyoutFilters.oSearchMatterFilters.MatterSubAreaOfLaw = oThisObject.find("span").attr("title");
                                                commonConstants.filterData(oPinnedFlyoutFilters.oSearchMatterFilters, false, sRefinerName);
                                                break;
                                            case oFindMatterConstants.FilterRefinerPinnedResponsibleAttorney:
                                                oPinnedFlyoutFilters.oSearchMatterFilters.MatterResponsibleAttorney = oThisObject.find("span").attr("title");
                                                commonConstants.filterData(oPinnedFlyoutFilters.oSearchMatterFilters, false, sRefinerName);
                                                break;
                                        }
                                    }
                                } else {
                                    commonConstants.updateFlyoutFilters(sRefinerName, sFilterFlyoutType, false, null, oThisObject);
                                    commonConstants.clearGridViewContent();
                                    getSearchMatters($("#gridView"), 1);
                                }
                                ////#endregion
                            } else {
                                ////#region Search Documents
                                if (4 === oCommonObject.iCurrentGridViewData()) {
                                    if ("singleselect" === sFilterFlyoutType.toLowerCase()) {
                                        switch (sRefinerName) {
                                            case oFindDocumentConstants.GridViewPinnedTitleProperty:
                                                oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentName = oThisObject.find("span").attr("title");
                                                commonConstants.filterData(oPinnedFlyoutFilters.oSearchDocumentFilters, false, sRefinerName);
                                                break;
                                            case oFindDocumentConstants.FilterRefinerPinnedDocumentClient:
                                                oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentClient = oThisObject.find("span").attr("title");
                                                commonConstants.filterData(oPinnedFlyoutFilters.oSearchDocumentFilters, false, sRefinerName);
                                                break;
                                            case oFindDocumentConstants.FilterRefinerPinnedAuthor:
                                                oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentOwner = oThisObject.find("span").attr("title");
                                                commonConstants.filterData(oPinnedFlyoutFilters.oSearchDocumentFilters, false, sRefinerName);
                                                break;
                                            case oFindDocumentConstants.FilterRefinerPinnedCheckOutUser:
                                                oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentCheckoutUser = oThisObject.find("span").attr("title");
                                                commonConstants.filterData(oPinnedFlyoutFilters.oSearchDocumentFilters, false, sRefinerName);
                                                break;

                                        }
                                    }
                                    $("#attachDocuments").addClass("is-disabled");
                                } else {
                                    commonConstants.updateFlyoutFilters(sRefinerName, sFilterFlyoutType, false, null, oThisObject);
                                    commonConstants.clearGridViewContent();
                                    getSearchDocuments($("#gridView"), 1);
                                }
                                //// #endregion
                            }
                        }
                    }
                }
            });

            $(document).on("click", ".filterFlyoutText", function (event) {
                event && event.stopPropagation();
            });

            //// Function to perform operation on click of search field in filter fly out
            $("#filterSearchBarHolder .filterFlyoutSearchText, #filterSearchBarHolder #filterTextPlaceHolder").on("click", function (event) {
                $("#filterTextPlaceHolder").hide();
                event && event.stopPropagation();
            });

            var $textField = $("#textFlyoutContent .ms-TextField");
            /* Bind JS of fabric component */
            $textField.textField(); /* Binding click event on search text box */
        },
        /* Function to format search term entered in the search box inside filter fly out */
        formatFilterSearchTerm: function (sRefinerName) {
            var ofilterSearchText = $(".filterFlyoutSearchText").length ? $(".filterFlyoutSearchText").val() : ""
              , sSearchQueryTerm = "";
            if ($.trim(ofilterSearchText)) {
                sSearchQueryTerm = sRefinerName + ":" + ofilterSearchText + "*";
            }
            return sSearchQueryTerm;
        },
        /* Function to filter data in pinned section */
        filterData: function (filter, bflag, sRefinerName) {
            var results = [], flag = true;
            $.each(oCommonObject.pinnedData, function (key, value) {
                $.each(filter, function (key, val) {
                    flag = true;
                    if (key != null && !(bflag && key === sRefinerName)) {
                        if ((!oCommonObject.isMatterView && (key === oFindDocumentConstants.FilterRefinerPinnedModifiedDate || key === oFindDocumentConstants.FilterRefinerPinnedCreatedDate)) || ((oCommonObject.isMatterView && (key === oFindMatterConstants.FilterRefinerPinnedCreatedDate || key === oFindMatterConstants.FilterRefinerPinnedModifiedDate)))) {
                            var sFromDate = (filter[key].sFromDate !== "") ? new Date(filter[key].sFromDate) : null;
                            var sToDate = (filter[key].sToDate !== "") ? new Date(filter[key].sToDate) : null;
                            var columnDate = new Date(value[key]);
                            if (sFromDate && sToDate) {
                                if (!(columnDate > sFromDate && columnDate < sToDate)) {
                                    flag = false;
                                    return false;
                                }
                            } else if (sFromDate && !sToDate) {
                                if (columnDate < sFromDate) {
                                    flag = false;
                                    return false;
                                }
                            } else if (!sFromDate && sToDate) {
                                if (columnDate > sToDate) {
                                    flag = false;
                                    return false;
                                }
                            }
                        } else {
                            if (-1 === value[key].indexOf(val)) {
                                flag = false;
                                return false;
                            }
                        }
                    }
                });
                if (bflag) {
                    var arrSplitValues = [];
                    if ($.trim(value[sRefinerName])) {
                        arrSplitValues = value[sRefinerName] && value[sRefinerName].split(";");
                    }
                    $.each(arrSplitValues, function (iCurrentCount, sValue) {
                        var sTrimmedValue = $.trim(sValue);
                        (flag && $.inArray(sTrimmedValue, oGridViewObject.filterData) === -1) && oGridViewObject.filterData.push(sTrimmedValue);
                    });
                } else {
                    flag && results.push(value);
                }

            });
            oCommonObject.pinnedFilterData = results;
            if (!bflag) {
                var bNoResults = false; // Flag to check if there are no results
                if (oCommonObject.pinnedFilterData.length === 0) {
                    oCommonObject.pinnedFilterData.push(oCommonObject.pinnedData[0]);   // Add first object to get the structure
                    bNoResults = true;
                }
                var gridViewJSON = oGridView.generateGridViewJSON();
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
                    //// Generate the common JSON structure for generating the grid view control
                    var GridConfig = {
                        container: "gridViewContainer",
                        data: results,
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
                        altRowColor: "white",
                        cellSpacing: 0
                    };
                    $("#gridViewContainer").empty();
                    new oGrid.JsonGrid(GridConfig);
                    oGridView.bindECB();
                    oGridView.addGridViewControlBindings();
                    oCommonObject.configureOnLoadView(); //// Display the columns as per selection using column picker
                    //// Display the filter icon on the column header
                    var oFilteredKeys = {
                    };
                    if (oCommonObject.isMatterView) { // Show filter for matters
                        oFilteredKeys = Object.keys(oPinnedFlyoutFilters.oSearchMatterFilters);
                    } else { // Show filter for documents
                        oFilteredKeys = Object.keys(oPinnedFlyoutFilters.oSearchDocumentFilters);
                    }
                    $.each(oFilteredKeys, function (sCurrentIndex, sCurrentValue) {
                        $(".jsonGridHeader[id=" + sCurrentValue + "]").find(".ms-Icon--filter").removeClass("hide");
                    });
                }
                if (bNoResults) { // Check if no results to show generic message
                    oCommonObject.pinnedFilterData.pop(); // Remove dummy element        
                    oGrid.gridObject[0].data.pop(); // Remove dummy element
                    $("#gridViewContainer_Grid tbody tr").removeClass("hide").addClass("invisible");
                    $("#gridViewContainer_Grid tbody").append(oGlobalConstants.No_Results_Message);
                }
            }
        },
        /* Function to clear the fly out filters when search using advance search */
        clearFlyOutFilters: function (bIsAdvanceSearchCall) {
            if (bIsAdvanceSearchCall) { //// Clear all the filters applied on grid view
                if (oFlyoutFilters && oFlyoutFilters.DateFilters) {
                    oFlyoutFilters.Name = "";
                    oFlyoutFilters.ClientName = "";
                    oFlyoutFilters.DateFilters.ModifiedFromDate = "";
                    oFlyoutFilters.DateFilters.ModifiedToDate = "";
                    oFlyoutFilters.DateFilters.CreatedFromDate = "";
                    oFlyoutFilters.DateFilters.CreatedToDate = "";
                    oFlyoutFilters.DateFilters.OpenDateFrom = "";
                    oFlyoutFilters.DateFilters.OpenDateTo = "";
                    oFlyoutFilters.ResponsibleAttorneys = [];
                    oFlyoutFilters.SubareaOfLaw = "";
                    oFlyoutFilters.DocumentCheckoutUsers = [];
                    oFlyoutFilters.DocumentAuthor = [];
                }
            }
        },
        /* Function to clear the content of grid view */
        clearGridViewContent: function () {
            oGridViewObject.pageNumber = 1; //// Reset the page number for grid view control
            $("#gridViewContainer").off("scroll").empty(); //// Unbind the scroll event on the grid view and Remove the data from the grid view            
            $("#loadingImageContainer").removeClass("hide"); //// Hide the loading image of grid view control
        },
        /* Function to configure on load view for grid view control */
        configureOnLoadView: function () {
            //// This function will need to be modified in future, when user selection persistence will be implemented
            var arrListHeaders = oCommonObject.isMatterView ? commonConstants.getArrayDifference(oFindMatterConstants.GridViewHeaderName.split(";"), oFindMatterConstants.GridViewNonDataColumns.split(";")) : commonConstants.getArrayDifference(oFindDocumentConstants.GridViewHeaderName.split(";"), oFindDocumentConstants.GridViewNonDataColumns.split(";"));
            var nListHeaderCount = arrListHeaders.length;
            var sCheckBoxSelector = "#options-checkbox-unselected";
            var arrDefaultColumns = [];
            var nNonDataColumns = oCommonObject.isMatterView ? oFindMatterConstants.GridViewNonDataColumns.split(";").length : oFindDocumentConstants.GridViewNonDataColumns.split(";").length;
            var nHeaderOptionMapping = 0, iIterator = 0;
            if (oGridViewObject && !oGridViewObject.bDefaultSelectionChanged) {
                arrDefaultColumns = oCommonObject.isMatterView ? oFindMatterConstants.GridViewDefaultColumns.split(";") : oFindDocumentConstants.GridViewDefaultColumns.split(";");
                for (iIterator = 1; iIterator <= nListHeaderCount - 1; iIterator++) {
                    if (-1 === $.inArray(arrListHeaders[iIterator], arrDefaultColumns)) {
                        nHeaderOptionMapping = iIterator + nNonDataColumns + 1;
                        $(".dataColumn" + (parseInt(iIterator) + 1)).parent().addClass("hide");
                        $(".jsonGridHeader:nth-child(" + nHeaderOptionMapping + ")").addClass("hide");
                        $(sCheckBoxSelector + iIterator).length ? $(sCheckBoxSelector + iIterator)[0].checked = false : "";
                        $(sCheckBoxSelector).length ? $(sCheckBoxSelector)[0].checked = false : "";
                    } else {
                        $(sCheckBoxSelector + iIterator).length ? $(sCheckBoxSelector + iIterator)[0].checked = true : "";
                    }
                }
            } else if (oGridViewObject && oGridViewObject.bDefaultSelectionChanged && 0 < oGridViewObject.arrUserSelectedColumns.length) {
                arrDefaultColumns = oGridViewObject.arrUserSelectedColumns;
                for (iIterator = 1; iIterator <= nListHeaderCount - 1; iIterator++) {
                    if (-1 === $.inArray(arrListHeaders[iIterator].split(",")[0], arrDefaultColumns)) {
                        nHeaderOptionMapping = iIterator + nNonDataColumns + 1;
                        $(".dataColumn" + (parseInt(iIterator) + 1)).parent().addClass("hide");
                        $(".jsonGridHeader:nth-child(" + nHeaderOptionMapping + ")").addClass("hide");
                        $(sCheckBoxSelector + iIterator).length ? $(sCheckBoxSelector + iIterator)[0].checked = false : "";
                        $(sCheckBoxSelector).length ? $(sCheckBoxSelector)[0].checked = false : "";
                    } else {
                        $(sCheckBoxSelector + iIterator).length ? $(sCheckBoxSelector + iIterator)[0].checked = true : "";
                    }
                }
                var nSelectedColumnCount = $("#columnOptions").find("input[type='checkbox']:checked").length;
                var nTotalColumnCount = $("#columnOptions").find("input[type='checkbox']").length;
                if (nTotalColumnCount - 1 === nSelectedColumnCount) {
                    $("#columnOptions").find("#options-checkbox-unselected").prop("checked", true);
                }
            } else {
                for (var colIndex = 2; colIndex <= nListHeaderCount; colIndex++) {
                    if (0 < $(".dataColumn" + colIndex).length && $(".dataColumn" + colIndex).parent().hasClass("hide")) {
                        $(".dataColumn" + colIndex).parent().each(function () { $(this).addClass("hide"); });
                    }
                }
            }
            commonConstants.adjustCaretIcon();
        },
        /* Function to get the difference between two arrays */
        getArrayDifference: function (firstArray, secondArray) {
            return $.grep(firstArray, function (item) {
                return 0 > $.inArray(item, secondArray);
            });
        },
        /* Function to update fly out filters */
        updateFlyoutFilters: function (sRefinerName, sFilterFlyoutType, bClearCurrentFilter, oFilterDetails, oThisObject) {
            if ("date" === sFilterFlyoutType) {
                if (sRefinerName === oGlobalConstants.Last_Modified_Time || sRefinerName === oGlobalConstants.Document_Last_Modified_Time) {
                    oFlyoutFilters.DateFilters.ModifiedFromDate = "";
                    oFlyoutFilters.DateFilters.ModifiedToDate = "";
                } else if (oCommonObject.isMatterView && sRefinerName === oFindMatterConstants.FilterRefinerOpenDate) {
                    oFlyoutFilters.DateFilters.OpenDateFrom = "";
                    oFlyoutFilters.DateFilters.OpenDateTo = "";
                } else if (!oCommonObject.isMatterView && sRefinerName === oFindDocumentConstants.FilterRefinerCreated) {
                    oFlyoutFilters.DateFilters.CreatedFromDate = "";
                    oFlyoutFilters.DateFilters.CreatedToDate = "";
                }
                return oFilterDetails;
            } else {
                if (oCommonObject.isMatterView) {
                    if ("singleselect" === sFilterFlyoutType.toLowerCase()) {
                        switch (sRefinerName) {
                            case oFindMatterConstants.GridViewTitleProperty: //// Matter
                                if (bClearCurrentFilter) {
                                    oFilterDetails.Name = "";
                                } else {
                                    oFlyoutFilters.Name = oThisObject.text();
                                }
                                break;
                            case oFindMatterConstants.FilterRefinerClientName: //// Client
                                if (bClearCurrentFilter) {
                                    oFilterDetails.ClientName = "";
                                } else {
                                    oFlyoutFilters.ClientName = oThisObject.text();
                                }
                                break;
                            case oFindMatterConstants.FilterRefinerSubAreaofLaw: //// Sub area of law
                                if (bClearCurrentFilter) {
                                    oFilterDetails.SubareaOfLaw = "";
                                } else {
                                    oFlyoutFilters.SubareaOfLaw = oThisObject.text();
                                }
                                break;
                            case oFindMatterConstants.FilterRefinerResponsibleAttorney: //// Responsible Attorney
                                if (bClearCurrentFilter) {
                                    oFilterDetails.ResponsibleAttorneys = "";
                                } else {
                                    oFlyoutFilters.ResponsibleAttorneys = oThisObject.text();
                                }
                                break;
                        }
                    }
                } else {
                    if ("singleselect" === sFilterFlyoutType.toLowerCase()) {
                        switch (sRefinerName) {
                            case oFindDocumentConstants.GridViewTitleProperty: //// Document
                                if (bClearCurrentFilter) {
                                    oFilterDetails.Name = "";
                                } else {
                                    oFlyoutFilters.Name = oThisObject.text();
                                }
                                break;
                            case oFindDocumentConstants.FilterRefinerDocumentClient: //// Client
                                if (bClearCurrentFilter) {
                                    oFilterDetails.ClientName = "";
                                } else {
                                    oFlyoutFilters.ClientName = oThisObject.text();
                                }
                                break;
                            case oFindDocumentConstants.FilterRefinerAuthor: //// Author
                                if (bClearCurrentFilter) {
                                    oFilterDetails.DocumentAuthor = "";
                                } else {
                                    oFlyoutFilters.DocumentAuthor = oThisObject.text();
                                }
                                break;
                            case oFindDocumentConstants.FilterRefinerCheckOutUser: //// Check out user
                                if (bClearCurrentFilter) {
                                    oFilterDetails.DocumentCheckoutUsers = "";
                                } else {
                                    oFlyoutFilters.DocumentCheckoutUsers = oThisObject.text();
                                }
                                break;
                        }
                    }
                }
            }
            return oFilterDetails;
        },
        /* Function to filter grid view on date filters */
        filterGridViewOnDate: function (sFromDate, sToDate) {
            var sRefinerName = $("#dateFlyoutContent").attr("data-refinername")
                , sFilterFlyoutType = $("#dateFlyoutContent").attr("data-filterflyouttype");
            if (4 === oCommonObject.iCurrentGridViewData()) {
                if (oCommonObject.isMatterView) {
                    switch (sRefinerName) {
                        case oFindMatterConstants.FilterRefinerPinnedModifiedDate:
                            oPinnedFlyoutFilters.oSearchMatterFilters.MatterModifiedDate = {
                            };
                            oPinnedFlyoutFilters.oSearchMatterFilters.MatterModifiedDate.sFromDate = sFromDate;
                            oPinnedFlyoutFilters.oSearchMatterFilters.MatterModifiedDate.sToDate = sToDate;
                            commonConstants.filterData(oPinnedFlyoutFilters.oSearchMatterFilters, false, sRefinerName);
                            break;
                        case oFindMatterConstants.FilterRefinerPinnedCreatedDate:
                            oPinnedFlyoutFilters.oSearchMatterFilters.MatterCreatedDate = {
                            };
                            oPinnedFlyoutFilters.oSearchMatterFilters.MatterCreatedDate.sFromDate = sFromDate;
                            oPinnedFlyoutFilters.oSearchMatterFilters.MatterCreatedDate.sToDate = sToDate;
                            commonConstants.filterData(oPinnedFlyoutFilters.oSearchMatterFilters, false, sRefinerName);
                            break;
                    }
                } else {
                    switch (sRefinerName) {
                        case oFindDocumentConstants.FilterRefinerPinnedModifiedDate:
                            oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentModifiedDate = {
                            };
                            oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentModifiedDate.sFromDate = sFromDate;
                            oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentModifiedDate.sToDate = sToDate;
                            commonConstants.filterData(oPinnedFlyoutFilters.oSearchDocumentFilters, false, sRefinerName);
                            break;
                        case oFindDocumentConstants.FilterRefinerPinnedCreatedDate:
                            oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentCreatedDate = {
                            };
                            oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentCreatedDate.sFromDate = sFromDate;
                            oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentCreatedDate.sToDate = sToDate;
                            commonConstants.filterData(oPinnedFlyoutFilters.oSearchDocumentFilters, false, sRefinerName);
                            break;
                    }
                }
            } else {
                // Append time to "from" part of date if not null
                if ($.trim(sFromDate)) {
                    sFromDate += oGlobalConstants.From_Date_Append_Time;
                }
                // Append time to "To" part of date if not null
                if ($.trim(sToDate)) {
                    sToDate += oGlobalConstants.To_Date_Append_Time;
                }
                if (sRefinerName === oGlobalConstants.Last_Modified_Time || sRefinerName === oGlobalConstants.Document_Last_Modified_Time) {
                    oFlyoutFilters.DateFilters.ModifiedFromDate = sFromDate;
                    oFlyoutFilters.DateFilters.ModifiedToDate = sToDate;

                } else if (oCommonObject.isMatterView && sRefinerName === oFindMatterConstants.FilterRefinerOpenDate) {
                    oFlyoutFilters.DateFilters.OpenDateFrom = sFromDate;
                    oFlyoutFilters.DateFilters.OpenDateTo = sToDate;
                } else if (!oCommonObject.isMatterView && sRefinerName === oFindDocumentConstants.FilterRefinerCreated) {
                    oFlyoutFilters.DateFilters.CreatedFromDate = sFromDate;
                    oFlyoutFilters.DateFilters.CreatedToDate = sToDate;
                }
                if (-1 === $.inArray(sRefinerName, commonConstants.arrCurrentRefiner)) {
                    commonConstants.arrCurrentRefiner.push(sRefinerName);
                }
                commonConstants.clearGridViewContent();
                (oCommonObject.isMatterView) ? getSearchMatters($("#gridView"), 1) : getSearchDocuments($("#gridView"), 1);
            }
        },
        /* Function to get the current sort details */
        getSortDetails: function (bIsFilterFlyoutCall) {
            if (!bIsFilterFlyoutCall) {
                if (1 === oCommonObject.iCurrentGridViewData()) { //// If All Matters/Documents section (flag is 1) is selected, then update the Sort filter
                    oCommonObject.oSortDetails().ByProperty = (oCommonObject.isMatterView) ? oFindMatterConstants.GridViewTitleProperty : oFindDocumentConstants.GridViewTitleProperty;
                    oCommonObject.oSortDetails().Direction = 0;
                } else if (2 === oCommonObject.iCurrentGridViewData()) { //// If My Matters/Documents section (flag is 2) is selected, then update the Sort filter
                    oCommonObject.oSortDetails().ByProperty = oCommonObject.isMatterView ? oGlobalConstants.Last_Modified_Time : oGlobalConstants.Document_Last_Modified_Time;
                    oCommonObject.oSortDetails().Direction = 1;
                } else {
                    oCommonObject.oSortDetails().ByProperty = "";
                    oCommonObject.oSortDetails().Direction = 0;
                }
            }
        },
        /* Function to update the positioning of the sort arrow based on the sort applied */
        updateSortingNotification: function () {
            var oCurrentSortObject = oCommonObject.oSortDetails();
            if ($.trim(oCurrentSortObject.ByProperty)) {
                //// Update the sorting notification in grid view header
                var oCurrentHeader = $(".jsonGridHeader[id=" + oCurrentSortObject.ByProperty + "]");
                if (oCurrentSortObject.Direction) { //// Descending sort applied
                    oCurrentHeader.find(".sort").html("&#x2193;").removeClass("hide");
                } else { //// Ascending sort applied
                    oCurrentHeader.find(".sort").html("&#x2191;").removeClass("hide");
                }
            }
            //// Update the filter notification in grid view header
            $.each(commonConstants.arrCurrentRefiner, function (iCurrentIndex, sCurrentValue) {
                $(".jsonGridHeader[id=" + sCurrentValue + "]").find(".ms-Icon--filter").removeClass("hide");
            });
        },
        /* Function to bind scroll event on filter fly out */
        bindScrollOnFilterFlyout: function () {
            $("#filterResultsContainer").on("scroll", function (event) {
                if (4 !== oCommonObject.iCurrentGridViewData()) {  //// no lazy loading for pinned section (flag = 4)
                    var oFilterObject = $(this); //// Filter Results container object
                    if (oFilterObject.length) {
                        var filterBodyVisibleHeight = oFilterObject.height();
                        var filterScrollableHeight = oFilterObject.prop("scrollHeight");
                        var filterHiddenContentHeight = filterScrollableHeight - filterBodyVisibleHeight;
                        //// Check if the remaining scrollable area (height of the hidden content - scroll top based upon overall content) is less then the threshold, to make the service call
                        if (!commonConstants.waitTillFilterDataLoaded && (filterHiddenContentHeight - oFilterObject.scrollTop() <= (oFilterObject.height() - (1 - (oGlobalConstants.FilterLazyLoadingLimit / 100)) * oFilterObject.height()))) {
                            var iMaxCurrentData = parseInt(oGlobalConstants.FilterFlyoutItemsPerCall) * (commonConstants.iFilterFlyoutPageNumber);
                            if (commonConstants.iColumnDataTotalResults > iMaxCurrentData) {
                                var oFilterFlyoutObject = $("#textFlyoutContent");
                                if (oFilterFlyoutObject.length) {
                                    var sRefinerName = oFilterFlyoutObject.attr("data-refinername")
                                        , sFilterFlyoutType = oFilterFlyoutObject.attr("data-filterflyouttype")
                                        , sFilterSearchTerm = oCommonObject.formatFilterSearchTerm(sRefinerName);
                                    commonConstants.waitTillFilterDataLoaded = true; //// Throttle the next request till current service request is completed
                                    commonConstants.iFilterFlyoutPageNumber++;
                                    commonConstants.getColumnData(sRefinerName, sFilterFlyoutType, sFilterSearchTerm);
                                }
                            }
                        }
                    }
                }
            });
        },
        /* Function to highlight selected filters inside filter fly out */
        highlightSelectedFilters: function (sRefinerName, sFilterFlyoutType) {
            if (oCommonObject.isMatterView) {
                //// #region Matter View
                if (4 === oCommonObject.iCurrentGridViewData()) {
                    if (oPinnedFlyoutFilters && oPinnedFlyoutFilters.oSearchMatterFilters) {
                        var oCurrentSearchFilter = oPinnedFlyoutFilters.oSearchMatterFilters;
                        if ("singleselect" === sFilterFlyoutType.toLowerCase()) {
                            switch (sRefinerName) {
                                case oFindMatterConstants.GridViewPinnedTitleProperty:
                                    if (oCurrentSearchFilter.hasOwnProperty("MatterName")) {
                                        var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oPinnedFlyoutFilters.oSearchMatterFilters.MatterName + "']");
                                        if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                            oCurrentSelectedValue.parent().addClass("selectedValue");
                                        }
                                        oPinnedFlyoutFilters.oSearchMatterFilters.MatterName && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oPinnedFlyoutFilters.oSearchMatterFilters.MatterName);
                                    }
                                    break;
                                case oFindMatterConstants.FilterRefinerPinnedClientName:
                                    if (oCurrentSearchFilter.hasOwnProperty("MatterClient")) {
                                        var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oPinnedFlyoutFilters.oSearchMatterFilters.MatterClient + "']");
                                        if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                            oCurrentSelectedValue.parent().addClass("selectedValue");
                                        }
                                        oPinnedFlyoutFilters.oSearchMatterFilters.MatterClient && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oPinnedFlyoutFilters.oSearchMatterFilters.MatterClient);
                                    }
                                    break;
                                case oFindMatterConstants.FilterRefinerPinnedSubAreaofLaw:
                                    if (oCurrentSearchFilter.hasOwnProperty("MatterSubAreaOfLaw")) {
                                        var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oPinnedFlyoutFilters.oSearchMatterFilters.MatterSubAreaOfLaw + "']");
                                        if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                            oCurrentSelectedValue.parent().addClass("selectedValue");
                                        }
                                        oPinnedFlyoutFilters.oSearchMatterFilters.MatterSubAreaOfLaw && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oPinnedFlyoutFilters.oSearchMatterFilters.MatterSubAreaOfLaw);
                                    }
                                    break;
                                case oFindMatterConstants.FilterRefinerPinnedResponsibleAttorney:
                                    if (oCurrentSearchFilter.hasOwnProperty("MatterResponsibleAttorney")) {
                                        var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oPinnedFlyoutFilters.oSearchMatterFilters.MatterResponsibleAttorney + "']");
                                        if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                            oCurrentSelectedValue.parent().addClass("selectedValue");
                                        }
                                        oPinnedFlyoutFilters.oSearchMatterFilters.MatterResponsibleAttorney && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oPinnedFlyoutFilters.oSearchMatterFilters.MatterResponsibleAttorney);
                                    }
                                    break;
                            }
                        }
                    }
                } else {
                    if ("singleselect" === sFilterFlyoutType.toLowerCase()) {
                        switch (sRefinerName) {
                            case oFindMatterConstants.GridViewTitleProperty: //// Matter
                                var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oFlyoutFilters.Name + "']");
                                if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                    oCurrentSelectedValue.parent().addClass("selectedValue");
                                }
                                oFlyoutFilters.Name && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oFlyoutFilters.Name);
                                break;
                            case oFindMatterConstants.FilterRefinerClientName: //// Client
                                var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oFlyoutFilters.ClientName + "']");
                                if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                    oCurrentSelectedValue.parent().addClass("selectedValue");
                                }
                                oFlyoutFilters.ClientName && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oFlyoutFilters.ClientName);
                                break;
                            case oFindMatterConstants.FilterRefinerSubAreaofLaw: //// Sub area of law
                                var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oFlyoutFilters.SubareaOfLaw + "']");
                                if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                    oCurrentSelectedValue.parent().addClass("selectedValue");
                                }
                                oFlyoutFilters.SubareaOfLaw && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oFlyoutFilters.SubareaOfLaw);
                                break;
                            case oFindMatterConstants.FilterRefinerResponsibleAttorney: //// Responsible  Attorney
                                var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oFlyoutFilters.ResponsibleAttorneys + "']");
                                if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                    oCurrentSelectedValue.parent().addClass("selectedValue");
                                }
                                oFlyoutFilters.ResponsibleAttorneys && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oFlyoutFilters.ResponsibleAttorneys);
                                break;
                        }
                    }
                }
                //// #endregion
            } else {
                //// #region Document View
                if (4 === oCommonObject.iCurrentGridViewData()) {
                    if (oPinnedFlyoutFilters && oPinnedFlyoutFilters.oSearchDocumentFilters) {
                        var oCurrentSearchFilter = oPinnedFlyoutFilters.oSearchDocumentFilters;
                        if ("singleselect" === sFilterFlyoutType.toLowerCase()) {
                            switch (sRefinerName) {
                                case oFindDocumentConstants.GridViewPinnedTitleProperty:
                                    if (oCurrentSearchFilter.hasOwnProperty("DocumentName")) {
                                        var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentName + "']");
                                        if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                            oCurrentSelectedValue.parent().addClass("selectedValue");
                                        }
                                        oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentName && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentName);
                                    }
                                    break;
                                case oFindDocumentConstants.FilterRefinerPinnedDocumentClient:
                                    if (oCurrentSearchFilter.hasOwnProperty("DocumentClient")) {
                                        var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentClient + "']");
                                        if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                            oCurrentSelectedValue.parent().addClass("selectedValue");
                                        }
                                        oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentClient && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentClient);
                                    }
                                    break;
                                case oFindDocumentConstants.FilterRefinerPinnedAuthor:
                                    if (oCurrentSearchFilter.hasOwnProperty("DocumentOwner")) {
                                        var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentOwner + "']");
                                        if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                            oCurrentSelectedValue.parent().addClass("selectedValue");
                                        }
                                        oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentOwner && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentOwner);
                                    }
                                    break;
                                case oFindDocumentConstants.FilterRefinerPinnedCheckOutUser:
                                    if (oCurrentSearchFilter.hasOwnProperty("DocumentCheckoutUser")) {
                                        var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentCheckoutUser + "']");
                                        if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                            oCurrentSelectedValue.parent().addClass("selectedValue");
                                        }
                                        oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentCheckoutUser && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentCheckoutUser);
                                    }
                                    break;
                            }
                        }
                    }
                } else {
                    if ("singleselect" === sFilterFlyoutType.toLowerCase()) {
                        switch (sRefinerName) {
                            case oFindDocumentConstants.GridViewTitleProperty: //// Document
                                var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oFlyoutFilters.Name + "']");
                                if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                    oCurrentSelectedValue.parent().addClass("selectedValue");
                                }
                                oFlyoutFilters.Name && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oFlyoutFilters.Name);
                                break;
                            case oFindDocumentConstants.FilterRefinerDocumentClient: //// Client
                                var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oFlyoutFilters.ClientName + "']");
                                if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                    oCurrentSelectedValue.parent().addClass("selectedValue");
                                }
                                oFlyoutFilters.ClientName && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oFlyoutFilters.ClientName);
                                break;
                            case oFindDocumentConstants.FilterRefinerAuthor: //// Author
                                var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oFlyoutFilters.DocumentAuthor + "']");
                                if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                    oCurrentSelectedValue.parent().addClass("selectedValue");
                                }
                                oFlyoutFilters.DocumentAuthor && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oFlyoutFilters.DocumentAuthor);
                                break;
                            case oFindDocumentConstants.FilterRefinerCheckOutUser: //// CheckOut User
                                var oCurrentSelectedValue = $(".filterValueLabels").find("span[title='" + oFlyoutFilters.DocumentCheckoutUsers + "']");
                                if (oCurrentSelectedValue.length && oCurrentSelectedValue.parent().length) {
                                    oCurrentSelectedValue.parent().addClass("selectedValue");
                                }
                                oFlyoutFilters.DocumentCheckoutUsers && oCommonObject.isSearchText && $(".filterFlyoutSearchText").click().val(oFlyoutFilters.DocumentCheckoutUsers);
                                break;
                        }
                    }
                }
                ////#endregion
            }
        },
        /* Function to set the date values in the 'From' and 'To' field */
        setDateFields: function (oFromFilter, oToFilter, sFromFilterValue, sToFilterValue) {
            var oCurrentTime = new Date();
            var iIndex = 0;
            if ($.trim(sFromFilterValue)) {
                // Check if time value is present in the variable
                iIndex = sFromFilterValue.indexOf("T");
                if (-1 !== iIndex) {
                    sFromFilterValue = sFromFilterValue.substring(0, sFromFilterValue.indexOf("T"));
                }
                var sFromDate = new Date(sFromFilterValue.replace(/-/g, "/"));
                oFromFilter.val($.datepicker.formatDate(oGlobalConstants.DatePickerFormat, sFromDate));
            } else {
                oFromFilter.val("");
            }
            if ($.trim(sToFilterValue)) {
                // Check if time value is present in the variable
                iIndex = sToFilterValue.indexOf("T");
                if (-1 !== iIndex) {
                    sToFilterValue = sToFilterValue.substring(0, sToFilterValue.indexOf("T"));
                }
                var sToDate = new Date(sToFilterValue.replace(/-/g, "/"));
                oToFilter.val($.datepicker.formatDate(oGlobalConstants.DatePickerFormat, sToDate));
            } else {
                oToFilter.val("");
            }

            // Check if null values are set in date then disable the button
            if (!oToFilter.val() && !oFromFilter.val()) {
                $("#btnOK").removeClass("activateButton");
            }
            //// Set the min and max dates in the date picker control
            $("#dateFlyoutContent .filterFlyoutToText").datepicker("option", "minDate", oFromFilter.val());
            $("#dateFlyoutContent .filterFlyoutFromText").datepicker("option", "maxDate", oToFilter.val() || "+0D");
        },
        /* Function to set the date filters */
        setDateFilters: function (sRefinerName, sFilterFlyoutType) {
            var oFromDate = $(".filterFlyoutFromText"), oToDate = $(".filterFlyoutToText");
            oCommonObject.isAllRowSelected = false;
            if (oFromDate.length && oToDate.length) {
                if (4 === oCommonObject.iCurrentGridViewData()) { //// Update the filters for pinned section
                    if (oCommonObject.isMatterView) {
                        if (sRefinerName === oFindMatterConstants.FilterRefinerPinnedModifiedDate) {
                            if (oPinnedFlyoutFilters.oSearchMatterFilters.MatterModifiedDate) {
                                commonConstants.setDateFields(oFromDate, oToDate, oPinnedFlyoutFilters.oSearchMatterFilters.MatterModifiedDate.sFromDate, oPinnedFlyoutFilters.oSearchMatterFilters.MatterModifiedDate.sToDate);
                            }
                        } else if (sRefinerName === oFindMatterConstants.FilterRefinerPinnedCreatedDate) {
                            if (oPinnedFlyoutFilters.oSearchMatterFilters.MatterCreatedDate) {
                                commonConstants.setDateFields(oFromDate, oToDate, oPinnedFlyoutFilters.oSearchMatterFilters.MatterCreatedDate.sFromDate, oPinnedFlyoutFilters.oSearchMatterFilters.MatterCreatedDate.sToDate);
                            }
                        }
                    } else {
                        if (sRefinerName === oFindDocumentConstants.FilterRefinerPinnedModifiedDate) {
                            if (oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentModifiedDate) {
                                commonConstants.setDateFields(oFromDate, oToDate, oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentModifiedDate.sFromDate, oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentModifiedDate.sToDate);
                            }
                        } else if (sRefinerName === oFindDocumentConstants.FilterRefinerPinnedCreatedDate) {
                            if (oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentCreatedDate) {
                                commonConstants.setDateFields(oFromDate, oToDate, oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentCreatedDate.sFromDate, oPinnedFlyoutFilters.oSearchDocumentFilters.DocumentCreatedDate.sToDate);
                            }
                        }
                    }
                } else {
                    if (sRefinerName === oGlobalConstants.Last_Modified_Time || sRefinerName === oGlobalConstants.Document_Last_Modified_Time) {
                        commonConstants.setDateFields(oFromDate, oToDate, oFlyoutFilters.DateFilters.ModifiedFromDate, oFlyoutFilters.DateFilters.ModifiedToDate);
                    } else if (oCommonObject.isMatterView && sRefinerName === oFindMatterConstants.FilterRefinerOpenDate) {
                        commonConstants.setDateFields(oFromDate, oToDate, oFlyoutFilters.DateFilters.OpenDateFrom, oFlyoutFilters.DateFilters.OpenDateTo);
                    } else if (!oCommonObject.isMatterView && sRefinerName === oFindDocumentConstants.FilterRefinerCreated) {
                        commonConstants.setDateFields(oFromDate, oToDate, oFlyoutFilters.DateFilters.CreatedFromDate, oFlyoutFilters.DateFilters.CreatedToDate);
                    }
                }
            }
        },
        /* Updates the position of caret icon for last visible column in grid view */
        adjustCaretIcon: function () {
            $(".jsonGridHeader .ms-Icon--caretDown").length && $(".jsonGridHeader .ms-Icon--caretDown").removeClass("lastCaretIcon").addClass("firstCaretIcon");
            $(".jsonGridHeader:visible:last .ms-Icon--caretDown").length && $(".jsonGridHeader:visible:last .ms-Icon--caretDown").removeClass("firstCaretIcon").addClass("lastCaretIcon");
        },
        /* Function to move filter fly out inside grid view control */
        moveFilterFlyout: function () {
            var sTextFlyoutWrapper = "<div id=\"textFlyoutContent\"></div>"
                , sDateFlyoutWrapper = "<div id=\"dateFlyoutContent\"></div>";
            //// Remove the already present filter fly outs inside grid view
            $("#textFlyoutContent, #dateFlyoutContent").remove();
            //// Move both the date and text fly out inside filter fly outs
            $("#gridViewContainer").append(sTextFlyoutWrapper);
            $("#gridViewContainer").append(sDateFlyoutWrapper);
            //// Populate the filter fly outs with the HTML from the original fly outs
            $("#textFlyoutContent").html($(".textFlyoutContent").html());
            $("#dateFlyoutContent").html($(".dateFlyoutContent").html());
            //// Hide both the fly outs
            $("#textFlyoutContent, #dateFlyoutContent").addClass("hide");
            //// Bind the events on the new filter fly outs
            commonConstants.addFilterFlyoutBindings();
            commonConstants.bindDatePickerControl();
        },
        /* Bind the date picker */
        bindDatePickerControl: function () {
            $("#dateFlyoutContent .filterFlyoutFromText").datepicker({
                dateFormat: oGlobalConstants.DatePickerFormat,
                buttonImage: "../Images/Calendar_30px_X_30px_color_666.png",
                buttonImageOnly: true,
                showOn: "both",
                changeMonth: true,
                changeYear: true,
                yearRange: oGlobalConstants.DatePickerFromYearRange,
                constrainInput: true,
                duration: "",
                gotoCurrent: true,
                maxDate: oGlobalConstants.DatePickerToMaxDate,
                onClose: function (selectedDate) {
                    commonConstants.onCloseEvent(selectedDate, 0);
                }
            }).datepicker();
            $("#dateFlyoutContent .filterFlyoutToText, #dateFlyoutContent .filterImageToDatePicker").datepicker({
                dateFormat: oGlobalConstants.DatePickerFormat,
                buttonImage: "../Images/Calendar_30px_X_30px_color_666.png",
                buttonImageOnly: true,
                showOn: "both",
                changeMonth: true,
                changeYear: true,
                yearRange: oGlobalConstants.DatePickerToYearRange,
                constrainInput: true,
                duration: "",
                gotoCurrent: true,
                maxDate: oGlobalConstants.DatePickerToMaxDate,
                onClose: function (selectedDate) {
                    commonConstants.onCloseEvent(selectedDate, 1);
                }
            }).datepicker();
            /* Filtering of List according to Modified Date takes place on click of OK button*/
            $("#btnOK").click(function () {
                var sFromDate = $(".filterFlyoutFromText").val();
                var sToDate = $(".filterFlyoutToText").val();
                var sFromFormatDate = commonConstants.formatDateForKQL(sFromDate);
                var sToFormatDate = commonConstants.formatDateForKQL(sToDate);
                if ($.trim(sFromFormatDate) || $.trim(sToFormatDate)) {
                    commonConstants.filterGridViewOnDate($.trim(sFromFormatDate), $.trim(sToFormatDate));
                    $("#btnOK").addClass("activateButton");
                    $("#attachDocuments").addClass("is-disabled");
                } else {
                    $("#btnOK").removeClass("activateButton");
                }
            });
            $(".ui-datepicker").click(function (event) {
                commonFunction.closeAllFilterExcept("ui-datepicker", event);
                event.stopPropagation();
            });
        },
        /* Common Date Picker Function For Applying Limit on Max and Min Dates */
        onCloseEvent: function (selectedDate, limit) {
            var sFromDate = $(".filterFlyoutFromText").val();
            var sToDate = $(".filterFlyoutToText").val();
            if (1 === limit) {
                $("#dateFlyoutContent .filterFlyoutFromText").datepicker("option", "maxDate", selectedDate);
            } else {
                $("#dateFlyoutContent .filterFlyoutToText").datepicker("option", "minDate", selectedDate);
            }
            if (sFromDate.length || sToDate.length) {
                $("#btnOK").addClass("activateButton");
            } else {
                $("#btnOK").removeClass("activateButton");
            }
        },
        /* Update the multi select filters */
        updateMultiSelectServerFilters: function (oThisObject, oCurrentFilter, sRefinerName) {
            //// If check box is previously checked then this value will be true
            if (oThisObject.find(".filterValueCheckbox").length && !oThisObject.find(".filterValueCheckbox")[0].checked && -1 === $.inArray(oThisObject.text(), oCurrentFilter)) {
                oCurrentFilter.push(oThisObject.text());
            } else {
                /* Pop out the value of current filter from the list  */
                oCurrentFilter = $.grep(oCurrentFilter, function (sCurrentValue) {
                    return sCurrentValue !== oThisObject.text();
                });
                if (0 === oCurrentFilter.length) {
                    commonConstants.arrCurrentRefiner = $.grep(commonConstants.arrCurrentRefiner, function (sCurrentValue) {
                        return sCurrentValue !== sRefinerName;
                    });
                }
            }
            return oCurrentFilter;
        },
        /* Function to delete all the client filters */
        clearClientFlyOutFilters: function () {
            oPinnedFlyoutFilters = {
                oSearchMatterFilters: {
                },
                oSearchDocumentFilters: {
                }
            };
            oCommonObject.pinnedFilterData.length = 0;
        },

        // Function to get the notification message to be displayed while performing upload
        getNotificationContent: function (sMsg, sContentCheckMsg, sOptionContent) {
            "use strict";
            var sMsgTitle = document.createElement("div");
            sMsgTitle.innerHTML = sMsg;
            sMsgTitle = $(sMsgTitle).text();

            var sContentResult = "<div class=\"notificationResult\">" + sContentCheckMsg + "</div>";
            var sContent = "";
            if ("undefined" !== typeof sContentCheckMsg && "" === sContentCheckMsg.trim()) {
                sContentResult = "<div class=\"notificationResult hide\">" + sContentCheckMsg + "</div>";
            }
            if (oGridConfig.inWebDashboard) {
                sContent = "<div class=\"notification uploadDocumentNotification warningNotification\" title=\"" + sMsgTitle + "\"> <img id=\"warningImg\" src = \"../Images/warning-message.png\" alt=\"Warning Message\"/> <div id=\"overWriteDocumentNameWebdashboard\"> " + sMsg + "</div> " + sContentResult + "  <div class=\"askForOverwrite\"> " + sOptionContent + " </div> </div>";
            } else {
                sContent = "<div class=\"notification warningNotification\" title=\"" + sMsgTitle + "\"> <img id=\"warningImg\" src = \"../Images/warning-message.png\" alt=\"Warning Message\"/> <div id=\"overWriteDocumentName\"> " + sMsg + "</div> " + sContentResult + "  <div class=\"askForOverwrite\"> " + sOptionContent + " </div> </div>";
            }
            return sContent;
        },

        // Function to continue upload, in case operation failed while getting the content check configuration from list
        continueUpload: function () {
            "use strict";
            $(".mailContainer .warningnotification").remove();
            oCommonObject.updateUploadPopupHeight(false);
        },

        // Function to display notification, in case operation failed while getting the content check configuration from list
        showContentCheckConfigError: function () {
            "use strict";
            var sMsg = oGlobalConstants.Upload_Content_Check_Setting_Failed;
            var sOptionContent = "<input type='button' value='" + oGlobalConstants.Upload_Continue_Button + "' onclick='oCommonObject.continueUpload();'/>";
            var sContent = oCommonObject.getNotificationContent(sMsg, "", sOptionContent);
            oCommonObject.updateUploadPopupHeight(true);
            (oGridConfig.inWebDashboard) ? $(".notificationContainerForMailPopup").append(sContent) : $(".notificationContainerForPopup").append(sContent);
        },

        // Function to get the content check configuration from the list
        getContentCheckConfigurations: function (sMatterUrl) {
            "use strict";
            var matterDetails = {
                "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "siteCollectionPath": sMatterUrl
            };
            oCommonObject.callProvisioningService("GetDefaultMatterConfigurations", matterDetails, commonConstants.getContentCheckConfigurationSuccess, commonConstants.getContentCheckConfigurationFailure);
        },

        // Function to be called on success while retrieving content check configuration from service
        getContentCheckConfigurationSuccess: function (result) {
            "use strict";
            if (null != result) {
                var matterConfigurations = JSON.parse(result.split("|$|")[0]);
                if (matterConfigurations && "undefined" !== typeof matterConfigurations.IsContentCheck) {
                    oUploadGlobal.bAllowContentCheck = matterConfigurations.IsContentCheck;
                } else {
                    oUploadGlobal.bAllowContentCheck = false;
                    //// Yellow banner to notify user that content check can not be performed
                    commonConstants.showContentCheckConfigError();
                }
            }
        },

        // Function to be called on failure while retrieving content check configuration from service
        getContentCheckConfigurationFailure: function (result) {
            "use strict";
            oUploadGlobal.bAllowContentCheck = false;
            //// Yellow banner to notify user that content check can not be performed
            commonConstants.showContentCheckConfigError();
        },

        // Function to Abort the content check operation
        abortContentCheck: function (isLocalUpload) {
            "use strict";
            if ("undefined" !== typeof oUploadGlobal.oXHR) {
                oUploadGlobal.oXHR.abort();
                oUploadGlobal.bIsAbortedCC = true;
                //// Show other three options on notification bar.
                var onClickFunction = (isLocalUpload) ? "oCommonObject.localOverWriteDocument" : "overWriteDocument";
                var sAppendContent = "";
                if (!isLocalUpload) {
                    $(".uploadDocumentLoading").remove();
                }
                if ("undefined" !== oUploadGlobal.bAppendOptionEnabled && oUploadGlobal.bAppendOptionEnabled) {
                    sAppendContent = "<input type='button' id = 'overWriteAppend' data-operation='append' title='" + oGlobalConstants.Upload_Append_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Append_Button + "' onClick='" + onClickFunction + "(this);' />";
                }
                var sOptionContent = "<input type='button' id = 'overWriteYes' title= '" + oGlobalConstants.Upload_Overwrite_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Overwrite_Button + "' data-operation='overwrite' onClick='" + onClickFunction + "(this);'/>" + sAppendContent + "<input type='button' id = 'overWriteNo' title='" + oGlobalConstants.Upload_Cancel_Button_Tooltip + "' value='" + oGlobalConstants.Upload_Cancel_Button + "' data-operation='ignore' onClick='" + onClickFunction + "(this);'/>";
                $(".notification").remove();
                var sContent = oCommonObject.getNotificationContent(oUploadGlobal.sNotificationMsg, oGlobalConstants.Content_Check_Abort, sOptionContent);
                (oGridConfig.inWebDashboard) ? $(".notificationContainerForMailPopup").append(sContent) : $(".notificationContainerForPopup").append(sContent);
            }
        },

        // Function to generate the content check notification message
        contentCheckNotification: function (isLocalUpload) {
            "use strict";
            //// Clear up existing message and display new message
            $(".notification").remove();
            var sMsg = oUploadGlobal.sNotificationMsg;
            var sOptions = "<div class='notificationCC'>" + oGlobalConstants.Performing_Content_Check + " <input id='cancelCC' type='button' value='Cancel' onclick='oCommonObject.abortContentCheck(" + isLocalUpload + ");'/> </div>";
            var sContent = oCommonObject.getNotificationContent(sMsg, "", sOptions);
            oCommonObject.updateUploadPopupHeight(true);
            (oGridConfig.inWebDashboard) ? $(".notificationContainerForMailPopup").append(sContent) : $(".notificationContainerForPopup").append(sContent);
        },
        // Function to enable autocomplete text box and search box
        enableSearch: function () {
            $(".ms-Icon--search")[0].disabled = false;
            $("#autoCompleteText").removeAttr("disabled", "disabled").removeClass("is-disabled");
        },
        disableSearch: function () {
            $(".ms-Icon--search")[0].disabled = true;
            $("#autoCompleteText").attr("disabled", "disabled").addClass("is-disabled");
        },

        //// Updates GridView for matters/documents
        updateGridView: function () {
            (oCommonObject.isMatterView) ? getPinnedMatters($("#gridView")) : getPinnedDocument($("#gridView"));
            if (oGridViewObject.isPageLoad) {
                (oCommonObject.isMatterView) ? getSearchMatters($("#gridView"), 1) : getSearchDocuments($("#gridView"), 1);
            } else {
                (oCommonObject.isMatterView) ? getSearchMatters($("#gridView"), 0) : getSearchDocuments($("#gridView"), 0);
                oGridViewObject.isPageLoad = 1;
            }
        },
        /* Function to set attributes to View document landing page button and bind click event */
        updateDocumentLandingAttributes: function ($Element) {
            var oViewDocumentLanding = $("#viewDocumentLandingPage");
            if (oViewDocumentLanding && oViewDocumentLanding.length) {
                var sClientRelativeURL, sLibraryName, sDocumentMatterUrl;
                if (4 === oCommonObject.iCurrentGridViewData() || 1 === oGridConfig.currentView) {
                    sClientRelativeURL = commonConstants.getClientURL($Element);
                } else {
                    sClientRelativeURL = $Element.attr("data-" + oGlobalConstants.SP_Web_Url); //// This will get correct URL in case of sub-site as well                
                }
                if (sClientRelativeURL) {
                    oViewDocumentLanding.attr({ "client": sClientRelativeURL.replace(oGlobalConstants.Site_Url, "") });
                    //// set the doc id based on service call
                }
                /* Function to navigate user to document landing page */
                oViewDocumentLanding.off().on("click", function () {
                    var oCurrentDocument = $(this);
                    if (oCurrentDocument && oCurrentDocument.length) {
                        var sClient, sListGUID, sDocumentId, sDocumentLandingURL, sQueryString;
                        sClient = oCurrentDocument.attr("client");
                        sListGUID = oCurrentDocument.attr("listGUID");
                        sDocumentId = oCurrentDocument.attr("documentGUID");
                        sQueryString = "client=" + encodeURIComponent(sClient) + "&listguid=" + encodeURIComponent(sListGUID) + "&docguid=" + encodeURIComponent(sDocumentId);
                        sDocumentLandingURL = oGlobalConstants.Central_Repository_Url + oGlobalConstants.DocumentLandingURL + "?" + sQueryString;
                        if ("undefined" !== typeof oWebDashboardConstants) {
                            window.open(sDocumentLandingURL, "_parent");
                        } else {
                            commonConstants.openWindow(sDocumentLandingURL, "_blank");
                        }
                    }
                });
            }
            $("#openDocument").off().on("click", function () {
                var sDocumentURL = "";
                sDocumentURL = $(this).attr("data-link");
                if ("undefined" !== typeof oWebDashboardConstants) {
                    window.open(sDocumentURL, "_parent");
                } else {
                    commonConstants.openWindow(sDocumentURL, "_blank");
                }
            });
        },
        /* Function to execute in case of success while getting document assets */
        getDocumentLandingAssetsSuccess: function (oResult) {
            "use strict";
            if (oResult) {
                var results = JSON.parse(oResult);
                if (!results.code) {
                    var results = results.value.split("$|$");
                    if (results[0] && results[1]) {
                        var oViewDocumentLanding = $("#viewDocumentLandingPage");
                        if (oViewDocumentLanding && oViewDocumentLanding.length) {
                            oViewDocumentLanding.attr({ "listGUID": results[0], "documentGUID": results[1] });
                        }
                    } else {
                        $("#viewDocumentLandingPage").off().addClass("disableButton");
                    }

                    if (oGridConfig.inWebDashboard) {
                        $("#documentPopupLoading").addClass("hide");
                        $(".documentPopupData").removeClass("hide");
                    } else {
                        $("#FlyoutPopupLoading").addClass("hide");
                        $("#gridViewContainer .InfoFlyout").find(".ms-Callout-inner").removeClass("hide");
                    }
                } else {
                    showCommonErrorPopUp(results.code);
                }
            }
        },

        /* Function to execute in case of failure while getting document assets */
        getDocumentLandingAssetsFailure: function (oResult) {
            "use strict";
            showCommonErrorPopUp(oResult.Result);
        },

        getDocumentLandingAssets: function (oElement) {
            "use strict";
            var $Element = $(oElement);
            if ($Element.length) {
                var sDocumentPath, sClientRelativeURL, sDocumentLibrary, sClient, sClientURL;
                if (4 === oCommonObject.iCurrentGridViewData() || 1 === oGridConfig.currentView) {
                    sDocumentPath = $Element.attr("data-" + oGlobalConstants.GridViewPinnedDocumentUrl);
                    sClientURL = commonConstants.getClientURL($Element);
                } else {
                    sDocumentPath = trimEndChar($Element.attr("data-" + oGlobalConstants.Path), "/");
                    sClientURL = $Element.attr("data-" + oGlobalConstants.SP_Web_Url); //// This will get correct URL in case of sub-site as well                    
                }
                if (sClientURL) {
                    sDocumentPath = sDocumentPath.replace(oGlobalConstants.Site_Url, "");
                    sClientRelativeURL = sClientURL.replace(oGlobalConstants.Site_Url, "");
                    sClient = new RegExp(sClientRelativeURL, "gi");
                    sDocumentLibrary = sClientRelativeURL + "/" + sDocumentPath.replace(sClient, "").split("/")[1];
                }

                //// call service with sDocumentName, sDocumentLibrary and sParentRelativeURL
                var oDocumentDetails = {
                    "requestObject": {
                        "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
                    }, "client": {
                        "Id": decodeURIComponent(sDocumentPath), "Name": decodeURIComponent(sDocumentLibrary), "Url": decodeURIComponent(sClientURL)
                    }
                };
                commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + oGlobalConstants.Go_To_Document_Landing, true);
                oCommonObject.callSearchService("GetDocumentAssets", oDocumentDetails, commonConstants.getDocumentLandingAssetsSuccess, commonConstants.getDocumentLandingAssetsFailure);
            }
        },
        getClientURL: function (oElement) {
            var sClientURL, sLibraryName, sDocumentMatterUrl, sReplaceData;
            sDocumentMatterUrl = oElement.attr("data-documentmatterurl");
            if (sDocumentMatterUrl) {
                sReplaceData = sDocumentMatterUrl.split("/");
                sLibraryName = sReplaceData[sReplaceData.length - 1];
                sClientURL = sDocumentMatterUrl.replace("/" + sLibraryName, "");
            }
            return sClientURL;
        },

        /* Function to check if content is present, otherwise return "NA" */
        renderStringContent: function (sValue) {
            sValue = sValue.trim();
            return (sValue) ? sValue : "NA";
        },
        getDecodeURIComponent: function (sValue) {
            sValue = decodeURIComponent(trimEndChar($.trim(sValue), "/").toLowerCase());
            return (sValue) ? sValue : "";
        },

        /* Function to open link according to provided parameter */
        openWindow: function (url, target) {
            window.open(url, target, "menubar=yes,toolbar=yes,location=yes,scrollbars=yes,resizable=yes");
        },
        /* Function to set attributes used for upload functionality */
        addUploadAttributes: function (sOriginalName, sMatterGuid) {
            if ("undefined" !== typeof sOriginalName && sOriginalName) {
                $("#mailContent").attr("data-originalname", sOriginalName.trim());
            }
            if ("undefined" !== typeof sMatterGuid && sMatterGuid) {
                $("#mailContent").attr("data-matterguid", sMatterGuid.trim());
            }
        },
        /* Function to get the name of root folder for matter library */
        getRootFolderForMatter: function (sCurrentFolderName) {
            //// Set the folder name once upload is completed
            var sMatterGUID = $("#mailContent").attr("data-matterguid"),
                sFolderName = "";
            if (sMatterGUID === sCurrentFolderName) {
                //// If upload if performed on root folder, display the matter title instead of matter GUID
                sFolderName = $("#mailContent").attr("data-originalname");
            } else {
                //// else display the returned folder from the service
                sFolderName = sCurrentFolderName;
            }
            return sFolderName;
        },
        /* Function to encode html */
        htmlEncode: function (sValue) {
            return $("<div/>").text(sValue).html();
        }
    };
    ////#endregion

    ////#region Filter for advance search
    /* JSON object to hold the filters for columns on grid view - Server side */
    var oFlyoutFilters = {
        Name: "",
        ClientName: "",
        DateFilters: {
            ModifiedFromDate: "",
            ModifiedToDate: "",
            CreatedFromDate: "",
            CreatedToDate: "",
            OpenDateFrom: "",
            OpenDateTo: ""
        },
        ResponsibleAttorneys: [],
        SubareaOfLaw: "",
        DocumentCheckoutUsers: [],
        DocumentAuthor: []
    };
    /* JSON object to hold the filters for columns on grid view - Client side */
    var oPinnedFlyoutFilters = {
        oSearchMatterFilters: {
        },
        oSearchDocumentFilters: {
        }
    };
    ////#endregion

    /* Return the object which will have functions called from external files */
    return {
        callProvisioningService: function (sWebMethodName, oParameters, onSuccess, onFailure, onBeforeSend, oParam) {
            commonConstants.sServiceLocation = commonConstants.sProvisioningServiceLocation;
            commonConstants.callService(sWebMethodName, oParameters, onSuccess, onFailure, onBeforeSend, oParam);
        },
        callSearchService: function (sWebMethodName, oParameters, onSuccess, onFailure, onBeforeSend, oParam) {
            commonConstants.sServiceLocation = commonConstants.sSearchServiceLocation;
            commonConstants.callService(sWebMethodName, oParameters, onSuccess, onFailure, onBeforeSend, oParam);
        },
        callLegalBriefcase: function (sWebMethodName, oParameters, onSuccess, onFailure, onBeforeSend, oParam) {
            commonConstants.sServiceLocation = commonConstants.sLegalBriefcaseLocation;
            commonConstants.callService(sWebMethodName, oParameters, onSuccess, onFailure, onBeforeSend, oParam);
        },
        highlightTerm: function (sResult, sTerm) {
            return commonConstants.highlightTerm(sResult, sTerm);
        },
        formatSearchText: function (sSearchTerm, sRefinerString) {
            return commonConstants.formatSearchText(sSearchTerm, sRefinerString);
        },
        getRecentMatters: function (container, pageNumber, isPage, source) {
            return commonConstants.getRecentMatters(container, pageNumber, isPage, source);
        },
        getRecentDocuments: function (container, pageNumber, isPage, source, event) {
            return commonConstants.getRecentDocuments(container, pageNumber, isPage, source, event);
        },
        getDeployedUrl: function () {
            return commonConstants.getDeployedUrl();
        },
        getAppSwitcher: function (sAppName) {
            return commonConstants.getAppSwitcher(sAppName);
        },
        renderAsText: function (text) {
            return commonConstants.renderAsText(text);
        },
        getSearchData: function (event) {
            return commonConstants.getSearchData(event);
        },
        getParameterByName: function (sParameterName) {
            return commonConstants.getParameterByName(sParameterName);
        },
        checkDateValid: function (fromDateText, toDateText) {
            return commonConstants.checkDateValid(fromDateText, toDateText);
        },
        ExtractFileTitle: function (fileNameWithExtn) {
            return commonConstants.ExtractFileTitle(fileNameWithExtn);
        },
        logEvent: function (eventName) {
            commonConstants.logEvent(eventName);
        },
        updateNotificationPosition: function () {
            return commonConstants.updateNotificationPosition();
        },
        showNotification: function (sMsg, resultClass) {
            return commonConstants.showNotification(sMsg, resultClass);
        },
        localOverWriteDocument: function (oDocument) {
            return commonConstants.localOverWriteDocument(oDocument);
        },
        overwriteConfiguration: function (fileName) {
            return commonConstants.overwriteConfiguration(fileName);
        },
        updateUploadPopupHeight: function (increaseHeight) {
            return commonConstants.updateUploadPopupHeight(increaseHeight);
        },
        bindAutocomplete: function (sSelector, bIsMultiUser) {
            return commonConstants.bindAutocomplete(sSelector, bIsMultiUser);
        },
        getUserName: function (sUserEmails, bIsName) {
            return commonConstants.getUserName(sUserEmails, bIsName);
        },
        oSiteUser: commonConstants.oSiteUser,
        getWidth: function () {
            return commonConstants.getWidth();
        },
        bHideAutoComplete: commonConstants.bHideAutoComplete,
        isMatterView: commonConstants.isMatterView,
        iCurrentGridViewData: function () {
            return commonConstants.iCurrentGridViewData;
        },
        sCurrentPage: commonConstants.sCurrentPage,
        userPinnedData: commonConstants.userPinnedData,
        pinnedData: commonConstants.pinnedData,
        updatePinnedStatus: function () {
            commonConstants.updatePinnedStatus();
        },
        checkMatterLandingURL: function (sClientURL, sOneNoteURL, sMatterLandingPageURL, sMatterLibraryURL, oContextualMenu) {
            commonConstants.checkMatterLandingURL(sClientURL, sOneNoteURL, sMatterLandingPageURL, sMatterLibraryURL, oContextualMenu);
        },
        generateCommonDropdown: function (sDropdownSelector, sDropdownFields) {
            commonConstants.generateCommonDropdown(sDropdownSelector, sDropdownFields);
        },
        generateColumnPicker: function (sColumnPickerSelector, sColumnPickerFields) {
            commonConstants.generateColumnPicker(sColumnPickerSelector, sColumnPickerFields);
        },
        addDropdownBindings: function () {
            commonConstants.addDropdownBindings();
        },
        addAutoCompleteBindings: function () {
            commonConstants.addAutoCompleteBindings();
        },
        autoCompleteSearch: function () {
            commonConstants.autoCompleteSearch();
        },
        addColumnPickerBindings: function () {
            commonConstants.addColumnPickerBindings();
        },
        onColumnPickerCheckboxClick: function (iIndex, event) {
            commonConstants.onColumnPickerCheckboxClick(iIndex, event);
        },
        onGridViewCheckboxClick: function () {
            commonConstants.onGridViewCheckboxClick();
        },
        getAutoCompleteDataFailure: function (result) {
            commonConstants.getAutoCompleteDataFailure(result);
        },
        getAutoCompleteDataSuccess: function (result) {
            commonConstants.getAutoCompleteDataSuccess(result);
        },
        getAutoCompleteData: function () {
            commonConstants.getAutoCompleteData();
        },
        getIconSource: function (sExtension) {
            return commonConstants.getIconSource(sExtension);
        },
        closeAllPopupExcept: function (divClass, event) {
            commonConstants.closeAllPopupExcept(divClass, event);
        },
        isServiceCallComplete: commonConstants.isServiceCallComplete,
        addECBBindings: function () {
            commonConstants.addECBBindings();
        },
        getColumnDataSuccess: function (result) {
            commonConstants.getColumnDataSuccess(result);
        },
        getColumnDataFailure: function (result) {
            commonConstants.getColumnDataFailure(result);
        },
        getColumnDataOnBefore: function (result) {
            commonConstants.getColumnDataOnBefore(result);
        },
        getColumnData: function (sRefinerValue, sFilterFlyoutType, sFilterSearchTerm) {
            commonConstants.getColumnData(sRefinerValue, sFilterFlyoutType, sFilterSearchTerm);
        },
        addFilterFlyoutBindings: function () {
            commonConstants.addFilterFlyoutBindings();
        },
        oHeaderFilterType: commonConstants.oHeaderFilterType,
        pinnedFilterData: commonConstants.pinnedFilterData,
        arrUniqueRefinerData: function () {
            return commonConstants.arrUniqueRefinerData;
        },
        populateFilterHTML: function (sRefinerName, sFilterFlyoutType, arrRefinerUniqueValues, bScroll) {
            commonConstants.populateFilterHTML(sRefinerName, sFilterFlyoutType, arrRefinerUniqueValues, bScroll);
        },
        formatFilterSearchTerm: function (sRefinerName) {
            return commonConstants.formatFilterSearchTerm(sRefinerName);
        },
        filterData: function (oPinnedFilter, bFlag, sRefinerName) {
            commonConstants.filterData(oPinnedFilter, bFlag, sRefinerName);
        },
        oFlyoutFilters: function () {
            return oFlyoutFilters;
        },
        configureOnLoadView: function () {
            commonConstants.configureOnLoadView();
        },
        oSortDetails: function () {
            return commonConstants.oSortDetails;
        },
        clearGridViewContent: function () {
            commonConstants.clearGridViewContent();
        },
        getSortDetails: function (bIsFilterFlyoutCall) {
            commonConstants.getSortDetails(bIsFilterFlyoutCall);
        },
        updateSortingNotification: function () {
            commonConstants.updateSortingNotification();
        },
        oPinnedFlyoutFilters: oPinnedFlyoutFilters,
        iFilterFlyoutPageNumber: function () {
            commonConstants.iFilterFlyoutPageNumber = 1; //// Reset the page number for filter fly out data to 1
        },
        iTypingTimer: commonConstants.iTypingTimer,
        isSearchText: commonConstants.isSearchText,
        isAllRowSelected: commonConstants.isAllRowSelected,
        isFilterFlyoutVisible: commonConstants.isFilterFlyoutVisible,
        setDateFilters: function (sRefinerName, sFilterFlyoutType) {
            commonConstants.setDateFilters(sRefinerName, sFilterFlyoutType);
        },
        highlightSelectedFilters: function (sRefinerName, sFilterFlyoutType) {
            commonConstants.highlightSelectedFilters(sRefinerName, sFilterFlyoutType);
        },
        adjustCaretIcon: function () {
            commonConstants.adjustCaretIcon();
        },
        moveFilterFlyout: function () {
            commonConstants.moveFilterFlyout();
        },
        getArrayDifference: function (arrFirstArray, arrSecondArray) {
            return commonConstants.getArrayDifference(arrFirstArray, arrSecondArray);
        },
        getNotificationContent: function (sMsg, sContentCheckMsg, sOptionContent) {
            return commonConstants.getNotificationContent(sMsg, sContentCheckMsg, sOptionContent);
        },
        continueUpload: function () {
            return commonConstants.continueUpload();
        },
        getContentCheckConfigurations: function (sMatterUrl) {
            return commonConstants.getContentCheckConfigurations(sMatterUrl);
        },
        abortContentCheck: function (isLocalUpload) {
            return commonConstants.abortContentCheck(isLocalUpload);
        },
        contentCheckNotification: function (isLocalUpload) {
            return commonConstants.contentCheckNotification(isLocalUpload);
        },
        enableSearch: function () {
            return commonConstants.enableSearch();
        },
        disableSearch: function () {
            return commonConstants.disableSearch();
        },
        updateDropDownToolTip: function () {
            return commonConstants.updateDropDownToolTip();
        },
        abortRequest: function () {
            return commonConstants.abortRequest();
        },
        updateGridView: function () {
            return commonConstants.updateGridView();
        },
        updateDocumentLandingAttributes: function ($Element) {
            return commonConstants.updateDocumentLandingAttributes($Element);
        },
        getDocumentLandingAssets: function ($Element) {
            return commonConstants.getDocumentLandingAssets($Element);
        },
        renderStringContent: function (sValue) {
            return commonConstants.renderStringContent(sValue);
        },
        getDecodeURIComponent: function (sValue) {
            return commonConstants.getDecodeURIComponent(sValue);
        },
        openWindow: function (url, target) {
            commonConstants.openWindow(url, target);
        },
        sSearchedKeyword: commonConstants.sSearchedKeyword,
        bCalledForSort: commonConstants.bCalledForSort,
        sFilterDetails: commonConstants.sFilterDetails,
        addUploadAttributes: function (sOriginalName, sMatterGuid) {
            commonConstants.addUploadAttributes(sOriginalName, sMatterGuid);
        },
        getRootFolderForMatter: function (sCurrentFolderName) {
            return commonConstants.getRootFolderForMatter(sCurrentFolderName);
        },
        htmlEncode: function (sValue) {
            return commonConstants.htmlEncode(sValue);
        }
    };
})();
