/// <disable>JS2074, JS3058</disable>
// Test suite
describe("Navigation Controller test suite", function () {

    var mockapi = function (navigationResource, callback) {
        getData(navigationResource, mocknavigationResource);
    };
    
    beforeEach(module("matterMain"));
    beforeEach(module("matterMain", function ($provide) {
        $provide.factory("api", ["matterResource", "documentResource", "documentDashBoardResource", "matterDashBoardResource", "homeResource", mockapi]);
    }));

    beforeEach(module("ui.router"));
    beforeEach(module("ui.bootstrap"));

    beforeEach(inject(function ($controller, $rootScope) {
        rootScope = $rootScope.$new();
        vm = $controller("navigationController as vm", { $scope: $scope, $state: $state, $stateParams: $stateParams, api: mockapi, $rootScope: rootScope });
    }));

    describe("Verification of canCreateMatter function", function () {
        it("It should display the navigation content", function () {
            vm.canCreateMatter();
            expect(vm.canLoginUserCreateMatter).toBe(true);
        });
    });
});


