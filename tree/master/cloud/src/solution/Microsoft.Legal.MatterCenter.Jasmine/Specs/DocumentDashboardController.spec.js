/// <reference path="../References.js" />
/// <disable>JS2074, JS3058</disable>
//// ***********************************************************************
//// Author           : MAQ USER
//// Created          : 31-08-2016
////
//// ***********************************************************************
//// <copyright file="DocumentDashboardController.spec.js" company="MAQSoftware">
////  Copyright (c) . All rights reserved.
//// </copyright>
//// <summary>Test suite for documentDashboard</summary>
//// ***********************************************************************

describe("DocumentDashboard Controller test suite", function () {
    "use strict";

    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("documentDashBoardResource", ["$resource", "auth", mockDocumentDashBoardResource]);
    }));

    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("api", ["matterResource", "documentResource", "documentDashBoardResource", "matterDashBoardResource", "homeResource", mockapi]);
    }));

    beforeEach(module("ui.router"));
    beforeEach(module("ui.bootstrap"));

    beforeEach(inject(function ($controller, $injector, $rootScope) {
        rootScope = $rootScope.$new();
        vm = $controller("DocumentDashBoardController as vm", { $scope: $scope, $state: $state, $stateParams: $stateParams, documentDashBoardResource: mockDocumentDashBoardResource, api: mockapi, $rootScope: rootScope, $http: $http, $location: $location });
    }));

    describe("Verification of closealldrops function", function () {
        it("It should close all dropdowns on click on page", function () {
            vm.closealldrops(event);
            expect(vm.searchdrop).toBe(false);
            expect(vm.downwarddrop).toBe(true);
            expect(vm.upwarddrop).toBe(false);
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
            expect(vm.sortbydrop).toBe(false);
            expect(vm.sortbydropvisible).toBe(false);
        });
    });

    describe("Verification of hideinnerdashboarddrop function", function () {
        it("It should close and hide inner dropdowns of search box", function () {
            vm.hideinnerdashboarddrop(event);
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
        });
    });

    describe("Verification of toggleChecker function", function () {
        it("It should enable toggle to check all documents", function () {
            vm.documentsCheckedCount = 2;
            vm.gridApi = {};
            vm.gridApi.grid = 1;
            vm.gridApi = gridrows;
            vm.toggleChecker(true, obj);
            expect(vm.documentsCheckedCount).toBe(0);
            expect(vm.cartelements).toBeDefined();
        });

        it("It should disable toggle to check all documents", function () {
            vm.documentsCheckedCount = 2;
            vm.gridApi = {};
            vm.gridApi.grid = 1;
            vm.gridApi = gridrows;
            vm.toggleChecker(false, obj);
            expect(vm.documentsCheckedCount).toBe(0);
        });
    });

    describe("Verification of removeAttachment function", function () {
        it("It should add elements to the cart", function () {
            vm.cartelements = obj;
            vm.documentsCheckedCount = 3;
            vm.gridApi = {};
            vm.gridApi.grid = 1;
            vm.gridApi = gridrows;
            vm.removeAttachment(obj[0]);
            expect(vm.cartelements.length).toBe(1);
            expect(vm.documentsCheckedCount).toBe(2);
        });

        it("It should remove all the elements from the cart", function () {
            vm.cartelements = {};
            vm.cartelements.length = 0;
            vm.gridApi = {};
            vm.gridApi.grid = 1;
            vm.gridApi = gridrows;
            vm.removeAttachment(obj[0]);
            expect(vm.cartelements.length).toBe(0);
            expect(vm.documentsCheckedCount).not.toBeLessThan(0);
        });
    });

    describe("Verification of toggleCheckerAll function", function () {
        it("It should select all the document inside grid simultaneously", function () {
            vm.documentGridOptions.data = obj;
            vm.documentsCheckedCount = 3;
            vm.toggleCheckerAll(true);
            expect(vm.cartelements.length).toBeDefined();
            expect(vm.documentsCheckedCount).toBe(1);
        });

        it("It should deselect the documents inside grid ", function () {
            vm.documentGridOptions.data = obj;
            vm.documentsCheckedCount = 3;
            vm.toggleCheckerAll(false);
            expect(vm.cartelements.length).toBe(0);
            expect(vm.documentsCheckedCount).toBe(0);
        });
    });

    describe("Verification of showupward function", function () {
        it("It should display upward dropdown box", function () {
            vm.showupward(event);
            expect(vm.searchdrop).toBe(true);
            expect(vm.downwarddrop).toBe(false);
            expect(vm.upwarddrop).toBe(true);
        });
    });

    describe("Verification of showdownward function", function () {
        it("It should display downward dropdown box", function () {
            vm.showdownward(event);
            expect(vm.searchdrop).toBe(false);
            expect(vm.upwarddrop).toBe(false);
            expect(vm.downwarddrop).toBe(true);
        });
    });

    describe("Verification of showsortby function", function () {
        it("It should show sortby dropdown box", function () {
            vm.sortbydropvisible = false;
            vm.showsortby(event);
            expect(vm.sortbydrop).toBe(true);
            expect(vm.sortbydropvisible).toBe(true);
        });

        it("It should hide show sortby dropdown box", function () {
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

        it("It should display pagination on the page", function () {
            vm.totalrecords = 16;
            vm.pagination();
            expect(vm.fromtopage).toBe("1 - 16");
            expect(vm.displaypagination).toBe(true);
        });
    });

    describe("Verification of collapseDateControls function", function () {
        it("It should collapse date controls", function () {
            vm.collapseDateControls();
            expect(vm.openedStartDate).toBe(false);
            expect(vm.openedEndDate).toBe(false);
        });
    });

    describe("Verification of authortypeheadselect function", function () {
        it("It should select author", function () {
            var index = { name: "MAQ" };
            vm.authortypeheadselect(index, "test");
            expect(vm.selectedAuthor).toBe("MAQ");
        });
    });

    describe("Verification of showDocTabs function", function () {
        it("It should show document tab", function () {
            vm.showInnerNav = true;
            vm.showDocTabs(event);
            expect(vm.showInnerNav).toBe(false);
            expect(vm.showNavTab).toBe(true);
        });
    });

    describe("Verification of showSelectedTabs function", function () {
        it("It should show selected tab", function () {
            vm.showSelectedTabs("Pinned documents", 5);
            expect(vm.selectedTab).toBe("Pinned documents");
            expect(vm.selectedTabInfo).toBe("Pinned documents (5)");
        });
    });

    describe("Verification of sortExpression function", function () {
        it("It shouls set sorting vlaues", function () {
            vm.sortExpression("name", "test", "ascending");
            expect(vm.lazyloaderdashboard).toBe(false);
            expect(vm.divuigrid).toBe(false);
            expect(vm.nodata).toBe(false);
            expect(vm.displaypagination).toBe(false);
        });
    });

    describe("Verification of switchFuction function", function () {
        it("It set switches display column for GridColumn1Header", function () {
            var out = vm.switchFuction("GridColumn1Header");
            expect(out).toBe("Document");
        });

        it("It set switches display column for GridColumn2Header", function () {
            var out = vm.switchFuction("GridColumn2Header");
            expect(out).toBe("Client");
        });

        it("It set switches display column for GridColumn3Header", function () {
            var out = vm.switchFuction("GridColumn3Header");
            expect(out).toBe("Author");
        });

        it("It set switches display column for GridColumn4Header", function () {
            var out = vm.switchFuction("GridColumn4Header");
            expect(out).toBe("Modified Date");
        });

        it("It set switches display column for GridColumn5Header", function () {
            var out = vm.switchFuction("GridColumn5Header");
            expect(out).toBe("Document ID");
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
            var out = vm.switchFuction("GridColumn8Header");
            expect(out).toBe("Matter Name");
        });

        it("It set switches display column for default", function () {
            var out = vm.switchFuction("test");
            expect(out).toBe("");
        });
    });
});
