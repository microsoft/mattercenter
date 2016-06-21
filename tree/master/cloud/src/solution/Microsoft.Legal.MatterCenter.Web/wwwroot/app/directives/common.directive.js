(function () {
   
    'use strict';
    function popover() {
        return {
            restrict: 'A',
            scope: { details: '@' },
            link: function (scope, element, attrs) {
                scope.$watch("details", function () {
                    var obj = "";
                    obj = eval('(' + attrs.details + ')');
                    var actualcontent = "";
                    actualcontent = '<div class="">\
                                   <div class="FlyoutBoxContent" style="width: 350px;">\
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
                                       <button class="ms-Button ms-Button--primary ms-Callout-content" id="viewMatters"><a class="ms-Button-label" href="https://msmatter.sharepoint.com/sites/microsoft/SitePages/' + obj.matterGuid + '.aspx" target="_blank">View matter details</a></button>\
                                       <button class="ms-Button ms-Button--primary ms-Callout-content" id="uploadToMatter"><a class="ms-Button-label" onclick="Openuploadmodal(\'' + obj.matterName + '\',\'' + obj.matterUrl + '\')" type="button">Upload to a matter</a></button>\
                                    </div>\
                                </div>';
                    $(element).popover({
                        html: true,
                        trigger: 'click',
                        delay: 500,
                        content: actualcontent,
                    });
                }, true);
            }
        }
    }

    'use strict';
    function onload($timeout) {
        return {
            restrict: 'E',
            link: function (scope, element, attrs) {
                $timeout(function () { $('[id^="asc"]').hide(); }, 1000);
                $timeout(function () { $('[id^="desc"]').hide(); }, 1000);
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
    function popoverdoc() {
        return {
            restrict: 'AE',
            scope: { details: '@' },
            link: function (scope, element, attrs) {
                scope.$watch("details", function () {
                    var obj = eval('(' + attrs.details + ')');
                    var content = '<div class="">\
                                   <div class="FlyoutBoxContent" style="width: 350px;">\
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
                                          <div class="ms-font-m FlyoutContent">' + obj.documentOwner + '</div> \
                                       </div>\
                                       <div class="ms-Callout-content commonFlyoutContaint">\
                                          <div class="fontWeight600 ms-font-m FlyoutContentHeading">Modified date:</div>\
                                          <div class="ms-font-m FlyoutContent" datefilter date='+ obj.documentModifiedDate + '>' + obj.documentModifiedDate + '</div>\
                                       </div>\
                                       <button class="ms-Button ms-Button--primary ms-Callout-content" id="viewMatters"><a class="ms-Button-label" href="' + obj.documentUrl + '" target="_blank">Open document</a></button>\
                                       <button class="ms-Button ms-Button--primary ms-Callout-content" id="uploadToMatter"><a class="ms-Button-label" href="https://msmatter.sharepoint.com/sites/catalog/SitePages/documentDetails.aspx" target="_blank">View document details</a></button>\
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

    var app = angular.module('matterMain');
    app.directive('popover', [popover]);
    app.directive('onload',['$timeout', [onload]]);
    app.directive('showbreadcrumb', [showbreadcrumb]);
    app.directive('datefilter', [datefilter]);
    app.directive('popoverdoc', [popoverdoc]);
    app.directive("toggletab", [toggletab]);
    app.directive('infopopover', [infopopover]);


})();














