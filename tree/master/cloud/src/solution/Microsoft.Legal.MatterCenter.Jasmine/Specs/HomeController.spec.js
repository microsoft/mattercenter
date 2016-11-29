/// <reference path="../References.js" />
//// ***********************************************************************
//// Author           : MAQ USER
//// Created          : 31-08-2016
////
//// ***********************************************************************
//// <copyright file="HomeController.spec.js" company="MAQSoftware">
////  Copyright (c) . All rights reserved.
//// </copyright>
//// <summary>Test suite for Home Controller</summary>
//// ***********************************************************************

describe("Home Controller test suite", function () {

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
});


