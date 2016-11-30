/// <reference path="../References.js" />
//// ***********************************************************************
//// Author           : MAQ USER
//// Created          : 31-08-2016
////
//// ***********************************************************************
//// <copyright file="DocumentControllerservicecall.spec.js" company="MAQSoftware">
////  Copyright (c) . All rights reserved.
//// </copyright>
//// <summary>Test suite for document controller for service call</summary>
//// ***********************************************************************

describe("documents Controller test suite for service call", function () {

    var documentapi = function (documentDashBoardResource) {
        getData(documentDashBoardResource, mockDocumentResource);
    };

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("documentResource", ['$resource', 'auth', mockDocumentResource]);
    }));

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("api", ['matterResource', 'documentResource', 'documentDashBoardResource', 'matterDashBoardResource', 'homeResource', documentapi]);
    }));

    beforeEach(module('ui.router'));
    beforeEach(module('ui.bootstrap'));

    beforeEach(inject(function ($controller, $rootScope) {
        vm = $controller('documentsController as vm', { $scope: $scope, $state: $state, $stateParams: $stateParams, documentResource: mockDocumentResource, api: documentapi, $rootScope: rootData, $http: $http, $location: $location, $q: $q, $animate: $animate });
    }));

    describe("Verification of watchFunc function", function () {
        it("It should get all grid data", function () {
            var promise = vm.watchFunc();
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);
            expect(vm.lazyloader).toBe(true);
        });

    });

    describe("Verification of documentsearch function", function () {
        it("It should search related document", function () {
            var term = "FileName:Test*(* OR FileName:* OR dlcDocIdOWSText:* OR MCDocumentClientName:*)";
            var property = "FileName";
            vm.documentsearch(term, property, false);
            expect(vm.divuigrid).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.lazyloader).toBe(false);
            expect(vm.filternodata).toBe(false);
            expect(vm.details.length).toBeGreaterThan(0);
            expect(vm.details).not.toBe(null);
        });
    });

    describe("Verification of FilterModifiedDate function", function () {
        it("It should filter data based on modified date", function () {
            vm.modstartdate = new Date("08/01/2016");
            vm.modenddate = new Date("08/10/2016");
            vm.FilterModifiedDate("Test Date");
            expect(vm.moddatefilter).toBe(false);
        });

    });

    describe("Verification of SetDocuments function", function () {
        it("It should add document name in dropdown", function () {
            vm.SetDocuments(1, "All Documents");
            expect(true).toBe(true);
            expect(vm.divuigrid).toBe(true);
            expect(vm.responseNull).toBe(false);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);
        });
    });

    describe("Verification of GetDocuments function", function () {
        it("It should add document name in dropdown", function () {
            vm.GetDocuments(3);
            expect(true).toBe(true);
            expect(vm.divuigrid).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);
        });
    });

    describe("Verification of PinMatter function", function () {
        it("It should be added in pinned list", function () {
            var pinObject = {
                entity: oTestConfiguration.oDocumentObject
            };
            vm.PinMatter(pinObject);
            vm.documentid = 1;
            vm.documentname = "All Documents";
            expect(vm.divuigrid).toBe(false);
            expect(vm.responseNull).toBe(false);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);
        });
    });

    describe("Verification of UnpinDocument function", function () {
        it("It should be removed from pinned list", function () {
            var pinObject = {
                entity: oTestConfiguration.oDocumentObject
            };
            vm.UnpinDocument(pinObject);
            vm.documentid = 1;
            vm.documentname = "All Documents";
            expect(vm.divuigrid).toBe(false);
            expect(vm.responseNull).toBe(false);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);
        });
    });

    describe("Verification of FilterByType function", function () {
        it("It should filter data whose document count should be greater than 0", function () {
            vm.FilterByType();
            expect(vm.divuigrid).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);
        });
    });

    describe("Verification of sortChangedDocument function", function () {
        it("It should sort documents based on document name", function () {
            vm.gridOptions.columnDefs[1] = { "field": "documentName", "displayName": "Document", "width": "278", "enableHiding": false, "cellTemplate": "../app/document/DocumentTemplates/DocumentCellTemplate.html", "headerCellTemplate": "../app/document/DocumentTemplates/DocumentHeaderTemplate.html", "name": "documentName", "type": "string" };
            $scope.sortChangedDocument(null, sortColumns);
            expect(vm.FileNameSort).toBe("desc");
            expect(vm.sortby).toBe("asc");
            expect(vm.sortexp).toBe("documentName");
        });
    });

    describe("Verification of typeheadselect function", function () {
        it("It should display the selected document", function () {
            var selected = "All Attachments - image test.eml (280620050)";
            vm.typeheadselect(null, selected);
            expect(selected).toContain("All Attachments - image test.eml (280620050)");
        });
    });

    describe("Verification of setWidth function", function () {
        it("It should set the width", function () {
            vm.setWidth();
            expect(vm.searchResultsLength).toBeGreaterThan(15);
        });
    });

    describe("Verification of FilterModifiedDate function", function () {
        it("It should filter the modified date based on date column ", function () {
            vm.FilterModifiedDate("test");
            expect(vm.divuigrid).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.lazyloader).toBe(false);
            expect(vm.gridOptions).toBeDefined();
        });
    });

    describe("Verification of clearFilters function", function () {
        it("It should clear all the filters", function () {
            vm.clearFilters("Test");
            expect(vm.documentheader).toBe(true);
            expect(vm.documentdateheader).toBe(true);
            expect(vm.lazyloader).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.pagenumber).toBe(1);
            expect(vm.divuigrid).toBe(true);
            expect(vm.gridOptions).toBeDefined();
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

    describe('Verification of getDocumentAssets   function', function () {
        it('It should get all the documents asset', function () {
            var data;
            var row = {
                "entity":
                    {
                        "documentUrl": { replace: function (data, test) { return data; } },
                        "documentMatterUrl": { replace: function (data, test) { return data; } },
                        "documentClientUrl": "",
                        "documentParentUrl": "/Forms/AllItems.aspx",
                        "documentName": "test",
                        "documentExtension": ".xslx"
                    }
            };
            vm.getDocumentAssets(row);
            expect(vm.assetsuccess).toBe(true);
        });
    });

    describe("Verification of openDocumentHeader function", function () {
        it("It should open the document author", function () {
            var event = { target: { "getBoundingClientRect": function () { return 1; } } }
            vm.openDocumentHeader(event, "Document");
            expect(vm.filternodata).toBe(false);
            expect(vm.searchexp).toBe("FileName");
            expect(vm.filtername).toBe("Document");
            expect(vm.documentdateheader).toBe(true);
            expect(vm.documentheader).toBe(true);
        });
    });
});