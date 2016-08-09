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

    var mockmatterResourceService = function ($resource, auth) {
        return $resource(null, null,
                {
                    'get': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/matter/get',
                        isArray: true
                    }),
                    'getPinnedMatters': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/matter/getpinned',
                        isArray: true
                    }),
                    'UnpinMatters': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/matter/unpin'
                    }),
                    'PinMatters': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/matter/pin'
                    }),
                    'getTaxonomyDetails': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/taxonomy/gettaxonomy'
                    }),
                    'checkMatterExists': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/matter/checkmatterexists'
                    }),
                    'getDefaultMatterConfigurations': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/matter/getconfigurations'
                    }),
                    'getUsers': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/user/getusers',
                        isArray: true
                    }),
                    'getRoles': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/user/getroles',
                        isArray: true
                    }),
                    'getPermissionLevels': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/user/getpermissionlevels',
                        isArray: true
                    }),
                    'checkSecurityGroupExists': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/matter/checksecuritygroupexists'

                    }),
                    'getFolderHierarchy': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/matter/getfolderhierarchy'

                    }),
                    'createMatter': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/matter/create'

                    }),
                    'assignUserPermissions': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/matter/assignuserpermissions'

                    }),
                    'assignContentType': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/matter/assigncontenttype'

                    }),
                    'createLandingPage': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/matter/createlandingpage'
                    }),
                    'updateMatterMetadata': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/matter/UpdateMetadata'
                    }),
                    'getStampedProperties': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/matter/getstampedproperties'
                    }),
                    'uploadEmail': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/document/UploadMail'
                    }),
                    'uploadAttachment': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/document/UploadAttachments'
                    }),
                    'uploadfiles': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/document/UploadAttachments'
                    }),
                    'getHelp': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/shared/help',
                        isArray: true
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
                  // $scope.$parent.vm.Error = true;
                  //vm.Error = true;
                  var message = 'oops something went wrong. ';
                  if (e.data != null)
                      if (e.data.Message != undefined)
                          message = e.data.Message;
                      else
                          message = e.data;
                  else
                      message = e.message + e.description + e.stack + e.number;

                  // alert(message);
                  //$scope.$parent.vm.guid = "kdsjfl";
                  // vm.guid = "kdsjfl";

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

    describe('Verification of clearpopup function', function () {
        it('errorPopUpBlock should be set to false', function () {
            vm.clearPopUp();
            var val = vm.errorPopUpBlock;
            expect(val).toBe(false);
        });

        it('errorPopUpBlock should not be set', function () {
            var val = vm.errorPopUpBlock;
            expect(val).toBeUndefined();
        });
    });

    describe('Verification of getMatterGUID function', function () {
        it('Should return the GUID for matter with length of 32 bit', function () {

            var val = vm.matterGUID;
            expect(val.length).toBe(32);
        });

        it('Should return the GUID for matter instead of null value', function () {

            var val = vm.matterGUID;
            expect(val).not.toBe(null);
        });

        it('Should return the GUID for matter with defined value', function () {

            var val = vm.matterGUID;
            expect(val).toBeDefined();
        });
    });

    describe('Verification of selectMatterTypePopUpClose function', function () {
        it('Should return the status of popupContainerBackground and popupContainer as hide', function () {
            vm.popupContainer = "Show";
            vm.selectMatterTypePopUpClose();
            var value = vm.popupContainerBackground;
            var data = vm.popupContainer;
            expect(value).toBe("hide");
            expect(data).toBe("hide");
        });

        it('Should not return the status of popupContainerBackground and popupContainer as hide', function () {
            vm.popupContainer = "hide";
            vm.selectMatterTypePopUpClose();
            var value = vm.popupContainerBackground;
            var data = vm.popupContainer;
            expect(value).toBe("Show");
            expect(data).toBe("hide");
        });
    });

    describe('Verification of getSelectedPracticeGroupValue function', function () {
        it('Should return the selected PracticeGroup Value (all AOL and SAOL terms)', function () {
            vm.selectedPracticeGroup = selectedPracticeGroup;
            vm.getSelectedPracticeGroupValue();

                expect(vm.areaOfLawTerms).not.toBe(null);
                expect(vm.subAreaOfLawTerms).not.toBe(null);
                expect(vm.activeSubAOLTerm.termName).toBe("Advertising, Marketing ＆Promotions");
                expect(vm.activeAOLTerm.folderNames).toBe("Email;Lorem;Ipsum");
                expect(vm.errorPopUp).toBe(false);

        });

        it('Should return the null value for selected PracticeGroup Value (all AOL and SAOL terms)', function () {
            vm.selectedPracticeGroup = null;
            vm.getSelectedPracticeGroupValue();

                expect(vm.areaOfLawTerms).toBe(null);
                expect(vm.subAreaOfLawTerms).toBe(null);
        });

    });

    describe('Verification of selectAreaOfLawTerm function', function () {
        it('Should return the subAOL items on selection of AOLTerm', function () {
            vm.selectAreaOfLawTerm(selectedPracticeGroup.areaTerms[0]);

            expect(vm.subAreaOfLawTerms).not.toBe(null);
            expect(vm.activeSubAOLTerm.termName).toBe("Advertising, Marketing ＆Promotions");
            expect(vm.errorPopUp).toBe(false);
            expect(vm.activeAOLTerm).toBe(selectedPracticeGroup.areaTerms[0]);
        });

        it('Should return defined subAOL items on selection of AOLTerm', function () {
            vm.selectAreaOfLawTerm(selectedPracticeGroup.areaTerms[0]);

            expect(vm.subAreaOfLawTerms).not.toBeUndefined();
            expect(vm.activeSubAOLTerm).not.toBeUndefined();
            expect(vm.errorPopUp).not.toBeUndefined();
            expect(vm.activeAOLTerm).not.toBeUndefined();
        });
    });

    describe('Verification of selectSubAreaOfLawTerm function', function () {
        it('Should return the subAOL items', function () {

            vm.selectSubAreaOfLawTerm(selectedPracticeGroup.areaTerms[0].subareaTerms[0]);
            expect(vm.activeSubAOLTerm).toBe(selectedPracticeGroup.areaTerms[0].subareaTerms[0]);
            expect(vm.errorPopUp).toBe(false);

        });

        it('Should return defined subAOL items', function () {

            vm.selectSubAreaOfLawTerm(selectedPracticeGroup.areaTerms[0].subareaTerms[0]);
            expect(vm.activeSubAOLTerm).not.toBeUndefined();
            expect(vm.errorPopUp).not.toBeUndefined();

        });
    });

    describe('Verification of selectDocumentTemplateTypeLawTerm function', function () {
        it('Should return the document template type term', function () {

            vm.selectDocumentTemplateTypeLawTerm(documentTemplateTypeLawTerm);

            expect(vm.removeDTItem).toBe(true);
            expect(vm.errorPopUp).toBe(false);
            expect(vm.activeDocumentTypeLawTerm).toBe(documentTemplateTypeLawTerm);
            expect(vm.primaryMatterType).toBe(true);

        });

        it('Should return expected document template type term', function () {

            vm.selectDocumentTemplateTypeLawTerm(documentTemplateTypeLawTerm);

            expect(vm.removeDTItem).not.toBe(false);
            expect(vm.errorPopUp).not.toBe(true);
            expect(vm.activeDocumentTypeLawTerm).not.toBe(null);
            expect(vm.primaryMatterType).not.toBe(false);

        });
    });

    describe('Verification of addToDocumentTemplate function', function () {
        it('Should return the document template type law terms', function () {
            vm.activeSubAOLTerm = documentTemplateTypeLawTerm;
            vm.documentTypeLawTerms = subareaTerms;
            vm.addToDocumentTemplate();
            expect(vm.documentTypeLawTerms).not.toBeUndefined();
        });

        it('Should return activeDocumentTypeLawTerm as null', function () {
            vm.activeSubAOLTerm = documentTemplateTypeLawTerm;
            vm.documentTypeLawTerms = subareaTerms;
            vm.addToDocumentTemplate();
            expect(vm.activeDocumentTypeLawTerm).not.toBe("null");
        });

    });

    describe('Verification of removeFromDocumentTemplate function', function () {
        it('Should not return document template type law terms while removing', function () {
            vm.removeDTItem = false;
            vm.removeFromDocumentTemplate();
            expect(vm.removeDTItem).not.toBe(true);
            expect(vm.primaryMatterType).not.toBe(true);
        });

        it('Should return the document template type law terms while removing', function () {
            vm.removeDTItem = true;
            vm.removeFromDocumentTemplate();
            expect(vm.removeDTItem).toBe(false);
            expect(vm.primaryMatterType).toBe(false);
        });
    });

    describe('Verification of onSelect function', function () {

        //it('Should return the conflicted ensured user', function () {
        //    vm.removeDTItem = false;
        //    vm.onSelect(item, "MAQ User", "MAQ User", "conflictcheckuser");
        //    expect(vm.selectedConflictCheckUser).toBe("MAQ User(MAQUser@LCADMS.onmicrosoft.com)");

        //});

        //it('Should return the blocked ensured user', function () {
        //    vm.removeDTItem = false;
        //    vm.onSelect(item, "MAQ User", "MAQ User", "blockuser");
        //    expect(vm.blockedUserName).toBe("MAQ User(MAQUser@LCADMS.onmicrosoft.com)");

        //});

        it('Should return the conflicted user', function () {
            $model = {};
            $label = { "assignedUser": "" };
            vm.onSelect(item, $model, $label, "conflictcheckuser", "on-blurr", event, item.name);
            expect(vm.oSiteUsers).toBeDefined();
            expect(vm.selectedConflictCheckUser).toBe("MAQ User(MAQUser@LCADMS.onmicrosoft.com)");
        });

        it('Should return the blocked user', function () {
            $model = {};
            $label = { "assignedUser": "" };
            vm.onSelect(item, $model, $label, "blockuser", "on-blurr", event, item.name);
            expect(vm.oSiteUsers).toBeDefined();
            expect(vm.blockedUserName).toBe("MAQ User(MAQUser@LCADMS.onmicrosoft.com)");
        });

        it('Should return the team member', function () {
            $model = {};
            $label = { "assignedUser": "" };
            vm.onSelect(item, $model, $label, "team", "on-blurr", event, item.name);
            expect(vm.oSiteUsers).toBeDefined();
            expect(vm.typehead).toBe(false);
            expect(vm.notificationPopUpBlock).toBe(false);
            expect($label.assignedUser).toBe("MAQ User(MAQUser@LCADMS.onmicrosoft.com)");
        });

        it('Should return the assigned user', function () {
            $item = {
                "email": "",
                "name": "No results found"
            };
            $model = {};
            $label = { "assignedUser": "" };
            vm.onSelect($item, $model, $label, "team", "on-blurr", event, item.name);
            expect(vm.user).toBe("MAQ User");
        });

    });

    describe('Verification of saveDocumentTemplates function', function () {
        it('Should return the saved document Templates', function () {
            vm.primaryMatterType = true;
            vm.activeDocumentTypeLawTerm = documentTemplateTypeLawTerm;
            vm.documentTypeLawTerms = subareaTerms;
            vm.saveDocumentTemplates();
            expect(vm.selectedDocumentTypeLawTerms).toBe(vm.documentTypeLawTerms);
            expect(vm.popupContainerBackground).toBe("hide");
            expect(vm.popupContainer).toBe("hide");

        });

        it('Should not return saved document Templates and error popup should be prompt', function () {
            vm.primaryMatterType = false;
            vm.saveDocumentTemplates();
            expect(vm.errorPopUp).toBe(true);
        });;

    });

    describe('Verification of open1 function', function () {
        it('Should return the status of date picker', function () {
            vm.open1();
            expect(vm.opened).toBe(true);
        });
    });


    describe('Verification of conflictRadioChange function', function () {
        it('Should return the status of secureMatterRadioEnabled as true', function () {
            vm.conflictRadioChange(true);
            expect(vm.secureMatterRadioEnabled).toBe(true);
            expect(vm.secureMatterCheck).toBe(true);

        });

        it('Should return the status of secureMatterRadioEnabled as false', function () {
            vm.conflictRadioChange(false);
            expect(vm.secureMatterRadioEnabled).toBe(false);

        });
    });

    describe('Verification of addNewAssignPermissions function', function () {
        it('Should return the assignPermissionTeams data', function () {
            vm.addNewAssignPermissions();
            expect(vm.assignPermissionTeams).not.toBeUndefined();

        });
    });

    describe('Verification of removeAssignPermissionsRow function', function () {
        it('Should return the remaining users', function () {
            vm.assignPermissionTeams.length = 3;
            vm.removeAssignPermissionsRow(0);
            var rows = vm.assignPermissionTeams.length;
            expect(rows).toBe(2);
        });

        it('Should return return no result', function () {
            vm.assignPermissionTeams.length = 0;
            vm.removeAssignPermissionsRow(0);
            expect(vm.assignPermissionTeams.length).toBe(0);
        });

    });

    describe('Verification of createAndNotify function', function () {
        it('Should display the Create and notify', function () {
            vm.createAndNotify(true);
            var buttonname = vm.createButton;
            expect(buttonname).toBe("Create and Notify");
        });

        it('Should display the Create', function () {
            vm.createAndNotify(false);
            var buttonname = vm.createButton;
            expect(buttonname).toBe("Create");
        });

    });


    //describe('Verification of NextClick /PreviousClick function', function () {
    //    it('navigateToSecondSection makes serivice call hence it need to be verify later', function () {
    //        var tape = jasmine.createSpyObj('tape', ['navigateToSecondSection']);
    //        expect(tape.navigateToSecondSection).toThrow(e);
    //        vm.iCurrentPage = 1;
    //        vm.NextClick();
            
    //    });
    //});

    describe('Verification of CheckPopUp function', function () {
        
        it('Should display the CheckPopUp', function () {
            vm.CheckPopUp(true);
            expect(vm.errorPopUpBlock).toBe(false);
            expect(vm.errorBorder).toBe("");
        });

    });
    
    describe('Verification of closesuccessbanner function', function () {

        it('Should set the successbanner as false', function () {
            vm.closesuccessbanner();
            expect(vm.successMsg).toBe("");
            expect(vm.successBanner).toBe(false);
        });
    });

});
