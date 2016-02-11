<%@ Page Title="Search Matter" Language="C#" MasterPageFile="~/MasterPage/MatterCenterMaster.Master" AutoEventWireup="true" CodeBehind="FindMatter.aspx.cs" Inherits="Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages.SearchMatter" %>

<asp:Content ID="PageTitleContent" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="server">
    Search Matter
</asp:Content>

<asp:Content ID="SearchMatterHeadContent" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link href="../Styles/GridView.css?ver=25.0.0.0" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="../Styles/UploadDocument.css?ver=25.0.0.0" />
    <link rel="stylesheet" href="../Styles/SearchMatter.css" />
    <link rel="stylesheet" type="text/css" href="../Styles/CommonControls.css?ver=25.0.0.0" />
</asp:Content>
<asp:Content ID="SearchMatterBodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="server">
    <div id="gridView" class="clear"></div>
    <div class="textFlyoutContent hide"></div>
    <div class="dateFlyoutContent hide"></div>
    <div class="InfoFlyout hide"></div>
</asp:Content>
<asp:Content ID="SearchMatterScriptContent" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
    <script type="text/javascript" src="../Scripts/jsonGrid.js?ver=25.0.0.0"></script>
    <script type="text/javascript" src="../Scripts/ECBControl.js?ver=25.0.0.0"></script>
    <script type="text/javascript" src="../Scripts/GridView.js?ver=25.0.0.0"></script>
    <script type="text/javascript" src="../Scripts/CommonControls.js?ver=25.0.0.0"></script>
    <script type="text/javascript" src="../Scripts/Upload.js?ver=25.0.0.0"></script>
    <script type="text/javascript" src="../Scripts/SearchMatter.js?ver=25.0.0.0"></script>
</asp:Content>
