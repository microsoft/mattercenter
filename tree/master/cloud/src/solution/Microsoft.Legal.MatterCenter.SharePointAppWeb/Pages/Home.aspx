<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages.Home" MasterPageFile="~/MasterPage/MatterCenterMaster.Master" %>
<asp:Content ID="PageTitleContent" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="server">
    Home
</asp:Content>
<asp:Content ID="HomeHeadContent" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="../Styles/Home.css?ver=25.0.0.0" />
</asp:Content>
<asp:Content ID="HomeBodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="server">
    <div id="HomeContainer">
        <header class="WelcomeBar ms-bg-color-neutralLighterAlt">
            <span class="ms-font-s">Welcome to Matter Center! Upload, share, and save your documents here. 
                <a class="LearnMoreLink" title="Learn more about Matter Center">Learn more</a> or <a class="DismissLink" title="Remove this section">dismiss</a>
            </span>
        </header>
        <div class="LinkContainer">
            <div class="LeftLinkContainer">
                <div class="MattersContainer" title="Search matters">
                    <figure>
                        <img src="../Images/MC_Matters.png" alt="Search Matters" />
                        <figcaption class="FigureCaption">Matters</figcaption>
                    </figure>
                </div>
                <div class="DocumentsContainer" title="Search documents">
                    <figure>
                        <img src="../Images/MC_Documents.png" alt="Search Documents" />
                        <figcaption class="FigureCaption">Documents</figcaption>
                    </figure>
                </div>
            </div>
            <div class="RightLinkContainer">
                <div class="SecondaryLinkContainer UploadAttachments" title="Upload documents to a matter">
                    <a class="UploadAttachmentsLink">
                        <img class="SecondaryLinksImg" src="../Images/MC_Upload.png" alt="Upload attachments" />
                    </a>
                    <a class="UploadAttachmentsLink">
                        <span class="SecondaryLinksText">Upload attachments</span>
                    </a>
                </div>
                <div class="SecondaryLinkContainer CreateMatter" runat="server" id="CreateMatterLink" title="Create a new matter site">
                    <a class="CreateMatterLink">
                        <img class="SecondaryLinksImg" src="../Images/MC_Create_a_matter.png" alt="Create a new matter" />
                    </a>
                    <a class="CreateMatterLink">
                        <span class="SecondaryLinksText">Create a new matter</span>
                    </a>
                </div>
                <div class="SecondaryLinkContainer MatterDashboard" title="Go to the web home page">
                    <a class="MatterDashboardLink">
                        <img class="SecondaryLinksImg" src="../Images/MC_Go_to_dashboard.png" alt="Go to Matter Center Home" />
                    </a>
                    <a class="MatterDashboardLink">
                        <span class="SecondaryLinksText">Go to Matter Center Home</span>
                    </a>
                </div>
            </div>
        </div>
        <footer class="SupportLink" title="Write an email to Support">
            <span class="ms-font-m ms-font-weight-semilight">Questions? Contact <a class="MatterCenterSupportLink">Matter Center Support</a></span>
        </footer>
        <div id="HeaderPlaceHolderContent">
            <span class="AppLogoContainer hide" title="Matter Center home">
                <img class="AppLogo" src="../Images/MC_Logo.png" alt="Matter Center" />                
            </span>
        </div>
    </div>
</asp:Content>
<asp:Content ID="HomeScriptContent" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
    <script src="../Scripts/Home.js?ver=25.0.0.0"></script>
</asp:Content>

