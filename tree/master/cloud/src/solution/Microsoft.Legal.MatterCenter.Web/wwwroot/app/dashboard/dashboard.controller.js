(function () {
    'use strict;'
    var app = angular.module("matterMain");
    app.controller('DashBoardController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'dashBoardResource', '$rootScope', 'uiGridConstants', '$location', '$http',
        function matterDashBoard($scope, $state, $interval, $stateParams, api, $timeout, dashBoardResource, $rootScope, uiGridConstants, $location, $http) {      	
    	    var vm = this;        
    	    var gridOptions = {
    		    paginationPageSize: 10,
    		    enableGridMenu: false,
    		    enableRowHeaderSelection: false,
    		    enableRowSelection: true,
    		    enableSelectAll: false,
    		    multiSelect: false,
    		    enableColumnMenus: false,
    		    enableFiltering: false
    	    }
        
    	    //#region Matter Grid Options
    	    vm.matterGridOptions = {
    		    paginationPageSize: gridOptions.paginationPageSize,
    		    enableGridMenu: gridOptions.enableGridMenu,
    		    enableRowHeaderSelection: gridOptions.enableRowHeaderSelection,
    		    enableRowSelection: gridOptions.enableRowSelection,
    		    enableSelectAll: gridOptions.enableSelectAll,
    		    multiSelect: gridOptions.multiSelect,
    		    enableFiltering : gridOptions.enableFiltering,
    		    columnDefs: [
                    { field: 'matterName', displayName: 'Matter', cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.matterModifiedDate}}"></div>'},
                    { field: 'matterClient', displayName: 'Client', cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.matterModifiedDate}}"></div>'},
                    { field: 'matterClientId', displayName: 'Client.MatterID', headerTooltip: 'Click to sort by client.matterid', enableCellEdit: true, },
                    { field: 'matterModifiedDate', displayName: 'Modified Date', cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.matterModifiedDate}}"></div>'},
                    { field: 'matterResponsibleAttorney', headerTooltip: 'Click to sort by attorney', displayName: 'Responsible attorney', visible: false },
                    { field: 'matterSubAreaOfLaw', headerTooltip: 'Click to sort by sub area of law', displayName: 'Sub area of law', visible: false },
                    { field: 'matterCreatedDate', headerTooltip: 'Click to sort by matter open date', displayName: 'Open date'},
    		    ],
    	    }
    	    //#endregion

    	    //#region Document Grid Options
    	    vm.documentGridOptions = {
    	        paginationPageSize: gridOptions.paginationPageSize,
                enablePagination:false,
    		    enableGridMenu: gridOptions.enableGridMenu,
    		    enableRowHeaderSelection: gridOptions.enableRowHeaderSelection,
    		    enableRowSelection: gridOptions.enableRowSelection,
    		    enableSelectAll: gridOptions.enableSelectAll,
    		    multiSelect: gridOptions.multiSelect,
    		    enableFiltering : gridOptions.enableFiltering,
    		    columnDefs: [
                    { field: 'checker', displayName: 'checked', cellTemplate: '/app/dashboard/cellCheckboxTemplate.html', headerCellTemplate: '/app/dashboard/headerCheckboxTemplate.html', enableColumnMenu:false },
                    { field: 'documentIconUrl', displayName: 'Icon', cellTemplate: '<div class="ui-grid-cell-contents"><img src="{{row.entity.documentIconUrl}}"/></div>', headerCellTemplate: '<div class="ui-grid-cell-contents"><img class="docTypeIconHeader" id="docTypeIcon" alt="Document type icon" src="https://msmatter.sharepoint.com/_layouts/15/images/generaldocument.png"></div>', enableColumnMenu: false },
    	            { field: 'documentName', displayName: 'Document', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.documentName}}</div>', enableColumnMenu: false },
                    { field: 'documentClientId', displayName: 'Client', cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.documentClientId}}</div>', enableColumnMenu: false },
                    { field: 'documentOwner', displayName: 'Author', enableColumnMenu: false },
                    { field: 'documentModifiedDate', displayName: 'Modified date', enableColumnMenu: false },
                    { field: 'documentId', displayName: 'Document ID', enableColumnMenu: false },
                    { field: 'documentVersion', displayName: 'Version', enableColumnMenu: false },
                    { field: 'pin', cellTemplate: '<div class="ui-grid-cell-contents"><img src="../Images/pin-666.png"/></div>', enableColumnMenu: false }                    
    		    ],
    		    onRegisterApi: function (gridApi) {
    		        vm.gridApi = gridApi    		        		       
    		    }
    	    }
    	   
            //function to toggle check all 
    	    vm.toggleChecker = function (checked) {
    	        var rows = vm.gridApi.core.getVisibleRows(vm.gridApi.grid),
                    allChecked = true;

    	        for (var r = 0; r < rows.length; r++) {
    	            if (rows[r].entity.checker !== true) {
    	                allChecked = false;
    	                break;
    	            }
    	        }    	        
    	        $("#chkAllDocCheckBox").prop('checked', allChecked);
    	    };

            //function to check all checkboxes inside grid
    	    vm.toggleCheckerAll = function (checked) {
    	        for (var i = 0; i < vm.documentGridOptions.data.length; i++) {
    	            vm.documentGridOptions.data[i].checker = checked;
    	        }    	        
    	    };
            
            //#region api call to get document information
        	//api call to get all documents
    	    function get(options, callback) {
    	    	api({
    	    	    resource: 'documentResource',
    	    		method: 'get',
    	    		data: options,
    	    		success: callback
    	    	});
    	    }

        	//api call to get all pinned documents
    	    function getPinDocuments(options, callback) {
    	    	api({
    	    		resource: 'documentResource',
    	    		method: 'getPinnedDocuments',
    	    		data: options,
    	    		success: callback
    	    	});
    	    }

        	//api call to get documents of the current logged in user
    	    function myDocuments(options, callback) {
    	    	api({
    	    	    resource: 'documentResource',
    	    	    method: 'get',
    	    		data: options,
    	    		success: callback
    	    	});
    	    }
        	//#endregion
            
    	    

            //function to get the documents based on search term
    	    vm.getDocuments = function (searchTerm) {    	    	
    	    	var documentRequest = {
    	    	    Client: {
    	    	        //ToDo: Need to read from config.js
    	    			Url: "https://msmatter.sharepoint.com/sites/catalog"
    	    		},
                    SearchObject:{
                        PageNumber: 1,
                        ItemsPerPage: gridOptions.paginationPageSize,
                        SearchTerm: searchTerm,
                        Filters: {                            
                            ClientsList: [],
                            FromDate: "",
                            ToDate: "",
                            DocumentAuthor: "",
                            //FilterByMe: 1
                        },
                        Sort:
                          {
                              ByProperty: "LastModifiedTime",
                              Direction: 1
                          }
                    }
    	    	}                
    	    	get(documentRequest, function(response){
                    vm.documentGridOptions.data = response
                });
    	    }

            //function to get the documents which are pinned by user
    	    vm.getPinnedDocuments = function () {    	        
    	        var client = {
    	            //ToDo: Need to read from config.js
    	            Url: "https://msmatter.sharepoint.com/sites/catalog"
    	        }    	            
    	       
    	        getPinDocuments(client, function (response) {
    	            vm.documentGridOptions.data = response.DocumentDataList
    	        });
    	    }

    	   

            //function to get the documents based on document type such as "all", "pinned document", "my documents"
    	    vm.getAllDocuments = function (searchTerm) {
    	        var documentRequest = {
    	            Client: {
    	                Url: "https://msmatter.sharepoint.com/sites/catalog"
    	            },
    	            SearchObject: {
    	                PageNumber: 1,
    	                ItemsPerPage: gridOptions.paginationPageSize,
    	                SearchTerm: searchTerm,
    	                Filters: {},
    	                Sort:
                          {
                              ByProperty: "LastModifiedTime",
                              Direction: 1
                          }
    	            }
    	        }
    	        get(documentRequest, function (response) {
    	            vm.documentGridOptions.data = response
    	        });
    	    }

    	    
    	    //#endregion
        }
	    //#endregion

    ])
})();