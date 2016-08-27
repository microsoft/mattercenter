"use strict";
var matter = matter || {};
matter.upload = matter.upload || {};
(function () {
    
    matter.upload.uploadManager = function () {        
        var oUploadGlobal = {
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
            bAllowContentCheck: false
        };

        function attachmentTokenCallbackEmailClient(asyncResult, userContext) {
            "use strict";
            if (asyncResult.status === "succeeded") {
                vm.attachmentToken = asyncResult.value;
                //createMailPopup();
            } else {
                //showNotification(oFindMatterConstants.Fail_Attachment_Token, "failNotification");
            }
        }

        vm.getIconSource = function (sExtension) {
            var iconSrc = configs.Upload.ImageDocumentIcon.replace("{0}", sExtension);
            iconSrc = (-1 < configs.Upload.PNGIconExtensions.indexOf(sExtension)) ?
                            iconSrc.substring(0, configs.Upload.ImageDocumentIcon.lastIndexOf(".") + 1) + "png" : iconSrc;
            return iconSrc;
        }

        function checkEmptyorWhitespace(input) {
            "use strict";
            if (/\S/.test(input)) {
                return input;
            }
            return oFindMatterConstants.No_Subject_Mail;
        }
    
        var publics =
        {
            attachmentTokenCallbackEmailClient: attachmentTokenCallbackEmailClient,
            oUploadGlobal: oUploadGlobal,
        };
        return publics;
    }
}());