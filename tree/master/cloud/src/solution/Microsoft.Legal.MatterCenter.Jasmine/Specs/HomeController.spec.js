//Test suite
describe('Home Controller test suite', function () {
    var $scope = {};
    var dm;
    var api;
    var matterResource;
    var rootScope = {};
    var $state = {"current":{"name":"test"},
        go: function () { }
    };
    var $stateParams;
    var $interval = { go: function () { } };
    var $watch;
    var $location;
    var adalService = {
        "userInfo": {
            "userName": "MAQUser@LCADMS.onmicrosoft.com",
            "profile": {
                "family_name": "LCADMS",
                "oid":786
            },
            "isAuthenticated":true
        },
       logOut:function(){}
    };

    var mockhomeResource = function ($resource, auth) {
        return $resource(null, null,
            {
                'getHelp': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/shared/help',
                    isArray: true
                }),
                'getUserProfilePicture': auth.attachCSRF({
                    method: 'POST',
                    url: '/api/v1/user/getuserprofilepicture'
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
                  // $scope.$parent.dm.Error = true;
                  //dm.Error = true;
                  var message = 'oops something went wrong. ';
                  if (e.data != null)
                      if (e.data.Message != undefined)
                          message = e.data.Message;
                      else
                          message = e.data;
                  else
                      message = e.message + e.description + e.stack + e.number;

                  // alert(message);
                  //$scope.$parent.dm.guid = "kdsjfl";
                  // dm.guid = "kdsjfl";

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
        $provide.factory("homeResource", ['$resource', 'auth', mockhomeResource]);
    }));

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("api", ['matterResource', 'documentResource', 'documentDashBoardResource', 'matterDashBoardResource', 'homeResource', mockapi]);
    }));

    beforeEach(module('ui.router'));
    beforeEach(module('ui.bootstrap'));

    beforeEach(inject(function ($controller, $rootScope) {
        rootScope = $rootScope.$new();
        dm = $controller('homeController as dm', { $scope: $scope, $state: $state, $stateParams: $stateParams, homeResource: mockhomeResource, api: mockapi, $rootScope: rootScope, $location: $location, adalAuthenticationService: adalService });
    }));

    //describe('Verification of setWidth function', function () {
    //    it('It should set width of screen', function () {

    //        dm.x = 5;
    //        dm.y = 2;
    //        dm.sum();
    //        expect(dm.z).toBe(7);
    //    });
    //});


   

});


