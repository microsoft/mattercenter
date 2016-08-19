// Test suite
describe('documents Controller service call test suite', function () {

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
        rootScope = $rootScope.$new();
        vm = $controller('documentsController as vm', { $scope: $scope, $state: $state, $stateParams: $stateParams, documentResource: mockDocumentResource, api: documentapi, $rootScope: rootScope, $http: $http, $location: $location, $q: $q, $animate: $animate });
    }));

    describe('Verification of watchFunc function', function () {
        it('It should get all grid data', function () {
            $scope.gridApi = { infiniteScroll: { dataLoaded: function () { } } };
            // $scope.gridApi.infiniteScroll.dataLoaded();
            var promise = vm.watchFunc();
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);
            expect(vm.lazyloader).toBe(true);
        });

    });

    describe('Verification of documentsearch function', function () {
        it('It should search related document', function () {
            var term = "FileName:Test*(* OR FileName:* OR dlcDocIdOWSText:* OR MCDocumentClientName:*)";
            var property = "FileName";
            vm.documentsearch(term, property, false);
            expect(vm.divuigrid).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.lazyloader).toBe(true);
            expect(vm.filternodata).toBe(false);
            expect(vm.details.length).toBeGreaterThan(0);
            expect(vm.details).not.toBe(null);
            //vm.documentsearch(term, property, true);

        });

    });

    describe('Verification of FilterModifiedDate function', function () {
        it('Data should be filtered based on modified date', function () {
            vm.modstartdate = new Date("08/01/2016");
            vm.modenddate = new Date("08/10/2016");
            vm.FilterModifiedDate("Modified Date");
            expect(vm.moddatefilter).toBe(true);
        });

    });

    describe('Verification of SetDocuments function', function () {
        it('document name should be added in dropdown', function () {
            $scope.gridApi = { infiniteScroll: { dataLoaded: function () { } } };
            vm.SetDocuments(1, "All Documents");
            expect(true).toBe(true);
            expect(vm.divuigrid).toBe(true);
            expect(vm.responseNull).toBe(false);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);
        });
    });

    describe('Verification of GetDocuments function', function () {
        it('document name should be added in dropdown', function () {
            $scope.gridApi = { infiniteScroll: { dataLoaded: function () { } } };
            vm.GetDocuments(3);
            expect(true).toBe(true);
            expect(vm.divuigrid).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);
        });
    });

    describe('Verification of PinMatter function', function () {
        it('It should be added in pinned list', function () {
            var pinObject = {
                entity: oTestConfiguration.oDocumentObject
            };
            vm.PinMatter(pinObject);
            vm.documentid = 1;
            vm.documentname = "All Documents";
            expect(vm.divuigrid).toBe(true);
            expect(vm.responseNull).toBe(false);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);

        });
    });

    describe('Verification of UnpinDocument function', function () {
        it('It should be removed from pinned list', function () {
            var pinObject = {
                entity: oTestConfiguration.oDocumentObject
            };
            vm.UnpinDocument(pinObject);
            vm.documentid = 1;
            vm.documentname = "All Documents";
            expect(vm.divuigrid).toBe(true);
            expect(vm.responseNull).toBe(false);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);

        });
    });

    describe('Verification of FilterByType function', function () {
        it('document count should be greater than 0', function () {
            vm.FilterByType();
            expect(vm.divuigrid).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);
        });
    });

    describe('Verification of sortChangedDocument function', function () {
        it('documents should be sort based on document name', function () {
            $scope.gridApi = { infiniteScroll: { dataLoaded: function () { }, resetScroll: function () { } } };
            var sortColumns = [{ "field": "documentName", "name": "documentName", "sort": "asc" }];
            vm.gridOptions.columnDefs[1] = { "field": "documentName", "displayName": "Document", "width": "278", "enableHiding": false, "cellTemplate": "../app/document/DocumentTemplates/DocumentCellTemplate.html", "headerCellTemplate": "../app/document/DocumentTemplates/DocumentHeaderTemplate.html", "name": "documentName", "type": "string" };
            $scope.sortChangedDocument(null, sortColumns);
            expect(vm.FileNameSort).toBe("desc");
            expect(vm.sortby).toBe("asc");
            expect(vm.sortexp).toBe("documentName");

        });
    });

    describe('Verification of typeheadselect function', function () {
        it('selected document result should be displayed', function () {
            $scope.gridApi = { infiniteScroll: { dataLoaded: function () { }, resetScroll: function () { } } };
            var selected = "All Attachments - image test.eml (280620050)";
            vm.typeheadselect(null, selected);
            expect(selected).toContain(vm.gridOptions.data[0].documentName);
        });
    });

});