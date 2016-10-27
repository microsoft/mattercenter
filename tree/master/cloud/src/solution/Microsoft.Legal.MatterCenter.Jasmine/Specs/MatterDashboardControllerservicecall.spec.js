/// <reference path="../References.js" />
//// ***********************************************************************
//// Author           : MAQ USER
//// Created          : 31-08-2016
////
//// ***********************************************************************
//// <copyright file="MatterDashboardControllerservicecall.spec.js" company="MAQSoftware">
////  Copyright (c) . All rights reserved.
//// </copyright>
//// <summary>Test suite for Matter Dashboard Controller for service call</summary>
//// ***********************************************************************

describe("MatterDashBoard Controller test suite for service call", function () {

    var mockapi = function (matterDashBoardResource) {
        getData(matterDashBoardResource, mockMatterDashBoardResource);
    };

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("matterDashBoardResource", ['$resource', 'auth', mockMatterDashBoardResource]);
    }));

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("api", ['matterResource', 'documentResource', 'documentDashBoardResource', 'matterDashBoardResource', 'homeResource', mockapi]);
    }));

    beforeEach(module('ui.router'));
    beforeEach(module('ui.bootstrap'));
    
    beforeEach(inject(function ($controller, $rootScope) {
        rootScope = $rootScope.$new();
        vm = $controller('MatterDashBoardController as vm', { $scope: $scope, $state: $state, $stateParams: $stateParams, matterDashBoardResource: mockMatterDashBoardResource, api: mockapi, $rootScope: rootScope, $http: $http, $location: $location, $q: $q });
    }));

    describe("Verification of getMatterCounts function", function () {
        it("It should show all matters count", function () {
            vm.getMatterCounts();
            expect(vm.allMatterCount).toBeGreaterThan(0);
            expect(vm.myMatterCount).toBeGreaterThan(0);
            expect(vm.pinMatterCount).toBeGreaterThan(0);
            expect(vm.totalrecords).toBeGreaterThan(0);
            expect(vm.lazyloaderdashboard).toBe(true);
        });
    });

    describe("Verification of getMatterPinned function", function () {
        it("It should show all the pinned matters", function () {
            vm.getMatterPinned();
            expect(vm.pinnedResponse).not.toBe(null);
            expect(vm.Pinnedobj.length).toBeDefined();
            expect(vm.matterGridOptions.data).toBeDefined();
            expect(vm.pinMatterCount).toBeDefined();
            expect(vm.totalrecords).toBeDefined();
            expect(vm.nodata).toBe(false);
            expect(vm.divuigrid).toBe(true);
            expect(vm.lazyloaderdashboard).toBe(true);
            expect(vm.displaypagination).toBe(true);
        });

    });

    //// **************** This Method is returning the value instead of setting in object in latest build ****************

    ////describe('Verification of searchMatters function', function () {
    ////    it('It should get searchMatters', function () {
    ////        vm.searchMatters("test");
    ////        expect(vm.pagenumber).toBe(1);
    ////    });
    ////});

    describe("Verification of myMatters function", function () {
        it("It should get all the matters", function () {
            vm.searchText = "test";
            vm.myMatterCount = 5;
            vm.myMatters();
            expect(vm.matterGridOptions.data.length).toBeGreaterThan(0);
            expect(vm.totalrecords).toBeGreaterThan(1);
            expect(vm.nodata).toBe(false);
            expect(vm.divuigrid).toBe(true);
            expect(vm.lazyloaderdashboard).toBe(true);
        });
    });

    describe("Verification of showClientDrop function", function () {
        it("It should show clientdrop", function () {
            vm.clientdropvisible = false;
            vm.clients = undefined;
            vm.showClientDrop(event);
            expect(vm.clients).toBeDefined();
            expect(vm.clientdrop).toBe(true);
            expect(vm.clientdropvisible).toBe(true);
            expect(vm.lazyloaderclient).toBe(true);
            expect(vm.pgdrop).toBe(false);
            expect(vm.pgdropvisible).toBe(false);
            expect(vm.aoldrop).toBe(false);
            expect(vm.aoldropvisible).toBe(false);
        });

        it("It should not show clientdrop", function () {
            vm.clientdropvisible = true;
            vm.showClientDrop(event);
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
            expect(vm.lazyloaderclient).toBe(true);
            expect(vm.pgdrop).toBe(false);
            expect(vm.pgdropvisible).toBe(false);
            expect(vm.aoldrop).toBe(false);
            expect(vm.aoldropvisible).toBe(false);

        });
    });

   describe("Verification of FilterByType function", function () {
        it("It should filter all the data by type", function () {
            vm.FilterByType();
            expect(vm.totalrecords).toBeGreaterThan(0);
            expect(vm.matterGridOptions.data.length).toBeGreaterThan(0);
            expect(vm.lazyloader).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.divuigrid).toBe(true);
            expect(vm.lazyloaderdashboard).toBe(true);
        });
    });

    describe("Verification of sortyby function", function () {
        it("It should sort all the data", function () {
            var sortexp = "AlphabeticalUp";
            vm.sortby(sortexp, "Searchkeyword");
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
            expect(vm.displaypagination).toBe(true);
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
            expect(vm.matterGridOptions.data.length).toBeGreaterThan(0);
            expect(vm.fromtopage).toBe(vm.first + " - " + vm.last);
            expect(vm.nodata).toBe(false);
            expect(vm.divuigrid).toBe(true);
            expect(vm.lazyloaderdashboard).toBe(true);
        });
    });

    describe("verification of showPracticegroupDrop function", function () {
        it("It should show PracticegroupDrop", function () {
            vm.pgdropvisible = false;
            vm.practiceGroups = undefined;
            vm.aolTerms = undefined;
            vm.showPracticegroupDrop(event);
            expect(vm.lazyloaderpg).toBe(true);
            expect(vm.pgdrop).toBe(true);
            expect(vm.pgdropvisible).toBe(true);
        });

        it("It should not show PracticegroupDrop", function () {
            vm.pgdropvisible = false;
            vm.practiceGroups = "data";
            vm.aolTerms = "test";
            vm.showPracticegroupDrop(event);
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
            expect(vm.pgdrop).toBe(true);
            expect(vm.pgdropvisible).toBe(true);
            expect(vm.aoldrop).toBe(false);
            expect(vm.aoldropvisible).toBe(false);
        });

        it("It should not showPracticegroupDrop", function () {
            vm.clientdropvisible = true;
            vm.showClientDrop(event);
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
            expect(vm.lazyloaderpg).toBe(true);
            expect(vm.pgdrop).toBe(false);
            expect(vm.pgdropvisible).toBe(false);
            expect(vm.aoldrop).toBe(false);
            expect(vm.aoldropvisible).toBe(false);
        });

    });

    describe("Verification of getFolderHierarchy function", function () {
        it("It should getFolderHierarchy", function () {
            vm.getFolderHierarchy("Default Matter", oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient", "6cbca4ab447c87302d3a1f0e3c32985a");
            expect(vm.showSelectedFolderTree).not.toBe(null);
        });
    });

    describe("Verification of Openuploadmodal function", function () {
        it("It should show Openuploadmodal", function () {
            vm.Openuploadmodal("Default Matter", oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient", "6cbca4ab447c87302d3a1f0e3c32985a");
            expect(vm.oUploadGlobal.successBanner).toBe(false);
            expect(vm.isLoadingFromDesktopStarted).toBe(false);
        });
    });

    describe("Verification of showclients function", function () {
        it("It should show showclients", function () {
            vm.client = undefined;
            vm.showclients(event);
            expect(vm.clients).toBeDefined();
            expect(vm.clientdrop).toBe(true);
            expect(vm.clientdropvisible).toBe(true);
        });
    });

    describe("Verification of getContentCheckConfigurations function", function () {
        it("It should getContentCheckConfigurations", function () {
            vm.getContentCheckConfigurations(oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient");
            expect(vm.oUploadGlobal.bAllowContentCheck).not.toBe(null);
        });
    });

    describe("Verification of showSelectedFolderTree function", function () {
        it("It should showSelectedFolderTree", function () {
            vm.showSelectedFolderTree(folder);
            expect(vm.showSelectedFolderTree).not.toThrow(Error);
        });
    });

    describe("Verification of localOverWriteDocument function", function () {
        it("It should show localOverWriteDocument", function () {
            vm.ducplicateSourceFile = {
                pop: function ()
                { return true; }
            }
            vm.oUploadGlobal = {
                "arrFiles": {
                    pop: function ()
                    { return obj; }
                }
            };
            vm.ducplicateSourceFile.length = 1;
            var duplicateFile = { "cancel": null };
            vm.localOverWriteDocument(duplicateFile, "ignore");
            expect(vm.files).toBeDefined();
        });
    });

    describe("Verification of search function", function () {
        it("It should show search results", function () {
            vm.search("Test");
            expect(vm.Pinnedobj).toBeDefined();
            expect(vm.pinMatterCount).toBeGreaterThan(0);
            expect(vm.matterGridOptions.data).toBeDefined();
            expect(vm.totalrecords).not.toBeLessThan(0);
            expect(vm.lazyloaderdashboard).toBe(true);
            expect(vm.divuigrid).toBe(true);

        });
    });

    describe("Verification of showAreaofLawDrop function", function () {
        it("It should show showAreaofLawDrop", function () {
            vm.aoldropvisible = false;
            vm.practiceGroups = undefined;
            vm.aolTerms = undefined;
            vm.showAreaofLawDrop(event);
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
            expect(vm.lazyloaderaol).toBe(false);
            expect(vm.pgdrop).toBe(false);
            expect(vm.pgdropvisible).toBe(false);
        });

        it("It should not show showAreaofLawDrop", function () {
            vm.clientdropvisible = true;
            vm.showClientDrop(event);
            expect(vm.clientdrop).toBe(false);
            expect(vm.clientdropvisible).toBe(false);
            expect(vm.lazyloaderaol).toBe(true);
            expect(vm.pgdrop).toBe(false);
            expect(vm.pgdropvisible).toBe(false);
            expect(vm.aoldrop).toBe(false);
            expect(vm.aoldropvisible).toBe(false);

        });
    });

    describe("Verification of pinorunpin function", function () {
        it("It should be added in pinned list and removed from pinned list", function () {     
            var count = vm.pinMatterCount;
            count = count + 1;
            event.currentTarget.src = "../images/pin-666.png";
            vm.pinorunpin(event, oTestConfiguration.oMatterObject);
            expect(count).toBeGreaterThan(0);
            expect(vm.lazyloaderdashboard).toBe(true);

        });
    });

    describe("Verification of typeheadslect function", function () {
        it("This should select the typehead", function () {
            vm.typeheadselect("test", "");
            expect(vm.displaypagination).toBe(true);
            expect(vm.matterGridOptions).toBeDefined();
            expect(vm.divuigrid).toBe(true);
            expect(vm.lazyloaderdashboard).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.allMatterCount).toBeGreaterThan(0);
            expect(vm.myMatterCount).toBeGreaterThan(0);
            expect(vm.pinMatterCount).toBeGreaterThan(0);
            expect(vm.totalrecords).toBeGreaterThan(0);
        });
    });

    describe("Verification of searchByTerm function", function () {
        it("This should search by term", function () {
            vm.searchByTerm();
            expect(vm.lazyloaderdashboard).toBe(true);
            expect(vm.divuigrid).toBe(true);
            expect(vm.displaypagination).toBe(true);
            expect(vm.matterid).toBe(1);
            expect(vm.pagenumber).toBe(1);
            expect(vm.mattername).toBe("All Matters");
            expect(vm.totalrecords).toBeGreaterThan(0);
            expect(vm.matterGridOptions).toBeDefined();
            expect(vm.allMatterCount).toBeGreaterThan(0);
            expect(vm.myMatterCount).toBeGreaterThan(0);
            expect(vm.pinMatterCount).toBeGreaterThan(0);
            expect(vm.totalrecords).toBeGreaterThan(0);
        });
    });

    describe("Verification of getSearchResults function", function () {
        it("This should get the search result", function () {
            vm.getSearchResults();
            expect(vm.lazyloaderdashboard).toBe(true);
            expect(vm.matterGridOptions).toBeDefined();
            expect(vm.divuigrid).toBe(true);
            expect(vm.searchdrop).toBe(false);
            expect(vm.totalrecords).toBeGreaterThan(0);
            expect(vm.nodata).toBe(false);
            expect(vm.allMatterCount).toBeGreaterThan(0);
            expect(vm.myMatterCount).toBeGreaterThan(0);
            expect(vm.pinMatterCount).toBeGreaterThan(0);
            expect(vm.totalrecords).toBeGreaterThan(0);
        });
    });

    describe("Verification of export function", function () {
        it("This should export the data in spreadsheet", function () {
            vm.export();
            expect(vm.exportDate).toBeDefined();
        });
    });

});
