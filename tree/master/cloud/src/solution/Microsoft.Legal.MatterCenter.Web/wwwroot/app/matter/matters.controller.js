(function () {
    'use strict';

    angular.module("matterMain")
        .controller('mattersController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'matterResource',
    function ($scope, $state, $interval, $stateParams, api, $timeout, matterResource) {
        var vm = this;

        vm.gridOptions = {
            enableGridMenu: true,
            columnDefs: [{ field: 'matterName', displayName: 'Matter', enableHiding: false, cellTemplate: '<div class="row"><div class="col-sm-8"> {{row.entity.matterName}} </div><div class="col-sm-4 text-right"><div class="dropdown"><button class="btn btn-default dropdown-toggle" type="button" data-toggle="dropdown">...</button><ul class="dropdown-menu"> <li class="cursor" ng-click="grid.appScope.Openuploadmodal()"><a>Upload to this Matter</a></li><li><a href="#">View Matter Details</li><li><a href="https://msmatter.sharepoint.com/sites/microsoft/" target="_blank">Go to Matter OneNote</li><li class="cursor" ng-click="grid.appScope.PinMatter(row)"><a>Pin this Matter</a></li><li class="cursor" ng-click="grid.appScope.UnpinMatter(row)"><a>Unpin this Matter</a></li></ul></div> </div></div>' },
                        { field: 'matterClient', displayName: 'Client', enableCellEdit: true },
                        //matterID
               { field: 'matterClientId', displayName: 'Client.MatterID', cellTemplate: '<div class="ngCellText">{{row.entity.matterClientId}}.{{row.entity.matterID}}</div>', enableCellEdit: true, },
                { field: 'matterModifiedDate', displayName: 'Modified Date', type: 'date', cellFilter: 'date:\'MMM dd,yyyy\'' },
                { field: 'matterResponsibleAttorney', displayName: 'Responsible attorney', visible: false },
                { field: 'matterSubAreaOfLaw', displayName: 'Sub area of law', visible: false },
                { field: 'matterCreatedDate', displayName: 'Open date', type: 'date', cellFilter: 'date:\'MMM dd,yyyy\'', visible: false },
            ],
            enableColumnMenus: false,
            onRegisterApi: function (gridApi) {
                $scope.gridApi = gridApi;
                gridApi.core.on.columnVisibilityChanged($scope, function (changedColumn) {
                    $scope.columnChanged = { name: changedColumn.colDef.name, visible: changedColumn.colDef.visible };
                });
            }
        };


        function getMatters(options, callback) {
            api({
                resource: 'matterResource',
                method: 'getMatters',
                data: options,
                success: callback
            });
        }



        getMatters(options, function (response) {
            console.log(response);
            vm.title = response.title;

        });

        function getPinnedMatters(options, callback) {
            api({
                resource: 'matterResource',
                method: 'getPinnedMatters',
                data: options,
                success: callback
            });
        }


        function get(options, callback) {
            api({
                    resource: 'matterResource',
                method: 'get',
                    data: options,
                    success: callback
                });
        }


        //Callback function for unpin
        function UnpinMatters(options, callback) {
            api({
                resource: 'matterResource',
                method: 'UnpinMatters',
                data: options,
                success: callback
            });
        }

        vm.searchMatter = function (val) {

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


        options = {
            Id: "123456",
            Name: "Microsoft",
            Url: "https://msmatter.sharepoint.com/sites/catalog"
        };


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
                vm.gridOptions.data = response.searchResults;
            });
        }


        //Code written for displaying types in dropdown
        //Start
        $scope.Matters = [{ Id: 1, Name: "All Matters" }, { Id: 2, Name: "My Matters" }, { Id: 3, Name: "Pinned Matters" }];
        $scope.ddlMatters = $scope.Matters[0];

        //End 


        //Hits when the Dropdown changes
        //Start
        $scope.GetMatters = function (id) {
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
                        Filters: {},
                        Sort:
                        {
                            ByProperty: "LastModifiedTime",
                            Direction: 1
                        }
                    }
                }

                get(AllMattersRequest, function (response) {
                    vm.gridOptions.data = response;
                });

            } else if (id == 2) {
                var MyMattersRequest = {
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
                            FilterByMe: 1
                        },
                        Sort:
                        {
                            ByProperty: "LastModifiedTime",
                            Direction: 1
                        }
                    }
                }

                get(MyMattersRequest, function (response) {
                    vm.gridOptions.data = response;
                });
            } else if (id == 3) {
                var pinnedMattersRequest = {
                    Id: "123456",
                    Name: "Microsoft",
                    Url: "https://msmatter.sharepoint.com/sites/catalog"
                }
                getPinnedMatters(pinnedMattersRequest, function (response) {
                    vm.gridOptions.data = response.matterDataList;
                });

            }

        }
        //End

        //To run GetMatters function on page load
        $scope.GetMatters(1);
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
                    matterName: alldata.matterName,
                    matterDescription: alldata.matterDescription,
                    matterCreatedDate: alldata.matterCreatedDate,
                    matterUrl: alldata.matterUrl,
                    matterPracticeGroup: alldata.matterPracticeGroup,
                    matterAreaOfLaw: alldata.matterAreaOfLaw,
                    matterSubAreaOfLaw: alldata.matterSubAreaOfLaw,
                    matterClientUrl: alldata.matterClientUrl,
                    matterClient: alldata.matterClient,
                    matterClientId: alldata.matterClientId,
                    hideUpload: alldata.hideUpload,
                    matterID: alldata.matterID,
                    matterResponsibleAttorney: alldata.matterResponsibleAttorney,
                    matterModifiedDate: alldata.matterModifiedDate,
                    matterGuid: alldata.matterGuid

                }
            }
            UnpinMatters(unpinRequest, function (response) {
                if (response.isMatterUnPinned) {
                    $scope.GetMatters($scope.ddlMatters);
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
                matterData: {
                    matterName: alldata.matterName,
                    matterDescription: alldata.matterDescription,
                    matterCreatedDate: alldata.matterCreatedDate,
                    matterUrl: alldata.matterUrl,
                    matterPracticeGroup: alldata.matterPracticeGroup,
                    matterAreaOfLaw: alldata.matterAreaOfLaw,
                    matterSubAreaOfLaw: alldata.matterSubAreaOfLaw,
                    matterClientUrl: alldata.matterClientUrl,
                    matterClient: alldata.matterClient,
                    matterClientId: alldata.matterClientId,
                    hideUpload: alldata.hideUpload,
                    matterID: alldata.matterID,
                    matterResponsibleAttorney: alldata.matterResponsibleAttorney,
                    matterModifiedDate: alldata.matterModifiedDate,
                    matterGuid: alldata.matterGuid
                }
            }
            PinMatters(pinRequest, function (response) {
                if (response.isMatterPinned) {
                    $scope.GetMatters($scope.ddlMatters);
                    alert("Success");
                }
            });
        }
        //End

        //To open the UploadMatterModal
        $scope.Openuploadmodal = function () {
            jQuery('#UploadMatterModal').modal("show");
        }



        //To display modal up in center of the screen
        //Start
        function reposition() {
            var modal = $(this),
                dialog = modal.find('.modal-dialog');
            modal.css('display', 'block');

            // Dividing by two centers the modal exactly, but dividing by three 
            // or four works better for larger screens.
            dialog.css("margin-top", Math.max(0, (jQuery(window).height() - dialog.height()) / 2));
        }
        // Reposition when a modal is shown
        jQuery('.modal').on('show.bs.modal', reposition);
        // Reposition when the window is resized
        jQuery(window).on('resize', function () {
            jQuery('.modal:visible').each(reposition);
        });

        $timeout(reposition(), 100);
        //End
    }]);
})();

