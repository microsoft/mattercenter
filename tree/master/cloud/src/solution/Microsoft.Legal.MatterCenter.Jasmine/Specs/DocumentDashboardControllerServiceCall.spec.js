//Test suite
describe('DocumentDashboard Controller test suite', function () {
    var $scope = {};
    var dm;
    var api;
    var matterResource;
    var rootScope = {};
    var $state = { go: function () { } };
    var $stateParams;
    var $interval = { go: function () { } };
    var $watch;
    var $http;
    var $location;

    var mockdocumentDashBoardResource = {
        'get': '/api/v1/document/getdocuments',
        'getPinnedDocuments': '/api/v1/document/getpinneddocuments',
        'getMyDocuments': '/api/v1/document/getdocuments',
        'getTaxonomyDetails': '/api/v1/taxonomy/gettaxonomy',
        'unPinDocument': '/api/v1/document/unpindocument',
        'pinDocument': '/api/v1/document/pindocument',
        'downloadattachmentsasstream': '/api/v1/email/downloadattachmentsasstream',
        'downloadAttachments': '/api/v1/email/downloadattachments',
        'getDocumentCounts': '/api/v1/document/getdocumentcounts'
    };

    var mockDocumentapi = function (documentDashBoardResource) {
        var mockdocumentDashBoardResource = {
            'get': '/api/v1/document/getdocuments',
            'getPinnedDocuments': '/api/v1/document/getpinneddocuments',
            'getMyDocuments': '/api/v1/document/getdocuments',
            'getTaxonomyDetails': '/api/v1/taxonomy/gettaxonomy',
            'unPinDocument': '/api/v1/document/unpindocument',
            'pinDocument': '/api/v1/document/pindocument',
            'downloadattachmentsasstream': '/api/v1/email/downloadattachmentsasstream',
            'downloadAttachments': '/api/v1/email/downloadattachments',
            'getDocumentCounts': '/api/v1/document/getdocumentcounts'
        };
        var url = "http://mattermaqdevsite.azurewebsites.net" + mockdocumentDashBoardResource[documentDashBoardResource.method];
        function IsJsonString(str) {
            try {
                JSON.parse(str);
            } catch (e) {
                return false;
            }
            return true;
        }
        callAPI(documentDashBoardResource.success);
        function callAPI(callback) {

            var http = new XMLHttpRequest();
            var postdata;

            if (!IsJsonString(documentDashBoardResource.data)) {
                postdata = JSON.stringify(documentDashBoardResource.data);
            } else {
                postdata = documentDashBoardResource.data;
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
        $provide.factory("documentDashBoardResource", ['$resource', 'auth', mockdocumentDashBoardResource]);
    }));

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("api", ['matterResource', 'documentResource', 'documentDashBoardResource', 'matterDashBoardResource', 'homeResource', mockDocumentapi]);
    }));

    beforeEach(module('ui.router'));
    beforeEach(module('ui.bootstrap'));

    beforeEach(inject(function ($controller, $injector, $rootScope) {
        rootScope = $rootScope.$new();
        dm = $controller('DocumentDashBoardController as dm', { $scope: $scope, $state: $state, $stateParams: $stateParams, documentDashBoardResource: mockdocumentDashBoardResource, api: mockDocumentapi, $rootScope: rootScope, $http: $http, $location: $location });
    }));

    //describe('Verification of getDocumentCounts function', function () {
    //    it('Documents count should be greater than or equal to 0', function () {
    //        //var documentRequest = { "Client": { "Url": "https://lcadms.sharepoint.com/sites/catalog" }, "SearchObject": { "PageNumber": 1, "ItemsPerPage": 28, "SearchTerm": "", "Filters": { "ClientsList": [], "FromDate": "", "ToDate": "", "DocumentAuthor": "", "FilterByMe": 0 }, "Sort": { "ByProperty": "LastModifiedTime", "Direction": 1 } } };
    //        $scope.$apply = function () { };
    //        dm.getDocumentCounts();
    //        expect(dm.allDocumentCount).not.toBeLessThan(0);
    //        expect(dm.myDocumentCount).not.toBeLessThan(0);
    //        expect(dm.pinDocumentCount).not.toBeLessThan(0);
    //        expect(dm.totalrecords).not.toBeLessThan(0);
    //    });
    //});

    //describe('Verification of getPinnedDocuments function', function () {
    //    it('It should return the all pinned documents', function () {
    //        dm.getPinnedDocuments();
    //        expect(dm.documentGridOptions.data).not.toBe(null);
    //        expect(dm.documentGridOptions.data.length).toBeGreaterThan(1);
    //        expect(dm.totalrecords).toBe(0);
    //        expect(dm.lazyloaderdashboard).toBe(true);
    //        expect(dm.divuigrid).toBe(true);
    //    });
    //});

    //describe('Verification of getMyDocuments function', function () {
    //    it('It should return the My documents', function () {
    //        dm.getMyDocuments();
    //        expect(dm.lazyloaderdashboard).toBe(false);
    //        expect(dm.displaypagination).toBe(false);
    //        expect(dm.divuigrid).toBe(false);
    //        expect(dm.nodata).toBe(false);
    //    });
    //});

    //describe('Verification of getDocuments function', function () {
    //    it('It should get the Documents', function () {
    //        dm.getDocuments();
    //        expect(dm.lazyloaderdashboard).toBe(true);
    //        expect(dm.displaypagination).toBe(false);
    //        expect(dm.divuigrid).toBe(true);
    //        expect(dm.nodata).toBe(false);
    //        expect(dm.pinDocumentCount).toBeGreaterThan(0);
    //        expect(dm.documentGridOptions.data.length).toBeGreaterThan(1);
    //        expect(dm.totalrecords).toBe(0);
    //        expect(dm.pinDocumentCount).toBeGreaterThan(0);
    //        expect(dm.nodata).toBe(false);

    //    });
    //});

    //describe('Verification of showclientdrop function', function () {
    //    it('It should show clientdrop', function () {
    //        dm.clientdropvisible = false;
    //        dm.clients = undefined;
    //        dm.showclientdrop(event);
    //        expect(dm.clients).toBeDefined();
    //        expect(dm.clientdrop).toBe(true);
    //        expect(dm.clientdropvisible).toBe(true);
    //        expect(dm.lazyloaderdocumentclient).toBe(true);

    //    });

    //    it('It should not show clientdrop', function () {
    //        dm.clientdropvisible = true;
    //        dm.showclientdrop(event);
    //        expect(dm.clientdrop).toBe(false);
    //        expect(dm.clientdropvisible).toBe(false);
    //        expect(dm.lazyloaderdocumentclient).toBe(true);

    //    });
    //});


    //describe('Verification of FilterByType function', function () {
    //    it('It should show FilterByType', function () {
    //        dm.FilterByType();
    //        expect(dm.totalrecords).toBeGreaterThan(0);
    //        expect(dm.documentGridOptions.data.length).toBeGreaterThan(0);
    //        expect(dm.lazyloader).toBe(true);
    //        expect(dm.nodata).toBe(false);
    //        expect(dm.divuigrid).toBe(true);
    //        expect(dm.lazyloaderdashboard).toBe(true);
    //    });
    //});

    //describe('Verification of sortyby function', function () {
    //    it('It should show sortyby', function () {
    //        var sortexp = "AlphabeticalUp";
    //        dm.sortyby(sortexp,"Searchkeyword");
    //        expect(dm.totalrecords).toBeGreaterThan(0);
    //        expect(dm.documentGridOptions.data.length).toBeGreaterThan(0);
    //        expect(dm.lazyloader).toBe(true);
    //        expect(dm.nodata).toBe(false);
    //        expect(dm.divuigrid).toBe(true);
    //        expect(dm.lazyloaderdashboard).toBe(true);
    //    });
    //});

    //describe('Verification of next function', function () {
    //    it('It should show next', function () {
    //        dm.last = 5;
    //        dm.totalrecords = 30;
    //        dm.next();
    //        expect(dm.first).toBeGreaterThan(0);
    //        expect(dm.last).toBeGreaterThan(0);
    //        expect(dm.total).toBeGreaterThan(0);
    //        expect(dm.pagenumber).toBeGreaterThan(0);
    //        expect(dm.fromtopage).toBe(dm.first + " - " + dm.totalrecords);
    //        expect(dm.lazyloader).toBe(true);
    //        expect(dm.documentGridOptions.data.length).toBeGreaterThan(0);
    //        expect(dm.nodata).toBe(false);
    //        expect(dm.divuigrid).toBe(true);
    //        expect(dm.lazyloaderdashboard).toBe(true);
    //    });
    //});

    //describe('Verification of prev function', function () {
    //    it('It should show prev', function () {
    //        dm.last = 50;
    //        dm.first = 50;
    //        dm.prev();
    //        expect(dm.first).toBeGreaterThan(0);
    //        expect(dm.last).toBeGreaterThan(0);
    //        expect(dm.pagenumber).toBe(0);
    //        expect(dm.documentGridOptions.data.length).toBeGreaterThan(0);
    //        expect(dm.fromtopage).toBe(dm.first + " - " + dm.last);
    //        expect(dm.nodata).toBe(false);
    //        expect(dm.divuigrid).toBe(true);
    //        expect(dm.lazyloaderdashboard).toBe(true);
    //    });
    //});

    //describe('Verification of downloadEmailAsAttachment function', function () {
    //    it('It should download email with attachment', function () {
    //        dm.cartelements = [{ documentClientUrl: "https://lcadms.sharepoint.com/sites/skype", selected: true, documentName: "Skype Litigation.txt", documentUrl: "https://lcadms.sharepoint.com/sites/skype/9694be04f87b22e8d238d69df9e29ff2/Skype Litigation.txt" }];
    //        dm.downloadEmailAsAttachment(true);

    //    });
    //});

    describe('Verification of pinorunpin function', function () {
        it('It should be added in pinned list and removed from pinned list', function () {
            var pinObject = {
                documentCheckoutUser: "MAQ User",
                documentClient: "Awesome Computers",
                documentClientId: "0016763",
                documentClientUrl: "https://lcadms.sharepoint.com/sites/awesomecomputers",
                documentCreatedDate: "2/11/2016 10:55:43 AM",
                documentExtension: "docx",
                documentIconUrl: "https://lcadms.sharepoint.com/_layouts/15/images/icdocx.gif",
                documentID: "17592407627949",
                documentMatter: "Test GitHub build",
                documentMatterId: "TGB001",
                documentMatterUrl: "https://lcadms.sharepoint.com/sites/AwesomeComputers/727f57900ca2b473b99b25f9e36506d0/Forms",
                documentModifiedDate: "2/8/2016 1:06:00 PM",
                documentName: "Test Document.docx",
                documentOWAUrl: "https://lcadms.sharepoint.com/sites/AwesomeComputers/_layouts/15/WopiFrame.aspx?sourcedoc={ADF55595-76D7-4131-8FE9-807989BA166D}&file=Test%20Document.docx&action=default&DefaultItemOpen=1",
                documentOwner: "MAQ User",
                documentParentUrl: "https://lcadms.sharepoint.com/sites/AwesomeComputers/727f57900ca2b473b99b25f9e36506d0/Forms/AllItems.aspx",
                documentUrl: "https://lcadms.sharepoint.com/sites/AwesomeComputers/_layouts/15/WopiFrame.aspx?sourcedoc={ADF55595-76D7-4131-8FE9-807989BA166D}&file=Test%20Document.docx&action=default&DefaultItemOpen=1",
                documentVersion: "2.0",
                pinType: "Pin"
            };
            var count = dm.pinDocumentCount;
            var event = {};
            event.currentTarget = {};
            event.currentTarget.src = "../images/pin-666.png";
            
            debugger;
            dm.pinorunpin(event, pinObject);
            count = count + 1;
            expect(count).toBe(dm.pinDocumentCount);
            event.currentTarget.src = "../images/unpin-666.png";
            count = count - 1;
            dm.documentGridOptions.data = [];
            dm.documentGridOptions.data[0] = pinObject;
            dm.pinorunpin(event, pinObject);
            expect(count).toBe(dm.pinDocumentCount);
        });
    });


});

//downloadEmailAsAttachment----
//getDocumentCounts
//getDocuments
//getPinnedDocuments
//getMyDocuments
//pinorunpin---
//showclientdrop
//FilterByType
//sortyby
//next
//prev
