//Test suite
describe('MattersController Controller test suite', function () {
    var $scope = { $watch: function () { } };
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
    $q.defer = function () { };
    var $animate = { enabled: function () { } };

    var mockmatterResource = function ($resource, auth) {
        return $resource(null, null,
            {
                'get': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/matter/get',
                    isArray: true
                }),
                'getPinnecmatters': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/matter/getpinned',
                    isArray: true
                }),
                'getMyMatters': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/matter/getpinned',
                    isArray: true
                }),
                'getTaxonomyDetails': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/taxonomy/gettaxonomy'
                }),
                'UnpinMatter': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/matter/unpin'
                }),
                'PinMatter': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/matter/pin'
                }),
                'getFolderHierarchy': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/matter/getfolderhierarchy'

                }),
                'getMatterCounts': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/matter/getmattercounts'

                }),
            });
    };

    var mockapi = function (matterResource, documentResource, documentDashBoardResource, matterDashBoardResource, homeResource) {
        var resources = {
            'matterResource': matterResource,
            'documentResource': documentResource,
            'documentDashBoardResource': documentDashBoardResource,
            'matterDashBoardResource': matterDashBoardResource,
            'homeResource': homeResource
        };

        function callAPI(options) {

            var resource = resources[options.resource];



            resource[options.method](options.data)
              .$promise.then(function (response) {

                  options.success(response);
              }).catch(function (e) {
                  // $scope.$parent.cm.Error = true;
                  //cm.Error = true;
                  var message = 'oops something went wrong. ';
                  if (e.data != null)
                      if (e.data.Message != undefined)
                          message = e.data.Message;
                      else
                          message = e.data;
                  else
                      message = e.message + e.description + e.stack + e.number;

                  // alert(message);
                  //$scope.$parent.cm.guid = "kdsjfl";
                  // cm.guid = "kdsjfl";

                  if (options.error) {
                      options.error(e);
                  } else {
                      ////if (e.status === 500) {
                      ////    window.location.href = "/Error/Index";
                      ////}
                      ////else {
                      ////    //ErrorNotification.notifyError(e.status);
                      ////}
                  }
              }).finally(function () {

              });

        }

        return function (api_options) {
            var apiOptions = api_options || {};
            if (!api_options.resource || !resources.hasOwnProperty(api_options.resource) ||
                !api_options.method || !api_options.success) {
                throw new Error('Invalid request. API, method and success are required.');
            }
            api_options.data = api_options.data || {};
            callAPI(apiOptions);
        };

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
        cm = $controller('mattersController as cm', { $scope: $scope, $state: $state, $stateParams: $stateParams, matterResource: mockmatterResource, api: mockapi, $rootScope: rootScope, $http: $http, $location: $location, $q: $q, $animate: $animate });
    }));


    describe('Verification of closeNotificationDialog function', function () {
        it('It should close notification dialog box', function () {
            cm.closeNotificationDialog();
            expect(cm.IsDupliacteDocument).toBe(false);
            expect(cm.IsNonIdenticalContent).toBe(false);
            expect(cm.showLoading).toBe(false);
        });
    });

    describe('Verification of attachmentTokenCallbackEmailClient function', function () {
        it('It should attach Token along with EmailClient-Need to call createMailPopup', function () {
            $scope.$apply = function () { };
            var asyncResult = { "status": "succeeded", "value": "testtoken" };
            cm.createMailPopup = function () { return 1; };
            cm.attachmentTokenCallbackEmailClient(asyncResult,obj);
            expect(cm.attachmentToken).toBe("testtoken");
            expect(cm.mailUpLoadSuccess).toBe(false);
            expect(cm.mailUploadedFolder).toBe(null);
            expect(cm.loadingAttachments).toBe(false);
            expect(cm.mailUploadedFile).toBe(null);
        });
    });

    describe('Verification of getIconSource function', function () {
        it('It should return source of icon', function () {
            var source = cm.getIconSource("sourceicon");
            expect(source).toBe("https://lcadms.sharepoint.com/_layouts/15/images/icsourceicon.gif");
        });
    });

    describe('Verification of closealldrops function', function () {
        it('It should close all the dropdowns', function () {
            cm.closealldrops();
            expect(cm.mattersdrop).toBe(false);
            expect(cm.mattersdropinner).toBe(true);
            expect(cm.matterheader).toBe(true);
            expect(cm.matterdateheader).toBe(true);
        });
    });

    describe('Verification of checkEmptyorWhitespace function', function () {
        it('It should check Empty or Whitespace of input', function () {
            var data = cm.checkEmptyorWhitespace("test");
            expect(data).toBe("test");
        });
    });

    describe('Verification of hideBreadCrumb function', function () {
        it('It should hide bread crumb', function () {
            
            cm.hideBreadCrumb();
            expect(rootScope.breadcrumb).toBe(true);
            expect(rootScope.foldercontent).toBe(false);
        });
    });

    describe('Verification of disabled function', function () {
        it('It should set the status', function () {
            var d = new Date()
            var status = cm.disabled(d,"day");
            expect(status).toBe(true);
        });
    });

    describe('Verification of showSortExp function', function () {
        it('It should sort as per the ascending order', function () {
            cm.sortexp = "test";
            cm.sortby="asc";
            $scope.$apply = function () { };
            cm.showSortExp();
            expect(angular.element()).toBeDefined();
        });
    });

    describe('Verification of showmatterdrop function', function () {
        it('It should show matter if if matter is pinned', function () {
            cm.mattersdropinner = true;
            cm.showmatterdrop(event);
            expect(cm.mattersdrop).toBe(true);
            expect(cm.mattersdropinner).toBe(false);
        });

        it('It should show matter if if matter is not pinned', function () {
            cm.mattersdropinner = false;
            cm.showmatterdrop(event);
            expect(cm.mattersdrop).toBe(false);
            expect(cm.mattersdropinner).toBe(true);
        });
    });

    describe('Verification of overwriteConfiguration function', function () {
        it('It should return overwrite configuration', function () {
            
            var bAppendEnabled = cm.overwriteConfiguration("testEmail.eml");
            expect(bAppendEnabled).toBe(true);
        });
    });

    describe('Verification of contentCheckNotification function', function () {
        it('It should check for contentCheckNotification', function () {
            var file = { "contentCheck": "", "saveLatestVersion": "", "cancel": "","append":true };
            cm.contentCheckNotification(file,true);
            expect(file.contentCheck).toBe("contentCheck");
            expect(file.saveLatestVersion).toBe("False");
            expect(file.cancel).toBe("False");
            expect(file.append).toBe(false);
        });
    });


    describe('Verification of abortContentCheck function', function () {
        it('It should set file parameter while aborting the content check', function () {
            cm.file = {};
            cm.file.value = "This";
            cm.abortContentCheck(cm.file, false);
            expect(cm.file.contentCheck).toBe(null);
            expect(cm.file.saveLatestVersion).toBe("True");
            expect(cm.file.value).toBe("This<br/><div>Content check has been aborted.</div>");
            expect(cm.file.cancel).toBe("True");
        });

    });

    describe('Verification of closeSuccessBanner function', function () {
        it('It should close success banner', function () {
            cm.closeSuccessBanner();
            expect(cm.oUploadGlobal.successBanner).toBe(false);
        });
    });

});
//setWidth
//editAttachment--> no value store
//saveAttachment--> no value store
//closeNotificationDialog
//attachmentTokenCallbackEmailClient
//getIconSource
//closealldrops
//checkEmptyorWhitespace
//hideBreadCrumb
//$watch--> this we need to comment as we cant handle on load
//disabled
//showSortExp
//showmatterdrop
//overwriteConfiguration
//contentCheckNotification
//abortContentCheck
//closeSuccessBanner

