<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="../MasterPage/MatterCenterMaster.Master" CodeBehind="SendToOneDrive.aspx.cs" Inherits="Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages.SendToOneDrive" %>

<asp:Content ID="SendToOneDriveHeadContent" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <title>Send to One Drive</title>
    <link href="../Styles/BriefcaseOperations.css?ver=25.0.0.0" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="SendToOneDriveBodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="server">
    <div class="Container">
        <div class="BigMessage"></div>
        <div class="Question">
        </div>
        <div class="LoadingImage">
            <img src="../Images/WindowsLoadingFast.GIF" alt="Loading..." />
        </div>
        <div class="ButtonContainer">
            <div class="Button" onclick="overWriteFiles(true)">OK</div>
            <div class="Button" onclick="overWriteFiles(false)">Cancel</div>
        </div>
        <div class="NotSupportedButton">
            <div class="Button" onclick="closePopup()">OK</div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="SendToOneDriveScriptContent" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
    <script type="text/javascript" src="../Scripts/SendToOneDrive.js?ver=25.0.0.0"></script>
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
