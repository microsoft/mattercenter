<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MatterCenterMaster.Master" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages.Settings" %>

<asp:Content ID="PageTitleContent" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="server">
    Settings
</asp:Content>

<asp:Content ID="SettingsPageHeadContent" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link href="../Styles/Settings.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="SettingsPageBodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="server">
    <div class="loadingImage">
        <img src="../Images/WindowsLoadingFast.GIF" alt="Loading" class="loadingImg" />
        <div class="loadingImageDiv">
        </div>
    </div>
    <div class="mainContainer hide">
        <div class="popupWait hide"></div>
        <div class="loadingImage hide" id="popupWaitLoadingImage">
            <a title="Matter Center Home" id="mcIcon" href="/sitepages/MatterCenterHome.aspx" target="_blank">
                <img title="Matter Center" src="../Images/WindowsLoadingFast.GIF" alt="Matter Center" />
            </a>
        </div>
        <div class="popupContainerBackground hide"></div>
        <div class="matterCenterHeaderBackground hide"></div>
        <!-- Client container -->
        <div id="clientContainer" class="hide">
            <div class="defaultFontStyle clientName" id="settingName"></div>
            <div class="defaultFontStyle contentDescription clientDescription"></div>
            <div class="tableContainer defaultFontStyle semiBold ClientNameTitle">
            </div>
            <div class="horizontalLine"></div>
            <div id="clientsList">
            </div>
        </div>
        <!--Container-->
        <div id="container" class="hide">
            <div class="errorPopUp hide">
                <div class="errTriangle popUpFloatLeft"></div>
                <div class="errTriangleBorder popUpFloatLeft"></div>
                <div class="errText popUpFloatRight"></div>
            </div>
            <div class="settingLink"><a class="clientLinks" href="#">Settings ></a></div>
            <div class="defaultFontStyle clientName"></div>
            <div class="defaultFontStyle pageDescription">
            </div>
            <!--Default Section-->
            <div class="defaultSection">
                <div class="defaultFontStyle titleSection"></div>
                <div class="defaultFontStyle descriptionSection"></div>
            </div>
            <!--End of Default Section-->
            <!--Matter Name-->
            <div class="tableContainer" id="matterNameSection">
                <div class="labelSection">
                    <div class="defaultFontStyle displayInline" id="matterName"></div>
                    <div class="defaultFontStyle contentDescription"></div>
                </div>
                <div class="ms-TextField defaultFontStyle userText valueSection defaultInputPosition">
                    <input class="ms-TextField-field inputMatterName"/>
                </div>
            </div>
            <!--End of Matter Name-->
            <!--Matter ID-->
            <div class="tableContainer" id="matterIdSection">
                <div class="labelSection">
                    <div class="defaultFontStyle displayInline" id="matterId"></div>
                    <div class="defaultFontStyle contentDescription"></div>
                </div>
                <div class="ms-TextField defaultFontStyle userText valueSection">
                    <input class="ms-TextField-field inputMatterId"/>
                </div>
            </div>
            <!--End of Matter ID-->
            <!--Matter Type-->
            <div class="defaultFontStyle tableContainer" id="defaultMatterTypeDescription"></div>
            <div class="contentMatterType">
                <div class="popUpHeader">
                    <div class="popUpHeaderTitle popUpFloatLeft defaultFontStyle semiBold" id="practiceGroupTitle"></div>
                    <div class="popUpPracticeGroup clear">
                        <div class="popUpPGDiv">
                            <input class="popUpPG practiceGroup defaultFontStyle" type="text" id="popUpPG" data-value="" data-foldernamespg="" disabled="disabled" autofocus="autofocus" />
                            <img class="ddpopUpPGIcon" title="Dropdown icon" src="../Images/icon-combobox.png" alt="Practice Group Icon" />
                        </div>
                        <div id="popUpPGList" class="hide">
                        </div>
                        <div class="loadingIconContentType">
                            <img src="../Images/loadingcirclests16.gif" alt="Loading" />
                        </div>
                    </div>
                </div>
                <div class="popUpBody popUpFloatLeft">
                    <div class="popUpMatterDescription popUpBodyBlocks popUpMarginRight popUpFloatLeft" id="divAOL">
                        <label class="popUpBodyHeader popUpFloatLeft  defaultFontStyle semiBold" id="areaOfLawTitle"></label>
                        <div class="popUpFloatLeft popUpBorder " id="popupAOLBorder">
                            <input type="text" class="popUpMDTextArea clear contentDescription searchByTyping" spellcheck="true" />
                            <div class="popUpCommonContent popUpMD" id="popupAOLContent">
                            </div>
                        </div>
                    </div>
                    <div class="popUpSubAreaOfLaw popUpBodyBlocks popUpMarginRight popUpFloatLeft" id="divSAOL">
                        <label class="popUpBodyHeader popUpFloatLeft  defaultFontStyle semiBold" id="subAreaOfLawTitle"></label>
                        <div class="popUpFloatLeft popUpBorder" id="popupSAOLBorder">
                            <input type="text" class="popUpMDTextArea clear contentDescription searchByTyping" spellcheck="true" />
                            <div class="popUpCommonContent popUpSAL" id="popupSAOLContent">
                            </div>
                        </div>
                    </div>
                    <div class="popUpMoveItem popUpBodyBlocks popUpMarginRight popUpFloatLeft" id="navigate">
                        <div class="popUpLeftClick popUpPosAbs">
                            <img alt="Left" class="iconForward iconPosition" src="../Images/forward_666.png" />
                        </div>
                        <div class="popUpRightClick popUpPosAbs">
                            <img alt="Right" class="iconBack iconPosition" src="../Images/back-666.png" />
                        </div>
                    </div>
                    <div class="popUpDocumentTemplates popUpBodyBlocks popUpFloatLeft matterTypePosition" id="divDocTemplate">
                        <label class="popUpBodyHeader popUpFloatLeft  defaultFontStyle semiBold" id="matterTypeTitle"></label>
                        <div class="popUpFloatLeft popUpBorder">
                            <div class="popUpCommonContent popDT">
                            </div>
                        </div>
                    </div>
                </div>
                <div class="defaultFontStyle matterTypeComplete" id="hideMatterTypeSection"></div>
            </div>
            <!--End of Matter Type-->
            <!--Hidden Section of Matter Type-->
            <div class="hideMatterType hide">
                <div class="defaultFontStyle" id="showMatterSection"></div>
            </div>
            <!--End of Hidden Matter Type-->
            <!--Restrict Access-->
            <div class="tableContainer" id="restrictAccessSection">
                <div class="labelSection">
                    <div class="defaultFontStyle displayInline" id="restrictAccessTitle"></div>
                    <div class="defaultFontStyle contentDescription">
                    </div>
                </div>

                <div class="ms-ChoiceFieldGroup displayInline valueSection defaultRadioPosition">
                    <div class="ms-ChoiceField ms-ChoiceFieldGroup displayInline">
                        <input id="assignTeamTrue" class="ms-ChoiceField-input" type="radio" name="restrictAccess" />
                        <label for="assignTeamTrue" class="ms-ChoiceField-field"><span class="ms-Label">Yes</span></label>
                    </div>
                    <div class="ms-ChoiceField ms-ChoiceFieldGroup displayInline radioSpace">
                        <input id="assignTeamFalse" class="ms-ChoiceField-input" type="radio" name="restrictAccess" />
                        <label for="assignTeamFalse" class="ms-ChoiceField-field"><span class="ms-Label">No</span></label>
                    </div>
                </div>
            </div>
            <!--End of Restrict Access-->
            <!--Assign Team-->
            <div class="content">
                <div class="sectionStep" id="snConflictCheck">
                    <div class="sectionContent">
                        <div class="defaultFontStyle tableContainer semiBold" id="assignPermissionsTitle"></div>
                        <div class="loadingIconAssignPermission">
                            <img src="../Images/loadingcirclests16.gif" alt="Loading" />
                        </div>
                        <label class="semiBold defaultFontStyle tableContainer defaultNamePosition" id="labelName"></label>
                        <div class="gutter"></div>
                        <label class="semiBold defaultFontStyle tableContainer" id="labelRole"></label>
                        <div class="gutter"></div>
                        <label class="semiBold defaultFontStyle tableContainer defaultPermissionPosition" id="labelPermissionLevel"></label>
                        <div class="row1 assignNewPerm">
                            <input type="text" class="clear inputAssignPerm teamName valueSection" id="txtAssign1" spellcheck="true" placeholder="Enter and select names" required="required" />
                            <div class="gutter"></div>
                            <div>
                                <div class="mediumSmallRole  floatContentLeft ddlRoleAssignDiv">
                                    <input class="assignRole assignRoleField teamRole valueSection" type="text" id="ddlRoleAssign1" data-value="" data-mandatory="" disabled="disabled" autofocus="autofocus" />
                                    <img class="ddlRoleAssignIcon" id="ddlRoleAssignIcon1" title="Dropdown icon" src="../Images/icon-combobox.png" alt="Role Icon" />
                                </div>
                                <div class="ddlRoleAssignList clear hide" id="ddlRoleAssignList1">
                                </div>
                            </div>
                            <div class="gutter"></div>
                            <div>
                                <div class="mediumSmallPermission  floatContentLeft ddlPermAssignDiv1">
                                    <input class="assignPermission assignPermField teamPermission Space valueSection" type="text" id="ddlPermAssign1" data-value="" disabled="disabled" autofocus="autofocus" />
                                    <img class="ddlPermAssignIcon" id="ddlPermAssignIcon1" title="Dropdown icon" src="../Images/icon-combobox.png" alt="Permission Icon" />
                                </div>
                                <div class="ddlPermAssignList clear hide" id="ddlPermAssignList1">
                                </div>
                            </div>
                            <div class="gutter"></div>
                            <div class="close">
                                <img alt="delete" src="../Images/icon-delete.png" />
                            </div>
                        </div>
                        <div id="addNewAssignPerm" class="clear"></div>
                        <a id="addMoreLink" class="defaultFontStyle valueSection"></a>
                    </div>
                </div>
            </div>
            <!--End of Assign Team-->
            <!--Include Calendar Section-->
            <div class="tableContainer" id="calendarSection">
                <div class="defaultFontStyle displayInline labelSection"></div>
                <div class="ms-ChoiceFieldGroup displayInline valueSection defaultRadioPosition">
                    <div class="ms-ChoiceField ms-ChoiceFieldGroup displayInline">
                        <input id="includeCalendarTrue" class="ms-ChoiceField-input" type="radio" name="includeCalendar" />
                        <label for="includeCalendarTrue" class="ms-ChoiceField-field "><span class="ms-Label">Yes</span></label>
                    </div>
                    <div class="ms-ChoiceField ms-ChoiceFieldGroup displayInline radioSpace">
                        <input id="includeCalendarFalse" class="ms-ChoiceField-input" type="radio" name="includeCalendar" />
                        <label for="includeCalendarFalse" class="ms-ChoiceField-field"><span class="ms-Label">No</span></label>
                    </div>
                </div>
            </div>
            <!--End of Include Calendar Section-->
            <!--RSS Feeds Section-->
            <div class="tableContainer" id="rssSection">
                <div class="defaultFontStyle displayInline labelSection"></div>
                <div class="ms-ChoiceFieldGroup displayInline valueSection defaultRadioPosition">
                    <div class="ms-ChoiceField ms-ChoiceFieldGroup displayInline">
                        <input id="includeRSSTrue" class="ms-ChoiceField-input" type="radio" name="includeRSSFeeds" />
                        <label for="includeRSSTrue" class="ms-ChoiceField-field"><span class="ms-Label">Yes</span></label>
                    </div>
                    <div class="ms-ChoiceField ms-ChoiceFieldGroup displayInline radioSpace">
                        <input id="includeRSSFalse" class="ms-ChoiceField-input" type="radio" name="includeRSSFeeds" />
                        <label for="includeRSSFalse" class="ms-ChoiceField-field"><span class="ms-Label">No</span></label>
                    </div>
                </div>
            </div>
            <!--RSS Feeds Section-->
            <!--Email Notification Section-->
            <div class="tableContainer" id="emailNotificationSection">
                <div class="defaultFontStyle displayInline labelSection"></div>
                <div class="ms-ChoiceFieldGroup displayInline valueSection defaultRadioPosition">
                    <div class="ms-ChoiceField ms-ChoiceFieldGroup displayInline">
                        <input id="includeEmailTrue" class="ms-ChoiceField-input" type="radio" name="includeEmail" />
                        <label for="includeEmailTrue" class="ms-ChoiceField-field"><span class="ms-Label">Yes</span></label>
                    </div>
                    <div class="ms-ChoiceField ms-ChoiceFieldGroup displayInline radioSpace">
                        <input id="includeEmailFalse" class="ms-ChoiceField-input" type="radio" name="includeEmail" />
                        <label for="includeEmailFalse" class="ms-ChoiceField-field"><span class="ms-Label">No</span></label>
                    </div>
                </div>
            </div>
            <!--End of Email Notification Section-->
            <!--Tasks Section-->
            <div class="tableContainer" id="tasksSection">
                <div class="defaultFontStyle displayInline labelSection"></div>
                <div class="ms-ChoiceFieldGroup displayInline valueSection defaultRadioPosition">
                    <div class="ms-ChoiceField ms-ChoiceFieldGroup displayInline">
                        <input id="includeTasksTrue" class="ms-ChoiceField-input" type="radio" name="includeTasks" />
                        <label for="includeTasksTrue" class="ms-ChoiceField-field"><span class="ms-Label">Yes</span></label>
                    </div>
                    <div class="ms-ChoiceField ms-ChoiceFieldGroup displayInline radioSpace">
                        <input id="includeTasksFalse" class="ms-ChoiceField-input" type="radio" name="includeTasks" />
                        <label for="includeTasksFalse" class="ms-ChoiceField-field"><span class="ms-Label">No</span></label>
                    </div>
                </div>
            </div>
            <!--End of Tasks Section-->
            <!--Default Matter Option-->
            <div class="defaultSection tableContainer defaultMatterSection" id="matterOptionsSection">
                <div id="matterOptionsTitle" class="defaultFontStyle titleSection"></div>
                <div id="matterOptionsDescription" class="defaultFontStyle descriptionSection"></div>
            </div>
            <!--End of Default Matter Option-->
            <!--Matter Description Required Section-->
            <div class="tableContainer" id="matterDescriptionSection">
                <div class="labelSection">
                    <div class="defaultFontStyle displayInline" id="matterDescriptionTitle"></div>
                    <div class="defaultFontStyle contentDescription"></div>
                </div>
                <div class="ms-ChoiceFieldGroup displayInline valueSection defaultRadioPosition">
                    <div class="ms-ChoiceField ms-ChoiceFieldGroup displayInline">
                        <input id="matterRequiredTrue" class="ms-ChoiceField-input" type="radio" name="matterRequired" />
                        <label for="matterRequiredTrue" class="ms-ChoiceField-field"><span class="ms-Label">Yes</span></label>
                    </div>
                    <div class="ms-ChoiceField ms-ChoiceFieldGroup displayInline radioSpace">
                        <input id="matterRequiredFalse" class="ms-ChoiceField-input" type="radio" name="matterRequired" />
                        <label for="matterRequiredFalse" class="ms-ChoiceField-field"><span class="ms-Label">No</span></label>
                    </div>
                </div>
            </div>
            <!--End of Matter Description Required Section-->
            <!--Matter Conflict Check Section-->
            <div class="tableContainer" id="conflictCheckSection">
                <div class="labelSection">
                    <div class="defaultFontStyle displayInline" id="conflictCheckTitle"></div>
                    <div class="defaultFontStyle contentDescription"></div>
                </div>
                <div class="ms-ChoiceFieldGroup valueSection">
                    <div class="ms-ChoiceField ms-ChoiceFieldGroup displayInline conflictCheckRadioPosition">
                        <input id="matterConflictTrue" class="ms-ChoiceField-input" type="radio" name="matterConflict" />
                        <label for="matterConflictTrue" class="ms-ChoiceField-field"><span class="ms-Label">Yes</span></label>
                    </div>
                    <div class="ms-ChoiceField ms-ChoiceFieldGroup displayInline radioSpace conflictCheckRadioPosition">
                        <input id="matterConflictFalse" class="ms-ChoiceField-input" type="radio" name="matterConflict" />
                        <label for="matterConflictFalse" class="ms-ChoiceField-field"><span class="ms-Label">No</span></label>
                    </div>
                </div>
            </div>
            <!--End of Matter Conflict Check Section-->
            <!--Button Section-->
            <div id="buttonSection">
                <button class="button clear" type="button" id="saveButton" disabled="disabled"></button>
                <button class="button" type="button" id="cancelButton"></button>
            </div>
            <!--End of Button Section-->
            <div class="defaultFontStyle tableContainer hide" id="successMessage"></div>
        </div>
        <!--End of Container-->
    </div>
    <!--End of Main Container-->
</asp:Content>

<asp:Content ID="SettingsPageScriptContent" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
    <script src="../Scripts/Settings.js"></script>
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
