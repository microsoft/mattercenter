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
        it("It should be triggered when the user clicks on 'Cancel' button in the filter panel", function () {
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
            vm.globalSettings = { overwriteDupliacteFileNameWithDateTimeFor: "BOTH" };
            var bAppendEnabled = vm.overwriteConfiguration("TestEmail.msg");
            expect(bAppendEnabled).toBe(true);
        });

        it("It should return overwrite configuration for the file for documents only", function () {
            vm.globalSettings = { overwriteDupliacteFileNameWithDateTimeFor: "DOCUMENTS ONLY" };
            var bAppendEnabled = vm.overwriteConfiguration("TestEmail.msg");
            expect(bAppendEnabled).toBe(true);
        });

        it("It should return overwrite configuration for the file for default", function () {
            vm.globalSettings = { overwriteDupliacteFileNameWithDateTimeFor: "DEFAUTL" };
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
            vm.uploadMessages = { content_Check_Abort: "Content check has been aborted." };
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

    describe("Verification of hideBreadCrumb function", function () {
        it("It should hide breadCrumb", function () {
            vm.hideBreadCrumb();
            expect(rootScope.breadcrumb).toBe(true);
        });
    });

    describe("Verification of switchFuction function", function () {
        it("It set switches display column for GridColumn1Header", function () {
            var out = vm.switchFuction("GridColumn1Header");
            expect(out).toBe("Matter");
        });

        it("It set switches display column for GridColumn2Header", function () {
            var out = vm.switchFuction("GridColumn2Header");
            expect(out).toBe("Client");
        });

        it("It set switches display column for GridColumn3Header", function () {
            var out = vm.switchFuction("GridColumn3Header");
            expect(out).toBe("Client.Matter ID");
        });

        it("It set switches display column for GridColumn4Header", function () {
            var out = vm.switchFuction("GridColumn4Header");
            expect(out).toBe("Modified Date");
        });

        it("It set switches display column for GridColumn5Header", function () {
            var out = vm.switchFuction("GridColumn5Header");
            expect(out).toBe("Responsible attorney");
        });

        it("It set switches display column for GridColumn6Header", function () {
            var out = vm.switchFuction("GridColumn6Header");
            expect(out).toBe("Version");
        });

        it("It set switches display column for GridColumn7Header", function () {
            var out = vm.switchFuction("GridColumn7Header");
            expect(out).toBe("Created Date");
        });

        it("It set switches display column for GridColumn8Header", function () {
            vm.documentDashboardConfigs = { GridColumn8Header: "Practice Group" };
            var out = vm.switchFuction("GridColumn8Header");
            expect(out).toBe("Practice Group");
        });

        it("It set switches display column for default", function () {
            var out = vm.switchFuction("test");
            expect(out).toBe("");
        });
    });

    describe("Verification of showSubAreaofLawDrop function", function () {
        it("It should show sub area of law", function () {
            vm.showSubAreaofLawDrop(event);
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
            expect(vm.pgdrop).toBe(false);
            expect(vm.pgdropvisible).toBe(false);
            expect(vm.aoldrop).toBe(false);
            expect(vm.aoldropvisible).toBe(false);
            expect(vm.lazyloadersubaol).toBe(false);
        });
    });

    describe("Verification of sortExpression function", function () {
        it("It should sort data with respect to inputs", function () {
            vm.sortExpression("MatterName", "MatterName", 0);
            expect(vm.lazyloaderdashboard).toBe(false);
            expect(vm.divuigrid).toBe(false);
            expect(vm.displaypagination).toBe(false);
            expect(vm.nodata).toBe(false);
        });
    });

    describe("Verification of sortby function", function () {
        it("It should call sortExpression with proper inputs", function () {
            vm.sortby("AlphabeticalUp", "data");
            expect(vm.lazyloaderdashboard).toBe(false);
            expect(vm.divuigrid).toBe(false);
            expect(vm.displaypagination).toBe(false);
            expect(vm.nodata).toBe(false);
        });
    });

    describe("Verification of showDocTabs function", function () {
        it("It should show hidden tab", function () {
            vm.showInnerNav = true;
            vm.showDocTabs(event);
            expect(vm.showNavTab).toBe(true);
            expect(vm.showInnerNav).toBe(false);
        });
    });

    describe("Verification of showSelectedTabs function", function () {
        it("It should show selected tab", function () {
            vm.showSelectedTabs("Pinned matters", 5);
            expect(vm.selectedTab).toBe("Pinned matters");
            expect(vm.selectedTabInfo).toBe("Pinned matters (5)");
        });
    });

    describe("Verification of openStartDate function", function () {
        it("It should return start date", function () {
            vm.openStartDate(event);
            expect(vm.openedStartDate).toBe(true);
        });
    });

    describe("Verification of openEndDate function", function () {
        it("It should return start date", function () {
            vm.openEndDate(event);
            expect(vm.openedEndDate).toBe(true);
        });
    });
});
