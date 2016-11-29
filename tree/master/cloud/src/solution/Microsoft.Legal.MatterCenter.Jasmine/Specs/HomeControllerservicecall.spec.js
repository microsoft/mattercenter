/// <reference path="../References.js" />
//// ***********************************************************************
//// Author           : MAQ USER
//// Created          : 31-08-2016
////
//// ***********************************************************************
//// <copyright file="HomeControllerservicecall.spec.js" company="MAQSoftware">
////  Copyright (c) . All rights reserved.
//// </copyright>
//// <summary>Test suite for Home Controller for service call</summary>
//// ***********************************************************************

describe("Home Controller test suite for service call", function () {

    var mockapi = function (homeResource) {
        getData(homeResource, mockHomeResource);
    };

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("homeResource", ['$resource', 'auth', mockHomeResource]);
    }));

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("api", ['matterResource', 'documentResource', 'documentDashBoardResource', 'matterDashBoardResource', 'homeResource', mockapi]);
    }));

    beforeEach(module('ui.router'));
    beforeEach(module('ui.bootstrap'));

    beforeEach(inject(function ($controller, $rootScope) {
        vm = $controller('homeController as vm', { $scope: $scope, $state: $state, $stateParams: $stateParams, homeResource: mockHomeResource, api: mockapi, $rootScope: rootData, $location: $location, adalAuthenticationService: adalService });
    }));

    describe("Verification of getUserProfilePicture function", function () {
        it("It should get User ProfilePicture", function () {
            vm.getUserProfilePicture();
            expect(vm.smallPictureUrl).toBeDefined();
            expect(vm.largePictureUrl).toBeDefined();
        });
    });

    describe("Verification of signOut function", function () {
        it("It should signOut", function () {
            vm.signOut();
            expect(vm.status).toBe(true);
        });
    });

    describe('Verification of showHamburgerIcon function', function () {
        it('It should show Hamburger Icon', function () {
            vm.showHamburgerIcon();
            expect(vm.showHamburger).toBe(true);
            expect(vm.showClose).toBe(false);
            expect(vm.showHeaderFlyout).toBe(false);
            expect(vm.showHeaderBackground).toBe(false);
        });
    });

    describe('Verification of showCloseIcon function', function () {
        it('It should show Close Icon', function () {
            vm.showCloseIcon();
            expect(vm.showHamburger).toBe(true);
            expect(vm.showClose).toBe(true);
            expect(vm.showHeaderFlyout).toBe(true);
            expect(vm.showHeaderBackground).toBe(true);
        });
    });

    describe('Verification of canCreateMatter function', function () {
        it('It should create the matter', function () {
            vm.canCreateMatter();
            expect(vm.canLoginUserCreateMatter).toBeDefined();
        });
    });
});


