<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="../MasterPage/MatterCenterMaster.Master" CodeBehind="ManagePermissions.aspx.cs" Inherits="Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages.ManagePermissions" %>

<asp:Content ID="ManagePermissionsHeadContent" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <title>Manage Permissions</title>
    <link href="../Styles/ManagePermissions.css?ver=25.0.0.0" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="ManagePermissionsBodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="server">
    <div class="loadingImage">
        <img class="loadingIcon" src="../Images/loadingGreen.GIF" alt="Loading" />
        <div class="loadingText">Working on it...</div>
    </div>
    <div class="error hide"></div>
    <div id="editpermissionContainer" class="hide">
        <div id="permissionLabel">
            <label class="entryLabels medium"><span class="mandatory">*</span>Name</label>
            <div class="gutter"></div>
            <label class="entryLabels small"><span class="mandatory">*</span>Role</label>
            <div class="gutter"></div>
            <label class="entryLabels"><span class="mandatory">*</span>Permission level</label>
        </div>
        <div class="errorPopUp hide">
            <div class="errTriangle popUpFloatLeft"></div>
            <div class="errTriangleBorder popUpFloatLeft"></div>
            <div class="errText popUpFloatRight"></div>
        </div>
        <div class="clear"></div>
        <div id="permissionContainer">
        </div>

        <div id="addMoreLink">
            <a id="addMorePermissions">Add More</a>
        </div>
        <div class="footer">
            <div class="loading hide">
                <img id="loadingImage" src="../Images/WindowsLoadingFast.GIF" alt="Loading" />
            </div>
            <button class="button" type="button" id="btnSave" value="Save">Save</button>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ManagePermissionsScriptContent" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.10.3.custom.js"></script>
    <script type="text/javascript" src="../Scripts/ManagePermissions.js?ver=25.0.0.0 "></script>
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
