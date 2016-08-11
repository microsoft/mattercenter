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
        logOut:function(){  dm.status=true;}
    };

   var mockhomeResource = {
        'getHelp': '/api/v1/shared/help',
        'getUserProfilePicture': '/api/v1/user/getuserprofilepicture'
    };

    var mockapi = function (homeResource) {
        var mockhomeResource = {
            'getHelp': '/api/v1/shared/help',
            'getUserProfilePicture': '/api/v1/user/getuserprofilepicture'
        };
        var url = "http://mattermaqdevsite.azurewebsites.net" + mockhomeResource[homeResource.method];
        function IsJsonString(str) {
            try {
                JSON.parse(str);
            } catch (e) {
                return false;
            }
            return true;
        }
        callAPI(homeResource.success);
        function callAPI(callback) {

            var http = new XMLHttpRequest();
            var postdata;

            if (!IsJsonString(homeResource.data)) {
                postdata = JSON.stringify(homeResource.data);
            } else {
                postdata = homeResource.data;
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

    describe('Verification of getUserProfilePicture function', function () {
        it('It should get User ProfilePicture', function () {
    
            dm.getUserProfilePicture();
            expect(dm.smallPictureUrl).toBe("https://lcadms-my.sharepoint.com:443/User%20Photos/Profile%20Pictures/maquser_lcadms_onmicrosoft_com_SThumb.jpg");
            expect(dm.largePictureUrl).toBe("https://lcadms-my.sharepoint.com:443/User%20Photos/Profile%20Pictures/maquser_lcadms_onmicrosoft_com_MThumb.jpg");

        });
    });

    describe('Verification of help function', function () {
        it('It should set help', function () {

            dm.help();
            expect(dm.helpData.length).toBeGreaterThan(0);
            expect(dm.helpData).toBeDefined();

        });
    });


    describe('Verification of signOut function', function () {
        it('It should signOut', function () {

            dm.signOut();
            expect(dm.status).toBe(true);
        });
    });
    //signOut
    //getUserProfilePicture
    //help


   

});


