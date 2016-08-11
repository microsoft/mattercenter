//Test suite
describe('CreateMatter Controller test suite', function () {
    var $scope = {};
    var vm;
    var api;
    var matterResource;
    var rootScope = {};
    var $filter;
    var $state = { go: function () { } };
    var $stateParams;
    var $window;
    var data = { "name": "nikunj" };

    var mockmatterResourceService = {

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

    var mockapi = function (matterResource, callback) {

        var mockmatterResourceService = {
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


        var url = "http://mattermaqdevsite.azurewebsites.net" + mockmatterResourceService[matterResource.method];
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
        $provide.factory("matterResource", ['$resource', 'auth', mockmatterResourceService]);
    }));

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("api", ['matterResource', 'documentResource', 'documentDashBoardResource', 'matterDashBoardResource', 'homeResource', mockapi]);
    }));

    beforeEach(module('ui.router'));
    beforeEach(module('ui.bootstrap'));

    beforeEach(inject(function ($controller, $injector, $rootScope) {
        rootScope = $rootScope.$new();
        vm = $controller('createMatterController as vm', { $scope: $scope, $rootScope: rootScope, $state: $state, $stateParams: $stateParams, matterResource: mockmatterResourceService, api: mockapi });
    }));

    describe('Verification of Check valid matter function', function () {
        it('It should return true for new matter', function () {
            vm.matterName = "Test Matter 112233";
            vm.clientUrl = "https://lcadms.sharepoint.com/sites/microsoft";
            vm.checkValidMatterName();
            expect(vm.checkValidMatterName).toBe(true);
        });

        it('It should return false for new matter', function () {
            vm.matterName = "Microsoft Test";
            vm.clientUrl = "https://lcadms.sharepoint.com/sites/microsoft";
            vm.checkValidMatterName();
            expect(vm.checkValidMatterName).toBe(false);
        });

    });

    describe('Verification of selectMatterType function', function () {
        it('Successfully get Practice Group, Area of law and SubArea of law', function () {
            vm.selectMatterType();
            expect(vm.selectedPracticeGroup).toBe(vm.pracitceGroupList[0]);
            expect(vm.areaOfLawTerms).toBe(vm.pracitceGroupList[0].areaTerms);
            expect(vm.subAreaOfLawTerms).toBe(vm.pracitceGroupList[0].areaTerms[0].subareaTerms);
            expect(vm.activeAOLTerm).toBe(vm.pracitceGroupList[0].areaTerms[0]);
            expect(vm.activeSubAOLTerm).toBe(vm.pracitceGroupList[0].areaTerms[0].subareaTerms[0]);
        });

    });

    describe('Verification of getSelectedClientValue function', function () {
        it('Successfully get data from matter configuration list', function () {

            var client = { id: 0016761, name: "Microsoft", url: 'https://lcadms.sharepoint.com/sites/microsoft' };
            debugger;
            vm.getSelectedClientValue(client);
            expect(vm.secureMatterCheck).toBe(true);
            expect(vm.includeCalendar).toBe(true);
            expect(vm.includeEmail).toBe(true);
            expect(vm.includeRssFeeds).toBe(true);
            expect(vm.defaultConfilctCheck).toBe(true);
            expect(vm.isMatterDescriptionMandatory).toBe(true);
            expect(vm.includeTasks).toBe(true);
        });

    });

    //describe('Verification of createMatterButton function', function () {
    //    it('Successfully create matter button', function () {
    //        vm.chkConfilctCheck = true;
    //        vm.conflictDate = "8/1/2016";
    //        vm.createMatterButton(event);
    //        expect(localStorage.getItem('IsRestrictedAccessSelected')).toBe('true');
    //        expect(localStorage.getItem('IsCalendarSelected')).toBe('true');
    //        expect(localStorage.getItem('IsRSSSelected')).toBe('true');
    //        expect(localStorage.getItem('IsEmailOptionSelected')).toBe('true');
    //        expect(localStorage.getItem('IsConflictCheck')).toBe('true');
    //        expect(localStorage.getItem('IsMatterDescriptionMandatory')).toBe('false');
    //        expect(localStorage.getItem('IsTaskSelected')).toBe('true');
    //    });
    //});



});
