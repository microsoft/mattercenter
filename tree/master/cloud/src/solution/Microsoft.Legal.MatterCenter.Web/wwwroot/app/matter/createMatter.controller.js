(function () {
    'use strict';

    var app=  angular.module("matterMain");
    app.controller('createMatterController', ['$scope', '$state', '$stateParams', 'api','matterResource',
        function ($scope, $state, $stateParams, api,matterResource) {
            var cm = this;
            cm.selectedConflictCheckUser = undefined;
            cm.blockedUserName = undefined;
            cm.chkConfilctCheck = undefined;
            cm.conflictRadioCheck = true;
            
            cm.createButton = "Create";

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
                oSelectedDocumentTypeLawTerms:[]
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
            cm.iCurrentPage = 0;
            cm.assignPermissionTeams = [{ assignedUser: '', assignedRole: '', assignedPermission: '', assigneTeamRowNumber:  1 }];
            cm.assignRoles = [];
            cm.assignPermissions = [];
            cm.secureMatterCheck = "True";
            cm.conflictRadioCheck = true;
            
            ///* Function to generate 32 bit GUID */
            function get_GUID() {
                function create_GUID(bFlag) {
                    var sCurrentGUID = (Math.random().toString(16) + "000000000").substr(2, 8);
                    return bFlag ? "-" + sCurrentGUID.substr(0, 4) + "-" + sCurrentGUID.substr(4, 4) : sCurrentGUID;
                }
                return create_GUID() + create_GUID(true) + create_GUID(true) + create_GUID();
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
            var siteCollectionPath = "https://msmatter.sharepoint.com/sites/catalog";
       
            function getDefaultMatterConfigurations(siteCollectionPath, callback) {
                api({
                    resource:'matterResource',
                    method: 'getDefaultMatterConfigurations',
                    data: JSON.stringify(siteCollectionPath),
                    success:callback
                });
            }

            function getTaxonomyDetailsForClient(optionsForClientGroup, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getTaxonomyDetails',
                    data: optionsForClientGroup,
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

            

            optionsForClientGroup = {
                Client: {
                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                },
                TermStoreDetails: {
                    TermGroup: "MatterCenterTerms",
                    TermSetName: "Clients",
                    CustomPropertyName: "ClientURL"
                }
            }
               
           
            optionsForPracticeGroup = {
                Client: {

                    Url: "https://msmatter.sharepoint.com/sites/catalog"
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

                    Url: "https://msmatter.sharepoint.com/sites/catalog"
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

                        Url: "https://msmatter.sharepoint.com/sites/catalog"
                    },
                    SearchObject: {
                        SearchTerm: val
                    }
                };

                return matterResource.getUsers(searchUserRequest).$promise;
            }

            //call back function for getting the clientNamesList
            getTaxonomyDetailsForClient(optionsForClientGroup, function (response) {              

                cm.clientNameList = response.clientTerms;
                cm.popupContainerBackground = "hide";// jQuery('#myModal').modal('show');


            });

            var optionsForRoles = new Object;
            optionsForRoles = {                                
               
                Url: "https://msmatter.sharepoint.com/sites/catalog"
                
            }
            getRoles(optionsForRoles, function (response) {
                //console.log("roles");
                //console.log(response);
                cm.assignRoles = response;
            });
            var optionsForPermissionLevels = new Object;
            optionsForPermissionLevels = {
               
                Url: "https://msmatter.sharepoint.com/sites/catalog"
            }
            getPermissionLevels(optionsForPermissionLevels, function (response) {
                //console.log("Permission Levels");
                //console.log(response);
                cm.assignPermissions = response;
            });

            //calls this function when selectType button clicks
            cm.selectMatterType = function () {
                cm.popupContainerBackground = "Show";
                if (cm.pracitceGroupList == null) {
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

             
                cm.clientId = cm.selectedClientName;
                if (null != cm.clientId) {

                    getDefaultMatterConfigurations(siteCollectionPath, function (response) {
                       // console.log(response); 
                    });
                }
                else {

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
                optionsForCheckMatterName.Matter.Name = cm.matterName.trim();
                if ("" !== cm.matterName.trim()) {
                    getCheckValidMatterName(optionsForCheckMatterName, function (response) {
                        if (response.code != 200) {
                            alert(response.code + "Matter Name already Exists");
                        } else {
                          //  alert("success");
                        }
                    });
                }

            }
            

            

         
   

            cm.navigateToSecondSection = function (sectionName) {
                if (sectionName == "snConflictCheck") {
                    if (null !== cm.selectedClientName && "" !== cm.selectedClientName.trim()) {
                        if ("" !== cm.clientId.trim() && null !== cm.clientId) {
                            if ("" != cm.matterName.trim() && null !== cm.matterName) {
                                if ("" !== cm.matterId.trim() && null != cm.matterId) {
                                    if ("" !== cm.matterDescription.trim() && null !== cm.matterDescription) {

                                        if (cm.selectedDocumentTypeLawTerms.length > 0) {

                                            oPageOneState.ClientValue.push({ ClientName: cm.clientNameList });
                                            oPageOneState.ClientId = cm.clientId.trim();
                                            oPageOneState.MatterName = cm.matterName.trim();
                                            oPageOneState.MatterId = cm.matterId.trim();
                                            oPageOneState.MatterDescription = cm.matterDescription.trim();
                                            oPageOneState.oAreaOfLawTerms = cm.areaOfLawTerms;
                                            oPageOneState.oSubAreaOfLawTerms = cm.subAreaOfLawTerms;
                                            oPageOneState.matterGUID = cm.matterGUID;
                                            oPageOneState.oSelectedDocumentTypeLawTerms = cm.selectedDocumentTypeLawTerms;
                                            //oPageOneState.oSectionName = cm.sectionName;
                                            localStorage.setItem('oPageOneData', JSON.stringify(oPageOneState));
                                            cm.sectionName = sectionName;
                                            localStorage.iLivePage = cm.iCurrentPage = 2;
                                        }
                                        else {
                                            alert("Select matter type by area of law for this matter");
                                        }
                                    }
                                    else {
                                        alert("Enter a description for this matter");
                                    }
                                }
                                else {
                                    alert("Enter a matter ID.");
                                }

                            }
                            else {
                                alert("Please enter a valid Matter name which contains only alphanumeric characters and spaces");
                            }

                        }
                        else {
                            alert("Selected  client for this matter clientId is null ");
                        }
                    }
                    else {
                        alert("Select a client for this matter ");
                    }
                }
                else if (sectionName == "snCreateAndShare") {


                    if (undefined !== cm.chkConfilctCheck && true == cm.chkConfilctCheck) {

                        cm.sectionName = sectionName;
                        

                        callCheckSecurityGroupExists();
                        // console.log(getAssignedUserRoles());
                       

                    }
                    else {
                        cm.sectionName = sectionName;

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
                else {
                    cm.sectionName = sectionName;

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
            
          var callCheckSecurityGroupExists=function(){
                
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

                        Url: "https://msmatter.sharepoint.com/sites/catalog"
                    },
                    Matter: {
                        Name : cm.matterName.trim(),
                        AssignUserNames : cm.arrAssignedUserName,
                        AssignUserEmails :cm.arrAssignedUserEmails,
                        Conflict:{
                            Identified : "True"
                        },
                        BlockUserNames : (undefined !== cm.blockedUserName && null !== cm.blockedUserName) ? getUserName(cm.blockedUserName.trim() + ";", false) : "",
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


            //cm.dateOptions = {
               
            //    formatYear: 'yy',
            //    maxDate: new Date(2020, 5, 22),
            //    minDate: new Date(),
            //    startingDay: 1
            //};

            //cm.open1 = function () {
            //    cm.opened = true;
            //};

            //cm.opened= false;
           
            //$scope.SelectModal= function(){
            //   // jQuery('#myModal').modal('show');
            //}
            cm.conflictRadioCheckValue = true;
            cm.conflictRadioChange = function (value) {
                cm.blockedUserName = "";
            }
           
            cm.addNewAssignPermissions = function () {
                var newItemNo = cm.assignPermissionTeams.length + 1;
                cm.assignPermissionTeams.push({ 'assigneTeamRowNumber':  newItemNo });
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
                if (localStorage.getItem("iLivePage") == 2) {
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

                    cm.sectionName = "";
                    cm.removeDTItem = false;
                    cm.primaryMatterType = cm.errorPopUp = false;
                    cm.matterGUID = oPageData.matterGUID;
                    cm.iCurrentPage = 0;
                    cm.navigateToSecondSection("");
                    cm.navigateToSecondSection("snConflictCheck");
                 

                }
            }

            cm.includeEmail = true;
            if (cm.includeEmail) {
                cm.createButton = "Create and Notify";
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





            cm.createMatterButton = function () {

                var matterGUID = cm.matterGUID;
                var arrFolderNames=[];
                arrFolderNames = retrieveFolderStructure();
                var arrRoles = [];
                arrRoles = getAssignedUserRoles();
                var arrPermissions = [];
                arrPermissions = getAssignedUserPermissions();
                var contentTypes = [];
                var defaultContentType = "";
                contentTypes = getDefaultContentTypeValues("contenttypes");
                defaultContentType = getDefaultContentTypeValues("defaultcontenttype");
                var sPracticeGroupName=getDefaultContentTypeValues("practicegroupname")
                    ,sPracticeGroupId=getDefaultContentTypeValues("practicegroupid")
                    ,sAreaOfLawName=getDefaultContentTypeValues("areatermname")
                    ,sAreaOfLawId=getDefaultContentTypeValues("areatermid"),
                    sSubareaOfLawName=getDefaultContentTypeValues("subareatermname"),
                    sSubareaOfLawId=getDefaultContentTypeValues("subareatermid");

                var optionsForMatterMetaDataVM =  {
                    Client: {
                        Id: cm.clientId,
                        Name:"Microsoft",
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
                    },
                    Matter: {
                        Name: cm.matterName.trim(),
                        Id: cm.matterId,
                        Description:cm.matterDescription,
                      
                        Conflict:{
                            Identified: cm.conflictRadioCheck,
                            CheckBy:"venkatm@MSmatter.onmicrosoft.com",
                          //  CheckBy: (undefined !== cm.selectedConflictCheckUser && null !== cm.selectedConflictCheckUser) ? getUserName(cm.selectedConflictCheckUser.trim() + ";", false) : "",
                            CheckOn : cm.conflictDate,
                            SecureMatter : "True"
                        },
                        AssignUserNames: cm.arrAssignedUserName,
                        AssignUserEmails : cm.arrAssignedUserEmails,
                        BlockUserNames:"SaiG@MSmatter.onmicrosoft.com",
                      //  BlockUserNames : (undefined !== cm.blockedUserName && null !== cm.blockedUserName) ? getUserName(cm.blockedUserName.trim() + ";", false) : "",
                        MatterGuid:cm.matterGUID,
                        FolderNames: arrFolderNames,
                        Roles: arrRoles
                    },
                    MatterConfigurations:{
                        IsConflictCheck: cm.chkConfilctCheck,
                        IsMatterDescriptionMandatory : true,
                        IsCalendarSelected: cm.includeCalendar,
                        IsTaskSelected: cm.includeTasks
                    },
                    UserIds: cm.userIDs,
                    HasErrorOccurred: false
                }

                createMatter(optionsForMatterMetaDataVM, function (response) {

                    ///  cm.clientNameList = response.clientTerms;
                    console.log(response);
                    cm.popupContainerBackground = "show";// jQuery('#myModal').modal('show');


                });
               


             


                

               


            }
          

            function associateContentTypes() {
                var optionsForAssignContentTypeMetadata = {
                    Client: {
                        Id: cm.clientId,
                        Name: "Microsoft",
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
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
            }

            function assignPermission() {
                var optionsForAssignUserPermissionMetadataVM = {

                    Client: {
                        Id: cm.clientId,
                        Name: "Microsoft",
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
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
            }


            function createMatterLandingPage() {

                var optionsForCreateMatterLandingPage = {
                    Client: {
                        Id: cm.clientId,
                        Name: "Microsoft",
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
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
                        BlockUserNames: (undefined !== cm.blockedUserName && null !== cm.blockedUserName) ? getUserName(cm.blockedUserName.trim() + ";", false) : "",
                        Conflict: {
                            Identified: cm.conflictRadioCheck,
                            CheckBy: (undefined !== cm.selectedConflictCheckUser && null !== cm.selectedConflictCheckUser) ? getUserName(cm.selectedConflictCheckUser.trim() + ";", false) : "",
                            CheckOn: cm.conflictDate,
                            SecureMatter: "True"
                        },
                        Permissions: arrPermissions,
                        MatterGuid: matterGUID
                    }
                }
            }
           

            function updateMatterMetadata() {
                var optionsForMatterMetadata = {
                    Client: {
                        Id: cm.clientId,
                        Name: "Microsoft",
                        Url: "https://msmatter.sharepoint.com/sites/catalog"
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
                        Id: cm.matterId,
                        Description: cm.matterDescription,
                        Conflict: {
                            Identified: cm.conflictRadioCheck,
                            CheckBy: (undefined !== cm.selectedConflictCheckUser && null !== cm.selectedConflictCheckUser) ? getUserName(cm.selectedConflictCheckUser.trim() + ";", false) : "",
                            CheckOn: cm.conflictDate,
                            SecureMatter: "True"
                        },
                        AssignUserNames: cm.arrAssignedUserName,
                        AssignUserEmails: cm.arrAssignedUserEmails,
                        BlockUserNames: (undefined !== cm.blockedUserName && null !== cm.blockedUserName) ? getUserName(cm.blockedUserName.trim() + ";", false) : "",

                        Permissions: arrPermissions,
                        MatterGuid: matterGUID
                    }
                }
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
                                    arrContents.push(arrContentTypes[nCount].termName);
                            
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


        }]);

        app.filter('getAssociatedDocumentTemplatesCount', function () {
            return function (input, splitChar) {

                return input.split(splitChar).length;;
            }
        });
      
        

       
})();