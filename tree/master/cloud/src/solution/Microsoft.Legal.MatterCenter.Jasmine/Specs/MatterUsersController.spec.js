/// <reference path="../References.js" />
///// <disable>JS2074, JS3058</disable>
//// ***********************************************************************
//// Author           : MAQ USER
//// Created          : 31-08-2016
////
//// ***********************************************************************
//// <copyright file="MatterUsersController.spec.js" company="MAQSoftware">
////  Copyright (c) . All rights reserved.
//// </copyright>
//// <summary>Test suite for Matter Users Controller</summary>
//// ***********************************************************************

describe("MatterUsers Controller test suite", function () {
    "use strict";
    var mockapi = function (matterResource) {
        getData(matterResource, mockMatterResource);
    };

    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("matterResource", ["$resource", "auth", mockMatterResource]);
    }));

    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("api", ["matterResource", "documentResource", "documentDashBoardResource", "matterDashBoardResource", "homeResource", mockapi]);
    }));

    beforeEach(module("ui.router"));
    beforeEach(module("ui.bootstrap"));

    beforeEach(inject(function ($controller, $rootScope) {
        rootScope = $rootScope.$new();
        vm = $controller("MatterUsersController as vm", { $scope: $scope, $state: $state, $stateParams: $stateParams, matterResource: mockMatterResource, api: mockapi, $rootScope: rootScope, $location: $location });
    }));

    describe("Verification of CheckPopUp function", function () {
        it("It should check error pop up", function () {
            vm.errorStatus = false;
            vm.CheckPopUp("test");
            expect(vm.errorPopUpBlock).toBe(false);
            expect(vm.errorBorder).toBe("");
            expect(vm.errorStatus).toBe(false);
        });

        it("It should not check the error pop up", function () {
            vm.errorStatus = true;
            vm.CheckPopUp("test");
            expect(vm.errorStatus).toBe(false);
        });
    });

    describe("Verification of removeAssignPermissionsRow function", function () {
        it("It should remove the assign permission", function () {
            vm.assignPermissionTeams = [];
            var team = {};
            team.assigneTeamRowNumber = 1;
            team.assignedAllUserNamesAndEmails = "";
            team.assignedRole = {};
            team.assignedPermission = {};
            team.assignedRole.name = "Responsible Attorney";
            team.assignedRole.mandatory = true;
            team.assignedPermission.name = "Full Control";
            team.assignedPermission.mandatory = true;
            team.assignedUser = "MatterCenter1";
            team.teamUsers = [];
            team.status = "success";
            team.teamUsers.push("MatterCenter1");
            vm.assignPermissionTeams.push(team);
            team = {};
            team.assigneTeamRowNumber = 2;
            team.assignedAllUserNamesAndEmails = "";
            team.assignedRole = {};
            team.assignedPermission = {};
            team.assignedRole.name = "Responsible Attorney";
            team.assignedRole.mandatory = true;
            team.assignedPermission.name = "Full Control";
            team.assignedPermission.mandatory = true;
            team.teamUsers = [];
            team.assignedUser = "MatterCenter2";
            team.teamUsers.push("MatterCenter2");
            team.status = "success";
            vm.assignPermissionTeams.push(team);
            var userDetails = {};
            userDetails.userEmail = "MatterCenter1";
            userDetails.userName = "MatterCenter1";
            userDetails.userRole = "Responsible Attorney";
            userDetails.userPermission = "Full Control";
            vm.stampedMatterUsers = [];
            vm.stampedMatterUsers.push(userDetails);
            vm.removeAssignPermissionsRow(1);
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

    describe("Verification of confirmUser function", function () {
        it("It should confirm the user", function () {
            vm.textInputUser = { "userConfirmation": false };
            vm.confirmUser(true);
            expect(vm.notificationPopUpBlock).toBe(false);
            expect(vm.notificationBorder).toBe("");
        });

        it("It should not confirm the user", function () {
            vm.textInputUser = { "assignedUser": null };
            vm.confirmUser(false);
            expect(vm.notificationPopUpBlock).toBe(false);
            expect(vm.notificationBorder).toBe("");
            expect(vm.textInputUser.assignedUser).toBe("");
        });

    });

    describe("Verification of checkUserExists function", function () {
        it("It should check if UserExists", function () {
            vm.checkUserExists(oEnvironmentConfiguration.loggedInUserEmail, event);
            expect(vm.notificationPopUpBlock).toBe(false);
        });

        it("It should not check if UserExists", function () {
            vm.assignPermissionTeams = [{ "team": { "assignedUser": oEnvironmentConfiguration.loggedInUserEmail } }];
            vm.checkUserExists("maquser@test.onmicrosoft.com", event);
            expect(vm.notificationPopUpBlock).toBe(false);
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
            expect(vm.notificationPopUpBlock).toBe(false);
        });

        it("It should return the blocked user", function () {
            vm.oSiteUsers = {
                "indexOf": function ($itemdata) { return 1; }
            };
            vm.onSelect(item, $model, $label, "team", "on-blurr", event, item.name);
            expect(vm.typehead).toBe(false);
            expect(vm.notificationPopUpBlock).toBe(false);
        });
    });

    describe("Verification of confirmUser function", function () {
        it("It should confirm the list of users", function () {
            vm.textInputUser = {};
            vm.confirmUser(true);
            expect(vm.notificationPopUpBlock).toBe(false);
            expect(vm.textInputUser.userConfirmation).toBe(true);
        });

        it("It should not confirm the list of users", function () {
            vm.textInputUser = {};
            vm.confirmUser(false);
            expect(vm.notificationPopUpBlock).toBe(false);
            expect(vm.textInputUser.userConfirmation).toBe(false);
            expect(vm.textInputUser.userExsists).toBe(false);
            expect(vm.textInputUser.assignedUser).toBe("");
        });
    });
});
