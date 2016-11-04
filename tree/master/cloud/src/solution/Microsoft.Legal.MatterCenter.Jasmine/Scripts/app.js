
(function () {

    // Enter Global Config Values & Instantiate ADAL AuthenticationContext
    window.config = {
        clientId: "89AE4FDE-B606-4817-A20E-5FFEFF3D8832", //Dummy client id (do not need to change)
        instance: 'https://login.microsoftonline.com/',
        postLogoutRedirectUri: window.location.origin,
    };

    var authContext = new AuthenticationContext(config);

    // Get UI jQuery Objects
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
        //if (null == sessionStorage.getItem('adal.idtoken') || "" === sessionStorage.getItem('adal.idtoken')) {
        //    authContext.login();
        //}
    });
}());