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
        it("It should remove the assigned permission", function () {
            vm.assignPermissionTeams = "MatterCenter";
            vm.assignPermissionTeams = {
                "splice": function (index, data) { vm.assignPermissionTeams = true; }
            }
            vm.removeAssignPermissionsRow(3);
            expect(vm.assignPermissionTeams).toBeDefined();
        });
    });

    describe("Verification of addNewAssignPermissions function", function () {
        it("It should add the assigned permission", function () {
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
            expect(vm.lazyloader).toBe(false);
            expect(vm.clientlist).toBe(true);
            expect(vm.showClientDetails).toBe(false);
            expect(vm.successmessage).toBe(false);
        });
    });
    describe("Verification of getBoolValues function", function () {
        it("It should get all the boolean values", function () {
        
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
            vm.showSelectedClient("Test", oEnvironmentConfiguration.tenantUrl);
            expect(vm.lazyloader).toBe(true);
            expect(vm.selected).toBe("Test");
            expect(vm.clienturl).toBe(oEnvironmentConfiguration.tenantUrl);
            expect(vm.nodata).toBe(false);
            expect(vm.lazyloader).toBe(true);
            expect(vm.clientlist).toBe(false);
            expect(vm.showClientDetails).toBe(true);
        });
    });

});
