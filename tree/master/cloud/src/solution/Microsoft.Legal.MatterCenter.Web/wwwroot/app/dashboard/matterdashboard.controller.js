(function () {
    'use strict;'
    var app = angular.module("matterMain");
    app.controller('MatterDashBoardController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'matterDashBoardResource', '$rootScope', 'uiGridConstants', '$location', '$http', '$q', '$filter', 'commonFunctions',
        function matterDashBoardController($scope, $state, $interval, $stateParams, api, $timeout, matterDashBoardResource, $rootScope, uiGridConstants, $location, $http, $q, $filter, commonFunctions) {
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
            vm.aoldrop = false;
            vm.aoldropvisible = false;
            vm.checkClient = false;
            vm.sortbydrop = false;
            vm.sortbydropvisible = false;
            vm.sortbytext = 'None';
            vm.searchText = '';
            vm.lazyloaderdashboard = true;
            vm.lazyloaderclient = true;
            vm.lazyloaderpg = true;
            vm.lazyloaderaol = true;
            vm.totalrecords = 0;
            //#endregion
            //#region Variable to show matter count            
            vm.allMatterCount = 0;
            vm.myMatterCount = 0;
            vm.pinMatterCount = 0;
            vm.Pinnedobj = [];
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
                paginationPageSize: 10,
                enableGridMenu: false,
                enableRowHeaderSelection: false,
                enableRowSelection: true,
                enableSelectAll: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableFiltering: false
            }


            //#region Matter Grid functionality
            vm.matterGridOptions = {
                enableHorizontalScrollbar: 0,
                enableVerticalScrollbar: 0,
                paginationPageSize: gridOptions.paginationPageSize,
                enableGridMenu: gridOptions.enableGridMenu,
                enableRowHeaderSelection: gridOptions.enableRowHeaderSelection,
                enableRowSelection: gridOptions.enableRowSelection,
                enableSelectAll: gridOptions.enableSelectAll,
                multiSelect: gridOptions.multiSelect,
                enableFiltering: gridOptions.enableFiltering,
                columnDefs: [
                    { field: 'matterName', width: '20%', displayName: 'Matter', cellTemplate: '../app/dashboard/MatterDashboardCellTemplate.html', enableColumnMenu: false },
                    { field: 'matterClient', width: '15%', displayName: 'Client', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.matterClient}}</div>', enableColumnMenu: false },
                    { field: 'matterClientId', width: '15%', displayName: 'Client.Matter ID', headerTooltip: 'Click to sort by client.matterid', enableCellEdit: true, cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.matterClientId}}.{{row.entity.matterClient}}</div>', enableColumnMenu: false },
                    { field: 'matterModifiedDate', width: '15%', displayName: 'Modified Date', cellTemplate: '<div class="ui-grid-cell-contents"  datefilter date="{{row.entity.matterModifiedDate}}"></div>', enableColumnMenu: false },
                    { field: 'matterResponsibleAttorney', width: '15%', headerTooltip: 'Click to sort by attorney', displayName: 'Responsible attorney', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.matterResponsibleAttorney}}</div>', enableColumnMenu: false },
                    { field: 'pin', displayName: '', width: '5%', cellTemplate: '<div class="ui-grid-cell-contents pad0" ><img ng-src="../Images/{{row.entity.pinType}}-666.png"  ng-click="grid.appScope.vm.pinorunpin($event, row.entity)"/></div>', enableColumnMenu: false },
                    { field: 'upload', displayName: '', width: '7%', cellTemplate: '<div class="ui-grid-cell-contents pad0"><img src="../Images/upload-666.png" ng-click="grid.appScope.vm.Openuploadmodal(row.entity.matterName,row.entity.matterUrl,row.entity.matterGuid)"/></div>', enableColumnMenu: false }
                ],
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
                    TermGroup: "MatterCenterTerms",
                    TermSetName: "Clients",
                    CustomPropertyName: "ClientURL"
                }
            };

            var optionsForPracticeGroup = {
                Client: {
                    Url: configs.global.repositoryUrl
                },
                TermStoreDetails: {
                    TermGroup: "MatterCenterTerms",
                    TermSetName: "Practice Groups",
                    CustomPropertyName: "ContentTypeName",
                    DocumentTemplatesName: "DocumentTemplates"
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
                        Direction: 1
                    }
                }
            };

            //#reion This function will get counts for all matters, my matters and pinned matters
            vm.getMatterCounts = function () {
                vm.lazyloaderdashboard = false;
                var searchToText = '';
                var finalSearchText = '';
                if (vm.searchText != "") {
                    searchToText = vm.searchText.replace("(", ",")
                    searchToText = searchToText.replace(")", "")
                    var firstText = searchToText.split(',')[0]
                    var secondText = searchToText.split(',')[1]
                    var finalSearchText = '(MCMatterName:"' + firstText.trim() + '" AND MCMatterID:"' + secondText.trim() + '")'
                }
                jsonMatterSearchRequest.SearchObject.SearchTerm = finalSearchText;
                getMatterCounts(jsonMatterSearchRequest, function (response) {
                    vm.allMatterCount = response.allMatterCounts;
                    vm.myMatterCount = response.myMatterCounts;
                    vm.pinMatterCount = response.pinnedMatterCounts;
                    vm.totalrecords = response.allMatterCounts;
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                    vm.pagination();
                    vm.lazyloaderdashboard = true;
                });
            }
            //#endregion

            //#region This api will get all matters which are pinned and this will be invoked when the user clicks on "Pinned Matters Tab"
            vm.getMatterPinned = function () {
                vm.lazyloaderdashboard = false;
                var pinnedMattersRequest = {
                    Url: configs.global.repositoryUrl//ToDo: Read from config.js
                }
                getPinnedMatters(pinnedMattersRequest, function (response) {
                    var pinnedResponse = response;
                    if (response && response.length > 0) {
                        angular.forEach(response, function (res) {
                            res.pinType = "unpin"
                        })
                    }
                    vm.Pinnedobj = response
                    vm.matterGridOptions.data = response;
                    vm.totalrecords = vm.pinMatterCount;
                    vm.pagination();
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }

                    vm.lazyloaderdashboard = true;
                });
            }
            //#endregion


            //#regionThis search function will be used when the user enters some text in the search text box and presses search button
            vm.searchMatters = function (val) {
                vm.lazyloaderdashboard = false;
                var searchRequest = {
                    Client: {
                        Url: configs.global.repositoryUrl
                    },
                    SearchObject: {
                        PageNumber: 1,
                        ItemsPerPage: 10,
                        SearchTerm: val,
                        Filters: {},
                        Sort: {
                            ByProperty: "LastModifiedTime",
                            Direction: 1
                        }
                    }
                };
                return matterDashBoardResource.get(searchRequest).$promise;
                vm.lazyloaderdashboard = true;
            }
            //#endregion

            vm.myMatters = function () {
                vm.lazyloaderdashboard = false;
                var searchToText = '';
                var finalSearchText = '';
                if (vm.searchText != "") {
                    searchToText = vm.searchText.replace("(", ",")
                    searchToText = searchToText.replace(")", "")
                    var firstText = searchToText.split(',')[0]
                    var secondText = searchToText.split(',')[1]
                    var finalSearchText = '(MCMatterName:"' + firstText.trim() + '" AND MCMatterID:"' + secondText.trim() + '")'
                }
                jsonMatterSearchRequest.SearchObject.SearchTerm = finalSearchText;
                jsonMatterSearchRequest.SearchObject.Filters.FilterByMe = 1;
                jsonMatterSearchRequest.SearchObject.PageNumber = 1;
                jsonMatterSearchRequest.SearchObject.ItemsPerPage = 10;
                get(jsonMatterSearchRequest, function (response) {
                    vm.matterGridOptions.data = response;
                    vm.lazyloaderdashboard = true;
                    vm.totalrecords = vm.myMatterCount;
                    vm.pagination();
                });
            }
            
            //This search function will be used for binding search results to the grid
            vm.search = function (isMy) {
                vm.lazyloaderdashboard = false;
                var searchToText = '';
                var finalSearchText = '';
                if (vm.searchText != "") {
                    searchToText = vm.searchText.replace("(", ",")
                    searchToText = searchToText.replace(")", "")
                    var firstText = searchToText.split(',')[0]
                    var secondText = searchToText.split(',')[1]
                    var finalSearchText = '(MCMatterName:"' + firstText.trim() + '" AND MCMatterID:"' + secondText.trim() + '")'
                }

                var pinnedMattersRequest = {
                    Url: configs.global.repositoryUrl
                }
                var tempMatters = [];
                jsonMatterSearchRequest.SearchObject.SearchTerm = "";
                jsonMatterSearchRequest.SearchObject.Filters.FilterByMe = 0;
                jsonMatterSearchRequest.SearchObject.PageNumber = 1;
                jsonMatterSearchRequest.SearchObject.ItemsPerPage = 10;
                get(jsonMatterSearchRequest, function (response) {
                    //We need to call pinned api to determine whether a matter is pinned or not                    
                    getPinnedMatters(pinnedMattersRequest, function (pinnedResponse) {
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
                            vm.pagination();
                            vm.lazyloaderdashboard = true;
                        }
                        else {
                            vm.lazyloaderdashboard = true;
                            vm.matterGridOptions.data = response;
                            vm.totalrecords = vm.allMatterCount;
                            vm.pagination();
                            vm.pinMatterCount = 0;
                        }
                    });

                });
            }


            //This function will pin or unpin the matter based on the image button clicked
            vm.pinorunpin = function (e, currentRowData) {
                vm.lazyloaderdashboard = false;
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
                        vm.lazyloaderdashboard = true;
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
                            e.currentTarget.src = "../images/pin-666.png";
                            vm.pinMatterCount = parseInt(vm.pinMatterCount, 10) - 1;
                            vm.matterGridOptions.data.splice(vm.matterGridOptions.data.indexOf(currentRowData), 1)
                        }
                        vm.lazyloaderdashboard = true;
                    });
                }

            }



            //#endregion

            //#region This event is going to file when the user clicks onm "Select All" and "UnSelect All" links
            vm.checkAll = function (checkAll, type, $event) {
                $event.stopPropagation();
                if (type === 'client') {
                    angular.forEach(vm.clients, function (client) {
                        client.Selected = checkAll;
                    });
                }
                if (type === 'pg') {
                    angular.forEach(vm.practiceGroups, function (pg) {
                        pg.Selected = checkAll;
                    });
                }
                if (type === 'aol') {
                    angular.forEach(vm.aolTerms, function (aol) {
                        aol.Selected = checkAll;
                    });
                }
            }

            //#region This event is going to fire when the user clicks on "OK" button in the filter panel
            vm.filterSearchOK = function (type) {
                if (type === 'client') {
                    vm.selectedClients = '';
                    angular.forEach(vm.clients, function (client) {
                        if (client.Selected) {
                            vm.selectedClients = vm.selectedClients + client.name + ","
                        }
                    });
                    vm.selectedClients = vm.selectedClients.slice(0, vm.selectedClients.length - 1);
                    vm.clientdrop = false;
                    vm.clientdropvisible = false;
                }
                if (type === 'pg') {
                    vm.selectedPGs = '';
                    vm.selectedAOLs = '';
                    angular.forEach(vm.practiceGroups, function (pg) {
                        if (pg.Selected) {
                            vm.selectedPGs = vm.selectedPGs + pg.termName + ","
                            //For each of the selected pg's select corresponding aol check boxes automatically and update the aol
                            //textbox accordingly
                            angular.forEach(pg.areaTerms, function (areaterm) {
                                areaterm.Selected = true;
                                vm.selectedAOLs = vm.selectedAOLs + areaterm.termName + ","
                            });
                        }
                    });
                    vm.selectedPGs = vm.selectedPGs.slice(0, vm.selectedPGs.length - 1);
                    vm.pgdrop = false;
                    vm.pgdropvisible = false;
                }

                if (type === 'aol') {
                    vm.selectedAOLs = '';
                    angular.forEach(vm.aolTerms, function (aol) {
                        if (aol.Selected) {
                            vm.selectedAOLs = vm.selectedAOLs + aol.termName + ","
                        }
                    });
                    vm.selectedAOLs = vm.selectedAOLs.slice(0, vm.selectedAOLs.length - 1);
                    vm.aoldrop = false;
                    vm.aoldropvisible = false;
                }
            }
            //#endregion

            //#region This event is going to fire when the user clicks on "Cancel" button in the filter panel
            vm.filterSearchCancel = function (type) {
                vm.clientdrop = false;
                vm.clientdropvisible = false;
                vm.pgdrop = false;
                vm.pgdropvisible = false;
                vm.aoldrop = false;
                vm.aoldropvisible = false;
            }
            //#endregion


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
                            vm.lazyloaderclient = true;
                        });
                    }
                    else {
                        vm.clientdrop = true;
                        vm.clientdropvisible = true;
                    }
                    vm.pgdrop = false;
                    vm.pgdropvisible = false;
                    vm.aoldrop = false;
                    vm.aoldropvisible = false;
                    
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
            //#endregion

            //#region showing and hiding practice group dropdown
            vm.showPracticegroupDrop = function ($event) {
                $event.stopPropagation();
                if (!vm.pgdropvisible) {
                    if ((vm.practiceGroups === undefined) && (vm.aolTerms === undefined)) {
                        vm.lazyloaderpg = false;
                        getTaxonomyDetailsForPractice(optionsForPracticeGroup, function (response) {
                            vm.practiceGroups = response.pgTerms;
                            vm.aolTerms = [];
                            angular.forEach(response.pgTerms, function (pgTerm) {
                                angular.forEach(pgTerm.areaTerms, function (areaterm) {
                                    vm.aolTerms.push(areaterm);
                                });
                            })
                            vm.pgdrop = true;
                            vm.pgdropvisible = true;
                            vm.lazyloaderpg = true;
                        });
                    }
                    else
                    {
                        vm.pgdrop = true;
                        vm.pgdropvisible = true;
                    }
                    vm.clientdrop = false;
                    vm.clientdropvisible = false;
                    vm.aoldrop = false;
                    vm.aoldropvisible = false;
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
                            vm.practiceGroups = response.pgTerms;
                            vm.aolTerms = [];
                            angular.forEach(response.pgTerms, function (pgTerm) {
                                angular.forEach(pgTerm.areaTerms, function (areaterm) {
                                    vm.aolTerms.push(areaterm);
                                });
                            })
                            vm.aoldrop = true;
                            vm.aoldropvisible = true;
                            vm.lazyloaderaol = true;
                        });
                    }
                    else
                    {
                        vm.aoldrop = true;
                        vm.aoldropvisible = true;
                    }
                    vm.clientdrop = false;
                    vm.clientdropvisible = false;
                    vm.pgdrop = false;
                    vm.pgdropvisible = false;
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
            $interval(function () { vm.getMatterCounts(); }, 800, 3);
            $interval(function () { vm.search() }, 500, 3);


            //#region For Sorting by Alphebatical or Created date
            var SortRequest = {
                Client: {
                    Url: configs.global.repositoryUrl
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
                                ByProperty: 'LastModifiedTime',
                                Direction: 1
                            }
                }
            }

            vm.FilterByType = function () {
                get(SortRequest, function (response) {
                    vm.lazyloader = true;
                    if (response.errorCode == "404") {
                        vm.divuigrid = false;
                        vm.nodata = true;
                        $scope.errorMessage = response.message;
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                        vm.matterGridOptions.data = response;
                        if (!$scope.$$phase) {
                            $scope.$apply();
                        }
                    }
                });
            }


            vm.sortyby = function (sortexp, data) {
                vm.sortbytext = data;
                if (sortexp == 'AlphabeticalUp') {
                    vm.lazyloader = false;
                    SortRequest.SearchObject.Sort.ByProperty = "MCMatterName";
                    SortRequest.SearchObject.Sort.Direction = 0;
                    vm.FilterByType();
                } else if (sortexp == 'AlphabeticalDown') {
                    vm.lazyloader = false;
                    SortRequest.SearchObject.Sort.ByProperty = "MCMatterName";
                    SortRequest.SearchObject.Sort.Direction = 1;
                    vm.FilterByType();
                } else if (sortexp == 'CreateddateUp') {
                    vm.lazyloader = false;
                    SortRequest.SearchObject.Sort.ByProperty = "MCOpenDate";
                    SortRequest.SearchObject.Sort.Direction = 0;
                    vm.FilterByType();
                }
                else if (sortexp == 'CreateddateDown') {
                    vm.lazyloader = false;
                    SortRequest.SearchObject.Sort.ByProperty = "MCOpenDate";
                    SortRequest.SearchObject.Sort.Direction = 1;
                    vm.FilterByType();
                } else {
                    vm.lazyloader = false;
                    SortRequest.SearchObject.Sort.ByProperty = "LastModifiedTime";
                    SortRequest.SearchObject.Sort.Direction = 1;
                    vm.FilterByType();
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
                if (vm.last < vm.totalrecords) {
                    vm.first = vm.first + gridOptions.paginationPageSize;
                    vm.last = vm.last + gridOptions.paginationPageSize;
                    vm.total = vm.totalrecords - gridOptions.paginationPageSize;
                    if (vm.total < gridOptions.paginationPageSize) {
                        vm.fromtopage = vm.first + " - " + vm.totalrecords;
                    } else {
                        vm.fromtopage = vm.first + " - " + vm.last;
                    }
                    vm.pagenumber = vm.pagenumber + 1;
                    jsonMatterSearchRequest.SearchObject.PageNumber = vm.pagenumber;
                    jsonMatterSearchRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                    get(jsonMatterSearchRequest, function (response) {
                        vm.lazyloader = true;
                        if (response.errorCode == "404") {
                            vm.divuigrid = false;
                            vm.nodata = true;
                            vm.errorMessage = response.message;
                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                            vm.matterGridOptions.data = response;
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
                if (vm.last > gridOptions.paginationPageSize) {
                    vm.first = vm.first - gridOptions.paginationPageSize;
                    vm.last = vm.last - gridOptions.paginationPageSize;
                    vm.pagenumber = vm.pagenumber - 1;
                    vm.fromtopage = vm.first + " - " + vm.last;
                    jsonMatterSearchRequest.SearchObject.PageNumber = vm.pagenumber;
                    jsonMatterSearchRequest.SearchObject.ItemsPerPage = gridOptions.paginationPageSize;
                    get(jsonMatterSearchRequest, function (response) {
                        vm.lazyloader = true;
                        if (response.errorCode == "404") {
                            vm.divuigrid = false;
                            vm.nodata = true;
                            vm.errorMessage = response.message;
                        } else {
                            vm.divuigrid = true;
                            vm.nodata = false;
                            vm.matterGridOptions.data = response;
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


            vm.getSearchResults = function () {
                var clientArray = [];
                var aolListarray = [];
                var pglistArray = [];
                if (vm.selectedClients != "") {
                    clientArray = vm.selectedClients.split(',');
                }
                if (vm.selectedPGs != "") {
                    pglistArray = vm.selectedPGs.split(',');
                }
                if (vm.selectedAOLs != "") {
                    aolListarray = vm.selectedAOLs.split(',');
                }
                jsonMatterSearchRequest.SearchObject.Filters.ClientsList = clientArray;
                jsonMatterSearchRequest.SearchObject.Filters.PGList = pglistArray;
                jsonMatterSearchRequest.SearchObject.Filters.AOLList = aolListarray;
                jsonMatterSearchRequest.SearchObject.Filters.FromDate = $scope.startdate.format("yyyy-MM-dd");
                jsonMatterSearchRequest.SearchObject.Filters.ToDate = $scope.enddate.format("yyyy-MM-dd");
                get(jsonMatterSearchRequest, function (response) {
                    vm.lazyloader = true;
                    if (response == "") {
                        vm.divuigrid = false;
                        vm.nodata = true;
                        vm.errorMessage = response.message;
                    } else {
                        vm.divuigrid = true;
                        vm.nodata = false;
                        vm.matterGridOptions.data = response;
                        if (!$scope.$$phase) {
                            $scope.$apply();
                        }
                    }
                });
            }
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