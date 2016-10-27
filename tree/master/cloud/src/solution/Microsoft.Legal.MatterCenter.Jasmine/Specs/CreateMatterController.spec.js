/// <reference path="../References.js" />
/// <disable>JS2074, JS3058, </disable>
//// ***********************************************************************
// Author           : MAQ USER
// Created          : 31-08-2016
//
// ***********************************************************************
// <copyright file="CreateMatterController.spec.js" company="MAQSoftware">
//  Copyright (c) . All rights reserved.
// </copyright>
// <summary>Test suite for createMatter controller</summary>
// ***********************************************************************

describe("CreateMatter Controller test suite", function () {
    "use strict";

    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("matterResource", ["$resource", "auth", mockMatterResourceService]);
    }));

    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("api", ["matterResource", "documentResource", "documentDashBoardResource", "matterDashBoardResource", "homeResource", mockapi]);
    }));

    beforeEach(module("ui.router"));
    beforeEach(module("ui.bootstrap"));

    beforeEach(inject(function ($controller, $injector, $rootScope) {
        rootScope = $rootScope.$new();
        vm = $controller("createMatterController as vm", { $scope: $scope, $rootScope: rootScope, $state: $state, $stateParams: $stateParams, matterResource: mockMatterResourceService, api: mockapi, adalAuthenticationService: mockadalAuthenticationService });
    }));

    describe("Verification of clearpopup function", function () {
        it("It should set an errorPopUpBlock to false", function () {
            vm.clearPopUp();
            expect(vm.errorPopUpBlock).toBe(false);
        });

        it("It should not define errorPopUpBlock", function () {
            expect(vm.errorPopUpBlock).toBeUndefined();
        });
    });
    
    describe("Verification of getMatterGUID function", function () {
        it("It should return the GUID for matter with length of 32 alphanumeric characters", function () {
            expect(vm.matterGUID.length).toBe(oTestConfiguration.nGUIDLength);
        });

        it("It should not return null value", function () {
            expect(vm.matterGUID).not.toBe(null);
        });

        it("It should return the GUID for matter with defined value", function () {
            expect(vm.matterGUID).toBeDefined();
        });
    });

    describe("Verification of selectMatterTypePopUpClose function", function () {
        it("It should hide popupContainerBackground and popupContainer", function () {
            vm.popupContainer = "Show";
            vm.selectMatterTypePopUpClose();
            expect(vm.popupContainerBackground).toBe("hide");
            expect(vm.popupContainer).toBe("hide");
        });

        it("It should show popupContainerBackground and should hide popupContainer", function () {
            vm.popupContainer = "hide";
            vm.selectMatterTypePopUpClose();
            expect(vm.popupContainerBackground).toBe("Show");
            expect(vm.popupContainer).toBe("hide");
        });
    });

    //// ***********This method is commented/ not present in latest microsoft build ***********

    //// describe("Verification of getSelectedPracticeGroupValue function", function () {
    ////    it("It should return the selected PracticeGroup Value for all AOL and SAOL terms)", function () {
    ////        vm.selectedPracticeGroup = selectedPracticeGroup;
    ////        vm.getSelectedPracticeGroupValue();
    ////        expect(vm.areaOfLawTerms).not.toBe(null);
    ////        expect(vm.subAreaOfLawTerms).not.toBe(null);
    ////        expect(vm.activeSubAOLTerm.termName).toBe(oTestConfiguration.sActiveSubAreaOfLawTerm);
    ////        expect(vm.activeAOLTerm.folderNames).toBe(oTestConfiguration.sFolderNames);
    ////        expect(vm.errorPopUp).toBe(false);
    ////    });

    ////    it("It should return the null value for selected PracticeGroup for AOL and SAOL terms", function () {
    ////        vm.selectedPracticeGroup = null;
    ////        vm.getSelectedPracticeGroupValue();
    ////        expect(vm.areaOfLawTerms).toBe(null);
    ////        expect(vm.subAreaOfLawTerms).toBe(null);
    ////    });
    ////});

    //// ***********This method is commented/ not present in latest microsoft build ***********

    ////describe("Verification of selectAreaOfLawTerm function", function () {
    ////    it("It should return the sub area of law term on selection of area of law term", function () {
    ////        vm.selectAreaOfLawTerm(selectedPracticeGroup.areaTerms[0]);
    ////        expect(vm.subAreaOfLawTerms).not.toBe(null);
    ////        expect(vm.activeSubAOLTerm.termName).toBe(oTestConfiguration.sActiveSubAreaOfLawTerm);
    ////        expect(vm.errorPopUp).toBe(false);
    ////        expect(vm.activeAOLTerm).toBe(selectedPracticeGroup.areaTerms[0]);
    ////    });

    ////    it("It should defined sub area of law term on selection of area of law term", function () {
    ////        vm.selectAreaOfLawTerm(selectedPracticeGroup.areaTerms[0]);
    ////        expect(vm.subAreaOfLawTerms).not.toBeUndefined();
    ////        expect(vm.activeSubAOLTerm).not.toBeUndefined();
    ////        expect(vm.errorPopUp).not.toBeUndefined();
    ////        expect(vm.activeAOLTerm).not.toBeUndefined();
    ////    });
    ////});

    //// ***********This method is commented/ not present in latest microsoft build ***********

    ////describe("Verification of selectSubAreaOfLawTerm function", function () {
    ////    it("It should return the sub area of law items", function () {
    ////        vm.selectSubAreaOfLawTerm(selectedPracticeGroup.areaTerms[0].subareaTerms[0]);
    ////        expect(vm.activeSubAOLTerm).toBe(selectedPracticeGroup.areaTerms[0].subareaTerms[0]);
    ////        expect(vm.errorPopUp).toBe(false);
    ////    });

    ////    it("It should not return undefined value for sub area of law items", function () {
    ////        vm.selectSubAreaOfLawTerm(selectedPracticeGroup.areaTerms[0].subareaTerms[0]);
    ////        expect(vm.activeSubAOLTerm).not.toBeUndefined();
    ////        expect(vm.errorPopUp).not.toBeUndefined();
    ////    });
    ////});

    describe("Verification of selectDocumentTemplateTypeLawTerm function", function () {
        it("It should not return null value for the document template type term", function () {
            vm.selectDocumentTemplateTypeLawTerm(documentTemplateTypeLawTerm);
            expect(vm.removeDTItem).toBe(true);
            expect(vm.errorPopUp).toBe(false);
            expect(vm.activeDocumentTypeLawTerm).toBe(documentTemplateTypeLawTerm);
            expect(vm.primaryMatterType).toBe(true);
        });

        it("It should return correct value for document template type term", function () {
            vm.selectDocumentTemplateTypeLawTerm(documentTemplateTypeLawTerm);
            expect(vm.removeDTItem).not.toBe(false);
            expect(vm.errorPopUp).not.toBe(true);
            expect(vm.activeDocumentTypeLawTerm).not.toBe(null);
            expect(vm.primaryMatterType).not.toBe(false);
        });
    });

    describe("Verification of addToDocumentTemplate function", function () {
        it("It should return the document template type law terms", function () {
            vm.activeSubAOLTerm = documentTemplateTypeLawTerm;
            vm.documentTypeLawTerms = subareaTerms;
            vm.addToDocumentTemplate();
            expect(vm.documentTypeLawTerms).not.toBeUndefined();
        });

        it("It should not return null value for activeDocumentTypeLawTerm", function () {
            vm.activeSubAOLTerm = documentTemplateTypeLawTerm;
            vm.documentTypeLawTerms = subareaTerms;
            vm.addToDocumentTemplate();
            expect(vm.activeDocumentTypeLawTerm).not.toBe("null");
        });
    });

    describe("Verification of removeFromDocumentTemplate function", function () {
        it("It should not return document template type law terms while removing", function () {
            vm.removeDTItem = false;
            vm.removeFromDocumentTemplate();
            expect(vm.removeDTItem).not.toBe(true);
            expect(vm.primaryMatterType).not.toBe(true);
        });

        it("It should return the document template type law terms while removing", function () {
            vm.removeDTItem = true;
            vm.removeFromDocumentTemplate();
            expect(vm.removeDTItem).toBe(false);
            expect(vm.primaryMatterType).toBe(false);
        });
    });

    describe("Verification of onSelect function", function () {

        it("It should return the conflicted user", function () {
            vm.onSelect(item, $model, $label, "conflictcheckuser", "on-blurr", event, item.name);
            expect(vm.oSiteUsers).toBeDefined();
            expect(vm.selectedConflictCheckUser).toBe(oEnvironmentConfiguration.loggedInUserName + "(" + oEnvironmentConfiguration.loggedInUserEmail + ")");
        });

        it("It should return the blocked user", function () {
            vm.onSelect(item, $model, $label, "blockuser", "on-blurr", event, item.name);
            expect(vm.oSiteUsers).toBeDefined();
            //expect(vm.blockedUserName).toBe(oEnvironmentConfiguration.loggedInUserName + "(" + oEnvironmentConfiguration.loggedInUserEmail + ")");
        });

        it("It should return the team member", function () {
            vm.onSelect(item, $model, $label, "team", "on-blurr", event, item.name);
            expect(vm.oSiteUsers).toBeDefined();
            expect(vm.typehead).toBe(false);
            expect(vm.notificationPopUpBlock).toBe(false);
            expect($label.assignedUser).toBe(oEnvironmentConfiguration.loggedInUserName + "(" + oEnvironmentConfiguration.loggedInUserEmail + ");");
        });

        it("It should return the assigned user", function () {
            vm.onSelect($item, $model, $label, "team", "on-blurr", event, item.name);
            expect(vm.user).toBe(oEnvironmentConfiguration.loggedInUserName);
        });
    });

    //// **************** New Angular version is introduced in latest build ****************

    ////describe("Verification of saveDocumentTemplates function", function () {
    ////    it("It should return the saved document Templates", function () {
    ////        vm.primaryMatterType = true;
    ////        vm.activeDocumentTypeLawTerm = documentTemplateTypeLawTerm;
    ////        vm.documentTypeLawTerms = subareaTerms;
    ////        vm.saveDocumentTemplates();
    ////        expect(vm.selectedDocumentTypeLawTerms).toBe(vm.documentTypeLawTerms);
    ////        expect(vm.popupContainerBackground).toBe("hide");
    ////        expect(vm.popupContainer).toBe("hide");
    ////    });

    ////    it("It should not return saved document Templates and should prompt error popup", function () {
    ////        vm.primaryMatterType = false;
    ////        vm.saveDocumentTemplates();
    ////        expect(vm.errorPopUp).toBe(true);
    ////    });
    ////});

    describe("Verification of open1 function", function () {
        it("It should open the date picker", function () {
            vm.open1();
            expect(vm.opened).toBe(true);
        });
    });

    describe("Verification of conflictRadioChange function", function () {
        it("It should enable the secureMatterRadioEnabled option", function () {
            vm.conflictRadioChange(true);
            expect(vm.secureMatterRadioEnabled).toBe(true);
            expect(vm.secureMatterCheck).toBe(true);
        });

        it("It should disable the secureMatterRadioEnabled option", function () {
            vm.conflictRadioChange(false);
            expect(vm.secureMatterRadioEnabled).toBe(false);
        });
    });

    describe("Verification of addNewAssignPermissions function", function () {
        it("It should return the data for assignPermissionTeams", function () {
            vm.addNewAssignPermissions();
            expect(vm.assignPermissionTeams).not.toBeUndefined();
        });
    });

    describe("Verification of removeAssignPermissionsRow function", function () {
        it("It should return the remaining users", function () {

            //Math.Random() function is used to randomize the length
            vm.assignPermissionTeams.length = Math.floor(Math.random() * 5) + 2;
            var rows = vm.assignPermissionTeams.length;
            vm.removeAssignPermissionsRow(0);
            expect(rows).toBe(vm.assignPermissionTeams.length + 1);
        });

        it("It should not return any data", function () {
            vm.assignPermissionTeams.length = 0;
            vm.removeAssignPermissionsRow(0);
            expect(vm.assignPermissionTeams.length).toBe(0);
        });
    });

    describe("Verification of createAndNotify function", function () {
        it("It should display the Create and Notify button", function () {
            vm.createAndNotify(true);
            expect(vm.createButton).toBe("Create and Notify");
        });

        it("It should display the Create button", function () {
            vm.createAndNotify(false);
            expect(vm.createButton).toBe("Create");
        });
    });

    describe("Verification of CheckPopUp function", function () {
        it("It should display the Check PopUp", function () {
            vm.CheckPopUp(true);
            expect(vm.errorPopUpBlock).toBe(false);
            expect(vm.errorBorder).toBe("");
        });
    });

    describe("Verification of closesuccessbanner function", function () {
        it("It should close the successbanner", function () {
            vm.closesuccessbanner();
            expect(vm.successMsg).toBe("");
            expect(vm.successBanner).toBe(false);
        });
    });

    describe("Verification of createMatterButton function", function () {
        it("Successfully create matter button", function () {
            vm.validateCurrentPage = function (id) { return true; };
            var validateCurrentPage = function (id) { return true; };
            vm.chkConfilctCheck = true;
            vm.conflictDate = "8/1/2016";
            vm.createMatterButton(event);
            expect(vm.sectionName).toBe("snConflictCheck");
            expect(vm.iCurrentPage).toBe(2);
        });
    });

    describe("Verification of navigateToSecondSection function", function () {
        it("it should navigateToSecondSection", function () {
            vm.iCurrentPage = 5;
            vm.navigateToSecondSection("snOpenMatter");
            expect(vm.sectionName).toBe("snOpenMatter");
            expect(vm.iCurrentPage).toBe(1);
            expect(localStorage.getItem("iLivePage")).toBe("1");
            expect(vm.prevButtonDisabled).toBe(true);
            expect(vm.nextButtonDisabled).toBe(false);
        });
    });

    describe("Verification of PreviousClick function", function () {
        it("it should navigate to previous setion", function () {
            vm.iCurrentPage = 2;
            vm.PreviousClick(event)
            expect(vm.iCurrentPage).toBe(1);
            expect(localStorage.getItem("iLivePage")).toBe("1");
            expect(vm.prevButtonDisabled).toBe(true);
            expect(vm.nextButtonDisabled).toBe(false);
        });
    });

    describe("Verification of confirmUser function", function () {
        it("It should confirm the user", function () {
            vm.textInputUser = { "assignedUser": null, "userConfirmation": null };
            vm.confirmUser(true);
            expect(vm.textInputUser.userConfirmation).toBe(true);
        });
        it("It should not confirm the user", function () {
            vm.textInputUser = { "assignedUser": null, "userConfirmation": null };
            vm.confirmUser(false);
            expect(vm.textInputUser.userConfirmation).toBe(false);

        });
    });

    describe('Verification of getSelectedLevelOne function', function () {
        it('it should check that SelectedLevelOne item is null', function () {
            vm.selectedLevelOneItem = null;
            vm.taxonomyHierarchyLevels = 5;
            vm.getSelectedLevelOne();
            expect(vm.levelTwoList).toBeNull();
            expect(vm.levelThreeList).toBeNull();
            expect(vm.levelFourList).toBeNull();
            expect(vm.levelFiveList).toBeNull();
        });
    });

    describe('Verification of selectLevelTwoItem   function', function () {
        it('It should check the levelTwoItem', function () {
            var oLevel = { level3: ['level3'] };
            vm.taxonomyHierarchyLevels = 3;
            vm.selectLevelTwoItem(oLevel);
            expect(vm.activeLevelThreeItem).toBe(vm.levelThreeList[0]);
        });

        it('it should check that taxonomyhierarchylevels is greater than equal to 3', function () {
            var oLevel = { level3: [{ level4: 'level4' }] };
            vm.taxonomyHierarchyLevels = 4;
            vm.selectLevelTwoItem(oLevel);
            expect(vm.activeLevelFourItem).toBe(vm.levelFourList[0]);

        });
    });

    describe('Verification of selectLevelThreeItem   function', function () {
        it('It should check the levelThreeItem', function () {
            var oLevel = { level4: ['level4'] };
            vm.taxonomyHierarchyLevels = 4;
            vm.selectLevelThreeItem(oLevel);
            expect(vm.activeLevelFourItem).toBe(vm.levelFourList[0]);
        });

        it('It should check that taxonomyHierarchyLevels is greater than equal to 4', function () {
            var oLevel = { level4: [{ level5: 'level5' }] };
            vm.taxonomyHierarchyLevels = 5;
            vm.selectLevelThreeItem(oLevel);
            expect(vm.activeLevelFiveItem).toBe(vm.levelFiveList[0]);
        });
    });

    describe('Verification of selectLevelFourItem   function', function () {
        it('It should check the levelFourItem', function () {
            var oLevel = { level5: ['level5'] };
            vm.taxonomyHierarchyLevels = 5;
            vm.selectLevelFourItem(oLevel);
            expect(vm.activeLevelFiveItem).toBe(vm.levelFiveList[0]);
        });

    });
});
