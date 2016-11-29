/// <reference path="../References.js" />
/// <disable>JS2074, JS3058</disable>
//// ***********************************************************************
//// Author           : MAQ USER
//// Created          : 31-08-2016
////
//// ***********************************************************************
//// <copyright file="CreateMatterControllerservicecall.spec.js" company="MAQSoftware">
////  Copyright (c) . All rights reserved.
//// </copyright>
//// <summary>Test suite for createMatter controller for service call</summary>
//// ***********************************************************************

describe("CreateMatter Controller test suite for service call", function () {
    "use strict";

    var mockapi = function (matterResource, callback) {
        getData(matterResource, mockMatterResourceService);
    };

    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("matterResource", ["$resource", "auth", mockMatterResourceService]);
    }));

    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("api", ["matterResource", "documentResource", "documentDashBoardResource", "matterDashBoardResource", "homeResource", mockapi]);
    }));

    beforeEach(module("ui.router"));
    beforeEach(module("ui.bootstrap"));

    beforeEach(inject(function ($controller, $injector, $rootScope) {
        vm = $controller("createMatterController as vm", { $scope: $scope, $rootScope: rootData, $state: $state, $stateParams: $stateParams, matterResource: mockMatterResourceService, api: mockapi, adalAuthenticationService: mockadalAuthenticationService });
    }));

    describe("Verification of getSelectedClientValue function", function () {
        it("It should successfully get data from matter configuration list", function () {
            vm.getSelectedClientValue(oTestConfiguration.oClientObj);
            expect(vm.clientId).toBe(16761);
            expect(vm.selectedClientName).toBe("Microsoft");
            expect(vm.clientUrl).toBe("https://MySharePoint.sharepoint.com/sites/microsoft");
            expect(vm.canCreateMatterPermission).toBe(false);
            expect(vm.errPermissionMessage).toBeDefined();
            expect(vm.errTextMsg).toBeDefined();
            expect(vm.errorBorder).toBe("client");
            expect(vm.errorPopUpBlock).toBe(true);
            expect(vm.popupContainerBackground).toBe("hide");
        });
    });

    describe("Verification of checkValidMatterName function", function () {
        it("It Should return true for new matter", function () {
            vm.matterName = oTestConfiguration.sValidMatterName;
            vm.clientUrl = oTestConfiguration.sValidMatterClientURL;
            vm.checkValidMatterName();
            expect(vm.errorPopUpBlock).not.toBe(null);
        });
    });
});
