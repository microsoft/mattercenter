// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="MailSettings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// This file is used for getting the mail configuration information from the appSettings.json file
    /// These properties will subsequently used when uploading email and email attachments
    /// </summary>
    public class MailSettings
    {
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
    }
}
