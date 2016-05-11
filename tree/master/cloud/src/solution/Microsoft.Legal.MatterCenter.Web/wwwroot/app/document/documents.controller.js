(function () {
    'use strict';

    var app = angular.module("matterMain");

    app.controller('documentsController', ['$scope', '$state', '$interval', '$stateParams', 'api', '$timeout', 'documentResource', '$rootScope',
function ($scope, $state, $interval, $stateParams, api, $timeout, documentResource, $rootScope) {
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
    var documentHeaderTemplate = "<div >\
    <div class='dropdown keep-open' style='float:right;'>\
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
            <div class='ms-font-m ms-font-weight-semilight clearText' title='Clear filters from Matter'><span>Clear filters from </span><span class='clearFilterTitle'>Matter</span></div>\
             <div id='filterResultsContainer'><div class='filterValueLabels ms-font-m ms-font-weight-semilight' ng-repeat='matter in grid.appScope.matters' ng-click='grid.appScope.filterdocumentName(matter.documentName)'>{{document.documentName}}</div> </div>\
        </div>\
        </div>\
    </div>\
    <div role='button' class='ui-grid-cell-contents ui-grid-header-cell-primary-focus' col-index='renderIndex'>\
        <span class='ui-grid-header-cell-label ng-binding'>{{ col.colDef.displayName }}</span>\
        <span ui-grid-visible='col.sort.direction'  class='ui-grid-invisible ui-grid-icon-blank'>&nbsp;</span>\
    </div>\
    <div class='ui-grid-column-menu-button ng-scope' ng-if='grid.options.enableColumnMenus && !col.isRowHeader && col.colDef.enableColumnMenu !== false' ng-click='grid.appScope.toggleMenu($event)' ng-class='{'ui-grid-column-menu-button-last-col': isLastCol}'>\
        <i  title='' aria-hidden='true' class='ui-grid-icon-up-dir'>&nbsp;</i>\
    </div>\
</div>"
    //End


    //Assigning html for Clientheadertemplate
    //Start
    var ClientHeaderTemplate = "<div >\
    <div class='dropdown keep-open' style='float:right;'>\
        <a href='javascript:;' dropdown class='prisma-header-dropdown-anchor id='acombo' dropdown-toggle' type='button' data-toggle='dropdown'>\
            <img src='../images/icon-combobox.png'/>\
        </a>\
        <div class='dropdown-menu flyoutWrapper' ng-click='$event.stopPropagation();' role='menu'  aria-labelledby='acombo'>\
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
        <span ui-grid-visible='col.sort.direction'  class='ui-grid-invisible ui-grid-icon-blank'>&nbsp;</span>\
    </div>\
    <div class='ui-grid-column-menu-button ng-scope' ng-if='grid.options.enableColumnMenus && !col.isRowHeader && col.colDef.enableColumnMenu !== false' ng-click='grid.appScope.toggleMenu($event)' ng-class='{'ui-grid-column-menu-button-last-col': isLastCol}'>\
        <i  title='' aria-hidden='true' class='ui-grid-icon-up-dir'>&nbsp;</i>\
    </div>\
</div>"
    //End



    //Assigning html for celltemplate
    //Start
    var documentCellTemplate = "<div class='row'>\
    <div class='col-sm-8'>\
        <a popoverdoc type='button' class='btn btn-link' details={{row.entity}} data-toggle='popover' data-container='body' data-placement='right' type='button' data-html='true' href='' > {{row.entity.documentName}} </a>\
        <div class='popover-content'></div>\
    </div>\
    <div class='col-sm-4 text-right'>\
        <div class='dropdown'>\
            <button class='btn btn-default dropdown-toggle' type='button' data-toggle='dropdown'>...</button><ul class='dropdown-menu'>\
                <li class='cursor' ng-click='grid.appScope.Openuploadmodal()'><a>Upload to this Matter</a></li>\
                <li><a href='#'>View Matter Details</a></li>\
                <li>\
                    <a href='https://msmatter.sharepoint.com/sites/microsoft/' target='_blank'>\
                        Go to Matter OneNote\
                    </a>\
                </li>\
                <li class='cursor' ng-click='row.entity.MatterInfo===undefined?grid.appScope.PinMatter(row):grid.appScope.UnpinMatter(row)'><a>{{row.entity.MatterInfo===undefined?'Pin this Matter':'Unpin this matter'}}</a></li>\
            </ul>\
        </div>\
    </div>\
</div>";
    //End

    vm.gridOptions = {
        enableGridMenu: true,
        columnDefs: [{
            field: 'documentName', displayName: 'Document', enableHiding: false, cellTemplate: documentCellTemplate,
            headerCellTemplate: documentHeaderTemplate
        },
            { field: 'documentClient', displayName: 'Client', enableCellEdit: true, headerCellTemplate: ClientHeaderTemplate },
             //matterID 
    { field: 'documentClientId', displayName: 'Client.Matter ID', cellTemplate: '<div class="ngCellText">{{row.entity.documentClientId}}.{{row.entity.DocumentMatterId}}</div>', enableCellEdit: true, },
     { field: 'documentModifiedDate', displayName: 'Modified Date', type: 'date', cellFilter: 'date:\'MMM dd,yyyy\'' },
     { field: 'documentOwner', displayName: 'Author', visible: false },
     { field: 'documentVersion', displayName: 'Document Version', visible: false },
     { field: 'documentCheckoutUser', displayName: 'Checked out to', visible: false },
     { field: 'documentCreatedDate', displayName: 'Created date', type: 'date', cellFilter: 'date:\'MMM dd,yyyy\'', visible: false },
        ],
        enableColumnMenus: false,
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
            gridApi.core.on.columnVisibilityChanged($scope, function (changedColumn) {
                $scope.columnChanged = { name: changedColumn.colDef.name, visible: changedColumn.colDef.visible };
            });
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


    //To open the UploadMatterModal 
    $scope.Openuploadmodal = function () {
        jQuery('#UploadDocumentModal').modal("show");
    }






    //To display modal up in center of the screen...
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
    $rootScope.breadcrumb = true;
    $rootScope.foldercontent = false;

    $scope.hideBreadCrumb = function () {
        $rootScope.breadcrumb = true;
        $rootScope.foldercontent = false;

    }
}]);
    //app.directive('toggle', function () {
    //    return {
    //        restrict: 'AE',
    //        link: function (scope, element, attrs) {
    //            if (attrs.toggle == "popoverdoc") {
    //                $(element).popoverdoc();
    //            }
    //        }
    //    };
    //})

    app.directive('popoverdoc', function () {
        return {
            restrict: 'AE',
            link: function (scope, element, attrs) {
                var obj = eval('(' + attrs.details + ')');
                var content = '<div>\
                                          ' + obj.documentName + ' \
                                          <div> <b>Matter :</b> '+ obj.documentMatter + '</div>\
                                          <div> <b>Client :</b> '+ obj.documentClient + '</div>\
                                          <div><b>DocumentID :</b> '+ obj.DocumentID + '</div>\
                                          <div><b>Author :</b> '+ obj.documentOwner + '</div> \
                                          <div><b>Modified date</b> : '+ obj.DocumentModifiedDate + '</div>\
                                          <div><button ><a href="https://msmatter.sharepoint.com/sites/microsoft/SitePages/'+ obj.matterGuid + '.aspx" target="_blank">Open document</a></button></div>\
                            <div><a onclick="Openuploadmodal(this)" documentName="' + obj.documentName + '" matterurl="' + obj.matterUrl + '" type="button">View document details</a></div>\
                       </div>';
                $(element).popover({
                    html: true,
                    trigger: 'click',
                    delay: 500,
                    content: content,
                });
            }
        }
    });


    app.directive('showbreadcrumb', function ($rootScope) {
        return {
            restrict: 'AE',
            link: function (scope, element, attrs) {
                $(element).find('ul li ul li').on("dblclick", function (e) {
                    $rootScope.breadcrumb = false;
                    $rootScope.foldercontent = true;
                    $rootScope.$apply();
                    var text = $(this).find('div').attr('title');
                    $('#breadcrumb #currentFolder').html(text);
                });

            }
        }
    });



})();

function Openuploadmodal(documentName) {
    var Name = documentName.getAttribute("documentName");
    var Url = documentName.getAttribute("matterurl");
    jQuery('#UploadMatterModal').modal("show");
}