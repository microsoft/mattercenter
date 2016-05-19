(function () {
    'use strict';

    var app = angular.module("matterMain");

    app.controller('documentsController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'matterResource', '$rootScope', 'uiGridConstants', '$location', '$http',
function ($scope, $state, $interval, $stateParams, api, $timeout, documentResource, $rootScope, uiGridConstants, $location, $http) {
    var vm = this;
    vm.selected = undefined;
    // Onload show ui grid and hide error div
    //start
    $scope.divuigrid = true;
    $scope.nodata = false;
    //end

    //to hide lazyloader on load
    //start
    $scope.lazyloader = true;
    //end

    //Assigning html for Matterheadertemplate
    //Start
    $scope.documentDropDowm = false;
    var documentHeaderTemplate = "<div ng-mouseenter='grid.appScope.documentDropDowm = true' ng-mouseleave='grid.appScope.documentDropDowm = false'>\
    <div class='dropdown keep-open' style='float:right;' ng-show='grid.appScope.documentDropDowm'>\
        <a href='javascript:;' dropdown class='prisma-header-dropdown-anchor id='acombo' dropdown-toggle' type='button' data-toggle='dropdown'>\
            <img src='../images/icon-combobox.png'/>\
        </a>\
        <div class='dropdown-menu flyoutWrapper' ng-click='$event.stopPropagation();' role='menu'  aria-labelledby='acombo'>\
         <div class='input-group'>\
           <input class='form-control' ng-model='grid.appScope.vm.searchTerm' placeholder='Search'/> \
                <div class='input-group-btn'>\
                 <button type='button' ng-click='grid.appScope.vm.searchDocumentGrid()' class='btn btn-default' ><img src='../images/search-15x15-666.png' /></button>\
                 </div>\
         </div>\
         <hr/> \
            <div class='clearFilterText' data-clearfiltertype='text'>\
            <div class='clearFiltersIcon'>\
                <img src='../Images/Filters_30px_X_30px_active_color_666.png' alt='clear' title='Clear filters'>\
            </div>\
            <div class='ms-font-m ms-font-weight-semilight clearText' title='Clear filters from Matter'><span>Clear filters from </span><span class='clearFilterTitle'>Document</span></div>\
             <div id='filterResultsContainer'><div class='filterValueLabels ms-font-m ms-font-weight-semilight' ng-repeat='matter in grid.appScope.matters' ng-click='grid.appScope.filterdocumentName(document.documentName)'>{{document.documentName}}</div> </div>\
        </div>\
        </div>\
    </div>\
     <div role='button' style='padding-left: 11px;' class='ui-grid-cell-contents ui-grid-header-cell-primary-focus' col-index='renderIndex'>\
        <span class='ui-grid-header-cell-label ng-binding'>{{ col.colDef.displayName }}</span>\
        <span ui-grid-visible='col.sort.direction' aria-label='{{getSortDirectionAriaLabel()}}' class='ui-grid-invisible'><sub ui-grid-visible='isSortPriorityVisible()' class='ui-grid-sort-priority-number'>{{col.sort.priority + 1}}</sub></span>\
        <span class='sort pull-right' ng-show='grid.appScope.sortMCDocumentName'>↑</span>\
        <span ng-show='grid.appScope.sortDownMCDocumentName' class='sort pull-right'>↓</span>\
    </div>\
    <div class='ui-grid-column-menu-button ng-scope' ng-if='grid.options.enableColumnMenus && !col.isRowHeader && col.colDef.enableColumnMenu !== false' ng-click='grid.appScope.toggleMenu($event)' ng-class='{'ui-grid-column-menu-button-last-col': isLastCol}'>\
        <i  title='' aria-hidden='true' class='ui-grid-icon-up-dir'>&nbsp;</i>\
    </div>\
</div>"
    //End


    //Assigning html for Clientheadertemplate
    //Start
    $scope.clientDropDowm = false;
    var ClientHeaderTemplate = "<div ng-mouseenter='grid.appScope.clientDropDowm = true' ng-mouseleave='grid.appScope.clientDropDowm = false'>\
    <div class='dropdown keep-open' style='float:right;' ng-show='grid.appScope.clientDropDowm'>\
        <a href='javascript:;' dropdown class='prisma-header-dropdown-anchor id='acombo' dropdown-toggle' type='button' data-toggle='dropdown'>\
            <img src='../images/icon-combobox.png'/>\
        </a>\
        <div class='dropdown-menu flyoutWrapper dropdown-menu-right' ng-click='$event.stopPropagation();' role='menu'  aria-labelledby='acombo'>\
         <div class='input-group'>\
           <input class='form-control' ng-model='grid.appScope.vm.searchClientTerm' placeholder='Search'/> \
                <div class='input-group-btn'>\
                 <button type='button' ng-click='grid.appScope.vm.searchClient()' class='btn btn-default' ><img src='../images/search-15x15-666.png' /></button>\
                 </div>\
         </div>\
         <hr/> \
            <div class='clearFilterText' data-clearfiltertype='text'>\
            <div class='clearFiltersIcon'>\
                <img src='../Images/Filters_30px_X_30px_active_color_666.png' alt='clear' title='Clear filters'>\
            </div>\
            <div class='ms-font-m ms-font-weight-semilight clearText' title='Clear filters from Client'><span>Clear filters from </span><span class='clearFilterTitle'>Client</span></div>\
             <div id='filterResultsContainer'><div class='filterValueLabels ms-font-m ms-font-weight-semilight' ng-repeat='client in grid.appScope.Clients' ng-click='grid.appScope.filterClientName(Client.matterClient)'>{{client.matterClient}}</div> </div>\
        </div>\
        </div>\
    </div>\
    <div role='button' class='ui-grid-cell-contents ui-grid-header-cell-primary-focus' col-index='renderIndex'>\
        <span class='ui-grid-header-cell-label ng-binding'>{{ col.colDef.displayName }}</span>\
        <span ui-grid-visible='col.sort.direction' aria-label='Sort None' class='ui-grid-invisible'></span>\
        <span class='sort pull-right' ng-show='grid.appScope.sortMCClient'>↑</span>\
        <span ng-show='grid.appScope.sortDownMCClient' class='sort pull-right'>↓</span>\
    </div>\
    <div class='ui-grid-column-menu-button ng-scope' ng-if='grid.options.enableColumnMenus && !col.isRowHeader && col.colDef.enableColumnMenu !== false' ng-click='grid.appScope.toggleMenu($event)' ng-class='{'ui-grid-column-menu-button-last-col': isLastCol}'>\
        <i  title='' aria-hidden='true' class='ui-grid-icon-up-dir'>&nbsp;</i>\
    </div>\
</div>"
    //End

    //Assigning html for ModifiedDateheadertemplate
    //Start
    $scope.modifieddateDropDowm = false;
    var ModifiedDateheadertemplate = "<div ng-mouseenter='grid.appScope.modifieddateDropDowm = true' ng-mouseleave='grid.appScope.modifieddateDropDowm = false'>\
    <div class='dropdown keep-open' style='float:right;' ng-show='grid.appScope.modifieddateDropDowm'>\
        <a href='javascript:;' dropdown class='prisma-header-dropdown-anchor id='acombo' dropdown-toggle' type='button' data-toggle='dropdown'>\
            <img src='../images/icon-combobox.png'/>\
        </a>\
        <div class='dropdown-menu flyoutWrapper dropdown-menu-right' ng-click='$event.stopPropagation();' role='menu'  aria-labelledby='acombo'>\
         <div class='input-group'>\
             <input type='text' placeholder='Start mm/dd/yyyy' class='calendar form-control'\
                    uib-datepicker-popup='MM/dd/yyyy'\
                    ng-model='grid.appScope.$parent.startdate'\
                    is-open='grid.appScope.openedStartDate' \
                    datepicker-options='grid.appScope.dateOptions'\
                    ng-required='true' close-text='Close'\
                    alt-input-formats='altInputFormats'/>\
                <span class='input-group-btn'>\
                    <button type='button' class='btn btn-default' ng-click='grid.appScope.openStartDate()'><i class='glyphicon glyphicon-calendar'></i></button>\
                </span>\
          </div>\
          <div class='input-group' style='margin-top:5px'>\
             <input type='text' placeholder='End mm/dd/yyyy' class='calendar form-control'\
                    uib-datepicker-popup='MM/dd/yyyy'\
                    ng-model='grid.appScope.$parent.enddate'\
                    is-open='grid.appScope.openedEndDate' \
                    datepicker-options='grid.appScope.enddateOptions'\
                    ng-required='true' close-text='Close'\
                    alt-input-formats='altInputFormats' />\
                <span class='input-group-btn'>\
                    <button type='button' class='btn btn-default' ng-click='grid.appScope.openEndDate()'><i class='glyphicon glyphicon-calendar'></i></button>\
                </span>\
          </div>\
          <div class='okDateButton' id='btnOK' ng-click='grid.appScope.FilterModifiedDate()'>Ok</div>\
          <hr/> \
            <div class='clearFilterText' data-clearfiltertype='text'>\
            <div class='clearFiltersIcon'>\
                <img src='../Images/Filters_30px_X_30px_active_color_666.png' alt='clear' title='Clear filters'>\
            </div>\
            <div class='ms-font-m ms-font-weight-semilight clearText' title='Clear filters from Modified Date'><span>Clear filters from </span><span class='clearFilterTitle'>Modified Date</span></div>\
             <div id='filterResultsContainer'><div class='filterValueLabels ms-font-m ms-font-weight-semilight' ng-repeat='client in grid.appScope.Clients' ng-click='grid.appScope.filterClientName(Client.matterClient)'>{{client.matterClient}}</div> </div>\
        </div>\
        </div>\
    </div>\
    <div role='button' class='ui-grid-cell-contents ui-grid-header-cell-primary-focus' col-index='renderIndex'>\
        <span class='ui-grid-header-cell-label ng-binding'>{{ col.colDef.displayName }}</span>\
        <span ui-grid-visible='col.sort.direction' aria-label='Sort None' class='ui-grid-invisible'></span>\
        <span class='sort pull-right' ng-show='grid.appScope.sortMCModifiedDate'>↑</span>\
        <span ng-show='grid.appScope.sortDownMCModifiedDate' class='sort pull-right'>↓</span>\
    </div>\
    <div class='ui-grid-column-menu-button ng-scope' ng-if='grid.options.enableColumnMenus && !col.isRowHeader && col.colDef.enableColumnMenu !== false' ng-click='grid.appScope.toggleMenu($event)' ng-class='{'ui-grid-column-menu-button-last-col': isLastCol}'>\
        <i  title='' aria-hidden='true' class='ui-grid-icon-up-dir'>&nbsp;</i>\
    </div>\
</div>"
    //End

    //Assigning html for celltemplate
    //Start
    var documentCellTemplate = "<div class='row'>\
    <div class='col-xs-7 col-sm-9 col-md-9 col-lg-10' id='documentPopup'>\
        <a popoverdoc type='button' class='btn btn-link col-xs-12 col-sm-12' style='text-align:left' details={{row.entity}} data-toggle='popover' data-container='body' data-placement='right' type='button' data-html='true' href='' > {{row.entity.documentName}} </a>\
        <div class='popover-content'></div>\
    </div>\
    <div class='col-xs-5 col-sm-3 col-md-3 col-lg-2 text-right'>\
        <div class='dropdown'>\
            <a class='btn-link dropdown-toggle ms-Icon ms-Icon--ellipsis ellipsis UiGrid-a' type='button' data-toggle='dropdown'></a><ul style='margin:0;padding:0' class='dropdown-menu'>\
                <li class='ms-ContextualMenu-item'><a class='ECBItem ms-ContextualMenu-link upload' ng-href='https://msmatter.sharepoint.com/sites/microsoft/Sites/Microsoft/'>Open this Document</a></li>\
                <li class='ms-ContextualMenu-item'><a class='ECBItem ms-ContextualMenu-link upload' ng-href='https://msmatter.sharepoint.com/sites/microsoft/SitePages/{{row.entity.matterGuid}}.aspx' target='_blank'>View Matter Details</a></li>\
                <li class='ms-ContextualMenu-item' ng-click='row.entity.MatterInfo===undefined?grid.appScope.PinMatter(row):grid.appScope.UnpinMatter(row)'><a class='ECBItem ms-ContextualMenu-link upload'>{{row.entity.MatterInfo===undefined?'Pin this Matter':'Unpin this matter'}}</a></li>\
            </ul>\
        </div>\
    </div>\
</div>";
    //End

    vm.gridOptions = {
        enableGridMenu: true,
        enableRowHeaderSelection: false,
        enableRowSelection: true,
        enableSelectAll: false,
        multiSelect: false,
        columnDefs: [{
            field: 'documentName', displayName: 'Document', enableHiding: false, cellTemplate: documentCellTemplate,
            headerCellTemplate: documentHeaderTemplate
        },
            { field: 'documentClientId', displayName: 'Client', enableCellEdit: true, headerCellTemplate: ClientHeaderTemplate },
             //matterID 
    { field: 'documentClientId', displayName: 'Client.Matter ID', cellTemplate: '<div class="ngCellText">{{row.entity.documentClientId}}.{{row.entity.documentMatterId}}</div>', enableCellEdit: true, },
     { field: 'documentModifiedDate', displayName: 'Modified Date', cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.documentModifiedDate}}"></div>', headerCellTemplate: ModifiedDateheadertemplate },
     { field: 'documentOwner', displayName: 'Author', visible: false },
     { field: 'documentVersion', displayName: 'Document Version', visible: false },
     { field: 'documentCheckoutUser', displayName: 'Checked out to', cellTemplate: '<div class="ngCellText">{{row.entity.documentCheckoutUser=="" ? "NA":row.entity.documentCheckoutUser}}</div>', visible: false },
     { field: 'documentCreatedDate', displayName: 'Created date', cellTemplate: '<div class="ui-grid-cell-contents" datefilter date="{{row.entity.documentCreatedDate}}"></div>', visible: false },
        ],
        enableColumnMenus: false,
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
            gridApi.core.on.columnVisibilityChanged($scope, function (changedColumn) {
                $scope.columnChanged = { name: changedColumn.colDef.name, visible: changedColumn.colDef.visible };
            });
            gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                vm.selectedRow = row.entity
            });
            $scope.gridApi.core.on.sortChanged($scope, $scope.sortChanged);
            $scope.sortChanged($scope.gridApi.grid, [vm.gridOptions.columnDefs[1]]);
        }
    };


    //search api call 
    function get(options, callback) {
        api({
            resource: 'documentResource',
            method: 'get',
            data: options,
            success: callback
        });
    }


    function getPinnedDocuments(options, callback) {
        api({
            resource: 'documentResource',
            method: 'getPinnedDocuments',
            data: options,
            success: callback
        });
    }


    //Callback function for pin 
    function pinDocuments(options, callback) {
        api({
            resource: 'documentResource',
            method: 'pinDocument',
            data: options,
            success: callback
        });
    }


    //Callback function for unpin 
    function UnpinDocuments(options, callback) {
        api({
            resource: 'documentResource',
            method: 'unPinDocument',
            data: options,
            success: callback
        });
    }


    vm.searchDocument = function (val) {


        var searchRequest =
          {
              Client: {
                  Id: "123456",
                  Name: "Microsoft",
                  Url: "https://msmatter.sharepoint.com/sites/catalog"
              },
              SearchObject: {
                  PageNumber: 1,
                  ItemsPerPage: 10,
                  SearchTerm: val,
                  Filters: {},
                  Sort:
                          {
                              ByProperty: "LastModifiedTime",
                              Direction: 1
                          }
              }
          };


        return documentResource.get(searchRequest).$promise;
    }


    vm.search = function () {


        var searchRequest =
          {
              Client: {
                  Id: "123456",
                  Name: "Microsoft",
                  Url: "https://msmatter.sharepoint.com/sites/catalog"
              },
              SearchObject: {
                  PageNumber: 1,
                  ItemsPerPage: 10,
                  SearchTerm: vm.searchTerm,
                  Filters: {},
                  Sort:
                          {
                              ByProperty: "LastModifiedTime",
                              Direction: 1
                          }
              }
          };
        get(searchRequest, function (response) {
            vm.gridOptions.data = response.matterDataList;
        });
    }


    //For Searching Matter in GridHeader Menu
    //Start
    vm.searchDocumentGrid = function () {
        $scope.lazyloader = false;

        var searchRequest =
          {
              Client: {
                  Id: "123456",
                  Name: "Microsoft",
                  Url: "https://msmatter.sharepoint.com/sites/catalog"
              },
              SearchObject: {
                  PageNumber: 1,
                  ItemsPerPage: 10,
                  SearchTerm: vm.searchTerm,
                  Filters: {},
                  Sort:
                          {
                              ByProperty: "MCdocumentName",
                              Direction: 0
                          }
              }
          };
        get(searchRequest, function (response) {
            $scope.lazyloader = true;
            $scope.matters = response;
        });
    }
    //End

    //For Searching client in GridHeader Menu
    //start
    vm.searchClient = function () {
        $scope.lazyloader = false;

        var searchRequest =
          {
              Client: {
                  Id: "123456",
                  Name: "Microsoft",
                  Url: "https://msmatter.sharepoint.com/sites/catalog"
              },
              SearchObject: {
                  PageNumber: 1,
                  ItemsPerPage: 10,
                  SearchTerm: vm.searchClientTerm,
                  Filters: {},
                  Sort:
                          {
                              ByProperty: "MCClientName",
                              Direction: 0
                          }
              }
          };
        get(searchRequest, function (response) {
            $scope.lazyloader = true;
            $scope.Clients = response;
        });
    }
    //end

    //For filtering documentName 
    //Start
    $scope.filterdocumentName = function (documentName) {
        $scope.lazyloader = false;
        var searchRequest =
          {
              Client: {
                  Id: "123456",
                  Name: "Microsoft",
                  Url: "https://msmatter.sharepoint.com/sites/catalog"
              },
              SearchObject: {
                  PageNumber: 1,
                  ItemsPerPage: 10,
                  SearchTerm: documentName,
                  Filters: {},
                  Sort:
                          {
                              ByProperty: "LastModifiedTime",
                              Direction: 1
                          }
              }
          };
        get(searchRequest, function (response) {
            $scope.lazyloader = true;
            vm.gridOptions.data = response;
            $scope.matters = [];
        });

    }

    //End


    //For filtering Clientname 
    //Start
    $scope.filterClientName = function (clientname) {
        $scope.lazyloader = false;
        var searchRequest =
          {
              Client: {
                  Id: "123456",
                  Name: "Microsoft",
                  Url: "https://msmatter.sharepoint.com/sites/catalog"
              },
              SearchObject: {
                  PageNumber: 1,
                  ItemsPerPage: 10,
                  SearchTerm: clientname,
                  Filters: {},
                  Sort:
                          {
                              ByProperty: "LastModifiedTime",
                              Direction: 1
                          }
              }
          };
        get(searchRequest, function (response) {
            $scope.lazyloader = true;
            vm.gridOptions.data = response;
            $scope.Clients = [];
        });

    }

    //End


    //Code for filtering ModifiedDate
    //start
    $scope.FilterModifiedDate = function () {
        $scope.lazyloader = false;
        var searchRequest =
          {
              Client: {
                  Id: "123456",
                  Name: "Microsoft",
                  Url: "https://msmatter.sharepoint.com/sites/catalog"
              },
              SearchObject: {
                  PageNumber: 1,
                  ItemsPerPage: 10,
                  SearchTerm: "",
                  Filters: {
                      OLList: "",
                      ClientName: "",
                      ClientsList: [],
                      DateFilters: { CreatedFromDate: "", CreatedToDate: "", ModifiedFromDate: $scope.startdate, ModifiedToDate: $scope.enddate, OpenDateFrom: "", OpenDateTo: "" },
                      DocumentAuthor: [],
                      DocumentCheckoutUsers: [],
                      FilterByMe: 1,
                      FromDate: "",
                      Name: "",
                      PGList: "",
                      ResponsibleAttorneys: [],
                      SubareaOfLaw: "",
                      ToDate: ""
                  },
                  Sort:
                          {
                              ByProperty: "LastModifiedTime",
                              Direction: 0
                          }
              }
          };
        get(searchRequest, function (response) {
            $scope.lazyloader = true;
            vm.gridOptions.data = response;
            $scope.startdate = "";
            $scope.enddate = "";
        });

    }

    //end

    //Code written for displaying types in dropdown 
    //Start 
    $scope.Documents = [{ Id: 1, Name: "All documents" }, { Id: 2, Name: "My documents" }, { Id: 3, Name: "Pinned documents" }];
    $scope.ddlDocuments = $scope.Documents[0];


    //End  

    $scope.Pinnedobj = [];
    $scope.getDocumentPinned = function () {

        var pinnedDocumentsRequest = {
            Id: "123456",
            Name: "Microsoft",
            Url: "https://msmatter.sharepoint.com/sites/catalog"
        }
        getPinnedDocuments(pinnedDocumentsRequest, function (response) {
            for (var i = 0; i < response.documentDataList.length; i++) {
                $scope.Pinnedobj.push(response.documentDataList[i]);
            }
        });
        return true;
    }

    $scope.getDocumentPinned();


    //Hits when the Dropdown changes 
    //Start 
    $scope.GetDocuments = function (id) {
        $scope.lazyloader = false;
        if (id == 1) {
            var AllMattersRequest = {
                Client: {
                    Id: "123456",
                    Name: "Microsoft",
                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                },
                SearchObject: {
                    PageNumber: 1,
                    ItemsPerPage: 10,
                    SearchTerm: "",
                    Filters: {
                        AOLList: "",
                        ClientsList: [],
                        FilterByMe: 0,
                        FromDate: "",
                        PGList: "",
                        ToDate: "",
                    },
                    Sort:
                            {
                                ByProperty: "LastModifiedTime",
                                Direction: 1
                            }
                }
            }


            get(AllMattersRequest, function (response) {
                $scope.lazyloader = true;
                if (response.errorCode == "404") {
                    $scope.divuigrid = false;
                    $scope.nodata = true;
                    $scope.errorMessage = response.message;
                } else {
                    $scope.getDocumentPinned();
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                    if ($scope.Pinnedobj.length > 0) {
                        angular.forEach($scope.Pinnedobj, function (pinobj) {
                            angular.forEach(response, function (res) {
                                if (pinobj.documentName == res.documentName) {
                                    if (res.ismatterdone == undefined && !res.ismatterdone) {
                                        res.MatterInfo = "Unpin this matter";
                                        res.ismatterdone = true;
                                    }
                                }
                            });
                        });
                        vm.gridOptions.data = response;
                        console.log(response);
                    } else {
                        vm.gridOptions.data = response;
                    }
                }
            });


        } else if (id == 2) {
            $scope.lazyloader = false;
            var MyDocumentsRequest = {
                Client: {
                    Id: "123456",
                    Name: "Microsoft",
                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                },
                SearchObject: {
                    PageNumber: 1,
                    ItemsPerPage: 10,
                    SearchTerm: "",
                    Filters: {
                        AOLList: "",
                        ClientsList: [],
                        FilterByMe: 1,
                        FromDate: "",
                        PGList: "",
                        ToDate: ""
                    },
                    Sort:
                            {
                                ByProperty: "LastModifiedTime",
                                Direction: 1
                            }
                }
            }


            get(MyDocumentsRequest, function (response) {
                $scope.lazyloader = true;
                if (response.errorCode == "404") {
                    $scope.divuigrid = false;
                    $scope.nodata = true;
                    $scope.errorMessage = response.message;
                } else {
                    $scope.getDocumentPinned();
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                    if ($scope.Pinnedobj.length > 0) {
                        angular.forEach($scope.Pinnedobj, function (pinobj) {
                            angular.forEach(response, function (res) {
                                if (pinobj.documentName == res.documentName) {
                                    if (res.ismatterdone == undefined && !res.ismatterdone) {
                                        res.MatterInfo = "Unpin this matter";
                                        res.ismatterdone = true;
                                    }
                                }
                            });
                        });
                        vm.gridOptions.data = response;
                    } else {
                        vm.gridOptions.data = response;
                    }
                }
            });
        } else if (id == 3) {
            $scope.lazyloader = false;
            var pinnedMattersRequest = {
                Id: "123456",
                Name: "Microsoft",
                Url: "https://msmatter.sharepoint.com/sites/catalog"
            }
            getPinnedDocuments(pinnedMattersRequest, function (response) {
                $scope.lazyloader = true;
                if (response.errorCode == "404") {
                    $scope.divuigrid = false;
                    $scope.nodata = true;
                    $scope.errorMessage = response.message;
                } else {
                    $scope.getDocumentPinned();
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                    angular.forEach(response.matterDataList, function (res) {
                        if (res.ismatterdone == undefined && !res.ismatterdone) {
                            res.MatterInfo = "Unpin this matter";
                            res.ismatterdone = true;
                        }
                    });
                    vm.gridOptions.data = response.matterDataList;
                }
            });
        }
    }
    //End 


    //To run GetDocuments function on page load 
    $scope.GetDocuments($scope.ddlDocuments.Id);
    //End 


    //Written for unpinning the matter 
    //Start 
    $scope.UnpinMatter = function (data) {
        var alldata = data.entity;
        var unpinRequest = {
            Client: {
                Id: "123456",
                Name: "Microsoft",
                Url: "https://msmatter.sharepoint.com/sites/catalog"
            },
            matterData: {
                documentName: alldata.matterUrl,
            }
        }
        UnpinDocuments(unpinRequest, function (response) {
            if (response.isMatterUnPinned) {
                $timeout(function () { $scope.GetDocuments($scope.ddldocuments.Id); }, 500);
                alert("Success");
            }
        });
    }
    //End 


    //Written for pinning the matter 
    //Start 
    $scope.PinMatter = function (data) {
        var alldata = data.entity;
        var pinRequest = {
            Client: {
                Id: "123456",
                Name: "Microsoft",
                Url: "https://msmatter.sharepoint.com/sites/catalog"
            },
            documentData: {
                documentName: alldata.documentName,
                DocumentVersion: alldata.DocumentVersion,
                DocumentClient: alldata.DocumentClient,
                DocumentClientId: alldata.DocumentClientId,
                DocumentClientUrl: alldata.DocumentClientUrl,
                DocumentMatter: alldata.DocumentMatter,
                DocumentMatterId: alldata.DocumentMatterId,
                DocumentOwner: alldata.DocumentOwner,
                DocumentUrl: alldata.DocumentUrl,
                DocumentOWAUrl: alldata.DocumentOWAUrl,
                DocumentExtension: alldata.DocumentExtension,
                DocumentCreatedDate: alldata.DocumentCreatedDate,
                DocumentModifiedDate: alldata.DocumentModifiedDate,
                DocumentCheckoutUser: alldata.DocumentCheckoutUser,
                DocumentMatterUrl: alldata.DocumentMatterUrl,
                DocumentParentUrl: alldata.DocumentParentUrl,
                DocumentID: alldata.DocumentID
            }
        }
        pinDocuments(pinRequest, function (response) {
            if (response.isDocumentPinned) {
                $timeout(function () { $scope.GetDocuments($scope.ddlDocuments.Id); }, 500);
                alert("Success");
            }
        });
    }
    //End 


    vm.menuClick = function () {
        var oAppMenuFlyout = $(".AppMenuFlyout");
        if (!(oAppMenuFlyout.is(":visible"))) {
            //// Display the close icon and close the fly out 
            $(".OpenSwitcher").addClass("hide");
            $(".CloseSwitcher").removeClass("hide");
            $(".MenuCaption").addClass("hideMenuCaption");
            oAppMenuFlyout.slideDown();
        } else {
            oAppMenuFlyout.slideUp();
            $(".CloseSwitcher").addClass("hide");
            $(".OpenSwitcher").removeClass("hide");
            $(".MenuCaption").removeClass("hideMenuCaption");
        }
    }

    //Angular Datepicker Starts here
    //Start
    $scope.dateOptions = {

        formatYear: 'yy',
        maxDate: new Date()
    };


    $scope.enddateOptions = {
        formatYear: 'yy',
        maxDate: new Date()
    }

    $scope.$watch('startdate', function (newval, oldval) {
        $scope.enddateOptions.minDate = newval;
    });


    $scope.openStartDate = function ($event) {
        if ($event) {
            $event.preventDefault();
            $event.stopPropagation();
        }
        this.openedStartDate = true;
    };
    $scope.openEndDate = function ($event) {
        if ($event) {
            $event.preventDefault();
            $event.stopPropagation();
        }
        this.openedEndDate = true;
    };

    $scope.openedStartDate = false;
    $scope.openedEndDate = false;

    $scope.disabled = function (date, mode) {
        return (mode === 'day' && (date.getDay() != 0));
    };



    //End

    //#region Custom Sorting functionality
    //Start
    var SortRequest = {
        Client: {
            Id: "123456",
            Name: "Microsoft",
            Url: "https://msmatter.sharepoint.com/sites/catalog"
        },
        SearchObject: {
            PageNumber: 1,
            ItemsPerPage: 10,
            SearchTerm: "",
            Filters: {
                AOLList: "",
                ClientsList: [],
                FilterByMe: 1,
                FromDate: "",
                PGList: "",
                ToDate: ""
            },
            Sort:
                    {
                        ByProperty: '',
                        Direction: 0
                    }
        }
    }

    $scope.FilterByType = function () {
        get(SortRequest, function (response) {
            $scope.lazyloader = true;
            if (response.errorCode == "404") {
                $scope.divuigrid = false;
                $scope.nodata = true;
                $scope.errorMessage = response.message;
            } else {
                $scope.divuigrid = true;
                $scope.nodata = false;
                vm.gridOptions.data = response;
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            }
        });
    }

    $scope.sortChanged = function (grid, sortColumns) {
        $scope.divuigrid = false;
        $scope.nodata = true;
        if (sortColumns.length != 0) {
            if (sortColumns[0].name == vm.gridOptions.columnDefs[0].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.FileNameSort == undefined || localStorage.FileNameSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "FileName";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.FileNameSort = "desc";
                    } else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "FileName";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.FileNameSort = "asc";
                    }
                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
            else if (sortColumns[0].name == vm.gridOptions.columnDefs[1].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.DocumentClientSort == undefined || localStorage.DocumentClientSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCDocumentClientName";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.DocumentClientSort = "desc";
                    }
                    else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCDocumentClientName";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.DocumentClientSort = "asc";
                    }
                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
            else if (sortColumns[0].name == vm.gridOptions.columnDefs[2].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.DocumentClientIDSort == undefined || localStorage.DocumentClientIDSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCDocumentClientID";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.DocumentClientIDSort = "desc";
                    } else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCDocumentClientID";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.DocumentClientIDSort = "asc";
                    }

                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
            else if (sortColumns[0].name == vm.gridOptions.columnDefs[3].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.ModiFiedDateSort == undefined || localStorage.ModiFiedDateSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCModifiedDate";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.ModiFiedDateSort = "desc";
                    } else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCModifiedDate";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.ModiFiedDateSort = "asc";
                    }

                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
            else if (sortColumns[0].name == vm.gridOptions.columnDefs[4].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.AuthorSort == undefined || localStorage.AuthorSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MSITOfficeAuthor";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.AuthorSort = "desc";
                    } else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MSITOfficeAuthor";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.AuthorSort = "asc";
                    }
                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
            else if (sortColumns[0].name == vm.gridOptions.columnDefs[5].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.VersionSort == undefined || localStorage.VersionSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCVersionNumber";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.VersionSort = "desc";
                    } else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCVersionNumber";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.VersionSort = "desc";
                    }
                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
            else if (sortColumns[0].name == vm.gridOptions.columnDefs[6].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.CheckoutSort == undefined || localStorage.CheckoutSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCCheckoutUser";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.CheckoutSort = "desc"
                    } else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "MCCheckoutUser";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.CheckoutSort = "asc"
                    }
                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
            else if (sortColumns[0].name == vm.gridOptions.columnDefs[7].name) {
                if (sortColumns[0].sort != undefined) {
                    if (localStorage.CreatedSort == undefined || localStorage.CreatedSort == "asc") {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "Created";
                        SortRequest.SearchObject.Sort.Direction = 0;
                        $scope.FilterByType();
                        localStorage.CreatedSort = "desc"
                    } else {
                        $scope.lazyloader = false;
                        SortRequest.SearchObject.Sort.ByProperty = "Created";
                        SortRequest.SearchObject.Sort.Direction = 1;
                        $scope.FilterByType();
                        localStorage.CreatedSort = "asc"
                    }
                } else {
                    $scope.divuigrid = true;
                    $scope.nodata = false;
                }
            }
        } else {
            $scope.lazyloader = false;
            SortRequest.SearchObject.Sort.ByProperty = "FileName";
            SortRequest.SearchObject.Sort.Direction = 0;
            $scope.FilterByType();
        }
    }
    //#endregion


}]);

    app.directive('popoverdoc', function () {
        return {
            restrict: 'AE',
            scope: { details: '@' },
            link: function (scope, element, attrs) {
                scope.$watch("details", function () {

                    var obj = eval('(' + attrs.details + ')');
                    var content = '<div class="">\
                                   <div class="FlyoutBoxContent">\
                                      <div class="FlyoutContent FlyoutHeading">\
                                          <div class="ms-Callout-content FlyoutHeadingText">  ' + obj.documentName + ' </div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Matter:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.documentMatter + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Client:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.documentClient + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Document ID:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.DocumentID + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Author:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.documentOwner + '</div> \
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Modified date:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.DocumentModifiedDate + '</div>\
                                       </div>\
                                       <button class="ms-Button ms-Button--primary ms-Callout-content" id="viewMatters"><a class="ms-Button-label" href="https://msmatter.sharepoint.com/sites/microsoft/SitePages/' + obj.matterGuid + '.aspx" target="_blank">Open document</a></button>\
                                       <button class="ms-Button ms-Button--primary ms-Callout-content" id="uploadToMatter"><a class="ms-Button-label" href="https://msmatter.sharepoint.com/sites/catalog/SitePages/documentDetails.aspx">View document details</a></button>\
                                    </div>\
                                </div>';
                    $(element).popover({
                        html: true,
                        trigger: 'click',
                        delay: 500,
                        content: content,
                    });
                });
            }
        }
    });

})();

