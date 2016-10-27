/// <reference path="../References.js" />
/// <disable>JS2074, JS3058</disable>
//// ***********************************************************************
//// Author           : MAQ USER
//// Created          : 31-08-2016
////
//// ***********************************************************************
//// <copyright file="SettingsController.spec.js" company="MAQSoftware">
////  Copyright (c) . All rights reserved.
//// </copyright>
//// <summary>Test suite for Settings controller</summary>
//// ***********************************************************************

describe("SettingsController test suite", function () {
    var mockapi = function (settingsResource) {
        getData(settingsResource, mockSettingsResource);

    };

    var mockapi = function (settingsResource, callback) {
        getData(settingsResource, mockSettingsResource);
    };

    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("settingsResource", ["$resource", "auth", mockSettingsResource]);
    }));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("api", ["matterResource", "documentResource", "documentDashBoardResource", "matterDashBoardResource", "homeResource", "settingsResource", mockapi]);
    }));

    beforeEach(module("ui.router"));
    beforeEach(module("ui.bootstrap"));

    beforeEach(inject(function ($controller, $rootScope) {
        rootScope = $rootScope.$new();
        vm = $controller("SettingsController as vm", { $scope: $scope, $state: $state, $stateParams: $stateParams, settingsResource: mockSettingsResource, api: mockapi, $rootScope: rootScope, $http: $http, $location: $location, $q: $q });
    }));

    describe("Verification of canCreateMatter function", function () {
        it("It should create matter", function () {
            vm.getTaxonomyData();
            expect(vm.taxonomydata).toBeDefined();
            expect(vm.clientlist).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.lazyloader).toBe(true);
        });
    });

    describe("Verification of getRolesData function", function () {
        it("It should get data related to all roles", function () {
            vm.getTaxonomyData();
            expect(vm.assignRoles).toBeDefined();
            expect(vm.clientlist).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.lazyloader).toBe(true);
        });
    });

    describe("Verification of getPermissionsData function", function () {
        it("It should get data related to all permissions", function () {
            vm.getPermissionsData();
            expect(vm.assignPermissions).toBeDefined();
            expect(vm.clientlist).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.lazyloader).toBe(true);
        });
    });

    describe("Verification of removeAssignPermissionsRow function", function () {
        it("It should remove the assign permission", function () {
            vm.assignPermissionTeams = "MatterCenter";
            vm.assignPermissionTeams = {
                "splice": function (index, data) { vm.assignPermissionTeams = true; }
            }
            vm.removeAssignPermissionsRow(3);
            expect(vm.assignPermissionTeams).toBeDefined();
        });
    });

    describe("Verification of addNewAssignPermissions function", function () {
        it("It should add the assign permission", function () {
            vm.assignPermissionTeams = "MatterCenter";
            vm.assignPermissionTeams = {
                "push": function (data) { vm.assignPermissionTeams = data; }
            }
            vm.assignRoles = ["test"], ["test"];
            vm.assignPermissions = ["test"], ["test"];
            vm.addNewAssignPermissions();
            expect(vm.assignPermissionTeams).toBeDefined();
        });
    });

    describe("Verification of onSelect function", function () {
        it("It should return the conflicted user", function () {
            var $item = {
                email: "",
                name: "No results found"
            };
            var $label = { "assignedUser": oEnvironmentConfiguration.loggedInUserEmail };
            vm.onSelect($item, $model, $label, "conflictcheckuser", "on-blurr", event);
            expect(vm.notificationPopUpBlock).toBeUndefined();
        });

        it("It should return the blocked user", function () {
            vm.oSiteUsers = {
                "indexOf": function ($itemdata) { return 1; }
            };
            vm.onSelect(item, $model, $label, "team", "on-blurr", event, item.name);
            expect(vm.notificationPopUpBlock).toBeUndefined()
        });
    });

    describe("Verification of saveSettings function", function () {
        it("It should save the final settings", function () {
            vm.saveSettings();
            vm.popupContainerBackground = "hide";
            expect(vm.lazyloader).toBe(true);
            expect(vm.clientlist).toBe(false);
            expect(vm.showClientDetails).toBe(true);
            expect(vm.successmessage).toBe(true);
        });
    });
    describe("Verification of getBoolValues function", function () {
        it("It should get all the bool values", function () {

            var data = vm.getBoolValues("Yes");
            expect(data).toBe(true);
        });
    });

    describe("Verification of showSettings function", function () {
        it("It should show all the settings", function () {
            vm.showSetting();
            expect(vm.clientlist).toBe(true);
            expect(vm.showClientDetails).toBe(false);
            expect(vm.successmessage).toBe(false);
        });
    });

    describe("Verification of setClientData function", function () {
        it("It should set all the client data", function () {
            vm.setClientData(data);
            expect(vm.assignteam).toBe("Yes");
            expect(vm.calendar).toBe("Yes");
            expect(vm.rss).toBe("Yes");
            expect(vm.email).toBe("Yes");
            expect(vm.tasks).toBe("Yes");
            expect(vm.matterdesc).toBe("Yes");
            expect(vm.conflict).toBe("Yes");
            expect(vm.showmatterconfiguration).toBe("DateTime");
        });

        it("It should not set the client data", function () {
            vm.setClientData(dataChunk);
            expect(vm.assignteam).toBe("No");
            expect(vm.calendar).toBe("No");
            expect(vm.rss).toBe("No");
            expect(vm.email).toBe("No");
            expect(vm.tasks).toBe("No");
            expect(vm.matterdesc).toBe("No");
            expect(vm.conflict).toBe("No");
            expect(vm.showmatterconfiguration).toBe("DateTime");
        });
    });

    describe("Verification of showSelectedClient function", function () {
        it("It should show all the selected clients", function () {
            vm.showSelectedClient("Test", oTestConfiguration.sValidMatterClientURL);
            expect(vm.lazyloader).toBe(true);
            expect(vm.selected).toBe("Test");
            expect(vm.clienturl).toBe(oTestConfiguration.sValidMatterClientURL);
            expect(vm.nodata).toBe(false);
            expect(vm.lazyloader).toBe(true);
            expect(vm.clientlist).toBe(false);
            expect(vm.showClientDetails).toBe(true);
        });
    });

    describe("Verification of showMatterId function", function () {
        it("It should show matter Id", function () {
            vm.showMatterId();
            expect(vm.showmatterconfiguration).toBe("Guid");
        });
    });

    describe("Verification of selectMatterTypePopUpClose function", function () {
        it("It should hide popup", function () {
            vm.popupContainer = "Show";
            vm.selectMatterTypePopUpClose();
            expect(vm.popupContainerBackground).toBe("hide");
            expect(vm.popupContainer).toBe("hide");

        });
    });

    describe("Verification of selectMatterType function", function () {
        it("It should show Matter type popup", function () {
            vm.selectMatterType();
            expect(vm.popupContainerBackground).toBe("Show");
            expect(vm.popupContainer).toBe("Show");

        });
    });

    //describe("Verification of removeFromDocumentTemplate function", function () {
    //    it("It should remove document from document template", function () {
    //        vm.removeDTItem = true;
    //        debugger;
    //        vm.removeFromDocumentTemplate();
    //        expect(vm.popupContainer).toBe("Show");

    //    });
    //});

    describe("Verification of selectDocumentTemplateTypeLawTerm function", function () {
        it("It should select document template type for law term", function () {
            vm.selectDocumentTemplateTypeLawTerm("AreaOfLaw");
            expect(vm.errorPopUp).toBe(false);
            expect(vm.removeDTItem).toBe(true);
            expect(vm.primaryMatterType).toBe(true);
            expect(vm.activeDocumentTypeLawTerm).toBe("AreaOfLaw");

        });
    });

    describe("Verification of saveDocumentTemplates function", function () {
        it("It should save document templates", function () {
            vm.primaryMatterType = true;
            vm.documentTypeLawTerms = [{ id: "test" }];
            vm.activeDocumentTypeLawTerm = { id: "test" };
            vm.saveDocumentTemplates();
            expect(vm.errorPopUp).toBe(false);
            expect(vm.popupContainerBackground).toBe("hide");
            expect(vm.popupContainer).toBe("hide");


        });

        it("It should not save document templates", function () {
            vm.saveDocumentTemplates();
            expect(vm.errorPopUp).toBe(true);

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
