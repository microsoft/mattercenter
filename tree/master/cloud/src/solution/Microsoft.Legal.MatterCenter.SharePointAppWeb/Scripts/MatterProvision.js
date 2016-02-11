/// <disable>JS1003,JS2076,JS2032,JS2064,JS2074,JS2024,JS2026,JS2005,JS3085,JS3116,JS3092,JS3057,JS3058,JS3056</disable>
//// Above exclusion list is signed off as per the Modern Cop Exclusion list.

/* Provision Matter Global Object */
var oMatterProvisionObject = (function () {
    "use strict";
    // #region Global Declaration
    /* Object to store all the global constants */
    var serviceConstants = {
        sCentralRepositoryLocation: oGlobalConstants.Central_Repository_Url,
        oTermStoreJson: {},
        iCurrentPage: 1,
        bClient: false,
        bRole: false,
        bPermission: false,
        bContentType: false,
        bValidMatterName: true,
        sPracticeGroupList: "",
        sAreaOfLawList: "",
        sSubAreaOfLawList: "",
        iRefreshCycle: oGlobalConstants.Refresh_Local_Storage,
        oMandatoryRoles: [],
        oMandatoryRoleNames: [],
        arrUserUploadPermissions: oGlobalConstants.User_Upload_Permissions && oGlobalConstants.User_Upload_Permissions.trim().split(","),
        arrReadOnlyUsers: [],
        iShowSuccessMessage: 0,
        iLastRowNumber: 2,
        bMatterLandingPage: true,
        bViewCreationFailed: false,
        bErrorOccured: false,
        sConflictScenario: "True",
        oDefaultConfiguration: [],
        isNextClicked: false,
        iPageClicked: 1,
        isValidClient: true,
        isErrorOccurred: false,
        iCallsComplete: 0,
        bCloseErrorPopup: true,
        matterGUID: "",
        oEmailRegex: new RegExp(oGlobalConstants.Email_Validation_Regex)
    },
    /* Object to store all the JQuery binding elements used */
    oJQueryObjects = {
        $Loading: "",
        $LoadingLookUp: "",
        $ClientName: "",
        $ClientId: "",
        $MatterName: "",
        $MatterId: "",
        $MatterDescription: "",
        $ContentTypes: "",
        $ConflictCheckConducted: "",
        $ConflictConductedBy: "",
        $ConflictCheckDate: "",
        $ConflictIdentified: "",
        $BlockedUsers: "",
        $BoolSecureMatter: "",
        $AssignPermissions: "",
        $CreateButton: "",
        $LookUpButton: "",
        $ErrorPopUp: "",
        $MenuClick: "",
        $PracticeGroup: "",
        $SwitchApp: "",
        $Paginate: ""
    },
    commonFunctions = {};    /* Object to store all the common functions */
    // #endregion

    /* Function to generate 32 bit GUID */
    function get_GUID() {
        function create_GUID(bFlag) {
            var sCurrentGUID = (Math.random().toString(16) + "000000000").substr(2, 8);
            return bFlag ? "-" + sCurrentGUID.substr(0, 4) + "-" + sCurrentGUID.substr(4, 4) : sCurrentGUID;
        }
        return create_GUID() + create_GUID(true) + create_GUID(true) + create_GUID();
    }

    /* Function to get the GUID by removing the hyphen character */
    function getMatterGUID() {
        serviceConstants.matterGUID = get_GUID().replace(/-/g, ""); //// Remove '-' (hyphen) from the GUID as this character is removed from URL by SharePoint
    }

    /* Function to display notifications in App */
    function showNotification(sElement, sMsg, sClass) {
        var sContent = "<div class='notification'>" + sMsg + "<span class='closeNotification'>x</span></div>";
        $(sElement).removeClass("successBanner warningBanner failureBanner").addClass(sClass).append(sContent);
    }

    /* Function to display error Tool tip */
    function showErrorNotification(sElement, sMsg) {
        var windowWidth = GetWidth();
        var removeTop = 75, removeLeft = 115;
        var posLeft = "50px";
        var triangleTopPos = 48;
        var errorBoxHeight = 55;
        var errorBoxTop = 7;
        var errorBoxLeft = 24;
        var oSecurityGroupError = sMsg.split("$|$");
        var bSecurityGroupError = false;
        if (2 === oSecurityGroupError.length) {
            bSecurityGroupError = true;
        }
        var oRoleObject = $(sElement);
        if (oRoleObject) {
            var iLeftPos = oRoleObject.offset().left
    , iTopPos = oRoleObject.offset().top
    , iCurrentWidth = oRoleObject.outerWidth();
            iLeftPos = parseInt(iLeftPos, 10) + parseInt(iCurrentWidth, 10) - 20;
            iTopPos = parseInt(iTopPos, 10) - 20;
            oJQueryObjects.$ErrorPopUp = $(".mainArea .errorPopUp");
            if (windowWidth > 734) {
                if (sElement === "#btnLookup") {
                    removeLeft = 180;
                    posLeft = "125px";
                    removeTop = 76;
                } else if (sElement === "#txtMatterDesc") {
                    triangleTopPos = 48;
                    removeTop = 75;
                    removeLeft = 100;
                    errorBoxHeight = 62;
                    errorBoxTop = 0;
                } else if (sElement === ".chkConflictCheckParent") {
                    removeTop = 52;
                    triangleTopPos = 31;
                    errorBoxHeight = 40;
                    errorBoxTop = 5;
                    removeLeft = 627;
                } else if (sElement === "#txtMatterId") {
                    removeLeft = 145;
                } else if (sElement === "#txtBlockUser") {
                    removeLeft = 107;
                    triangleTopPos = 50;
                    removeTop = 76;
                    errorBoxHeight = 64;
                    errorBoxTop = 0;
                } else if (sElement === "#ddlRoleAssign1") {
                    removeLeft = 203;
                    triangleTopPos = 48;
                    removeTop = 75;
                } else if (sElement === "#btnCreateMatter") {
                    if (bSecurityGroupError) {
                        errorBoxTop = -8;
                        errorBoxHeight = 71;
                        sMsg = oSecurityGroupError[0];
                    }
                    removeTop = 76;
                    removeLeft = 94;
                    triangleTopPos = 49;
                } else if (sElement === "#txtMatterName") {
                    if (bSecurityGroupError) {
                        sMsg = oSecurityGroupError[0];
                    }
                } else {
                    removeLeft = 100;
                }
            } else {
                iTopPos = iTopPos - 50;
                if (sElement === "#txtMatterId") {
                    removeTop = 76;
                    removeLeft = 145;
                } else if (sElement === "#txtMatterDesc") {
                    triangleTopPos = 48;
                    removeTop = 75;
                    removeLeft = 305;
                    errorBoxHeight = 62;
                    errorBoxTop = 0;
                } else if (sElement === "#btnLookup") {
                    removeLeft = 94;
                } else if (sElement === ".chkConflictCheckParent") {
                    triangleTopPos = 48;
                    removeLeft = 287;
                } else if (sElement === "#txtBlockUser") {
                    triangleTopPos = 47;
                    removeTop = 74;
                    removeLeft = 285;
                } else if (sElement === "#ddlRoleAssign1") {
                    removeLeft = 152;
                    removeTop = 76;
                    triangleTopPos = 48;
                    errorBoxLeft = 33;
                } else if (sElement === "#btnCreateMatter") {
                    if (bSecurityGroupError) {
                        errorBoxTop = -9;
                        errorBoxHeight = 71;
                        posLeft = -3;
                        errorBoxLeft = -31;
                        sMsg = oSecurityGroupError[0];
                    }
                    removeLeft = 90;
                    removeTop = 77;
                    triangleTopPos = 48;
                } else if (sElement === "#txtMatterName") {
                    removeLeft = 305;
                    if (bSecurityGroupError) {
                        sMsg = oSecurityGroupError[0];
                    }
                } else {
                    removeLeft = 305;
                }
            }
            oJQueryObjects.$ErrorPopUp.css("left", iLeftPos - removeLeft).css("top", iTopPos - removeTop).removeClass("hide").find(".errText").text(sMsg);
            $(".mainArea .errTriangleBorder").css("left", posLeft);
            $(".mainArea .errTriangle").css("left", posLeft);
            $(".mainArea .errTriangle").css("top", "50%").css("top", "+=" + (triangleTopPos) + "px");
            $(".mainArea .errTriangleBorder").css("top", "50%").css("top", "+=" + (triangleTopPos - 1) + "px");
            $(".mainArea .errText").css("min-height", errorBoxHeight + "px").css("top", errorBoxTop + "px").css("left", errorBoxLeft + "px");
            oRoleObject.css("border-width", "2px").css("border-color", "red");
            oJQueryObjects.$Loading.addClass("hide");
            $("#btnCreateMatter").removeAttr("disabled");
            $(".ms-ChoiceField-field").removeClass("disable-checkbox");
        }
    }

    /* Function to sort drop downs */
    function sortDropDownListByText() {
        // Loop for each select element on the page.
        $("select").each(function () {  //// Here we sort all the dropdowns present on DOM thus general selector is used

            // Keep track of the selected option.
            var selectedValue = $(this).val();

            // Sort all the options by text. I could easily sort these by val.
            $(this).html($("option", $(this)).sort(function (a, b) {
                return a.text === b.text ? 0 : a.text < b.text ? -1 : 1;
            }));

            // Select one option.
            $(this).val(selectedValue);
        });
    }

    /* Function to populate pop-up data */
    function populateContentTypes() {
        var iCount = 0, iLength = serviceConstants.oTermStoreJson.PGTerms.length, sFolderNames, sPracticeGroupId;
        localStorage.removeItem("iSelectedPG");
        localStorage.removeItem("iSelectedAOL");

        for (iCount = 0; iCount < iLength; iCount++) {
            sFolderNames = serviceConstants.oTermStoreJson.PGTerms[iCount].FolderNames ? serviceConstants.oTermStoreJson.PGTerms[iCount].FolderNames : "";
            sPracticeGroupId = serviceConstants.oTermStoreJson.PGTerms[iCount].Id ? serviceConstants.oTermStoreJson.PGTerms[iCount].Id : "";
            oJQueryObjects.$PracticeGroupList.append("<div class='popUpOptions' title='" + serviceConstants.oTermStoreJson.PGTerms[iCount].TermName + "' data-value='" + iCount + "' data-folderNamesPG='" + sFolderNames + "' data-practicegroup-id='" + sPracticeGroupId + "' >" + serviceConstants.oTermStoreJson.PGTerms[iCount].TermName + "</div>");
        }
        $(".popUpOptions").click(function (event) {
            var selectedPG = this.innerHTML, $Element = $("#popUpPG");
            $Element.val(selectedPG).attr("data-value", $(this).attr("data-value")).attr("data-foldernamespg", $(this).attr("data-foldernamespg")).attr("data-practicegroup-id", $(this).attr("data-practicegroup-id"));
            $("#popUpPGList").addClass("hide");
        });

        $(".popUpOptions").mouseover(function () {
            $(this).addClass("ddListOnHover");
        }).mouseout(function () {
            $(this).removeClass("ddListOnHover");
        });

        $(".popUpOptions").click(function () { $(".popUpMDTextArea").val(""); });
        $(".popUpOptions").click(function () {
            if ($.isEmptyObject(serviceVariables.oTermStoreData)) {
                serviceVariables.oTermStoreData = oMatterProvisionObject.getTermStoreData();
            }
            var oAOL = $(".popUpMD"), sFolderNames, sAreaOfLawId;
            $(".popUpMDContent, .popUpSALContent").remove();
            localStorage.iSelectedPG = $(".popUpPG").attr("data-value");
            for (var jIterator = 0; jIterator < serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms.length; jIterator++) {
                // Addition of folder names
                sFolderNames = serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[jIterator].FolderNames ? serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[jIterator].FolderNames : "";
                sAreaOfLawId = serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[jIterator].Id ? serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[jIterator].Id : "";
                oAOL.append("<div class='popUpMDContent' data-areaoflaw-id='" + sAreaOfLawId + "' data-value=" + jIterator + " data-folderNamesAOL=" + sFolderNames + " >" + serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[jIterator].TermName + "</div>");
            }

            $(".popUpMD .popUpMDContent:first-child").click();
        });

        $($(".popUpOptions")[0]).click();

        oJQueryObjects.$LoadingLookUp.addClass("hide");
        oJQueryObjects.$LookUpButton.removeAttr("disabled");
        sortDropDownListByText();
    }

    /* Functions to change the tabs on menu item click */
    function navigateTab(event) {
        oJQueryObjects.$ErrorPopUp.addClass("hide");
        oJQueryObjects.$MenuClick.removeClass("menuTextSelected");
        $(event.target).addClass("menuTextSelected");
        $(".sectionStep").hide();
        serviceConstants.iCurrentPage = parseInt($(event.target).attr("data-section-num"), 10);
        $(".header .menuTextGroupHeader").html(oMatterProvisionConstants.Header_Display_Step.replace("{0}", serviceConstants.iCurrentPage).replace("{1}", $(".menu .menuTextGroup .menuText").length));
        localStorage.iLivePage = serviceConstants.iCurrentPage;
        $("#" + $(event.target).attr("data-section-name")).show();
        oJQueryObjects.$Loading.addClass("hide");
        if (2 <= serviceConstants.iCurrentPage) {
            if (localStorage.getItem("oPageTwoData")) {
                var oSavedPageTwoData = JSON.parse(localStorage.getItem("oPageTwoData"));
                var isPageLoad = localStorage.getItem("IsPageLoad") && $.parseJSON(localStorage.getItem("IsPageLoad"));
                $("#conflictCheck, #assignTeam").removeClass("hide");

                if (!serviceConstants.oDefaultConfiguration.IsConflictCheck) {
                    $("#conflictCheck, #assignTeam").addClass("hide");
                    $("input[name=rdbConflictCheck][value=False]").click();
                    serviceConstants.sConflictScenario = "False";
                } else {
                    if (isPageLoad) {
                        $("input[name=rdbConflictCheck][value=True]").click();
                        $("#snBlockUser").show();
                        serviceConstants.sConflictScenario = "True";
                    }
                    if ("true" === oSavedPageTwoData.ConflictIdent.toLowerCase()) {
                        $("input[name=rdbConflictCheck][value=True]").click();
                        $("#snBlockUser").show();
                        serviceConstants.sConflictScenario = "True";
                    } else {
                        $("input[name=rdbConflictCheck][value=False]").click();
                        $("#snBlockUser").hide();
                        serviceConstants.sConflictScenario = "False";
                    }
                }
                if (isPageLoad) {
                    $("input[name=rdbSecureMatterCheck][value=True]").click();
                } else if ("False" === oSavedPageTwoData.BoolSecureMatter && $("input[name=rdbSecureMatterCheck][value=False]")) {
                    $("input[name=rdbSecureMatterCheck][value=False]").click();
                } else {
                    $("input[name=rdbSecureMatterCheck][value=True]").click();
                }
                var oSendMailObject = $("#demo-checkbox-unselected2");
                if (oSendMailObject.length) {
                    var bSendMailCheck = localStorage.getItem("oSendMailCheckbox") && $.parseJSON(localStorage.getItem("oSendMailCheckbox"));
                    oSendMailObject[0].checked = bSendMailCheck;
                    if (bSendMailCheck) {
                        $("#btnCreateMatter").text(oMatterProvisionConstants.Create_Notify_Button_Text);
                    } else {
                        $("#btnCreateMatter").text(oMatterProvisionConstants.Create_Button_Text);
                    }
                }

                var oCalenderObject = $("#demo-checkbox-unselected0");
                if (oCalenderObject.length) {
                    oCalenderObject[0].checked = localStorage.getItem("oCalenderCheckbox") && $.parseJSON(localStorage.getItem("oCalenderCheckbox"));
                }

                var oRSSFeedObject = $("#demo-checkbox-unselected1");
                if (oRSSFeedObject.length) {
                    oRSSFeedObject[0].checked = localStorage.getItem("oRSSFeedCheckbox") && $.parseJSON(localStorage.getItem("oRSSFeedCheckbox"));
                }

                var oTaskObject = $("#demo-checkbox-unselected3");
                if (oTaskObject.length && "undefined" !== localStorage.getItem("oTaskCheckbox")) {
                    oTaskObject[0].checked = localStorage.getItem("oTaskCheckbox") && $.parseJSON(localStorage.getItem("oTaskCheckbox"));
                }
            }
        }
        localStorage.setItem("IsPageLoad", false);
    }


    function onMenuTextClick(e) {
        if (serviceConstants.isValidClient) {
            serviceConstants.iPageClicked = parseInt($(e.target).attr("data-section-num"), 10);
            if (2 <= serviceConstants.iPageClicked) {
                serviceConstants.isNextClicked = true;
            } else {
                serviceConstants.isNextClicked = false;
            }
            sortDropDownListByText();
            /* Perform validation */
            if (null !== serviceConstants.bValidMatterName) {
                serviceConstants.isNextClicked = false;
                var bIsValid = false;
                if (serviceConstants.iCurrentPage >= parseInt($(e.target).attr("data-section-num"), 10)) {
                    bIsValid = true;
                } else {
                    bIsValid = commonFunctions.validateCurrentPage(serviceConstants.iCurrentPage);
                }
                if (serviceConstants.bValidMatterName) {
                    if (bIsValid) {
                        /* Maintain state of App */
                        commonFunctions.maintainAppState(serviceConstants.iCurrentPage);
                        if (2 === serviceConstants.iCurrentPage && 2 < parseInt($(e.target).attr("data-section-num"), 10)) {
                            oJQueryObjects.$ConflictIdentified = $("input[name=rdbConflictCheck]:checked");
                            var sConflictIdentified = (oJQueryObjects.$ConflictIdentified.length) ? oJQueryObjects.$ConflictIdentified.val() : "";
                            commonFunctions.checkSecurityGroupExists(e);
                        } else {
                            navigateTab(e);
                        }
                    }
                } else {
                    showErrorNotification("#txtMatterName", oMatterProvisionConstants.Error_Duplicate_Matter);
                }
            }
        }
    }

    /* Function to show/hide block user on conflict identified option click */
    function onConflictIdentifiedChange(e) {
        var sValue = serviceConstants.sConflictScenario = $(e.target).val(), $BlockUser = $("#snBlockUser");
        if ("True" === sValue) {
            $BlockUser.show();
            $(".matterCheckYes").prop("checked", true);
            $("input[name=rdbSecureMatterCheck]").attr("disabled", true);
        } else {
            $BlockUser.hide();
            oJQueryObjects.$BlockedUsers.val("").removeAttr("data-resolved");
            $("input[name=rdbSecureMatterCheck]").removeAttr("disabled");
        }
        var oSavedPageTwoData = JSON.parse(localStorage.getItem("oPageTwoData"));
        oSavedPageTwoData.ConflictIdent = sValue;
        localStorage.setItem("oPageTwoData", JSON.stringify(oSavedPageTwoData));
    }

    /* Function to display Look up pop up */
    function showPopup() {
        $(".popupContainerBackground, .popUpContainer").removeClass("hide");
        oJQueryObjects.$ContentTypes = $("#documentTemplates > .docTemplateItem");
        oJQueryObjects.$PracticeGroup[0] ? oJQueryObjects.$PracticeGroup[0].selectedIndex = 0 : null;
        $(".popUpPG")[0].selectedIndex = 0;
        $(".popUpPG").change();

        /* Display selected items in pop-up */
        var oPopUpDT = $(".popDT");
        if (oJQueryObjects.$ContentTypes[0]) {
            oPopUpDT.append(oJQueryObjects.$ContentTypes.clone().removeClass("docTemplateItem").addClass("popUpDTContent"));
        }
        if ($(".popUpDTContent").length && !($(".popUpDTContent").hasClass("popUpSelected"))) {
            setDefaultContentTypeSelectionMessage();
        } else {
            removeDefaultContentTypeSelectionMessage();
        }

    }

    /* Function for search by typing on LookUp Pop up */
    function searchByTyping(sSelector, textValue) {
        var oDivContent = $(sSelector);
        if (oDivContent[0]) {
            oDivContent.show();
            if (textValue) {
                $.each(oDivContent, function () {
                    if (-1 === $(this).text().toLowerCase().indexOf(textValue.toLowerCase())) {
                        $(this).hide();
                    }
                });
            }
            oDivContent[0].click();
        }
    }

    /* Function to check valid matter name */
    function checkValidMatterName() {
        var bInValid = false
                , RegularExpression = new RegExp(oMatterProvisionConstants.Special_Character_Expression_Matter_Title)
                , sCurrMatterName = oJQueryObjects.$MatterName.val().trim();
        if (null !== sCurrMatterName && "" !== sCurrMatterName) {
            var arrValidMatch = sCurrMatterName.match(RegularExpression);
            if (null === arrValidMatch || arrValidMatch[0] !== sCurrMatterName) {
                bInValid = false;
            } else {
                bInValid = true;
            }
        }
        if (bInValid) {
            if ("1" === $(".menuText.menuTextSelected").attr("data-section-num") && serviceConstants.isValidClient) { // Check if only on Step 1
                serviceConstants.bValidMatterName = null;
                var isInvalidToken = oCommonObject.getParameterByName(oGlobalConstants.TokenRequestFailedQueryString)
                    , sMatterName = oJQueryObjects.$MatterName.val().trim()
                    , sClientUrl = oJQueryObjects.$ClientName.attr("data-value")
                    , matterDetails = {
                        "requestObject": {
                            "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
                        }, "client": {
                            "Url": sClientUrl
                        }, "matter": {
                            "Name": sMatterName,
                            "MatterGuid": localStorage.getItem("matterGuid") ? localStorage.getItem("matterGuid") : serviceConstants.matterGUID
                        }, "hasErrorOccurred": (isInvalidToken) ? true : false
                    };
                if (sMatterName && sClientUrl) {
                    oJQueryObjects.$Loading.removeClass("hide");
                    oCommonObject.callProvisioningService("CheckMatterExists", matterDetails, commonFunctions.matterNameExistsSuccess, commonFunctions.matterNameExistsFailure);
                } else {
                    serviceConstants.bValidMatterName = true;
                }
            }
        } else {
            serviceConstants.bValidMatterName = true; // This is specifically added to fix issue with Matter name with special characters error message
        }
    }

    /* Function to check if user has entered valid list of blocked users */
    function isValidBlockUser() {
        var arrBlockUserList = oCommonObject.getUserName(oJQueryObjects.$BlockedUsers.val().trim(), false);
        for (var iIterator = 0; iIterator < arrBlockUserList.length - 1; iIterator++) {
            if (-1 === $.inArray(arrBlockUserList[iIterator].trim(), oCommonObject.oSiteUser)) {
                return false;
            }
        }
        return true;
    }

    /* Function to retrieve the list of folders for the specific matter */
    function retrieveFolderStructure() {
        "use strict";
        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrFolderStructure = [];
        if (oPageOneData && oPageOneData.ContentTypes) {
            arrContentTypes = oPageOneData.ContentTypes;
            nLength = arrContentTypes.length;
            for (nCount = 0; nCount < nLength; nCount++) {
                if (arrContentTypes[nCount].DataDocumentTemplate === localStorage.setDefaultContentType || 0 === nCount) {
                    // Check if the isNoFolderStructurePresent flag is set to true
                    if (arrContentTypes[nCount].DataisNoFolderStructurePresent && "false" === arrContentTypes[nCount].DataisNoFolderStructurePresent.toLowerCase()) {
                        // If the folder at the specific level is not present then move to the parent level
                        arrFolderStructure = arrContentTypes[nCount].DataFolderNamesSAL && "" !== arrContentTypes[nCount].DataFolderNamesSAL ? arrContentTypes[nCount].DataFolderNamesSAL.split(";") : arrContentTypes[nCount].DataFolderNamesAOL && "" !== arrContentTypes[nCount].DataFolderNamesAOL ? arrContentTypes[nCount].DataFolderNamesAOL.split(";") : arrContentTypes[nCount].DataFolderNamesPG && "" !== arrContentTypes[nCount].DataFolderNamesPG ? arrContentTypes[nCount].DataFolderNamesPG.split(";") : [];
                    }
                }

            }
        }
        return arrFolderStructure;
    }

    /* Function to handle any error scenario while provisioning a matter */
    commonFunctions.handleErrorScenarios = function (result, sValidationmessage) {
        $(".ms-ChoiceField-field").removeClass("disable-checkbox");
        serviceConstants.bErrorOccured = true;
        $(".notificationContainer .notification .closeNotification").click();
        oJQueryObjects.$Loading.addClass("hide");
        oJQueryObjects.$LookUpButton.removeAttr("disabled");
        oJQueryObjects.$CreateButton.removeAttr("disabled");
        if (result.code && "string" === typeof result.code && (-1 < result.code.indexOf("#|#"))) {
            var arrErrorPopUpData = result.code.split("#|#")
                , iErrorCode = (arrErrorPopUpData[0] && arrErrorPopUpData[0].trim()) ? arrErrorPopUpData[0] : "NA";
            if (oGlobalConstants.TokenRequestFailedErrorCode === iErrorCode) {
                reauthenticateUser();
            } else {
                showNotification(".notificationContainer", oMatterProvisionConstants.Error_Provisioning_Process.replace("{0}", iErrorCode), "failureBanner");
            }
        } else {
            showErrorNotification("#btnCreateMatter", sValidationmessage + result.value);
        }
    };

    // #region Service Calls

    /* Functions for checking if matter name exists */
    commonFunctions.matterNameExistsSuccess = function (result) {
        serviceConstants.isValidClient = true;
        if (result) {
            var arrResults = JSON.parse(result);
            if (arrResults && parseInt(arrResults.code, 10)) {
                if ("string" === typeof arrResults.code && (-1 < arrResults.code.indexOf("#|#"))) {
                    showCommonErrorPopUp(arrResults.code);
                } else {
                    showErrorNotification("#txtMatterName", arrResults.value);
                }
                serviceConstants.bValidMatterName = false;
            } else {
                serviceConstants.bValidMatterName = true;
                if (serviceConstants.isNextClicked) {
                    oJQueryObjects.$MenuClick[serviceConstants.iPageClicked - 1].click();
                }
            }
        }
        oJQueryObjects.$Loading.addClass("hide");
    };
    commonFunctions.matterNameExistsFailure = function (result) {
        return false;
    };

    /* Functions for creating matter */
    commonFunctions.createMatterSuccess = function (result) {
        result = JSON.parse(result);
        //// Remove the data from Local Storage as the Matter Library is created in SharePoint
        $(".notificationContainer .notification .closeNotification").click();
        if (!result.code) {
            showNotification(".notificationContainer", oMatterProvisionConstants.Success_Matter_Step_2, "successBanner");
            commonFunctions.createdMatterUrl = result.value;
            commonFunctions.associateContentTypes();
            commonFunctions.assignPermission();
            commonFunctions.createMatterLandingPage();
        } else {
            !(serviceConstants.bErrorOccured) && commonFunctions.handleErrorScenarios(result, oMatterProvisionConstants.Error_Create_Matter);
        }
    };
    commonFunctions.createMatterFailure = function (result) {
        return false;
    };
    commonFunctions.createMatter = function () {
        //// Do the offline check here
        var sClientId = oJQueryObjects.$ClientId.val()
        , sClientUrl = oJQueryObjects.$ClientName.attr("data-value")
        , sClientName = (oJQueryObjects.$ClientName.length) ? $(oJQueryObjects.$ClientName).val() : ""
        , matterId = oJQueryObjects.$MatterId.val()
        , matterName = oJQueryObjects.$MatterName.val().trim()
        , matterDesc = oJQueryObjects.$MatterDescription.val().trim()
        , conflictCheckBy = oCommonObject.getUserName(oJQueryObjects.$ConflictConductedBy.val().trim(), false)[0]
        , conflictCheckOn = oJQueryObjects.$ConflictCheckDate.val()
        , ConflictIdentified = oJQueryObjects.$ConflictIdentified.val()
        , blockUserNames = ""
        , boolSecureMatter = oJQueryObjects.$BoolSecureMatter
        , arrUserNames = []
        , arrUserEmails = []
        , arrPermissions = []
        , arrRoles = []
        , arrFolderNames = []
        , matterDetails = {}
        , oDocumentTemplates = $(".docTemplateItem")
        , userId = [];
        serviceConstants.isErrorOccurred = false;
        serviceConstants.iShowSuccessMessage = 0;
        serviceConstants.iCallsComplete = 0;
        ////Store the PGList, AOLList and SAOLList
        serviceConstants.sPracticeGroupList = "";
        serviceConstants.sAreaOfLawList = "";
        serviceConstants.sSubAreaOfLawList = "";
        $.each(oDocumentTemplates, function () {
            if (-1 === serviceConstants.sPracticeGroupList.indexOf($(this).attr("data-practicegroup"))) {
                serviceConstants.sPracticeGroupList = $(this).attr("data-practicegroup") + "; " + serviceConstants.sPracticeGroupList;
            }
            if (-1 === serviceConstants.sAreaOfLawList.indexOf($(this).attr("data-areaoflaw"))) {
                serviceConstants.sAreaOfLawList = $(this).attr("data-areaoflaw") + "; " + serviceConstants.sAreaOfLawList;
            }
            if (-1 === serviceConstants.sSubAreaOfLawList.indexOf($(this).attr("data-document-template"))) {
                serviceConstants.sSubAreaOfLawList = $(this).attr("data-document-template") + "; " + serviceConstants.sSubAreaOfLawList;
            }
        });

        if ("True" === ConflictIdentified) {
            blockUserNames = oCommonObject.getUserName(oJQueryObjects.$BlockedUsers.val().trim(), false);
        }
        $.each($("input[id^=txtAssign]"), function () {
            arrUserNames.push(oCommonObject.getUserName($(this).val().trim(), true));
            arrUserEmails.push(oCommonObject.getUserName($(this).val().trim(), false));
            userId.push(this.id);
        });
        $.each($("input[id^=ddlRoleAssign]"), function () {
            arrRoles.push($(this).val());
        });
        $.each($("input[id^=ddlPermAssign]"), function (iPosition) {
            arrPermissions.push($.trim($(this).val()));
            if ($.trim($(this).val()).toLowerCase() === oGlobalConstants.User_Upload_Permissions.toLowerCase()) {
                var iCurrentCount = $(this).attr("id") && $(this).attr("id").split("ddlPermAssign");
                if (1 < iCurrentCount.length) {
                    serviceConstants.arrReadOnlyUsers.push(trimEndChar(oCommonObject.getUserName($("input[id^=txtAssign" + iCurrentCount[1] + "]").val().trim(), false).join(";"), ";"));
                }
            }
        });
        var isCalenderSelected = $("#demo-checkbox-unselected0")[0].checked;
        var isTaskSelected = $("#demo-checkbox-unselected3")[0].checked;

        matterDetails = {
            "requestObject": {
                "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
            }, "client": {
                "Url": sClientUrl, "Id": sClientId, "Name": sClientName
            }, "matter": {
                "Name": matterName, "Id": matterId, "Description": matterDesc, "Conflict": { "Identified": ConflictIdentified, "CheckBy": conflictCheckBy, "CheckOn": conflictCheckOn, "SecureMatter": boolSecureMatter }, "BlockUserNames": blockUserNames, "AssignUserNames": arrUserNames, "AssignUserEmails": arrUserEmails, "Roles": arrRoles, "MatterGuid": serviceConstants.matterGUID
            }, "matterConfigurations": {
                "IsConflictCheck": serviceConstants.oDefaultConfiguration.IsConflictCheck, "IsMatterDescriptionMandatory": serviceConstants.oDefaultConfiguration.IsMatterDescriptionMandatory, "IsCalendarSelected": isCalenderSelected, "IsTaskSelected": isTaskSelected
            },
            "userId": userId, "hasErrorOccurred": false
        };

        /* Validate both pages */
        var bValidPage = commonFunctions.validateCurrentPage(2);
        if (bValidPage) {
            bValidPage = commonFunctions.validateCurrentPage(1);
            if (bValidPage) {
                $(".notificationContainer .notification .closeNotification").click();
                showNotification(".notificationContainer", oMatterProvisionConstants.Success_Matter_Step_1, "successBanner");
                // Addition of the Folder Structure
                arrFolderNames = retrieveFolderStructure();
                matterDetails.matter.FolderNames = arrFolderNames;
                localStorage.setItem("matterGuid", serviceConstants.matterGUID); //// Store the matter GUID in case an error while provisioning, will be used to delete matter landing page
                oCommonObject.callProvisioningService("CreateMatter", matterDetails, commonFunctions.createMatterSuccess, commonFunctions.createMatterFailure);
            } else {
                $(".menuText")[0].click();
                $("#btnCreateMatter, #btnLookup").removeAttr("disabled");
            }
        } else {
            $(".menuText")[1].click();
            $("#btnCreateMatter, #btnLookup").removeAttr("disabled");
        }
        /* Validate both pages */
    };

    /* Functions for retrieving clients */
    commonFunctions.getClientSuccess = function (result) {
        oJQueryObjects.$ClientList.html("<div class='ddlClientListItem' title='- Select a client -' data-value='' data-client-id=''>- Select a client -</div>");
        var arrResults = JSON.parse(result), clientItem, clientOption, iCount = 0, iLength, arrOptions = [];
        if (!arrResults.code) {
            serviceConstants.arrClients = JSON.parse(JSON.stringify(arrResults));
            iLength = serviceConstants.arrClients.ClientTerms.length;
            for (iCount = 0; iCount < iLength; iCount++) {
                clientOption = "<div class='ddlClientListItem' title='" + serviceConstants.arrClients.ClientTerms[iCount].Name + "' data-value='" + serviceConstants.arrClients.ClientTerms[iCount].Url + "' data-client-id='" + serviceConstants.arrClients.ClientTerms[iCount].Id + "'>" + serviceConstants.arrClients.ClientTerms[iCount].Name + "</div>";
                arrOptions.push(clientOption);
            }
            oJQueryObjects.$ClientList.append(arrOptions.join(""));
            $($(oJQueryObjects.$ClientList[0]).find(".ddlClientListItem")).attr("data-client-id");

            $(".ddlClientListItem").click(function (event) {
                var selectedClient = this.innerHTML, $Element = $("#ddlClient");
                $Element.val(selectedClient).attr("data-value", $(this).attr("data-value")).attr("data-client-id", $(this).attr("data-client-id"));
                $("#ddlClientList").addClass("hide");
                $(".ddlClientListItem").removeClass("ddListOnHover");
                commonFunctions.getDefaultValues(this);
            });

            $(".ddlClientListItem").mouseover(function () {
                $(".ddlClientListItem").removeClass("ddListOnHover");
                $(this).addClass("ddListOnHover");
            }).mouseout(function () {
                $(this).removeClass("ddListOnHover");
            });

            $(".ddlClientListItem").click(commonFunctions.onClientChange);
            $("#ddlClient").val($($(".ddlClientListItem")[0]).text());
        } else {
            if (arrResults.code && "string" === typeof arrResults.code && (-1 < arrResults.code.indexOf("#|#"))) {
                showCommonErrorPopUp(arrResults.code);
            } else {
                showErrorNotification("#txtMatterId", arrResults.value);
            }
        }
        serviceConstants.bClient = true;
        oJQueryObjects.$Loading.addClass("hide");
        oJQueryObjects.$ClientList.removeAttr("disabled");
    };
    commonFunctions.getClientFailure = function (result) {
        return false;
    };
    commonFunctions.getClient = function () {
        var sClientUrl = serviceConstants.sCentralRepositoryLocation
        , matterDetails = {
            "requestObject": {
                "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
            }, "client": {
                "Url": sClientUrl
            }, "details": {
                "TermGroup": oGlobalConstants.Common_Term_Set_Group_Name, "TermSetName": oGlobalConstants.Client_Term_Set_Name, "CustomPropertyName": oGlobalConstants.Client_Custom_Properties_Url
            }
        };
        oJQueryObjects.$ClientName.attr("disabled", "disabled");
        oCommonObject.callProvisioningService("GetTaxonomyData", matterDetails, commonFunctions.getClientSuccess, commonFunctions.getClientFailure);
    };

    commonFunctions.getDefaultValuesSuccess = function (result) {
        serviceConstants.oDefaultConfiguration = [];
        $(".popupWait, .loadingImage").addClass("hide");
        var arrResults = JSON.parse(result.split("|$|")[0]);    // result[1] contain item modified date which is used to handle concurrency issue on settings page
        if (!arrResults.code) {
            $.each(arrResults, function (key, value) {
                serviceConstants.oDefaultConfiguration[key] = value;
            });
            $("#txtMatterName")[0].value = serviceConstants.oDefaultConfiguration.DefaultMatterName ? serviceConstants.oDefaultConfiguration.DefaultMatterName : "";
            checkValidMatterName();
            $("#txtMatterId")[0].value = serviceConstants.oDefaultConfiguration.DefaultMatterId ? serviceConstants.oDefaultConfiguration.DefaultMatterId : "";
            if (serviceConstants.oDefaultConfiguration.IsMatterDescriptionMandatory) {
                $(".description").show();
            } else {
                $(".description").hide();
            }
            $("#txtMatterDesc").empty().focus().blur();
            $(".assignNewPerm").remove();
            if (serviceConstants.oDefaultConfiguration.MatterUsers) {
                var arrUsers = serviceConstants.oDefaultConfiguration.MatterUsers.split("$|$")
                , arrRoles = serviceConstants.oDefaultConfiguration.MatterRoles.split("$|$")
                , arrUserEmails = []
                , arrPermissions = serviceConstants.oDefaultConfiguration.MatterPermissions.split("$|$")
                , count = 1
                , iCounter
                , iCount
                , sEmail = "";
                if (serviceConstants.oDefaultConfiguration.MatterUserEmails && "" !== serviceConstants.oDefaultConfiguration.MatterUserEmails) {
                    arrUserEmails = serviceConstants.oDefaultConfiguration.MatterUserEmails.split("$|$");
                }


                count = 1;
                for (iCounter = 0; iCounter < arrUsers.length; iCounter++) {
                    commonFunctions.addMorePermissions(count);
                    var userName = "#txtAssign" + count, permissions = "#ddlPermAssign" + count;
                    //// Get value from default configuration and set it in text box. i.e. UserName(user email address)
                    if (arrUsers && 0 < arrUsers.length && arrUsers[iCounter]) {
                        var arrAllUsers = arrUsers[iCounter].split(";");
                        if (arrUserEmails && 0 < arrUserEmails.length && arrUserEmails[iCounter]) {
                            var arrAllUserEmails = arrUserEmails[iCounter].split(";");
                            for (iCount = 0; iCount < arrAllUsers.length; iCount++) {
                                if (arrAllUsers[iCount] && "" !== arrAllUsers[iCount].trim()) {
                                    sEmail = arrAllUserEmails[iCount];
                                    if (serviceConstants.oEmailRegex.test(sEmail)) {
                                        $(userName)[0].value += (trimEndChar(arrAllUsers[iCount], ";") + " (" + arrAllUserEmails[iCount] + ");");
                                    } else {
                                        $(userName)[0].value += (trimEndChar(arrAllUsers[iCount], ";") + ";");
                                    }
                                }
                            }

                        } else {
                            for (iCount = 0; iCount < arrAllUsers.length; iCount++) {
                                if (arrAllUsers[iCount] && "" !== arrAllUsers[iCount].trim()) {
                                    $(userName).val(trimEndChar(arrAllUsers[iCount], ";") + ";");
                                }
                            }
                        }
                    } else {
                        $(userName).val("");
                    }
                    $(permissions).val(arrPermissions[iCounter]).attr("data-value", arrPermissions[iCounter]);
                    oCommonObject.bindAutocomplete(userName, true);
                    count++;
                }
                if (serviceConstants.oDefaultConfiguration.MatterRoles) {
                    count = 1;
                    $.each(arrPermissions, function (key, value) {
                        var permissions = "#ddlPermAssign" + count;
                        count++;
                        $(permissions).val(value).attr("data-value", value);
                    });
                }
                if (serviceConstants.oDefaultConfiguration.MatterPermissions) {
                    count = 1;
                    $.each(arrRoles, function (key, value) {
                        var roles = "#ddlRoleAssign" + count;
                        count++;
                        if (-1 === $.inArray(value, serviceConstants.oMandatoryRoleNames)) {
                            $(roles).val($.trim(value)).attr("data-value", $.trim(value)).attr("data-mandatory", "false");
                        } else {
                            $(roles).val($.trim(value)).attr("data-value", $.trim(value)).attr("data-mandatory", "true");
                        }
                    });
                }
            } else {
                commonFunctions.addMorePermissions(1);
            }
            var arrMatterType = serviceConstants.oDefaultConfiguration.MatterTypes ? serviceConstants.oDefaultConfiguration.MatterTypes.split("$|$") : ""
            , arrAreaofLaw = serviceConstants.oDefaultConfiguration.MatterAreaofLaw ? serviceConstants.oDefaultConfiguration.MatterAreaofLaw.split("$|$") : ""
            , arrPracticeGroup = serviceConstants.oDefaultConfiguration.MatterPracticeGroup ? serviceConstants.oDefaultConfiguration.MatterPracticeGroup.split("$|$") : ""
            , count = 0
            , iCounter = 0;
            $("#documentTemplates").empty();
            localStorage.setDefaultContentType = "";
            $.each(arrMatterType, function (key, matterType) {
                $.each(serviceConstants.oTermStoreJson.PGTerms, function (key, pgTerms) {
                    $.each(pgTerms.AreaTerms, function (key, areaTerms) {
                        $.each(areaTerms.SubareaTerms, function (key, subAreaTerms) {
                            if ($.trim(subAreaTerms.TermName) === $.trim(matterType) && $.trim(areaTerms.TermName) === $.trim(arrAreaofLaw[iCounter]) && $.trim(pgTerms.TermName) === $.trim(arrPracticeGroup[iCounter])) {
                                var oContentTypeContainer = $("#documentTemplates");
                                var sDataDocumentTemplate = subAreaTerms.DocumentTemplates;
                                var sDataAssociatedDocumentTemplate = subAreaTerms.DocumentTemplateNames;
                                var sSelectedPracticeGroup = pgTerms.TermName;
                                var sSelectedPracticeGroupID = pgTerms.Id;
                                var sSelectedPracticeGroupFolderStructure = pgTerms.FolderNames;
                                var sSelectedAreaOfLaw = areaTerms.TermName;
                                var sSelectedAreaOfLawID = areaTerms.Id;
                                var sSelectedAreaOfLawFolderStructure = areaTerms.FolderNames;
                                var sSelectedSubAreaOfLawID = subAreaTerms.Id;
                                var sSelectedSubAreaOfLawFolderStructure = subAreaTerms.FolderNames;
                                var sSelectedSubAreaOfLawIsNofolderStructurePresent = subAreaTerms.IsNoFolderStructurePresent;
                                var iFoldersCount = 0;
                                if (sDataDocumentTemplate) {
                                    iFoldersCount = sDataAssociatedDocumentTemplate.split(";").length;
                                }
                                if ($.trim(subAreaTerms.TermName) === $.trim(serviceConstants.oDefaultConfiguration.DefaultMatterType)) {
                                    oContentTypeContainer.append("<div class='docTemplateItem popUpSelected' data-value= 0 data-document-template='" + sDataDocumentTemplate + "' data-associated-document-template='" + sDataAssociatedDocumentTemplate + "' data-practicegroup='" + sSelectedPracticeGroup + "' data-areaoflaw='" + sSelectedAreaOfLaw + "' data-practicegroup-id='" + sSelectedPracticeGroupID + "' data-areaoflaw-id='" + sSelectedAreaOfLawID + "' data-subareaoflaw-id='" + sSelectedSubAreaOfLawID + "' data-foldernamespg='" + sSelectedPracticeGroupFolderStructure + "' data-foldernamesaol='" + sSelectedSubAreaOfLawFolderStructure + "' data-foldernamessal='" + sSelectedSubAreaOfLawFolderStructure + "' data-isnofolderstructurepresent='" + sSelectedSubAreaOfLawIsNofolderStructurePresent + "' data-display-name='" + subAreaTerms.TermName + "' >" + $.trim(matterType) + " (" + iFoldersCount + ")</div>");
                                    localStorage.setDefaultContentType = sDataDocumentTemplate;
                                } else {
                                    oContentTypeContainer.append("<div class='docTemplateItem' data-value='" + count + "' data-document-template='" + sDataDocumentTemplate + "' data-associated-document-template='" + sDataAssociatedDocumentTemplate + "' data-practicegroup='" + sSelectedPracticeGroup + "' data-areaoflaw='" + sSelectedAreaOfLaw + "' data-practicegroup-id='" + sSelectedPracticeGroupID + "' data-areaoflaw-id='" + sSelectedAreaOfLawID + "' data-subareaoflaw-id='" + sSelectedSubAreaOfLawID + "' data-foldernamespg='" + sSelectedPracticeGroupFolderStructure + "' data-foldernamesaol='" + sSelectedSubAreaOfLawFolderStructure + "' data-foldernamessal='" + sSelectedSubAreaOfLawFolderStructure + "' data-isnofolderstructurepresent='" + sSelectedSubAreaOfLawIsNofolderStructurePresent + "' data-display-name='" + subAreaTerms.TermName + "' >" + $.trim(matterType) + " (" + iFoldersCount + ")</div>");
                                    count++;
                                }
                            }
                        });
                    });
                });
                iCounter++;
            });
            var data;
            if (serviceConstants.oDefaultConfiguration.IsRestrictedAccessSelected) {
                $("input[name=rdbSecureMatterCheck][value=True]").prop("checked", "checked");
            } else {
                $("input[name=rdbSecureMatterCheck][value=False]").prop("checked", "checked");
            }
            if (!serviceConstants.oDefaultConfiguration.IsConflictCheck) {
                $("#conflictCheck, #assignTeam").addClass("hide");
                $("input[name=rdbConflictCheck][value=False]").prop("checked", "checked");
                serviceConstants.sConflictScenario = "False";
            } else {
                $("#conflictCheck, #assignTeam").removeClass("hide");
                $("input[name=rdbConflictCheck][value=True]").prop("checked", "checked");
                $("#snBlockUser").show();
                serviceConstants.sConflictScenario = "True";
            }
            if (serviceConstants.oDefaultConfiguration.IsCalendarSelected) {
                $("#demo-checkbox-unselected0")[0].checked = true;
            }
            if (serviceConstants.oDefaultConfiguration.IsRSSSelected) {
                $("#demo-checkbox-unselected1")[0].checked = true;
            }
            if (serviceConstants.oDefaultConfiguration.IsEmailOptionSelected) {
                $("#demo-checkbox-unselected2")[0].checked = true;
                $("#btnCreateMatter").html(oMatterProvisionConstants.Create_Notify_Button_Text);
            }
            if (serviceConstants.oDefaultConfiguration.IsTaskSelected) {
                $("#demo-checkbox-unselected3")[0].checked = true;
            }
            localStorage.setItem("IsConflictCheck", serviceConstants.oDefaultConfiguration.IsConflictCheck);
            localStorage.setItem("oCalenderCheckbox", serviceConstants.oDefaultConfiguration.IsCalendarSelected);
            localStorage.setItem("oRSSFeedCheckbox", serviceConstants.oDefaultConfiguration.IsRSSSelected);
            localStorage.setItem("oSendMailCheckbox", serviceConstants.oDefaultConfiguration.IsEmailOptionSelected);
            localStorage.setItem("oTaskCheckbox", serviceConstants.oDefaultConfiguration.IsTaskSelected);
            commonFunctions.maintainAppState(1);
            commonFunctions.maintainAppState(2);
        } else {
            serviceConstants.bCloseErrorPopup = false;
            serviceConstants.isValidClient = false;
            if (arrResults.code && "string" === typeof arrResults.code && (-1 < arrResults.code.indexOf("#|#"))) {
                showCommonErrorPopUp(arrResults.code);
            } else {
                arrResults.value && showErrorNotification("#ddlClient", arrResults.value);
            }
        }
    };
    commonFunctions.getDefaultValuesFailure = function (result) {
        return false;
    };
    commonFunctions.getDefaultValues = function (element) {
        if ($(element).attr("data-client-id")) {
            $(".popupWait, .loadingImage").removeClass("hide");
            $(".loadingImage").css("position", "absolute");
            var sClientUrl = $("#ddlClient").attr("data-value")
            , matterDetails = {
                "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "siteCollectionPath": sClientUrl
            };
            oJQueryObjects.$ClientName.attr("disabled", "disabled");
            oCommonObject.callProvisioningService("GetDefaultMatterConfigurations", matterDetails, commonFunctions.getDefaultValuesSuccess, commonFunctions.getDefaultValuesFailure);
        } else {
            $("#txtMatterName")[0].value = "";
            $("#txtMatterId")[0].value = "";
            $(".description").show();
            $("#documentTemplates").empty();
            $("#txtMatterDesc").empty().focus().blur();
            commonFunctions.maintainAppState(1);
        }
    };

    commonFunctions.onClientChange = function () {
        serviceConstants.bCloseErrorPopup = true;
        serviceConstants.isValidClient = true;
        oJQueryObjects.$ClientId.val($(oJQueryObjects.$ClientName).attr("data-client-id"));
    };

    /* Functions for retrieving roles */
    commonFunctions.getRoleSuccess = function (result) {
        var $RoleObject = $("#ddlRoleAssignList1");
        $RoleObject.html("");
        var arrResults = JSON.parse(result), roleItem, roleOption, arrOptions = [], iIterator = 0, iLength;
        if (!arrResults.code) {
            serviceConstants.oMandatoryRoles = arrResults;
            serviceConstants.arrRoles = JSON.parse(JSON.stringify(arrResults));
            for (roleItem in arrResults) {
                roleOption = "<div class='roleValue assignPerm1' title='" + arrResults[roleItem].Name + "'  data-value='" + arrResults[roleItem].Name + "' data-mandatory='" + arrResults[roleItem].Mandatory + "'>" + arrResults[roleItem].Name + "</div>";
                arrOptions.push(roleOption);
            }
            iLength = serviceConstants.oMandatoryRoles.length;
            for (iIterator = 0; iIterator < iLength; iIterator++) {
                if (serviceConstants.oMandatoryRoles[iIterator].Mandatory) {
                    serviceConstants.oMandatoryRoleNames.push(serviceConstants.oMandatoryRoles[iIterator].Name);
                }
            }
            $RoleObject.append(arrOptions.join(""));
            // This function will display drop-down menu
            $(".ddlRoleAssignDiv").click(function (event) {
                oJQueryObjects.$ErrorPopUp.addClass("hide");
                $("div input:text , textarea , select").css({ "border": "1px #c8c8c8 solid" });
                var $Element = $RoleObject;
                toggleThisElement($Element);
                var windowWidth = GetWidth();
                if (windowWidth <= 734) {
                    $RoleObject.css("top", parseInt($(this).offset().top, 10) + $(this).height() - 204);

                } else {
                    $RoleObject.css("top", parseInt($(this).offset().top, 10) + $(this).height() - 70);
                }
                event.stopPropagation();
            });

            $(".roleValue").mouseover(function () {
                $(this).addClass("ddListOnHover");
            }).mouseout(function () {
                $(this).removeClass("ddListOnHover");
            });

            // This function will select the item from drop-down list
            $(".assignPerm1").click(function (event) {
                var selectedClient = this.innerHTML, $Element = $("#ddlRoleAssign1");
                $Element.val(selectedClient).attr("data-value", $(this).attr("data-value")).attr("data-mandatory", $(this).attr("data-mandatory"));
                $("#ddlRoleAssignList1").addClass("hide");
                $(".assignPerm1").removeClass("ddListOnHover");
                event.stopPropagation();
            });

            // Default selection should be Responsible Attorney
            $($(".assignPerm1")[0]).click();
        } else {
            if (arrResults.code && "string" === typeof arrResults.code && (-1 < arrResults.code.indexOf("#|#"))) {
                showCommonErrorPopUp(arrResults.code);
            } else {
                showErrorNotification("#txtMatterId", arrResults.value);
            }
        }
        serviceConstants.bRole = true;
    };
    commonFunctions.getRoleFailure = function (result) {
        return false;
    };
    commonFunctions.getRole = function () {
        var sClientUrl = serviceConstants.sCentralRepositoryLocation
            , matterDetails = {
                "requestObject": {
                    "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
                }, "client": {
                    "Url": sClientUrl
                }
            };

        oCommonObject.callProvisioningService("GetRoleData", matterDetails, commonFunctions.getRoleSuccess, commonFunctions.getRoleFailure);
    };

    /* Functions for retrieving permissions */
    commonFunctions.getPermSuccess = function (result) {
        var $PermissionObject = $("#ddlPermAssignList1");
        $PermissionObject.html("");
        var arrResults = JSON.parse(result), permItem, permOption, arrOptions = [];
        if (!arrResults.code) {
            serviceConstants.arrPermissions = JSON.parse(JSON.stringify(arrResults));
            for (permItem in arrResults) {
                permOption = "<div class='permValue permContent1' data-value='" + arrResults[permItem].Name + "'  title='" + arrResults[permItem].Name + "'>" + arrResults[permItem].Name + "</div>";
                arrOptions.push(permOption);
            }
            $PermissionObject.append(arrOptions.join(""));
            // This function will display drop-down menu
            $(".ddlPermAssignDiv1").click(function (event) {
                closeAllPopupExcept("");
                var $Element = $PermissionObject;
                toggleThisElement($Element);

                var windowWidth = GetWidth();
                if (windowWidth <= 734) {
                    $PermissionObject.css("top", parseInt($(this).offset().top, 10) + $(this).height() - 204);

                } else {
                    $PermissionObject.css("top", parseInt($(this).offset().top, 10) + $(this).height() - 70);
                }
                event.stopPropagation();
            });

            $(".permValue").mouseover(function () {
                $(this).addClass("ddListOnHover");
            }).mouseout(function () {
                $(this).removeClass("ddListOnHover");
            });

            // This function will select the item from drop-down list
            $(".permContent1").click(function (event) {
                var selectedClient = this.innerHTML, $Element = $("#ddlPermAssign1");
                $Element.val(selectedClient).attr("data-value", $(this).attr("data-value"));
                $("#ddlPermAssignList1").addClass("hide");
                $(".permContent1").removeClass("ddListOnHover");
                event.stopPropagation();
            });
            // Default value should be full control
            $($(".permValue")[0]).click();
        } else {
            if (arrResults.code && "string" === typeof arrResults.code && (-1 < arrResults.code.indexOf("#|#"))) {
                showCommonErrorPopUp(arrResults.code);
            } else {
                showErrorNotification("#txtMatterId", arrResults.value);
            }
        }
        serviceConstants.bPermission = true;
    };
    commonFunctions.getPermFailure = function (result) {
        return false;
    };
    commonFunctions.getPerm = function () {
        var sClientUrl = serviceConstants.sCentralRepositoryLocation
            , matterDetails = {
                "requestObject": {
                    "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
                }, "client": {
                    "Url": sClientUrl
                }
            };

        oCommonObject.callProvisioningService("GetPermissionLevels", matterDetails, commonFunctions.getPermSuccess, commonFunctions.getpermFailure);
    };

    /* Functions for stamping properties */
    commonFunctions.stampPropertiesSuccess = function (result) {
        result = JSON.parse(result);
        if (!result.code) {
            commonFunctions.clearFields(result);
        } else {
            !(serviceConstants.bErrorOccured) && commonFunctions.handleErrorScenarios(result, oMatterProvisionConstants.Error_Stamp_Properties);
        }
    };
    commonFunctions.stampPropertiesFailure = function (result) {
        return false;
    };
    commonFunctions.stampProperties = function () {
        $(".notificationContainer .notification .closeNotification").click();
        showNotification(".notificationContainer", oMatterProvisionConstants.Success_Matter_Step_3, "successBanner");
        var oAssignPermList = $("input[id^=txtAssign]"), iErrorFlag = 0, sResponsibleAttorney = [], sResponsibleAttorneyEmail = [], arrTeamMembers = [];
        $.each(oAssignPermList, function () {
            var sCurrElementID = $(this).attr("id");
            if (sCurrElementID) {
                sCurrElementID = sCurrElementID.trim().split("txtAssign")[1];
                var sCurrRole = $("#ddlRoleAssign" + sCurrElementID), sCurrPermission = $("#txtAssign" + sCurrElementID);
                if (sCurrRole && sCurrPermission) {
                    if (-1 !== $.inArray(sCurrRole.val(), serviceConstants.oMandatoryRoleNames)) {
                        sResponsibleAttorney.push(oCommonObject.getUserName($.trim($(this).val()), true).join(";"));
                        sResponsibleAttorneyEmail.push(oCommonObject.getUserName($.trim($(this).val()), false).join(";"));
                    }
                }
            }
        });

        var sClientId = oJQueryObjects.$ClientId ? oJQueryObjects.$ClientId.val() : ""
        , sClientUrl = oJQueryObjects.$ClientName.attr("data-value")
        , sClientName = oJQueryObjects.$ClientName ? $(oJQueryObjects.$ClientName).val() : ""
        , matterId = oJQueryObjects.$MatterId ? oJQueryObjects.$MatterId.val() : ""
        , matterName = oJQueryObjects.$MatterName ? oJQueryObjects.$MatterName.val().trim() : ""
        , matterDesc = oJQueryObjects.$MatterDescription ? oJQueryObjects.$MatterDescription.val().trim() : ""
        , conflictCheckBy = oJQueryObjects.$ConflictConductedBy ? oCommonObject.getUserName(oJQueryObjects.$ConflictConductedBy.val().trim(), false)[0] : ""
        , conflictCheckOn = oJQueryObjects.$ConflictCheckDate ? oJQueryObjects.$ConflictCheckDate.val() : ""
        , ConflictIdentified = oJQueryObjects.$ConflictIdentified ? oJQueryObjects.$ConflictIdentified.val() : ""
        , boolSecureMatter = oJQueryObjects.$BoolSecureMatter
        , arrUserNames = []
        , arrUserEmails = []
        , matterDetails = {}
        , arrRoles = []
        , arrPermissions = []
        , oDocumentTemplates = $(".docTemplateItem")
        , arrBlockUserNames = ""
        , sBlockedUserNames = oJQueryObjects.$BlockedUsers.val()
        , defaultContentType = ""
        , contentTypes = []
        , arrDocumentTemplatesCount = []
        , roleInformation = {}
        , subAreaofLaw = ""
        , oMatterProvisionFlags = {
        };

        // getting the value of SAOL from data-display-name attribute
        $.each(oDocumentTemplates, function () {
            if (-1 === subAreaofLaw.indexOf($(this).attr("data-display-name"))) {
                subAreaofLaw += $(this).attr("data-display-name") + "; ";
                if (($(this).hasClass("popUpSelected"))) {
                    defaultContentType = $(this).attr("data-display-name");
                }
            }
        });

        $.each(oAssignPermList, function () {
            var sCurrElementID = $(this).attr("id");
            if (sCurrElementID) {
                sCurrElementID = sCurrElementID.trim().split("txtAssign")[1];
                var sCurrRole = $("#ddlRoleAssign" + sCurrElementID), sCurrPermission = $("#txtAssign" + sCurrElementID);
                if (sCurrRole && sCurrPermission) {
                    if (roleInformation.hasOwnProperty(sCurrRole.val())) {
                        // This role is already present. append the new role with semicolon separated value
                        roleInformation[sCurrRole.val()] = roleInformation[sCurrRole.val()] + sCurrPermission.val();
                    } else {
                        // Add this role to the object
                        roleInformation[sCurrRole.val()] = sCurrPermission.val();
                    }

                }
            }
        });

        $.each(roleInformation, function (key, item) {
            roleInformation[key] = trimEndChar(item.trim(), ";");
        });

        $.each($("input[id^=ddlRoleAssign]"), function () {
            arrRoles.push($(this).val());
        });

        $.each($("input[id^=txtAssign]"), function () {
            arrUserNames.push(oCommonObject.getUserName($(this).val().trim(), true));
            arrUserEmails.push(oCommonObject.getUserName($(this).val().trim(), false));
            arrTeamMembers.push(oCommonObject.getUserName($.trim($(this).val()), true).join(";"));
        });

        $.each($("input[id^=ddlPermAssign]"), function () {
            arrPermissions.push($(this).val());
        });

        if ("True" === ConflictIdentified) {
            if (sBlockedUserNames && "" !== sBlockedUserNames && oJQueryObjects.$BlockedUsers.length) {
                arrBlockUserNames = oCommonObject.getUserName(oJQueryObjects.$BlockedUsers.val().trim(), false);
            }
        }
        contentTypes = subAreaofLaw.trim().split(";");

        $(".docTemplateItem").each(function (iCurrentIndex, oCurrentObject) {
            arrDocumentTemplatesCount.push($(oCurrentObject).attr("data-associated-document-template").split(";").length.toString());
        });
        arrDocumentTemplatesCount.reverse();


        oMatterProvisionFlags = {
            "MatterLandingFlag": serviceConstants.bMatterLandingPage,
            "SendEmailFlag": null
        };


        if ($("#demo-checkbox-unselected2")[0].checked) {
            oMatterProvisionFlags.SendEmailFlag = true;
        } else {
            oMatterProvisionFlags.SendEmailFlag = false;
        }

        matterDetails = {
            "requestObject": {
                "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
            }, "client": {
                "Url": sClientUrl, "Id": sClientId, "Name": sClientName
            }, "matter": {
                "Name": matterName, "Id": matterId, "Description": matterDesc, "Conflict": { "Identified": ConflictIdentified, "CheckBy": conflictCheckBy, "CheckOn": conflictCheckOn, "SecureMatter": boolSecureMatter }, "BlockUserNames": arrBlockUserNames, "AssignUserNames": arrUserNames, "AssignUserEmails": arrUserEmails, "ContentTypes": contentTypes, "DefaultContentType": defaultContentType, "Permissions": arrPermissions, "Roles": arrRoles, "DocumentTemplateCount": arrDocumentTemplatesCount, "MatterGuid": serviceConstants.matterGUID
            }, "matterDetails": {
                "PracticeGroup": serviceConstants.sPracticeGroupList, "AreaOfLaw": serviceConstants.sAreaOfLawList, "SubareaOfLaw": trimEndChar(subAreaofLaw, ";"), "ResponsibleAttorney": sResponsibleAttorney.join(";").replace(/;;/g, ";"), "ResponsibleAttorneyEmail": sResponsibleAttorneyEmail.join(";").replace(/;;/g, ";"), "UploadBlockedUsers": serviceConstants.arrReadOnlyUsers, "TeamMembers": arrTeamMembers.join(";"), "RoleInformation": JSON.stringify(roleInformation)
            }, "matterProvisionChecks": oMatterProvisionFlags, "matterConfigurations": {
                "IsConflictCheck": serviceConstants.oDefaultConfiguration.IsConflictCheck, "IsMatterDescriptionMandatory": serviceConstants.oDefaultConfiguration.IsMatterDescriptionMandatory
            }
        };
        oCommonObject.callProvisioningService("UpdateMetadataForList", matterDetails, commonFunctions.stampPropertiesSuccess, commonFunctions.stampPropertiesFailure);

    };

    /* Functions for associating content types to matter */
    commonFunctions.associateContentTypesSuccess = function (result) {
        serviceConstants.iCallsComplete++;
        result = JSON.parse(result.replace(new RegExp("\r", "g"), "\\r").replace(new RegExp("\n", "g"), "\\n"));
        if (!result.code) {
            if (result.value && "string" === typeof result.value && "false" === result.value) {
                serviceConstants.bViewCreationFailed = true;
            }
            serviceConstants.iShowSuccessMessage++;
            (3 === parseInt(serviceConstants.iShowSuccessMessage, 10)) ? commonFunctions.stampProperties() : "";
        } else {
            serviceConstants.isErrorOccurred = true;
            !(serviceConstants.bErrorOccured) && commonFunctions.handleErrorScenarios(result, oMatterProvisionConstants.Error_Associate_Content_Type);
        }
        (3 === parseInt(serviceConstants.iCallsComplete, 10) && serviceConstants.isErrorOccurred) && commonFunctions.deleteMatterLandingPage();
    };
    commonFunctions.associateContentTypesFailure = function (result) {
        return false;
    };
    commonFunctions.associateContentTypes = function () {
        var sClientUrl = oJQueryObjects.$ClientName.attr("data-value")
        , matterName = oJQueryObjects.$MatterName.val().trim()
        , matterId = oJQueryObjects.$MatterId.val()
        , clientId = oJQueryObjects.$ClientId.val()
        , clientName = oJQueryObjects.$ClientName[0] ? $(oJQueryObjects.$ClientName).val() : ""
        , contentTypes = [], matterDetails = {}, defaultContentType;
        var $Element = $("#documentTemplates .docTemplateItem.popUpSelected");
        var sPracticeGroupName = $Element.attr("data-practicegroup")
        , sPracticeGroupId = $Element.attr("data-practicegroup-id")
        , sAreaOfLawName = $Element.attr("data-areaoflaw")
        , sAreaOfLawId = $Element.attr("data-areaoflaw-id")
        , sSubareaOfLawName = $Element.attr("data-display-name")
        , sSubareaOfLawId = $Element.attr("data-subareaoflaw-id");
        oJQueryObjects.$ContentTypes.each(function () {
            var currDocumentTemplate = $(this).attr("data-document-template");
            if (-1 === $.inArray(currDocumentTemplate, contentTypes)) {
                contentTypes.push($.trim(currDocumentTemplate));
            }
            var arrAssociatedDocumentTemplates = $(this).attr("data-associated-document-template").split(";");
            for (var iIterator = 0; iIterator < arrAssociatedDocumentTemplates.length; iIterator++) {
                if (-1 === $.inArray(arrAssociatedDocumentTemplates[iIterator], contentTypes)) {
                    contentTypes.push($.trim(arrAssociatedDocumentTemplates[iIterator]));
                }
            }
        });
        defaultContentType = localStorage.setDefaultContentType;

        matterDetails = {
            "requestObject": {
                "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
            },
            "matterMetadata": {
                "Matter": {
                    "Name": matterName,
                    "Id": matterId,
                    "ContentTypes": contentTypes,
                    "DefaultContentType": defaultContentType,
                    "MatterGuid": serviceConstants.matterGUID
                }, "Client": {
                    "Url": sClientUrl,
                    "Name": clientName,
                    "Id": clientId
                }, "PracticeGroupTerm": {
                    "TermName": sPracticeGroupName,
                    "Id": sPracticeGroupId
                }, "AreaTerm": {
                    "TermName": sAreaOfLawName,
                    "Id": sAreaOfLawId
                }, "SubareaTerm": {
                    "TermName": sSubareaOfLawName,
                    "Id": sSubareaOfLawId
                }
            }
        };

        oCommonObject.callProvisioningService("AssignContentType", matterDetails, commonFunctions.associateContentTypesSuccess, commonFunctions.associateContentTypesFailure);
    };

    /* Functions for fetching content types */
    commonFunctions.fetchContentTypesSuccess = function (result) {
        serviceConstants.bContentType = true;
        serviceConstants.oTermStoreJson = JSON.parse(result);
        if (!serviceConstants.oTermStoreJson.code) {
            populateContentTypes();
        } else {
            if (serviceConstants.oTermStoreJson.code && "string" === typeof serviceConstants.oTermStoreJson.code && (-1 < serviceConstants.oTermStoreJson.code.indexOf("#|#"))) {
                showCommonErrorPopUp(serviceConstants.oTermStoreJson.code);
            } else {
                showErrorNotification("#txtMatterId", serviceConstants.oTermStoreJson.value);
            }
        }
    };
    commonFunctions.fetchContentTypesFailure = function (result) {
        return false;
    };
    commonFunctions.fetchContentTypes = function () {
        var sClientUrl = serviceConstants.sCentralRepositoryLocation
        , matterDetails = {
            "requestObject": {
                "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
            }, "client": {
                "Url": sClientUrl
            }, "details": {
                "TermGroup": oGlobalConstants.Common_Term_Set_Group_Name, "TermSetName": oGlobalConstants.Practice_Group_Term_Set_Name, "CustomPropertyName": oGlobalConstants.Practice_Group_Custom_Properties, "DocumentTemplatesName": oGlobalConstants.Sub_Area_Of_Law_Document_Templates
            }
        };
        oJQueryObjects.$LoadingLookUp.removeClass("hide");
        oCommonObject.callProvisioningService("GetTaxonomyData", matterDetails, commonFunctions.fetchContentTypesSuccess, commonFunctions.fetchContentTypesFailure);
    };


    /* Functions for fetching content types */
    commonFunctions.assignPermissionSuccess = function (result) {
        serviceConstants.iCallsComplete++;
        result = JSON.parse(result);
        if (!result.code) {
            serviceConstants.iShowSuccessMessage++;
            (3 === parseInt(serviceConstants.iShowSuccessMessage, 10)) ? commonFunctions.stampProperties() : "";
        } else {
            serviceConstants.isErrorOccurred = true;
            !(serviceConstants.bErrorOccured) && commonFunctions.handleErrorScenarios(result, oMatterProvisionConstants.Error_Assign_Permission);
        }
        (3 === parseInt(serviceConstants.iCallsComplete, 10) && serviceConstants.isErrorOccurred) && commonFunctions.deleteMatterLandingPage();
    };
    commonFunctions.assignPermissionFailure = function (result) {
        return false;
    };
    commonFunctions.assignPermission = function () {
        var sClientUrl = oJQueryObjects.$ClientName.attr("data-value"), matterName = oJQueryObjects.$MatterName.val().trim(), arrUserNames = [], arrUserEmails = [], arrPermissions = [], matterDetails;
        $.each($("input[id^=txtAssign]"), function () {
            arrUserNames.push(oCommonObject.getUserName($(this).val().trim(), true));
            arrUserEmails.push(oCommonObject.getUserName($(this).val().trim(), false));
        });
        $.each($("input[id^=ddlPermAssign]"), function () {
            arrPermissions.push($.trim($(this).val()));
        });
        var isCalenderSelected = $("#demo-checkbox-unselected0")[0].checked;
        var isTaskSelected = $("#demo-checkbox-unselected3")[0].checked;
        matterDetails = {
            "requestObject": {
                "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
            }, "client": {
                "Url": sClientUrl
            }, "matter": {
                "Name": matterName, "Permissions": arrPermissions, "AssignUserNames": arrUserNames, "AssignUserEmails": arrUserEmails, "MatterGuid": serviceConstants.matterGUID
            }, "matterConfigurations": {
                "IsCalendarSelected": isCalenderSelected,
                "IsTaskSelected": isTaskSelected
            },
        };

        oCommonObject.callProvisioningService("AssignUserPermissions", matterDetails, commonFunctions.assignPermissionSuccess, commonFunctions.assignPermissionFailure);
    };

    /* Functions for creating matter landing page */
    commonFunctions.createMatterLandingPageSuccess = function (result) {
        serviceConstants.iCallsComplete++;
        result = JSON.parse(result);
        if (!result.code) {
            serviceConstants.iShowSuccessMessage++;
            (3 === parseInt(serviceConstants.iShowSuccessMessage, 10)) ? commonFunctions.stampProperties() : "";
        } else {
            serviceConstants.isErrorOccurred = true;
            serviceConstants.bMatterLandingPage = false;
            !(serviceConstants.bErrorOccured) && commonFunctions.handleErrorScenarios(result, oMatterProvisionConstants.Error_Matter_Landing_Page_Creation);
        }
        (3 === parseInt(serviceConstants.iCallsComplete, 10) && serviceConstants.isErrorOccurred) && commonFunctions.deleteMatterLandingPage();
    };
    commonFunctions.createMatterLandingPageFailure = function (result) {
        return false;
    };
    commonFunctions.createMatterLandingPage = function () {
        var sClientUrl = oJQueryObjects.$ClientName.attr("data-value")
        , matterName = (oJQueryObjects.$MatterName.length) ? oJQueryObjects.$MatterName.val().trim() : ""
        , arrBlockUserNames = ""
    , sBlockedUserNames = (oJQueryObjects.$BlockedUsers.length) ? oJQueryObjects.$BlockedUsers.val() : ""
    , ConflictIdentified = (oJQueryObjects.$ConflictIdentified.length) ? oJQueryObjects.$ConflictIdentified.val() : ""
    , arrUserNames = []
    , arrUserEmails = []
    , conflictCheckBy = oCommonObject.getUserName(oJQueryObjects.$ConflictConductedBy.val().trim(), false)[0]
    , conflictCheckOn = oJQueryObjects.$ConflictCheckDate.val()
    , boolSecureMatter = oJQueryObjects.$BoolSecureMatter
    , matterDesc = oJQueryObjects.$MatterDescription.val().trim()
    , matterDetails = {}
    , arrPermissions = [];
        $.each($("input[id^=txtAssign]"), function () {
            arrUserNames.push(oCommonObject.getUserName($(this).val().trim(), true));
            arrUserEmails.push(oCommonObject.getUserName($(this).val().trim(), false));
        });
        if ("True" === ConflictIdentified) {
            if (sBlockedUserNames && "" !== sBlockedUserNames && oJQueryObjects.$BlockedUsers.length) {
                arrBlockUserNames = oCommonObject.getUserName(oJQueryObjects.$BlockedUsers.val().trim(), false);
            }
        }
        $.each($("input[id^=ddlPermAssign]"), function () {
            arrPermissions.push($.trim($(this).val()));
        });
        var isCalendarSelected, isRSSFeedSelected, isTaskSelected;
        if ($("#demo-checkbox-unselected0")[0].checked) {
            isCalendarSelected = true;
        } else {
            isCalendarSelected = false;
        }
        if ($("#demo-checkbox-unselected1")[0].checked) {
            isRSSFeedSelected = true;
        } else {
            isRSSFeedSelected = false;
        }

        if ($("#demo-checkbox-unselected3")[0].checked) {
            isTaskSelected = true;
        } else {
            isTaskSelected = false;
        }

        matterDetails = {
            "requestObject": {
                "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
            }, "client": {
                "Url": sClientUrl
            }, "matter": {
                "Name": matterName, "Description": matterDesc, "AssignUserNames": arrUserNames, "AssignUserEmails": arrUserEmails, "BlockUserNames": arrBlockUserNames, "Conflict": { "Identified": ConflictIdentified, "CheckBy": conflictCheckBy, "CheckOn": conflictCheckOn, "SecureMatter": boolSecureMatter }, "Permissions": arrPermissions, "MatterGuid": serviceConstants.matterGUID
            }, "matterConfigurations": {
                "IsConflictCheck": serviceConstants.oDefaultConfiguration.IsConflictCheck, "IsMatterDescriptionMandatory": serviceConstants.oDefaultConfiguration.IsMatterDescriptionMandatory, "IsCalendarSelected": isCalendarSelected, "IsRSSSelected": isRSSFeedSelected, "IsTaskSelected": isTaskSelected
            }
        };

        oCommonObject.callProvisioningService("CreateMatterLandingPage", matterDetails, commonFunctions.createMatterLandingPageSuccess, commonFunctions.createMatterLandingPageFailure);
    };

    /* Functions for checking if security group exists in team members list*/
    commonFunctions.checkSecurityGroupExistsSuccess = function (result) {
        $(".popupWait, .loadingImage").addClass("hide");
        var event = result.oParam.currentEvent && result.oParam.currentEvent.target ? result.oParam.currentEvent : "";
        var oResult = JSON.parse(result.Result), controlId;
        if (!oResult.code) {
            navigateTab(event);
        } else {
            var sValidationResult = oResult.value;
            var oValidationDetails = sValidationResult.split("$|$"), sSecurityGroupRow, sSecurityGroupError;
            if (2 === oValidationDetails.length) {
                sSecurityGroupError = oValidationDetails[0];
                sSecurityGroupRow = oValidationDetails[1];
            }
            if ("-1" === sSecurityGroupRow) {
                controlId = "#txtBlockUser";
            } else if ("-2" === sSecurityGroupRow) {
                controlId = "#" + $(".inputAssignPerm")[0].id;
            } else {
                controlId = "#" + sSecurityGroupRow;
            }
            showErrorNotification(controlId, sSecurityGroupError);

        }
    };
    commonFunctions.checkSecurityGroupExistsFailure = function (result) {
        return false;
    };
    commonFunctions.checkSecurityGroupExists = function (event) {
        $(".popupWait, .loadingImage").removeClass("hide");
        $(".loadingImage").css("position", "absolute");
        oJQueryObjects.$ConflictIdentified = $("input[name=rdbConflictCheck]:checked");
        oJQueryObjects.$BlockedUsers = $("#txtBlockUser");
        var sClientUrl = oJQueryObjects.$ClientName.attr("data-value")
        , sConflictIdentified = (oJQueryObjects.$ConflictIdentified.length) ? oJQueryObjects.$ConflictIdentified.val() : ""
        , sBlockedUserNames = (oJQueryObjects.$BlockedUsers.length) ? oJQueryObjects.$BlockedUsers.val() : ""
        , matterName = (oJQueryObjects.$MatterName.length) ? oJQueryObjects.$MatterName.val().trim() : ""
        , arrUserNames = []
        , arrUserEmails = []
        , arrBlockUserNames = []
        , oSecurityGroupCheck = {}
        , userId = []
        , oParam = {
        };
        $.each($("input[id^=txtAssign]"), function () {
            arrUserNames.push(oCommonObject.getUserName($(this).val().trim(), true));
            arrUserEmails.push(oCommonObject.getUserName($(this).val().trim(), false));
            userId.push(this.id);
        });
        if ("True" === sConflictIdentified) {
            if (sBlockedUserNames && "" !== sBlockedUserNames && oJQueryObjects.$BlockedUsers.length) {
                arrBlockUserNames = oCommonObject.getUserName(oJQueryObjects.$BlockedUsers.val().trim(), false);
            }
        }
        oParam = {
            "currentEvent": event
        };
        oSecurityGroupCheck = {
            "requestObject": {
                "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
            }, "client": {
                "Url": sClientUrl
            }, "matter": {
                "Name": matterName, "AssignUserNames": arrUserNames, "AssignUserEmails": arrUserEmails, "Conflict": { "Identified": sConflictIdentified }, "BlockUserNames": arrBlockUserNames
            },
            "userId": userId
        };
        oCommonObject.callProvisioningService("CheckSecurityGroupExists", oSecurityGroupCheck, commonFunctions.checkSecurityGroupExistsSuccess, commonFunctions.checkSecurityGroupExistsFailure, null, oParam);
    };

    /* Functions to delete matter landing page if error occurs  */
    commonFunctions.deleteMatterLandingPage = function () {
        var sMatterName = oJQueryObjects.$MatterName.val().trim()
            , sClientUrl = oJQueryObjects.$ClientName.attr("data-value")
            , matterDetails = {
                "requestObject": {
                    "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken
                }, "client": {
                    "Url": sClientUrl
                }, "matter": {
                    "Name": sMatterName,
                    "MatterGuid": serviceConstants.matterGUID
                }, "hasErrorOccurred": true
            };
        if (sMatterName && sClientUrl) {
            oCommonObject.callProvisioningService("CheckMatterExists", matterDetails, commonFunctions.deleteMatterLandingPageSuccess, commonFunctions.deleteMatterLandingPageFailure);
        }
    };

    commonFunctions.deleteMatterLandingPageSuccess = function (result) {
        return false;
    };
    commonFunctions.deleteMatterLandingPageFailure = function (result) {
        return false;
    };

    // #endregion

    /* Function to validate the entries in Blocked user field */
    function validateBlockUsers() {
        var validity = true, culprit = "", arrBlockedUsers = oJQueryObjects.$BlockedUsers.val().trim().split(";");
        if (!oJQueryObjects.$BlockedUsers.val().trim()) {
            return true;
        }
        $.each(arrBlockedUsers, function (iIndex) {
            if (arrBlockedUsers[iIndex]) {
                $.each($("input[id^=txtAssign]"), function () {
                    if (-1 !== $(this).val().trim().indexOf(arrBlockedUsers[iIndex].trim())) {
                        validity = false;
                        culprit = $(this).attr("id");
                        return false;
                    }
                });
                if (!validity) {
                    return false;
                }
            }
        });
        if (validity) {
            validity = true;
        }
        if (!validity && culprit) {
            showErrorNotification("#" + culprit, oMatterProvisionConstants.Error_Invalid_User);
        }
        return validity;
    }

    /* Function to validate Assigned Permissions */
    function validateAssignedPerm() {
        "use strict";
        var oAssignPermList = $("input[id^=txtAssign]"), iErrorFlag = 0;
        //// Here we have a list of ID's of Assign Permissions which are currently present in Step 2
        $.each(oAssignPermList, function (iIterator) {
            var iCurrId = $(this).attr("id").trim().split("txtAssign")[1], iCurrUser = $("#txtAssign" + iCurrId), oCurrRoleItem = $("#ddlRoleAssign" + iCurrId);
            if (!$.trim(iCurrUser.val())) {
                iErrorFlag = 1;
                showErrorNotification(iCurrUser, oGlobalConstants.Edit_Matter_Validate_User.replace("{0}", oCurrRoleItem.val()));
                return false;
            }
        });
        if (1 === iErrorFlag) {
            return false;
        }
        return true;
    }

    /* Function to check person who is conducting conflict check is not in block user */
    function validateConflictUser() {
        var arrBlockedUsers = oJQueryObjects.$BlockedUsers.val().trim().split(";")
                , sConflictCheckUser = oCommonObject.getUserName(oJQueryObjects.$ConflictConductedBy.val().trim(), false)[0]
            , bConflict = false;
        $.each(arrBlockedUsers, function (iIndex) {
            if (sConflictCheckUser === arrBlockedUsers[iIndex].trim()) {
                bConflict = true;
                return false;
            }
        });
        return (!bConflict);
    }

    /* Function to validate Team Assignment line items */
    function validateTeamAssigmentRole() {
        var oAssignList = $("input[id^=ddlRoleAssign]")
    , iExpectedCount = 0, iActualCount = 0, iIterator = 0, iLength = serviceConstants.oMandatoryRoles.length;
        serviceConstants.oMandatoryRoleNames.length = 0;
        for (iIterator = 0; iIterator < iLength; iIterator++) {
            if (serviceConstants.oMandatoryRoles[iIterator].Mandatory) {
                iExpectedCount++;
                serviceConstants.oMandatoryRoleNames.push(serviceConstants.oMandatoryRoles[iIterator].Name);
            }
        }
        $.each(oAssignList, function () {
            if ($(this)[0]) {
                if ("true" === $($(this)[0]).attr("data-mandatory")) {
                    iActualCount++;
                }
            }
        });
        if (iExpectedCount <= iActualCount) {
            return true;
        }
        return false;
    }

    /* Function to validate Permission */
    function validatePermission() {
        "use strict";
        var oPermissionList = $("input[id^=ddlPermAssign]"), bIsFullControl = false;
        $.each(oPermissionList, function (oPermissionList, oPermissionListItem) {
            if (oPermissionListItem) {
                if (oMatterProvisionConstants.Edit_Matter_Mandatory_Permission_Level === $(oPermissionListItem).attr("data-value")) {
                    bIsFullControl = true;
                }
            }
        });
        return bIsFullControl;
    }

    /* Function to resize error pop for responsive UI */
    function resizeErrorPopup() {
        "use strict";
        closeAllPopupExcept("");
        var windowWidth = GetWidth();
        var width;
        $(".ui-autocomplete").hide();

        if (windowWidth <= 734) {
            $(".mainArea .errTriangleBorder").css("left", "210px");
            $(".mainArea .errTriangle").css("left", "210px");
        }
    }

    /* Function to get the screen width */
    function GetWidth() {
        "use strict";
        var x = 0;
        if (self.innerHeight) {
            x = self.innerWidth;
        } else if (document.documentElement && document.documentElement.clientHeight) {
            x = document.documentElement.clientWidth;
        } else if (document.body) {
            x = document.body.clientWidth;
        }
        return x;
    }

    /* Function to toggle element */
    function toggleThisElement($Element) {
        if ($Element.is(":visible")) {
            $Element.addClass("hide");
        } else {
            $Element.removeClass("hide");
        }
    }

    /* Function to check if all the users entered in the list are valid User names */
    function isValidUserList(sUserList) {
        var arrUserList = oCommonObject.getUserName(sUserList.trim(), false), bFlag = true; //// This will return an array with last element as "".
        //// You have all the users in oCommonObject.oSiteUser JSON object and you have users entered by user in arrUserList
        //// Check if each user in arrUserList is a part of oCommonObject.oSiteUser array. If at least one of it is not there return false flag
        if (!arrUserList) {
            return false;
        }
        $.each(arrUserList, function (iIterator) {
            var sCurrentUser = arrUserList[iIterator] && arrUserList[iIterator].trim();
            sCurrentUser = trimEndChar(sCurrentUser, ";");
            if (sCurrentUser && -1 === $.inArray(sCurrentUser, oCommonObject.oSiteUser)) {
                bFlag = false;
            }
            if (!bFlag) {
                return;
            }
        });
        return bFlag;
    }

    /* Function to perform validation */
    commonFunctions.validateCurrentPage = function (iCurrentPage) {
        var bInValid = false
            , RegularExpression = new RegExp(oMatterProvisionConstants.Special_Character_Expression_Matter_Title)
            , sCurrMatterName = oJQueryObjects.$MatterName.val().trim()
            , arrValidMatch = sCurrMatterName.match(RegularExpression)
            , iIterator = 1;
        switch (iCurrentPage) {
            case 1:
                if (oJQueryObjects.$ClientName.attr("data-value").length && oJQueryObjects.$ClientName.attr("data-client-id").length) {
                    if (oJQueryObjects.$MatterName.length && oJQueryObjects.$MatterName[0].validity.valid) {
                        if (null === arrValidMatch || arrValidMatch[0] !== sCurrMatterName) {
                            bInValid = false;
                            $("#txtMatterName").css("border-width", "2px").css("border-color", "red");
                        } else {
                            bInValid = true;
                            $("#txtMatterName").css("border-width", "1px").css("border-color", "#c8c8c8");
                        }
                        if (bInValid) {
                            /* Re-factored code */
                            RegularExpression = new RegExp(oMatterProvisionConstants.Special_Character_Expression_Matter_Id);
                            if (oJQueryObjects.$MatterId.length && oJQueryObjects.$MatterId[0].validity.valid) {
                                arrValidMatch = oJQueryObjects.$MatterId.val().match(RegularExpression);
                                if (!arrValidMatch || arrValidMatch[0] !== oJQueryObjects.$MatterId.val()) {
                                    bInValid = false;
                                    $("#txtMatterId").css("border-width", "2px").css("border-color", "red");
                                } else {
                                    bInValid = true;
                                    $("#txtMatterId").css("border-width", "1px").css("border-color", "#c8c8c8");
                                }
                                if (bInValid) {
                                    oJQueryObjects.$ContentTypes = $("#documentTemplates > .docTemplateItem");
                                    if (oJQueryObjects.$ContentTypes.length) {
                                        if (localStorage.setDefaultContentType.length) {
                                            RegularExpression = new RegExp(oMatterProvisionConstants.Special_Character_Expression_Matter_Description);
                                            if (oJQueryObjects.$MatterDescription.length && oJQueryObjects.$MatterDescription[0].validity.valid && "" !== $.trim(oJQueryObjects.$MatterDescription.val())) {
                                                arrValidMatch = oJQueryObjects.$MatterDescription.val().trim().match(RegularExpression);
                                                if (!arrValidMatch || arrValidMatch[0] !== $.trim(oJQueryObjects.$MatterDescription.val())) {
                                                    bInValid = false;
                                                    $("#txtMatterDesc").css("border-width", "2px").css("border-color", "red");
                                                } else {
                                                    bInValid = true;
                                                    $("#txtMatterDesc").css("border-width", "1px").css("border-color", "#c8c8c8");
                                                }
                                                if (bInValid) {
                                                    return true;
                                                } else {
                                                    showErrorNotification("#txtMatterDesc", oMatterProvisionConstants.Error_Special_Character);
                                                }
                                            } else {
                                                if (serviceConstants.oDefaultConfiguration.IsMatterDescriptionMandatory) {
                                                    showErrorNotification("#txtMatterDesc", oMatterProvisionConstants.Error_Matter_Description);
                                                } else {
                                                    return true;
                                                }
                                            }
                                        } else {
                                            showErrorNotification("#btnLookup", oMatterProvisionConstants.Error_Default_Matter_Type);
                                        }
                                    } else {
                                        showErrorNotification("#btnLookup", oMatterProvisionConstants.Error_Select_Content_Type);
                                    }
                                } else {
                                    showErrorNotification("#txtMatterId", oMatterProvisionConstants.Error_Special_Character);
                                }
                            } else {
                                showErrorNotification("#txtMatterId", oMatterProvisionConstants.Error_Matter_ID);
                            }
                            /* Refactored code */
                        } else {
                            showErrorNotification("#txtMatterName", oMatterProvisionConstants.Error_Valid_Matter_Name);
                        }
                    } else {
                        showErrorNotification("#txtMatterName", oMatterProvisionConstants.Error_Matter_Title);
                    }
                } else {
                    showErrorNotification("#ddlClient", oMatterProvisionConstants.Error_Select_Client);
                }
                oJQueryObjects.$Loading.addClass("hide");
                break;
            case 2:
                oJQueryObjects.$ConflictIdentified = $("input[name=rdbConflictCheck]:checked");
                if (serviceConstants.oDefaultConfiguration.IsConflictCheck) {
                    if ($("#chkConflictCheck:checked").length) {
                        if (oJQueryObjects.$ConflictConductedBy.length && oJQueryObjects.$ConflictConductedBy[0].validity.valid && (oJQueryObjects.$ConflictConductedBy.attr("data-resolved") || isValidUserList(oJQueryObjects.$ConflictConductedBy.val()))) {
                            if (oJQueryObjects.$ConflictCheckDate.length && !oJQueryObjects.$ConflictCheckDate[0].validity.valueMissing) {
                                if (oJQueryObjects.$ConflictCheckDate.length && !oJQueryObjects.$ConflictCheckDate[0].validity.patternMismatch) {
                                    if (oJQueryObjects.$ConflictCheckDate.length && oJQueryObjects.$ConflictCheckDate[0].validity.valid) {
                                        if ((oJQueryObjects.$ConflictIdentified.length && "False" === oJQueryObjects.$ConflictIdentified.val()) ||
                                            (oJQueryObjects.$ConflictIdentified.length && "True" === oJQueryObjects.$ConflictIdentified.val() && (oJQueryObjects.$BlockedUsers.attr("data-resolved") || isValidUserList(oJQueryObjects.$BlockedUsers.val())))) {
                                            //// validate if person who conducted conflict cannot be in block user
                                            if (validateConflictUser()) {
                                                //// validate for each Assign permission entry
                                                if (validateAssignedPerm()) {
                                                    if (validateTeamAssigmentRole()) {
                                                        if (validatePermission()) {
                                                            if (validateBlockUsers()) {
                                                                if (isValidBlockUser()) {
                                                                    return true;
                                                                } else {
                                                                    showErrorNotification("#txtBlockUser", oMatterProvisionConstants.Error_Valid_Block_Users);
                                                                }
                                                            }
                                                        } else {
                                                            showErrorNotification("#" + $(".assignPermission")[0].id, oMatterProvisionConstants.Error_Edit_Matter_Mandatory_Permission);
                                                        }
                                                    } else {
                                                        showErrorNotification("#" + $(".assignRole")[0].id, oMatterProvisionConstants.Error_Mandatory_Role.replace("{0}", serviceConstants.oMandatoryRoleNames.join(",")));
                                                        serviceConstants.oMandatoryRoleNames.length = 0;
                                                    }
                                                }
                                            } else {
                                                showErrorNotification("#txtBlockUser", oMatterProvisionConstants.Error_Conflict_Check_User);
                                            }
                                        } else {
                                            showErrorNotification("#txtBlockUser", oMatterProvisionConstants.Error_Blocked_Users);
                                        }
                                    } else {
                                        showErrorNotification("#txtConflictCheckDate", oMatterProvisionConstants.Error_Select_Date);
                                    }
                                } else {
                                    showErrorNotification("#txtConflictCheckDate", oMatterProvisionConstants.Error_Valid_Date);
                                }
                            } else {
                                showErrorNotification("#txtConflictCheckDate", oMatterProvisionConstants.Error_Select_Date);
                            }
                        } else {
                            showErrorNotification("#txtConflictCheckBy", oMatterProvisionConstants.Error_Conflict_Reviewer_User);
                        }
                    } else {
                        showErrorNotification(".chkConflictCheckParent", oMatterProvisionConstants.Error_Conflict_Exists);
                    }
                } else {
                    if (validateAssignedPerm()) {
                        if (validateTeamAssigmentRole()) {
                            if (validatePermission()) {
                                return true;
                            } else {
                                showErrorNotification("#" + $(".assignPermission")[0].id, oMatterProvisionConstants.Error_Edit_Matter_Mandatory_Permission);
                            }
                        } else {
                            showErrorNotification("#" + $(".assignRole")[0].id, oMatterProvisionConstants.Error_Mandatory_Role.replace("{0}", serviceConstants.oMandatoryRoleNames.join(",")));
                            serviceConstants.oMandatoryRoleNames.length = 0;
                        }
                    }
                }
                oJQueryObjects.$Loading.addClass("hide");
                return false;
            default:
                serviceConstants.oMandatoryRoleNames.length = 0;
                return true;
        }
    };

    /* Function to clear all the fields once matter is created */
    commonFunctions.clearFields = function (result) {
        var sClientId = (oJQueryObjects.$ClientId.length) ? oJQueryObjects.$ClientId.val() : "",
         sClientName = (oJQueryObjects.$ClientName.length) ? $(oJQueryObjects.$ClientName).val() : "",
         sMatterId = (oJQueryObjects.$MatterId.length) ? oJQueryObjects.$MatterId.val() : "",
         sMatterName = (oJQueryObjects.$MatterName.length) ? oJQueryObjects.$MatterName.val().trim() : "",
         sClientUrl = (oJQueryObjects.$ClientName.length) ? oJQueryObjects.$ClientName.attr("data-value") : "";

        serviceConstants.iShowSuccessMessage = 0;
        // #region Clear Page 1 fields
        $($(".ddlClientListItem")[0]).click();
        oJQueryObjects.$ClientId.val("");
        oJQueryObjects.$MatterName.val("");
        oJQueryObjects.$MatterId.val("");
        oJQueryObjects.$MatterDescription.val("");
        oJQueryObjects.$ConflictConductedBy.val("").removeAttr("data-resolved");
        oJQueryObjects.$ConflictCheckDate.val("");
        oJQueryObjects.$BlockedUsers.val("").removeAttr("data-resolved");
        oJQueryObjects.$ContentTypes.remove();
        $($(".popUpOptions")[0]).click();
        //// #endregion 
        // #region Clear Page 2 fields
        oJQueryObjects.$AssignPermissions = $(".assignNewPerm");
        oJQueryObjects.$AssignPermissions.remove();
        commonFunctions.addMorePermissions(1);
        oJQueryObjects.$AssignPermissions.find("input[id^=txtAssign]").val("").removeAttr("data-resolved");
        oJQueryObjects.$AssignPermissions.find("select[id^=ddlRoleAssign]").val(serviceConstants.oMandatoryRoleNames[0]);
        $(".permContent1[data-value='Full Control']").click();
        $(".assignPerm1[data-value='Responsible Attorney']").click();
        oJQueryObjects.$AssignPermissions.find("select[id^=ddlPermAssign]").val("Full Control");
        oJQueryObjects.$ConflictCheckConducted.click();
        oJQueryObjects.$SwitchApp = $(".switchApp");
        $("input[name=rdbConflictCheck][value=True]").click(); //// Default Conflict Check to Yes
        $("input[name=rdbSecureMatterCheck][value=True]").click(); //// Default Pessimistic security Check to Yes
        localStorage.iLivePage = 1;
        oJQueryObjects.$Loading.addClass("hide");
        $(".notificationContainer .notification .closeNotification").click();
        //// #endregion
        // #region Clear Local Storage data
        localStorage.removeItem("oPageOneData");
        localStorage.removeItem("oPageTwoData");
        localStorage.removeItem("setDefaultContentType");
        localStorage.removeItem("IsConflictCheck");
        localStorage.removeItem("oCalenderCheckbox");
        localStorage.removeItem("oRSSFeedCheckbox");
        localStorage.removeItem("oSendMailCheckbox");
        localStorage.removeItem("oTaskCheckbox");

        // #endregion
        //// Click first menu tab
        oJQueryObjects.$LookUpButton.removeAttr("disabled");
        oJQueryObjects.$CreateButton.removeAttr("disabled");
        $(".ms-ChoiceField-field").removeClass("disable-checkbox");
        $("#demo-checkbox-unselected0")[0].checked = false;
        $("#demo-checkbox-unselected1")[0].checked = false;
        $("#demo-checkbox-unselected2")[0].checked = false;
        if ($("#demo-checkbox-unselected2")[0].checked) {
            $("#btnCreateMatter").text(oMatterProvisionConstants.Create_Notify_Button_Text);
        } else {
            $("#btnCreateMatter").text(oMatterProvisionConstants.Create_Button_Text);
        }
        var sSuccessMessage = commonFunctions.createdMatterUrl;
        if (serviceConstants.bMatterLandingPage) {
            sSuccessMessage += "/" + oGlobalConstants.Matter_Landing_Page_Repository + "/" + serviceConstants.matterGUID + ".aspx";
        }
        if (serviceConstants.bViewCreationFailed) {
            var sCreateViewFailureMessage = oMatterProvisionConstants.Success_Matter_Creation + " " + oMatterProvisionConstants.Create_View_Failure;
            showNotification(".notificationContainer", sCreateViewFailureMessage.replace("{0}", sSuccessMessage), "warningBanner");
        }
        if (result.value && "false" === result.value) {
            var sShareMatterFailureMessage = oMatterProvisionConstants.Success_Matter_Creation + " " + oMatterProvisionConstants.Share_Matter_Failure;
            showNotification(".notificationContainer", sShareMatterFailureMessage.replace("{0}", sSuccessMessage), "warningBanner"); //// display final success message
        } else {
            !(serviceConstants.bViewCreationFailed) && showNotification(".notificationContainer", oMatterProvisionConstants.Success_Matter_Creation.replace("{0}", sSuccessMessage), "successBanner"); //// display final success message
        }
        result && $(oJQueryObjects.$MenuClick[0]).click();
        getMatterGUID(); //// Create new GUID for next matter library
        localStorage.setItem("matterGuid", serviceConstants.matterGUID); //// Store the matter GUID in case an error while provisioning, will be used to delete matter landing page
    };
    /* Function to save current state of the page */
    commonFunctions.maintainAppState = function (iCurrentPage) {
        var oPageOneState = {
            ClientValue: [],
            ClientId: "",
            MatterTitle: "",
            MatterId: "",
            MatterDescription: "",
            ContentTypes: [],
            matterMandatory: ""
        },
        oPageTwoState = {
            BlockedSiteUsers: [],
            ConfCheckCond: "",
            ConfCheckCondBy: {
            },
            CheckPerfOn: "",
            ConflictIdent: "",
            BlockUsers: {
            },
            BoolSecureMatter: "",
            AssignPermission: []
        };
        oJQueryObjects.$ContentTypes = $("#documentTemplates > .docTemplateItem");
        oJQueryObjects.$AssignPermissions = $(".assignNewPerm");
        oJQueryObjects.$BlockedUsers = $("#txtBlockUser");
        switch (iCurrentPage) {
            case 1:
                ////store Page 1 data on click of step 2
                oPageOneState.ClientValue.push({ ClientName: oJQueryObjects.$ClientName.val(), value: oJQueryObjects.$ClientName.attr("data-value"), ClientId: oJQueryObjects.$ClientName.attr("data-client-id") });
                oPageOneState.ClientId = oJQueryObjects.$ClientId.val();
                oPageOneState.MatterTitle = oJQueryObjects.$MatterName.val().trim();
                oPageOneState.MatterId = oJQueryObjects.$MatterId.val();
                oPageOneState.MatterDescription = oJQueryObjects.$MatterDescription.val().trim();
                $.each(oJQueryObjects.$ContentTypes, function () {
                    oPageOneState.ContentTypes.push({ DataValue: $(this).attr("data-value"), DataDocumentTemplate: $(this).attr("data-document-template"), DataAssociatedDocumentTemplate: $(this).attr("data-associated-document-template"), DocumentTemplateText: $(this).text(), DataPracticeGroup: $(this).attr("data-practicegroup"), DataAreaOfLaw: $(this).attr("data-areaoflaw"), DataFolderNamesPG: $(this).attr("data-folderNamesPG"), DataFolderNamesAOL: $(this).attr("data-folderNamesAOL"), DataFolderNamesSAL: $(this).attr("data-folderNamesSAL"), DataisNoFolderStructurePresent: $(this).attr("data-isNoFolderStructurePresent"), DataDisplayName: $(this).attr("data-display-name"), DataPracticeGroupId: $(this).attr("data-practicegroup-id"), DataAreaOfLawId: $(this).attr("data-areaoflaw-id"), DataSubAreaOfLawId: $(this).attr("data-subareaoflaw-id") });
                });
                oPageOneState.matterMandatory = serviceConstants.oDefaultConfiguration.IsMatterDescriptionMandatory;
                localStorage.setItem("oPageOneData", JSON.stringify(oPageOneState));
                break;
            case 2:
                ////store Page 2 data on click of step 3
                oPageTwoState.ConfCheckCond = $("input[id=chkConflictCheck]:checked").length;
                oPageTwoState.ConfCheckCondBy.Text = oJQueryObjects.$ConflictConductedBy.val();
                oPageTwoState.ConfCheckCondBy.DataResolved = oJQueryObjects.$ConflictConductedBy.attr("data-resolved") ? true : false; //// store true if 1
                oPageTwoState.CheckPerfOn = oJQueryObjects.$ConflictCheckDate.val();
                oPageTwoState.ConflictIdent = $("input[name=rdbConflictCheck]:checked").val();  //// bind this on click
                oPageTwoState.BlockUsers.Text = oJQueryObjects.$BlockedUsers.val().trim();
                oPageTwoState.BlockUsers.DataResolved = oJQueryObjects.$BlockedUsers.attr("data-resolved") ? true : false; //// store true if 1
                oPageTwoState.BlockedSiteUsers = oCommonObject.oSiteUser;
                if ($("input[name=rdbSecureMatterCheck]:checked")) {
                    oJQueryObjects.$BoolSecureMatter = $("input[name=rdbSecureMatterCheck]:checked").val();
                }
                oPageTwoState.BoolSecureMatter = oJQueryObjects.$BoolSecureMatter;
                $.each(oJQueryObjects.$AssignPermissions, function () {
                    oPageTwoState.AssignPermission.push({ NameList: $(this).find("input[id^=txtAssign]").val(), DataResolved: $(this).find("input[id^=txtAssign]").attr("data-resolved") ? true : false, Role: $(this).find("input[id^=ddlRoleAssign]").val(), Permission: $(this).find("input[id^=ddlPermAssign]").val() });
                });
                localStorage.setItem("oPageTwoData", JSON.stringify(oPageTwoState));
                break;
        }
    };

    /* Function to add more team assignment line items */
    commonFunctions.addMorePermissions = function (iAssignPermCount) {
        var iNextAssignPermContent = parseInt(iAssignPermCount, 10);
        // For setting proper class name suffix 

        if (1 === iNextAssignPermContent) {
            serviceConstants.iLastRowNumber = iAssignPermCount;
        } else {
            iNextAssignPermContent = serviceConstants.iLastRowNumber;
        }
        var permItem, permOption, roleItem, roleOption, arrOptions = []
            // On click of Add more link, this chunk will be displayed
           , sAssignPermContent = "<div class='row" + iNextAssignPermContent + " assignNewPerm'>"
                                    + "<input type='text' class='medium clear inputAssignPerm' id='txtAssign" + iNextAssignPermContent + "' spellcheck='true' placeholder='Enter and select names' required />"
                                    + "<div class='gutter'></div>"

                                    + "<div class='mediumSmallRole floatContentLeft ddlRoleAssignDiv" + iNextAssignPermContent + "'>"
                                    + "<input class='mediumSmallRole assignRole assignRoleField' type='text' id='ddlRoleAssign" + iNextAssignPermContent + "' data-value='' data-mandatory='' disabled='disabled' autofocus='autofocus' />"
                                    + "<img class='ddlRoleAssignIcon' id='ddlRoleAssignIcon" + iNextAssignPermContent + "' title='Dropdown icon' src='../Images/icon-combobox.png' />"
                                    + "</div>"
                                    + "<div id='ddlRoleAssignList" + iNextAssignPermContent + "' class='ddlRoleAssignList clear hide'>"
                                    + "</div>"

                                    + "<div class='gutter'></div>"
                                    + "<div class='mediumSmallPermission floatContentLeft ddlPermAssignDiv" + iNextAssignPermContent + "'>"
                                    + "<input class='mediumSmallPermission assignPermission assignPermField' type='text' id='ddlPermAssign" + iNextAssignPermContent + "' data-value='' disabled='disabled' autofocus='autofocus' />"
                                    + "<img class='ddlPermAssignIcon' id='ddlPermAssignIcon" + iNextAssignPermContent + "' title='Dropdown icon' src='../Images/icon-combobox.png' />"
                                    + "</div>"
                                    + "<div id='ddlPermAssignList" + iNextAssignPermContent + "' class='ddlPermAssignList clear hide'>"
                                    + "</div>"

                                    + "<div class='gutter'></div>"
                                    + "<div class='close'>"
                                    + "<img alt='delete' src='../Images/icon-delete.png' title='delete'/>"
                                    + "</div>"
                                    + "</div>";
        $("#addNewAssignPerm").before(sAssignPermContent);

        for (roleItem in serviceConstants.arrRoles) {
            roleOption = "<div class='roleValue assignPerm" + iNextAssignPermContent + "' title='" + serviceConstants.arrRoles[roleItem].Name + "' data-value='" + serviceConstants.arrRoles[roleItem].Name + "' data-mandatory='" + serviceConstants.arrRoles[roleItem].Mandatory + "'>" + serviceConstants.arrRoles[roleItem].Name + "</div>";
            arrOptions.push(roleOption);
        }
        var $RoleObject = $("#ddlRoleAssignList" + iNextAssignPermContent + "");
        $RoleObject.append(arrOptions.join(""));
        // This function will display drop-down menu
        $(".ddlRoleAssignDiv" + iNextAssignPermContent + "").click(function (event) {
            closeAllPopupExcept("");
            var $Element = $RoleObject;
            toggleThisElement($Element);
            var windowWidth = GetWidth();
            if (windowWidth <= 734) {
                $RoleObject.css("top", parseInt($(this).offset().top, 10) + $(this).height() - 204);

            } else {
                $RoleObject.css("top", parseInt($(this).offset().top, 10) + $(this).height() - 70);
            }
            event.stopPropagation();
        });

        $(".roleValue").mouseover(function () {
            $(this).addClass("ddListOnHover");
        }).mouseout(function () {
            $(this).removeClass("ddListOnHover");
        });

        // This function will select the item from drop-down list
        $(".assignPerm" + iNextAssignPermContent + "").click(function (event) {
            var selectedClient = this.innerHTML, $Element = $("#ddlRoleAssign" + iNextAssignPermContent + "");
            $Element.val(selectedClient).attr("data-value", $(this).attr("data-value")).attr("data-mandatory", $(this).attr("data-mandatory"));
            $RoleObject.addClass("hide");
            $(".assignPerm" + iNextAssignPermContent + "").removeClass("ddListOnHover");
            event.stopPropagation();
        });

        $($(".assignPerm" + iNextAssignPermContent + "")[0]).click();

        arrOptions = [];
        for (permItem in serviceConstants.arrPermissions) {
            permOption = "<div class='permValue permContent" + iNextAssignPermContent + "' data-value='" + serviceConstants.arrPermissions[permItem].Name + "'  title='" + serviceConstants.arrPermissions[permItem].Name + "'>" + serviceConstants.arrPermissions[permItem].Name + "</div>";
            arrOptions.push(permOption);
        }
        var $PermissionObject = $("#ddlPermAssignList" + iNextAssignPermContent + "");
        $PermissionObject.append(arrOptions.join(""));

        // This function will display drop-down menu
        $(".ddlPermAssignDiv" + iNextAssignPermContent + "").click(function (event) {
            closeAllPopupExcept("");
            var $Element = $PermissionObject;
            toggleThisElement($Element);
            var windowWidth = GetWidth();
            if (windowWidth <= 734) {
                $PermissionObject.css("top", parseInt($(this).offset().top, 10) + $(this).height() - 204);

            } else {
                $PermissionObject.css("top", parseInt($(this).offset().top, 10) + $(this).height() - 70);
            }
            event.stopPropagation();
        });

        $(".permValue").mouseover(function () {
            $(this).addClass("ddListOnHover");
        }).mouseout(function () {
            $(this).removeClass("ddListOnHover");
        });

        // This function will select the item from drop-down list
        $(".permContent" + iNextAssignPermContent + "").click(function (event) {
            var selectedClient = this.innerHTML, $Element = $("#ddlPermAssign" + iNextAssignPermContent + "");
            $Element.val(selectedClient).attr("data-value", $(this).attr("data-value"));
            $PermissionObject.addClass("hide");
            $(".permContent" + iNextAssignPermContent + "").removeClass("ddListOnHover");
            event.stopPropagation();
        });

        $($(".permContent" + iNextAssignPermContent + "")[0]).click();

        oCommonObject.bindAutocomplete("#txtAssign" + iNextAssignPermContent, true);
        sortDropDownListByText();
        serviceConstants.iLastRowNumber++;
    };

    /* Function to load app state */
    commonFunctions.loadAppStateData = function (iCurrPage) {
        //// Load Page 1 data        
        if (localStorage.getItem("oPageOneData")) {
            var oSavedPageOneData = JSON.parse(localStorage.getItem("oPageOneData"))
                , oContentTypeContainer = $("#documentTemplates")
                , iContent = 0, oSavedPageTwoData, iCount = 0, iNextPerm, iAssignPermLength;
            oJQueryObjects.$ClientName.val(oSavedPageOneData.ClientValue[0].ClientName).attr("data-value", oSavedPageOneData.ClientValue[0].value).attr("data-client-id", oSavedPageOneData.ClientValue[0].ClientId);
            oJQueryObjects.$ClientId.val(oSavedPageOneData.ClientId);
            oJQueryObjects.$MatterName.val(oSavedPageOneData.MatterTitle);
            checkValidMatterName();
            oJQueryObjects.$MatterId.val(oSavedPageOneData.MatterId);
            oJQueryObjects.$MatterDescription.val(oSavedPageOneData.MatterDescription);
            for (iContent = 0; iContent < oSavedPageOneData.ContentTypes.length; iContent++) {
                if (oSavedPageOneData.ContentTypes[iContent].DataDocumentTemplate === localStorage.setDefaultContentType) {
                    oContentTypeContainer.append("<div class='docTemplateItem popUpSelected' data-value='" + oSavedPageOneData.ContentTypes[iContent].DataValue + "' data-document-template='" + oSavedPageOneData.ContentTypes[iContent].DataDocumentTemplate + "' data-associated-document-template='" + oSavedPageOneData.ContentTypes[iContent].DataAssociatedDocumentTemplate + "' data-practicegroup='" + oSavedPageOneData.ContentTypes[iContent].DataPracticeGroup + "' data-areaoflaw='" + oSavedPageOneData.ContentTypes[iContent].DataAreaOfLaw + "' data-practicegroup-id='" + oSavedPageOneData.ContentTypes[iContent].DataPracticeGroupId + "' data-areaoflaw-id='" + oSavedPageOneData.ContentTypes[iContent].DataAreaOfLawId + "' data-subareaoflaw-id='" + oSavedPageOneData.ContentTypes[iContent].DataSubAreaOfLawId + "' data-foldernamespg='" + oSavedPageOneData.ContentTypes[iContent].DataFolderNamesPG + "' data-foldernamesaol='" + oSavedPageOneData.ContentTypes[iContent].DataFolderNamesAOL + "' data-foldernamessal='" + oSavedPageOneData.ContentTypes[iContent].DataFolderNamesSAL + "' data-isnofolderstructurepresent='" + oSavedPageOneData.ContentTypes[iContent].DataisNoFolderStructurePresent + "' data-display-name='" + oSavedPageOneData.ContentTypes[iContent].DataDisplayName + "' >" + oSavedPageOneData.ContentTypes[iContent].DocumentTemplateText + "</div>");
                } else {
                    oContentTypeContainer.append("<div class='docTemplateItem' data-value='" + oSavedPageOneData.ContentTypes[iContent].DataValue + "' data-document-template='" + oSavedPageOneData.ContentTypes[iContent].DataDocumentTemplate + "' data-associated-document-template='" + oSavedPageOneData.ContentTypes[iContent].DataAssociatedDocumentTemplate + "' data-practicegroup='" + oSavedPageOneData.ContentTypes[iContent].DataPracticeGroup + "' data-areaoflaw='" + oSavedPageOneData.ContentTypes[iContent].DataAreaOfLaw + "' data-practicegroup-id='" + oSavedPageOneData.ContentTypes[iContent].DataPracticeGroupId + "' data-areaoflaw-id='" + oSavedPageOneData.ContentTypes[iContent].DataAreaOfLawId + "' data-subareaoflaw-id='" + oSavedPageOneData.ContentTypes[iContent].DataSubAreaOfLawId + "' data-foldernamespg='" + oSavedPageOneData.ContentTypes[iContent].DataFolderNamesPG + "' data-foldernamesaol='" + oSavedPageOneData.ContentTypes[iContent].DataFolderNamesAOL + "' data-foldernamessal='" + oSavedPageOneData.ContentTypes[iContent].DataFolderNamesSAL + "' data-isnofolderstructurepresent='" + oSavedPageOneData.ContentTypes[iContent].DataisNoFolderStructurePresent + "' data-display-name='" + oSavedPageOneData.ContentTypes[iContent].DataDisplayName + "' >" + oSavedPageOneData.ContentTypes[iContent].DocumentTemplateText + "</div>");
                }
            }
            serviceConstants.oDefaultConfiguration.IsMatterDescriptionMandatory = oSavedPageOneData.matterMandatory;
            if (oSavedPageOneData.matterMandatory) {
                $(".description").show();
            } else {
                $(".description").hide();
            }
            if (localStorage.getItem("IsConflictCheck")) {
                serviceConstants.oDefaultConfiguration.IsConflictCheck = JSON.parse(localStorage.getItem("IsConflictCheck"));
                if (!serviceConstants.oDefaultConfiguration.IsConflictCheck) {
                    $("#conflictCheck, #assignTeam").addClass("hide");
                    $("input[name=rdbConflictCheck][value=False]").prop("checked", "checked");
                    serviceConstants.sConflictScenario = "True";
                } else {
                    $("#conflictCheck, #assignTeam").removeClass("hide");
                    $("input[name=rdbConflictCheck][value=True]").prop("checked", "checked");
                    $("#snBlockUser").show();
                    serviceConstants.sConflictScenario = "False";
                }
            }
            if (localStorage.getItem("oPageTwoData")) {
                oSavedPageTwoData = JSON.parse(localStorage.getItem("oPageTwoData"));
                //// Load Page 2 data if iLivePage  = 3
                if ("3" === iCurrPage) {
                    iAssignPermLength = oSavedPageTwoData.AssignPermission.length;
                    oJQueryObjects.$ConflictCheckConducted.click();
                    oJQueryObjects.$ConflictConductedBy.val(oSavedPageTwoData.ConfCheckCondBy.Text);
                    if (oSavedPageTwoData.ConfCheckCondBy.DataResolved) {
                        oJQueryObjects.$ConflictConductedBy.attr("data-resolved", "1");
                    }
                    oJQueryObjects.$ConflictCheckDate.val(oSavedPageTwoData.CheckPerfOn);
                    if ("False" === oSavedPageTwoData.ConflictIdent && $("input[name=rdbConflictCheck][value=False]")) {
                        $("input[name=rdbConflictCheck][value=False]").click();
                    }
                    oJQueryObjects.$BlockedUsers.val(oSavedPageTwoData.BlockUsers.Text);
                    if (oSavedPageTwoData.BlockUsers.DataResolved) {
                        oJQueryObjects.$BlockedUsers.attr("data-resolved", "1");
                    }
                    //// Set oSiteUser from local storage
                    if (oSavedPageTwoData.BlockedSiteUsers) {
                        oCommonObject.oSiteUser = oSavedPageTwoData.BlockedSiteUsers;
                    }
                    oJQueryObjects.$BoolSecureMatter = oSavedPageTwoData.BoolSecureMatter;
                    if ("False" === oSavedPageTwoData.BoolSecureMatter && $("input[name=rdbSecureMatterCheck][value=False]")) {
                        $("input[name=rdbSecureMatterCheck][value=False]").click();
                    }
                    for (iCount = 0; iCount < iAssignPermLength; iCount++) {
                        iNextPerm = parseInt(iCount, 10) + 1;
                        if (iCount > 0) {
                            commonFunctions.addMorePermissions(iNextPerm);
                        }
                        $("#txtAssign" + iNextPerm).val(oSavedPageTwoData.AssignPermission[iCount].NameList);
                        if (oSavedPageTwoData.AssignPermission[iCount].DataResolved) {
                            $("#txtAssign" + iNextPerm).attr("data-resolved", "1");
                        }
                        $(".assignPerm" + iNextPerm + "").each(function (index) {
                            if (oSavedPageTwoData.AssignPermission[iCount].Role === $(this).attr("data-value")) {
                                $(this).click();
                            }
                        });
                        $(".permContent" + iNextPerm + "").each(function (index) {
                            if (oSavedPageTwoData.AssignPermission[iCount].Permission === $(this).attr("data-value")) {
                                $(this).click();
                            }
                        });
                    }
                    var oSendMailObject = $("#demo-checkbox-unselected2");
                    if (oSendMailObject.length) {
                        var bSendMailCheck = localStorage.getItem("oSendMailCheckbox") && $.parseJSON(localStorage.getItem("oSendMailCheckbox"));
                        oSendMailObject[0].checked = bSendMailCheck;
                        if (bSendMailCheck) {
                            $("#btnCreateMatter").text(oMatterProvisionConstants.Create_Notify_Button_Text);
                        } else {
                            $("#btnCreateMatter").text(oMatterProvisionConstants.Create_Button_Text);
                        }
                    }

                    var oCalenderObject = $("#demo-checkbox-unselected0");
                    if (oCalenderObject.length) {
                        oCalenderObject[0].checked = localStorage.getItem("oCalenderCheckbox") && $.parseJSON(localStorage.getItem("oCalenderCheckbox"));
                    }

                    var oRSSFeedObject = $("#demo-checkbox-unselected1");
                    if (oRSSFeedObject.length) {
                        oRSSFeedObject[0].checked = localStorage.getItem("oRSSFeedCheckbox") && $.parseJSON(localStorage.getItem("oRSSFeedCheckbox"));
                    }

                    var oTaskObject = $("#demo-checkbox-unselected3");
                    if (oTaskObject.length && "undefined" !== localStorage.getItem("oTaskCheckbox")) {
                        oTaskObject[0].checked = localStorage.getItem("oTaskCheckbox") && $.parseJSON(localStorage.getItem("oTaskCheckbox"));
                    }

                }
            }
            ////Click corresponding menu tab
            oJQueryObjects.$MenuClick[parseInt(iCurrPage, 10) - 1].click();
        }
    };

    /* Function to change the color of next and previous link in pagination bar */
    function setActivePagination(paginationMove) {
        var currentPageNo = paginationMove ? parseInt(serviceConstants.iCurrentPage + 1, 10) : parseInt(serviceConstants.iCurrentPage - 1, 10);
        var oPaginate = $(".Paginate");
        if (oPaginate && 0 < currentPageNo && 4 > currentPageNo) {  //// If oPaginate object exists and current page if between 1 to 3
            if (1 === currentPageNo) {
                oPaginate.first().removeClass("active");
                oPaginate.last().addClass("active");
            } else if (2 === currentPageNo) {
                serviceConstants.isNextClicked = true;
                oPaginate.addClass("active");
            } else if (3 === currentPageNo) {
                oPaginate.first().addClass("active");
                oPaginate.last().removeClass("active");
            }
        }
    }

    /* Function to remove team assignment row on click of delete image */
    function closePermissionRow(event) {
        if (1 < $(".assignNewPerm").length) {
            $("." + event.currentTarget.parentElement.className.split(" ")[0]).remove();
            $(".ddlPermAssignList, .ddlRoleAssignList").addClass("hide");
            $(oJQueryObjects.$ErrorPopUp).addClass("hide");
            event.stopPropagation();
        } else {
            showErrorNotification(".close", "Assign permission to at least one team member");
        }
    }

    /* Function to determine search template pop-up size */
    function popupSize() {
        "use strict";
        var iSize = $(window).width();
        iSize *= 0.2;

    }

    /* Function to close all pop up except the passed one */
    function closeAllPopupExcept(divClass) {
        "use strict";
        if ("ddlClientList" !== divClass) {
            $("#ddlClientList").addClass("hide");
            $(".ddlClientListItem").removeClass("ddListOnHover");
        }
        if ("popUpPGList" !== divClass) {
            $("#popUpPGList").addClass("hide");
        }
        // Hide assign permission role drop-down
        $(".ddlRoleAssignList").addClass("hide");
        // Hide Permission level drop-down
        $(".ddlPermAssignList").addClass("hide");
    }

    /* Function to perform operations on page load */
    $(document).ready(function () {
        /* Set current page to 4 */
        oPageConfig.currentPage = 4;
        oCommonObject.sCurrentPage = oGlobalConstants.App_Name_Provision_Matters;
        getContextualHelpData(4);
        //// Update the validation for HTML fields
        $("#txtMatterName").attr("maxlength", oGlobalConstants.Matter_Name_Max_Length);
        $("#txtMatterId").attr("maxlength", oGlobalConstants.Matter_Id_Max_Length);
        $("#txtMatterDesc").attr("maxlength", oGlobalConstants.Matter_Description_Max_Length);
        /* Display the header */
        $(".AppHeader").removeClass("hide");
        $(".matterCheckYes").prop("checked", true);
        $("input[name=rdbSecureMatterCheck]").attr("disabled", true);

        $(".ddClientIcon, .ddlClientDiv").click(function (event) {
            var $Element = $("#ddlClientList");
            var oSelectedDataValue = $(this).find("input").attr("data-value");
            var oSelectedClientID = $(this).find("input").attr("data-client-id");


            toggleThisElement($Element);

            event.stopPropagation();
        });


        $(".ddpopUpPGIcon, .popUpPGDiv").click(function (event) {
            var $Element = $("#popUpPGList");
            toggleThisElement($Element);
            event.stopPropagation();
        });


        $("#demo-checkbox-unselected2").click(function () {
            var oSendMailCheckbox = $("#demo-checkbox-unselected2")[0], checkChange;

            if ($(oSendMailCheckbox).is(":checked")) {
                localStorage.setItem("oSendMailCheckbox", true);
                checkChange = oMatterProvisionConstants.Create_Notify_Button_Text;
            } else {
                localStorage.setItem("oSendMailCheckbox", false);
                checkChange = oMatterProvisionConstants.Create_Button_Text;
            }

            $("#btnCreateMatter").html(checkChange);
        });

        $("#demo-checkbox-unselected0").click(function () {
            var oCalenderCheckbox = $("#demo-checkbox-unselected0")[0];
            changeCheckbox("oCalenderCheckbox", oCalenderCheckbox);
        });

        $("#demo-checkbox-unselected1").click(function () {
            var oRSSFeedCheckbox = $("#demo-checkbox-unselected1")[0];
            changeCheckbox("oRSSFeedCheckbox", oRSSFeedCheckbox);
        });

        $("#demo-checkbox-unselected3").click(function () {
            var oTaskCheckbox = $("#demo-checkbox-unselected3")[0];
            changeCheckbox("oTaskCheckbox", oTaskCheckbox);
        });

        function changeCheckbox(value, element) {
            if ($(element).is(":checked")) {
                localStorage.setItem(value, true);
            } else {
                localStorage.setItem(value, false);
            }
        }
        $.ajax({
            url: "filterpanel.html?ver=25.0.0.0",
            success: function (response) {
                // App switcher
                oCommonObject.getAppSwitcher(oGlobalConstants.App_Name_Provision_Matters);
                $(".appSwitch .appSwitchMenu ul li:nth-child(3) ").addClass("selected");
                $(".TryAgainLink").attr("href", "javascript:window.top.location.reload()");
                commonFunction.onLoadActions(false);
            }
        });
        commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + "/" + oCommonObject.sCurrentPage, true);
        popupSize();

        //// #region JQuery Objects binding

        oJQueryObjects.$Loading = $(".loading");
        oJQueryObjects.$ClientList = $("#ddlClientList");
        oJQueryObjects.$ClientName = $("#ddlClient");
        oJQueryObjects.$ClientId = $("#txtClientId");
        oJQueryObjects.$MatterName = $("#txtMatterName");
        oJQueryObjects.$MatterId = $("#txtMatterId");
        oJQueryObjects.$MatterDescription = $("#txtMatterDesc");
        oJQueryObjects.$ContentTypes = $("#documentTemplates > .docTemplateItem");
        oJQueryObjects.$ConflictCheckConducted = $("#chkConflictCheck");
        oJQueryObjects.$ConflictConductedBy = $("#txtConflictCheckBy");
        oJQueryObjects.$ConflictCheckDate = $("#txtConflictCheckDate");
        oJQueryObjects.$ConflictIdentified = $("input[name=rdbConflictCheck]:checked");
        oJQueryObjects.$BlockedUsers = $("#txtBlockUser");
        oJQueryObjects.$AssignPermissions = $(".assignNewPerm");
        oJQueryObjects.$CreateButton = $("#btnCreateMatter");
        oJQueryObjects.$LookUpButton = $("#btnLookup");
        oJQueryObjects.$ErrorPopUp = $(".mainArea .errorPopUp");
        oJQueryObjects.$LoadingLookUp = $(".loadingLookUp");
        oJQueryObjects.$MenuClick = $(".menuText");
        oJQueryObjects.$PracticeGroup = $(".popUpPG");
        oJQueryObjects.$PracticeGroupList = $("#popUpPGList");
        oJQueryObjects.$Paginate = $(".Paginate");
        //// #endregion
        // #region Service calls
        commonFunctions.getClient();
        commonFunctions.getRole();
        commonFunctions.getPerm();
        commonFunctions.fetchContentTypes();
        //// #endregion
        // #region Bindings
        // #region jQuery UI auto-complete
        oCommonObject.bindAutocomplete("#txtConflictCheckBy", false); //// Bind the jQuery UI auto-complete to single user text field
        oCommonObject.bindAutocomplete("#txtBlockUser", true); //// Bind the jQuery UI auto-complete to multi user text field
        oCommonObject.bindAutocomplete("#txtAssign1", true); //// Bind the jQuery UI auto-complete to multi user text field
        // #endregion
        oJQueryObjects.$MenuClick.click(function (e) { onMenuTextClick(e); });
        oJQueryObjects.$CreateButton.click(function () {
            $(".ms-ChoiceField-field").addClass("disable-checkbox");
            $(this).attr("disabled", "disabled");
            $(".loading").removeClass("hide");
            serviceConstants.bMatterLandingPage = true;
            serviceConstants.bErrorOccured = false;
            serviceConstants.bViewCreationFailed = false;
            commonFunctions.createMatter();
        });
        oJQueryObjects.$LookUpButton.click(function () {
            showPopup();
        });
        oJQueryObjects.$Paginate.click(function (e) {
            var toMove = $(this).attr("data-move");
            toMove = parseInt(toMove, 10);
            setActivePagination(toMove);
            var iPageToClick = toMove ? parseInt(serviceConstants.iCurrentPage + 1, 10) : parseInt(serviceConstants.iCurrentPage - 1, 10);
            $("span[data-section-num=" + iPageToClick + "]").click();
            e.stopPropagation();
        });
        $("input[name=rdbConflictCheck]").click(function (e) { onConflictIdentifiedChange(e); });
        oJQueryObjects.$ConflictCheckConducted.click();
        $(document).on("click", ".notificationContainer .notification .closeNotification", function () {
            $(this).parent().remove();
        });
        $(document).on("keypress, input", "#txtConflictCheckBy, #txtBlockUser, input[id^=txtAssign]", function () {
            $(this).removeAttr("data-resolved");
        });
        $("#addMoreLink").click(function () {
            oJQueryObjects.$AssignPermissions = $(".assignNewPerm");
            var iAssignPermCount = oJQueryObjects.$AssignPermissions.length;
            commonFunctions.addMorePermissions(parseInt(iAssignPermCount, 10) + 1);
        });
        oJQueryObjects.$ConflictCheckDate.datepicker({ maxDate: "+0d" });
        //// hide the error tooltip
        $(document).on("click", ".content, .roleValue", function () {
            if (serviceConstants.bCloseErrorPopup) {
                oJQueryObjects.$ErrorPopUp.addClass("hide");
                $("div input:text , textarea , select").css({ "border": "1px #c8c8c8 solid" });
            }
        });
        $(document).on("click", ".close", function (e) {
            closePermissionRow(e);
        });
        $("#txtMatterName").focusout(function () {
            checkValidMatterName();
            serviceConstants.isValidClient = true;
        });
        $(window).on("resize", function () {
            "use strict";
            resizeErrorPopup();
            $(".errorPopUp").addClass("hide");
            $.datepicker._hideDatepicker();
            $("#hasDatepicker").blur();
            popupSize();
        });
        $(document).on("click", document, function (event) {
            "use strict";
            if ("ddClientIcon" === $($(event.target)[0]).attr("class") || "ddlClient" === $(event.target)[0].id) {
                closeAllPopupExcept("ddlClientList");
            } else if ($($(event.target)[0]).hasClass("popUpPGDiv")) {
                closeAllPopupExcept("popUpPGList");
            } else {
                closeAllPopupExcept("");
            }
        });
        //// #endregion
        // #region Search By Typing
        //// Search by typing for Area of Law
        $(document).on("input", ".popUpMatterDescription .popUpMDTextArea", function () { searchByTyping(".popUpMD .popUpMDContent", $(this).val()); });
        //// Search by typing for Sub Area of Law
        $(document).on("input", ".popUpSubAreaOfLaw .popUpMDTextArea", function () { searchByTyping(".popUpSAL .popUpSALContent", $(this).val()); });
        //// #endregion
        // #region Maintain State of App
        if (localStorage.getItem("iLivePage")) {
            ////Wait till data is fetched from SharePoint
            var iInterval = setInterval(function () {
                if (serviceConstants.bClient && serviceConstants.bRole && serviceConstants.bPermission && serviceConstants.bContentType) {
                    clearInterval(iInterval);
                    serviceConstants.bClient = serviceConstants.bRole = serviceConstants.bPermission = serviceConstants.bContentType = false;
                    var currentPage = localStorage.iLivePage;
                    if ("2" === currentPage) {
                        localStorage.setItem("IsPageLoad", true);
                    } else {
                        localStorage.setItem("IsPageLoad", false);
                    }
                    ////Fill data from local storage
                    commonFunctions.loadAppStateData(currentPage);
                }
            }, 500);
        }
        //// #endregion
        getMatterGUID();
    });
    // #region Return exposed Functions and Variables
    return ({
        getTermStoreData: function () {
            return serviceConstants.oTermStoreJson;
        }, //// Function which will be called through popUp.js
        showErrorNotification: function (sElement, sMsg) {
            return showErrorNotification(sElement, sMsg);
        }, //// Function which will be called through common.js
        sConflictScenario: function () {
            return serviceConstants.sConflictScenario;
        }
    });
    // #endregion
}());

