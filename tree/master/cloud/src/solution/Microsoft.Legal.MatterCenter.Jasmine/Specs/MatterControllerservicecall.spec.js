//Test suite
describe('MattersController Controller test suite', function () {
    var $scope = {
        $apply: function () { },
        "gridApi": { "infiniteScroll": { dataLoaded: function () { } } }
    };
    var cm;
    var api;
    var matterResource;
    var rootScope = {};
    var $state = { go: function () { } };
    var $stateParams;
    var $interval = { go: function () { } };
    var $watch;
    var $http;
    var $location;
    var $q = {};
    $q.defer = function () { return { resolve: function () { } } };



    var mockmatterResource = {
        'get': '/api/v1/matter/get',
        'getPinnedMatters': '/api/v1/matter/getpinned',
        'UnpinMatters': '/api/v1/matter/unpin',
        'PinMatters': '/api/v1/matter/pin',
        'getTaxonomyDetails': '/api/v1/taxonomy/gettaxonomy',
        'checkMatterExists': '/api/v1/matter/checkmatterexists',
        'getDefaultMatterConfigurations': '/api/v1/matter/getconfigurations',
        'getUsers': '/api/v1/user/getusers',
        'getRoles': '/api/v1/user/getroles',
        'getPermissionLevels': '/api/v1/user/getpermissionlevels',
        'checkSecurityGroupExists': '/api/v1/matter/checksecuritygroupexists',
        'getFolderHierarchy': '/api/v1/matter/getfolderhierarchy',
        'createMatter': '/api/v1/matter/create',
        'assignUserPermissions': '/api/v1/matter/assignuserpermissions',
        'assignContentType': '/api/v1/matter/assigncontenttype',
        'createLandingPage': '/api/v1/matter/createlandingpage',
        'updateMatterMetadata': '/api/v1/matter/UpdateMetadata',
        'getStampedProperties': '/api/v1/matter/getstampedproperties',
        'uploadEmail': '/api/v1/document/UploadMail',
        'uploadAttachment': '/api/v1/document/UploadAttachments',
        'uploadfiles': '/api/v1/document/UploadAttachments',
        'getHelp': '/api/v1/shared/help'
    };

    var mockapi = function (matterResource) {
        var mockmatterResource = {
            'get': '/api/v1/matter/get',
            'getPinnedMatters': '/api/v1/matter/getpinned',
            'UnpinMatters': '/api/v1/matter/unpin',
            'PinMatters': '/api/v1/matter/pin',
            'getTaxonomyDetails': '/api/v1/taxonomy/gettaxonomy',
            'checkMatterExists': '/api/v1/matter/checkmatterexists',
            'getDefaultMatterConfigurations': '/api/v1/matter/getconfigurations',
            'getUsers': '/api/v1/user/getusers',
            'getRoles': '/api/v1/user/getroles',
            'getPermissionLevels': '/api/v1/user/getpermissionlevels',
            'checkSecurityGroupExists': '/api/v1/matter/checksecuritygroupexists',
            'getFolderHierarchy': '/api/v1/matter/getfolderhierarchy',
            'createMatter': '/api/v1/matter/create',
            'assignUserPermissions': '/api/v1/matter/assignuserpermissions',
            'assignContentType': '/api/v1/matter/assigncontenttype',
            'createLandingPage': '/api/v1/matter/createlandingpage',
            'updateMatterMetadata': '/api/v1/matter/UpdateMetadata',
            'getStampedProperties': '/api/v1/matter/getstampedproperties',
            'uploadEmail': '/api/v1/document/UploadMail',
            'uploadAttachment': '/api/v1/document/UploadAttachments',
            'uploadfiles': '/api/v1/document/UploadAttachments',
            'getHelp': '/api/v1/shared/help'
        };

        var url = "http://mattermaqdevsite.azurewebsites.net" + mockmatterResource[matterResource.method];
        function IsJsonString(str) {
            try {
                JSON.parse(str);
            } catch (e) {
                return false;
            }
            return true;
        }
        callAPI(matterResource.success);
        function callAPI(callback) {

            var http = new XMLHttpRequest();
            var postdata;

            if (!IsJsonString(matterResource.data)) {
                postdata = JSON.stringify(matterResource.data);
            } else {
                postdata = matterResource.data;
            }

            http.open("POST", url, false);
            var accessToken = "Bearer " + sessionStorage.getItem('adal.idtoken');
            //Send the proper header information along with the request
            http.setRequestHeader("Content-type", "application/json");
            http.setRequestHeader("Accept", "application/json");
            http.setRequestHeader("Authorization", accessToken);
            http.send(postdata);

            if (http.status === 200) {// That's HTTP for 'ok'
                console.log(http.responseText);
                if (callback)
                    callback(JSON.parse(http.responseText));
                else
                    return JSON.parse(http.responseText);
            }
        }

    };

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("matterResource", ['$resource', 'auth', mockmatterResource]);
    }));

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("api", ['matterResource', 'documentResource', 'documentDashBoardResource', 'matterDashBoardResource', 'homeResource', mockapi]);
    }));

    beforeEach(module('ui.router'));
    beforeEach(module('ui.bootstrap'));

    beforeEach(inject(function ($controller, $rootScope) {
        rootScope = $rootScope.$new();
        cm = $controller('mattersController as cm', { $scope: $scope, $state: $state, $stateParams: $stateParams, matterResource: mockmatterResource, api: mockapi, $rootScope: rootScope, $http: $http, $location: $location, $q: $q });
    }));

    describe('Verification of SetMatters function', function () {
        it('matter name should be added in dropdown', function () {
            cm.SetMatters(1, "All Matters");
            expect(cm.divuigrid).toBe(false);
            expect(cm.responseNull).toBe(false);
            expect(cm.nodata).toBe(false);
            expect(cm.gridOptions.data.length).toBeGreaterThan(0);
            expect(cm.gridOptions.data).not.toBe(null);
        });
    });

    describe('Verification of GetMatters function', function () {
        it('matter name should be added in dropdown', function () {
            cm.GetMatters(3);
            expect(cm.divuigrid).toBe(true);
            expect(cm.nodata).toBe(false);
            expect(cm.gridOptions.data.length).toBeGreaterThan(0);
            expect(cm.gridOptions.data).not.toBe(null);
        });
    });

    describe('Verification of mattersearch function', function () {
        it('It should search related matter', function () {
            var term = "MCMatterName:Test*(MCMatterName:* OR MCMatterID:* OR MCClientName:*)";
            var property = "MCMatterName";
            cm.mattersearch(term, property, false);
            expect(cm.divuigrid).toBe(true);
            expect(cm.nodata).toBe(false);
            expect(cm.lazyloader).toBe(true);
            expect(cm.filternodata).toBe(false);
            expect(cm.details.length).toBeGreaterThan(0);
            expect(cm.details).not.toBe(null);

        });

    });

    describe('Verification of FilterModifiedDate function', function () {
        it('Data should be filtered based on modified date', function () {
            cm.modstartdate = new Date("08/01/2016");
            cm.modenddate = new Date("08/10/2016");
            cm.FilterModifiedDate("Modified Date");
            expect(cm.moddatefilter).toBe(true);
        });

    });

    describe('Verification of PinMatter function', function () {
        it('It should be added in pinned list', function () {
            var currentRowData = {
                entity: {
                    matterName: "Default Matter",
                    matterDescription: "Test Matter",
                    matterCreatedDate: "19/08/2016",
                    matterUrl: "https://lcadms.sharepoint.com/sites/subsiteclient/SitePages/6cbca4ab447c87302d3a1f0e3c32985a.aspx",
                    matterPracticeGroup: "Business Transactions",
                    matterAreaOfLaw: "Family Business",
                    matterSubAreaOfLaw: "Family Business",
                    matterClientUrl: "https://lcadms.sharepoint.com/sites/subsiteclient",
                    matterClient: "Microsoft",
                    matterClientId: "578cfafb-59eb-4f4c-b219-47886c61e384",
                    hideUpload: true,
                    matterID: "1516561262162",
                    matterResponsibleAttorney: "CELA",
                    matterModifiedDate: "19/08/2016",
                    matterGuid: "578cfafb-59eb-4f4c-b219-47886c61e384",
                    pinType: 'unpin'
                }
            };
            cm.PinMatter(currentRowData);
            expect(cm.divuigrid).toBe(false);
            expect(cm.responseNull).toBe(false);
            expect(cm.nodata).toBe(false);
            expect(cm.gridOptions.data.length).toBeGreaterThan(0);
            expect(cm.gridOptions.data).not.toBe(null);

        });
    });

    describe('Verification of UnpinMatter function', function () {
        it('It should be removed from pinned list', function () {
            var pinObject = {
                entity: {
                    matterName: "Default Matter",
                    matterDescription: "Test Matter",
                    matterCreatedDate: "19/08/2016",
                    matterUrl: "https://lcadms.sharepoint.com/sites/subsiteclient/SitePages/6cbca4ab447c87302d3a1f0e3c32985a.aspx",
                    matterPracticeGroup: "Business Transactions",
                    matterAreaOfLaw: "Family Business",
                    matterSubAreaOfLaw: "Family Business",
                    matterClientUrl: "https://lcadms.sharepoint.com/sites/subsiteclient",
                    matterClient: "Microsoft",
                    matterClientId: "578cfafb-59eb-4f4c-b219-47886c61e384",
                    hideUpload: true,
                    matterID: "1516561262162",
                    matterResponsibleAttorney: "CELA",
                    matterModifiedDate: "19/08/2016",
                    matterGuid: "578cfafb-59eb-4f4c-b219-47886c61e384",
                    pinType: 'unpin'
                }
            };
            cm.UnpinMatter(pinObject);;
            expect(cm.divuigrid).toBe(false);
            expect(cm.responseNull).toBe(false);
            expect(cm.nodata).toBe(false);
            expect(cm.gridOptions.data.length).toBeGreaterThan(0);
            expect(cm.gridOptions.data).not.toBe(null);

        });
    });

    describe('Verification of sortChanged function', function () {
        it('documents should be sort based on matter name', function () {
            $scope.gridApi = { infiniteScroll: { dataLoaded: function () { }, resetScroll: function () { } } };
            var sortColumns = [{ "field": "matterName", "name": "matterName", "sort": "asc" }];
            cm.gridOptions.columnDefs[0] = { "field": "matterName", "displayName": "Matter", "enableHiding": false, "width": "275", "cellTemplate": "../app/matter/MatterTemplates/MatterCellTemplate.html", "headerCellTemplate": "../app/matter/MatterTemplates/MatterHeaderTemplate.html", "name": "matterName", "type": "string" };
            $scope.sortChanged(null, sortColumns);
            expect(cm.MatterNameSort).toBe("desc");
            expect(cm.sortby).toBe("asc");
            expect(cm.sortexp).toBe("matterName");
        });
    });


    describe('Verification of typeheadselect function', function () {
        it('selected document result should be displayed', function () {
            $scope.gridApi = { infiniteScroll: { dataLoaded: function () { }, resetScroll: function () { } } };
            var selected = "Default Matter (11111)";
            cm.typeheadselect(null, selected);
            expect(selected).toContain(cm.gridOptions.data[0].matterName);
        });
    });


    describe('Verification of watchFunc function', function () {
        it('It should watchFunc', function () {
            cm.responseNull = false;
            cm.watchFunc();
            expect(cm.lazyloader).toBe(true);
            expect(cm.pagenumber).toBeGreaterThan(0);
            expect(cm.lazyloader).toBe(true);
            expect(cm.gridOptions.data.length).toBeGreaterThan(0);
        });
    });

    describe('Verification of getFolderHierarchy function', function () {
        it('It should show getFolderHierarchy', function () {

            cm.getFolderHierarchy("Default Matter", "https://lcadms.sharepoint.com/sites/subsiteclient", "6cbca4ab447c87302d3a1f0e3c32985a");
            expect(cm.oUploadGlobal.bAllowContentCheck).toBe(true);
            expect(cm.foldersList.length).toBeGreaterThan(0);
            expect(cm.showSelectedFolderTree).not.toBe(null);
            expect(cm.lazyloader).toBe(true);
        });
    });

    describe('Verification of search function', function () {
        it('It should search related matter', function () {
            cm.selected = "Test Matter";
            cm.search();
            expect(cm.divuigrid).toBe(true);
            expect(cm.nodata).toBe(false);
            expect(cm.gridOptions.data.length).toBeGreaterThan(0);
            expect(cm.gridOptions.data).not.toBe(null);
        });
    });

    describe('Verification of FilterByType function', function () {
        it('It should show FilterByType', function () {
            cm.FilterByType();
            expect(cm.gridOptions.data.length).toBeGreaterThan(0);
            expect(cm.nodata).toBe(false);
            expect(cm.divuigrid).toBe(true);
        });
    });

    describe('Verification of Openuploadmodal function', function () {
        it('It should show Openuploadmodal', function () {

            cm.Openuploadmodal("Default Matter", "https://lcadms.sharepoint.com/sites/subsiteclient", "6cbca4ab447c87302d3a1f0e3c32985a");
            expect(cm.oUploadGlobal.successBanner).toBe(false);
            expect(cm.isLoadingFromDesktopStarted).toBe(false);
        });
    });

    describe('Verification of localOverWriteDocument function', function () {
        it('It should show localOverWriteDocument', function () {
            cm.ducplicateSourceFile = {
                pop: function ()
                { return true; },
                filter: function () {
                    return true;
                }
            }
            cm.oUploadGlobal = {
                "arrFiles": {
                    pop: function ()
                    { return obj; }
                }
            };
            cm.ducplicateSourceFile.length = 1;
            var duplicateFile = { "cancel": null, "fileType": "remotefile" };
            cm.localOverWriteDocument(duplicateFile, "ignore");
            expect(cm.files).toBeDefined();
        });
        
        it('It should show localOverWriteDocument', function () {
            cm.ducplicateSourceFile = {
                pop: function ()
                { return true; },
                filter: function () {
                    return true;
                }
            }
           
            var duplicateFile = { "cancel": null, "fileType": "" };
            cm.localOverWriteDocument(duplicateFile, "overwrite");
            expect(cm.ducplicateSourceFile).toBe(true);
        });

    });


    describe('Verification of showSelectedFolderTree function', function () {
        it('It should show showSelectedFolderTree', function () {
            var folder = {
                "parentURL": "https://lcadms.sharepoint.com/sites/subsiteclient",
                "active": true,
                "children": { "child": { "active": true }}
            };
            cm.showSelectedFolderTree(folder);
            expect(cm.showSelectedFolderTree).not.toThrow(Error);
        });
    });

    describe('Verification of getContentCheckConfigurations function', function () {
        it('It should show getContentCheckConfigurations', function () {

            cm.getContentCheckConfigurations("https://lcadms.sharepoint.com/sites/subsiteclient");
            expect(cm.oUploadGlobal.bAllowContentCheck).toBe(true);
        });
    });
});


//FilterByType
//search
//Openuploadmodal
//getFolderHierarchy
//localOverWriteDocument
//showSelectedFolderTree
//getContentCheckConfigurations




//watchFunc
//getFolderHierarchy
//handleOutlookDrop
//GetMatters
//UnpinMatter
//PinMatter
//FilterByType
//sortChanged
//typeheadselect
//handleDesktopDrop
//getContentCheckConfigurations
//Openuploadmodal
//oUploadGlobal

//initOutlook
//createMailPopup
//searchMatter
//search
//SetMatters
//mattersearch
//FilterModifiedDate
//ducplicateSourceFile
//showSelectedFolderTree
//localOverWriteDocument
//uploadEmail
//uploadAttachment
//overWriteDocument