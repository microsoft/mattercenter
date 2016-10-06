(function () {
    'use strict;'
    var app = angular.module("matterMain");
    app.controller('MatterDashBoardController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'matterDashBoardResource', '$rootScope', 'uiGridConstants', '$location', '$http', '$q', '$filter', 'commonFunctions', '$window',
        function matterDashBoardController($scope, $state, $interval, $stateParams, api, $timeout, matterDashBoardResource, $rootScope, uiGridConstants, $location, $http, $q, $filter, commonFunctions, $window) {
            var vm = this;
            vm.selectedRow = {
                matterClientUrl: '',
                matterName: '',
                matterGuid: ''
            };
            vm.downwarddrop = true;
            vm.upwarddrop = false;
            vm.loadLocation = false;
            vm.AuthornoResults = false;
            vm.clientdrop = false;
            vm.clientdropvisible = false;
            vm.pgdrop = false;
            vm.pgdropvisible = false;
            vm.configsUri = configs.uri;
            vm.configSearchContent = configs.search;
            vm.center = configs.search.Schema.toLowerCase();
            vm.matterDashboardConfigs = uiconfigs.MatterDashboard;
            vm.matterConfigContent = uiconfigs.Matters;
            vm.globalSettings = configs.global;
            sortPropertyForAllMatters = configs.search.ManagedPropertyMatterName;
            vm.aoldrop = false;
            vm.aoldropvisible = false;
            vm.checkClient = false;
            vm.sortbydrop = false;
            vm.sortbydropvisible = false;
            vm.searchText = '';
            vm.lazyloaderdashboard = true;
            vm.lazyloaderclient = true;
            vm.lazyloaderpg = true;
            vm.lazyloaderaol = true;
            vm.totalrecords = 0;
            $rootScope.bodyclass = "bodymain";
            $rootScope.profileClass = "hide";
            $rootScope.displayOverflow = "display";
            vm.tabClicked = "All Matters";
            vm.sortbytext = vm.tabClicked == "All Matters" ? vm.matterDashboardConfigs.DropDownOptionText : vm.matterDashboardConfigs.DrpDownOption1Text;
            vm.showNavTab = false;
            vm.showInnerNav = true;
            vm.selectedTab = vm.matterDashboardConfigs.Tab1HeaderText;

            //#endregion
            //#region Variable to show the matter count            
            vm.allMatterCount = 0;
            vm.myMatterCount = 0;
            vm.pinMatterCount = 0;
            vm.selectedTabInfo = vm.matterDashboardConfigs.Tab2HeaderText + " (" + vm.allMatterCount + ")";
            vm.Pinnedobj = [];
            //#endregion            
            vm.teamName = '';
            //#region Get Querystring values
            if ($location.search() && $location.search().teamName) {
                vm.teamName = $location.search().teamName;
                vm.selectedClients = vm.teamName;
            }
            //#endregion

            //#region closing all dropdowns on click of page
            vm.closealldrops = function () {
                vm.searchdrop = false;
                vm.downwarddrop = true;
                vm.upwarddrop = false;
                vm.clientdrop = false;
                vm.clientdropvisible = false;
                vm.pgdrop = false;
                vm.pgdropvisible = false;
                vm.aoldrop = false;
                vm.aoldropvisible = false;
                vm.sortbydrop = false;
                vm.sortbydropvisible = false;
                vm.showNavTab = false;
                vm.showInnerNav = true;
            }
            //#endregion

            //#region closing and hiding innerdropdowns of search box
            vm.hideinnerdrop = function ($event) {
                $event.stopPropagation();
                vm.clientdrop = false;
                vm.clientdropvisible = false;
                vm.pgdrop = false;
                vm.pgdropvisible = false;
                vm.aoldrop = false;
                vm.aoldropvisible = false;
            }
            //#endregion


            var gridOptions = {
                paginationPageSize: 30,
                enableGridMenu: false,
                enableRowHeaderSelection: false,
                enableRowSelection: true,
                enableSelectAll: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableFiltering: false,
                enableSorting: false
            }
            var columnDefs1 = [];
            angular.forEach(configs.search.searchColumnsUIPickerForMatter, function (value, key) {
                if (key == "matterName") {
                    if (value.displayInUI == true && value.position != -1) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterDashboardConfigs.GridColumn1Header,
                            enableColumnMenu: false,
                            width: "230",
                            cellTemplate: '../app/dashboard/MatterDashboardCellTemplate.html',
                            position: value.position,
                            visible: true
                        });

                    }
                }
                if (key == "matterClient") {
                    if (value.displayInUI == true && value.position != -1) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterDashboardConfigs.GridColumn2Header,
                            enableColumnMenu: false,
                            width: "150",
                            cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.matterClient}}</div>',
                            position: value.position,
                            visible: value.displayInDashboard
                        });
                    }
                }
                if (key == "matterClientId") {
                    if (value.displayInUI == true && value.position != -1) {
                        columnDefs1.push({
                            field: key,
                            width: '200',
                            displayName: vm.matterDashboardConfigs.GridColumn3Header,
                            headerTooltip: 'Click to sort by client.matterid',
                            headerCellClass: 'matterGridClientClass',
                            cellClass: 'matterGridClientClass',
                            enableCellEdit: true,
                            cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.matterClientId}}.{{row.entity.matterClient}}</div>',
                            enableColumnMenu: false,
                            position: value.position,
                            visible: value.displayInDashboard
                        });

                    }

                }
                if (key == "matterModifiedDate") {
                    if (value.displayInUI == true && value.position != -1) {
                        columnDefs1.push({
                            field: key,
                            width: '200',
                            headerCellClass: 'matterGridModDateClass',
                            cellClass: 'matterGridModDateClass',
                            displayName: vm.matterDashboardConfigs.GridColumn4Header,
                            cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.matterModifiedDate}}"></div>',
                            enableColumnMenu: false,
                            position: value.position,
                            visible: value.displayInDashboard
                        });

                    }
                }
                if (key == "matterResponsibleAttorney") {
                    if (value.displayInUI == true && value.position != -1) {
                        columnDefs1.push({
                            field: key,
                            width: '175',
                            headerCellClass: 'matterGridAttorClass',
                            cellClass: 'matterGridAttorClass',
                            headerTooltip: 'Click to sort by attorney',
                            displayName: vm.matterDashboardConfigs.GridColumn5Header,
                            cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.matterResponsibleAttorney}}</div>',
                            enableColumnMenu: false,
                            position: value.position,
                            visible: value.displayInDashboard
                        });

                    }
                }
                if (key == "matterSubAreaOfLaw") {
                    if (value.displayInUI == true && value.position != -1) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterDashboardConfigs.GridColumn4Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            enableColumnMenu: false,
                            width: "210",
                            visible: value.displayInDashboard,
                            position: value.position
                        });

                    }
                }
                if (key == "matterCreatedDate") {
                    if (value.displayInUI == true && value.position != -1) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterDashboardConfigs.GridColumn5Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            enableColumnMenu: false,
                            width: "170",
                            cellTemplate: '<div class="ui-grid-cell-contents" datefilter date="{{row.entity.matterCreatedDate}}"></div>',
                            position: value.position,
                            visible: value.displayInDashboard
                        });
                    }
                }
                if (key == "matterDescription" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterDashboardConfigs.GridColumn8Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "210",
                            position: value.position,
                            visible: value.displayInDashboard
                        });

                    }
                }
                if (key == "matterUrl" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterDashboardConfigs.GridColumn9Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "210",
                            position: value.position,
                            visible: value.displayInDashboard
                        });

                    }
                }
                if (key == "matterClientUrl" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterDashboardConfigs.GridColumn10Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "210",
                            position: value.position,
                            visible: value.displayInDashboard
                        });

                    }
                }
                if (key == "matterPracticeGroup" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterDashboardConfigs.GridColumn2Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "210",
                            enableColumnMenu: false,
                            position: value.position,
                            visible: value.displayInDashboard
                        });

                    }
                }

                if (key == "matterAreaOfLaw" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterDashboardConfigs.GridColumn3Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            enableColumnMenu: false,
                            width: "210",
                            position: value.position,
                            visible: value.displayInDashboard
                        });

                    }
                }
                if (key == "hideUpload" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterDashboardConfigs.GridColumn13Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "210",
                            position: value.position,
                            visible: value.displayInDashboard
                        });

                    }
                }
                if (key == "matterID" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterDashboardConfigs.GridColumn14Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "210",
                            position: value.position,
                            visible: value.displayInDashboard
                        });

                    }
                }
                if (key == "matterGuid" && value.position != -1) {
                    if (value.displayInUI == true) {
                        columnDefs1.push({
                            field: key,
                            displayName: vm.matterDashboardConfigs.GridColumn15Header,
                            headerCellClass: 'gridclass',
                            cellClass: 'gridclass',
                            width: "210",
                            position: value.position,
                            visible: value.displayInDashboard
                        });

                    }
                }

            });

            columnDefs1.push({
                field: 'pin',
                displayName: '',
                width: '50',
                cellTemplate: '<div class="ui-grid-cell-contents pad0" ><img ng-src="../Images/{{row.entity.pinType}}-666.png"  ng-click="grid.appScope.vm.pinorunpin($event, row.entity)"/></div>',
                enableColumnMenu: false,
                position: 75
            });
            columnDefs1.push({
                field: 'upload',
                displayName: '',
                width: '60',
                cellTemplate: '<div class="ui-grid-cell-contents pad0"><img src="../Images/upload-666.png" ng-click="grid.appScope.vm.Openuploadmodal(row.entity.matterName,row.entity.matterClientUrl,row.entity.matterGuid)"/></div>',
                enableColumnMenu: false,
                position: 76
            });
            function getSortFunction(fieldName) {
                return function (col1, col2) {
                    return parseInt(col1[fieldName]) - parseInt(col2[fieldName]);
                }
            }

            columnDefs1.sort(getSortFunction("position"));


            //#region Matter Grid functionality
            vm.matterGridOptions = {
                enableHorizontalScrollbar: 0,
                enableVerticalScrollbar: 0,
                enableSorting: false,
                paginationPageSize: gridOptions.paginationPageSize,
                enableGridMenu: gridOptions.enableGridMenu,
                enableRowHeaderSelection: gridOptions.enableRowHeaderSelection,
                enableRowSelection: gridOptions.enableRowSelection,
                enableSelectAll: gridOptions.enableSelectAll,
                multiSelect: gridOptions.multiSelect,
                enableFiltering: gridOptions.enableFiltering,
                columnDefs: columnDefs1,
                onRegisterApi: function (gridApi) {
                    vm.gridApi = gridApi;
                    //Set the selected row of the grid to selectedRow property of the controller
                    gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                        vm.selectedRow = row.entity
                    });
                }
            }
            //#endregion

            //#region API to get the client taxonomy and Practice Group taxonomy
            var optionsForClientGroup = {
                Client: {
                    Url: configs.global.repositoryUrl
                },
                TermStoreDetails: {
                    TermGroup: configs.taxonomy.termGroup,
                    TermSetName: configs.taxonomy.clientTermSetName,
                    CustomPropertyName: configs.taxonomy.clientCustomPropertiesURL,
                }
            };

            var optionsForPracticeGroup = {
                Client: {
                    Url: configs.global.repositoryUrl
                },
                TermStoreDetails: {
                    TermGroup: configs.taxonomy.termGroup,
                    TermSetName: configs.taxonomy.practiceGroupTermSetName,
                    CustomPropertyName: configs.taxonomy.subAreaOfLawCustomContentTypeProperty,
                    DocumentTemplatesName: configs.taxonomy.subAreaOfLawDocumentContentTypeProperty,
                }
            }

            function getTaxonomyDetailsForClient(optionsForClientGroup, callback) {
                api({
                    resource: 'matterDashBoardResource',
                    method: 'getTaxonomyDetails',
                    data: optionsForClientGroup,
                    success: callback
                });
            }

            function getTaxonomyDetailsForPractice(optionsForPracticeGroup, callback) {
                api({
                    resource: 'matterDashBoardResource',
                    method: 'getTaxonomyDetails',
                    data: optionsForPracticeGroup,
                    success: callback
                });
            }

            function getMatterCounts(searchRequest, callback) {
                api({
                    resource: 'matterDashBoardResource',
                    method: 'getMatterCounts',
                    data: searchRequest,
                    success: callback
                });
            }
            //#endregion

            //#region API to get matters for the selected criteria and bind data to grid
            //api for matter search
            function get(options, callback) {
                api({
                    resource: 'matterDashBoardResource',
                    method: 'get',
                    data: options,
                    success: callback
                });
            }

            //api to get pinned matters
            function getPinnedMatters(options, callback) {
                api({
                    resource: 'matterDashBoardResource',
                    method: 'getPinnedMatters',
                    data: options,
                    success: callback
                });
            }

            //Callback function for pin 
            function pinMatter(options, callback) {
                api({
                    resource: 'matterDashBoardResource',
                    method: 'PinMatter',
                    data: options,
                    success: callback
                });
            }


            //Callback function for unpin 
            function unpinMatter(options, callback) {
                api({
                    resource: 'matterDashBoardResource',
                    method: 'UnpinMatter',
                    data: options,
                    success: callback
                });
            }

            function getFolderHierarchy(options, callback) {
                api({
                    resource: 'matterDashBoardResource',
                    method: 'getFolderHierarchy',
                    data: options,
                    success: callback
                });
            }


            //SearchRequest Object that will be filled up for different search requirements
            var jsonMatterSearchRequest = {
                Client: {
                    Url: configs.global.repositoryUrl
                },
                SearchObject: {
                    PageNumber: 1,
                    ItemsPerPage: gridOptions.paginationPageSize,
                    SearchTerm: '',
                    Filters: {
                        ClientsList: [""],
                        PGList: [""],
                        AOLList: [""],
                        FromDate: "",
                        ToDate: "",
                        FilterByMe: 0
                    },
                    Sort: {
                        ByProperty: 'LastModifiedTime',
                        Direction: 1,
                        ByColumn: ""
                    }
                }
            };

            //#reion This function will get counts for all matters, my matters and pinned matters
            vm.getMatterCounts = function () {
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.displaypagination = false;
                getMatterCounts(jsonMatterSearchRequest, function (response) {
                    vm.allMatterCount = response.allMatterCounts;
                    vm.myMatterCount = response.myMatterCounts;
                    vm.pinMatterCount = response.pinnedMatterCounts;
                    vm.totalrecords = response.myMatterCounts;
                    //vm.selectedTabInfo = vm.matterDashboardConfigs.Tab1HeaderText + " (" + vm.myMatterCount + ")";
                    if (vm.selectedTab == vm.matterDashboardConfigs.Tab1HeaderText) {
                        vm.selectedTabInfo = vm.matterDashboardConfigs.Tab1HeaderText + " (" + response.myMatterCounts + ")";
                    } else if (vm.selectedTab == vm.matterDashboardConfigs.Tab2HeaderText) {
                        vm.selectedTabInfo = vm.matterDashboardConfigs.Tab2HeaderText + " (" + response.allMatterCounts + ")";
                    } else {
                        vm.selectedTabInfo = vm.matterDashboardConfigs.Tab3HeaderText + " (" + response.pinnedMatterCounts + ")";
                    }
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                    vm.pagination();
                    vm.lazyloaderdashboard = true;
                    vm.divuigrid = true;
                    vm.displaypagination = true;
                });
            }
            //#endregion

            //#region This api will get all matters which are pinned and this will be invoked when the user clicks on "Pinned Matters Tab"
            vm.getMatterPinned = function () {
                vm.tabClicked = "Pinned Matters";
                vm.selectedTab == "Pinned matters";
                vm.sortbytext = vm.matterDashboardConfigs.DrpDownOption1Text;
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.displaypagination = false;
                vm.nodata = false;
                var pinnedMattersRequest = {
                    Url: configs.global.repositoryUrl//ToDo: Read from config.js
                }
                getPinnedMatters(jsonMatterSearchRequest, function (response) {
                    if (response == "") {
                        vm.nodata = true;
                        vm.divuigrid = false;
                        vm.displaypagination = false;
                        vm.lazyloaderdashboard = true;
                    }
                    else {
                        var pinnedResponse = response;
                        if (response && response.length > 0) {
                            angular.forEach(response, function (res) {
                                res.pinType = "unpin"
                            })
                        }
                        vm.Pinnedobj = response
                        vm.matterGridOptions.data = response;
                        vm.totalrecords = vm.pinMatterCount;
                        vm.selectedTabCount = vm.pinMatterCount;
                        vm.pagination();
                        if (!$scope.$$phase) {
                            $scope.$apply();
                        }
                        vm.nodata = false;
                        vm.divuigrid = true;
                        vm.lazyloaderdashboard = true;
                        vm.displaypagination = true;
                    }
                });
            }
            //#endregion


            //#regionThis search function will be used when the user enters some text in the search text box and presses search button
            vm.searchMatters = function (val) {
                var finalSearchText = "";
                if (val != "") {
                    finalSearchText = "(" + configs.search.ManagedPropertyMatterName + ":" + val + "* OR " + configs.search.ManagedPropertyMatterId + ":" + val + "*)";
                }
                vm.pagenumber = 1;
                jsonMatterSearchRequest.SearchObject.PageNumber = vm.pagenumber;
                jsonMatterSearchRequest.SearchObject.SearchTerm = finalSearchText;
                jsonMatterSearchRequest.SearchObject.Sort.Direction = 1;
                return matterDashBoardResource.get(jsonMatterSearchRequest).$promise;
            }
            //#endregion

            //#region
            vm.typeheadselect = function (index, selected) {
                var searchToText = '';
                var finalSearchText = '';
                vm.displaypagination = false;
                if (selected != "") {
                    searchToText = selected.replace("(", ",")
                    searchToText = searchToText.replace(")", "")
                    var firstText = searchToText.split(',')[0]
                    var secondText = searchToText.split(',')[1]
                    var finalSearchText = "(" + configs.search.ManagedPropertyMatterName + ":" + val + "* OR " + configs.search.ManagedPropertyMatterId + ":" + val + "*)";
                }
                jsonMatterSearchRequest.SearchObject.SearchTerm = finalSearchText;
                jsonMatterSearchRequest.SearchObject.Sort.Direction = 0;
                vm.FilterByType();
            }

            //#endregion

            //#region for searching matters when entering text in serach box
            vm.searchText = "";
            vm.searchByTerm = function () {
                vm.lazyloaderdashboard = false;
                vm.displaypagination = false;
                vm.divuigrid = false;
                vm.matterid = 1;
                vm.mattername = "All Matters";
                vm.pagenumber = 1;
                var searchToText = '';
                var finalSearchText = '';
                if (vm.searchText != "") {
                    if (vm.searchText.indexOf("(") > -1) {
                        searchToText = vm.searchText.replace("(", ",")
                        searchToText = searchToText.replace(")", "")
                        var firstText = searchToText.split(',')[0]
                        var secondText = searchToText.split(',')[1]
                        finalSearchText = '(' + configs.search.ManagedPropertyMatterName + ':"' + firstText.trim() + '" AND ' + configs.search.ManagedPropertyMatterId + ':"' + secondText.trim() + '")'
                    } else {
                        finalSearchText = commonFunctions.searchFilter(vm.searchText);
                    }
                }
                jsonMatterSearchRequest.SearchObject.SearchTerm = finalSearchText;
                jsonMatterSearchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                jsonMatterSearchRequest.SearchObject.Sort.Direction = 1;
                get(jsonMatterSearchRequest, function (response) {
                    if (response == "") {
                        vm.totalrecords = response.length;
                        vm.getMatterCounts();
                        vm.matterGridOptions.data = response;
                        //vm.lazyloaderdashboard = true;
                        //vm.divuigrid = false;
                        vm.nodata = true;
                        vm.pagination();
                    } else {
                        vm.getMatterCounts();
                        vm.totalrecords = response.length;
                        vm.matterGridOptions.data = response;
                        //vm.divuigrid = true;
                        vm.nodata = false;
                        //vm.lazyloaderdashboard = true;
                        vm.pagination();
                    }
                });
            }
            //#endregion


            vm.myMatters = function () {
                vm.tabClicked = "My Matters";
                vm.selectedTab == "My matters";
                vm.sortbytext = vm.matterDashboardConfigs.DrpDownOption1Text;
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.displaypagination = false;
                vm.nodata = false;
                var searchToText = '';
                var finalSearchText = '';
                if (vm.searchText != "") {
                    if (vm.searchText.indexOf("(") > -1) {
                        searchToText = vm.searchText.replace("(", ",")
                        searchToText = searchToText.replace(")", "")
                        var firstText = searchToText.split(',')[0]
                        var secondText = searchToText.split(',')[1]
                        var finalSearchText = '(' + configs.search.ManagedPropertyMatterName + ':"' + firstText.trim() + '" AND ' + configs.search.ManagedPropertyMatterId + ':"' + secondText.trim() + '")'
                        //var finalSearchText = '(MCMatterName:"' + firstText.trim() + '" AND MCMatterID:"' + secondText.trim() + '")'
                    } else {
                        finalSearchText = commonFunctions.searchFilter(vm.searchText);
                    }
                }
                jsonMatterSearchRequest.SearchObject.SearchTerm = finalSearchText;
                jsonMatterSearchRequest.SearchObject.Filters.FilterByMe = 1;
                jsonMatterSearchRequest.SearchObject.PageNumber = 1;
                jsonMatterSearchRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                jsonMatterSearchRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                get(jsonMatterSearchRequest, function (response) {
                    if (response == "") {
                        vm.lazyloaderdashboard = true;
                        vm.divuigrid = false;
                        vm.displaypagination = false;
                        vm.nodata = true;
                    }
                    else {
                        getPinnedMatters(jsonMatterSearchRequest, function (pinnedResponse) {
                            if (pinnedResponse && pinnedResponse.length > 0) {
                                vm.Pinnedobj = pinnedResponse;
                                vm.pinMatterCount = vm.Pinnedobj.length
                                angular.forEach(pinnedResponse, function (pinobj) {
                                    angular.forEach(response, function (res) {
                                        //Check if the pinned matter name is equal to search matter name
                                        if (pinobj.matterName == res.matterName) {
                                            if (res.ismatterdone == undefined && !res.ismatterdone) {
                                                res.ismatterdone = true;
                                                res.pinType = "unpin"
                                            }
                                        }
                                    });
                                });
                                vm.matterGridOptions.data = response;
                                vm.totalrecords = vm.myMatterCount;
                                vm.selectedTabCount = vm.myMatterCount;
                                vm.pagination();
                                vm.lazyloaderdashboard = true;
                                vm.divuigrid = true;
                                vm.getMatterCounts();
                            }
                            else {
                                vm.lazyloaderdashboard = true;
                                vm.matterGridOptions.data = response;
                                vm.totalrecords = vm.myMatterCount;
                                vm.selectedTabCount = vm.myMatterCount;
                                vm.pagination();
                                vm.pinMatterCount = 0;
                                vm.divuigrid = true;
                                vm.getMatterCounts();
                            }
                        });
                    }
                });
            }

            //This search function will be used for binding search results to the grid
            vm.search = function (isMy) {
                vm.tabClicked = "All Matters";
                vm.selectedTab == "All matters";
                vm.sortbytext = vm.matterDashboardConfigs.DropDownOptionText;
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.displaypagination = false;
                vm.nodata = false;
                var searchToText = '';
                var finalSearchText = '';
                if (vm.searchText != "") {
                    if (vm.searchText.indexOf("(") > -1) {
                        searchToText = vm.searchText.replace("(", ",")
                        searchToText = searchToText.replace(")", "")
                        var firstText = searchToText.split(',')[0]
                        var secondText = searchToText.split(',')[1]
                        var finalSearchText = '(' + configs.search.ManagedPropertyMatterName + ':"' + firstText.trim() + '" AND ' + configs.search.ManagedPropertyMatterId + ':"' + secondText.trim() + '")'
                        //var finalSearchText = '(MCMatterName:"' + firstText.trim() + '" AND MCMatterID:"' + secondText.trim() + '")';
                    } else {
                        finalSearchText = commonFunctions.searchFilter(vm.searchText);
                    }
                }

                var pinnedMattersRequest = {
                    Url: configs.global.repositoryUrl
                }
                var tempMatters = [];
                jsonMatterSearchRequest.SearchObject.Filters.FilterByMe = 0;
                jsonMatterSearchRequest.SearchObject.PageNumber = 1;
                jsonMatterSearchRequest.SearchObject.Sort.ByProperty = sortPropertyForAllMatters;
                jsonMatterSearchRequest.SearchObject.Sort.Direction = 0;
                jsonMatterSearchRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                get(jsonMatterSearchRequest, function (response) {
                    //We need to call pinned api to determine whether a matter is pinned or not                    
                    getPinnedMatters(jsonMatterSearchRequest, function (pinnedResponse) {
                        if (pinnedResponse && pinnedResponse.length > 0) {
                            vm.Pinnedobj = pinnedResponse;
                            vm.pinMatterCount = vm.Pinnedobj.length
                            angular.forEach(pinnedResponse, function (pinobj) {
                                angular.forEach(response, function (res) {
                                    //Check if the pinned matter name is equal to search matter name
                                    if (pinobj.matterName == res.matterName) {
                                        if (res.ismatterdone == undefined && !res.ismatterdone) {
                                            res.ismatterdone = true;
                                            res.pinType = "unpin"
                                        }
                                    }
                                });
                            });
                            vm.matterGridOptions.data = response;
                            vm.totalrecords = vm.allMatterCount;
                            vm.selectedTabCount = vm.allMatterCount;
                            vm.pagination();
                            vm.lazyloaderdashboard = true;
                            vm.divuigrid = true;

                        }
                        else {
                            vm.lazyloaderdashboard = true;
                            vm.matterGridOptions.data = response;
                            vm.totalrecords = vm.allMatterCount;
                            vm.selectedTabCount = vm.allMatterCount;
                            vm.pagination();
                            vm.pinMatterCount = 0;
                            vm.divuigrid = true;
                        }
                    });

                });
            }


            //This function will pin or unpin the matter based on the image button clicked
            vm.pinorunpin = function (e, currentRowData) {
                //vm.lazyloaderdashboard = false;
                if (e.currentTarget.src.toLowerCase().indexOf("images/pin-666.png") > 0) {
                    e.currentTarget.src = "../Images/loadingGreen.gif";
                    var pinRequest = {
                        Client: {
                            Url: configs.global.repositoryUrl
                        },
                        matterData: {
                            matterName: currentRowData.matterName,
                            matterDescription: currentRowData.matterDescription,
                            matterCreatedDate: currentRowData.matterCreatedDate,
                            matterUrl: currentRowData.matterUrl,
                            matterPracticeGroup: currentRowData.matterPracticeGroup,
                            matterAreaOfLaw: currentRowData.matterAreaOfLaw,
                            matterSubAreaOfLaw: currentRowData.matterSubAreaOfLaw,
                            matterClientUrl: currentRowData.matterClientUrl,
                            matterClient: currentRowData.matterClient,
                            matterClientId: currentRowData.matterClientId,
                            hideUpload: currentRowData.hideUpload,
                            matterID: currentRowData.matterID,
                            matterResponsibleAttorney: currentRowData.matterResponsibleAttorney,
                            matterModifiedDate: currentRowData.matterModifiedDate,
                            matterGuid: currentRowData.matterGuid,
                            pinType: 'unpin'
                        }
                    }
                    pinMatter(pinRequest, function (response) {
                        if (response.isMatterPinned) {
                            e.currentTarget.src = "../images/unpin-666.png";
                            vm.pinMatterCount = parseInt(vm.pinMatterCount, 10) + 1;
                        }
                        //vm.lazyloaderdashboard = true;
                    });
                }
                else if (e.currentTarget.src.toLowerCase().indexOf("images/unpin-666.png") > 0) {
                    e.currentTarget.src = "../Images/loadingGreen.gif";
                    var unpinRequest = {
                        Client: {
                            Url: configs.global.repositoryUrl
                        },
                        matterData: {
                            matterName: currentRowData.matterUrl,
                        }
                    }
                    unpinMatter(unpinRequest, function (response) {
                        if (response.isMatterUnPinned) {

                            vm.pinMatterCount = parseInt(vm.pinMatterCount, 10) - 1;
                            if (vm.tabClicked.toLowerCase().indexOf("pinned") >= 0) {
                                e.currentTarget.src = "../images/unpin-666.png";
                                vm.matterGridOptions.data.splice(vm.matterGridOptions.data.indexOf(currentRowData), 1)
                            }
                            else {
                                e.currentTarget.src = "../images/pin-666.png";
                            }
                        }
                        //vm.lazyloaderdashboard = true;
                    });
                }

            }

            //#endregion 

            //#region Closing and Opening searchbar dropdowns
            vm.showupward = function ($event) {
                $event.stopPropagation();
                vm.searchdrop = true;
                vm.downwarddrop = false;
                vm.upwarddrop = true;
            }
            vm.showdownward = function ($event) {
                $event.stopPropagation();
                vm.searchdrop = false;
                vm.upwarddrop = false;
                vm.downwarddrop = true;
            }
            //#endregion

            //#region Showing and Hiding the sortby dropdown
            vm.showsortby = function ($event) {
                $event.stopPropagation();
                if (!vm.sortbydropvisible) {
                    vm.sortbydrop = true;
                    vm.sortbydropvisible = true;
                } else {
                    vm.sortbydrop = false;
                    vm.sortbydropvisible = false;
                }
            }
            //#endregion

            //#region Angular Datepicker Starts here
            //Start
            vm.dateOptions = {
                formatYear: 'yy',
                maxDate: new Date()
            };
            vm.enddateOptions = {
                formatYear: 'yy',
                maxDate: new Date()
            }
            $scope.$watch('vm.startdate', function (newval, oldval) {
                vm.enddateOptions.minDate = newval;
            });
            vm.openStartDate = function ($event) {
                if ($event) {
                    $event.preventDefault();
                    $event.stopPropagation();
                }
                this.openedStartDate = true;
            };
            vm.openEndDate = function ($event) {
                if ($event) {
                    $event.preventDefault();
                    $event.stopPropagation();
                }
                this.openedEndDate = true;
            };
            vm.openedStartDate = false;
            vm.openedEndDate = false;
            //#endregion

            //#region showing and hiding client dropdown
            vm.showClientDrop = function ($event) {
                $event.stopPropagation();
                if (!vm.clientdropvisible) {
                    if (vm.clients === undefined) {
                        vm.lazyloaderclient = false;
                        getTaxonomyDetailsForClient(optionsForClientGroup, function (response) {
                            vm.clients = response.clientTerms;
                            vm.clientdrop = true;
                            vm.clientdropvisible = true;
                            if (vm.selectedClients !== undefined && vm.selectedClients.length > 0) {
                                vm.customSelection(vm.matterDashboardConfigs.AdvSearchLabel1InternalFuncParamText);
                            }
                            vm.lazyloaderclient = true;
                        });
                    }
                    else {
                        if (vm.selectedClients !== undefined && vm.selectedClients.length > 0) {
                            vm.customSelection(vm.matterDashboardConfigs.AdvSearchLabel1InternalFuncParamText);
                        }
                        vm.clientdrop = true;
                        vm.clientdropvisible = true;
                    }
                    vm.pgdrop = false;
                    vm.pgdropvisible = false;
                    vm.aoldrop = false;
                    vm.aoldropvisible = false;

                } else if (vm.clientdropvisible && $event.type === "keyup") {
                    vm.customSelection(vm.matterDashboardConfigs.AdvSearchLabel1InternalFuncParamText);
                } else {
                    vm.clientdrop = false;
                    vm.clientdropvisible = false;
                    vm.pgdrop = false;
                    vm.pgdropvisible = false;
                    vm.aoldrop = false;
                    vm.aoldropvisible = false;
                    vm.lazyloaderclient = true;
                }
            }

            //#region showing and hiding practice group dropdown
            vm.showPracticegroupDrop = function ($event) {
                $event.stopPropagation();
                if (!vm.pgdropvisible) {
                    if ((vm.practiceGroups === undefined) && (vm.aolTerms === undefined)) {
                        vm.lazyloaderpg = false;
                        getTaxonomyDetailsForPractice(optionsForPracticeGroup, function (response) {
                            vm.practiceGroups = response.level1;
                            vm.aolTerms = [];
                            if (!vm.globalSettings.isBackwardCompatible) {
                                angular.forEach(response.level1, function (pgTerm) {
                                    angular.forEach(pgTerm.level2, function (areaterm) {
                                        vm.aolTerms.push(areaterm);
                                    });
                                });
                            } else {
                                angular.forEach(response.level1, function (pgTerm) {
                                    angular.forEach(pgTerm.level2, function (areaterm) {
                                        angular.forEach(areaterm.level3, function (term) {
                                            vm.aolTerms.push(term);
                                        });
                                    });
                                });
                            }
                            vm.pgdrop = true;
                            vm.pgdropvisible = true;
                            if (vm.selectedPGs !== undefined && vm.selectedPGs.length > 0) {
                                vm.customSelection(vm.matterDashboardConfigs.AdvSearchLabel2InternalFuncParamText);
                            }
                            vm.lazyloaderpg = true;
                        });
                    }
                    else {
                        if (vm.selectedPGs !== undefined && vm.selectedPGs.length > 0) {
                            vm.customSelection(vm.matterDashboardConfigs.AdvSearchLabel2InternalFuncParamText);
                        }
                        vm.pgdrop = true;
                        vm.pgdropvisible = true;
                    }
                    vm.clientdrop = false;
                    vm.clientdropvisible = false;
                    vm.aoldrop = false;
                    vm.aoldropvisible = false;
                } else if (vm.pgdropvisible && $event.type === "keyup") {
                    vm.customSelection(vm.matterDashboardConfigs.AdvSearchLabel2InternalFuncParamText);
                } else {
                    vm.clientdrop = false;
                    vm.clientdropvisible = false;
                    vm.pgdrop = false;
                    vm.pgdropvisible = false;
                    vm.aoldrop = false;
                    vm.aoldropvisible = false;
                    vm.lazyloaderpg = true;
                }
            }
            //#endregion

            //#region showing and hiding area of law dropdown
            vm.showAreaofLawDrop = function ($event) {
                $event.stopPropagation();
                if (!vm.aoldropvisible) {
                    if ((vm.practiceGroups === undefined) && (vm.aolTerms === undefined)) {
                        vm.lazyloaderaol = false;
                        getTaxonomyDetailsForPractice(optionsForPracticeGroup, function (response) {
                            vm.practiceGroups = response.level1;
                            vm.aolTerms = [];
                            angular.forEach(response.level1, function (pgTerm) {
                                angular.forEach(pgTerm.level2, function (areaterm) {
                                    vm.aolTerms.push(areaterm);
                                });
                            })
                            vm.aoldrop = true;
                            vm.aoldropvisible = true;
                            if (vm.selectedAOLs !== undefined && vm.selectedAOLs.length > 0) {
                                vm.customSelection(vm.matterDashboardConfigs.AdvSearchLabel3InternalFuncParamText);
                            }
                            vm.lazyloaderaol = true;
                        });
                    }
                    else {
                        if (vm.selectedAOLs !== undefined && vm.selectedAOLs.length > 0) {
                            vm.customSelection(vm.matterDashboardConfigs.AdvSearchLabel3InternalFuncParamText);
                        }
                        vm.aoldrop = true;
                        vm.aoldropvisible = true;
                    }
                    vm.clientdrop = false;
                    vm.clientdropvisible = false;
                    vm.pgdrop = false;
                    vm.pgdropvisible = false;
                } else if (vm.aoldropvisible && $event.type === "keyup") {
                    vm.customSelection(vm.matterDashboardConfigs.AdvSearchLabel3InternalFuncParamText);
                } else {
                    vm.clientdrop = false;
                    vm.clientdropvisible = false;
                    vm.pgdrop = false;
                    vm.pgdropvisible = false;
                    vm.aoldrop = false;
                    vm.aoldropvisible = false;
                    vm.lazyloaderaol = true;
                }
            }
            //#endregion          

            //#Region : Function handle the keyup events in advanced search to check and unchecked user selection.
            vm.customSelection = function (type) {

                if (type !== undefined && type === vm.matterDashboardConfigs.AdvSearchLabel1InternalFuncParamText) {
                    var selectdClients = vm.selectedClients.split(',');  //user altered text value
                    angular.forEach(vm.clients, function (client) {
                        client.Selected = false;
                        angular.forEach(selectdClients, function (clientInput) {
                            if (clientInput.toString().length > 0 && client.name.toString().toLowerCase().indexOf(clientInput.toString().toLowerCase()) !== -1) {
                                client.Selected = true;
                            }
                        })
                    });
                } else if (type !== undefined && type === vm.matterDashboardConfigs.AdvSearchLabel2InternalFuncParamText) {
                    var selectdPGs = vm.selectedPGs.split(',');  //user altered text value
                    angular.forEach(vm.practiceGroups, function (pgGroup) {
                        pgGroup.Selected = false;
                        angular.forEach(selectdPGs, function (pgInput) {
                            if (pgInput.toString().length > 0 && pgGroup.termName.toString().toLowerCase().indexOf(pgInput.toString().toLowerCase()) !== -1) {
                                pgGroup.Selected = true;
                            }
                        })
                    });
                } else if (type !== undefined && type === vm.matterDashboardConfigs.AdvSearchLabel3InternalFuncParamText) {
                    var selectedAOLs = vm.selectedAOLs.split(',');  //user altered text value
                    angular.forEach(vm.aolTerms, function (aol) {
                        aol.Selected = false;
                        angular.forEach(selectedAOLs, function (aolInput) {
                            if (aolInput.toString().length > 0 && aol.termName.toString().toLowerCase().indexOf(aolInput.toString().toLowerCase()) !== -1) {
                                aol.Selected = true;
                            }
                        })
                    });
                }
            }
            //#endregion

            //#region This event is going to file when the user clicks onm "Select All" and "UnSelect All" links
            vm.checkAll = function (checkAll, type, $event) {
                $event.stopPropagation();
                if (type === vm.matterDashboardConfigs.AdvSearchLabel1InternalFuncParamText) {
                    angular.forEach(vm.clients, function (client) {
                        client.Selected = checkAll;
                    });
                }
                if (type === vm.matterDashboardConfigs.AdvSearchLabel2InternalFuncParamText) {
                    angular.forEach(vm.practiceGroups, function (pg) {
                        pg.Selected = checkAll;
                    });
                }
                if (type === vm.matterDashboardConfigs.AdvSearchLabel3InternalFuncParamText) {
                    angular.forEach(vm.aolTerms, function (aol) {
                        aol.Selected = checkAll;
                    });
                }
            }

            //#region This event is going to fire when the user clicks on "OK" button in the filter panel
            vm.filterSearchOK = function (type) {
                if (type === vm.matterDashboardConfigs.AdvSearchLabel1InternalFuncParamText) {
                    vm.selectedClients = '';
                    angular.forEach(vm.clients, function (client) {
                        if (client.Selected) {
                            vm.selectedClients = vm.selectedClients + client.name + ","
                        }
                    });
                    vm.selectedClients = vm.selectedClients.slice(0, vm.selectedClients.length - 1);
                    vm.selectedClientsForCancel = vm.selectedClients;
                    vm.clientdrop = false;
                    vm.clientdropvisible = false;
                }
                if (type === vm.matterDashboardConfigs.AdvSearchLabel2InternalFuncParamText) {
                    vm.selectedPGs = '';
                    vm.selectedAOLs = '';
                    angular.forEach(vm.practiceGroups, function (pg) {
                        if (pg.Selected) {
                            vm.selectedPGs = vm.selectedPGs + pg.termName + ","
                            //For each of the selected pg's select corresponding aol check boxes automatically and update the aol
                            //textbox accordingly
                            angular.forEach(pg.level2, function (areaterm) {
                                areaterm.Selected = true;
                                vm.selectedAOLs = vm.selectedAOLs + areaterm.termName + ","
                            });
                        }
                    });
                    vm.selectedPGs = vm.selectedPGs.slice(0, vm.selectedPGs.length - 1);
                    vm.selectedAOLs = vm.selectedAOLs.slice(0, vm.selectedAOLs.length - 1);
                    if (vm.selectedPGs == "") {
                        angular.forEach(vm.aolTerms, function (aol) {
                            aol.Selected = false;
                        });
                    }
                    vm.selectedPGsForCancel = vm.selectedPGs;
                    vm.selectedAOLsForCancel = vm.selectedAOLs;
                    vm.pgdrop = false;
                    vm.pgdropvisible = false;
                }

                if (type === vm.matterDashboardConfigs.AdvSearchLabel3InternalFuncParamText) {
                    vm.selectedAOLs = '';
                    angular.forEach(vm.aolTerms, function (aol) {
                        if (aol.Selected) {
                            vm.selectedAOLs = vm.selectedAOLs + aol.termName + ","
                        }
                    });
                    vm.selectedAOLs = vm.selectedAOLs.slice(0, vm.selectedAOLs.length - 1);
                    vm.selectedAOLsForCancel = vm.selectedAOLs;
                    vm.aoldrop = false;
                    vm.aoldropvisible = false;
                }
            }
            //#endregion

            //#region This event is going to fire when the user clicks on "Cancel" button in the filter panel
            vm.filterSearchCancel = function (type) {
                if (type !== undefined && type === vm.matterDashboardConfigs.AdvSearchLabel1InternalFuncParamText) {
                    if (vm.selectedClientsForCancel !== undefined && vm.selectedClientsForCancel.toString().length > 0) {
                        vm.selectedClients = vm.selectedClientsForCancel;
                        angular.forEach(vm.clients, function (client) {
                            if (vm.selectedClients.indexOf(client.name) > 0) {
                                client.Selected = true;
                            }
                        });
                    }
                }
                if (type === vm.matterDashboardConfigs.AdvSearchLabel2InternalFuncParamText) {
                    if (vm.selectedPGsForCancel !== undefined && vm.selectedPGsForCancel.toString().length > 0) {
                        vm.selectedPGs = vm.selectedPGsForCancel;
                        angular.forEach(vm.practiceGroups, function (pg) {
                            if (vm.selectedPGs.indexOf(pg.termName) > 0) {
                                pg.Selected = true;
                            }
                        });
                    }
                }
                if (type === vm.matterDashboardConfigs.AdvSearchLabel3InternalFuncParamText) {
                    if (vm.selectedAOLsForCancel !== undefined && vm.selectedAOLsForCancel.toString().length > 0) {
                        vm.selectedAOLs = vm.selectedAOLsForCancel;
                        angular.forEach(vm.aolTerms, function (aol) {
                            if (vm.selectedAOLs.indexOf(aol.termName) > 0) {
                                aol.Selected = true;
                            }
                        });
                    }
                }
                vm.clientdrop = false;
                vm.clientdropvisible = false;
                vm.pgdrop = false;
                vm.pgdropvisible = false;
                vm.aoldrop = false;
                vm.aoldropvisible = false;
            }
            //#endregion
            //#region File upload functionality
            vm.Openuploadmodal = function (matterName, matterUrl, matterGUID) {
                vm.getFolderHierarchy(matterName, matterUrl, matterGUID);
                vm.oUploadGlobal.successBanner = false;
                vm.isLoadingFromDesktopStarted = false;
            }

            vm.getFolderHierarchy = function (matterName, matterUrl, matterGUID) {

                if ((matterName && matterName !== "") && (matterUrl && matterUrl !== "") && (matterGUID && matterGUID !== "")) {

                    vm.selectedRow.matterName = matterName;
                    vm.selectedRow.matterClientUrl = matterUrl;
                    vm.selectedRow.matterGuid = matterGUID;
                }


                vm.allAttachmentDetails = [];
                var matterData = {
                    MatterName: vm.selectedRow.matterName,
                    MatterUrl: vm.selectedRow.matterClientUrl
                };
                vm.getContentCheckConfigurations(vm.selectedRow.matterClientUrl);
                getFolderHierarchy(matterData, function (response) {
                    vm.foldersList = response.foldersList;
                    vm.uploadedFiles = [];

                    function getNestedChildren(arr, parent) {
                        var parentList = []
                        for (var i in arr) {
                            if (arr[i].parentURL == parent) {
                                var children = getNestedChildren(arr, arr[i].url)

                                if (children.length) {
                                    arr[i].children = children;
                                    arr[i].active = parent == null ? true : false;
                                }

                                parentList.push(arr[i]);

                            }
                        }
                        return parentList
                    }

                    vm.foldersList = getNestedChildren(vm.foldersList, null);
                    if (vm.foldersList[0] !== null) { vm.showSelectedFolderTree(vm.foldersList[0]); }

                    jQuery('#UploadMatterModal').modal("show");
                    //Initialize Officejs library                     
                    //Office.initialize = function (reason) {
                    //     vm.initOutlook();
                    //};
                    //vm.initOutlook();
                    vm.lazyloader = true;
                });
            }

            //This function will handle the files that has been dragged from the user desktop
            vm.ducplicateSourceFile = [];
            vm.handleDesktopDrop = function (targetDropUrl, sourceFiles, isOverwrite) {
                vm.oUploadGlobal.successBanner = false;
                vm.isLoadingFromDesktopStarted = true;
                // vm.files = sourceFiles.files;
                var fd = new FormData();
                fd.append('targetDropUrl', targetDropUrl);
                fd.append('folderUrl', targetDropUrl)
                fd.append('documentLibraryName', vm.selectedRow.matterName)
                fd.append('clientUrl', vm.selectedRow.matterClientUrl);
                fd.append('AllowContentCheck', vm.oUploadGlobal.bAllowContentCheck);
                var nCount = 0;
                angular.forEach(sourceFiles, function (file) {
                    fd.append('file', file);
                    fd.append("Overwrite" + nCount++, isOverwrite);
                });

                $http.post("/api/v1/document/uploadfiles", fd, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined },
                    timeout: vm.oUploadGlobal.canceler.promise
                }).then(function (response) {
                    vm.isLoadingFromDesktopStarted = false;
                    if (response.status == 200) {
                        if (response.data.length !== 0) {
                            var tempFile = [];
                            for (var i = 0; i < response.data.length; i++) {
                                if (!response.data[i].isError) {
                                    response.data[i].dropFolder = response.data[i].dropFolder == vm.selectedRow.matterGuid ? vm.selectedRow.matterName : response.data[i].dropFolder;
                                    vm.uploadedFiles.push(response.data[i]);
                                    tempFile.push(response.data[i]);
                                    vm.oUploadGlobal.successBanner = (tempFile.length == sourceFiles.length) ? true : false;

                                } else {
                                    vm.IsDupliacteDocument = true;
                                    if (response.data[i].value.split("|")[1]) {
                                        response.data[i].contentCheck = response.data[i].value.split("|")[1];
                                        response.data[i].saveLatestVersion = "True";
                                        response.data[i].cancel = "True";
                                        response.data[i].append = vm.overwriteConfiguration(response.data[i].fileName);
                                        response.data[i].value = response.data[i].value.split("|")[0];
                                        vm.ducplicateSourceFile.push(response.data[i]);
                                        vm.oUploadGlobal.arrFiles.push(vm.files[i]);
                                        vm.oUploadGlobal.successBanner = false;
                                    }
                                    else {
                                        var file = $filter("filter")(vm.ducplicateSourceFile, response.data[i].fileName);
                                        file[0].value = file[0].value + "<br/><br/>" + response.data[i].value;
                                        file[0].saveLatestVersion = "True";
                                        file[0].cancel = "True";
                                        file[0].contentCheck = "False";

                                    }
                                }
                            }

                        }
                    } else {
                        //To Do error handling implementation
                    }
                }).catch(function (response) {
                    vm.isLoadingFromDesktopStarted = false;
                    console.error('Gists error', response.status, response.data);
                })

            }
            vm.uploadedFiles = [];
            //#endregion

            //Call search api on page load
            //$interval(function () { vm.getMatterCounts(); }, 800, 3);
            $timeout(function () { vm.myMatters() }, 500);


            //#region For Sorting by Alphebatical or Created date

            vm.FilterByType = function () {
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.displaypagination = false;
                vm.nodata = false;
                if (vm.tabClicked === "Pinned Matters") {
                    getPinnedMatters(jsonMatterSearchRequest, function (response) {
                        if (response == "") {
                            vm.nodata = true;
                            vm.divuigrid = false;
                            vm.displaypagination = false;
                            vm.lazyloaderdashboard = true;
                        }
                        else {
                            var pinnedResponse = response;
                            if (response && response.length > 0) {
                                angular.forEach(response, function (res) {
                                    res.pinType = "unpin"
                                })
                            }
                            vm.Pinnedobj = response
                            vm.matterGridOptions.data = response;
                            vm.totalrecords = vm.pinMatterCount;
                            vm.selectedTabCount = vm.pinMatterCount;
                            vm.pagination();
                            if (!$scope.$$phase) {
                                $scope.$apply();
                            }
                            vm.nodata = false;
                            vm.divuigrid = true;
                            vm.lazyloaderdashboard = true;
                            vm.displaypagination = true;
                        }
                    });
                }
                else {
                    get(jsonMatterSearchRequest, function (response) {
                        vm.lazyloader = true;
                        if (response == "") {
                            vm.divuigrid = false;
                            vm.nodata = true;
                            vm.lazyloaderdashboard = true;
                            vm.getMatterCounts();
                            vm.pagination();
                            $scope.errorMessage = response.message;
                        } else {
                            vm.matterGridOptions.data = response;
                            vm.divuigrid = true;
                            vm.nodata = false;
                            vm.lazyloaderdashboard = true;
                            vm.getMatterCounts();
                            vm.pagination();
                            if (!$scope.$$phase) {
                                $scope.$apply();
                            }
                        }
                    });
                }
            }


            vm.sortExpression = function (byProperty, byColumn, sortDirection) {
                jsonMatterSearchRequest.SearchObject.Sort.ByProperty = byProperty;
                jsonMatterSearchRequest.SearchObject.Sort.Direction = sortDirection;
                jsonMatterSearchRequest.SearchObject.Sort.ByColumn = byColumn;
                vm.FilterByType();
            }

            vm.sortby = function (sortexp, data) {
                vm.sortbytext = data;
                vm.sortbydrop = false;
                if (vm.tabClicked !== "Pinned Matters") {
                    if (sortexp == 'AlphabeticalUp') {
                        vm.sortExpression(vm.configSearchContent.ManagedPropertyMatterName, vm.configSearchContent.ManagedPropertyMatterName, 0);
                    }
                    else if (sortexp == 'AlphabeticalDown') {
                        vm.sortExpression(vm.configSearchContent.ManagedPropertyMatterName, vm.configSearchContent.ManagedPropertyMatterName, 1);
                    }
                    else if (sortexp == 'CreateddateUp') {
                        vm.sortExpression(vm.configSearchContent.ManagedPropertyOpenDate, vm.configSearchContent.ManagedPropertyOpenDate, 0);
                    }
                    else if (sortexp == 'CreateddateDown') {
                        vm.sortExpression(vm.configSearchContent.ManagedPropertyOpenDate, vm.configSearchContent.ManagedPropertyOpenDate, 1);
                    }
                    else {
                        vm.sortExpression(vm.configSearchContent.ManagedPropertyLastModifiedTime, vm.configSearchContent.ManagedPropertyLastModifiedTime, 1);
                    }
                }
                else {
                    if (sortexp == 'AlphabeticalUp') {
                        vm.sortExpression("MatterName", "MatterName", 0);
                    }
                    else if (sortexp == 'AlphabeticalDown') {
                        vm.sortExpression("MatterName", "MatterName", 1);
                    }
                    else if (sortexp == 'CreateddateUp') {
                        vm.sortExpression("MatterCreatedDate", "MatterCreatedDate", 0);
                    }
                    else if (sortexp == 'CreateddateDown') {
                        vm.sortExpression("MatterCreatedDate", "MatterCreatedDate", 1);
                    }
                    else {
                        vm.sortExpression("MatterModifiedDate", "MatterModifiedDate", 1);
                    }
                }
            }

            //#endregion

            //#region Pagination

            vm.first = 1;
            vm.last = gridOptions.paginationPageSize;
            vm.total = 0;
            vm.pagenumber = 1;
            vm.fromtopage = vm.first + " - " + vm.last;
            vm.displaypagination = false;

            vm.pagination = function () {
                vm.first = 1;
                vm.last = gridOptions.paginationPageSize;
                vm.total = 0;
                vm.pagenumber = 1;
                vm.fromtopage = vm.first + " - " + vm.last;
                vm.displaypagination = false;
                vm.total = vm.totalrecords - gridOptions.paginationPageSize;
                if (vm.totalrecords > gridOptions.paginationPageSize) {
                    vm.fromtopage = vm.first + " - " + vm.last;
                }
                else {
                    if (vm.total < gridOptions.paginationPageSize) { vm.fromtopage = vm.first + " - " + vm.totalrecords; } else {
                        vm.fromtopage = vm.first + " - " + vm.last;
                    }
                }

                if (vm.totalrecords == 0) {
                    vm.displaypagination = false;
                } else {
                    vm.displaypagination = true;
                }
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            };

            vm.next = function () {
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.displaypagination = false;
                vm.nodata = false;
                if (vm.last < vm.totalrecords) {
                    vm.first = vm.first + gridOptions.paginationPageSize;
                    vm.last = vm.last + gridOptions.paginationPageSize;
                    vm.total = vm.totalrecords - gridOptions.paginationPageSize;
                    if (vm.last > vm.totalrecords) {
                        vm.fromtopage = vm.first + " - " + vm.totalrecords;
                    } else {
                        vm.fromtopage = vm.first + " - " + vm.last;
                    }
                    vm.pagenumber = vm.pagenumber + 1;
                    jsonMatterSearchRequest.SearchObject.PageNumber = vm.pagenumber;
                    jsonMatterSearchRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                    get(jsonMatterSearchRequest, function (response) {
                        vm.lazyloaderdashboard = true;
                        if (response == "") {
                            vm.divuigrid = false;
                            vm.nodata = true;
                            vm.lazyloaderdashboard = true;
                            vm.displaypagination = true;
                        } else {
                            getPinnedMatters(jsonMatterSearchRequest, function (pinnedResponse) {
                                if (pinnedResponse && pinnedResponse.length > 0) {
                                    vm.Pinnedobj = pinnedResponse;
                                    vm.pinMatterCount = vm.Pinnedobj.length
                                    angular.forEach(pinnedResponse, function (pinobj) {
                                        angular.forEach(response, function (res) {
                                            //Check if the pinned matter name is equal to search matter name
                                            if (pinobj.matterName == res.matterName) {
                                                if (res.ismatterdone == undefined && !res.ismatterdone) {
                                                    res.ismatterdone = true;
                                                    res.pinType = "unpin"
                                                }
                                            }
                                        });
                                    });
                                    vm.matterGridOptions.data = response;
                                    vm.lazyloaderdashboard = true;
                                    vm.divuigrid = true;

                                }
                                else {
                                    vm.lazyloaderdashboard = true;
                                    vm.pinMatterCount = 0;
                                    vm.divuigrid = true;
                                }
                                vm.displaypagination = true;
                            });
                            if (!$scope.$$phase) {
                                $scope.$apply();
                            }
                        }
                    });
                } else {
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                }
            };

            vm.prev = function () {
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.displaypagination = false;
                vm.nodata = false;
                if (vm.last > gridOptions.paginationPageSize) {
                    vm.first = vm.first - gridOptions.paginationPageSize;
                    vm.last = vm.last - gridOptions.paginationPageSize;
                    vm.pagenumber = vm.pagenumber - 1;
                    vm.fromtopage = vm.first + " - " + vm.last;
                    jsonMatterSearchRequest.SearchObject.PageNumber = vm.pagenumber;
                    jsonMatterSearchRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                    get(jsonMatterSearchRequest, function (response) {
                        vm.lazyloaderdashboard = true;
                        if (response == "") {
                            vm.divuigrid = false;
                            vm.nodata = true;
                            vm.lazyloaderdashboard = true;
                            vm.displaypagination = true;
                        } else {
                            getPinnedMatters(jsonMatterSearchRequest, function (pinnedResponse) {
                                if (pinnedResponse && pinnedResponse.length > 0) {
                                    vm.Pinnedobj = pinnedResponse;
                                    vm.pinMatterCount = vm.Pinnedobj.length
                                    angular.forEach(pinnedResponse, function (pinobj) {
                                        angular.forEach(response, function (res) {
                                            //Check if the pinned matter name is equal to search matter name
                                            if (pinobj.matterName == res.matterName) {
                                                if (res.ismatterdone == undefined && !res.ismatterdone) {
                                                    res.ismatterdone = true;
                                                    res.pinType = "unpin"
                                                }
                                            }
                                        });
                                    });
                                    vm.matterGridOptions.data = response;
                                    vm.lazyloaderdashboard = true;
                                    vm.divuigrid = true;

                                }
                                else {
                                    vm.lazyloaderdashboard = true;
                                    vm.pinMatterCount = 0;
                                    vm.divuigrid = true;
                                }
                                vm.displaypagination = true;
                            });
                            if (!$scope.$$phase) {
                                $scope.$apply();
                            }
                        }
                    });
                } else {
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                }
            };

            //#endregion

            //#region 
            vm.showclients = function ($event) {
                $event.stopPropagation();
                if (vm.clients === undefined) {
                    getTaxonomyDetailsForClient(optionsForClientGroup, function (response) {
                        vm.clients = response.clientTerms;
                    });
                }
                vm.clientdrop = true;
                vm.clientdropvisible = true;
            }

            //endregion


            //#region upload desktop files functionality starts
            vm.oUploadGlobal = {
                regularInvalidCharacter: new RegExp("[\*\?\|\\\t/:\"\"'<>#{}%~&]", "g"),
                regularStartEnd: new RegExp("^[\. ]|[\. ]$", "g"),
                regularExtraSpace: new RegExp(" {2,}", "g"),
                regularInvalidRule: new RegExp("[\.]{2,}", "g"),
                oUploadParameter: [],
                sClientRelativeUrl: "",
                sFolderUrl: "",
                arrContent: [],
                arrFiles: [],
                arrOverwrite: [],
                src: [],
                iActiveUploadRequest: 0,
                oDrilldownParameter: { nCurrentLevel: 0, sCurrentParentUrl: "", sRootUrl: "" },
                sNotificationMsg: "",
                bAppendOptionEnabled: false,
                oXHR: new XMLHttpRequest(),
                bIsAbortedCC: false,
                bAllowContentCheck: false,
                canceler: $q.defer(),
                successBanner: false
            };
            $rootScope.breadcrumb = true;
            $rootScope.foldercontent = false;
            vm.hideBreadCrumb = function () {
                $rootScope.breadcrumb = true;
                $rootScope.foldercontent = false;

            }

            //#region To getContentCheckConfigurations
            //start

            function getContentCheckConfigurations(options, callback) {
                api({
                    resource: 'matterResource',
                    method: 'getDefaultMatterConfigurations',
                    data: options,
                    success: callback
                });
            }

            vm.getContentCheckConfigurations = function (siteCollectionPath) {
                siteCollectionPath = JSON.stringify(siteCollectionPath);
                getContentCheckConfigurations(siteCollectionPath, function (response) {
                    if (!response.isError) {
                        var defaultMatterConfig = JSON.parse(response.code);
                        vm.oUploadGlobal.bAllowContentCheck = defaultMatterConfig.IsContentCheck;

                    } else {
                        vm.oUploadGlobal.bAllowContentCheck = false;
                    }

                });

            }

            //#region To expand and collapse the folder tree structure in upload
            vm.showSelectedFolderTree = function (folder) {
                function setActiveItem(item) {
                    if (item.children !== null) {
                        angular.forEach(item.children, function (child) {
                            if (item.parentURL !== null) {
                                if (item.active) {
                                    child.active = child.active ? false : true;
                                    if (!child.active) { setActiveItem(child); }
                                } else {
                                    child.active = false;
                                    setActiveItem(child);
                                }
                            }
                            else {
                                child.active = child.active ? false : true;
                                if (!child.active) {
                                    setActiveItem(child);
                                }
                            }
                        });
                    }

                }
                setActiveItem(folder);

            }
            //#endRegion
            //#region To do contentcheck or save as latestversion
            vm.localOverWriteDocument = function (duplicateFile, sOperation) {
                if ("contentCheck" === sOperation) {
                    vm.files = [vm.oUploadGlobal.arrFiles[vm.oUploadGlobal.arrFiles.length - 1]];
                } else {
                    vm.files = [vm.oUploadGlobal.arrFiles.pop()];
                    duplicateFile.cancel = null;
                }

                var nOperation = "";
                if ("ignore" !== sOperation) {
                    switch (sOperation) {
                        case "overwrite":
                            nOperation = "0";
                            break;
                        case "append":
                            nOperation = "1";
                            break;
                        case "contentCheck":
                            nOperation = "2";
                            break;
                        case "cancelContentCheck":
                            nOperation = "3";
                            break;
                    }
                    // uploadFile(oUploadGlobal.sClientRelativeUrl, oUploadGlobal.sFolderUrl, nOperation);

                    vm.handleDesktopDrop(vm.clientRelativeUrl, vm.files, nOperation);



                } else {
                    duplicateFile.cancel = "False";
                    if (vm.ducplicateSourceFile.length > 0) {
                        vm.ducplicateSourceFile.pop();
                    }
                }
            }

            // Function to configure time stamp
            vm.overwriteConfiguration = function (fileName) {
                // Update the content as per the logic.
                var selectedOverwriteConfiguration = configs.uploadMessages.overwrite_Config_Property.trim().toLocaleUpperCase(),
                    fileExtension = fileName.trim().substring(fileName.trim().lastIndexOf(".") + 1),
                    bAppendEnabled = false;

                switch (selectedOverwriteConfiguration) {
                    case "BOTH":
                        bAppendEnabled = true;
                        break;
                    case "DOCUMENT ONLY":
                        bAppendEnabled = "eml" === fileExtension || "msg" === fileExtension ? false : true;
                        break;
                    default:
                        bAppendEnabled = "eml" === fileExtension || "msg" === fileExtension ? true : false;
                        break;
                }
                return bAppendEnabled;
            }

            vm.contentCheckNotification = function (file, isLocalUpload) {
                file.contentCheck = "contentCheck";
                file.saveLatestVersion = "False";
                file.cancel = "False";

            }
            vm.abortContentCheck = function (file, isLocalUpload) {
                "use strict";
                if (isLocalUpload) {
                    vm.oUploadGlobal.canceler.resolve();
                    vm.oUploadGlobal.canceler = $q.defer();
                }
                file.contentCheck = null;
                file.saveLatestVersion = "True";
                file.value = file.value + "<br/><div>" + configs.uploadMessages.content_Check_Abort + "</div>";
                file.cancel = "True";

            }

            vm.closeSuccessBanner = function () {
                vm.oUploadGlobal.successBanner = false;
            }
            //#end region

            //#region performs action when clicked on the search button in header flyout
            vm.getSearchResults = function () {
                angular.element('#allMatters').addClass("active");
                angular.element('#myMatters').removeClass("active");
                angular.element('#pinMatters').removeClass("active");
                vm.lazyloaderdashboard = false;
                vm.divuigrid = false;
                vm.displaypagination = false;
                vm.nodata = false;
                vm.searchdrop = false;
                var clientArray = [];
                var aolListarray = [];
                var pglistArray = [];
                var startdate = "";
                var enddate = "";
                if (vm.selectedClients != "" && vm.selectedClients != undefined) {
                    clientArray = vm.selectedClients.split(',');
                }
                if (vm.selectedPGs != "" && vm.selectedPGs != undefined) {
                    pglistArray = vm.selectedPGs.split(',');
                }
                if (vm.selectedAOLs != "" && vm.selectedAOLs != undefined) {
                    aolListarray = vm.selectedAOLs.split(',');
                }
                if (vm.selectedAOLs != "" && vm.selectedAOLs != undefined) {
                    aolListarray = vm.selectedAOLs.split(',');
                }
                if (vm.startdate != "" && vm.startdate != undefined) {
                    startdate = vm.startdate.format("yyyy-MM-ddT00:00:00Z");
                }
                if (vm.enddate != "" && vm.enddate != undefined) {
                    enddate = vm.enddate.format("yyyy-MM-ddT23:59:59Z");
                }
                jsonMatterSearchRequest.SearchObject.Filters.FilterByMe = 0;
                jsonMatterSearchRequest.SearchObject.Filters.ClientsList = clientArray;
                jsonMatterSearchRequest.SearchObject.Filters.PGList = pglistArray;
                jsonMatterSearchRequest.SearchObject.Filters.AOLList = aolListarray;
                jsonMatterSearchRequest.SearchObject.Filters.FromDate = startdate;
                jsonMatterSearchRequest.SearchObject.Filters.ToDate = enddate;
                get(jsonMatterSearchRequest, function (response) {
                    vm.lazyloaderdashboard = true;
                    if (response == "") {
                        vm.lazyloaderdashboard = true;
                        vm.divuigrid = false;
                        vm.nodata = true;
                        vm.totalrecords = response.length;
                        vm.getMatterCounts();
                        vm.displaypagination = false;
                    } else {
                        vm.matterGridOptions.data = response;
                        vm.totalrecords = response.length;
                        vm.getMatterCounts();
                        vm.lazyloaderdashboard = true;
                        vm.displaypagination = true;
                        vm.divuigrid = true;
                        vm.nodata = false;
                        if (!$scope.$$phase) {
                            $scope.$apply();
                        }
                    }
                });
            }
            //#endregion

            if (vm.teamName !== '') {
                vm.selectedTab = "All matters";

                vm.getSearchResults();
            }
            //#region Exporting to Excel Test
            vm.export = function () {
                //vm.lazyloaderdashboard = false;
                var exportMatterSearchRequest = {
                    Client: {
                        Url: configs.global.repositoryUrl
                    },
                    SearchObject: {
                        PageNumber: 1,
                        ItemsPerPage: 500,
                        SearchTerm: jsonMatterSearchRequest.SearchObject.SearchTerm,
                        Filters: {
                            ClientsList: jsonMatterSearchRequest.SearchObject.Filters.ClientsList,
                            PGList: jsonMatterSearchRequest.SearchObject.Filters.PGList,
                            AOLList: jsonMatterSearchRequest.SearchObject.Filters.AOLList,
                            FromDate: jsonMatterSearchRequest.SearchObject.Filters.FromDate,
                            ToDate: jsonMatterSearchRequest.SearchObject.Filters.ToDate,
                            FilterByMe: jsonMatterSearchRequest.SearchObject.Filters.FilterByMe
                        },
                        Sort: {
                            ByProperty: jsonMatterSearchRequest.SearchObject.Sort.ByProperty,
                            Direction: jsonMatterSearchRequest.SearchObject.Sort.Direction
                        }
                    }
                };
                if (vm.tabClicked != "Pinned Matters") {

                    get(exportMatterSearchRequest, function (response) {
                        if (response == "") {
                            //vm.lazyloaderdashboard = true;
                        } else {
                            vm.exportDate = response;

                            $timeout(function () {
                                $("#exportable").table2excel({
                                    // exclude CSS class
                                    exclude: ".noExl",
                                    name: "Matters",
                                    filename: "Matters" //do not include extension
                                });

                            }, 1000);

                        }
                    });
                } else {
                    var pinnedMattersRequest = {
                        Url: configs.global.repositoryUrl//ToDo: Read from config.js
                    }
                    getPinnedMatters(exportMatterSearchRequest, function (response) {
                        if (response == "") {

                        } else {
                            vm.exportDate = response;

                            $timeout(function () {
                                $("#exportable").table2excel({
                                    // exclude CSS class
                                    exclude: ".noExl",
                                    name: "Matters",
                                    filename: "Matters" //do not include extension
                                });
                            }, 1000);
                        }
                    });
                }
            }

            //#endregion

            //#region for opening view matters url in new window
            vm.viewMatterDetails = function (url, guid) {
                var viewmatterurl = url + '/SitePages/' + guid + '.aspx';
                window.open(viewmatterurl, '_parent');
            }
            //#endregion

            //#region showing the hidden tabs in responsive
            vm.showDocTabs = function ($event) {
                $event.stopPropagation();
                if (vm.showInnerNav) {
                    vm.showNavTab = true;
                    vm.showInnerNav = false;
                }
            }

            vm.showSelectedTabs = function (name, count) {
                vm.selectedTab = name;
                vm.selectedTabInfo = vm.selectedTab + " (" + count + ")";
                if (name == vm.matterDashboardConfigs.Tab1HeaderText) {
                    vm.myMatters();
                }
                else if (name == vm.matterDashboardConfigs.Tab2HeaderText) {
                    vm.search();
                } else {
                    vm.getMatterPinned();
                }
            }
            //#endregion

            //#region To display modal up in center of the screen...
            //Start 


            vm.reposition = function () {
                var modal = $(this)

                var dialog = modal.find('.modal-dialog');
                modal.css('display', 'block');
                // Dividing by two centers the modal exactly, but dividing by three  
                // or four works better for larger screens. 
                dialog.css("margin-top", Math.max(0, (screen.height - dialog.height()) / 4));
            }
            // Reposition when a modal is shown 
            jQuery('.modal').on('show.bs.modal', vm.reposition);
            // Reposition when the window is resized 
            jQuery(window).on('resize', function () {
                jQuery('.modal:visible').each(vm.reposition);
            });

            $timeout(vm.reposition(), 100);
            //#endregion 

            angular.element($window).bind('resize', function () {
                if ($window.innerWidth > 867) {
                    vm.showNavTab = false;
                    vm.showInnerNav = true;
                }
            });
        }
    ]);

    app.filter('selectclients', function () {
        return function (input, output) {
            var filteredresult = [];
            for (var i = 0; i < input.length; i++) {
                if (input[i].name.indexOf(output)) {
                    input[i].checked = true;
                    filteredresult.push(input[i]);
                } else {
                    input[i].checked = false;
                    filteredresult.pop(input[i]);
                }
            }
            return filteredresult;
        };
    })

})();