/// <disable>JS2074, JS3058</disable>
// Test suite
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

    //describe("Verification of closealldrops function", function () {
    //    it("It should close all dropdowns on click of page", function () {
    //        vm.x = 5;
    //        vm.y = 2;
    //        vm.sum();
    //        expect(vm.z).toBe(7);

    //    });
    //});

    describe("Verification of closealldrops function", function () {
        it("It should close all dropdowns on click of page", function () {
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
            vm.toggleChecker(true, obj);
            expect(vm.documentsCheckedCount).toBe(3);
            expect(vm.cartelements).toBeDefined();
        });
        it("It should disable toggle to check all documents", function () {
            vm.documentsCheckedCount = 2;
            vm.gridApi = {};
            vm.gridApi.grid = 1;
            vm.gridApi = gridrows;
            vm.toggleChecker(false, obj);
            expect(vm.documentsCheckedCount).toBe(1);
        });
    });

    describe("Verification of removeAttachment function", function () {
        it("It should add an elements to the cart", function () {
            vm.cartelements = obj;
            vm.documentsCheckedCount = 3;
            vm.removeAttachment(obj[0]);
            expect(vm.cartelements.length).toBe(1);
            expect(vm.documentsCheckedCount).toBe(2);
        });
        it("It should remove all the elements from the cart", function () {
            vm.cartelements = {};
            vm.cartelements.length = 0;
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
});