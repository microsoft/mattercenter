/// <reference path="../References.js" />
/// <disable> JS2074,JS3058,JS2085,JS3056,JS3116,JS3054,JS2064,JS2043,JS2073,JS2038 </disable>
//// ***********************************************************************
//// Author           : MAQ USER
//// Created          : 31-08-2016
////
//// ***********************************************************************
//// <copyright file="DocumentDashboardControllerServiceCall.spec.js" company="MAQSoftware">
////  Copyright (c) . All rights reserved.
//// </copyright>
//// <summary>Test suite for documentDashboard for service call</summary>
//// ***********************************************************************

describe("DocumentDashboard Controller test suite for service call", function () {
    var mockDocumentapi = function (documentDashBoardResource) {
        getData(documentDashBoardResource, mockDocumentDashBoardResource);
    };

    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("documentDashBoardResource", ["$resource", "auth", mockDocumentDashBoardResource]);
    }));

    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("api", ["matterResource", "documentResource", "documentDashBoardResource", "matterDashBoardResource", "homeResource", mockDocumentapi]);
    }));

    beforeEach(module("ui.router"));
    beforeEach(module("ui.bootstrap"));

    beforeEach(inject(function ($controller, $injector, $rootScope) {
        rootScope = $rootScope.$new();
        vm = $controller("DocumentDashBoardController as vm", { $scope: $scope, $state: $state, $stateParams: $stateParams, documentDashBoardResource: mockDocumentDashBoardResource, api: mockDocumentapi, $rootScope: rootScope, $http: $http, $location: $location });
    }));

    describe("Verification of getDocumentCounts function", function () {
        it("It should get a count of all documents whose count is greater than or equal to 0 ", function () {
            vm.getDocumentCounts();
            expect(vm.allDocumentCount).not.toBeLessThan(0);
            expect(vm.myDocumentCount).not.toBeLessThan(0);
            expect(vm.pinDocumentCount).not.toBeLessThan(0);
            expect(vm.totalrecords).not.toBeLessThan(0);
        });
    });

    describe("Verification of getPinnedDocuments function", function () {
        it("It should return the all pinned documents", function () {
            vm.getPinnedDocuments();
            expect(vm.documentGridOptions.data).not.toBe(null);
            expect(vm.documentGridOptions.data.length).toBeGreaterThan(1);
            expect(vm.totalrecords).not.toBe(null);
            expect(vm.lazyloaderdashboard).toBe(true);
            expect(vm.divuigrid).toBe(true);
        });
    });

    describe("Verification of export function", function () {
        it("It should set export vlaues", function () {
            vm.export();
            expect(vm.exportDate).toBeDefined();
        });
    });

    describe("Verification of getMyDocuments function", function () {
        it("It should return My documents", function () {
            vm.getMyDocuments();
            expect(vm.lazyloaderdashboard).toBe(true);
            expect(vm.displaypagination).toBe(true);
            expect(vm.divuigrid).toBe(true);
            expect(vm.nodata).toBe(false);
        });
    });

    describe("Verification of getDocuments function", function () {
        it("It should get the Documents", function () {
            vm.getDocuments();
            expect(vm.lazyloaderdashboard).toBe(true);
            expect(vm.divuigrid).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.pinDocumentCount).toBeGreaterThan(0);
            expect(vm.documentGridOptions.data.length).toBeGreaterThan(1);
            expect(vm.totalrecords).not.toBeLessThan(0);
            expect(vm.pinDocumentCount).toBeGreaterThan(0);
            expect(vm.nodata).toBe(false);

        });
    });

    describe("Verification of showclientdrop function", function () {
        it("It should show clientdrop", function () {
            vm.clientdropvisible = false;
            vm.clients = undefined;
            vm.showclientdrop(event);
            expect(vm.clients).toBeDefined();
            expect(vm.clientdrop).toBe(true);
            expect(vm.clientdropvisible).toBe(true);
            expect(vm.lazyloaderdocumentclient).toBe(true);
        });

        it("It should not show clientdrop", function () {
            vm.clientdropvisible = true;
            vm.showclientdrop(event);
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
            expect(vm.lazyloaderdocumentclient).toBe(true);
        });
    });

    describe("Verification of FilterByType function", function () {
        it("It should show filter the data by type", function () {
            vm.FilterByType();
            expect(vm.totalrecords).toBeGreaterThan(0);
            expect(vm.documentGridOptions.data.length).toBeGreaterThan(0);
            expect(vm.nodata).toBe(false);
            expect(vm.divuigrid).toBe(true);
            expect(vm.lazyloaderdashboard).toBe(true);
        });
    });

    describe("Verification of sortyby function", function () {
        it("It should show sort the data", function () {
            var sortexp = "AlphabeticalUp";
            vm.sortyby(sortexp, "Searchkeyword");
            expect(vm.totalrecords).toBeGreaterThan(0);
            expect(vm.documentGridOptions.data.length).toBeGreaterThan(0);
            expect(vm.nodata).toBe(false);
            expect(vm.divuigrid).toBe(true);
            expect(vm.lazyloaderdashboard).toBe(true);
        });
    });

    describe("Verification of next function", function () {
        it("It should show next", function () {
            vm.last = 5;
            vm.totalrecords = 30;
            vm.next();
            expect(vm.first).toBeGreaterThan(0);
            expect(vm.last).toBeGreaterThan(0);
            expect(vm.total).not.toBeLessThan(0);
            expect(vm.pagenumber).toBeGreaterThan(0);
            expect(vm.fromtopage).toBe(vm.first + " - " + vm.totalrecords);
            expect(vm.lazyloader).toBe(true);
            expect(vm.documentGridOptions.data.length).toBeGreaterThan(0);
            expect(vm.nodata).toBe(false);
            expect(vm.divuigrid).toBe(true);
            expect(vm.lazyloaderdashboard).toBe(true);
        });
    });

    describe("Verification of prev function", function () {
        it("It should show prev", function () {
            vm.last = 50;
            vm.first = 50;
            vm.prev();
            expect(vm.first).toBeGreaterThan(0);
            expect(vm.last).toBeGreaterThan(0);
            expect(vm.pagenumber).toBe(0);
            expect(vm.documentGridOptions.data.length).toBeGreaterThan(0);
            expect(vm.fromtopage).toBe(vm.first + " - " + vm.last);
            expect(vm.nodata).toBe(false);
            expect(vm.divuigrid).toBe(true);
            expect(vm.lazyloaderdashboard).toBe(true);
        });
    });

    describe("Verification of pinorunpin function", function () {
        it("It should be added in pinned list and removed from pinned list", function () {
            var iCount = vm.pinDocumentCount;
            event.currentTarget.src = "../images/pin-666.png";
            vm.pinorunpin(event, oTestConfiguration.oDocumentObject);
            iCount = iCount + 1;
            expect(iCount).toBe(vm.pinDocumentCount);
            event.currentTarget.src = "../images/unpin-666.png";
            iCount = iCount - 1;
            vm.documentGridOptions.data = [];
            vm.documentGridOptions.data[0] = oTestConfiguration.oDocumentObject;
            vm.pinorunpin(event, oTestConfiguration.oDocumentObject);
            expect(iCount).toBe(vm.pinDocumentCount);
        });
    });

    describe('Verification of search  function', function () {
        it('It should execute a search', function () {
            vm.search();
            expect(vm.pagenumber).toBe(1)
            expect(vm.displaypagination).toBe(true);
            expect(vm.documentname).toBe("All Documents");
            expect(vm.documentid).toBe(1);
            expect(vm.lazyloaderdashboard).toBe(true);
            expect(vm.divuigrid).toBe(true);
            expect(vm.responseNull).toBe(false);
            expect(vm.documentGridOptions).toBeDefined();
            expect(vm.nodata).toBe(false);
        });
    });

    describe('Verification of typeheadselect function', function () {
        it('It should select a typehead ', function () {
            vm.typeheadselect("test", "");
            expect(vm.documentname).toBe("All Documents");
            expect(vm.documentid).toBe(1);
            expect(vm.totalrecords).toBeGreaterThan(0);
            expect(vm.documentGridOptions.data.length).toBeGreaterThan(0);
            expect(vm.nodata).toBe(false);
            expect(vm.divuigrid).toBe(true);
            expect(vm.lazyloaderdashboard).toBe(true);
        });
    });

    describe("Verification of filterSearchCancel function", function () {
        it("It should be triggered when the user clicks on 'Cancel' button in the filter panel", function () {
            vm.filterSearchCancel("client");
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
            expect(vm.lazyloaderdocumentclient).toBe(true);
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
    })

    describe("Verification of openStartDate function", function () {
        it("It should return start date", function () {
            vm.openStartDate(event);
            expect(vm.openedStartDate).toBe(true);
        });
    })

    describe("Verification of openEndDate function", function () {
        it("It should return start date", function () {
            vm.openEndDate(event);
            expect(vm.openedEndDate).toBe(true);
        });
    })

    describe('Verification of getDocumentAssets function', function () {
        it('It should get all the documents asset', function () {
            var data;
            vm.getDocumentAssets(row);
            expect(vm.searchdrop).toBe(false);
            expect(vm.downwarddrop).toBe(true);
            expect(vm.upwarddrop).toBe(false);
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
            expect(vm.sortbydrop).toBe(false);
            expect(vm.sortbydropvisible).toBe(false);
        });
    });

    describe("Verification of getSearchResults function", function () {
        it("This should get the search results", function () {
            vm.getSearchResults();
            expect(vm.lazyloaderdashboard).toBe(true);
            expect(vm.documentGridOptions).toBeDefined();
            expect(vm.divuigrid).toBe(true);
            expect(vm.searchdrop).toBe(false);
            expect(vm.totalrecords).toBeGreaterThan(0);
            expect(vm.nodata).toBe(false);
            expect(vm.allDocumentCount).not.toBeLessThan(0);
            expect(vm.myDocumentCount).not.toBeLessThan(0);
            expect(vm.pinDocumentCount).not.toBeLessThan(0);
            expect(vm.totalrecords).not.toBeLessThan(0);
        });
    });
});

