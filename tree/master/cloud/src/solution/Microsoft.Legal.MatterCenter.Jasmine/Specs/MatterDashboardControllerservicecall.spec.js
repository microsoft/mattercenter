//Test suite
describe('MatterDashBoard Controller test suite', function () {
    var $scope = {};
    var pm;
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
    $q.defer = function () { };
    $scope.$apply = function () { };

    var mockmatterDashBoardResource = {
        'get': '/api/v1/matter/get',
        'getPinnedMatters': '/api/v1/matter/getpinned',
        'getMyMatters': '/api/v1/matter/getpinned',
        'getTaxonomyDetails': '/api/v1/taxonomy/gettaxonomy',
        'UnpinMatter': '/api/v1/matter/unpin',
        'PinMatter': '/api/v1/matter/pin',
        'getFolderHierarchy': '/api/v1/matter/getfolderhierarchy',
        'getMatterCounts': '/api/v1/matter/getmattercounts'
    };
    var mockapi = function (matterDashBoardResource) {
        var mockmatterDashBoardResource = {
            'get': '/api/v1/matter/get',
            'getPinnedMatters': '/api/v1/matter/getpinned',
            'getMyMatters': '/api/v1/matter/getpinned',
            'getTaxonomyDetails': '/api/v1/taxonomy/gettaxonomy',
            'UnpinMatter': '/api/v1/matter/unpin',
            'PinMatter': '/api/v1/matter/pin',
            'getFolderHierarchy': '/api/v1/matter/getfolderhierarchy',
            'getMatterCounts': '/api/v1/matter/getmattercounts',
            'getDefaultMatterConfigurations': '/api/v1/matter/getconfigurations'
        };

        var url = "http://mattermaqdevsite.azurewebsites.net" + mockmatterDashBoardResource[matterDashBoardResource.method];
        function IsJsonString(str) {
            try {
                JSON.parse(str);
            } catch (e) {
                return false;
            }
            return true;
        }
        callAPI(matterDashBoardResource.success);
        function callAPI(callback) {

            var http = new XMLHttpRequest();
            var postdata;

            if (!IsJsonString(matterDashBoardResource.data)) {
                postdata = JSON.stringify(matterDashBoardResource.data);
            } else {
                postdata = matterDashBoardResource.data;
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
        $provide.factory("matterDashBoardResource", ['$resource', 'auth', mockmatterDashBoardResource]);
    }));

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("api", ['matterResource', 'documentResource', 'documentDashBoardResource', 'matterDashBoardResource', 'homeResource', mockapi]);
    }));

    beforeEach(module('ui.router'));
    beforeEach(module('ui.bootstrap'));

    beforeEach(inject(function ($controller, $rootScope) {
        rootScope = $rootScope.$new();
        pm = $controller('MatterDashBoardController as pm', { $scope: $scope, $state: $state, $stateParams: $stateParams, matterDashBoardResource: mockmatterDashBoardResource, api: mockapi, $rootScope: rootScope, $http: $http, $location: $location, $q: $q });
    }));


    describe('Verification of getMatterCounts function', function () {
        it('It should show MatterCounts', function () {
            $scope.$apply = function () { };
            pm.getMatterCounts();
            expect(pm.allMatterCount).toBeGreaterThan(0);
            expect(pm.myMatterCount).toBeGreaterThan(0);
            expect(pm.pinMatterCount).toBeGreaterThan(0);
            expect(pm.totalrecords).toBeGreaterThan(0);
            expect(pm.lazyloaderdashboard).toBe(true);
        });
    });

    describe('Verification of getMatterPinned function', function () {
        it('It should get MatterPinned', function () {
            $scope.$apply = function () { };
            pm.getMatterPinned();
            expect(pm.pinnedResponse).not.toBe(null);
            expect(pm.Pinnedobj.length).toBeDefined();
            expect(pm.matterGridOptions.data).toBeDefined();
            expect(pm.pinMatterCount).toBeDefined();
            expect(pm.totalrecords).toBeDefined();
            expect(pm.nodata).toBe(false);
            expect(pm.divuigrid).toBe(true);
            expect(pm.lazyloaderdashboard).toBe(true);
            expect(pm.displaypagination).toBe(true);
        });

    });

    //describe('Verification of searchMatters function', function () {
    //    it('It should get searchMatters', function () {
    //        pm.searchMatters("test");
    //        expect(pm.pagenumber).toBe(1);
            
    //    });

    //});


    describe('Verification of myMatters function', function () {
        it('It should get myMatters', function () {
            pm.searchText = "test";
            pm.myMatterCount = 5;
            pm.myMatters();
            expect(pm.matterGridOptions.data.length).toBeGreaterThan(0);
            expect(pm.totalrecords).toBeGreaterThan(1);
            expect(pm.nodata).toBe(false);
            expect(pm.divuigrid).toBe(true);
            expect(pm.lazyloaderdashboard).toBe(true);

        });

    });


    describe('Verification of showClientDrop function', function () {
        it('It should show clientdrop', function () {
            pm.clientdropvisible = false;
            pm.clients = undefined;
            pm.showClientDrop(event);
            expect(pm.clients).toBeDefined();
            expect(pm.clientdrop).toBe(true);
            expect(pm.clientdropvisible).toBe(true);
            expect(pm.lazyloaderclient).toBe(true);
            expect(pm.pgdrop).toBe(false);
            expect(pm.pgdropvisible).toBe(false);
            expect(pm.aoldrop).toBe(false);
            expect(pm.aoldropvisible).toBe(false);

        });

        it('It should not show clientdrop', function () {
            pm.clientdropvisible = true;
            pm.showClientDrop(event);
            expect(pm.clientdrop).toBe(false);
            expect(pm.clientdropvisible).toBe(false);
            expect(pm.lazyloaderclient).toBe(true);
            expect(pm.pgdrop).toBe(false);
            expect(pm.pgdropvisible).toBe(false);
            expect(pm.aoldrop).toBe(false);
            expect(pm.aoldropvisible).toBe(false);

        });
    });



    describe('Verification of myMatters function', function () {
        it('It should get myMatters', function () {
            pm.searchText = "test";
            pm.myMatterCount = 5;
            pm.myMatters();
            expect(pm.matterGridOptions.data.length).toBeGreaterThan(0);
            expect(pm.totalrecords).toBeGreaterThan(1);
            expect(pm.nodata).toBe(false);
            expect(pm.divuigrid).toBe(true);
            expect(pm.lazyloaderdashboard).toBe(true);

        });

    });


    describe('Verification of FilterByType function', function () {
        it('It should show FilterByType', function () {
            pm.FilterByType();
            expect(pm.totalrecords).toBeGreaterThan(0);
            expect(pm.matterGridOptions.data.length).toBeGreaterThan(0);
            expect(pm.lazyloader).toBe(true);
            expect(pm.nodata).toBe(false);
            expect(pm.divuigrid).toBe(true);
            expect(pm.lazyloaderdashboard).toBe(true);
        });
    });

    describe('Verification of sortyby function', function () {
        it('It should show sortyby', function () {
            var sortexp = "AlphabeticalUp";
            pm.sortyby(sortexp, "Searchkeyword");
            expect(pm.nodata).toBe(true);
            expect(pm.divuigrid).toBe(false);
            expect(pm.lazyloaderdashboard).toBe(true);
        });
    });

    describe('Verification of next function', function () {
        it('It should show next', function () {
            pm.last = 5;
            pm.totalrecords = 30;
            
            pm.next();
            expect(pm.first).toBeGreaterThan(0);
            expect(pm.last).toBeGreaterThan(0);
            expect(pm.total).toBeGreaterThan(0);
            expect(pm.pagenumber).toBeGreaterThan(0);
            expect(pm.fromtopage).toBe(pm.first + " - " + pm.totalrecords);
            expect(pm.displaypagination).toBe(true);
            expect(pm.matterGridOptions.data.length).toBeGreaterThan(0);
            expect(pm.nodata).toBe(false);
            expect(pm.divuigrid).toBe(true);
            expect(pm.lazyloaderdashboard).toBe(true);
        });
    });

    describe('Verification of prev function', function () {
        it('It should show prev', function () {
            pm.last = 50;
            pm.first = 50;
            pm.prev();
            expect(pm.first).toBeGreaterThan(0);
            expect(pm.last).toBeGreaterThan(0);
            expect(pm.pagenumber).toBe(0);
            expect(pm.matterGridOptions.data.length).toBeGreaterThan(0);
            expect(pm.fromtopage).toBe(pm.first + " - " + pm.last);
            expect(pm.nodata).toBe(false);
            expect(pm.divuigrid).toBe(true);
            expect(pm.lazyloaderdashboard).toBe(true);
        });
    });


    describe('verification of showPracticegroupDrop function', function () {
        it('It should show PracticegroupDrop', function () {
            pm.pgdropvisible = false;
            pm.practiceGroups = undefined;
            pm.aolTerms = undefined;
            
            pm.showPracticegroupDrop(event);
            expect(pm.practiceGroups).toBeDefined();
            expect(pm.lazyloaderpg).toBe(true);
            expect(pm.pgdrop).toBe(true);
            expect(pm.pgdropvisible).toBe(true);

        });

        it('It should not show PracticegroupDrop', function () {
            pm.pgdropvisible = false;
            pm.practiceGroups = "data";
            pm.aolTerms = "test";

            pm.showPracticegroupDrop(event);
            expect(pm.clientdrop).toBe(false);
            expect(pm.clientdropvisible).toBe(false);
            expect(pm.pgdrop).toBe(true);
            expect(pm.pgdropvisible).toBe(true);
            expect(pm.aoldrop).toBe(false);
            expect(pm.aoldropvisible).toBe(false);

        });

        it('It should not showPracticegroupDrop', function () {
            pm.clientdropvisible = true;
            pm.showClientDrop(event);
            expect(pm.clientdrop).toBe(false);
            expect(pm.clientdropvisible).toBe(false);
            expect(pm.lazyloaderpg).toBe(true);
            expect(pm.pgdrop).toBe(false);
            expect(pm.pgdropvisible).toBe(false);
            expect(pm.aoldrop).toBe(false);
            expect(pm.aoldropvisible).toBe(false);

        });

    });


    describe('Verification of getFolderHierarchy function', function () {
        it('It should show getFolderHierarchy', function () {
           
            pm.getFolderHierarchy("Default Matter", "https://lcadms.sharepoint.com/sites/subsiteclient", "6cbca4ab447c87302d3a1f0e3c32985a");
            expect(pm.oUploadGlobal.bAllowContentCheck).toBe(true);
            expect(pm.foldersList.length).toBeGreaterThan(0);
            expect(pm.showSelectedFolderTree).not.toBe(null);
            expect(pm.lazyloader).toBe(true);
        });
    });

    describe('Verification of Openuploadmodal function', function () {
        it('It should show Openuploadmodal', function () {

            pm.Openuploadmodal("Default Matter", "https://lcadms.sharepoint.com/sites/subsiteclient", "6cbca4ab447c87302d3a1f0e3c32985a");
            expect(pm.oUploadGlobal.successBanner).toBe(false);
            expect(pm.isLoadingFromDesktopStarted).toBe(false);
        });
    });

    describe('Verification of showclients function', function () {
        it('It should show showclients', function () {
            pm.client = undefined;
            pm.showclients(event);
            expect(pm.clients).toBeDefined();
            expect(pm.clientdrop).toBe(true);
            expect(pm.clientdropvisible).toBe(true);
        });
    });

    describe('Verification of getContentCheckConfigurations function', function () {
        it('It should show getContentCheckConfigurations', function () {

            pm.getContentCheckConfigurations("https://lcadms.sharepoint.com/sites/subsiteclient");
            expect(pm.oUploadGlobal.bAllowContentCheck).toBe(true);
        });
    });

    describe('Verification of showSelectedFolderTree function', function () {
        it('It should show showSelectedFolderTree', function () {
            var folder = {
                "parentURL": "https://lcadms.sharepoint.com/sites/subsiteclient",
                "active": true,
                "children":{"active":true}
            };
            pm.showSelectedFolderTree(folder);
            expect(pm.showSelectedFolderTree).not.toThrow(Error);
        });
    });

    describe('Verification of localOverWriteDocument function', function () {
        it('It should show localOverWriteDocument', function () {
            pm.ducplicateSourceFile = {
                pop: function ()
                { return true; }
            }
            pm.oUploadGlobal = {
                "arrFiles": {
                    pop: function ()
                    { return obj; }
                }
            };
            pm.ducplicateSourceFile.length = 1;
            var duplicateFile = { "cancel":null };
            pm.localOverWriteDocument(duplicateFile, "ignore");
            expect(pm.files).toBeDefined();
        });
    });

    describe('Verification of search function', function () {
        it('It should show search', function () {
            pm.search("Test");
            expect(pm.Pinnedobj).toBeDefined();
            expect(pm.pinMatterCount).toBeGreaterThan(0);
            expect(pm.matterGridOptions.data).toBeDefined();
            expect(pm.totalrecords).toBe(0);
            expect(pm.lazyloaderdashboard).toBe(true);
            expect(pm.divuigrid).toBe(true);
            
        });
    });

    describe('Verification of showAreaofLawDrop function', function () {
        it('It should show showAreaofLawDrop', function () {
            pm.aoldropvisible = false;
            pm.practiceGroups = undefined;
            pm.aolTerms = undefined;
            pm.showAreaofLawDrop(event);
            expect(pm.clientdrop).toBe(false);
            expect(pm.clientdropvisible).toBe(false);
            expect(pm.lazyloaderaol).toBe(true);
            expect(pm.pgdrop).toBe(false);
            expect(pm.pgdropvisible).toBe(false);
        });

        it('It should not show showAreaofLawDrop', function () {
            pm.clientdropvisible = true;
            pm.showClientDrop(event);
            expect(pm.clientdrop).toBe(false);
            expect(pm.clientdropvisible).toBe(false);
            expect(pm.lazyloaderaol).toBe(true);
            expect(pm.pgdrop).toBe(false);
            expect(pm.pgdropvisible).toBe(false);
            expect(pm.aoldrop).toBe(false);
            expect(pm.aoldropvisible).toBe(false);

        });
    });

    describe('Verification of pinorunpin function', function () {
        it('It should be added in pinned list and removed from pinned list', function () {
            var currentRowData = {
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
            };
            var count = pm.pinMatterCount;
            count = count + 1;
            var event = {};
            event.currentTarget = {};
            event.currentTarget.src = "../images/pin-666.png";

            pm.pinorunpin(event, currentRowData);
            expect(count).toBeGreaterThan(0);
            expect(pm.lazyloaderdashboard).toBe(true);
            
        });
    });

});

//search***
//showAreaofLawDrop**
//pinorunpin**
//searchMatters##

    //FilterByType
    //showClientDrop
    //sortyby
    //next
    //prev
    //getMatterCounts
    //getMatterPinned
    //myMatters
    //showPracticegroupDrop
    //Openuploadmodal
    //getFolderHierarchy
    //handleDesktopDrop
    //showclients
    //getContentCheckConfigurations
    //showSelectedFolderTree
    //localOverWriteDocument