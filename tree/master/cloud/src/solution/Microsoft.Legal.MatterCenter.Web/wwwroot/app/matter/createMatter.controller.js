(function () {
    'use strict';

    var app = angular.module("matterMain");
    app.controller('createMatterController', ['$scope', '$state', '$stateParams', 'api', 'matterResource', '$filter', '$window', '$rootScope', 'adalAuthenticationService',
        function ($scope, $state, $stateParams, api, matterResource, $filter, $window, $rootScope, adalService) {
            ///All Variables
            var cm = this;
            $rootScope.pageIndex = "4";
            cm.selectedConflictCheckUser = undefined;
            $rootScope.bodyclass = "";
            $rootScope.profileClass = "";
            cm.blockedUserName = undefined;
            cm.defaultConfilctCheck = false;
            cm.createContent = uiconfigs.CreateMatter;
            cm.createMatterTaxonomyColumnNames = configs.contentTypes.managedColumns;
            cm.header = uiconfigs.Header;
            cm.chkConfilctCheck = undefined;
            cm.conflictRadioCheck = true;
            cm.iShowSuccessMessage = 0;
            cm.oMandatoryRoleNames = [];
            cm.bMatterLandingPage = false;
            cm.oSiteUsers = [];
            cm.successBanner = false;
            cm.selectedClient = undefined;
            cm.createBtnDisabled = false;
            cm.createButton = "Create";
            cm.clientUrl = "";
            cm.errorStatus = false;
            cm.prevButtonDisabled = true;
            cm.invalidUserCheck = false;
            cm.showRoles = true;
            cm.showMatterId = true;
            cm.matterIdType = "Custom";
            $rootScope.displayOverflow = "";
            cm.nextButtonDisabled = false; cm.prevButtonDisabled = true;
            cm.taxonomyHierarchyLevels = configs.taxonomy.levels;
            cm.schema = configs.search.Schema;
            cm.isBackwardCompatible = configs.global.isBackwardCompatible;
            cm.isClientMappedWithHierachy = configs.global.isClientMappedWithHierachy;
            cm.taxonomyHierarchyLevels = parseInt(cm.taxonomyHierarchyLevels);
            if (cm.taxonomyHierarchyLevels >= 2) {
                cm.parentLevelOneList = [];
                cm.levelOneList = [];
                cm.levelTwoList = [];
                cm.createContent.Tab1Textbox5Label = cm.createContent.Tab1Textbox5Label;
            }
            if (cm.taxonomyHierarchyLevels >= 3) {
                cm.levelThreeList = [];
                cm.createContent.Tab1Textbox5Label = cm.createContent.Tab1Textbox5LabelForLevel3;
            }
            if (cm.taxonomyHierarchyLevels >= 4) {
                cm.levelFourList = [];
                cm.createContent.Tab1Textbox5Label = cm.createContent.Tab1Textbox5LabelForLevel4;
            }
            if (cm.taxonomyHierarchyLevels >= 5) {
                cm.levelFiveList = [];
                cm.createContent.Tab1Textbox5Label = cm.createContent.Tab1Textbox5LabelForLevel5;
            }


            //var managedColumns = { }
            //   for (var i = 0; i < cm.taxonomyHierarchyLevels; i++) {
            //       var columnName = configs.contentTypes.managedColumns["ColumnName" +(i +1)];
            //       managedColumns[columnName] = {};

            //   }

            //   console.log(managedColumns);

            var w = angular.element($window);

            w.bind('resize', function () {
                cm.clearPopUp();
                $scope.$apply();
            });

            var oPageOneState = {
                ClientValue: [],
                ClientId: "",
                MatterName: "",
                MatterId: "",
                MatterDescription: "",
                ContentTypes: [],
                matterMandatory: "",
                //  oAreaOfLawTerms: [],
                // oSubAreaOfLawTerms: [],
                oSelectedDocumentTypeLawTerms: [],
                oValidMatterName: undefined,
                isNextClick: false,
                sectionClickName: "",
                showRoles: true,
                showMatterId: true,
                matterIdType: "Custom",
                specialCharacterExpressionMatter: "[A-Za-z0-9_]+[-A-Za-z0-9_, ]*",
                isBackwardCompatible: false,
                isClientMappedWithHierachy:false
            }

            var oPageTwoState = {
                ChkConfilctCheck: false,
                SelectedConflictCheckUser: "",
                ConflictDate: null,
                ConflictRadioCheck: "",
                BlockedUserName: [],
                SecureMatterCheck: true,
                AssignPermissionTeams: [],
                oSiteUsers: []
            }

            cm.clientId = "";
            cm.selectedClientName = "";
            cm.matterName = "";
            cm.matterId = "";
            cm.matterDescription = "";
            cm.clientNameList = [];
            //cm.areaOfLawTerms = [];
            //cm.subAreaOfLawTerms = [];
            //cm.documentTypeLawTerms = [];
            cm.selectedDocumentTypeLawTerms = [];
            //cm.activeAOLTerm = null;
            //cm.activeSubAOLTerm = null;
            //cm.activeDocumentTypeLawTerm = null;
            cm.popupContainerBackground = "Show";
            cm.popupContainer = "hide";
            cm.sectionName = "snOpenMatter";
            cm.removeDTItem = false;
            cm.primaryMatterType = cm.errorPopUp = false;
            cm.matterGUID = "";
            cm.iCurrentPage = 1;
            cm.assignPermissionTeams = [{ assignedUser: '', assignedRole: '', assignedPermission: '', assigneTeamRowNumber: 1, userConfirmation: false }];
            cm.assignRoles = [];
            cm.assignPermissions = [];
            cm.secureMatterCheck = true;
            cm.conflictRadioCheck = true;
            cm.includeTasks = false;

            ///* Function to generate 32 bit GUID */
            function get_GUID() {
                function create_GUID(bFlag) {
                    var sCurrentGUID = (Math.random().toString(16) + "000000000").substr(2, 8);
                    return bFlag ? "-" + sCurrentGUID.substr(0, 4) + "-" + sCurrentGUID.substr(4, 4) : sCurrentGUID;
                }
                return create_GUID() + create_GUID(true) + create_GUID(true) + create_GUID();
            }

            cm.clearPopUp = function () {
                cm.errorPopUpBlock = false;
                cm.notificationPopUpBlock = false;
            }

            /* Function to get the GUID by removing the hyphen character */
            function getMatterGUID() {
                cm.matterGUID = get_GUID().replace(/-/g, ""); //// Remove '-' (hyphen) from the GUID as this character is removed from URL by SharePoint
            }

            getMatterGUID();


            var optionsForClientGroup = new Object;
            var optionsForPracticeGroup = new Object;
            var optionsForCheckMatterName = new Object;
            // var optionsForUsers = new Object;
            var siteCollectionPath = "";

            ////API calling functions


            function getTaxonomyDetailsForClient(optionsForClientGroup, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getTaxonomyDetails',
                    data: optionsForClientGroup,
                    success: callback
                });

            }

            function getDefaultMatterConfigurations(siteCollectionPath, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getDefaultMatterConfigurations',
                    data: JSON.stringify(siteCollectionPath),
                    success: callback
                });
            }


            function getTaxonomyDetailsForPractice(optionsForPracticeGroup, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getTaxonomyDetails',
                    data: optionsForPracticeGroup,
                    success: callback
                });
            }

            function getCheckValidMatterName(optionsForCheckMatterName, callback) {
                api({
                    resource: 'matterResource',
                    method: 'checkMatterExists',
                    data: optionsForCheckMatterName,
                    success: callback
                });
            }

            function getUsers(optionsForUsers, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getUsers',
                    data: optionsForUsers,
                    success: callback
                });
            }

            function getRoles(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getRoles',
                    data: options,
                    success: callback
                });
            }
            function getPermissionLevels(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getPermissionLevels',
                    data: options,
                    success: callback
                });
            }

            function createMatter(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'createMatter',
                    data: options,
                    success: callback
                });
            }


            function assignContentTypeMetadata(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'assignContentType',
                    data: options,
                    success: callback
                });
            }


            function assignUserPermissionsAPI(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'assignUserPermissions',
                    data: options,
                    success: callback
                });
            }

            function createMatterLandingPageAPI(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'createLandingPage',
                    data: options,
                    success: callback
                });
            }

            function updateMatterMetadataAPI(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'updateMatterMetadata',
                    data: options,
                    success: callback
                });
            }

            function getStampedProperties(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getStampedProperties',
                    data: options,
                    success: callback
                });
            }



            optionsForClientGroup = {
                Client: {
                    Url: configs.global.repositoryUrl
                },
                TermStoreDetails: {
                    TermGroup: configs.taxonomy.termGroup,
                    TermSetName: configs.taxonomy.clientTermSetName,
                    CustomPropertyName: configs.taxonomy.clientCustomPropertiesURL,
                }
            }

            //input parameters building here for all the api's
            optionsForPracticeGroup = {
                Client: {

                    Url: configs.global.repositoryUrl
                },
                TermStoreDetails: {
                    TermGroup: configs.taxonomy.termGroup,
                    TermSetName: configs.taxonomy.practiceGroupTermSetName,
                    CustomPropertyName: configs.taxonomy.subAreaOfLawCustomContentTypeProperty,
                    DocumentTemplatesName: configs.taxonomy.subAreaOfLawDocumentContentTypeProperty,
                }
            }

            optionsForCheckMatterName = {
                Client: {

                    Url: cm.clientUrl
                },
                Matter: {
                    MatterGuid: "",
                    Name: cm.matterName.trim()
                }
            }
            //optionsForUsers

            cm.searchUsers = function (val) {
                cm.typehead = true;
                var searchUserRequest = {
                    Client: {

                        Url: cm.clientUrl
                    },
                    SearchObject: {
                        SearchTerm: val
                    }
                };
                //if (matterResource.getUsers(searchUserRequest).$promise)
                //console.log(matterResource.getUsers(searchUserRequest).$promise.$$state.value);
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


            function getTaxonomyHierarchy(data) {
                var levelsDefined = data.levels;
                if (levelsDefined >= 2) {
                    cm.levelOneList = data.level1;
                    cm.levelTwoList = cm.levelOneList[0].level2;
                    cm.activeLevelTwoItem = cm.levelTwoList[0];
                }
                if (levelsDefined >= 3) {
                    cm.levelThreeList = cm.levelTwoList[0].level3;
                    cm.activeLevelThreeItem = cm.levelThreeList[0];
                }
                if (levelsDefined >= 4) {
                    cm.levelFourList = cm.levelThreeList[0].level4;
                    cm.activeLevelFourItem = cm.levelFourList[0];
                }
                if (levelsDefined >= 5) {
                    cm.levelFiveList = cm.levelFourList[0].level5;
                    cm.activeLevelFiveItem = cm.levelFiveList[0];
                }
            }

            //call back function for getting the clientNamesList
            function getTaxonomyData() {
                getTaxonomyDetailsForClient(optionsForClientGroup, function (response) {

                    cm.clientNameList = response.clientTerms;
                    // jQuery('#myModal').modal('show');
                    // optionsForPracticeGroup.Client.Url=cm.clientUrl;
                    getTaxonomyDetailsForPractice(optionsForPracticeGroup, function (response) {
                        if (response.isError !== undefined && response.isError) {
                            showApiErrorMessages(response);
                        }
                        else {
                            // cm.pracitceGroupList = response.pgTerms;
                            cm.levelOneList = response.level1;
                            cm.parentLevelOneList = response;
                            cm.selectedLevelOneItem = response.level1[0];
                            if (cm.iCurrentPage == 1) {
                                getTaxonomyHierarchy(response);
                            }
                            else {
                                if (cm.isClientMappedWithHierachy) {
                                    getClientsPracticeGroup(cm.selectedClientName);
                                }
                            }
                            getRoles(optionsForRoles, function (response) {
                                cm.assignRoles = response;

                                angular.forEach(cm.assignPermissionTeams, function (team) { //For loop
                                    //  term.assignedRole = term.assignedRole.name;
                                    if ("" !== team.assignedRole) {
                                        angular.forEach(cm.assignRoles, function (role) {
                                            if (role.id == team.assignedRole.id) {
                                                team.assignedRole = role;

                                            }
                                        });
                                    }
                                    else {
                                        team.assignedRole = cm.assignRoles[0];

                                    }
                                });

                                getPermissionLevels(optionsForPermissionLevels, function (response) {

                                    //console.log("Permission Levels");
                                    //console.log(response);
                                    cm.assignPermissions = response;
                                    angular.forEach(cm.assignPermissionTeams, function (team) { //For loop
                                        //  term.assignedRole = term.assignedRole.name;
                                        if ("" !== team.assignedPermission) {
                                            angular.forEach(cm.assignPermissions, function (permission) {
                                                if (permission.id == team.assignedPermission.id) {
                                                    team.assignedPermission = permission;

                                                }
                                            });
                                        }
                                        else {
                                            team.assignedPermission = cm.assignPermissions[0];

                                        }

                                        cm.popupContainerBackground = "hide";
                                    });

                                });

                            });
                        }

                    });
                });
            }

            getTaxonomyData();

            //cm.popupContainerBackground = "Show";
            //cm.popupContainer = "Show";

            //calls this function when selectType button clicks
            //cm.selectMatterType = function (value) {
            //    //cm.popupContainerBackground = "Show";
            //    //cm.popupContainer = "Show";
            //}
            cm.selectMatterType = function (value) {
                cm.popupContainer = "Show";
                cm.popupContainerBackground = "Show";
            }
            //calls this function when selectType button clicks
            //cm.selectMatterType = function (value) {

            //    cm.popupContainerBackground = "Show";

            //    if (cm.levelOneList == null) {
            //        getTaxonomyDetailsForPractice(optionsForPracticeGroup, function (response) {
            //            cm.levelOneList = response.level1;
            //            cm.popupContainer = "Show";
            //            cm.popupContainerBackground = "Show";
            //        });
            //    }
            //    else {
            //        cm.popupContainer = "Show";
            //        cm.popupContainerBackground = "Show";
            //    }
            //    if (cm.levelOneList[0]) {
            //        cm.selectedLevelOne = cm.levelOneList[0];
            //        cm.areaOfLawTerms = cm.levelOneList[0].level2;
            //        cm.subAreaOfLawTerms = cm.levelOneList[0].level2[0].level3;
            //        cm.activeAOLTerm = cm.levelOneList[0].level2[0];
            //        cm.activeSubAOLTerm = cm.levelOneList[0].level2[0].level3[0];

            //    }
            //}

            //function for closing the popup
            cm.selectMatterTypePopUpClose = function () {
                if (cm.popupContainer == "Show") {
                    cm.popupContainerBackground = "hide";
                    cm.popupContainer = "hide";
                }


            }
            //function to get the clientId from ClientName dropdown
            cm.getSelectedClientValue = function (client) {
                if (undefined !== client && null !== client) {
                    cm.clientId = client.id;
                    cm.selectedClientName = client.name;
                    cm.clientUrl = client.url;
                    cm.popupContainerBackground = "Show";
                    siteCollectionPath = cm.clientUrl;
                    if (cm.isClientMappedWithHierachy) {
                        getClientsPracticeGroup(cm.selectedClientName);
                    }                 
                    getDefaultMatterConfigurations(siteCollectionPath, function (result) {
                        if (result.isError) {
                            cm.errTextMsg = result.value;
                            cm.errorBorder = "client";
                            showErrorNotification("client");
                            cm.errorPopUpBlock = true;

                        }
                        else {
                            var dMatterAreaOfLaw = "", dMatterPracticeGroup = "", dMatterSubAreOfLaw = "", dMatterTypes = "", dPrimaryMatterType = "", dMatterUsers = "", dMatterUserEmails = "", dMatterPermissions = "", dMatterRoles = "";

                            var defaultMatterConfig = JSON.parse(result.code);
                            cm.matterName = defaultMatterConfig.DefaultMatterName;
                            cm.checkValidMatterName();
                            cm.matterId = defaultMatterConfig.DefaultMatterId;
                            cm.secureMatterCheck = true;
                            if (defaultMatterConfig.IsRestrictedAccessSelected) {
                                cm.secureMatterCheck = defaultMatterConfig.IsRestrictedAccessSelected;
                            }
                            //else {
                            //    cm.secureMatterCheck = defaultMatterConfig.IsRestrictedAccessSelected;
                            //}
                            if (defaultMatterConfig.IsCalendarSelected) {
                                cm.includeCalendar = defaultMatterConfig.IsCalendarSelected;
                            }
                            else {
                                cm.includeCalendar = defaultMatterConfig.IsCalendarSelected;
                            }
                            if (defaultMatterConfig.IsEmailOptionSelected) {
                                cm.includeEmail = defaultMatterConfig.IsEmailOptionSelected;
                                cm.createButton = "Create and Notify";
                            }
                            else {
                                cm.includeEmail = defaultMatterConfig.IsEmailOptionSelected;
                            }
                            if (defaultMatterConfig.IsRSSSelected) {
                                cm.includeRssFeeds = defaultMatterConfig.IsRSSSelected;
                            }
                            else {
                                cm.includeRssFeeds = defaultMatterConfig.IsRSSSelected;
                            }
                            if (defaultMatterConfig.IsConflictCheck) {
                                cm.defaultConfilctCheck = defaultMatterConfig.IsConflictCheck;
                                cm.conflictRadioCheck = cm.defaultConfilctCheck;
                                cm.secureMatterRadioEnabled = cm.defaultConfilctCheck;
                            }
                            else {
                                cm.defaultConfilctCheck = defaultMatterConfig.IsConflictCheck;
                                cm.secureMatterRadioEnabled = cm.defaultConfilctCheck;
                                cm.conflictRadioCheck = cm.defaultConfilctCheck;

                            }
                            if (defaultMatterConfig.IsMatterDescriptionMandatory) {
                                cm.isMatterDescriptionMandatory = defaultMatterConfig.IsMatterDescriptionMandatory;
                            }
                            else {
                                cm.isMatterDescriptionMandatory = defaultMatterConfig.IsMatterDescriptionMandatory;
                            }
                            // if (defaultMatterConfig.IsContentCheck) {
                            // cm.secureMatterCheck = "True";
                            // }
                            if (defaultMatterConfig.IsTaskSelected) {
                                cm.includeTasks = defaultMatterConfig.IsTaskSelected;
                            }
                            else {
                                cm.includeTasks = defaultMatterConfig.IsTaskSelected;
                            }
                            var arrDMatterAreaOfLaw = [];
                            var arrDMatterPracticeGroup = [], arrDMatterUsers = [], arrDMatterUserEmails = [], arrDMatterPermissions = [], arrDMatterRoles = [];
                            arrDMatterAreaOfLaw = defaultMatterConfig.MatterAreaofLaw.split('$|$');
                            arrDMatterPracticeGroup = defaultMatterConfig.MatterPracticeGroup.split('$|$');
                            //     dMatterAreaOfLaw = defaultMatterConfig.MatterAreaofLaw ? defaultMatterConfig.MatterAreaofLaw : "";
                            //   dMatterPracticeGroup = defaultMatterConfig.MatterPracticeGroup?defaultMatterConfig.MatterPracticeGroup: "";
                            //   dMatterSubAreOfLaw = defaultMatterConfig.?: "";
                            dMatterTypes = defaultMatterConfig.MatterTypes ? defaultMatterConfig.MatterTypes : "";
                            cm.showRoles = defaultMatterConfig.ShowRole;
                            cm.showMatterId = defaultMatterConfig.ShowMatterId;
                            cm.matterIdType = defaultMatterConfig.MatterIdType;
                            setMatterId(cm.matterIdType);
                            var arrDMatterTypes = dMatterTypes.split('$|$');
                            dPrimaryMatterType = defaultMatterConfig.DefaultMatterType ? defaultMatterConfig.DefaultMatterType : "";
                            dMatterUsers = defaultMatterConfig.MatterUsers ? defaultMatterConfig.MatterUsers : "";
                            arrDMatterUsers = dMatterUsers.split('$|$');
                            dMatterUserEmails = defaultMatterConfig.MatterUserEmails ? defaultMatterConfig.MatterUserEmails : "";
                            arrDMatterUserEmails = dMatterUserEmails.split('$|$');
                            dMatterPermissions = defaultMatterConfig.MatterPermissions ? defaultMatterConfig.MatterPermissions : "";
                            arrDMatterPermissions = dMatterPermissions.split('$|$');
                            dMatterRoles = defaultMatterConfig.MatterRoles ? defaultMatterConfig.MatterRoles : "";
                            arrDMatterRoles = dMatterRoles.split('$|$');
                            //cm.selectMatterType();
                            cm.popupContainer = "hide";

                            getMatterGUID();
                            cm.selectedDocumentTypeLawTerms = [];
                            cm.documentTypeLawTerms = [];
                            if (cm.taxonomyHierarchyLevels == 2) {
                                setDefaultTaxonomyHierarchyLeveTwo(arrDMatterTypes, dPrimaryMatterType);
                            }
                            if (cm.taxonomyHierarchyLevels == 3) {
                                setDefaultTaxonomyHierarchyLevelThree(arrDMatterTypes, dPrimaryMatterType);
                            }
                            if (cm.taxonomyHierarchyLevels == 4) {
                                setDefaultTaxonomyHierarchyLevelFour(arrDMatterTypes, dPrimaryMatterType);
                            }
                            if (cm.taxonomyHierarchyLevels == 5) {
                                setDefaultTaxonomyHierarchyLevelFive(arrDMatterTypes, dPrimaryMatterType);
                            }

                            cm.selectedConflictCheckUser = ""; cm.blockedUserName = ""; cm.conflictDate = "";
                            //     cm.assignPermissionTeams
                            // cm.assignPermissionTeams.splice(0, 1);
                            cm.assignPermissionTeams = [];
                            // cm.assignPermissionTeams = [{ assignedUser: '', assignedRole: '', assignedPermission: '', assigneTeamRowNumber: 1 }];

                            for (var aCount = 0; aCount < arrDMatterUsers.length; aCount++) {
                                var assignPermTeam = {};
                                if ("" !== arrDMatterUsers[aCount]) {
                                    arrDMatterUsers[aCount] = arrDMatterUsers[aCount].replace(/\;$/, '');
                                    arrDMatterUserEmails[aCount] = arrDMatterUserEmails[aCount].replace(/\;$/, '');
                                    assignPermTeam.assignedUser = arrDMatterUsers[aCount] + "(" + arrDMatterUserEmails[aCount] + ")";
                                    if (-1 == cm.oSiteUsers.indexOf(arrDMatterUserEmails[aCount])) {
                                        cm.oSiteUsers.push(arrDMatterUserEmails[aCount]);
                                    }
                                }
                                else {
                                    assignPermTeam.assignedUser = "";
                                    assignPermTeam.assignedRole = cm.assignRoles[0];
                                    assignPermTeam.assignedPermission = cm.assignPermissions[0];
                                }


                                //cm.assignRoles   cm.assignPermissions 
                                //assignedRole  assignedPermission
                                angular.forEach(cm.assignRoles, function (assignRole) {
                                    if (arrDMatterRoles[aCount] == assignRole.name) {
                                        assignPermTeam.assignedRole = assignRole;
                                    }
                                });
                                angular.forEach(cm.assignPermissions, function (assignPermission) {
                                    if (arrDMatterPermissions[aCount] == assignPermission.name) {
                                        assignPermTeam.assignedPermission = assignPermission;
                                    }
                                });
                                assignPermTeam.assigneTeamRowNumber = aCount + 1;

                                cm.assignPermissionTeams.push(assignPermTeam);
                                cm.checkUserExists(assignPermTeam);
                            }




                        }
                        cm.popupContainerBackground = "hide";
                    });
                }
                else {
                    localStorage.iLivePage = cm.iCurrentPage = 1;
                    cm.matterName = "";
                    cm.matterId = "";
                    cm.matterDescription = "";
                    cm.selectedDocumentTypeLawTerms = [];
                    cm.documentTypeLawTerms = [];
                    cm.showMatterId = true;
                    cm.showRoles = true;
                    cm.matterIdType = "Custom";
                    cm.clientId = "";
                    cm.selectedClientName = undefined;

                }
            }

            function setMatterId(matterIDType) {
                if (matterIDType !== "") {
                    switch (matterIDType) {
                        case "Guid":
                            cm.matterId = cm.matterGUID;
                            break;
                        case "DateTime":
                            Date.prototype.yyyymmdd = function () {
                                var yyyy = this.getFullYear().toString();
                                var mm = (this.getMonth() + 1).toString(); // getMonth() is zero-based         
                                var dd = this.getDate().toString();
                                return yyyy + (mm[1] ? mm : "0" + mm[0]) + (dd[1] ? dd : "0" + dd[0]) + this.getHours().toString() + this.getMinutes().toString() + this.getSeconds().toString() + this.getMilliseconds().toString();;
                            };
                            var date = new Date();
                            cm.matterId = date.yyyymmdd();
                            break;
                        case "Custom":
                            cm.matterId = cm.matterId;
                            break;
                    }
                }
            }


            function setDefaultTaxonomyHierarchyLeveTwo(arrDMatterTypes, dPrimaryMatterType) {
                angular.forEach(cm.levelOneList, function (levelOneTerm) {
                    angular.forEach(levelOneTerm.level2, function (levelTwoTerm) {

                        for (var iCount = 0; iCount < arrDMatterTypes.length; iCount++) {

                            if (levelTwoTerm.termName == arrDMatterTypes[iCount]) {
                                //  cm.selectedDocumentTypeLawTerms = 
                                var documentType = levelTwoTerm;
                                documentType.levelOneFolderNames = levelOneTerm.folderNames;
                                documentType.levelOneTermId = levelOneTerm.id;
                                documentType.levelOneTermName = levelOneTerm.termName;
                                documentType.termChainName = levelOneTerm.termName;
                                if (cm.taxonomyHierarchyLevels >= 2) {
                                    documentType.levelTwoFolderNames = levelTwoTerm.folderNames;
                                    documentType.levelTwoTermId = levelTwoTerm.id;
                                    documentType.levelTwoTermName = levelTwoTerm.termName;
                                    documentType.termChainName = documentType.termChainName + ">" + documentType.levelTwoTermName;
                                }

                                cm.documentTypeLawTerms.push(documentType);
                                documentType.primaryMatterType = false;
                                if (levelTwoTerm.termName == dPrimaryMatterType) {
                                    documentType.primaryMatterType = true;
                                    cm.activeDocumentTypeLawTerm = levelTwoTerm;
                                }
                                cm.selectedDocumentTypeLawTerms.push(documentType);
                            }

                        }
                    });

                });

            }
            function setDefaultTaxonomyHierarchyLevelThree(arrDMatterTypes, dPrimaryMatterType) {
                angular.forEach(cm.levelOneList, function (levelOneTerm) {
                    angular.forEach(levelOneTerm.level2, function (levelTwoTerm) {

                        angular.forEach(levelTwoTerm.level3, function (levelThreeTerm) {

                            for (var iCount = 0; iCount < arrDMatterTypes.length; iCount++) {

                                if (levelThreeTerm.termName == arrDMatterTypes[iCount]) {
                                    //  cm.selectedDocumentTypeLawTerms = 
                                    var documentType = levelThreeTerm;
                                    documentType.levelOneFolderNames = levelOneTerm.folderNames;
                                    documentType.levelOneTermId = levelOneTerm.id;
                                    documentType.levelOneTermName = levelOneTerm.termName;
                                    documentType.termChainName = levelOneTerm.termName;
                                    if (cm.taxonomyHierarchyLevels >= 2) {
                                        documentType.levelTwoFolderNames = levelTwoTerm.folderNames;
                                        documentType.levelTwoTermId = levelTwoTerm.id;
                                        documentType.levelTwoTermName = levelTwoTerm.termName;
                                        documentType.termChainName = documentType.termChainName + ">" + documentType.levelTwoTermName;
                                    }
                                    if (cm.taxonomyHierarchyLevels >= 3) {
                                        documentType.levelThreeFolderNames = levelThreeTerm.folderNames;
                                        documentType.levelThreeId = levelThreeTerm.id;
                                        documentType.levelThreeTermName = levelThreeTerm.termName;
                                        documentType.termChainName = documentType.termChainName + ">" + documentType.levelThreeTermName;
                                    }
                                    cm.documentTypeLawTerms.push(documentType);
                                    documentType.primaryMatterType = false;
                                    if (levelThreeTerm.termName == dPrimaryMatterType) {
                                        documentType.primaryMatterType = true;
                                        cm.activeDocumentTypeLawTerm = levelThreeTerm;
                                    }
                                    cm.selectedDocumentTypeLawTerms.push(documentType);
                                }

                            }
                        });

                    });




                });
            }
            function setDefaultTaxonomyHierarchyLevelFour(arrDMatterTypes, dPrimaryMatterType) {
                angular.forEach(cm.levelOneList, function (levelOneTerm) {
                    angular.forEach(levelOneTerm.level2, function (levelTwoTerm) {

                        angular.forEach(levelTwoTerm.level3, function (levelThreeTerm) {
                            angular.forEach(levelThreeTerm.level4, function (levelFourTerm) {
                                for (var iCount = 0; iCount < arrDMatterTypes.length; iCount++) {

                                    if (levelFourTerm.termName == arrDMatterTypes[iCount]) {
                                        //  cm.selectedDocumentTypeLawTerms = 
                                        var documentType = levelFourTerm;
                                        documentType.levelOneFolderNames = levelOneTerm.folderNames;
                                        documentType.levelOneTermId = levelOneTerm.id;
                                        documentType.levelOneTermName = levelOneTerm.termName;
                                        documentType.termChainName = levelOneTerm.termName;
                                        if (cm.taxonomyHierarchyLevels >= 2) {
                                            documentType.levelTwoFolderNames = levelTwoTerm.folderNames;
                                            documentType.levelTwoTermId = levelTwoTerm.id;
                                            documentType.levelTwoTermName = levelTwoTerm.termName;
                                            documentType.termChainName = documentType.termChainName + ">" + documentType.levelTwoTermName;
                                        }
                                        if (cm.taxonomyHierarchyLevels >= 3) {
                                            documentType.levelThreeFolderNames = levelThreeTerm.folderNames;
                                            documentType.levelThreeId = levelThreeTerm.id;
                                            documentType.levelThreeTermName = levelThreeTerm.termName;
                                            documentType.termChainName = documentType.termChainName + ">" + documentType.levelThreeTermName;
                                        }
                                        if (cm.taxonomyHierarchyLevels >= 4) {
                                            documentType.levelFourFolderNames = levelFourTerm.folderNames;
                                            documentType.levelFourId = levelFourTerm.id;
                                            documentType.levelFourTermName = levelFourTerm.termName;
                                            documentType.termChainName = documentType.termChainName + ">" + documentType.levelFourTermName;
                                        }

                                        cm.documentTypeLawTerms.push(documentType);
                                        documentType.primaryMatterType = false;
                                        if (levelFourTerm.termName == dPrimaryMatterType) {
                                            documentType.primaryMatterType = true;
                                            cm.activeDocumentTypeLawTerm = levelFourTerm;
                                        }
                                        cm.selectedDocumentTypeLawTerms.push(documentType);
                                    }

                                }
                            });

                        });
                    });
                });
            }
            function setDefaultTaxonomyHierarchyLevelFive(arrDMatterTypes, dPrimaryMatterType) {
                angular.forEach(cm.levelOneList, function (levelOneTerm) {
                    angular.forEach(levelOneTerm.level2, function (levelTwoTerm) {

                        angular.forEach(levelTwoTerm.level3, function (levelThreeTerm) {
                            angular.forEach(levelThreeTerm.level4, function (levelFourTerm) {
                                angular.forEach(levelFourTerm.level5, function (levelFiveTerm) {
                                    for (var iCount = 0; iCount < arrDMatterTypes.length; iCount++) {

                                        if (levelFiveTerm.termName == arrDMatterTypes[iCount]) {
                                            //  cm.selectedDocumentTypeLawTerms = 
                                            var documentType = levelFiveTerm;
                                            documentType.levelOneFolderNames = levelOneTerm.folderNames;
                                            documentType.levelOneTermId = levelOneTerm.id;
                                            documentType.levelOneTermName = levelOneTerm.termName;
                                            documentType.termChainName = levelOneTerm.termName;
                                            if (cm.taxonomyHierarchyLevels >= 2) {
                                                documentType.levelTwoFolderNames = levelTwoTerm.folderNames;
                                                documentType.levelTwoTermId = levelTwoTerm.id;
                                                documentType.levelTwoTermName = levelTwoTerm.termName;
                                                documentType.termChainName = documentType.termChainName + ">" + documentType.levelTwoTermName;
                                            }
                                            if (cm.taxonomyHierarchyLevels >= 3) {
                                                documentType.levelThreeFolderNames = levelThreeTerm.folderNames;
                                                documentType.levelThreeId = levelThreeTerm.id;
                                                documentType.levelThreeTermName = levelThreeTerm.termName;
                                                documentType.termChainName = documentType.termChainName + ">" + documentType.levelThreeTermName;
                                            }
                                            if (cm.taxonomyHierarchyLevels >= 4) {
                                                documentType.levelFourFolderNames = levelFourTerm.folderNames;
                                                documentType.levelFourId = levelFourTerm.id;
                                                documentType.levelFourTermName = levelFourTerm.termName;
                                                documentType.termChainName = documentType.termChainName + ">" + documentType.levelFourTermName;
                                            }
                                            if (cm.taxonomyHierarchyLevels >= 5) {
                                                documentType.levelFiveFolderNames = levelFiveTerm.folderNames;
                                                documentType.levelFiveId = levelFiveTerm.id;
                                                documentType.levelFiveTermName = levelFiveTerm.termName;
                                                documentType.termChainName = documentType.termChainName + ">" + documentType.levelFiveTermName;
                                            }


                                            cm.documentTypeLawTerms.push(documentType);
                                            documentType.primaryMatterType = false;
                                            if (levelFiveTerm.termName == dPrimaryMatterType) {
                                                documentType.primaryMatterType = true;
                                                cm.activeDocumentTypeLawTerm = levelFiveTerm;
                                            }
                                            cm.selectedDocumentTypeLawTerms.push(documentType);
                                        }

                                    }
                                });

                            });

                        });
                    });


                });
            }

            //function to get the all AOL and SAOL terms
            //cm.getSelectedPracticeGroupValue = function () {


            //    if (cm.selectedLevelOne != null) {
            //        cm.levelTwoList = cm.selectedLevelOne.level2;
            //        cm.levelThreeList = cm.selectedLevelOne.level2[0].level3[0];
            //      //  cm.activeSubAOLTerm = cm.selectedLevelOne.level2[0].level3[0];
            //       // cm.activeAOLTerm = cm.selectedLevelOne.level2[0];
            //        cm.errorPopUp = false;
            //    } else {
            //       // cm.areaOfLawTerms = cm.subAreaOfLawTerms = null;

            //    }

            //}

            //function to get the subAOL items on selection of AOLTerm
            //cm.selectAreaOfLawTerm = function (areaOfLawTerm) {
            //    cm.errorPopUp = false;
            //    cm.activeAOLTerm = areaOfLawTerm;
            //  //  cm.subAreaOfLawTerms = areaOfLawTerm.subareaTerms;
            //   // cm.activeSubAOLTerm = areaOfLawTerm.subareaTerms[0];

            //}
            ////function to for seclection of subAOL items 
            //cm.selectSubAreaOfLawTerm = function (subAreaOfLawTerm) {
            //    cm.errorPopUp = false;
            //  //  cm.activeSubAOLTerm = subAreaOfLawTerm;

            //}

            //cm.selectDocumentTemplateTypeLawTerm = function (documentTemplateTypeLawTerm) {
            //    // alert(documentTemplateTypeLawTerm);
            //    if (documentTemplateTypeLawTerm != null) {
            //        cm.errorPopUp = false;;
            //        cm.removeDTItem = true;
            //        cm.activeDocumentTypeLawTerm = documentTemplateTypeLawTerm;
            //        cm.primaryMatterType = true;
            //    }

            //}

            //cm.addToDocumentTemplate = function () {
            //    var isThisNewDocTemplate = true;
            //    if (cm.activeSubAOLTerm != null) {
            //        angular.forEach(cm.documentTypeLawTerms, function (term) { //For loop
            //            if (cm.activeSubAOLTerm.id == term.id) {// this line will check whether the data is existing or not
            //                isThisNewDocTemplate = false;
            //            }
            //        });
            //        if (isThisNewDocTemplate) {                        
            //            var documentType = cm.activeSubAOLTerm;
            //            documentType.foldernamespg = cm.selectedLevelOne.folderNames;
            //            documentType.practicegroupId = cm.selectedLevelOne.id;
            //            documentType.foldernamesaol = cm.activeAOLTerm.folderNames;
            //            documentType.areaoflawId = cm.activeAOLTerm.id;
            //            documentType.areaoflaw = cm.activeAOLTerm.termName;
            //            documentType.practicegroup = cm.selectedLevelOne.termName;



            //            cm.documentTypeLawTerms.push(documentType);
            //            cm.activeDocumentTypeLawTerm = null;
            //            //   console.log("doc");
            //            //   console.log(cm.documentTypeLawTerms)
            //            //cm.primaryMatterType = true; alert(cm.primaryMatterType);
            //            //  cm.activeSubAOLTerm = null;
            //        }
            //    }
            //}

            //cm.removeFromDocumentTemplate = function () {
            //    //  alert(cm.activeDocumentTypeLawTerm);
            //    if (cm.removeDTItem) {
            //        var index = cm.documentTypeLawTerms.indexOf(cm.activeDocumentTypeLawTerm);
            //        cm.documentTypeLawTerms.splice(index, 1);
            //        cm.removeDTItem = false;
            //        cm.primaryMatterType = false;
            //        cm.activeDocumentTypeLawTerm = null;
            //    }

            //}
            cm.checkValidMatterName = function () {
                oPageOneState.oValidMatterName = undefined;
                var bInValid = false;
                var RegularExpression = new RegExp(oPageOneState.specialCharacterExpressionMatter);
                var sCurrMatterName = cm.matterName.trim();
                if (null !== sCurrMatterName && "" !== sCurrMatterName) {
                    var arrValidMatch = sCurrMatterName.match(RegularExpression);
                    if (null === arrValidMatch || arrValidMatch[0] !== sCurrMatterName) {
                        bInValid = false;
                    } else {
                        bInValid = true;
                    }
                }
                if (bInValid) {
                    optionsForCheckMatterName.Matter.Name = cm.matterName.trim();
                    optionsForCheckMatterName.Client.Url = cm.clientUrl;
                    getCheckValidMatterName(optionsForCheckMatterName, function (response) {
                        cm.errorPopUpBlock = false;
                        cm.errorBorder = "";
                        if (response.code != 200) {
                            if (cm.iCurrentPage == 1) {
                                cm.errTextMsg = cm.createContent.ErrorMessageEntityLibraryCreated;
                                        //"Matter library for this Matter is already created. Kindly delete the library or please enter a different Matter name.";
                                cm.errorBorder = "mattername"; showErrorNotification("mattername");
                                cm.errorPopUpBlock = true;
                                oPageOneState.oValidMatterName = false;
                            }
                            return false;
                        } else {
                            //  alert("success");
                            oPageOneState.oValidMatterName = true;
                            if (oPageOneState.isNextClick) {
                                cm.navigateToSecondSection(oPageOneState.sectionClickName);
                            }
                            return true;
                        }

                    });
                }

            }





            cm.navigateToSecondSection = function (sectionName) {
                cm.errorPopUpBlock = false;
                cm.errorBorder = "";
                cm.notificationPopUpBlock = false;
                cm.notificationBorder = "";
                oPageOneState.sectionClickName = sectionName;
                if (sectionName == "snConflictCheck" && cm.iCurrentPage !== 2) {
                    if (validateCurrentPage(cm.iCurrentPage)) {
                        cm.sectionName = sectionName;
                        oPageOneState.isNextClick = false;
                        cm.iCurrentPage = 2;
                        localStorage.iLivePage = 2;
                        makePrevOrNextButton();
                    }
                }
                else if (sectionName == "snCreateAndShare" && cm.iCurrentPage !== 3) {

                    if (validateCurrentPage(cm.iCurrentPage)) {

                       
                        if (cm.iCurrentPage == 2) {
                            callCheckSecurityGroupExists("snCreateAndShare");
                        } else {
                            cm.sectionName = sectionName;
                            cm.iCurrentPage = 3;
                            localStorage.iLivePage = 3;
                            makePrevOrNextButton();
                    }

                    }

                }
                else if (sectionName == "snOpenMatter" && cm.iCurrentPage !== 1) {
                    cm.iCurrentPage = 1; cm.sectionName = sectionName;
                    localStorage.iLivePage = 1;
                    makePrevOrNextButton();
                }
                else {
                    cm.sectionName = sectionName;
                    makePrevOrNextButton();
                }
            }

            function makePrevOrNextButton() {
                switch (cm.iCurrentPage) {
                    case 1:
                        cm.prevButtonDisabled = true;
                        cm.nextButtonDisabled = false;
                        break;
                    case 2:
                        cm.prevButtonDisabled = false;
                        cm.nextButtonDisabled = false;
                        break;
                    case 3:
                        cm.prevButtonDisabled = false;
                        cm.nextButtonDisabled = true;
                        break;
                }
            }


            var validateAttornyUserRolesAndPermissins = function () {
                var responsibleAttorny = 0, fullControl = 0,teamRowNumber=1;
                if (!cm.showRoles) {
                    assignDefaultRolesToTeamMembers();
                }
                for (var iCount = 0; iCount < cm.assignPermissionTeams.length; iCount++) {
                    teamRowNumber = iCount == 0 ? cm.assignPermissionTeams[iCount].assigneTeamRowNumber : teamRowNumber;
                    if ("" !== cm.assignPermissionTeams[iCount].assignedUser) {

                        if (cm.assignPermissionTeams[iCount].assignedRole && "" !== cm.assignPermissionTeams[iCount].assignedRole.name) {
                            if (cm.assignPermissionTeams[iCount].assignedPermission && "" != cm.assignPermissionTeams[iCount].assignedPermission.name) {
                                if (cm.assignPermissionTeams[iCount].assignedRole.mandatory) {
                                    responsibleAttorny++;
                                }
                                if (cm.assignPermissionTeams[iCount].assignedPermission.name == "Full Control") {
                                    fullControl++;
                                }

                            }
                            else {
                                cm.errTextMsg = cm.createContent.ErrorMessageEntityPermission;
                                    //"Please provide at least one permission on this  matter. ";
                                cm.errorBorder = "";
                                cm.errorPopUpBlock = true;

                                return false;
                            }
                        }
                        else {
                            cm.errorPopUpBlock = true;
                            cm.errTextMsg = cm.createContent.ErrorMessageEntityTeamRole1;
                                //"Enter at least one role for this matter.";
                            cm.errorBorder = "";
                            return false;
                        }
                    }
                    else {
                        cm.errTextMsg = cm.createContent.ErrorMessageTeamMember1;
                        // cm.assignPermissionTeams[iCount].assignedRole.name + " cannot be empty.";
                        cm.errorBorder = "";
                        showErrorNotificationAssignTeams(cm.errTextMsg, cm.assignPermissionTeams[iCount].assigneTeamRowNumber, "user");
                        cm.errorPopUpBlock = true;
                        return false;
                    }
                }             
                if (responsibleAttorny >= 1) {
                    if (fullControl >= 1) {
                        return true;
                    }
                    else {
                        cm.errTextMsg = cm.createContent.ErrorMessageEntityTeamPermission2;
                            //"Please provide at least one user who has Full Control permission on this  matter.";
                        cm.errorBorder = "permUser" + teamRowNumber;
                        showErrorNotificationAssignTeams(cm.errTextMsg, teamRowNumber, "perm");
                        cm.errorPopUpBlock = true;
                        return false;
                    }
                }
                else {
                    cm.errTextMsg = cm.createContent.ErrorMessageEntityTeamRole2;
                        //"Enter at least one Responsible Attorney for this matter.";
                    cm.errorBorder = "roleUser" + teamRowNumber;
                    showErrorNotificationAssignTeams(cm.errTextMsg, teamRowNumber, "role");
                    cm.errorPopUpBlock = true;
                    return false;
                }
            }
            function setTeamConfirmationValues() {
                angular.forEach(cm.assignPermissionTeams, function (team) {
                    if (team.userConfirmation) {
                        angular.element('#txtUser' + team.assigneTeamRowNumber).attr('confirm', "true");
                    }
                });
            }
            function validateCheckUserExisits() {
                var validUsers = false; var keepGoing = true;
                setTeamConfirmationValues();
                angular.forEach(cm.assignPermissionTeams, function (team) {
                    if (keepGoing) {
                        var userVal = angular.element('#txtUser' + team.assigneTeamRowNumber).attr('confirm');
                        if (userVal == "false") {
                            cm.textInputUser = team;
                            if (cm.iCurrentPage == 2) {
                                showNotificatoinMessages(team.assigneTeamRowNumber);
                                cm.notificationPopUpBlock = true;
                            }

                        }
                        validUsers = (userVal == "false") ? false : true;
                        if (!validUsers) {
                            keepGoing = false;
                        }
                    }
                });
                return validUsers;
            }


            function validateUsers() {
                var keepGoing = true;
                var username = "";
                if (cm.defaultConfilctCheck) {
                    if (undefined==cm.selectedConflictCheckUser || "" == cm.selectedConflictCheckUser) {
                        cm.errTextMsg = cm.createContent.ErrorMessageConflictUser;
                            //"Enter the conflict reviewers name (for auditing purposes).";
                        cm.errorBorder = "ccheckuser";
                        showErrorNotification("ccheckuser");
                        cm.errorPopUpBlock = true;
                        return false;
                    }
                    if (cm.conflictRadioCheck) {
                    if (undefined == cm.blockedUserName || "" == cm.blockedUserName) {
                        cm.errTextMsg = cm.createContent.ErrorMessageConflictUser1;
                            //"Enter users that are conflicted with this matter.";
                        cm.errorBorder = "cblockuser";
                        showErrorNotification("cblockuser");
                        cm.errorPopUpBlock = true;
                        return false;
                    }
                }
                }

                if (cm.selectedConflictCheckUser && "" !== cm.selectedConflictCheckUser) {
                    username = getUserName(cm.selectedConflictCheckUser + ";", false);
                    if (-1 == cm.oSiteUsers.indexOf(username[0])) {
                        //  cm.blockedUserName.trim()
                        cm.errTextMsg = cm.createContent.ErrorMessageConflictUser;
                            //"Enter the conflict reviewers name (for auditing purposes).";
                        cm.errorBorder = "ccheckuser";
                        showErrorNotification("ccheckuser");
                        cm.errorPopUpBlock = true;
                        return false;
                    }
                }
                if (cm.blockedUserName && "" !== cm.blockedUserName) {
                    username = getUserName(cm.blockedUserName + ";", false);
                    if (-1 == cm.oSiteUsers.indexOf(username[0])) {
                        //  cm.blockedUserName.trim()
                        cm.errTextMsg = cm.createContent.ErrorMessageConflictUser1;
                            //"Enter users that are conflicted with this matter.";
                        cm.errorBorder = "cblockuser";
                        showErrorNotification("cblockuser");
                        cm.errorPopUpBlock = true;
                        return false;
                    }
                }

                angular.forEach(cm.assignPermissionTeams, function (team) {
                    if (keepGoing) {
                        if (team.assignedUser && team.assignedUser != "") {//For loop
                            username = getUserName(team.assignedUser + ";", false)
                            if (team.userExsists) {
                            if (-1 == cm.oSiteUsers.indexOf(username[0])) {
                                //  cm.blockedUserName.trim()
                                cm.errTextMsg = cm.createContent.ErrorMessageEntityUsers1;
                                    //"Please enter valid team members.";
                                cm.errorBorder = "";
                                cm.errorPopUpBlock = true;
                                showErrorNotificationAssignTeams(cm.errTextMsg, team.assigneTeamRowNumber, "user")
                                cm.errorBorder = "txtUser" + team.assigneTeamRowNumber; keepGoing = false;
                                return false;

                            }
                            } else {
                                if (!team.userConfirmation) {
                                    cm.checkUserExists(team);
                                    if (!cm.invalidUserCheck) {
                                        keepGoing = false;
                                        return false;
                                    }
                                }
                            }

                            if (cm.blockedUserName && cm.blockedUserName != "") {
                                if (team.assignedUser == cm.blockedUserName) {
                                    cm.errTextMsg = cm.createContent.ErrorMessageEntityUsers2;
                                        //"Please enter individual who is not conflicted.";
                                    cm.errorBorder = "";
                                    cm.errorPopUpBlock = true;
                                    showErrorNotificationAssignTeams(cm.errTextMsg, team.assigneTeamRowNumber, "user")
                                    cm.errorBorder = "txtUser" + team.assigneTeamRowNumber; keepGoing = false;
                                    return false;
                                }
                            }
                        }
                        else {
                            
                            showErrorNotificationAssignTeams(cm.createContent.ErrorMessageTeamMember1, team.assigneTeamRowNumber, "user")
                          //  showErrorNotificationAssignTeams(team.assignedRole.name + " cannot be empty", team.assigneTeamRowNumber, "user")
                            cm.errorBorder = "txtUser" + team.assigneTeamRowNumber;
                            keepGoing = false;
                            return false;
                        }
                    }
                });

                if (keepGoing) {
                    return true;
                } else {
                    return false;
                }
            }

            //setting the team  roles to default i.e responsible attrony when showRole is false from default settings.
            function assignDefaultRolesToTeamMembers() {
                if (!cm.showRoles) {
                    var arrAssigneTeams = cm.assignPermissionTeams, nCount = 0, nlength;
                    if (arrAssigneTeams) {
                        nlength = arrAssigneTeams.length;
                        for (nCount = 0; nCount < nlength; nCount++) {
                            if (arrAssigneTeams[nCount] && arrAssigneTeams[nCount].assignedUser && "" !== arrAssigneTeams[nCount].assignedUser) {
                                angular.forEach(cm.assignRoles, function (role) {
                                    if (role.mandatory) {
                                        arrAssigneTeams[nCount].assignedRole = role;
                                    }
                                });
                            }
                        }
                    }

                }
            }



            cm.arrAssignedUserName = [], cm.arrAssignedUserEmails = [], cm.userIDs = [];

            function checkSecurityGroupExists(options, callback) {

                api({
                    resource: 'matterResource',
                    method: 'checkSecurityGroupExists',
                    data: options,
                    success: callback
                });
            }

            cm.externalusers = [];

            cm.onSelect = function ($item, $model, $label, value, fucnValue, $event, username) {
              
                var typeheadelelen = angular.element('.dropdown-menu li').length;
                var noresults = true;
                if (typeheadelelen == 1) {
                    if (angular.element('.dropdown-menu li a')[0]) {
                        if (angular.element('.dropdown-menu li a')[0].innerHTML == "No results found") {
                            noresults = false;
                            if ($event.keyCode == 9 || $event.keyCode == 13) {
                                cm.user = angular.element('#' + $event.currentTarget.id).val();
                            }
                        }
                    }
                }
                if ($item && $item.name !== "No results found") {
                    if (value == "conflictcheckuser") {
                        cm.selectedConflictCheckUser = $item.name + '(' + $item.email + ')';
                    }
                    if (value == "blockuser") {
                        cm.blockedUserName = $item.name + '(' + $item.email + ')';
                    }
                    if (value == "team") {
                        $label.assignedUser = $item.name + '(' + $item.email + ')';
                        cm.typehead = false;
                        cm.notificationPopUpBlock = false;
                    }
                    if (-1 == cm.oSiteUsers.indexOf($item.email)) {
                        cm.oSiteUsers.push($item.email);
                    }
                    if (value == "team") {
                        $label.userConfirmation = false;
                        cm.checkUserExists($label);
                    }

                }
                else {
                    if (fucnValue == "on-blurr") {
                        cm.user = username;
                    }
                    if (fucnValue == "on-blurr" && typeheadelelen == 0 && noresults) {
                        cm.checkUserExists($label, $event);
                    }
                    if (!noresults) {
                        if (value == "conflictcheckuser") {
                            $label = "";
                            cm.selectedConflictCheckUser = "";
                        }
                        if (value == "blockuser") {
                            $label = "";
                            cm.blockedUserName = "";
                        }
                        if (value == "team") {
                            $label.assignedUser = "";
                            $label.assignedUser = cm.user;
                        }
                      
                    }
                }
            }

            function getArrAssignedUserNamesAndEmails() {
                cm.arrAssignedUserName = [], cm.arrAssignedUserEmails = [], cm.userIDs = [];
                var count = 1;
                angular.forEach(cm.assignPermissionTeams, function (team) { //For loop
                    cm.arrAssignedUserName.push(getUserName(team.assignedUser + ";", true));
                    cm.arrAssignedUserEmails.push(getUserName(team.assignedUser + ";", false));
                    cm.userIDs.push("txtUser" + count++);
                });

            }


            var callCheckSecurityGroupExists = function (sectionName) {
                //console.log(cm.assignPermissionTeams);
                getArrAssignedUserNamesAndEmails();                
                var optionsForSecurityGroupCheck = {
                    Client: {

                        Url: cm.clientUrl
                    },
                    Matter: {
                        Name: cm.matterName.trim(),
                        AssignUserNames: cm.arrAssignedUserName,
                        AssignUserEmails: cm.arrAssignedUserEmails,
                        Conflict: {
                            Identified: cm.conflictRadioCheck
                        },
                        BlockUserNames: (undefined !== cm.blockedUserName && null !== cm.blockedUserName) ? getUserName(cm.blockedUserName.trim() + ";", false) : [],
                    },
                    UserIds: cm.userIDs
                }
                //optionsForSecurityGroupCheck.Matter.Name=cm.matterName.trim();
                //optionsForSecurityGroupCheck.Matter.AssignUserNames = cm.arrAssignedUserName;
                //optionsForSecurityGroupCheck.Matter.AssignUserEmails = cm.arrAssignedUserEmails;
                //optionsForSecurityGroupCheck.Matter.BlockUserNames = (undefined !== cm.blockedUserName && null !== cm.blockedUserName) ? getUserName(cm.blockedUserName.trim() + ";", false) : "";
                // var value = getUserName("Venkat M (venkatm@MSmatter.onmicrosoft.com);", true);
                // console.log(optionsForSecurityGroupCheck);

                checkSecurityGroupExists(optionsForSecurityGroupCheck, function (response) {
                    // console.log(response);                   
                    if (!response.value) {
                        cm.errTextMsg = response.message.split('$')[0];
                        cm.errorBorder = "";
                        cm.errorStatus = true;
                        cm.errorPopUpBlock = true;
                        var rowNumber= parseInt(response.message.split('$')[1].replace(/[^\d.]/g, ''),10);
                        cm.errorBorder = "txtUser" + rowNumber;
                        showErrorNotificationAssignTeams(cm.errTextMsg, rowNumber, "securityuser")
                        cm.popupContainerBackground = "hide";
                        cm.sectionName = "snConflictCheck";
                        cm.iCurrentPage = 2;
                        localStorage.iLivePage = 2;
                        makePrevOrNextButton();
                    } else {
                        cm.iCurrentPage = 3; cm.popupContainerBackground = "hide";
                        cm.sectionName = sectionName;
                    }

                });
            }
            var getUserName = function (sUserEmails, bIsName) {
                "use strict";
                var arrUserNames = [], sEmail = "", oEmailRegex = new RegExp("^[\\s]*\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*[\\s]*$");
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
            }

            //cm.saveDocumentTemplates = function () {

            //    if (cm.primaryMatterType) {
            //        cm.errorPopUp = false;
            //        angular.forEach(cm.documentTypeLawTerms, function (term) {
            //            var primaryType = false;
            //            //For loop
            //            if (cm.activeDocumentTypeLawTerm.id == term.id) {// this line will check whether the data is existing or not
            //                primaryType = true;
            //            }
            //            term.primaryMatterType = primaryType;
            //            cm.popupContainerBackground = "hide";
            //            cm.popupContainer = "hide";


            //        });

            //        cm.selectedDocumentTypeLawTerms = cm.documentTypeLawTerms;
            //    }
            //    else {
            //        cm.errorPopUp = true;
            //    }
            //}

            cm.dateOptions = {

                formatYear: 'yy',
                maxDate: new Date(),
                // minDate: new Date(),
                startingDay: 1
            };

            cm.open1 = function ($event) {
                if ($event) {
                    $event.preventDefault();
                    $event.stopPropagation();
                }
                cm.opened = cm.opened ? false : true;
            };

            cm.opened = false;
            cm.conflictDate = $filter('date')(cm.conflictDate, 'MM/dd/yyyy');


            //cm.$watch('cm.conflictCheckDate', function ()
            //{
            //    var val = 'MM\dd\yyyy';
            //    cm.conflictDate = $filter('date')(new Date(), val);
            //}, true);


            function convert(str) {
                var date = new Date(str),
                    mnth = ("0" + (date.getMonth() + 1)).slice(-2),
                    day = ("0" + date.getDate()).slice(-2);
                return [mnth, day, date.getFullYear(), ].join("/");
            }

            //$scope.SelectModal= function(){
            //   // jQuery('#myModal').modal('show');
            //}
            cm.secureMatterRadioEnabled = true;
            cm.conflictRadioCheckValue = true;
            cm.conflictRadioChange = function (value) {
                cm.blockedUserName = "";
                if (value) {
                    cm.secureMatterRadioEnabled = true;
                    cm.secureMatterCheck = true;
                }
                else {
                    cm.secureMatterRadioEnabled = false;
                }
            }


            cm.addNewAssignPermissions = function () {
                var newItemNo = cm.assignPermissionTeams.length + 1;
                cm.assignPermissionTeams.push({ 'assigneTeamRowNumber': newItemNo, 'assignedRole': cm.assignRoles[0], 'assignedPermission': cm.assignPermissions[0] });
            };

            cm.removeAssignPermissionsRow = function (index) {
                var remainingRows = cm.assignPermissionTeams.length;
                if (1 < remainingRows) {

                    cm.assignPermissionTeams.splice(index, 1);
                    //  console.log(cm.assignPermissionTeams);
                    //  console.log(cm.assignPermissionTeams.length);
                }
            };

            if (localStorage.getItem("iLivePage")) {
                if (localStorage.getItem("iLivePage") == 2 || localStorage.getItem("iLivePage") == 3) {
                    var oPageData = JSON.parse(localStorage.getItem("oPageOneData"));
                    cm.clientId = oPageData.ClientId;
                    cm.clientUrl = oPageData.Client.url;
                    cm.selectedClientName = oPageData.Client.name;
                    cm.selectedClient = oPageData.Client;
                    cm.matterName = oPageData.MatterName;
                    cm.matterId = oPageData.MatterId;
                    cm.matterDescription = oPageData.MatterDescription;

                    cm.clientNameList = [];
                    cm.showRoles = oPageData.showRoles;
                    cm.showMatterId = oPageData.showMatterId;
                    cm.matterIdType = oPageData.matterIdType;
                    //  cm.areaOfLawTerms = [];
                    //  cm.subAreaOfLawTerms = [];
                    //  cm.documentTypeLawTerms = [];
                    cm.selectedDocumentTypeLawTerms = [];
                    // cm.activeAOLTerm = null;
                    // cm.activeSubAOLTerm = null;
                    //  cm.activeDocumentTypeLawTerm = oPageData.oSelectedDocumentTypeLawTerms;
                    cm.selectedDocumentTypeLawTerms = cm.documentTypeLawTerms = oPageData.oSelectedDocumentTypeLawTerms;
                    cm.popupContainerBackground = "Show";
                    cm.popupContainer = "hide";
                    cm.sectionName = "";
                    cm.sectionName = "snConflictCheck";
                    cm.removeDTItem = false;
                    cm.primaryMatterType = cm.errorPopUp = false;
                    cm.matterGUID = oPageData.matterGUID;
                    cm.iCurrentPage = 2;
                    cm.isBackwardCompatible = oPageData.isBackwardCompatible;
                    cm.isClientMappedWithHierachy = oPageData.isClientMappedWithHierachy
                    cm.includeRssFeeds = (localStorage.getItem("IsRSSSelected") === "true");
                    cm.includeEmail = (localStorage.getItem("IsEmailOptionSelected") === "true");
                    cm.includeCalendar = (localStorage.getItem("IsCalendarSelected") === "true");
                    cm.isMatterDescriptionMandatory = (localStorage.getItem("IsMatterDescriptionMandatory") === "true");
                    cm.defaultConfilctCheck = (localStorage.getItem("IsConflictCheck") === "true");
                    cm.includeTasks = (localStorage.getItem("IsTaskSelected") === "true");
                    cm.secureMatterCheck = (localStorage.getItem("IsRestrictedAccessSelected") === "true");               
                    if (cm.includeEmail) {
                        cm.createButton = "Create and Notify";
                    }
                    oPageOneState.oValidMatterName = oPageData.oValidMatterName;
                    cm.nextButtonDisabled = false; cm.prevButtonDisabled = false;
                    // cm.navigateToSecondSection("");
                    // cm.navigateToSecondSection("snConflictCheck");


                }
                if (localStorage.getItem("iLivePage") == 3) {
                    cm.iCurrentPage = 1;
                    var oPageData = JSON.parse(localStorage.getItem("oPageTwoData"));
                    if (oPageData && oPageData !== null) {
                        cm.chkConfilctCheck = oPageData.ChkConfilctCheck;
                        cm.selectedConflictCheckUser = oPageData.SelectedConflictCheckUser;                       
                        cm.conflictDate = oPageData.ConflictDate;
                        cm.conflictDate = $filter('date')(cm.conflictDate, 'MM/dd/yyyy');
                        cm.conflictDate = new Date(cm.conflictDate);
                        cm.conflictRadioCheck = oPageData.ConflictRadioCheck;
                        cm.blockedUserName = oPageData.BlockedUserName;
                        cm.secureMatterCheck = oPageData.SecureMatterCheck;
                        cm.secureMatterCheck = (localStorage.getItem("IsRestrictedAccessSelected") === "true");
                        cm.assignPermissionTeams = oPageData.AssignPermissionTeams;
                        cm.oSiteUsers = oPageData.oSiteUsers; cm.nextButtonDisabled = true;
                        cm.iCurrentPage = 3;
                    }
                    if (cm.includeEmail) {
                        cm.createButton = "Create and Notify";
                    }

                    cm.sectionName = "snCreateAndShare";


                }

            }

            cm.createAndNotify = function (value) {
                if (value) {
                    cm.createButton = "Create and Notify";
                }
                else {
                    cm.createButton = "Create";
                }
            }
            cm.menuClick = function () {
                var oAppMenuFlyout = $(".AppMenuFlyout");
                if (!(oAppMenuFlyout.is(":visible"))) {
                    //// Display the close icon and close the fly out 
                    $(".OpenSwitcher").addClass("hide");
                    $(".CloseSwitcher").removeClass("hide");
                    $(".MenuCaption").addClass("hideMenuCaption");
                    oAppMenuFlyout.slideDown();
                } else {
                    oAppMenuFlyout.slideUp();
                    $(".CloseSwitcher").addClass("hide");
                    $(".OpenSwitcher").removeClass("hide");
                    $(".MenuCaption").removeClass("hideMenuCaption");
                }
            }

            function showNotificatoinMessages(teamRowNumber) {
                var temp = document.getElementById('txtUser' + teamRowNumber);
                var notificationEle = document.getElementById("notificationBlock");
                var notificationTrinageleBlockEle = document.getElementById("notificatoinTrinagleBlock");
                var notificationTrinagleBorderEle = document.getElementById("notificationTrinagleBroderBlock");
                var notificationTextEle = document.getElementById("notificationText");
                notificationEle.className = ""; notificationTrinageleBlockEle.className = ""; notificationTrinagleBorderEle.className = ""; notificationTextEle.className = "";
                notificationEle.classList.add("notificationPopUp");
                notificationTrinageleBlockEle.classList.add("notificatonTriangle");
                notificationTrinageleBlockEle.classList.add("popUpFloatLeft");
                notificationTrinagleBorderEle.classList.add("notificationTriangleBorder");
                notificationTrinagleBorderEle.classList.add("popUpFloatLeft");
                notificationTextEle.classList.add("notificatonText");
                notificationTextEle.classList.add("popUpFloatRight");
                var notifcationPopUpCAttorny = document.createElement('style'),
                    notifcationTringleBlockCAttorny = document.createElement('style'),
                    notifcationTringleBorderCAttorny = document.createElement('style'),
                    notifcationTextMatterCAttorny = document.createElement('style');
                notifcationPopUpCAttorny.type = 'text/css';
                notifcationTringleBlockCAttorny.type = 'text/css';
                notifcationTringleBorderCAttorny.type = 'text/css';
                notifcationTextMatterCAttorny.type = 'text/css';

                var width = GetWidth();
                var x = 0, y = 0;
                if (width > 734) {
                    y = temp.offsetTop - 50, x = temp.offsetLeft + 85;
                }
                else {
                    y = temp.offsetTop + 32, x = temp.offsetLeft + 78;
                }
                cm.notificationBorder = "txtUser" + teamRowNumber;

                notifcationPopUpCAttorny.innerHTML = ".notifcationPopUpCAttorny{top:" + y + "px;left:" + x + "px;}";
                notifcationTringleBlockCAttorny.innerHTML = "{min-height: 40px;top: 17px !important;left: 24px;width:100%}";
                notifcationTringleBorderCAttorny.innerHTML = "{min-height: 40px,top: 17px !important;left: 24px;width:100%}";
                notifcationTextMatterCAttorny.innerHTML = "{min-height:40px;top:21px !important;left: 24px;width:100%}";
                document.getElementsByTagName('head')[0].appendChild(notifcationPopUpCAttorny);
                document.getElementsByTagName('head')[0].appendChild(notifcationTringleBlockCAttorny);
                document.getElementsByTagName('head')[0].appendChild(notifcationTringleBorderCAttorny);
                document.getElementsByTagName('head')[0].appendChild(notifcationTextMatterCAttorny);
                // cm.errTextMsg = errorMsg;

                //  cm.errorPopUpBlock = true;
                notificationEle.classList.add("notifcationPopUpCAttorny");
                notificationTrinageleBlockEle.classList.add("notifcationTringleBlockCAttorny");
                notificationTrinagleBorderEle.classList.add("notifcationTringleBorderCAttorny");
                notificationTextEle.classList.add("notifcationTextCAttorny");
            }

            cm.confirmUser = function (confirmUser) {
                if (confirmUser) {
                    cm.notificationPopUpBlock = false;
                    cm.notificationBorder = "";
                    cm.textInputUser.userConfirmation = true;
                    angular.element('#txtUser' + cm.textInputUser.assigneTeamRowNumber).attr('uservalue', cm.textInputUser.assignedUser);
                    angular.element('#txtUser' + cm.textInputUser.assigneTeamRowNumber).attr('confirm', "true");
                    angular.element('#txtUser' + cm.textInputUser.assigneTeamRowNumber).css('border-color', '#ccc');
                } else {
                    cm.notificationPopUpBlock = false;
                    cm.textInputUser.assignedUser = "";
                    cm.textInputUser.userExsists = false;
                    cm.textInputUser.userConfirmation = false;
                    cm.notificationBorder = "";
                }
            }

            cm.checkUserExists = function (teamDetails, $event) {
                var userMailId = teamDetails.assignedUser;
                if ($event) {
                    $event.stopPropagation();
                }
                function validateEmail(email) {
                    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                    return re.test(email);
                }
                function validate(email) {

                    if (validateEmail(email)) {
                        var checkEmailExists = false;
                        if (cm.textInputUser && cm.textInputUser != "") {
                            var oldUserEmail = angular.element('#txtUser' + teamDetails.assigneTeamRowNumber).attr('uservalue');
                            if (oldUserEmail !== email) {
                                checkEmailExists = true;
                                teamDetails.userConfirmation = false;
                            }
                            else {
                                teamDetails.userConfirmation = teamDetails.userConfirmation;
                                cm.invalidUserCheck = true;
                            }

                        } else {
                            checkEmailExists = true;
                        }
                        if (checkEmailExists) {
                            var optionsForUserExsists = new Object;
                            optionsForUserExsists = {
                                Url: cm.clientUrl,
                                Name: email
                            }
                            userexists(optionsForUserExsists, function (response) {
                                if (!response.isUserExistsInSite) {
                                    angular.forEach(cm.assignPermissionTeams, function (team) {
                                        var userEmail = getUserName(team.assignedUser + ";", false)
                                        if (userEmail[0] == email && team.assigneTeamRowNumber == teamDetails.assigneTeamRowNumber) {
                                            cm.textInputUser = team;
                                            team.userExsists = response.isUserExistsInSite;
                                            team.userConfirmation = false;
                                            if (cm.iCurrentPage == 2) {
                                                showNotificatoinMessages(team.assigneTeamRowNumber);
                                            }
                                            return false;
                                        }
                                    });
                                    if (cm.iCurrentPage == 2) {
                                        cm.notificationPopUpBlock = true;
                                    }
                                }
                                else {
                                    cm.notificationPopUpBlock = false;
                                    angular.forEach(cm.assignPermissionTeams, function (team) {
                                        var userEmail = getUserName(team.assignedUser + ";", false)
                                        if (userEmail[0] == email) {
                                            cm.textInputUser = team;
                                            team.userExsists = response.isUserExistsInSite;
                                            team.userConfirmation = true;
                                            cm.confirmUser(true);
                                        }
                                    });
                                }
                            });
                        }
                    }
                    else {
                        angular.forEach(cm.assignPermissionTeams, function (team) {
                            var userEmail = getUserName(team.assignedUser + ";", false)
                            if (userEmail[0] == email) {
                                cm.errTextMsg = cm.createContent.ErrorMessageEntityUsers3;
                                 //   "Please enter a valid email address.";
                                cm.errorBorder = "";
                                cm.errorStatus = true;
                                cm.errorPopUpBlock = true;
                                showErrorNotificationAssignTeams(cm.errTextMsg, team.assigneTeamRowNumber, "user")
                                team.userConfirmation = false;
                                angular.element('#txtUser' + team.assigneTeamRowNumber).attr('confirm', "false");
                                cm.errorBorder = "txtUser" + team.assigneTeamRowNumber;
                                return false;
                            }

                        });
                        cm.invalidUserCheck = false;

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

            function userexists(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'userexists',
                    data: options,
                    success: callback
                });
            }

            cm.createMatterButton = function ($event) {
                getMatterGUID();
                var isPageValid = validateCurrentPage(1);
                if (isPageValid) {
                    isPageValid = validateCurrentPage(2);
                } else {
                    cm.sectionName = "snOpenMatter";
                    cm.iCurrentPage = 1;
                }

                if (isPageValid) {
                    //
                    if (cm.isBackwardCompatible) {
                        var team = {};
                        team.assigneTeamRowNumber = cm.assignPermissionTeams.length + 1;
                        team.assignedUser = adalService.userInfo.profile.given_name + ' ' + adalService.userInfo.profile.family_name + '(' + adalService.userInfo.userName + ')';
                        team.userConfirmation = true;
                        team.userExsists = true;

                        angular.forEach(cm.assignRoles, function (assignRole) {
                            if (assignRole.mandatory) {
                                team.assignedRole = assignRole;
                            }
                        });
                        angular.forEach(cm.assignPermissions, function (assignPermission) {
                            if (assignPermission.name == "Full Control") {
                                team.assignedPermission = assignPermission;
                            }
                        });
                        cm.assignPermissionTeams.push(team);

                    }
                    //

                    cm.popupContainerBackground = "hide";
                    var matterGUID = cm.matterGUID;
                    var arrFolderNames = [];
                    arrFolderNames = retrieveFolderStructure();
                    var arrRoles = [];
                    arrRoles = getAssignedUserRoles();
                    var arrPermissions = [];
                    arrPermissions = getAssignedUserPermissions();
                    var contentTypes = [];
                    var defaultContentType = "";
                    cm.conflictDate = $filter('date')(cm.conflictDate, 'MM/dd/yyyy');
                    contentTypes = getDefaultContentTypeValues("contenttypes");
                    cm.bMatterLandingPage = true;
                    defaultContentType = getDefaultContentTypeValues("defaultcontenttype");
                    var sCheckByUserEmail = (undefined !== cm.selectedConflictCheckUser && null !== cm.selectedConflictCheckUser) ? getUserName(cm.selectedConflictCheckUser.trim() + ";", false) : "";
                    var sCheckBy = getUserEmail(sCheckByUserEmail);
                    var sBlockUserEmail = (undefined !== cm.blockedUserName && null !== cm.blockedUserName) ? getUserName(cm.blockedUserName.trim() + ";", false) : [];
                    var sBlockUserName = sBlockUserEmail;
                    var bValid = false;
                    if (cm.defaultConfilctCheck) {
                        if ("" !== sCheckBy) {
                            if (cm.conflictRadioCheck) {
                                if ("" != sBlockUserName[0]) {
                                    bValid = true;
                                }
                                else {
                                    bValid = false;
                                }
                            } else {
                                bValid = true;
                            }

                        }
                        else {
                            cm.popupContainerBackground = "hide";
                            cm.errTextMsg = cm.createContent.ErrorMessageEntityCreation1;
                                //"Error in creation of matter: Incorrect inputs.";
                            showErrorNotificationAssignTeams(cm.errTextMsg, "", "btnCreateMatter");
                            cm.errorBorder = "";
                            cm.errorPopUpBlock = true;
                            cm.popupContainerBackground = "hide";
                            $event.stopPropagation(); cm.createBtnDisabled = false; cm.successBanner = false;
                            bValid = false;
                            cm.conflictDate = $filter('date')(cm.conflictDate, 'MM/dd/yyyy');
                            cm.conflictDate = new Date(cm.conflictDate);
                            return false;
                        }
                    } else {
                        bValid = true;
                    }
                    if (bValid) {
                        getArrAssignedUserNamesAndEmails();
                        var matterMetdataVM = {

                            Matter: {
                                Name: cm.matterName.trim(),
                                Id: cm.matterId,
                                Description: cm.matterDescription,
                                Conflict: {
                                    Identified: cm.conflictRadioCheck,
                                    CheckBy: sCheckBy,
                                    CheckOn: cm.conflictDate,
                                    SecureMatter: cm.secureMatterCheck
                                },
                                BlockUserNames: sBlockUserName,
                                AssignUserNames: cm.arrAssignedUserName,
                                AssignUserEmails: cm.arrAssignedUserEmails,
                                Roles: arrRoles,
                                MatterGuid: cm.matterGUID,
                                FolderNames: arrFolderNames

                            },
                            Client: {
                                Id: cm.clientId,
                                Name: "Microsoft",
                                Url: cm.clientUrl
                            },
                            MatterConfigurations: {
                                IsConflictCheck: cm.chkConfilctCheck,
                                IsMatterDescriptionMandatory: cm.isMatterDescriptionMandatory,
                                IsCalendarSelected: cm.includeCalendar,
                                IsTaskSelected: cm.includeTasks
                            },
                            UserIds: cm.userIDs,
                            MatterProvisionFlags: {},
                            HasErrorOccurred: false
                        };

                        console.log(matterMetdataVM);

                        cm.successMsg = cm.createContent.LabelEntityCreationSuccessMsgText1;
                        cm.successBanner = true; cm.createBtnDisabled = true;
                        createMatter(matterMetdataVM, function (response) {

                            console.log("createMatter API success");
                            console.log(response);
                            cm.successMsg = cm.createContent.LabelEntityCreationSuccessMsgText2;

                            associateContentTypes();
                            assignPermission();
                            createMatterLandingPage();


                        });
                    }
                }
                else {
                    cm.sectionName = "snConflictCheck";
                    cm.iCurrentPage = 2;
                }
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

            function associateContentTypes() {
                var matterGUID = cm.matterGUID;
                var contentTypes = [];
                var defaultContentType = "";
                contentTypes = getDefaultContentTypeValues("contenttypes");
                defaultContentType = getDefaultContentTypeValues("defaultcontenttype");


                var managedColumns = {}
                for (var i = 0; i < cm.taxonomyHierarchyLevels; i++) {
                    var columnName = configs.contentTypes.managedColumns["ColumnName" + (i + 1)];
                    managedColumns[columnName] = { TermName: getDefaultContentTypeValues("level" + (i + 1) + "Name"), Id: getDefaultContentTypeValues("level" + (i + 1) + "Id") };

                }

                var optionsForAssignContentTypeMetadata = {
                    Client: {
                        Id: cm.clientId,
                        Name: cm.selectedClientName,
                        Url: cm.clientUrl
                    },
                    Matter: {
                        Name: cm.matterName.trim(),
                        Id: cm.matterId,
                        ContentTypes: contentTypes,
                        DefaultContentType: defaultContentType,
                        MatterGuid: matterGUID
                    },
                    //PracticeGroupTerm: {
                    //    TermName: sPracticeGroupName,
                    //    Id: sPracticeGroupId
                    //},
                    //AreaTerm: {
                    //    TermName: sAreaOfLawName,
                    //    Id: sAreaOfLawId
                    //},
                    //SubareaTerm: {
                    //    TermName: sSubareaOfLawName,
                    //    Id: sSubareaOfLawId
                    //},
                    ManagedColumnTerms: managedColumns

                }

                console.log("options for optionsForAssignContentTypeMetadata matter");
                console.log(optionsForAssignContentTypeMetadata);

                assignContentTypeMetadata(optionsForAssignContentTypeMetadata, function (response) {

                    console.log(" assignContentTypeMetadataAPI Success");
                    console.log(response);
                    cm.iShowSuccessMessage++;
                    (3 === parseInt(cm.iShowSuccessMessage, 10)) ? stampProperties() : "";

                });



            }

            function assignPermission() {
                var matterGUID = cm.matterGUID;
                var arrPermissions = [];
                arrPermissions = getAssignedUserPermissions();
                var optionsForAssignUserPermissionMetadataVM = {
                    Client: {
                        Id: cm.clientId,
                        Name: cm.selectedClientName,
                        Url: cm.clientUrl
                    },
                    Matter: {
                        Name: cm.matterName.trim(),
                        Permissions: arrPermissions,
                        AssignUserNames: cm.arrAssignedUserName,
                        AssignUserEmails: cm.arrAssignedUserEmails,
                        MatterGuid: matterGUID
                    },
                    MatterConfigurations: {

                        IsCalendarSelected: cm.includeCalendar,
                        IsTaskSelected: cm.includeTasks
                    }
                }
                console.log("options for assignPermission matter");
                console.log(optionsForAssignUserPermissionMetadataVM);
                assignUserPermissionsAPI(optionsForAssignUserPermissionMetadataVM, function (response) {

                    console.log(" assignUserPermissionsAPI Success");
                    console.log(response);
                    cm.iShowSuccessMessage++;
                    (3 === parseInt(cm.iShowSuccessMessage, 10)) ? stampProperties() : "";


                });

            }


            function createMatterLandingPage() {
                var matterGUID = cm.matterGUID;
                var sCheckByUserEmail = (undefined !== cm.selectedConflictCheckUser && null !== cm.selectedConflictCheckUser) ? getUserName(cm.selectedConflictCheckUser.trim() + ";", false) : "";
                var sCheckBy = getUserEmail(sCheckByUserEmail);
                var sBlockUserEmail = (undefined !== cm.blockedUserName && null !== cm.blockedUserName) ? getUserName(cm.blockedUserName.trim() + ";", false) : [];
                var sBlockUserName = sBlockUserEmail;
                var arrPermissions = [];
                arrPermissions = getAssignedUserPermissions();
                var optionsForCreateMatterLandingPage = {

                    Client: {
                        Id: cm.clientId,
                        Name: cm.selectedClientName,
                        Url: cm.clientUrl
                    },
                    MatterConfigurations: {

                        IsConflictCheck: cm.chkConfilctCheck,
                        IsMatterDescriptionMandatory: cm.isMatterDescriptionMandatory,
                        IsCalendarSelected: cm.includeCalendar,
                        IsTaskSelected: cm.includeTasks,
                        IsRSSSelected: cm.includeRssFeeds
                    },
                    Matter: {
                        Name: cm.matterName.trim(),
                        Description: cm.matterDescription,
                        AssignUserNames: cm.arrAssignedUserName,
                        AssignUserEmails: cm.arrAssignedUserEmails,
                        BlockUserNames: sBlockUserName,
                        Conflict: {
                            Identified: cm.conflictRadioCheck,
                            CheckBy: sCheckBy,
                            CheckOn: cm.conflictDate,
                            SecureMatter: "True"
                        },
                        Permissions: arrPermissions,
                        MatterGuid: matterGUID
                    }
                }

                console.log("options for optionsForCreateMatterLandingPage matter");
                console.log(optionsForCreateMatterLandingPage);

                createMatterLandingPageAPI(optionsForCreateMatterLandingPage, function (response) {

                    console.log("createMatterLandingPageAPI Success");
                    console.log(response);
                    cm.iShowSuccessMessage++;
                    (3 === parseInt(cm.iShowSuccessMessage, 10)) ? stampProperties() : "";


                });

            }



            function stampProperties() {
                //  alert();
                var matterGUID = cm.matterGUID;
                cm.successBanner = true;
                cm.successMsg = cm.createContent.LabelEntityCreationSuccessMsgText3;
                var sCheckByUserEmail = (undefined !== cm.selectedConflictCheckUser && null !== cm.selectedConflictCheckUser) ? getUserName(cm.selectedConflictCheckUser.trim() + ";", false) : "";
                var sCheckBy = getUserEmail(sCheckByUserEmail);
                var sBlockUserEmail = (undefined !== cm.blockedUserName && null !== cm.blockedUserName) ? getUserName(cm.blockedUserName.trim() + ";", false) : [];
                var sBlockUserName = sBlockUserEmail;
                var arrPermissions = [];
                arrPermissions = getAssignedUserPermissions();
                var arrRoles = [];
                arrRoles = getAssignedUserRoles();
                var contentTypes = [];
                var defaultContentType = "";
                contentTypes = getDefaultContentTypeValues("contenttypes");
                defaultContentType = getDefaultContentTypeValues("defaultcontenttype");
                var arrUserNames = [], arrUserEmails = [], arrTeamMembers = [];
                // var sPracticeGroupList = "", sAreaOfLawList = "", sSubAreaOfLawList = "";
                var oMatterProvisionFlags = {};
                var sLevel1List = "", sLevel2List = "", sLevel3List = "", sLevel4List = "", sLevel5List = "";

                var sResponsibleAttorney = [], sResponsibleAttorneyEmail = [], arrTeamMembers = [], arrDocumentTemplatesCount = [];
                var oDocumentTemplates = cm.selectedDocumentTypeLawTerms;
                var subAreaofLaw = "", roleInformation = {}, arrReadOnlyUsers = [];


                var User_Upload_Permissions = "Read";
                angular.forEach(cm.assignPermissionTeams, function (item) {
                    //    arrPermissions.push($.trim($(this).val()));
                    if (item.assignedPermission.name.toLowerCase() === User_Upload_Permissions.toLowerCase()) {
                        arrReadOnlyUsers.push(getUserName(item.assignedRole.name.trim() + ";", false).join(";"), ";");
                    }
                });
                validateTeamAssigmentRole();
                angular.forEach(cm.assignPermissionTeams, function (item) {
                    // var sCurrElementID = $(this).attr("id");
                    if (1 <= cm.assignPermissionTeams.length) {
                        if ("" !== item.assignedRole && "" !== item.assignedPermission) {
                            if (-1 !== cm.oMandatoryRoleNames.indexOf(item.assignedRole.name)) {
                                sResponsibleAttorney.push(getUserName(item.assignedUser + ";", true).join(";"));
                                sResponsibleAttorneyEmail.push(getUserName(item.assignedUser + ";", false).join(";"));
                            }
                        }
                        // sCurrElementID = sCurrElementID.trim().split("txtAssign")[1];
                        // var sCurrRole = $("#ddlRoleAssign" + sCurrElementID), sCurrPermission = $("#txtAssign" + sCurrElementID);
                        //if (sCurrRole && sCurrPermission) {
                        //if (-1 !== $.inArray(sCurrRole.val(), serviceConstants.oMandatoryRoleNames)) {
                        //    sResponsibleAttorney.push(oCommonObject.getUserName($.trim($(this).val()), true).join(";"));
                        //    sResponsibleAttorneyEmail.push(oCommonObject.getUserName($.trim($(this).val()), false).join(";"));
                        //}
                        // }
                    }
                });


                //angular.forEach(oDocumentTemplates, function (item) {
                //    if (-1 === subAreaofLaw.indexOf(item.termName)) {
                //        subAreaofLaw += item.termName + "; ";
                //        if (item.primaryMatterType) {
                //            defaultContentType = item.termName;
                //        }
                //    }
                //});

                angular.forEach(cm.assignPermissionTeams, function (item) {
                    //  var sCurrElementID = $(this).attr("id");
                    if ("" !== item.assignedRole && "" !== item.assignedPermission) {
                        //  sCurrElementID = sCurrElementID.trim().split("txtAssign")[1];
                        //   var sCurrRole = $("#ddlRoleAssign" + sCurrElementID), sCurrPermission = $("#txtAssign" + sCurrElementID);
                        //  if (sCurrRole && sCurrPermission) {
                        if (roleInformation.hasOwnProperty(item.assignedRole.name)) {
                            // This role is already present. append the new role with semicolon separated value
                            //   roleInformation[sCurrRole.val()] = roleInformation[sCurrRole.val()] + sCurrPermission.val();
                            roleInformation[item.assignedRole.name] = roleInformation[item.assignedRole.name] + ";" + item.assignedUser;
                        } else {
                            // Add this role to the object
                            roleInformation[item.assignedRole.name] = item.assignedUser;
                        }

                        // }
                    }
                });
                //angular.forEach(roleInformation, function (key, item) {
                //    roleInformation[key] = item.trim();
                //});
                angular.forEach(cm.assignPermissionTeams, function (item) {
                    arrUserNames.push(getUserName(item.assignedUser.trim() + ";", true));
                    arrUserEmails.push(getUserName(item.assignedUser.trim() + ";", false));
                    arrTeamMembers.push(getUserName(item.assignedUser.trim() + ";", true).join(";"));
                });
                contentTypes = subAreaofLaw.trim().split(";");
                angular.forEach(oDocumentTemplates, function (item) {
                    arrDocumentTemplatesCount.push(item.documentTemplateNames.split(";").length.toString());
                });
                arrDocumentTemplatesCount.reverse();


                oMatterProvisionFlags = {
                    "MatterLandingFlag": cm.bMatterLandingPage,
                    "SendEmailFlag": cm.includeEmail
                };

                angular.forEach(oDocumentTemplates, function (item) {
                    if (item.primaryMatterType) {
                        defaultContentType = item.termName;
                    }
                    if (cm.taxonomyHierarchyLevels >= 2) {
                        if (-1 === sLevel1List.indexOf(item.levelOneTermName)) {
                            sLevel1List = item.levelOneTermName + "; " + sLevel1List;
                        }
                        if (-1 === sLevel2List.indexOf(item.levelTwoTermName)) {
                            sLevel2List = item.levelTwoTermName + "; " + sLevel2List;
                        }
                    }
                    if (cm.taxonomyHierarchyLevels >= 3) {
                        if (-1 === sLevel3List.indexOf(item.levelThreeTermName)) {
                            sLevel3List = item.levelThreeTermName + "; " + sLevel3List;
                        }
                    }
                    if (cm.taxonomyHierarchyLevels >= 4) {
                        if (-1 === sLevel4List.indexOf(item.levelFourTermName)) {
                            sLevel4List = item.levelFourTermName + "; " + sLevel4List;
                        }
                    }
                    if (cm.taxonomyHierarchyLevels >= 5) {
                        if (-1 === sLevel5List.indexOf(item.levelFiveTermName)) {
                            sLevel5List = item.levelFiveTermName + "; " + sLevel5List;
                        }
                    }
                });

                var managedColumns = {}
                for (var i = 0; i < cm.taxonomyHierarchyLevels; i++) {
                    var columnName = configs.contentTypes.managedColumns["ColumnName" + (i + 1)];
                    managedColumns[columnName] = { TermName: "", Id: "" };
                    if (i === 0) { managedColumns[columnName].TermName = sLevel1List; }
                    if (i === 1) { managedColumns[columnName].TermName = sLevel2List; }
                    if (i === 2) { managedColumns[columnName].TermName = sLevel3List; }
                    if (i === 3) { managedColumns[columnName].TermName = sLevel4List; }
                    if (i === 4) { managedColumns[columnName].TermName = sLevel5List; }
                }


                var optionsForStampMatterDetails = {
                    Client: {
                        Id: cm.clientId,
                        Name: cm.selectedClientName,
                        Url: cm.clientUrl
                    },
                    Matter: {
                        Name: cm.matterName.trim(),
                        Id: cm.matterId.trim(),
                        Description: cm.matterDescription,
                        Conflict: {
                            Identified: cm.conflictRadioCheck,
                            CheckBy: sCheckBy,
                            CheckOn: cm.conflictDate,
                            SecureMatter: cm.secureMatterCheck
                        },
                        BlockUserNames: sBlockUserName,
                        AssignUserNames: cm.arrAssignedUserName,
                        AssignUserEmails: cm.arrAssignedUserEmails,
                        ContentTypes: contentTypes,
                        DefaultContentType: defaultContentType,
                        Permissions: arrPermissions,
                        Roles: arrRoles,
                        DocumentTemplateCount: arrDocumentTemplatesCount,
                        MatterGuid: matterGUID
                    },
                    MatterDetails: {
                        //PracticeGroup: sPracticeGroupList,
                        //AreaOfLaw: sAreaOfLawList,
                        //SubareaOfLaw: subAreaofLaw,
                        ResponsibleAttorney: sResponsibleAttorney.join(";").replace(/;;/g, ";"),
                        ResponsibleAttorneyEmail: sResponsibleAttorneyEmail.join(";").replace(/;;/g, ";"),
                        UploadBlockedUsers: arrReadOnlyUsers,
                        TeamMembers: arrTeamMembers.join(";"),
                        RoleInformation: JSON.stringify(roleInformation),
                        ManagedColumnTerms: managedColumns

                    },
                    MatterProvisionFlags: oMatterProvisionFlags,
                    MatterConfigurations: {
                        IsConflictCheck: cm.chkConfilctCheck,
                        IsMatterDescriptionMandatory: cm.isMatterDescriptionMandatory
                    }
                }
                console.log("options for optionsForStampMatterDetails matter");
                console.log(optionsForStampMatterDetails);
                updateMatterMetadataAPI(optionsForStampMatterDetails, function (response) {

                    console.log("stampProperties Success");
                    console.log(response);
                    cm.successMsg = cm.createContent.LabelSuccessEntityCreation + " <a target='_blank' href='" + cm.clientUrl + "/SitePages/" + cm.matterGUID + ".aspx'>here</a>.";
                    clearAllProperties();

                    cm.navigateToSecondSection(cm.sectionName);
                    cm.popupContainerBackground = "hide";

                    //  updateMatterMetadata();

                });

                // updateMatterMetadata();

            }

            //function updateMatterMetadata() {
            //    var matterGUID = cm.matterGUID;
            //    var arrPermissions = [];
            //    arrPermissions = getAssignedUserPermissions();
            //    var sCheckByUserEmail = (undefined !== cm.selectedConflictCheckUser && null !== cm.selectedConflictCheckUser) ? getUserName(cm.selectedConflictCheckUser.trim() + ";", false) : "";
            //    var sCheckBy = getUserEmail(sCheckByUserEmail);
            //    var optionsForMatterMetadata = {
            //        Client: {
            //            Id: cm.clientId,
            //            Name: "Microsoft",
            //            Url: 
            //        },
            //        MatterConfigurations: {

            //            IsConflictCheck: cm.chkConfilctCheck,
            //            IsMatterDescriptionMandatory: true,
            //            IsCalendarSelected: cm.includeCalendar,
            //            IsTaskSelected: cm.includeTasks,
            //            IsRSSSelected: cm.includeRssFeeds
            //        },
            //        Matter: {
            //            Name: cm.matterName.trim(),
            //            Id: cm.matterId,
            //            Description: cm.matterDescription,
            //            Conflict: {
            //                Identified: cm.conflictRadioCheck,
            //                CheckBy: sCheckBy,
            //                CheckOn: cm.conflictDate,
            //                SecureMatter: "True"
            //            },
            //            AssignUserNames: cm.arrAssignedUserName,
            //            AssignUserEmails: cm.arrAssignedUserEmails,
            //            BlockUserNames: (undefined !== cm.blockedUserName && null !== cm.blockedUserName) ? getUserName(cm.blockedUserName.trim() + ";", false) : "",
            //            Permissions: arrPermissions,
            //            MatterGuid: matterGUID
            //        }
            //    }

            //    updateMatterMetadataAPI(optionsForMatterMetadata, function (response) {
            //        console.log("updateMatterMetadataAPI Success");
            //        console.log(response);

            //        console.log("Grand Success");

            //    });

            //}

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


            function retrieveFolderStructure() {
                "use strict";
                var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrFolderStructure = [];
                if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                    arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                    nLength = arrContentTypes.length;
                    for (nCount = 0; nCount < nLength; nCount++) {
                        if (arrContentTypes[nCount].primaryMatterType === true || 0 === nCount) {
                            // Check if the isNoFolderStructurePresent flag is set to true                            
                            if (arrContentTypes[nCount].isNoFolderStructurePresent && "false" === arrContentTypes[nCount].isNoFolderStructurePresent.toLowerCase()) {
                                // If the folder at the specific level is not present then move to the parent level
                                arrFolderStructure = arrContentTypes[nCount].folderNames && "" !== arrContentTypes[nCount].folderNames ? arrContentTypes[nCount].folderNames.split(";") : arrContentTypes[nCount].foldernamesaol && "" !== arrContentTypes[nCount].foldernamesaol ? arrContentTypes[nCount].foldernamesaol.split(";") : arrContentTypes[nCount].foldernamespg && "" !== arrContentTypes[nCount].foldernamespg ? arrContentTypes[nCount].foldernamespg.split(";") : [];
                            }
                            
                        }

                    }
                }
                return arrFolderStructure;
            }


            function getDefaultContentTypeValues(contentTypeValue) {
                var returnedValue;
                switch (contentTypeValue) {
                    case "contenttypes":
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [], defaultContentType = "";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("contenttypes" == contentTypeValue) {
                                    if (-1 == arrContents.indexOf(arrContentTypes[nCount].documentTemplates)) {
                                        arrContents.push(arrContentTypes[nCount].documentTemplates);
                                    }

                                    var arrAssociatedDocumentTemplates = arrContentTypes[nCount].documentTemplateNames.split(";");
                                    for (var iIterator = 0; iIterator < arrAssociatedDocumentTemplates.length; iIterator++) {
                                        if (-1 == arrContents.indexOf(arrAssociatedDocumentTemplates[iIterator])) {
                                            arrContents.push(arrAssociatedDocumentTemplates[iIterator]);
                                        }
                                    }
                                }
                            }
                        }
                        returnedValue = arrContents;
                        break;
                    case "defaultcontenttype":
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [], defaultContentType = "";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("defaultcontenttype" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].documentTemplates;
                                    }
                                }
                            }
                        }
                        returnedValue = defaultContentType;
                        break;
                    case "level1Name":

                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [], defaultContentType = "";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("level1Name" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].levelOneTermName;
                                    }
                                }
                            }
                        }
                        returnedValue = defaultContentType;
                        break;
                    case "level1Id":

                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [], defaultContentType = "";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("level1Id" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].levelOneTermId;
                                    }
                                }
                            }
                        }
                        returnedValue = defaultContentType;
                        break;

                    case "level2Name":
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [], defaultContentType = "";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("level2Name" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].levelTwoTermName;
                                    }
                                }
                            }
                        }
                        returnedValue = defaultContentType;
                        break;
                    case "level2Id":
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [], defaultContentType = "";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("level2Id" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].levelTwoTermId;
                                    }
                                }
                            }
                        }
                        returnedValue = defaultContentType;
                        break;
                    case "level3Name":
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [], defaultContentType = "";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("level3Name" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].levelThreeTermName;
                                    }
                                }
                            }
                        }
                        returnedValue = defaultContentType;
                        break;
                    case "level3Id":

                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [], defaultContentType = "";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("level3Id" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].levelThreeId;
                                    }
                                }
                            }
                        }
                        returnedValue = defaultContentType;
                        break;
                    case "level4Name":
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [], defaultContentType = "";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("level4Name" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].levelFourTermName;
                                    }
                                }
                            }
                        }
                        returnedValue = defaultContentType;
                        break;
                    case "level4Id":

                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [], defaultContentType = "";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("level4Id" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].levelFourId;
                                    }
                                }
                            }
                        }
                        returnedValue = defaultContentType;
                        break;
                    case "level5Name":
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [], defaultContentType = "";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("level5Name" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].levelFiveTermName;
                                    }
                                }
                            }
                        }
                        returnedValue = defaultContentType;
                        break;
                    case "level5Id":

                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [], defaultContentType = "";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("level5Id" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].levelFiveId;
                                    }
                                }
                            }
                        }
                        returnedValue = defaultContentType;
                        break;

                }

                return returnedValue;


            }

            //function getContentTypes(value) {
            //    "use strict";
            //    var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [],defaultContentType="";
            //    if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
            //        arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
            //        nLength = arrContentTypes.length;
            //        for (nCount = 0; nCount < nLength; nCount++) {
            //          //  if (arrContentTypes[nCount].primaryMatterType === true || 0 === nCount) {
            //                // Check if the isNoFolderStructurePresent flag is set to true
            //               // if (arrContentTypes[nCount].isNoFolderStructurePresent && "false" === arrContentTypes[nCount].isNoFolderStructurePresent.toLowerCase()) {

            //            // If the folder at the specific level is not present then move to the parent level
            //            if("contenttypes"==value){
            //                arrContents.push(arrContentTypes[nCount].termName);

            //                var arrAssociatedDocumentTemplates=arrContentTypes[nCount].documentTemplateNames.split(";");
            //                for(var iIterator = 0; iIterator < arrAssociatedDocumentTemplates.length; iIterator++){
            //                    if(-1==arrContents.indexOf(arrAssociatedDocumentTemplates[iIterator])){
            //                        arrContents.push(arrAssociatedDocumentTemplates[iIterator]);
            //                    }
            //                }
            //            }
            //            if ("defaultcontenttype"==value ) {
            //                if (arrContentTypes[nCount].primaryMatterType === true) {
            //                    defaultContentType = arrContentTypes[nCount].termName;
            //                }
            //            }
            //               // }
            //           // }

            //        }
            //    }
            //    if (value == "defaultcontenttype") {
            //        return defaultContentType;
            //    } else {
            //        return arrContents;
            //    }
            //}

            //  var contentTypes = getContentType("contentTypes");
            // var  defaultContentType = getContentType("defaultcontenttype");
            //   console.log("----");
            ////   var obj = retrieveFolderStructure();
            //   console.log(contentTypes);
            //   console.log(defaultContentType);

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


            function clearAllProperties() {
                cm.clientId = "";
                cm.selectedClient = "";
                cm.clientUrl = "";
                cm.selectedClientName = "";
                cm.matterName = "";
                cm.matterId = "";
                cm.matterDescription = "";
                cm.clientNameList = [];
                //cm.areaOfLawTerms = [];
                //  cm.subAreaOfLawTerms = [];
                cm.documentTypeLawTerms = [];
                cm.selectedDocumentTypeLawTerms = [];
                //  cm.activeAOLTerm = null;
                // cm.activeSubAOLTerm = null;
                // cm.activeDocumentTypeLawTerm = null;
                cm.popupContainerBackground = "hide";
                cm.popupContainer = "hide";
                cm.createBtnDisabled = false;
                cm.sectionName = "snOpenMatter";
                cm.removeDTItem = false;
                cm.primaryMatterType = cm.errorPopUp = false;
                cm.matterGUID = "";
                cm.iCurrentPage = 1;
                cm.assignPermissionTeams = [{ assignedUser: '', assignedRole: '', assignedPermission: '', assigneTeamRowNumber: 1 }];
                cm.assignRoles = [];
                cm.assignPermissions = [];
                cm.secureMatterCheck = true;
                cm.conflictRadioCheck = false;
                localStorage.iLivePage = 1;
                localStorage.removeItem("oPageOneData");
                localStorage.removeItem("oPageTwoData");
                getMatterGUID();
                getTaxonomyData();
            }

            function storeMatterDataToLocalStorageFirstPage() {
                oPageOneState.Client = cm.selectedClient
                oPageOneState.ClientId = cm.clientId.trim();
                oPageOneState.MatterName = cm.matterName.trim();
                oPageOneState.MatterId = cm.matterId.trim();
                oPageOneState.MatterDescription = cm.matterDescription.trim();
                //  oPageOneState.oAreaOfLawTerms = cm.areaOfLawTerms;
                //   oPageOneState.oSubAreaOfLawTerms = cm.subAreaOfLawTerms;
                oPageOneState.matterGUID = cm.matterGUID;
                oPageOneState.oSelectedDocumentTypeLawTerms = cm.selectedDocumentTypeLawTerms;
                oPageOneState.showRoles = cm.showRoles;
                oPageOneState.showMatterId = cm.showMatterId;
                oPageOneState.matterIdType = cm.matterIdType;
                oPageOneState.isBackwardCompatible = cm.isBackwardCompatible;
                oPageOneState.isClientMappedWithHierachy = cm.isClientMappedWithHierachy;
                //  oPageOneState.chkConflictCheck = cm.chkConfilctCheck;
                //  oPageOneState.oValidMatterName = oPageOneState.oValidMatterName;

                //oPageOneState.oDefaultSecureMatterCheck = cm.secureMatterCheck;
                //oPageOneState.oDefaultInculdeCalendar = cm.includeCalendar;
                //oPageOneState.oDefaultIncludeEmail = cm.includeEmail;
                //oPageOneState.oDefaultRssFeeds = cm.includeRssFeeds;
                //oPageOneState.oDefaultIsTaskSelected = cm.includeTasks;
                //oPageTwoState.oDefaultIsConflictCheck = cm.chkConfilctCheck;
                //oPageTwoState.oDefaultIsMatterDescriptionMandatory = cm.isMatterDescriptionMandatory;

                //oPageOneState.oSectionName = cm.sectionName;
                localStorage.setItem('oPageOneData', JSON.stringify(oPageOneState));
                localStorage.setItem('IsCalendarSelected', cm.includeCalendar);
                localStorage.setItem('IsRestrictedAccessSelected', cm.secureMatterCheck);
                localStorage.setItem('IsRSSSelected', cm.includeRssFeeds);
                localStorage.setItem('IsEmailOptionSelected', cm.includeEmail);
                localStorage.setItem('IsMatterDescriptionMandatory', cm.isMatterDescriptionMandatory);
                localStorage.setItem('IsConflictCheck', cm.defaultConfilctCheck);
                localStorage.setItem('IsMatterDescriptionMandatory', cm.isMatterDescriptionMandatory);
                localStorage.setItem('IsTaskSelected', cm.includeTasks);

                //cm.sectionName = sectionName;
                localStorage.iLivePage = 2;

            }

            function storeMatterDataToLocalStorageSecondPage() {
                oPageTwoState.ChkConfilctCheck = cm.chkConfilctCheck;
                oPageTwoState.SelectedConflictCheckUser = cm.selectedConflictCheckUser;
                oPageTwoState.ConflictDate = cm.conflictDate;
                oPageTwoState.ConflictRadioCheck = cm.conflictRadioCheck;
                oPageTwoState.BlockedUserName = cm.blockedUserName;
                oPageTwoState.SecureMatterCheck = cm.secureMatterCheck;
                oPageTwoState.AssignPermissionTeams = cm.assignPermissionTeams;
                oPageTwoState.oSiteUsers = cm.oSiteUsers;
                localStorage.setItem('oPageTwoData', JSON.stringify(oPageTwoState));
                localStorage.iLivePage = 3;
            }

            cm.NextClick = function ($event) {
                if (cm.iCurrentPage == 1) {
                    cm.navigateToSecondSection("snConflictCheck");
                    $event.stopPropagation();
                }
                else if (cm.iCurrentPage == 2) {
                    cm.navigateToSecondSection("snCreateAndShare");
                    $event.stopPropagation();
                }

            }

            cm.PreviousClick = function ($event) {
                if (cm.iCurrentPage == 2) {
                    cm.navigateToSecondSection("snOpenMatter");
                    $event.stopPropagation();
                }
                else if (cm.iCurrentPage == 3) {
                    cm.navigateToSecondSection("snConflictCheck");
                    $event.stopPropagation();
                }

            }


            cm.CheckPopUp = function (e) {
                //  e.stopPropagation();
                if (!cm.errorStatus) {
                    cm.errorPopUpBlock = false;
                    cm.errorBorder = "";
                }
                cm.errorStatus = false;

            }

            function validateCurrentPage(iCurrPage) {
                if (iCurrPage == 1) {

                    var windowWidth = GetWidth();
                    var RegularExpression;
                    if (undefined !== cm.selectedClientName && null !== cm.selectedClientName && "" !== cm.selectedClientName.trim()) {
                        if ("" !== cm.clientId.trim() && null !== cm.clientId) {
                            var bInValid = false;
                            RegularExpression = new RegExp(oPageOneState.specialCharacterExpressionMatter);
                            var sCurrMatterName = cm.matterName.trim();
                            if (null !== sCurrMatterName && "" !== sCurrMatterName) {
                                var arrValidMatch = sCurrMatterName.match(RegularExpression);
                                if (null === arrValidMatch || arrValidMatch[0] !== sCurrMatterName) {
                                    bInValid = false;
                                } else {
                                    bInValid = true;
                                }
                            }
                            if (bInValid) {
                                //  var matVal = cm.checkValidMatterName();
                                oPageOneState.isNextClick = true;
                                if (undefined !== oPageOneState.oValidMatterName) {
                                    if (oPageOneState.oValidMatterName) {
                                        //  RegularExpression = new RegExp(oMatterProvisionConstants.Special_Character_Expression_Matter_Id);
                                        bInValid = false;
                                        // if (cm.matterId && "" !== cm.matterId.trim() && null != cm.matterId) {
                                        var sCurrentMatterId = cm.matterId;
                                        if (undefined !== sCurrentMatterId && null !== sCurrentMatterId && "" !== sCurrentMatterId) {
                                            sCurrentMatterId = sCurrentMatterId.trim();
                                            var arrValidMatch = sCurrentMatterId.match(RegularExpression);
                                            if (null === arrValidMatch || arrValidMatch[0] !== sCurrentMatterId) {
                                                bInValid = false;
                                            } else {
                                                bInValid = true;
                                            }
                                        }
                                        else {
                                            cm.errTextMsg = cm.createContent.ErrorMessageEntityId1;
                                                //"Enter a matter ID.";
                                            cm.errorBorder = "matterid";
                                            showErrorNotification("matterid");
                                            cm.errorPopUpBlock = true; return false;
                                        }
                                        if (bInValid) {
                                            if (cm.isMatterDescriptionMandatory) {
                                                var sCurrentMatterDesc = cm.matterDescription;
                                                if (undefined !== sCurrentMatterDesc && null !== sCurrentMatterDesc && "" !== sCurrentMatterDesc) {
                                                    sCurrentMatterDesc = sCurrentMatterDesc.trim();
                                                    bInValid = true;
                                                    //var arrValidMatch = sCurrentMatterDesc.match(RegularExpression);
                                                    //if (null === arrValidMatch || arrValidMatch[0] !== sCurrentMatterDesc) {
                                                    //    bInValid = false;
                                                    //} else {
                                                    //    bInValid = true;
                                                    //}
                                                }
                                                else {
                                                    cm.errTextMsg = cm.createContent.ErrorMessageEntityDescription;
                                                        //"Enter a description for this matter.";

                                                    showErrorNotification("matterdescription");
                                                    cm.errorBorder = "matterdescription";
                                                    cm.errorPopUpBlock = true; return false;
                                                }
                                            }
                                            else {                                                
                                                bInValid = true;
                                            }

                                            // if (cm.matterDescription && "" !== cm.matterDescription.trim() && null !== cm.matterDescription) {
                                            if (bInValid) {
                                                if (cm.selectedDocumentTypeLawTerms.length > 0) {
                                                    storeMatterDataToLocalStorageFirstPage();
                                                    return true;
                                                }
                                                else {
                                                    cm.errTextMsg = cm.createContent.ErrorMessageSelectType;
                                                        //"Select matter type by area of law for this matter";
                                                    cm.errorBorder = ""; showErrorNotification("selecttemp");
                                                    cm.errorPopUpBlock = true; return false;
                                                }
                                            }
                                            else {
                                                // alert("Enter a description for this matter");
                                                cm.errTextMsg = cm.createContent.ErrorMessageSpecialCharacters;
                                                    //"Please enter a valid text which contains only alphanumeric characters, spaces & hyphen";
                                                //  cm.errTextMsg = "Enter a description for this matter.";
                                                //   cm.errorPopUpBlock = "matterDescription";
                                                showErrorNotification("matterdescription");
                                                cm.errorBorder = "matterdescription";
                                                cm.errorPopUpBlock = true; return false;

                                            }
                                        }
                                        else {
                                            cm.errTextMsg = cm.createContent.ErrorMessageSpecialCharacters;
                                                //"Please enter a valid text which contains only alphanumeric characters, spaces & hyphen.";
                                            cm.errorBorder = "matterid";
                                            showErrorNotification("matterid");
                                            cm.errorPopUpBlock = true; return false;
                                        }

                                    }
                                    else {
                                        cm.errTextMsg = cm.createContent.ErrorMessageEntityLibraryCreated;
                                            //"Matter library for this Matter is already created. Kindly delete the library or please enter a different Matter name.";
                                        cm.errorBorder = "mattername";
                                        showErrorNotification("mattername");
                                        //var matterErrorEle = document.getElementById("errorBlock");
                                        //var matterErrorTrinageleBlockEle = document.getElementById("errTrinagleBlock");
                                        //var matterErrorTrinagleBorderEle = document.getElementById("errTrinagleBroderBlock");
                                        //var matterErrorTextEle = document.getElementById("errText");
                                        //matterErrorEle.className = 'errPopUpMatterName';
                                        //matterErrorTrinageleBlockEle.className = 'errTringleBlockMatterName';
                                        //matterErrorTrinagleBorderEle.className = 'errTringleBorderBlockMatterName';
                                        //matterErrorTextEle.className = "errTextMatterName";
                                        cm.errorPopUpBlock = true; return false;
                                    }
                                }

                            }
                            else {
                                cm.errTextMsg = cm.createContent.ErrorMessageEntityNameSpecialCharacters;
                                    //"Please enter a valid Matter name which contains only alphanumeric characters and spaces";
                                cm.errorBorder = "mattername";
                                showErrorNotification("mattername");
                                cm.errorPopUpBlock = true; return false;
                            }

                        }
                        else {
                            cm.errTextMsg = cm.createContent.ErrorMessageEntityId2;
                                //"Selected  client for this matter clientId is null ";
                            showErrorNotification("client");
                            cm.errorBorder = "client";
                            cm.errorPopUpBlock = true; return false;
                        }
                    }
                    else {
                        cm.errTextMsg = cm.createContent.ErrorMessageEntityTeamOrClient1;
                            //"Select a client for this matter ";
                        cm.errorBorder = "client";
                        showErrorNotification("client");
                        cm.errorPopUpBlock = true;
                        return false;
                    }

                }
                else if (iCurrPage == 2) {

                    if (cm.defaultConfilctCheck) {
                        if (undefined !== cm.chkConfilctCheck && true == cm.chkConfilctCheck) {

                            if (undefined !== cm.conflictDate && null !== cm.conflictDate && "" != cm.conflictDate) {
                                // cm.conflictDate = new Date();
                                
                                var validUsers = validateUsers();
                                var checkUserDExists = false;
                                if (validUsers) {
                                    checkUserDExists = validateCheckUserExisits();
                                    if (checkUserDExists) {
                                        var attornyCheck = validateAttornyUserRolesAndPermissins();
                                        if (attornyCheck) {
                                            cm.popupContainerBackground = "Show";
                                            storeMatterDataToLocalStorageSecondPage();
                                            return true;
                                            // cm.sectionName = sectionName;
                                        }
                                    }
                                }

                            }
                            else {
                                cm.errTextMsg = cm.createContent.ErrorMessageEntityDate;
                                    //"Enter the date on which the conflict check was performed ";
                                cm.errorBorder = "cdate"; showErrorNotification("cdate");
                                cm.errorPopUpBlock = true; return false;
                            }
                            // callCheckSecurityGroupExists();
                            // console.log(getAssignedUserRoles());
                        }
                        else {
                            //  cm.sectionName = sectionName;
                            cm.errTextMsg = cm.createContent.ErrorMessageEntityConflictCheck;
                                //"A confilct check must be completed prior to provisioning this matter ";
                            cm.errorBorder = "";
                            showErrorNotification("conflictcheck");
                            cm.errorPopUpBlock = true;


                        }
                    } else {
                        var validUsers = validateUsers();
                        var checkUserExisits = false;
                        if (validUsers) {
                            checkUserExisits = validateCheckUserExisits();
                            if (checkUserExisits) {
                                var attornyCheck = validateAttornyUserRolesAndPermissins();
                                if (attornyCheck) {
                                    cm.popupContainerBackground = "Show";
                                    storeMatterDataToLocalStorageSecondPage();
                                    return true;
                                }
                            }
                        }

                    }

                }
                else if (iCurrPage == 3) {
                    return true;
                }

            }


            function showErrorNotification(errorCase) {
                if (errorCase && errorCase != "") {
                    var matterErrorEle = document.getElementById("errorBlock");
                    var matterErrorTrinageleBlockEle = document.getElementById("errTrinagleBlock");
                    var matterErrorTrinagleBorderEle = document.getElementById("errTrinagleBroderBlock");
                    var matterErrorTextEle = document.getElementById("errText");
                    matterErrorEle.className = ""; matterErrorTrinageleBlockEle.className = ""; matterErrorTrinagleBorderEle.className = ""; matterErrorTextEle.className = "";
                    matterErrorEle.classList.add("errorPopUp");
                    matterErrorTrinageleBlockEle.classList.add("errTriangle");
                    matterErrorTrinageleBlockEle.classList.add("popUpFloatLeft");
                    matterErrorTrinagleBorderEle.classList.add("errTriangleBorder");
                    matterErrorTrinagleBorderEle.classList.add("popUpFloatLeft");
                    matterErrorTextEle.classList.add("errText");
                    matterErrorTextEle.classList.add("popUpFloatRight");
                    switch (errorCase) {
                        case "mattername":
                            if (cm.showMatterId) {
                                matterErrorEle.classList.add("errPopUpMatterName");
                            }
                            else {
                                matterErrorEle.classList.add("errPopUpMatterNameWithOutMatterID");
                            }

                            matterErrorTrinageleBlockEle.classList.add("errTringleBlockMatterName");
                            matterErrorTrinagleBorderEle.classList.add("errTringleBorderBlockMatterName");
                            matterErrorTextEle.classList.add("errTextMatterName");
                            break;

                        case "client":
                            matterErrorEle.classList.add("errPopUpMatterClient");
                            matterErrorTrinageleBlockEle.classList.add("errTringleBlockMatterClient");
                            matterErrorTrinagleBorderEle.classList.add("errTringleBorderBlockMatterClient");
                            matterErrorTextEle.classList.add("errTextMatterClient");
                            break;

                        case "matterdescription":
                            if (cm.showMatterId) {
                                matterErrorEle.classList.add("errPopUpMatterDescr");
                            }
                            else {
                                matterErrorEle.classList.add("errPopUpMatterDescrWithOutMatterID");
                            }
                            matterErrorTrinageleBlockEle.classList.add("errTringleBlockMatterDescr");
                            matterErrorTrinagleBorderEle.classList.add("errTringleBorderBlockMatterDescr");
                            matterErrorTextEle.classList.add("errTextMatterDescr");
                            break;

                        case "matterid":
                            matterErrorEle.classList.add("errPopUpMatterId");
                            matterErrorTrinageleBlockEle.classList.add("errTringleBlockMatterId");
                            matterErrorTrinagleBorderEle.classList.add("errTringleBorderBlockMatterId");
                            matterErrorTextEle.classList.add("errTextMatterId");
                            break;
                        case "selecttemp":

                            if (cm.showMatterId) {
                                matterErrorEle.classList.add("errPopUpMatterSelectTemp");
                            }
                            else {
                                matterErrorEle.classList.add("errPopUpMatterSelectTempWithOutMatterID");
                            }
                            matterErrorTrinageleBlockEle.classList.add("errTringleBlockMatterSelectTemp");
                            matterErrorTrinagleBorderEle.classList.add("errTringleBorderBlockSelectTemp");
                            matterErrorTextEle.classList.add("errTextMatterSelectTemp");
                            break;
                        case "conflictcheck":
                            matterErrorEle.classList.add("errPopUpCCheck");
                            matterErrorTrinageleBlockEle.classList.add("errTringleBlockCCheck");
                            matterErrorTrinagleBorderEle.classList.add("errTringleBorderCCheck");
                            matterErrorTextEle.classList.add("errTextMatterCCheck");
                            break;
                        case "cdate":
                            matterErrorEle.classList.add("errPopUpCDate");
                            matterErrorTrinageleBlockEle.classList.add("errTringleBlockCDate");
                            matterErrorTrinagleBorderEle.classList.add("errTringleBorderCDate");
                            matterErrorTextEle.classList.add("errTextMatterCDate");
                            break;

                        case "attorny":
                            matterErrorEle.classList.add("errPopUpCAttorny");
                            matterErrorTrinageleBlockEle.classList.add("errTringleBlockCAttorny");
                            matterErrorTrinagleBorderEle.classList.add("errTringleBorderCAttorny");
                            matterErrorTextEle.classList.add("errTextMatterCAttorny");
                            break;
                        case "ccheckuser":
                            matterErrorEle.classList.add("errPopUpCCheckUser");
                            matterErrorTrinageleBlockEle.classList.add("errTringleBlockCCheckUser");
                            matterErrorTrinagleBorderEle.classList.add("errTringleBorderCCheckUser");
                            matterErrorTextEle.classList.add("errTextMatterCCheckUser");
                            break;
                        case "cblockuser":
                            matterErrorEle.classList.add("errPopUpCBlockUser");
                            matterErrorTrinageleBlockEle.classList.add("errTringleBlockCBlockUser");
                            matterErrorTrinagleBorderEle.classList.add("errTringleBorderCBlockUser");
                            matterErrorTextEle.classList.add("errTextMatterCBlockUser");
                            break;

                        case "mcreate":
                            matterErrorEle.classList.add("errPopUpCreate");
                            matterErrorTrinageleBlockEle.classList.add("errTringleBlockCreate");
                            matterErrorTrinagleBorderEle.classList.add("errTringleBorderCreate");
                            matterErrorTextEle.classList.add("errTextMatterCreate");
                            break;
                    }

                }

            }

            function showErrorNotificationAssignTeams(errorMsg, teamRowNumber, type) {
                var fieldType = "";

                if (type == "user" || type == "securityuser") {
                    fieldType = "txtUser";
                }
                else if (type == "role") { fieldType = "roleUser" }
                else if (type == "perm") {
                    fieldType = "permUser";
                }
                else if (type == "btnCreateMatter") {
                    fieldType = "btnCreateMatter";
                    teamRowNumber = "";
                }

                var temp = document.getElementById(fieldType + teamRowNumber);
                var matterErrorEle = document.getElementById("errorBlock");
                var matterErrorTrinageleBlockEle = document.getElementById("errTrinagleBlock");
                var matterErrorTrinagleBorderEle = document.getElementById("errTrinagleBroderBlock");
                var matterErrorTextEle = document.getElementById("errText");
                matterErrorEle.className = ""; matterErrorTrinageleBlockEle.className = ""; matterErrorTrinagleBorderEle.className = ""; matterErrorTextEle.className = "";
                matterErrorEle.classList.add("errorPopUp");
                matterErrorTrinageleBlockEle.classList.add("errTriangle");
                matterErrorTrinageleBlockEle.classList.add("popUpFloatLeft");
                matterErrorTrinagleBorderEle.classList.add("errTriangleBorder");
                matterErrorTrinagleBorderEle.classList.add("popUpFloatLeft");
                matterErrorTextEle.classList.add("errText");
                matterErrorTextEle.classList.add("popUpFloatRight");
                var errPopUpCAttorny = document.createElement('style'),
                    errTringleBlockCAttorny = document.createElement('style'),
                    errTringleBorderCAttorny = document.createElement('style'),
                    errTextMatterCAttorny = document.createElement('style'),
                    errTxtMatterMsgText = document.createElement('style');
                errPopUpCAttorny.type = 'text/css';
                errTringleBlockCAttorny.type = 'text/css';
                errTringleBorderCAttorny.type = 'text/css';
                errTextMatterCAttorny.type = 'text/css';
                errTxtMatterMsgText.type = 'text/css';

                var width = GetWidth();
                var x = 0, y = 0;
                if (width > 734) {
                    y = temp.offsetTop - 26, x = temp.offsetLeft + 45;
                }
                else {
                    y = temp.offsetTop + 57, x = temp.offsetLeft + 10;
                }


                errPopUpCAttorny.innerHTML = ".errPopUpCAttorny{top:" + y + "px;left:" + x + "px;}";
                errTringleBlockCAttorny.innerHTML = "{min-height: 40px;top: 17px !important;left: 24px;width:100%}";
                errTringleBorderCAttorny.innerHTML = "{min-height: 40px,top: 17px !important;left: 24px;width:100%}";
                errTextMatterCAttorny.innerHTML = "{min-height:40px;top:21px !important;left: 24px;width:100%}";
                errTxtMatterMsgText.innerHTML = ".errTxtMatterMsgText{width:335px !important;}";
                document.getElementsByTagName('head')[0].appendChild(errPopUpCAttorny);
                document.getElementsByTagName('head')[0].appendChild(errTringleBlockCAttorny);
                document.getElementsByTagName('head')[0].appendChild(errTringleBorderCAttorny);
                document.getElementsByTagName('head')[0].appendChild(errTextMatterCAttorny);               

                //matterErrorEle.style.top = te.offsetTop-26 + "px";
                //matterErrorEle.style.left = te.offsetLeft + 45 + "px";
                //matterErrorTrinageleBlockEle.style.left = 50 + "px";
                //matterErrorTrinagleBorderEle.style.left = 50 + "px";
                //matterErrorTextEle.style.top = 12 + "px";
                //  console.log(te.offsetTop);
                cm.errTextMsg = errorMsg;

                cm.errorPopUpBlock = true;
                matterErrorEle.classList.add("errPopUpCAttorny");
                matterErrorTrinageleBlockEle.classList.add("errTringleBlockCAttorny");
                matterErrorTrinagleBorderEle.classList.add("errTringleBorderCAttorny");
                matterErrorTextEle.classList.add("errTextMatterCAttorny");
                if (type == "securityuser") {
                    document.getElementsByTagName('head')[0].appendChild(errTxtMatterMsgText);
                    matterErrorTextEle.classList.add("errTxtMatterMsgText");
                }
            }

            function resizeErrorPopup() {
                "use strict";
                var windowWidth = GetWidth();
                var width;
                cm.errorPopUpBlock = false;
                cm.notificationPopUpBlock = false;
                if (windowWidth <= 734) {
                    cm.errorPopUpBlock = false;
                    cm.notificationPopUpBlock = false;
                }
            }

            cm.closesuccessbanner = function () {
                cm.successMsg = "";
                cm.successBanner = false;
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

            //#region for validations in assign team
            cm.checkAssign = function (id) {
                var assignTeamlen = angular.element('.assignTeam').length;
                if (assignTeamlen > 1) {
                    var attrval = angular.element('#' + id).attr('confirm');
                    angular.element('.checkuser').each(function (e) {
                        var emaillength = 1;
                        var confirmed = this.attributes.confirm.value;
                        //if (confirmed == "false") {
                        //    angular.element(this).css('border-color', 'red');
                        //    return false;
                        //} else {
                        //    angular.element(this).css('border-color', '#ccc');
                        //    return true;
                        //}
                        emaillength++;
                    });
                }
            }

            //#endregion

            //#region  for taxonomy
            cm.documentTypeLawTerms = [];
            cm.getSelectedLevelOne = function () {
                if (cm.selectedLevelOneItem != null) {
                    // cm.levelTwoList = cm.selectedLevelOneItem.level2;                  
                    //   cm.levelThreeList = cm.selectedLevelOneItem.level2[0].level3;
                    if (cm.taxonomyHierarchyLevels >= 2) {
                        cm.levelTwoList = [];
                        cm.levelTwoList = cm.selectedLevelOneItem.level2;
                        cm.activeLevelTwoItem = cm.selectedLevelOneItem.level2[0];
                    }
                    if (cm.taxonomyHierarchyLevels >= 3) {
                        cm.levelThreeList = [];
                        cm.levelThreeList = cm.levelTwoList[0].level3;
                        cm.activeLevelThreeItem = cm.levelThreeList[0];
                    }
                    if (cm.taxonomyHierarchyLevels >= 4) {
                        cm.levelFourList = [];
                        cm.levelFourList = cm.levelThreeList[0].level4;
                        cm.activeLevelFourItem = (cm.levelFourList && cm.levelFourList[0] != undefined) ? cm.levelFourList[0] : [];
                    }
                    if (cm.taxonomyHierarchyLevels >= 5) {
                        cm.levelFiveList = [];
                        cm.levelFiveList = cm.levelFourList[0].level5;
                        cm.activeLevelFiveItem = (cm.levelFiveList && cm.levelFiveList[0] != undefined) ? cm.levelFiveList[0] : [];
                    }

                    cm.errorPopUp = false;
                } else {
                    cm.levelTwoList = cm.levelThreeList = null;
                    if (cm.taxonomyHierarchyLevels >= 2) {
                        cm.levelTwoList = null;

                    }
                    if (cm.taxonomyHierarchyLevels >= 3) {
                        cm.levelThreeList = null;
                    }
                    if (cm.taxonomyHierarchyLevels >= 4) {
                        cm.levelFourList = null;
                    }
                    if (cm.taxonomyHierarchyLevels >= 5) {
                        cm.levelFiveList = null;
                    }
                }

            }

            // function to get the subAOL items on selection of AOLTerm
            cm.selectLevelTwoItem = function (levelTwoItem) {
                cm.errorPopUp = false;
                cm.activeLevelTwoItem = levelTwoItem;
                if (cm.taxonomyHierarchyLevels >= 3) {
                    cm.levelThreeList = cm.activeLevelTwoItem.level3;
                    cm.activeLevelThreeItem = cm.levelThreeList[0];
                }
                if (cm.taxonomyHierarchyLevels >= 4) {
                    cm.levelFourList = cm.levelThreeList[0] != undefined && cm.levelThreeList[0].level4 ? cm.levelThreeList[0].level4 : [];
                    cm.activeLevelFourItem = cm.levelFourList[0] != undefined ? cm.levelFourList[0] : [];
                }
                if (cm.taxonomyHierarchyLevels >= 5) {
                    cm.levelFiveList = cm.levelFourList[0] != undefined && cm.levelFourList[0].level5 ? cm.levelFourList[0].level5 : [];
                    cm.activeLevelFiveItem = cm.levelFourList[0] != undefined ? cm.levelFiveList[0] : [];
                }


            }
            //function to for seclection of subAOL items 
            cm.selectLevelThreeItem = function (levelThreeItem) {
                cm.errorPopUp = false;
                cm.activeLevelThreeItem = levelThreeItem;
                if (cm.taxonomyHierarchyLevels >= 4) {
                    cm.levelFourList = cm.activeLevelThreeItem != undefined ? cm.activeLevelThreeItem.level4 : [];
                    cm.activeLevelFourItem = (cm.levelFourList != undefined && cm.levelFourList[0] != undefined) ? cm.levelFourList[0] : [];
                }
                if (cm.taxonomyHierarchyLevels >= 5) {
                    cm.levelFiveList = (cm.levelFourList != undefined && cm.levelFourList[0] != undefined && cm.levelFourList[0].level5) ? cm.levelFourList[0].level5 : [];
                    cm.activeLevelFiveItem = (cm.levelFourList != undefined && cm.levelFourList[0] != undefined) ? cm.levelFiveList[0] : [];
                }

            }


            cm.selectLevelFourItem = function (levelFourItem) {
                cm.errorPopUp = false;
                cm.activeLevelFourItem = levelFourItem;
                if (cm.taxonomyHierarchyLevels >= 5) {
                    cm.levelFiveList = cm.activeLevelFourItem.level5;
                    cm.activeLevelFiveItem = cm.levelFiveList[0];
                }

            }
            cm.selectLevelFiveItem = function (levelFiveItem) {
                cm.errorPopUp = false;
                cm.activeLevelFiveItem = levelFiveItem;
            }

            cm.addToDocumentTemplate = function () {
                var isThisNewDocTemplate = true;
                var selectedHighestLevelItem = null;
                switch (cm.taxonomyHierarchyLevels) {
                    case 2:
                        selectedHighestLevelItem = cm.activeLevelTwoItem;
                        makeDisableSelectedItemInColumn(cm.levelTwoList, selectedHighestLevelItem);
                        break;
                    case 3:
                        selectedHighestLevelItem = cm.activeLevelThreeItem;
                        makeDisableSelectedItemInColumn(cm.levelThreeList, selectedHighestLevelItem);
                        break;
                    case 4:
                        selectedHighestLevelItem = cm.activeLevelFourItem;
                        makeDisableSelectedItemInColumn(cm.levelFourList, selectedHighestLevelItem);
                        break;
                    case 5:
                        selectedHighestLevelItem = cm.activeLevelFiveItem;
                        makeDisableSelectedItemInColumn(cm.levelFiveList, selectedHighestLevelItem);
                        break;

                }
                if (selectedHighestLevelItem != null) {
                    angular.forEach(cm.documentTypeLawTerms, function (term) { //For loop
                        if (selectedHighestLevelItem.id == term.id) {// this line will check whether the data is existing or not
                            isThisNewDocTemplate = false;
                        }
                    });
                    if (isThisNewDocTemplate) {

                        var documentType = selectedHighestLevelItem;
                        documentType.levelOneFolderNames = cm.selectedLevelOneItem.folderNames;
                        documentType.levelOneTermId = cm.selectedLevelOneItem.id;
                        documentType.levelOneTermName = cm.selectedLevelOneItem.termName;
                        documentType.termChainName = cm.selectedLevelOneItem.termName;
                        if (cm.taxonomyHierarchyLevels >= 2) {
                            documentType.levelTwoFolderNames = cm.activeLevelTwoItem.folderNames;
                            documentType.levelTwoTermId = cm.activeLevelTwoItem.id;
                            documentType.levelTwoTermName = cm.activeLevelTwoItem.termName;
                            documentType.termChainName = documentType.termChainName + ">" + documentType.levelTwoTermName;
                        }
                        if (cm.taxonomyHierarchyLevels >= 3) {
                            documentType.levelThreeFolderNames = cm.activeLevelThreeItem.folderNames;
                            documentType.levelThreeId = cm.activeLevelThreeItem.id;
                            documentType.levelThreeTermName = cm.activeLevelThreeItem.termName;
                            documentType.termChainName = documentType.termChainName + ">" + documentType.levelThreeTermName;
                        }
                        if (cm.taxonomyHierarchyLevels >= 4) {
                            documentType.levelFourFolderNames = cm.activeLevelFourItem.folderNames;
                            documentType.levelFourId = cm.activeLevelFourItem.id;
                            documentType.levelFourTermName = cm.activeLevelFourItem.termName;
                            documentType.termChainName = documentType.termChainName + ">" + documentType.levelFourTermName;
                        }
                        if (cm.taxonomyHierarchyLevels >= 5) {
                            documentType.levelFiveFolderNames = cm.activeLevelFiveItem.folderNames;
                            documentType.levelFiveId = cm.activeLevelFiveItem.id;
                            documentType.levelFiveTermName = cm.activeLevelFiveItem.termName;
                            documentType.termChainName = documentType.termChainName + ">" + documentType.levelFiveTermName;
                        }
                        cm.documentTypeLawTerms.push(documentType);
                        cm.activeDocumentTypeLawTerm = null;
                        //   console.log("doc");
                        //   console.log(cm.documentTypeLawTerms)
                        //cm.primaryMatterType = true; alert(cm.primaryMatterType);
                        //  cm.activeSubAOLTerm = null;
                    }
                }
            }



            function makeDisableSelectedItemInColumn(levelList, selectedItem) {
                angular.forEach(levelList, function (levelListItem) {
                    if (levelListItem.termName == selectedItem.termName) {
                        levelListItem.state = "disable";
                    }

                });
            }
            function makeEnableSelectedItemInColumn(selectedItem) {
                var levelList = [];
                if (cm.taxonomyHierarchyLevels == 2) {
                    levelList = cm.levelTwoList;
                }
                if (cm.taxonomyHierarchyLevels == 3) {
                    levelList = cm.levelThreeList;
                }
                if (cm.taxonomyHierarchyLevels == 4) {
                    levelList = cm.levelFourList;
                }
                if (cm.taxonomyHierarchyLevels == 5) {
                    levelList = cm.levelFiveList;
                }

                angular.forEach(levelList, function (levelListItem) {
                    if (levelListItem.termName == selectedItem.termName) {
                        levelListItem.state = "enable";
                    }

                });
            }

            cm.removeFromDocumentTemplate = function () {
                //  alert(cm.activeDocumentTypeLawTerm);
                if (cm.removeDTItem) {
                    var index = cm.documentTypeLawTerms.indexOf(cm.activeDocumentTypeLawTerm);
                    makeEnableSelectedItemInColumn(cm.activeDocumentTypeLawTerm);
                    cm.documentTypeLawTerms.splice(index, 1);
                    cm.removeDTItem = false;
                    cm.primaryMatterType = false;
                    cm.activeDocumentTypeLawTerm = null;
                }

            }

            cm.selectDocumentTemplateTypeLawTerm = function (documentTemplateTypeLawTerm) {
                // alert(documentTemplateTypeLawTerm);
                if (documentTemplateTypeLawTerm != null) {
                    cm.errorPopUp = false;;
                    cm.removeDTItem = true;
                    cm.activeDocumentTypeLawTerm = documentTemplateTypeLawTerm;
                    cm.primaryMatterType = true;
                }

            }

            cm.saveDocumentTemplates = function () {

                if (cm.primaryMatterType) {
                    cm.errorPopUp = false;
                    angular.forEach(cm.documentTypeLawTerms, function (term) {
                        var primaryType = false;
                        //For loop
                        if (cm.activeDocumentTypeLawTerm.id == term.id) {// this line will check whether the data is existing or not
                            primaryType = true;
                        }
                        term.primaryMatterType = primaryType;
                        cm.popupContainerBackground = "hide";
                        cm.popupContainer = "hide";
                        angular.element('#myModal').modal("hide");

                    });

                    cm.selectedDocumentTypeLawTerms = cm.documentTypeLawTerms;
                }
                else {
                    cm.errorPopUp = true;
                }
            }


            //#endregion


            //#region
            //function to filter practice groups
            function getClientsPracticeGroup(clientName) {
                if (clientName && clientName!=null && clientName != "") {
                    var levelOneList = [];
                    var pgTermList = cm.parentLevelOneList.level1;
                    console.log(pgTermList);
                    angular.forEach(pgTermList, function (pgTerm) {
                        if (pgTerm.level2) {
                            angular.forEach(pgTerm.level2, function (levelTwoTerm) {
                                if (levelTwoTerm.termName === clientName) {
                                    levelOneList.push(pgTerm);
                                    console.log(levelOneList);
                                }
                            });
                        }
                    });
                    cm.levelOneList = levelOneList;
                    var data={};
                    data.name = cm.parentLevelOneList.name;
                    data.levels = cm.parentLevelOneList.levels;
                    data.level1 = levelOneList;
                    cm.selectedLevelOneItem = cm.levelOneList[0];
                    getTaxonomyHierarchy(data);

                   // if(pgList.)
                }

            }
            //#endregion

        }]);

    app.filter('getAssociatedDocumentTemplatesCount', function () {
        return function (input, splitChar) {

            return input.split(splitChar).length;;
        }
    });

    app.directive('showcreatesuccessbannermessage', function ($rootScope) {
        return {
            restrict: 'A',
            scope: { datanotification: '@' },
            link: function (scope, element, attrs) {
                scope.$watch("datanotification", function () {
                    console.log(attrs.datanotification);
                    $(element).html(attrs.datanotification);

                });

            }
        }


    });
})();
