<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="../MasterPage/MatterCenterMaster.Master" CodeBehind="WebDashboard.aspx.cs" Inherits="Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages.WebDashboard" %>

<asp:Content ID="PageTitleContent" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="server">
    Matter Center Beta Web Dashboard
</asp:Content>

<asp:Content ID="WebdashboardHeadContent" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" href="../Styles/jsonGridWebDashboard.css?ver=25.0.0.0" />
    <link rel="stylesheet" type="text/css" href="../Styles/UploadDocument.css?ver=25.0.0.0" />
    <link rel="stylesheet" type="text/css" href="../Styles/WebDashboard.css?ver=25.0.0.0" />
    <link rel="stylesheet" type="text/css" href="../Styles/MailCartPopup.css?ver=25.0.0.0" />
</asp:Content>
<asp:Content ID="WebdashboardBodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="server">
    <div class="popupWait hide"></div>
    <div class="loadingImage hide" id="popupWaitLoadingImage">
        <a title="Matter Center Home" id="mcIcon" href="/sitepages/MatterCenterHome.aspx" target="_parent">
            <img title="Matter Center" src="../Images/WindowsLoadingFast.GIF" alt="Matter Center" />
        </a>
    </div>
    <div class="popupContainerBackground hide"></div>
    <div id="mainDivContainer">
        <div class="matterPopup"></div>
        <div class="documentPopup"></div>
        <div class="clear"></div>
        <div class="popUpContainer hide">
            <header class="popUpHeader">
                <div class="popUpHeaderTitle popUpFloatLeft"></div>
                <div class="notificationContainerForPopup"></div>
                <img class="mailCartCloseIcon" alt="Close" src="../Images/close-666.png" title="Close" />
            </header>
            <div id="PopupBody">
                <div class="selectMailFormat">
                    <div class="sendEmailOptions sendToOneDriveOption" onclick="sendToBriefcase();">
                        <img class="mailCartLogo" src="../Images/my-briefcase-666.png" alt="Send to My OneDrive for Business" title="Send to My OneDrive" />
                        <div title="Send to My OneDrive for Business" class="inlineValues">Send to My OneDrive for Business</div>
                    </div>
                    <div class="sendEmailOptions emailAttachmentOption" onclick="downloadEmail(true);">
                        <img class="mailCartLogo" src="../Images/attach_666_27X27.png" alt="Email as attachment" title="Email as attachment" /><div title="Email as attachment" class="inlineValues">Email as attachment</div>
                    </div>
                    <div class="sendEmailOptions emailLinkOption" onclick="downloadEmail(false);">
                        <img class="mailCartLogo" src="../Images/link-666.png" alt="Email as link" title="Email as link" /><div title="Email as link" class="inlineValues">Email as link</div>
                    </div>
                </div>
                <div class="popUpBody popUpFloatLeft">
                    <div class="checkOutPopup">
                        <div class="checkOutPopupPointer"></div>
                        <div class="checkOutPopupData">
                            <div class="checkOutMetadata marginTop10"></div>
                        </div>
                    </div>
                    <ul class="emailDocumentNames">
                    </ul>
                    <div class='pinLoadingImage' id='mailCartLoadingImage'>
                        <img src='/Images/WindowsLoadingFast.GIF' title='Loading' class='LoadingImage' alt='Loading...' />
                    </div>
                </div>
            </div>
        </div>
        <div class="matterCenterHeaderBackground hide"></div>
        <div id="contentDiv">
            <div class="LeftContent">

                <div class="mailpopupContainerBackground hide"></div>
                <div class="mailContainer hide">
                    <div class="notificationContainerForMailPopup"></div>
                    <img class="popUpCloseIcon " alt="Close" src="../Images/close-666.png" title="Close" />
                    <div id="mailContent">
                        <header class="mailHeader">
                            <div id="attachmentHeader" class="hide" title="Drag and drop items to the folders below">
                                <span class="attachmentHeaderInstruction"></span>
                            </div>
                        </header>
                        <div id="mailBody"></div>
                        <div id="mailFooter">
                            <img alt="info" id="popupInfoIcon" src="../Images/information-blue.png" />
                            <div id="popupFooterText">
                                You can also drag and drop files from your desktop.
                            </div>
                        </div>
                    </div>
                </div>
                <div class="appName hide">
                    <div class="appLink">
                        <a>
                            <img alt="Matter Center" class="appImage" src="../Images/Portal_logo_png.png" />
                            <div class="appTitle"><span>Matter Center</span></div>
                            <div class="appTitleSmall"><span>for Office 365</span></div>
                        </a>
                    </div>
                </div>
                <div class="Search" id="Search">
                    <div class="bannerMatterWorkspace">
                    </div>
                </div>
                <div id="DisplayResults">
                    <div id="DisplayFindMattersResults" class="fallbackFlex">
                        <div title="Manage your recent matters: upload emails or documents to a matter, review matter details, or pin matters" class="mySearch active" id="RecentMattersHeader" onclick="togglePinnedView(this, 2)">
                            <span class="resultsTab">My matters</span>
                            <span class="ResultNumbers">(0)</span>
                        </div>
                        <div title="Manage matters: upload emails or documents to a matter, review matter details, or pin matters" class="allSearch" onclick="togglePinnedView(this, 0)">
                            <span class="resultsTab">All matters</span>
                            <span class="ResultNumbers">(0)</span>
                        </div>

                        <div title="Manage your pinned matters: upload emails or documents to a matter, review matter details, or unpin matters" class="pinnedSearch" id="PinnedMatters" onclick="togglePinnedView(this, 1)">
                            <span class="resultsTab">Pinned matters</span>
                            <span class="ResultNumbers">(0)</span>
                        </div>


                        <div class="viewSwitcher"><span>…</span></div>
                    </div>
                    <div id="DisplaySearchDocumentsResults" class="hide">
                        <div title="Manage your recent documents: view, edit, or pin a document" class="mySearch active" id="RecentDocuments" onclick="togglePinnedView(this, 2)">
                            <span class="resultsTab">My documents</span>
                            <span class="ResultNumbers">(0)</span>
                        </div>
                        <div title="Manage documents: view, edit, or pin a document" class="allSearch" onclick="togglePinnedView(this, 0)">
                            <span class="resultsTab">All documents</span>
                            <span class="ResultNumbers">(0)</span>
                        </div>
                        <div title="Manage documents: view, edit, or pin a document" class="pinnedSearch" id="PinnedDocuments" onclick="togglePinnedView(this, 1)">
                            <span class="resultsTab">Pinned documents</span>
                            <span class="ResultNumbers">(0)</span>
                        </div>
                        <div class="viewSwitcher"><span>…</span></div>
                    </div>
                </div>
                <div class="extraTabHolder">…</div>
                <div class="searchDocumentHeader">
                    <div class="switchTab hide">
                        <div class="navDiv" onclick="changeNavigationTabs(this);">Search Results</div>
                    </div>
                </div>
                <div class="EmailContentDiv">
                    <div id="Email">
                        <span id="EmailDiv">
                            <span id="EmailImg">
                                <img src="../Images/cart_icon_30px_X_30px.png" class="mailCartIcon" title="Review the contents of your mail cart and send the selected documents to your OneDrive or via email." alt="Mail Cart" />
                            </span>
                            <span id="EmailText">&nbsp;
                                <span id="EmailTextCount">(0)</span>
                            </span>
                        </span>
                        <div class="sdBanner">
                            <div class="sdBannerDropdown">
                                <div class="sdBannerLabel"><span>Sort by: </span></div>
                                <input class="sdBannerText" id="sdBannerDropDown" type="text" data-result="2" value="None" data-sort-value="" data-order="0" data-value="" data-ss-value="" disabled="disabled" />
                                <img alt="Sort By" class="sdBannerDD" title="Sort By" src="../Images/icon-combobox.png" />
                            </div>
                            <div id="sdBannerDropdownForDocuments" class="sdBannerPanel hide">
                                <div class="sdBannerPanelItem sdBannerPanelFirstItem" id="optionNoneForDocuments" data-sort-value="" data-order="0" data-value=""></div>
                                <div class="sdBannerPanelItem" id="optionFileNameASCForDocuments" data-order="0" data-value="DocumentName" data-ss-value="SearchName"></div>
                                <div class="sdBannerPanelItem" id="optionFileNameDESCForDocuments" data-order="1" data-value="DocumentName" data-ss-value="SearchName"></div>
                                <div class="sdBannerPanelItem" id="optionCreatedDateASCForDocuments" data-order="0" data-value="DocumentCreatedDate" data-ss-value="DateTime"></div>
                                <div class="sdBannerPanelItem" id="optionCreatedDateDESCForDocuments" data-order="1" data-value="DocumentCreatedDate" data-ss-value="DateTime"></div>
                                <div class="sdBannerPanelItem" id="optionModifiedDateASCForDocuments" data-order="0" data-value="DocumentModifiedDate" data-ss-value="ModifiedDate"></div>
                                <div class="sdBannerPanelItem sdBannerPanelLastItem" id="optionModifiedDateDESCForDocuments" data-order="1" data-value="DocumentModifiedDate" data-ss-value="ModifiedDate"></div>
                            </div>
                            <div id="sdBannerDropdownForMatters" class="sdBannerPanel hide">
                                <div class="sdBannerPanelItem sdBannerPanelFirstItem" id="optionNoneForMatters" data-sort-value="" data-order="0" data-value=""></div>
                                <div class="sdBannerPanelItem" id="optionFileNameASCForMatters" data-order="0" data-value="MatterName" data-ss-value="SearchName"></div>
                                <div class="sdBannerPanelItem" id="optionFileNameDESCForMatters" data-order="1" data-value="MatterName" data-ss-value="SearchName"></div>
                                <div class="sdBannerPanelItem" id="optionCreatedDateASCForMatters" data-order="0" data-value="MatterCreatedDate" data-ss-value="DateTime"></div>
                                <div class="sdBannerPanelItem sdBannerPanelLastItem" id="optionCreatedDateDESCForMatters" data-order="1" data-value="MatterCreatedDate" data-ss-value="DateTime"></div>
                                <div class="sdBannerPanelItem hide" id="optionModifiedDateASCForMatters" data-order="0" data-value="MatterModifiedDate" data-ss-value="ModifiedDate"></div>
                                <div class="sdBannerPanelItem hide" id="optionModifiedDateDESCForMatters" data-order="1" data-value="MatterModifiedDate" data-ss-value="ModifiedDate"></div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="detailSeparator"></div>
                <div class="Grid" id="grid">
                </div>
                <div class="Grid hide" id="pinnedGrid">
                </div>
                <div id="recentView">
                    <div id="RecentDocumentContainer" class=""></div>
                    <div id="recentMatters" class="hide">
                    </div>
                </div>
            </div>

        </div>
        </div>
</asp:Content>
<asp:Content ID="WebdashboardScriptContent" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
    <script src="../Scripts/AdvancedSearch.js?ver=25.0.0.0"></script>
    <script src="../Scripts/jsonGridWebDashboard.js?ver=25.0.0.0"></script>
    <script src="../Scripts/gridConfigWebDashboard.js?ver=25.0.0.0"></script>
    <script src="../Scripts/WebDashboard.js?ver=25.0.0.0"></script>
    <script src="../Scripts/popup.js?ver=25.0.0.0"></script>
    <script src="../Scripts/MailCart.js?ver=25.0.0.0"></script>
    <script type="text/javascript" src="/_layouts/15/sp.ui.dialog.js"></script>
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
