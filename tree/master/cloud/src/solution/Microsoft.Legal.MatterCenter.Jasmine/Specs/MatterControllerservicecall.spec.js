//Test suite
describe('MattersController Controller test suite', function () {

    var mockapi = function (matterResource) {
        getData(matterResource, mockMatterResource);

    };

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("matterResource", ['$resource', 'auth', mockMatterResource]);
    }));

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("api", ['matterResource', 'documentResource', 'documentDashBoardResource', 'matterDashBoardResource', 'homeResource', mockapi]);
    }));

    beforeEach(module('ui.router'));
    beforeEach(module('ui.bootstrap'));

    beforeEach(inject(function ($controller, $rootScope) {
        rootScope = $rootScope.$new();
        vm = $controller('mattersController as vm', { $scope: $scope, $state: $state, $stateParams: $stateParams, matterResource: mockMatterResource, api: mockapi, $rootScope: rootScope, $http: $http, $location: $location, $q: $q, $animate: $animate });
    }));

    //describe('Verification of SetMatters function', function () {
    //    it('matter name should be added in dropdown', function () {
    //        vm.SetMatters(1, "All Matters");
    //        expect(vm.divuigrid).toBe(false);
    //        expect(vm.responseNull).toBe(false);
    //        expect(vm.nodata).toBe(false);
    //        expect(vm.gridOptions.data.length).toBeGreaterThan(0);
    //        expect(vm.gridOptions.data).not.toBe(null);
    //    });
    //});

    //describe('Verification of GetMatters function', function () {
    //    it('matter name should be added in dropdown', function () {
    //        vm.GetMatters(3);
    //        expect(vm.divuigrid).toBe(true);
    //        expect(vm.nodata).toBe(false);
    //        expect(vm.gridOptions.data.length).toBeGreaterThan(0);
    //        expect(vm.gridOptions.data).not.toBe(null);
    //    });
    //});

    //describe('Verification of mattersearch function', function () {
    //    it('It should search related matter', function () {
    //        var term = "MCMatterName:Test*(MCMatterName:* OR MCMatterID:* OR MCClientName:*)";
    //        var property = "MCMatterName";
    //        vm.mattersearch(term, property, false);
    //        expect(vm.divuigrid).toBe(true);
    //        expect(vm.nodata).toBe(false);
    //        expect(vm.lazyloader).toBe(true);
    //        expect(vm.filternodata).toBe(false);
    //        expect(vm.details.length).toBeGreaterThan(0);
    //        expect(vm.details).not.toBe(null);

    //    });

    //});

    //describe('Verification of FilterModifiedDate function', function () {
    //    it('Data should be filtered based on modified date', function () {
    //        vm.modstartdate = new Date("08/01/2016");
    //        vm.modenddate = new Date("08/10/2016");
    //        vm.FilterModifiedDate("Modified Date");
    //        expect(vm.moddatefilter).toBe(true);
    //    });

    //});

    describe('Verification of PinMatter function', function () {
        it('It should be added in pinned list', function () {
            var currentRowData = {
                entity: oTestConfiguration.oMatterObject
            };
            vm.PinMatter(currentRowData);
            expect(vm.divuigrid).toBe(false);
            expect(vm.responseNull).toBe(false);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).not.toBeLessThan(0);
            expect(vm.gridOptions.data).not.toBe(null);

        });
    });

    describe('Verification of UnpinMatter function', function () {
        it('It should be removed from pinned list', function () {
            var pinObject = {
                entity: oTestConfiguration.oMatterObject
            };
            vm.UnpinMatter(pinObject);;
            expect(vm.divuigrid).toBe(false);
            expect(vm.responseNull).toBe(false);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).not.toBeLessThan(0);
            expect(vm.gridOptions.data).not.toBe(null);

        });
    });

    describe('Verification of sortChanged function', function () {
        it('documents should be sort based on matter name', function () {
            var sortColumns = [{ "field": "matterName", "name": "matterName", "sort": "asc" }];
            vm.gridOptions.columnDefs[0] = { "field": "matterName", "displayName": "Matter", "enableHiding": false, "width": "275", "cellTemplate": "../app/matter/MatterTemplates/MatterCellTemplate.html", "headerCellTemplate": "../app/matter/MatterTemplates/MatterHeaderTemplate.html", "name": "matterName", "type": "string" };
            $scope.sortChanged(null, sortColumns);
            expect(vm.MatterNameSort).toBe("desc");
            expect(vm.sortby).toBe("asc");
            expect(vm.sortexp).toBe("matterName");
        });
    });


    describe('Verification of typeheadselect function', function () {
        it('selected document result should be displayed', function () {
            var selected = "Default Matter (11111)";
            vm.typeheadselect(null, selected);
            expect(selected).toContain(vm.gridOptions.data[0].matterName);
        });
    });


    describe('Verification of watchFunc function', function () {
        it('It should watchFunc', function () {
            vm.responseNull = false;
            vm.watchFunc();
            expect(vm.lazyloader).toBe(true);
            expect(vm.pagenumber).toBeGreaterThan(0);
            expect(vm.lazyloader).toBe(true);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
        });
    });

    describe('Verification of getFolderHierarchy function', function () {
        it('It should show getFolderHierarchy', function () {
            vm.getFolderHierarchy("Default Matter", "https://lcadms.sharepoint.com/sites/subsiteclient", "6cbca4ab447c87302d3a1f0e3c32985a");
            expect(vm.oUploadGlobal.bAllowContentCheck).toBe(true);
            expect(vm.foldersList.length).toBeGreaterThan(0);
            expect(vm.showSelectedFolderTree).not.toBe(null);
            expect(vm.lazyloader).toBe(true);
        });
    });

    describe('Verification of search function', function () {
        it('It should search related matter', function () {
            vm.selected = "Test Matter";
            vm.search();
            expect(vm.divuigrid).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);
        });
    });

    describe('Verification of FilterByType function', function () {
        it('It should show FilterByType', function () {
            vm.FilterByType();
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.nodata).toBe(false);
            expect(vm.divuigrid).toBe(true);
        });
    });

    describe('Verification of Openuploadmodal function', function () {
        it('It should show Openuploadmodal', function () {
            vm.Openuploadmodal("Default Matter", "https://lcadms.sharepoint.com/sites/subsiteclient", "6cbca4ab447c87302d3a1f0e3c32985a");
            expect(vm.oUploadGlobal.successBanner).toBe(false);
            expect(vm.isLoadingFromDesktopStarted).toBe(false);
        });
    });

    describe('Verification of localOverWriteDocument function', function () {
        it('It should show localOverWriteDocument', function () {
            vm.ducplicateSourceFile = {
                pop: function ()
                { return true; },
                filter: function () {
                    return true;
                }
            }
            vm.oUploadGlobal = {
                "arrFiles": {
                    pop: function ()
                    { return obj; }
                }
            };
            vm.ducplicateSourceFile.length = 1;
            var duplicateFile = { "cancel": null, "fileType": "remotefile" };
            vm.localOverWriteDocument(duplicateFile, "ignore");
            expect(vm.files).toBeDefined();
        });

        it('It should show localOverWriteDocument', function () {
            vm.ducplicateSourceFile = {
                pop: function ()
                { return true; },
                filter: function () {
                    return true;
                }
            }

            var duplicateFile = { "cancel": null, "fileType": "" };
            vm.localOverWriteDocument(duplicateFile, "overwrite");
            expect(vm.ducplicateSourceFile).toBe(true);
        });

    });


    describe('Verification of showSelectedFolderTree function', function () {
        it('It should show showSelectedFolderTree', function () {
            var folder = {
                "parentURL": "https://lcadms.sharepoint.com/sites/subsiteclient",
                "active": true,
                "children": { "child": { "active": true } }
            };
            vm.showSelectedFolderTree(folder);
            expect(vm.showSelectedFolderTree).not.toThrow(Error);
        });
    });

    describe('Verification of getContentCheckConfigurations function', function () {
        it('It should show getContentCheckConfigurations', function () {

            vm.getContentCheckConfigurations("https://lcadms.sharepoint.com/sites/subsiteclient");
            expect(vm.oUploadGlobal.bAllowContentCheck).toBe(true);
        });
    });
});