//Test suite
describe('Home Controller service call test suite', function () {

    var mockapi = function (homeResource) {
        getData(homeResource, mockHomeResource);

    };

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("homeResource", ['$resource', 'auth', mockHomeResource]);
    }));

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("api", ['matterResource', 'documentResource', 'documentDashBoardResource', 'matterDashBoardResource', 'homeResource', mockapi]);
    }));

    beforeEach(module('ui.router'));
    beforeEach(module('ui.bootstrap'));

    beforeEach(inject(function ($controller, $rootScope) {
        rootScope = $rootScope.$new();
        vm = $controller('homeController as vm', { $scope: $scope, $state: $state, $stateParams: $stateParams, homeResource: mockHomeResource, api: mockapi, $rootScope: rootScope, $location: $location, adalAuthenticationService: adalService });
    }));

    describe('Verification of getUserProfilePicture function', function () {
        it('It should get User ProfilePicture', function () {
            vm.getUserProfilePicture();
            expect(vm.smallPictureUrl).toBe("https://lcadms-my.sharepoint.com:443/User%20Photos/Profile%20Pictures/maquser_lcadms_onmicrosoft_com_SThumb.jpg");
            expect(vm.largePictureUrl).toBe("https://lcadms-my.sharepoint.com:443/User%20Photos/Profile%20Pictures/maquser_lcadms_onmicrosoft_com_MThumb.jpg");

        });
    });

    describe('Verification of help function', function () {
        it('It should set help', function () {
            vm.help();
            expect(vm.helpData.length).toBeGreaterThan(0);
            expect(vm.helpData).toBeDefined();

        });
    });


    describe('Verification of signOut function', function () {
        it('It should signOut', function () {
            vm.signOut();
            expect(vm.status).toBe(true);
        });
    });
});


