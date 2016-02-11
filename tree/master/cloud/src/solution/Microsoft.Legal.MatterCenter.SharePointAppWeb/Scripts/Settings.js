/// <disable>JS1003,JS2005,JS2016,JS2023,JS2024,JS2026,JS2027,JS2031,JS2032,JS2052,JS2053,JS2064,JS2073,JS2074,JS2076,JS3054,JS3056,JS3057,JS3058,JS3085,JS3092,JS3116</disable>
//// Above exclusion list is signed off as per the Modern Cop Exclusion list.

// #region Global Declaration
/* Object to store all the global constants */
var serviceConstants = {
    sCentralRepositoryLocation: oGlobalConstants.Central_Repository_Url,
    oTermStoreJson: {},
    bRole: false,
    bPermission: false,
    bContentType: false,
    sPracticeGroupList: "",
    sAreaOfLawList: "",
    sSubAreaOfLawList: "",
    oMandatoryRoles: [],
    oMandatoryRoleNames: [],
    arrReadOnlyUsers: [],
    iLastRowNumber: 2,
    bIsRoleProcessed: false,
    bIsPermissionProcessed: false,
    bIsContentTypeProcessed: false,
    bIsConfigurationsRetrieved: false,
    bIsContentTypeValuesSet: false,
    bIsPermissionValuesSet: false,
    clientUrl: "",
    clientName: "",
    bClientMode: false,
    sItemModifiedDate: "",
    sErrorModifiedDate: "0"
},
/* Object to store all the JQuery binding elements used */
oJQueryObjects = {
    $Loading: "",
    $LoadingLookUp: "",
    $ContentTypes: "",
    $AssignPermissions: "",
    $ErrorPopUp: "",
    $PracticeGroup: "",
},
commonFunctions = {};

var serviceVariables = {
    oTermStoreData: {},
    oMatterConfigurations: {}
};

/* Function to get the screen width */
function getWidth() {
    "use strict";
    var iWidth = 0;
    if (self.innerHeight) {
        iWidth = self.innerWidth;
    } else if (document.documentElement && document.documentElement.clientHeight) {
        iWidth = document.documentElement.clientWidth;
    } else if (document.body) {
        iWidth = document.body.clientWidth;
    }
    return iWidth;
}

// Sets the footer at the end of window
function setHeight() {
    "use strict";
    var oDifference = 100;
    var oFooterPosition = $(".mainContainer").height() + oDifference + 110;
    if ($(window).height() >= oFooterPosition) {
        $("#footer").css("margin-top", $(window).height() - oFooterPosition);
    }
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

// Function is used to read parameter from the URL
function getParameterByName(name) {
    "use strict";
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(decodeURIComponent(location.search));
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

/* Function to remove team assignment row on click of delete image */
function closePermissionRow(event) {
    "use strict";
    closeAllPopupExcept("");
    if (1 < $(".assignNewPerm").length) {
        $("." + event.currentTarget.parentElement.className.split(" ")[0]).remove();
        $(oJQueryObjects.$ErrorPopUp).addClass("hide");
        event.stopPropagation();
    }
    setHeight();
}

/* Function to sort drop downs */
function sortDropDownListByText() {
    "use strict";
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

/* Function for search by typing on LookUp Pop up */
function searchByTyping(sSelector, textValue) {
    "use strict";
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

// Function is used to set the text of fields with that read from resource file
function setTextForFieldsOnPage(bIsClientMode) {
    "use strict";
    if (oSettingsConstants) {
        if (!bIsClientMode) {
            $(".settingLink .clientLinks").attr("href", oGlobalConstants.Site_Url + oSettingsConstants.Settings_Page_Url).attr("target", "_parent");
            $(".pageDescription").text(oSettingsConstants.Page_Default_Description);
            $(".defaultSection .titleSection").text(oSettingsConstants.Default_Values_Section_Title);
            $(".defaultSection .descriptionSection").text(oSettingsConstants.Default_Values_Section_Description);
            $("#matterId").text(oSettingsConstants.Matter_Id_Default_Title);
            $("#matterIdSection .labelSection .contentDescription").text(oSettingsConstants.Matter_Id_Default_Description);
            $("#matterName").text(oSettingsConstants.Matter_Name_Default_Title);
            $("#matterNameSection .labelSection .contentDescription").text(oSettingsConstants.Matter_Name_Default_Description);
            $("#defaultMatterTypeDescription").text(oSettingsConstants.Matter_Type_Default_Title);
            $("#restrictAccessTitle").text(oSettingsConstants.Restricted_Access_Default_Title);
            $("#restrictAccessSection .contentDescription").text(oSettingsConstants.Restricted_Access_Description);
            $("#assignPermissionsTitle").text(oSettingsConstants.Assign_Team_Permission_Title);
            $("#labelName").text(oSettingsConstants.Name_Column_Text);
            $("#labelRole").text(oSettingsConstants.Role_Column_Text);
            $("#labelPermissionLevel").text(oSettingsConstants.Permission_Column_Text);
            $("#addMoreLink").text(oSettingsConstants.Add_More_Link);
            $("#calendarSection .labelSection").text(oSettingsConstants.Include_Calendar_Default_Title);
            $("#rssSection .labelSection").text(oSettingsConstants.Include_RSS_Feeds_Default_Title);
            $("#emailNotificationSection .labelSection").text(oSettingsConstants.Include_Email_Notification_Default_Title);
            $("#tasksSection .labelSection").text(oSettingsConstants.Include_Tasks_Title);
            $("#matterOptionsSection .titleSection").text(oSettingsConstants.Set_Matter_Options_Default_Title);
            $("#matterOptionsSection .descriptionSection").text(oSettingsConstants.Set_Matter_Options_Default_Description);
            $("#matterDescriptionTitle").text(oSettingsConstants.Matter_Description_Required_Title);
            $("#matterDescriptionSection .contentDescription").text(oSettingsConstants.Matter_Description_Field_Description);
            $("#conflictCheckTitle").text(oSettingsConstants.Matter_Conflict_Check_Title);
            $("#conflictCheckSection .contentDescription").text(oSettingsConstants.Matter_Conflict_Check_Description);
            $("#saveButton").text(oSettingsConstants.Save_Button_Text).attr("title", oSettingsConstants.Save_Button_Text);
            $("#cancelButton").text(oSettingsConstants.Cancel_Button_Text).attr("title", oSettingsConstants.Cancel_Button_Text);
            $("#successMessage").html(oSettingsConstants.Save_Success_Message);
            $("#hideMatterTypeSection").html(oSettingsConstants.Matter_Type_Hidden_Message);
            $("#matterTypeTitle").text(oSettingsConstants.Matter_Type_Column_Title).attr("title", oSettingsConstants.Matter_Type_Column_Title);
            $(".searchByTyping").text(oSettingsConstants.Search_By_Typing_Placeholder).attr("placeholder", oSettingsConstants.Search_By_Typing_Placeholder);
            $("#subAreaOfLawTitle").text(oSettingsConstants.Sub_Area_Law_Text).attr("title", oSettingsConstants.Sub_Area_Law_Text);
            $("#areaOfLawTitle").text(oSettingsConstants.Area_Law_Text).attr("title", oSettingsConstants.Area_Law_Text);
            $("#practiceGroupTitle").text(oSettingsConstants.Select_Practice_Group).attr("title", oSettingsConstants.Select_Practice_Group);
            $(".iconForward").attr("title", oSettingsConstants.Icon_Title_Forward);
            $(".iconBack").attr("title", oSettingsConstants.Icon_Title_Backward);
            $("#imgDelete").attr("title", oSettingsConstants.Delete_Icon_Title);
            $("#showMatterSection").html(oSettingsConstants.Show_Matter_Section);
        } else {
            $("#clientContainer .clientName").text(oSettingsConstants.Clients_Header);
            $("#clientContainer .clientDescription").text(oSettingsConstants.Clients_View_Message);
            $("#clientContainer .tableContainer").text(oSettingsConstants.Client_Name);
        }
    }
}

/* Function to toggle element */
function toggleThisElement($Element) {
    "use strict";
    if ($Element.is(":visible")) {
        $Element.addClass("hide");
    } else {
        $Element.removeClass("hide");
    }
}

/* Function to populate pop-up data */
function populateMatterTypes() {
    "use strict";
    var iCount = 0, iLength = serviceConstants.oTermStoreJson.PGTerms.length, sFolderNames;
    localStorage.removeItem("iSelectedPG");
    localStorage.removeItem("iSelectedAOL");

    for (iCount = 0; iCount < iLength; iCount++) {
        sFolderNames = serviceConstants.oTermStoreJson.PGTerms[iCount].FolderNames ? serviceConstants.oTermStoreJson.PGTerms[iCount].FolderNames : "";
        oJQueryObjects.$PracticeGroupList.append("<div class='popUpOptions' title='" + serviceConstants.oTermStoreJson.PGTerms[iCount].TermName + "' data-value='" + iCount + "' data-folderNamesPG='" + sFolderNames + "' >" + serviceConstants.oTermStoreJson.PGTerms[iCount].TermName + "</div>");
    }

    $(".popUpOptions").mouseover(function () {
        $(this).addClass("ddListOnHover");
    }).mouseout(function () {
        $(this).removeClass("ddListOnHover");
    });

    $(".popUpOptions").click(function () {
        $(".popUpMDTextArea").val("");
        var selectedPG = this.innerHTML, $Element = $("#popUpPG");
        $Element.val(selectedPG).attr("data-value", $(this).attr("data-value")).attr("data-foldernamespg", $(this).attr("data-foldernamespg"));
        $("#popUpPGList").addClass("hide");
        if ($.isEmptyObject(serviceVariables.oTermStoreData)) {
            serviceVariables.oTermStoreData = serviceConstants.oTermStoreJson;
        }
        var oAOL = $(".popUpMD"), sFolderNames;
        $(".popUpMDContent, .popUpSALContent").remove();
        localStorage.iSelectedPG = $(".popUpPG").attr("data-value");
        for (var jIterator = 0; jIterator < serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms.length; jIterator++) {
            // Addition of folder names
            sFolderNames = serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[jIterator].FolderNames ? serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[jIterator].FolderNames : "";
            oAOL.append("<div class='popUpMDContent' data-value=" + jIterator + " data-folderNamesAOL=" + sFolderNames + " >" + serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[jIterator].TermName + "</div>");
        }

        $(".popUpMD .popUpMDContent:first-child").click();
    });

    $($(".popUpOptions")[0]).click();
    oJQueryObjects.$LoadingLookUp.addClass("hide");
    sortDropDownListByText();
}

/* Functions for retrieving roles */
commonFunctions.getRoleSuccess = function (result) {
    "use strict";
    var $RoleObject = $("#ddlRoleAssignList1");
    $RoleObject.empty();
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
            closeAllPopupExcept("");
            var $Element = $RoleObject;
            toggleThisElement($Element);
            var windowWidth = getWidth();
            $RoleObject.css("top", parseInt($(this).offset().top, 10) + $(this).height() - 10);
            $RoleObject.css("left", parseInt($(this).position().left, 10) + 1);
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
        serviceConstants.bIsRoleProcessed = true;
        if (serviceConstants.bIsPermissionProcessed && serviceConstants.bIsConfigurationsRetrieved) {
            setDefaultPermissions();
        }
    } else {
        if (arrResults.code && "string" === typeof arrResults.code && (-1 < arrResults.code.indexOf("#|#"))) {
            showCommonErrorPopUp(arrResults.code);
        } else {
            showErrorNotification("#matterNameSection input", arrResults.value);
            $(window).scrollTop(0);
        }
    }
    serviceConstants.bRole = true;
};
commonFunctions.getRoleFailure = function (result) {
    "use strict";
    return false;
};
commonFunctions.getRole = function () {
    "use strict";
    var sClientUrl = serviceConstants.sCentralRepositoryLocation
        , matterDetails = {
            "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": {
                "Url": sClientUrl
            }
        };

    oCommonObject.callProvisioningService("GetRoleData", matterDetails, commonFunctions.getRoleSuccess, commonFunctions.getRoleFailure);
};

// Assigns the default permissions from the configurations
function setDefaultPermissions() {
    "use strict";
    // Check if permissions values are not set
    if (!serviceConstants.bIsPermissionValuesSet) {
        $(".loadingIconAssignPermission").addClass("hide");
        serviceConstants.bIsPermissionValuesSet = !serviceConstants.bIsPermissionValuesSet;
        if (serviceVariables.oMatterConfigurations.MatterUsers) {
            $(".assignNewPerm").remove();

            var arrUsers = serviceVariables.oMatterConfigurations.MatterUsers.split("$|$")
, arrUserEmails = []
            , arrRoles = serviceVariables.oMatterConfigurations.MatterRoles.split("$|$")
            , arrPermissions = serviceVariables.oMatterConfigurations.MatterPermissions.split("$|$")
            , count = 1
            , iCounter
            , iCount
            , sEmail = ""
            , oEmailRegex = new RegExp(oGlobalConstants.Email_Validation_Regex);
            if (serviceVariables.oMatterConfigurations.MatterUserEmails && "" !== serviceVariables.oMatterConfigurations.MatterUserEmails) {
                arrUserEmails = serviceVariables.oMatterConfigurations.MatterUserEmails.split("$|$");
            }
            for (iCounter = 0; iCounter < arrUsers.length; iCounter++) {
                commonFunctions.addMorePermissions(count);
                var userName = "#txtAssign" + count;
                if (arrUsers && 0 < arrUsers.length && arrUsers[iCounter]) {
                    if (arrUserEmails && 0 < arrUserEmails.length && arrUserEmails[iCounter]) {
                        var arrAllUsers = arrUsers[iCounter].split(";"), arrAllUserEmails = arrUserEmails[iCounter].split(";");
                        for (iCount = 0; iCount < arrAllUsers.length; iCount++) {
                            if (arrAllUsers[iCount] && "" !== arrAllUsers[iCount].trim()) {
                                sEmail = arrAllUserEmails[iCount];
                                if (oEmailRegex.test(sEmail)) {
                                    $(userName)[0].value += (trimEndChar(arrAllUsers[iCount], ";") + " (" + arrAllUserEmails[iCount] + ");");
                                } else {
                                    $(userName)[0].value += (trimEndChar(arrAllUsers[iCount], ";") + ";");
                                }
                            }
                        }
                    } else {
                        $(userName).val(trimEndChar(arrUsers[iCounter], ";") + ";");
                    }
                } else {
                    $(userName).val("");
                }
                oCommonObject.bindAutocomplete(userName, true);
                count++;
            }
            if (serviceVariables.oMatterConfigurations.MatterPermissions) {
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
            if (serviceVariables.oMatterConfigurations.MatterRoles) {
                count = 1;
                $.each(arrPermissions, function (key, value) {
                    var permissions = "#ddlPermAssign" + count;
                    count++;
                    $(permissions).val(value).attr("data-value", value);
                });
            }
        }
        enableSaveButton();
        setHeight();
    }
}

// Sets the value in Matter Type section from the value in the Configurations
function setDefaultMatterTypes() {
    "use strict";
    // Check if the matter types values are not set
    if (!serviceConstants.bIsContentTypeValuesSet) {
        serviceConstants.bIsContentTypeValuesSet = !serviceConstants.bIsContentTypeValuesSet;
        if (serviceVariables.oMatterConfigurations.MatterTypes && serviceVariables.oMatterConfigurations.MatterAreaofLaw && serviceVariables.oMatterConfigurations.MatterPracticeGroup) {
            var arrMatterType = serviceVariables.oMatterConfigurations.MatterTypes.split("$|$")
                    , arrAreaofLaw = serviceVariables.oMatterConfigurations.MatterAreaofLaw.split("$|$")
                    , arrPracticeGroup = serviceVariables.oMatterConfigurations.MatterPracticeGroup.split("$|$")
                    , count = 0
                    , iCounter = 0;
            $.each(arrMatterType, function (key, matterType) {
                $.each(serviceConstants.oTermStoreJson.PGTerms, function (key, pgTerms) {
                    $.each(pgTerms.AreaTerms, function (key, areaTerms) {
                        $.each(areaTerms.SubareaTerms, function (key, subAreaTerms) {
                            if ($.trim(subAreaTerms.TermName) === $.trim(matterType) && $.trim(areaTerms.TermName) === $.trim(arrAreaofLaw[iCounter]) && $.trim(pgTerms.TermName) === $.trim(arrPracticeGroup[iCounter])) {
                                var oContentTypeContainer = $(".popDT");
                                var sDataDocumentTemplate = subAreaTerms.DocumentTemplates;
                                var sDataAssociatedDocumentTemplate = subAreaTerms.DocumentTemplateNames;
                                var sSelectedPracticeGroup = pgTerms.TermName;
                                var sSelectedPracticeGroupFolderStructure = pgTerms.FolderNames;
                                // Fetch the folder structure from the Practice Group, Area of Law and Sub Area of Law levels

                                var sSelectedAreaOfLaw = areaTerms.TermName;
                                var sSelectedAreaOfLawFolderStructure = areaTerms.FolderNames;
                                var sSelectedSubAreaOfLawFolderStructure = subAreaTerms.FolderNames;
                                var sSelectedSubAreaOfLawIsNofolderStructurePresent = subAreaTerms.IsNoFolderStructurePresent;
                                var iFoldersCount = 0;
                                if (sDataDocumentTemplate) {
                                    iFoldersCount = sDataAssociatedDocumentTemplate.split(";").length;
                                }
                                if ($.trim(subAreaTerms.TermName) === $.trim(serviceVariables.oMatterConfigurations.DefaultMatterType)) {
                                    oContentTypeContainer.append("<div class='popUpDTContent popUpSelected' data-value= '0' data-document-template='" + sDataDocumentTemplate + "' data-associated-document-template='" + sDataAssociatedDocumentTemplate + "' data-practicegroup='" + sSelectedPracticeGroup + "' data-areaoflaw='" + sSelectedAreaOfLaw + "' data-foldernamespg='" + sSelectedPracticeGroupFolderStructure + "' data-foldernamesaol='" + sSelectedSubAreaOfLawFolderStructure + "' data-foldernamessal='" + sSelectedSubAreaOfLawFolderStructure + "' data-isnofolderstructurepresent='" + sSelectedSubAreaOfLawIsNofolderStructurePresent + "' >" + $.trim(matterType) + " (" + iFoldersCount + ")</div>");
                                } else {
                                    oContentTypeContainer.append("<div class='popUpDTContent' data-value='" + count + "' data-document-template='" + sDataDocumentTemplate + "' data-associated-document-template='" + sDataAssociatedDocumentTemplate + "' data-practicegroup='" + sSelectedPracticeGroup + "' data-areaoflaw='" + sSelectedAreaOfLaw + "' data-foldernamespg='" + sSelectedPracticeGroupFolderStructure + "' data-foldernamesaol='" + sSelectedSubAreaOfLawFolderStructure + "' data-foldernamessal='" + sSelectedSubAreaOfLawFolderStructure + "' data-isnofolderstructurepresent='" + sSelectedSubAreaOfLawIsNofolderStructurePresent + "' >" + $.trim(matterType) + " (" + iFoldersCount + ")</div>");
                                    count++;
                                }
                            }
                        });
                    });
                });
                iCounter++;
            });
        }
        $(".loadingIconContentType").addClass("hide");
        enableSaveButton();
    }
}

// Enables the save button when all the values are loaded
function enableSaveButton() {
    "use strict";
    if (serviceConstants.bIsConfigurationsRetrieved && serviceConstants.bIsContentTypeValuesSet && serviceConstants.bIsPermissionValuesSet) {
        $("#saveButton").removeAttr("disabled");
    }
}

// Navigate to clients page or dashboard page
function navigateToPage(bClient) {
    "use strict";
    var sClientUrl = "";
    if (bClient) {
        sClientUrl = oGlobalConstants.Site_Url + oSettingsConstants.Settings_Page_Url;
    } else {
        sClientUrl = oGlobalConstants.Site_Url + oSettingsConstants.WebDashboard_Page_Url;
    }
    window.open(sClientUrl, "_parent");
}

/* Functions for retrieving permissions */
commonFunctions.getPermSuccess = function (result) {
    "use strict";
    var $PermissionObject = $("#ddlPermAssignList1");
    $PermissionObject.empty();
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
            var windowWidth = getWidth();
            $PermissionObject.css("top", parseInt($(this).offset().top, 10) + $(this).height() - 10);
            $PermissionObject.css("left", parseInt($(this).position().left, 10) + 1);
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
        serviceConstants.bIsPermissionProcessed = true;
        if (serviceConstants.bIsRoleProcessed && serviceConstants.bIsConfigurationsRetrieved) {
            setDefaultPermissions();
        }
    } else {
        if (arrResults.code && "string" === typeof arrResults.code && (-1 < arrResults.code.indexOf("#|#"))) {
            showCommonErrorPopUp(arrResults.code);
        } else {
            showErrorNotification("#matterNameSection input", arrResults.value);
            $(window).scrollTop(0);
        }
    }
    serviceConstants.bPermission = true;
};
commonFunctions.getPermFailure = function (result) {
    "use strict";
    return false;
};
commonFunctions.getPerm = function () {
    "use strict";
    var sClientUrl = serviceConstants.sCentralRepositoryLocation
        , matterDetails = {
            "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": {
                "Url": sClientUrl
            }
        };

    oCommonObject.callProvisioningService("GetPermissionLevels", matterDetails, commonFunctions.getPermSuccess, commonFunctions.getpermFailure);
};

// Function is used to set fields based on configurations retrieved from service
function setFields(oMatterConfigurations) {
    "use strict";
    if (oMatterConfigurations.DefaultMatterName) {
        $("#matterNameSection .ms-TextField-field").val(oMatterConfigurations.DefaultMatterName);
    }

    if (oMatterConfigurations.DefaultMatterId) {
        $("#matterIdSection .ms-TextField-field").val(oMatterConfigurations.DefaultMatterId);
    }
    makeFieldSelected("#includeCalendarTrue", "#includeCalendarFalse", oMatterConfigurations.IsCalendarSelected);
    makeFieldSelected("#includeRSSTrue", "#includeRSSFalse", oMatterConfigurations.IsRSSSelected);
    makeFieldSelected("#includeEmailTrue", "#includeEmailFalse", oMatterConfigurations.IsEmailOptionSelected);
    makeFieldSelected("#matterRequiredTrue", "#matterRequiredFalse", oMatterConfigurations.IsMatterDescriptionMandatory);
    makeFieldSelected("#matterConflictTrue", "#matterConflictFalse", oMatterConfigurations.IsConflictCheck);
    makeFieldSelected("#assignTeamTrue", "#assignTeamFalse", oMatterConfigurations.IsRestrictedAccessSelected);
    makeFieldSelected("#includeTasksTrue", "#includeTasksFalse", oMatterConfigurations.IsTaskSelected);
    if (serviceConstants.bIsPermissionProcessed && serviceConstants.bIsRoleProcessed) {
        setDefaultPermissions();
    }

    if (serviceConstants.bIsContentTypeProcessed) {
        setDefaultMatterTypes();
    }
}

// Make the field selected
function makeFieldSelected(sOptionYesElementId, sOptionNoElementId, bIsSelected) {
    "use strict";
    if (bIsSelected) {
        $(sOptionYesElementId).prop("checked", "checked");
    } else {
        $(sOptionNoElementId).prop("checked", "checked");
    }
}

/* Functions for retrieving permissions */
commonFunctions.getConfigurationsSuccess = function (result) {
    "use strict";
    if (null != result) {
        var arrResult = result.split("|$|");
        if (0 < arrResult.length) {
            if (serviceConstants.sErrorModifiedDate == arrResult[1]) {  // New configurations created                
                serviceConstants.sItemModifiedDate = serviceConstants.sErrorModifiedDate;
            } else {
                serviceConstants.sItemModifiedDate = arrResult[1];
            }
            var matterConfigurations = JSON.parse(arrResult[0]);
            setHeight();
            serviceConstants.bIsConfigurationsRetrieved = true;
            serviceVariables.oMatterConfigurations = JSON.parse(arrResult[0]);
            if (!matterConfigurations.code) {
                setFields(matterConfigurations);
                enableSaveButton();
            } else {
                if (matterConfigurations.code && "string" === typeof matterConfigurations.code && (-1 < matterConfigurations.code.indexOf("#|#"))) {
                    showCommonErrorPopUp(matterConfigurations.code);
                } else {
                    showErrorNotification("#matterNameSection input", matterConfigurations.value);
                    $(window).scrollTop(0);
                }
            }
        }
    }
};
commonFunctions.getConfigurationsFailure = function (result) {
    "use strict";
    serviceConstants.bIsConfigurationsRetrieved = true;
    return false;
};
commonFunctions.getConfigurations = function (sClientUrl) {
    "use strict";
    var matterDetails = {
        "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "siteCollectionPath": sClientUrl
    };

    oCommonObject.callProvisioningService("GetDefaultMatterConfigurations", matterDetails, commonFunctions.getConfigurationsSuccess, commonFunctions.getConfigurationsFailure);
};

/* Functions for fetching content types */
commonFunctions.fetchMatterTypesSuccess = function (result) {
    "use strict";
    serviceConstants.bContentType = true;
    serviceConstants.oTermStoreJson = JSON.parse(result);
    if (!serviceConstants.oTermStoreJson.code) {
        populateMatterTypes();
        serviceConstants.bIsContentTypeProcessed = true;
        if (serviceConstants.bIsConfigurationsRetrieved) {
            setDefaultMatterTypes();
        }
    } else {
        if (serviceConstants.oTermStoreJson.code && "string" === typeof serviceConstants.oTermStoreJson.code && (-1 < serviceConstants.oTermStoreJson.code.indexOf("#|#"))) {
            showCommonErrorPopUp(serviceConstants.oTermStoreJson.code);
        } else {
            showErrorNotification("#matterNameSection input", serviceConstants.oTermStoreJson.value);
            $(window).scrollTop(0);
        }
    }
};
commonFunctions.fetchMatterTypesFailure = function (result) {
    "use strict";
    return false;
};
commonFunctions.fetchMatterTypes = function () {
    "use strict";
    var sClientUrl = serviceConstants.sCentralRepositoryLocation
    , matterDetails = {
        "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": sClientUrl }, "details": {
            "TermGroup": oGlobalConstants.Common_Term_Set_Group_Name, "TermSetName": oGlobalConstants.Practice_Group_Term_Set_Name, "CustomPropertyName": oGlobalConstants.Practice_Group_Custom_Properties, "DocumentTemplatesName": oGlobalConstants.Sub_Area_Of_Law_Document_Templates
        }
    };
    oJQueryObjects.$LoadingLookUp.removeClass("hide");
    oCommonObject.callProvisioningService("GetTaxonomyData", matterDetails, commonFunctions.fetchMatterTypesSuccess, commonFunctions.fetchMatterTypesFailure);
};

/* Function to add more team assignment line items */
commonFunctions.addMorePermissions = function (iAssignPermCount) {
    "use strict";
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
                            + "<input type='text' class='clear inputAssignPerm teamName valueSection' id='txtAssign" + iNextAssignPermContent + "' spellcheck='true' placeholder='Enter and select names' required />"
                            + "<div class='gutter'></div>"

                            + "<div class='mediumSmallRole floatContentLeft ddlRoleAssignDiv" + iNextAssignPermContent + "'>"
                            + "<input class='assignRole assignRoleField teamRole valueSection' type='text' id='ddlRoleAssign" + iNextAssignPermContent + "' data-value='' data-mandatory='' disabled='disabled' autofocus='autofocus' />"
                            + "<img class='ddlRoleAssignIcon' id='ddlRoleAssignIcon" + iNextAssignPermContent + "' title='Dropdown icon' src='../Images/icon-combobox.png' />"
                            + "</div>"
                            + "<div id='ddlRoleAssignList" + iNextAssignPermContent + "' class='ddlRoleAssignList clear hide'>"
                            + "</div>"

                            + "<div class='gutter'></div>"
                            + "<div class='mediumSmallPermission floatContentLeft ddlPermAssignDiv" + iNextAssignPermContent + "'>"
                            + "<input class='assignPermission assignPermField teamPermission Space valueSection' type='text' id='ddlPermAssign" + iNextAssignPermContent + "' data-value='' disabled='disabled' autofocus='autofocus' />"
                            + "<img class='ddlPermAssignIcon' id='ddlPermAssignIcon" + iNextAssignPermContent + "' title='Dropdown icon' src='../Images/icon-combobox.png' />"
                            + "</div>"
                            + "<div id='ddlPermAssignList" + iNextAssignPermContent + "' class='ddlPermAssignList clear hide'>"
                            + "</div>"

                            + "<div class='gutter'></div>"
                            + "<div class='close'>"
                            + "<img class='imgDelete' alt='delete' src='../Images/icon-delete.png' title='delete'/>"
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
        var windowWidth = getWidth();
        $RoleObject.css("top", parseInt($(this).offset().top, 10) + $(this).height() - 10);
        $RoleObject.css("left", parseInt($(this).offset().left, 10));
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
        var windowWidth = getWidth();
        $PermissionObject.css("top", parseInt($(this).offset().top, 10) + $(this).height() - 10);
        $PermissionObject.css("left", parseInt($(this).offset().left, 10));
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
    setHeight();
};

/* Functions for retrieving clients */
commonFunctions.getClientSuccess = function (result) {
    "use strict";
    var arrResults = JSON.parse(result), clientItem, clientOption, iCount = 0, iLength, arrOptions = [];
    $(".loadingImage").addClass("hide");
    $(".mainContainer").removeClass("hide");
    $("#clientContainer").removeClass("hide");
    if (!arrResults.code) {
        serviceConstants.arrClients = JSON.parse(JSON.stringify(arrResults));
        iLength = serviceConstants.arrClients.ClientTerms.length;
        for (iCount = 0; iCount < iLength; iCount++) {
            clientOption = "<div class=\"defaultFontStyle clientNames\"><a class=\"clientLinks\" href=\"" + serviceConstants.arrClients.ClientTerms[iCount].Url + "/SitePages/Settings.aspx\" target=\"_parent\" title=\"" + serviceConstants.arrClients.ClientTerms[iCount].Name + "\">" + serviceConstants.arrClients.ClientTerms[iCount].Name + "</a></div>";
            arrOptions.push(clientOption);
        }

        if (0 === arrOptions.length) {
            $("#clientsList").text(oSettingsConstants.No_Clients_Message);
        } else {
            $("#clientsList").append(arrOptions);
        }
    }
    setHeight();
};
commonFunctions.getClientFailure = function (result) {
    "use strict";
    return false;
};
commonFunctions.getClient = function () {
    "use strict";
    var sClientUrl = serviceConstants.sCentralRepositoryLocation
    , matterDetails = {
        "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": { "Url": sClientUrl }, "details": {
            "TermGroup": oGlobalConstants.Common_Term_Set_Group_Name, "TermSetName": oGlobalConstants.Client_Term_Set_Name, "CustomPropertyName": oGlobalConstants.Client_Custom_Properties_Url
        }
    };
    oCommonObject.callProvisioningService("GetTaxonomyData", matterDetails, commonFunctions.getClientSuccess, commonFunctions.getClientFailure);
};

// Get configuration values from the list
function getConfigurationsFromFields() {
    "use strict";
    var oMatterConfigurations = {
        "DefaultMatterName": "",
        "DefaultMatterId": "",
        "DefaultMatterType": "",
        "MatterTypes": "",
        "MatterUsers": "",
        "MatterUserEmails": "",
        "MatterRoles": "",
        "MatterPermissions": "",
        "IsCalendarSelected": null,
        "IsRSSSelected": null,
        "IsEmailOptionSelected": null,
        "IsRestrictedAccessSelected": null,
        "IsConflictCheck": null,
        "IsMatterDescriptionMandatory": null,
        "IsTaskSelected": null,
        "MatterPracticeGroup": "",
        "MatterAreaofLaw": ""
    };

    var sMatterName = $("#matterNameSection input").val();
    if (sMatterName) {
        oMatterConfigurations.DefaultMatterName = sMatterName;
    }

    var sMatterId = $("#matterIdSection input").val();
    if (sMatterId) {
        oMatterConfigurations.DefaultMatterId = sMatterId;
    }

    oMatterConfigurations.MatterUsers = getUsers();
    oMatterConfigurations.MatterUserEmails = getUserEmails();
    if (oMatterConfigurations.MatterUsers) {
        oMatterConfigurations.MatterPermissions = getPermissionsOrRoles("ddlPermAssign");
        oMatterConfigurations.MatterRoles = getPermissionsOrRoles("ddlRoleAssign");
    }
    oMatterConfigurations = getMatterTypes(oMatterConfigurations);
    oMatterConfigurations.IsCalendarSelected = getFieldValue("#includeCalendarTrue");
    oMatterConfigurations.IsRSSSelected = getFieldValue("#includeRSSTrue");
    oMatterConfigurations.IsEmailOptionSelected = getFieldValue("#includeEmailTrue");
    oMatterConfigurations.IsRestrictedAccessSelected = getFieldValue("#assignTeamTrue");
    oMatterConfigurations.IsConflictCheck = getFieldValue("#matterConflictTrue");
    oMatterConfigurations.IsMatterDescriptionMandatory = getFieldValue("#matterRequiredTrue");
    oMatterConfigurations.IsTaskSelected = getFieldValue("#includeTasksTrue");
    return oMatterConfigurations;
}

// Gets the checked property of the checkbox
function getFieldValue(sComponentId) {
    "use strict";
    return $(sComponentId).prop("checked");
}

// Gets the matter types attributes
function getMatterTypes(oMatterConfigurations) {
    "use strict";
    var arrMatterTypes = [], arrPracticeGroup = [], arrAreaofLaw = [], sDefaultMatter = "", sMatterType = "";
    $.each($(".popDT div"), function () {
        sMatterType = $.trim($(this).text());
        sMatterType = sMatterType.substring(0, sMatterType.indexOf("(") - 1);
        arrMatterTypes.push(sMatterType);
        arrPracticeGroup.push($.trim($(this).attr("data-practicegroup")));
        arrAreaofLaw.push($.trim($(this).attr("data-areaoflaw")));
        if ($(this).hasClass("popUpSelected")) {
            sDefaultMatter = sMatterType;
        }
    });

    if (arrMatterTypes.length) {
        oMatterConfigurations.MatterTypes = arrMatterTypes.join("$|$");
        oMatterConfigurations.MatterPracticeGroup = arrPracticeGroup.join("$|$");
        oMatterConfigurations.MatterAreaofLaw = arrAreaofLaw.join("$|$");
        oMatterConfigurations.DefaultMatterType = sDefaultMatter;
    }

    return oMatterConfigurations;
}

// Gets the permission level for each of the user
function getPermissionsOrRoles(sId) {
    "use strict";
    var arrPermissionsOrRoles = [];
    $.each($("input[id^=" + sId + "]"), function () {
        arrPermissionsOrRoles.push($.trim($(this).attr("data-value")));
    });
    if (arrPermissionsOrRoles.length) {
        return arrPermissionsOrRoles.join("$|$");
    } else {
        return "";
    }
}

// Gets the list of users from the textboxes
function getUsers() {
    "use strict";
    var arrUserNames = [];
    $.each($("input[id^=txtAssign]"), function () {
        arrUserNames.push(oCommonObject.getUserName($(this).val().trim(), true));
    });
    if (arrUserNames.length) {
        return arrUserNames.join("$|$").replace(/,/g, ";");
    } else {
        return "";
    }
}

// Gets the list of users from the textboxes
function getUserEmails() {
    "use strict";
    var arrUseEmails = [];
    $.each($("input[id^=txtAssign]"), function () {
        arrUseEmails.push(oCommonObject.getUserName($(this).val().trim(), false));
    });
    if (arrUseEmails.length) {
        return arrUseEmails.join("$|$").replace(/,/g, ";");
    } else {
        return "";
    }
}

// Gets the user ids of the divs containing the User names
function getUserIds() {
    "use strict";
    var userId = [];
    $.each($("input[id^=txtAssign]"), function () {
        userId.push(this.id);
    });
    return userId;
}

// Save configurations functions
commonFunctions.saveConfigurationsSuccess = function (result) {
    "use strict";
    $(".loadingImage").addClass("hide");
    if (null != result) {
        var arrResult = result.split("|$|");
        serviceConstants.sItemModifiedDate = arrResult[1];
        var oResult = JSON.parse(arrResult[0]), controlId;
        if (!oResult.code) {
            $("#successMessage").removeClass("hide");
            setHeight();
            $(window).scrollTop($("#container").height());
        } else {
            if (oResult.code && "string" === typeof oResult.code && (-1 < oResult.code.indexOf("#|#"))) {
                showCommonErrorPopUp(oResult.code);
            } else {
                var sValidationResult = oResult.value;
                var oValidationDetails = sValidationResult.split("$|$"), sSecurityGroupRow, sSecurityGroupError;
                if (2 === oValidationDetails.length) {
                    sSecurityGroupError = oValidationDetails[0];
                    sSecurityGroupRow = oValidationDetails[1];
                }
                if ("" !== sSecurityGroupRow) {
                    controlId = "#" + sSecurityGroupRow;
                } else {
                    controlId = "#saveButton";
                    sSecurityGroupError = oSettingsConstants.Already_Edited_Message;
                }
                showErrorNotification(controlId, sSecurityGroupError);
            }
        }
    }
};
commonFunctions.saveConfigurationsFailure = function () {
    "use strict";
    return false;
};
commonFunctions.saveConfigurations = function () {
    "use strict";
    var oMatterConfigurations = getConfigurationsFromFields();
    $("#successMessage").addClass("hide");
    setHeight();
    $(".loadingImage").removeClass("hide");
    var userIds = [];
    if ("" !== oMatterConfigurations.MatterUsers) {
        userIds = getUserIds();
    }
    var sClientUrl = serviceConstants.sCentralRepositoryLocation, clientUrl = serviceConstants.clientUrl
    , matterDetails = {
        "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "siteCollectionPath": clientUrl, "matterConfigurations": oMatterConfigurations, "userId": userIds, "cachedItemModifiedDate": serviceConstants.sItemModifiedDate
    };

    oCommonObject.callProvisioningService("SaveMatterConfigurations", matterDetails, commonFunctions.saveConfigurationsSuccess, commonFunctions.saveConfigurationsFailure);
};
/* Function to display error tool tip */
function showErrorNotification(sElement, sMsg) {
    "use strict";
    var windowWidth = getWidth();
    var removeTop = 67, removeLeft = 115;
    var posLeft = "50px";
    var triangleTopPos = 42;
    var errorBoxHeight = 55;
    var errorBoxTop = 7;
    var errorBoxLeft = 24;
    var oRoleObject = $(sElement);
    if (oRoleObject) {
        var iLeftPos = oRoleObject.offset().left
        , iTopPos = oRoleObject.offset().top
        , iCurrentWidth = oRoleObject.outerWidth();
        iLeftPos = parseInt(iLeftPos, 10) + parseInt(iCurrentWidth, 10) - 20;
        iTopPos = parseInt(iTopPos, 10) - 20;
        $("#container .errorPopUp").css("left", iLeftPos - removeLeft).css("top", iTopPos - removeTop).removeClass("hide").find(".errText").text(sMsg);
        $("#container .errTriangleBorder").css("left", posLeft);
        $("#container .errTriangle").css("left", posLeft);
        if ("#saveButton" !== sElement) {
            $("#container .errTriangleBorder").css("top", "calc(50% - -" + (triangleTopPos - 1) + "px)");
            $("#container .errTriangle").css("top", "calc(50% - -" + (triangleTopPos) + "px)");
        } else {
            $("#container .errTriangleBorder").css("top", "66px");
            $("#container .errTriangle").css("top", "67px");
        }
        $("#container .errText").css("min-height", errorBoxHeight + "px").css("top", errorBoxTop + "px").css("left", errorBoxLeft + "px");
        oRoleObject.addClass("redBorder");
        $("#container .errorPopup").removeClass("hide");
    }
}
$("#searchMatterTab").click(function () {
    "use strict";
    commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oGlobalConstants.App_Name_Settings_Page + oGlobalConstants.View_Matter, true);
    window.top.parent.location.href = oGlobalConstants.Site_Url + oGlobalConstants.TenantWebDashboard + "?section=1";
});

$("#searchDocumentTab").click(function () {
    "use strict";
    commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oGlobalConstants.App_Name_Settings_Page + oGlobalConstants.View_Document, true);
    window.top.parent.location.href = oGlobalConstants.Site_Url + oGlobalConstants.TenantWebDashboard + "?section=2";
});
// Document ready function
$(document).ready(function () {
    "use strict";
    oCommonObject.sCurrentPage = oGlobalConstants.App_Name_Settings_Page;
    commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oCommonObject.sCurrentPage + "/" + oCommonObject.sCurrentPage, true);
    var bIsClientMode = true;
    var clientDetails = getParameterByName("clientdetails");
    $("#MasterPageContainer").removeClass("MasterPageContent");
    if ("" !== clientDetails) {
        bIsClientMode = false;
    }
    if (!bIsClientMode) {
        var clientUrl = decodeURIComponent(clientDetails.split("$|$")[0]), clientName = "";
        if (2 === clientDetails.split("$|$").length) {
            clientName = decodeURIComponent(clientDetails.split("$|$")[1]);
        }
        setTextForFieldsOnPage(bIsClientMode);
        serviceConstants.clientUrl = clientUrl;
        $(".loadingImage").addClass("hide");
        $(".mainContainer").removeClass("hide");
        $("#container").removeClass("hide");
        setHeight();
        $(".ddpopUpPGIcon, .popUpPGDiv").click(function (event) {
            var $Element = $("#popUpPGList");
            toggleThisElement($Element);
            event.stopPropagation();
        });
        if ("" !== clientName) {
            $(".clientName").text(oSettingsConstants.Client_Name_PlaceHolder.replace("{0}", clientName));
        } else {
            $(".clientName").text(oSettingsConstants.Client_Name_PlaceHolder.replace("{0}", "Client"));
        }
        oJQueryObjects.$ContentTypes = $("#documentTemplates > .docTemplateItem");
        oJQueryObjects.$AssignPermissions = $(".assignNewPerm");
        oJQueryObjects.$ErrorPopUp = $(".mainArea .errorPopUp");
        oJQueryObjects.$LoadingLookUp = $(".loadingLookUp");
        oJQueryObjects.$PracticeGroup = $(".popUpPG");
        oJQueryObjects.$PracticeGroupList = $("#popUpPGList");

        // #region Service calls    
        commonFunctions.getConfigurations(clientUrl);
        commonFunctions.getRole();
        commonFunctions.fetchMatterTypes();
        commonFunctions.getPerm();
        oCommonObject.bindAutocomplete("#txtAssign1", true); //// Bind the jQuery UI auto-complete to multi user text field

        // #region Search By Typing
        //// Search by typing for Area of Law
        $(document).on("input", ".popUpMatterDescription .popUpMDTextArea", function () { searchByTyping(".popUpMD .popUpMDContent", $(this).val()); });
        //// Search by typing for Sub Area of Law
        $(document).on("input", ".popUpSubAreaOfLaw .popUpMDTextArea", function () { searchByTyping(".popUpSAL .popUpSALContent", $(this).val()); });
        //// #endregion

        $(document).on("click", ".popUpSALContent", function () {
            $(".popUpSALContent").removeClass("popUpSelected");
            $(this).addClass("popUpSelected");
        });

        $(document).on("click", ".popUpDTContent", function () {
            $(".popUpDTContent").removeClass("popUpSelected");
            $(this).addClass("popUpSelected");
        });

        $(".popUpLeftClick").click(function () {
            var oSelected = $(".popUpSAL .popUpSelected");
            /* Add Practice Groups and Area of Laws */
            var oPopUpPG = $(".popUpPG");
            if (oPopUpPG[0]) {
                var oSelectedPGOption = oPopUpPG;
                var oSelectedAreaOfLaw = $(".popUpMDContent.popUpSelected");
                var oSelectedSubAreaOfLaw = $(".popUpSALContent.popUpSelected");

                var sSelectedPracticeGroup = oSelectedPGOption.val();
                var sSelectedPracticeGroupFolderStructure = oSelectedPGOption.attr("data-folderNamesPG");
                // Fetch the folder structure from the Practice Group, Area of Law and Sub Area of Law levels

                var sSelectedAreaOfLaw = oSelectedAreaOfLaw.text();
                var sSelectedAreaOfLawFolderStructure = oSelectedAreaOfLaw.attr("data-folderNamesAOL");
                var sSelectedSubAreaOfLawFolderStructure = oSelectedAreaOfLaw.attr("data-folderNamesSAL");
                var sSelectedSubAreaOfLawIsNofolderStructurePresent = oSelectedAreaOfLaw.attr("data-isNoFolderStructurePresent");

                // Practice Group
                oSelected.attr("data-PracticeGroup", sSelectedPracticeGroup);
                oSelected.attr("data-folderNamesPG", sSelectedPracticeGroupFolderStructure);

                // Area of Law
                oSelected.attr("data-AreaOfLaw", sSelectedAreaOfLaw);
                oSelected.attr("data-folderNamesAOL", sSelectedAreaOfLawFolderStructure);

                // Sub Area of Law
                oSelected.attr("data-folderNamesSAL", sSelectedSubAreaOfLawFolderStructure);
                oSelected.attr("data-isNoFolderStructurePresent", sSelectedSubAreaOfLawIsNofolderStructurePresent);
            }

            var bPresent = false;
            if (oSelected) {
                /*Fixed duplicate issue*/
                var oIterate = $(".popUpDTContent");
                $.each(oIterate, function () {
                    if ($(this).attr("data-document-template") === oSelected.attr("data-document-template")) {
                        bPresent = true;
                        return false;
                    }
                });
                if (!bPresent) {
                    $(".popUpSAL > div, .popDT > div").removeClass("popUpSelected");
                    $(".popDT").append(oSelected.clone().removeClass("popUpSALContent").addClass("popUpDTContent popUpSelected"));
                    var currentSelectedSubAreaOfLaw = $(".popDT .popUpSelected");
                    if (currentSelectedSubAreaOfLaw.length) {
                        var arrAssociatedDocumentTemplates = currentSelectedSubAreaOfLaw.attr("data-associated-document-template").split(";");
                        currentSelectedSubAreaOfLaw.html(currentSelectedSubAreaOfLaw.text() + " (" + arrAssociatedDocumentTemplates.length + ")");
                    }
                }
            }
            $(".popUpDTContent").removeClass("popUpSelected");
        });

        $(".popUpRightClick").click(function () {
            var oSelected = $(".popDT .popUpSelected");
            if (oSelected) {
                $(".popUpSAL > div, .popDT > div").removeClass("popUpSelected");
                oSelected.remove();
            }
        });

        $(document).on("click", ".popUpMDContent", function () {
            var oSAL = $(".popUpSAL"), sFolderNames, boolIsFolderStructurePresent;
            oSAL.find(".popUpSALContent").remove();
            $(".popUpMDContent").removeClass("popUpSelected");
            $(this).addClass("popUpSelected");
            localStorage.iSelectedAOL = $(this).attr("data-value");
            for (var iIterator = 0; iIterator < serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms.length; iIterator++) {
                sFolderNames = serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].FolderNames ? serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].FolderNames : "";
                boolIsFolderStructurePresent = serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].IsNoFolderStructurePresent ? serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].IsNoFolderStructurePresent : "";
                oSAL.append("<div class='popUpSALContent' data-value='" + iIterator + "' data-document-template='" + serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].DocumentTemplates + "' data-associated-document-template='" + serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].DocumentTemplateNames + "' data-folderNamesSAL='" + sFolderNames + "' data-isNoFolderStructurePresent='" + boolIsFolderStructurePresent + "' >" + serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].TermName + "</div>");
            }
            $(".popUpSAL .popUpSALContent:first-child").click();
        });

        $("#addMoreLink").click(function () {
            oJQueryObjects.$AssignPermissions = $(".assignNewPerm");
            var iAssignPermCount = oJQueryObjects.$AssignPermissions.length;
            commonFunctions.addMorePermissions(parseInt(iAssignPermCount, 10) + 1);
        });
        //// hide the error tooltip
        $(document).on("click", "#container", function () {
            var oErrorPopup = $("#container .errorPopup");
            if (!$(oErrorPopup).hasClass("hide")) {
                $(oErrorPopup).addClass("hide");
                $("div input:text , textarea , select").removeClass("redBorder");
            }
        });

        $(document).on("click", ".close", function (event) {
            closePermissionRow(event);
        });

        $(document).on("click", "#saveButton", function (event) {
            commonFunction.AppLogEvent(oGlobalConstants.Events_Tracking_Pages + oGlobalConstants.App_Name_Settings_Page + oGlobalConstants.Save_Settings, true);
            commonFunctions.saveConfigurations();
            event.stopPropagation();
        });

        $(document).on("click", "#cancelButton", function (event) {
            navigateToPage(true);
            event.stopPropagation();
        });

        $(document).on("click", "#goToClients", function (event) {
            navigateToPage(true);
            event.stopPropagation();
        });

        $(document).on("click", "#hideSection", function (event) {
            $(".hideMatterType").removeClass("hide");
            $(".contentMatterType").addClass("hide");
            setHeight();
        });

        $(document).on("click", "#showMatterType", function (event) {
            $(".contentMatterType").removeClass("hide");
            $(".hideMatterType").addClass("hide");
            setHeight();
        });

        $(document).on("click", document, function (event) {
            if ("ddClientIcon" === $($(event.target)[0]).attr("class") || "ddlClient" === $(event.target)[0].id) {
                closeAllPopupExcept("ddlClientList");
            } else if ($($(event.target)[0]).hasClass("popUpPGDiv")) {
                closeAllPopupExcept("popUpPGList");
            } else {
                closeAllPopupExcept("");
            }
        });
        setHeight();
    }
    else {
        serviceConstants.bClientMode = true;
        setTextForFieldsOnPage(bIsClientMode);
        commonFunctions.getClient();
    }
    //// Update the validation for HTML fields
    $(".inputMatterName").attr("maxlength", oGlobalConstants.Matter_Name_Max_Length);
    $(".inputMatterId").attr("maxlength", oGlobalConstants.Matter_Id_Max_Length);
});