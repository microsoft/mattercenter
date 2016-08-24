
(function () {

    // Enter Global Config Values & Instantiate ADAL AuthenticationContext
    window.config = {
        instance: 'https://login.microsoftonline.com/',
        tenant: '9a689192-bfc9-4d08-94b9-64b31bc60540', //Enter tenant Name e.g. microsoft.onmicrosoft.com
        clientId: '61394aba-09ba-4e25-ae32-e10805c6841b', //Enter your app Client ID created in Azure Portal
        postLogoutRedirectUri: window.location.origin,
        //cacheLocation: 'localStorage', // enable this for IE, as sessionStorage does not work for localhost.
    };

    var authContext = new AuthenticationContext(config);

    // Get UI jQuery Objects
    //var $panel = $(".panel-body");
    var $userDisplay = $(".app-user");
    var $signInButton = $(".app-login");
    var $signOutButton = $(".app-logout");
    var $errorMessage = $(".app-error");
    var $apiBtn = $("#btnAPI");

    // Check For & Handle Redirect From AAD After Login
    var isCallback = authContext.isCallback(window.location.hash);
    authContext.handleWindowCallback();
    $errorMessage.html(authContext.getLoginError());

    if (isCallback && !authContext.getLoginError()) {
        window.location = authContext._getItem(authContext.CONSTANTS.STORAGE.LOGIN_REQUEST);
    }

    // Check Login Status, Update UI
    var user = authContext.getCachedUser();
    if (user) {
        $userDisplay.html(user.userName);
        $userDisplay.show();
        $signInButton.hide();
        $signOutButton.show();
    } else {
        $userDisplay.empty();
        $userDisplay.hide();
        $signInButton.show();
        $signOutButton.hide();
    }

    // Register NavBar Click Handlers
    $signOutButton.click(function () {
        authContext.logOut();
    });
    $(document).ready(function () {
        if (null == sessionStorage.getItem('adal.idtoken') || "" === sessionStorage.getItem('adal.idtoken')) {
            authContext.login();
        }
    });
}());