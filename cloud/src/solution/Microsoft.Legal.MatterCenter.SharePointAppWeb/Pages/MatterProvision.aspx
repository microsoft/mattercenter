<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="../MasterPage/MatterCenterMaster.Master" CodeBehind="MatterProvision.aspx.cs" Inherits="Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages.MatterProvision" %>

<asp:Content ID="PageTitleContent" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="server">
    Provision Matter
</asp:Content>
<asp:Content ID="MatterProvisionHeadContent" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="../Styles/popupStyles.css?ver=25.0.0.0" />
    <link rel="stylesheet" type="text/css" href="../Styles/MatterProvision.css?ver=25.0.0.0" />
    <link rel="stylesheet" type="text/css" href="../Styles/CommonControls.css?ver=25.0.0.0" />
</asp:Content>
<asp:Content ID="MatterProvisionBodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="server">
    <div class="container">
        <div class="popupWait hide"></div>
        <div class="mainArea">
            <div class="notificationContainer"></div>
            <div class="errorPopUp hide">
                <div class="errTriangle popUpFloatLeft"></div>
                <div class="errTriangleBorder popUpFloatLeft"></div>
                <div class="errText popUpFloatRight"></div>
            </div>
            <!-- Top menu area -->
            <div class="menu">
                <div class="menuTextGroup">
                    <div><span title="Step 1: Select a client and provide basic information for your new matter." class="menuText menuTextSelected" data-section-name="snOpenMatter" data-section-num="1">1 Open Matter</span></div>
                    <div><span title="Step 2: Confirm that a conflict check was conducted and specify who should be conflicted from or given access to this matter." class="menuText" data-section-name="snConflictCheck" data-section-num="2">2 Assign Permissions</span></div>
                    <div><span title="Step 3: When you've entered all relevant information for your new matter, create the matter and notify your team." class="menuText" data-section-name="snCreateAndShare" data-section-num="3">3 Create and Notify</span></div>
                </div>
            </div>
            <!-- Center content area -->
            <div class="content">
                <!-- Open matter section -->
                <section class="sectionStep" id="snOpenMatter">
                    <div class="sectionContent">
                        <label class="contentLabel medium" id="selectClient"><span class="mandatory">*&nbsp;</span>Client Name</label>
                        <div class="gutter"></div>
                        <label class="contentLabel small" id="clientId">Client ID</label>
                        <div class="medium clear ddlClientDiv">
                            <input class="medium clear" type="text" id="ddlClient" data-value="" data-client-id="" disabled="disabled" autofocus="autofocus" />
                            <img class="ddClientIcon" title="Dropdown icon" src="../Images/icon-combobox.png" alt="Client Icon" />
                        </div>
                        <div id="ddlClientList" class="medium clear hide">
                        </div>
                        <div class="gutter"></div>
                        <input class="small" id="txtClientId" placeholder="" type="text" disabled="disabled" title="Client ID" />
                        <label class="contentLabel medium clear" id="enterMatterName"><span class="mandatory">*&nbsp;</span>Matter Name</label>
                        <div class="gutter"></div>
                        <label class="contentLabel small" id="enterMatterId"><span class="mandatory">*&nbsp;</span>Matter ID</label>
                        <input class="medium clear" id="txtMatterName" type="text" value="" placeholder="Enter a name for this matter" title="Matter Name" />
                        <div class="gutter"></div>
                        <input class="small" id="txtMatterId" type="text" value="" placeholder="Enter a matter ID" title="Matter ID" required />
                        <label class="contentLabel large clear"><span class="mandatory description">*&nbsp;</span>Matter Description</label>
                        <textarea class="large clear" id="txtMatterDesc" placeholder="Enter a brief description of the matter" rows="2" cols="400" spellcheck="true" ></textarea>
                        <label class="contentLabel large clear"><span class="mandatory">*&nbsp;</span>Matter Type (click Select to define the practice group and area of law)</label>
                        <div class="large" id="documentTemplates"></div>
                        <div class="gutter"></div>
                        <button class="button" type="button" id="btnLookup" value="Lookup" disabled="disabled">Select Type</button>
                        <div class="loadingLookUp">
                            <img id="loadingLookupImage" src="../Images/WindowsLoadingFast.GIF" alt="Loading..." />
                        </div>
                    </div>
                </section>
                <!-- Conflict check section -->
                <section class="sectionStep hide" id="snConflictCheck">
                    <div class="sectionContent">
                        <div id="conflictCheck">
                            <label class="medium chkConflictCheckParent conflictCheckPosition" id="chkConflictCheckParent">
                                <input type="checkbox" id="chkConflictCheck" checked="checked" value="Yes" />Conflict check conducted (must be complete prior to provisioning matter)</label>
                            <label class="contentLabel medium clear" id="conflictCheckBy"><span class="mandatory">*&nbsp;</span>Who conducted the conflict check?</label>
                            <div class="gutter"></div>
                            <label class="contentLabel mediumSmall" id="checkedPerformedOn"><span class="mandatory">*&nbsp;</span>Date Conducted</label>
                            <div class="gutter"></div>
                            <label class="contentLabel mediumSmall" id="conflictIdentified">Conflict Identified?</label>
                            <input type="text" class="medium clear" id="txtConflictCheckBy" spellcheck="true" placeholder="Enter and select name" />
                            <div class="gutter"></div>
                            <input class="mediumSmall" id="txtConflictCheckDate" type="text" value="" placeholder="mm/dd/yyyy" pattern="([1-9]|0[1-9]|1[012])[-/.]([1-9]|0[1-9]|[12][0-9]|3[01])[-/.]\d{4}" required />
                            <div class="gutter"></div>
                            <div class="conflictRadioBtn">
                                <label class="smaller">
                                    <input type="radio" name="rdbConflictCheck" checked="checked" value="True" />Yes</label>
                                <label class="smaller">
                                    <input type="radio" name="rdbConflictCheck" value="False" />No</label>
                            </div>
                            <div class="clear"></div>
                            <section id="snBlockUser">
                                <label class="contentLabel medium clear"><span class="mandatory">*&nbsp;</span>Conflicted Users</label>
                                <input type="text" class="medium clear" id="txtBlockUser" spellcheck="true" placeholder="Enter and select names" />
                                <div class="gutter"></div>
                                <div class="large clear" id="areaBlockUser"></div>
                            </section>
                        </div>
                        <label class="secureMatterLabel assignTeamRadioPosition">Would you like this matter to be secured only to the assigned team?</label>
                        <div class="secureMatterRadioBtn assignTeamRadioPosition">
                            <label class="rdbOptions">
                                <input type="radio" class="matterCheckYes" name="rdbSecureMatterCheck" value="True" />Yes</label>
                            <label class="rdbOptions">
                                <input type="radio" class="matterCheckNo" name="rdbSecureMatterCheck" value="False" />No</label>
                        </div>
                        <label class="contentLabel medium clear blue" id="assignTeam">Assign Team</label>
                        <label class="medium clear contentLabel" id="labelName"><span class="mandatory">*&nbsp;</span>Name</label>
                        <div class="gutter"></div>
                        <label class="mediumSmallRole contentLabel"><span class="mandatory">*&nbsp;</span>Role</label>
                        <div class="gutter"></div>
                        <label class="mediumSmallPermission contentLabel"><span class="mandatory">*&nbsp;</span>Permission Level</label>
                        <div class="row1 assignNewPerm">
                            <input type="text" class="medium clear inputAssignPerm" id="txtAssign1" spellcheck="true" placeholder="Enter and select names" />
                            <div class="gutter"></div>
                            <div class="addNewMember">
                                <div class="mediumSmallRole  floatContentLeft ddlRoleAssignDiv">
                                    <input class="mediumSmallRole assignRole assignRoleField" type="text" id="ddlRoleAssign1" data-value="" data-mandatory="" disabled="disabled" autofocus="autofocus" />
                                    <img class="ddlRoleAssignIcon" id="ddlRoleAssignIcon1" title="Dropdown icon" src="../Images/icon-combobox.png" alt="Role Icon" />
                                </div>
                                <div class="ddlRoleAssignList clear hide" id="ddlRoleAssignList1">
                                </div>
                            </div>
                            <div class="gutter"></div>
                            <div class="">
                                <div class="mediumSmallPermission  floatContentLeft ddlPermAssignDiv1">
                                    <input class="mediumSmallPermission assignPermission assignPermField" type="text" id="ddlPermAssign1" data-value="" disabled="disabled" autofocus="autofocus" />
                                    <img class="ddlPermAssignIcon" id="ddlPermAssignIcon1" title="Dropdown icon" src="../Images/icon-combobox.png" alt="Permission Icon" />
                                </div>
                                <div class="ddlPermAssignList clear hide" id="ddlPermAssignList1">
                                </div>
                            </div>
                            <div class="gutter"></div>
                            <div class="close">
                                <img alt="Delete" src="../Images/icon-delete.png" title="Delete" />
                            </div>
                        </div>
                        <div id="addNewAssignPerm" class="clear"></div>
                        <a id="addMoreLink">Add More Names</a>
                    </div>
                </section>
                <!-- Create and share matter section -->
                <section class="sectionStep hide" id="snCreateAndShare">
                    <div class="sectionContent">
                        <label class="clear createNotifyDescription">You have entered all of the necessary information to create this matter. After you click Create and Notify, the matter will be created and all team members will receive an email containing a link to this matter’s location.</label>
                        <div class="ms-ChoiceField calendarPosition">
                            <input id="demo-checkbox-unselected0" class="ms-ChoiceField-input" type="checkbox" />
                            <label for="demo-checkbox-unselected0" class="ms-ChoiceField-field"><span id="checkbox-label-text0" class="ms-Label checkbox">Include calendar</span></label>
                        </div>
                        <div class="ms-ChoiceField clear rssFeedsPosition">
                            <input id="demo-checkbox-unselected1" class="ms-ChoiceField-input" type="checkbox" />
                            <label for="demo-checkbox-unselected1" class="ms-ChoiceField-field"><span id="checkbox-label-text1" class="ms-Label checkbox">Include RSS Feeds</span></label>
                        </div>
                        <div class="ms-ChoiceField emailPosition">
                            <input id="demo-checkbox-unselected2" class="ms-ChoiceField-input" type="checkbox" />
                            <label for="demo-checkbox-unselected2" class="ms-ChoiceField-field"><span id="checkbox-label-text2" class="ms-Label checkbox">Include email notification</span></label>
                        </div>
                        <div class="ms-ChoiceField taskPosition">
                            <input id="demo-checkbox-unselected3" class="ms-ChoiceField-input" type="checkbox" />
                            <label for="demo-checkbox-unselected3" class="ms-ChoiceField-field"><span id="checkbox-label-text3" class="ms-Label checkbox">Include tasks</span></label>
                        </div>
                        <button class="button clear" type="button" id="btnCreateMatter" value="Create">Create</button>
                    </div>
                    <div class="loading">
                        <img id="loadingImage" src="../Images/WindowsLoadingFast.GIF" alt="Loading..." />
                    </div>
                </section>
                <div id="space"></div>
            </div>
        </div>
    </div>
    <div class="pagination">
        <div class="nextPreviousDiv">
            <span data-move="0" class="Paginate">Previous</span>
            <span data-move="1" class="Paginate active">Next</span>
        </div>
    </div>
    <div class="popupContainerBackground hide"></div>
    <div class="popUpContainer hide">
        <img class="popUpCloseIcon popUpFloatRight" alt="Close" src="../Images/close-666.png" title="Close" />
        <div class="alignCenter">
            <header class="popUpHeader">
                <div class="popUpHeaderTitle popUpFloatLeft" title="Select a Practice Group">Select a Practice Group</div>
                <div class="popUpPracticeGroup clear">
                    <div class="popUpPGDiv">
                        <input class="popUpPG" type="text" id="popUpPG" data-value="" data-foldernamespg="" disabled="disabled" autofocus="autofocus" />
                        <img class="ddpopUpPGIcon" title="Dropdown icon" src="../Images/icon-combobox.png" alt="Practice Group Icon" />
                    </div>
                    <div id="popUpPGList" class="hide">
                    </div>
                </div>
            </header>
            <div class="popUpBody popUpFloatLeft">
                <div class="popUpMatterDescription popUpBodyBlocks popUpMarginRight popUpFloatLeft" id="divAOL">
                    <label class="popUpBodyHeader popUpFloatLeft" title="Area of Law">Area of Law</label>
                    <div class="popUpFloatLeft popUpBorder " id="popupAOLBorder">
                        <input type="text" class="popUpMDTextArea clear" spellcheck="true" placeholder="Search by typing" />
                        <div class="popUpCommonContent popUpMD" id="popupAOLContent">
                        </div>
                    </div>
                </div>
                <div class="popUpSubAreaOfLaw popUpBodyBlocks popUpMarginRight popUpFloatLeft" id="divSAOL">
                    <label class="popUpBodyHeader popUpFloatLeft" title="Sub Area of Law (select templates)">Sub Area of Law (select matter type)</label>
                    <div class="popUpFloatLeft popUpBorder" id="popupSAOLBorder">
                        <input type="text" class="popUpMDTextArea clear" spellcheck="true" placeholder="Search by typing" />
                        <div class="popUpCommonContent popUpSAL" id="popupSAOLContent">
                        </div>
                    </div>
                </div>
                <div class="popUpMoveItem popUpBodyBlocks popUpMarginRight popUpFloatLeft" id="navigate">
                    <div class="popUpLeftClick popUpPosAbs">
                        <img alt="Left" class="iconForward" src="../Images/forward_666.png" title="Add" />
                    </div>
                    <div class="popUpRightClick popUpPosAbs">
                        <img alt="Right" class="iconBack" src="../Images/back-666.png" title="Remove" />
                    </div>
                </div>
                <div class="popUpDocumentTemplates popUpBodyBlocks popUpFloatLeft" id="divDocTemplate">
                    <div class="errorPopUpArea hide">
                        <div class="errTriangleArea popUpFloatLeft"></div>
                        <div class="errTriangleBorderArea popUpFloatLeft"></div>
                        <div class="errTextArea popUpFloatRight"><span class="selectDefaultContentTypeMessage">Please select primary matter type</span></div>
                    </div>

                    <label class="popUpBodyHeader popUpFloatLeft" title="Matter Type">Matter Type</label>
                    <div class="popUpFloatLeft popUpBorder">
                        <div class="popUpCommonContent popDT">
                        </div>
                    </div>
                </div>
                <div id="defaultMessageDiv">
                    <img id="infoImg" alt="info" src="../Images/information-blue.png" />
                    <span id="defaultMessage">Please select primary matter type before you save.</span>
                </div>
                <button type="button" class="SavePopUpEntries">Save</button>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="MatterProvisionScriptContent" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
    <script type="text/javascript" src="../Scripts/MatterProvision.js?ver=25.0.0.0"></script>
    <script type="text/javascript" src="../Scripts/popup.js?ver=25.0.0.0"></script>
</asp:Content>
