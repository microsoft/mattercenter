//Test suite
describe('documents Controller test suite', function () {
    var $scope = {};
    var om;
    var api;
    var matterResource;
    var rootScope = {};
    var $state = { go: function () { } };
    var $stateParams;
    var $interval = { go: function () { } };
    var $watch;
    var $http;
    var $location = {
        absUrl: function () {
            var url = "https://mattermaqdevsite.azurewebsites.net&test=1&attempt=2|jasminetest.html";
            return url;
        }
    };
    var $q = {};
    $q.defer = function () { };

    var mockdocumentResource = function ($resource, auth) {

        return $resource(null, null,
            {
                'get': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/document/getdocuments',
                    isArray: true
                }),
                'getPinnedDocuments': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/document/getpinneddocuments',
                    isArray: true
                }),
                'unPinDocument': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/document/unpindocument'
                }),
                'pinDocument': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/document/pindocument'
                }),
                'getassets': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/document/getassets'
                })
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
                  // $scope.$parent.om.Error = true;
                  //om.Error = true;
                  var message = 'oops something went wrong. ';
                  if (e.data != null)
                      if (e.data.Message != undefined)
                          message = e.data.Message;
                      else
                          message = e.data;
                  else
                      message = e.message + e.description + e.stack + e.number;

                  // alert(message);
                  //$scope.$parent.om.guid = "kdsjfl";
                  // om.guid = "kdsjfl";

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
        $provide.factory("documentResource", ['$resource', 'auth', mockdocumentResource]);
    }));

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("api", ['matterResource', 'documentResource', 'documentDashBoardResource', 'matterDashBoardResource', 'homeResource', mockapi]);
    }));

    beforeEach(module('ui.router'));
    beforeEach(module('ui.bootstrap'));

    beforeEach(inject(function ($controller, $rootScope) {
        rootScope = $rootScope.$new();
        om = $controller('documentsController as om', { $scope: $scope, $state: $state, $stateParams: $stateParams, documentResource: mockdocumentResource, api: mockapi, $rootScope: rootScope, $http: $http, $location: $location, $q: $q });
    }));

    describe('Verification of showdocdrop function', function () {
        it('It should show docdrop menu', function () {
            om.docdropinner = true;
            om.showdocdrop(event);
            expect(om.documentsdrop).toBe(true);
            expect(om.docdropinner).toBe(false);

        });
        it('It should show hide docdrop menu', function () {
            om.docdropinner = false;
            om.showdocdrop(event);
            expect(om.documentsdrop).toBe(false);
            expect(om.docdropinner).toBe(true);

        });
    });

    describe('Verification of closealldrops function', function () {
        it('It should close all dropdown menu', function () {
            om.closealldrops();
            expect(om.documentsdrop).toBe(false);
            expect(om.docdropinner).toBe(true);
            expect(om.documentheader).toBe(true);
            expect(om.documentdateheader).toBe(true);

        });
    });

    describe('Verification of getTableHeight function', function () {
        it('It should set dynamic height to the grid', function () {
            om.isOutlook = true;
            var heightobj = om.getTableHeight();
            expect(heightobj).toBeDefined();
            expect(heightobj).not.toBe(null);
        });
        it('It should not set dynamic height to the grid', function () {
            om.isOutlook = false;
            var heightobj = om.getTableHeight();
            expect(heightobj).toBeDefined();
            expect(heightobj).not.toBe(null);

        });
    });
    describe('Verification of isOutlookAsAttachment function', function () {
        it('It should set dynamic height to the grid', function () {
            om.isOutlookAsAttachment(true);
            expect(om.showAttachment).not.toBe(true);
            expect(om.showAttachment).toBeDefined();
            expect(om.enableAttachment).toBe(false);

        });

    });
    describe('Verification of closeNotification function', function () {
        it('It should close all the notification', function () {
            om.closeNotification();
            expect(om.showPopUpHolder).toBe(false);
            expect(om.showSuccessAttachments).toBe(false);

        });
    });

    describe('Verification of search function', function () {
        it('It should perform the search text', function () {
            om.selected = "";
            om.search();
            expect(om.pagenumber).toBe(1);
            expect(om.documentname).toBe("All Documents");
            expect(om.documentid).toBe(1);
            expect(om.lazyloader).toBe(false);
            expect(om.responseNull).toBe(false);
            expect(om.divuigrid).toBe(false);

        });
    });

    describe('Verification of disabled function', function () {
        it('It should set the status', function () {
            var d = new Date()
            var status = om.disabled(d, "day");
            expect(status).toBe(true);
        });
    });

    describe('Verification of showSortExp function', function () {
        it('It should sort as per the ascending order', function () {
            om.sortexp = "test";
            om.sortby = "asc";
            $scope.$apply = function () { };
            om.showSortExp();
            expect(angular.element()).toBeDefined();
        });
    });


    describe('Verification of toggleCheckerAll function', function () {
        it('It should check all checkboxes inside grid', function () {
            $scope.$apply = function () { };
            $scope.gridApi = {
                "selection":
                    { selectAllRows: function () { },
                    clearSelectedRows: function () { }}
            };
            om.gridOptions.data = obj;
            om.toggleCheckerAll(true);
            expect(om.gridOptions.data[0].checker).toBe(true);
            expect(om.documentsCheckedCount).toBe(2);
            expect(om.selectedRows).toBe(obj);
        });
        it('It should not check checkboxes inside grid', function () {
            $scope.$apply = function () { };
            $scope.gridApi = {
                "selection":
                    {
                        selectAllRows: function () { },
                        clearSelectedRows: function () { }
                    }
            };
            om.gridOptions.data = obj;
            om.toggleCheckerAll(false);
            expect(om.gridOptions.data[0].checker).toBe(false);
            expect(om.documentsCheckedCount).toBe(0);
            expect(om.selectedRows).not.toBe(null);
            expect(om.showErrorAttachmentInfo).toBe(false);
        });
    });


});



