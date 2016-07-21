(function () {
    'use strict';
    var app = angular.module("matterMain");
    app.controller('MatterUsersController', ['$scope', '$state', '$stateParams', 'api', 'matterResource', '$filter', '$window', '$rootScope', '$location',
        function ($scope, $state, $stateParams, api, matterResource, $filter, $window, $rootScope, $location) {
            var cm = this;
            cm.arrAssignedUserName = [],
            cm.arrAssignedUserEmails = [],
            cm.userIDs = [];
            cm.matterProperties = undefined;
            cm.assignPermissionTeams =[];

            //#region Service API Call
            //API call to get roles that are configured in the system
            function getRoles(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getRoles',
                    data: options,
                    success: callback
                });
            }
            //API call to get permission levels that are configured in the system
            function getPermissionLevels(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getPermissionLevels',
                    data: options,
                    success: callback
                });
            }

            //API call to users from matter stamped properties api
            function getStampedProperties(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getStampedProperties',
                    data: options,
                    success: callback
                });
            }
            //API call to update matter permissions
            function updateMatterPermissions(optionsForUsers, callback) {
                api({
                    resource: 'matterResource',
                    method: 'updateMatter',
                    data: '',
                    success: callback
                });
            }
            //API call to check whether a user exists or not?
            function userexists(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'userexists',
                    data: options,
                    success: callback
                });
            }
            //#endregion

            //#region
            cm.searchUsers = function (val) {
                var searchUserRequest = {
                    Client: {
                        //Need to get the matter url from query string
                        Url: 'https://msmatter.sharepoint.com/sites/client'
                    },
                    SearchObject: {
                        SearchTerm: val
                    }
                };
                return matterResource.getUsers(searchUserRequest).$promise;
            }

            var optionsForRoles = new Object;
            optionsForRoles = {
                Url: configs.global.repositoryUrl
            }

            var optionsForPermissionLevels = new Object;
            optionsForPermissionLevels = {
                Url: configs.global.repositoryUrl
            }

            var optionsForStampedProperties = new Object;
            optionsForStampedProperties = {
                Client: {
                    Url: 'https://msmatter.sharepoint.com/sites/microsoft'
                },
                Matter: {
                    // Name:'Microsoft Matter'
                    Name: '56Test127'
                }
            }
            //endregion

            //#region Main function calss
            function getMatterUsers() {
                getStampedProperties(optionsForStampedProperties, function (response) {
                    cm.matterProperties = response
                    console.log(response);
                    //Get all roles from catalog site collection
                    getRoles(optionsForRoles, function (response) {
                        cm.assignRoles = response;
                        //Get all permissions from catalog site collection
                        getPermissionLevels(optionsForPermissionLevels, function (response) {
                            cm.assignPermissions = response;
                            getUsersRolesAndPermissions();
                        });
                    });
                });
            }

            getMatterUsers();


            function getUsersRolesAndPermissions() {
                var tempMatterProp = cm.matterProperties;
                var userEmails = tempMatterProp.matterObject.assignUserEmails;
                var userNames = tempMatterProp.matterObject.assignUserNames;
                var permissions = tempMatterProp.matterObject.permissions;
                var roles = tempMatterProp.matterObject.roles;
                var assigendTeams = [];

                if (userEmails && userNames && permissions && roles && userEmails.length === userNames.length && userNames.length === permissions.length && permissions.length === roles.length) {
                    for (var i = 0; i < userEmails.length; i++) {
                        var assignedTeam = {};
                        assignedTeam.assignedUser = userNames[i][0] + "(" + userEmails[i][0] + ")";
                        // assignedTeam.assignedRole = roles[i];
                        angular.forEach(cm.assignRoles, function (role) {
                            if (role.name == roles[i]) {
                                assignedTeam.assignedRole = role;

                            }
                        });
                        angular.forEach(cm.assignPermissions, function (permission) {
                            if (permission.name == permissions[i]) {
                                assignedTeam.assignedPermission = permission;

                            }
                        });
                        // assignedTeam.assignedPermission = permissions[i];
                        cm.assignPermissionTeams = (cm.assignPermissionTeams.length == 1 && cm.assignPermissionTeams[0].assignedUser == "") ? [] : cm.assignPermissionTeams;
                        assignedTeam.assigneTeamRowNumber = (cm.assignPermissionTeams.length == 1 && cm.assignPermissionTeams[0].assignedUser == "") ? 1 : cm.assignPermissionTeams.length + 1;
                        cm.assignPermissionTeams.push(assignedTeam);

                    }
                    console.log(cm.assignPermissionTeams);
                }

            }

            cm.removeAssignPermissionsRow = function (index) {
                var remainingRows = cm.assignPermissionTeams.length;
                if (1 < remainingRows) {
                    cm.assignPermissionTeams.splice(index, 1);
                    
                }
            };
            cm.addNewAssignPermissions = function () {
                var newItemNo = cm.assignPermissionTeams.length + 1;
                cm.assignPermissionTeams.push({ 'assigneTeamRowNumber': newItemNo, 'assignedRole': cm.assignRoles[0], 'assignedPermission': cm.assignPermissions[0] });
            };

            //getPermissionsAndRoles();

            var arrRoles = [];
            arrRoles = getAssignedUserRoles();

            var arrPermissions = [];
            arrPermissions = getAssignedUserPermissions();
            //#endregion


            cm.checkUserExists = function (userMailId) {
                function validateEmail(email) {
                    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                    return re.test(email);
                }
                function validate(email) {
                    if (validateEmail(email)) {
                        var optionsForUserExsists = new Object;
                        optionsForUserExsists = {
                            Url: cm.clientUrl,
                            Name: email
                        }
                        userexists(optionsForUserExsists, function (response) {
                            console.log(response);
                        });

                    }
                    else {
                        return false;
                    }
                }
                if (userMailId && userMailId != "") {
                    var pattern = /\(([^)]+)\)/, matches = userMailId.match(pattern);
                    if (matches && matches.length > 0) {
                        userMailId = matches[1];
                    }
                    validate(userMailId);
                }
            }

            //#region Utilty functions

            function validateTeamAssigmentRole() {
                var oAssignList = cm.assignPermissionTeams
                , iExpectedCount = 0, iActualCount = 0, iIterator = 0, iLength = cm.assignRoles.length;
                for (iIterator = 0; iIterator < iLength; iIterator++) {
                    if (cm.assignRoles[iIterator].mandatory) {
                        iExpectedCount++;
                        cm.oMandatoryRoleNames.push(cm.assignRoles[iIterator].name);
                    }
                }
                angular.forEach(oAssignList, function (oItem) {
                    if (true == oItem.assignedRole.mandatory) {
                        iActualCount++;
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
                var oPermissionList = cm.assignPermissionTeams, bIsFullControl = false;
                var Edit_Matter_Mandatory_Permission_Level = "Full Control";
                angular.forEach(oPermissionList, function (oPermissionList, oPermissionListItem) {
                    if (oPermissionListItem) {
                        if (Edit_Matter_Mandatory_Permission_Level === oPermissionListItem.assignedPermission.name) {
                            bIsFullControl = true;
                        }
                    }
                });
                return bIsFullControl;
            }

            function getUserEmail(arrUsersEmails) {
                var sEmail = "";
                if (arrUsersEmails && 0 < arrUsersEmails.length) {
                    for (var nCount = 0; nCount < arrUsersEmails.length; nCount++) {
                        if ("" !== arrUsersEmails[nCount]) {
                            sEmail = arrUsersEmails[nCount];
                        }
                    }
                }
                return sEmail;
            }

            function getArrAssignedUserNamesAndEmails() {
                cm.arrAssignedUserName = [], cm.arrAssignedUserEmails = [], cm.userIDs = [];
                var count = 1;
                angular.forEach(cm.assignPermissionTeams, function (team) { //For loop
                    cm.arrAssignedUserName.push(getUserName(team.assignedUser + ";", true));
                    cm.arrAssignedUserEmails.push(getUserName(team.assignedUser + ";", false));
                    cm.userIDs.push("txtAssign" + count++);
                });
            }

            function getAssignedUserRoles() {
                "use strict";
                var arrAssigneTeams = cm.assignPermissionTeams, nCount = 0, nlength, arrRoles = [];
                if (arrAssigneTeams) {
                    nlength = arrAssigneTeams.length;
                    for (nCount = 0; nCount < nlength; nCount++) {
                        if (arrAssigneTeams[nCount] && arrAssigneTeams[nCount].assignedRole) {
                            if (arrAssigneTeams[nCount].assignedRole && arrAssigneTeams[nCount].assignedRole.name) {
                                if ("" !== arrAssigneTeams[nCount].assignedRole.name) {
                                    arrRoles.push(arrAssigneTeams[nCount].assignedRole.name);
                                }
                            }
                        }
                    }
                }
                return arrRoles;
            }

            function getAssignedUserPermissions() {
                "use strict";
                var arrAssigneTeams = cm.assignPermissionTeams, nCount = 0, nlength, arrAssignRoles, arrPermissions = [];
                if (arrAssigneTeams) {
                    nlength = arrAssigneTeams.length;
                    for (nCount = 0; nCount < nlength; nCount++) {
                        if (arrAssigneTeams[nCount] && arrAssigneTeams[nCount].assignedPermission) {
                            if (arrAssigneTeams[nCount].assignedPermission && arrAssigneTeams[nCount].assignedPermission.name) {
                                if ("" !== arrAssigneTeams[nCount].assignedPermission.name) {
                                    arrPermissions.push(arrAssigneTeams[nCount].assignedPermission.name);
                                }
                            }
                        }
                    }
                }
                return arrPermissions;
            }
            //#endregion
        }]);
})();