<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="../MasterPage/MatterCenterMaster.Master" CodeBehind="BriefcaseOperations.aspx.cs" Inherits="Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages.BriefcaseOperations" %>

<asp:Content ID="BriefcaseOperationHeadContent" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <title>Processing</title>
    <link href="../Styles/BriefcaseOperations.css?ver=25.0.0.0" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="BriefcaseOperationBodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="server">
    <div class="Container">
        <div class="BigMessage"></div>
        <div class="Question">
        </div>
        <div class="ButtonContainer">
            <span class="Button" onclick="checkOperation()">OK</span>
            <span class="Button" onclick="closePopup()">Cancel</span>
        </div>
        <div class="NotSupportedButton">
            <span class="Button" onclick="closePopup()">OK</span>
        </div>
    </div>
    <div class="VersionDiv">
        <div class="CheckInContainer">
            <div class="CheckInRow RowHeaders VersionDetails">
                Version
            </div>
            <div class="CheckInRow VersionDetails">
                <span class="CheckInColumn FirstColumn">Select the type of version you would like to check in.
                </span>
                <span class="CheckInColumn SecondColumn">
                    <span class="CheckInSecondCloumnHeader">What kind of version would you like to check in?</span>
                    <input type="radio" name="confirmVersion" value="0" class="CheckInOptions" checked="checked" /><span class="MinorVersion"> Minor Version (draft)</span>
                    <input type="radio" name="confirmVersion" value="1" class="CheckInOptions" /><span class="MajorVersion"> Major Version (publish)</span>
                    <input type="radio" name="ConfirmVersion" value="2" class="CheckInOptions" /><span class="OverwriteVersion"> Overwrite the current minor version</span>
                </span>
            </div>
            <div class="CheckInRow RowHeaders">
                Retain Checkout
            </div>
            <div class="CheckInRow">
                <span class="CheckInColumn FirstColumn">Other users will not see your changes until you check in. If you wish to continue editing, you can retain your check out after checking in.
                </span>
                <span class="CheckInColumn SecondColumn">
                    <span class="CheckInSecondCloumnHeader">Retain your check out after checking in?</span>
                    <input type="radio" name="confirm" class="RetainOptions" id="RetainOptionsTrue" value="true" onclick="toggleConvenienceOption()" />
                    <span class="RetainValues" id="RetainOptionYes">Yes</span>
                    <input type="radio" name="confirm" class="RetainOptions" id="RetainOptionsFalse" value="false" checked="checked" onclick="toggleConvenienceOption()" />
                    <span class="RetainValues" id="RetainOptionNo">No</span>

                    <span id="ConvenienceCopy">
                        <span class="ConvenienceCopyCloumnHeader">Would you like to keep a convenience copy in your OneDrive upon check in?</span>

                        <input type="radio" name="confirmConvenience" id="confirmConvenienceTrue" class="ConvenienceOptions" value="true" />
                        <span class="confirmConvenienceValues" id="confirmConvenienceOptionYes">Yes</span>
                        <input type="radio" name="confirmConvenience" id="confirmConvenienceFalse" class="ConvenienceOptions" checked="checked" value="false" />
                        <span class="confirmConvenienceValues" id="confirmConvenienceOptionNo">No</span>
                    </span>
                </span>
            </div>
            <div class="CheckInRow RowHeaders">
                Comments
            </div>
            <div class="CheckInRow">
                <span class="CheckInColumn FirstColumn">Type comments describing what has changed in this version.
                </span>
                <span class="CheckInColumn SecondColumn">
                    <span class="commentsDiv">Comments:</span>
                    <textarea id="CheckInComments" rows="5" cols="35" title="Comments"></textarea>
                </span>
            </div>
        </div>
        <div class="CheckInButtonContainer">
            <div class="Button" onclick="checkInDocument()">
                OK
            </div>
            <div class="Button" onclick="closePopup()">
                Cancel
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="BriefcaseOperationScriptContent" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
    <script type="text/javascript" src="../Scripts/BriefcaseOperations.js?ver=25.0.0.0"></script>
    <script type="text/javascript">
        (function () {
            if (window.top !== window.self) {
                function GenerateGUID() {
                    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
                }
                var guid = (GenerateGUID() + GenerateGUID() + "-" + GenerateGUID() + "-4" + GenerateGUID().substr(0, 3) + "-" + GenerateGUID() + "-" + GenerateGUID() + GenerateGUID() + GenerateGUID()).toLowerCase();
                oMasterGlobal.Tokens = guid + ":" + guid;
            }
        })();
    </script>
</asp:Content>
