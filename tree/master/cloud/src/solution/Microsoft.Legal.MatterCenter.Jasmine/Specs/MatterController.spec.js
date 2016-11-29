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
        vm = $controller("mattersController as vm", { $scope: $scope, $state: $state, $stateParams: $stateParams, matterResource: mockMatterResource, api: mockapi, $rootScope: rootData, $http: $http, $location: $location, $q: $q, $animate: $animate });
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
            vm.uploadMessages = { uploadImageDocumentIcon: "/_layouts/15/images/icsourceicon.png", uploadPNGIconExtensions: "/_layouts/15/images/icsourceicon.png" };
            oTestConfiguration = { sSourceIconURL: "https://MySharePoint.sharepoint.com/_layouts/15/images/icsourceicon.png" };
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
            expect(rootData.breadcrumb).toBe(true);
            expect(rootData.foldercontent).toBe(false);
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
        it("It should return overwrite configuration for the file for both", function () {
            vm.globalSettings = { overwriteDupliacteFileNameWithDateTimeFor: "BOTH" };
            var bAppendEnabled = vm.overwriteConfiguration("abc.msg");
            expect(bAppendEnabled).toBe(true);
        });

        it("It should return overwrite configuration for the file for documnets only", function () {
            vm.globalSettings = { overwriteDupliacteFileNameWithDateTimeFor: "DOCUMENT ONLY" };
            var bAppendEnabled = vm.overwriteConfiguration("abc.msg");
            expect(bAppendEnabled).toBe(false);
        });

        it("It should return overwrite configuration for the file for default", function () {
            vm.globalSettings = { overwriteDupliacteFileNameWithDateTimeFor: "DEFAULT" };
            var bAppendEnabled = vm.overwriteConfiguration("abc.msg");
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
            vm.uploadMessages = { content_Check_Abort: "Content check has been aborted." };
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

    describe("Verification of getTableHeight function", function () {
        it("It should return table height", function () {
            var height = vm.getTableHeight();
            var outHeight = parseInt(height.height.substring(0, 3));
            expect(outHeight).toBeGreaterThan(100);
        });
    });

    describe("Verification of switchFuction function", function () {
        it("It set switches display column for GridColumn1Header", function () {
            var out = vm.switchFuction("GridColumn1Header");
            expect(out[0]).toBe("Matter");
        });

        it("It set switches display column for GridColumn2Header", function () {
            var out = vm.switchFuction("GridColumn2Header");
            expect(out[0]).toBe("Client");
        });

        it("It set switches display column for GridColumn3Header", function () {
            var out = vm.switchFuction("GridColumn3Header");
            expect(out[0]).toBe("Client.MatterID");
        });

        it("It set switches display column for GridColumn4Header", function () {
            var out = vm.switchFuction("GridColumn4Header");
            expect(out[0]).toBe("Modified Date");
        });

        it("It set switches display column for GridColumn5Header", function () {
            var out = vm.switchFuction("GridColumn5Header");
            expect(out[0]).toBe("Responsible Attorney");
        });

        it("It set switches display column for GridColumn6Header", function () {
            var out = vm.switchFuction("GridColumn6Header");
            expect(out[0]).toBe("Sub Area of Law");
        });

        it("It set switches display column for GridColumn7Header", function () {
            var out = vm.switchFuction("GridColumn7Header");
            expect(out[0]).toBe("Open Date");
        });

        it("It set switches display column for GridColumn8Header", function () {
            var out = vm.switchFuction("GridColumn8Header");
            expect(out[0]).toBe("Area of Law");
        });
    });

    describe("Verification of filterSearch function", function () {
        it("It should perform search for matters", function () {
            vm.searchexp = "MCMatterName";
            vm.filterSearch("test");
            expect(vm.lazyloaderFilter).toBe(false);
            expect(vm.filternodata).toBe(false);
        });
    });

    describe("Verification of clearAllFilter function", function () {
        it("It should clear all filters of search", function () {
            vm.clearAllFilter();
            expect(vm.matterfilter).toBe(false);
            expect(vm.clientfilter).toBe(false);
            expect(vm.attorneyfilter).toBe(false);
            expect(vm.practiceGroupfilter).toBe(false);
            expect(vm.areafilter).toBe(false);
            expect(vm.areaoflawfilter).toBe(false);
            expect(vm.subareafilter).toBe(false);
            expect(vm.projectIDfilter).toBe(false);
            expect(vm.matterdateheader).toBe(true);
            expect(vm.matterheader).toBe(true);
            expect(vm.moddatefilter).toBe(false);
        });
    });

    describe("Verification of clearAllFiltersofSort function", function () {
        it("It should clear sorting vlaues", function () {
            vm.clearAllFiltersofSort();
            expect(vm.MatterNameSort).toBe(undefined);
            expect(vm.ClientSort).toBe(undefined);
            expect(vm.ClientIDSort).toBe(undefined);
            expect(vm.ModiFiedTimeSort).toBe(undefined);
            expect(vm.ResAttoSort).toBe(undefined);
            expect(vm.SubAreaSort).toBe(undefined);
            expect(vm.OpenDateSort).toBe(undefined);
        });
    });

    describe("Verification of SetPreviousFilterVlaues function", function () {
        it("It set sorting vlaues", function () {
            vm.previousMatterIdValue = 'test';
            vm.SetPreviousFilterVlaues();
            expect(vm.previousMatterIdValue).toBe('');
        });
    });

    describe("Verification of clearFilterValuesOnSorting function", function () {
        it("It should clear all filters of search on sorting", function () {
            vm.clearFilterValuesOnSorting();
            expect(vm.matterfilter).toBe(false);
            expect(vm.clientfilter).toBe(false);
            expect(vm.attorneyfilter).toBe(false);
            expect(vm.practiceGroupfilter).toBe(false);
            expect(vm.areafilter).toBe(false);
            expect(vm.areaoflawfilter).toBe(false);
            expect(vm.subareafilter).toBe(false);
            expect(vm.projectIDfilter).toBe(false);
            expect(vm.matterdateheader).toBe(true);
            expect(vm.matterheader).toBe(true);
            expect(vm.moddatefilter).toBe(false);
        });
    });

    describe("Verification of clearFiltersForSearch function", function () {
        it("It should clear all filters of search", function () {
            vm.clearFiltersForSearch();
            expect(vm.attorneyfilter).toBe(false);
            expect(vm.opendatefilter).toBe(false);
            expect(vm.areafilter).toBe(false);
            expect(vm.projectIDfilter).toBe(false);
            expect(vm.matterfilter).toBe(false);
            expect(vm.clientfilter).toBe(false);
            expect(vm.areaoflawfilter).toBe(false);
            expect(vm.practiceGroupfilter).toBe(false);
            expect(vm.moddatefilter).toBe(false);
            expect(vm.subareafilter).toBe(false);
            expect(vm.ProjectID).toBe(false);
        });
    });

    describe("Verification of SetMatters function", function () {
        it("It should set matter that is passed as input", function () {
            vm.SetMatters("test", "test");
            expect(vm.matterfilter).toBe(false);
            expect(vm.clientfilter).toBe(false);
            expect(vm.attorneyfilter).toBe(false);
            expect(vm.practiceGroupfilter).toBe(false);
            expect(vm.areafilter).toBe(false);
            expect(vm.areaoflawfilter).toBe(false);
            expect(vm.subareafilter).toBe(false);
            expect(vm.projectIDfilter).toBe(false);
            expect(vm.matterdateheader).toBe(true);
            expect(vm.matterheader).toBe(true);
            expect(vm.moddatefilter).toBe(false);
            expect(vm.MatterNameSort).toBe(undefined);
            expect(vm.ClientSort).toBe(undefined);
            expect(vm.ClientIDSort).toBe(undefined);
            expect(vm.ModiFiedTimeSort).toBe(undefined);
            expect(vm.ResAttoSort).toBe(undefined);
            expect(vm.SubAreaSort).toBe(undefined);
            expect(vm.OpenDateSort).toBe(undefined);
        });
    });

    describe("Verification of PinMatter function", function () {
        it("It set pin given matter", function () {
            vm.PinMatter(matterDataForPin);
            expect(vm.pinnedorunpinned).toBe(true);
            expect(vm.lazyloader).toBe(false);
            expect(vm.divuigrid).toBe(false);
        });
    });

    describe("Verification of modEndDate function", function () {
        it("It should return start date", function () {
            vm.modEndDate(event);
            expect(vm.modifiedenddate).not.toBe(null);
        });
    });
});


