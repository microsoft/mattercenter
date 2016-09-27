// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="SearchSettings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// This file is used for getting the search configuration information from the appSettings.json file
    /// These properties will subsequently used when getting the matter information from SPO
    /// </summary>
    public class SearchSettings
    {
        public string SearchResultSourceID { get; set; }
        public string ManagedPropertyExtension { get; set; }

        #region Matter Search Related Properties
        public string Schema { get; set; }
        public string ManagedPropertyTitle { get; set; }
        public string ManagedPropertyName { get; set; }
        public string ManagedPropertyDescription { get; set; }
        public string ManagedPropertySiteName { get; set; }
        public string ManagedPropertyLastModifiedTime { get; set; }
        public string ManagedPropertyPracticeGroup { get; set; }
        public string ManagedPropertyAreaOfLaw { get; set; }
        public string ManagedPropertySubAreaOfLaw { get; set; }
        public string ManagedPropertyMatterId { get; set; }
        public string ManagedPropertyCustomTitle { get; set; }
        public string ManagedPropertyPath { get; set; }
        public string ManagedPropertyMatterName { get; set; }
        public string ManagedPropertyOpenDate { get; set; }
        public string ManagedPropertyClientName { get; set; }
        public string ManagedPropertyBlockedUploadUsers { get; set; }
        public string ManagedPropertyResponsibleAttorney { get; set; }
        public string ManagedPropertyClientID { get; set; }
        public string ManagedPropertyMatterGuidLogging { get; set; }
        public string ManagedPropertyTeamMembers { get; set; }
        public string ManagedPropertyFileName { get; set; }
        public string ManagedPropertyDocumentCheckOutUser { get; set; }
        public string ManagedPropertyCreated { get; set; }
        public string ManagedPropertyMatterGuid { get; set; }
        public string ManagedPropertyIsMatter { get; set; }
        public string UserPermissions { get; set; }
        public string MatterConfigurationTitleValue { get; set; }
        public string ColumnNameModifiedDate { get; set; }
        public string MatterConfigurationColumn { get; set; }
        public string ManagedPropertySubAreaOfLaw1 { get; set; }
        public string ManagedPropertySubAreaOfLaw2 { get; set; }
        #endregion

        #region Document Search Related Properties
        public string ManagedPropertyDocumentLastModifiedTime { get; set; }
        public string FindDocumentInvalidExtensions { get; set; }
        public string ManagedPropertyAuthor { get; set; }
        public string ManagedPropertyDocumentClientId { get; set; }
        public string ManagedPropertyDocumentMatterId { get; set; }
        public string ManagedPropertyDocumentClientName { get; set; }
        public string ManagedPropertyUIVersionStringOWSTEXT { get; set; }
        public string ManagedPropertyServerRelativeUrl { get; set; }

        public string ManagedPropertyFileExtension { get; set; }
        public string ManagedPropertySiteTitle { get; set; }
        public string ManagedPropertyDocumentMatterName { get; set; }
        public string ManagedPropertyDocumentId { get; set; }
        public string ManagedPropertyCheckOutByUser { get; set; }
        public string ManagedPropertySPWebUrl { get; set; }
        public string ManagedPropertyDocumentVersion { get; set; }
        public string ManagedPropertyIsDocument { get; set; }
        public string DocId { get; set; }
        #endregion

        #region Pin
        public string PinnedListColumnUserAlias { get; set; }
        public string PinnedListColumnMatterDetails { get; set; }
        public string PinnedListColumnDocumentDetails { get; set; }

        #endregion

        #region Column Names
        public string ColumnNameFileLeafRef { get; set; }
        public string ColumnNameFileRef { get; set; }
        public string ColumnNameFileDirRef { get; set; }
        #endregion

        #region PeoplePicker
        public bool PeoplePickerAllowMultipleEntities { get; set; }
        public int PeoplePickerMaximumEntitySuggestions { get; set; }
        public int PeoplePickerMaxRecords { get; set; }

        #endregion

    }
}
