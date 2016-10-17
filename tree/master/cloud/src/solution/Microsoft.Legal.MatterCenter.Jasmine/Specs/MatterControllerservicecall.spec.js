//// ***********************************************************************
//// Author           : MAQ USER
//// Created          : 31-08-2016
////
//// ***********************************************************************
//// <copyright file="MatterControllerservicecall.spec.js" company="MAQSoftware">
////  Copyright (c) . All rights reserved.
//// </copyright>
//// <summary>Test suite for Matters Controller for service call</summary>
//// ***********************************************************************

describe("Matters Controller test suite for service call", function () {

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

    describe("Verification of SetMatters function", function () {
        it("matter name should be added in dropdown", function () {
            vm.SetMatters(1, "All Matters");
            expect(vm.divuigrid).toBe(false);
            expect(vm.responseNull).toBe(false);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);
        });
    });

    describe("Verification of GetMatters function", function () {
        it("matter name should be added in dropdown", function () {
            vm.GetMatters(3);
            expect(vm.divuigrid).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);
        });
    });

    describe("Verification of mattersearch function", function () {
        it("It should search related matter", function () {
            var term = "MCMatterName:Test*(MCMatterName:* OR MCMatterID:* OR MCClientName:*)";
            var property = "MCMatterName";
            vm.mattersearch(term, property, false);
            expect(vm.divuigrid).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.lazyloader).toBe(true);
            expect(vm.filternodata).toBe(false);
            expect(vm.details.length).toBeGreaterThan(0);
            expect(vm.details).not.toBe(null);
        });
    });

    describe("Verification of FilterModifiedDate function", function () {
        it("Data should be filtered based on modified date", function () {
            vm.modstartdate = new Date("08/01/2016");
            vm.modenddate = new Date("08/10/2016");
            vm.FilterModifiedDate("Modified Date");
            expect(vm.moddatefilter).toBe(true);
        });
    });

    describe("Verification of PinMatter function", function () {
        it("It should be added in pinned list", function () {
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

    describe("Verification of UnpinMatter function", function () {
        it("It should be removed from pinned list", function () {
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

    describe("Verification of sortChanged function", function () {
        it("documents should be sort based on matter name", function () {
            var sortColumns = [{ "field": "matterName", "name": "matterName", "sort": "asc" }];
            vm.gridOptions.columnDefs[0] = { "field": "matterName", "displayName": "Matter", "enableHiding": false, "width": "275", "cellTemplate": "../app/matter/MatterTemplates/MatterCellTemplate.html", "headerCellTemplate": "../app/matter/MatterTemplates/MatterHeaderTemplate.html", "name": "matterName", "type": "string" };
            $scope.sortChanged(null, sortColumns);
            expect(vm.MatterNameSort).toBe("desc");
            expect(vm.sortby).toBe("asc");
            expect(vm.sortexp).toBe("matterName");
        });
    });


    describe("Verification of typeheadselect function", function () {
        it("selected document result should be displayed", function () {
            var selected = "Default Matter (11111)";
            vm.typeheadselect(null, selected);
            expect(selected).toContain("Default Matter (11111)");
        });
    });


    describe("Verification of watchFunc function", function () {
        it("It should watch the function", function () {
            vm.responseNull = false;
            vm.watchFunc();
            expect(vm.lazyloader).toBe(true);
            expect(vm.pagenumber).toBeGreaterThan(0);
            expect(vm.lazyloader).toBe(true);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
        });
    });

    describe("Verification of getFolderHierarchy function", function () {
        it("It should get the folder hierarchy", function () {
            vm.getFolderHierarchy("Default Matter", oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient", "6cbca4ab447c87302d3a1f0e3c32985a");
            expect(vm.oUploadGlobal.bAllowContentCheck).not.toBe(null);
            expect(vm.showSelectedFolderTree).not.toBe(null);
            expect(vm.lazyloader).not.toBe(null);
        });
    });

    describe("Verification of search function", function () {
        it("It should search related matter", function () {
            vm.selected = "Test Matter";
            vm.search();
            expect(vm.divuigrid).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.gridOptions.data).not.toBe(null);
        });
    });

    describe("Verification of FilterByType function", function () {
        it("It should filter data by type", function () {
            vm.FilterByType();
            expect(vm.gridOptions.data.length).toBeGreaterThan(0);
            expect(vm.nodata).toBe(false);
            expect(vm.divuigrid).toBe(true);
        });
    });

    describe("Verification of Openuploadmodal function", function () {
        it("It should show Openuploadmodal", function () {
            vm.Openuploadmodal("Default Matter", oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient", "6cbca4ab447c87302d3a1f0e3c32985a");
            expect(vm.oUploadGlobal.successBanner).toBe(false);
            expect(vm.isLoadingFromDesktopStarted).toBe(false);
        });
    });

    describe("Verification of localOverWriteDocument function", function () {
        it("It should show localOverWriteDocument", function () {
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

        it("It should not show localOverWriteDocument", function () {
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


    describe("Verification of showSelectedFolderTree function", function () {
        it("It should show showSelectedFolderTree", function () {
            vm.showSelectedFolderTree(folder);
            expect(vm.showSelectedFolderTree).not.toThrow(Error);
        });
    });

    describe("Verification of getContentCheckConfigurations function", function () {
        it("It should show Content Check Configurations", function () {
            vm.getContentCheckConfigurations(oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient");
            expect(vm.oUploadGlobal.bAllowContentCheck).not.toBe(null);
        });
    });

    describe("Verification of closeNotificationDialog function", function () {
        it("This should close Notification Dialog", function () {
            vm.closeNotificationDialog();
            expect(vm.IsDupliacteDocument).toBe(false);
            expect(vm.IsNonIdenticalContent).toBe(false);
            expect(vm.showLoading).toBe(false);
        });
    });

    describe("Verification of Openuploadmodal function", function () {
        it("This should Open upload modal", function () {
            vm.Openuploadmodal("Default Matter", oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient", "6cbca4ab447c87302d3a1f0e3c32985a");
            expect(vm.IsDupliacteDocument).toBe(false);
            expect(vm.IsNonIdenticalContent).toBe(false);
            expect(vm.showLoading).toBe(false);
            expect(vm.oUploadGlobal.bAllowContentCheck).not.toBe(null);
            expect(vm.showSelectedFolderTree).not.toBe(null);
            expect(vm.lazyloader).not.toBe(null);
        });
    });

    describe("Verification of getIconSource function", function () {
        it("This should get Icon Source", function () {
            var data = vm.getIconSource(".docx");
            expect(data).toBe(oEnvironmentConfiguration.tenantUrl + "/_layouts/15/images/ic.docx.gif");
            expect(vm.mailUpLoadSuccess).toBe(false);
        });
    });

    describe("Verification of clearFilters function", function () {
        it("It should clear all the filters", function () {
            vm.clearFilters("Responsible Attorney");
            expect(vm.matterdateheader).toBe(true);
            expect(vm.matterheader).toBe(true);
            expect(vm.lazyloader).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.pagenumber).toBe(1);
            expect(vm.divuigrid).toBe(true);
            expect(vm.gridOptions).toBeDefined();
        });
    });

    describe("Verification of modStartDate function", function () {
        it("It should return start date", function () {
            vm.modStartDate(event);
            expect(vm.modifiedStartDate).toBe(true);
        });
    })

    describe("Verification of disabled function", function () {
        it("It should change the status as per the date", function () {
            var date = { getDay: function () { return 1;}}
            var data = vm.disabled(date,"day");
            expect(data).toBe(true);
        });
    })

    describe("Verification of openMatterHeader function", function () {
        it("It should open the matter header", function () {
            var event = { target: { "getBoundingClientRect": function () { return 1; } } }
            vm.openMatterHeader(event, "Matter");
            expect(vm.filternodata).toBe(false);
            expect(vm.searchexp).toBe("MCMatterName");
            expect(vm.filtername).toBe("Matter");
            expect(vm.matterdateheader).toBe(true);
            expect(vm.matterheader).toBe(true);
        });
    });
    describe("Verification of filtermatter function", function () {
        it("It should filter the matter", function () {
            var data = vm.filtermatter("Matter");
            expect(data).toBe("Matter");
        });
    });

    describe("Verification of export function", function () {
        it("This should export the data in spreadsheet", function () {
            vm.searchexp = "MCMatterName";
            vm.filterSearch("test");
            expect(vm.divuigrid).toBe(true);
            expect(vm.nodata).toBe(false);
            expect(vm.lazyloaderFilter).toBe(true);
            expect(vm.details.length).toBeGreaterThan(0);
            expect(vm.filternodata).toBe(false);
        });
    });
});