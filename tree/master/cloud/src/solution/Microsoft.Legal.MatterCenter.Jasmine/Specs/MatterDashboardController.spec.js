/// <reference path="../References.js" />
/// <disable>JS2074, JS3058</disable>
//// ***********************************************************************
//// Author           : MAQ USER
//// Created          : 31-08-2016
////
//// ***********************************************************************
//// <copyright file="MatterDashboardController.spec.js" company="MAQSoftware">
////  Copyright (c) . All rights reserved.
//// </copyright>
//// <summary>Test suite for Matter Dashboard Controller</summary>
//// ***********************************************************************

describe("MatterDashBoard Controller test suite", function () {
    "use strict";


    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("matterDashBoardResource", ["$resource", "auth", mockMatterDashBoardResource]);
    }));

    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("api", ["matterResource", "documentResource", "documentDashBoardResource", "matterDashBoardResource", "homeResource", mockapi]);
    }));

    beforeEach(module("ui.router"));
    beforeEach(module("ui.bootstrap"));
    
    beforeEach(inject(function ($controller, $rootScope) {
        rootScope = $rootScope.$new();
        vm = $controller("MatterDashBoardController as vm", { $scope: $scope, $state: $state, $stateParams: $stateParams, matterDashBoardResource: mockMatterDashBoardResource, api: mockapi, $rootScope: rootScope, $http: $http, $location: $location, $q: $q });
    }));

    describe("Verification of closealldrops function", function () {
        it("It should close all dropdowns on click of the page", function () {
            vm.closealldrops();
            expect(vm.searchdrop).toBe(false);
            expect(vm.downwarddrop).toBe(true);
            expect(vm.upwarddrop).toBe(false);
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
            expect(vm.pgdrop).toBe(false);
            expect(vm.pgdropvisible).toBe(false);
            expect(vm.aoldrop).toBe(false);
            expect(vm.aoldropvisible).toBe(false);
            expect(vm.sortbydrop).toBe(false);
            expect(vm.sortbydropvisible).toBe(false);
        });
    });

    describe("Verification of hideinnerdrop function", function () {
        it("It should hide all inner dropdownns", function () {
            vm.hideinnerdrop(event);
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
            expect(vm.pgdrop).toBe(false);
            expect(vm.pgdropvisible).toBe(false);
            expect(vm.aoldrop).toBe(false);
            expect(vm.aoldropvisible).toBe(false);
        });
    });

    describe("Verification of filterSearchOK function", function () {
        it("It should return selected client", function () {
            vm.selectedClients = "";
            vm.clients = clientobj;
            vm.filterSearchOK("client");
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
            expect(vm.selectedClientsForCancel).toBe(oTestConfiguration.sSelectedClient);
            expect(vm.selectedClients).toBe(oTestConfiguration.sSelectedClient);
        });

        it("It should return selected practice group", function () {
            vm.selectedPGs = "";
            vm.practiceGroups = practicegroup;
            vm.filterSearchOK("level1");
            expect(vm.pgdropvisible).toBe(false);
            expect(vm.pgdrop).toBe(false);
            expect(vm.selectedPGsForCancel).toBe(oTestConfiguration.sSelectedPracticeGroup);
            expect(vm.selectedPGs).toBe(oTestConfiguration.sSelectedPracticeGroup);
        });

        it("It should return selected AOL", function () {
            vm.selectedAOLs = "";
            vm.aolTerms = practicegroup;
            vm.filterSearchOK("level2");
            expect(vm.aoldropvisible).toBe(false);
            expect(vm.aoldrop).toBe(false);
            expect(vm.selectedAOLsForCancel).toBe(oTestConfiguration.sSelectedAreaOfLaw);
            expect(vm.selectedAOLs).toBe(oTestConfiguration.sSelectedAreaOfLaw);
        });
    });

    describe("Verification of filterSearchCancel function", function () {
        it("It should trigger when the user clicks on 'Cancel' button in the filter panel", function () {
            vm.filterSearchCancel("client");
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
            expect(vm.pgdrop).toBe(false);
            expect(vm.pgdropvisible).toBe(false);
            expect(vm.aoldropvisible).toBe(false);
            expect(vm.aoldrop).toBe(false);
        });

    });

    describe("Verification of showupward function", function () {
        it("It should show upward dropdown box", function () {
            vm.showupward(event);
            expect(vm.searchdrop).toBe(true);
            expect(vm.downwarddrop).toBe(false);
            expect(vm.upwarddrop).toBe(true);
        });
    });

    describe("Verification of showdownward function", function () {
        it("It should show downward dropdown box", function () {
            vm.showdownward(event);
            expect(vm.searchdrop).toBe(false);
            expect(vm.upwarddrop).toBe(false);
            expect(vm.downwarddrop).toBe(true);
        });
    });

    describe("Verification of showsortby function", function () {
        it("It should show sortby box", function () {
            vm.sortbydropvisible = false;
            vm.showsortby(event);
            expect(vm.sortbydrop).toBe(true);
            expect(vm.sortbydropvisible).toBe(true);
        });
        it("It should hide sortby box", function () {
            vm.sortbydropvisible = true;
            vm.showsortby(event);
            expect(vm.sortbydrop).toBe(false);
            expect(vm.sortbydropvisible).toBe(false);
        });
    });

    describe("Verification of pagination function", function () {
        it("It should disable pagination on the page", function () {
            vm.totalrecords = 0;
            vm.pagination();
            expect(vm.fromtopage).toBe("1 - 0");
            expect(vm.displaypagination).toBe(false);
        });
        it("It should enable pagination on the page", function () {
            vm.totalrecords = 16;
            vm.pagination();
            expect(vm.fromtopage).toBe("1 - 16");
            expect(vm.displaypagination).toBe(true);

        });
    });

    describe("Verification of overwriteConfiguration function", function () {
        it("It should return overwrite configuration for the file", function () {
            var bAppendEnabled = vm.overwriteConfiguration("TestEmail.msg");
            expect(bAppendEnabled).toBe(true);
        });
    });

    describe("Verification of contentCheckNotification function", function () {
        it("It should set upload file configurations to perform content check", function () {
            vm.file = {};
            vm.contentCheckNotification(vm.file, true);
            expect(vm.file.contentCheck).toBe("contentCheck");
            expect(vm.file.saveLatestVersion).toBe("False");
            expect(vm.file.cancel).toBe("False");
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
        it("It should close success banner", function () {
            vm.closeSuccessBanner();
            expect(vm.oUploadGlobal.successBanner).toBe(false);
        });
    });

    describe("Verification of showDocTabs function", function () {
        it("It should display document tabs", function () {
            vm.showInnerNav = true;
            vm.showDocTabs(event);
            expect(vm.showNavTab).toBe(true);
            expect(vm.showInnerNav).toBe(false);
        });
    });

});
