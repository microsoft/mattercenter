/// <reference path="../References.js" />
/// <disable>JS2074, JS3058</disable>
//// ***********************************************************************
//// Author           : MAQ USER
//// Created          : 31-08-2016
////
//// ***********************************************************************
//// <copyright file="MatterController.spec.js" company="MAQSoftware">
////  Copyright (c) . All rights reserved.
//// </copyright>
//// <summary>Test suite for Matters Controller</summary>
//// ***********************************************************************

describe("Matters Controller test suite", function () {
    "use strict";
   
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
        vm = $controller("mattersController as vm", { $scope: $scope, $state: $state, $stateParams: $stateParams, matterResource: mockMatterResource, api: mockapi, $rootScope: rootScope, $http: $http, $location: $location, $q: $q, $animate: $animate });
    }));


    describe("Verification of closeNotificationDialog function", function () {
        it("It should close the notification dialog box", function () {
            vm.closeNotificationDialog();
            expect(vm.IsDupliacteDocument).toBe(false);
            expect(vm.IsNonIdenticalContent).toBe(false);
            expect(vm.showLoading).toBe(false);
        });
    });

    describe("Verification of attachmentTokenCallbackEmailClient function", function () {
        it("It should attach the token along with EmailClient-Need to call createMailPopup", function () {
            vm.createMailPopup = function () { return 1; };
            vm.attachmentTokenCallbackEmailClient(asyncResult, obj);
            expect(vm.attachmentToken).toBe("testtoken");
            expect(vm.mailUpLoadSuccess).toBe(false);
            expect(vm.mailUploadedFolder).toBe(null);
            expect(vm.loadingAttachments).toBe(false);
            expect(vm.mailUploadedFile).toBe(null);
        });
    });

    describe("Verification of getIconSource function", function () {
        it("It should return the source of icon", function () {
            var source = vm.getIconSource("sourceicon");
            expect(source).toBe(oTestConfiguration.sSourceIconURL);
        });
    });

    describe("Verification of closealldrops function", function () {
        it("It should close all the dropdowns", function () {
            vm.closealldrops();
            expect(vm.mattersdrop).toBe(false);
            expect(vm.mattersdropinner).toBe(true);
            expect(vm.matterheader).toBe(true);
            expect(vm.matterdateheader).toBe(true);
        });
    });

    describe("Verification of checkEmptyorWhitespace function", function () {
        it("It should check empty or whitespace in the input", function () {
            var data = vm.checkEmptyorWhitespace("test");
            expect(data).toBe("test");
        });
    });

    describe("Verification of hideBreadCrumb function", function () {
        it("It should hide the bread crumb", function () {
            vm.hideBreadCrumb();
            expect(rootScope.breadcrumb).toBe(true);
            expect(rootScope.foldercontent).toBe(false);
        });
    });

    describe("Verification of disabled function", function () {
        it("It should set the status based on current date.", function () {
            var oDate = new Date();
            var status = vm.disabled(oDate, "day");
            expect(status).toBe(true);
        });
    });

    describe("Verification of showSortExp function", function () {
        it("It should sort the elements in ascending order", function () {
            vm.sortexp = "test";
            vm.sortby = "asc";
            $scope.$apply = function () { };
            vm.showSortExp();
            expect(angular.element()).toBeDefined();
        });
    });

    describe("Verification of showmatterdrop function", function () {
        it("It should show matter dropdown if matter is pinned", function () {
            vm.mattersdropinner = true;
            vm.showmatterdrop(event);
            expect(vm.mattersdrop).toBe(true);
            expect(vm.mattersdropinner).toBe(false);
        });

        it("It should hide matter dropdown if matter is not pinned", function () {
            vm.mattersdropinner = false;
            vm.showmatterdrop(event);
            expect(vm.mattersdrop).toBe(false);
            expect(vm.mattersdropinner).toBe(true);
        });
    });

    describe("Verification of overwriteConfiguration function", function () {
        it("It should return overwrite configuration for the file", function () {
            var bAppendEnabled = vm.overwriteConfiguration(oTestConfiguration.sOverwriteConfigFileName);
            expect(bAppendEnabled).toBe(true);
        });
    });

    describe("Verification of contentCheckNotification function", function () {
        it("It should set upload file configurations to perform content check", function () {
            var file = { contentCheck: "", saveLatestVersion: "", cancel: "", append: true };
            vm.contentCheckNotification(file, true);
            expect(file.contentCheck).toBe("contentCheck");
            expect(file.saveLatestVersion).toBe("False");
            expect(file.cancel).toBe("False");
            expect(file.append).toBe(false);
        });
    });

    describe("Verification of abortContentCheck function", function () {
        it("It should set upload file configuration to abort content check", function () {
            vm.file = {};
            vm.file.value = "This";
            vm.abortContentCheck(vm.file, false);
            expect(vm.file.contentCheck).toBe(null);
            expect(vm.file.saveLatestVersion).toBe("True");
            expect(vm.file.value).toBe("This<br/><div>Content check has been aborted.</div>");
            expect(vm.file.cancel).toBe("True");
        });
    });

    describe("Verification of closeSuccessBanner function", function () {
        it("It should close the success banner", function () {
            vm.closeSuccessBanner();
            expect(vm.oUploadGlobal.successBanner).toBe(false);
        });
    });
    
});


