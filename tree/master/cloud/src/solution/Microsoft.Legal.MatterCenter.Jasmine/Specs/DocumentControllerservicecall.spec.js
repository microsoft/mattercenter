//Test suite
describe('documents Controller test suite', function () {
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
    var $location = {
        absUrl: function () {
            var url = "https://mattermaqdevsite.azurewebsites.net&test=1&attempt=2|jasminetest.html";
            return url;
        }
    };
    var $q = {};
    $q.defer = function () { return { resolve: function () { } } };
    $scope.$apply = function () { };

    var mockdocumentResource = {
        'get': '/api/v1/document/getdocuments',
        'getPinnedDocuments': '/api/v1/document/getpinneddocuments',
        'unPinDocument': '/api/v1/document/unpindocument',
        'pinDocument': '/api/v1/document/pindocument',
        'getassets': '/api/v1/document/getassets'
    };

    var documentapi = function (documentDashBoardResource) {
        var mockdocumentResource = {
            'get': '/api/v1/document/getdocuments',
            'getPinnedDocuments': '/api/v1/document/getpinneddocuments',
            'unPinDocument': '/api/v1/document/unpindocument',
            'pinDocument': '/api/v1/document/pindocument',
            'getassets': '/api/v1/document/getassets'
        };
        var url = "http://mattermaqdevsite.azurewebsites.net" + mockdocumentResource[documentDashBoardResource.method];
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
        $provide.factory("documentResource", ['$resource', 'auth', mockdocumentResource]);
    }));

    beforeEach(module('matterMain'));
    beforeEach(module('matterMain', function ($provide) {
        $provide.factory("api", ['matterResource', 'documentResource', 'documentDashBoardResource', 'matterDashBoardResource', 'homeResource', documentapi]);
    }));

    beforeEach(module('ui.router'));
    beforeEach(module('ui.bootstrap'));

    beforeEach(inject(function ($controller, $rootScope) {
        rootScope = $rootScope.$new();
        dm = $controller('documentsController as dm', { $scope: $scope, $state: $state, $stateParams: $stateParams, documentResource: mockdocumentResource, api: documentapi, $rootScope: rootScope, $http: $http, $location: $location, $q: $q });
    }));

    describe('Verification of watchFunc function', function () {
        it('It should get all grid data', function () {
            $scope.gridApi = { infiniteScroll: { dataLoaded: function () { } } };
            // $scope.gridApi.infiniteScroll.dataLoaded();
            var promise = dm.watchFunc();
            expect(dm.gridOptions.data.length).toBeGreaterThan(0);
            expect(dm.gridOptions.data).not.toBe(null);
            expect(dm.lazyloader).toBe(true);
        });

    });

    describe('Verification of documentsearch function', function () {
        it('It should search related document', function () {
            var term = "FileName:Test*(* OR FileName:* OR dlcDocIdOWSText:* OR MCDocumentClientName:*)";
            var property = "FileName";
            $scope.gridApi = { infiniteScroll: { dataLoaded: function () { } } };
            dm.documentsearch(term, property, false);
            expect(dm.divuigrid).toBe(true);
            expect(dm.nodata).toBe(false);
            expect(dm.lazyloader).toBe(true);
            expect(dm.filternodata).toBe(false);
            expect(dm.details.length).toBeGreaterThan(0);
            expect(dm.details).not.toBe(null);
            debugger;;
            //dm.documentsearch(term, property, true);

        });

    });

    describe('Verification of FilterModifiedDate function', function () {
        it('Data should be filtered based on modified date', function () {
            dm.modstartdate = new Date("08/01/2016");
            dm.modenddate = new Date("08/10/2016");
            dm.FilterModifiedDate("Modified Date");
            expect(dm.moddatefilter).toBe(true);
        });

    });

    describe('Verification of SetDocuments function', function () {
        it('document name should be added in dropdown', function () {
            $scope.gridApi = { infiniteScroll: { dataLoaded: function () { } } };
            dm.SetDocuments(1, "All Documents");
            expect(true).toBe(true);
            expect(dm.divuigrid).toBe(true);
            expect(dm.responseNull).toBe(false);
            expect(dm.nodata).toBe(false);
            expect(dm.gridOptions.data.length).toBeGreaterThan(0);
            expect(dm.gridOptions.data).not.toBe(null);
        });
    });

    describe('Verification of GetDocuments function', function () {
        it('document name should be added in dropdown', function () {
            $scope.gridApi = { infiniteScroll: { dataLoaded: function () { } } };
            dm.GetDocuments(3);
            expect(true).toBe(true);
            expect(dm.divuigrid).toBe(true);
            expect(dm.nodata).toBe(false);
            expect(dm.gridOptions.data.length).toBeGreaterThan(0);
            expect(dm.gridOptions.data).not.toBe(null);
        });
    });

    describe('Verification of PinMatter function', function () {
        it('It should be added in pinned list', function () {
            var pinObject = {
                entity: {
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
                }
            };

            dm.PinMatter(pinObject);
            dm.documentid = 1;
            dm.documentname = "All Documents";
            expect(dm.divuigrid).toBe(true);
            expect(dm.responseNull).toBe(false);
            expect(dm.nodata).toBe(false);
            expect(dm.gridOptions.data.length).toBeGreaterThan(0);
            expect(dm.gridOptions.data).not.toBe(null);

        });
    });

    describe('Verification of UnpinDocument function', function () {
        it('It should be removed from pinned list', function () {
            var pinObject = {
                entity: {
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
                }
            };

            dm.UnpinDocument(pinObject);
            dm.documentid = 1;
            dm.documentname = "All Documents";
            expect(dm.divuigrid).toBe(true);
            expect(dm.responseNull).toBe(false);
            expect(dm.nodata).toBe(false);
            expect(dm.gridOptions.data.length).toBeGreaterThan(0);
            expect(dm.gridOptions.data).not.toBe(null);

        });
    });

    describe('Verification of FilterByType function', function () {
        it('document count should be greater than 0', function () {
            dm.FilterByType();
            expect(dm.divuigrid).toBe(true);
            expect(dm.nodata).toBe(false);
            expect(dm.gridOptions.data.length).toBeGreaterThan(0);
            expect(dm.gridOptions.data).not.toBe(null);
        });
    });

    describe('Verification of sortChangedDocument function', function () {
        it('documents should be sort based on document name', function () {
            $scope.gridApi = { infiniteScroll: { dataLoaded: function () { }, resetScroll: function () { } } };
            var sortColumns = [{ "field": "documentName", "name": "documentName", "sort": "asc" }];
            dm.gridOptions.columnDefs[1] = { "field": "documentName", "displayName": "Document", "width": "278", "enableHiding": false, "cellTemplate": "../app/document/DocumentTemplates/DocumentCellTemplate.html", "headerCellTemplate": "../app/document/DocumentTemplates/DocumentHeaderTemplate.html", "name": "documentName", "type": "string" };
            $scope.sortChangedDocument(null,sortColumns);
            expect(dm.FileNameSort).toBe("desc");
            expect(dm.sortby).toBe("asc");
            expect(dm.sortexp).toBe("documentName");
                     
        });
    });

    describe('Verification of typeheadselect function', function () {
        it('selected document result should be displayed', function () {
            $scope.gridApi = { infiniteScroll: { dataLoaded: function () { }, resetScroll: function () { } } };
            var selected = "All Attachments - image test.eml (280620050)";
            dm.typeheadselect(null, selected);
            expect(selected).toContain(dm.gridOptions.data[0].documentName);
        });
    });

});