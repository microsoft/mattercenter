/// <disable>JS3092,JS3058,JS3057,JS3054</disable>
/// <dictionary>Nofolder,arr,bool</dictionary>
var serviceVariables = {
    oTermStoreData: {}
};

(function () {
    "use strict";


    $(document).ready(function () {
        $(document).on("click", ".popUpSALContent", function () {
            $(".popUpSALContent").removeClass("popUpSelected");
            $(this).addClass("popUpSelected");
        });

        $(document).on("click", ".popUpDTContent", function () {
            $(".popUpDTContent").removeClass("popUpSelected");
            $(this).addClass("popUpSelected");
            removeDefaultContentTypeSelectionMessage();
        });

        $(".popUpLeftClick").click(function () {
            var oSelected = $(".popUpSAL .popUpSelected");
            /* Add Practice Groups and Area of Laws */
            var oPopUpPG = $(".popUpPG");
            if (oPopUpPG[0]) {
                var oSelectedPGOption = oPopUpPG;
                var oSelectedAreaOfLaw = $(".popUpMDContent.popUpSelected");
                var oSelectedSubAreaOfLaw = $(".popUpSALContent.popUpSelected");

                var sSelectedPracticeGroup = oSelectedPGOption.val();
                var sSelectedPracticeGroupId = oSelectedPGOption.attr("data-practicegroup-id");
                var sSelectedPracticeGroupFolderStructure = oSelectedPGOption.attr("data-folderNamesPG");
                // Fetch the folder structure from the Practice Group, Area of Law and Sub Area of Law levels

                var sSelectedAreaOfLaw = oSelectedAreaOfLaw.text();
                var sSelectedAreaOfLawId = oSelectedAreaOfLaw.attr("data-areaoflaw-id");
                var sSelectedAreaOfLawFolderStructure = oSelectedAreaOfLaw.attr("data-folderNamesAOL");
                var sSelectedSubAreaOfLawId = oSelectedSubAreaOfLaw.attr("data-subareaoflaw-id");
                var sSelectedSubAreaOfLawFolderStructure = oSelectedAreaOfLaw.attr("data-folderNamesSAL");
                var sSelectedSubAreaOfLawIsNofolderStructurePresent = oSelectedAreaOfLaw.attr("data-isNoFolderStructurePresent");

                // Practice Group
                oSelected.attr("data-PracticeGroup", sSelectedPracticeGroup);
                oSelected.attr("data-practicegroup-id", sSelectedPracticeGroupId);
                oSelected.attr("data-folderNamesPG", sSelectedPracticeGroupFolderStructure);

                // Area of Law
                oSelected.attr("data-AreaOfLaw", sSelectedAreaOfLaw);
                oSelected.attr("data-areaoflaw-id", sSelectedAreaOfLawId);
                oSelected.attr("data-folderNamesAOL", sSelectedAreaOfLawFolderStructure);

                // Sub Area of Law
                oSelected.attr("data-folderNamesSAL", sSelectedSubAreaOfLawFolderStructure);
                oSelected.attr("data-subareaoflaw-id", sSelectedSubAreaOfLawId);
                oSelected.attr("data-isNoFolderStructurePresent", sSelectedSubAreaOfLawIsNofolderStructurePresent);
            }

            var bPresent = false;
            if (oSelected) {
                /*Fixed duplicate issue*/
                var oIterate = $(".popUpDTContent");
                $.each(oIterate, function () {
                    if ($(this).attr("data-document-template") === oSelected.attr("data-document-template")) {
                        bPresent = true;
                        return false;
                    }
                });
                if (!bPresent) {
                    $(".popUpSAL > div, .popDT > div").removeClass("popUpSelected");
                    $(".popDT").append(oSelected.clone().removeClass("popUpSALContent").addClass("popUpDTContent popUpSelected"));
                    var currentSelectedSubAreaOfLaw = $(".popDT .popUpSelected");
                    if (currentSelectedSubAreaOfLaw.length > 0) {
                        var arrAssociatedDocumentTemplates = currentSelectedSubAreaOfLaw.attr("data-associated-document-template").split(";");
                        currentSelectedSubAreaOfLaw.attr("data-display-name", currentSelectedSubAreaOfLaw.text());
                        currentSelectedSubAreaOfLaw.html(currentSelectedSubAreaOfLaw.text() + " (" + arrAssociatedDocumentTemplates.length + ")");
                    }
                }
                /*-----*/
            }
            $(".popUpDTContent").removeClass("popUpSelected");
        });

        $(".popUpRightClick").click(function () {
            var oSelected = $(".popDT .popUpSelected");
            if (oSelected) {
                $(".popUpSAL > div, .popDT > div").removeClass("popUpSelected");
                oSelected.remove();
            }
        });

        $(document).on("click", ".popUpMDContent", function () {
            var oSAL = $(".popUpSAL"), sFolderNames, boolIsFolderStructurePresent, sSubAreaOfLawId;
            oSAL.find(".popUpSALContent").remove();
            $(".popUpMDContent").removeClass("popUpSelected");
            $(this).addClass("popUpSelected");
            localStorage.iSelectedAOL = $(this).attr("data-value");
            for (var iIterator = 0; iIterator < serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms.length; iIterator++) {
                sFolderNames = serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].FolderNames ? serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].FolderNames : "";
                boolIsFolderStructurePresent = serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].IsNoFolderStructurePresent ? serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].IsNoFolderStructurePresent : "";
                sSubAreaOfLawId = serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].Id ? serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].Id : "";
                oSAL.append("<div class='popUpSALContent' data-value='" + iIterator + "' data-document-template='" + serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].DocumentTemplates + "' data-associated-document-template='" + serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].DocumentTemplateNames + "' data-folderNamesSAL='" + sFolderNames + "' data-isNoFolderStructurePresent='" + boolIsFolderStructurePresent + "' data-display-name='" + serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].TermName + "' data-subareaoflaw-id='" + sSubAreaOfLawId + "' >" + serviceVariables.oTermStoreData.PGTerms[localStorage.iSelectedPG].AreaTerms[localStorage.iSelectedAOL].SubareaTerms[iIterator].TermName + "</div>");
            }
            $(".popUpSAL .popUpSALContent:first-child").click();
        });



        $(".popUpCloseIcon").click(function () {
            $(".popupContainerBackground, .popUpContainer").addClass("hide");
            $(".popDT .popUpDTContent").remove();
            // Clear search by typing fields
            $(".popUpMDTextArea").val("");
            removeDefaultContentTypeSelectionMessage();
        });
        $(document).on("click", ".popupContainerBackground", function () {
            $(".popUpCloseIcon").click();
            removeDefaultContentTypeSelectionMessage();
        });
    });
}());

$(".popUpContainer").click(function (e) {
    "use strict";
    if (e.target.className === "SavePopUpEntries") {
        saveEntry();
    } else {
        removeDefaultContentTypeSelectionMessage();
    }
});
function setDefaultContentTypeSelectionMessage() {
    "use strict";
    $(".selectDefaultContentTypeMessage").css("visibility", "visible");
    $(".errorPopUpArea").removeClass("hide");
}
//// function for removing error popup message

function removeDefaultContentTypeSelectionMessage() {
    "use strict";
    $(".selectDefaultContentTypeMessage").css("visibility", "hidden");
    $(".errorPopUpArea").addClass("hide");
}
function saveEntry() {
    "use strict";
    var defaultContentType = $(".popDT .popUpSelected").attr("data-document-template");
    if (defaultContentType) {
        localStorage.setDefaultContentType = defaultContentType;
    } else {
        localStorage.setDefaultContentType = "";
    }
    if (0 < $(".popDT .popUpSelected").length) {
        $(".popupContainerBackground, .popUpContainer").addClass("hide");
        $("#documentTemplates .docTemplateItem").remove();

        /* Display selected items on the page */
        var oPopUpDTContent = $(".popUpDTContent");
        var oContentTypes = $("#documentTemplates");
        if (oPopUpDTContent[0] && oContentTypes) {
            oContentTypes.append(oPopUpDTContent.removeClass("popUpDTContent").addClass("docTemplateItem"));
        }
        $(".popDT .popUpDTContent").remove();
        // Clear search by typing fields
        $(".popUpMDTextArea").val("");
    } else {
        $(".selectDefaultContentTypeMessage").css("visibility", "visible");
    }
    if (!($(".popUpDTContent").hasClass("popUpSelected"))) {
        setDefaultContentTypeSelectionMessage();
    } else {
        removeDefaultContentTypeSelectionMessage();
    }
}