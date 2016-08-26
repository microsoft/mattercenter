/// <disable>JS1003,JS3057,JS3058,JS2074,JS2076,JS3092,JS3054,JS2005,JS3056,JS2073,JS2024,JS2032,JS3116,JS3085, JS2064</disable>
//// Above exclusion list is signed off as per the Modern Cop Exclusion list.

var oDocumentLandingObject = (function () {
    "use strict";

    var oDocumentLanding = {
        sSPFileUrl: _spPageContextInfo.webAbsoluteUrl + "/" + _spPageContextInfo.layoutsUrl + "/sp.js",
        sClientRelativeUrl: "", //// Will be coming from query string
        sDocumentParentListId: "", //// Will be coming from query string
        sDocumentGUID: "", //// Will be coming from query string
        sMatterGUID: "",
        sDocumentParentList: "",
        sSPTenantUrl: "",
        oCurrentList: "",
        oFilteredItems: "",
        sCurrentDocumentUrl: "",
        sCurrentDocumentName: "",
        sCurrentDocumentUniqueAccess: "",
        sCurrentDocumentID: "",
        sCurrentDocumentRelativeUrl: "",
        sCurrentDocumentDownloadUrl: "",
        sDocumentItemId: "",
        sDocumentListItemId: "",
        sFilePropertiesURL: "",
        sUserLoginName: "",
        /* File properties */
        sFileTitle: "",
        sDocumentClient: "",
        sMatterName: "",
        sClientID: "",
        sMatterId: "",
        sCheckOutUser: "",
        sCheckOutUserEmail: "",
        sAuthor: "",
        sCreatedDate: "",
        sCreatedDateUTC: "",
        sPracticeGroup: "",
        sAreaOfLaw: "",
        sSubAreaOfLaw: "",
        sCurrentDocumentType: "",
        iSize: 0,
        /* Other properties for pin */
        sModifiedDate: "",
        sCheckOutUserPinned: "",
        sDocumentAJAXCallURL: "",
        sEmailSubject: "Matter Center Document(s)",
        sSendToOneDriveTitle: "Send To OneDrive",
        sDocumentAJAXParameters: "/_api/web/getFileByServerRelativeUrl('{0}')/{1}",
        sTitleAJAXParameters: "/_api/web/lists/GetByTitle('{0}')/RootFolder/{1}",
        sWOPIFrameURL: "/_layouts/15/WopiFrame.aspx?sourcedoc={0}&action=interactivepreview",
        sVisioWebAccessURL: "/_layouts/15/VisioWebAccess/VisioWebAccess.aspx?listguid={0}&itemid={1}&DefaultItemOpen=1",
        sOWAAJAXParameters: "/_api/web/lists(guid'{0}')/items({1})/getWOPIFrameUrl({2})",
        sCheckInURL: "/_layouts/15/Checkin.aspx?FileName={0}&Checkin=true",
        sPermissionURL: "/_layouts/15/User.aspx?obj={0},{1},LISTITEM",
        sShareURL: "/_layouts/15/aclinv.aspx?forSharing=1&obj={0},{1},DOCUMENT",
        sContextInfoURL: "/_api/contextinfo",
        sDocumentPreviewURL: "/_layouts/15/DocIdRedir.aspx?ID=",
        sVersionHistoryURL: "/_layouts/15/Versions.aspx?list={{0}}&ID={1}",
        sDownloadURL: "/_layouts/15/download.aspx?SourceUrl={0}&Source={1}",
        sDisplayFormURL: "/Forms/DispForm.aspx?ID=",
        sSendToOneDriveURL: "[[[Send to OneDrive]]]?SPListItemId={0}$|${1}$|${2}",//// Will be updated via OneClick
        sPropertyToInclude: "FileLeafRef,HasUniqueRoleAssignments,_dlc_DocId,FileRef,DocIcon,_UIVersionString,Modified,Editor,FileDirRef",
        sVisioExtension: "vsdx,vsdm",
        oVersionInfo: {},
        enumWOPIFrame: {},
        sWOPIFrameDocumentURL: "",
        sFormDigest: "",
        sAuthorEmail: "",
        oCurrenVersionDetails: {
            sCurrentVersionNumber: "",
            sCurrentModifiedDate: "",
            sCurrentModifiedBy: "",
            sCurrentModifiedByEmail: ""
        },
        oMonths: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]
    },
    oAppInsights = {
        sPageName: "Document Details/",
        sMatter: "Web Matters",
        sDocument: "Web Documents",
        sSettings: "Settings page",
        sSearch: "Search",
        sOpen: "Open",
        sDownload: "Download",
        sShare: "Share",
        sCheckIn: "Check In",
        sCheckOut: "Check Out",
        sSend: "Send As Link",
        sOneDrive: "Send to OneDrive",
        sPin: "Pin Document",
        sUnpin: "Unpin Document",
        sFile: "File Properties",
        sVersion: "Version History",
        sFeedback: "Feedback & Support",
        sPrivacy: "Privacy & cookies",
        sTermsOfUse: "Terms of use",
        sMicrosoft: "Microsoft",
        sSharedProperties: "View shared properties"
    },
    oPinDocumentKeys = {
        "DocumentName": "DocumentName",
        "DocumentVersion": "DocumentVersion",
        "DocumentClient": "DocumentClient",
        "DocumentClientId": "DocumentClientId",
        "DocumentClientUrl": "DocumentClientUrl",
        "DocumentMatter": "DocumentMatter",
        "DocumentMatterId": "DocumentMatterId",
        "DocumentOwner": "DocumentOwner",
        "DocumentUrl": "DocumentUrl",
        "DocumentOWAUrl": "DocumentOWAUrl",
        "DocumentExtension": "DocumentExtension",
        "DocumentCreatedDate": "DocumentCreatedDate",
        "DocumentModifiedDate": "DocumentModifiedDate",
        "DocumentCheckOutUser": "DocumentCheckOutUser",
        "DocumentMatterUrl": "DocumentMatterUrl",
        "DocumentParentUrl": "DocumentParentUrl",
        "DocumentID": "DocumentID"
    },
    oErrorMessages = {
        sInsufficientParametersMessage: "Insufficient Parameters",
        sErrorMessage: "Something went wrong",
        sPropertiesRetrievalFailureMessage: "Failed to retrieve document properties",
        sDocumentRetrievalFailure: "Failed to get current document"
    },
    oSelectProperties = {
        sDocumentProperties: "vti_x005f_timecreated,vti_x005f_timelastmodified,vti_x005f_filesize,vti_x005f_title",
        sMatterProperties: "vti_x005f_listname,MatterGUID",
        sCheckedOutUserProperties: "CheckedOutByUser,Title,Email",
        sAuthorProperties: "Title,Email",
        sClientID: "Client_Id",
        sClientName: "Client_Name",
        sMatterID: "Matter_Id",
        sMatterName: "Matter_Name",
        sPracticeGroup: "PracticeGroup",
        sAreaOfLaw: "AreaOfLaw",
        sSubAreaOfLaw: "SubareaOfLaw",
        sItemId: "Id"
    },
    oCommonFunctions = {};

    oGlobalConstants.sListName = "UserPinnedDetails"; //// This should be updated with different deployments
    oGlobalConstants.sPinColumn = "UserPinDocumentDetails";
    oGlobalConstants.Pin_Document = "PinDocument";
    oGlobalConstants.Unpin_Document = "UnpinDocument";
    oGlobalConstants.iSuccessCounter = 0;
    oGlobalConstants.sPageTitle = "Document Details";

    /* Function to read query string parameters by name */
    oCommonFunctions.getParameterByName = function (sParamterName) {
        sParamterName = sParamterName.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var oRegex = new RegExp("[\\?&]" + sParamterName + "=([^&#]*)", "i"),
            oResults = oRegex.exec(location.search);
        return oResults === null ? "" : decodeURIComponent(oResults[1].replace(/\+/g, " "));
    };

    /* Common AJAX function */
    oCommonFunctions.getData = function getData(sURL, oSuccessFunction, oFailureFunction, sMethod) {
        var oHeaders = { "Accept": "application/json; odata=verbose" };
        if ("POST" === sMethod) {
            oHeaders["X-RequestDigest"] = oDocumentLanding.sFormDigest;
        }
        $.ajax({
            url: sURL,
            method: sMethod,
            headers: oHeaders,
            success: oSuccessFunction,
            error: oFailureFunction
        });
    };

    /* Function to get the tenant URL based on the location of document landing page */
    oCommonFunctions.getTenantUrl = function () {
        var sCurrentTenantUrl = "";
        if ("/" !== _spPageContextInfo.webServerRelativeUrl) { //// If document landing page is not on tenant
            sCurrentTenantUrl = _spPageContextInfo.webAbsoluteUrl.substring(0, _spPageContextInfo.webAbsoluteUrl.indexOf(_spPageContextInfo.webServerRelativeUrl));
        } else {
            sCurrentTenantUrl = _spPageContextInfo.webAbsoluteUrl;
        }
        return sCurrentTenantUrl;
    };

    /* Function to generate WOPI frame URL for document if supported */
    oCommonFunctions.getDocumentOWAURL = function () {
        var arrVisioDocuments = oDocumentLanding.sVisioExtension && oDocumentLanding.sVisioExtension.split(",")
            , sCurrentDocumentOWAURL;

        if (-1 < $.inArray(oDocumentLanding.sCurrentDocumentType, arrVisioDocuments)) {
            sCurrentDocumentOWAURL = oDocumentLanding.sSPTenantUrl + oDocumentLanding.sClientRelativeUrl + oDocumentLanding.sVisioWebAccessURL.replace("{0}", oDocumentLanding.sDocumentParentListId).replace("{1}", oDocumentLanding.sDocumentItemId);
            oDocumentLanding.sWOPIFrameDocumentURL = sCurrentDocumentOWAURL;
            $("#documentPreviewSource").attr("src", sCurrentDocumentOWAURL); //// Set the source URL for document preview
            $("#openDocument a").attr({ "target": "_self", "href": sCurrentDocumentOWAURL });
            $("#metadataProperties li:first-child, .actionChild:first-child, #loadingDocumentPreview").addClass("hide");
            $("#metadataProperties li:not(:first-child), .actionChild:not(:first-child)").removeClass("hide");
            $("#documentPreviewSource").addClass("hide");
            $("#documentPreviewNotSupported").removeClass("hide");
        } else {
            oCommonFunctions.getOWAUrl(oDocumentLanding.enumWOPIFrame.interactivePreview, oCommonFunctions.getOWAUrlSuccess);
            oCommonFunctions.getOWAUrl(oDocumentLanding.enumWOPIFrame.view, oCommonFunctions.getOpenUrlSuccess);
        }
    };

    /* Function to execute on success to get document URL */
    oCommonFunctions.getDocumentURLSuccess = function () {
        if (oDocumentLanding.oCurrentList && oDocumentLanding.oFilteredItems && oDocumentLanding.oFilteredItems.length) {
            var oCurrentDocument = oDocumentLanding.oFilteredItems[0];
            oDocumentLanding.sDocumentParentList = oDocumentLanding.oCurrentList.get_title();
            if (oCurrentDocument) {
                var sCurrentLibraryPath = oCurrentDocument.get_item("FileDirRef").toString();/* Get the current library path in which document is present*/
                oDocumentLanding.sCurrentLibrary = sCurrentLibraryPath.substr(sCurrentLibraryPath.lastIndexOf("/") + 1); /*Get the library name*/
                oDocumentLanding.sCurrentDocumentName = oCurrentDocument.get_item("FileLeafRef"); /* Document name */
                oDocumentLanding.sCurrentDocumentUrl = oDocumentLanding.sSPTenantUrl + oCurrentDocument.get_item("FileRef"); /* Document URL */
                oDocumentLanding.sCurrentDocumentType = oCurrentDocument.get_item("DocIcon"); /* Document type */
                oDocumentLanding.sCurrentDocumentUniqueAccess = oCurrentDocument.get_hasUniqueRoleAssignments(); /* Does document have unique access */
                oDocumentLanding.sCurrentDocumentID = oCurrentDocument.get_item("_dlc_DocId") ? oCurrentDocument.get_item("_dlc_DocId") : oGlobalConstants.sNotApplicable; /* Document ID */
                oDocumentLanding.sCurrentDocumentRelativeUrl = oCurrentDocument.get_item("FileRef"); /* Document relative URL */
                oDocumentLanding.sCurrentDocumentRelativeUrl = oCommonFunctions.replaceAll(oDocumentLanding.sCurrentDocumentRelativeUrl, "$", "$$$$");
                oDocumentLanding.sCurrentDocumentDownloadUrl = encodeURIComponent(oDocumentLanding.sCurrentDocumentRelativeUrl); /*Set document download URL*/
                oDocumentLanding.sCurrentDocumentRelativeUrl = oDocumentLanding.sCurrentDocumentRelativeUrl.replace("'", "''"); /*Handling documents with apostrophe in its URL*/
                /* Get the current version details of the document */
                oDocumentLanding.oCurrenVersionDetails.sCurrentVersionNumber = oCurrentDocument.get_item("_UIVersionString"); /* Document version number */
                oDocumentLanding.oCurrenVersionDetails.sCurrentModifiedDate = oCurrentDocument.get_item("Modified"); /* Document modified date */
                oDocumentLanding.oCurrenVersionDetails.sCurrentModifiedBy = oCurrentDocument.get_item("Editor").get_lookupValue(); /* Document modified by name */
                oDocumentLanding.oCurrenVersionDetails.sCurrentModifiedByEmail = oCurrentDocument.get_item("Editor").get_email(); /* Document modified by email */
                if (null !== oDocumentLanding.sCurrentDocumentName || null !== oDocumentLanding.sCurrentDocumentRelativeUrl) {
                    //// Make required AJAX calls
                    oCommonFunctions.retrieveListItems();
                    oCommonFunctions.getDocumentProperties(); //// Date created, file size
                    oCommonFunctions.getMatterDetails(); //// Client, Matter, Client.Matter ID, Practice group, Area of law, Sub area of law, List GUID
                    oCommonFunctions.getCheckOutUser(); //// Check out to
                    oCommonFunctions.getAuthor(); //// Author
                    oCommonFunctions.getVersionInfo();
                    oCommonFunctions.getListItem();
                    //// Set the required properties
                    $("#documentTitle").text(oDocumentLanding.sCurrentDocumentName).attr("title", oDocumentLanding.sCurrentDocumentName);
                    var sDownloadParameters = oDocumentLanding.sSPTenantUrl + oDocumentLanding.sClientRelativeUrl + oDocumentLanding.sDownloadURL.replace("{0}", oDocumentLanding.sCurrentDocumentDownloadUrl).replace("{1}", oDocumentLanding.sSPTenantUrl + oDocumentLanding.sClientRelativeUrl + "/" + oDocumentLanding.sDocumentParentList);
                    $("#download a").attr({ "target": "_self", "href": sDownloadParameters }); //// Set the href for download action
                } else {
                    oCommonFunctions.showCommonErrorPopUp(oErrorMessages.sPropertiesRetrievalFailureMessage);
                }
            } else {
                oCommonFunctions.showCommonErrorPopUp(oErrorMessages.sDocumentRetrievalFailure);
            }
        } else {
            oCommonFunctions.showCommonErrorPopUp(oErrorMessages.sDocumentRetrievalFailure);

        }
    };

    /* Function to execute on failure to get document URL */
    oCommonFunctions.getDocumentURLFailure = function (sender, oError) {
        if (typeof oError.get_message === "function") {
            oCommonFunctions.showCommonErrorPopUp(oError.get_message());
        } else {
            oCommonFunctions.showCommonErrorPopUp(oErrorMessages.sErrorMessage);
        }
    };

    /* Function to get the URL of current document */
    oCommonFunctions.getDocumentURL = function () {
        var oClientContext = new SP.ClientContext(oDocumentLanding.sSPTenantUrl + oDocumentLanding.sClientRelativeUrl)
            , oWeb = oClientContext.get_web()
            , oCamlQuery = new SP.CamlQuery();
        oDocumentLanding.oCurrentList = oWeb.get_lists().getById(oDocumentLanding.sDocumentParentListId);
        var arrCurrentProperties = oDocumentLanding.sPropertyToInclude.split(","), sQueryProperty = "";
        if (arrCurrentProperties.length) {
            $.each(arrCurrentProperties, function (sCurrentIndex, sCurrentValue) {
                sQueryProperty += "<FieldRef Name='" + sCurrentValue + "' />";
            });
        }
        var sCamlQueryString = "<View Scope='RecursiveAll'>" + //// Scan entire depth
                                "<Query><Where><And><Eq>" +
                                    "<FieldRef Name='UniqueId' /><Value Type='Guid'>" + oDocumentLanding.sDocumentGUID + "</Value>" + //// Filter on document GUID
                                "</Eq><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>0</Value></Eq></And></Where></Query>" +
                                "<ViewFields>" + sQueryProperty + "</ViewFields></View>";

        oCamlQuery.set_viewXml(sCamlQueryString);
        var oItems = oDocumentLanding.oCurrentList.getItems(oCamlQuery);

        oClientContext.load(oDocumentLanding.oCurrentList);
        oDocumentLanding.oFilteredItems = oClientContext.loadQuery(oItems, "Include(" + oDocumentLanding.sPropertyToInclude + ")"); //// Explicitly include the required properties

        oClientContext.executeQueryAsync(oCommonFunctions.getDocumentURLSuccess, oCommonFunctions.getDocumentURLFailure);
    };

    /* Function to send document as link */
    oCommonFunctions.sendDocumentAsLink = function () {
        oGlobalConstants.sEventName = oAppInsights.sSend;
        oCommonFunctions.LogEvent(true);
        var sSendAsLinkEmail = "mailto://?Subject=" + oDocumentLanding.sEmailSubject + "&Body=";
        sSendAsLinkEmail += "%0D%0A" + "1) " + encodeURIComponent(oDocumentLanding.sCurrentDocumentName) + ": " + "<" + encodeURIComponent(oDocumentLanding.sCurrentDocumentUrl) + ">";
        window.top.location.href = sSendAsLinkEmail;
    };

    /* Function to send document to OneDrive */
    oCommonFunctions.sendDocumentToOneDrive = function () {
        oGlobalConstants.sEventName = oAppInsights.sOneDrive;
        oCommonFunctions.LogEvent(true);
        var oDialogOptions = SP.UI.$create_DialogOptions();
        oDialogOptions.width = 600;
        oDialogOptions.height = 240;
        oDialogOptions.title = oDocumentLanding.sSendToOneDriveTitle;
        oDialogOptions.url = oCommonLinks.AzureSiteUrl + oDocumentLanding.sSendToOneDriveURL.replace("{0}", oDocumentLanding.sDocumentItemId).replace("{1}", oDocumentLanding.sDocumentListItemId).replace("{2}", oDocumentLanding.sSPTenantUrl + oDocumentLanding.sClientRelativeUrl);
        oDialogOptions.dialogReturnValueCallback = function (oDialogResult, sReturnValue) {
            oCommonFunctions.onSPModalPopupClose(oDialogResult, sReturnValue);
        };
        SP.UI.ModalDialog.showModalDialog(oDialogOptions);
    };

    /* Function to close common SP modal pop up */
    oCommonFunctions.onSPModalPopupClose = function (oDialogResult, sReturnValue) {
        window.location.reload(); // Forces a reload of page
        /* Below commented code is kept for future purpose */
        ////SP.UI.ModalDialog.RefreshPage(SP.UI.DialogResult.OK);
        ////if (oDialogResult == SP.UI.DialogResult.OK) {
        ////} else if (oDialogResult == SP.UI.DialogResult.cancel) {
        ////} else {
        ////}
    };

    /* Function to open version history pop up */
    oCommonFunctions.openVersionHistory = function () {
        oGlobalConstants.sEventName = oAppInsights.sVersion;
        oCommonFunctions.LogEvent(true);
        var oDialogOptions = SP.UI.$create_DialogOptions();
        oDialogOptions.url = oDocumentLanding.sSPTenantUrl + oDocumentLanding.sClientRelativeUrl + oDocumentLanding.sVersionHistoryURL.replace("{0}", oDocumentLanding.sDocumentParentListId).replace("{1}", oDocumentLanding.sDocumentItemId);
        oDialogOptions.dialogReturnValueCallback = function (oDialogResult, sReturnValue) {
            oCommonFunctions.onSPModalPopupClose(oDialogResult, sReturnValue);
        };
        SP.UI.ModalDialog.showModalDialog(oDialogOptions);
    };

    /* Function to set the pin unpin icon for document */
    oCommonFunctions.setTextForPinOperation = function (sOperationType) {
        oGlobalConstants.sOperationType = sOperationType;
        if (oGlobalConstants.sOperationPin === oGlobalConstants.sOperationType) {
            //// Not pinned
            $("#unpinDocument").addClass("hide");
            $("#pinDocument").removeClass("hide");
        } else if (oGlobalConstants.sOperationUnpin === oGlobalConstants.sOperationType) {
            //// Already pinned
            $("#pinDocument").addClass("hide");
            $("#unpinDocument").removeClass("hide");
        }
    };

    /* Function to execute on list item deleted */
    oCommonFunctions.onListItemDeleted = function () {
        oCommonFunctions.setTextForPinOperation(oGlobalConstants.sOperationPin);
    };

    /* Function to remove list item for last pinned item */
    oCommonFunctions.removeList = function (oPinnedObject) {
        var listItemEnumerator = oGlobalConstants.collListItem.getEnumerator(), oListItem;
        while (listItemEnumerator.moveNext()) {
            oListItem = listItemEnumerator.get_current();
            oGlobalConstants.documentPinnedDetails = oListItem.get_item(oGlobalConstants.sPinColumn);
        }
        var deletion = JSON.parse(oGlobalConstants.documentPinnedDetails),
            url = oCommonFunctions.htmlEncode(decodeURIComponent(oDocumentLanding.sCurrentDocumentUrl.toLowerCase()));
        $.each(deletion, function (key, value) {
            if (decodeURIComponent(oCommonFunctions.trimEndChar($.trim(key), "/").toLowerCase()) === url) {
                delete deletion[key];
                return false;
            }
        });
        var newDocumentPinnedDetails = JSON.stringify(deletion);
        oListItem.set_item(oGlobalConstants.sPinColumn, newDocumentPinnedDetails);
        oListItem.update();
        var iCountValue = 0;
        for (var key in deletion) {
            if (deletion.hasOwnProperty(key)) {
                iCountValue++;
            }
        }
        if (0 === iCountValue) {
            oListItem.deleteObject();
        }
        oGlobalConstants.clientContext.executeQueryAsync(Function.createDelegate(this, function () { oCommonFunctions.onListItemDeleted(); }), Function.createDelegate(this, function () { oCommonFunctions.onQueryFailed(oGlobalConstants.sOperationUnpin); }));
    };

    /* Function to update the list for pinned item */
    oCommonFunctions.updateList = function (oPinnedObject) {
        var listItemEnumerator = oGlobalConstants.collListItem.getEnumerator(), oListItem, isExists = false;
        while (listItemEnumerator.moveNext()) {
            oListItem = listItemEnumerator.get_current();
            oGlobalConstants.documentPinnedDetails = oListItem.get_item(oGlobalConstants.sPinColumn);
        }
        var arrCurrentPinnedData = decodeURIComponent(oGlobalConstants.documentPinnedDetails.toLowerCase());
        var isAlreadyPinned = arrCurrentPinnedData.search(decodeURIComponent(oDocumentLanding.sCurrentDocumentUrl.toLowerCase()));
        if (-1 === isAlreadyPinned) {
            var valueIndex = oGlobalConstants.documentPinnedDetails.lastIndexOf("}");
            oGlobalConstants.documentPinnedDetails = oGlobalConstants.documentPinnedDetails.substring(0, valueIndex);
            oGlobalConstants.documentPinnedDetails += ", \n" + oPinnedObject + "\n }";
            oListItem.set_item(oGlobalConstants.sPinColumn, oGlobalConstants.documentPinnedDetails);
            oListItem.update();
            oGlobalConstants.clientContext.executeQueryAsync(Function.createDelegate(this, function () { oCommonFunctions.onListItemCreated("undefined", oGlobalConstants.clientContext); }), Function.createDelegate(this, function () { oCommonFunctions.onQueryFailed(oGlobalConstants.sOperationPin); }));
        }
    };

    /* Function to execute on creation of list item in pinned list */
    oCommonFunctions.onListItemCreated = function (oListItem, clientContext) {
        if ("undefined" !== oListItem) {
            var objListItem = oListItem;
            var collRoleDefinitionBinding = SP.RoleDefinitionBindingCollection.newObject(clientContext);
            collRoleDefinitionBinding.add(clientContext.get_web().get_roleDefinitions().getByType(SP.RoleType.reader));
            var oUser = clientContext.get_web().ensureUser(_spPageContextInfo.userLoginName);
            objListItem.breakRoleInheritance(false);
            objListItem.get_roleAssignments().add(oUser, collRoleDefinitionBinding);
            objListItem.update();
            clientContext.load(oUser);
            clientContext.load(objListItem);
            clientContext.executeQueryAsync(function () { return true; }, function () { return true; });
        }
        oCommonFunctions.setTextForPinOperation(oGlobalConstants.sOperationUnpin);
    };

    /* Function to create list item in pinned list */
    oCommonFunctions.createListItemAndPin = function (oPinnedObject) {
        var clientContext = new SP.ClientContext(oDocumentLanding.sSPTenantUrl + oCommonLinks.sCatalogSite);
        var oList = clientContext.get_web().get_lists().getByTitle(oGlobalConstants.sListName);

        var itemCreateInfo = new SP.ListItemCreationInformation();
        oGlobalConstants.oListItem = oList.addItem(itemCreateInfo);
        var oPinnedBody = "{" + oPinnedObject + "}";
        oGlobalConstants.oListItem.set_item(oGlobalConstants.sUserAliasColumn, oGlobalConstants.sUserLoginName);
        oGlobalConstants.oListItem.set_item(oGlobalConstants.sPinColumn, oPinnedBody);

        oGlobalConstants.oListItem.update();
        clientContext.load(oGlobalConstants.oListItem);
        clientContext.executeQueryAsync(Function.createDelegate(this, function () { oCommonFunctions.onListItemCreated(oGlobalConstants.oListItem, clientContext); }), Function.createDelegate(this, function () { oCommonFunctions.onQueryFailed(oGlobalConstants.sOperationPin); }));
    };

    /* Function to get already pinned object */
    oCommonFunctions.getPinnedObject = function () {
        var sPinnedObject = "\"" + oCommonFunctions.htmlEncode(decodeURIComponent(oDocumentLanding.sCurrentDocumentUrl).toLowerCase()) + "\": {\n\t \"" + oPinDocumentKeys.DocumentName + "\": \"" +
                            oDocumentLanding.sCurrentDocumentName.replace(new RegExp("." + oDocumentLanding.sCurrentDocumentType + "$"), "") + "\", \n\t \"" + oPinDocumentKeys.DocumentVersion + "\": \"" +
                                oDocumentLanding.oCurrenVersionDetails.sCurrentVersionNumber + "\", \n\t \"" + oPinDocumentKeys.DocumentClient + "\": \"" +
                                oDocumentLanding.sDocumentClient + "\", \n\t \"" + oPinDocumentKeys.DocumentClientId + "\": \"" +
                                oDocumentLanding.sClientID + "\", \n\t \"" + oPinDocumentKeys.DocumentClientUrl + "\": \"" +
                                            decodeURIComponent(oDocumentLanding.sSPTenantUrl + oDocumentLanding.sClientRelativeUrl) + "\", \n\t \"" + oPinDocumentKeys.DocumentMatter + "\": \"" +
                                oDocumentLanding.sMatterName + "\", \n\t \"" + oPinDocumentKeys.DocumentMatterId + "\": \"" +
                                oDocumentLanding.sMatterId + "\", \n\t \"" + oPinDocumentKeys.DocumentOwner + "\": \"" +
                                oDocumentLanding.sAuthor + "\", \n\t \"" + oPinDocumentKeys.DocumentUrl + "\": \"" +
                            oCommonFunctions.htmlEncode(decodeURIComponent(oDocumentLanding.sCurrentDocumentUrl)) + "\", \n\t \"" + oPinDocumentKeys.DocumentOWAUrl + "\": \"" +
                            decodeURIComponent(oDocumentLanding.sWOPIFrameDocumentURL) + "\", \n\t \"" + oPinDocumentKeys.DocumentExtension + "\": \"" +
                                oDocumentLanding.sCurrentDocumentType + "\", \n\t \"" + oPinDocumentKeys.DocumentCreatedDate + "\": \"" +
                            oDocumentLanding.sCreatedDateUTC + "\", \n\t \"" + oPinDocumentKeys.DocumentModifiedDate + "\": \"" +
                            oCommonFunctions.getUTCDate(oDocumentLanding.sModifiedDate) + "\", \n\t \"" + oPinDocumentKeys.DocumentCheckOutUser + "\": \"" +
                            oDocumentLanding.sCheckOutUserPinned + "\", \n\t \"" + oPinDocumentKeys.DocumentMatterUrl + "\": \"" +
                            decodeURIComponent(oDocumentLanding.sSPTenantUrl + oDocumentLanding.sClientRelativeUrl + "/" + oDocumentLanding.sMatterGUID) + "\",\n\t\"" + oPinDocumentKeys.DocumentParentUrl + "\":\"" +
                            decodeURIComponent(oDocumentLanding.sCurrentDocumentUrl.substring(0, oDocumentLanding.sCurrentDocumentUrl.indexOf("/" + oDocumentLanding.sCurrentDocumentName))) + "\", \n\t \"" + oPinDocumentKeys.DocumentID + "\": \"" +
                                oDocumentLanding.sCurrentDocumentID + "\" \n }";
        return sPinnedObject;
    };
    /* Function to encode html*/
    oCommonFunctions.htmlEncode = function (value) {
        // Create a in-memory div, set it's inner text(which jQuery automatically encodes)
        // Then grab the encoded contents back out.  The div never exists on the page.
        return $("<div/>").text(value).html();
    };

    /* Function to check if document is already pinned */
    oCommonFunctions.checkAlreadyPinned = function (sDocumentDetails) {
        var iPresent = sDocumentDetails.indexOf(oDocumentLanding.sCurrentDocumentUrl.toLowerCase());
        if (0 > iPresent) {
            oCommonFunctions.setTextForPinOperation(oGlobalConstants.sOperationPin);
        } else {
            oCommonFunctions.setTextForPinOperation(oGlobalConstants.sOperationUnpin);
        }
    };

    /* Function to execute on success of pin/ unpin */
    oCommonFunctions.onQuerySucceeded = function () {
        oGlobalConstants.iSuccessCounter++;
        var iCount = 0, oListItem, oPinnedObject;
        var listItemEnumerator = oGlobalConstants.collListItem.getEnumerator();
        while (listItemEnumerator.moveNext()) {
            iCount++;
            oListItem = listItemEnumerator.get_current();
            oGlobalConstants.documentPinnedDetails = oListItem.get_item(oGlobalConstants.sPinColumn);
        }
        if ("" !== oGlobalConstants.sOperationType) {
            if (0 === iCount) {
                if (oGlobalConstants.sOperationPin === oGlobalConstants.sOperationType) {
                    oPinnedObject = oCommonFunctions.getPinnedObject();
                    oCommonFunctions.createListItemAndPin(oPinnedObject);
                }
            } else {
                if (oGlobalConstants.sOperationPin === oGlobalConstants.sOperationType) {
                    oPinnedObject = oCommonFunctions.getPinnedObject();
                    oCommonFunctions.updateList(oPinnedObject);
                } else if (oGlobalConstants.sOperationUnpin === oGlobalConstants.sOperationType) {
                    oPinnedObject = oCommonFunctions.getPinnedObject();
                    oCommonFunctions.removeList(oPinnedObject);
                }
            }
        }

        if (0 < iCount) {
            oCommonFunctions.checkAlreadyPinned(oGlobalConstants.documentPinnedDetails);
        }
        oCommonFunctions.populateMetadata();
    };

    /* Function to execute on failure of pin/ unpin */
    oCommonFunctions.onQueryFailed = function (sOperationType) {
        //// TODO
        return;
    };

    /* Function to retrieve pinned document list */
    oCommonFunctions.retrieveListItems = function () {
        // Wait for getCurrentUserTitle to complete
        if (!oGlobalConstants.sUserLoginName) {
            setTimeout(function () { oCommonFunctions.retrieveListItems(); }, 1000);
        } else {
            if (oGlobalConstants.sUserLoginName) {
                oGlobalConstants.clientContext = new SP.ClientContext(oDocumentLanding.sSPTenantUrl + oCommonLinks.sCatalogSite);
                oGlobalConstants.oList = oGlobalConstants.clientContext.get_web().get_lists().getByTitle(oGlobalConstants.sListName);
                var camlQuery = new SP.CamlQuery();
                camlQuery.set_viewXml("<View><Query><Where><Eq><FieldRef Name='" + oGlobalConstants.sUserAliasColumn + "'/> <Value Type='Text'>" + oGlobalConstants.sUserLoginName + "</Value></Eq></Where></Query><ViewFields> <FieldRef Name='" + oGlobalConstants.sPinColumn + "' /></ViewFields></View>");
                oGlobalConstants.collListItem = oGlobalConstants.oList.getItems(camlQuery);
                oGlobalConstants.clientContext.load(oGlobalConstants.collListItem);
                oGlobalConstants.clientContext.executeQueryAsync(Function.createDelegate(this, function () { oCommonFunctions.onQuerySucceeded(); }), Function.createDelegate(this, oCommonFunctions.onQueryFailed("RetrieveItems")));
            } else {
                onQueryFailed(oGlobalConstants.sOperationPin);
            }
        }
    };

    /* Function to pin the document */
    oCommonFunctions.pinDocument = function () {
        oGlobalConstants.sEventName = oAppInsights.sPin;
        oCommonFunctions.LogEvent(true);
        oGlobalConstants.sOperationType = oGlobalConstants.sOperationPin;
        oGlobalConstants.eventName = oGlobalConstants.Pin_Document;
        ////LogEvent();
        oCommonFunctions.retrieveListItems();
    };

    /* Function to unpin the document */
    oCommonFunctions.unPinDocument = function () {
        oGlobalConstants.sEventName = oAppInsights.sUnpin;
        oCommonFunctions.LogEvent(true);
        oGlobalConstants.sOperationType = oGlobalConstants.sOperationUnpin;
        oGlobalConstants.eventName = oGlobalConstants.Unpin_Document;
        ////LogEvent();
        oCommonFunctions.retrieveListItems();
    };

    /* Function to execute on success of document properties */
    oCommonFunctions.getDocumentPropertiesSuccess = function (oData) {
        oGlobalConstants.iSuccessCounter++;
        if (oData && oData.d) {
            if (oData.d.vti_x005f_timecreated) {
                var sOrignalDate = oData.d.vti_x005f_timecreated;
                if (null === sOrignalDate.match(/Z$/)) { //// If time created does not ends with Z i.e. zone information
                    sOrignalDate = sOrignalDate + "Z";
                }
                oDocumentLanding.sCreatedDateUTC = sOrignalDate;
                oDocumentLanding.sCreatedDate = oCommonFunctions.getDate(sOrignalDate);
            } else {
                oDocumentLanding.sCreatedDate = oGlobalConstants.sNotApplicable;
            }
            if (oData.d.vti_x005f_timelastmodified) {
                var sModifiedDate = oData.d.vti_x005f_timelastmodified;
                if (null === sModifiedDate.match(/Z$/)) { //// If time created does not ends with Z i.e. zone information
                    sModifiedDate = sModifiedDate + "Z";
                }
                oDocumentLanding.sModifiedDate = sModifiedDate;
            } else {
                oDocumentLanding.sModifiedDate = oGlobalConstants.sNotApplicable;
            }
            oDocumentLanding.iSize = oData.d.vti_x005f_filesize ? (oData.d.vti_x005f_filesize / 1024).toFixed(0) + "KB" : oGlobalConstants.sNotApplicable;
            if ("0KB" === oDocumentLanding.iSize && 0 < oData.d.vti_x005f_filesize) { //// File size is between 0 and 1 KB
                oDocumentLanding.iSize = "< 1KB";
            }
            oDocumentLanding.sFileTitle = oData.d.vti_x005f_title ? oData.d.vti_x005f_title : oGlobalConstants.sNotApplicable;
            oCommonFunctions.populateMetadata();
        }
    };

    /* Function to execute on failure of document properties */
    oCommonFunctions.getDocumentPropertiesFailure = function (sender, oError) {
        if (typeof oError.get_message === "function") {
            oCommonFunctions.showCommonErrorPopUp(oError.get_message());
        } else {
            oCommonFunctions.showCommonErrorPopUp(oErrorMessages.sErrorMessage);
        }
    };

    /* Function to get the document properties */
    oCommonFunctions.getDocumentProperties = function () {
        var sURL = oDocumentLanding.sDocumentAJAXCallURL + oDocumentLanding.sDocumentAJAXParameters.replace("{0}", oDocumentLanding.sCurrentDocumentRelativeUrl).replace("{1}", "Properties?$Select=" + oSelectProperties.sDocumentProperties);
        oCommonFunctions.getData(sURL, oCommonFunctions.getDocumentPropertiesSuccess, oCommonFunctions.getDocumentPropertiesFailure, "GET");
    };

    /* Function to execute on success of fetching matter details */
    oCommonFunctions.getMatterDetailsSuccess = function (oData) {
        oGlobalConstants.iSuccessCounter++;
        if (oData && oData.d) {
            oDocumentLanding.sMatterGUID = oData.d.MatterGUID ? oData.d.MatterGUID : oDocumentLanding.sDocumentParentList;
            oDocumentLanding.sDocumentListItemId = oData.d.vti_x005f_listname ? oData.d.vti_x005f_listname : oGlobalConstants.sNotApplicable;
            oDocumentLanding.sFilePropertiesURL = oDocumentLanding.sSPTenantUrl + oDocumentLanding.sClientRelativeUrl + "/" + "{0}" + oDocumentLanding.sDisplayFormURL;
            oDocumentLanding.sFilePropertiesURL = oData.d.MatterGUID ? oDocumentLanding.sFilePropertiesURL.replace("{0}", oData.d.MatterGUID) : oDocumentLanding.sFilePropertiesURL.replace("{0}", oDocumentLanding.sCurrentLibrary);
            oCommonFunctions.populateMetadata();
        }
    };

    /* Function to decode values from property bag */
    oCommonFunctions.decodeValue = function (sValue) {
        var div = document.createElement("div");
        div.innerHTML = sValue;
        return $(div).text();
    };

    /* Function to execute on failure while fetching matter details */
    oCommonFunctions.getMatterDetailsFailure = function (sender, oError) {
        if (typeof oError.get_message === "function") {
            oCommonFunctions.showCommonErrorPopUp(oError.get_message());
        } else {
            oCommonFunctions.showCommonErrorPopUp(oErrorMessages.sErrorMessage);
        }
    };

    /* Function to get the matter details */
    oCommonFunctions.getMatterDetails = function () {
        var sURL = oDocumentLanding.sDocumentAJAXCallURL + oDocumentLanding.sTitleAJAXParameters.replace("{0}", oDocumentLanding.sDocumentParentList).replace("{1}", "Properties?$Select=" + oSelectProperties.sMatterProperties);
        oCommonFunctions.getData(sURL, oCommonFunctions.getMatterDetailsSuccess, oCommonFunctions.getMatterDetailsFailure, "GET");
    };

    /* Function to execute on on success of author of the document */
    oCommonFunctions.getAuthorSuccess = function (oData) {
        oGlobalConstants.iSuccessCounter++;
        if (oData && oData.d) {
            oDocumentLanding.sAuthor = oData.d.Title ? oData.d.Title : oGlobalConstants.sNotApplicable;
            oDocumentLanding.sAuthorEmail = oData.d.Email ? oData.d.Email : oGlobalConstants.sNotApplicable;
            oCommonFunctions.populateMetadata();
            if (oGlobalConstants.sNotApplicable === oDocumentLanding.sAuthor) {
                $("#authorLyncIndicator").addClass("hide");
                $("#authorValue").addClass("buffer");
            }
        }
    };

    /* Function to execute on on failure of author of the document */
    oCommonFunctions.getAuthorFailure = function (sender, oError) {
        if (typeof oError.get_message === "function") {
            oCommonFunctions.showCommonErrorPopUp(oError.get_message());
        } else {
            oCommonFunctions.showCommonErrorPopUp(oErrorMessages.sErrorMessage);
        }
    };

    /* Function to get the author of the document */
    oCommonFunctions.getAuthor = function () {
        var sURL = oDocumentLanding.sDocumentAJAXCallURL + oDocumentLanding.sDocumentAJAXParameters.replace("{0}", oDocumentLanding.sCurrentDocumentRelativeUrl).replace("{1}", "Author?$Select=" + oSelectProperties.sAuthorProperties);
        oCommonFunctions.getData(sURL, oCommonFunctions.getAuthorSuccess, oCommonFunctions.getAuthorFailure, "GET");
    };

    /* Function to execute on success while getting checked out user of document */
    oCommonFunctions.getCheckOutUserSuccess = function (oData) {
        oGlobalConstants.iSuccessCounter++;
        if (oData && oData.d) {
            if ("undefined" === typeof oData.d.CheckedOutByUser) {
                oDocumentLanding.sCheckOutUser = "undefined" === typeof oData.d.Title ? oGlobalConstants.sNotApplicable : oData.d.Title;
                oDocumentLanding.sCheckOutUserEmail = "undefined" === typeof oData.d.Email ? oGlobalConstants.sNotApplicable : oData.d.Email;
                oDocumentLanding.sCheckOutUserPinned = oDocumentLanding.sCheckOutUser;
            } else {
                oDocumentLanding.sCheckOutUser = "None";
                oDocumentLanding.sCheckOutUserPinned = oGlobalConstants.sNotApplicable;
                $("#checkInDocument, #checkOutLyncIndicator").addClass("hide");
                $("#checkOutDocument").removeClass("hide");
                $("#checkOutUserValue").addClass("buffer");
            }
            oCommonFunctions.populateMetadata();
        }
        //// Disable Check out option
    };

    /* Function to execute on failure while getting checked out user of document */
    oCommonFunctions.getCheckOutUserFailure = function (sender, oError) {
        if (typeof oError.get_message === "function") {
            oCommonFunctions.showCommonErrorPopUp(oError.get_message());
        } else {
            oCommonFunctions.showCommonErrorPopUp(oErrorMessages.sErrorMessage);
        }
    };

    /* Function to get the checked out user of document */
    oCommonFunctions.getCheckOutUser = function () {
        var sURL = oDocumentLanding.sDocumentAJAXCallURL + oDocumentLanding.sDocumentAJAXParameters.replace("{0}", oDocumentLanding.sCurrentDocumentRelativeUrl).replace("{1}", "CheckedOutByUser?$Select=" + oSelectProperties.sCheckedOutUserProperties);
        oCommonFunctions.getData(sURL, oCommonFunctions.getCheckOutUserSuccess, oCommonFunctions.getCheckOutUserFailure, "GET");
    };

    /* Function to convert array to string */
    oCommonFunctions.convertToString = function (arrValues) {
        var sReturnValue = "";
        arrValues = $.map(arrValues, function (oItem) {
            return ($.trim(oItem.Label));
        });
        sReturnValue = arrValues.join("; ");
        return sReturnValue;
    };

    /* Function to execute on success while fetching the document item id */
    oCommonFunctions.getListItemSuccess = function (oData) {
        oGlobalConstants.iSuccessCounter++;
        if (oData && oData.d) {
            oDocumentLanding.sDocumentItemId = oData.d.Id ? oData.d.Id : oGlobalConstants.sNotApplicable;
            oDocumentLanding.sFilePropertiesURL += oDocumentLanding.sDocumentItemId;
            $("#viewMoreFileProperties").attr({ "target": "_self", "href": oDocumentLanding.sFilePropertiesURL }); //// Set the view more link URL for file properties
            oDocumentLanding.sClientID = oData.d[oSelectProperties.sClientID] ? oCommonFunctions.decodeValue(oData.d[oSelectProperties.sClientID]) : oGlobalConstants.sNotApplicable;
            oDocumentLanding.sMatterId = oData.d[oSelectProperties.sMatterID] ? oCommonFunctions.decodeValue(oData.d[oSelectProperties.sMatterID]) : oGlobalConstants.sNotApplicable;
            oDocumentLanding.sPracticeGroup = oData.d[oSelectProperties.sPracticeGroup] ? (oData.d[oSelectProperties.sPracticeGroup].results.length ? oCommonFunctions.decodeValue(oCommonFunctions.convertToString(oData.d[oSelectProperties.sPracticeGroup].results)) : oGlobalConstants.sNotApplicable) : oGlobalConstants.sNotApplicable;
            oDocumentLanding.sAreaOfLaw = oData.d[oSelectProperties.sAreaOfLaw] ? (oData.d[oSelectProperties.sAreaOfLaw].results.length ? oCommonFunctions.decodeValue(oCommonFunctions.convertToString(oData.d[oSelectProperties.sAreaOfLaw].results)) : oGlobalConstants.sNotApplicable) : oGlobalConstants.sNotApplicable;
            oDocumentLanding.sSubAreaOfLaw = oData.d[oSelectProperties.sSubAreaOfLaw] ? (oData.d[oSelectProperties.sSubAreaOfLaw].results.length ? oCommonFunctions.decodeValue(oCommonFunctions.convertToString(oData.d[oSelectProperties.sSubAreaOfLaw].results)) : oGlobalConstants.sNotApplicable) : oGlobalConstants.sNotApplicable;
            oDocumentLanding.sMatterName = oData.d[oSelectProperties.sMatterName] ? oCommonFunctions.decodeValue(oData.d[oSelectProperties.sMatterName]) : oGlobalConstants.sNotApplicable;
            oDocumentLanding.sDocumentClient = oData.d[oSelectProperties.sClientName] ? oCommonFunctions.decodeValue(oData.d[oSelectProperties.sClientName]) : oGlobalConstants.sNotApplicable;
            oCommonFunctions.populateMetadata();
        }
    };

    /* Function to execute on failure while fetching the document item id */
    oCommonFunctions.getListItemFailure = function (sender, oError) {
        if (typeof oError.get_message === "function") {
            oCommonFunctions.showCommonErrorPopUp(oError.get_message());
        } else {
            oCommonFunctions.showCommonErrorPopUp(oErrorMessages.sErrorMessage);
        }
    };

    /* Function to get the document item id */
    oCommonFunctions.getListItem = function () {
        var sListItemProperties = oSelectProperties.sItemId + "," + oSelectProperties.sClientID + "," + oSelectProperties.sMatterID + "," + oSelectProperties.sClientName + "," + oSelectProperties.sMatterName + "," + oSelectProperties.sPracticeGroup + "," + oSelectProperties.sAreaOfLaw + "," + oSelectProperties.sSubAreaOfLaw, sURL;
        sURL = oDocumentLanding.sDocumentAJAXCallURL + oDocumentLanding.sDocumentAJAXParameters.replace("{0}", oDocumentLanding.sCurrentDocumentRelativeUrl).replace("{1}", "ListItemAllFields?$Select=" + sListItemProperties);
        oCommonFunctions.getData(sURL, oCommonFunctions.getListItemSuccess, oCommonFunctions.getListItemFailure, "GET");
    };

    /* Function to share the document */
    oCommonFunctions.share = function () {
        oGlobalConstants.sEventName = oAppInsights.sShare;
        oCommonFunctions.LogEvent(true);
        var oDialogOptions = SP.UI.$create_DialogOptions();
        oDialogOptions.title = "Share '" + oDocumentLanding.sCurrentDocumentName + "'";
        oDialogOptions.url = oDocumentLanding.sSPTenantUrl + oDocumentLanding.sClientRelativeUrl + oDocumentLanding.sShareURL.replace("{0}", oDocumentLanding.sDocumentListItemId).replace("{1}", oDocumentLanding.sDocumentItemId.toString());
        oDialogOptions.dialogReturnValueCallback = function (oDialogResult, sReturnValue) {
            oCommonFunctions.onSPModalPopupClose(oDialogResult, sReturnValue);
        };
        SP.UI.ModalDialog.showModalDialog(oDialogOptions);
    };

    /* Function to execute on success while fetching checked out status */
    oCommonFunctions.checkOutSuccess = function () {
        window.location.reload(); // Forces a reload of page
    };

    /* Function to execute on failure while fetching checked out status */
    oCommonFunctions.checkOutFailure = function (sender, oError) {
        if (sender) {
            var error = JSON.parse(sender.responseText);
            oCommonFunctions.showCommonErrorPopUp(error.error.message.value);
        }
    };

    /* Function to check out the document */
    oCommonFunctions.checkOut = function () {
        oGlobalConstants.sEventName = oAppInsights.sCheckOut;
        oCommonFunctions.LogEvent(true);
        $("#checkOutImage").addClass("hide");
        $("#checkOutLoading").removeClass("hide");
        $.ajax({
            url: oDocumentLanding.sDocumentAJAXCallURL + oDocumentLanding.sContextInfoURL,
            method: "POST",
            headers: {
                "ACCEPT": "application/json;odata=verbose",
            },
            success: function (oData) {
                oDocumentLanding.sFormDigest = oData.d.GetContextWebInformation.FormDigestValue;
                var sURL = oDocumentLanding.sDocumentAJAXCallURL + oDocumentLanding.sDocumentAJAXParameters.replace("{0}", oDocumentLanding.sCurrentDocumentRelativeUrl).replace("{1}", "CheckOut()");
                oCommonFunctions.getData(sURL, oCommonFunctions.checkOutSuccess, oCommonFunctions.checkOutFailure, "POST");
            },
            error: oCommonFunctions.checkOutFailure
        });
    };

    /* Function to execute on success while fetching the version information */
    oCommonFunctions.getVersionInfoSuccess = function (oData) {
        oGlobalConstants.iSuccessCounter++;
        if (oData && oData.d) {
            $("#documentMetadata li:first-child").addClass("hide");

            var sVersionInfo = oData.d.results, sHTMLChunk = "<li><span class=\"versionNumber ellipsis\" title=\"{0}\">{0}</span><span class=\"modifiedDate ellipsis\" title=\"{1}\">{1}</span><span class=\"ms-verticalAlignTop ms-noWrap\"><span class=\"ms-imnSpan\"><a class=\"ms-imnlink ms-spimn-presenceLink\" onclick='WriteDocEngagementLog(\"DocModifiedByPresenceClick\", \"ODModifiedByPresenceClick\"); IMNImageOnClick(event);return false;' href=\"#\"><span class=\"customPresence ms-spimn-presenceWrapper ms-imnImg ms-spimn-imgSize-10x10\"><img name=\"imnmark\" title=\"\" class=\"ms-spimn-img ms-spimn-presence-disconnected-10x10x32\" id=\"imn_{4},type=sip\" alt=\"No presence information\" src=\"/_layouts/15/images/spimn.png\" sip=\"{3}\" showofflinepawn=\"1\"></span></a></span></span><span class=\"modifiedBy ellipsis\" title=\"{2}\">{2}</span></li>", sData = "";
            sData = sHTMLChunk.replace(/\{0\}/g, oDocumentLanding.oCurrenVersionDetails.sCurrentVersionNumber).replace(/\{1\}/g, oCommonFunctions.getDate(oDocumentLanding.oCurrenVersionDetails.sCurrentModifiedDate)).replace(/\{2\}/g, oDocumentLanding.oCurrenVersionDetails.sCurrentModifiedBy).replace("{3}", oDocumentLanding.oCurrenVersionDetails.sCurrentModifiedByEmail).replace("{4}", 0);
            if (sVersionInfo.length) {
                sVersionInfo = sVersionInfo.reverse();
                $.each(sVersionInfo, function (sKey, sValue) {
                    if (4 > sKey) {
                        var sCreatedDate = oCommonFunctions.getDate(sValue.Created);
                        sData = sData + sHTMLChunk.replace(/\{0\}/g, sValue.VersionLabel).replace(/\{1\}/g, sCreatedDate).replace(/\{2\}/g, sValue.CreatedBy.Title).replace("{3}", sValue.CreatedBy.Email).replace("{4}", sKey + 1);
                    } else {
                        return false;
                    }
                });
            }
            $("#versionHeader").removeClass("hide");
            $("#documentMetadata").append(sData);
            oCommonFunctions.populateMetadata();
        }
    };

    /* Function to execute on failure while fetching the version information */
    oCommonFunctions.getVersionInfoFailure = function (sender, oError) {
        if (typeof oError.get_message === "function") {
            oCommonFunctions.showCommonErrorPopUp(oError.get_message());
        } else {
            oCommonFunctions.showCommonErrorPopUp(oErrorMessages.sErrorMessage);
        }
    };

    /* Function to get the version information of document */
    oCommonFunctions.getVersionInfo = function () {
        var sURL = oDocumentLanding.sDocumentAJAXCallURL + oDocumentLanding.sDocumentAJAXParameters.replace("{0}", oDocumentLanding.sCurrentDocumentRelativeUrl).replace("{1}", "Versions?$expand=CreatedBy");
        oCommonFunctions.getData(sURL, oCommonFunctions.getVersionInfoSuccess, oCommonFunctions.getVersionInfoFailure, "GET");
    };

    /* Function to execute on success of fetching OWA URL (interactive preview) */
    oCommonFunctions.getOWAUrlSuccess = function (oData) {
        if (oData && oData.d && oData.d.GetWOPIFrameUrl) {
            oDocumentLanding.sWOPIFrameDocumentURL = oData.d.GetWOPIFrameUrl;
            $("#documentPreviewSource").attr("src", oDocumentLanding.sWOPIFrameDocumentURL);
            $("#documentPreviewSource").removeClass("hide");
        } else {
            oDocumentLanding.sWOPIFrameDocumentURL = oDocumentLanding.sCurrentDocumentUrl;
            $("#documentPreviewSource").addClass("hide");
            $("#documentPreviewNotSupported").removeClass("hide");
        }
    };

    /* Function to execute on success of fetching OWA URL (view) */
    oCommonFunctions.getOpenUrlSuccess = function (oData) {
        if (oData && oData.d && oData.d.GetWOPIFrameUrl) {
            oDocumentLanding.sWOPIFrameDocumentURL = oData.d.GetWOPIFrameUrl;
            $("#openDocument a").attr({ "target": "_self", "href": oDocumentLanding.sWOPIFrameDocumentURL }); //// Set the href for open action                        
        } else {
            oDocumentLanding.sWOPIFrameDocumentURL = oDocumentLanding.sCurrentDocumentUrl;
            $("#openDocument").addClass("hide"); //// Hide open action as WOPI frame URL is not present            
        }
        $("#metadataProperties li:first-child, .actionChild:first-child, #loadingDocumentPreview").addClass("hide");
        $("#metadataProperties li:not(:first-child), .actionChild:not(:first-child)").removeClass("hide");
    };

    /* Function to execute on failure while fetching the OWA URL */
    oCommonFunctions.getOWAUrlFailure = function (sender, oError) {
        if (sender) {
            var error = JSON.parse(sender.responseText);
            oCommonFunctions.showCommonErrorPopUp(error.error.message.value);
        }
    };

    /* Function to get the OWA URL of document */
    oCommonFunctions.getOWAUrl = function (sAction, sSuccessFunction) {
        $.ajax({
            url: oDocumentLanding.sDocumentAJAXCallURL + oDocumentLanding.sContextInfoURL,
            method: "POST",
            headers: {
                "ACCEPT": "application/json;odata=verbose",
            },
            success: function (oData) {
                oDocumentLanding.sFormDigest = oData.d.GetContextWebInformation.FormDigestValue;
                var sURL = oDocumentLanding.sDocumentAJAXCallURL + oDocumentLanding.sOWAAJAXParameters.replace("{0}", oDocumentLanding.sDocumentListItemId.replace("{", "").replace("}", "")).replace("{1}", oDocumentLanding.sDocumentItemId).replace("{2}", sAction);
                oCommonFunctions.getData(sURL, sSuccessFunction, oCommonFunctions.getOWAUrlFailure, "POST");
            },
            error: oCommonFunctions.getOWAUrlFailure
        });
    };

    /* Function to check in the document */
    oCommonFunctions.checkIn = function () {
        oGlobalConstants.sEventName = oAppInsights.sCheckIn;
        oCommonFunctions.LogEvent(true);
        var oDialogOptions = SP.UI.$create_DialogOptions();
        oDialogOptions.url = oDocumentLanding.sSPTenantUrl + oDocumentLanding.sClientRelativeUrl + oDocumentLanding.sCheckInURL.replace("{0}", oDocumentLanding.sCurrentDocumentUrl);
        oDialogOptions.dialogReturnValueCallback = function (oDialogResult, sReturnValue) {
            oCommonFunctions.onSPModalPopupClose(oDialogResult, sReturnValue);
        };
        SP.UI.ModalDialog.showModalDialog(oDialogOptions);
    };

    /* Function to show common error pop up */
    oCommonFunctions.showCommonErrorPopUp = function (sErrorPopUpData) {
        $("#metadataProperties li:first-child, .actionChild:first-child, #loadingDocumentPreview").addClass("hide");
        $("#metadataProperties li:not(:first-child), .actionChild:not(:first-child), #documentPreviewSource").removeClass("hide");
        $("#documentMetadata li:first-child").addClass("hide");
        $("#checkOutLyncIndicator, #authorLyncIndicator").addClass("hide");
        $("#openDocument, #download, #shareDocument, #checkInDocument, #checkOutDocument, #sendLink, #sendToOneDrive, #pinDocument, #unpinDocument, #viewMoreVersions").off();
        $(".errorPopUpHolder").removeClass("hide");
        $("#genericMessage").text(oErrorMessages.sErrorMessage);
        $("#errorMessage").text(sErrorPopUpData).attr("title", sErrorPopUpData);
    };

    /* Function to toggle current section */
    oCommonFunctions.toggleSection = function (oElement, iCurrentSection) {
        if (oElement.length) {
            if (oElement.is(":visible")) {
                oElement.slideUp();
                //// Update the toggle image
                if (iCurrentSection) {
                    $(".toggleVersionSection").removeClass("hide");
                    $("#CloseVersionHistory").addClass("hide");
                } else {
                    $(".toggleFileSection").removeClass("hide");
                    $("#CloseFileProperties").addClass("hide");
                }
            } else {
                oElement.slideDown();
                //// Update the toggle image
                if (iCurrentSection) {
                    $(".toggleVersionSection").removeClass("hide");
                    $("#OpenVersionHistory").addClass("hide");
                } else {
                    $(".toggleFileSection").removeClass("hide");
                    $("#OpenFileProperties").addClass("hide");
                }
            }
        }
    };

    /* Function to populate meta-data of document */
    oCommonFunctions.populateMetadata = function () {
        if (7 === oGlobalConstants.iSuccessCounter) {
            oCommonFunctions.getDocumentOWAURL();
            //// Set the unique access for document
            if (oDocumentLanding.sCurrentDocumentUniqueAccess) {
                $("#accessValue").text("Unique");
            } else {
                $("#accessValue").text("Same as matter");
            }
            $("#accessValue").attr({ "target": "_self", "href": oDocumentLanding.sSPTenantUrl + oDocumentLanding.sClientRelativeUrl + oDocumentLanding.sPermissionURL.replace("{0}", oDocumentLanding.sDocumentParentListId).replace("{1}", oDocumentLanding.sDocumentItemId) });
            $("#fileTitleValue").text(oDocumentLanding.sFileTitle).attr("title", oDocumentLanding.sFileTitle);
            $("#clientValue").text(oDocumentLanding.sDocumentClient).attr("title", oDocumentLanding.sDocumentClient);
            $("#matterValue").text(oDocumentLanding.sMatterName).attr("title", oDocumentLanding.sMatterName);
            $("#clientMatterIdValue").text(oDocumentLanding.sClientID + "." + oDocumentLanding.sMatterId).attr("title", oDocumentLanding.sClientID + "." + oDocumentLanding.sMatterId);
            $("#documentIdValue").text(oDocumentLanding.sCurrentDocumentID).attr("title", oDocumentLanding.sCurrentDocumentID);
            $("#checkOutUserValue").text(oDocumentLanding.sCheckOutUser).attr("title", oDocumentLanding.sCheckOutUser);
            $("#checkOutLyncIndicator .customLyncPresence").attr("sip", oDocumentLanding.sCheckOutUserEmail);
            $("#authorValue").text(oDocumentLanding.sAuthor).attr("title", oDocumentLanding.sAuthor);
            $("#authorLyncIndicator .customLyncPresence").attr("sip", oDocumentLanding.sAuthorEmail);
            $("#dateCreatedValue").text(oDocumentLanding.sCreatedDate).attr("title", oDocumentLanding.sCreatedDate);
            $("#practiceGroupValue").text(oDocumentLanding.sPracticeGroup).attr("title", oDocumentLanding.sPracticeGroup);
            $("#areaOfLawValue").text(oDocumentLanding.sAreaOfLaw).attr("title", oDocumentLanding.sAreaOfLaw);
            $("#subAreaOfLaw").text(oDocumentLanding.sSubAreaOfLaw).attr("title", oDocumentLanding.sSubAreaOfLaw);
            $("#fileTypeValue").text(oDocumentLanding.sCurrentDocumentType).attr("title", oDocumentLanding.sCurrentDocumentType);
            $("#fileSizeValue").text(oDocumentLanding.iSize).attr("title", oDocumentLanding.iSize);
            if (_spPageContextInfo.userLoginName.toUpperCase() !== oDocumentLanding.sCheckOutUserEmail.toUpperCase()) {
                $("#checkInDocument").addClass("hide");
            }
            ProcessImn(); //// Refresh the lync status of user
        }
    };

    /* Function to apply binding son DOM elements */
    oCommonFunctions.applyBindings = function () {
        //// #region Application Insights
        $("#download").click(function () {
            oGlobalConstants.sEventName = oAppInsights.sDownload;
            oCommonFunctions.LogEvent(true);
        });

        $("#viewMoreFileProperties").click(function () {
            oGlobalConstants.sEventName = oAppInsights.sFile;
            oCommonFunctions.LogEvent(true);
        });

        $("#openDocument").click(function () {
            oGlobalConstants.sEventName = oAppInsights.sOpen;
            oCommonFunctions.LogEvent(true);
        });

        $("#gotoAllDocuments").click(function () {
            oGlobalConstants.sEventName = oAppInsights.sDocument;
            oCommonFunctions.LogEvent(true);
        });

        $("#matterLink").click(function () {
            oGlobalConstants.sEventName = oAppInsights.sMatter;
            oCommonFunctions.LogEvent(true);
        });

        $("#documentLink").click(function () {
            oGlobalConstants.sEventName = oAppInsights.sDocument;
            oCommonFunctions.LogEvent(true);
        });

        $("#settingsLink").click(function () {
            oGlobalConstants.sEventName = oAppInsights.sSettings;
            oCommonFunctions.LogEvent(true);
        });

        $("#mcIcon").click(function () {
            oGlobalConstants.sEventName = oAppInsights.sMatter;
            oCommonFunctions.LogEvent(true);
        });

        $(".searchIcon").click(function () {
            oGlobalConstants.sEventName = oAppInsights.sSearch;
            oCommonFunctions.LogEvent(true);
        });

        $("#footerLogo").click(function () {
            oGlobalConstants.sEventName = oAppInsights.sMicrosoft;
            oCommonFunctions.LogEvent(false);
        });

        $("#feedbackSupport").click(function () {
            oGlobalConstants.sEventName = oAppInsights.sFeedback;
            oCommonFunctions.LogEvent(false);
        });

        $("#privacyLink").click(function () {
            oGlobalConstants.sEventName = oAppInsights.sPrivacy;
            oCommonFunctions.LogEvent(false);
        });

        $("#termsOfUse").click(function () {
            oGlobalConstants.sEventName = oAppInsights.sTermsOfUse;
            oCommonFunctions.LogEvent(false);
        });

        $("#accessValue").click(function () {
            oGlobalConstants.sEventName = oAppInsights.sSharedProperties;
            oCommonFunctions.LogEvent(true);
        });

        ////#endregion

        $("#gotoAllDocuments a").attr({ "target": "_self", "href": oDocumentLanding.sSPTenantUrl + oFooterLinks.dashboard + oFooterLinks.dashboardDocumentsQueryString });

        $("#sendLink").click(function () {
            oCommonFunctions.sendDocumentAsLink();
        });

        $("#sendToOneDrive").click(function () {
            oCommonFunctions.sendDocumentToOneDrive();
        });

        $("#pinDocument").click(function () {
            oCommonFunctions.pinDocument();
        });

        $("#unpinDocument").click(function () {
            oCommonFunctions.unPinDocument();
        });

        $("#checkOutDocument").click(function () {
            oCommonFunctions.checkOut();
        });

        $("#checkInDocument").click(function () {
            oCommonFunctions.checkIn();
        });

        $("#viewMoreVersions").click(function () {
            oCommonFunctions.openVersionHistory();
        });

        $("#shareDocument").click(function () {
            oCommonFunctions.share();
        });

        $(document).on("click", ".ErrorPopUpCloseIcon, .ErrorPopupBackground", function () {
            $(".ErrorPopUpHolder").addClass("hide");
        });

        $(".toggleFileSection").click(function () {
            var oFileProperties = $("#metadataProperties");
            oCommonFunctions.toggleSection(oFileProperties, 0);
        });

        $(".toggleVersionSection").click(function () {
            var oVersionProperties = $("#documentMetadataContainer");
            oCommonFunctions.toggleSection(oVersionProperties, 1);
        });

        $(".errorPopUpCloseIcon").on("click", function (event) {
            $(".errorPopUpHolder").addClass("hide");
        });

        $(".popupBackground").click(function () {
            $(".closeIcon").click();
        });

        $("#expandMessage, #collapseMessage").click(function () {
            if ($("#errorMessage").is(":visible")) {
                $("#errorMessage, #collapseMessage").addClass("hide");
                $("#expandMessage").removeClass("hide");
            } else {
                $("#errorMessage, #collapseMessage").removeClass("hide");
                $("#expandMessage").addClass("hide");
            }
        });
    };

    /* Function to log application insights events */
    oCommonFunctions.LogEvent = function (bInherited) {
        var appInsights = initializeApplicationInsights();
        var sEventName = bInherited ? oAppInsights.sPageName + oGlobalConstants.sEventName : oGlobalConstants.sEventName;
        appInsights.trackEvent(sEventName);
    };

    /* Function to get formatted date */
    oCommonFunctions.getDate = function (sDate) {
        var oDate = new Date(sDate), sDay, sMonth, sYear, sFormattedDate = oGlobalConstants.sNotApplicable;
        if (oDate) {
            sDay = oDate.getUTCDate();
            sMonth = oDocumentLanding.oMonths[oDate.getUTCMonth()];
            sYear = oDate.getUTCFullYear();
            sFormattedDate = sMonth + " " + sDay + ", " + sYear;
        }
        return sFormattedDate;
    };

    /* Function to trim trailing character if present */
    oCommonFunctions.trimEndChar = function (sOriginalString, sCharToTrim) {
        var sTrimmedString = sOriginalString;
        if (sCharToTrim === sOriginalString.substr(-1)) {
            sTrimmedString = sOriginalString.substr(0, sOriginalString.length - 1);
        }
        return sTrimmedString;
    };
    /* Function used to get UTC date */
    oCommonFunctions.getUTCDate = function (sOriginalDate) {
        var oDate = new Date(sOriginalDate), sUTCDate, sTimePeriod, sHours, sUTCHours;
        sUTCHours = oDate.getUTCHours();
        if (sUTCHours > 11) {
            sTimePeriod = "PM";
            sHours = sUTCHours > 12 ? sUTCHours - 12 : sUTCHours;
        } else {
            sTimePeriod = "AM";
            sHours = sUTCHours;
        }
        if (0 === sHours) {
            sHours = 12;
        }
        var sUTCDate = oDate.getUTCMonth() + 1 + "/" + oDate.getUTCDate() + "/" + oDate.getUTCFullYear() + " " + sHours + ":" + oDate.getUTCMinutes() + ":" + oDate.getUTCSeconds() + " " + sTimePeriod; //// Added 1 in the UTC month as it returns value from 0
        return sUTCDate;

    };

    /* Function used to escape special characters */
    oCommonFunctions.escapeRegExp = function (string) {
        return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
    };
    /* Function used to handle special characters */
    oCommonFunctions.replaceAll = function (string, find, replace) {
        return string.replace(new RegExp(oCommonFunctions.escapeRegExp(find), "g"), replace);
    };

 oCommonFunctions.applyConfigSettings = function () {
 
 
	$("#tabTitle").html(uiconfigs.DocumentDetails.Label1TabTitleText);
	$("#collapseMessageDetails").html(uiconfigs.DocumentDetails.Link1ErrorDetailsCaptionText);
	$("#gotoAllDocuments").attr("title",uiconfigs.DocumentDetails.Link2AllDocumentText);
	$("#linkGoToAllDocument").html(uiconfigs.DocumentDetails.Link2AllDocumentText);
	$("#previewUnsupportedText").html(uiconfigs.DocumentDetails.LabelNoPreviewText);
	$("#imgNoPreview").attr("title",uiconfigs.DocumentDetails.ImgNoPreviewSuppoertText);
	$("#spanViewMore").attr("title",uiconfigs.DocumentDetails.Link3ViewMoreText);
	$("#viewMoreFileProperties").html(uiconfigs.DocumentDetails.Link3ViewMoreText); 
	$("#viewMoreVersions").attr("title",uiconfigs.DocumentDetails.Link3ViewMoreText);
	$("#viewMoreVersions").html(uiconfigs.DocumentDetails.Link3ViewMoreText);
	
	$("#spanDocumentName").html(uiconfigs.DocumentDetails.Label1HeaderText+ ":");
	$("#Section1Header1").html(uiconfigs.DocumentDetails.Label2Section2HeaderText);
 
	$("#Section1Header1").html(uiconfigs.DocumentDetails.Label2Section2HeaderText);
	$("#Section1Column1").html(uiconfigs.DocumentDetails.Label1HeaderText + ":");
	$("#Section1Column1").attr("title",uiconfigs.DocumentDetails.Label1HeaderText);
	$("#Section1Column2").html(uiconfigs.DocumentDetails.Label4Section2Column2Text+ ":");
	$("#Section1Column2").attr("title",uiconfigs.DocumentDetails.Label4Section2Column2Text);
	$("#Section1Column3").html(uiconfigs.DocumentDetails.Label5Section2Column3Text+ ":");
	$("#Section1Column3").attr("title",uiconfigs.DocumentDetails.Label5Section2Column3Text);
	$("#Section1Column4").html(uiconfigs.DocumentDetails.Label6Section2Column4Text+ ":");
	$("#Section1Column4").attr("title",uiconfigs.DocumentDetails.Label6Section2Column4Text);
	$("#Section1Column5").html(uiconfigs.DocumentDetails.Label7Section2Column5Text+ ":");
	$("#Section1Column5").attr("title",uiconfigs.DocumentDetails.Label7Section2Column5Text);
	$("#Section1Column6").html(uiconfigs.DocumentDetails.Label8Section2Column6Text+ ":");
	$("#Section1Column6").attr("title",uiconfigs.DocumentDetails.Label8Section2Column6Text);
	$("#Section1Column7").html(uiconfigs.DocumentDetails.Label9Section2Column7Text+ ":");
	$("#Section1Column7").attr("title",uiconfigs.DocumentDetails.Label9Section2Column7Text);
	$("#Section1Column8").html(uiconfigs.DocumentDetails.Label10Section2Column8Text+ ":");
	$("#Section1Column8").attr("title",uiconfigs.DocumentDetails.Label10Section2Column8Text);
	$("#Section1Column9").html(uiconfigs.DocumentDetails.Label11Section2Column9Text+ ":");
	$("#Section1Column9").attr("title",uiconfigs.DocumentDetails.Label11Section2Column9Text);
	$("#Section1Column10").html(uiconfigs.DocumentDetails.Label12Section2Column10Text+ ":");
	$("#Section1Column10").attr("title",uiconfigs.DocumentDetails.Label12Section2Column10Text);
	$("#Section1Column11").html(uiconfigs.DocumentDetails.Label13Section2Column11Text+ ":");
	$("#Section1Column11").attr("title",uiconfigs.DocumentDetails.Label13Section2Column11Text);
	$("#Section1Column12").html(uiconfigs.DocumentDetails.Label14Section2Column12Text+ ":");
	$("#Section1Column12").attr("title",uiconfigs.DocumentDetails.Label14Section2Column12Text);
	$("#Section1Column13").html(uiconfigs.DocumentDetails.Label15Section2Column13Text+ ":");
	$("#Section1Column13").attr("title",uiconfigs.DocumentDetails.Label15Section2Column13Text);
	$("#Section1Column14").html(uiconfigs.DocumentDetails.Label16Section2Column14Text+ ":");
	$("#Section1Column14").attr("title",uiconfigs.DocumentDetails.Label16Section2Column14Text);
 
  
	$("#Section2Header").html(uiconfigs.DocumentDetails.Label17Section2HeaderText);
	$("#Section2Column1").html(uiconfigs.DocumentDetails.Label18Section2Column1Text);
	$("#Section2Column2").html(uiconfigs.DocumentDetails.Label19Section2Column2Text);
	$("#Section2Column3").html(uiconfigs.DocumentDetails.Label20Section2Column3Text);
	
	$("#spanOpenDownload").html(uiconfigs.DocumentDetails.Label21MenuHeaderOption1Text);
	$("#spanDownload").html(uiconfigs.DocumentDetails.Label22MenuHeaderOption2Text);
	$("#spanShare").html(uiconfigs.DocumentDetails.Label23MenuHeaderOption3Text);
	$("#spanCheckin").html(uiconfigs.DocumentDetails.Label24MenuHeaderOption4Text);
	$("#spanCheckOut").html(uiconfigs.DocumentDetails.Label25MenuHeaderOption5Text);
	$("#spanSendLink").html(uiconfigs.DocumentDetails.Label26MenuHeaderOption6Text);
	$("#spanPin").html(uiconfigs.DocumentDetails.Label27MenuHeaderOption7Text);
	$("#spanUnpin").html(uiconfigs.DocumentDetails.Label28MenuHeaderOption8Text);

	$("#openDocument").attr("title",uiconfigs.DocumentDetails.Label21MenuHeaderOption1Text);
	$("#download").attr("title",uiconfigs.DocumentDetails.Label22MenuHeaderOption2Text);
	$("#shareDocument").attr("title",uiconfigs.DocumentDetails.Label23MenuHeaderOption3Text);
	$("#checkInDocument").attr("title",uiconfigs.DocumentDetails.Label24MenuHeaderOption4Text);
	$("#checkOutDocument").attr("title",uiconfigs.DocumentDetails.Label25MenuHeaderOption5Text);
	$("#sendLink").attr("title",uiconfigs.DocumentDetails.Label26MenuHeaderOption6Text);
	$("#pinDocument").attr("title",uiconfigs.DocumentDetails.Label27MenuHeaderOption7Text);
	$("#unpinDocument").attr("title",uiconfigs.DocumentDetails.Label28MenuHeaderOption8Text);
       
	}

    /* Function to be executed once DOM loaded completely */
    $(document).ready(function () {
        $("title").text(oGlobalConstants.sPageTitle);
        displayHeaderAndFooterContent();
        oDocumentLanding.sClientRelativeUrl = oCommonFunctions.getParameterByName("client");
        oDocumentLanding.sDocumentParentListId = oCommonFunctions.getParameterByName("listguid");
        oDocumentLanding.sDocumentGUID = oCommonFunctions.getParameterByName("docguid");
        oCommonFunctions.applyBindings();
        oCommonFunctions.applyConfigSettings();
        if ("" === oDocumentLanding.sClientRelativeUrl || "" === oDocumentLanding.sDocumentParentListId || "" === oDocumentLanding.sDocumentGUID) {
            oCommonFunctions.showCommonErrorPopUp(oErrorMessages.sInsufficientParametersMessage);
        } else {
            SP.SOD.executeOrDelayUntilScriptLoaded(function () {
                oDocumentLanding.enumWOPIFrame = new SP.Utilities.SPWOPIFrameAction;
                oDocumentLanding.sSPTenantUrl = oCommonFunctions.getTenantUrl();
                oDocumentLanding.sDocumentAJAXCallURL = oDocumentLanding.sSPTenantUrl + oDocumentLanding.sClientRelativeUrl;
                getCurrentUserTitle(); //// Get the current logged in user title
                oCommonFunctions.getDocumentURL();
            }, "sp.js");
        }
    });
})();

(function () {
    "use strict";
    function receiveMessage(event) {
        if (event.data && "CloseOneDriveActionDialog" === event.data) {
            window.location.reload();
        }
    }

    window.onload = function () {
        if ("addEventListener" in window) {
            window.addEventListener("message", receiveMessage, true);
        } else if ("attachEvent" in window) { // IE
            window.attachEvent("onmessage", receiveMessage);
        }
    };
})();