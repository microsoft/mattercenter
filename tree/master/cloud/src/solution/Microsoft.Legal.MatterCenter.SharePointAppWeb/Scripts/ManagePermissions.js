/// <disable>JS1003,JS2005,JS2023,JS2024,JS2027,JS2031,JS2032,JS2052,JS2053,JS2064,JS2073,JS2074,JS2076,JS2085,JS3054,JS3056,JS3057,JS3058,JS3085,JS3092,JS3116</disable>

var oManagePermissionsOnMatter = (function () {
    "use strict";
    var serviceConstants = {
        sCentralRepositoryLocation: oGlobalConstants.Central_Repository_Url,
        arrRoles: [],
        arrPermissions: [],
        oMandatoryRoles: [],
        arrReadOnlyUsers: [],
        oMandatoryRoleNames: [],
        arrUserUploadPermissions: oGlobalConstants.User_Upload_Permissions && oGlobalConstants.User_Upload_Permissions.trim().split(","),
        sConflictScenario: "",
        bHasEmailStamped: false,
        oEmailRegex: new RegExp(oGlobalConstants.Email_Validation_Regex)
    },
    oEditPermissionOnMatter = {
        sPermissionToAppend: "<div id='row{0}' class='assignNewPermission'>"
        + "<div class='clear'></div>"
        + "<input type='text' class='medium inputContent usernameInput' id='txtAssign{0}' spellcheck='true' placeholder='Enter and select names' required='required' />"
        + "<div class='gutter'></div>"
        + "<div class='roleInput ddlRoleAssignDiv{0}'><input class='small inputContent' type='text' id='ddlRoleAssign{0}' data-value='' data-mandatory='' disabled='disabled' autofocus='autofocus' /><img class='dropdownIcon' id='ddlRoleAssignIcon{0}' title='Dropdown icon' src='../Images/icon-combobox.png' alt='Role Icon' /><div class='ddlRoleAssignList hide' id='ddlRoleAssignList{0}'></div></div>"
        + "<div class='gutter'></div>"
        + "<div class='permissionInput ddlPermAssignDiv{0}'><input class='small inputContent' type='text' id='ddlPermAssign{0}' data-value='' data-mandatory='' disabled='disabled' autofocus='autofocus' /><img class='dropdownIcon' id='ddlPermAssignIcon{0}' title='Dropdown icon' src='../Images/icon-combobox.png' alt='Permission Icon' /><div class='ddlPermAssignList hide' id='ddlPermAssignList{0}'></div></div>"
        + "<img class='deleteIcon' alt='delete' src='../Images/icon-delete.png' title='delete' /></div>",
        iPermissionCount: 1
    },
    commonFunctions = {},
    /* Object to store all the JQuery binding elements used */
    oJQueryObjects = {
        edit: "",
        clientUrl: "",
        libraryName: "",
        arrResults: "",
        iShowSuccessMessage: ""
    };

    /* Functions for retrieving roles */
    commonFunctions.getRoleFailure = function (result) {
        "use strict";
        return false;
    };
    commonFunctions.getRoleSuccess = function (result) {
        "use strict";
        var $RoleObject = $("#ddlRoleAssignList1");
        $RoleObject.empty();
        var arrResults = JSON.parse(result), roleItem, roleOption, arrOptions = [], iLengthvar
      , iExpectedCount = 0, iActualCount = 0, iIterator = 0, iLength;
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
            // This function will display "role" drop-down menu
            $(".ddlRoleAssignDiv1").click(function (event) {
                closeAllPopupExcept("");
                var $Element = $RoleObject;
                toggleThisElement($Element);
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
                showErrorNotification("#btnSave", arrResults.value);
            }
        }
        iLength = serviceConstants.oMandatoryRoles.length;
        serviceConstants.oMandatoryRoleNames.length = 0;
        for (iIterator = 0; iIterator < iLength; iIterator++) {
            if (serviceConstants.oMandatoryRoles[iIterator].Mandatory) {
                iExpectedCount++;
                serviceConstants.oMandatoryRoleNames.push(serviceConstants.oMandatoryRoles[iIterator].Name);
            }
        }
        oJQueryObjects.iShowSuccessMessage++;
        if (2 == oJQueryObjects.iShowSuccessMessage) {
            oJQueryObjects.edit = getParameterByName("IsEdit");
            commonFunctions.getTeamMembers();
            if ("false" === oJQueryObjects.edit) {
                commonFunctions.addMorePermissions(1);
            }
        }
    };
    commonFunctions.getMatterCenterRoles = function () {
        "use strict";
        var sClientUrl = serviceConstants.sCentralRepositoryLocation
            , matterDetails = {
                "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": {
                    "Url": sClientUrl
                }
            };
        oCommonObject.callProvisioningService("GetRoleData", matterDetails, commonFunctions.getRoleSuccess, commonFunctions.getRoleFailure);
    };

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
            // This function will display "permission" drop-down menu
            $(".ddlPermAssignDiv1").click(function (event) {
                closeAllPopupExcept("");
                var $Element = $PermissionObject;
                toggleThisElement($Element);
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
                showErrorNotification("#btnSave", arrResults.value);
            }
        }
        oJQueryObjects.iShowSuccessMessage++;
        if (2 == oJQueryObjects.iShowSuccessMessage) {
            oJQueryObjects.edit = getParameterByName("IsEdit");
            commonFunctions.getTeamMembers();
            if ("false" === oJQueryObjects.edit) {
                commonFunctions.addMorePermissions(1);
            }
        }
    };
    commonFunctions.getPermFailure = function (result) {
        "use strict";
        return false;
    };
    commonFunctions.getMatterCenterPermissions = function () {
        "use strict";
        var sClientUrl = serviceConstants.sCentralRepositoryLocation
            , matterDetails = {
                "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": {
                    "Url": sClientUrl
                }
            };

        oCommonObject.callProvisioningService("GetPermissionLevels", matterDetails, commonFunctions.getPermSuccess, commonFunctions.getpermFailure);
    };

    /* Function to add permission row and bind the events*/
    commonFunctions.bindPermissionEvents = function (iUniqueId) {
        "use strict";
        var permItem, permOption, roleItem, roleOption, arrOptions = [];
        for (roleItem in serviceConstants.arrRoles) {
            roleOption = "<div class='roleValue assignPerm" + iUniqueId + "' title='" + serviceConstants.arrRoles[roleItem].Name + "' data-value='" + serviceConstants.arrRoles[roleItem].Name + "' data-mandatory='" + serviceConstants.arrRoles[roleItem].Mandatory + "'>" + serviceConstants.arrRoles[roleItem].Name + "</div>";
            arrOptions.push(roleOption);
        }
        var $RoleObject = $("#ddlRoleAssignList" + iUniqueId);
        $RoleObject.append(arrOptions.join(""));
        // This function will display "role" drop-down menu
        $(".ddlRoleAssignDiv" + iUniqueId).click(function (event) {
            closeAllPopupExcept("");
            var $Element = $RoleObject;
            toggleThisElement($Element);
            event.stopPropagation();
        });

        $(".roleValue").mouseover(function () {
            $(this).addClass("ddListOnHover");
        }).mouseout(function () {
            $(this).removeClass("ddListOnHover");
        });

        // This function will select the item from drop-down list
        $(".assignPerm" + iUniqueId + "").click(function (event) {
            var selectedClient = this.innerHTML, $Element = $("#ddlRoleAssign" + iUniqueId);
            $Element.val(selectedClient).attr("data-value", $(this).attr("data-value")).attr("data-mandatory", $(this).attr("data-mandatory"));
            $RoleObject.addClass("hide");
            $(".assignPerm" + iUniqueId).removeClass("ddListOnHover");
            event.stopPropagation();
        });

        $($(".assignPerm" + iUniqueId)[0]).click();

        arrOptions = [];
        for (permItem in serviceConstants.arrPermissions) {
            permOption = "<div class='permValue permContent" + iUniqueId + "' data-value='" + serviceConstants.arrPermissions[permItem].Name + "'  title='" + serviceConstants.arrPermissions[permItem].Name + "'>" + serviceConstants.arrPermissions[permItem].Name + "</div>";
            arrOptions.push(permOption);
        }
        var $PermissionObject = $("#ddlPermAssignList" + iUniqueId);
        $PermissionObject.append(arrOptions.join(""));

        // This function will display permission drop-down menu
        $(".ddlPermAssignDiv" + iUniqueId).click(function (event) {
            closeAllPopupExcept("");
            var $Element = $PermissionObject;
            toggleThisElement($Element);
            event.stopPropagation();
        });

        $(".permValue").mouseover(function () {
            $(this).addClass("ddListOnHover");
        }).mouseout(function () {
            $(this).removeClass("ddListOnHover");
        });

        // This function will select the item from drop-down list
        $(".permContent" + iUniqueId).click(function (event) {
            var selectedClient = this.innerHTML, $Element = $("#ddlPermAssign" + iUniqueId);
            $Element.val(selectedClient).attr("data-value", $(this).attr("data-value"));
            $PermissionObject.addClass("hide");
            $(".permContent" + iUniqueId).removeClass("ddListOnHover");
            event.stopPropagation();
        });

        $($(".permContent" + iUniqueId)[0]).click();

        oCommonObject.bindAutocomplete("#txtAssign" + iUniqueId, true);
    };

    /* Function to add more team assignment line items */
    commonFunctions.addMorePermissions = function (iUniqueId) {
        "use strict";
        var sPermissionChunk = $.trim(oEditPermissionOnMatter.sPermissionToAppend);
        sPermissionChunk = sPermissionChunk ? sPermissionChunk.replace(/\{0\}/g, iUniqueId) : "";
        sPermissionChunk && $("#permissionContainer").append(sPermissionChunk);

        if (null !== oJQueryObjects.add && 1 !== parseInt(iUniqueId, 10)) {
            commonFunctions.bindPermissionEvents(parseInt(iUniqueId, 10));
        } else {
            commonFunctions.bindPermissionEvents(parseInt(iUniqueId, 10));
        }
        oCommonObject.bindAutocomplete("#txtAssign" + iUniqueId, true);
        oEditPermissionOnMatter.iPermissionCount++;
    };

    commonFunctions.getTeamMembers = function () {
        "use strict";
        oJQueryObjects.clientUrl = getParameterByName("clientUrl");
        oJQueryObjects.libraryName = getParameterByName("matterName");
        var matterDetails = {
            "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken }, "client": {
                "Url": oJQueryObjects.clientUrl
            },
            "matter": { "Name": oJQueryObjects.libraryName }
        };
        oCommonObject.callProvisioningService("RetrieveMatterStampedProperties", matterDetails, commonFunctions.getTeamMembersSuccess, commonFunctions.getTeamMembersFailure);
    };
    commonFunctions.getTeamMembersSuccess = function (result) {
        "use strict";
        if (result) {
            var arrResults = JSON.parse(result);
            var iCounter = 0, iCount = 0, userRole, userPermission, sEmail = "";
            $(".loadingImage").hide();
            $("#editpermissionContainer").removeClass("hide");
            oJQueryObjects.arrResults = arrResults;
            if (!arrResults.code) {
                if (0 < arrResults.MatterObject.BlockUserNames.length) {
                    serviceConstants.sConflictScenario = "True";
                } else {
                    serviceConstants.sConflictScenario = "False";
                }
            }
            if ("true" === oJQueryObjects.edit) {
                $("#permissionContainer").empty();
                if (!arrResults.code) {
                    if (0 < arrResults.MatterObject.AssignUserNames.length) {
                        if (arrResults.MatterObject.AssignUserNames) {
                            for (iCounter = 0; iCounter < arrResults.MatterObject.AssignUserNames.length; iCounter++) {
                                var userName = "#txtAssign" + (iCounter), roles = "#ddlRoleAssign" + (iCounter), permissions = "#ddlPermAssign" + (iCounter);
                                commonFunctions.addMorePermissions(iCounter);
                                for (iCount = 0; iCount < arrResults.MatterObject.AssignUserNames[iCounter].length; iCount++) {
                                    if (arrResults.MatterObject.AssignUserEmails && 0 < arrResults.MatterObject.AssignUserEmails.length && arrResults.MatterObject.AssignUserEmails[iCounter][iCount]) {
                                        sEmail = arrResults.MatterObject.AssignUserEmails[iCounter][iCount];
                                        if (serviceConstants.oEmailRegex.test(sEmail)) {
                                            $(userName)[0].value += arrResults.MatterObject.AssignUserNames[iCounter][iCount] + " (" + sEmail + ")" + ";";
                                        } else {
                                            $(userName)[0].value += arrResults.MatterObject.AssignUserNames[iCounter][iCount] + ";";
                                        }
                                        oManagePermissionsOnMatter.bHasEmailStamped = true;
                                    } else {
                                        $(userName)[0].value += arrResults.MatterObject.AssignUserNames[iCounter][iCount] + ";";

                                    }
                                }
                                $(userName).attr("data-resolved", "1");
                                userRole = arrResults.MatterObject.Roles ? arrResults.MatterObject.Roles[iCounter] ? arrResults.MatterObject.Roles[iCounter] : serviceConstants.arrRoles[0].Name : serviceConstants.arrRoles[0].Name;
                                userPermission = arrResults.MatterObject.Permissions[iCounter] ? arrResults.MatterObject.Permissions[iCounter] ? arrResults.MatterObject.Permissions[iCounter] : serviceConstants.arrPermissions[0].Name : serviceConstants.arrPermissions[0].Name;
                                if (-1 === $.inArray(userRole, serviceConstants.oMandatoryRoleNames)) {
                                    $(roles).val(userRole).attr("data-value", userRole).attr("data-mandatory", "false");
                                } else {
                                    $(roles).val(userRole).attr("data-value", userRole).attr("data-mandatory", "true");
                                }
                                $(permissions).val(userPermission).attr("data-value", userPermission);
                            }
                        }
                    } else {
                        if (null !== arrResults.IsNewMatter) {
                            if (arrResults.MatterDetailsObject.ResponsibleAttorney) {
                                var userName = "#txtAssign" + (iCounter), roles = "#ddlRoleAssign" + (iCounter), permissions = "#ddlPermAssign" + (iCounter);
                                commonFunctions.addMorePermissions(iCounter);
                                $(userName)[0].value = arrResults.MatterDetailsObject.ResponsibleAttorney;
                                $(userName).attr("data-resolved", "1");
                                $(roles).val(serviceConstants.arrRoles[0].Name).attr("data-value", userRole).attr("data-mandatory", "true");
                                $(permissions).val("").attr("data-value", "");
                            }
                            if (arrResults.MatterDetailsObject.TeamMembers) {
                                iCounter++;
                                var teamMembers = arrResults.MatterDetailsObject.TeamMembers.split(";");
                                for (iCount = 0; iCount < teamMembers.length; iCount++) {
                                    var userName = "#txtAssign" + (iCounter), roles = "#ddlRoleAssign" + (iCounter), permissions = "#ddlPermAssign" + (iCounter);
                                    commonFunctions.addMorePermissions(iCounter);
                                    iCounter++;
                                    $(userName)[0].value += teamMembers[iCount];
                                    $(userName).attr("data-resolved", "1");
                                    $(roles).val("").attr("data-value", userRole).attr("data-mandatory", "");
                                    $(permissions).val("").attr("data-value", userPermission).attr("data-mandatory", "");
                                }
                            }
                        }

                    }


                } else {
                    if (arrResults.code && "string" === typeof arrResults.code && (-1 < arrResults.code.indexOf("#|#"))) {
                        showCommonErrorPopUp(arrResults.code);
                    } else {
                        showErrorNotification("#btnSave", arrResults.value);
                    }
                }
            }
        }
    };
    commonFunctions.getTeamMembersFailure = function () {
        "use strict";
        return false;
    };

    commonFunctions.validatePopup = function (iCurrentPage) {
        "use strict";
        switch (iCurrentPage) {
            case 1:
                if (validateAssignedUser()) {
                    if (validateTeamAssigmentRole()) {
                        if (validatePermission()) {
                            if (validateBlockUsers()) {
                                return true;
                            } else {
                                showErrorNotification("#btnSave", oMatterProvisionConstants.Error_Invalid_User);
                            }
                        } else {
                            showErrorNotification("#btnSave", oMatterProvisionConstants.Error_Edit_Matter_Mandatory_Permission);
                        }

                    } else {
                        showErrorNotification("#btnSave", oMatterProvisionConstants.Error_Mandatory_Role.replace("{0}", serviceConstants.oMandatoryRoleNames.join(",")));
                        serviceConstants.oMandatoryRoleNames.length = 0;
                    }
                }
                return false;

                break;
            case 2:
                if (validateAssignedUser()) {
                    if (validateBlockUsers()) {
                        return true;
                    } else {
                        showErrorNotification("#btnSave", oMatterProvisionConstants.Error_Invalid_User);
                    }
                }
                return false;
            default:
                return false;
        }
    };

    $(document).ready(function () {
        "use strict";
        oJQueryObjects.clientUrl = getParameterByName("clientUrl");
        oJQueryObjects.libraryName = getParameterByName("matterName");
        oJQueryObjects.edit = getParameterByName("IsEdit");
        if (validateParameters()) {
            commonFunctions.getMatterCenterRoles();
            commonFunctions.getMatterCenterPermissions();
        } else {
            $(".loadingImage").hide();
            $(".error").html(oGlobalConstants.Edit_Matter_Insufficient_Parameter).removeClass("hide");
        }

        $("#addMorePermissions").on("click", function () {
            commonFunctions.addMorePermissions(oEditPermissionOnMatter.iPermissionCount);
        });
        $(document).on("click", ".deleteIcon", function () {
            if (1 < $(".assignNewPermission").length) {
                var oParentObject = $(this).parent();
                oParentObject[0] && oParentObject.remove();
            }
            closeAllPopupExcept("");
        });
    });
    $("#btnSave").click(function (event) {
        "use strict";
        $(".errorPopUp").addClass("hide");
        $(".loading").removeClass("hide");
        $("#btnSave").attr("disabled", "disabled");
        var isValid = "true" === oJQueryObjects.edit ? commonFunctions.validatePopup(1) : commonFunctions.validatePopup(2);
        var oAssignPermList = $("input[id^=txtAssign]"), iErrorFlag = 0, sResponsibleAttorney = [], sResponsibleAttorneyEmail = [];
        var arrResults = oJQueryObjects.arrResults
        , userId = [];
        if (arrResults.ClientObject) {
            var sClientId = arrResults.ClientObject.Id ? arrResults.ClientObject.Id : ""
            , sClientUrl = arrResults.ClientObject.Url ? arrResults.ClientObject.Url : ""
            , sClientName = arrResults.ClientObject.Name ? arrResults.ClientObject.Name : "";
        }
        if (arrResults.MatterObject) {
            var
             matterName = arrResults.MatterObject.Name ? arrResults.MatterObject.Name : ""
            , arrUserNames = []
            , arrUserEmails = []
            , arrTeamMembers = []
            , matterDetails = {}
            , arrRoles = []
            , arrPermissions = []
            , arrBlockUserNames = arrResults.MatterObject.BlockUserNames ? arrResults.MatterObject.BlockUserNames : ""
            , roleInformation = {};
        }
        if (isValid) {
            $.each($("input[id^=ddlPermAssign]"), function (iPosition) {
                var $CurrObject = $(this)[0];
                if ($CurrObject) {
                    var userPermission = $($CurrObject).val();
                    if (-1 < $.inArray(userPermission, serviceConstants.arrUserUploadPermissions)) {
                        var readUsersList = oCommonObject.getUserName($($("input[id^=txtAssign]")[iPosition]).val().trim(), false);
                        if (readUsersList) {
                            var readUsersLength = readUsersList.length;
                            for (var iIterator = 0; iIterator < readUsersLength; iIterator++) {
                                var tempUser = readUsersList[iIterator];
                                if (tempUser && -1 === $.inArray(tempUser, serviceConstants.arrReadOnlyUsers)) {
                                    serviceConstants.arrReadOnlyUsers.push(readUsersList[iIterator]); //// store the users in the list
                                }
                            }
                        }
                    }
                }
            });

            $.each(oAssignPermList, function () {
                var sCurrElementID = $(this).attr("id");
                if (sCurrElementID) {
                    sCurrElementID = sCurrElementID.trim().split("txtAssign")[1];
                    var sCurrRole = $("#ddlRoleAssign" + sCurrElementID), sCurrPermission = $("#txtAssign" + sCurrElementID);
                    if (sCurrRole && sCurrPermission) {
                        if (-1 !== $.inArray(sCurrRole.val(), serviceConstants.oMandatoryRoleNames)) {
                            sResponsibleAttorney.push(oCommonObject.getUserName($.trim($(this).val()), true).join(";"));
                            sResponsibleAttorneyEmail.push(oCommonObject.getUserName($.trim($(this).val()), false).join(";"));     // Removed and Appended semicolon to ensure all users are separated by semicolon                    
                        }
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
                            //// This role is already present. append the new role with semicolon separated value
                            roleInformation[sCurrRole.val()] = roleInformation[sCurrRole.val()] + sCurrPermission.val();
                        } else {
                            //// Add this role to the object
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
                arrUserNames.push(oCommonObject.getUserName($.trim($(this).val()), true));
                arrUserEmails.push(oManagePermissionsOnMatter.bHasEmailStamped ? oCommonObject.getUserName($.trim($(this).val(), false)) : getValidUserName($.trim($(this).val())));
                arrTeamMembers.push(oCommonObject.getUserName($.trim($(this).val()), true).join(";"));
                userId.push(this.id);
            });

            $.each($("input[id^=ddlPermAssign]"), function () {
                arrPermissions.push($(this).val());
            });

            matterDetails = {
                "requestObject": { "SPAppToken": oSharePointContext.SPAppToken, "RefreshToken": oSharePointContext.RefreshToken },
                "client": { "Url": oJQueryObjects.clientUrl, "Id": sClientId, "Name": sClientName },
                "matter": { "Name": oJQueryObjects.libraryName, "BlockUserNames": arrBlockUserNames, "AssignUserNames": arrUserNames, "AssignUserEmails": arrUserEmails, "Permissions": arrPermissions, "Roles": arrRoles, "Conflict": { "Identified": serviceConstants.sConflictScenario } },
                "matterDetails": { "ResponsibleAttorney": sResponsibleAttorney.join(";").replace(/;;/g, ";"), "ResponsibleAttorneyEmail": sResponsibleAttorneyEmail.join(";").replace(/;;/g, ";"), "UploadBlockedUsers": serviceConstants.arrReadOnlyUsers, "TeamMembers": arrTeamMembers.join(";"), "RoleInformation": JSON.stringify(roleInformation) }
                , "editMode": oJQueryObjects.edit, "userId": userId
            };
            oCommonObject.callProvisioningService("UpdateMatterDetails", matterDetails, commonFunctions.matterUpdateSuccess, commonFunctions.matterUpdateFailure, commonFunctions.beforeSendMatterUpdate);
        } else {
            event.stopPropagation();
        }

    });
    commonFunctions.beforeSendMatterUpdate = function () {
        $(".popupWait").removeClass("hide");
    };
    commonFunctions.matterUpdateSuccess = function (result) {
        "use strict";
        if (result) {
            var arrResults = JSON.parse(result);
            $("#btnSave").removeAttr("Disabled");
            $(".loading, .popupWait").addClass("hide");
            if (!arrResults.code) {
                sendMessageToParent("true");
            } else {
                if (arrResults.code && "string" === typeof arrResults.code && (-1 < arrResults.code.indexOf("#|#"))) {
                    showCommonErrorPopUp(arrResults.code);
                } else {
                    var results = JSON.stringify(oJQueryObjects.arrResults);
                    var errorMessage;
                    commonFunctions.getTeamMembersSuccess(results);
                    if (arrResults.code === oGlobalConstants.Error_Code_Security_Group_Exists || arrResults.code === oGlobalConstants.Incorrect_Team_Members_Code) {
                        errorMessage = arrResults.value.split("$|$")[0];
                    } else {
                        errorMessage = arrResults.value;
                    }
                    showErrorNotification("#btnSave", errorMessage);
                }
            }
        }
    };
    commonFunctions.matterUpdateFailure = function (result) {
        "use strict";
        $(".loading,.popupWait").addClass("hide");
        $(".error").html(oGlobalConstants.Edit_Matter_Failure).removeClass("hide");
        $("#btnSave").removeAttr("Disabled");
    };

    function validateAssignedPerm() {
        "use strict";
        var oAssignPermList = $("input[id^=txtAssign]"), iAssignPermPos = 1, iErrorFlag = 0, arrAssignIdList = [], oRolesList = $("input[id^=ddlRoleAssign]");
        //// Here we have a list of ID's of Assign Permissions which are currently present in Step 2
        $.each(oAssignPermList, function (iIterator) {
            if ("1" !== $(this).attr("data-resolved")) {
                if ($(this).attr("id")) {
                    var iCurrId = $(this).attr("id").trim().split("txtAssign")[1], oCurrRoleItem = $("#ddlRoleAssign" + iCurrId);
                    if (oCurrRoleItem) {
                        var sErrorId = $(this).attr("id"), sAssignedRole = oCurrRoleItem.val();
                        if ("true" === $($("input[id=ddlRoleAssign" + iCurrId + "]")[0]).attr("data-mandatory")) {
                            iErrorFlag = 1;
                        }
                    }
                }
            }
        });
        if (1 === iErrorFlag) {
            return false;
        }
        return true;
    };

    // Function to get email address from text box if present else take user name instead of email address
    function getValidUserName(sUserEmails) {
        "use strict";
        var arrUserNames = [], sEmail = "", oEmailRegex = new RegExp(oGlobalConstants.Email_Validation_Regex);
        if (sUserEmails && null !== sUserEmails && "" !== sUserEmails) {
            arrUserNames = sUserEmails.split(";");
            for (var iIterator = 0; iIterator < arrUserNames.length - 1; iIterator++) {
                if (arrUserNames[iIterator] && null !== arrUserNames[iIterator] && "" !== arrUserNames[iIterator]) {
                    if (-1 !== arrUserNames[iIterator].lastIndexOf("(")) {
                        sEmail = $.trim(arrUserNames[iIterator].substring(arrUserNames[iIterator].lastIndexOf("(") + 1, arrUserNames[iIterator].lastIndexOf(")")));
                        if (oEmailRegex.test(sEmail)) {
                            arrUserNames[iIterator] = sEmail;
                        }
                    }
                }
            }
        }
        return arrUserNames;
    };

    function validateTeamAssigmentRole() {
        "use strict";
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

    function getParameterByName(name) {
        "use strict";
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(decodeURIComponent(location.search));
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    /* Function to validate the entries in Blocked user field */
    function validateBlockUsers() {
        "use strict";
        var validity = true, arrBlockedUsers = oJQueryObjects.arrResults.MatterObject.BlockUserNames ? oJQueryObjects.arrResults.MatterObject.BlockUserNames : null;
        if (!arrBlockedUsers) {
            return true;
        }
        $.each(arrBlockedUsers, function (iIndex) {
            if (arrBlockedUsers[iIndex]) {
                $.each($("input[id^=txtAssign]"), function () {
                    if ($(this).val().trim() && -1 !== $(this).val().trim().indexOf(arrBlockedUsers[iIndex].trim()) && arrBlockedUsers[iIndex] && arrBlockedUsers[iIndex].trim()) {
                        validity = false;
                        return false;
                    }
                });
                if (!validity) {
                    return false;
                }
            }
        });
        return validity;
    }

    $(document).on("click", function () {
        "use strict";
        closeAllPopupExcept("");
    });

    function showErrorNotification(sElement, sMsg) {
        "use strict";
        $(".loading").addClass("hide");
        $("#btnSave").removeAttr("Disabled");
        var windowWidth = GetWidth();
        var removeTop = -247, removeLeft = -430;
        var posLeft = "50px";
        var triangleTopPos = 48;
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
            oJQueryObjects.$ErrorPopUp = $(".errorPopUp");
            if (windowWidth > 734) {
                if (sElement === "#btnSave" && !($("#editpermissionContainer").hasClass("hide"))) {
                    removeLeft = 90;
                    posLeft = "125px";
                    removeTop = 73;
                }
            }
            oJQueryObjects.$ErrorPopUp.css("left", iLeftPos - removeLeft).css("top", iTopPos - removeTop).removeClass("hide").find(".errText").text(sMsg);
            $(".mainArea .errTriangleBorder").css("left", posLeft);
            $(".mainArea .errTriangle").css("left", posLeft);
            $(".mainArea .errTriangleBorder").css("top", "calc(50% - -" + (triangleTopPos - 1) + "px)");
            $(".mainArea .errTriangle").css("top", "calc(50% - -" + (triangleTopPos) + "px)");
            $(".mainArea .errText").css("min-height", errorBoxHeight + "px").css("top", errorBoxTop + "px").css("left", errorBoxLeft + "px");
            oRoleObject.css("border-width", "2px").css("border-color", "red");
        }
    }

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

    $(document).on("keypress, input", "input[id^=txtAssign]", function () {
        $(this).removeAttr("data-resolved");
    });

    $(document).on("click", "#editpermissionContainer", function () {
        "use strict";
        $(".errorPopUp").addClass("hide");
    });
    function validateParameters() {
        "use strict";
        if (!oJQueryObjects.clientUrl || !oJQueryObjects.libraryName || !oJQueryObjects.edit) {
            return false;
        }
        return true;
    }

    /* Function to validate Assigned Users */
    function validateAssignedUser() {
        "use strict";
        var oAssignPermList = $("input[id^=txtAssign]"), iErrorFlag = 0;
        //// Here we have a list of ID's of Assign Permissions which are currently present in Step 2
        $.each(oAssignPermList, function (iIterator) {
            var iCurrId = $(this).attr("id").trim().split("txtAssign")[1], iCurrUser = $("#txtAssign" + iCurrId), oCurrRoleItem = $("#ddlRoleAssign" + iCurrId);
            if (!iCurrUser.val()) {
                iErrorFlag = 1;
                showErrorNotification("#btnSave", oGlobalConstants.Edit_Matter_Validate_User.replace("{0}", oCurrRoleItem.val()));
            }
        });
        if (1 === iErrorFlag) {
            return false;
        }
        return true;
    }

    // Function is used to close popup dialog
    function sendMessageToParent(status) {
        "use strict";
        if (status) {
            var iFrameURL = oGlobalConstants.Site_Url;
            window.top.parent.postMessage(status, iFrameURL);
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
    // #region Return exposed Functions and Variables
    return ({
        sConflictScenario: function () {
            return serviceConstants.sConflictScenario;
        }
    });
})();
