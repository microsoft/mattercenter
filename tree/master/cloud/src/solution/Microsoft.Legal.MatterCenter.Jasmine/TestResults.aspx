<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestResults.aspx.cs" Inherits="Microsoft.Legal.MatterCenter.Jasmine.TestResults" %>
<!DOCTYPE html>
<html>
<head>
    <title>Jasmine Verification</title>
    <script src="lib/jquery/dist/jquery.js"></script>
    <link rel="shortcut icon" type="image/png" href="lib/jasmine-2.4.1/jasmine_favicon.png" />
    <script src="lib/angular/angular.js"></script>
    <script src="/lib/jasmine-2.4.1/jasmine.js"></script>
    <link href="/lib/jasmine-2.4.1/jasmine.css" rel="stylesheet" />
    <script src="/lib/jasmine-2.4.1/jasmine-html.js"></script>
    <script src="/lib/jasmine-2.4.1/boot.js"></script>
    <script src="/lib/jasmine-2.4.1/console.js"></script>
    <script data-require="angular-mocks" data-semver="1.2.1" src="//code.angularjs.org/1.2.1/angular-mocks.js"></script>
    <script src="/lib/jquery/dist/jquery.js"></script>
    <script src="/lib/jquery-validation/dist/jquery.validate.js"></script>
    <script src="/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js"></script>
    <script src="/lib/bootstrap/dist/js/bootstrap.js"></script>
    <script src="/lib/angular-route/angular-route.js"></script>
    <script data-require="angular-mocks" data-semver="1.2.1" src="//code.angularjs.org/1.2.1/angular-mocks.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/angular-ui-router/0.2.15/angular-ui-router.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.3.16/angular-touch.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.5.2/angular-animate.js"></script>
    <script src="/lib/bootstrap/dist/js/bootstrap-tooltip.js"></script>
    <script src="/lib/bootstrap/dist/js/bootstrap-popover.js"></script>
    <script src="/lib/angular-resource/angular-resource.js"></script>
    <script src="/lib/angular-sanitize/angular-sanitize.js"></script>
    <script src="/lib/adal-angular/dist/adal.min.js"></script>
    <script src="/lib/adal-angular/dist/adal-angular.min.js"></script>
    <script src="/lib/angular-ui-grid/ui-grid.min.js"></script>
    <script src="/lib/angular-bootstrap/ui-bootstrap-tpls.js"></script>
    <script src="/scripts/site.js" asp-append-version="true"></script>
    <script src="https://appsforoffice.microsoft.com/lib/1.1/hosted/office.js"></script>
    <script src="lib/angular-sanitize/angular-sanitize.js"></script>
    <style>
        #load {
            position: absolute;
            border: 16px solid #f3f3f3; /* Light grey */
            border-top: 16px solid #8a4182; /* Purple */
            border-radius: 50%;
            width: 120px;
            height: 120px;
            animation: spin 2s linear infinite;
            left: 46%;
            top: 40%;
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }
    </style>

    <script type="text/javascript">

        function waitForElementToDisplay(selector, time) {

            if (document.querySelector(selector) != null) {
                document.getElementById("load").style.visibility = "hidden"
                return;
            }
            else {
                setTimeout(function () {
                    waitForElementToDisplay(selector, time);
                }, 0);
            }
        }

    </script>

</head>
<body>

    <div id="load"> </div>
    <script type="text/javascript">waitForElementToDisplay(".jasmine-summary", 0);</script>

    <div id="errorBlock" class="hide">
    </div>
    <div id="errTrinagleBlock" class="hide">
    </div>
    <div id="errTrinagleBroderBlock" class="hide">
    </div>
    <div id="errText" class="hide">
    </div>
    <div id="txtUser1" class="hide">
    </div>
    <%--<script src="/Scripts/app.js"></script>--%>
    <script src="/Object/mockObjects.js"></script>
    <script src="/Scripts/TestConfig.js"></script>
    <script src="/Object/ObjectCollection.js"></script>
    <script src="/Scripts/uiconfig.js"></script>
    <script src="js/app.js"></script>
    <script src="js/config.js"></script>
    <script src="Scripts/SourceReference.js"></script>

    <!-- *******************  All the specs file to verify Controllers ************************-->

    <script src="Specs/CreateMatterController.spec.js"></script>
    <script src="Specs/CreateMatterControllerservicecall.spec.js"></script>

    <script src="Specs/DocumentController.spec.js"></script>
    <script src="Specs/DocumentControllerservicecall.spec.js"></script>

    <script src="Specs/DocumentDashboardController.spec.js"></script>
    <script src="Specs/DocumentDashboardControllerServiceCall.spec.js"></script>

    <script src="Specs/HomeController.spec.js"></script>
    <script src="Specs/HomeControllerservicecall.spec.js"></script>

    <script src="Specs/MatterController.spec.js"></script>
    <script src="Specs/MatterControllerservicecall.spec.js"></script>

    <script src="Specs/MatterDashboardController.spec.js"></script>
    <script src="Specs/MatterDashboardControllerservicecall.spec.js"></script>

    <script src="Specs/NavigationController.spec.js"></script>
    <script src="Specs/MatterUsersController.spec.js"></script>
    <script src="Specs/SettingsController.spec.js"></script>

</body>
</html>