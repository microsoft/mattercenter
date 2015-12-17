// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-rijadh
// Created          : 11-28-2014
//
// ***********************************************************************
// <copyright file="SearchConstants.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains search constants for managed search properties.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    #endregion

    internal static class SearchConstants
    {

        /// <summary>
        /// The managed property responsible attorney
        /// </summary>
        internal static readonly string ManagedPropertyResponsibleAttorney = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Responsible_Attorney", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property team members
        /// </summary>
        internal static readonly string ManagedPropertyTeamMembers = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Team_Members", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property site name
        /// </summary>
        internal static readonly string ManagedPropertySiteName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Site_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property custom title
        /// </summary>
        internal static readonly string ManagedPropertyCustomTitle = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Custom_Title", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property is matter
        /// </summary>
        internal static readonly string ManagedPropertyIsMatter = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Is_Matter", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property title
        /// </summary>
        internal static string ManagedPropertyTitle = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Title", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property name
        /// </summary>
        internal static string ManagedPropertyName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property description
        /// </summary>
        internal static readonly string ManagedPropertyDescription = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Description", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property last modified time
        /// </summary>
        internal static readonly string ManagedPropertyLastModifiedTime = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Last_Modified_Time", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property last modified time for documents
        /// </summary>
        internal static readonly string ManagedPropertyDocumentLastModifiedTime = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_Last_Modified_Time", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property practice group
        /// </summary>
        internal static readonly string ManagedPropertyPracticeGroup = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Practice_Group", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property area of law
        /// </summary>
        internal static readonly string ManagedPropertyAreaOfLaw = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Area_Of_Law", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property sub area of law
        /// </summary>
        internal static readonly string ManagedPropertySubAreaOfLaw = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Sub_Area_Of_Law", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property matter identifier
        /// </summary>
        internal static readonly string ManagedPropertyMatterId = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Matter_Id", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property path
        /// </summary>
        internal static readonly string ManagedPropertyPath = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Path", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property matter name
        /// </summary>
        internal static readonly string ManagedPropertyMatterName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Matter_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property open date
        /// </summary>
        internal static readonly string ManagedPropertyOpenDate = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Open_Date", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property client name
        /// </summary>
        internal static readonly string ManagedPropertyClientName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Client_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property author
        /// </summary>
        internal static readonly string ManagedPropertyAuthor = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Author", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property author
        /// </summary>
        internal static readonly string ManagedPropertyFileName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_File_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property is document
        /// </summary>
        internal static readonly string ManagedPropertyIsDocument = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Is_Document", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property server relative URL
        /// </summary>
        internal static readonly string ManagedPropertyServerRelativeUrl = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Server_Relative_Url", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property file extension
        /// </summary>
        internal static readonly string ManagedPropertyFileExtension = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_File_Extension", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property created
        /// </summary>
        internal static readonly string ManagedPropertyCreated = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Created", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property UI version string OWSTEXT
        /// </summary>
        internal static readonly string ManagedPropertyUIVersionStringOWSTEXT = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_UI_Version_String_OWSTEXT", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property site title
        /// </summary>
        internal static readonly string ManagedPropertySiteTitle = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Site_Title", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property document client identifier
        /// </summary>
        internal static readonly string ManagedPropertyDocumentClientId = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_Client_Id", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property document client name
        /// </summary>
        internal static readonly string ManagedPropertyDocumentClientName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_Client_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property document matter identifier
        /// </summary>
        internal static readonly string ManagedPropertyDocumentMatterId = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_Matter_Id", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property document matter name
        /// </summary>
        internal static readonly string ManagedPropertyDocumentMatterName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_Matter_Name", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property document identifier
        /// </summary>
        internal static readonly string ManagedPropertyDocumentId = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_Id", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property checked out by user
        /// </summary>
        internal static readonly string ManagedPropertyCheckOutByUser = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_CheckOut_By_User", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The managed property blocked upload users
        /// </summary>
        internal static readonly string ManagedPropertyBlockedUploadUsers = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Blocked_Upload_Users", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The manage property Client ID
        /// </summary>
        internal static readonly string ManagedPropertyClientID = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Client_Id", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Represents the SPWebUrl managed property associated with the document
        /// </summary>
        internal static readonly string ManagedPropertySPWebUrl = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_SPWeb_Url", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Holds the managed property for retrieving version of the document
        /// </summary>
        internal static readonly string ManagedPropertyDocumentVersion = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_Version", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Holds the managed property for retrieving checked out user of the document
        /// </summary>
        internal static readonly string ManagedPropertyDocumentCheckOutUser = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Managed_Property_Document_CheckOutUser", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Holds the managed property for retrieving GUID of matter
        /// </summary>
        internal static readonly string ManagedPropertyMatterGuid = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_GUID", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Holds the search result source ID
        /// </summary>
        internal static readonly string SearchResultSourceID = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Result_Source_ID", Enumerators.ResourceFileLocation.App_GlobalResources);
    }
}