// <copyright file="ServiceConstantStrings.cs" company="Microsoft">
// Microsoft.com. All rights reserved.
// </copyright>
// <author>MAQ Software</author>
#region Page Summary
/// *****************************************************************
///
/// Project:    Matter Center
/// Solution:   Microsoft.Legal.MatterCenter.ProviderService
///
/// Date:    November 28, 2014
/// Description: This file is being used as an adapter between constant resource and service
///
/// Change History:
/// Name            Date            Version         Description
/// -------------------------------------------------------------------------------
/// Jonathan C      April 7, 2014   1.0.0.0         Accessing from resource file for service
/// Shahid A        Nov 28, 2014    1.0.0.0         Added service constants for people picker control
/// Rishikesh J     December 1, 2014   1.0.1.0     Implementation for Red Banner
/// -------------------------------------------------------------------------------
/// Copyright (C) <copyright information of the client>.
/// -------------------------------------------------------------------------------
#endregion

namespace Microsoft.Legal.MatterCenter.ProviderService
{
    #region Using
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
        /// The message no inputs
        /// </summary>
        private static string messageNoInputs = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Message_No_Inputs", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The central repository URL
        /// </summary>
        private static string centralRepositoryUrl = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Central_Repository_Url", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The DMS matter list name
        /// </summary>
        private static string dmsMatterListName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "DMS_Matter_List_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The send mail list name
        /// </summary>
        private static string sendMailListName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Send_Mail_List_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The share list column matter path
        /// </summary>
        private static string shareListColumnMatterPath = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Share_List_Column_Matter_Path", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The share list column mail list
        /// </summary>
        private static string shareListColumnMailList = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Share_List_Column_Mail_List", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The share list column mail list
        /// </summary>
        private static string shareListColumnMailBody = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Share_List_Column_Mail_Body", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The share list column mail list
        /// </summary>
        private static string shareListColumnMailSubject = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Share_List_Column_Mail_Subject", Enumerators.ResourceFileLocation.App_GlobalResources);

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
        /// The managed property responsible attorney
        /// </summary>
        private static string managedPropertyResponsibleAttorney = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Responsible_Attorney", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property team members
        /// </summary>
        private static string managedPropertyTeamMembers = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Team_Members", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property site name
        /// </summary>
        private static string managedPropertySiteName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Site_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property custom title
        /// </summary>
        private static string managedPropertyCustomTitle = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Custom_Title", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property is matter
        /// </summary>
        private static string managedPropertyIsMatter = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Is_Matter", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property title
        /// </summary>
        private static string managedPropertyTitle = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Title", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property name
        /// </summary>
        private static string managedPropertyName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property description
        /// </summary>
        private static string managedPropertyDescription = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Description", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property last modified time
        /// </summary>
        private static string managedPropertyLastModifiedTime = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Last_Modified_Time", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property practice group
        /// </summary>
        private static string managedPropertyPracticeGroup = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Practice_Group", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property area of law
        /// </summary>
        private static string managedPropertyAreaOfLaw = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Area_Of_Law", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property sub area of law
        /// </summary>
        private static string managedPropertySubAreaOfLaw = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Sub_Area_Of_Law", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property matter identifier
        /// </summary>
        private static string managedPropertyMatterId = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Matter_Id", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property path
        /// </summary>
        private static string managedPropertyPath = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Path", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property matter name
        /// </summary>
        private static string managedPropertyMatterName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Matter_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property open date
        /// </summary>
        private static string managedPropertyOpenDate = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Open_Date", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property client name
        /// </summary>
        private static string managedPropertyClientName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Client_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

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
        /// The managed property author
        /// </summary>
        private static string managedPropertyAuthor = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Author", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property author
        /// </summary>
        private static string managedPropertyFileName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_File_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property is document
        /// </summary>
        private static string managedPropertyIsDocument = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Is_Document", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property server relative URL
        /// </summary>
        private static string managedPropertyServerRelativeUrl = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Server_Relative_Url", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property file extension
        /// </summary>
        private static string managedPropertyFileExtension = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_File_Extension", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property created
        /// </summary>
        private static string managedPropertyCreated = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Created", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property UI version string OWSTEXT
        /// </summary>
        private static string managedPropertyUIVersionStringOWSTEXT = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_UI_Version_String_OWSTEXT", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property site title
        /// </summary>
        private static string managedPropertySiteTitle = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Site_Title", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property document client identifier
        /// </summary>
        private static string managedPropertyDocumentClientId = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_Client_Id", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property document client name
        /// </summary>
        private static string managedPropertyDocumentClientName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_Client_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property document matter identifier
        /// </summary>
        private static string managedPropertyDocumentMatterId = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_Matter_Id", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property document matter name
        /// </summary>
        private static string managedPropertyDocumentMatterName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_Matter_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property document identifier
        /// </summary>
        private static string managedPropertyDocumentId = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_Id", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property checked out by user
        /// </summary>
        private static string managedPropertyCheckOutByUser = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_CheckOut_By_User", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The user pinned document list name
        /// </summary>
        private static string userPinnedDocumentListName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "User_Pinned_Document_List_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The user saved search list name
        /// </summary>
        private static string userSavedSearchListName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "User_Saved_Search_List_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

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
        /// The column name saved matters details
        /// </summary>
        private static string columnNameSavedMattersDetails = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Column_Name_Saved_Matters_Details", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The column name saved documents details
        /// </summary>
        private static string columnNameSavedDocumentsDetails = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Column_Name_Saved_Documents_Details", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The column name user alias
        /// </summary>
        private static string columnNameUserAlias = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Column_Name_User_Alias", Enumerators.ResourceFileLocation.App_GlobalResources);

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
        /// The managed property blocked upload users
        /// </summary>
        private static string managedPropertyBlockedUploadUsers = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Blocked_Upload_Users", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The find document valid extensions
        /// </summary>
        private static string findDocumentInvalidExtensions = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Find_Document_Invalid_Extensions", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Pin image location
        /// </summary>
        private static string pinImageLocation = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Pin_Image_Url", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Unpin image location
        /// </summary>
        private static string unpinImageLocation = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Unpin_Image_Url", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// One Note image location
        /// </summary>
        private static string oneNoteImageLocation = ConstantStrings.GetConfigurationFromResourceFile("Constants", "OneNote_Image_Url", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Share image location
        /// </summary>
        private static string shareImageLocation = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Share_Image_Url", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Matter Landing page image location
        /// </summary>
        private static string matterLandingPageLogo = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Landing_Page_Icon", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Tenant level Web Dashboard location
        /// </summary>
        private static string tenantWebDashboardLink = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Tenant_WebDashboard_Link", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Microsoft Logo publish location
        /// </summary>
        private static string microsoftImageLocation = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Microsoft_Logo_Location", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// HTML chunk of the Default content type for Matter creation mail
        /// </summary>
        private static string matterMailDefaultContentTypeHtmlChunk = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Mail_Default_Content_Type_Html_Chunk", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Subject of the matter creation mail
        /// </summary>
        private static string matterMailSubject = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Mail_Subject", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The valid date format
        /// </summary>
        private static string validDateFormat = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Valid_Date_Format", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Suffix to be appended in calendar name
        /// </summary>
        private static string calendarNameSuffix = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Calendar_Name_Suffix", Enumerators.ResourceFileLocation.App_GlobalResources);


        /// <summary>
        /// The manage property Client ID
        /// </summary>
        private static string managedPropertyClientID = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Client_Id", Enumerators.ResourceFileLocation.App_GlobalResources);

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
        /// Incorrect Request Object message
        /// </summary>
        private static string incorrectInputRequestObjectMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Request_Object_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Request Object code
        /// </summary>
        private static string incorrectInputRequestObjectCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Request_Object_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Client Url message
        /// </summary>
        private static string incorrectInputClientUrlMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Client_Url_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Client Url code
        /// </summary>
        private static string incorrectInputClientUrlCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Client_Url_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Client Id message
        /// </summary>
        private static string incorrectInputClientIdMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Client_Id_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Client Id code
        /// </summary>
        private static string incorrectInputClientIdCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Client_Id_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Client name message
        /// </summary>
        private static string incorrectInputClientNameMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Client_Name_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Client name code
        /// </summary>
        private static string incorrectInputClientNameCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Client_Name_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Practice Group message
        /// </summary>
        private static string incorrectInputPracticeGroupMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Practice_Group_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Practice Group code
        /// </summary>
        private static string incorrectInputPracticeGroupCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Practice_Group_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Area Of Law message
        /// </summary>
        private static string incorrectInputAreaOfLawMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Area_Of_Law_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Area Of Law code
        /// </summary>
        private static string incorrectInputAreaOfLawCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Area_Of_Law_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Sub Area Of Law message
        /// </summary>
        private static string incorrectInputSubareaOfLawMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Subarea_Of_Law_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Sub Area Of Law code
        /// </summary>
        private static string incorrectInputSubareaOfLawCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Subarea_Of_Law_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Responsible Attorney message
        /// </summary>
        private static string incorrectInputResponsibleAttorneyMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Responsible_Attorney_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Responsible Attorney code
        /// </summary>
        private static string incorrectInputResponsibleAttorneyCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Responsible_Attorney_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Matter name message
        /// </summary>
        private static string incorrectInputMatterNameMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Matter_Name_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Matter name code
        /// </summary>
        private static string incorrectInputMatterNameCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Matter_Name_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Matter Id message 
        /// </summary>
        private static string incorrectInputMatterIdMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Matter_Id_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Matter Id code
        /// </summary>
        private static string incorrectInputMatterIdCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Matter_Id_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect user name message
        /// </summary>
        private static string incorrectInputUserNamesMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Names_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect user name code
        /// </summary>
        private static string incorrectInputUserNamesCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Names_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect user permission message
        /// </summary>
        private static string incorrectInputUserPermissionsMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Permissions_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect user permission code
        /// </summary>
        private static string incorrectInputUserPermissionsCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Permissions_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Content Type message
        /// </summary>
        private static string incorrectInputContentTypeMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Content_Type_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Content Type code
        /// </summary>
        private static string incorrectInputContentTypeCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Content_Type_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Matter description message
        /// </summary>
        private static string incorrectInputMatterDescriptionMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Matter_Description_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Matter description code
        /// </summary>
        private static string incorrectInputMatterDescriptionCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Matter_Description_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect conflict date message
        /// </summary>
        private static string incorrectInputConflictDateMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Conflict_Date_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect conflict date code
        /// </summary>
        private static string incorrectInputConflictDateCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Conflict_Date_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect conflict identified message
        /// </summary>
        private static string incorrectInputConflictIdentifiedMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Conflict_Identified_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect conflict identified code
        /// </summary>
        private static string incorrectInputConflictIdentifiedCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Conflict_Identified_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect user roles message
        /// </summary>
        private static string incorrectInputUserRolesMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Roles_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect user roles code
        /// </summary>
        private static string incorrectInputUserRolesCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Roles_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect conflict check by message
        /// </summary>
        private static string incorrectInputConflictCheckByMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Conflict_Check_By_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect conflict check by code
        /// </summary>
        private static string incorrectInputConflictCheckByCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Conflict_Check_By_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect block users message
        /// </summary>
        private static string incorrectInputBlockUserNamesMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Block_User_Names_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect block users code
        /// </summary>
        private static string incorrectInputBlockUserNamesCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Block_User_Names_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The Is Read Only User key name, used to determine if user is read-only user for particular matter
        /// </summary>
        private static string isReadOnlyUser = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Is_Read_Only_User", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect save search name message
        /// </summary>
        private static string incorrectInputSaveSearchNameMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Save_Search_Name_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect save search name code
        /// </summary>
        private static string incorrectInputSaveSearchNameCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Save_Search_Name_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect existing search name message
        /// </summary>
        private static string incorrectInputExistingSearchNameMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Existing_Search_Name_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect existing search name code
        /// </summary>
        private static string incorrectInputExistingSearchNameCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Existing_Search_Name_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect current search name message
        /// </summary>
        private static string incorrectInputCurrentSearchNameMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Current_Search_Name_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect current search name code
        /// </summary>
        private static string incorrectInputCurrentSearchNameCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Current_Search_Name_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect input code for user access in Provision Matter Group
        /// </summary>
        private static string incorrectInputUserAccessCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Access_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect input code for user access in Provision Matter Group
        /// </summary>
        private static string incorrectInputUserAccessMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Access_Message", Enumerators.ResourceFileLocation.App_GlobalResources);
        

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
        private static string mailSoapReuqest =
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
        /// XML definition of the left web part
        /// </summary>
        private static string leftBarWebPart = @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <WebPart xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/WebPart/v2"">
                      <Title>Content Editor</Title>
                      <FrameType>None</FrameType>
                      <Description>Allows authors to enter rich text content.</Description>
                      <IsIncluded>true</IsIncluded>
                      <ZoneID>MiddleLeftZone</ZoneID>
                      <PartOrder>0</PartOrder>
                      <FrameState>Normal</FrameState>
                      <Height />
                      <Width>200px</Width>
                      <AllowRemove>true</AllowRemove>
                      <AllowZoneChange>true</AllowZoneChange>
                      <AllowMinimize>true</AllowMinimize>
                      <AllowConnect>true</AllowConnect>
                      <AllowEdit>true</AllowEdit>
                      <AllowHide>true</AllowHide>
                      <IsVisible>true</IsVisible>
                      <DetailLink />
                      <HelpLink />
                      <HelpMode>Modeless</HelpMode>
                      <Dir>Default</Dir>
                      <PartImageSmall />
                      <MissingAssembly>Cannot import this Web Part.</MissingAssembly>
                      <PartImageLarge>/_layouts/15/images/mscontl.gif</PartImageLarge>
                      <IsIncludedFilter />
                      <Assembly>Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>
                      <TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>
                      <ContentLink xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                      <Content xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"">
                        <![CDATA[
                            <div id=""LeftNavHeader""></div> 
                          ]]>
                      </Content>
                      <PartStorage xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                    </WebPart>";

        /// <summary>
        /// XML definition of the template web part
        /// </summary>
        private static string templateWebPart = @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <WebPart xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/WebPart/v2"">
                      <Title>Content Editor</Title>
                      <FrameType>None</FrameType>
                      <Description>Allows authors to enter rich text content.</Description>
                      <IsIncluded>true</IsIncluded>
                      <ZoneID>MiddleLeftZone</ZoneID>
                      <PartOrder>0</PartOrder>
                      <FrameState>Normal</FrameState>
                      <Height />
                      <Width>200px</Width>
                      <AllowRemove>true</AllowRemove>
                      <AllowZoneChange>true</AllowZoneChange>
                      <AllowMinimize>true</AllowMinimize>
                      <AllowConnect>true</AllowConnect>
                      <AllowEdit>true</AllowEdit>
                      <AllowHide>true</AllowHide>
                      <IsVisible>true</IsVisible>
                      <DetailLink />
                      <HelpLink />
                      <HelpMode>Modeless</HelpMode>
                      <Dir>Default</Dir>
                      <PartImageSmall />
                      <MissingAssembly>Cannot import this Web Part.</MissingAssembly>
                      <PartImageLarge>/_layouts/15/images/mscontl.gif</PartImageLarge>
                      <IsIncludedFilter />
                      <Assembly>Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>
                      <TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>
                      <ContentLink xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                      <Content xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"">
                        <![CDATA[
                            <div class=""TemplatesDiv"">
                                Templates
                            </div>
                            <div class=""TemplateText"">
                                Looking for Templates?
                                <a href=""{0}"" class=""TemplateLink"">
                                    click here
                                </a>
                            </div>
                          ]]>
                      </Content>
                      <PartStorage xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                    </WebPart>";

        /// <summary>
        /// XML definition of the top menu web part
        /// </summary>
        private static string topMenuWebPart = @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <WebPart xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/WebPart/v2"">
                      <Title>Content Editor</Title>
                      <FrameType>None</FrameType>
                      <Description>Allows authors to enter rich text content.</Description>
                      <IsIncluded>true</IsIncluded>
                      <ZoneID>MiddleMiddleZone</ZoneID>
                      <PartOrder>0</PartOrder>
                      <FrameState>Normal</FrameState>
                      <Height />
                      <Width />
                      <AllowRemove>true</AllowRemove>
                      <AllowZoneChange>true</AllowZoneChange>
                      <AllowMinimize>true</AllowMinimize>
                      <AllowConnect>true</AllowConnect>
                      <AllowEdit>true</AllowEdit>
                      <AllowHide>true</AllowHide>
                      <IsVisible>true</IsVisible>
                      <DetailLink />
                      <HelpLink />
                      <HelpMode>Modeless</HelpMode>
                      <Dir>Default</Dir>
                      <PartImageSmall />
                      <MissingAssembly>Cannot import this Web Part.</MissingAssembly>
                      <PartImageLarge>/_layouts/15/images/mscontl.gif</PartImageLarge>
                      <IsIncludedFilter />
                      <Assembly>Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>
                      <TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>
                      <ContentLink xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                      <Content xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"">
                        <![CDATA[
                            <div id=""FunctionalityHeader"">
                                <div id=""OneNote""><a href=""javascript:void(0)"" data-href=""{0}"" id=""GoToOneNote""><img src=""{5}"" alt=""Loading"" />Loading</a></div>
                                <div id=""PinMatter"" onclick='pinUnpinMatter()'><img src = ""{6}"" id = ""pinImg"" alt=""Pin""/><img id=""unPinImg"" src= ""{7}"" alt=""Unpin"" />Pin</div>
                                <div id=""Share"" onclick='showPopup(""{1}"", ""{2}"")'><img src= ""{8}"" alt=""Share"" />Share</div>
                            </div> 
                        <script type=""text/javascript"">
                        var oGlobalConstants = {{}};
                        oGlobalConstants.sCatalogSite = ""{3}"";
                        </script>
                            <script src=""{4}"" type=""text/javascript""></script>                            
                            ]]>
                        </Content>
                      <PartStorage xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                    </WebPart>";

        /// <summary>
        /// XML definition of the footer web part
        /// </summary>
        private static string footerWebPart = @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <WebPart xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/WebPart/v2"">
                      <Title>Content Editor</Title>
                      <FrameType>None</FrameType>
                      <Description>Allows authors to enter rich text content.</Description>
                      <IsIncluded>true</IsIncluded>
                      <ZoneID>FooterZone</ZoneID>
                      <PartOrder>0</PartOrder>
                      <FrameState>Normal</FrameState>
                      <Height />
                      <Width />
                      <AllowRemove>true</AllowRemove>
                      <AllowZoneChange>true</AllowZoneChange>
                      <AllowMinimize>true</AllowMinimize>
                      <AllowConnect>true</AllowConnect>
                      <AllowEdit>true</AllowEdit>
                      <AllowHide>true</AllowHide>
                      <IsVisible>true</IsVisible>
                      <DetailLink />
                      <HelpLink />
                      <HelpMode>Modeless</HelpMode>
                      <Dir>Default</Dir>
                      <PartImageSmall />
                      <MissingAssembly>Cannot import this Web Part.</MissingAssembly>
                      <PartImageLarge>/_layouts/15/images/mscontl.gif</PartImageLarge>
                      <IsIncludedFilter />
                      <Assembly>Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>
                      <TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>
                      <ContentLink xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                      <Content xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor""><![CDATA[
                    <div id=""mainDivContainer""> 
                       <div id=""contentDiv""> 
                          <footer id=""footer"">
                             <div>
                                <div class=""rightFooter"">
                                   <ul>
                                      <li class=""rightend""> 
                                         <a href=""http://www.microsoft.com/en-in/default.aspx"" target=""_blank"">
                                            <img title=""Microsoft"" class=""mslogo"" alt=""Microsoft"" src=""{0}""/></a></li>
                                   </ul>
                                </div>
                                <div class=""leftFooter"">
                                   <ul>
                                      <li class=""leftFooterElement""> 
                                         <a id=""feedbackandsupport"" href=""mailto:LCAHelp@microsoft.com"" target=""_blank"">Feedback &amp; Support</a></li>
                                      <li class=""leftFooterElement""> 
                                         <a href=""http://go.microsoft.com/fwlink/?LinkId=248681"" target=""_blank"">Privacy</a></li>
                                      <li class=""leftFooterElement""> 
                                         <a href=""http://www.microsoft.com/en-us/legal/intellectualproperty/copyright/default.aspx"" target=""_blank"">Terms of use</a></li>
                                      <li class=""leftFooterElement"">© 2014 Microsoft</li>
                                   </ul>
                                </div>
                             </div> </footer>​​​​​
                    </div> 
                    </div> ]]></Content>
                      <PartStorage xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                    </WebPart>";

        /// <summary>
        /// XML definition of the header web part
        /// </summary>
        private static string headerWebPart = @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <WebPart xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/WebPart/v2"">
                      <Title>Content Editor</Title>
                      <FrameType>None</FrameType>
                      <Description>Allows authors to enter rich text content.</Description>
                      <IsIncluded>true</IsIncluded>
                      <ZoneID>HeaderZone</ZoneID>
                      <PartOrder>0</PartOrder>
                      <FrameState>Normal</FrameState>
                      <Height />
                      <Width />
                      <AllowRemove>true</AllowRemove>
                      <AllowZoneChange>true</AllowZoneChange>
                      <AllowMinimize>true</AllowMinimize>
                      <AllowConnect>true</AllowConnect>
                      <AllowEdit>true</AllowEdit>
                      <AllowHide>true</AllowHide>
                      <IsVisible>true</IsVisible>
                      <DetailLink />
                      <HelpLink />
                      <HelpMode>Modeless</HelpMode>
                      <Dir>Default</Dir>
                      <PartImageSmall />
                      <MissingAssembly>Cannot import this Web Part.</MissingAssembly>
                      <PartImageLarge>/_layouts/15/images/mscontl.gif</PartImageLarge>
                      <IsIncludedFilter />
                      <Assembly>Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>
                      <TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>
                      <ContentLink xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                      <Content xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"">
                      <![CDATA[                        
                        <link href=""{0}"" rel=""stylesheet"" type=""text/css"" />                                              
                            <div class=""Container"">
                                <div id=""IconBar"">
                                        <a href=""{1}""><img id=""Logo"" src=""{2}"" alt=""Matter Center"" title=""Matter Center""/></a>                                        
										<a href=""{1}""><span id=""AppTitle"">Matter Center <span id=""ExtraText"">for Office 365</span> </span></a>
                                    </div>
                                </div>                                              
                      ]]>
                      </Content>
                      <PartStorage xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                    </WebPart>
                    ";

        /// <summary>
        /// XML definition of the html data
        /// </summary>
        private static string htmlData = @"<div class=""allUsers"">
                    <ul>
                    <li id=""assign"" class=""users"">Team</li>
                    <li id=""block"" class=""users"">Blocked users</li>
                    </ul> 
                    <div id=""userDetails""> </div> <div id=""blockUserDetails""> </div> </div>
                    <script type=""text/javascript"" src=""{4}"" ></script>
                    <script type=""text/javascript"">
                        var user= ""{0}"" ;
                        var userEmail = ""{1}"" ;
                        var blockUser = ""{2}"" ;
                        var blockUserEmail = ""{3}"" ;
                    </script>";

        /// <summary>
        /// Html chunk to be added on the page
        /// </summary>
        private static string htmlChunk = @"<script type=""text/javascript"" src=""{4}"" ></script> <script type=""text/javascript""> $.getScript(""{3}"", function() {{  getMetaDataProperties(""{0}"", ""{1}"", ""{2}""); }}); </script>  <div id=""{2}""> </div> <br/>";

        /// <summary>
        /// XML definition of the RSS feed web part
        /// </summary>
        private static string rssFeedWebpart = @"<webParts>
                  <webPart xmlns=""http://schemas.microsoft.com/WebPart/v3"">
                    <metaData>
                      <type name=""Microsoft.SharePoint.Portal.WebControls.RSSAggregatorWebPart, Microsoft.SharePoint.Portal, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"" />
                      <importErrorMessage>Cannot import this web part.</importErrorMessage>
                    </metaData>
                    <data>
                      <properties>
                        <property name=""InitialAsyncDataFetch"" type=""bool"">False</property>
                        <property name=""ChromeType"" type=""chrometype"">None</property>
                        <property name=""ListName"" type=""string"" null=""true"" />
                        <property name=""Height"" type=""string"" />
                        <property name=""CacheXslStorage"" type=""bool"">True</property>
                        <property name=""Default"" type=""string"" />
                        <property name=""ParameterBindings"" type=""string"">&lt;ParameterBinding Name=""RequestUrl"" Location=""WPProperty[FeedUrl]""/&gt;</property>
                        <property name=""AllowZoneChange"" type=""bool"">True</property>
                        <property name=""AutoRefresh"" type=""bool"">False</property>
                        <property name=""XmlDefinitionLink"" type=""string"" />
                        <property name=""DataFields"" type=""string"" />
                        <property name=""FeedLimit"" type=""int"">5</property>
                        <property name=""Hidden"" type=""bool"">False</property>
                        <property name=""NoDefaultStyle"" type=""string"" />
                        <property name=""XslLink"" type=""string"" null=""true"" />
                        <property name=""ViewFlag"" type=""string"">0</property>
                        <property name=""CatalogIconImageUrl"" type=""string"" />
                        <property name=""CacheXslTimeOut"" type=""int"">600</property>
                        <property name=""AutoRefreshInterval"" type=""int"">60</property>
                        <property name=""AllowConnect"" type=""bool"">True</property>
                        <property name=""FeedUrl"" type=""string"">http://www.bing.com/search?q={0}&amp;format=rss</property>
                        <property name=""AllowClose"" type=""bool"">True</property>
                        <property name=""ShowWithSampleData"" type=""bool"">False</property>       
                        <property name=""EnableOriginalValue"" type=""bool"">False</property>
                        <property name=""ExpandFeed"" type=""bool"">False</property>
                        <property name=""ListUrl"" type=""string"" null=""true"" />
                        <property name=""DataSourceID"" type=""string"" />
                        <property name=""FireInitialRow"" type=""bool"">True</property>
                        <property name=""ManualRefresh"" type=""bool"">False</property>
                        <property name=""ViewFlags"" type=""Microsoft.SharePoint.SPViewFlags, Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"">None</property>
                        <property name=""ChromeState"" type=""chromestate"">Normal</property>
                        <property name=""AllowHide"" type=""bool"">True</property>
                        <property name=""ListDisplayName"" type=""string"" null=""true"" />
                        <property name=""SampleData"" type=""string"" null=""true"" />
                        <property name=""AsyncRefresh"" type=""bool"">False</property>
                        <property name=""Direction"" type=""direction"">NotSet</property>
                        <property name=""Title"" type=""string"">RSS Viewer</property>
                        <property name=""ListId"" type=""System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"">00000000-0000-0000-0000-000000000000</property>
                        <property name=""Description"" type=""string"">Displays an RSS feed.</property>
                        <property name=""AllowMinimize"" type=""bool"">True</property>
                        <property name=""TitleUrl"" type=""string"" />
                        <property name=""DataSourcesString"" type=""string"">
                        &lt;%@ Register TagPrefix=""WebControls"" Namespace=""Microsoft.SharePoint.WebControls"" Assembly=""Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"" %&gt;
                        &lt;%@ Register TagPrefix=""WebPartPages"" Namespace=""Microsoft.SharePoint.WebPartPages"" Assembly=""Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"" %&gt;
                        &lt;WebControls:XmlUrlDataSource runat=""server"" AuthType=""None"" HttpMethod=""GET""&gt;
                        &lt;DataFileParameters&gt;
                        &lt;WebPartPages:DataFormParameter Name=""RequestUrl"" ParameterKey=""RequestUrl"" PropertyName=""ParameterValues""/&gt;
                        &lt;/DataFileParameters&gt;
                        &lt;/WebControls:XmlUrlDataSource&gt;</property>
                        <property name=""DisplayName"" type=""string"" />
                        <property name=""PageType"" type=""Microsoft.SharePoint.PAGETYPE, Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"">PAGE_NORMALVIEW</property>
                        <property name=""Width"" type=""string"" />
                        <property name=""AllowEdit"" type=""bool"">True</property>
                        <property name=""ExportMode"" type=""exportmode"">All</property>
                        <property name=""CacheRefreshTimeInMins"" type=""int"">120</property>
                        <property name=""PageSize"" type=""int"">-1</property>
                        <property name=""ViewContentTypeId"" type=""string"" />
                        <property name=""HelpUrl"" type=""string"" />
                        <property name=""XmlDefinition"" type=""string"" />
                        <property name=""UseSQLDataSourcePaging"" type=""bool"">True</property>
                        <property name=""TitleIconImageUrl"" type=""string"" />
                        <property name=""MissingAssembly"" type=""string"">Cannot import this web part.</property>
                        <property name=""HelpMode"" type=""helpmode"">Modeless</property>
                      </properties>
                    </data>
                  </webPart>
                </webParts>";

        /// <summary>
        /// XML definition of the calendar web part
        /// </summary>
        private static string calendarWebpart = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <WebPart xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/WebPart/v2"">
                  <Title>Calendar</Title>
                  <FrameType>None</FrameType>
                  <Description />
                  <IsIncluded>true</IsIncluded>
                  <ZoneID>wpz</ZoneID>
                  <PartOrder>5</PartOrder>
                  <FrameState>Normal</FrameState>
                  <Height />
                  <Width />
                  <AllowRemove>true</AllowRemove>
                  <AllowZoneChange>true</AllowZoneChange>
                  <AllowMinimize>true</AllowMinimize>
                  <AllowConnect>true</AllowConnect>
                  <AllowEdit>true</AllowEdit>
                  <AllowHide>true</AllowHide>
                  <IsVisible>true</IsVisible>
                  <DetailLink></DetailLink>
                  <HelpLink />
                  <HelpMode>Modeless</HelpMode>
                  <Dir>Default</Dir>
                  <PartImageSmall />
                  <MissingAssembly>Cannot import this Web Part.</MissingAssembly>
                  <PartImageLarge>/_layouts/15/images/itevent.png?rev=35</PartImageLarge>
                  <IsIncludedFilter />
                  <Assembly>Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>
                  <TypeName>Microsoft.SharePoint.WebPartPages.ListViewWebPart</TypeName>
                  <WebId xmlns=""http://schemas.microsoft.com/WebPart/v2/ListView"">00000000-0000-0000-0000-000000000000</WebId>
                  <ListViewXml xmlns=""http://schemas.microsoft.com/WebPart/v2/ListView"">&lt;View Name=""{0}"" MobileView=""TRUE"" Type=""CALENDAR"" Hidden=""TRUE"" TabularView=""FALSE"" RecurrenceRowset=""TRUE"" DisplayName="" "" Url=""{1}"" Level=""1"" BaseViewID=""2"" ContentTypeID=""0x"" MobileUrl=""_layouts/15/mobile/viewdaily.aspx"" ImageUrl=""/_layouts/15/images/events.png?rev=37""&gt;&lt;Toolbar Type=""None"" /&gt;&lt;ViewHeader /&gt;&lt;ViewBody /&gt;&lt;ViewFooter /&gt;&lt;ViewEmpty /&gt;&lt;ParameterBindings&gt;&lt;ParameterBinding Name=""NoAnnouncements"" Location=""Resource(wss,noXinviewofY_LIST)"" /&gt;&lt;ParameterBinding Name=""NoAnnouncementsHowTo"" Location=""Resource(wss,noXinviewofY_DEFAULT)"" /&gt;&lt;/ParameterBindings&gt;&lt;ViewFields&gt;&lt;FieldRef Name=""EventDate"" /&gt;&lt;FieldRef Name=""EndDate"" /&gt;&lt;FieldRef Name=""fRecurrence"" /&gt;&lt;FieldRef Name=""EventType"" /&gt;&lt;FieldRef Name=""WorkspaceLink"" /&gt;&lt;FieldRef Name=""Title"" /&gt;&lt;FieldRef Name=""Location"" /&gt;&lt;FieldRef Name=""Description"" /&gt;&lt;FieldRef Name=""Workspace"" /&gt;&lt;FieldRef Name=""MasterSeriesItemID"" /&gt;&lt;FieldRef Name=""fAllDayEvent"" /&gt;&lt;/ViewFields&gt;&lt;ViewData&gt;&lt;FieldRef Name=""Title"" Type=""CalendarMonthTitle"" /&gt;&lt;FieldRef Name=""Title"" Type=""CalendarWeekTitle"" /&gt;&lt;FieldRef Name=""Location"" Type=""CalendarWeekLocation"" /&gt;&lt;FieldRef Name=""Title"" Type=""CalendarDayTitle"" /&gt;&lt;FieldRef Name=""Location"" Type=""CalendarDayLocation"" /&gt;&lt;/ViewData&gt;&lt;Query&gt;&lt;Where&gt;&lt;DateRangesOverlap&gt;&lt;FieldRef Name=""EventDate"" /&gt;&lt;FieldRef Name=""EndDate"" /&gt;&lt;FieldRef Name=""RecurrenceID"" /&gt;&lt;Value Type=""DateTime""&gt;&lt;Month /&gt;&lt;/Value&gt;&lt;/DateRangesOverlap&gt;&lt;/Where&gt;&lt;/Query&gt;&lt;/View&gt;</ListViewXml>
                  <ListName xmlns=""http://schemas.microsoft.com/WebPart/v2/ListView"">{2}</ListName>
                  <ListId xmlns=""http://schemas.microsoft.com/WebPart/v2/ListView"">{3}</ListId>
                  <ViewFlag xmlns=""http://schemas.microsoft.com/WebPart/v2/ListView"">8921097</ViewFlag>
                  <ViewFlags xmlns=""http://schemas.microsoft.com/WebPart/v2/ListView"">Html Hidden RecurrenceRowset Calendar Mobile</ViewFlags>
                  <ViewContentTypeId xmlns=""http://schemas.microsoft.com/WebPart/v2/ListView"">0x</ViewContentTypeId>
                </WebPart>";

        /// <summary>
        /// XML definition of the list view web part
        /// </summary>
        private static string listviewWebpart = @"
                <webParts>
                    <webPart xmlns=""http://schemas.microsoft.com/WebPart/v3"">
                        <metaData>
                            <type name=""Microsoft.SharePoint.WebPartPages.XsltListViewWebPart, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"" />
                            <importErrorMessage>Cannot import this Web Part.</importErrorMessage>
                        </metaData>
                        <data>
                            <properties>
                                <property name=""ShowWithSampleData"" type=""bool"">False</property>
                                <property name=""Default"" type=""string"" />
                                <property name=""NoDefaultStyle"" type=""string"" null=""true"" />
                                <property name=""CacheXslStorage"" type=""bool"">True</property>
                                <property name=""ViewContentTypeId"" type=""string"" />
                                <property name=""XmlDefinitionLink"" type=""string"" />
                                <property name=""ManualRefresh"" type=""bool"">False</property>
                                <property name=""ListUrl"" type=""string"" />
                                <property name=""ListId"" type=""System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"">{0}</property>
                                <property name=""TitleUrl"" type=""string"">{1}</property>
                                <property name=""EnableOriginalValue"" type=""bool"">False</property>
                                <property name=""Direction"" type=""direction"">NotSet</property>
                                <property name=""ServerRender"" type=""bool"">False</property>
                                <property name=""ViewFlags"" type=""Microsoft.SharePoint.SPViewFlags, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"">Html, TabularView, Hidden, Mobile</property>
                                <property name=""AllowConnect"" type=""bool"">True</property>
                                <property name=""ListName"" type=""string"">{2}</property>
                                <property name=""ListDisplayName"" type=""string"" />
                                <property name=""AllowZoneChange"" type=""bool"">True</property>
                                <property name=""ChromeState"" type=""chromestate"">Normal</property>
                                <property name=""DisableSaveAsNewViewButton"" type=""bool"">False</property>
                                <property name=""ViewFlag"" type=""string"" />
                                <property name=""DataSourceID"" type=""string"" />
                                <property name=""ExportMode"" type=""exportmode"">All</property>
                                <property name=""AutoRefresh"" type=""bool"">False</property>
                                <property name=""FireInitialRow"" type=""bool"">True</property>
                                <property name=""AllowEdit"" type=""bool"">True</property>
                                <property name=""Description"" type=""string"" />
                                <property name=""HelpMode"" type=""helpmode"">Modeless</property>
                                <property name=""BaseXsltHashKey"" type=""string"" null=""true"" />
                                <property name=""AllowMinimize"" type=""bool"">True</property>
                                <property name=""CacheXslTimeOut"" type=""int"">86400</property>
                                <property name=""ChromeType"" type=""chrometype"">None</property>
                                <property name=""Xsl"" type=""string"" null=""true"" />
                                <property name=""JSLink"" type=""string"" null=""true"" />
                                <property name=""CatalogIconImageUrl"" type=""string"">/_layouts/15/images/itdl.png?rev=33</property>
                                <property name=""SampleData"" type=""string"" null=""true"" />
                                <property name=""UseSQLDataSourcePaging"" type=""bool"">True</property>
                                <property name=""TitleIconImageUrl"" type=""string"" />
                                <property name=""PageSize"" type=""int"">-1</property>
                                <property name=""ShowTimelineIfAvailable"" type=""bool"">True</property>
                                <property name=""Width"" type=""string"" />
                                <property name=""DataFields"" type=""string"" />
                                <property name=""Hidden"" type=""bool"">False</property>
                                <property name=""Title"" type=""string"" />
                                <property name=""PageType"" type=""Microsoft.SharePoint.PAGETYPE, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"">PAGE_NORMALVIEW</property>
                                <property name=""DataSourcesString"" type=""string"" />
                                <property name=""AllowClose"" type=""bool"">True</property>
                                <property name=""InplaceSearchEnabled"" type=""bool"">False</property>
                                <property name=""WebId"" type=""System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"">00000000-0000-0000-0000-000000000000</property>
                                <property name=""Height"" type=""string"" />
                                <property name=""GhostedXslLink"" type=""string"">main.xsl</property>
                                <property name=""DisableViewSelectorMenu"" type=""bool"">False</property>
                                <property name=""DisplayName"" type=""string"" />
                                <property name=""IsClientRender"" type=""bool"">False</property>
                                <property name=""XmlDefinition"" type=""string"">&lt;View Name=""{3}"" MobileView=""TRUE"" Type=""HTML"" Hidden=""TRUE"" DisplayName="" "" Url=""{4}"" Level=""1"" BaseViewID=""1"" ContentTypeID=""0x"" ImageUrl=""/_layouts/15/images/dlicon.png?rev=37"" &gt;&lt;Query&gt;&lt;OrderBy&gt;&lt;FieldRef Name=""FileLeafRef""/&gt;&lt;/OrderBy&gt;&lt;/Query&gt;&lt;ViewFields&gt;&lt;FieldRef Name=""DocIcon""/&gt;&lt;FieldRef Name=""LinkFilename""/&gt;&lt;FieldRef Name=""Modified""/&gt;&lt;FieldRef Name=""Editor""/&gt;&lt;/ViewFields&gt;&lt;RowLimit Paged=""TRUE""&gt;30&lt;/RowLimit&gt;&lt;JSLink&gt;clienttemplates.js&lt;/JSLink&gt;&lt;XslLink Default=""TRUE""&gt;main.xsl&lt;/XslLink&gt;&lt;Toolbar Type=""Standard""/&gt;&lt;/View&gt;</property>
                                <property name=""InitialAsyncDataFetch"" type=""bool"">False</property>
                                <property name=""AllowHide"" type=""bool"">True</property>
                                <property name=""ParameterBindings"" type=""string"">
                                    &lt;ParameterBinding Name=""dvt_sortdir"" Location=""Postback;Connection""/&gt;
                                    &lt;ParameterBinding Name=""dvt_sortfield"" Location=""Postback;Connection""/&gt;
                                    &lt;ParameterBinding Name=""dvt_startposition"" Location=""Postback"" DefaultValue="" ""/&gt;
                                    &lt;ParameterBinding Name=""dvt_firstrow"" Location=""Postback;Connection""/&gt;
                                    &lt;ParameterBinding Name=""OpenMenuKeyAccessible"" Location=""Resource(wss,OpenMenuKeyAccessible)"" /&gt;
                                    &lt;ParameterBinding Name=""open_menu"" Location=""Resource(wss,open_menu)"" /&gt;
                                    &lt;ParameterBinding Name=""select_deselect_all"" Location=""Resource(wss,select_deselect_all)"" /&gt;
                                    &lt;ParameterBinding Name=""idPresEnabled"" Location=""Resource(wss,idPresEnabled)"" /&gt;&lt;ParameterBinding Name=""NoAnnouncements"" Location=""Resource(wss,noitemsinview_doclibrary)"" /&gt;&lt;ParameterBinding Name=""NoAnnouncementsHowTo"" Location=""Resource(wss,noitemsinview_doclibrary_howto2)"" /&gt;</property>
                                <property name=""DataSourceMode"" type=""Microsoft.SharePoint.WebControls.SPDataSourceMode, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"">List</property>
                                <property name=""AutoRefreshInterval"" type=""int"">60</property>
                                <property name=""AsyncRefresh"" type=""bool"">False</property>
                                <property name=""HelpUrl"" type=""string"" />
                                <property name=""MissingAssembly"" type=""string"">Cannot import this Web Part.</property>
                                <property name=""XslLink"" type=""string"" null=""true"" />
                                <property name=""SelectParameters"" type=""string"" />
                                <property name=""HasClientDataSource"" type=""bool"">False</property>
                            </properties>
                        </data>
                    </webPart>
                </webParts>";

        /// <summary>
        /// XML definition of the content editor web part for users
        /// </summary>
        private static string contentEditorWebpartUsers = @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <WebPart xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/WebPart/v2"">
                        <Title>User Details</Title>
                        <FrameType>None</FrameType>
                        <Description>Allows authors to enter rich text content.</Description>
                        <IsIncluded>true</IsIncluded>
                        <ZoneID>wpz</ZoneID>
                        <PartOrder>0</PartOrder>
                        <FrameState>Normal</FrameState>
                        <Height />
                        <Width />
                        <AllowRemove>true</AllowRemove>
                        <AllowZoneChange>true</AllowZoneChange>
                        <AllowMinimize>true</AllowMinimize>
                        <AllowConnect>true</AllowConnect>
                        <AllowEdit>true</AllowEdit>
                        <AllowHide>true</AllowHide>
                        <IsVisible>true</IsVisible>
                        <DetailLink />
                        <HelpLink />
                        <HelpMode>Modeless</HelpMode>
                        <Dir>Default</Dir>
                        <PartImageSmall />
                        <MissingAssembly>Cannot import this Web Part.</MissingAssembly>
                        <PartImageLarge>/_layouts/15/images/mscontl.gif</PartImageLarge>
                        <IsIncludedFilter />
                        <Assembly>Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>
                        <TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>
                        <ContentLink xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                        <Content xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"">
                        <![CDATA[
                             {0}      
                        ]]>
                        </Content>
                        <PartStorage xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                    </WebPart>";

        /// <summary>
        /// XML definition of content editor web part for metadata
        /// </summary>
        private static string contentEditorWebpartMetadata = @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <WebPart xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/WebPart/v2"">
                        <Title></Title>
                        <FrameType>None</FrameType>
                        <Description>Allows authors to enter rich text content.</Description>
                         <IsIncluded>true</IsIncluded>
                         <ZoneID>wpz</ZoneID>
                         <PartOrder>0</PartOrder>
                         <FrameState>Normal</FrameState>
                         <Height></Height>
                         <Width>825px</Width>
                         <AllowRemove>true</AllowRemove>
                         <AllowZoneChange>true</AllowZoneChange>
                         <AllowMinimize>true</AllowMinimize>
                         <AllowConnect>true</AllowConnect>
                         <AllowEdit>true</AllowEdit>
                         <AllowHide>true</AllowHide>
                         <IsVisible>true</IsVisible>
                         <DetailLink />
                         <HelpLink />
                         <HelpMode>Modeless</HelpMode>
                         <Dir>Default</Dir>
                         <PartImageSmall />
                         <MissingAssembly>Cannot import this Web Part.</MissingAssembly>
                         <PartImageLarge>/_layouts/15/images/mscontl.gif</PartImageLarge>
                         <IsIncludedFilter />
                         <Assembly>Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>
                         <TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>
                         <ContentLink xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                         <Content xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor""> 
                            <![CDATA[ {0} ]]>                       
                        </Content>
                        <PartStorage xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                    </WebPart>";

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
        /// The no mail subject
        /// </summary>
        private static string noMailSubject = ConstantStrings.GetConfigurationFromResourceFile("Constants", "No_Mail_Subject", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The provision DMS matter app URL
        /// </summary>
        private static string provisionMatterAppURL = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Provision_Matter_App_URL", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The matter configurations list name
        /// </summary>
        private static string matterConfigurationsList = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Configurations_List", Enumerators.ResourceFileLocation.App_GlobalResources);

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
        /// The missing parameters message
        /// </summary>
        private static string missingParametersMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Parameter_Missing_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The invalid parameters message
        /// </summary>
        private static string invalidParametersMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Parameter_Invalid_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

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
        /// The mail cart mail subject
        /// </summary>
        private static string mailCartMailSubject = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Mail_Cart_Mail_Subject", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The mail cart mail body
        /// </summary>
        private static string mailCartMailBody = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Mail_Cart_Mail_Body", Enumerators.ResourceFileLocation.App_GlobalResources);

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
        /// Link to templates document library on content type hub
        /// </summary>
        private static string templateDocLibraryLink = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Template_Document_Library_Link", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Link to templates document library on content type hub
        /// </summary>
        private static string fileNotAvailableMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "File_Not_Available_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Success message on deletion of matter
        /// </summary>
        private static string matterDeletedSuccessfully = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Deletion_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

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
        /// Holds the message for no data returned from people picker control based on the search term.
        /// </summary>
        private static string peoplePickerNoResults = ConstantStrings.GetConfigurationFromResourceFile("Constants", "People_Picker_No_Results_Found", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Holds the relative location for OneNote file that is uploaded upon matter creation
        /// </summary>
        private static string oneNoteRelativeFilePath = "~/Open Notebook.onetoc2";

        /// <summary>
        /// Holds the managed property for retrieving version of the document
        /// </summary>
        private static string managedPropertyDocumentVersion = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_Version", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Holds the managed property for retrieving checked out user of the document
        /// </summary>
        private static string managedPropertyDocumentCheckOutUser = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_CheckOutUser", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Property value column of Matter Configurations list
        /// </summary>
        private static string matterConfigurationColumn = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Configuration_List_Column_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Content check flag default value
        /// </summary>
        private static bool isContentCheck = Convert.ToBoolean(ConstantStrings.GetConfigurationFromResourceFile("Constants", "Is_Content_Check", Enumerators.ResourceFileLocation.App_GlobalResources), CultureInfo.InvariantCulture);

        /// <summary>
        /// The tenant url
        /// </summary>
        private static string siteURL = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Site_Url", Enumerators.ResourceFileLocation.App_GlobalResources);

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
        /// Gets the message no inputs.
        /// </summary>
        /// <value>The message no inputs.</value>
        internal static string MessageNoInputs
        {
            get
            {
                return messageNoInputs;
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
        /// Gets the name of the send mail list.
        /// </summary>
        /// <value>The name of the send mail list.</value>
        internal static string SendMailListName
        {
            get
            {
                return sendMailListName;
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
        /// Gets the managed property responsible attorney.
        /// </summary>
        /// <value>The managed property responsible attorney.</value>
        internal static string ManagedPropertyResponsibleAttorney
        {
            get
            {
                return managedPropertyResponsibleAttorney;
            }
        }

        /// <summary>
        /// Gets the managed property team members.
        /// </summary>
        /// <value>The managed property team members.</value>
        internal static string ManagedPropertyTeamMembers
        {
            get
            {
                return managedPropertyTeamMembers;
            }
        }

        /// <summary>
        /// Gets the name of the managed property site.
        /// </summary>
        /// <value>The name of the managed property site.</value>
        internal static string ManagedPropertySiteName
        {
            get
            {
                return managedPropertySiteName;
            }
        }

        /// <summary>
        /// Gets the managed property custom title.
        /// </summary>
        /// <value>The managed property custom title.</value>
        internal static string ManagedPropertyCustomTitle
        {
            get
            {
                return managedPropertyCustomTitle;
            }
        }

        /// <summary>
        /// Gets the managed property is matter.
        /// </summary>
        /// <value>The managed property is matter.</value>
        internal static string ManagedPropertyIsMatter
        {
            get
            {
                return managedPropertyIsMatter;
            }
        }

        /// <summary>
        /// Gets the managed property title.
        /// </summary>
        /// <value>The managed property title.</value>
        internal static string ManagedPropertyTitle
        {
            get
            {
                return managedPropertyTitle;
            }
        }

        /// <summary>
        /// Gets the name of the managed property.
        /// </summary>
        /// <value>The name of the managed property.</value>
        internal static string ManagedPropertyName
        {
            get
            {
                return managedPropertyName;
            }
        }

        /// <summary>
        /// Gets the managed property description.
        /// </summary>
        /// <value>The managed property description.</value>
        internal static string ManagedPropertyDescription
        {
            get
            {
                return managedPropertyDescription;
            }
        }

        /// <summary>
        /// Gets the managed property last modified time.
        /// </summary>
        /// <value>The managed property last modified time.</value>
        internal static string ManagedPropertyLastModifiedTime
        {
            get
            {
                return managedPropertyLastModifiedTime;
            }
        }

        /// <summary>
        /// Gets the managed property practice group.
        /// </summary>
        /// <value>The managed property practice group.</value>
        internal static string ManagedPropertyPracticeGroup
        {
            get
            {
                return managedPropertyPracticeGroup;
            }
        }

        /// <summary>
        /// Gets the managed property area of law.
        /// </summary>
        /// <value>The managed property area of law.</value>
        internal static string ManagedPropertyAreaOfLaw
        {
            get
            {
                return managedPropertyAreaOfLaw;
            }
        }

        /// <summary>
        /// Gets the managed property sub area of law.
        /// </summary>
        /// <value>The managed property sub area of law.</value>
        internal static string ManagedPropertySubAreaOfLaw
        {
            get
            {
                return managedPropertySubAreaOfLaw;
            }
        }

        /// <summary>
        /// Gets the managed property matter identifier.
        /// </summary>
        /// <value>The managed property matter identifier.</value>
        internal static string ManagedPropertyMatterId
        {
            get
            {
                return managedPropertyMatterId;
            }
        }

        /// <summary>
        /// Gets the managed property path.
        /// </summary>
        /// <value>The managed property path.</value>
        internal static string ManagedPropertyPath
        {
            get
            {
                return managedPropertyPath;
            }
        }

        /// <summary>
        /// Gets the name of the managed property matter.
        /// </summary>
        /// <value>The name of the managed property matter.</value>
        internal static string ManagedPropertyMatterName
        {
            get
            {
                return managedPropertyMatterName;
            }
        }

        /// <summary>
        /// Gets the managed property open date.
        /// </summary>
        /// <value>The managed property open date.</value>
        internal static string ManagedPropertyOpenDate
        {
            get
            {
                return managedPropertyOpenDate;
            }
        }

        /// <summary>
        /// Gets the name of the managed property client.
        /// </summary>
        /// <value>The name of the managed property client.</value>
        internal static string ManagedPropertyClientName
        {
            get
            {
                return managedPropertyClientName;
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
        /// Gets the managed property author.
        /// </summary>
        /// <value>The managed property author.</value>
        internal static string ManagedPropertyAuthor
        {
            get
            {
                return managedPropertyAuthor;
            }
        }

        /// <summary>
        /// Gets the managed property file name.
        /// </summary>
        /// <value>The managed property author.</value>
        internal static string ManagedPropertyFileName
        {
            get
            {
                return managedPropertyFileName;
            }
        }

        /// <summary>
        /// Gets the managed property is document.
        /// </summary>
        /// <value>The managed property is document.</value>
        internal static string ManagedPropertyIsDocument
        {
            get
            {
                return managedPropertyIsDocument;
            }
        }

        /// <summary>
        /// Gets the managed property server relative URL.
        /// </summary>
        /// <value>The managed property server relative URL.</value>
        internal static string ManagedPropertyServerRelativeUrl
        {
            get
            {
                return managedPropertyServerRelativeUrl;
            }
        }

        /// <summary>
        /// Gets the managed property file extension.
        /// </summary>
        /// <value>The managed property file extension.</value>
        internal static string ManagedPropertyFileExtension
        {
            get
            {
                return managedPropertyFileExtension;
            }
        }

        /// <summary>
        /// Gets the managed property created.
        /// </summary>
        /// <value>The managed property created.</value>
        internal static string ManagedPropertyCreated
        {
            get
            {
                return managedPropertyCreated;
            }
        }

        /// <summary>
        /// Gets the managed property UI version string OWS text.
        /// </summary>
        /// <value>The managed property UI version string OWS text.</value>
        internal static string ManagedPropertyUIVersionStringOWSTEXT
        {
            get
            {
                return managedPropertyUIVersionStringOWSTEXT;
            }
        }

        /// <summary>
        /// Gets the managed property site title.
        /// </summary>
        /// <value>The managed property site title.</value>
        internal static string ManagedPropertySiteTitle
        {
            get
            {
                return managedPropertySiteTitle;
            }
        }

        /// <summary>
        /// Gets the managed property document client identifier.
        /// </summary>
        /// <value>The managed property document client identifier.</value>
        internal static string ManagedPropertyDocumentClientId
        {
            get
            {
                return managedPropertyDocumentClientId;
            }
        }

        /// <summary>
        /// Gets the name of the managed property document client.
        /// </summary>
        /// <value>The name of the managed property document client.</value>
        internal static string ManagedPropertyDocumentClientName
        {
            get
            {
                return managedPropertyDocumentClientName;
            }
        }

        /// <summary>
        /// Gets the managed property document matter identifier.
        /// </summary>
        /// <value>The managed property document matter identifier.</value>
        internal static string ManagedPropertyDocumentMatterId
        {
            get
            {
                return managedPropertyDocumentMatterId;
            }
        }

        /// <summary>
        /// Gets the name of the managed property document matter.
        /// </summary>
        /// <value>The name of the managed property document matter.</value>
        internal static string ManagedPropertyDocumentMatterName
        {
            get
            {
                return managedPropertyDocumentMatterName;
            }
        }

        /// <summary>
        /// Gets the managed property document identifier.
        /// </summary>
        /// <value>The managed property document identifier.</value>
        internal static string ManagedPropertyDocumentId
        {
            get
            {
                return managedPropertyDocumentId;
            }
        }

        /// <summary>
        /// Gets the managed property check out by user.
        /// </summary>
        /// <value>The managed property check out by user.</value>
        internal static string ManagedPropertyCheckOutByUser
        {
            get
            {
                return managedPropertyCheckOutByUser;
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
        /// Gets the name of the user saved search list.
        /// </summary>
        /// <value>The name of the user saved search list.</value>
        internal static string UserSavedSearchListName
        {
            get
            {
                return userSavedSearchListName;
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
        /// Gets the column name saved matters details.
        /// </summary>
        /// <value>The column name saved matters details.</value>
        internal static string ColumnNameSavedMattersDetails
        {
            get
            {
                return columnNameSavedMattersDetails;
            }
        }

        /// <summary>
        /// Gets the column name saved documents details.
        /// </summary>
        /// <value>The column name saved documents details.</value>
        internal static string ColumnNameSavedDocumentsDetails
        {
            get
            {
                return columnNameSavedDocumentsDetails;
            }
        }

        /// <summary>
        /// Gets the column name user alias.
        /// </summary>
        /// <value>The column name user alias.</value>
        internal static string ColumnNameUserAlias
        {
            get
            {
                return columnNameUserAlias;
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
        /// Gets the managed property blocked upload users.
        /// </summary>
        /// <value>The managed property blocked upload users.</value>
        internal static string ManagedPropertyBlockedUploadUsers
        {
            get
            {
                return managedPropertyBlockedUploadUsers;
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
        /// Gets the mail SOAP request.
        /// </summary>
        /// <value>The mail SOAP request.</value>
        internal static string MailSoapReuqest
        {
            get
            {
                return mailSoapReuqest;
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
        /// Gets the no mail subject.
        /// </summary>
        /// <value>The no mail subject.</value>
        internal static string NoMailSubject
        {
            get
            {
                return noMailSubject;
            }
        }

        /// <summary>
        /// Gets the provision matter application URL.
        /// </summary>
        /// <value>The provision matter application URL.</value>
        internal static string ProvisionMatterAppURL
        {
            get
            {
                return provisionMatterAppURL;
            }
        }

        /// <summary>
        /// Gets the matter configurations list name
        /// </summary>
        /// <value>The matter configurations list name</value>
        internal static string MatterConfigurationsList
        {
            get
            {
                return matterConfigurationsList;
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
        /// Gets the missing parameters message.
        /// </summary>
        /// <value>The missing parameters message.</value>
        internal static string MissingParametersMessage
        {
            get
            {
                return missingParametersMessage;
            }
        }

        /// <summary>
        /// Gets the invalid parameters message.
        /// </summary>
        /// <value>The invalid parameters message.</value>
        internal static string InvalidParametersMessage
        {
            get
            {
                return invalidParametersMessage;
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
        /// Gets the mail cart mail subject.
        /// </summary>
        /// <value>The mail cart mail subject.</value>
        internal static string MailCartMailSubject
        {
            get
            {
                return mailCartMailSubject;
            }
        }

        /// <summary>
        /// Gets the mail cart mail body.
        /// </summary>
        /// <value>The mail cart mail body.</value>
        internal static string MailCartMailBody
        {
            get
            {
                return mailCartMailBody;
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
        /// Gets the share list column mail body.
        /// </summary>
        /// <value>The share list column mail body.</value>
        internal static string ShareListColumnMailBody
        {
            get
            {
                return shareListColumnMailBody;
            }
        }

        /// <summary>
        /// Gets the share list column mail subject
        /// </summary>
        /// <value>The share list column mail subject.</value>
        internal static string ShareListColumnMailSubject
        {
            get
            {
                return shareListColumnMailSubject;
            }
        }

        /// <summary>
        /// Gets the XML chunk for top menu web part
        /// </summary>
        internal static string TopMenuWebPart
        {
            get
            {
                return topMenuWebPart;
            }
        }

        /// <summary>
        /// Gets the XML chunk for RSS feed web part
        /// </summary>
        internal static string RssFeedWebpart
        {
            get
            {
                return rssFeedWebpart;
            }
        }

        /// <summary>
        /// Gets the XML chunk for Header Web Part
        /// </summary>
        internal static string HeaderWebPart
        {
            get
            {
                return headerWebPart;
            }
        }

        /// <summary>
        /// Gets the XML chunk for Left bar web part
        /// </summary>
        internal static string LeftBarWebPart
        {
            get
            {
                return leftBarWebPart;
            }
        }

        /// <summary>
        /// Gets the XML chunk for Footer Web Part
        /// </summary>
        internal static string FooterWebPart
        {
            get
            {
                return footerWebPart;
            }
        }

        /// <summary>
        /// Gets the XML chunk for Calendar Web part
        /// </summary>
        internal static string CalendarWebpart
        {
            get
            {
                return calendarWebpart;
            }
        }

        /// <summary>
        /// Gets the XML chunk for List view Web part
        /// </summary>
        internal static string ListviewWebpart
        {
            get
            {
                return listviewWebpart;
            }
        }

        /// <summary>
        /// Gets the XML chunk for Content Editor Web part for Metadata
        /// </summary>
        internal static string ContentEditorWebpartMetadata
        {
            get
            {
                return contentEditorWebpartMetadata;
            }
        }

        /// <summary>
        /// Gets the html chunk for Matter Landing page
        /// </summary>
        internal static string HtmlChunk
        {
            get
            {
                return htmlChunk;
            }
        }

        /// <summary>
        /// Gets the XML chunk for Content editor web part for users
        /// </summary>
        internal static string ContentEditorWebpartUsers
        {
            get
            {
                return contentEditorWebpartUsers;
            }
        }

        /// <summary>
        /// Gets the html data for Matter Landing page
        /// </summary>
        internal static string HtmlData
        {
            get
            {
                return htmlData;
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
        /// Gets location of the pin image
        /// </summary>
        internal static string PinImageLocation
        {
            get
            {
                return pinImageLocation;
            }
        }

        /// <summary>
        /// Gets location of the Unpin image
        /// </summary>
        internal static string UnPinImageLocation
        {
            get
            {
                return unpinImageLocation;
            }
        }

        /// <summary>
        /// Gets location of the One Note image
        /// </summary>
        internal static string OneNoteImageLocation
        {
            get
            {
                return oneNoteImageLocation;
            }
        }

        /// <summary>
        /// Gets location of the share image
        /// </summary>
        internal static string ShareImageLocation
        {
            get
            {
                return shareImageLocation;
            }
        }

        /// <summary>
        /// Gets location of the share image
        /// </summary>
        internal static string MatterLandingPageLogo
        {
            get
            {
                return matterLandingPageLogo;
            }
        }

        /// <summary>
        /// Gets location of the Microsoft Logo Image
        /// </summary>
        internal static string MicrosoftImageLocation
        {
            get
            {
                return microsoftImageLocation;
            }
        }

        /// <summary>
        /// Gets the body of the matter mail for matter information
        /// </summary>
        internal static string MatterMailBodyInformation
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Mail_Body_Matter_Information", Enumerators.ResourceFileLocation.App_GlobalResources); ;
            }
        }

        /// <summary>
        /// Gets the body of the matter mail for conflict check
        /// </summary>
        internal static string MatterMailBodyConflictCheck
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Mail_Body_Conflict_Check", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the body of the matter mail for Team Members
        /// </summary>
        internal static string MatterMailBodyTeamMembers
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Mail_Body_Team_Members", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets HTML chunk of the Default content type for Matter creation mail
        /// </summary>
        internal static string MatterMailDefaultContentTypeHtmlChunk
        {
            get
            {
                return matterMailDefaultContentTypeHtmlChunk;
            }
        }

        /// <summary>
        /// Gets the subject of the matter creation mail
        /// </summary>
        internal static string MatterMailSubject
        {
            get
            {
                return matterMailSubject;
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
        /// Gets the Tenant level web dashboard location
        /// </summary>
        internal static string TenantWebDashboardLink
        {
            get
            {
                return tenantWebDashboardLink;
            }
        }

        /// <summary>
        /// Gets the XML definition of Template Web Part
        /// </summary>
        internal static string TemplateWebPart
        {
            get
            {
                return templateWebPart;
            }
        }

        /// <summary>
        /// Gets the link of the Template Document library
        /// </summary>
        internal static string TemplateDocLibraryLink
        {
            get
            {
                return templateDocLibraryLink;
            }
        }

        /// <summary>
        /// Gets the manage property for Client ID
        /// </summary>
        internal static string ManagedPropertyClientID
        {
            get
            {
                return managedPropertyClientID;
            }
        }

        /// <summary>
        /// Gets File not available Message
        /// </summary>
        /// <value>Message for file not available</value>
        internal static string FileNotAvailableMessage
        {
            get
            {
                return fileNotAvailableMessage;
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
        /// Gets the incorrect Request Object message
        /// </summary>
        /// <value>The incorrect Request Object message</value>
        internal static string IncorrectInputRequestObjectMessage
        {
            get
            {
                return incorrectInputRequestObjectMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect Request Object code
        /// </summary>
        /// <value>The incorrect Request Object code</value>
        internal static string IncorrectInputRequestObjectCode
        {
            get
            {
                return incorrectInputRequestObjectCode;
            }
        }

        /// <summary>
        /// Gets the incorrect Client Url message
        /// </summary>
        /// <value>The incorrect Client Url message</value>
        internal static string IncorrectInputClientUrlMessage
        {
            get
            {
                return incorrectInputClientUrlMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect Client Url code
        /// </summary>
        /// <value>The incorrect Client Url code</value>
        internal static string IncorrectInputClientUrlCode
        {
            get
            {
                return incorrectInputClientUrlCode;
            }
        }

        /// <summary>
        /// Gets the incorrect Client id message
        /// </summary>
        /// <value>The incorrect Client id message</value>
        internal static string IncorrectInputClientIdMessage
        {
            get
            {
                return incorrectInputClientIdMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect Client id code
        /// </summary>
        /// <value>The incorrect Client id code</value>
        internal static string IncorrectInputClientIdCode
        {
            get
            {
                return incorrectInputClientIdCode;
            }
        }

        /// <summary>
        /// Gets the incorrect Client name message
        /// </summary>
        /// <value>The incorrect Client name message/value>
        internal static string IncorrectInputClientNameMessage
        {
            get
            {
                return incorrectInputClientNameMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect Client name code
        /// </summary>
        /// <value>The incorrect Client name code/value>
        internal static string IncorrectInputClientNameCode
        {
            get
            {
                return incorrectInputClientNameCode;
            }
        }

        /// <summary>
        /// Gets the incorrect Practice Group message
        /// </summary>
        /// <value>The incorrect Practice Group message</value>
        internal static string IncorrectInputPracticeGroupMessage
        {
            get
            {
                return incorrectInputPracticeGroupMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect Practice Group code
        /// </summary>
        /// <value>The incorrect Practice Group code</value>
        internal static string IncorrectInputPracticeGroupCode
        {
            get
            {
                return incorrectInputPracticeGroupCode;
            }
        }

        /// <summary>
        /// Gets the incorrect Area Of Law message
        /// </summary>
        /// <value>The incorrect Area Of Law message</value>
        internal static string IncorrectInputAreaOfLawMessage
        {
            get
            {
                return incorrectInputAreaOfLawMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect Area Of Law code
        /// </summary>
        /// <value>The incorrect Area Of Law code</value>
        internal static string IncorrectInputAreaOfLawCode
        {
            get
            {
                return incorrectInputAreaOfLawCode;
            }
        }

        /// <summary>
        /// Gets the incorrect Sub Area Of Law message
        /// </summary>
        /// <value>The incorrect Sub Area Of Law message</value>
        internal static string IncorrectInputSubareaOfLawMessage
        {
            get
            {
                return incorrectInputSubareaOfLawMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect Sub Area Of Law code
        /// </summary>
        /// <value>The incorrect Sub Area Of Law code</value>
        internal static string IncorrectInputSubareaOfLawCode
        {
            get
            {
                return incorrectInputSubareaOfLawCode;
            }
        }

        /// <summary>
        /// Gets the incorrect Responsible Attorney message
        /// </summary>
        /// <value>The incorrect Responsible Attorney message</value>
        internal static string IncorrectInputResponsibleAttorneyMessage
        {
            get
            {
                return incorrectInputResponsibleAttorneyMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect Responsible Attorney code
        /// </summary>
        /// <value>The incorrect Responsible Attorney code</value>
        internal static string IncorrectInputResponsibleAttorneyCode
        {
            get
            {
                return incorrectInputResponsibleAttorneyCode;
            }
        }

        /// <summary>
        /// Gets the incorrect Matter name message
        /// </summary>
        /// <value>The incorrect Matter name message</value>
        internal static string IncorrectInputMatterNameMessage
        {
            get
            {
                return incorrectInputMatterNameMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect Matter name code
        /// </summary>
        /// <value>The incorrect Matter name code</value>
        internal static string IncorrectInputMatterNameCode
        {
            get
            {
                return incorrectInputMatterNameCode;
            }
        }

        /// <summary>
        /// Gets the incorrect Matter id message
        /// </summary>
        /// <value>The incorrect Matter id message</value>
        internal static string IncorrectInputMatterIdMessage
        {
            get
            {
                return incorrectInputMatterIdMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect Matter id code
        /// </summary>
        /// <value>The incorrect Matter id code</value>
        internal static string IncorrectInputMatterIdCode
        {
            get
            {
                return incorrectInputMatterIdCode;
            }
        }

        /// <summary>
        /// Gets the incorrect user name message
        /// </summary>
        /// <value>The incorrect user name message</value>
        internal static string IncorrectInputUserNamesMessage
        {
            get
            {
                return incorrectInputUserNamesMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect user name code
        /// </summary>
        /// <value>The incorrect user name code</value>
        internal static string IncorrectInputUserNamesCode
        {
            get
            {
                return incorrectInputUserNamesCode;
            }
        }

        /// <summary>
        /// Gets the incorrect user permission message
        /// </summary>
        /// <value>The incorrect user permission message</value>
        internal static string IncorrectInputUserPermissionsMessage
        {
            get
            {
                return incorrectInputUserPermissionsMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect user permission code
        /// </summary>
        /// <value>The incorrect user permission code</value>
        internal static string IncorrectInputUserPermissionsCode
        {
            get
            {
                return incorrectInputUserPermissionsCode;
            }
        }

        /// <summary>
        /// Gets the incorrect Content Type message
        /// </summary>
        /// <value>The incorrect Content Type message</value>
        internal static string IncorrectInputContentTypeMessage
        {
            get
            {
                return incorrectInputContentTypeMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect Content Type code
        /// </summary>
        /// <value>The incorrect Content Type code</value>
        internal static string IncorrectInputContentTypeCode
        {
            get
            {
                return incorrectInputContentTypeCode;
            }
        }

        /// <summary>
        /// Gets the incorrect Matter description message
        /// </summary>
        /// <value>The incorrect Matter description message</value>
        internal static string IncorrectInputMatterDescriptionMessage
        {
            get
            {
                return incorrectInputMatterDescriptionMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect Matter description code
        /// </summary>
        /// <value>The incorrect Matter description code</value>
        internal static string IncorrectInputMatterDescriptionCode
        {
            get
            {
                return incorrectInputMatterDescriptionCode;
            }
        }

        /// <summary>
        /// Gets the incorrect conflict date message
        /// </summary>
        /// <value>The incorrect conflict date message</value>
        internal static string IncorrectInputConflictDateMessage
        {
            get
            {
                return incorrectInputConflictDateMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect conflict date code
        /// </summary>
        /// <value>The incorrect conflict date code</value>
        internal static string IncorrectInputConflictDateCode
        {
            get
            {
                return incorrectInputConflictDateCode;
            }
        }

        /// <summary>
        /// Gets the incorrect conflict identified message
        /// </summary>
        /// <value>The incorrect conflict identified message</value>
        internal static string IncorrectInputConflictIdentifiedMessage
        {
            get
            {
                return incorrectInputConflictIdentifiedMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect conflict identified code
        /// </summary>
        /// <value>The incorrect conflict identified code</value>
        internal static string IncorrectInputConflictIdentifiedCode
        {
            get
            {
                return incorrectInputConflictIdentifiedCode;
            }
        }

        /// <summary>
        /// Gets the incorrect user roles message
        /// </summary>
        /// <value>The incorrect user roles message</value>
        internal static string IncorrectInputUserRolesMessage
        {
            get
            {
                return incorrectInputUserRolesMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect user roles code
        /// </summary>
        /// <value>The incorrect user roles code</value>
        internal static string IncorrectInputUserRolesCode
        {
            get
            {
                return incorrectInputUserRolesCode;
            }
        }

        /// <summary>
        /// Gets the incorrect conflict check by message
        /// </summary>
        /// <value>The incorrect conflict check by message</value>
        internal static string IncorrectInputConflictCheckByMessage
        {
            get
            {
                return incorrectInputConflictCheckByMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect conflict check by code
        /// </summary>
        /// <value>The incorrect conflict check by code</value>
        internal static string IncorrectInputConflictCheckByCode
        {
            get
            {
                return incorrectInputConflictCheckByCode;
            }
        }

        /// <summary>
        /// Gets the incorrect block users message
        /// </summary>
        /// <value>The incorrect block users message</value>
        internal static string IncorrectInputBlockUserNamesMessage
        {
            get
            {
                return incorrectInputBlockUserNamesMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect block users code
        /// </summary>
        /// <value>The incorrect block users code</value>
        internal static string IncorrectInputBlockUserNamesCode
        {
            get
            {
                return incorrectInputBlockUserNamesCode;
            }
        }

        /// <summary>
        /// Gets the key for read only user flag
        /// </summary>
        /// <value>The key value of read only user flag</value>
        internal static string IsReadOnlyUser
        {
            get
            {
                return isReadOnlyUser;
            }
        }

        /// <summary>
        /// Gets the incorrect save search name message
        /// </summary>
        /// <value>The incorrect save search name message</value>
        internal static string IncorrectInputSaveSearchNameMessage
        {
            get
            {
                return incorrectInputSaveSearchNameMessage;
            }
        }


        /// <summary>
        /// Gets the incorrect save search name code
        /// </summary>
        /// <value>The incorrect save search name code</value>
        internal static string IncorrectInputSaveSearchNameCode
        {
            get
            {
                return incorrectInputSaveSearchNameCode;
            }
        }

        /// <summary>
        /// Gets the incorrect existing search name message
        /// </summary>
        /// <value>The incorrect  existing search name message</value>
        internal static string IncorrectInputExistingSearchNameMessage
        {
            get
            {
                return incorrectInputExistingSearchNameMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect  existing search name code
        /// </summary>
        /// <value>The incorrect  existing search name code</value>
        internal static string IncorrectInputExistingSearchNameCode
        {
            get
            {
                return incorrectInputExistingSearchNameCode;
            }
        }

        /// <summary>
        /// Gets the incorrect  current search name message
        /// </summary>
        /// <value>The incorrect  current search name message</value>
        internal static string IncorrectInputCurrentSearchNameMessage
        {
            get
            {
                return incorrectInputCurrentSearchNameMessage;
            }
        }

        /// <summary>
        /// Gets the incorrect  current search name code
        /// </summary>
        /// <value>The incorrect  current search name code</value>
        internal static string IncorrectInputCurrentSearchNameCode
        {
            get
            {
                return incorrectInputCurrentSearchNameCode;
            }
        }

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
        /// Gets the success message on deletion of matter
        /// </summary>
        /// <value>The success message on deletion of matter</value>
        internal static string MatterDeletedSuccessfully
        {
            get
            {
                return matterDeletedSuccessfully;
            }
        }

        /// <summary>
        /// Gets the error code for issue when Content Types selected by the users are not present in the Content Type hub or site collection where the matter library is created
        /// </summary>
        /// <value>Error code when for issue when Content Types selected by the users are not present in the Content Type hub or site collection where the matter library is created</value>
        internal static string ErrorCodeContentTypes
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorCodeContentType", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the error message for issue when Content Types selected by the users are not present in the Content Type hub or site collection where the matter library is created
        /// </summary>
        /// <value>Error message when for issue when Content Types selected by the users are not present in the Content Type hub or site collection where the matter library is created</value>
        internal static string ErrorMessageContentTypes
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorMessageContentType", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the error code when there is an issue while creation of matter landing page
        /// </summary>
        /// <value>Error code when there is an issue while creation of matter landing page</value>
        internal static string ErrorCodeMatterLandingPage
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorCodeMatterLandingPage", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the error message when there is an issue while creation of matter landing page
        /// </summary>
        /// <value>Error message when there is an issue while creation of matter landing page</value>
        internal static string ErrorMessageMatterLandingPage
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorMessageMatterLandingPage", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the error code when there is an issue while assigning permissions to calendar list
        /// </summary>
        /// <value>Error code when there is an issue while assigning permissions to calendar list</value>
        internal static string ErrorCodeCalendarCreation
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorCodeCalendarCreation", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the error message when there is an issue while assigning permissions to calendar list
        /// </summary>
        /// <value>Error message when there is an issue while assigning permissions to calendar list</value>
        internal static string ErrorMessageCalendarCreation
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorMessageCalendarCreation", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the error code when there is an issue while creating calendar list
        /// </summary>
        /// <value>Error code when there is an issue while creating calendar list</value>
        internal static string ErrorCodeAddCalendarList
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorCodeAddCalendarList", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets the error message when there is an issue while creating calendar list
        /// </summary>
        /// <value>Error message when there is an issue while creating calendar list</value>
        internal static string ErrorMessageAddCalendarList
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorMessageAddCalendarList", Enumerators.ResourceFileLocation.App_GlobalResources);
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
        /// Holds the message for no data returned from people picker control based on the search term.
        /// </summary>
        internal static string PeoplePickerNoResults
        {
            get
            {
                return peoplePickerNoResults;
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
        /// Holds the error message for matter landing existence
        /// </summary>
        internal static string ErrorMessageMatterLandingPageExists
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorMessageMatterLandingPageExists", Enumerators.ResourceFileLocation.App_GlobalResources);
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
        /// Represents the SPWebUrl managed property associated with the document
        /// </summary>
        internal static string ManagedPropertySPWebUrl
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_SPWeb_Url", Enumerators.ResourceFileLocation.App_GlobalResources); ;
            }
        }

        /// <summary>
        /// Gets the relative server path for the OneNote file to upload.
        /// </summary>
        /// <value>The name of the practice group term set.</value>
        internal static string OneNoteRelativeFilePath
        {
            get
            {
                return oneNoteRelativeFilePath;
            }
        }

        /// <summary>
        /// Gets the matter configuration column name of matter configurations list
        /// </summary>
        /// <value>The name of the matter configuration column</value>
        internal static string MatterConfigurationColumn
        {
            get
            {
                return matterConfigurationColumn;
            }
        }

        /// <summary>
        /// Represents the error message when user tries to remove self permission
        /// </summary>
        internal static string EditMatterSelfRemovalError
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Edit_Matter_Self_Removal_Error", Enumerators.ResourceFileLocation.App_GlobalResources); ;
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
        /// Represents the managed property for retrieving version of the document
        /// </summary>
        internal static string ManagedPropertyDocumentVersion
        {
            get
            {
                return managedPropertyDocumentVersion;
            }
        }

        /// <summary>
        /// Represents the managed property for retrieving checked out user of the document
        /// </summary>
        internal static string ManagedPropertyDocumentCheckOutUser
        {
            get
            {
                return managedPropertyDocumentCheckOutUser;
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
        /// Gets the content check flag
        /// </summary>
        /// <value>Content check flag</value>
        internal static bool IsContentCheck
        {
            get
            {
                return isContentCheck;
            }
        }

        /// <summary>
        /// Get the tenant site URL
        /// </summary>
        internal static string SiteURL
        {
            get
            {
                return siteURL;
            }
        }
    }
}
