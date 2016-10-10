(function () {

    'use strict';
    function onload($timeout) {
        return {
            restrict: 'AE',
            scope: { sortdetails: '@' },
            link: function (scope, element, attrs) {
                scope.$watch("sortdetails", function () {
                    $timeout(function () { $('[id^="asc"]').hide(); }, 1000);
                    $timeout(function () { $('[id^="desc"]').hide(); }, 1000);
                }, true);
            }
        }
    }

    'use strict';
    function showbreadcrumb($rootScope) {
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
    }

    'use strict';
    function datefilter() {
        return {
            restrict: 'A',
            scope: { date: '@' },
            link: function (scope, element, attrs) {
                scope.$watch("date", function () {
                    var dDate = attrs.date;
                    var oDates = new Date(dDate), months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], date = "", oLocalDate = dDate;
                    if (isNaN(oDates)) {
                        var arrSplitedDate = dDate.replace(/[-]/g, "/");
                        arrSplitedDate = arrSplitedDate.split("/");
                        dDate = arrSplitedDate[1] + "-" + arrSplitedDate[0] + "-" + arrSplitedDate[2];
                        oDates = new Date(dDate);
                    }
                    if (0 > oLocalDate.indexOf("Z")) {
                        date += months[parseInt(oDates.getMonth(), 10)] + " ";
                        date += oDates.getDate() + ", ";
                        date += oDates.getFullYear();
                    } else {
                        date += months[parseInt(oDates.getUTCMonth(), 10)] + " ";
                        date += oDates.getUTCDate() + ", ";
                        date += oDates.getUTCFullYear();
                    }
                    $(element).html(date);
                    return date;
                });
            }
        }
    }


    'use strict';
    function toggletab() {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                $(element).find('a').click(function (e) {
                    e.preventDefault()
                    $(this).tab('show')
                })
            }

        }
    }

    'use strict';
    function infopopover() {
        return {
            restrict: 'AE',
            link: function (scope, element, attrs) {
                var content = "<div class='cartelementpopup' style='width:250px'>\
                                   <div class='checkOutMetadata marginTop10'>When sending to your OneDrive the DMS version will be checked out to you until you check it back in.</div>\
                               </div>";
                $(element).popover({
                    html: true,
                    trigger: 'click',
                    delay: 500,
                    content: content,
                });
            }
        }
    }

    'use strict';
    function matterflyout($compile, $templateCache) {
        return {
            restrict: 'A',
            scope: {
                control: '&'
            },
            link: function (scope, element, attrs) {
                $(element).click(function (e) {
                    var obj = "";
                    obj = eval('(' + attrs.details + ')');
                    if (obj.matterSubAreaOfLaw == "") {
                        obj.matterSubAreaOfLaw = "NA";
                    }
                    var actualcontent = "";
                    if (typeof Word === 'undefined') {
                        actualcontent = '<div class="" style="position:relative;display:table-row" ng-click="stopEvent($event)">\
                                   <div class="FlyoutBoxContent flyoutwidth">\
                                      <div class="flyoutLeftarrow hidden-xs" style="top: 11px;left: -9px;"></div>\
                                      <div class="flyoutToparrow visible-xs" style="top: -8px;"></div>\
                                      <div class="FlyoutContent FlyoutHeading">\
                                          <div class="ms-Callout-content FlyoutHeadingText">  ' + obj.matterName + ' </div>\
                                       </div>\
                                        <div class="ms-Callout-content commonFlyoutContaint"  ng-show=' + configs.search.searchColumnsUIPickerForMatter.matterClient.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutMenuColumn1DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterClient + '</div>\
                                       </div>\
                                        <div class="ms-Callout-content commonFlyoutContaint" ng-show=' + configs.search.searchColumnsUIPickerForMatter.matterClientId.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutMenuColumn3DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterClientId + '.' + obj.matterID + '</div>\
                                       </div>\
                                        <div class="ms-Callout-content commonFlyoutContaint" ng-show=' + configs.search.searchColumnsUIPickerForMatter.matterPracticeGroup.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutMenuColumn2DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterPracticeGroup + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint"  ng-show=' + configs.search.searchColumnsUIPickerForMatter.matterAreaOfLaw.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutMenuColumn3DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterAreaOfLaw + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint"  ng-show=' + configs.search.searchColumnsUIPickerForMatter.matterSubAreaOfLaw.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutMenuColumn4DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterSubAreaOfLaw + '</div> \
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint" ng-show=' + configs.search.searchColumnsUIPickerForMatter.matterResponsibleAttorney.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutMenuColumn6DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterResponsibleAttorney + '</div>\
                                       </div>\
                                       <a id="viewMatters" class="ms-Button-label ms-Button ms-Button--primary ms-Callout-content" href="" ng-click="redirectViewMatters(\'' + obj.matterClientUrl + '\',\'' + obj.matterGuid + '\')">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutButton1Text + '</a><br/>\
                                      <a class="ms-Button-label ms-Button ms-Button--primary ms-Callout-content"  id="uploadToMatter" ng-click="openUpload(\'' + obj.matterName + '\',\'' + obj.matterClientUrl + '\',\'' + obj.matterGuid + '\')" type="button">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutButton2Text + '</a>\
                                   </div>\
                                </div>';
                    }
                    else {
                        var matterUrl = obj.matterClientUrl + "/" + obj.matterGuid;
                        actualcontent = '<div class="" style="position:relative;display:table-row" ng-click="stopEvent($event)">\
                                   <div class="FlyoutBoxContent flyoutwidth">\
                                      <div class="flyoutLeftarrow hidden-xs" style="top: 11px;left: -9px;"></div>\
                                      <div class="flyoutToparrow visible-xs" style="top: -8px;"></div>\
                                      <div class="FlyoutContent FlyoutHeading">\
                                          <div class="ms-Callout-content FlyoutHeadingText">  ' + obj.matterName + ' </div>\
                                       </div>\
                                        <div class="ms-Callout-content commonFlyoutContaint"  ng-show=' + configs.search.searchColumnsUIPickerForMatter.matterClient.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutMenuColumn1DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterClient + '</div>\
                                       </div>\
                                        <div class="ms-Callout-content commonFlyoutContaint" ng-show=' + configs.search.searchColumnsUIPickerForMatter.matterClientId.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutMenuColumn3DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterClientId + '.' + obj.matterID + '</div>\
                                       </div>\
                                        <div class="ms-Callout-content commonFlyoutContaint" ng-show=' + configs.search.searchColumnsUIPickerForMatter.matterPracticeGroup.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutMenuColumn2DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterPracticeGroup + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint"  ng-show=' + configs.search.searchColumnsUIPickerForMatter.matterAreaOfLaw.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutMenuColumn3DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterAreaOfLaw + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint"  ng-show=' + configs.search.searchColumnsUIPickerForMatter.matterSubAreaOfLaw.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutMenuColumn4DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterSubAreaOfLaw + '</div> \
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint" ng-show=' + configs.search.searchColumnsUIPickerForMatter.matterResponsibleAttorney.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutMenuColumn6DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterResponsibleAttorney + '</div>\
                                       </div>\
                                        <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading" style="width:250px">Save the document to this locations</div><br/>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">URL:</div><br/>\
                                          <input type="text" value="' + matterUrl + '"><br/>\
                                       </div>\<a id="viewMatters" class="ms-Button-label ms-Button ms-Button--primary ms-Callout-content" href="" ng-click="redirectViewMatters(\'' + obj.matterClientUrl + '\',\'' + obj.matterGuid + '\')">' + scope.$parent.$parent.$parent.grid.appScope.vm.matterConfigContent.FlyoutButton1Text + '</a><br/>\
                                      </div>\
                                </div>';
                    }


                    $templateCache.put("test.html", actualcontent);
                    var template = $templateCache.get("test.html");
                    var a = $compile("<div>" + actualcontent + "</div>")(scope);
                    $('.dropdown').removeClass('open');
                    $('.popcontent').css('display', 'none');
                    e.stopPropagation();
                    var obj = e.target.getBoundingClientRect();
                    $(this).parent().find('.popcontent').html(a[0]);
                    if (obj.top < 350) {
                        $(this).parent().find('.popcontent').css({ 'display': 'block', 'left': '268px', 'top': '0' });
                        $(this).parent().find('.popcontent').find('.flyoutLeftarrow').css('top', '11px');
                    } else {
                        if (scope.$parent.$parent.$parent.grid.appScope.vm.center == "mattercenter") {
                            $(this).parent().find('.popcontent').css({ 'display': 'block', 'left': '268px', 'top': '-238px' });
                            $(this).parent().find('.popcontent').find('.flyoutLeftarrow').css('top', '244px');
                        } else {
                            $(this).parent().find('.popcontent').css({ 'display': 'block', 'left': '268px', 'top': '-218px' });
                            $(this).parent().find('.popcontent').find('.flyoutLeftarrow').css('top', '220px');
                        }
                    }
                    scope.$apply();
                });
            },
            controller: function ($scope) {
                $scope.openUpload = function (matterName, matterUrl, matterGUID) {
                    $scope.$parent.$parent.$parent.grid.appScope.vm.Openuploadmodal(matterName, matterUrl, matterGUID);
                    $('.popcontent').css('display', 'none');
                };
                $scope.redirectViewMatters = function (url, guid) {
                    $scope.$parent.$parent.$parent.grid.appScope.vm.viewMatterDetails(url, guid);
                }
                $scope.stopEvent = function ($event) {
                    $event.stopPropagation();
                };
            }
        }
    };

    'use strict';
    function documentflyout($http, $compile, $templateCache) {
        return {
            restrict: 'A',
            scope: { loader: '=' },
            link: function (scope, element, attrs) {
                $(element).click(function (e) {
                    var obj = "";
                    obj = eval('(' + attrs.details + ')');
                    if (obj.documentMatter == "") {
                        obj.documentMatter = "NA";
                    }
                    if (obj.documentClient == "") {
                        obj.documentClient = "NA";
                    }
                    if (obj.documentID == "") {
                        obj.documentID = "NA";
                    }
                    if (obj.documentOwner == "") {
                        obj.documentOwner = "NA";
                    }
                    if (obj.documentModifiedDate == "") {
                        obj.documentModifiedDate = "NA";
                    }

                    var actualcontent = "";
                    actualcontent = '<div class="" ng-click="stopEvent($event)">\
                                     <img id="FlyoutPopupLoading" ng-if="!loader" title="Loading" src="../Images/WindowsLoadingFast.GIF" alt="Loading">\
                                   <div class="FlyoutBoxContent flyoutwidth" style="height:270px">\
                                      <div class="flyoutLeftarrow hidden-xs" style="top: 11px;left: -9px;"></div>\
                                           <div>\
                                       <div class="flyoutToparrow visible-xs" style="top: -8px;"></div>\
                                      <div class="FlyoutContent">\
                                          <div class="ms-Callout-content FlyoutHeadingText">  ' + obj.documentName + ' </div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint" ng-show=' + configs.search.searchColumnsUIPickerForDocument.documentMatterName.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.documentConfigContent.FlyoutMenuColumn1DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.documentMatterName + '</div>\
                                       </div>\
                                        <div class="ms-Callout-content commonFlyoutContaint" ng-show=' + configs.search.searchColumnsUIPickerForDocument.documentPracticeGroup.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.documentConfigContent.FlyoutMenuColumn2DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.documentPracticeGroup + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint" ng-show=' + configs.search.searchColumnsUIPickerForDocument.documentClient.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.documentConfigContent.GridColumn2Header + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.documentClient + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint" ng-show=' + configs.search.searchColumnsUIPickerForDocument.documentID.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.documentConfigContent.FlyoutLable2Text + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.documentID + '</div>\
                                       </div>\
                                        <div class="ms-Callout-content commonFlyoutContaint" ng-show=' + configs.search.searchColumnsUIPickerForDocument.documentVersion.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.documentConfigContent.FlyoutMenuColumn5DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.documentVersion + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint" ng-show=' + configs.search.searchColumnsUIPickerForDocument.documentOwner.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.documentConfigContent.FlyoutMenuColumn6DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent" toggle="parentToggle(message)">' + obj.documentOwner + '</div> \
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint" ng-show=' + configs.search.searchColumnsUIPickerForDocument.documentModifiedDate.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.documentConfigContent.FlyoutMenuColumn7DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent" datefilter date='+ obj.documentModifiedDate + '>' + obj.documentModifiedDate + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint" ng-show=' + configs.search.searchColumnsUIPickerForDocument.documentCreatedDate.displayInFlyOut + '>\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">' + scope.$parent.$parent.$parent.grid.appScope.vm.documentConfigContent.FlyoutMenuColumn7DisplayName + ':</div>\
                                          <div class="ms-font-m FlyoutContent" datefilter date=' + obj.documentCreatedDate + '>' + obj.documentCreatedDate + '</div>\
                                       </div>\
                                       <a class="ms-Button-label ms-Button ms-Button--primary ms-Callout-content" id="viewMatters" style="width:190px;padding-left: 12.5%;" href="' + obj.documentUrl + '" target="_blank">' + scope.$parent.$parent.$parent.grid.appScope.vm.documentConfigContent.FlyoutButton1Text + '</a>\
                                       <a id="uploadToMatter" class="ms-Button-label ms-Button ms-Button--primary ms-Callout-content" ng-show="loader" style="width:190px" ng-click="gotoUrl(\'' + obj.documentClientUrl + '\')" target="_blank">' + scope.$parent.$parent.$parent.grid.appScope.vm.documentConfigContent.FlyoutButton2Text + '</a>\
                                      </div>\
                                    </div>\
                                </div>';
                    $templateCache.put("test.html", actualcontent);
                    var template = $templateCache.get("test.html");
                    var a = $compile("<div>" + template + "</div>")(scope);
                    $('.dropdown').removeClass('open');
                    $('.popcontent').css('display', 'none');
                    e.stopPropagation();
                    var obj = e.target.getBoundingClientRect();
                    $(this).parent().find('.popcontent').html(a[0]);
                    if (obj.top < 350) {
                        $(this).parent().find('.popcontent').css({ 'display': 'block', 'left': '284px', 'top': '0' });
                        $(this).parent().find('.popcontent').find('.flyoutLeftarrow').css('top', '11px');
                    } else {
                        $(this).parent().find('.popcontent').css({ 'display': 'block', 'left': '284px', 'top': '-238px' });
                        $(this).parent().find('.popcontent').find('.flyoutLeftarrow').css('top', '244px');
                    }
                });
            },
            controller: function ($scope) {
                $scope.gotoUrl = function (url) {
                    $scope.$parent.$parent.$parent.grid.appScope.vm.gotoDocumentUrl(url);
                }
                $scope.stopEvent = function ($event) {
                    $event.stopPropagation();
                };

            }
        }
    };


    'use strict';
    function onload($timeout) {
        return {
            restrict: 'AE',
            scope: { sortdetails: '@' },
            link: function (scope, element, attrs) {
                scope.$watch("sortdetails", function () {
                    $timeout(function () { jQuery('[id^="asc"]').hide(); }, 1500);
                    $timeout(function () { jQuery('[id^="desc"]').hide(); }, 1500);
                }, true);
            }
        }
    };

    'use strict';
    function fallbacksrc() {
        return {
            restrict: 'A',
            link: function postLink(scope, iElement, iAttrs) {
                iElement.bind('error', function () {
                    var rest = iAttrs.src.substring(0, iAttrs.src.lastIndexOf("/") + 1);
                    var last = iAttrs.src.substring(iAttrs.src.lastIndexOf("/") + 1, iAttrs.src.length);
                    last = "generaldocument.png";
                    iAttrs.src = rest + last;
                    angular.element(this).attr("src", iAttrs.src);
                });
            }
        }
    };

    'use strict'
    function myEnter() {
        return function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attrs.myEnter);
                    });

                    event.preventDefault();
                }
            });
        };
    }

    'use strict'
    function uiGridMenuButton($window, $timeout) {
        return {
            restrict: 'AE',
            link: function (scope, element, attrs) {
                element.find('.ui-grid-icon-menu').addClass('showExpandIcon');

                $(element).on("click", function (e) {
                    e.stopPropagation();
                    $timeout(function () {
                        if (element.find('.ng-isolate-scope').html() != "<!-- ngIf: shown -->") {
                            element.find('.ui-grid-icon-menu').removeClass('showExpandIcon');
                            element.find('.ui-grid-icon-menu').addClass('closeColumnPicker');
                        } else {
                            element.find('.ui-grid-icon-menu').addClass('showExpandIcon');
                            element.find('.ui-grid-icon-menu').removeClass('closeColumnPicker');
                        }
                    }, 200)

                    if (element.find('.ui-grid-menu-mid')) {
                        element.find('.ui-grid-menu-mid').css('height', $window.innerHeight - 160);
                    }
                });
            }
        }
    }

    'use strict'
    function uiGridViewport($window) {
        return {
            restrict: 'AE',
            link: function (scope, element, attrs) {
                $(element).scroll(function (e) {
                    if ($window.innerHeight > 442) {
                        $('.popcontent').css('display', 'none');
                        $('.dropdown').removeClass("open");
                    }
                });
            }
        }
    }


    'use strict'
    function dropdown() {
        return {
            restrict: 'AE',
            link: function (scope, element, attrs) {
                $(element).click(function (e) {
                    var obj = e.target.getBoundingClientRect();
                    if (obj.top > 450) {
                        $(this).parent().addClass('dropup');
                    }
                });
            }
        }
    }

    'use strict'
    function assignTeamKeyDown() {
        return function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 8 || (event.which === 46 && event.key == "Delete")) {
                    scope.$apply(function () {
                        scope.assignTeam.assignedAllUserNamesAndEmails = scope.assignTeam.assignedUser;
                        var userEmails = getUserName(scope.assignTeam.assignedUser, false);
                        var exsistingTeams = [];
                        for (var i = 0; i < userEmails.length; i++) {
                            if (userEmails[i] != "") {
                                angular.forEach(scope.assignTeam.teamUsers, function (team) {
                                    if (team.userName == userEmails[i]) {
                                        exsistingTeams.push(team);
                                    }
                                });
                            }
                        }
                        scope.assignTeam.teamUsers = exsistingTeams;
                        // $scope.projects.splice(i, 1);
                        scope.$parent.cm.errorPopUpBlock = false;
                        scope.$parent.cm.errorBorder = "";
                    });
                } else {
                    // var txtUserEmails = scope.assignTeam.assignedUser;
                    //var userEmails= txtUserEmails.split(";");
                    // scope.assignTeam.assignedAllUserNamesAndEmails = scope.assignTeam.assignedUser;
                    if (event.which === 186) {
                        scope.assignTeam.assignedAllUserNamesAndEmails = scope.assignTeam.assignedUser + ";";
                    }
                    scope.$parent.cm.errorPopUpBlock = false;
                    scope.$parent.cm.errorBorder = "";
                }
            });
        };
    }
    'use strict'
    function getUserName(sUserEmails, bIsName) {

        var arrUserNames = [], sEmail = "", oEmailRegex = new RegExp("^[\\s]*\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*[\\s]*$");
        if (sUserEmails && null !== sUserEmails && "" !== sUserEmails) {
            arrUserNames = sUserEmails.split(";");
            for (var iIterator = 0; iIterator < arrUserNames.length - 1; iIterator++) {
                if (arrUserNames[iIterator] && null !== arrUserNames[iIterator] && "" !== arrUserNames[iIterator]) {
                    if (-1 !== arrUserNames[iIterator].lastIndexOf("(")) {
                        sEmail = $.trim(arrUserNames[iIterator].substring(arrUserNames[iIterator].lastIndexOf("(") + 1, arrUserNames[iIterator].lastIndexOf(")")));
                        if (oEmailRegex.test(sEmail)) {
                            arrUserNames[iIterator] = bIsName ? $.trim(arrUserNames[iIterator].substring(0, arrUserNames[iIterator].lastIndexOf("("))) : sEmail;
                        }
                    }
                }
            }
        }
        return arrUserNames;
    }

    var app = angular.module('matterMain');
    app.directive('onload', ['$timeout', onload]);
    app.directive('showbreadcrumb', [showbreadcrumb]);
    app.directive('datefilter', [datefilter]);
    app.directive("toggletab", [toggletab]);
    app.directive('infopopover', [infopopover]);
    app.directive('matterflyout', ['$compile', '$templateCache', matterflyout]);
    app.directive('documentflyout', ['$http', '$compile', '$templateCache', documentflyout]);
    app.directive('fallbacksrc', [fallbacksrc]);
    app.directive('myEnter', [myEnter]);
    app.directive('uiGridMenuButton', ['$window', '$timeout', uiGridMenuButton]);
    //Adding Window
    app.directive('uiGridViewport', ['$window', uiGridViewport]);
    app.directive('dropdown', [dropdown]);
    app.directive('assignteamkeydown', [assignTeamKeyDown]);
})();














