var vm, matterResource, $filter, $window, $watch, $http, $stateParams;
var $rootScope = { logEvent: function () { }, setAuthenticatedUserContext: function () { } };
var rootScope = { logEvent: function () { } };
var rootData = { logEvent: function () { }, setAuthenticatedUserContext: function () { }};
var $model = {};
var $label = { "assignedUser": oEnvironmentConfiguration.loggedInUserEmail };
var $item = {
    email: "",
    name: "No results found"
};
var $state = { go: function () { }, current: { "name": "" } };
var $interval = { go: function () { } };
var $animate = { enabled: function () { } };
var $q = { defer: function () { return { resolve: function () { } } } };
var $scope = { $watch: function () { }, $apply: function () { }, gridApi: { infiniteScroll: { dataLoaded: function () { }, resetScroll: function () { } }, selection: { selectAllRows: function () { }, clearSelectedRows: function () { } } } };
var $location = {
    absUrl: function () {
        var url = "https://" + oEnvironmentConfiguration.azureSiteName + ".azurewebsites.net&test=1&attempt=2|jasminetest.html";
        return url;
    }
};

var adalService = {
    "userInfo": {
        "userName": oEnvironmentConfiguration.loggedInUserEmail,
        "profile": {
            "given_name": "MAQ",
            "family_name": oEnvironmentConfiguration.tenantUrl,
            "oid": 786
        },
        "isAuthenticated": true
    },
    logOut: function () { vm.status = true; }
};

var mockapi = function () {
};

var mocknavigationResource = {
    'canCreateMatter': '/api/v1/matter/cancreate'
};

var mockMatterDashBoardResource = {
    'get': '/api/v1/matter/get',
    'getPinnedMatters': '/api/v1/matter/getpinned',
    'getMyMatters': '/api/v1/matter/getpinned',
    'getTaxonomyDetails': '/api/v1/taxonomy/gettaxonomy',
    'UnpinMatter': '/api/v1/matter/unpin',
    'PinMatter': '/api/v1/matter/pin',
    'getFolderHierarchy': '/api/v1/matter/getfolderhierarchy',
    'getMatterCounts': '/api/v1/matter/getmattercounts',
    'getDefaultMatterConfigurations': '/api/v1/matter/getconfigurations'
};

var mockMatterResource = {
    'get': '/api/v1/matter/get',
    'getPinnedMatters': '/api/v1/matter/getpinned',
    'UnpinMatters': '/api/v1/matter/unpin',
    'PinMatters': '/api/v1/matter/pin',
    'getTaxonomyDetails': '/api/v1/taxonomy/gettaxonomy',
    'checkMatterExists': '/api/v1/matter/checkmatterexists',
    'getDefaultMatterConfigurations': '/api/v1/matter/getconfigurations',
    'getUsers': '/api/v1/user/getusers',
    'getRoles': '/api/v1/user/getroles',
    'getPermissionLevels': '/api/v1/user/getpermissionlevels',
    'checkSecurityGroupExists': '/api/v1/matter/checksecuritygroupexists',
    'getFolderHierarchy': '/api/v1/matter/getfolderhierarchy',
    'createMatter': '/api/v1/matter/create',
    'assignUserPermissions': '/api/v1/matter/assignuserpermissions',
    'assignContentType': '/api/v1/matter/assigncontenttype',
    'createLandingPage': '/api/v1/matter/createlandingpage',
    'updateMatterMetadata': '/api/v1/matter/UpdateMetadata',
    'getStampedProperties': '/api/v1/matter/getstampedproperties',
    'uploadEmail': '/api/v1/document/UploadMail',
    'uploadAttachment': '/api/v1/document/UploadAttachments',
    'uploadfiles': '/api/v1/document/UploadAttachments',
    'getHelp': '/api/v1/shared/help',
    'userexists': '/api/v1/user/userexists'
};

var mockHomeResource = {
    'getHelp': '/api/v1/shared/help',
    'getUserProfilePicture': '/api/v1/user/getuserprofilepicture',
    'canCreateMatter': '/api/v1/matter/cancreate'
};

var mockDocumentDashBoardResource = {
    'get': '/api/v1/document/getdocuments',
    'getPinnedDocuments': '/api/v1/document/getpinneddocuments',
    'getMyDocuments': '/api/v1/document/getdocuments',
    'getTaxonomyDetails': '/api/v1/taxonomy/gettaxonomy',
    'unPinDocument': '/api/v1/document/unpindocument',
    'pinDocument': '/api/v1/document/pindocument',
    'downloadattachmentsasstream': '/api/v1/email/downloadattachmentsasstream',
    'downloadAttachments': '/api/v1/email/downloadattachments',
    'getDocumentCounts': '/api/v1/document/getdocumentcounts',
    'getassets': '/api/v1/document/getassets',
    'getUsers': '/api/v1/user/getusers'
};

var mockDocumentResource = {
    'get': '/api/v1/document/getdocuments',
    'getPinnedDocuments': '/api/v1/document/getpinneddocuments',
    'unPinDocument': '/api/v1/document/unpindocument',
    'pinDocument': '/api/v1/document/pindocument',
    'getassets': '/api/v1/document/getassets'
};

var mockMatterResourceService = {
    'get': '/api/v1/matter/get',
    'getPinnedMatters': '/api/v1/matter/getpinned',
    'UnpinMatters': '/api/v1/matter/unpin',
    'PinMatters': '/api/v1/matter/pin',
    'getTaxonomyDetails': '/api/v1/taxonomy/gettaxonomy',
    'checkMatterExists': '/api/v1/matter/checkmatterexists',
    'getDefaultMatterConfigurations': '/api/v1/matter/getconfigurations',
    'getUsers': '/api/v1/user/getusers',
    'getRoles': '/api/v1/user/getroles',
    'getPermissionLevels': '/api/v1/user/getpermissionlevels',
    'checkSecurityGroupExists': '/api/v1/matter/checksecuritygroupexists',
    'getFolderHierarchy': '/api/v1/matter/getfolderhierarchy',
    'createMatter': '/api/v1/matter/create',
    'assignUserPermissions': '/api/v1/matter/assignuserpermissions',
    'assignContentType': '/api/v1/matter/assigncontenttype',
    'createLandingPage': '/api/v1/matter/createlandingpage',
    'updateMatterMetadata': '/api/v1/matter/UpdateMetadata',
    'getStampedProperties': '/api/v1/matter/getstampedproperties',
    'uploadEmail': '/api/v1/document/UploadMail',
    'uploadAttachment': '/api/v1/document/UploadAttachments',
    'uploadfiles': '/api/v1/document/UploadAttachments',
    'getHelp': '/api/v1/shared/help'
};

var mockSettingsResource = {
    'getTaxonomyData': '/api/v1/taxonomy/gettaxonomy',
    'getRoles': '/api/v1/user/getroles',
    'getPermissionLevels': '/api/v1/user/getpermissionlevels',
    'getUsers': '/api/v1/user/getusers',
    'getDefaultConfigurations': '/api/v1/matter/getconfigurations',
    'saveConfigurations': '/api/v1/matter/saveconfigurations'
};

var selectedPracticeGroup = {
    "termName": "Advertising, Marketing ＆ Promotions",
    "parentTermName": "Practice Groups",
    "folderNames": "Email;Lorem;Ipsum",
    "areaTerms": [
    {
        "termName": "Advertising, Marketing ＆ Promotions",
        "parentTermName": "Advertising, Marketing ＆ Promotions",
        "folderNames": "Email;Lorem;Ipsum",
        "subareaTerms": [
        {
            "termName": "Advertising, Marketing ＆ Promotions",
            "parentTermName": "Advertising, Marketing ＆ Promotions",
            "folderNames": "Email;Lorem;Ipsum",
            "isNoFolderStructurePresent": "false",
            "documentTemplates": "Advertising, Marketing ＆ Promotions",
            "documentTemplateNames": "Agribusiness;Aircraft;California Public Utilities Commission (CPUC);Class Action Defense",
            "id": "683ec070-7ed0-4e82-b07c-13a1b4485b7b",
            "wssId": 0,
            "subareaTerms": null,
            "$$hashKey": "object:153"
        }
        ],
        "id": "16827aa4-a8b3-4275-920b-184a04bc60ea",
        "wssId": 0,
        "$$hashKey": "object:149"
    }
    ],
    "id": "a42ab615-0d27-4de2-9f55-144e71219770",
    "wssId": 0,
    "$$hashKey": "object:122"
}

var documentTemplateTypeLawTerm = {
    "areaoflaw": "Family Business",
    "areaoflawId": "584a173b-7686-427d-aadb-62b13a2fd624",
    "documentTemplateNames": "Copyright;Copyright Litigation",
    "documentTemplates": "Family Business",
    "folderNames": "Email;Lorem;Ipsum",
    "foldernamesaol": "Email;Lorem;Ipsum",
    "foldernamespg": "Email;Lorem;Ipsum",
    "id": "578cfafb-59eb-4f4c-b219-47886c61e384",
    "isNoFolderStructurePresent": "false",
    "parentTermName": "Family Business",
    "practicegroup": "Business Transactions",
    "practicegroupId": "b36c5722-b6ee-4253-902b-743d63fa80ce",
    "primaryMatterType": "true",
    "subareaTerms": "null",
    "termName": "Family Business",
    "wssId": "0"
}

var subareaTerms =
       [{
           "termName": "Advertising, Marketing ＆ Promotions",
           "parentTermName": "Advertising, Marketing ＆ Promotions",
           "folderNames": "Email;Lorem;Ipsum",
           "isNoFolderStructurePresent": "false",
           "documentTemplates": "Advertising, Marketing ＆ Promotions",
           "documentTemplateNames": "Agribusiness;Aircraft;California Public Utilities Commission (CPUC);Class Action Defense",
           "id": "578cfafb-59eb-4f4c-b219-47886c61e384",
           "primaryMatterType": "true",
           "wssId": 0,
           "subareaTerms": null,
           "$$hashKey": "object:153"
       }]

var item = {
    "email": oEnvironmentConfiguration.loggedInUserEmail,
    "entityType": "User",
    "largePictureUrl": null,
    "logOnName": "i:0#.f|membership|" + oEnvironmentConfiguration.loggedInUserEmail,
    "name": oEnvironmentConfiguration.loggedInUserName,
}

var obj = [{
    "documentName": "SharePoint Online is out of storage space.eml",
    "documentVersion": "2.0",
    "documentClient": "SubsiteClient",
    "documentClientId": "98052",
    "documentClientUrl": oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient",
    "documentMatter": "Default Matter",
    "documentMatterId": "11111",
    "documentOwner": oEnvironmentConfiguration.loggedInUserName,
    "documentUrl": oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient/6cbca4ab447c87302d3a1f0e3c32985a/Ipsum/SharePoint Online is out of storage space.eml",
    "documentOWAUrl": oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient/6cbca4ab447c87302d3a1f0e3c32985a/Ipsum/SharePoint Online is out of storage space.eml",
    "documentExtension": "eml",
    "documentCreatedDate": "5/17/2016 12:41:06 PM",
    "documentModifiedDate": "5/17/2016 12:41:07 PM",
    "documentCheckoutUser": "",
    "documentMatterUrl": oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient/6cbca4ab447c87302d3a1f0e3c32985a",
    "documentParentUrl": oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient/6cbca4ab447c87302d3a1f0e3c32985a/Ipsum",
    "documentID": "425427363",
    "checker": true,
    "documentIconUrl": oEnvironmentConfiguration.tenantUrl + "/_layouts/15/images/iceml.gif",
    "pinType": "Pin",
    "$$hashKey": "object:286",
    "selected": true
},
{
    "documentName": "SharePoint Online is out of storage space.eml",
    "documentVersion": "2.0",
    "documentClient": "SubsiteClient",
    "documentClientId": "98052",
    "documentClientUrl": oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient",
    "documentMatter": "Default Matter",
    "documentMatterId": "11111",
    "documentOwner": oEnvironmentConfiguration.loggedInUserName,
    "documentUrl": oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient/6cbca4ab447c87302d3a1f0e3c32985a/Ipsum/SharePoint Online is out of storage space.eml",
    "documentOWAUrl": oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient/6cbca4ab447c87302d3a1f0e3c32985a/Ipsum/SharePoint Online is out of storage space.eml",
    "documentExtension": "eml",
    "documentCreatedDate": "5/17/2016 12:41:06 PM",
    "documentModifiedDate": "5/17/2016 12:41:07 PM",
    "documentCheckoutUser": "",
    "documentMatterUrl": oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient/6cbca4ab447c87302d3a1f0e3c32985a",
    "documentParentUrl": oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient/6cbca4ab447c87302d3a1f0e3c32985a/Ipsum",
    "documentID": "425427363",
    "checker": true,
    "documentIconUrl": oEnvironmentConfiguration.tenantUrl + "/_layouts/15/images/iceml.gif",
    "pinType": "Pin",
    "$$hashKey": "object:286",
    "selected": true
}]

var clientobj = [{
    "id": "0016765",
    "name": "A. Datum Corporation",
    "url": oEnvironmentConfiguration.tenantUrl + "/sites/ADatumCorporation",
    "$$hashKey": "object:320",
    "Selected": true
},
	{
	    "id": "0016761",
	    "name": "AdventureWorks Cycles",
	    "url": oEnvironmentConfiguration.tenantUrl + "/sites/AdventureWorksCycles",
	    "$$hashKey": "object:321",
	    "Selected": false
	},
	{
	    "id": "0016762",
	    "name": "Alpine Ski House",
	    "url": oEnvironmentConfiguration.tenantUrl + "/sites/AlpineSkiHouse",
	    "$$hashKey": "object:322",
	    "Selected": false
	}]

var event = {
    preventDefault: function () { this.defaultPrevented = true; },
    isDefaultPrevented: function () { return this.defaultPrevented === true; },
    stopImmediatePropagation: function () { this.immediatePropagationStopped = true; },
    isImmediatePropagationStopped: function () { return this.immediatePropagationStopped === true; },
    stopPropagation: function () { },
    currentTarget: { src: "" },
    target: { "getBoundingClientRect": function () { return 1; } }
}

var $itemdata = {
    email: oEnvironmentConfiguration.loggedInUserEmail,
    name: oEnvironmentConfiguration.loggedInUserName
};

var sortColumns = [
    {
        "field": "documentName", "name": "documentName", "sort": "asc"
    }
];

var asyncResult = { status: "succeeded", value: "testtoken" };

var folder = {
    "parentURL": oEnvironmentConfiguration.tenantUrl + "/sites/subsiteclient",
    "active": true,
    "children": { "child": { "active": true } }
};

var data = {
    "DefaultMatterName": "Test",
    "DefaultMatterId": 1122,
    "IsRestrictedAccessSelected": true,
    "IsCalendarSelected": true,
    "IsRSSSelected": true,
    "IsEmailOptionSelected": true,
    "IsTaskSelected": true,
    "IsMatterDescriptionMandatory": true,
    "IsConflictCheck": true,
    "name": ""
};
var dataChunk = { "name": "" }; var testdata = {
    "DefaultMatterName": "Test",
    "DefaultMatterId": 1122,
    "IsRestrictedAccessSelected": false,
    "IsCalendarSelected": false,
    "IsRSSSelected": false,
    "IsEmailOptionSelected": false,
    "IsTaskSelected": false,
    "IsMatterDescriptionMandatory": false,
    "IsConflictCheck": false
};

var practicegroup = [
	{
	    "termName": "Advertising, Marketing ＆ Promotions",
	    "parentTermName": "Practice Groups",
	    "folderNames": "Email;Lorem;Ipsum",
	    "areaTerms": [
			{
			    "termName": "Advertising, Marketing ＆ Promotions",
			    "parentTermName": "Advertising, Marketing ＆ Promotions",
			    "folderNames": "Email;Lorem;Ipsum",
			    "subareaTerms": [
					{
					    "termName": "Advertising, Marketing ＆ Promotions",
					    "parentTermName": "Advertising, Marketing ＆ Promotions",
					    "folderNames": "Email;Lorem;Ipsum",
					    "isNoFolderStructurePresent": "false",
					    "documentTemplates": "Advertising, Marketing ＆ Promotions",
					    "documentTemplateNames": "Agribusiness;Aircraft;California Public Utilities Commission (CPUC);Class Action Defense",
					    "id": "683ec070-7ed0-4e82-b07c-13a1b4485b7b",
					    "wssId": 0,
					    "subareaTerms": null
					}
			    ],
			    "id": "16827aa4-a8b3-4275-920b-184a04bc60ea",
			    "wssId": 0,
			    "Selected": true
			}
	    ],
	    "id": "a42ab615-0d27-4de2-9f55-144e71219770",
	    "wssId": 0,
	    "$$hashKey": "object:582",
	    "Selected": true
	},
	{
	    "termName": "Bankruptcy ＆ Creditors Rights",
	    "parentTermName": "Practice Groups",
	    "folderNames": "Email;Lorem;Ipsum",
	    "areaTerms": [
			{
			    "termName": "Bankruptcy",
			    "parentTermName": "Bankruptcy ＆ Creditors Rights",
			    "folderNames": "Email;Lorem;Ipsum",
			    "subareaTerms": [
					{
					    "termName": "Personal Bankruptcy",
					    "parentTermName": "Bankruptcy",
					    "folderNames": "Email;Bankruptcy",
					    "isNoFolderStructurePresent": "false",
					    "documentTemplates": "Bankruptcy ＆ Creditors Rights",
					    "documentTemplateNames": "California Public Utilities Commission (CPUC);Class Action Defense",
					    "id": "2aa63c68-bcde-411b-bb0f-dc266de78a46",
					    "wssId": 0,
					    "subareaTerms": null
					}
			    ],
			    "id": "b6678495-0a16-424e-a94e-8a73d326e5bb",
			    "wssId": 0,
			    "Selected": false
			}
	    ],
	    "id": "1a185a92-569a-466e-8c94-e2c31083a821",
	    "wssId": 0,
	    "$$hashKey": "object:583",
	    "Selected": false
	}]

var gridrows = {
    "core": {
        getVisibleRows: function (data) { return 0; }
    }
}

function getData(objectData, resourceData) {
    var sURL = oTestConfiguration.sSiteURL + resourceData[objectData.method];
    var http = new XMLHttpRequest();
    var sPostdata;

    if (!IsJsonString(objectData.data)) {
        sPostdata = JSON.stringify(objectData.data);
    } else {
        sPostdata = objectData.data;
    }
    http.open("POST", sURL, false);
    var accessToken = "Bearer " + sessionStorage.getItem("adal.idtoken");
    // Send the proper header information along with the request
    http.setRequestHeader("Content-type", "application/json");
    http.setRequestHeader("Accept", "application/json");
    http.setRequestHeader("Authorization", accessToken);
    http.send(sPostdata);

    if (http.status === 200) {// That's HTTP for 'ok'
        if (objectData.success) {
            objectData.success(JSON.parse(http.responseText));
        }
        else {
            return JSON.parse(http.responseText);
        }
    }
}

function IsJsonString(sValue) {
    try {
        JSON.parse(sValue);
    } catch (exception) {
        return false;
    }
    return true;
}