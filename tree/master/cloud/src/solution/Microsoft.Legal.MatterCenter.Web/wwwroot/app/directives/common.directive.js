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
                    var actualcontent = "";
                    actualcontent = '<div class="" style="position:relative;display:table-row" ng-click="stopEvent($event)">\
                                   <div class="FlyoutBoxContent flyoutwidth">\
                                      <div class="flyoutLeftarrow hidden-xs" style="top: 11px;left: -9px;"></div>\
                                      <div class="flyoutToparrow visible-xs" style="top: -8px;"></div>\
                                      <div class="FlyoutContent FlyoutHeading">\
                                          <div class="ms-Callout-content FlyoutHeadingText">  ' + obj.matterName + ' </div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Client:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterClient + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Client.Matter ID:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterClientId + '.' + obj.matterID + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Sub area of law:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterSubAreaOfLaw + '</div> \
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Responsible attorney:</div>\
                                          <div class="ms-font-m FlyoutContent">' + obj.matterResponsibleAttorney + '</div>\
                                       </div>\
                                       <a id="viewMatters" class="ms-Button-label ms-Button ms-Button--primary ms-Callout-content" href="' + obj.matterClientUrl + '/SitePages/' + obj.matterGuid + '.aspx" target="_blank">View matter details</a><br/>\
                                       <a class="ms-Button-label ms-Button ms-Button--primary ms-Callout-content"  id="uploadToMatter" ng-click="openUpload(\'' + obj.matterName + '\',\'' + obj.matterClientUrl + '\',\'' + obj.matterGuid + '\')" type="button">Upload to a matter</a>\
                                    </div>\
                                </div>';
                    $templateCache.put("test.html", actualcontent);
                    var template = $templateCache.get("test.html");
                    var a = $compile("<div>" + template + "</div>")(scope);
                    $('.popcontent').css('display', 'none');
                    e.stopPropagation();
                    var obj = $(this).parent().position();
                    $(this).parent().find('.popcontent').html(a[0]);
                    $(this).parent().find('.popcontent').css({ 'display': 'block', 'left': '220px' });
                    $(this).parent().find('.popcontent').css('top', "0");
                });
            },
            controller: function ($scope) {
                $scope.openUpload = function (matterName, matterUrl, matterGUID) {
                    $scope.$parent.$parent.$parent.grid.appScope.vm.Openuploadmodal(matterName, matterUrl, matterGUID);
                    $('.popcontent').css('display', 'none');
                };
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
                                          <div class="ms-font-m FlyoutContent">' + obj.documentID + '</div>\
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Author:</div>\
                                          <div class="ms-font-m FlyoutContent" toggle="parentToggle(message)">' + obj.documentOwner + '</div> \
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Modified date:</div>\
                                          <div class="ms-font-m FlyoutContent" datefilter date='+ obj.documentModifiedDate + '>' + obj.documentModifiedDate + '</div>\
                                       </div>\
                                       <a class="ms-Button-label ms-Button ms-Button--primary ms-Callout-content" id="viewMatters" style="width:190px;padding-left: 12.5%;" href="' + obj.documentUrl + '" target="_blank">Open document</a>\
                                       <a id="uploadToMatter" class="ms-Button-label ms-Button ms-Button--primary ms-Callout-content" style="width:190px" ng-click="gotoUrl(\'' + obj.documentClientUrl + '\')" target="_blank">View document details</a>\
                                      </div>\
                                    </div>\
                                </div>';
                    $templateCache.put("test.html", actualcontent);
                    var template = $templateCache.get("test.html");
                    var a = $compile("<div>" + template + "</div>")(scope);
                    $('.popcontent').css('display', 'none');
                    e.stopPropagation();
                    var obj = $(this).parent().position();
                    $(this).parent().find('.popcontent').html(a[0]);
                    $(this).parent().find('.popcontent').css({ 'display': 'block', 'left': '240px' });
                    $(this).parent().find('.popcontent').css('top', "0");
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
})();














