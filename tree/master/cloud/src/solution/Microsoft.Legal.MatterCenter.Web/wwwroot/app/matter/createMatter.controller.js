(function () {
    'use strict';

    var app=  angular.module("matterMain");
    app.controller('createMatterController', ['$scope', '$state', '$stateParams', 'api','matterResource','$filter','$window',
        function ($scope, $state, $stateParams, api, matterResource, $filter,$window) {
            ///All Variables
            var cm = this;
            cm.selectedConflictCheckUser = undefined;
            cm.blockedUserName = undefined;
            cm.chkConfilctCheck = undefined;
            cm.conflictRadioCheck = true;
            cm.iShowSuccessMessage = 0;
            cm.oMandatoryRoleNames = [];
            cm.bMatterLandingPage = false;
            cm.oSiteUsers = [];
            cm.successBanner = false;
cm.createBtnDisabled=false;
            
            cm.createButton = "Create";


            var w = angular.element($window);

            w.bind('resize', function () {
                cm.clearPopUp();
                $scope.$apply();
            });

            var oPageOneState={
                ClientValue: [],
                ClientId: "",
                MatterName: "",
                MatterId: "",
                MatterDescription: "",
                ContentTypes: [],
                matterMandatory: "",
                oAreaOfLawTerms: [],
                oSubAreaOfLawTerms: [],
                oSelectedDocumentTypeLawTerms: [],
                oValidMatterName: undefined,
                isNextClick: false,
                sectionClickName:"",
                specialCharacterExpressionMatter: "[A-Za-z0-9_]+[-A-Za-z0-9_, ]*"
                
            }

            var oPageTwoState = {
                ChkConfilctCheck: false,
                SelectedConflictCheckUser: "",
                ConflictDate: null,
                ConflictRadioCheck:"",
                BlockedUserName:[],
                SecureMatterCheck: true,
                AssignPermissionTeams: [],
                oSiteUsers:[]
            }
           
            cm.clientId = "";
            cm.selectedClientName = "";
            cm.matterName = "";
            cm.matterId = "";
            cm.matterDescription = "";

            cm.clientNameList = [];
            cm.areaOfLawTerms = [];
            cm.subAreaOfLawTerms = [];
            cm.documentTypeLawTerms = [];
            cm.selectedDocumentTypeLawTerms = [];
            cm.activeAOLTerm = null;
            cm.activeSubAOLTerm = null;
            cm.activeDocumentTypeLawTerm = null;
            cm.popupContainerBackground = "Show";
            cm.popupContainer = "hide";

            cm.sectionName = "snOpenMatter";
            cm.removeDTItem = false;
            cm.primaryMatterType = cm.errorPopUp = false;
            cm.matterGUID = "";
            cm.iCurrentPage = 1;
            cm.assignPermissionTeams = [{ assignedUser: '', assignedRole: '', assignedPermission: '', assigneTeamRowNumber:  1 }];
            cm.assignRoles = [];
            cm.assignPermissions = [];
            cm.secureMatterCheck = false;
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
            var siteCollectionPath = "https://msmatter.sharepoint.com/sites/microsoft";

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
                    resource:'matterResource',
                    method: 'getDefaultMatterConfigurations',
                    data: JSON.stringify(siteCollectionPath),
                    success:callback
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
                    Url: "https://msmatter.sharepoint.com/sites/microsoft"
                },
                TermStoreDetails: {
                    TermGroup: "MatterCenterTerms",
                    TermSetName: "Clients",
                    CustomPropertyName: "ClientURL"
                }
            }
               
           //input parameters building here for all the api's
            optionsForPracticeGroup = {
                Client: {

                    Url: "https://msmatter.sharepoint.com/sites/microsoft"
                },
                TermStoreDetails: {
                    TermGroup: "MatterCenterTerms",
                    TermSetName: "Practice Groups",
                    CustomPropertyName: "ContentTypeName",
                    DocumentTemplatesName:"DocumentTemplates"
                }
            }

            optionsForCheckMatterName={
                Client: {

                    Url: "https://msmatter.sharepoint.com/sites/microsoft"
                },
                Matter: {
                    MatterGuid: "",
                    Name:cm.matterName.trim()
                }
            }
            //optionsForUsers

            cm.searchUsers = function (val) {
                var searchUserRequest = {
                    Client: {

                        Url: "https://msmatter.sharepoint.com/sites/microsoft"
                    },
                    SearchObject: {
                        SearchTerm: val
                    }
                };

                return matterResource.getUsers(searchUserRequest).$promise;
            }

            var optionsForRoles = new Object;
            optionsForRoles = {

                Url: "https://msmatter.sharepoint.com/sites/catalog"

            }

            var optionsForPermissionLevels = new Object;
            optionsForPermissionLevels = {

                Url: "https://msmatter.sharepoint.com/sites/microsoft"
            }

            //call back function for getting the clientNamesList
        function getTaxonomyData(){
    getTaxonomyDetailsForClient(optionsForClientGroup, function (response) {              

                cm.clientNameList = response.clientTerms;
               // jQuery('#myModal').modal('show');
                getTaxonomyDetailsForPractice(optionsForPracticeGroup, function (response) {
                    cm.pracitceGroupList = response.pgTerms;
                    // cm.popupContainerBackground = "hide";

                    getRoles(optionsForRoles, function (response) {
                        //console.log("roles");
                        //console.log(response);

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
                });

            });
}

           getTaxonomyData();

          
          

            //calls this function when selectType button clicks
            cm.selectMatterType = function (value) {
               
                    cm.popupContainerBackground = "Show";
                
                if (cm.pracitceGroupList == null ) {
                    getTaxonomyDetailsForPractice(optionsForPracticeGroup, function (response) {
                        cm.pracitceGroupList = response.pgTerms;
                     
                            cm.popupContainer = "Show";
                            cm.popupContainerBackground = "Show";
                       
                    });
                }
                else {
                    cm.popupContainer = "Show";
                    cm.popupContainerBackground = "Show";

                }
                if(cm.pracitceGroupList[0]){
                 cm.selectedPracticeGroup=cm.pracitceGroupList[0];
                 cm.areaOfLawTerms=cm.pracitceGroupList[0].areaTerms;
                 cm.subAreaOfLawTerms=cm.pracitceGroupList[0].areaTerms[0].subareaTerms;
                 cm.activeAOLTerm=cm.pracitceGroupList[0].areaTerms[0];
                 cm.activeSubAOLTerm = cm.pracitceGroupList[0].areaTerms[0].subareaTerms[0];
                
                 }
            }

            //function for closing the popup
            cm.selectMatterTypePopUpClose = function () {
                if (cm.popupContainer == "Show") {
                    cm.popupContainerBackground = "hide";
                    cm.popupContainer = "hide";
                } 
            }
            //function to get the clientId from ClientName dropdown
            cm.getSelectedClientValue = function (client) {              
                
                cm.clientId = cm.selectedClientName = client;
              
                if (null != cm.clientId) {
                    cm.popupContainerBackground = "Show";

                    getDefaultMatterConfigurations(siteCollectionPath, function (result) {
                      
                        if (result.isError) {
                           
                        //    alert();
                        }
                        else {
                            var dMatterAreaOfLaw = "", dMatterPracticeGroup = "", dMatterSubAreOfLaw = "", dMatterTypes = "", dPrimaryMatterType = "", dMatterUsers = "", dMatterUserEmails = "", dMatterPermissions = "", dMatterRoles = "";

                          
                            var defaultMatterConfig = JSON.parse(result.code);

                            cm.matterName = defaultMatterConfig.DefaultMatterName;
                            cm.checkValidMatterName();
                            cm.matterId = defaultMatterConfig.DefaultMatterId;
                            if (defaultMatterConfig.IsRestrictedAccessSelected) {
                                cm.secureMatterCheck = defaultMatterConfig.IsRestrictedAccessSelected;
                            }
                            if (defaultMatterConfig.IsCalendarSelected) {
                                cm.includeCalendar = defaultMatterConfig.IsCalendarSelected;
                            }
                            if (defaultMatterConfig.IsEmailOptionSelected) {
                                cm.includeEmail = defaultMatterConfig.IsEmailOptionSelected;
                                cm.createButton = "Create and Notify";
                            }
                            if (defaultMatterConfig.IsRSSSelected) {
                                cm.includeRssFeeds = defaultMatterConfig.IsRSSSelected;
                            }
                            if (defaultMatterConfig.IsConflictCheck) {
                                cm.chkConfilctCheck = defaultMatterConfig.IsConflictCheck;
                            }
                            if (defaultMatterConfig.IsMatterDescriptionMandatory) {
                                cm.isMatterDescriptionMandatory = defaultMatterConfig.IsMatterDescriptionMandatory;
                            }
                           // if (defaultMatterConfig.IsContentCheck) {
                               // cm.secureMatterCheck = "True";
                           // }
                            if (defaultMatterConfig.IsTaskSelected) {
                                cm.includeTasks = defaultMatterConfig.IsTaskSelected;
                            }
                            var arrDMatterAreaOfLaw = [];
                            var arrDMatterPracticeGroup = [],arrDMatterUsers=[],arrDMatterUserEmails=[],arrDMatterPermissions=[],arrDMatterRoles=[];
                            arrDMatterAreaOfLaw = defaultMatterConfig.MatterAreaofLaw.split('$|$');
                            arrDMatterPracticeGroup = defaultMatterConfig.MatterPracticeGroup.split('$|$');
                       //     dMatterAreaOfLaw = defaultMatterConfig.MatterAreaofLaw ? defaultMatterConfig.MatterAreaofLaw : "";
                         //   dMatterPracticeGroup = defaultMatterConfig.MatterPracticeGroup?defaultMatterConfig.MatterPracticeGroup: "";
                         //   dMatterSubAreOfLaw = defaultMatterConfig.?: "";
                            dMatterTypes = defaultMatterConfig.MatterTypes ? defaultMatterConfig.MatterTypes : "";

                            var arrDMatterTypes = dMatterTypes.split('$|$');
                            dPrimaryMatterType = defaultMatterConfig.DefaultMatterType?defaultMatterConfig.DefaultMatterType: "";
                            dMatterUsers = defaultMatterConfig.MatterUsers ? defaultMatterConfig.MatterUsers : "";
                            arrDMatterUsers = dMatterUsers.split('$|$');
                            dMatterUserEmails = defaultMatterConfig.MatterUserEmails ? defaultMatterConfig.MatterUserEmails : "";
                            arrDMatterUserEmails = dMatterUserEmails.split('$|$');
                            dMatterPermissions = defaultMatterConfig.MatterPermissions ? defaultMatterConfig.MatterPermissions : "";
                            arrDMatterPermissions = dMatterPermissions.split('$|$');
                            dMatterRoles = defaultMatterConfig.MatterRoles ? defaultMatterConfig.MatterRoles : "";
                            arrDMatterRoles = dMatterRoles.split('$|$');
                            cm.selectMatterType();
                            cm.popupContainer = "hide";
                          
                            getMatterGUID();
                            cm.selectedDocumentTypeLawTerms = [];
                          
                            angular.forEach(cm.pracitceGroupList, function (pgTerm) {                             
                               
                                   
                                        angular.forEach(pgTerm.areaTerms, function (areaTerm) {
                                           
                                                    angular.forEach(areaTerm.subareaTerms, function (subAreaTerm) {

                                                        for (var iCount = 0; iCount < arrDMatterTypes.length; iCount++) {

                                                            if (subAreaTerm.termName == arrDMatterTypes[iCount]) {
                                                                //  cm.selectedDocumentTypeLawTerms = 
                                                                var documentType = subAreaTerm;
                                                                documentType.foldernamespg = pgTerm.folderNames;
                                                                documentType.practicegroupId = pgTerm.id;
                                                                documentType.foldernamesaol = areaTerm.folderNames;
                                                                documentType.areaoflawId = areaTerm.id;
                                                                documentType.areaoflaw = areaTerm.termName;
                                                                documentType.practicegroup = pgTerm.termName;
                                                                //cm.documentTypeLawTerms

                                                                cm.documentTypeLawTerms.push(subAreaTerm);
                                                                documentType.primaryMatterType = false;
                                                                if (subAreaTerm.termName == dPrimaryMatterType) {
                                                                    documentType.primaryMatterType = true;
                                                                    cm.activeDocumentTypeLawTerm = subAreaTerm;
                                                                }
                                                                cm.selectedDocumentTypeLawTerms.push(documentType);
                                                            }

                                                        }
                                                    });
                                            
                                        });


                                    
                                
                            });
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

                            }


                            //angular.forEach(cm.assignPermissionTeams, function (assignTeam) {


                            //});

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
                  

                }

              

            }
            //function to get the all AOL and SAOL terms
            cm.getSelectedPracticeGroupValue = function () {

               
                if (cm.selectedPracticeGroup != null) {
                    cm.areaOfLawTerms = cm.selectedPracticeGroup.areaTerms;
                    cm.subAreaOfLawTerms = cm.selectedPracticeGroup.areaTerms[0].subareaTerms;
                    cm.activeSubAOLTerm = cm.selectedPracticeGroup.areaTerms[0].subareaTerms[0];
                    cm.activeAOLTerm = cm.selectedPracticeGroup.areaTerms[0];
                    cm.errorPopUp = false;
                } else {
                    cm.areaOfLawTerms = cm.subAreaOfLawTerms = null;

                }

            }

            //function to get the subAOL items on selection of AOLTerm
            cm.selectAreaOfLawTerm = function (areaOfLawTerm) {
                cm.errorPopUp = false;
                cm.activeAOLTerm = areaOfLawTerm;

                cm.subAreaOfLawTerms = areaOfLawTerm.subareaTerms;
                cm.activeSubAOLTerm = areaOfLawTerm.subareaTerms[0];

            }
            ////function to for seclection of subAOL items 
            cm.selectSubAreaOfLawTerm = function (subAreaOfLawTerm) {
                cm.errorPopUp = false;
                cm.activeSubAOLTerm = subAreaOfLawTerm;
                
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

            cm.addToDocumentTemplate = function () {
                var   isThisNewDocTemplate = true;
                if (cm.activeSubAOLTerm != null) {
                    angular.forEach(cm.documentTypeLawTerms, function (term) { //For loop
                        if (cm.activeSubAOLTerm.id == term.id) {// this line will check whether the data is existing or not
                            isThisNewDocTemplate = false;
                        }
                    });
                    if (isThisNewDocTemplate) {
                        //console.log(cm.selectedPracticeGroup);
                        //console.log("AOL");
                        //console.log(cm.activeAOLTerm);
                        //console.log("SAOL");
                        //console.log(cm.activeSubAOLTerm);
                        var documentType = cm.activeSubAOLTerm;
                        documentType.foldernamespg = cm.selectedPracticeGroup.folderNames;
                        documentType.practicegroupId = cm.selectedPracticeGroup.id;
                        documentType.foldernamesaol = cm.activeAOLTerm.folderNames;
                        documentType.areaoflawId = cm.activeAOLTerm.id;
                        documentType.areaoflaw = cm.activeAOLTerm.termName;
                        documentType.practicegroup = cm.selectedPracticeGroup.termName;
                        


                        cm.documentTypeLawTerms.push(documentType);
                        cm.activeDocumentTypeLawTerm = null;
                     //   console.log("doc");
                     //   console.log(cm.documentTypeLawTerms)
                        //cm.primaryMatterType = true; alert(cm.primaryMatterType);
                      //  cm.activeSubAOLTerm = null;
                    }
                }
            }

            cm.removeFromDocumentTemplate = function () {
                //  alert(cm.activeDocumentTypeLawTerm);
                if (cm.removeDTItem) {
                    var index = cm.documentTypeLawTerms.indexOf(cm.activeDocumentTypeLawTerm);
                    cm.documentTypeLawTerms.splice(index, 1);
                    cm.removeDTItem = false;
                    cm.primaryMatterType = false;
                    cm.activeDocumentTypeLawTerm = null;
                }
               
            }
            cm.checkValidMatterName = function () {
                oPageOneState.oValidMatterName = undefined;
                var bInValid = false;
                var RegularExpression = new RegExp(oPageOneState.specialCharacterExpressionMatter);
                var sCurrMatterName=cm.matterName.trim();
                if (null !== sCurrMatterName && "" !== sCurrMatterName) {
                    var arrValidMatch = sCurrMatterName.match(RegularExpression);
                    if (null === arrValidMatch || arrValidMatch[0] !== sCurrMatterName) {
                        bInValid = false;
                    } else {
                        bInValid = true;
                    }
                }
                if (bInValid){
                optionsForCheckMatterName.Matter.Name = cm.matterName.trim();
               
                    getCheckValidMatterName(optionsForCheckMatterName, function (response) {
                        if (response.code != 200) {
                            cm.errTextMsg = "Matter library for this Matter is already created. Kindly delete the library or please enter a different Matter name.";
                            cm.errorBorder = "mattername"; showErrorNotification("mattername");
                            cm.errorPopUpBlock = true;
                            oPageOneState.oValidMatterName = false;
                          return false;
                        } else {
                            //  alert("success");
                            oPageOneState.oValidMatterName = true;
                            if ( oPageOneState.isNextClick) {
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
           oPageOneState.sectionClickName=sectionName;
                if (sectionName == "snConflictCheck") {
                    if (validateCurrentPage(cm.iCurrentPage)) {
                        cm.sectionName = sectionName;
                        oPageOneState.isNextClick = false;
                        cm.iCurrentPage = 2;
                        localStorage.iLivePage = 2;
                        
                    }
                }
                else if (sectionName == "snCreateAndShare") {

                    if (validateCurrentPage(cm.iCurrentPage)) {
                        cm.sectionName = sectionName;
                        cm.iCurrentPage = 3;
                        localStorage.iLivePage = 3;
                    }
                  

                    //if (sectionName == "snConflictCheck") {
                    //    if (cm.clientId != null && cm.selectedClientName != null){

                    //        if()
                    //    }
                    //        && (cm.matterName != null || cm.matterName.trim() != '') && (cm.matterId != null || cm.matterId.trim() != '') && (cm.matterDescription != null || cm.matterDescription.trim() != '')) {
                    //        cm.sectionName = sectionName;

                    //    }
                    //    else {

                    //        alert("Incomplete");
                    //    }
                    //}
                    //else {
                    //    cm.sectionName = sectionName;
                    //}
                }
                else if (sectionName == "snOpenMatter") {
                    cm.iCurrentPage = 1; cm.sectionName = sectionName;
 localStorage.iLivePage = 1;

                }
            }

            var validateAttornyUserRolesAndPermissins = function () {
                var responsibleAttorny=0,fullControl=0;
                for (var iCount = 0; iCount < cm.assignPermissionTeams.length; iCount++) {

                    if ("" !== cm.assignPermissionTeams[iCount].assignedUser) {
                      
                        if (cm.assignPermissionTeams[iCount].assignedRole && "" !== cm.assignPermissionTeams[iCount].assignedRole.name) {
                            if (cm.assignPermissionTeams[iCount].assignedPermission && "" != cm.assignPermissionTeams[iCount].assignedPermission.name) {
                                if (cm.assignPermissionTeams[iCount].assignedRole.mandatory) {
                                    responsibleAttorny++;
                                }
                                if (cm.assignPermissionTeams[iCount].assignedPermission.name=="Full Control") {
                                    fullControl++;
                                }
                               
                            }
                            else {
                                cm.errTextMsg = "Please provide at least one permission on this  matter. ";
                                cm.errorBorder = "";
                                cm.errorPopUpBlock = true;

                                return false;
                            }
                        }
                        else {
                            cm.errorPopUpBlock = true;
                            cm.errTextMsg = "Enter at least one role for this matter.";
                            cm.errorBorder = "";
                           
                            return false;
                        }
                    }
                    else {
                        cm.errTextMsg =cm.assignPermissionTeams[iCount].assignedRole.name+" cannot be empty.";
                        cm.errorBorder = "";
                        showErrorNotificationAssignTeams(cm.errTextMsg, cm.assignPermissionTeams[iCount].assigneTeamRowNumber,"user");
                        cm.errorPopUpBlock = true;
                        return false;
                    }

                }

                if (responsibleAttorny >= 1) {
                    if (fullControl >= 1) {
                        return true;

                    }
                    else {
                        cm.errTextMsg = "Please provide at least one user who has Full Control permission on this  matter.";
                         cm.errorBorder = "permUser1";
                         showErrorNotificationAssignTeams( cm.errTextMsg,1,"perm");
                        cm.errorPopUpBlock = true;
                        return false;

                    }
                }
                else {
                    cm.errTextMsg = "Enter at least one Responsible Attorney for this matter.";
                    cm.errorBorder = "roleUser1";
                    showErrorNotificationAssignTeams( cm.errTextMsg,1,"role");
                    cm.errorPopUpBlock = true;
                    return false;
                }

            }

            function validateUsers() {
                var keepGoing=true;

                var username = "";
                if (cm.selectedConflictCheckUser && "" !== cm.selectedConflictCheckUser) {
                    username = getUserName(cm.selectedConflictCheckUser + ";", false);
                     if (-1 == cm.oSiteUsers.indexOf(username[0])) {
                        //  cm.blockedUserName.trim()
                        cm.errTextMsg = "Enter the conflict reviewers name (for auditing purposes).";
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
                        cm.errTextMsg = "Enter users that are conflicted with this matter.";
                        cm.errorBorder = "cblockuser";
                        showErrorNotification("cblockuser");
                        cm.errorPopUpBlock = true;
                        return false;
                    }
                }
                
                 angular.forEach(cm.assignPermissionTeams, function (team) {
                   
                     if (keepGoing){
                     if (team.assignedUser && team.assignedUser != "") {//For loop
                         username = getUserName(team.assignedUser + ";", false)
                         if (-1 == cm.oSiteUsers.indexOf(username[0])) {
                             //  cm.blockedUserName.trim()
                             cm.errTextMsg = "Please enter valid team members.";
                             cm.errorBorder = "";
                             cm.errorPopUpBlock = true;
                             showErrorNotificationAssignTeams(cm.errTextMsg, team.assigneTeamRowNumber, "user")
                             cm.errorBorder = "txtUser" + team.assigneTeamRowNumber; keepGoing = false;
                             return false;

                         }

                         if (cm.blockedUserName && cm.blockedUserName != "") {
                             if (team.assignedUser == cm.blockedUserName) {
                                 cm.errTextMsg = "Please enter individual who is not conflicted.";
                                 cm.errorBorder = "";
                                 cm.errorPopUpBlock = true;
                                 showErrorNotificationAssignTeams(cm.errTextMsg, team.assigneTeamRowNumber, "user")
                                 cm.errorBorder = "txtUser" + team.assigneTeamRowNumber; keepGoing = false;
                                 return false;
                             }
                         }
                     }
                     else {
                       
                       
                         showErrorNotificationAssignTeams(team.assignedRole.name + " cannot be empty", team.assigneTeamRowNumber,"user")
                     

                         //  console.log(te.offsetTop);
                       //  cm.errTextMsg = "Enter cannot be empty.";
                         cm.errorBorder = "txtUser"+team.assigneTeamRowNumber;
                      //   cm.errorPopUpBlock = true;
                      keepGoing = false;
                         return false;
                     }
                 }
                });
               
                 if (keepGoing) {
                     return true;
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
            
            cm.onSelect = function ($item, $model, $label,value) {
                if (value == "conflictcheckuser") {
                    cm.selectedConflictCheckUser = $item.name + '(' + $item.email + ')';
                }
                if (value == "blockuser") {
                    cm.blockedUserName = $item.name + '(' + $item.email + ')';
                }
                if (value == "team") {
                    $label.assignedUser = $item.name + '(' + $item.email + ')';
                }               
                //  console.log($item);
              
               
                if (-1 == cm.oSiteUsers.indexOf($item.email)) {
                    cm.oSiteUsers.push($item.email);
                }
            }

           

          var callCheckSecurityGroupExists=function(sectionName){
                
                cm.arrAssignedUserName = [], cm.arrAssignedUserEmails = [], cm.userIDs = [];
                //console.log(cm.assignPermissionTeams);
                var count=1;
                angular.forEach(cm.assignPermissionTeams, function (team) { //For loop
                    cm.arrAssignedUserName.push(getUserName(team.assignedUser + ";", true));
                    cm.arrAssignedUserEmails.push(getUserName(team.assignedUser + ";", false));
                    cm.userIDs.push("txtAssign" +count++ );
                });
                var optionsForSecurityGroupCheck = {
                    Client: {

                        Url: "https://msmatter.sharepoint.com/sites/microsoft"
                    },
                    Matter: {
                        Name : cm.matterName.trim(),
                        AssignUserNames : cm.arrAssignedUserName,
                        AssignUserEmails :cm.arrAssignedUserEmails,
                        Conflict:{
                            Identified : "True"
                        },
                        BlockUserNames : (undefined !== cm.blockedUserName && null !== cm.blockedUserName) ? getUserName(cm.blockedUserName.trim() + ";", false) : [],
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
                        alert(" Assign roles and permissions to a particular user for this matter ");
                    } else {
                        cm.iCurrentPage = 3; cm.popupContainerBackground = "hide";
                        cm.sectionName=sectionName;
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
                       
                    });

                    cm.selectedDocumentTypeLawTerms = cm.documentTypeLawTerms;
                }
                else {
                    cm.errorPopUp=true;
                }
            }






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
                cm.opened = true;
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
                return [ mnth, day,date.getFullYear(), ].join("/");
            }
           
            //$scope.SelectModal= function(){
            //   // jQuery('#myModal').modal('show');
            //}
            cm.conflictRadioCheckValue = true;
            cm.conflictRadioChange = function (value) {
                cm.blockedUserName = "";
            }
           
            cm.addNewAssignPermissions = function () {
                var newItemNo = cm.assignPermissionTeams.length + 1;
                cm.assignPermissionTeams.push({ 'assigneTeamRowNumber': newItemNo, 'assignedRole': cm.assignRoles[0],'assignedPermission':cm.assignPermissions[0] });
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
                    cm.selectedClientName = oPageData.ClientId;
                    cm.matterName = oPageData.MatterName;
                    cm.matterId = oPageData.MatterId;
                    cm.matterDescription = oPageData.MatterDescription;

                    cm.clientNameList = [];
                    cm.areaOfLawTerms = [];
                    cm.subAreaOfLawTerms = [];
                    cm.documentTypeLawTerms = [];
                    cm.selectedDocumentTypeLawTerms = [];
                    cm.activeAOLTerm = null;
                    cm.activeSubAOLTerm = null;
                    cm.activeDocumentTypeLawTerm = oPageData.oSelectedDocumentTypeLawTerms;
                    cm.selectedDocumentTypeLawTerms = cm.documentTypeLawTerms = oPageData.oSelectedDocumentTypeLawTerms;
                    cm.popupContainerBackground = "Show";
                    cm.popupContainer = "hide";

                    cm.sectionName = "snConflictCheck";
                    cm.removeDTItem = false;
                    cm.primaryMatterType = cm.errorPopUp = false;
                    cm.matterGUID = oPageData.matterGUID;
                    cm.iCurrentPage = 2;
                    cm.includeRssFeeds = (localStorage.getItem("IsRSSSelected") === "true");
                    cm.includeEmail = (localStorage.getItem("IsEmailOptionSelected") === "true");
                    cm.includeCalendar = (localStorage.getItem("IsCalendarSelected") === "true");
                    cm.isMatterDescriptionMandatory = (localStorage.getItem("IsMatterDescriptionMandatory") === "true");
                    cm.chkConfilctCheck = (localStorage.getItem("IsConflictCheck") === "true");
                    cm.includeTasks = (localStorage.getItem("IsTaskSelected") === "true");

                    cm.secureMatterCheck = (localStorage.getItem("IsRestrictedAccessSelected") === "true"); 
                       oPageOneState.oValidMatterName = oPageData.oValidMatterName;
                   // cm.navigateToSecondSection("");
                   // cm.navigateToSecondSection("snConflictCheck");
                 

                }
                if (localStorage.getItem("iLivePage") == 3) {
                    cm.iCurrentPage = 1;
                     var oPageData = JSON.parse(localStorage.getItem("oPageTwoData"));
                     if (oPageData && oPageData!==null) {
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
                  cm.oSiteUsers = oPageData.oSiteUsers;
                  cm.iCurrentPage = 2;
                     }
                 
                
                 
                  if (cm.includeEmail) {
                      cm.createButton = "Create and Notify";
                  }
                  
                  cm.sectionName = "snCreateAndShare";
                  
                 // cm.navigateToSecondSection("snCreateAndShare");
                 }
                // cm.navigateToSecondSection(cm.sectionName);
            }

          //  cm.includeEmail = true;
          
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





            cm.createMatterButton = function ($event) {
               
               // cm.popupContainerBackground = "Show";
                var matterGUID = cm.matterGUID;
                var arrFolderNames=[];
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
                //var sPracticeGroupName=getDefaultContentTypeValues("practicegroupname")
                //    ,sPracticeGroupId=getDefaultContentTypeValues("practicegroupid")
                //    ,sAreaOfLawName=getDefaultContentTypeValues("areatermname")
                //    ,sAreaOfLawId=getDefaultContentTypeValues("areatermid"),
                //    sSubareaOfLawName=getDefaultContentTypeValues("subareatermname"),
                //    sSubareaOfLawId = getDefaultContentTypeValues("subareatermid");
                var sCheckByUserEmail = (undefined !== cm.selectedConflictCheckUser && null !== cm.selectedConflictCheckUser) ? getUserName(cm.selectedConflictCheckUser.trim() + ";", false) : "";
                var sCheckBy = getUserEmail(sCheckByUserEmail);
                var sBlockUserEmail = (undefined !== cm.blockedUserName && null !== cm.blockedUserName) ? getUserName(cm.blockedUserName.trim() + ";", false) : "";
                var sBlockUserName =sBlockUserEmail;
                var matterMetdataVM = {

                    Matter: {
                        Name: cm.matterName.trim(),
                        Id: cm.matterId,
                        Description: cm.matterDescription,
                        Conflict: {
                            Identified: cm.conflictRadioCheck,
                            CheckBy: sCheckBy,
                            CheckOn: cm.conflictDate,
                            SecureMatter: "True"
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
                        Url: "https://msmatter.sharepoint.com/sites/microsoft"
                    },
                    MatterConfigurations: {
                        IsConflictCheck: cm.chkConfilctCheck,
                        IsMatterDescriptionMandatory: true,
                        IsCalendarSelected: cm.includeCalendar,
                        IsTaskSelected: cm.includeTasks
                    },
                    UserIds: cm.userIDs,                   
                    MatterProvisionFlags: {},
                    HasErrorOccurred:false
                };
  console.log("options for creaitn matter");
                console.log(matterMetdataVM);
               // sCheckBy = ""; sBlockUserName = "";
                if ("" !== sCheckBy && "" != sBlockUserName) {
                    cm.successMsg="Step 1/3: Creating matter library and OneNote library...";
                    cm.successBanner = true; cm.createBtnDisabled=true;
                    createMatter(matterMetdataVM, function (response) {

                        ///  cm.clientNameList = response.clientTerms;
                        console.log("createMatter API success");
                        console.log(response);

                       
                        cm.successMsg="Step 2/3: Assigning permissions to matter library and OneNote library,associating Content Types, creating view and matter landing page..."
                        //    cm.popupContainerBackground = "show";// jQuery('#myModal').modal('show');
                        associateContentTypes();
                        assignPermission();
                        createMatterLandingPage();

                    });
                } else {
                   // cm.popupContainerBackground = "hide";
                    cm.errTextMsg = "Error in creation of matter: Incorrect inputs.";
                    // showErrorNotification("mcreate"); 
                    showErrorNotificationAssignTeams(cm.errTextMsg, "", "btnCreateMatter");
                    cm.errorBorder = "";
                    cm.errorPopUpBlock = true;
                    cm.popupContainerBackground = "hide";
                    $event.stopPropagation();cm.createBtnDisabled=false;  cm.successBanner = false;
                    return false;

                   

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
                var sPracticeGroupName = getDefaultContentTypeValues("practicegroupname")
                   , sPracticeGroupId = getDefaultContentTypeValues("practicegroupid")
                   , sAreaOfLawName = getDefaultContentTypeValues("areatermname")
                   , sAreaOfLawId = getDefaultContentTypeValues("areatermid"),
                   sSubareaOfLawName = getDefaultContentTypeValues("subareatermname"),
                   sSubareaOfLawId = getDefaultContentTypeValues("subareatermid");

                var optionsForAssignContentTypeMetadata = {
                    Client: {
                        Id: cm.clientId,
                        Name: "Microsoft",
                        Url: "https://msmatter.sharepoint.com/sites/microsoft"
                    },
                    Matter: {
                        Name: cm.matterName.trim(),
                        Id: cm.matterId,
                        ContentTypes: contentTypes,
                        DefaultContentType: defaultContentType,
                        MatterGuid: matterGUID
                    },
                    PracticeGroupTerm: {
                        TermName: sPracticeGroupName,
                        Id: sPracticeGroupId
                    },
                    AreaTerm: {
                        TermName: sAreaOfLawName,
                        Id: sAreaOfLawId
                    },
                    SubareaTerm: {
                        TermName: sSubareaOfLawName,
                        Id: sSubareaOfLawId
                    }

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
                        Name: "Microsoft",
                        Url: "https://msmatter.sharepoint.com/sites/microsoft"
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
                var sBlockUserEmail = (undefined !== cm.blockedUserName && null !== cm.blockedUserName) ? getUserName(cm.blockedUserName.trim() + ";", false) : "";
                var sBlockUserName = sBlockUserEmail;
                var arrPermissions = [];
                arrPermissions = getAssignedUserPermissions();
                var optionsForCreateMatterLandingPage = {

                    Client: {
                        Id: cm.clientId,
                        Name: "Microsoft",
                        Url: "https://msmatter.sharepoint.com/sites/microsoft"
                    },
                    MatterConfigurations: {

                        IsConflictCheck: cm.chkConfilctCheck,
                        IsMatterDescriptionMandatory: true,
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
                cm.successMsg = "Step 3/3: Updating metadata of matter library and sharing matter with the users...";
                var sCheckByUserEmail = (undefined !== cm.selectedConflictCheckUser && null !== cm.selectedConflictCheckUser) ? getUserName(cm.selectedConflictCheckUser.trim() + ";", false) : "";
                var sCheckBy = getUserEmail(sCheckByUserEmail);
                var sBlockUserEmail = (undefined !== cm.blockedUserName && null !== cm.blockedUserName) ? getUserName(cm.blockedUserName.trim() + ";", false) : "";
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
                var sPracticeGroupList = "", sAreaOfLawList = "", sSubAreaOfLawList = ""; var oMatterProvisionFlags = {};
               

                var sResponsibleAttorney = [], sResponsibleAttorneyEmail = [], arrTeamMembers = [], arrDocumentTemplatesCount=[];
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


                angular.forEach(oDocumentTemplates, function (item) {
                    if (-1 === subAreaofLaw.indexOf(item.termName)) {
                        subAreaofLaw += item.termName + "; ";
                        if (item.primaryMatterType) {
                            defaultContentType = item.termName;
                        }
                    }
                });

                angular.forEach(cm.assignPermissionTeams, function (item) {
                  //  var sCurrElementID = $(this).attr("id");
                    if ("" !== item.assignedRole && "" !== item.assignedPermission) {
                      //  sCurrElementID = sCurrElementID.trim().split("txtAssign")[1];
                     //   var sCurrRole = $("#ddlRoleAssign" + sCurrElementID), sCurrPermission = $("#txtAssign" + sCurrElementID);
                      //  if (sCurrRole && sCurrPermission) {
                        if (roleInformation.hasOwnProperty(item.assignedRole.name)) {
                                // This role is already present. append the new role with semicolon separated value
                            //   roleInformation[sCurrRole.val()] = roleInformation[sCurrRole.val()] + sCurrPermission.val();
                            roleInformation[item.assignedRole.name] = roleInformation[item.assignedRole.name]+";" + item.assignedUser;
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
                    arrUserNames.push(getUserName(item.assignedUser.trim()+ ";", true));
                    arrUserEmails.push(getUserName(item.assignedUser.trim()+ ";", false));
                    arrTeamMembers.push(getUserName(item.assignedUser.trim()+ ";",true).join(";"));
                });
                contentTypes = subAreaofLaw.trim().split(";");
                angular.forEach(oDocumentTemplates,  function (item) {
                    arrDocumentTemplatesCount.push(item.documentTemplateNames.split(";").length.toString());
                });
                arrDocumentTemplatesCount.reverse();

               
               oMatterProvisionFlags = {
                    "MatterLandingFlag": cm.bMatterLandingPage,
                    "SendEmailFlag": true
                };
               
                angular.forEach(oDocumentTemplates, function (item) {
                    if (-1 === sPracticeGroupList.indexOf(item.practicegroup)) {
                        sPracticeGroupList = item.practicegroup + "; " + sPracticeGroupList;
                    }
                    if (-1 === sAreaOfLawList.indexOf(item.areaoflaw)) {
                        sAreaOfLawList = item.areaoflaw + "; " + sAreaOfLawList;
                    }
                    if (-1 === sSubAreaOfLawList.indexOf(item.documentTemplates)) {
                        sSubAreaOfLawList =item.documentTemplates + "; " + sSubAreaOfLawList;
                    }
                });
               

                var optionsForStampMatterDetails = {
                    Client: {
                        Id: cm.clientId,
                        Name: "Microsoft",
                        Url: "https://msmatter.sharepoint.com/sites/microsoft"
                    },
                    Matter: {
                        Name: cm.matterName.trim(),
                        Id: cm.matterId.trim(),
                        Description: cm.matterDescription,
                        Conflict: {
                            Identified: cm.conflictRadioCheck,
                            CheckBy: sCheckBy,
                            CheckOn: cm.conflictDate,
                            SecureMatter: "True"
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
                    MatterDetails:{
                        PracticeGroup: sPracticeGroupList,
                        AreaOfLaw: sAreaOfLawList,
                        SubareaOfLaw: subAreaofLaw,
                        ResponsibleAttorney: sResponsibleAttorney.join(";").replace(/;;/g, ";"),
                        ResponsibleAttorneyEmail: sResponsibleAttorneyEmail.join(";").replace(/;;/g, ";"),
                        UploadBlockedUsers: arrReadOnlyUsers,
                        TeamMembers: arrTeamMembers.join(";"),
                        RoleInformation: JSON.stringify(roleInformation)
                    },
                    MatterProvisionFlags: oMatterProvisionFlags,
                    MatterConfigurations: {
                        IsConflictCheck: cm.chkConfilctCheck,
                        IsMatterDescriptionMandatory: true                       
                    }
                }
                console.log("options for optionsForStampMatterDetails matter");
                console.log(optionsForStampMatterDetails);
                updateMatterMetadataAPI(optionsForStampMatterDetails, function (response) {
                  console.log("stampProperties Success");
                  console.log(response);
                  cm.successMsg="Matter is successfully created. You can find recently created matter over here.";
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
            //            Url: "https://msmatter.sharepoint.com/sites/microsoft"
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


            function getDefaultContentTypeValues(contentTypeValue){
                var returnedValue;
                switch (contentTypeValue) {
                    case "contenttypes":
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [],defaultContentType="";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("contenttypes" == contentTypeValue) {
                                    if(-1==arrContents.indexOf(arrContentTypes[nCount].termName)){
                                    arrContents.push(arrContentTypes[nCount].termName);
                                    }
                            
                                    var arrAssociatedDocumentTemplates=arrContentTypes[nCount].documentTemplateNames.split(";");
                                    for(var iIterator = 0; iIterator < arrAssociatedDocumentTemplates.length; iIterator++){
                                        if(-1==arrContents.indexOf(arrAssociatedDocumentTemplates[iIterator])){
                                            arrContents.push(arrAssociatedDocumentTemplates[iIterator]);
                                        }
                                    }
                                }
                            }
                        }
                        returnedValue=arrContents;
                        break;
                    case "defaultcontenttype":
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [],defaultContentType="";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("defaultcontenttype" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].termName;
                                    }
                                }
                            }
                        }
                        returnedValue=defaultContentType;
                        break;
                    case "practicegroupname":
                      
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [],defaultContentType="";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("practicegroupname" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].practicegroup;
                                    }
                                }
                            }
                        }
                        returnedValue=defaultContentType;
                        break;
                    case "practicegroupid":
                      
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [],defaultContentType="";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("practicegroupid"==contentTypeValue ) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].practicegroupId;
                                    }
                                }
                            }
                        }
                        returnedValue=defaultContentType;
                        break;
                    case "subareatermname":
                       
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [],defaultContentType="";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("subareatermname" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].termName;
                                    }
                                }
                            }
                        }
                        returnedValue=defaultContentType;
                        break;
                    case "subareatermid":
                       
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [],defaultContentType="";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("subareatermid" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].id;
                                    }
                                }
                            }
                        }
                        returnedValue=defaultContentType;
                        break;
                    case "areatermname":
                       
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [],defaultContentType="";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("areatermname" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].areaoflaw;
                                    }
                                }
                            }
                        }
                        returnedValue=defaultContentType;
                        break;
                    case "areatermid":
                       
                        var oPageOneData = JSON.parse(localStorage.oPageOneData), nCount = 0, nLength, arrContentTypes, arrContents = [],defaultContentType="";
                        if (oPageOneData && oPageOneData.oSelectedDocumentTypeLawTerms) {
                            arrContentTypes = oPageOneData.oSelectedDocumentTypeLawTerms;
                            nLength = arrContentTypes.length;
                            for (nCount = 0; nCount < nLength; nCount++) {
                                if ("areatermid" == contentTypeValue) {
                                    if (arrContentTypes[nCount].primaryMatterType === true) {
                                        defaultContentType = arrContentTypes[nCount].areaoflawId;
                                    }
                                }
                            }
                        }
                        returnedValue=defaultContentType;
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
                var arrAssigneTeams = cm.assignPermissionTeams, nCount = 0, nlength ,  arrRoles = [];
                if(arrAssigneTeams){
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


            function clearAllProperties(){
                cm.clientId = "";
                cm.selectedClientName = "";
                cm.matterName = "";
                cm.matterId = "";
                cm.matterDescription = "";

                cm.clientNameList = [];
                cm.areaOfLawTerms = [];
                cm.subAreaOfLawTerms = [];
                cm.documentTypeLawTerms = [];
                cm.selectedDocumentTypeLawTerms = [];
                cm.activeAOLTerm = null;
                cm.activeSubAOLTerm = null;
                cm.activeDocumentTypeLawTerm = null;
                cm.popupContainerBackground = "hide";
                cm.popupContainer = "hide";
cm.createBtnDisabled=false;

                cm.sectionName = "snOpenMatter";
                cm.removeDTItem = false;
                cm.primaryMatterType = cm.errorPopUp = false;
                cm.matterGUID = "";
                cm.iCurrentPage = 1;
                cm.assignPermissionTeams = [{ assignedUser: '', assignedRole: '', assignedPermission: '', assigneTeamRowNumber: 1 }];
                cm.assignRoles = [];
                cm.assignPermissions = [];
                cm.secureMatterCheck = true;
                cm.conflictRadioCheck = true;
                localStorage.iLivePage = 1;
                localStorage.removeItem("oPageOneData");
                localStorage.removeItem("oPageTwoData");
                getMatterGUID();

               getTaxonomyData();

            }

            function storeMatterDataToLocalStorageFirstPage() {
                oPageOneState.ClientValue.push({ ClientName: cm.clientNameList });
                oPageOneState.ClientId = cm.clientId.trim();
                oPageOneState.MatterName = cm.matterName.trim();
                oPageOneState.MatterId = cm.matterId.trim();
                oPageOneState.MatterDescription = cm.matterDescription.trim();
                oPageOneState.oAreaOfLawTerms = cm.areaOfLawTerms; 
                oPageOneState.oSubAreaOfLawTerms = cm.subAreaOfLawTerms;
                oPageOneState.matterGUID = cm.matterGUID;
                oPageOneState.oSelectedDocumentTypeLawTerms = cm.selectedDocumentTypeLawTerms;
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
                 localStorage.setItem('IsCalendarSelected',cm.includeCalendar);
                  localStorage.setItem('IsRestrictedAccessSelected',  cm.secureMatterCheck);
                    localStorage.setItem('IsRSSSelected', cm.includeRssFeeds);
                  localStorage.setItem('IsEmailOptionSelected',  cm.includeEmail);
                    localStorage.setItem('IsMatterDescriptionMandatory',  cm.isMatterDescriptionMandatory );
                     localStorage.setItem('IsConflictCheck',  cm.chkConfilctCheck);
                    localStorage.setItem('IsMatterDescriptionMandatory',  cm.isMatterDescriptionMandatory);
                     localStorage.setItem('IsTaskSelected', cm.includeTasks);
                  

                //cm.sectionName = sectionName;
                localStorage.iLivePage =  2;

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
                localStorage.iLivePage  = 3;
            }

            cm.NextClick = function () {
                if (cm.iCurrentPage == 1) {
                    cm.navigateToSecondSection("snConflictCheck");
                }
                else if (cm.iCurrentPage == 2) {
                    cm.navigateToSecondSection("snCreateAndShare");
                }

            }

            cm.PreviousClick = function () {
                if (cm.iCurrentPage == 2) {
                cm.navigateToSecondSection("snOpenMatter");

                }
                else if (cm.iCurrentPage == 3) {
                 cm.navigateToSecondSection("snConflictCheck");
                }

            }


            cm.CheckPopUp = function (e) {
                //  e.stopPropagation();

             
                cm.errorPopUpBlock = false;
                cm.errorBorder = "";
             
            }

            function validateCurrentPage(iCurrPage) {
                if (iCurrPage == 1) {
                    var windowWidth = GetWidth();
                    var RegularExpression;
                    if (undefined !==cm.selectedClientName && null !== cm.selectedClientName && "" !== cm.selectedClientName.trim()) {
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
                                        var sCurrentMatterId=cm.matterId;
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
                                            cm.errTextMsg = "Enter a matter ID.";
                                            cm.errorBorder = "matterid";
                                            showErrorNotification("matterid");
                                            cm.errorPopUpBlock = true; return false;
                                        }
                                        if (bInValid) {

                                           
                                            if (cm.isMatterDescriptionMandatory) {
                                                var sCurrentMatterDesc = cm.matterDescription;
                                            if (undefined !== sCurrentMatterDesc && null !== sCurrentMatterDesc && "" !== sCurrentMatterDesc) {
                                                sCurrentMatterDesc = sCurrentMatterDesc.trim(); bInValid = false;
                                                var arrValidMatch = sCurrentMatterDesc.match(RegularExpression);
                                                if (null === arrValidMatch || arrValidMatch[0] !== sCurrentMatterDesc) {
                                                    bInValid = false;
                                                } else {
                                                    bInValid = true;
                                                }
                                            }
                                            else {
                                                cm.errTextMsg = "Enter a description for this matter.";
                                              
                                                showErrorNotification("matterdescription");
                                                cm.errorBorder = "matterdescription";
                                                cm.errorPopUpBlock = true; return false;
                                            }
                                        }
                                            else {
                                                cm.matterDescription = "";
                                                bInValid = true;
                                        }

                                           // if (cm.matterDescription && "" !== cm.matterDescription.trim() && null !== cm.matterDescription) {
                                            if (bInValid) {
                                                if (cm.selectedDocumentTypeLawTerms.length > 0) {
                                                    storeMatterDataToLocalStorageFirstPage();
                                                   
                                                    return true;
                                                }
                                                else {
                                                    cm.errTextMsg = "Select matter type by area of law for this matter";
                                                    cm.errorBorder = ""; showErrorNotification("selecttemp");
                                                    cm.errorPopUpBlock = true; return false;
                                                }
                                            }
                                            else {
                                                // alert("Enter a description for this matter");
                                                cm.errTextMsg = "Please enter a valid text which contains only alphanumeric characters, spaces & hyphen";
                                              //  cm.errTextMsg = "Enter a description for this matter.";
                                                //   cm.errorPopUpBlock = "matterDescription";
                                                showErrorNotification("matterdescription");
                                                cm.errorBorder = "matterdescription";
                                                cm.errorPopUpBlock = true; return false;

                                            }
                                        }
                                        else {
                                            cm.errTextMsg = "Please enter a valid text which contains only alphanumeric characters, spaces & hyphen.";
                                            cm.errorBorder = "matterid";
                                            showErrorNotification("matterid");
                                            cm.errorPopUpBlock = true; return false;
                                        }

                                    }
                                    else {
                                        cm.errTextMsg = "Matter library for this Matter is already created. Kindly delete the library or please enter a different Matter name.";
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
                                cm.errTextMsg = "Please enter a valid Matter name which contains only alphanumeric characters and spaces";
                                cm.errorBorder = "mattername";
                                showErrorNotification("mattername");
                              
                                cm.errorPopUpBlock = true; return false;
                            }

                        }
                        else {
                            cm.errTextMsg = "Selected  client for this matter clientId is null ";
                            cm.errorBorder = "client";
                            cm.errorPopUpBlock = true; return false;
                        }
                    }
                    else {
                        cm.errTextMsg = "Select a client for this matter ";
                        cm.errorBorder = "client";                      
                        showErrorNotification("client");
                        cm.errorPopUpBlock = true;
                        return false;
                    }

                }
                else if (iCurrPage == 2) {
                    if (undefined !== cm.chkConfilctCheck && true == cm.chkConfilctCheck) {

                        if (undefined !== cm.conflictDate && null !== cm.conflictDate && "" != cm.conflictDate) {
                            // cm.conflictDate = new Date();
                            var validUsers = validateUsers();
                            if (validUsers) {
                                var attornyCheck = validateAttornyUserRolesAndPermissins();
                                if (attornyCheck) {
                                    cm.popupContainerBackground = "Show";
                                    storeMatterDataToLocalStorageSecondPage();
                                    callCheckSecurityGroupExists("snCreateAndShare");

                                    // cm.sectionName = sectionName;
                                }
                            }

                        }
                        else {
                            cm.errTextMsg = "Enter the date on which the conflict check was performed ";
                            cm.errorBorder = "cdate"; showErrorNotification("cdate");
                            cm.errorPopUpBlock = true; return false;
                        }


                        // callCheckSecurityGroupExists();
                        // console.log(getAssignedUserRoles());


                    }
                    else {
                        //  cm.sectionName = sectionName;
                        cm.errTextMsg = "A confilct check must be completed prior to provisioning this matter ";
                        cm.errorBorder = "";
                        showErrorNotification("conflictcheck");
                        cm.errorPopUpBlock = true;

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

                            matterErrorEle.classList.add("errPopUpMatterName");
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

                            matterErrorEle.classList.add("errPopUpMatterDescr");
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
                            matterErrorEle.classList.add("errPopUpMatterSelectTemp");
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

            function showErrorNotificationAssignTeams(errorMsg, teamRowNumber, type)
            {
                var fieldType = "";

                if (type == "user") {
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

                var temp = document.getElementById(fieldType +teamRowNumber);
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
                    errTextMatterCAttorny = document.createElement('style');
                errPopUpCAttorny.type = 'text/css';
                errTringleBlockCAttorny.type = 'text/css';
                errTringleBorderCAttorny.type = 'text/css';
                errTextMatterCAttorny.type = 'text/css';

                var width = GetWidth();
                var x=0,y=0;
                if (width > 734) {
                    y = temp.offsetTop - 26, x = temp.offsetLeft + 45;
                }
                else {
                    y = temp.offsetTop+57 , x = temp.offsetLeft+10 ;
                }
               
               
                errPopUpCAttorny.innerHTML = ".errPopUpCAttorny{top:" + y + "px;left:" +x+ "px;}";
                errTringleBlockCAttorny.innerHTML = "{min-height: 40px;top: 17px !important;left: 24px;width:100%}";
                errTringleBorderCAttorny.innerHTML = "{min-height: 40px,top: 17px !important;left: 24px;width:100%}";
                errTextMatterCAttorny.innerHTML = "{min-height:40px;top:21px !important;left: 24px;width:100%}";
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
            }

            function resizeErrorPopup() {
                "use strict";
                
                var windowWidth = GetWidth();
                var width;
                cm.errorPopUpBlock = false;

                if (windowWidth <= 734) {
                    cm.errorPopUpBlock = false;
                }
            }

            cm.closeSuccessBanner = function () {
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

        }]);

        app.filter('getAssociatedDocumentTemplatesCount', function () {
            return function (input, splitChar) {

                return input.split(splitChar).length;;
            }
        });

      
        

       
})();