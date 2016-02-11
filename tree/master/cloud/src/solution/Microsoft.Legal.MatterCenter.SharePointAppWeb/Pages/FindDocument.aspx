<%@ Page Title="Search Document" Language="C#" MasterPageFile="~/MasterPage/MatterCenterMaster.Master" AutoEventWireup="true" CodeBehind="FindDocument.aspx.cs" Inherits="Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages.SearchDocument" %>

<asp:Content ID="PageTitleContent" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="server">
    Search Document
</asp:Content>
<asp:Content ID="SearchDocumentHeadContent" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link href="../Styles/GridView.css?ver=25.0.0.0" rel="stylesheet" />
    <link href="../Styles/SearchDocument.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="../Styles/CommonControls.css?ver=25.0.0.0" />
</asp:Content>
<asp:Content ID="SearchDocumentBodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="server">
    <div class="errorAttachDocument hide">
        <img title="Warning" class="warningMessageIcon" src="../images/warning-message.png" alt="Warning icon" />
        <div class="warningMessageText"></div>
    </div>
    <div class="warningPopUpHolder hide">
        <div class="warningPopupBackground"></div>
        <div class="warningPopUpContainer">
            <div class="attachedSuccessPopUp hide">
                <div class="warningPopUpDetails"></div>
                <div title="Close" class="warningPopUpCloseIconContainer">Close</div>
            </div>
            <div class="attachedProgressPopUp hide">
                <div class="progressPopUpDetails"></div>
                <div class="warningLoadingIcon">
                    <img src="../Images/WindowsLoadingFast.GIF" alt="Loading..." title="Loading..." />
                </div>
            </div>
            <div class="attachedFailedPopUp hide">
                <div class="attachedPopUpDetails"></div>
                <div class="failureDocumentList"></div>
                <div title="Close" class="attachedPopUpCloseIconContainer">Close</div>
            </div>
        </div>
    </div>
    <div id="gridView" class="clear"></div>
    <button class="ms-Button ms-Button--primary ms-Callout-content is-disabled hide" id="attachDocuments" value="Save">
        <span class="ms-Button-label" id="attachButtonText"></span>
        <span class="ms-Button-description"></span>
    </button>
    <div class="gridViewButtonGroup hide">
        <div id="SendToOneDrive"></div>
        <div id="EmailDocumentLinks"></div>
    </div>
    <div class="textFlyoutContent hide"></div>
    <div class="dateFlyoutContent hide"></div>
    <div class="InfoFlyout hide"></div>
</asp:Content>
<asp:Content ID="SearchDocumentScriptContent" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
    <script type="text/javascript" src="../Scripts/jsonGrid.js?ver=25.0.0.0"></script>
    <script type="text/javascript" src="../Scripts/ECBControl.js?ver=25.0.0.0"></script>
    <script type="text/javascript" src="../Scripts/GridView.js?ver=25.0.0.0"></script>
    <script type="text/javascript" src="../Scripts/CommonControls.js?ver=25.0.0.0"></script>
    <script type="text/javascript" src="../Scripts/SearchDocument.js?ver=25.0.0.0"></script>
</asp:Content>

