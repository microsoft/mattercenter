/// <reference path="../References.js" />
///// <disable>JS2074, JS3058</disable>
//// ***********************************************************************
//// Author           : MAQ USER
//// Created          : 25-11-2016
////
//// ***********************************************************************
//// <copyright file="AdminController.spec.js" company="MAQSoftware">
////  Copyright (c) . All rights reserved.
//// </copyright>
//// <summary>Test suite for admin Controller</summary>
//// ***********************************************************************

describe("Admin Controller test suite", function () {
    "use strict";
    var mockapi = function (adminResource) {
        getData(adminResource, mockAdminResource);
    };

    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("adminResource", ["$resource", "auth", mockAdminResource]);
    }));

    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("api", ["matterResource", "documentResource", "documentDashBoardResource", "matterDashBoardResource", "homeResource", "adminResource", mockapi]);
    }));

    beforeEach(module("ui.router"));
    beforeEach(module("ui.bootstrap"));

    beforeEach(inject(function ($controller, $rootScope) {
        vm = $controller("AdminController as vm", { $scope: $scope, $state: $state, $stateParams: $stateParams, adminResource: mockAdminResource, api: mockapi, $rootScope: rootData, $location: $location });
    }));

    describe("Verification of updateLabels function", function () {
        it("It should update labels with error information", function () {
            vm.updateLabels(1);
            expect(vm.errorMsg).not.toBe(null);
            expect(vm.successMsg).toContain("Updated Successfully");
        });

        it("It should update labels with error or success information", function () {
            vm.updateLabels(2);
            expect(vm.errorMsg).not.toBe(null);
            expect(vm.successMsg).toContain("Updated Successfully");
        });
    });
});
