(function () {
    'use strict';

    angular.module("matterMain")
        .controller('mattersController', ['$scope', '$state','$interval', '$stateParams', 'api',
      function ($scope, $state,$interval, $stateParams, api) {
          var vm = this;
          var options = new Object;
          vm.gridOptions = {
              enableGridMenu: true,
              columnDefs: [{ field: 'matterName', displayName: 'Matter', enableHiding: false, cellTemplate: '<div class="row"><div class="col-sm-8"> {{row.entity.matterName}} </div><div class="col-sm-4 text-right"><div class="dropdown"><button class="btn btn-default dropdown-toggle" type="button" data-toggle="dropdown">...</button><ul class="dropdown-menu"> <li><a href="#">Upload to this Matter</a></li><li><a href="#">View Matter Details</li><li><a href="#">Go to Matter OneNote</li><li><a href="#">Unpin this Matter</li></ul></div> </div></div>' },
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

          options = {
              Id: "123456",
              Name: "Microsoft",
              Url: "https://msmatter.sharepoint.com/sites/catalog"
          };

          getPinnedMatters(options, function (response) {
              //vm.matters = response.userPinnedMattersList;
              console.log(response);
              vm.gridOptions.data = response.userPinnedMattersList;


          });

          //$scope.ChangeHeaderMenuIcon = function () {

          //    jQuery('[ui-grid-menu-button]').find('i').removeClass('ui-grid-icon-menu').addClass('ui-grid-change-icon');
          //}

          //$interval(function () { $scope.ChangeHeaderMenuIcon(); }, 0, 1);

          $scope.ShowMenuWrapper = function () {
              alert("Hi");
          }
      }]);
})();
