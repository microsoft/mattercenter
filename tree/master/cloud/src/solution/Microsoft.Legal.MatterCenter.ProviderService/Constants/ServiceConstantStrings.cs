// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-nikhid
// Created          : 11-28-2014
//
// ***********************************************************************
// <copyright file="ServiceConstantStrings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file is being used as an adapter between constant resource and service.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using System;
    using System.Globalization;
    #endregion

    /// <summary>
    /// Provides constants used in Matter Center.
    /// </summary>
    internal static class ServiceConstantStrings
    {
        /// <summary>
        /// Accessing from resource file for service
        /// </summary>
        private static string practiceGroupTermSetName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Practice_Group_Term_Set_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The client term set name
        /// </summary>
        private static string clientTermSetName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Client_Term_Set_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The log table name
        /// </summary>
        private static string logTableName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "LogTableName", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The DMS role list name
        /// </summary>
        private static string dmsRoleListName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "DMS_Role_List_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The DMS role query
        /// </summary>
        private static string dmsRoleQuery = ConstantStrings.GetConfigurationFromResourceFile("Constants", "DMS_Role_Query", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The column name unique identifier
        /// </summary>
        private static string columnNameGuid = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Column_Name_Guid", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The role list column role name
        /// </summary>
        private static string roleListColumnRoleName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Role_List_Column_Role_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The role list column is role mandatory
        /// </summary>
        private static string roleListColumnIsRoleMandatory = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Role_List_Column_Is_Role_Mandatory", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The column name modified date
        /// </summary>
        private static string columnNameModifiedDate = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Column_Name_Modified", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The central repository URL
        /// </summary>
        private static string centralRepositoryUrl = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Central_Repository_Url", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The DMS matter list name
        /// </summary>
        private static string dmsMatterListName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "DMS_Matter_List_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The share list column matter path
        /// </summary>
        private static string shareListColumnMatterPath = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Share_List_Column_Matter_Path", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The share list column mail list
        /// </summary>
        private static string shareListColumnMailList = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Share_List_Column_Mail_List", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The stamped property practice group
        /// </summary>
        private static string stampedPropertyPracticeGroup = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Practice_Group", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The stamped property area of law
        /// </summary>
        private static string stampedPropertyAreaOfLaw = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Area_Of_Law", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The stamped property sub area of law
        /// </summary>
        private static string stampedPropertySubAreaOfLaw = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Sub_Area_Of_Law", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The stamped property matter name
        /// </summary>
        private static string stampedPropertyMatterName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Matter_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The stamped property matter identifier
        /// </summary>
        private static string stampedPropertyMatterID = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Matter_ID", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The stamped property client name
        /// </summary>
        private static string stampedPropertyClientName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Client_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The stamped property client identifier
        /// </summary>
        private static string stampedPropertyClientID = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Client_ID", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The stamped property responsible attorney
        /// </summary>
        private static string stampedPropertyResponsibleAttorney = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Responsible_Attorney", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The stamped property responsible attorney
        /// </summary>
        private static string stampedPropertyResponsibleAttorneyEmail = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Responsible_Attorney_Email", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The stamped property team members
        /// </summary>
        private static string stampedPropertyTeamMembers = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Team_Members", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The stamped property is matter
        /// </summary>
        private static string stampedPropertyIsMatter = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Is_Matter", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The stamped property open date
        /// </summary>
        private static string stampedPropertyOpenDate = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Open_Date", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The stamped property secure matter
        /// </summary>
        private static string stampedPropertySecureMatter = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Secure_Matter", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Stamped Property Success
        /// </summary>
        private static string stampedPropertySuccess = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Success", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The content type column client identifier
        /// </summary>
        private static string contentTypeColumnClientId = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Content_Type_Column_Client_Id", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The content type column client name
        /// </summary>
        private static string contentTypeColumnClientName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Content_Type_Column_Client_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The content type column matter identifier
        /// </summary>
        private static string contentTypeColumnMatterId = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Content_Type_Column_Matter_Id", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The content type column matter name
        /// </summary>
        private static string contentTypeColumnMatterName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Content_Type_Column_Matter_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The content type column practice group
        /// </summary>
        private static string contentTypeColumnPracticeGroup = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Content_Type_Column_Practice_Group", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The content type column area of law
        /// </summary>
        private static string contentTypeColumnAreaOfLaw = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Content_Type_Column_Area_Of_Law", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The content type column sub area of law
        /// </summary>
        private static string contentTypeColumnSubareaOfLaw = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Content_Type_Column_Subarea_Of_Law", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The matters list column title
        /// </summary>
        private static string mattersListColumnTitle = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matters_List_Column_Title", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The matters list column client name
        /// </summary>
        private static string mattersListColumnClientName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matters_List_Column_Client_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The matters list column client identifier
        /// </summary>
        private static string mattersListColumnClientID = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matters_List_Column_Client_ID", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The matters list column matter name
        /// </summary>
        private static string mattersListColumnMatterName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matters_List_Column_Matter_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The matters list column matter identifier
        /// </summary>
        private static string mattersListColumnMatterID = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matters_List_Column_Matter_ID", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The matters list column conflict check by
        /// </summary>
        private static string mattersListColumnConflictCheckBy = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matters_List_Column_Conflict_Check_By", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The matters list column conflict check on
        /// </summary>
        private static string mattersListColumnConflictCheckOn = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matters_List_Column_Conflict_Check_On", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The matters list column conflict identified
        /// </summary>
        private static string mattersListColumnConflictIdentified = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matters_List_Column_Conflict_Identified", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The matters list column block users
        /// </summary>
        private static string mattersListColumnBlockUsers = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matters_List_Column_Block_Users", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The matters list column managing attorney
        /// </summary>
        private static string mattersListColumnManagingAttorney = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matters_List_Column_Managing_Attorney", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The matters list column support
        /// </summary>
        private static string mattersListColumnSupport = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matters_List_Column_Support", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The all folders query
        /// </summary>
        private static string allFoldersQuery = ConstantStrings.GetConfigurationFromResourceFile("Constants", "All_Folders_Query", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The user pinned matter list name
        /// </summary>
        private static string userPinnedMatterListName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "User_Pinned_Matter_List_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The pinned list column matter details
        /// </summary>
        private static string pinnedListColumnMatterDetails = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Pinned_List_Column_Matter_Details", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The user pinned details query
        /// </summary>
        private static string userPinnedDetailsQuery = ConstantStrings.GetConfigurationFromResourceFile("Constants", "User_Pinned_Details_Query", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The pinned list column user alias
        /// </summary>
        private static string pinnedListColumnUserAlias = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Pinned_List_Column_User_Alias", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The user pinned document list name
        /// </summary>
        private static string userPinnedDocumentListName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "User_Pinned_Document_List_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The pinned list column document details
        /// </summary>
        private static string pinnedListColumnDocumentDetails = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Pinned_List_Column_Document_Details", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The column name file leaf reference
        /// </summary>
        private static string columnNameFileLeafRef = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Column_Name_File_Leaf_Ref", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The column name file reference
        /// </summary>
        private static string columnNameFileRef = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Column_Name_File_Ref", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The column name file DIR reference
        /// </summary>
        private static string columnNameFileDirRef = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Column_Name_File_Dir_Ref", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The property name VTI indexed property keys
        /// </summary>
        private static string propertyNameVtiIndexedPropertyKeys = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Property_Name_Vti_Indexed_Property_Keys", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The sub area custom property folder names
        /// </summary>
        private static string subAreaCustomPropertyFolderNames = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Sub_Area_CustomProperty_FolderNames", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The sub area custom property is no folder structure present
        /// </summary>
        private static string subAreaCustomPropertyisNoFolderStructurePresent = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Sub_Area_CustomProperty_isNoFolderStructurePresent", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The area custom property folder names
        /// </summary>
        private static string areaCustomPropertyFolderNames = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Area_CustomProperty_FolderNames", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The practice group custom property folder names
        /// </summary>
        private static string practiceGroupCustomPropertyFolderNames = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Practice_Group_CustomProperty_FolderNames", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The sub area of law document templates
        /// </summary>
        private static string subAreaOfLawDocumentTemplates = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Sub_Area_Of_Law_Document_Templates", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The hidden content type
        /// </summary>
        private static string hiddenContentType = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Hidden_Content_Type", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The stamped property blocked upload users
        /// </summary>
        private static string stampedPropertyBlockedUploadUsers = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Blocked_Upload_Users", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The find document valid extensions
        /// </summary>
        private static string findDocumentInvalidExtensions = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Find_Document_Invalid_Extensions", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The valid date format
        /// </summary>
        private static string validDateFormat = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Valid_Date_Format", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Suffix to be appended in calendar name
        /// </summary>
        private static string calendarNameSuffix = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Calendar_Name_Suffix", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Title for Lists Path
        /// </summary>
        private static string titleListsPath = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Title_Lists_Path", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Suffix to be appended in task list name
        /// </summary>
        private static string taskNameSuffix = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Task_Name_Suffix", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Matter name length
        /// </summary>
        private static string matterNameLength = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Name_Length", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Matter Id length
        /// </summary>
        private static string matterIdLength = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Id_Length", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Matter description length
        /// </summary>
        private static string matterDescriptionLength = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Description_Length", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Content Type length
        /// </summary>
        private static string contentTypeLength = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Content_Type_Length", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The Matter Center help section list name 
        /// </summary>
        private static string matterCenterHelpSectionListName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Center_Help_Section_List_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The Matter Center help links list name 
        /// </summary>
        private static string matterCenterHelpLinksListName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Center_Help_Links_List_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Name of the column storing Link Title
        /// </summary>
        private static string contextualHelpLinksColumnLinkTitle = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Contextual_Help_Links_Column_LinkTitle", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Name of the column storing Link URL 
        /// </summary>
        private static string contextualHelpLinksColumnLinkURL = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Contextual_Help_Links_Column_LinkURL", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Name of the column storing Link Order
        /// </summary>
        private static string contextualHelpLinksColumnLinkOrder = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Contextual_Help_Links_Column_LinkOrder", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Name of the column storing Section ID
        /// </summary>
        private static string contextualHelpLinksColumnSectionID = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Contextual_Help_Links_Column_SectionID", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Name of the column storing Section ID
        /// </summary>
        private static string contextualHelpSectionColumnSectionID = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Contextual_Help_Section_Column_SectionID", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Name of the column storing Section Title
        /// </summary>
        private static string contextualHelpSectionColumnSectionTitle = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Contextual_Help_Section_Column_SectionTitle", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Name of the column storing Page Name
        /// </summary>
        private static string contextualHelpSectionColumnPageName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Contextual_Help_Section_Column_PageName", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Name of the column storing Section Order
        /// </summary>
        private static string contextualHelpSectionColumnSectionOrder = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Contextual_Help_Section_Column_SectionOrder", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Name of the column storing Number of Columns
        /// </summary>
        private static string contextualHelpSectionColumnNumberOfColumns = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Contextual_Help_Section_Column_NumberOfColumns", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Query to be used for retrieving contextual help sections
        /// </summary>
        private static string retrieveContextualHelpSectionsQuery = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Retrieve_Contextual_Help_Sections_Query", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Query to be used for retrieving contextual help links 
        /// </summary>
        private static string retrieveContextualHelpLinksQuery = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Retrieve_Contextual_Help_Links_Query", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// ';' separated list of page names to be used in contextual help functionality 
        /// </summary>
        private static string matterCenterPages = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Center_Pages", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// ';' separated list to build OR part of CAMLquery to support include sections (link)
        /// </summary>
        private static string contextualHelpQueryIncludeOrCondition = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Contextual_Help_Query_Include_Or_Condition", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The mail SOAP request
        /// </summary>
        private static string mailSoapRequest =
                    @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                    xmlns:m=""http://schemas.microsoft.com/exchange/services/2006/messages""
                    xmlns:t=""http://schemas.microsoft.com/exchange/services/2006/types"" 
                    xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                    <soap:Header>
                    <t:RequestServerVersion Version=""Exchange2013""/>
                    </soap:Header>
                    <soap:Body>
                    <m:GetItem>
                    <m:ItemShape>
                        <t:BaseShape>Default</t:BaseShape>
                        <t:IncludeMimeContent>true</t:IncludeMimeContent>
                        <t:AdditionalProperties>
		                    <t:FieldURI FieldURI=""item:ConversationId""/>		          
			                <t:FieldURI FieldURI=""message:ConversationTopic""/>			  
        		        </t:AdditionalProperties>
                    </m:ItemShape>
                    <m:ItemIds>
                    <t:ItemId Id=""{0}""/>
                    </m:ItemIds>
                    </m:GetItem>
                    </soap:Body>
                    </soap:Envelope>";

        /// <summary>
        /// The attachment SOAP request
        /// </summary>
        private static string attachmentSoapRequest =
                     @"<?xml version=""1.0"" encoding=""utf-8""?>
                     <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                     xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                     xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""
                     xmlns:t=""http://schemas.microsoft.com/exchange/services/2006/types"">
                     <soap:Header>
                     <t:RequestServerVersion Version=""Exchange2013"" />
                     </soap:Header>
                     <soap:Body>
                     <GetAttachment xmlns=""http://schemas.microsoft.com/exchange/services/2006/messages""
                      xmlns:t=""http://schemas.microsoft.com/exchange/services/2006/types"">
                     <AttachmentShape>
                     <t:IncludeMimeContent>true</t:IncludeMimeContent>
                        <t:AdditionalProperties>
		                    <t:FieldURI FieldURI=""item:ConversationId""/>		                    
			                <t:FieldURI FieldURI=""message:ConversationTopic""/>			                
        		        </t:AdditionalProperties>
                     </AttachmentShape>
                     <AttachmentIds>
                     <t:AttachmentId Id=""{0}""/>
                     </AttachmentIds>
                     </GetAttachment>
                     </soap:Body>
                    </soap:Envelope>";


        /// <summary>
        /// The invalid char regex
        /// </summary>
        private static string invalidCharRegex = @"[\*\?\|\\\t/:""'<>#{}%~&]";

        /// <summary>
        /// The start end regex
        /// </summary>
        private static string startEndregex = @"^[\. ]|[\. ]$";

        /// <summary>
        /// The invalid rule regex
        /// </summary>
        private static string invalidRuleRegex = @"\.{2,}";

        /// <summary>
        /// The extra space regex
        /// </summary>
        private static string extraSpaceRegex = " {2,}";

        /// <summary>
        /// The invalid file name regex
        /// </summary>
        private static string invalidFileNameRegex = "_fajlovi|.files|-Dateien|_fichiers|_bestanden|_file|_archivos|-filer|_tiedostot|_pliki|_soubory|_elemei|_ficheiros|_arquivos|_dosyalar|_datoteke|_fitxers|_failid|_fails|_bylos|_fajlovi|_fitxategiak$";

        /// <summary>
        /// The is tenant deployment
        /// </summary>
        private static bool isTenantDeployment = Convert.ToBoolean(ConstantStrings.GetConfigurationFromResourceFile("Constants", "IsTenantDeployment", Enumerators.ResourceFileLocation.App_GlobalResources), CultureInfo.InvariantCulture);

        /// <summary>
        /// The is major version enable
        /// </summary>
        private static bool isMajorVersionEnable = Convert.ToBoolean(ConstantStrings.GetConfigurationFromResourceFile("Constants", "IsMajorVersionEnable", Enumerators.ResourceFileLocation.App_GlobalResources), CultureInfo.InvariantCulture);

        /// <summary>
        /// The is minor version enable
        /// </summary>
        private static bool isMinorVersionEnable = Convert.ToBoolean(ConstantStrings.GetConfigurationFromResourceFile("Constants", "IsMinorVersionEnable", Enumerators.ResourceFileLocation.App_GlobalResources), CultureInfo.InvariantCulture);

        /// <summary>
        /// The is force check out
        /// </summary>
        private static bool isForceCheckOut = Convert.ToBoolean(ConstantStrings.GetConfigurationFromResourceFile("Constants", "IsForceCheckOut", Enumerators.ResourceFileLocation.App_GlobalResources), CultureInfo.InvariantCulture);

        /// <summary>
        /// This indicates whether to create calendar or not 
        /// </summary>
        private static bool isCreateCalendarEnabled = Convert.ToBoolean(ConstantStrings.GetConfigurationFromResourceFile("Constants", "IsCreateCalendarEnabled", Enumerators.ResourceFileLocation.App_GlobalResources), CultureInfo.InvariantCulture);

        /// <summary>
        /// The brief case items query
        /// </summary>
        private static string briefCaseItemsQuery = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Get_Briefcase_Items_Query", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The get item by name query
        /// </summary>
        private static string getItemByNameQuery = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Get_Item_By_Name_Query", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The briefcase folder query
        /// </summary>
        private static string briefcaseFolderQuery = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Get_Legal_Briefcase_Folder_Query", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The briefcase folder contents query
        /// </summary>
        private static string briefcaseFolderContentsQuery = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Get_Legal_Briefcase_Folder_Contents", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The legal briefcase folder
        /// </summary>
        private static string legalBriefcaseFolder = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Legal_Briefcase_Folder_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The OneDrive content type name
        /// </summary>
        private static string oneDriveContentTypeName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "One_Drive_Content_Type", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The already check out
        /// </summary>
        private static string alreadyCheckOut = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Already_Check_Out_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The send to matter query
        /// </summary>
        private static string sendToMatterQuery = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Send_To_Matter_Query", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The user My site not present
        /// </summary>
        private static string userMySiteNotPresent = ConstantStrings.GetConfigurationFromResourceFile("Constants", "User_My_Site_Not_Present", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The URL is user My site not present
        /// </summary>
        private static string oneDriveNotSetupUrl = ConstantStrings.GetConfigurationFromResourceFile("Constants", "OneDriveNotSetupUrl", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The OneDrive parent content type
        /// </summary>
        private static string oneDriveParentContentType = ConstantStrings.GetConfigurationFromResourceFile("Constants", "One_Drive_Parent_Content_Type", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The OneDrive site column schema
        /// </summary>
        private static string oneDriveSiteColumnSchema = ConstantStrings.GetConfigurationFromResourceFile("Constants", "One_Drive_Site_Column_Schema", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The OneDrive content type group
        /// </summary>
        private static string oneDriveContentTypeGroup = ConstantStrings.GetConfigurationFromResourceFile("Constants", "One_Drive_Content_Type_Group", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The OneDrive site column
        /// </summary>
        private static string oneDriveSiteColumn = ConstantStrings.GetConfigurationFromResourceFile("Constants", "One_Drive_Site_Column", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The OneDrive site column type
        /// </summary>
        private static string oneDriveSiteColumnType = ConstantStrings.GetConfigurationFromResourceFile("Constants", "One_Drive_Site_Column_Type", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The exchange service URL
        /// </summary>
        private static string exchangeServiceURL = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Exchange_Service_URL", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The user Permissions on create matter library
        /// </summary>
        private static string userPermissions = ConstantStrings.GetConfigurationFromResourceFile("Constants", "User_Permissions", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// jQuery file name
        /// </summary>
        private static string jqueryFileName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Landing_Page_jQuery_File_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Matter Landing page JS file name
        /// </summary>
        private static string matterLandingJSFileName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Landing_Page_Script_File_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Matter Landing page CSS file name
        /// </summary>
        private static string matterLandingCSSFileName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Landing_Page_CSS_File_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Matter Landing page folder name on SharePoint
        /// </summary>
        private static string matterLandingFolderName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Landing_Folder", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Common files folder name on SharePoint
        /// </summary>
        private static string commonFolderName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Common_Folder", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Common js file on SharePoint
        /// </summary>
        private static string commonJSFileLink = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Common_JS_File_Location", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Common css file on SharePoint
        /// </summary>
        private static string commonCSSFileLink = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Common_CSS_File_Location", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Holds the number of search results (users) to pull from people picker service based on the search term. This will return email enabled users as well as non-email enabled users.
        /// </summary>
        private static string peoplePickerMaximumEntitySuggestions = ConstantStrings.GetConfigurationFromResourceFile("Constants", "People_Picker_Max_Entity_Suggestions", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Holds the number of records to pull which has email property from client picker service results.
        /// </summary>
        private static string peoplePickerMaxRecords = ConstantStrings.GetConfigurationFromResourceFile("Constants", "People_Picker_Max_Records", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Holds the flag to allow multiple users to be searched using people picker service.
        /// </summary>
        private static string peoplePickerAllowMultipleEntities = ConstantStrings.GetConfigurationFromResourceFile("Constants", "People_Picker_Allow_Multiple_Entities", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The temp email name
        /// </summary>
        private static string tempEmailName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Temp_Email_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect input code for user access in Provision Matter Group
        /// </summary>
        private static string incorrectInputUserAccessCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Access_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect input code for user access in Provision Matter Group
        /// </summary>
        private static string incorrectInputUserAccessMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Access_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// User's OneDrive document library title
        /// </summary>
        private static string oneDriveDocumentLibraryTitle = ConstantStrings.GetConfigurationFromResourceFile("Constants", "One_Drive_Document_Library_Title", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Date format for dates displayed in Matter Center app
        /// </summary>
        private static string matterCenterDateFormat = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Center_Date_Format", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// ENUM operationTypes
        /// </summary>
        internal enum OperationTypes
        {
            /// <summary>
            /// The current
            /// </summary>
            Current,

            /// <summary>
            /// The update
            /// </summary>
            Update,

            /// <summary>
            /// The checkout
            /// </summary>
            Checkout,

            /// <summary>
            /// The detach
            /// </summary>
            Detach,

            /// <summary>
            /// The check in
            /// </summary>
            CheckIn
        }

        /// <summary>
        /// Gets the name of the practice group term set.
        /// </summary>
        /// <value>The name of the practice group term set.</value>
        internal static string PracticeGroupTermSetName
        {
            get
            {
                return practiceGroupTermSetName;
            }
        }

        /// <summary>
        /// Gets the name of the client term set.
        /// </summary>
        /// <value>The name of the client term set.</value>
        internal static string ClientTermSetName
        {
            get
            {
                return clientTermSetName;
            }
        }

        /// <summary>
        /// Gets the name of the log table.
        /// </summary>
        /// <value>The name of the log table.</value>
        internal static string LogTableName
        {
            get
            {
                return logTableName;
            }
        }

        /// <summary>
        /// Gets the name of the DMS role list.
        /// </summary>
        /// <value>The name of the DMS role list.</value>
        internal static string DMSRoleListName
        {
            get
            {
                return dmsRoleListName;
            }
        }

        /// <summary>
        /// Gets the DMS role query.
        /// </summary>
        /// <value>The DMS role query.</value>
        internal static string DMSRoleQuery
        {
            get
            {
                return dmsRoleQuery;
            }
        }

        /// <summary>
        /// Gets the column name unique identifier.
        /// </summary>
        /// <value>The column name unique identifier.</value>
        internal static string ColumnNameGuid
        {
            get
            {
                return columnNameGuid;
            }
        }

        /// <summary>
        /// Gets the name of the role list column role.
        /// </summary>
        /// <value>The name of the role list column role.</value>
        internal static string RoleListColumnRoleName
        {
            get
            {
                return roleListColumnRoleName;
            }
        }

        /// <summary>
        /// Gets the role list column is role mandatory.
        /// </summary>
        /// <value>The role list column is role mandatory.</value>
        internal static string RoleListColumnIsRoleMandatory
        {
            get
            {
                return roleListColumnIsRoleMandatory;
            }
        }

        /// <summary>
        /// Gets the central repository URL.
        /// </summary>
        /// <value>The central repository URL.</value>
        internal static string CentralRepositoryUrl
        {
            get
            {
                return centralRepositoryUrl;
            }
        }

        /// <summary>
        /// Gets the name of the DMS matter list.
        /// </summary>
        /// <value>The name of the DMS matter list.</value>
        internal static string DMSMatterListName
        {
            get
            {
                return dmsMatterListName;
            }
        }

        /// <summary>
        /// Gets the share list column matter path.
        /// </summary>
        /// <value>The share list column matter path.</value>
        internal static string ShareListColumnMatterPath
        {
            get
            {
                return shareListColumnMatterPath;
            }
        }

        /// <summary>
        /// Gets the stamped property practice group.
        /// </summary>
        /// <value>The stamped property practice group.</value>
        internal static string StampedPropertyPracticeGroup
        {
            get
            {
                return stampedPropertyPracticeGroup;
            }
        }

        /// <summary>
        /// Gets the stamped property area of law.
        /// </summary>
        /// <value>The stamped property area of law.</value>
        internal static string StampedPropertyAreaOfLaw
        {
            get
            {
                return stampedPropertyAreaOfLaw;
            }
        }

        /// <summary>
        /// Gets the stamped property sub area of law.
        /// </summary>
        /// <value>The stamped property sub area of law.</value>
        internal static string StampedPropertySubAreaOfLaw
        {
            get
            {
                return stampedPropertySubAreaOfLaw;
            }
        }

        /// <summary>
        /// Gets the name of the stamped property matter.
        /// </summary>
        /// <value>The name of the stamped property matter.</value>
        internal static string StampedPropertyMatterName
        {
            get
            {
                return stampedPropertyMatterName;
            }
        }

        /// <summary>
        /// Gets the stamped property matter identifier.
        /// </summary>
        /// <value>The stamped property matter identifier.</value>
        internal static string StampedPropertyMatterID
        {
            get
            {
                return stampedPropertyMatterID;
            }
        }

        /// <summary>
        /// Gets the name of the stamped property client.
        /// </summary>
        /// <value>The name of the stamped property client.</value>
        internal static string StampedPropertyClientName
        {
            get
            {
                return stampedPropertyClientName;
            }
        }

        /// <summary>
        /// Gets the stamped property client identifier.
        /// </summary>
        /// <value>The stamped property client identifier.</value>
        internal static string StampedPropertyClientID
        {
            get
            {
                return stampedPropertyClientID;
            }
        }

        /// <summary>
        /// Gets the stamped property responsible attorney.
        /// </summary>
        /// <value>The stamped property responsible attorney.</value>
        internal static string StampedPropertyResponsibleAttorney
        {
            get
            {
                return stampedPropertyResponsibleAttorney;
            }
        }

        /// <summary>
        /// Gets the stamped property responsible attorney email.
        /// </summary>
        /// <value>The stamped property responsible attorney email.</value>
        internal static string StampedPropertyResponsibleAttorneyEmail
        {
            get
            {
                return stampedPropertyResponsibleAttorneyEmail;
            }
        }

        /// <summary>
        /// Gets the stamped property team members.
        /// </summary>
        /// <value>The stamped property team members.</value>
        internal static string StampedPropertyTeamMembers
        {
            get
            {
                return stampedPropertyTeamMembers;
            }
        }

        /// <summary>
        /// Gets the stamped property is matter.
        /// </summary>
        /// <value>The stamped property is matter.</value>
        internal static string StampedPropertyIsMatter
        {
            get
            {
                return stampedPropertyIsMatter;
            }
        }

        /// <summary>
        /// Gets the stamped property open date.
        /// </summary>
        /// <value>The stamped property open date.</value>
        internal static string StampedPropertyOpenDate
        {
            get
            {
                return stampedPropertyOpenDate;
            }
        }

        /// <summary>
        /// Gets the item property modified date.
        /// </summary>
        /// <value>The item property modified date.</value>
        internal static string ColumnNameModifiedDate
        {
            get
            {
                return columnNameModifiedDate;
            }
        }

        /// <summary>
        /// Gets the stamped property secure matter.
        /// </summary>
        /// <value>The stamped property secure matter.</value>
        internal static string StampedPropertySecureMatter
        {
            get
            {
                return stampedPropertySecureMatter;
            }
        }

        /// <summary>
        /// Gets the stamped property success
        /// </summary>
        /// <value>The stamped property success</value>
        internal static string StampedPropertySuccess
        {
            get
            {
                return stampedPropertySuccess;
            }
        }

        /// <summary>
        /// Gets the content type column client identifier.
        /// </summary>
        /// <value>The content type column client identifier.</value>
        internal static string ContentTypeColumnClientId
        {
            get
            {
                return contentTypeColumnClientId;
            }
        }

        /// <summary>
        /// Gets the name of the content type column client.
        /// </summary>
        /// <value>The name of the content type column client.</value>
        internal static string ContentTypeColumnClientName
        {
            get
            {
                return contentTypeColumnClientName;
            }
        }

        /// <summary>
        /// Gets the content type column matter identifier.
        /// </summary>
        /// <value>The content type column matter identifier.</value>
        internal static string ContentTypeColumnMatterId
        {
            get
            {
                return contentTypeColumnMatterId;
            }
        }

        /// <summary>
        /// Gets the name of the content type column matter.
        /// </summary>
        /// <value>The name of the content type column matter.</value>
        internal static string ContentTypeColumnMatterName
        {
            get
            {
                return contentTypeColumnMatterName;
            }
        }

        /// <summary>
        /// Gets the name of the content type column matter.
        /// </summary>
        /// <value>The name of the content type column matter.</value>
        internal static string ContentTypeColumnPracticeGroup
        {
            get
            {
                return contentTypeColumnPracticeGroup;
            }
        }

        /// <summary>
        /// Gets the name of the content type column matter.
        /// </summary>
        /// <value>The name of the content type column matter.</value>
        internal static string ContentTypeColumnAreaOfLaw
        {
            get
            {
                return contentTypeColumnAreaOfLaw;
            }
        }

        /// <summary>
        /// Gets the name of the content type column matter.
        /// </summary>
        /// <value>The name of the content type column matter.</value>
        internal static string ContentTypeColumnSubareaOfLaw
        {
            get
            {
                return contentTypeColumnSubareaOfLaw;
            }
        }

        /// <summary>
        /// Gets the matters list column title.
        /// </summary>
        /// <value>The matters list column title.</value>
        internal static string MattersListColumnTitle
        {
            get
            {
                return mattersListColumnTitle;
            }
        }

        /// <summary>
        /// Gets the name of the matters list column client.
        /// </summary>
        /// <value>The name of the matters list column client.</value>
        internal static string MattersListColumnClientName
        {
            get
            {
                return mattersListColumnClientName;
            }
        }

        /// <summary>
        /// Gets the matters list column client identifier.
        /// </summary>
        /// <value>The matters list column client identifier.</value>
        internal static string MattersListColumnClientID
        {
            get
            {
                return mattersListColumnClientID;
            }
        }

        /// <summary>
        /// Gets the name of the matters list column matter.
        /// </summary>
        /// <value>The name of the matters list column matter.</value>
        internal static string MattersListColumnMatterName
        {
            get
            {
                return mattersListColumnMatterName;
            }
        }

        /// <summary>
        /// Gets the matters list column matter identifier.
        /// </summary>
        /// <value>The matters list column matter identifier.</value>
        internal static string MattersListColumnMatterID
        {
            get
            {
                return mattersListColumnMatterID;
            }
        }

        /// <summary>
        /// Gets the matters list column conflict check by.
        /// </summary>
        /// <value>The matters list column conflict check by.</value>
        internal static string MattersListColumnConflictCheckBy
        {
            get
            {
                return mattersListColumnConflictCheckBy;
            }
        }

        /// <summary>
        /// Gets the matters list column conflict check on.
        /// </summary>
        /// <value>The matters list column conflict check on.</value>
        internal static string MattersListColumnConflictCheckOn
        {
            get
            {
                return mattersListColumnConflictCheckOn;
            }
        }

        /// <summary>
        /// Gets the matters list column conflict identified.
        /// </summary>
        /// <value>The matters list column conflict identified.</value>
        internal static string MattersListColumnConflictIdentified
        {
            get
            {
                return mattersListColumnConflictIdentified;
            }
        }

        /// <summary>
        /// Gets the matters list column block users.
        /// </summary>
        /// <value>The matters list column block users.</value>
        internal static string MattersListColumnBlockUsers
        {
            get
            {
                return mattersListColumnBlockUsers;
            }
        }

        /// <summary>
        /// Gets the matters list column managing attorney.
        /// </summary>
        /// <value>The matters list column managing attorney.</value>
        internal static string MattersListColumnManagingAttorney
        {
            get
            {
                return mattersListColumnManagingAttorney;
            }
        }

        /// <summary>
        /// Gets the matters list column support.
        /// </summary>
        /// <value>The matters list column support.</value>
        internal static string MattersListColumnSupport
        {
            get
            {
                return mattersListColumnSupport;
            }
        }

        /// <summary>
        /// Gets all folders query.
        /// </summary>
        /// <value>All folders query.</value>
        internal static string AllFoldersQuery
        {
            get
            {
                return allFoldersQuery;
            }
        }

        /// <summary>
        /// Gets the name of the user pinned matter list.
        /// </summary>
        /// <value>The name of the user pinned matter list.</value>
        internal static string UserPinnedMatterListName
        {
            get
            {
                return userPinnedMatterListName;
            }
        }

        /// <summary>
        /// Gets the pinned list column matter details.
        /// </summary>
        /// <value>The pinned list column matter details.</value>
        internal static string PinnedListColumnMatterDetails
        {
            get
            {
                return pinnedListColumnMatterDetails;
            }
        }

        /// <summary>
        /// Gets the user pinned details query.
        /// </summary>
        /// <value>The user pinned details query.</value>
        internal static string UserPinnedDetailsQuery
        {
            get
            {
                return userPinnedDetailsQuery;
            }
        }

        /// <summary>
        /// Gets the pinned list column user alias.
        /// </summary>
        /// <value>The pinned list column user alias.</value>
        internal static string PinnedListColumnUserAlias
        {
            get
            {
                return pinnedListColumnUserAlias;
            }
        }

        /// <summary>
        /// Gets the name of the user pinned document list.
        /// </summary>
        /// <value>The name of the user pinned document list.</value>
        internal static string UserPinnedDocumentListName
        {
            get
            {
                return userPinnedDocumentListName;
            }
        }

        /// <summary>
        /// Gets the pinned list column document details.
        /// </summary>
        /// <value>The pinned list column document details.</value>
        internal static string PinnedListColumnDocumentDetails
        {
            get
            {
                return pinnedListColumnDocumentDetails;
            }
        }

        /// <summary>
        /// Gets the column name file leaf reference.
        /// </summary>
        /// <value>The column name file leaf reference.</value>
        internal static string ColumnNameFileLeafRef
        {
            get
            {
                return columnNameFileLeafRef;
            }
        }

        /// <summary>
        /// Gets the column name file reference.
        /// </summary>
        /// <value>The column name file reference.</value>
        internal static string ColumnNameFileRef
        {
            get
            {
                return columnNameFileRef;
            }
        }

        /// <summary>
        /// Gets the column name file directory reference.
        /// </summary>
        /// <value>The column name file directory reference.</value>
        internal static string ColumnNameFileDirRef
        {
            get
            {
                return columnNameFileDirRef;
            }
        }

        /// <summary>
        /// Gets the property name VTI indexed property keys.
        /// </summary>
        /// <value>The property name VTI indexed property keys.</value>
        internal static string PropertyNameVtiIndexedPropertyKeys
        {
            get
            {
                return propertyNameVtiIndexedPropertyKeys;
            }
        }

        /// <summary>
        /// Gets the sub area custom property folder names.
        /// </summary>
        /// <value>The sub area custom property folder names.</value>
        internal static string SubAreaCustomPropertyFolderNames
        {
            get
            {
                return subAreaCustomPropertyFolderNames;
            }
        }

        /// <summary>
        /// Gets the sub area custom property is no folder structure present.
        /// </summary>
        /// <value>The sub area custom property is no folder structure present.</value>
        internal static string SubAreaCustomPropertyisNoFolderStructurePresent
        {
            get
            {
                return subAreaCustomPropertyisNoFolderStructurePresent;
            }
        }

        /// <summary>
        /// Gets the area custom property folder names.
        /// </summary>
        /// <value>The area custom property folder names.</value>
        internal static string AreaCustomPropertyFolderNames
        {
            get
            {
                return areaCustomPropertyFolderNames;
            }
        }

        /// <summary>
        /// Gets the practice group custom property folder names.
        /// </summary>
        /// <value>The practice group custom property folder names.</value>
        internal static string PracticeGroupCustomPropertyFolderNames
        {
            get
            {
                return practiceGroupCustomPropertyFolderNames;
            }
        }

        /// <summary>
        /// Gets the sub area of law document templates.
        /// </summary>
        /// <value>The sub area of law document templates.</value>
        internal static string SubAreaOfLawDocumentTemplates
        {
            get
            {
                return subAreaOfLawDocumentTemplates;
            }
        }

        /// <summary>
        /// Gets the type of the hidden content.
        /// </summary>
        /// <value>The type of the hidden content.</value>
        internal static string HiddenContentType
        {
            get
            {
                return hiddenContentType;
            }
        }

        /// <summary>
        /// Gets the stamped property blocked upload users.
        /// </summary>
        /// <value>The stamped property blocked upload users.</value>
        internal static string StampedPropertyBlockedUploadUsers
        {
            get
            {
                return stampedPropertyBlockedUploadUsers;
            }
        }

        /// <summary>
        /// Gets the stamped property matter description
        /// </summary>
        /// <value>The stamped property matter description.</value>
        internal static string StampedPropertyMatterDescription
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Matter_Description", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the stamped property of matter conflict check date
        /// </summary>
        /// <value>The stamped property of matter conflict check date.</value>
        internal static string StampedPropertyConflictCheckDate
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Conflict_Check_Date", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the stamped property of matter conflict check by
        /// </summary>
        /// <value>The stamped property of matter conflict check by.</value>
        internal static string StampedPropertyConflictCheckBy
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Conflict_Check_By", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the stamped property of matter users
        /// </summary>
        /// <value>The stamped property of matter users.</value>
        internal static string StampedPropertyMatterCenterUsers
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_MatterCenter_Users", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the stamped property of matter center user emails
        /// </summary>
        /// <value>The stamped property of matter center users emails.</value>
        internal static string StampedPropertyMatterCenterUserEmails
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_MatterCenter_User_Emails", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the stamped property of matter roles
        /// </summary>
        /// <value>The stamped property of matter roles.</value>
        internal static string StampedPropertyMatterCenterRoles
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_MatterCenter_Roles", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the stamped property of matter Permissions
        /// </summary>
        /// <value>The stamped property of matter roles.</value>
        internal static string StampedPropertyMatterCenterPermissions
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_MatterCenter_Permissions", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the stamped property of matter default content type
        /// </summary>
        /// <value>The stamped property of matter roles.</value>
        internal static string StampedPropertyDefaultContentType
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_DefaultContentType", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the stamped property of matter conflict identified flag
        /// </summary>
        /// <value>The stamped property of matter roles.</value>
        internal static string StampedPropertyIsConflictIdentified
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_IsConflictIdentified", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the stamped property of matter document template count
        /// </summary>
        /// <value>The stamped property of matter roles.</value>
        internal static string StampedPropertyDocumentTemplateCount
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_DocumentTempateCount", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the stamped property of matter blocked upload users
        /// </summary>
        /// <value>The stamped property of matter roles.</value>
        internal static string StampedPropertyBlockedUsers
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Blocked_Users", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the stamped property of matter GUID
        /// </summary>
        /// <value>The stamped property of matter GUID.</value>
        internal static string StampedPropertyMatterGUID
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Stamped_Property_Matter_GUID", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the code for delete matter functionality
        /// </summary>
        /// <value>Code for delete matter functionality.</value>
        internal static string DeleteMatterCode
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "DeleteMatterCode", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the code if matter library already exists
        /// </summary>
        /// <value>code if matter library already exists.</value>
        internal static string MatterLibraryExistsCode
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "MatterLibraryExistsCode", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the code if matter landing page already exists
        /// </summary>
        /// <value>code if matter landing page already exists.</value>
        internal static string MatterLandingExistsCode
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "MatterLandingExistsCode", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the mail SOAP request.
        /// </summary>
        /// <value>The mail SOAP request.</value>
        internal static string MailSoapRequest
        {
            get
            {
                return mailSoapRequest;
            }
        }

        /// <summary>
        /// Gets the attachment SOAP request.
        /// </summary>
        /// <value>The attachment SOAP request.</value>
        internal static string AttachmentSoapRequest
        {
            get
            {
                return attachmentSoapRequest;
            }
        }

        /// <summary>
        /// Gets the invalid character regex.
        /// </summary>
        /// <value>The invalid character regex.</value>
        internal static string InvalidCharRegex
        {
            get
            {
                return invalidCharRegex;
            }
        }

        /// <summary>
        /// Gets the start end REGEX.
        /// </summary>
        /// <value>The start end REGEX.</value>
        internal static string StartEndregex
        {
            get
            {
                return startEndregex;
            }
        }

        /// <summary>
        /// Gets the invalid rule REGEX.
        /// </summary>
        /// <value>The invalid rule REGEX.</value>
        internal static string InvalidRuleRegex
        {
            get
            {
                return invalidRuleRegex;
            }
        }

        /// <summary>
        /// Gets the extra space regex.
        /// </summary>
        /// <value>The extra space regex.</value>
        internal static string ExtraSpaceRegex
        {
            get
            {
                return extraSpaceRegex;
            }
        }

        /// <summary>
        /// Gets the invalid file name regex.
        /// </summary>
        /// <value>The invalid file name regex.</value>
        internal static string InvalidFileNameRegex
        {
            get
            {
                return invalidFileNameRegex;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is tenant deployment.
        /// </summary>
        /// <value><c>true</c> if this instance is tenant deployment; otherwise, <c>false</c>.</value>
        internal static bool IsTenantDeployment
        {
            get
            {
                return isTenantDeployment;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is major version enable.
        /// </summary>
        /// <value><c>true</c> if this instance is major version enable; otherwise, <c>false</c>.</value>
        internal static bool IsMajorVersionEnable
        {
            get
            {
                return isMajorVersionEnable;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is minor version enable.
        /// </summary>
        /// <value><c>true</c> if this instance is minor version enable; otherwise, <c>false</c>.</value>
        internal static bool IsMinorVersionEnable
        {
            get
            {
                return isMinorVersionEnable;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is force check out.
        /// </summary>
        /// <value><c>true</c> if this instance is force check out; otherwise, <c>false</c>.</value>
        internal static bool IsForceCheckOut
        {
            get
            {
                return isForceCheckOut;
            }
        }

        /// <summary>
        ///  Gets a value indicating whether to create calendar for matter or not
        /// </summary>
        /// <value><c>true</c>if creation of calendar is to be done; otherwise, <c>false</c></value>
        internal static bool IsCreateCalendarEnabled
        {
            get
            {
                return isCreateCalendarEnabled;
            }
        }

        /// <summary>
        /// Gets the brief case items query.
        /// </summary>
        /// <value>The brief case items query.</value>
        internal static string BriefCaseItemsQuery
        {
            get
            {
                return briefCaseItemsQuery;
            }
        }

        /// <summary>
        /// Gets the get item by name query.
        /// </summary>
        /// <value>The get item by name query.</value>
        internal static string GetItemByNameQuery
        {
            get
            {
                return getItemByNameQuery;
            }
        }

        /// <summary>
        /// Gets the briefcase folder query.
        /// </summary>
        /// <value>The briefcase folder query.</value>
        internal static string BriefcaseFolderQuery
        {
            get
            {
                return briefcaseFolderQuery;
            }
        }

        /// <summary>
        /// Gets the briefcase folder contents query.
        /// </summary>
        /// <value>The briefcase folder contents query.</value>
        internal static string BriefcaseFolderContentsQuery
        {
            get
            {
                return briefcaseFolderContentsQuery;
            }
        }

        /// <summary>
        /// Gets the legal briefcase folder.
        /// </summary>
        /// <value>The legal briefcase folder.</value>
        internal static string LegalBriefcaseFolder
        {
            get
            {
                return legalBriefcaseFolder;
            }
        }

        /// <summary>
        /// Gets the name of the OneDrive content type.
        /// </summary>
        /// <value>The name of the OneDrive content type.</value>
        internal static string OneDriveContentTypeName
        {
            get
            {
                return oneDriveContentTypeName;
            }
        }

        /// <summary>
        /// Gets the already check out.
        /// </summary>
        /// <value>The already check out.</value>
        internal static string AlreadyCheckOut
        {
            get
            {
                return alreadyCheckOut;
            }
        }

        /// <summary>
        /// Gets the send to matter query.
        /// </summary>
        /// <value>The send to matter query.</value>
        internal static string SendToMatterQuery
        {
            get
            {
                return sendToMatterQuery;
            }
        }

        /// <summary>
        /// Gets the user my site not present.
        /// </summary>
        /// <value>The user my site not present.</value>
        internal static string UserMySiteNotPresent
        {
            get
            {
                return userMySiteNotPresent;
            }
        }

        /// <summary>
        /// Gets the user my site not present URL when OneDrive is not configured.
        /// </summary>
        /// <value>The URL when user my site not present.</value>
        internal static string OneDriveNotSetupUrl
        {
            get
            {
                return oneDriveNotSetupUrl;
            }
        }

        /// <summary>
        /// Gets the type of the OneDrive parent content.
        /// </summary>
        /// <value>The type of the OneDrive parent content.</value>
        internal static string OneDriveParentContentType
        {
            get
            {
                return oneDriveParentContentType;
            }
        }

        /// <summary>
        /// Gets the OneDrive site column schema.
        /// </summary>
        /// <value>The OneDrive site column schema.</value>
        internal static string OneDriveSiteColumnSchema
        {
            get
            {
                return oneDriveSiteColumnSchema;
            }
        }

        /// <summary>
        /// Gets the OneDrive content type group.
        /// </summary>
        /// <value>The OneDrive content type group.</value>
        internal static string OneDriveContentTypeGroup
        {
            get
            {
                return oneDriveContentTypeGroup;
            }
        }

        /// <summary>
        /// Gets the OneDrive site column.
        /// </summary>
        /// <value>The OneDrive site column.</value>
        internal static string OneDriveSiteColumn
        {
            get
            {
                return oneDriveSiteColumn;
            }
        }

        /// <summary>
        /// Gets the type of the OneDrive site column.
        /// </summary>
        /// <value>The type of the OneDrive site column.</value>
        internal static string OneDriveSiteColumnType
        {
            get
            {
                return oneDriveSiteColumnType;
            }
        }

        /// <summary>
        /// Gets the exchange service URL.
        /// </summary>
        /// <value>The exchange service URL.</value>
        internal static string ExchangeServiceURL
        {
            get
            {
                return exchangeServiceURL;
            }
        }

        /// <summary>
        /// Gets the share list column mail list.
        /// </summary>
        /// <value>The share list column mail list.</value>
        internal static string ShareListColumnMailList
        {
            get
            {
                return shareListColumnMailList;
            }
        }

        /// <summary>
        /// Gets the jQuery file name from the Constants file
        /// </summary>
        internal static string JQueryFileName
        {
            get
            {
                return jqueryFileName;
            }
        }

        /// <summary>
        /// Gets the JS file name from resource file used in Matter Landing page
        /// </summary>
        internal static string MatterLandingJSFileName
        {
            get
            {
                return matterLandingJSFileName;
            }
        }

        /// <summary>
        /// Gets the CSS file name of the matter landing page
        /// </summary>
        internal static string MatterLandingCSSFileName
        {
            get
            {
                return matterLandingCSSFileName;
            }
        }

        /// <summary>
        /// Gets the appropriate User Permissions
        /// </summary>
        internal static string UserPermissions
        {
            get
            {
                return userPermissions;
            }
        }

        /// <summary>
        /// Gets the list of invalid extension not to be displayed in find document app
        /// </summary>
        internal static string FindDocumentInvalidExtensions
        {
            get
            {
                return findDocumentInvalidExtensions;
            }
        }

        /// <summary>
        /// Gets the Valid Date format
        /// </summary>
        internal static string ValidDateFormat
        {
            get
            {
                return validDateFormat;
            }
        }

        /// <summary>
        /// Suffix to be appended in calendar name
        /// </summary>
        internal static string CalendarNameSuffix
        {
            get
            {
                return calendarNameSuffix;
            }
        }

        /// <summary>
        /// Suffix to be appended in calendar name
        /// </summary>
        internal static string TaskNameSuffix
        {
            get
            {
                return taskNameSuffix;
            }
        }

        /// <summary>
        /// Gets the Matter Landing Folder Name
        /// </summary>
        internal static string MatterLandingFolderName
        {
            get
            {
                return matterLandingFolderName;
            }
        }

        /// <summary>
        /// Gets the Common Folder Name
        /// </summary>
        internal static string CommonFolderName
        {
            get
            {
                return commonFolderName;
            }
        }

        /// <summary>
        /// Gets the common js file location on SharePoint
        /// </summary>
        internal static string CommonJSFileLink
        {
            get
            {
                return commonJSFileLink;
            }
        }

        /// <summary>
        /// Gets the common css file location on SharePoint
        /// </summary>
        internal static string CommonCSSFileLink
        {
            get
            {
                return commonCSSFileLink;
            }
        }

        /// <summary>
        /// Gets the matter Id length
        /// </summary>
        /// <value>The matter Id length</value>
        internal static string MatterIdLength
        {
            get
            {
                return matterIdLength;
            }
        }

        /// <summary>
        /// Gets the matter name length
        /// </summary>
        /// <value>The matter name length</value>
        internal static string MatterNameLength
        {
            get
            {
                return matterNameLength;
            }
        }

        /// <summary>
        /// Gets the matter description length
        /// </summary>
        /// <value>The matter description length</value>
        internal static string MatterDescriptionLength
        {
            get
            {
                return matterDescriptionLength;
            }
        }

        /// <summary>
        /// Gets the content type length
        /// </summary>
        /// <value>The content type length</value>
        internal static string ContentTypeLength
        {
            get
            {
                return contentTypeLength;
            }
        }

        /// <summary>
        /// Gets the error message when matter library is not found when trying to delete
        /// </summary>
        /// <value>Error message when matter library is not found when trying to delete</value>
        internal static string MatterNotPresent
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Not_Present", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the name of the custom view.
        /// </summary>
        /// <value>The name of the custom view</value>
        internal static string ViewName
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "View_Name", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the list of columns for the custom view.
        /// </summary>
        /// <value>The list of columns for the custom view</value>
        internal static string ViewColumnList
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "View_Column_List", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the column to set OrderBy for the custom view.
        /// </summary>
        /// <value>The column to set OrderBy for the custom view</value>
        internal static string ViewOrderByColumn
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "View_OrderBy_Column", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the query to set OrderBy for the custom view.
        /// </summary>
        /// <value>The query to set OrderBy for the custom view</value>
        internal static string ViewOrderByQuery
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "View_OrderBy_Query", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Holds the number of search results (users) to pull from people picker service based on the search term. This will return email enabled users as well as non-email enabled users.
        /// </summary>
        internal static string PeoplePickerMaximumEntitySuggestions
        {
            get
            {
                return peoplePickerMaximumEntitySuggestions;
            }
        }

        /// <summary>
        /// Holds the number of records to pull which has email property from client picker service results.
        /// </summary>
        internal static string PeoplePickerMaxRecords
        {
            get
            {
                return peoplePickerMaxRecords;
            }
        }

        /// <summary>
        /// Holds the flag to allow multiple users to be searched using people picker service.
        /// </summary>
        internal static string PeoplePickerAllowMultipleEntities
        {
            get
            {
                return peoplePickerAllowMultipleEntities;
            }
        }

        /// <summary>
        /// Holds the flag to allow multiple users to be searched using people picker service.
        /// </summary>
        internal static string TempEmailName
        {
            get
            {
                return tempEmailName;
            }
        }

        /// <summary>
        /// Holds the Client ID property for Clients Term Set
        /// </summary>
        internal static string ClientCustomPropertiesId
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Client_Custom_Properties_Id", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Holds the error code for matter landing existence
        /// </summary>
        internal static string ErrorCodeMatterLandingPageExists
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorCodeMatterLandingPageExists", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Suffix to be appended in OneNote library name
        /// </summary>
        internal static string OneNoteLibrarySuffix
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "OneNoteLibrary_Name_Suffix", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Represents the library name holding the matter landing pages for the site collection
        /// </summary>
        internal static string MatterLandingPageRepositoryName
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Landing_Page_Repository_Name", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the content of the Matter Center Help Section List.
        /// </summary>
        /// <value>The Matter Center help section list name.</value>
        internal static string MatterCenterHelpSectionListName
        {
            get
            {
                return matterCenterHelpSectionListName;
            }
        }

        /// <summary>
        /// Name of the column storing Link Title.
        /// </summary>
        /// <value>Name of the column storing Link Title.</value>
        internal static string ContextualHelpLinksColumnLinkTitle
        {
            get
            {
                return contextualHelpLinksColumnLinkTitle;
            }
        }

        /// <summary>
        /// Name of the column storing Link URL.
        /// </summary>
        /// <value>Name of the column storing Link URL.</value>
        internal static string ContextualHelpLinksColumnLinkURL
        {
            get
            {
                return contextualHelpLinksColumnLinkURL;
            }
        }

        /// <summary>
        /// Name of the column storing Link Order.
        /// </summary>
        /// <value>Name of the column storing Link Order.</value>
        internal static string ContextualHelpLinksColumnLinkOrder
        {
            get
            {
                return contextualHelpLinksColumnLinkOrder;
            }
        }

        /// <summary>
        /// Name of the column storing Section ID.
        /// </summary>
        /// <value>Name of the column storing Section ID.</value>
        internal static string ContextualHelpLinksColumnSectionID
        {
            get
            {
                return contextualHelpLinksColumnSectionID;
            }
        }

        /// <summary>
        /// Name of the column storing Section ID.
        /// </summary>
        /// <value>Name of the column storing Section ID.</value>
        internal static string ContextualHelpSectionColumnSectionID
        {
            get
            {
                return contextualHelpSectionColumnSectionID;
            }
        }

        /// <summary>
        /// Name of the column storing Section Title.
        /// </summary>
        /// <value>Name of the column storing Section Title.</value>
        internal static string ContextualHelpSectionColumnSectionTitle
        {
            get
            {
                return contextualHelpSectionColumnSectionTitle;
            }
        }

        /// <summary>
        /// Name of the column storing Page Name.
        /// </summary>
        /// <value>Name of the column storing Page Name.</value>
        internal static string ContextualHelpSectionColumnPageName
        {
            get
            {
                return contextualHelpSectionColumnPageName;
            }
        }

        /// <summary>
        /// Name of the column storing Section Order.
        /// </summary>
        /// <value>Name of the column storing Section Order.</value>
        internal static string ContextualHelpSectionColumnSectionOrder
        {
            get
            {
                return contextualHelpSectionColumnSectionOrder;
            }
        }

        /// <summary>
        /// Name of the column storing Number of Columns.
        /// </summary>
        /// <value>Name of the column storing Number of Columns.</value>
        internal static string ContextualHelpSectionColumnNumberOfColumns
        {
            get
            {
                return contextualHelpSectionColumnNumberOfColumns;
            }
        }

        /// <summary>
        /// Gets the content of the Matter Center Help links List.
        /// </summary>
        /// <value>The Matter Center help links list name .</value>
        internal static string MatterCenterHelpLinksListName
        {
            get
            {
                return matterCenterHelpLinksListName;
            }
        }

        /// <summary>
        /// Gets contextual help sections.
        /// </summary>
        /// <value>Query to be used for retrieving contextual help sections.</value>
        internal static string RetrieveContextualHelpSectionsQuery
        {
            get
            {
                return retrieveContextualHelpSectionsQuery;
            }
        }

        /// <summary>
        /// Gets contextual help links.
        /// </summary>
        /// <value>Query to be used for retrieving contextual help links.</value>
        internal static string RetrieveContextualHelpLinksQuery
        {
            get
            {
                return retrieveContextualHelpLinksQuery;
            }
        }

        /// <summary>
        /// Represents page names to be used in contextual help functionality
        /// </summary>
        /// <value>';' separated list of page names to be used in contextual help functionality.</value>
        internal static string MatterCenterPages
        {
            get
            {
                return matterCenterPages;
            }
        }

        /// <summary>
        /// Represents OR part of CAMLquery to support include sections (link)
        /// </summary>
        /// <value>';' separated list to build OR part of CAMLquery to support include sections (link).</value>
        internal static string ContextualHelpQueryIncludeOrCondition
        {
            get
            {
                return contextualHelpQueryIncludeOrCondition;
            }
        }

        /// <summary>
        /// Represents the error message when Full Control permission is not available
        /// </summary>
        internal static string ErrorEditMatterMandatoryPermission
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Error_Edit_Matter_Mandatory_Permission", Enumerators.ResourceFileLocation.App_GlobalResources); ;
            }
        }

        /// <summary>
        /// Represents the error code when user tries to remove self permission
        /// </summary>
        internal static string IncorrectInputSelfPermissionRemoval
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Self_Permission_Removal_Code", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Represents the error code when security group exists in team members
        /// </summary>
        internal static string ErrorCodeSecurityGroupExists
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Security_Group_Exists_Code", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Represents the error message when security group exists in team members
        /// </summary>
        internal static string ErrorMessageSecurityGroupExists
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Security_Group_Exists_Message", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Represents the error code when team members are not valid
        /// </summary>
        internal static string IncorrectTeamMembersCode
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Team_Members_Code", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Represents the error message when team members are not valid
        /// </summary>
        internal static string IncorrectTeamMembersMessage
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Team_Members_Message", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Represents the matter configuration entry title value
        /// </summary>
        internal static string MatterConfigurationTitleValue
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Configurations_Title_Value", Enumerators.ResourceFileLocation.App_GlobalResources); ;
            }
        }

        /// <summary>
        /// Represents the matter configurations list query
        /// </summary>
        internal static string MatterConfigurationsListQuery
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Configurations_List_Query", Enumerators.ResourceFileLocation.App_GlobalResources); ;
            }
        }

        /// <summary>
        /// Title for Lists Path
        /// </summary>
        internal static string TitleListsPath
        {
            get
            {
                return titleListsPath;
            }
        }

        /// <summary>
        /// The matter configurations list name
        /// </summary>
        internal static readonly string MatterConfigurationsList = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Configurations_List", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Content check flag default value
        /// </summary>
        internal static readonly bool IsContentCheck = Convert.ToBoolean(ConstantStrings.GetConfigurationFromResourceFile("Constants", "Is_Content_Check", Enumerators.ResourceFileLocation.App_GlobalResources), CultureInfo.InvariantCulture);

        /// <summary>
        /// Property value column of Matter Configurations list
        /// </summary>
        internal static readonly string MatterConfigurationColumn = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Configuration_List_Column_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The tenant URL
        /// </summary>
        internal static readonly string SiteURL = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Site_Url", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The Email format for send as email functionality
        /// </summary>
        internal static string SendAsEmailFormat = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Send_As_Email_Format", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The Email CSS style for send as email functionality
        /// </summary>
        internal static string SendAsEmailFontStyle = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Send_As_Email_Font_Style", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the incorrect input code for user access in Provision Matter Group
        /// </summary>
        /// <value>The incorrect input code for user access in Provision Matter Group</value>
        internal static string IncorrectInputUserAccessCode
        {
            get
            {
                return incorrectInputUserAccessCode;
            }
        }

        /// <summary>
        /// Gets the incorrect input message for user access in Provision Matter Group
        /// </summary>
        /// <value>The incorrect input message for user access in Provision Matter Group</value>
        internal static string IncorrectInputUserAccessMessage
        {
            get
            {
                return incorrectInputUserAccessMessage;
            }
        }

        /// <summary>
        /// User's OneDrive document library title
        /// </summary>
        /// <value>User's OneDrive document library title</value>
        internal static string OneDriveDocumentLibraryTitle
        {
            get
            {
                return oneDriveDocumentLibraryTitle;
            }
        }

        /// <summary>
        /// Date format for dates displayed in Matter Center app
        /// </summary>
        /// <value>Date format for dates displayed in Matter Center app</value>
        internal static string MatterCenterDateFormat
        {
            get
            {
                return matterCenterDateFormat;
            }
        }

        /// <summary>
        /// The message for user not present in site owner for a particular client
        /// </summary>
        internal static readonly string UserNotSiteOwnerMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "User_Not_Site_Owner_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The code for user not present in site owner for a particular client
        /// </summary>
        internal static readonly string UserNotSiteOwnerCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "User_Not_Site_Owner_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The message for duplicate matter library
        /// </summary>
        internal static readonly string ErrorDuplicateMatter = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Error_Duplicate_Matter", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The message for duplicate matter landing page
        /// </summary>
        internal static readonly string ErrorDuplicateMatterLandingPage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Error_Duplicate_MatterLandingPage", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The Attachments tag in Exchange response
        /// </summary>
        internal const string AttachmentsTag = "Attachments";

        /// <summary>
        /// The FileAttachment tag in Exchange response
        /// </summary>
        internal const string FileAttachmentTag = "FileAttachment";

        /// <summary>
        /// The ItemAttachment tag in Exchange response
        /// </summary>
        internal const string ItemAttachmentTag = "ItemAttachment";

        /// <summary>
        /// The Name tag in Exchange response
        /// </summary>
        internal const string FileNameTag = "Name";

        /// <summary>
        /// The AttachmentID tag in Exchange response
        /// </summary>
        internal const string AttachmentIdTag = "AttachmentId";

        /// <summary>
        /// The Id attribute in Exchange response
        /// </summary>
        internal const string IdAttribute = "Id";

        /// <summary>
        /// The name of the HTTP header for web request
        /// </summary>
        internal const string RequestHeaderName = "Authorization";

        /// <summary>
        /// The value of the HTTP header for web request
        /// </summary>
        internal const string RequestHeaderValue = "Bearer {0}";

        /// <summary>
        /// The Method for web request
        /// </summary>
        internal const string RequestMethod = "POST";

        /// <summary>
        /// The value of the Content-type HTTP header for web request
        /// </summary>
        internal const string RequestContentType = "text/xml; charset=utf-8";

        /// <summary>
        /// The column name for document GUID
        /// </summary>
        internal const string DocumentGUIDColumnName = "UniqueId";

        /// <summary>
        /// Open square brace
        /// </summary>
        internal const string OpenSquareBrace = "[";

        /// <summary>
        /// Close square brace
        /// </summary>
        internal const string CloseSquareBrace = "]";

        /// <summary>
        /// Name of the path field in results returned from SharePoint search
        /// </summary>
        internal const string PathFieldName = "Path";

        /// <summary>
        /// Double quote
        /// </summary>
        internal const string DoubleQuotes = "\"";

        /// <summary>
        /// String to be used for creating default value for metadata. This string is in following format: WSSID;#VAL|GUID
        /// </summary>
        internal const string MetadataDefaultValue = "{0};#{1}|{2}";

        #region Azure Cache keys
        /// <summary>
        /// Clients cache key
        /// </summary>
        internal const string CACHE_CLIENTS = "Clients";

        /// <summary>
        /// Matter type cache key
        /// </summary>
        internal const string CACHE_MATTER_TYPE = "MatterType";

        /// <summary>
        /// Roles cache key
        /// </summary>
        internal const string CACHE_ROLES = "Roles";
        #endregion
    }
}
